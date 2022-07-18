// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Resources;
using System.Text;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Resources.Tools;
using System.Xml;
using System.Reflection;

namespace System.Tools;

public static partial class ResGen
{
    private partial class ResGenRunner
    {
        private readonly List<Action> _bufferedOutput = new(2); // Common case, we write 2 lines of output.
        private readonly List<ResourceSet> _resourceSets = new();
        private bool _hadErrors;

        private void AddResource(
            ResourceSet resourceSet,
            string name,
            object value,
            string inputFileName,
            int lineNumber,
            int linePosition)
        {
            if (!resourceSet.TryAddResource(name, value))
            {
                Warning($"""Duplicate resource key! Name was "{name}".""", inputFileName, lineNumber, linePosition);
            }
        }

        private void AddResource(ResourceSet resourceSet, string name, object value, string inputFileName)
        {
            if (!resourceSet.TryAddResource(name, value))
            {
                Warning($"""Duplicate resource key! Name was "{name}".""", inputFileName);
            }
        }

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
        private void Error(string message, int errorNumber = 0)
        {
            string errorFormat = "ResGen : error RG{1:0000}: {0}";
            BufferErrorLine(errorFormat, message, errorNumber);
            Interlocked.Increment(ref errors);
            _hadErrors = true;
        }

        // Use this for a general error w.r.t. a file, like a missing file.
        private void Error(string message, string fileName, int errorNumber = 0)
        {
            string errorFormat = "{0} : error RG{1:0000}: {2}";
            BufferErrorLine(errorFormat, fileName, errorNumber, message);
            Interlocked.Increment(ref errors);
            _hadErrors = true;
        }

        // For specific errors about the contents of a file and you know where
        // the error occurred.
        private void Error(string message, string fileName, int line, int column)
        {
            Error(message, fileName, line, column, 0);
        }

        // For specific errors about the contents of a file and you know where
        // the error occurred.
        private void Error(string message, string fileName, int line, int column, int errorNumber)
        {
            string errorFormat = "{0}({1},{2}): error RG{3:0000}: {4}";
            BufferErrorLine(errorFormat, fileName, line, column, errorNumber, message);
            Interlocked.Increment(ref errors);
            _hadErrors = true;
        }

        // General warnings
        private void Warning(string message)
        {
            string warningFormat = "ResGen : warning RG0000 : {0}";
            BufferErrorLine(warningFormat, message);
            Interlocked.Increment(ref warnings);
        }

        // Warnings in a particular file, but we don't have line number info
        private void Warning(string message, string fileName, int warningNumber = 0)
        {
            string warningFormat = "{0} : warning RG{1:0000}: {2}";
            BufferErrorLine(warningFormat, fileName, warningNumber, message);
            Interlocked.Increment(ref warnings);
        }

        // Warnings in a file on a particular line and character
        private void Warning(string message, string fileName, int line, int column)
        {
            Warning(message, fileName, line, column, 0);
        }

        // Warnings in a file on a particular line and character
        private void Warning(string message, string fileName, int line, int column, int warningNumber)
        {
            string warningFormat = "{0}({1},{2}): warning RG{3:0000}: {4}";
            BufferErrorLine(warningFormat, fileName, line, column, warningNumber, message);
            Interlocked.Increment(ref warnings);
        }

        private void BufferErrorLine(string formatString, params object[] args)
            => _bufferedOutput.Add(() => { Console.Error.WriteLine(formatString, args); });

        private void BufferWriteLine()
            => BufferWriteLine(string.Empty);

        private void BufferWriteLine(string formatString, params object[] args)
            => _bufferedOutput.Add(() => { Console.WriteLine(formatString, args); });

        private void BufferWrite(string formatString, params object[] args)
            => _bufferedOutput.Add(() => { Console.Write(formatString, args); });

