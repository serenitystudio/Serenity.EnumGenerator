using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace Serenity.EnumGenerator.Benchmark.Tests;

[Config(typeof(BenchmarkConfig))]
public class GetNamesBenchmark
{
    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public string[] EnumGetNames()
    {
        return Enum.GetNames<TestEnum>();
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public string[] GetNamesFast()
    {
        return TestEnumExtensions.GetNamesFast();
    }
}