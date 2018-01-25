using AspectSharp.Core.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using Xunit;

namespace AspectSharp.Build.UnitTests
{
    public class UnitTest1
    {
        public interface IPerson
        {
            string Name { get; set; }
        }

        public interface IPersonAge
        {
            int Age { get; set; }
        }

        public interface IPerson2 : IPerson, IPersonAge
        {
        }

        public interface IPersonReadOnly
        {
            string Name { get; }
        }

        public partial class Person : IPerson
        {
            public string Name { get; set; }
        }

        partial class Person : IPersonReadOnly
        {
        }

        //partial class Person
        //{
        //    [CompilerGenerated]
        //    private readonly Lazy<PersonExtension1> generatedExtension1 = new Lazy<PersonExtension1>(MakeExtension1);

        //    private PersonExtension1 MakeExtension1() => new PersonExtension1(this);

        //    public int Age
        //    {
        //        get => generatedExtension1.Age;
        //        set => generatedExtension1.Age = value;
        //    }
        //}

        public class PersonExtension1 : ExtensionBase<Person>
        {
            public PersonExtension1(Person person) : base(person)
            {
            }

            public int Age { get; set; }
        }

        public class PersonExtension2 : ExtensionBase<Person>, IPersonAge
        {
            public PersonExtension2(Person person) : base(person)
            {
            }

            public int Age { get; set; }
        }

        public class PersonExtension3 : ExtensionBase<Person>, IPerson2
        {
            public PersonExtension3(Person person) : base(person)
            {
            }

            public int Age { get; set; }

            public string Name
            {
                get => Extension.Name;
                set => Extension.Name = value;
            }
        }

        public class IPersonExtension1 : ExtensionBase<IPerson>
        {
            protected IPersonExtension1(IPerson extension) : base(extension)
            {
            }
        }

        [ExtensionConstrain("Class1 | Class2 | Class* | *:Person")]
        [ExtensionConstrain(typeof(Person), typeof(Person))]
        public class GenericExtension<T> : ExtensionBase<T>, IUniqueObject
        {
            private static int StaticId = 0;

            private readonly int id;

            protected GenericExtension(T extension) : base(extension)
            {
                InMemoryId = Interlocked.Increment(ref StaticId);
            }

            public int InMemoryId { get; private set; }
        }

        public interface IUniqueObject
        {
            int InMemoryId { get; }
        }

        [Fact]
        public async void Test1()
        {
            var sample1 = File.ReadAllText("Samples\\Sample1.cs");
            var tree = CSharpSyntaxTree.ParseText(sample1);
            var root = await tree.GetRootAsync();

            var references = new MetadataReference[]
            {
                //MetadataReference.CreateFromFile("System.dll"),
                //MetadataReference.CreateFromFile(typeof (ExtensionBase<>).Assembly.Location)
            };

            var compilation = CSharpCompilation.Create("IntermediateAssembly", references: references);
            compilation = compilation.AddSyntaxTrees(tree);

            var model = compilation.GetSemanticModel(tree);

            var findExtensions = new FindExtensions(model);

            findExtensions.Visit(root);

            //            var options = new CSharpCompilationOptions() { };
            
        }

        [Fact]
        public async void Test2()
        {
            var sample2 = File.ReadAllText("Samples\\Sample2.cs");
            var tree = CSharpSyntaxTree.ParseText(sample2);
            var root = await tree.GetRootAsync();

            var typeLocator = new TypeLocator(CancellationToken.None);
            typeLocator.Visit(root);
        }
    }
}
