﻿namespace AspectSharp.Core.Language
{
    public enum SyntaxTokenKind
    {
        None,

        Identifier,
        Dot,
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
        GetProperty,
        SetProperty,

        White,
        EndOfText
    }
}