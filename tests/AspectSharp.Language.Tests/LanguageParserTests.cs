using AspectSharp.Core.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AspectSharp.Language.Tests
{
    public class LanguageParserTests
    {
        private const string NullString = null;

        [Theory]
        [InlineData(MemberVisibility.Public, "string Namespace.Class.Method()")]
        [InlineData(MemberVisibility.Public, "public string Namespace.Class.Method()")]
        [InlineData(MemberVisibility.Protected, "protected string Namespace.Class.Method()")]
        [InlineData(MemberVisibility.ProtectedInternal, "protected internal string Namespace.Class.Method()")]
        [InlineData(MemberVisibility.Private, "private string Namespace.Class.Method()")]
        [InlineData(MemberVisibility.Internal, "internal string Namespace.Class.Method()")]
        [InlineData(MemberVisibility.Public, "+ string Namespace.Class.Method()")]
        [InlineData(MemberVisibility.Protected, "# string Namespace.Class.Method()")]
        [InlineData(MemberVisibility.Private, "- string Namespace.Class.Method()")]
        [InlineData(MemberVisibility.Any, "* string Namespace.Class.Method()")]
        public void CheckMethodVisibility(MemberVisibility memberVisibility, string pointcutDef)
        {
            var pointcut = ParseMember(pointcutDef);

            Assert.NotNull(pointcut);
            Assert.Equal(memberVisibility, pointcut.Visibility);
        }

        [Theory]
        [InlineData(MemberScope.Any, "public string Namespace.Class.Method()")]
        [InlineData(MemberScope.Static, "public static string Namespace.Class.Method()")]
        [InlineData(MemberScope.Instance, "public instance string Namespace.Class.Method()")]
        public void CheckMethodScope(MemberScope scope, string pointcutDef)
        {
            var pointcut = ParseMember(pointcutDef);

            Assert.NotNull(pointcut);
            Assert.Equal(scope, pointcut.Scope);
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
            var pointcut = ParseMember(pointcutDef);
            var constructorPointcut = Assert.IsType<ConstructorPointcutSyntax>(pointcut);

            Assert.NotNull(constructorPointcut);
            Assert.NotNull(constructorPointcut.Type);
            Assert.Equal(matchType, constructorPointcut.Type.Name.MatchType);
            Assert.Equal(name, constructorPointcut.Type.Name.Name);
        }

        [Theory]
        [InlineData(QualifiedNameMatchType.Any, "public *.ctor", null)]
        [InlineData(QualifiedNameMatchType.Any, "public *.*.ctor", null)]
        [InlineData(QualifiedNameMatchType.Strict, "public string.ctor", "string")]
        [InlineData(QualifiedNameMatchType.EndsWith, "public *string.ctor", "string")]
        [InlineData(QualifiedNameMatchType.StartsWith, "public string*.ctor", "string")]
        [InlineData(QualifiedNameMatchType.Contains, "public *string*.ctor", "string")]
        public void CheckCtorBasedConstructor(QualifiedNameMatchType matchType, string pointcutDef, string name)
        {
            var pointcut = ParseMember(pointcutDef);
            var constructorPointcut = Assert.IsType<ConstructorPointcutSyntax>(pointcut);

            Assert.NotNull(constructorPointcut);
            Assert.NotNull(constructorPointcut.Type);
            Assert.NotNull(constructorPointcut.Parameters);
            Assert.Equal(matchType, constructorPointcut.Type.Name.MatchType);
            Assert.Equal(name, constructorPointcut.Type.Name.Name);
            Assert.Same(ParameterListSyntax.Any, constructorPointcut.Parameters);
        }

        public static object[] parameterizedConstructorData = new object[]
        {
            new object [] { "public *.new", new (ParameterModifier, string)[0] },
            new object [] { "public *.new()", new (ParameterModifier, string)[0] },
            new object [] { "public *.new(..)", new (ParameterModifier, string)[0] },
            new object [] { "public *.new(*)", new [] { (ParameterModifier.In, NullString) } },
            new object [] { "public *.new(int)", new [] { (ParameterModifier.In, "int") } },
            new object [] { "public *.new(out int)", new [] { (ParameterModifier.Out, "int") } },
            new object [] { "public *.new(ref int)", new [] { (ParameterModifier.Ref, "int") } },
            new object [] { "public *.new(ref int, out int)", new [] { (ParameterModifier.Ref, "int"), (ParameterModifier.Out, "int") } },
            new object [] { "public *.new(out *)", new [] { (ParameterModifier.Out, NullString) } },
            new object [] { "public *.new(ref *)", new [] { (ParameterModifier.Ref, NullString) } },
            new object [] { "public Namespace.Class.new(Namespace.Class)", new [] { (ParameterModifier.In, "Class") } },
        };

        [Theory]
        [MemberData("parameterizedConstructorData")]
        public void CheckParameterizedConstructor(string pointcutDef, (ParameterModifier Modifier, string TypeName)[] parameters)
        {
            var pointcut = ParseMember(pointcutDef);
            var constructorPointcut = Assert.IsType<ConstructorPointcutSyntax>(pointcut);

            Assert.NotNull(constructorPointcut);
            Assert.NotNull(constructorPointcut.Type);
            Assert.NotNull(constructorPointcut.Parameters);
            Assert.Equal(parameters.Length, constructorPointcut.Parameters.Count);

            foreach (var item in constructorPointcut.Parameters.Zip(parameters, (p1, p2) => ((p1.Modifier, p1.TypeName.Name.Name), p2)).ToArray())
            {
                Assert.Equal(item.p2.Modifier, item.Item1.Modifier);
                Assert.Equal(item.p2.TypeName, item.Item1.Name);
            }
        }

        /*[Theory]
        [InlineData("public int Namespace.Class.GetProperty.get", MemberVisibility.Public, "int", "Namespace.Class", "GetProperty", true, false)]
        [InlineData("public int Namespace.Class.SetProperty.set", MemberVisibility.Public, "int", "Namespace.Class", "SetProperty", false, true)]
        [InlineData("public int Namespace.Class.Property.property", MemberVisibility.Public, "int", "Namespace.Class", "Property", true, true)]*/
        public void CheckProperty(string pointcutDef, MemberVisibility visibility, string propertyType, string definedType, string propertyName, bool isGet, bool isSet)
        {
            var pointcut = ParseMember(pointcutDef);
            var propertyPointcut = Assert.IsType<PropertyPointcutSyntax>(pointcut);

            Assert.NotNull(propertyPointcut);
            Assert.NotNull(propertyPointcut.Type);
            Assert.NotNull(propertyPointcut.DeclaredType);
            Assert.NotNull(propertyPointcut.Name);

            Assert.Equal(visibility, propertyPointcut.Visibility);
            Assert.Equal(propertyType, propertyPointcut.Type.Name.Name);
            Assert.Equal(definedType, propertyPointcut.DeclaredType.Name.Name);
            Assert.Equal(propertyName, propertyPointcut.Name.Name);
            Assert.Equal(isGet, propertyPointcut.IsGet);
            Assert.Equal(isSet, propertyPointcut.IsSet);
        }


        private static MemberPointcutSyntax ParseMember(string pointcutDef)
        {
            var language = new LanguageText(pointcutDef);
            var lexer = new LanguageLexer(language);
            var parser = new LanguageParser(lexer);
            return parser.ParseMemberPointcut();
        }
    }
}