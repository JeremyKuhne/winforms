namespace System.Tools;

public static partial class ResGen
{
    private struct ResourceEntry
    {
        public ResourceEntry(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name;
        public object Value;

        public void Deconstruct(out string name, out object value)
        {
            name = Name;
            value = Value;
        }
    }
}