        public void ProcessFile(
            string inputFile,
            string outputFileOrDirectory,
            ResourceClassOptions resourceClassOptions,
            bool useSourcePath)
        {
            ProcessFileWorker(inputFile, outputFileOrDirectory, resourceClassOptions, useSourcePath);
            lock (consoleOutputLock)
            {
                foreach (Action writeAction in _bufferedOutput)
                {
                    writeAction.Invoke();
                }
            }

            // If there was an error, delete the output file, ensuring the build won't
            // continue using half-generated output file.
            if (_hadErrors && outputFileOrDirectory is not null && File.Exists(outputFileOrDirectory)
                && GetFormat(inputFile) != Format.Assembly // outFileOrDir is a directory when the input file is an assembly
                && GetFormat(outputFileOrDirectory) != Format.Assembly // Never delete an assembly since we don't ever actually write to assemblies.
                )
            {
                // Do a full collect and wait for finalizers to clear out any lingering file handles.
                GC.Collect(2);
                GC.WaitForPendingFinalizers();
                try
                {
                    File.Delete(outputFileOrDirectory);
                }
                catch { }
            }
        }

        public void ProcessFileWorker(
            string inputFile,
            string outputFileOrDirectory,
            ResourceClassOptions resourceClassOptions,
            bool useSourcePath)
        {
            try
            {
                // Explicitly handle missing input files here - don't catch a FileNotFoundException since we can get
                // them from the loader if we try loading an assembly version we can't find.
                if (!File.Exists(inputFile))
                {
                    Error($"""Couldn't find input file "{inputFile}".""");
                    return;
                }

                if (GetFormat(inputFile) != Format.Assembly
                    // outFileOrDir is a directory when the input file is an assembly
                    && GetFormat(outputFileOrDirectory) == Format.Assembly)
                {
                    Error($"""ResGen cannot write assemblies, only read from them. """ +
                        """Cannot create assembly "{outputFileOrDirectory}".""");
                    return;
                }

                ReadResources(inputFile, useSourcePath);
            }
            catch (ArgumentException ae)
            {
                if (ae.InnerException is XmlException xe)
                {
                    Error(xe.Message, inputFile, xe.LineNumber, xe.LinePosition);
                }
                else
                {
                    Error(ae.Message, inputFile);
                }

                return;
            }
            catch (TextFileException tfe)
            {
                // Used to pass back error context from ReadTextResources to here.
                Error(tfe.Message, tfe.FileName, tfe.LineNumber, tfe.LinePosition);
                return;
            }
            catch (XmlException xe)
            {
                Error(xe.Message, inputFile, xe.LineNumber, xe.LinePosition);
                return;
            }
            catch (Exception e)
            {
                Error(e.Message, inputFile);

                // We need to give meaningful error messages to the user.  Note that ResXResourceReader wraps any
                // exception it gets in an ArgumentException with the message "Invalid ResX input." If you don't look
                // at the InnerException, you have to attach a debugger to find the problem.
                if (e.InnerException is not null)
                {
                    Exception inner = e.InnerException;
                    StringBuilder builder = new(200);
                    builder.Append(e.Message);
                    while (inner is not null)
                    {
                        builder.Append($" ---> {inner.GetType().Name}: {inner.Message}");
                        inner = inner.InnerException;
                    }

                    // TODO: Error(SR.GetString(SR.SpecificError, e.InnerException.GetType().Name, sb.ToString()), inputFile);
                }

                return;
            }

            string currentOutputFile = null;
            string currentOutputDirectory = null;
            string currentOutputSourceCodeFile = null;
            bool currentOutputDirectoryAlreadyExisted = true;

            try
            {
                if (GetFormat(inputFile) == Format.Assembly)
                {
                    foreach (ResourceSet reader in _resourceSets)
                    {
                        string currentOutputFileNoPath = $"{reader.OutputFileName}.resw";
                        currentOutputFile = null;
                        currentOutputDirectoryAlreadyExisted = true;
                        currentOutputDirectory = Path.Join(
                            outputFileOrDirectory ?? string.Empty,
                            reader.CultureName ?? string.Empty);

                        if (currentOutputDirectory.Length == 0)
                        {
                            // Write output file to current working directory
                            currentOutputFile = currentOutputFileNoPath;
                        }
                        else
                        {
                            if (!Directory.Exists(currentOutputDirectory))
                            {
                                currentOutputDirectoryAlreadyExisted = false;
                                Directory.CreateDirectory(currentOutputDirectory);
                            }

                            currentOutputFile = Path.Join(currentOutputDirectory, currentOutputFileNoPath);
                        }

                        WriteResources(reader, currentOutputFile);
                    }
                }
                else
                {
                    currentOutputFile = outputFileOrDirectory;
                    Contract.Assert(
                        _resourceSets.Count == 1,
                        $"We have 0 readers, or we have multiple readers that we're ignoring.  Num readers: {_resourceSets.Count}");
                    WriteResources(_resourceSets[0], outputFileOrDirectory);

                    if (resourceClassOptions is not null)
                    {
                        Contract.Assert(
                            _resourceSets.Count == 1,
                            $"We have 0 readers, or we have multiple readers that we're ignoring.  Num readers: {_resourceSets.Count}");
                        CreateStronglyTypedResources(
                            _resourceSets[0],
                            outputFileOrDirectory,
                            resourceClassOptions,
                            inputFile,
                            out currentOutputSourceCodeFile);
                    }
                }
            }
            catch (IOException io)
            {
                if (currentOutputFile is not null)
                {
                    // TODO: Error(SR.GetString(SR.WriteError, currentOutputFile), currentOutputFile);
                    if (io.Message is not null)
                    {
                        // TODO: Error(SR.GetString(SR.SpecificError, io.GetType().Name, io.Message), currentOutputFile);
                    }

                    if (File.Exists(currentOutputFile) && GetFormat(currentOutputFile) != Format.Assembly)
                    {
                        RemoveCorruptedFile(currentOutputFile);

                        if (currentOutputSourceCodeFile is not null)
                        {
                            RemoveCorruptedFile(currentOutputSourceCodeFile);
                        }
                    }
                }

                if (currentOutputDirectory is not null && currentOutputDirectoryAlreadyExisted == false)
                {
                    // Do not annoy the user by removing an empty directory we did not create.
                    try
                    {
                        Directory.Delete(currentOutputDirectory); // Remove output directory if empty
                    }
                    catch (Exception)
                    {
                        // Fail silently (we are not even checking if the call to File.Delete succeeded)
                    }
                }

                return;
            }
            catch (Exception e)
            {
                if (currentOutputFile != null)
                {
                    // TODO: Error(SR.GetString(SR.GenericWriteError, currentOutputFile));
                }

                if (e.Message is not null)
                {
                    // TODO: Error(SR.GetString(SR.SpecificError, e.GetType().Name, e.Message));
                }
            }
        }

