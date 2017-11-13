using System;
using System.Linq;
using System.Collections.Generic;

namespace AspectSharp.Core.Language
{
    public class LanguageParser
    {
        private const int FullyQualifiedNameDepth = 125;
        private readonly LanguageLexer lexer;
        private SyntaxToken[] tokens = new SyntaxToken[0];
        private int tokenOffset = 0;

        public LanguageParser(LanguageLexer lexer)
        {
            this.lexer = lexer ?? throw new ArgumentNullException();
        }

        protected SyntaxToken CurrentToken => tokenOffset < tokens.Length ? tokens[tokenOffset] : null;

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

        protected bool Eat(SyntaxTokenKind t)
        {
            var token = CurrentToken;
            if (token?.TokenKind == t)
            {
                AdvanceToken(1);
                return true;
            }

            return false;
        }

        protected bool Eat(SyntaxTokenKind t1, SyntaxTokenKind? t2)
        {
            var token = CurrentToken;
            if (token?.TokenKind == t1)
            {
                token = PeekToken(1);
                if (token?.TokenKind == t2)
                {
                    AdvanceToken(2);
                    return true;
                }
            }

            return false;
        }

        protected bool Probe(SyntaxTokenKind t1)
        {
            return PeekToken(0)?.TokenKind == t1;
        }

        protected bool Probe(SyntaxTokenKind t1, SyntaxTokenKind t2)
        {
            return PeekToken(0)?.TokenKind == t1 && PeekToken(1)?.TokenKind == t2;
        }

        public PointcutSyntax ParsePointcut()
        {
            return ParseMemberPointcut();
        }

        public MemberPointcutSyntax ParseMemberPointcut()
        {
            Lex();

            var visibility = ParseVisibility();
            var memberScope = ParseMemberScope();
            var returnType = ParseTypeName();

            if (Eat(SyntaxTokenKind.Dot, SyntaxTokenKind.New) || Eat(SyntaxTokenKind.Dot, SyntaxTokenKind.Ctor))
            {
                var parameterListSyntax = Probe(SyntaxTokenKind.LeftP) ? ParseParameterList() : ParameterListSyntax.Any;

                return new ConstructorPointcutSyntax
                {
                    Visibility = visibility,
                    Scope = memberScope,
                    Type = returnType,
                    Parameters = parameterListSyntax
                };
            }
            else
            {
                var typeName = ParseTypeName();
                TypeNameSyntax declaredTypeName = null;//ParseTypeName();

                if (Probe(SyntaxTokenKind.Dot, SyntaxTokenKind.GetProperty))
                {
                    return new PropertyPointcutSyntax
                    {
                        Visibility = visibility,
                        Scope = memberScope,
                        Type = typeName,
                        DeclaredType = declaredTypeName,
                        IsGet = true,
                        IsSet = false
                    };
                }
                else if (Eat(SyntaxTokenKind.Dot, SyntaxTokenKind.SetProperty))
                {
                    return new PropertyPointcutSyntax
                    {
                        Visibility = visibility,
                        Type = typeName,
                        DeclaredType = declaredTypeName,
                        IsGet = false,
                        IsSet = true
                    };
                }
                else if (Eat(SyntaxTokenKind.Dot, SyntaxTokenKind.Property))
                {
                    return new PropertyPointcutSyntax
                    {
                        Visibility = visibility,
                        Type = typeName,
                        DeclaredType = declaredTypeName,
                        IsGet = true,
                        IsSet = true
                    };
                }
                /*else if (Probe(SyntaxTokenKind.LeftP))
                {
                    return null; // method def
                }*/
                else
                {
                    return new MemberPointcutSyntax
                    {
                        Visibility = visibility,
                        Scope = memberScope,
                        Type = typeName
                    };
                }

            }
        }

