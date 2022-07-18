namespace System.Tools;


public static partial class ResGen
{
    // For flow of control & passing sufficient error context back 
    // from ReadTextResources
    internal sealed class TextFileException : Exception
    {
        internal TextFileException(string message, string fileName, int lineNumber, int linePosition) : base(message)
        {
            FileName = fileName;
            LineNumber = lineNumber;
            LinePosition = linePosition;
        }

        internal string FileName { get; }

        internal int LineNumber { get; }

        internal int LinePosition { get; }
    }
}
