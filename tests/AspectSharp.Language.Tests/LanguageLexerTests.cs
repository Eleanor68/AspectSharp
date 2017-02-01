using AspectSharp.Core.SyntaxTree;
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
            Assert.Equal(SyntaxTokenKind.Identifier, token.TokenKind);
        }
    }
}