        protected TypeNameSyntax ParseTypeName()
        {
            var parts = ParseFullyQualifiedName();

            if (parts.Count == 1)
            {
                return new TypeNameSyntax(FullyQualifiedNameSyntax.None, parts.First());
            }
            else if (parts.Count == 2)
            {
                var p1 = parts.First();
                var p2 = parts.Last();
                return p2.MatchType == QualifiedNameMatchType.Any
                    ? new TypeNameSyntax(FullyQualifiedNameSyntax.Any, p2)
                    : new TypeNameSyntax(new FullyQualifiedNameSyntax(new[] { p1 }), p2);
            }
            else
            {
                var fqn = new FullyQualifiedNameSyntax(parts.Take(parts.Count - 1).ToArray());
                return new TypeNameSyntax(fqn, parts.Last());
            }
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

        private MemberVisibility ParseVisibility()
        {
            var token = CurrentToken;
            CheckToken(token);

            MemberVisibility visibility;

            switch (token.TokenKind)
            {
                case SyntaxTokenKind.Times:
                    visibility = MemberVisibility.Any;
                    break;
                case SyntaxTokenKind.None:
                case SyntaxTokenKind.Public:
                case SyntaxTokenKind.Plus:
                    visibility = MemberVisibility.Public;
                    break;
                case SyntaxTokenKind.Private:
                case SyntaxTokenKind.Minus:
                    visibility = MemberVisibility.Private;
                    break;
                case SyntaxTokenKind.Protected:
                case SyntaxTokenKind.Hash:
                    visibility = MemberVisibility.Protected;
                    break;
                case SyntaxTokenKind.Internal:
                    visibility = MemberVisibility.Internal;
                    break;
                default:
                    return MemberVisibility.Public;
            }

            if (visibility == MemberVisibility.Protected)
            {
                var nextToken = PeekToken(1);
                if (nextToken?.TokenKind == SyntaxTokenKind.Internal)
                {
                    AdvanceToken(2);
                    return MemberVisibility.ProtectedInternal;
                }
            }

            AdvanceToken(1);
            return visibility;
        }

        private MemberScope ParseMemberScope()
        {
            var token = CurrentToken;
            CheckToken(token);

            switch (token.TokenKind)
            {
                case SyntaxTokenKind.Instance:
                    AdvanceToken(1);
                    return MemberScope.Instance;
                case SyntaxTokenKind.Static:
                    AdvanceToken(1);
                    return MemberScope.Static;
                default:
                    return MemberScope.Any;
            }
        }

        private QualifiedNameSyntax ParseQualifiedName()
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
                        AdvanceToken(1);
                        return QualifiedNameSyntax.Any;
                    }
                    break;

                default:
                    return null;
            }

            return new QualifiedNameSyntax(name, matchType);
        }

        private IReadOnlyCollection<QualifiedNameSyntax> ParseFullyQualifiedName()
        {
            var names = new List<QualifiedNameSyntax>(FullyQualifiedNameDepth);

            do
            {
                var qualifiedName = ParseQualifiedName();
                if (qualifiedName != null)
                {
                    names.Add(qualifiedName);
                    var token = CurrentToken;

                    if (token?.TokenKind == SyntaxTokenKind.Dot)
                    {
                        AdvanceToken(1);
                        continue;
                    }
                }
                else if (names.Count > 0)
                {
                    AdvanceToken(-1);
                }

                break;
            }
            while (true);

            return names;
        }

        private ParameterListSyntax ParseParameterList()
        {
            var token = CurrentToken;
            CheckToken(token, new[] { SyntaxTokenKind.LeftP });

            if (Eat(SyntaxTokenKind.LeftP))
            {
                if (Eat(SyntaxTokenKind.RightP)) return ParameterListSyntax.Empty;
                if (Eat(SyntaxTokenKind.Dot))
                {
                    if (Eat(SyntaxTokenKind.Dot))
                    {
                        if (Eat(SyntaxTokenKind.RightP)) return ParameterListSyntax.Any;
                        throw new Exception($"Expected {SyntaxTokenKind.RightP} token");
                    }
                    else
                    {
                        throw new Exception($"Expected {SyntaxTokenKind.Dot} token");
                    }
                }

                var parameterList = new List<ParameterSyntax>();

                do
                {
                    if (TryParseParameterModifier(CurrentToken, out ParameterModifier? parameterModifier))
                    {
                        AdvanceToken(1);
                    }

                    var typeName = ParseTypeName();

                    parameterList.Add(new ParameterSyntax(parameterModifier ?? ParameterModifier.In, typeName));

                    if (Eat(SyntaxTokenKind.RightP))
                    {
                        break;
                    }
                    else if (Eat(SyntaxTokenKind.Comma))
                    {
                        continue;
                    }
                } while (true);

                return new ParameterListSyntax(parameterList);
            }

            throw new Exception("Expected token is ");
        }

        private bool TryParseParameterModifier(SyntaxToken token, out ParameterModifier? parameterModifier)
        {
            if (token?.TokenKind == SyntaxTokenKind.Out)
            {
                parameterModifier = ParameterModifier.Out;
                return true;
            }
            else if (token?.TokenKind == SyntaxTokenKind.Ref)
            {
                parameterModifier = ParameterModifier.Ref;
                return true;
            }

            parameterModifier = null;
            return false;
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