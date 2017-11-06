using System;
using System.Collections;
using System.Collections.Generic;

namespace AspectSharp.Core.Language
{
    public sealed class FullyQualifiedNameSyntax : IReadOnlyCollection<QualifiedNameSyntax>
    {
        private readonly IReadOnlyCollection<QualifiedNameSyntax> parts;

        private FullyQualifiedNameSyntax()
        {
            parts = new QualifiedNameSyntax[0]; 
        }

        public FullyQualifiedNameSyntax(IReadOnlyCollection<QualifiedNameSyntax> parts)
        {
            this.parts = parts ?? throw new ArgumentNullException(nameof(parts));
        }

        public static FullyQualifiedNameSyntax Any { get; } = new FullyQualifiedNameSyntax();

        public static FullyQualifiedNameSyntax None { get; } = new FullyQualifiedNameSyntax();

        public bool IsAny => ReferenceEquals(this, Any);

        public bool IsNone => ReferenceEquals(this, None);

        public int Count => parts.Count;

        public IEnumerator<QualifiedNameSyntax> GetEnumerator() => parts.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}