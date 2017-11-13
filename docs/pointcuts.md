# Pointcuts Syntax

``` def
<pointcut-token> := constructor-token | property-token | method-token
```

___

## Constructors

``` def
<constructor-token> := visibility-token member-scope-token? fully-qualified-name-token '.' (new-keyword | ctor-keyword) argument-list?
```

### Constructor Form-1

|     | Expression                      | Meaning                           |
| --- | ------------------------------- | --------------------------------- |
| [x] | `public Namespace.Class.new`                    | all public constructors from Namespace.Class |
| [x] | `public Namespace.Class.new()`                  | default constructor from Namespace.Class |
| [x] | `public Namespace.Class.new(..)`                | all public constructors from Namespace.Class with any number of parameters |
| [x] | `public Namespace.Class.new(*)`                 | all public constructors from Namespace.Class with one parameter of any type |
| [x] | `public Namespace.Class.new(int)`               | cast constructor from Namespace.Class with one parameter of `int` type |
| [x] | `public Namespace.Class.new(ref int)`           | cast constructor from Namespace.Class with one `ref` paratemer of `int` type |
| [x] | `public Namespace.Class.new(out int)`           | cast constructor from Namespace.Class with one `out` parameter of `int` type |
| [x] | `public Namespace.Class.new(out *)`             | cast constructor from Namespace.Class with one `out` parameter of any type |
| [x] | `public Namespace.Class.new(Namespace.Class)`   | copy constructor from Namespace.Class |
| [ ] | `public Namespace.Class.new(int[])`             | constructor from Namespace.Class |
| [ ] | `public Namespace.Class.new(IEnumerable<int>)`  | constructor from Namespace.Class |

> The `new` keyword can be replaced with `ctor`

### Examples

`public static AspectSharp.Skeletons.AllInOne.new()` - public static constructor from class AllInOne from namespace AspectSharp.Skeletons

___

## Properties

``` def
<property-token> := visibility-token? member-scope-token? fully-qualified-name-token? fully-qualified-name-token '.' ('get' | 'set' | 'prop' | 'property')
<property-token> := visibility-token? member-scope-token? fully-qualified-name-token '.' ('get' | 'set' | 'prop' | 'property') -> fully-qualified-name-token
```

### Property Form-1

|     | Expression                      | Meaning                           |
| --- | ------------------------------- | --------------------------------- |
| [ ] | `public string DomainObjects.Person.Name.property` | public read-write property `Name` of `string` type from `DomainObjects.Person` |
| [ ] | `public DomainObjects.Person.Name.property` | public read-write property `Name` of any type from `DomainObjects.Person` |
| [ ] | `public * DomainObjects.Person.Name.property` | public read-write property `Name` of any type from `DomainObjects.Person` |
| [ ] | `public * Person.*.property` | public read-write property `Name` of any type from class `Person` |
| [ ] | `public Person.*.property` | public read-write property `Name` of any type from class `Person` |
| [ ] | `public DomainObjects.*.*.property` | all public read-write properties of any types from namespace `DomainObjects` |

### Property Form-2 [ ]

|     | Expression                      | Meaning                           |
| --- | ------------------------------- | --------------------------------- |
| [ ] | `public DomainObjects.Person.Name.property -> string` | public read-write property `Name` of `string` type from `DomainObjects.Person` |
| [ ] | `public DomainObjects.Person.Name.property -> *` | public read-write property `Name` of any type from `DomainObjects.Person` |
| [ ] | `public Person.*.property -> *` | public read-write property `Name` of any type from class `Person` |
| [ ] | `public DomainObjects.*.*.property -> string` | all public read-write properties of type string from namespace `DomainObjects` |

* The `visibility` keyword can be omited and in this case the default value will be `public`
* The `property` keyword has an alias `prop`
* If we replace `property` with `get` the pointcut will target only property get accessor
* If we replace `property` with `set` the pointcut will target only property set accessor

___

## Methods

``` def
<method-token> := visibility-token member-scope-token? qualified-name-token argument-list
```

### Form-1

`public Namespace.Class.Method()`

___

## Base tokens

``` def
<w> := ['a'-'z']+
<W> := ['A'-'Z']+
<d> := ['0'-'9']+
```

### Keywords

``` def
<new-keyword> := 'new'
<ctor-keyword> := 'ctor'
```

