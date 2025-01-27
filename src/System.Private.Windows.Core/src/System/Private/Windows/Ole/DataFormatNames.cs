// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Private.Windows.Ole;

internal static partial class DataFormatNames
{
    internal const string Text = "Text";
    internal const string UnicodeText = "UnicodeText";
    internal const string Dib = "DeviceIndependentBitmap";
    internal const string Bitmap = "Bitmap";
    internal const string Emf = "EnhancedMetafile";
    internal const string Wmf = "MetaFilePict";
    internal const string SymbolicLink = "SymbolicLink";
    internal const string Dif = "DataInterchangeFormat";
    internal const string Tiff = "TaggedImageFileFormat";
    internal const string OemText = "OEMText";
    internal const string Palette = "Palette";
    internal const string PenData = "PenData";
    internal const string Riff = "RiffAudio";
    internal const string WaveAudio = "WaveAudio";
    internal const string FileDrop = "FileDrop";
    internal const string Locale = "Locale";
    internal const string Html = "HTML Format";
    internal const string Rtf = "Rich Text Format";
    internal const string Csv = "Csv";
    internal const string String = "System.String";
    internal const string Serializable = "WindowsForms10PersistentObject";
    internal const string Xaml = "Xaml";
    internal const string XamlPackage = "XamlPackage";
    internal const string InkSerializedFormat = "Ink Serialized Format";
    internal const string FileNameAnsi = "FileName";
    internal const string FileNameUnicode = "FileNameW";
    internal const string BinaryFormatBitmap = "System.Drawing.Bitmap";

    /// <summary>
    ///  Adds all the "synonyms" for the specified format.
    /// </summary>
    internal static void AddMappedFormats<T>(string format, T formats)
        where T : ICollection<string>
    {
        switch (format)
        {
            case Text:
                formats.Add(String);
                formats.Add(UnicodeText);
                break;
            case UnicodeText:
                formats.Add(String);
                formats.Add(Text);
                break;
            case String:
                formats.Add(Text);
                formats.Add(UnicodeText);
                break;
            case FileDrop:
                formats.Add(FileNameUnicode);
                formats.Add(FileNameAnsi);
                break;
            case FileNameUnicode:
                formats.Add(FileDrop);
                formats.Add(FileNameAnsi);
                break;
            case FileNameAnsi:
                formats.Add(FileDrop);
                formats.Add(FileNameUnicode);
                break;
            case Bitmap:
                formats.Add(BinaryFormatBitmap);
                break;
            case BinaryFormatBitmap:
                formats.Add(Bitmap);
                break;
        }
    }

    /// <summary>
    ///  Check if the <paramref name="format"/> is one of the restricted formats, which formats that
    ///  correspond to primitives or are pre-defined in the OS such as strings, bitmaps, and OLE types.
    /// </summary>
    internal static bool IsRestrictedFormat(string format) => RestrictDeserializationToSafeTypes(format)
        || format is Text
            or UnicodeText
            or Rtf
            or Html
            or OemText
            or FileDrop
            or FileNameAnsi
            or FileNameUnicode;

    /// <summary>
    ///  We are restricting binary serialization and deserialization of formats that represent strings, bitmaps or OLE types.
    /// </summary>
    /// <param name="format">format name</param>
    /// <returns><see langword="true" /> - serialize only safe types, strings or bitmaps.</returns>
    /// <remarks>
    ///  <para>
    ///   These formats are also restricted in WPF
    ///   https://github.com/dotnet/wpf/blob/db1ae73aae0e043326e2303b0820d361de04e751/src/Microsoft.DotNet.Wpf/src/PresentationCore/System/Windows/dataobject.cs#L2801
    ///  </para>
    /// </remarks>
    internal static bool RestrictDeserializationToSafeTypes(string format) =>
        format is String
            or BinaryFormatBitmap
            or Csv
            or Dib
            or Dif
            or Locale
            or PenData
            or Riff
            or SymbolicLink
            or Tiff
            or WaveAudio
            or Bitmap
            or Emf
            or Palette
            or Wmf;
}
