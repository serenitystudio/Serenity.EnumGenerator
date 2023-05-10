using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Serenity.EnumGenerator;

[Generator(LanguageNames.CSharp)]
public class EnumExtensionSourceGenerator : ISourceGenerator
{
    public const string NameSpace = "Serenity.EnumGenerator";
    public const string AttributeName = "EnumExtensions";

    private const string ExtensionMethodNameGetLength = "Length";
    private const string ExtensionMethodNameToString = "ToStringFast";
    private const string ExtensionMethodNameGetValues = "GetValuesFast";
    private const string ExtensionMethodNameGetNames = "GetNamesFast";
    private const string ExtensionMethodNameIsDefined = "IsDefinedFast";
    private const string ExtensionMethodNameHasFlag = "HasFlagFast";

    private const string Attribute = $@"
using System;

namespace {NameSpace}
{{
    [AttributeUsage(AttributeTargets.Enum)]
    public sealed class {AttributeName}Attribute : Attribute
    {{
    }}
}}";

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var moduleName = context.Compilation.SourceModule.Name.AsSpan();
        if (moduleName.StartsWith("UnityEngine.")) return;
        if (moduleName.StartsWith("UnityEditor.")) return;
        if (moduleName.StartsWith("Unity.")) return;

        context.AddSource($"{AttributeName}Attribute.g.cs", SourceText.From(Attribute, Encoding.UTF8));

        var syntaxReceiver = (SyntaxReceiver)context.SyntaxReceiver!;

        if (syntaxReceiver.WorkItems.Count == 0) return;

        var codeWriter = new CodeWriter();
        foreach (var workItem in syntaxReceiver.WorkItems)
        {
            var semanticModel = context.Compilation.GetSemanticModel(workItem.EnumDeclarationSyntax.SyntaxTree);
            if (semanticModel.GetDeclaredSymbol(workItem.EnumDeclarationSyntax) is not INamedTypeSymbol enumSymbol)
                continue;

            var symbol = semanticModel.GetDeclaredSymbol(workItem.EnumDeclarationSyntax);

            if (symbol == null) continue;

            var sourceText = WriteExtensions(codeWriter,
                symbol.ContainingNamespace.IsGlobalNamespace ? string.Empty : symbol.ContainingNamespace.ToString(),
                symbol.Name, workItem);
            context.AddSource($"{symbol.Name}Extensions.g.cs", SourceText.From(sourceText, Encoding.UTF8));
            codeWriter.Clear();
        }
    }

    private static string WriteExtensions(in CodeWriter codeWriter, string namespaceName, string enumName,
        WorkItem workItem)
    {
        codeWriter.AppendLine("using System;");
        codeWriter.AppendLine();

        if (!string.IsNullOrEmpty(namespaceName))
        {
            codeWriter.AppendLine($"namespace {namespaceName}");
            codeWriter.BeginBlock();
        }

        codeWriter.AppendLine($"public static partial class {enumName}Extensions");
        codeWriter.BeginBlock();

        AppendLength(codeWriter, workItem);
        codeWriter.AppendLine();
        AppendToStringFast(codeWriter, enumName, workItem);
        codeWriter.AppendLine();
        AppendValuesFast(codeWriter, enumName, workItem);
        codeWriter.AppendLine();
        AppendNamesFast(codeWriter, enumName, workItem);
        codeWriter.AppendLine();
        AppendIsDefinedFast(codeWriter, enumName, workItem);

        if (workItem.HasFlagAttribute)
        {
            codeWriter.AppendLine();
            AppendHasFlagFast(codeWriter, enumName);
        }

        if (!string.IsNullOrEmpty(namespaceName))
        {
            codeWriter.EndBlock();
        }

        codeWriter.EndBlock();
        return codeWriter.ToString();
    }

    private static void AppendLength(in CodeWriter sourceBuilder, WorkItem e)
    {
        sourceBuilder.AppendLine(
            $"public const int {ExtensionMethodNameGetLength} = {e.EnumMembers.Count().ToString()};");
    }

    private static void AppendToStringFast(in CodeWriter sourceBuilder, string enumName, WorkItem workItem)
    {
        using (sourceBuilder.BeginBlockScope(
                   $"public static string {ExtensionMethodNameToString}(this {enumName} value)"))
        {
            using (sourceBuilder.BeginBlockScope("return value switch", true))
            {
                foreach (var member in workItem.EnumMembers)
                    sourceBuilder.AppendLine($"{enumName}.{member} => nameof({enumName}.{member}),");

                sourceBuilder.AppendLine("_ => value.ToString()");
            }
        }
    }

    private static void AppendValuesFast(in CodeWriter sourceBuilder, string enumName, WorkItem workItem)
    {
        using (sourceBuilder.BeginBlockScope($"public static {enumName}[] {ExtensionMethodNameGetValues}()"))
        {
            using (sourceBuilder.BeginBlockScope("return new[]", true))
            {
                foreach (var member in workItem.EnumMembers)
                    sourceBuilder.AppendLine($"{enumName}.{member},");
            }
        }
    }

    private static void AppendNamesFast(in CodeWriter sourceBuilder, string enumName, WorkItem workItem)
    {
        using (sourceBuilder.BeginBlockScope($"public static string[] {ExtensionMethodNameGetNames}()"))
        {
            using (sourceBuilder.BeginBlockScope("return new[]", true))
            {
                foreach (var member in workItem.EnumMembers)
                    sourceBuilder.AppendLine($@"nameof({enumName}.{member}),");
            }
        }
    }

    private static void AppendIsDefinedFast(in CodeWriter sourceBuilder, string enumName, WorkItem workItem)
    {
        using (sourceBuilder.BeginBlockScope($"public static bool {ExtensionMethodNameIsDefined}({enumName} value)"))
        {
            using (sourceBuilder.BeginBlockScope("return value switch", true))
            {
                foreach (var member in workItem.EnumMembers)
                    sourceBuilder.AppendLine($@"{enumName}.{member} => true,");

                sourceBuilder.AppendLine("_ => false");
            }
        }
    }

    private static void AppendHasFlagFast(in CodeWriter sourceBuilder, string enumName)
    {
        using (sourceBuilder.BeginBlockScope(
                   $"public static bool {ExtensionMethodNameHasFlag}(this {enumName} value, {enumName} flag)"))
        {
            sourceBuilder.AppendLine($"return flag == 0 ? true : (value & flag) == flag;");
        }
    }
}