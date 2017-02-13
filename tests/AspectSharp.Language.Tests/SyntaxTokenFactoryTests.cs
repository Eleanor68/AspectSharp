using AspectSharp.Core.SyntaxTree;
using System.Collections.Generic;
using Xunit;

namespace AspectSharp.Language.Tests
{
    public class SyntaxTokenFactoryTests
    {
        [Theory]
        [InlineData('.', SyntaxTokenKind.Dot)]
        [InlineData('!', SyntaxTokenKind.Not)]
        [InlineData('(', SyntaxTokenKind.LeftP)]
        [InlineData(')', SyntaxTokenKind.RightP)]
        [InlineData('+', SyntaxTokenKind.Plus)]
        [InlineData('-', SyntaxTokenKind.Minus)]
        [InlineData('*', SyntaxTokenKind.Times)]
        [InlineData('#', SyntaxTokenKind.Hash)]
        [InlineData('&', SyntaxTokenKind.And)]
        [InlineData('|', SyntaxTokenKind.Or)]
        public void CheckPucts(char c, SyntaxTokenKind token)
        {
            var t1 = SyntaxTokenFactory.MakePunct(c);
            var t2 = SyntaxTokenFactory.MakePunct(c);
            Assert.Same(t1, t2);
            Assert.Equal(token, t1.TokenKind);
        }
    }
}
