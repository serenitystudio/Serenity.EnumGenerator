namespace Serenity.EnumGenerator.Tests;

[EnumExtensions]
[Flags]
public enum TestEnum : short
{
    First = 0,
    Second = 1,
    Third = 2,
    Flag = Second | Third
}