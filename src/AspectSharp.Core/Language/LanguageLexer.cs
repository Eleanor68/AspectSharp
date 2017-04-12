using System;

namespace AspectSharp.Core.Language
{
    public class LanguageLexer
    {
        private enum State : byte
        {
            Initial = 0,
            Ident,
            White,
            IdentDone,
            WhiteDone,
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

        //Is it really faster?
        private static readonly byte[] charMarkers = new byte[128]
        {
            //0..9
            (byte)CharMark.EndOfText, (byte)CharMark.Unknown, (byte)CharMark.Unknown, (byte)CharMark.Unknown, (byte)CharMark.Unknown, (byte)CharMark.Unknown, (byte)CharMark.Unknown, (byte)CharMark.Unknown, (byte)CharMark.Unknown, (byte)CharMark.White,
            //10..13
            (byte)CharMark.White, (byte)CharMark.White, (byte)CharMark.White, (byte)CharMark.White, (byte)CharMark.Unknown,
            //14..31
            (byte)CharMark.Unknown, (byte)CharMark.Unknown, (byte)CharMark.Unknown, (byte)CharMark.Unknown, (byte)CharMark.Unknown, (byte)CharMark.Unknown, (byte)CharMark.Unknown, (byte)CharMark.Unknown, (byte)CharMark.Unknown,
            (byte)CharMark.Unknown, (byte)CharMark.Unknown, (byte)CharMark.Unknown, (byte)CharMark.Unknown, (byte)CharMark.Unknown, (byte)CharMark.Unknown, (byte)CharMark.Unknown, (byte)CharMark.Unknown,
            //32..39
            (byte)CharMark.White, (byte)CharMark.Punct, (byte)CharMark.Unknown, (byte)CharMark.Punct, (byte)CharMark.Unknown, (byte)CharMark.Unknown, (byte)CharMark.Punct, (byte)CharMark.Unknown,
            //40..47
            (byte)CharMark.Punct, (byte)CharMark.Punct, (byte)CharMark.Punct, (byte)CharMark.Punct, (byte)CharMark.Unknown, (byte)CharMark.Punct, (byte)CharMark.Punct, (byte)CharMark.Unknown,
            //48..57
            (byte)CharMark.Digit, (byte)CharMark.Digit, (byte)CharMark.Digit, (byte)CharMark.Digit, (byte)CharMark.Digit, (byte)CharMark.Digit, (byte)CharMark.Digit, (byte)CharMark.Digit, (byte)CharMark.Digit, (byte)CharMark.Digit,
            //58..64
            (byte)CharMark.Unknown, (byte)CharMark.Unknown, (byte)CharMark.Unknown, (byte)CharMark.Unknown, (byte)CharMark.Unknown, (byte)CharMark.Unknown, (byte)CharMark.Unknown,
            //65..90
            (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter,
            (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter,
            (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter,
            //91..96
            (byte)CharMark.Unknown, (byte)CharMark.Unknown, (byte)CharMark.Unknown, (byte)CharMark.Unknown, (byte)CharMark.Letter, (byte)CharMark.Unknown,
            //97..122
            (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter,
            (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter,
            (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter, (byte)CharMark.Letter,
            //123..127
            (byte)CharMark.Unknown, (byte)CharMark.Punct, (byte)CharMark.Unknown, (byte)CharMark.Unknown, (byte)CharMark.Unknown
        };

        private static readonly byte[,] transitions = 
        {
            //Initial
            {
                (byte)State.Ident,      //Letter
                (byte)State.Bad,        //Digit
                (byte)State.PunctDone,  //Punct
                (byte)State.White,      //White
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
            //White
            {
                (byte)State.WhiteDone,  //Letter
                (byte)State.WhiteDone,  //Digit
                (byte)State.WhiteDone,  //Punct
                (byte)State.White,      //White
                (byte)State.WhiteDone,  //EndOfText
                (byte)State.Bad         //Unknown
            },
        };

        private LanguageText languageText;

        /*static LanguageLexer()
        {
            charMarkers = new byte[255];
            for (var i = 0; i < charMarkers.Length; i++)
            {
                charMarkers[i] = (byte)CharMark.Unknown;
            }

            charMarkers[0] = (byte)CharMark.EndOfText;

            for (var i = (byte)'A'; i <= (byte)'Z'; i++)
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
        }*/


        public LanguageLexer(LanguageText languageText)
        {
            this.languageText = languageText ?? throw new ArgumentNullException(nameof(languageText));
        }

        public SyntaxToken GetNextToken()
        {
            var state = State.Initial;
            int n = languageText.Length, i = languageText.Offset, lexemeStart = i;

            if (i >= n)
            {
                return SyntaxTokenFactory.MakePunct('\0');
            }

            do
            {
                char c = '\0';

                if (i < n)
                {
                    c = languageText[i];
                }

                var charMark = charMarkers[(byte)c];

                state = (State)transitions[(byte)state, charMark];

                if (state >= State.IdentDone) break;

                i++;

            } while (i <= n);

            if (state == State.IdentDone)
            {
                languageText.Advance(i - lexemeStart);
                return SyntaxTokenFactory.MakeIdent(languageText, lexemeStart, i - lexemeStart);
            }

            if (state == State.PunctDone)
            {
                var p = languageText.Peek();
                languageText.Advance(1);
                return SyntaxTokenFactory.MakePunct(p);
            }

            if (state == State.WhiteDone)
            {
                languageText.Advance(i - lexemeStart);
                return SyntaxTokenFactory.MakeWhite();
            }

            return null;
        }
    }
}