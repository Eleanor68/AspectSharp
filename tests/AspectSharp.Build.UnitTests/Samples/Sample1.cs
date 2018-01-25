using System;
using AspectSharp.Core.Extensions;

namespace AspectSharp.Build.UnitTests.Samples.Sample1
{
    public class Person
    {
        public string Name { get; set; }
    }

    public class PersonExtension : /*AspectSharp.Core.Extensions.*/ExtensionBase<Person>
    {
        public PersonExtension(Person person) : base(person)
        {
        }

        public int Age { get; set; }
    }

    namespace Generated
    {
        class Person
        {
            #region AspectSharp Extensions

            //generated field
            private readonly Lazy<PersonExtension> generatedExtension;

            public Person()
            {
                //injected at the end of the existing ctor or generate one new ctor
                //if (generatedExtension == null) generatedExtension = new Lazy<PersonExtension>(() => new PersonExtension(this), true);
            }

            #endregion

            public string Name { get; set; }

            public int Age
            {
                get => generatedExtension.Value.Age;
                set => generatedExtension.Value.Age = value;
            }
        }
    }
    
}