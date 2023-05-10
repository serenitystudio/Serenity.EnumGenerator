using FluentAssertions;
using Xunit;

namespace Serenity.EnumGenerator.Tests;

public class EnumTests
{
    [Fact]
    public void TestLength()
    {
        var lengthFast = TestEnumExtensions.Length;
        lengthFast.Should().Be(Enum.GetValues<TestEnum>().Length);
    }

    [Theory]
    [InlineData(TestEnum.First)]
    [InlineData(TestEnum.Second)]
    [InlineData(TestEnum.Third)]
    [InlineData(TestEnum.Flag)]
    [InlineData((TestEnum)5)]
    public void TestToString(TestEnum testEnum)
    {
        var toStringFast = testEnum.ToStringFast();
        toStringFast.Should().Be(testEnum.ToString());
    }

    [Fact]
    public void TestGetValues()
    {
        var getValues = Enum.GetValues<TestEnum>();
        var getValuesFast = TestEnumExtensions.GetValuesFast();
        getValuesFast.Should().BeEquivalentTo(getValues);
    }
    
    [Fact]
    public void TestGetNames()
    {
        var getNames = Enum.GetNames<TestEnum>();
        var getNamesFast = TestEnumExtensions.GetNamesFast();
        getNamesFast.Should().BeEquivalentTo(getNames);
    }
    
    [Theory]
    [InlineData(TestEnum.First)]
    [InlineData(TestEnum.Second)]
    [InlineData(TestEnum.Third)]
    [InlineData(TestEnum.Flag)]
    [InlineData((TestEnum)5)]
    public void TestIsDefined(TestEnum testEnum)
    {
        var isDefinedFast = TestEnumExtensions.IsDefinedFast(testEnum);
        isDefinedFast.Should().Be(Enum.IsDefined(testEnum));
    }
    
    [Theory]
    [InlineData(TestEnum.Flag, TestEnum.First)]
    [InlineData(TestEnum.Flag, TestEnum.Second)]
    [InlineData(TestEnum.Flag, TestEnum.Flag)]
    [InlineData(TestEnum.First, TestEnum.Flag)]
    public void TestHasFlag(TestEnum testEnum, TestEnum flag)
    {
        var hasFlagFast = testEnum.HasFlagFast(flag);
        hasFlagFast.Should().Be(testEnum.HasFlag(flag));
    }
}