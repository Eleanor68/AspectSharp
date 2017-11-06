namespace AspectSharp.Core.Language
{
    public enum SyntaxTokenKind
    {
        None,

        Identifier,
        Dot,
        Comma,
        Not,
        LeftP,
        RightP,
        Plus,
        Minus,
        Times,
        Hash,
        And,
        Or,
        Public,
        Private,
        Protected,
        Internal,
        Instance,
        Static,
        Class,
        New,
        Ctor,
        GetProperty,
        SetProperty,
        Out,
        Ref,


        White,
        EndOfText
    }
}