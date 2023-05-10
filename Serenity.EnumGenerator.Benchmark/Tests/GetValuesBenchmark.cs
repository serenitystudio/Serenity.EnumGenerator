using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace Serenity.EnumGenerator.Benchmark.Tests;

[Config(typeof(BenchmarkConfig))]
public class GetValuesBenchmark
{
    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum[] EnumGetValues()
    {
        return Enum.GetValues<TestEnum>();
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public TestEnum[] GetValuesFast()
    {
        return TestEnumExtensions.GetValuesFast();
    }
}