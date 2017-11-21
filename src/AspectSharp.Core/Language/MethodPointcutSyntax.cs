namespace AspectSharp.Core.Language
{
    public sealed class MethodPointcutSyntax : MemberPointcutSyntax
    {
        public override PointcutSyntaxKind Kind => PointcutSyntaxKind.Method;

        public IdentifierNameSyntax MethodName { get; set; }

        public TypeNameSyntax ReturnType { get; set; }

        public ParameterListSyntax Parameters { get; set; }
    }
}