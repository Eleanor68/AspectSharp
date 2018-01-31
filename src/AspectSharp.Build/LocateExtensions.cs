using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspectSharp.Build
{
    public sealed class LocateExtensions : Microsoft.Build.Utilities.Task, ICancelableTask
    {
        private const string StubsFolderName = "AspectSharp.Extensions";

        private readonly CancellationTokenSource cts;
        private readonly CancellationToken CancellationToken;

        public LocateExtensions()
        {
            cts = new CancellationTokenSource();
            CancellationToken = cts.Token;
        }

        [Required]
        public string AssemblyName { get; set; }

        [Required]
        public string IntermediateOutputPath { get; set; }

        [Required]
        public ITaskItem[] Files { get; set; }

        /// <remarks>
        /// 1. Do fast processing for DesignTimeBuild
        /// 2. Generate only methods/properties stubs otherwise we will break the IntelliSense
        /// 3. Stubs should be generated only with empty body
        /// </remarks>
        public bool DesignTimeBuild { get; set; }

        [Output]
        public ITaskItem[] RemoveFiles { get; set; }

        [Output]
        public ITaskItem[] GeneratedFiles { get; set; }

        public void Cancel()
        {
            cts.Cancel();
        }

        private static async Task<SyntaxNode> ParseText(string f, CancellationToken cancellationToken)
        {
            using (var fs = File.OpenRead(f))
            {
                var source = SourceText.From(fs, canBeEmbedded: false);
                var syntaxTree = CSharpSyntaxTree.ParseText(source, cancellationToken: cancellationToken);
                return await syntaxTree.GetRootAsync(cancellationToken);
            }
        }

        public override bool Execute()
        {
            Log.LogMessage($"LocateExtensions: HostObject = {HostObject?.GetType().Name}, BuildEngine = {BuildEngine?.GetType().Name}");
            Log.LogMessage($"LocateExtensions: Files = {{{ string.Join("; ", Files.Select(i => i.ItemSpec)) }}}");

            var stubsPath = Path.Combine(IntermediateOutputPath, StubsFolderName);
            Directory.CreateDirectory(stubsPath);

            var aCs = Path.Combine(stubsPath, "a.cs");
            File.WriteAllText(aCs, "namespace X { public class A {  } }");

            var bCs = Path.Combine(stubsPath, "class3.g.cs");
            File.WriteAllText(bCs, @"namespace AspectSharp.Build.Tests
{
    public class Class3
    {
        public static void Z() {  }
        public static void Y() {  }
    }
}");

            var typeLocator = new TypeLocator(CancellationToken);

            foreach (var f in Files)
            {
                if (CancellationToken.IsCancellationRequested) return false;
                var rootNode = ParseText(f.ItemSpec, CancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
                typeLocator.Visit(rootNode);
            }

            var msg = string.Join("; ", typeLocator.Types.Select(t => t.Identifier.Text));
            Log.LogMessage($"LocateExtensions: Project Types = {{{ msg }}}");

            GeneratedFiles = new[] { new TaskItem(aCs), new TaskItem(bCs) };

            //RemoveFiles = Array.Empty<ITaskItem>();
            RemoveFiles = new[] { new TaskItem("Class3.cs") };

            return true;
        }
    }

    public sealed class TypeLocator : CSharpSyntaxWalker
    {
        private readonly CancellationToken cancellationToken;

        private IList<TypeDeclarationSyntax> types = new List<TypeDeclarationSyntax>();
        
        public IEnumerable<TypeDeclarationSyntax> Types => types;

        public TypeLocator(CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;
        }

        public override void DefaultVisit(SyntaxNode node)
        {
            if (cancellationToken.IsCancellationRequested) return;
            base.DefaultVisit(node);
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            if (cancellationToken.IsCancellationRequested) return;
            types.Add(node);
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            if (cancellationToken.IsCancellationRequested) return;
            types.Add(node);
        }

        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            if (cancellationToken.IsCancellationRequested) return;
            types.Add(node);
        }

        public override void Visit(SyntaxNode node)
        {
            if (cancellationToken.IsCancellationRequested) return;
            base.Visit(node);
        }
    }

    public sealed class FindExtensions : CSharpSyntaxWalker
    {
        private readonly SemanticModel semanticModel;

        private IList<TypeDeclarationSyntax> types = new List<TypeDeclarationSyntax>();

        public FindExtensions(SemanticModel semanticModel)
        {
            this.semanticModel = semanticModel;
        }

        public override void VisitDeclarationExpression(DeclarationExpressionSyntax node)
        {
            base.VisitDeclarationExpression(node);
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            if (node.BaseList != null)
            {

            }
            else
            {

            }

            //node.BaseList.Types

            base.VisitClassDeclaration(node);
        }

        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            base.VisitStructDeclaration(node);
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            base.VisitInterfaceDeclaration(node);
        }

        public override void VisitBaseList(BaseListSyntax node)
        {
            for (int i = 0; i < node.Types.Count; i++)
            {
                var t = node.Types[i];
                if (t.IsMissing) continue;

                GenericNameSyntax extensionBaseType;

                if (t.Type.IsKind(SyntaxKind.QualifiedName))
                {
                    var qualifiedName = t.Type as QualifiedNameSyntax;
                    if (!qualifiedName.Right.IsKind(SyntaxKind.GenericName)) continue;

                    extensionBaseType = qualifiedName.Right as GenericNameSyntax;
                }
                else if (t.Type.IsKind(SyntaxKind.GenericName))
                {
                    extensionBaseType = t.Type as GenericNameSyntax;
                }
                else continue;

                if (extensionBaseType.Identifier.ValueText != "ExtensionBase") continue;

                var extensionBaseTypeSymbol = semanticModel.GetTypeInfo(t.Type);
            }
        }
    }
}
