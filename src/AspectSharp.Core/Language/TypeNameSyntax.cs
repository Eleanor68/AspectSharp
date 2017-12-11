using System;
using System.Linq;
using System.Collections.Generic;

namespace AspectSharp.Core.Language
{
    public sealed partial class TypeNameSyntax
    {
        private TypeNameSyntax(QualifiedNameSyntax @namespace, IdentifierNameSyntax typeName)
        {
            Namespace = @namespace;
            TypeName = typeName;
        }

        public QualifiedNameSyntax Namespace { get; }

        public IdentifierNameSyntax TypeName { get; }

        public bool IsAny => ReferenceEquals(this, Any);

        public bool IsNone => ReferenceEquals(this, None);

        public static TypeNameSyntax Any { get; } = new TypeNameSyntax(QualifiedNameSyntax.Any, IdentifierNameSyntax.Any);

        public static TypeNameSyntax None { get; } = new TypeNameSyntax(QualifiedNameSyntax.None, IdentifierNameSyntax.Any);

        public override string ToString()
        {
            var n = Namespace.ToString();
            return string.IsNullOrEmpty(n) ? TypeName.ToString() : n + '.' + TypeName.ToString();
        }
    }

    partial class TypeNameSyntax
    {
        public static TypeNameSyntax Create(QualifiedNameSyntax @namespace, IdentifierNameSyntax typeName)
        {
            if (@namespace == null) throw new ArgumentNullException(nameof(@namespace));
            if (typeName == null) throw new ArgumentNullException(nameof(typeName));

            if (@namespace.IsAny && typeName.IsAny) return Any;
            if (@namespace.IsNone && typeName.IsAny) return None;
            return new TypeNameSyntax(@namespace, typeName);
        }

        public static TypeNameSyntax Create(IdentifierNameSyntax identifierName)
        {
            return Create(new[] { identifierName ?? throw new ArgumentNullException(nameof(identifierName)) });
        }

        public static TypeNameSyntax Create(IdentifierNameSyntax identifierName1, IdentifierNameSyntax identifierName2)
        {
            return Create(new[] {
                identifierName1 ?? throw new ArgumentNullException(nameof(identifierName1)),
                identifierName2 ?? throw new ArgumentNullException(nameof(identifierName2))});
        }

        public static TypeNameSyntax Create(IdentifierNameSyntax identifierName1, IdentifierNameSyntax identifierName2, IdentifierNameSyntax identifierName3)
        {
            return Create(new[] {
                identifierName1 ?? throw new ArgumentNullException(nameof(identifierName1)),
                identifierName2 ?? throw new ArgumentNullException(nameof(identifierName2)),
                identifierName3 ?? throw new ArgumentNullException(nameof(identifierName3))});
        }

        public static TypeNameSyntax Create(IReadOnlyCollection<IdentifierNameSyntax> parts)
        {
            if (parts == null) throw new ArgumentNullException(nameof(parts));

            if (ReferenceEquals(parts, QualifiedNameSyntax.Any)) return Any;
            if (ReferenceEquals(parts, QualifiedNameSyntax.None)) return None;

            if (parts.Count == 0)
            {
                return None;
            }
            if (parts.Count == 1)
            {
                var p = parts.First();
                return p.IsAny ? None : new TypeNameSyntax(QualifiedNameSyntax.None, p);
            }
            else if (parts.Count == 2)
            {
                var p1 = parts.First();
                var p2 = parts.Last();

                if (p1.IsAny && p2.IsAny) return Any;

                return p1.IsAny
                    ? new TypeNameSyntax(QualifiedNameSyntax.Any, p2)
                    : new TypeNameSyntax(QualifiedNameSyntax.Create(new[] { p1 }), p2);
            }
            else
            {
                var fqn = QualifiedNameSyntax.Create(parts.Take(parts.Count - 1).ToArray());
                return new TypeNameSyntax(fqn, parts.Last());
            }
        }
    }
}