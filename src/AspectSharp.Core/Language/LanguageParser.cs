using System;
using System.Linq;
using System.Collections.Generic;

namespace AspectSharp.Core.Language
{
    public class LanguageParser
    {
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

        protected bool EatToken(SyntaxTokenKind t)
        {
            var token = CurrentToken;
            if (token?.TokenKind == t)
            {
                AdvanceToken(1);
                return true;
            }

            return false;
        }

        protected bool EatToken(SyntaxTokenKind t1, SyntaxTokenKind? t2)
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

        protected int EatWhiteTokens()
        {
            var c = 0;
            for (; EatToken(SyntaxTokenKind.White); c++) ;
            return c;
        }

        protected bool ProbeToken(SyntaxTokenKind t1)
        {
            return PeekToken(0)?.TokenKind == t1;
        }

        protected bool ProbeToken(SyntaxTokenKind t1, SyntaxTokenKind t2)
        {
            return PeekToken(0)?.TokenKind == t1 && PeekToken(1)?.TokenKind == t2;
        }

        public PointcutSyntax ParsePointcut()
        {
            return ParseMemberPointcut();
        }

        public MemberPointcutSyntax ParseMemberPointcut()
        {
            Lex(false);

            var visibility = ParseVisibility();
            var memberScope = ParseMemberScope();
            var returnTypeParts = ParseQualifiedName();

            if (EatToken(SyntaxTokenKind.Dot, SyntaxTokenKind.New) || EatToken(SyntaxTokenKind.Dot, SyntaxTokenKind.Ctor))
            {
                var parameterListSyntax = ProbeToken(SyntaxTokenKind.LeftP) ? ParseParameterList() : ParameterListSyntax.Any;

                return new ConstructorPointcutSyntax
                {
                    Visibility = visibility,
                    Scope = memberScope,
                    DeclaredType = TypeNameSyntax.Create(returnTypeParts),
                    Parameters = parameterListSyntax
                };
            }
            else
            {
                var declaredTypeParts = ParseQualifiedName();

                TypeNameSyntax returnTypeName;

                if (declaredTypeParts.Any())
                {
                    returnTypeName = TypeNameSyntax.Create(returnTypeParts);
                }
                else
                {
                    declaredTypeParts = returnTypeParts;
                    returnTypeName = TypeNameSyntax.None;
                }

                if (ProbeToken(SyntaxTokenKind.Dot, SyntaxTokenKind.GetProperty))
                {
                    Explode(declaredTypeParts, out var declaredTypeName, out var propertyName);
                    return new PropertyPointcutSyntax
                    {
                        Visibility = visibility,
                        Scope = memberScope,
                        PropertyType = returnTypeName,
                        DeclaredType = declaredTypeName,
                        Name = propertyName,
                        IsGet = true,
                        IsSet = false
                    };
                }
                else if (EatToken(SyntaxTokenKind.Dot, SyntaxTokenKind.SetProperty))
                {
                    Explode(declaredTypeParts, out var declaredTypeName, out var propertyName);
                    return new PropertyPointcutSyntax
                    {
                        Visibility = visibility,
                        PropertyType = returnTypeName,
                        DeclaredType = declaredTypeName,
                        Name = propertyName,
                        IsGet = false,
                        IsSet = true
                    };
                }
                else if (EatToken(SyntaxTokenKind.Dot, SyntaxTokenKind.Property))
                {
                    Explode(declaredTypeParts, out var declaredTypeName, out var propertyName);
                    return new PropertyPointcutSyntax
                    {
                        Visibility = visibility,
                        PropertyType = returnTypeName,
                        DeclaredType = declaredTypeName,
                        Name = propertyName,
                        IsGet = true,
                        IsSet = true
                    };
                }
                else if (ProbeToken(SyntaxTokenKind.LeftP))
                {
                    var parameterListSyntax = ParseParameterList();
                    Explode(declaredTypeParts, out var declaredTypeName, out var methodName);

                    return new MethodPointcutSyntax
                    {
                        Visibility = visibility,
                        Scope = memberScope,
                        DeclaredType = declaredTypeName,
                        MethodName = methodName,
                        ReturnType = returnTypeName,
                    };
                }
                else
                {
                    return new MemberPointcutSyntax
                    {
                        Visibility = visibility,
                        Scope = memberScope,
                        DeclaredType = TypeNameSyntax.Create(returnTypeParts)
                    };
                }
            }
        }

        private void Explode(IReadOnlyCollection<IdentifierNameSyntax> parts, out TypeNameSyntax typeName, out IdentifierNameSyntax identifierName)
        {
            identifierName = parts.Last();
            typeName = TypeNameSyntax.Create(parts.Take(parts.Count - 1).ToArray());
        }

        protected TypeNameSyntax ParseTypeName()
        {
            return TypeNameSyntax.Create(ParseQualifiedName());
        }

