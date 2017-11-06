namespace AspectSharp.Core.Language
{
    public sealed class ParameterSyntax
    {
        public ParameterSyntax(ParameterModifier modifier, TypeNameSyntax typeName)
        {
            Modifier = modifier;
            TypeName = typeName;
        }

        public ParameterModifier Modifier { get; set; }

        public TypeNameSyntax TypeName { get; set; }
    }
}