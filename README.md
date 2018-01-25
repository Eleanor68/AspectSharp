# AspectSharp

This is a __PROOF OF CONCEPT__ project which intends to enable the Aspect-Oriented Programming (AOP) for C#.
# The Aspect-Oriented Framework

[Pointcuts Syntax](./docs/pointcuts.md)

# CodeGen and Meta-Programming

## Extend instance class

``` C#
public class Class
{
	public int Property { get; set; }
	public void Method() {  }
}

public class ClassExtension : ExtensionBase<Class>
{
	public ClassExtension(Class extension) : base (extension) {  }
	
	public int NewProperty { get; set; }
	
	public void NewMethod() 
	{
		Extension.Method();
	}
}

//somewhere in the source code
var c = new Class();
c.Property = 0;

//magic happens behind the scene
c.NewProperty = 0;
c.NewMethod();
```

The power of this idea is the integration with Design-Time and Background build from Visual Studio. This will allow the developer to see live changes in the Visual Studio.

### How does it work? It is simple ...

1) The developer creates a new extension class with ExtensionBase as a base class.
2) Magic happens behind the scene (actually magic will happen in AspectSharp.targets and AspectSharp.Build.dll).
3) The developer sees all the new properties/methods right to the extended class live in Visual Studio.

### The magic

We are extending the `MSBuild` build process with our targets from AspectSharp.targets which will be applied right before compile. Our targets will find all extensions and then all extended classes. Based on the extensions new extended classes will be generated and included into the compiled files. Respectively original extended classes will be excluded from the compiled files. Therefore the generated extended classes will be included in the output assembly.

### Visual Studio Design-Time build

# Contributing

Anyone is welcome!

# License

AspectSharp is distributed under the [GPL-3.0](./LICENSE) License.