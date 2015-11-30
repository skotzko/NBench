﻿// Copyright (c) Petabridge <https://petabridge.com/>. All rights reserved.
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using NBench.Metrics;
using NBench.Reporting.Targets;
using NBench.Sdk;
using Xunit;

namespace NBench.Tests.Sdk
{
    /// <summary>
    ///     Specs designed to test <see cref="Benchmark" />s with <see cref="RunMode.Throughput"/>.
    /// </summary>
    public class BenchmarkThroughputSpecs
    {
        private static readonly CounterMetricName CounterName = new CounterMetricName("Test");
        private Counter _counter;
        private ActionBenchmarkInvoker _benchmarkMethods;

        public const int IterationSpeedMs = 30;

        public BenchmarkThroughputSpecs()
        {
            _benchmarkMethods = new ActionBenchmarkInvoker(GetType().Name, BenchmarkSetupMethod, BenchmarkTestMethod,
                ActionBenchmarkInvoker.NoOp);
        }

        public void BenchmarkSetupMethod(BenchmarkContext context)
        {
            _counter = context.GetCounter(CounterName.CounterName);
        }

        public void BenchmarkTestMethod(BenchmarkContext context)
        {
            _counter.Increment();
            {
                var bytes = new byte[1 << 13];
                bytes = null;
            }
            Thread.Sleep(IterationSpeedMs);
        }

        [Theory]
        [InlineData(3, 100)]
        [InlineData(10, 150)] // keep the values small since there's a real delay involved
        [InlineData(2, 300)] // keep the values small since there's a real delay involved
        public void ShouldComputeMetricsCorrectly(int iterationCount, int millisecondRuntime)
        {
            var assertionOutput = new ActionBenchmarkOutput(report =>
            {
                var counterResults = report.Metrics[CounterName];
                var projectedRuns = Math.Ceiling(millisecondRuntime/(double)IterationSpeedMs); // roughly the max value of this counter
                Assert.Equal(projectedRuns, counterResults.Stats.Max);
            }, results =>
            {
                var counterResults = results.Data.StatsByMetric[CounterName].Maxes.Sum;
                Assert.Equal(iterationCount, counterResults);
            });

            var counterBenchmark = new CounterBenchmarkSetting(CounterName.CounterName, AssertionType.Total, Assertion.Empty);
            var gcBenchmark = new GcBenchmarkSetting(GcMetric.TotalCollections, GcGeneration.AllGc, AssertionType.Total,
                Assertion.Empty);
            var memoryBenchmark = new MemoryBenchmarkSetting(MemoryMetric.TotalBytesAllocated, Assertion.Empty);

            var settings = new BenchmarkSettings(TestMode.Measurement, RunMode.Throughput, iterationCount, millisecondRuntime,
               new[] { gcBenchmark }, new[] { memoryBenchmark }, new[] { counterBenchmark });

            var benchmark = new Benchmark(settings, _benchmarkMethods, assertionOutput);

            benchmark.Run();
        }
    }
}

