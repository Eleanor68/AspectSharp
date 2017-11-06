using System.Diagnostics;

namespace AspectSharp.Core.Language
{
    [DebuggerDisplay("MatchType={MatchType} Name={Name}")]
    public sealed class QualifiedNameSyntax
    {
        public QualifiedNameSyntax(string name, QualifiedNameMatchType matchType)
        {
            Name = name;
            MatchType = matchType;
        }

        public QualifiedNameMatchType MatchType { get; }

        public string Name { get; }

        public static QualifiedNameSyntax Any { get; } = new QualifiedNameSyntax(null, QualifiedNameMatchType.Any);
    }
}