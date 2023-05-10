using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Serenity.EnumGenerator;

public sealed class SyntaxReceiver : ISyntaxReceiver
{
    public List<WorkItem> WorkItems { get; } = new();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (TryGetWorkItem(syntaxNode, out var workItem))
        {
            WorkItems.Add(workItem);
        }
    }

    private static bool TryGetWorkItem(SyntaxNode syntax, out WorkItem? workItem)
    {
        if (syntax is EnumDeclarationSyntax { AttributeLists.Count: > 0 } enumDeclarationSyntax)
        {
            var attributes = enumDeclarationSyntax.AttributeLists.SelectMany(x => x.Attributes);
            foreach (var attributeSyntax in attributes)
            {
                var attributeName = attributeSyntax.Name.ToString();
                switch (attributeName)
                {
                    case EnumExtensionSourceGenerator.AttributeName:
                    case $"{EnumExtensionSourceGenerator.AttributeName}Attribute":
                    case $"{EnumExtensionSourceGenerator.NameSpace}.{EnumExtensionSourceGenerator.AttributeName}":
                    case $"{EnumExtensionSourceGenerator.NameSpace}.{EnumExtensionSourceGenerator.AttributeName}Attribute":
                        workItem = new WorkItem(enumDeclarationSyntax);
                        return true;
                }
            }
        }

        workItem = null;
        return false;
    }
}