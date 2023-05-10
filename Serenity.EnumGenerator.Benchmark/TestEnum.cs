namespace Serenity.EnumGenerator.Benchmark;

[EnumExtensions]
[Flags]
public enum TestEnum
{
    First = 0,
    Second = 1,
    Third = 2,
    Flag = Second | Third
}