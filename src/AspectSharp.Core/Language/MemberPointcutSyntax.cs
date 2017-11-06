namespace AspectSharp.Core.Language
{
    public class MemberPointcutSyntax : PointcutSyntax
    {
        public MemberVisibility Visibility { get; set; }

        public MemberScope Scope { get; set; }

        public TypeNameSyntax Type { get; set; }

        public override PointcutSyntaxKind Kind => PointcutSyntaxKind.Member;
    }
}