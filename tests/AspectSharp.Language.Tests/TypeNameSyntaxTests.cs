using AspectSharp.Core;
using AspectSharp.Core.Language;
using System;
using System.Collections.Generic;
using Xunit;

namespace AspectSharp.Language.Tests
{
    public class TypeNameSyntaxTests
    {
        public const string NamespaceString = "Namespace";

        public const string ClassString = "Class";

        [Fact]
        public void CreateNullArguments()
        {
            Assert.Throws<ArgumentNullException>(() => TypeNameSyntax.Create((QualifiedNameSyntax)null, null));
            Assert.Throws<ArgumentNullException>(() => TypeNameSyntax.Create(QualifiedNameSyntax.Any, null));
            Assert.Throws<ArgumentNullException>(() => TypeNameSyntax.Create((QualifiedNameSyntax)null, IdentifierNameSyntax.Any));
            Assert.Throws<ArgumentNullException>(() => TypeNameSyntax.Create(parts: null));
            Assert.Throws<ArgumentNullException>(() => TypeNameSyntax.Create((IdentifierNameSyntax)null));
            Assert.Throws<ArgumentNullException>(() => TypeNameSyntax.Create((IdentifierNameSyntax)null, null));
            Assert.Throws<ArgumentNullException>(() => TypeNameSyntax.Create(null, null, null));
        }

        public static IEnumerable<object[]> parameterizedToStringData = new[]
        {
            new object[] { "*.*", QualifiedNameSyntax.Any, IdentifierNameSyntax.Any },
            new object[] { "*.*", QualifiedNameSyntax.Any, IdentifierNameSyntax.Any },
            new object[] { "*.*", QualifiedNameSyntax.Create(new IdentifierNameSyntax[0]), IdentifierNameSyntax.Any },
            new object[] { "*.*", QualifiedNameSyntax.Create(new[] { IdentifierNameSyntax.Any }), IdentifierNameSyntax.Any },
            new object[] { $"{NamespaceString}.*", QualifiedNameSyntax.Create(new[] { IdentifierNameSyntax.Create(NamespaceString) }), IdentifierNameSyntax.Any },
            new object[] { $"*{NamespaceString}*.*", QualifiedNameSyntax.Create(new[] { IdentifierNameSyntax.Create(NamespaceString, IdentifierNameMatchType.Contains) }), IdentifierNameSyntax.Any },
            new object[] { $"{NamespaceString}*.*", QualifiedNameSyntax.Create(new[] { IdentifierNameSyntax.Create(NamespaceString, IdentifierNameMatchType.StartsWith) }), IdentifierNameSyntax.Any },
            new object[] { $"*{NamespaceString}.*", QualifiedNameSyntax.Create(new[] { IdentifierNameSyntax.Create(NamespaceString, IdentifierNameMatchType.EndsWith) }), IdentifierNameSyntax.Any },
            new object[] { $"{NamespaceString}.{ClassString}", QualifiedNameSyntax.Create(new[] { IdentifierNameSyntax.Create(NamespaceString) }), IdentifierNameSyntax.Create(ClassString) },
            new object[] { $"{NamespaceString}.*{ClassString}*", QualifiedNameSyntax.Create(new[] { IdentifierNameSyntax.Create(NamespaceString) }), IdentifierNameSyntax.Create(ClassString, IdentifierNameMatchType.Contains) },
            new object[] { $"{NamespaceString}.{ClassString}*", QualifiedNameSyntax.Create(new[] { IdentifierNameSyntax.Create(NamespaceString) }), IdentifierNameSyntax.Create(ClassString, IdentifierNameMatchType.StartsWith) },
            new object[] { $"{NamespaceString}.*{ClassString}", QualifiedNameSyntax.Create(new[] { IdentifierNameSyntax.Create(NamespaceString) }), IdentifierNameSyntax.Create(ClassString, IdentifierNameMatchType.EndsWith) },
            new object[] { "int", QualifiedNameSyntax.None, IdentifierNameSyntax.Create("int") },
            new object[] { "System.Net.WebRequest", QualifiedNameSyntax.Create(new[] { IdentifierNameSyntax.Create("System"), IdentifierNameSyntax.Create("Net") }), IdentifierNameSyntax.Create("WebRequest") }
        };

        [Theory]
        [MemberData(nameof(parameterizedToStringData))]
        public void CheckToString(string expectedString, QualifiedNameSyntax @namespace, IdentifierNameSyntax typeName)
        {
            var tns = TypeNameSyntax.Create(@namespace, typeName);
            Assert.Equal(expectedString, tns.ToString());
        }

        [Fact]
        public void CreateFlyweightAny()
        {
            Assert.Same(TypeNameSyntax.Any, TypeNameSyntax.Create(QualifiedNameSyntax.Any, IdentifierNameSyntax.Any));
            Assert.NotSame(TypeNameSyntax.Any, TypeNameSyntax.Create(new IdentifierNameSyntax[0]));
            Assert.Same(TypeNameSyntax.Any, TypeNameSyntax.Create(QualifiedNameSyntax.Any));
            Assert.Same(TypeNameSyntax.Any, TypeNameSyntax.Create(new[] { IdentifierNameSyntax.Any, IdentifierNameSyntax.Any }));
            Assert.NotSame(TypeNameSyntax.Any, TypeNameSyntax.Create(new[] { IdentifierNameSyntax.Any }));
        }

        [Fact]
        public void CreateFlyweightNone()
        {
            Assert.Same(TypeNameSyntax.None, TypeNameSyntax.Create(new IdentifierNameSyntax[0]));
            Assert.Same(TypeNameSyntax.None, TypeNameSyntax.Create(new[] { IdentifierNameSyntax.Any }));
            Assert.Same(TypeNameSyntax.None, TypeNameSyntax.Create(QualifiedNameSyntax.None));
            Assert.Same(TypeNameSyntax.None, TypeNameSyntax.Create(QualifiedNameSyntax.None, IdentifierNameSyntax.Any));

            Assert.NotSame(TypeNameSyntax.None, TypeNameSyntax.Create(new[] { IdentifierNameSyntax.Create("int") }));

            //JoinPointEntryFactory.CreateMethod("Namespace", "Class", "Method");
        }
    }
}