# Pointcuts Syntax

``` def
<pointcut-token> := constructor-token | property-token | method-token
```

___

## Constructors

``` def
<constructor-token> := visibility-token? member-scope-token? qualified-name-token '.' (new-keyword | ctor-keyword) argument-list?
```

### Constructor pointcut expression ✔✘

|    | Expression                      | Meaning                           |
| -- | ------------------------------- | --------------------------------- |
| ✔ | `public Namespace.Class.new`                    | all public constructors from Namespace.Class |
| ✔ | `public Namespace.Class.new()`                  | default constructor from Namespace.Class |
| ✔ | `public Namespace.Class.new(..)`                | all public constructors from Namespace.Class with any number of parameters |
| ✔ | `public Namespace.Class.new(*)`                 | all public constructors from Namespace.Class with one parameter of any type |
| ✔ | `public Namespace.Class.new(int)`               | cast constructor from Namespace.Class with one parameter of `int` type |
| ✔ | `public Namespace.Class.new(ref int)`           | cast constructor from Namespace.Class with one `ref` paratemer of `int` type |
| ✔ | `public Namespace.Class.new(out int)`           | cast constructor from Namespace.Class with one `out` parameter of `int` type |
| ✔ | `public Namespace.Class.new(out *)`             | cast constructor from Namespace.Class with one `out` parameter of any type |
| ✔ | `public Namespace.Class.new(Namespace.Class)`   | copy constructor from Namespace.Class |
| ✘ | `public Namespace.Class.new(int[])`             | constructor from Namespace.Class |
| ✘ | `public Namespace.Class.new(IEnumerable<int>)`  | constructor from Namespace.Class |

#### Notes

- The default value of visibility is `public` so it could be skipped 
- The keyword `ctor` is an alias for `new` keyword 

### Examples

`public static AspectSharp.Skeletons.AllInOne.new()` - public static constructor from class AllInOne from namespace AspectSharp.Skeletons

___

## Properties

``` def
<property-token> := visibility-token? member-scope-token? qualified-name-token? qualified-name-token '.' ('get' | 'set' | 'prop' | 'property')
<property-token> := visibility-token? member-scope-token? qualified-name-token '.' ('get' | 'set' | 'prop' | 'property') -> qualified-name-token
```

### Property pointcut expression (Form-1) ✔

|    | Expression                      | Meaning                           |
| -- | ------------------------------- | --------------------------------- |
| ✔ | `public string DomainObjects.Person.Name.property` | public read-write property `Name` of `string` type from `DomainObjects.Person` |
| ✔ | `public DomainObjects.Person.Name.property` | public read-write property `Name` of any type from `DomainObjects.Person` |
| ✔ | `DomainObjects.Person.*.property` | all public read-write properties of any type from `DomainObjects.Person` |
| ✔ | `public * DomainObjects.Person.Name.property` | public read-write property `Name` of any type from `DomainObjects.Person` |
| ✔ | `public * Person.*.property` | public read-write property `Name` of any type from class `Person` |
| ✔ | `public Person.*.property` | public read-write property `Name` of any type from class `Person` |
| ✔ | `public DomainObjects.*.*.property` | all public read-write properties of any types from namespace `DomainObjects` |

### Property pointcut expression (Form-2) ✘

|    | Expression                      | Meaning                           |
| -- | ------------------------------- | --------------------------------- |
| ✘ | `public DomainObjects.Person.Name.property -> string` | public read-write property `Name` of `string` type from `DomainObjects.Person` |
| ✘ | `public DomainObjects.Person.Name.property -> *` | public read-write property `Name` of any type from `DomainObjects.Person` |
| ✘ | `public Person.*.property -> *` | public read-write property `Name` of any type from class `Person` |
| ✘ | `public DomainObjects.*.*.property -> string` | all public read-write properties of type string from namespace `DomainObjects` |

#### Notes

- The default value of visibility is `public` so it could be skipped 
- The `prop` keyword is an alias for `property` keyword
- Replace `property` with `get` to target only property get accessor
- Replace `property` with `set` to target only property set accessor
- The property type can be omitted
- You can target only `static` | `instance` properties by specifying it between visibility and property type

___

## Methods

``` def
<method-token> := visibility-token? member-scope-token? qualified-name-token argument-list
```

### Method pointcut expression (Form-1) ✔✘

|    | Expression                      | Meaning                           |
| -- | ------------------------------- | --------------------------------- |
| ✔ | `public void Namespace.Class.Method()` |  |
| ✔ | `public Class.Method()` |  |
| ✔ | `public Class.Method(..)` |  |
| ✔ | `public Class.Method(*)` |  |
| ✔ | `public Class.Method(*, *)` |  |
| ✔ | `public Class.Method(int)` |  |
| ✔ | `public Class.Method(Class)` |  |
| ✔ | `public Class.Method(out int)` |  |
| ✔ | `public Class.Method(ref int)` |  |
| ✔ | `public Class.Method(ref *)` |  |
| ✘ | `public Class.Method(int[])` |  |
| ✘ | `public Class.Method(IEnumerable<int>)` |  |

### Method pointcut expression (Form-2) ✘

|    | Expression                      | Meaning                           |
| -- | ------------------------------- | --------------------------------- |
| ✘ | `public Namespace.Class.Method() -> void` |  |


#### Notes

- The default value of visibility is `public` so it could be skipped 
- The return type can be omitted 
- You can target only `static` | `instance` methods by specifying it between visibility and return type

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
<visibility-token> := 'public' | 'private' | 'protected' | 'internal' | 'protected internal' | '+' | '-' | '#'

<identifier-token> := (w | W | '_')+ (w | W | d | '_')*
<identifier-name-token> := identifier-token | ('*' identifier-token) | (identifier-token '*') | ('*' identifier-token '*') | '*'
<qualified-name-token> := identifier-name-token | (identifier-name-token '.' identifier-name-token)+

<argument-list> := {to be completed}
```

___

## Conflicts

None

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