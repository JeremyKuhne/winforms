using System.Text;
using System.Runtime.InteropServices;

namespace System.Tools;


public static partial class ResGen
{
    internal sealed class LineNumberStreamReader : StreamReader
    {
        // Line numbers start from 1, as well as line position.
        // For better error reporting, set line number to 1 and col to 0.
        private int _lineNumber;
        private int _linePosition;

        internal LineNumberStreamReader(
            string fileName,
            Encoding encoding,
            bool detectEncoding) : base(fileName, encoding, detectEncoding)
        {
            _lineNumber = 1;
            _linePosition = 0;
        }

        internal LineNumberStreamReader(Stream stream) : base(stream)
        {
            _lineNumber = 1;
            _linePosition = 0;
        }

        public override int Read()
        {
            int ch = base.Read();
            if (ch != -1)
            {
                _linePosition++;
                if (ch == '\n')
                {
                    _lineNumber++;
                    _linePosition = 0;
                }
            }

            return ch;
        }

        public override int Read([In, Out] char[] chars, int index, int count)
        {
            int r = base.Read(chars, index, count);
            for (int i = 0; i < r; i++)
            {
                if (chars[i + index] == '\n')
                {
                    _lineNumber++;
                    _linePosition = 0;
                }
                else
                {
                    _linePosition++;
                }
            }

            return r;
        }

        public override string ReadLine()
        {
            string s = base.ReadLine();
            if (s is not null)
            {
                _lineNumber++;
                _linePosition = 0;
            }

            return s;
        }

        public override string ReadToEnd() => throw new NotImplementedException();

        internal int LineNumber => _lineNumber;

        internal int LinePosition => _linePosition;
    }
}
