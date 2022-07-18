namespace System.Tools;


public static partial class ResGen
{
    internal sealed class ResourceClassOptions
    {
        internal ResourceClassOptions(
            string language,
            string nameSpace,
            string className,
            string outputFileName,
            bool isClassInternal,
            bool simulateVS)
        {
            Language = language;
            NameSpace = nameSpace;
            ClassName = className;
            OutputFileName = outputFileName;
            InternalClass = isClassInternal;
            SimulateVS = simulateVS;
        }

        internal string Language { get; }

        internal string NameSpace { get; }

        internal string ClassName { get; }

        internal string OutputFileName { get; }

        internal bool InternalClass { get; set; }

        // Mimic how VS's project system gives a list of ResXDataNodes to
        // StronglyTypedResourceBuilder, etc.
        internal bool SimulateVS { get; set; }
    }
}
