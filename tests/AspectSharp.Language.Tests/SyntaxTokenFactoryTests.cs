using AspectSharp.Core.Language;
using Xunit;

namespace AspectSharp.Language.Tests
{
    public class SyntaxTokenFactoryTests
    {
        [Theory]
        [InlineData('.', SyntaxTokenKind.Dot)]
        [InlineData(',', SyntaxTokenKind.Comma)]
        [InlineData('!', SyntaxTokenKind.Not)]
        [InlineData('(', SyntaxTokenKind.LeftP)]
        [InlineData(')', SyntaxTokenKind.RightP)]
        [InlineData('+', SyntaxTokenKind.Plus)]
        [InlineData('-', SyntaxTokenKind.Minus)]
        [InlineData('*', SyntaxTokenKind.Times)]
        [InlineData('#', SyntaxTokenKind.Hash)]
        [InlineData('&', SyntaxTokenKind.And)]
        [InlineData('|', SyntaxTokenKind.Or)]
        [InlineData('\0', SyntaxTokenKind.EndOfText)]
        public void CheckFlyweightPucts(char c, SyntaxTokenKind token)
        {
            var t1 = SyntaxTokenFactory.MakePunct(c);
            var t2 = SyntaxTokenFactory.MakePunct(c);
            Assert.Same(t1, t2);
            Assert.Equal(token, t1.TokenKind);
        }

        [Theory]
        [InlineData("public", SyntaxTokenKind.Public)]
        [InlineData("private", SyntaxTokenKind.Private)]
        [InlineData("protected", SyntaxTokenKind.Protected)]
        [InlineData("internal", SyntaxTokenKind.Internal)]
        [InlineData("instance", SyntaxTokenKind.Instance)]
        [InlineData("static", SyntaxTokenKind.Static)]
        [InlineData("class", SyntaxTokenKind.Class)]
        [InlineData("new", SyntaxTokenKind.New)]
        [InlineData("ctor", SyntaxTokenKind.Ctor)]
        [InlineData("get", SyntaxTokenKind.GetProperty)]
        [InlineData("set", SyntaxTokenKind.SetProperty)]
        [InlineData("property", SyntaxTokenKind.Property)]
        [InlineData("prop", SyntaxTokenKind.Property)]
        [InlineData("out", SyntaxTokenKind.Out)]
        [InlineData("ref", SyntaxTokenKind.Ref)]
        public void CheckFlyweightKeywords(string text, SyntaxTokenKind token)
        {
            var langText = new LanguageText(text);
            var t1 = SyntaxTokenFactory.MakeIdent(langText, langText.Offset, langText.Length);
            var t2 = SyntaxTokenFactory.MakeIdent(langText, langText.Offset, langText.Length);
            Assert.Same(t1, t2);
            Assert.Equal(token, t1.TokenKind);
        }

        [Theory]
        [InlineData("iNsTaNce", SyntaxTokenKind.Identifier)]
        [InlineData("Class", SyntaxTokenKind.Identifier)]
        public void CheckKeywordsCaseInsensitive(string text, SyntaxTokenKind token)
        {
            var langText = new LanguageText(text);
            var t1 = SyntaxTokenFactory.MakeIdent(langText, langText.Offset, langText.Length);
            var t2 = SyntaxTokenFactory.MakeIdent(langText, langText.Offset, langText.Length);
            Assert.NotSame(t1, t2);
            Assert.Equal(token, t1.TokenKind);
        }
    }
}
