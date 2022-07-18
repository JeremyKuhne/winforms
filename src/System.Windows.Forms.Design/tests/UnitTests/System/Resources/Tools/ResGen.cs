using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.CodeDom.Compiler;
using System.Reflection;

namespace System.Tools;

// .NET Development Platform Resource file Generator
//
// This program will read in text files or ResX files of name-value pairs,
// or assemblies (on Windows), and produce a .NET .resources file.
// Additionally ResGen can change data from any of these four formats
// to any of the other formats, except assemblies can only be read from,
// not written to, and text files only support strings.
// Currently one can only convert resources in assemblies to resx files, but support
// could easily be added to convert assemblies to other formats.
//
// The text files must have lines of the form name=value, and comments are
// allowed ('#' at the beginning of the line).
//
// The code to read resources from assemblies (on Windows) is adapted from
// ndp\clr\src\ToolBox\ResView\ResView.cs

public static partial class ResGen
{
    private const int errorCode = -1;

    private static int errors;
    private static int warnings;

    private static List<AssemblyName> assemblyList;
    private static readonly List<string> definesList = new(); // For #ifdef support in .resText (& .txt) files

    private static readonly object consoleOutputLock = new();

    private static string BadFileExtensionResourceString;

    // Comments on Error handling:
    // Our build tools all expect error messages to be in a particular
    // format.  The grammar is slightly complex with a number of optional
    // parameters.  But these methods below should conform with this spec.
    // Talk to the xmake team (msbuild?) for more details on the exact
    // spec build tools like ResGen should follow.  -- BrianGru

    // To sum up their spec:
    // Error and warning messages consist of strings like the following:
    // Origin : [subcategory] category code : text
    // like this:
    // cl : Command line warning D4024 : Unrecognized source file type
    // Origin can be the tool name, file name, or file(line,pos).  Origin
    // must not be localized if it is a tool or file name.
    // Subcategory is optional and should be localized.
    // Category is "warning" or "error" & must not be localized.
    // Code must not contain spaces & be non-localized.
    // Text is the localized error message

    // Use this for general resgen errors with no specific file info
    private static void Error(string message, int errorNumber = 0)
    {
        Console.Error.WriteLine($"ResGen : error RG{errorNumber:0000}: {message}");
        errors++;
    }

    // Use this for a general error w.r.t. a file, like a missing file.
    private static void Error(string message, string fileName, int errorNumber = 0)
    {
        Console.Error.WriteLine($"{fileName} : error RG{errorNumber:0000}: {message}");
        errors++;
    }

    private static void Error(string message, string fileName, int line, int column, int errorNumber = 0)
    {
        Console.Error.WriteLine($"{fileName}({line},{column}): error RG{errorNumber:0000}: {message}");
        errors++;
    }

    // General warnings
    private static void Warning(string message)
    {
        Console.Error.WriteLine($"ResGen : warning RG0000 : {message}");
        warnings++;
    }

    private static void Warning(string message, string fileName, int warningNumber = 0)
    {
        Console.Error.WriteLine($"{fileName} : warning RG{warningNumber:0000}: {message}");
        warnings++;
    }

    private static void Warning(string message, string fileName, int line, int column, int warningNumber  = 0)
    {
        Console.Error.WriteLine($"{fileName}({line},{column}): warning RG{warningNumber:0000}: {message}");
        warnings++;
    }

    private static Format GetFormat(string filename)
    {
        string extension = Path.GetExtension(filename);

        return extension.ToLowerInvariant() switch
        {
            ".txt" or ".restext" => Format.Text,
            ".resx" or ".resw" => Format.XML,
            ".resources.dll" or ".dll" or ".exe" => Format.Assembly,
            ".resources" => Format.Binary,
            _ => Fail(extension, filename)
        };

        static Format Fail(string extension, string filename)
        {
            Error($"Unknown file extension \"{extension}\" for file \"{filename}\"");
            Environment.Exit(errorCode);
            return Format.Text;
        }
    }

    // Remove a corrupted file, with error handling and a warning if we fail.
    private static void RemoveCorruptedFile(string filename)
    {
        Error($"Output file is possibly corrupt.  Deleting \"{filename}\"");
        try
        {
            File.Delete(filename);
        }
        catch (Exception)
        {
            Error($"Could not delete possibly corrupted output file \"{filename}\".");
        }
    }

