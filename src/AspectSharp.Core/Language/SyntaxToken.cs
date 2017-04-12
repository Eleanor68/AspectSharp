using System.Diagnostics;

namespace AspectSharp.Core.Language
{
    [DebuggerDisplay("Kind={TokenKind} Text='{IdentifierText}'")]
    public class SyntaxToken
    {
        public string IdentifierText { get; set; }

        public SyntaxTokenKind TokenKind { get; set; }
    }
}