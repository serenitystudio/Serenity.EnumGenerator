using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Serenity.EnumGenerator;

public partial class EnumExtensionSourceGenerator
{
    internal class Emitter
    {
        private readonly SourceProductionContext _context;
        private readonly ParseResult[] _results;
        private readonly CodeWriter _codeWriter;

        public Emitter(SourceProductionContext context, ParseResult[] results)
        {
            _context = context;
            _results = results;
            _codeWriter = new CodeWriter();
        }

        public void Emit()
        {
            foreach (var result in _results)
            {
                _codeWriter.Clear();

                var sourceText = WriteExtensions(result);
                _context.AddSource($"{result.Name}Extensions.g.cs", SourceText.From(sourceText, Encoding.UTF8));
            }
        }


        private string WriteExtensions(ParseResult result)
        {
            _codeWriter.AppendLine("using System;");
            _codeWriter.AppendLine();

            if (!string.IsNullOrEmpty(result.Namespace))
            {
                _codeWriter.AppendLine($"namespace {result.Namespace}");
                _codeWriter.BeginBlock();
            }

            _codeWriter.AppendLine($"public static partial class {result.Name}Extensions");
            _codeWriter.BeginBlock();

            AppendLength(result);
            _codeWriter.AppendLine();
            AppendToStringFast(result);
            _codeWriter.AppendLine();
            AppendValuesFast(result);
            _codeWriter.AppendLine();
            AppendNamesFast(result);
            _codeWriter.AppendLine();
            AppendIsDefinedFast(result);

            if (result.HasFlagAttribute)
            {
                _codeWriter.AppendLine();
                AppendHasFlagFast(result.Name);
            }

            if (!string.IsNullOrEmpty(result.Namespace)) _codeWriter.EndBlock();

            _codeWriter.EndBlock();
            return _codeWriter.ToString();
        }

        private void AppendLength(ParseResult e)
        {
            _codeWriter.AppendLine(
                $"public const int Length = {e.EnumMembers.Count().ToString()};");
        }

        private void AppendToStringFast(ParseResult result)
        {
            using (_codeWriter.BeginBlockScope(
                       $"public static string ToStringFast(this {result.Name} value)"))
            {
                using (_codeWriter.BeginBlockScope("return value switch", true))
                {
                    foreach (var member in result.EnumMembers)
                        _codeWriter.AppendLine($"{result.Name}.{member} => nameof({result.Name}.{member}),");

                    _codeWriter.AppendLine("_ => value.ToString()");
                }
            }
        }

        private void AppendValuesFast(ParseResult result)
        {
            using (_codeWriter.BeginBlockScope($"public static {result.Name}[] GetValuesFast()"))
            {
                using (_codeWriter.BeginBlockScope("return new[]", true))
                {
                    foreach (var member in result.EnumMembers)
                        _codeWriter.AppendLine($"{result.Name}.{member},");
                }
            }
        }

        private void AppendNamesFast(ParseResult result)
        {
            using (_codeWriter.BeginBlockScope($"public static string[] GetNamesFast()"))
            {
                using (_codeWriter.BeginBlockScope("return new[]", true))
                {
                    foreach (var member in result.EnumMembers)
                        _codeWriter.AppendLine($@"nameof({result.Name}.{member}),");
                }
            }
        }

        private void AppendIsDefinedFast(ParseResult result)
        {
            using (_codeWriter.BeginBlockScope($"public static bool IsDefinedFast({result.Name} value)"))
            {
                using (_codeWriter.BeginBlockScope("return value switch", true))
                {
                    foreach (var member in result.EnumMembers)
                        _codeWriter.AppendLine($@"{result.Name}.{member} => true,");

                    _codeWriter.AppendLine("_ => false");
                }
            }
        }

        private void AppendHasFlagFast(string enumName)
        {
            using (_codeWriter.BeginBlockScope(
                       $"public static bool HasFlagFast(this {enumName} value, {enumName} flag)"))
            {
                _codeWriter.AppendLine($"return flag == 0 ? true : (value & flag) == flag;");
            }
        }
    }
}