    // For command line apps, cmd.exe cannot display right-to-left languages currently.  Change the
    // CurrentUICulture to meet their needs, falling back to our invariant resources.
    private static void SetConsoleUICulture()
    {
        Thread thread = Thread.CurrentThread;
        thread.CurrentUICulture = CultureInfo.CurrentUICulture.GetConsoleFallbackUICulture();

        if (Console.OutputEncoding.CodePage != Encoding.UTF8.CodePage &&
            Console.OutputEncoding.CodePage != thread.CurrentUICulture.TextInfo.OEMCodePage &&
            Console.OutputEncoding.CodePage != thread.CurrentUICulture.TextInfo.ANSICodePage)
        {
            thread.CurrentUICulture = new CultureInfo("en-US");
        }
    }

    public static void ResGenMain(string[] args)
    {
        // Tell build we had an error, then set this to 0 if we complete successfully.
        Environment.ExitCode = errorCode;

        SetConsoleUICulture();

        BadFileExtensionResourceString = "The file named \"{0}\" does not have a known extension.  Managed resource files must end in .ResX, .restext, .txt, .resources, .resources.dll, .dll or .exe. Response files must end in .rsp and be specified as @respFile.rsp.";

        if (args.Length < 1
            || args[0].Equals("-h", StringComparison.OrdinalIgnoreCase)
            || args[0].Equals("-?", StringComparison.OrdinalIgnoreCase)
            || args[0].Equals("/h", StringComparison.OrdinalIgnoreCase)
            || args[0].Equals("/?", StringComparison.OrdinalIgnoreCase))
        {
            Usage();
            return;
        }

        bool foundResponseFile = false;

        // Handle response files first; expand each and add to list
        List<string> argList = new();
        foreach (string arg in args)
        {
            // Response files are prepended with "@"
            if (arg.StartsWith("@", StringComparison.OrdinalIgnoreCase))
            {
                // already found another response file, but we only support one.
                if (foundResponseFile)
                {
                    Error("You specified multiple response files; at most one is allowed.");
                    break;
                }

                // handle malformed response file names and bad file extensions
                if (arg.Length == 1)
                {
                    Error($"""
                        You must specify response file names like this:
                        @respFile.rsp
                        You passed in \"{arg}\"
                        """);
                    break;
                }

                string responseFileName = arg[1..];
                if (!ValidResponseFileName(responseFileName))
                {
                    Error(string.Format(BadFileExtensionResourceString, responseFileName));
                    break;
                }

                if (!File.Exists(responseFileName))
                {
                    Error($"The specified response file doesn't exist. You passed in \"{responseFileName}\"");
                    break;
                }

                foundResponseFile = true;

                try
                {
                    // valid response file name: parse by line and add to list
                    string[] lines = File.ReadAllLines(responseFileName);
                    foreach (string line in lines)
                    {
                        // trim; beginning or ending whitespace on a line can cause
                        // an option not to be recognized
                        string trimmedLine = line.Trim();

                        // skip over comments and blank lines
                        if (trimmedLine.Length != 0 && !trimmedLine.StartsWith("#", StringComparison.OrdinalIgnoreCase))
                        {
                            if (trimmedLine.StartsWith("/compile", StringComparison.OrdinalIgnoreCase)
                                && trimmedLine.Length > 8)
                            {
                                Error($"Response files must be line-delimited; \"{responseFileName}\" contains \"{trimmedLine}\"");
                                break;
                            }
                            else
                            {
                                argList.Add(trimmedLine);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Error(e.Message, responseFileName);
                }

                // we could break here, but this allows there to be options in the
                // response file and /compile args on the command line.
            }
            else
            {
                // not a response file; just add to list
                argList.Add(arg);
            }
        }

        string[] inFiles = null;
        string[] outFilesOrDirs = null;

        // Default resource class options for all classes
        ResourceClassOptions resourceClassOptions = null;
        int argIndex = 0;
        bool setSimpleInputFile = false; // For resgen a.resources a.resx
        bool gotOutputFileName = false;  // For resgen a.txt a.resources b.txt
        bool useSourcePath = false;
        bool isClassInternal = true;
        bool simulateVS = false;

        while (argIndex < argList.Count && errors == 0)
        {
            if (argList[argIndex].Equals("/compile", StringComparison.OrdinalIgnoreCase))
            {
                SortedSet<string> sortedSetOfOutputFilepaths = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
                inFiles = new string[argList.Count - argIndex - 1];
                outFilesOrDirs = new string[argList.Count - argIndex - 1];
                for (int i = 0; i < inFiles.Length; i++)
                {
                    inFiles[i] = argList[argIndex + 1];
                    int index = inFiles[i].IndexOf(',');
                    if (index != -1)
                    {
                        string tmp = inFiles[i];
                        inFiles[i] = tmp[..index];
                        if (!ValidResourceFileName(inFiles[i]))
                        {
                            Error(string.Format(BadFileExtensionResourceString, inFiles[i]));
                            break;
                        }

                        if (index == tmp.Length - 1)
                        {
                            Error($"""
                                You must specify an input & outfile file name like this:
                                inFile.txt,outFile.resources.
                                You passed in "{tmp}".
                                """);
                            inFiles = Array.Empty<string>();
                            break;
                        }

                        outFilesOrDirs[i] = tmp[(index + 1)..];

                        if (GetFormat(inFiles[i]) == Format.Assembly)
                        {
                            Error("""
                                /compile is not supported with assemblies (.resources.dll, .dll or .exe) as input.
                                Use ResGen /? for usage information.
                                """);
                            break;
                        }

                        if (!ValidResourceFileName(outFilesOrDirs[i]))
                        {
                            Error(string.Format(BadFileExtensionResourceString, outFilesOrDirs[i]));
                            break;
                        }
                    }
                    else
                    {
                        if (!ValidResourceFileName(inFiles[i]))
                        {
                            if (inFiles[i][0] == '/' || inFiles[i][0] == '-')
                            {
                                // TODO: Error(SR.GetString(SR.InvalidCommandLineSyntax, "/compile", inFiles[i]));
                            }
                            else
                            {
                                Error(string.Format(BadFileExtensionResourceString, inFiles[i]));
                            }

                            break;
                        }

                        string resourceFileName = GetResourceFileName(inFiles[i]);
                        Debug.Assert(resourceFileName is not null, "Unexpected null file name!");
                        outFilesOrDirs[i] = resourceFileName;
                    }

                    string outputFileFullPath = Path.GetFullPath(outFilesOrDirs[i]);

                    if (sortedSetOfOutputFilepaths.Contains(outputFileFullPath))
                    {
                        // TODO: Error(SR.GetString(SR.DuplicateOutputFilenames, outputFileFullPath));
                        break;
                    }
                    else
                    {
                        sortedSetOfOutputFilepaths.Add(outputFileFullPath);
                    }

                    argIndex++;
                }
            }
            else if (argList[argIndex].StartsWith("/str:", StringComparison.OrdinalIgnoreCase))
            {
                // Strongly typed resource class syntax:
                // <language>[,<namespace>[,<class name>[,<file name>]]]]
                string s = argList[argIndex];
                int end = s.IndexOf(',', 5);
                if (end == -1)
                {
                    end = s.Length;
                }

                string language = s[5..end];  // Skip over "/str:"
                string nameSpace = null;
                string className = null;
                string outputFile = null;

                // Read namespace
                int first = end + 1;
                if (end < s.Length)
                {
                    end = s.IndexOf(',', first);
                    if (end == -1)
                    {
                        end = s.Length;
                    }
                }

                // empty namespaces are allowed
                if (first <= end)
                {
                    nameSpace = s[first..end];

                    // Read class name
                    if (end < s.Length)
                    {
                        first = end + 1;
                        end = s.IndexOf(',', first);
                        if (end == -1)
                        {
                            end = s.Length;
                        }

                        className = s[first..end];
                    }

                    // Read output file name
                    first = end + 1;
                    if (first < s.Length)
                    {
                        outputFile = s[first..];
                    }
                }

                resourceClassOptions = new(language, nameSpace, className, outputFile, isClassInternal, simulateVS);
            }
            else if (argList[argIndex].StartsWith("/define:", StringComparison.OrdinalIgnoreCase)
                || argList[argIndex].StartsWith("-define:", StringComparison.OrdinalIgnoreCase)
                || argList[argIndex].StartsWith("/D:", StringComparison.OrdinalIgnoreCase)
                || argList[argIndex].StartsWith("-D:", StringComparison.OrdinalIgnoreCase)
                || argList[argIndex].StartsWith("/d:", StringComparison.OrdinalIgnoreCase)
                || argList[argIndex].StartsWith("-d:", StringComparison.OrdinalIgnoreCase))
            {
                string defines;
                if (argList[argIndex].StartsWith("/D:", StringComparison.OrdinalIgnoreCase)
                    || argList[argIndex].StartsWith("-D:", StringComparison.OrdinalIgnoreCase)
                    || argList[argIndex].StartsWith("/d:", StringComparison.OrdinalIgnoreCase)
                    || argList[argIndex].StartsWith("-d:", StringComparison.OrdinalIgnoreCase))
                {
                    defines = argList[argIndex][3..];
                }
                else
                {
                    defines = argList[argIndex][8..];  // Skip over "/define:"
                }

                foreach (string define in defines.Split(','))
                {
                    if (define.Length == 0 || define.Contains('&') || define.Contains('|') || define.Contains('('))
                    {
                        // TODO: Error(SR.GetString(SR.InvalidIfdef, define));
                    }

                    definesList.Add(define);
                }
            }
            else if (argList[argIndex].StartsWith("/r:", StringComparison.OrdinalIgnoreCase)
                || argList[argIndex].StartsWith("-r:", StringComparison.OrdinalIgnoreCase))
            {
                // assembly names syntax /r:c:\system\System.Drawing.dll
                string s = argList[argIndex];
                s = s[3..];  // Skip over "/r:"
                assemblyList ??= new();

                try
                {
                    assemblyList.Add(AssemblyName.GetAssemblyName(s));
                }
                catch (Exception e)
                {
                    // TODO: Error(SR.GetString(SR.CantLoadAssembly, s, e.GetType().Name, e.Message));
                }
            }
            else if (argList[argIndex].Equals("/usesourcepath", StringComparison.OrdinalIgnoreCase)
                || argList[argIndex].Equals("-usesourcepath", StringComparison.OrdinalIgnoreCase))
            {
                useSourcePath = true;
            }
            else if (argList[argIndex].Equals("/publicclass", StringComparison.OrdinalIgnoreCase)
                || argList[argIndex].Equals("-publicclass", StringComparison.OrdinalIgnoreCase))
            {
                isClassInternal = false;
#if DEBUG
            }
            else if (argList[argIndex].Equals("/vs", StringComparison.OrdinalIgnoreCase)
                || argList[argIndex].Equals("-vs", StringComparison.OrdinalIgnoreCase))
            {
                simulateVS = true;
                if (resourceClassOptions != null)
                {
                    resourceClassOptions.SimulateVS = true;
                }
#endif
            }
            else
            {
                if (ValidResourceFileName(argList[argIndex]))
                {
                    if (!setSimpleInputFile)
                    {
                        inFiles = new string[1];
                        inFiles[0] = argList[argIndex];
                        outFilesOrDirs = new string[1];
                        outFilesOrDirs[0] = GetFormat(inFiles[0]) == Format.Assembly ? null : GetResourceFileName(inFiles[0]);
                        setSimpleInputFile = true;
                    }
                    else
                    {
                        if (!gotOutputFileName)
                        {
                            outFilesOrDirs[0] = argList[argIndex];
                            if (GetFormat(inFiles[0]) == Format.Assembly)
                            {
                                if (ValidResourceFileName(outFilesOrDirs[0]))
                                {
                                    // TODO: Warning(SR.GetString(SR.MustProvideOutputDirectoryNotFilename, outFilesOrDirs[0]));
                                }

                                if (!Directory.Exists(outFilesOrDirs[0]))
                                {
                                    // TODO: Error(SR.GetString(SR.OutputDirectoryMustExist, outFilesOrDirs[0]));
                                }
                            }

                            gotOutputFileName = true;
                        }
                        else
                        {
                            // TODO: Error(SR.GetString(SR.InvalidCommandLineSyntax, "<none>", argList[argIndex]));
                            break;
                        }
                    }
                }
                else if (setSimpleInputFile && !gotOutputFileName && GetFormat(inFiles[0]) == Format.Assembly)
                {
                    // The output is a directory name
                    outFilesOrDirs[0] = argList[argIndex];

                    if (!Directory.Exists(outFilesOrDirs[0]))
                    {
                        // TODO: Error(SR.GetString(SR.OutputDirectoryMustExist, outFilesOrDirs[0]));
                    }

                    gotOutputFileName = true;
                }
                else
                {
                    if (argList[argIndex][0] == '/' || argList[argIndex][0] == '-')
                    {
                        // TODO: Error(SR.GetString(SR.BadCommandLineOption, argList[argIndex]));
                    }
                    else
                    {
                        // TODO: Error(SR.GetString(BadFileExtensionResourceString, argList[argIndex]));
                    }

                    return;
                }
            }

            argIndex++;
        }

        if ((inFiles == null || inFiles.Length == 0) && errors == 0)
        {
            Usage();
            return;
        }

        if (resourceClassOptions is not null)
        {
            resourceClassOptions.InternalClass = isClassInternal;

            // Verify we don't produce two identically named resource classes, 
            // or write different classes to the same file when using the 
            // /compile option.
            if (inFiles.Length > 1)
            {
                if (resourceClassOptions.ClassName != null || resourceClassOptions.OutputFileName != null)
                {
                    // TODO: Error(SR.GetString(SR.CompileAndSTRDontMix));
                }
            }

            if (GetFormat(inFiles[0]) == Format.Assembly)
            {
                // TODO: Error(SR.GetString(SR.STRSwitchNotSupportedForAssemblies));
            }
        }

        // Do all the work.
        if (errors == 0)
        {
            Parallel.For(
                0,
                inFiles.Length,
                i =>
                {
                    ResGenRunner runner = new ResGenRunner();
                    runner.ProcessFile(inFiles[i], outFilesOrDirs[i], resourceClassOptions, useSourcePath);
                });
        }

        // Quit & report errors, if necessary.
        if (warnings != 0)
        {
            // TODO: Console.Error.WriteLine(SR.GetString(SR.WarningCount, warnings));
        }

        if (errors != 0)
        {
            // TODO: Console.Error.WriteLine(SR.GetString(SR.ErrorCount, errors));
            Debug.Assert(Environment.ExitCode != 0);
        }
        else
        {
            // Tell build we succeeded.
            Environment.ExitCode = 0;
        }
    }

    private static string GetResourceFileName(string inFile)
    {
        if (inFile is null)
        {
            return null;
        }

        // Note that the naming scheme is basename.[en-US.]resources
        int end = inFile.LastIndexOf('.');
        return end == -1 ? null : $"{inFile[..end]}.resources";
    }

    private static bool ValidResourceFileName(string inFile)
    {
        if (inFile is null)
        {
            return false;
        }

        return inFile.EndsWith(".resx", StringComparison.OrdinalIgnoreCase)
            || inFile.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)
            || inFile.EndsWith(".restext", StringComparison.OrdinalIgnoreCase)
            || inFile.EndsWith(".resources.dll", StringComparison.OrdinalIgnoreCase)
            || inFile.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)
            || inFile.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
            || inFile.EndsWith(".resources", StringComparison.OrdinalIgnoreCase);
    }

    private static bool ValidResponseFileName(string inFile)
        => inFile is not null && inFile.EndsWith(".rsp", StringComparison.OrdinalIgnoreCase);

    // Returns whether the #ifdefs are appropriately defined, or not defined for "!FOO".
    private static bool IfdefsAreActive(IEnumerable<string> searchForAll, IList<string> defines)
    {
        foreach (string define in searchForAll)
        {
            // Check for #ifdef FOO and #ifdef !BAR
            if (define[0] == '!')
            {
                if (defines.Contains(define[1..]))
                {
                    return false;
                }
            }
            else
            {
                if (!defines.Contains(define))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static void Usage()
    {
        // TODO: Console.WriteLine(SR.GetString(SR.UsageOnWindows, ThisAssembly.InformationalVersion, CommonResStrings.CopyrightForCmdLine));

        // TODO: Console.WriteLine(SR.GetString(SR.ValidLanguages));

        CompilerInfo[] compilerInfos = CodeDomProvider.GetAllCompilerInfo();
        for (int i = 0; i < compilerInfos.Length; i++)
        {
            string[] languages = compilerInfos[i].GetLanguages();
            if (i != 0)
            {
                Console.Write(", ");
            }

            for (int j = 0; j < languages.Length; j++)
            {
                if (j != 0)
                {
                    Console.Write(", ");
                }

                Console.Write(languages[j]);
            }
        }

        Console.WriteLine();
    }

    // Text files are just name/value pairs.  ResText is the same format
    // with a unique extension to work around some ambiguities with MSBuild
    // ResX is our existing XML format from V1.
    private enum Format
    {
        Text,       // .txt or .restext
        XML,        // .resx
        Assembly,   // .dll, .exe or .resources.dll
        Binary,     // .resources
    }
}
