using AspectSharp.Core.Language;
using Xunit;

namespace AspectSharp.Language.Tests
{
    public class LanguageParserTests
    {
        [Theory]
        [InlineData(MemberAccessibility.Public, "string Namespace.Class.Method()")]
        [InlineData(MemberAccessibility.Public, "public string Namespace.Class.Method()")]
        [InlineData(MemberAccessibility.Protected, "protected string Namespace.Class.Method()")]
        [InlineData(MemberAccessibility.ProtectedInternal, "protected internal string Namespace.Class.Method()")]
        [InlineData(MemberAccessibility.Private, "private string Namespace.Class.Method()")]
        [InlineData(MemberAccessibility.Internal, "internal string Namespace.Class.Method()")]
        [InlineData(MemberAccessibility.Public, "+ string Namespace.Class.Method()")]
        [InlineData(MemberAccessibility.Protected, "# string Namespace.Class.Method()")]
        [InlineData(MemberAccessibility.Private, "- string Namespace.Class.Method()")]
        [InlineData(MemberAccessibility.Any, "* string Namespace.Class.Method()")]
        public void CheckMethodAccessibility(MemberAccessibility memberAccessibility, string pointcutDef)
        {
            var language = new LanguageText(pointcutDef);
            var lexer = new LanguageLexer(language);
            var parser = new LanguageParser(lexer);
            var pointcut = parser.ParsePointcut();

            Assert.NotNull(pointcut);
            Assert.Equal(pointcut.Accessibility, memberAccessibility);
        }

        [Theory]
        [InlineData(MemberScope.Any, "public string Namespace.Class.Method()")]
        [InlineData(MemberScope.Static, "public static string Namespace.Class.Method()")]
        [InlineData(MemberScope.Instance, "public instance string Namespace.Class.Method()")]
        public void CheckMethodScope(MemberScope scope, string pointcutDef)
        {
            var language = new LanguageText(pointcutDef);
            var lexer = new LanguageLexer(language);
            var parser = new LanguageParser(lexer);
            var pointcut = parser.ParsePointcut();

            Assert.NotNull(pointcut);
            Assert.Equal(pointcut.Scope, scope);
        }

        [Theory]
        [InlineData(QualifiedNameMatchType.Any, "public *.new", null)]
        [InlineData(QualifiedNameMatchType.Any, "public *.*.new", null)]
        [InlineData(QualifiedNameMatchType.Strict, "public string.new", "string")]
        [InlineData(QualifiedNameMatchType.EndsWith, "public *string.new", "string")]
        [InlineData(QualifiedNameMatchType.StartsWith, "public string*.new", "string")]
        [InlineData(QualifiedNameMatchType.Contains, "public *string*.new", "string")]
        public void CheckNewBasedConstructor(QualifiedNameMatchType matchType, string pointcutDef, string name)
        {
            var language = new LanguageText(pointcutDef);
            var lexer = new LanguageLexer(language);
            var parser = new LanguageParser(lexer);
            var pointcut = parser.ParsePointcut();

            var constructorPointcut = Assert.IsType<ConstructorPointcutDefinition>(pointcut);

            Assert.NotNull(constructorPointcut);
            Assert.NotNull(constructorPointcut.Type);
            Assert.Equal(constructorPointcut.Type.Name.MatchType, matchType);
            Assert.Equal(constructorPointcut.Type.Name.Name, name);
        }
    }
}
