using System.Diagnostics;

namespace AspectSharp.Core.Language
{
    [DebuggerDisplay("Kind={TokenKind} Text='{ValueText}'")]
    public class SyntaxToken
    {
        public string ValueText { get; set; }

        public SyntaxTokenKind TokenKind { get; set; }
    }
}