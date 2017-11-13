namespace AspectSharp.Core.Language
{
    public sealed class PropertyPointcutSyntax : MemberPointcutSyntax
    {
        public QualifiedNameSyntax Name { get; set; }

        public TypeNameSyntax DeclaredType { get; set; }

        public bool IsGet { get; set; } = true;

        public bool IsSet { get; set; } = true;

        public override PointcutSyntaxKind Kind => IsGet && IsSet 
            ? PointcutSyntaxKind.Property 
            : IsGet 
                ? PointcutSyntaxKind.GetProperty
                : PointcutSyntaxKind.SetProperty;
    }
}