        private void Lex(bool skipeWhite = true)
        {
            var tokenList = new List<SyntaxToken>();

            for (var token = lexer.GetNextToken(); true; token = lexer.GetNextToken())
            {
                if (token?.TokenKind == SyntaxTokenKind.EndOfText) break;

                if (skipeWhite && token.TokenKind == SyntaxTokenKind.White) continue;

                tokenList.Add(token);
            }

            tokens = tokenList.ToArray();
        }

        private MemberVisibility ParseVisibility()
        {
            EatWhiteTokens();

            var token = CurrentToken;
            CheckToken(token);

            MemberVisibility visibility;

            switch (token.TokenKind)
            {
                case SyntaxTokenKind.Times:
                    if (PeekToken(1)?.TokenKind != SyntaxTokenKind.White) return MemberVisibility.Public;
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
                AdvanceToken(1);
                var whiteTokens = EatWhiteTokens();
                
                if (ProbeToken(SyntaxTokenKind.Internal))
                {
                    AdvanceToken(1);
                    visibility = MemberVisibility.ProtectedInternal;
                }
                else
                {
                    AdvanceToken(-whiteTokens);
                }
            }
            else
            {
                AdvanceToken(1);
            }

            return visibility;
        }

        private MemberScope ParseMemberScope()
        {
            EatWhiteTokens();

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

        private IdentifierNameSyntax ParseIdentifierName()
        {
            var token = CurrentToken;
            CheckToken(token, new[] { SyntaxTokenKind.Identifier, SyntaxTokenKind.Times });

            IdentifierNameMatchType matchType;
            string name;

            switch (token.TokenKind)
            {
                case SyntaxTokenKind.Identifier:
                    name = token.ValueText;
                    token = PeekToken(1);
                    if (token?.TokenKind == SyntaxTokenKind.Times)
                    {
                        AdvanceToken(2);
                        matchType = IdentifierNameMatchType.StartsWith;
                    }
                    else
                    {
                        AdvanceToken(1);
                        matchType = IdentifierNameMatchType.Strict;
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
                            matchType = IdentifierNameMatchType.Contains;
                        }
                        else
                        {
                            AdvanceToken(2);
                            matchType = IdentifierNameMatchType.EndsWith;
                        }
                    }
                    else
                    {
                        AdvanceToken(1);
                        return IdentifierNameSyntax.Any;
                    }
                    break;

                default:
                    return null;
            }

            return IdentifierNameSyntax.Create(name, matchType);
        }

        private IReadOnlyCollection<IdentifierNameSyntax> ParseQualifiedName()
        {
            EatWhiteTokens();

            try
            {
                var identifierName = ParseIdentifierName();

                if (identifierName == null) return QualifiedNameSyntax.None;

                var names = new List<IdentifierNameSyntax>(QualifiedNameSyntax.QualifiedNameDepth);

                while (true)
                {
                    names.Add(identifierName);

                    if (EatToken(SyntaxTokenKind.Dot))
                    {
                        identifierName = ParseIdentifierName();
                        if (identifierName != null) continue;
                        AdvanceToken(-1);
                    }

                    break;
                }

                return names;
            }
            finally
            {
                EatWhiteTokens();
            }
        }

        private ParameterListSyntax ParseParameterList()
        {
            EatWhiteTokens();

            var token = CurrentToken;
            CheckToken(token, new[] { SyntaxTokenKind.LeftP });

            if (EatToken(SyntaxTokenKind.LeftP))
            {
                EatWhiteTokens();
                if (EatToken(SyntaxTokenKind.RightP)) return ParameterListSyntax.Empty;
                if (EatToken(SyntaxTokenKind.Dot))
                {
                    EatWhiteTokens();
                    if (EatToken(SyntaxTokenKind.Dot))
                    {
                        EatWhiteTokens();
                        if (EatToken(SyntaxTokenKind.RightP)) return ParameterListSyntax.Any;
                        throw new Exception($"Expected {SyntaxTokenKind.RightP} token");
                    }
                    else
                    {
                        throw new Exception($"Expected {SyntaxTokenKind.Dot} token");
                    }
                }

                var parameterList = new List<ParameterSyntax>();

                while(true)
                {
                    EatWhiteTokens();
                    if (TryParseParameterModifier(CurrentToken, out ParameterModifier? parameterModifier))
                    {
                        AdvanceToken(1);
                        EatWhiteTokens();
                    }

                    var typeName = ParseTypeName();

                    parameterList.Add(new ParameterSyntax(parameterModifier ?? ParameterModifier.In, typeName));

                    if (EatToken(SyntaxTokenKind.RightP))
                    {
                        break;
                    }
                    else if (EatToken(SyntaxTokenKind.Comma))
                    {
                        continue;
                    }
                } 

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