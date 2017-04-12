using System;
using System.Collections.Generic;

namespace AspectSharp.Core.Language
{
    public class TypeReference
    {
        public string Namespace { get; set; }

        public string Class { get; set; }

        public string Name { get; set; }
    }

    public class PointcutDefinition
    {
        public AccessibilityToken Accessibility { get; internal set; }

        public TypeReference Type { get; private set; }

        public TypeReference ReturnType { get; set; }
    }

    public class LanguageParser
    {
        private readonly LanguageLexer lexer;
        private SyntaxToken[] tokens = new SyntaxToken[0];
        private int tokenOffset = 0;

        protected SyntaxToken CurrentToken => tokens[tokenOffset];

        protected SyntaxToken PeekToken(int n = 0) => tokens[tokenOffset + n];

        protected void AdvanceToken(int n = 1) => tokenOffset += n;

        public LanguageParser(LanguageLexer lexer)
        {
            this.lexer = lexer ?? throw new ArgumentNullException();
        }

        public PointcutDefinition ParsePointcut()
        {
            Lex();

            var accessibility = ParseAccessibility();
            return new PointcutDefinition { Accessibility = accessibility };
        }

        private void Lex()
        {
            var tokenList = new List<SyntaxToken>();

            for (var token = lexer.GetNextToken(); true; token = lexer.GetNextToken())
            {
                if (token?.TokenKind == SyntaxTokenKind.EndOfText) break;

                if (token.TokenKind == SyntaxTokenKind.White) continue;

                tokenList.Add(token);
            }

            tokens = tokenList.ToArray();
        }

        private AccessibilityToken ParseAccessibility()
        {
            var token = CurrentToken;
            if (token == null)
            {
                //TODO: bad token
            }

            AccessibilityToken accessibility;

            switch (token.TokenKind)
            {
                case SyntaxTokenKind.Times:
                    accessibility = AccessibilityToken.Any;
                    break;
                case SyntaxTokenKind.None:
                case SyntaxTokenKind.Public:
                case SyntaxTokenKind.Plus:
                    accessibility = AccessibilityToken.Public;
                    break;
                case SyntaxTokenKind.Private:
                case SyntaxTokenKind.Minus:
                    accessibility = AccessibilityToken.Private;
                    break;
                case SyntaxTokenKind.Protected:
                case SyntaxTokenKind.Hash:
                    accessibility = AccessibilityToken.Protected;
                    break;
                case SyntaxTokenKind.Internal:
                    accessibility = AccessibilityToken.Internal;
                    break;
                default:
                    return AccessibilityToken.Public;
            }

            if (accessibility == AccessibilityToken.Protected)
            {
                var nextToken = PeekToken(1);
                if (nextToken?.TokenKind == SyntaxTokenKind.Internal)
                {
                    AdvanceToken(2);
                    return AccessibilityToken.ProtectedInternal;
                }
            }

            AdvanceToken(1);
            return accessibility;
        }
    }
}