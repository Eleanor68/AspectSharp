namespace AspectSharp.Language.Tokens

open System
open System.Reflection
open System.Collections.Generic
open AspectSharp.Language

    type AccessibilityToken =
    | Public
    | Private
    | Protected
    | Internal
    with 
        static member public Parse value = 
            match value with
            | "public" | "+"-> Public
            | "private" | "-" -> Private
            | "protected" | "#" -> Protected
            | "internal" -> Internal
            | "" | null -> Public 
            | _ -> failwith "Unknown accessibility token " + value

    type InstanceModeToken = 
    | Instance
    | Static
    | Nothing
    with 
        static member public Parse value = 
            match value with 
            | "instance" -> Instance
            | "static" -> Static
            | "" | null -> Nothing
            | _ -> failwith "Unknown instance mode token " + value

    type NameWildcards =
    | StartsWith
    | EndsWith
    | Contains

    type NameToken =
    | Fixed of string
    | Wildcards of string * NameWildcards
    | Any
    with
        static member public Parse name = 
            match name with 
            | x when x = "*" -> Any
            | x when x.StartsWith("*") && x.EndsWith("*") -> Wildcards (x.Replace("*", ""), NameWildcards.Contains)
            | x when x.StartsWith("*") -> Wildcards (x.TrimStart('*'), NameWildcards.EndsWith)
            | x when x.EndsWith("*") -> Wildcards (x.TrimEnd('*'), NameWildcards.StartsWith)
            | x -> Fixed(x)

        member public self.Name =
            match self with
            | Fixed name -> name
            | Wildcards (name, _) -> name
            | Any -> "*"

    type ClassToken = { 
        scope: NamespaceToken option
        name: NameToken
    } 
    with         
        static member public GetContext (names:string list) = 
            match String.Join(".", names) with
            | "" -> option.None 
            | x -> Some(x)

        static member public MakeFromString (name:string) = { name = NameToken.Parse name; context = option.None }

        static member public MakeFromStrings (names:string list) = { name = NameToken.Parse names.Head; context = ClassToken.GetContext names.Tail }

        static member public Make (t:Type) = { name = Fixed(t.Name); context = Some(t.Namespace) }

        override self.ToString() = (match self.context with | Some(str) -> str + "." | None -> "") + self.name.Name

    end

    type ParameterDirectionToken =
    | In
    | Out
    | Ref
    with 
        static member Default = ParameterDirectionToken.In

    type ParameterListToken = (ParameterDirectionToken option * ClassToken) list 

    type MethodToken = {
        accessibility: AccessibilityToken;
        instanceMode: InstanceModeToken;
        scope: ClassToken;
        name: NameToken;
        returnType: ClassToken
        parameterList: ParameterListToken
    }

    type PropertyAccessModeToken =
    | Get
    | Set
    | GetSet

    type PropertyToken = {
        accessibility: AccessibilityToken;
        instanceMode: InstanceModeToken;
        scope: ClassToken;
        name: NameToken;
        valueType: ClassToken
        accessMode: PropertyAccessModeToken
    }

    type CtorToken = {
        accessibility: AccessibilityToken;
        instanceMode: InstanceModeToken;
        scope: ClassToken;
        name: NameToken;
        valueType: ClassToken
    }

    type FieldToken = {
        accessibility: AccessibilityToken;
        instanceMode: InstanceModeToken;
        scope: ClassToken;
        name: NameToken;
        valueType: ClassToken
    }

//    type Target = 
//    | Method        of MethodToken
//    | Property      of PropertyToken
//    | Constructor   of CtorToken
//    | Field         of FieldToken
//    with
//        static member public MakeArguments (p:ParameterInfo array) = 
//            let rec f (p:ParameterInfo list) = 
//                match p with
//                | [] -> []
//                | head :: tail -> 
//                    let o = 
//                        if head.IsOut then Some(ParameterDirectionToken.Out)
//                        else if head.IsRetval then Some(ParameterDirectionToken.Ref)
//                        else if head.IsIn then Some(ParameterDirectionToken.In)
//                        else option.None
//
//                    (o, ClassToken.Make(head.ParameterType)) :: f tail
//
//            p |> List.ofArray |> List.rev |> f
//
//        static member public ToTarget (c:ConstructorInfo) = 
//            Constructor(Class.Make(c.DeclaringType), Target.MakeArguments(c.GetParameters()))
//
//        static member public ToTarget (p:PropertyInfo) = 
//            Property(Class.Make(p.DeclaringType), Fixed(p.Name), 
//                if p.CanRead && not p.CanWrite then PropertyAccessOption.Get
//                else if p.CanWrite && not p.CanRead then PropertyAccessOption.Set
//                else PropertyAccessOption.Auto)
//
//        static member private ArgumentsToString (a:Arguments) = 
//            let rec args (a:Arguments) =
//
//                let arg (o, c) = 
//                    let so = (match o with | Some(x) -> x.ToString() | option.None -> "")
//                    let cs = c.ToString()
//                    (so + cs)
//                    
//                match a with
//                | [] -> ""
//                | [x] -> arg x
//                | head :: tail -> args tail + "," + (arg head)
//
//            "(" + (a |> args) + ")"
//
//        override self.ToString() =
//            match self with 
//            | Method (c,n,a) -> c.name.Name + "." + n.Name + Target.ArgumentsToString(a)
//            | Property (c,n,a) -> c.name.Name + "." + n.Name + a.ToString()
//            | Constructor (c,a) -> c.name.Name + ".ctor" + Target.ArgumentsToString(a)
//    end 
//
//    type Targets = Target list

    type AfterOption = 
    | Throw
    | Return
    | Finally
    | Nothing
    with static member Default = AfterOption.Nothing
    
    type AdviceOption = 
    | WithTarget
    | WithContext
    | CanThrow
    | Nothing
    with static member Default = AdviceOption.Nothing

    type InvokeOption = 
    | Safe
    | Unsafe
    with static member Default = InvokeOption.Unsafe

    type Advice = 
    | Before of AdviceOption option
    | After  of AdviceOption option * AfterOption
    | Around of AdviceOption option

    type MethodToken = 

    //type PoincutMatch = { advice:Advice; targets:Targets }
    type PointcutPattern =
    | Constructor of CtorToken
    | Method of MethodToken
    | Property of PropertyToken
    | Field of FieldToken
    | Within of NameToken