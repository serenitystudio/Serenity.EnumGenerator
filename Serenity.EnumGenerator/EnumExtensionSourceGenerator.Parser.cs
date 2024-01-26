using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Serenity.EnumGenerator;

public partial class EnumExtensionSourceGenerator
{
    internal record ParseResult(string Name, string Namespace, bool HasFlagAttribute, string[] EnumMembers);

    internal class Parser
    {
        private readonly ImmutableArray<GeneratorAttributeSyntaxContext> _sources;

        public Parser(ImmutableArray<GeneratorAttributeSyntaxContext> sources)
        {
            _sources = sources;
        }

        public ParseResult[] Parse()
        {
            var list = new List<ParseResult>();

            foreach (var item in _sources)
            {
                if (item.TargetSymbol is not INamedTypeSymbol enumSymbol) continue;

                var name = enumSymbol.Name;
                var nameSpace = enumSymbol.ContainingNamespace.IsGlobalNamespace
                    ? string.Empty
                    : enumSymbol.ContainingNamespace.ToString();
                var hasFlagsAttribute = false;
                foreach (var attributeData in enumSymbol.GetAttributes())
                {
                    if (attributeData.AttributeClass?.Name is "FlagsAttribute" or "Flags" &&
                        attributeData.AttributeClass.ToDisplayString() == "System.FlagsAttribute")
                        hasFlagsAttribute = true;
                }

                var enumMembers = enumSymbol.GetMembers()
                    .Where(x => !(x is not IFieldSymbol fieldSymbol || fieldSymbol.ConstantValue is null))
                    .Select(x => x.Name).ToArray();

                var result = new ParseResult(name, nameSpace, hasFlagsAttribute, enumMembers);
                list.Add(result);
            }

            return list.ToArray();
        }
    }
}