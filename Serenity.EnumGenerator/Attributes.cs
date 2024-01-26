namespace Serenity.EnumGenerator;

public static class Attributes
{
    private const string NameSpace = "Serenity.EnumGenerator";
    public const string EnumExtensionsName = "EnumExtensionsAttribute";
    public const string EnumExtensionsDisplayName = $"{NameSpace}.{EnumExtensionsName}";

    public const string EnumExtensionsAttribute = $$"""
                                                    using System;

                                                    namespace {{NameSpace}}
                                                    {
                                                        [AttributeUsage(AttributeTargets.Enum)]
                                                        public sealed class {{EnumExtensionsName}} : Attribute
                                                        {
                                                        }
                                                    }
                                                    """;
}