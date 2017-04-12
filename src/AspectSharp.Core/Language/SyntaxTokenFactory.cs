namespace AspectSharp.Core.Language
{
    /// <remarks>
    /// We want to share immutable tokens.
    /// </remarks>
    public static class SyntaxTokenFactory
    {
        private static readonly SyntaxToken Dot = new SyntaxToken { TokenKind = SyntaxTokenKind.Dot, IdentifierText = "." };
        private static readonly SyntaxToken Not = new SyntaxToken { TokenKind = SyntaxTokenKind.Not, IdentifierText = "!" };
        private static readonly SyntaxToken LeftP = new SyntaxToken { TokenKind = SyntaxTokenKind.LeftP, IdentifierText = "(" };
        private static readonly SyntaxToken RightP = new SyntaxToken { TokenKind = SyntaxTokenKind.RightP, IdentifierText = ")" };
        private static readonly SyntaxToken Plus = new SyntaxToken { TokenKind = SyntaxTokenKind.Plus, IdentifierText = "+" };
        private static readonly SyntaxToken Minus = new SyntaxToken { TokenKind = SyntaxTokenKind.Minus, IdentifierText = "-" };
        private static readonly SyntaxToken Times = new SyntaxToken { TokenKind = SyntaxTokenKind.Times, IdentifierText = "*" };
        private static readonly SyntaxToken Hash = new SyntaxToken { TokenKind = SyntaxTokenKind.Hash, IdentifierText = "#" };
        private static readonly SyntaxToken And = new SyntaxToken { TokenKind = SyntaxTokenKind.And, IdentifierText = "&" };
        private static readonly SyntaxToken Or = new SyntaxToken { TokenKind = SyntaxTokenKind.Or, IdentifierText = "|" };
        private static readonly SyntaxToken EndOfText = new SyntaxToken { TokenKind = SyntaxTokenKind.EndOfText };
        private static readonly SyntaxToken White = new SyntaxToken { TokenKind = SyntaxTokenKind.White, IdentifierText = " " };

        private static readonly SyntaxToken[] Keywords = new []
        {
            new SyntaxToken { TokenKind = SyntaxTokenKind.Public, IdentifierText = "public" },
            new SyntaxToken { TokenKind = SyntaxTokenKind.Private, IdentifierText = "private" },
            new SyntaxToken { TokenKind = SyntaxTokenKind.Protected, IdentifierText = "protected" },
            new SyntaxToken { TokenKind = SyntaxTokenKind.Internal, IdentifierText = "internal" },
            new SyntaxToken { TokenKind = SyntaxTokenKind.Instance, IdentifierText = "instance" },
            new SyntaxToken { TokenKind = SyntaxTokenKind.Static, IdentifierText = "static" },
            new SyntaxToken { TokenKind = SyntaxTokenKind.Class, IdentifierText = "class" },
            new SyntaxToken { TokenKind = SyntaxTokenKind.Ctor, IdentifierText = "ctor" },
            new SyntaxToken { TokenKind = SyntaxTokenKind.GetProperty, IdentifierText = "get" },
            new SyntaxToken { TokenKind = SyntaxTokenKind.SetProperty, IdentifierText = "set" },
        };

        public static SyntaxToken MakeIdent(LanguageText text, int offset, int count)
        {
            var lexemeText = text.GetString(offset, count);

            if (string.IsNullOrEmpty(lexemeText)) return EndOfText;

            for (int i = 0; i < Keywords.Length; i++)
            {
                var keyword = Keywords[i];
                if (keyword.IdentifierText.Equals(lexemeText, System.StringComparison.OrdinalIgnoreCase))
                {
                    return keyword;
                }
            }

            return new SyntaxToken { TokenKind = SyntaxTokenKind.Identifier, IdentifierText = lexemeText };
        }

        public static SyntaxToken MakePunct(char c)
        {
            //todo: check if the char is known and return object from cache.
            switch (c)
            {
                case '.': return Dot;
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