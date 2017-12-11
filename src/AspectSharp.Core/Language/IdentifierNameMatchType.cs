namespace AspectSharp.Core.Language
{
    public enum IdentifierNameMatchType
    {
        Any,
        Strict,
        StartsWith,
        EndsWith,
        Contains,
        Not // - new feature `public !void Namespace.Class.*(..)`
    }
}