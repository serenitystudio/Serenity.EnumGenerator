using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace Serenity.EnumGenerator.Benchmark.Tests;

[Config(typeof(BenchmarkConfig))]
public class HasFlagBenchmark
{
    private static readonly TestEnum Enum = TestEnum.Flag;
    private const int Count = 1000;

    [Benchmark(Baseline = true)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool HasFlag()
    {
        var flag = false;
        for (var i = 0; i < Count; i++)
        {
            flag = Enum.HasFlag(TestEnum.First);
        }

        return flag;
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public bool HasFlagFast()
    {
        var flag = false;
        for (var i = 0; i < Count; i++)
        {
            flag = Enum.HasFlagFast(TestEnum.First);
        }

        return flag;
    }
}