        private void CreateStronglyTypedResources(
            ResourceSet resourceSet,
            string outputFile,
            ResourceClassOptions options,
            string inputFile,
            out string sourceFile)
        {
            CodeDomProvider provider = CodeDomProvider.CreateProvider(options.Language);

            // Assume the resource class name equals the base name, unless the resource class options override it.
            string baseName = outputFile[..outputFile.LastIndexOf('.')];
            int last = baseName.LastIndexOfAny(new char[]
                {
                    Path.VolumeSeparatorChar,
                    Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar
                });

            if (last != -1)
            {
                baseName = baseName[(last + 1)..];
            }

            string nameSpace = options.NameSpace;
            string className = options.ClassName;
            if (string.IsNullOrEmpty(className))
            {
                className = baseName;
            }

            sourceFile = options.OutputFileName;
            if (string.IsNullOrEmpty(sourceFile))
            {
                string baseFileName = outputFile[..outputFile.LastIndexOf('.')];
                sourceFile = $"{baseFileName}.{provider.FileExtension}";
            }

            // Do the classname fixup in advance so we can do the right checking and give the right error message. 
            string fixedClassName = StronglyTypedResourceBuilder.VerifyResourceName(className, provider);
            if (fixedClassName is not null)
            {
                className = fixedClassName;
            }

            string generatedBaseName;
            if (string.IsNullOrEmpty(nameSpace))
            {
                BufferWrite($"""Creating strongly typed resource class "{className}"...""");
                generatedBaseName = className;
            }
            else
            {
                BufferWrite($"""Creating strongly typed resource class "{nameSpace}.{className}"...""");
                generatedBaseName = $"{nameSpace}.{className}";
            }

            if (!baseName.Equals(generatedBaseName, StringComparison.OrdinalIgnoreCase)
                && outputFile.EndsWith(".resources", StringComparison.OrdinalIgnoreCase))
            {
                BufferWriteLine();
                // TODO: Warning(SR.GetString(SR.ClassnameMustMatchBasename, baseName, generatedBaseName), inputFileName);
            }

            CodeCompileUnit compileUnit = StronglyTypedResourceBuilder.Create(
                resourceSet.AsDictionary,
                className,
                nameSpace,
                provider,
                options.InternalClass,
                out string[] errors);

            compileUnit.ReferencedAssemblies.Add("System.dll");

            CodeGeneratorOptions codeGenOptions = new CodeGeneratorOptions();

            UTF8Encoding enc = new UTF8Encoding(true, true);
            using (TextWriter output = new StreamWriter(sourceFile, false, enc))
            {
                provider.GenerateCodeFromCompileUnit(compileUnit, output, codeGenOptions);
            }

            if (errors.Length > 0)
            {
                BufferWriteLine();
                foreach (string error in errors)
                {
                    Error($"""Could not create a property on the strongly typed resource class for the resource name "{error}".""", inputFile);
                }
            }
            else
            {
                BufferWriteLine("Done.");
            }
        }

