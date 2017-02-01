using System;
using System.Runtime.InteropServices;

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
    }

    public class SyntaxToken
    {
        public string IdentifierText { get; set; }

        public SyntaxTokenKind TokenKind { get; set; }
    }

    /// <remarks>
    /// We want to share immutable tokens.
    /// </remarks>
    public static class SyntaxTokenFactory
    {
        public static SyntaxToken MakeIdent(LanguageText text, int offset, int count)
        {
            //todo: check if the text is keyword and return object from cache.
            return new SyntaxToken { TokenKind = SyntaxTokenKind.Identifier };
        }

        public static SyntaxToken MakePunct(char c)
        {
            //todo: check if the char is known and return object from cache.
            switch (c)
            {
                case '.' : return new SyntaxToken { TokenKind = SyntaxTokenKind.Dot };
            }

            return null;
        }
    }

    public enum SyntaxTokenKind
    {
        None,
        Identifier,
        Dot
    }

    public class LanguageLexer
    {
        private enum State : byte
        { 
            Initial = 0,
            Ident,
            IdentDone,
            PunctDone,
            Bad
        }

        private enum CharMark : byte
        {
            Letter,
            Digit,
            Punct,
            White,
            EndOfText,
            Unknown
        }

        private static readonly byte[] charMarkers;

        private static byte[,] transitions = new byte[,] 
        {
            //Initial
            {   
                (byte)State.Ident,      //Letter
                (byte)State.Bad,        //Digit
                (byte)State.PunctDone,  //Punct
                (byte)State.Initial,    //White
                (byte)State.Bad,        //EndOfText
                (byte)State.Bad         //Unknown
            },
            //Ident
            {
                (byte)State.Ident,      //Letter
                (byte)State.Ident,      //Digit
                (byte)State.IdentDone,  //Punct
                (byte)State.IdentDone,  //White
                (byte)State.IdentDone,  //EndOfText
                (byte)State.Bad         //Unknown
            },
        };

        private LanguageText languageText;

        static LanguageLexer()
        {
            charMarkers = new byte[255];
            for (var i = 0; i < charMarkers.Length; i++)
            {
                charMarkers[i] = (byte)CharMark.Unknown;
            }

            charMarkers[0] = (byte)CharMark.EndOfText;

            for (var i = (byte)'A'; i <= (byte)'Z' ; i++)
            {
                charMarkers[i] = (byte)CharMark.Letter;
                charMarkers[i + 32] = (byte)CharMark.Letter;
            }

            charMarkers[(byte)'_'] = (byte)CharMark.Letter;

            for (var i = (byte)'0'; i <= (byte)'9'; i++)
            {
                charMarkers[i] = (byte)CharMark.Digit;
            }

            charMarkers[(byte)' '] = (byte)CharMark.White;
            charMarkers[(byte)'\r'] = (byte)CharMark.White;
            charMarkers[(byte)'\n'] = (byte)CharMark.White;
            charMarkers[(byte)'\t'] = (byte)CharMark.White;

            charMarkers[(byte)'.'] = (byte)CharMark.Punct;
        }


        public LanguageLexer(LanguageText languageText)
        {
            this.languageText = languageText ?? throw new ArgumentNullException(nameof(languageText));
        }

        public SyntaxToken GetNextToken()
        {
            var state = State.Initial;
            var n = languageText.CharacterCount;
            int i = languageText.Offset, lexemeStart = i;

            for (; i <= n; i++)
            {
                var c = languageText[i];

                var charMark = charMarkers[(byte)c];

                state = (State)transitions[(byte)state, charMark];

                if (state >= State.IdentDone) break;
            }

            if (state == State.IdentDone)
            {
                return SyntaxTokenFactory.MakeIdent(languageText, lexemeStart, i - lexemeStart);
            }

            if (state == State.PunctDone)
            {
                languageText.Advance(i);

                return SyntaxTokenFactory.MakePunct(languageText.Peek());
            }

            return null;
        }
    }
}