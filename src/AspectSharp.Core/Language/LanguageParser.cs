using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace AspectSharp.Core.Language
{
    public class TypeName
    {
        public TypeName(FullyQualifiedName @namespace, QualifiedName name)
        {
            Namespace = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public FullyQualifiedName Namespace { get; set; }

        public QualifiedName Name { get; set; }

        //public static TypeName Any { get; } = new TypeName(FullyQualifiedName.Any, QualifiedName.Any);
    }

    public class MethodDef
    {
        public TypeName Type { get; set; }
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
        public QualifiedName(string name, QualifiedNameMatchType matchType)
        {
            Name = name;
            MatchType = matchType;
        }

        public QualifiedNameMatchType MatchType { get; }

        public string Name { get; }

        public static QualifiedName Any { get; } = new QualifiedName(null, QualifiedNameMatchType.Any);
    }

    public class FullyQualifiedName : IReadOnlyCollection<QualifiedName>
    {
        private readonly IReadOnlyCollection<QualifiedName> parts;

        private FullyQualifiedName()
        {
            parts = new QualifiedName[0]; 
        }

        public FullyQualifiedName(IReadOnlyCollection<QualifiedName> parts)
        {
            this.parts = parts ?? throw new ArgumentNullException(nameof(parts));
        }

        public static FullyQualifiedName Any { get; } = new FullyQualifiedName();

        public static FullyQualifiedName None { get; } = new FullyQualifiedName();

        public bool IsAny => ReferenceEquals(this, Any);

        public bool IsNone => ReferenceEquals(this, None);

        public int Count => parts.Count;

        public IEnumerator<QualifiedName> GetEnumerator() => parts.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => parts.GetEnumerator();
    }

    public class ConstructorPointcutDefinition : PointcutDef
    {
        public override PointcutDefKind Kind => PointcutDefKind.Constructor;
    }

    public class PropertyPointcutDefinition : PointcutDef
    {
        public bool IsGet { get; set; } = true;

        public bool IsSet { get; set; } = true;

        public override PointcutDefKind Kind => IsGet && IsSet 
            ? PointcutDefKind.Property 
            : IsGet 
                ? PointcutDefKind.GetProperty
                : PointcutDefKind.SetProperty;
    }

    public class PointcutDef
    {
        public MemberAccessibility Accessibility { get; set; }

        public MemberScope Scope { get; set; }

        public TypeName Type { get; set; }

        public virtual PointcutDefKind Kind => PointcutDefKind.Any;
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
        private const int FullyQualifiedNameDepth = 125;
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
            return CurrentToken?.TokenKind == t1;
        }

        public LanguageParser(LanguageLexer lexer)
        {
            this.lexer = lexer ?? throw new ArgumentNullException();
        }    

        public PointcutDef ParsePointcut()
        {
            Lex();

            var accessibility = ParseAccessibility();
            var memberScope = ParseMemberScope();
            var typeName = ParseTypeName();

            if (Eat(SyntaxTokenKind.Dot, SyntaxTokenKind.New))
            {
                return new ConstructorPointcutDefinition
                {
                    Accessibility = accessibility,
                    Scope = memberScope,
                    Type = typeName
                };
            }
            else if (Eat(SyntaxTokenKind.Dot, SyntaxTokenKind.GetProperty))
            {
                return new PropertyPointcutDefinition { IsGet = true, IsSet = false };
            }
            else if (Eat(SyntaxTokenKind.Dot, SyntaxTokenKind.SetProperty))
            {
                return new PropertyPointcutDefinition { IsGet = true, IsSet = false };
            }
            else if (Probe(SyntaxTokenKind.LeftP))
            {
                return null; // method def
            }
            else
            {
                return new PointcutDef
                {
                    Accessibility = accessibility,
                    Scope = memberScope,
                    Type = typeName
                };
            }
        }

        protected TypeName ParseTypeName()
        {
            var parts = ParseFullyQualifiedName();

            if (parts.Count == 1)
            {
                return new TypeName(FullyQualifiedName.None, parts.First());
            }
            else if (parts.Count == 2)
            {
                var p1 = parts.First();
                var p2 = parts.Last();
                return p2.MatchType == QualifiedNameMatchType.Any
                    ? new TypeName(FullyQualifiedName.Any, p2)
                    : new TypeName(new FullyQualifiedName(new[] { p1 }), p2);
            }
            else
            {
                var fqn = new FullyQualifiedName(parts.Take(parts.Count - 1).ToArray());
                return new TypeName(fqn, parts.Last());
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

        private MemberAccessibility ParseAccessibility()
        {
            var token = CurrentToken;
            CheckToken(token);

            MemberAccessibility accessibility;

            switch (token.TokenKind)
            {
                case SyntaxTokenKind.Times:
                    accessibility = MemberAccessibility.Any;
                    break;
                case SyntaxTokenKind.None:
                case SyntaxTokenKind.Public:
                case SyntaxTokenKind.Plus:
                    accessibility = MemberAccessibility.Public;
                    break;
                case SyntaxTokenKind.Private:
                case SyntaxTokenKind.Minus:
                    accessibility = MemberAccessibility.Private;
                    break;
                case SyntaxTokenKind.Protected:
                case SyntaxTokenKind.Hash:
                    accessibility = MemberAccessibility.Protected;
                    break;
                case SyntaxTokenKind.Internal:
                    accessibility = MemberAccessibility.Internal;
                    break;
                default:
                    return MemberAccessibility.Public;
            }

            if (accessibility == MemberAccessibility.Protected)
            {
                var nextToken = PeekToken(1);
                if (nextToken?.TokenKind == SyntaxTokenKind.Internal)
                {
                    AdvanceToken(2);
                    return MemberAccessibility.ProtectedInternal;
                }
            }

            AdvanceToken(1);
            return accessibility;
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
                        AdvanceToken(1);
                        return QualifiedName.Any;
                    }
                    break;

                default:
                    return null;
            }

            return new QualifiedName(name, matchType);
        }

        private IReadOnlyCollection<QualifiedName> ParseFullyQualifiedName()
        {
            var names = new List<QualifiedName>(FullyQualifiedNameDepth);

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

        private static void CheckToken(SyntaxToken token, SyntaxTokenKind[] allowedTokens = null, SyntaxTokenKind[] expectedTokens = null)
        {
            if (token == null)
            {
                //TODO: bad token
            }
        }

    }
}