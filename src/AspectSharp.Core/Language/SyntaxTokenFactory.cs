namespace AspectSharp.Core.SyntaxTree
{
    /// <remarks>
    /// We want to share immutable tokens.
    /// </remarks>
    public static class SyntaxTokenFactory
    {
        private static readonly SyntaxToken Dot = new SyntaxToken { TokenKind = SyntaxTokenKind.Dot };
        private static readonly SyntaxToken Not = new SyntaxToken { TokenKind = SyntaxTokenKind.Not };
        private static readonly SyntaxToken LeftP = new SyntaxToken { TokenKind = SyntaxTokenKind.LeftP };
        private static readonly SyntaxToken RightP = new SyntaxToken { TokenKind = SyntaxTokenKind.RightP };
        private static readonly SyntaxToken Plus = new SyntaxToken { TokenKind = SyntaxTokenKind.Plus };
        private static readonly SyntaxToken Minus = new SyntaxToken { TokenKind = SyntaxTokenKind.Minus };
        private static readonly SyntaxToken Times = new SyntaxToken { TokenKind = SyntaxTokenKind.Times };
        private static readonly SyntaxToken Hash = new SyntaxToken { TokenKind = SyntaxTokenKind.Hash };
        private static readonly SyntaxToken And = new SyntaxToken { TokenKind = SyntaxTokenKind.And };
        private static readonly SyntaxToken Or = new SyntaxToken { TokenKind = SyntaxTokenKind.Or };

        private static readonly SyntaxToken Public = new SyntaxToken { TokenKind = SyntaxTokenKind.Public, IdentifierText = "public" };



        public static SyntaxToken MakeIdent(LanguageText text, int offset, int count)
        {
            //todo: check if the text is keyword and return object from cache.

            var lexemeText = text.GetString(offset, count);
            if (Public.IdentifierText.Equals(lexemeText, System.StringComparison.OrdinalIgnoreCase))
            {
                return Public;
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
            }

            return null;
        }
    }
}