﻿# Pointcuts Syntax

``` def
<pointcut-token> := constructor-token | property-token | method-token
```

___

## Constructors

``` def
<constructor-token> := visibility-token member-scope-token? qualified-name-token '.' (new-keyword | ctor-keyword) argument-list?
```

### Form-1

| Expression                      | Meaning                           |
| ------------------------------- | --------------------------------- |
| `public Namespace.Class(..)`    | all public constructors from Namespace.Class |
| `public Namespace.Class()`      | default constructor only from Namespace.Class |
| `public Namespace.Class(int)`   | cast constructor only from Namespace.Class |
| `public Namespace.*.(new\|ctor)` | all public constructors for all classes from namespace Namespace |

### Form-2

| Expression                      | Meaning                           |
| ------------------------------- | --------------------------------- |
| `public Namespace.Class.(new\|ctor)`       | all public constructors from Namespace.Class |
| `public Namespace.Class.(new\|ctor)(..)`   | all public constructors from Namespace.Class |
| `public Namespace.Class.(new\|ctor)()`     | default constructor only from Namespace.Class |
| `public Namespace.Class.(new\|ctor)(int)`  | cast constructor only from Namespace.Class |
| `public Namespace.*.(new\|ctor)`           | all public constructors for all classes from namespace Namespace |

### Examples

`public static AspectSharp.Skeletons.AllInOne.new()` - public static constructor from class AllInOne from namespace AspectSharp.Skeletons

`public static AspectSharp.Skeletons.AllInOne()`

___

## Properties

``` def
<property-token> := visibility-token member-scope-token? qualified-name-token '.' ('get' | 'set' | 'prop' | 'property')?
```

### Form-1

| Expression                      | Meaning                           |
| ------------------------------- | --------------------------------- |
| `public DomainObjects.Person.Name` `('.' (prop \| property))` | ?public read-write property `Name` from `DomainObjects.Person` |
| `public DomainObjects.Person.Name.get` | public get accessor for `Name` from `DomainObjects.Person` |
| `public DomainObjects.Person.Name.set` | public set accessor for `Name` from `DomainObjects.Person` |
| `public DomainObjects.Person.*.(set\|get)` | all public get or set accessors from `DomainObjects.Person` |
| `public DomainObjects.*.*.(set\|get)` | all public get or set accessors from namespace `DomainObjects` |

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

Keywords:

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

#### Whithin

`* *.*(*)`

`call(*.new()) && within(Namespace.*ss)`

`+ (Namesapce1.Class1 || Namespace2.Class2 || Class3).new()`

`*.new() && within(Namesapce1.Class1 || Namespace2.Class2 || Class3)`

`within(Namespace-token)`