using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace AspectSharp.Core.Language
{
    public sealed partial class QualifiedNameSyntax : IReadOnlyCollection<IdentifierNameSyntax>
    {
        public const int QualifiedNameDepth = 124;

        private readonly IReadOnlyCollection<IdentifierNameSyntax> parts;

        private QualifiedNameSyntax()
        {
            parts = new IdentifierNameSyntax[0]; 
        }

        private QualifiedNameSyntax(IReadOnlyCollection<IdentifierNameSyntax> parts)
        {
            this.parts = parts;
        }

        public static QualifiedNameSyntax Any { get; } = new QualifiedNameSyntax();

        public static QualifiedNameSyntax None { get; } = new QualifiedNameSyntax();

        public bool IsAny => ReferenceEquals(this, Any);

        public bool IsNone => ReferenceEquals(this, None);

        public int Count => parts.Count;

        public IEnumerator<IdentifierNameSyntax> GetEnumerator() => parts.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString()
        {
            if (IsAny) return "*";
            if (IsNone) return string.Empty;
            return string.Join(".", parts);
        }
    }

    partial class QualifiedNameSyntax
    {

        /// <summary>
        /// Create an instance of <see cref="QualifiedNameSyntax"/> based on a set of <see cref="IdentifierNameSyntax"/>
        /// </summary>
        /// <param name="parts">A set of <see cref="IdentifierNameSyntax"/> instances (including <see cref="IdentifierNameSyntax.Any"/>)</param>
        /// <returns>
        /// None - if the <paramref name="parts"/> is null or is a reference to <see cref="None"/>
        /// Any - if the collection is empty or there is one element and it is <see cref="IdentifierNameSyntax.IsAny"/>
        /// New instance if above conditions were not meet
        /// </returns>
        public static QualifiedNameSyntax Create(IReadOnlyCollection<IdentifierNameSyntax> parts)
        {
            if (ReferenceEquals(parts, None) || ReferenceEquals(parts, null)) return None;

            if (parts.Count == 0) return Any;

            return parts.Count == 1 && parts.First().IsAny ? Any : new QualifiedNameSyntax(parts);
        }
    }
}