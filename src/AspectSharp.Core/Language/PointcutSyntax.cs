namespace AspectSharp.Core.Language
{
    public abstract class PointcutSyntax
    {
        public virtual PointcutSyntaxKind Kind => PointcutSyntaxKind.Any;
    }
}