using System;

namespace AspectSharp.Core.SyntaxTree
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
            return text[Offset];
        }

        public void Advance(int count)
        {
            Offset += count;

            if (Offset >= CharacterCount) Offset = CharacterCount;
        }

        public int CharacterCount => text.Length;

        public int Offset { get; private set; }

        public char this[int index]
        {
            get
            {
                return index >= CharacterCount ? '\0' : text[index];
            }
        }

        public string GetString(int offset, int count)
        {
            return text.Substring(offset, count);
        }
    }
}