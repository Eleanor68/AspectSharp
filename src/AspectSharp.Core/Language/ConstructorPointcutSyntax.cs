namespace AspectSharp.Core.Language
{
    public sealed class ConstructorPointcutSyntax : MemberPointcutSyntax
    {
        public override PointcutSyntaxKind Kind => PointcutSyntaxKind.Constructor;

        public ParameterListSyntax Parameters { get; set; }
    }
}