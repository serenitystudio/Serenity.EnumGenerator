using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace Serenity.EnumGenerator.Benchmark.Tests;

[Config(typeof(BenchmarkConfig))]
public class LengthBenchmark
{
    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public int EnumLength()
    {
        return Enum.GetValues<TestEnum>().Length;
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public int LengthFast()
    {
        return TestEnumExtensions.Length;
    }
}