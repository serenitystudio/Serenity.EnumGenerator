using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Serenity.EnumGenerator;

public sealed class WorkItem
{
    public readonly EnumDeclarationSyntax EnumDeclarationSyntax;
    public readonly IEnumerable<string> EnumMembers;
    public readonly bool HasFlagAttribute;

    public WorkItem(EnumDeclarationSyntax enumDeclarationSyntax)
    {
        EnumDeclarationSyntax = enumDeclarationSyntax;
        EnumMembers = EnumDeclarationSyntax.Members.Select(x => x.Identifier.ValueText);

        if (EnumDeclarationSyntax.AttributeLists.Count <= 1) return;

        var attributes = EnumDeclarationSyntax.AttributeLists.SelectMany(x => x.Attributes);
        foreach (var attributeSyntax in attributes)
        {
            var attributeName = attributeSyntax.Name.ToString();
            switch (attributeName)
            {
                case "Flags":
                case "FlagsAttribute":
                case "System.Flags":
                case "System.FlagsAttribute":
                    HasFlagAttribute = true;
                    break;
            }
        }
    }
}