using System;
using System.Diagnostics;

namespace AspectSharp.Core.Language
{
    [DebuggerDisplay("{ToString()}")]
    public sealed partial class IdentifierNameSyntax
    {
        private IdentifierNameSyntax(string name, IdentifierNameMatchType matchType)
        {
            Name = name;
            MatchType = matchType;
        }

        public IdentifierNameMatchType MatchType { get; }

        public string Name { get; }

        public bool IsAny => ReferenceEquals(this, Any);

        public static IdentifierNameSyntax Any { get; } = new IdentifierNameSyntax(null, IdentifierNameMatchType.Any);

        public override string ToString()
        {
            if (IsAny) return "*";

            switch (MatchType)
            {
                case IdentifierNameMatchType.Any: return "*";
                case IdentifierNameMatchType.Strict: return Name;
                case IdentifierNameMatchType.StartsWith: return Name + '*';
                case IdentifierNameMatchType.EndsWith: return '*' + Name;
                case IdentifierNameMatchType.Contains: return '*' + Name + '*';
                default: throw new IndexOutOfRangeException();
            }
        }
    }

    partial class IdentifierNameSyntax
    {
        public static IdentifierNameSyntax Create(string name, IdentifierNameMatchType matchType = IdentifierNameMatchType.Strict)
        {
            if (name == null && matchType == IdentifierNameMatchType.Any) return Any;
            return new IdentifierNameSyntax(name, matchType);
        }
    }
}