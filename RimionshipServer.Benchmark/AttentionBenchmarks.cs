using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using RimionshipServer.Services;

namespace RimionshipServer.Benchmark;

[MemoryDiagnoser]
[HtmlExporter]
[SimpleJob(RunStrategy.Throughput, RuntimeMoniker.Net60)]
public class AttentionBenchmarks
{

    private AttentionService _attention = new ();
    
    [IterationCleanup]
    public void CleanUp()
    {
        _attention.Dispose();
        _attention = new AttentionService();
    }

    [Benchmark]
    public void IncreaseIntrestScoreParallel()
    {
        Parallel.For(0, 8192, i => _attention.IncreaseAttentionScore(i.ToString(), 60));
    }
}