using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AspectSharp.Core.Language
{
    public class TypeRef
    {
        public string Namespace { get; set; }

        public string Class { get; set; }

        public string Name { get; set; }
    }

    public enum QualifiedNameMatchType
    {
        Any,
        Strict,
        StartsWith,
        EndsWith,
        Contains
    }

    [DebuggerDisplay("MatchType={MatchType} Name={Name}")]
    public class QualifiedName
    {
        public QualifiedNameMatchType MatchType { get; set; }

        public string Name { get; set; }
    }

    public class FullyQualifiedName
    {

    }

    public class PointcutDef
    {
        public AccessibilityToken Accessibility { get; internal set; }

        public MemberScopeToken MemberScope { get; set; }

        public QualifiedName Type { get; set; }

        public TypeRef ReturnType { get; set; }
    }

    public enum PointcutDefKind
    {
        Any,
        Member,
        Constructor,
        Property,
        GetProperty,
        SetProperty,
        //?Field
    }

    public class LanguageParser
    {
        private readonly LanguageLexer lexer;
        private SyntaxToken[] tokens = new SyntaxToken[0];
        private int tokenOffset = 0;

        protected SyntaxToken CurrentToken => tokens[tokenOffset];

        protected SyntaxToken PeekToken(int n = 0)
        {
            if (tokenOffset + n < tokens.Length)
            {
                return tokens[tokenOffset + n];
            }
            else
            {
                return null;
            }
        }

        protected void AdvanceToken(int n = 1) => tokenOffset += n;

        public LanguageParser(LanguageLexer lexer)
        {
            this.lexer = lexer ?? throw new ArgumentNullException();
        }

        public PointcutDef ParsePointcut()
        {
            Lex();

            var accessibility = ParseAccessibility();
            var memberScope = ParseMemberScope();
            var name = ParseQualifiedName();

            return new PointcutDef
            {
                Accessibility = accessibility,
                MemberScope = memberScope,
                Type = name
            };
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
            CheckToken(token);

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

        private MemberScopeToken ParseMemberScope()
        {
            var token = CurrentToken;
            CheckToken(token);

            switch (token.TokenKind)
            {
                case SyntaxTokenKind.Instance:
                    AdvanceToken(1);
                    return MemberScopeToken.Instance;
                case SyntaxTokenKind.Static:
                    AdvanceToken(1);
                    return MemberScopeToken.Static;
                default:
                    return MemberScopeToken.Any;
            }
        }

        private QualifiedName ParseQualifiedName()
        {
            var token = CurrentToken;
            CheckToken(token, new[] { SyntaxTokenKind.Identifier, SyntaxTokenKind.Times });

            QualifiedNameMatchType matchType;
            string name;

            switch (token.TokenKind)
            {
                case SyntaxTokenKind.Identifier:
                    name = token.ValueText;
                    token = PeekToken(1);
                    if (token?.TokenKind == SyntaxTokenKind.Times)
                    {
                        AdvanceToken(2);
                        matchType = QualifiedNameMatchType.StartsWith;
                    }
                    else
                    {
                        AdvanceToken(1);
                        matchType = QualifiedNameMatchType.Strict;
                    }
                    break;

                case SyntaxTokenKind.Times:
                    token = PeekToken(1);
                    if (token?.TokenKind == SyntaxTokenKind.Identifier)
                    {
                        name = token.ValueText;
                        token = PeekToken(2);
                        if (token?.TokenKind == SyntaxTokenKind.Times)
                        {
                            AdvanceToken(3);
                            matchType = QualifiedNameMatchType.Contains;
                        }
                        else
                        {
                            AdvanceToken(2);
                            matchType = QualifiedNameMatchType.EndsWith;
                        }
                    }
                    else
                    {
                        AdvanceToken(2);
                        matchType = QualifiedNameMatchType.Any;
                        name = null;
                    }
                    break;

                default:
                    return null;
            }

            return new QualifiedName { MatchType = matchType, Name = name };
        }

        private static void CheckToken(SyntaxToken token, SyntaxTokenKind[] allowedTokens = null, SyntaxTokenKind[] expectedTokens = null)
        {
            if (token == null)
            {
                //TODO: bad token
            }
        }

    }
}