        private void ReadResources(string filename, bool useSourcePath)
        {
            Format format = GetFormat(filename);

            if (format == Format.Assembly) // Multiple input .resources files within one assembly
            {
                ReadAssemblyResources(filename);
            }
            else
            {
                ResourceSet readerInfo = new ResourceSet();
                _resourceSets.Add(readerInfo);
                switch (format)
                {
                    case Format.Text:
                        ReadTextResources(readerInfo, filename);
                        break;

                    case Format.XML:
                        ResXResourceReader resXReader;
                        resXReader = assemblyList is not null
                            ? new ResXResourceReader(filename, assemblyList.ToArray())
                            : new ResXResourceReader(filename);

                        if (useSourcePath)
                        {
                            resXReader.BasePath = Path.GetDirectoryName(Path.GetFullPath(filename));
                        }

                        // ReadResources closes the reader for us
                        ReadResources(readerInfo, resXReader, filename);
                        break;

                    case Format.Binary:
                        ReadResources(readerInfo, new ResourceReader(filename), filename); // closes reader for us
                        break;

                    default:
                        Debug.Fail($"Unknown format {format}");
                        break;
                }

                BufferWriteLine($"""Read in {readerInfo.Resources.Count} resources from "{filename}".""");
            }
        }

        // Closes reader when done. File name is for error reporting.
        private void ReadResources(ResourceSet resourceSet, IResourceReader reader, string fileName)
        {
            using (reader)
            {
                var enumerator = reader.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    string name = (string)enumerator.Key;
                    object value = enumerator.Value;
                    AddResource(resourceSet, name, value, fileName);
                }
            }
        }

