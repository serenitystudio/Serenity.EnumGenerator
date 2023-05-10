using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace Serenity.EnumGenerator.Benchmark.Tests;

[Config(typeof(BenchmarkConfig))]
public class IsDefinedBenchmark
{
    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool EnumIsDefined()
    {
        return Enum.IsDefined(TestEnum.First);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool IsDefinedFast()
    {
        return TestEnumExtensions.IsDefinedFast(TestEnum.First);
    }
}