using System;
using System.Collections;
using System.Collections.Generic;

namespace AspectSharp.Core.Language
{
    public sealed class ParameterListSyntax : IReadOnlyCollection<ParameterSyntax>
    {
        private readonly IReadOnlyCollection<ParameterSyntax> parameterList;

        public ParameterListSyntax(IReadOnlyCollection<ParameterSyntax> parameterList)
        {
            this.parameterList = parameterList ?? throw new ArgumentNullException(nameof(parameterList));
        }

        public static ParameterListSyntax Empty { get; } = new ParameterListSyntax(new ParameterSyntax[0]);

        public static ParameterListSyntax Any { get; } = new ParameterListSyntax(new ParameterSyntax[0]);

        public int Count => parameterList.Count;
        
        public IEnumerator<ParameterSyntax> GetEnumerator() => parameterList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator(); 
    }
}