        private void ReadTextResources(ResourceSet resourceSet, string fileName)
        {
            Stack<string> currentIfdefs = new Stack<string>();
            bool ignoreLine = false;  // Whether we're in a set of #ifdef's that aren't currently defined.

            // Check for byte order marks in the beginning of the input file, but default to UTF-8.
            using LineNumberStreamReader sr = new(fileName, new UTF8Encoding(true), true);

            StringBuilder name = new(40);
            StringBuilder value = new(120);

            int ch = sr.Read();
            while (ch != -1)
            {
                if (ch == '\n' || ch == '\r')
                {
                    ch = sr.Read();
                    continue;
                }

                // #ifdef support. Support a simplistic grammar, the union of C++, C# & VB, only
                // without support for // in comments. Like this:
                //
                // #ifndef SYMBOL   # equivalent to #if !SYMBOL.
                // <conditional code here>
                // #endif   # comment
                //
                // VB syntax "#If SYMBOL Then" followed by "#End If"
                //
                // Not currently supporting && and ||, nor #elif or #else.
                if (ch == '#')
                {
                    string maybeIfdef = sr.ReadLine();
                    if (string.IsNullOrEmpty(maybeIfdef))
                    {
                        // Treat as if it were a comment. Ignore.
                        ch = sr.Read();
                        continue;
                    }

                    if (maybeIfdef.StartsWith("ifdef ", StringComparison.InvariantCulture)
                        || maybeIfdef.StartsWith("ifndef ", StringComparison.InvariantCulture)
                        || maybeIfdef.StartsWith("if ", StringComparison.InvariantCulture)
                        || maybeIfdef.StartsWith("If ", StringComparison.InvariantCulture))
                    {
                        // Strip off leading "if" or "ifdef", then remove comments at the end.
                        string define = maybeIfdef[(maybeIfdef.IndexOf(' ') + 1)..].Trim();
                        for (int i = 0; i < define.Length; i++)
                        {
                            if (define[i] == '#' || define[i] == ';')
                            {
                                // Comments
                                define = define[..i].Trim();
                                break;
                            }
                        }

                        // Check for VB's "#If SYMBOL Then", and remove "Then"
                        if (maybeIfdef[0] == 'I' && define.EndsWith(" Then", StringComparison.InvariantCulture))
                        {
                            define = define[..^5];
                        }

                        if (define.Length == 0 || define.Contains('&') || define.Contains('|') || define.Contains('('))
                        {
                            throw new TextFileException(
                                $"""Found an invalid #ifdef value, "{define}".""",
                                fileName,
                                sr.LineNumber - 1,
                                7);
                        }

                        if (maybeIfdef.StartsWith("ifndef", StringComparison.InvariantCulture))
                        {
                            define = $"!{define}";
                        }

                        currentIfdefs.Push(define);
                        ignoreLine = !IfdefsAreActive(currentIfdefs, definesList);
                    }
                    else if (maybeIfdef.StartsWith("endif", StringComparison.InvariantCulture)
                        || maybeIfdef.StartsWith("End If", StringComparison.InvariantCulture))
                    {
                        if (currentIfdefs.Count == 0)
                        {
                            throw new TextFileException(
                                "Found an #endif without a matching #ifdef.",
                                fileName,
                                sr.LineNumber - 1,
                                1);
                        }

                        currentIfdefs.Pop();
                        ignoreLine = !IfdefsAreActive(currentIfdefs, definesList);
                    }

                    // Treat as a comment. Ignore.
                    ch = sr.Read();
                    continue;
                }

                // Skip over commented lines, ones starting with whitespace, or ones that shouldn't be included based on
                // our current ifdefs. Support LocStudio INF format's comment char, ';'
                if (ignoreLine || ch == '\t' || ch == ' ' || ch == ';')
                {
                    // Comment char (or blank line) - skip line.
                    sr.ReadLine();
                    ch = sr.Read();
                    continue;
                }

                // Note that in Beta of version 1 we recommended users should put a [strings] section in their file.
                // Now it's completely unnecessary and can only cause bugs.  We will not parse anything using '[' stuff
                // now and we should give a warning about seeing [strings] stuff. In V1.1 or V2, we can rip this out
                // completely, I hope.
                if (ch == '[')
                {
                    string skip = sr.ReadLine();
                    if (skip.Equals("strings]", StringComparison.OrdinalIgnoreCase))
                    {
                        Warning(
                            """The "[strings]" tag is no longer necessary in your text files.  Please remove it.""",
                            fileName,
                            sr.LineNumber - 1,
                            1);
                    }
                    else
                    {
                        throw new TextFileException(
                            $"""Unexpected INF file bracket syntax: "[{skip}".""",
                            fileName,
                            sr.LineNumber - 1,
                            1);
                    }

                    ch = sr.Read();
                    continue;
                }

                // Read in name
                name.Length = 0;
                while (ch != '=')
                {
                    if (ch == '\r' || ch == '\n')
                    {
                        throw new TextFileException(
                            """Found a resource that had a new line in it, but couldn't find the equal sign within! """ +
                                $"""Length: {name.Length}  name: '{name}'.""",
                            fileName,
                            sr.LineNumber,
                            sr.LinePosition);
                    }

                    name.Append((char)ch);
                    ch = sr.Read();
                    if (ch == -1)
                    {
                        break;
                    }
                }

                if (name.Length == 0)
                {
                    throw new TextFileException(
                        """Found an equals sign at beginning of a line! Expected a name / value pair like 'name = value'.""",
                        fileName,
                        sr.LineNumber,
                        sr.LinePosition);
                }

                // For the INF file, we must allow a space on both sides of the equals// sign. Deal with it.
                if (name[^1] == ' ')
                {
                    name.Length--;
                }

                // move past =
                ch = sr.Read();

                // If it exists, move past the first space after the equals sign.
                if (ch == ' ')
                {
                    ch = sr.Read();
                }

                // Read in value
                value.Length = 0;

                while (ch != -1)
                {
                    // Did we read @"\r" or @"\n"?
                    bool quotedNewLine = false;
                    if (ch == '\\')
                    {
                        ch = sr.Read();
                        switch (ch)
                        {
                            case '\\':
                                // nothing needed
                                break;
                            case 'n':
                                ch = '\n';
                                quotedNewLine = true;
                                break;
                            case 'r':
                                ch = '\r';
                                quotedNewLine = true;
                                break;
                            case 't':
                                ch = '\t';
                                break;
                            case '"':
                                ch = '\"';
                                break;
                            case 'u':
                                char[] hex = new char[4];
                                int numChars = 4;
                                int index = 0;
                                while (numChars > 0)
                                {
                                    int n = sr.Read(hex, index, numChars);
                                    if (n == 0)
                                    {
                                        throw new TextFileException(
                                            """Unsupported or invalid escape character in value! """ +
                                                $"""Escape char: '{(char)ch}' Name was: "{name}".""",
                                            fileName,
                                            sr.LineNumber,
                                            sr.LinePosition);
                                    }

                                    index += n;
                                    numChars -= n;
                                }

                                ch = (char)ushort.Parse(new string(hex), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                                quotedNewLine = (ch == '\n' || ch == '\r');
                                break;

                            default:
                                throw new TextFileException(
                                    """Unsupported or invalid escape character in value! """ +
                                        $"""Escape char: '{(char)ch}' Name was: "{name}".""",
                                    fileName,
                                    sr.LineNumber,
                                    sr.LinePosition);
                        }
                    }

                    // Consume endline...
                    //   Endline can be \r\n or \n.  But do not treat a quoted newline (ie, @"\r" or @"\n" in text) as a
                    //   real new line. They aren't the end of a line.
                    if (!quotedNewLine)
                    {
                        if (ch == '\r')
                        {
                            ch = sr.Read();
                            if (ch == -1)
                            {
                                break;
                            }
                            else if (ch == '\n')
                            {
                                ch = sr.Read();
                                break;
                            }
                        }
                        else if (ch == '\n')
                        {
                            ch = sr.Read();
                            break;
                        }
                    }

                    value.Append((char)ch);
                    ch = sr.Read();
                }

                // Note that value can be an empty string
                AddResource(resourceSet, name.ToString(), value.ToString(), fileName, sr.LineNumber, sr.LinePosition);
            }

            if (currentIfdefs.Count > 0)
            {
                throw new TextFileException(
                    """Found an #ifdef but not a matching #endif before reaching the end of the file. """ +
                        $"""Unmatched #ifdef: "{currentIfdefs.Pop()}".""",
                    fileName,
                    sr.LineNumber - 1,
                    1);
            }
        }

        private void WriteResources(ResourceSet reader, string filename)
        {
            Format format = GetFormat(filename);
            switch (format)
            {
                case Format.Text:
                    WriteTextResources(reader, filename);
                    break;
                case Format.XML:
                    WriteResources(reader, new ResXResourceWriter(filename)); // closes writer for us
                    break;
                case Format.Assembly:
                    Error($"""ResGen cannot write assemblies, only read from them. Cannot create assembly "{filename}".""");
                    break;
                case Format.Binary:
                    WriteResources(reader, new ResourceWriter(filename)); // closes writer for us
                    break;
                default:
                    Debug.Fail($"Unknown format {format}");
                    break;
            }
        }

        // closes writer automatically
        private void WriteResources(ResourceSet resourceSet, IResourceWriter writer)
        {
            Exception capturedException = null;
            try
            {
                foreach ((string key, object value) in resourceSet.Resources)
                {
                    writer.AddResource(key, value);
                }

                BufferWrite("Writing resource file...");
            }
            catch (Exception e)
            {
                capturedException = e; // Rethrow this after catching exceptions thrown by Close().
            }
            finally
            {
                if (capturedException == null)
                {
                    writer.Close(); // If this throws, exceptions will be caught upstream.
                }
                else
                {
                    // It doesn't hurt to call Close() twice. In the event of a full disk, we *need* to call Close() twice.
                    // In that case, the first time we catch an exception indicating that the XML written to disk is malformed,
                    // specifically an InvalidOperationException: "Token EndElement in state Error would result in an invalid XML document."
                    try
                    { writer.Close(); }
                    catch (Exception) { } // We agressively catch all exception types since we already have one we will throw.
                    // The second time we catch the out of disk space exception.
                    try
                    { writer.Close(); }
                    catch (Exception) { } // We agressively catch all exception types since we already have one we will throw.
                    throw capturedException; // In the event of a full disk, this is an out of disk space IOException.
                }
            }

            BufferWriteLine("Done.");
        }

        private void WriteTextResources(ResourceSet reader, string fileName)
        {
            using StreamWriter writer = new(fileName, append: false, Encoding.UTF8);

            foreach ((string key, object value) in reader.Resources)
            {
                string stringValue = value as string;
                if (stringValue is null)
                {
                    // TODO: Error(SR.GetString(SR.OnlyString, key, v.GetType().FullName), fileName);
                }

                // Escape any special characters in the String.
                stringValue = stringValue.Replace("\\", "\\\\");
                stringValue = stringValue.Replace("\n", "\\n");
                stringValue = stringValue.Replace("\r", "\\r");
                stringValue = stringValue.Replace("\t", "\\t");

                writer.WriteLine("{0}={1}", key, stringValue);
            }
        }

        internal void ReadAssemblyResources(string name)
        {
            Assembly assembly = null;
            bool mainAssembly = false;
            bool failedLoadingCultureInfo = false;
            NeutralResourcesLanguageAttribute neutralResourcesLanguageAttribute = null;
            AssemblyName assemblyName = null;

            try
            {
                assembly = Assembly.UnsafeLoadFrom(name);
                assemblyName = assembly.GetName();
                CultureInfo ci = null;
                try
                {
                    ci = assemblyName.CultureInfo;
                }
                catch (ArgumentException e)
                {
                    // TODO: Warning(SR.GetString(SR.CreatingCultureInfoFailed, e.GetType().Name, e.Message, assemblyName.ToString()));
                    failedLoadingCultureInfo = true;
                }

                if (!failedLoadingCultureInfo)
                {
                    mainAssembly = ci.Equals(CultureInfo.InvariantCulture);
                    neutralResourcesLanguageAttribute = CheckAssemblyCultureInfo(name, assemblyName, ci, assembly, mainAssembly);
                }
            }
            catch (BadImageFormatException)
            {
                // TODO: Error(SR.GetString(SR.BadImageFormat, name));
            }
            catch (Exception e)
            {
                // TODO: Error(SR.GetString(SR.CannotLoadAssemblyLoadFromFailed, name, e));
            }

            if (assembly is null)
            {
                return;
            }

            string[] resources = assembly.GetManifestResourceNames();
            CultureInfo satCulture = null;
            string expectedExt = null;
            if (!failedLoadingCultureInfo)
            {
                satCulture = assemblyName.CultureInfo;
                if (!satCulture.Equals(CultureInfo.InvariantCulture))
                {
                    expectedExt = $".{satCulture.Name}.resources";
                }
            }

            foreach (string resName in resources)
            {
                if (!resName.EndsWith(".resources", StringComparison.InvariantCultureIgnoreCase))
                {
                    // Skip non-.resources assembly blobs
                    continue;
                }

                if (mainAssembly)
                {
                    if (CultureInfo.InvariantCulture.CompareInfo.IsSuffix(resName, ".en-US.resources"))
                    {
                        // TODO: Error(SR.GetString(SR.ImproperlyBuiltMainAssembly, resName, name));
                        continue;
                    }
                }
                else if (!failedLoadingCultureInfo && !CultureInfo.InvariantCulture.CompareInfo.IsSuffix(resName, expectedExt))
                {
                    // TODO: Error(SR.GetString(SR.ImproperlyBuiltSatelliteAssembly, resName, expectedExt, name));
                    continue;
                }

                try
                {
                    Stream s = assembly.GetManifestResourceStream(resName);
                    using IResourceReader reader = new ResourceReader(s);
                    ResourceSet info = new()
                    {
                        OutputFileName = resName.Remove(resName.Length - 10) // Remove the .resources extension
                    };

                    if (satCulture is not null && !string.IsNullOrEmpty(satCulture.Name))
                    {
                        info.CultureName = satCulture.Name;
                    }
                    else if (neutralResourcesLanguageAttribute != null && !string.IsNullOrEmpty(neutralResourcesLanguageAttribute.CultureName))
                    {
                        info.CultureName = neutralResourcesLanguageAttribute.CultureName;
                        Warning($"""This assembly contains neutral resources corresponding to the culture "{info.CultureName}". """ +
                            """These resources will not be considered neutral in the output format as we are unable """ +
                            $"""to preserve this information. The resources will continue to correspond to "{info.CultureName}" in the output format.""");
                    }

                    if (info.CultureName is not null)
                    {
                        // Remove the culture from the filename
                        if (info.OutputFileName.EndsWith($".{info.CultureName}", StringComparison.OrdinalIgnoreCase))
                        {
                            info.OutputFileName = info.OutputFileName.Remove(info.OutputFileName.Length - (info.CultureName.Length + 1));
                        }
                    }

                    _resourceSets.Add(info);

                    foreach (DictionaryEntry pair in reader)
                    {
                        AddResource(info, (string)pair.Key, pair.Value, resName);
                    }

                    BufferWriteLine($"""Read in {info.Resources.Count} resources from "{resName}".""");
                }
                catch (FileNotFoundException)
                {
                    Error($"""Couldn't find the linked resources file "{resName}" listed in the assembly manifest.""");
                }
            }
        }

        private NeutralResourcesLanguageAttribute CheckAssemblyCultureInfo(
            string name,
            AssemblyName assemblyName,
            CultureInfo culture,
            Assembly a,
            bool mainAssembly)
        {
            NeutralResourcesLanguageAttribute neutralResourcesLanguageAttribute = null;

            if (mainAssembly)
            {
                object[] attrs = a.GetCustomAttributes(typeof(NeutralResourcesLanguageAttribute), inherit: false);
                if (attrs.Length != 0)
                {
                    neutralResourcesLanguageAttribute = (NeutralResourcesLanguageAttribute)attrs[0];
                    var location = neutralResourcesLanguageAttribute.Location;
                    if (location is not UltimateResourceFallbackLocation.Satellite and not UltimateResourceFallbackLocation.MainAssembly)
                    {
                        Warning($"Invalid or unrecognized UltimateResourceFallbackLocation value in the NeutralResourcesLanguageAttribute for assembly \"{name}\". Location: \"{location}\"");
                    }

                    if (!ContainsProperlyNamedResourcesFiles(a, true))
                    {
                        Error("This assembly claims to contain neutral resources, but doesn't contain any .resources files as manifest resources.  Either the NeutralResourcesLanguageAttribute was wrong, or there is a build-related problem with this assembly.");
                    }
                }
            }
            else
            {
                // Satellite assembly, or a mal-formed main assembly
                if (!assemblyName.Name.EndsWith(".resources", StringComparison.InvariantCultureIgnoreCase))
                {
                    Error($"""The assembly in file "{name}" has an assembly culture, indicating it is a satellite assembly for culture"""
                        + $""" "{culture.Name}.  But satellite assembly simple names must end in ".resources", while this one's simple"""
                        + $""" name is "{assemblyName.Name}".  This is either a main assembly with the culture incorrectly set, or a"""
                        + "satellite assembly with an incorrect simple name.");
                    return null;
                }

                Type[] types = a.GetTypes();
                if (types.Length > 0)
                {
                    Warning($"""The assembly "{name}" says it is a satellite assembly, but it contains code."""
                        + " Main assemblies shouldn't specify the assembly culture in their manifest, and satellites"
                        + " should not contain code.  This is almost certainly an error in your build process.");
                }

                if (!ContainsProperlyNamedResourcesFiles(a, false))
                {
                    Warning("This assembly claims to be a satellite assembly, but doesn't contain any properly named"
                        + ".resources files as manifest resources.  The name of the files should end in"
                        + $" {assemblyName.CultureInfo.Name}.resources.  There is probably a build-related problem with"
                        + " this assembly.");
                }
            }

            return neutralResourcesLanguageAttribute;
        }

        private static bool ContainsProperlyNamedResourcesFiles(Assembly a, bool mainAssembly)
        {
            string postfix = mainAssembly ? ".resources" : a.GetName().CultureInfo.Name + ".resources";
            foreach (string manifestResourceName in a.GetManifestResourceNames())
            {
                if (manifestResourceName.EndsWith(postfix, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
