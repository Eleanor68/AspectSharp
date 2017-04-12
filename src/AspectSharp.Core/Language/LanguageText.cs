using System;
using System.Diagnostics;

namespace AspectSharp.Core.Language
{
    public class LanguageText
    {
        private readonly string text;

        public LanguageText(string text)
        {
            if (string.IsNullOrEmpty(text)) throw new ArgumentException(nameof(text));

            this.text = text;
        }

        public char Peek()
        {
            return this[Offset];
        }

        public void Advance(int count)
        {
            Offset += count;

            if (Offset >= Length) Offset = Length;
        }

        public int Length => text.Length;

        public int Offset { get; private set; }

        public char this[int index]
        {
            get
            {
                return index >= Length ? '\0' : text[index];
            }
        }

        public string GetString(int offset, int count)
        {
            return text.Substring(offset, count);
        }
    }
}