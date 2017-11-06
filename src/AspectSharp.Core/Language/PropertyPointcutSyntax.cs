namespace AspectSharp.Core.Language
{
    public sealed class PropertyPointcutSyntax : MemberPointcutSyntax
    {
        public bool IsGet { get; set; } = true;

        public bool IsSet { get; set; } = true;

        public override PointcutSyntaxKind Kind => IsGet && IsSet 
            ? PointcutSyntaxKind.Property 
            : IsGet 
                ? PointcutSyntaxKind.GetProperty
                : PointcutSyntaxKind.SetProperty;
    }
}