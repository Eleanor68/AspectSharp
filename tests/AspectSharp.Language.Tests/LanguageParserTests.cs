using AspectSharp.Core.Language;
using Xunit;

namespace AspectSharp.Language.Tests
{
    public class LanguageParserTests
    {
        [Fact]
        public void ParsePublicPointcut()
        {
            var language = new LanguageText("public string Namespace.Class.Method()");
            var lexer = new LanguageLexer(language);
            var parser = new LanguageParser(lexer);
            var pointcut = parser.ParsePointcut();
        }
    }
}
