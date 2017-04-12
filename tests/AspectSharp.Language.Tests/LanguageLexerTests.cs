using AspectSharp.Core.Language;
using System;
using Xunit;

namespace AspectSharp.Language.Tests
{
    public class LanguageLexerTests
    {
        [Fact]
        public void LexEmptyText()
        {
            Assert.Throws<ArgumentException>(() => new LanguageText(null));
            Assert.Throws<ArgumentException>(() => new LanguageText(string.Empty));
        }

        [Fact]
        public void LexEndOfText()
        {
            var text = new LanguageText(".");
            var lexer = new LanguageLexer(text);
            var token = lexer.GetNextToken();

            token = lexer.GetNextToken();
            Assert.NotNull(token);
            Assert.Equal(SyntaxTokenKind.EndOfText, token.TokenKind);

            token = lexer.GetNextToken();
            Assert.NotNull(token);
            Assert.Equal(SyntaxTokenKind.EndOfText, token.TokenKind);
        }

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
        public void LexWhiteDotToken()
        {
            var text = new LanguageText("  .");
            var lexer = new LanguageLexer(text);

            var token = lexer.GetNextToken();
            Assert.NotNull(token);
            Assert.Equal(SyntaxTokenKind.White, token.TokenKind);

            token = lexer.GetNextToken();
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

        [Fact]
        public void LexWhitePublicToken()
        {
            var text = new LanguageText("  public");
            var lexer = new LanguageLexer(text);

            var token = lexer.GetNextToken();
            Assert.NotNull(token);
            Assert.Equal(SyntaxTokenKind.White, token.TokenKind);

            token = lexer.GetNextToken();
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
