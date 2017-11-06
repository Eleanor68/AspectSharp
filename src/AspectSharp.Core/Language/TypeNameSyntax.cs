using System;

namespace AspectSharp.Core.Language
{
    public sealed class TypeNameSyntax
    {
        public TypeNameSyntax(FullyQualifiedNameSyntax @namespace, QualifiedNameSyntax name)
        {
            Namespace = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public FullyQualifiedNameSyntax Namespace { get; }

        public QualifiedNameSyntax Name { get; }

        //public static TypeName Any { get; } = new TypeName(FullyQualifiedName.Any, QualifiedName.Any);
    }
}