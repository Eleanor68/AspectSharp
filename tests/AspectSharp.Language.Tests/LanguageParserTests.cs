using AspectSharp.Core.Language;
using Xunit;

namespace AspectSharp.Language.Tests
{
    public class LanguageParserTests
    {
        [Theory]
        [InlineData(AccessibilityToken.Public, "string Namespace.Class.Method()")]
        [InlineData(AccessibilityToken.Public, "public string Namespace.Class.Method()")]
        [InlineData(AccessibilityToken.Protected, "protected string Namespace.Class.Method()")]
        [InlineData(AccessibilityToken.ProtectedInternal, "protected internal string Namespace.Class.Method()")]
        [InlineData(AccessibilityToken.Private, "private string Namespace.Class.Method()")]
        [InlineData(AccessibilityToken.Internal, "internal string Namespace.Class.Method()")]
        [InlineData(AccessibilityToken.Public, "+ string Namespace.Class.Method()")]
        [InlineData(AccessibilityToken.Protected, "# string Namespace.Class.Method()")]
        [InlineData(AccessibilityToken.Private, "- string Namespace.Class.Method()")]
        [InlineData(AccessibilityToken.Any, "* string Namespace.Class.Method()")]
        public void CheckMethodAccessibility(AccessibilityToken accessibilityToken, string pointcutDef)
        {
            var language = new LanguageText(pointcutDef);
            var lexer = new LanguageLexer(language);
            var parser = new LanguageParser(lexer);
            var pointcut = parser.ParsePointcut();

            Assert.NotNull(pointcut);
            Assert.Equal(pointcut.Accessibility, accessibilityToken);
        }

        [Theory]
        [InlineData(MemberScopeToken.Any, "public string Namespace.Class.Method()")]
        [InlineData(MemberScopeToken.Static, "public static string Namespace.Class.Method()")]
        [InlineData(MemberScopeToken.Instance, "public instance string Namespace.Class.Method()")]
        public void CheckMethodScope(MemberScopeToken scope, string pointcutDef)
        {
            var language = new LanguageText(pointcutDef);
            var lexer = new LanguageLexer(language);
            var parser = new LanguageParser(lexer);
            var pointcut = parser.ParsePointcut();

            Assert.NotNull(pointcut);
            Assert.Equal(pointcut.MemberScope, scope);
        }

        [Theory]
        [InlineData(QualifiedNameMatchType.Any, "public *", null)]
        [InlineData(QualifiedNameMatchType.Strict, "public string", "string")]
        [InlineData(QualifiedNameMatchType.EndsWith, "public *string", "string")]
        [InlineData(QualifiedNameMatchType.StartsWith, "public string*", "string")]
        [InlineData(QualifiedNameMatchType.Contains, "public *string*", "string")]
        public void CheckQualifiedName(QualifiedNameMatchType matchType, string pointcutDef, string name)
        {
            var language = new LanguageText(pointcutDef);
            var lexer = new LanguageLexer(language);
            var parser = new LanguageParser(lexer);
            var pointcut = parser.ParsePointcut();

            Assert.NotNull(pointcut);
            Assert.NotNull(pointcut.Type);
            Assert.Equal(pointcut.Type.MatchType, matchType);
            Assert.Equal(pointcut.Type.Name, name);
        }
    }
}
