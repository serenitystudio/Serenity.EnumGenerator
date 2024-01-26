using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Serenity.EnumGenerator;

[Generator(LanguageNames.CSharp)]
public partial class EnumExtensionSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var source = context.SyntaxProvider.ForAttributeWithMetadataName(
            Attributes.EnumExtensionsDisplayName,
            static (node, _) => node is EnumDeclarationSyntax,
            static (context, _) => context);

        context.RegisterPostInitializationOutput((postInitializationContext) =>
        {
            postInitializationContext.AddSource($"{Attributes.EnumExtensionsName}.g.cs",
                SourceText.From(Attributes.EnumExtensionsAttribute, Encoding.UTF8));
        });
        context.RegisterSourceOutput(context.CompilationProvider.Combine(source.Collect()), Execute);
    }

    private static void Execute(SourceProductionContext context,
        (Compilation, ImmutableArray<GeneratorAttributeSyntaxContext>) tuple)
    {
        var (compilation, sources) = tuple;
        if (sources.Length == 0) return;

        var moduleName = compilation.SourceModule.Name;
        if (moduleName.StartsWith("UnityEngine.") ||
            moduleName.StartsWith("UnityEditor.") ||
            moduleName.StartsWith("Unity.")) return;

        var result = new Parser(sources).Parse();
        if (result.Length != 0)
        {
            var emitter = new Emitter(context, result);
            emitter.Emit();
        }
    }
}