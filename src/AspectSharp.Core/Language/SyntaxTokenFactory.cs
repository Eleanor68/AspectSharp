namespace AspectSharp.Core.Language
{
    /// <remarks>
    /// We want to share immutable tokens.
    /// </remarks>
    public static class SyntaxTokenFactory
    {
        private static readonly SyntaxToken Dot = new SyntaxToken { TokenKind = SyntaxTokenKind.Dot, ValueText = "." };
        private static readonly SyntaxToken Comma = new SyntaxToken { TokenKind = SyntaxTokenKind.Comma, ValueText = "," };
        private static readonly SyntaxToken Not = new SyntaxToken { TokenKind = SyntaxTokenKind.Not, ValueText = "!" };
        private static readonly SyntaxToken LeftP = new SyntaxToken { TokenKind = SyntaxTokenKind.LeftP, ValueText = "(" };
        private static readonly SyntaxToken RightP = new SyntaxToken { TokenKind = SyntaxTokenKind.RightP, ValueText = ")" };
        private static readonly SyntaxToken Plus = new SyntaxToken { TokenKind = SyntaxTokenKind.Plus, ValueText = "+" };
        private static readonly SyntaxToken Minus = new SyntaxToken { TokenKind = SyntaxTokenKind.Minus, ValueText = "-" };
        private static readonly SyntaxToken Times = new SyntaxToken { TokenKind = SyntaxTokenKind.Times, ValueText = "*" };
        private static readonly SyntaxToken Hash = new SyntaxToken { TokenKind = SyntaxTokenKind.Hash, ValueText = "#" };
        private static readonly SyntaxToken And = new SyntaxToken { TokenKind = SyntaxTokenKind.And, ValueText = "&" };
        private static readonly SyntaxToken Or = new SyntaxToken { TokenKind = SyntaxTokenKind.Or, ValueText = "|" };
        private static readonly SyntaxToken EndOfText = new SyntaxToken { TokenKind = SyntaxTokenKind.EndOfText };
        private static readonly SyntaxToken White = new SyntaxToken { TokenKind = SyntaxTokenKind.White, ValueText = " " };

        private static readonly SyntaxToken[] Keywords = new []
        {
            new SyntaxToken { TokenKind = SyntaxTokenKind.Public, ValueText = "public" },
            new SyntaxToken { TokenKind = SyntaxTokenKind.Private, ValueText = "private" },
            new SyntaxToken { TokenKind = SyntaxTokenKind.Protected, ValueText = "protected" },
            new SyntaxToken { TokenKind = SyntaxTokenKind.Internal, ValueText = "internal" },
            new SyntaxToken { TokenKind = SyntaxTokenKind.Instance, ValueText = "instance" },
            new SyntaxToken { TokenKind = SyntaxTokenKind.Static, ValueText = "static" },
            new SyntaxToken { TokenKind = SyntaxTokenKind.Class, ValueText = "class" },
            new SyntaxToken { TokenKind = SyntaxTokenKind.New, ValueText = "new" },
            new SyntaxToken { TokenKind = SyntaxTokenKind.Ctor, ValueText = "ctor" },
            new SyntaxToken { TokenKind = SyntaxTokenKind.GetProperty, ValueText = "get" },
            new SyntaxToken { TokenKind = SyntaxTokenKind.SetProperty, ValueText = "set" },
            new SyntaxToken { TokenKind = SyntaxTokenKind.Out, ValueText = "out" },
            new SyntaxToken { TokenKind = SyntaxTokenKind.Ref, ValueText = "ref" }
        };

        public static SyntaxToken MakeIdent(LanguageText text, int offset, int count)
        {
            var lexemeText = text.GetString(offset, count);

            if (string.IsNullOrEmpty(lexemeText)) return EndOfText;

            for (int i = 0; i < Keywords.Length; i++)
            {
                var keyword = Keywords[i];
                if (keyword.ValueText.Equals(lexemeText, System.StringComparison.OrdinalIgnoreCase))
                {
                    return keyword;
                }
            }

            return new SyntaxToken { TokenKind = SyntaxTokenKind.Identifier, ValueText = lexemeText };
        }

        public static SyntaxToken MakePunct(char c)
        {
            //todo: check if the char is known and return object from cache.
            switch (c)
            {
                case '.': return Dot;
                case ',': return Comma;
                case '!': return Not;
                case '(': return LeftP;
                case ')': return RightP;
                case '+': return Plus;
                case '-': return Minus;
                case '*': return Times;
                case '#': return Hash;
                case '&': return And;
                case '|': return Or;
                case '\0': return EndOfText;
            }

            return null;
        }

        public static SyntaxToken MakeWhite() => White;
    }
}