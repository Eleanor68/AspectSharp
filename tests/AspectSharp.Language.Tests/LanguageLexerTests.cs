using AspectSharp.Core.SyntaxTree;
using System.Diagnostics;
using Xunit;

namespace AspectSharp.Language.Tests
{

    public class LanguageLexerTests
    {
        [Fact]
        public void LexDotToken()
        {
            var text = new LanguageText(".");
            var lexer = new LanguageLexer(text);
            var token = lexer.GetNextToken();

            Assert.NotNull(token);
            Assert.Equal(SyntaxTokenKind.Dot, token.TokenKind);
        }

        [Fact]
        public void LexPublicToken()
        {
            var text = new LanguageText("public");
            var lexer = new LanguageLexer(text);
            var token = lexer.GetNextToken();

            Assert.NotNull(token);
            Assert.Equal(SyntaxTokenKind.Public, token.TokenKind);
        }

        /*[Fact]
        public void DisplayCharMarks()
        {
            var i = 0;
            foreach (var c in LanguageLexer.charMarkers)
            {
                Trace.WriteLine($"[{i}] = `{(char)i}` - {(LanguageLexer.CharMark)c}");
                i++;
            }
        }*/

    }
}
