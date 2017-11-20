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
        [InlineData(IdentifierNameMatchType.Any, "*.new", null)]
        [InlineData(IdentifierNameMatchType.Any, "public *.new", null)]
        [InlineData(IdentifierNameMatchType.Any, "public *.*.new", null)]
        [InlineData(IdentifierNameMatchType.Strict, "public string.new", "string")]
        [InlineData(IdentifierNameMatchType.EndsWith, "public *string.new", "string")]
        [InlineData(IdentifierNameMatchType.StartsWith, "public string*.new", "string")]
        [InlineData(IdentifierNameMatchType.Contains, "public *string*.new", "string")]
        public void CheckNewBasedConstructor(IdentifierNameMatchType matchType, string pointcutDef, string name)
        {
            var pointcut = ParseMember(pointcutDef);
            var constructorPointcut = Assert.IsType<ConstructorPointcutSyntax>(pointcut);

            Assert.NotNull(constructorPointcut);
            Assert.NotNull(constructorPointcut.Type);
            Assert.Equal(matchType, constructorPointcut.Type.TypeName.MatchType);
            Assert.Equal(name, constructorPointcut.Type.TypeName.Name);
        }

        [Theory]
        [InlineData(IdentifierNameMatchType.Any, "public *.ctor", null)]
        [InlineData(IdentifierNameMatchType.Any, "public *.*.ctor", null)]
        [InlineData(IdentifierNameMatchType.Strict, "public string.ctor", "string")]
        [InlineData(IdentifierNameMatchType.EndsWith, "public *string.ctor", "string")]
        [InlineData(IdentifierNameMatchType.StartsWith, "public string*.ctor", "string")]
        [InlineData(IdentifierNameMatchType.Contains, "public *string*.ctor", "string")]
        public void CheckCtorBasedConstructor(IdentifierNameMatchType matchType, string pointcutDef, string name)
        {
            var pointcut = ParseMember(pointcutDef);
            var constructorPointcut = Assert.IsType<ConstructorPointcutSyntax>(pointcut);

            Assert.NotNull(constructorPointcut);
            Assert.NotNull(constructorPointcut.Type);
            Assert.NotNull(constructorPointcut.Parameters);
            Assert.Equal(matchType, constructorPointcut.Type.TypeName.MatchType);
            Assert.Equal(name, constructorPointcut.Type.TypeName.Name);
            Assert.Same(ParameterListSyntax.Any, constructorPointcut.Parameters);
        }

        public static IEnumerable<object[]> parameterizedConstructorData = new []
        {
            new object [] { "*.new", new (ParameterModifier, string)[0] },
            new object [] { "public *.new", new(ParameterModifier, string)[0] },
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
        [MemberData(nameof(parameterizedConstructorData))]
        public void CheckParameterizedConstructor(string pointcutDef, (ParameterModifier Modifier, string TypeName)[] parameters)
        {
            var pointcut = ParseMember(pointcutDef);
            var constructorPointcut = Assert.IsType<ConstructorPointcutSyntax>(pointcut);

            Assert.NotNull(constructorPointcut);
            Assert.NotNull(constructorPointcut.Type);
            Assert.NotNull(constructorPointcut.Parameters);
            Assert.Equal(parameters.Length, constructorPointcut.Parameters.Count);

            foreach (var item in constructorPointcut.Parameters.Zip(parameters, (p1, p2) => ((p1.Modifier, p1.TypeName.TypeName.Name), p2)).ToArray())
            {
                Assert.Equal(item.p2.Modifier, item.Item1.Modifier);
                Assert.Equal(item.p2.TypeName, item.Item1.Name);
            }
        }

        [Theory]
        //[InlineData("*Property.property", MemberVisibility.Public, "*.*", "*.*", "*Property", true, true)] 
        //we can not detect if the * from `*Property` is related to visibility or to name because we ignore whitespaces
        [InlineData("*.property", MemberVisibility.Public, "*", "*", "*", true, true)] //a kind of shortcut, allowed by language because of precedence of '.'
        [InlineData("*.*Property.property", MemberVisibility.Public, "*", "*", "*Property", true, true)]
        [InlineData("*.*.get", MemberVisibility.Public, "*", "*", "*", true, false)]
        [InlineData("public int Namespace.Class.GetProperty.get", MemberVisibility.Public, "int", "Namespace.Class", "GetProperty", true, false)]
        [InlineData("public int Namespace.Class.SetProperty.set", MemberVisibility.Public, "int", "Namespace.Class", "SetProperty", false, true)]
        [InlineData("public int Namespace.Class.Property.property", MemberVisibility.Public, "int", "Namespace.Class", "Property", true, true)]
        [InlineData("int Namespace.Class.GetProperty.get", MemberVisibility.Public, "int", "Namespace.Class", "GetProperty", true, false)]
        [InlineData("int Namespace.Class.SetProperty.set", MemberVisibility.Public, "int", "Namespace.Class", "SetProperty", false, true)]
        [InlineData("int Namespace.Class.Property.property", MemberVisibility.Public, "int", "Namespace.Class", "Property", true, true)]
        [InlineData("Namespace.Class.GetProperty.get", MemberVisibility.Public, "*", "Namespace.Class", "GetProperty", true, false)]
        [InlineData("Namespace.Class.SetProperty.set", MemberVisibility.Public, "*", "Namespace.Class", "SetProperty", false, true)]
        [InlineData("Namespace.Class.Property.property", MemberVisibility.Public, "*", "Namespace.Class", "Property", true, true)]
        [InlineData("Namespace.Class.*.property", MemberVisibility.Public, "*", "Namespace.Class", "*", true, true)]
        [InlineData("Namespace.Class.*Property*.property", MemberVisibility.Public, "*", "Namespace.Class", "*Property*", true, true)]
        [InlineData("Namespace.Class.Property*.property", MemberVisibility.Public, "*", "Namespace.Class", "Property*", true, true)]
        [InlineData("Namespace.Class.*Property.property", MemberVisibility.Public, "*", "Namespace.Class", "*Property", true, true)]
        [InlineData("Namespace.*.*Property.property", MemberVisibility.Public, "*", "Namespace.*", "*Property", true, true)]
        [InlineData("- Namespace.SubNamespace.Class.GetProperty.get", MemberVisibility.Private, "*", "Namespace.SubNamespace.Class", "GetProperty", true, false)]
        [InlineData("- Namespace.SubNamespace.Class.SetProperty.set", MemberVisibility.Private, "*", "Namespace.SubNamespace.Class", "SetProperty", false, true)]
        [InlineData("- Namespace.SubNamespace.Class.Property.property", MemberVisibility.Private, "*", "Namespace.SubNamespace.Class", "Property", true, true)]

        public void CheckProperty(string pointcutDef, MemberVisibility visibility, string propertyType, string definedType, string propertyName, bool isGet, bool isSet)
        {
            var pointcut = ParseMember(pointcutDef);
            var propertyPointcut = Assert.IsType<PropertyPointcutSyntax>(pointcut);

            Assert.NotNull(propertyPointcut);
            Assert.NotNull(propertyPointcut.Type);
            Assert.NotNull(propertyPointcut.DeclaredType);
            Assert.NotNull(propertyPointcut.Name);

            Assert.Equal(visibility, propertyPointcut.Visibility);
            Assert.Equal(propertyType, propertyPointcut.Type.ToString());
            Assert.Equal(definedType, propertyPointcut.DeclaredType.ToString());
            Assert.Equal(propertyName, propertyPointcut.Name.ToString());
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