``` def
<member-scope-token> := 'instance' | 'static'
<visibility-token> := 'public' | 'private' | 'protected' | 'internal' | 'protected internal'

<identifier-token> := (w | W | '_')+ (w | W | d | '_')*
<qualified-name-token> := identifier-token | ('*' identifier-token) | (identifier-token '*') | ('*' identifier-token '*') | '*'
<fully-qualified-name-token> := qualified-name-token | (qualified-name-token '.' qualified-name-token)+

<argument-list> := {to be completed}
```
___

## Conflicts

Conflict 1: Constructor vs Method

* `public Namespace1.Namespace2.Class()`
* `public Namespace1.Namespace2.Class.Method()`

Solution 1: Introduce class closure

* `public class(Namespace1.Namespace2.Class)()`
* `public class(Namespace1.Namespace2.Class).Method(..)`

Solution 2: Allow only constructor definitions based on ('.' 'new') ending. IMHO we will loose constructor def elegance.

___

## Ideas

Specifiy sub-namespaces by using `..` :

``` def
?? <namespace-token> := (name-token '..') | (name-token+ '.')
```

`* *.*(*)`

`call(*.new()) && within(Namespace.*ss)`

`+ (Namesapce1.Class1 || Namespace2.Class2 || Class3).new()`

* `public static * Namespace.Class.*` - all public static members
* `public instance * Namespace.Class.*` - all public instance members
* `public * Namespace.Class.*` - all public members

* `int Namespace.Class.(Property)`  - points to public Property get/set
* `int Namespace.Class.*.get` - all public get accessors
* `int Namespace.Class.*.set` - all public set accessors
* `int get(Namesapce.Class.*)` - all public get accessors log form
* `int set(Namesapce.Class.*)` - all public set accessors log form

* `int Namesapce.Class.Property.get - points only to get accessor of propert Property`
* `int get(Namesapce.Class.Property)`
* `int get(*.*)`

* `int Namesapce.Class.Property.set - points only to set accessor of propert Property`
* `int set(Namesapce.Class.Property)`

### Long form

* `+ member(*,*)`
* `+ member(Namespace, *)`
* `+ member(Namespace, Class)`
* `+ member(Namespace.., *)`
* `+ class(Namespace.., *)`
* `+ class(*, class_name)`
* `+ within(Namespace.Class)`
* `+ within(Namespace..)`
* `+ within(namespace(namespace_name))`
* `+ within(class(*, class_name))`
* `+ namespace(Namespace).class(*).member(*)`

`public void Namespace.Class.(*,*)`

`public void Namespace.Class.Dispose()`

#### Whithin class or namespace

`*.new() && within(Namesapce1.Class1 || Namespace2.Class2 || Class3)`

`within(Namespace-token | Class-token)` implicit class or namespace detection?

`whithin(class, Namespace.Class)` vs `class(Namespace.Class)`

`whithin(namespace, Namespace)` vs `namespace(Namespace)`

#### Attribute

`attribute(class, SerializableAttribute)`

`attribute(member, LengthAttribute)`

`attribute(LengthAttribute)` - defined at class or member level

#### Nested class

`nested(Namespace.Class.NestedClass)` vs `nested(*.NestedClass)` vs `nested(Namespace.*.NestedClass)`

`public * *.get & nested(NestedClass)` - all read-only properties from nested class `NestedClass`

`public * *.get & nested(Class.NestedClass)` - all read-only properties from nested class `NestedClass` from `Class`

#### Aspect scope

``` C#
[Aspect(Whitin="public class *.Class")]
public class Aspect
{
    [Before("public * *.get")]
    public void Before()
    {
        //applied only to get properties from class Class
    }
}
```

``` C#
[Aspect("Namespace.Class")]
public class Aspect
{
    [Before("public * *.get")]
    public void Before()
    {
        //applied only to get properties from class Class
    }
}
```

``` C#
[Aspect("whithin(Namespace..)")]
public class Aspect
{
    [Before("public * *.get")]
    public void Before()
    {
        //applied only to get properties from class Class
    }
}
```

#### Extend instance class

``` C#
public class Class
{
    public int Property { get; set; }
}

[Extend("Namespace.Class")]
public class ClassExtension
{
    public int NewProperty { get; set; }

    public void NewMethod () { }
}

//somewhere in the source code
var c = new Class();
c.Property = 0;
c.NewProperty = 0;
c.NewMethod();
```