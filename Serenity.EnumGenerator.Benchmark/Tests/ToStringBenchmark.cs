using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace Serenity.EnumGenerator.Benchmark.Tests;

[Config(typeof(BenchmarkConfig))]
public class ToStringBenchmark
{
    private static readonly TestEnum Enum = TestEnum.Second;

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public string EnumToString()
    {
        return Enum.ToString();
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public string ToStringFast()
    {
        return Enum.ToStringFast();
    }
}
