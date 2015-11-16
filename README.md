# NBench
Performance benchmarking and testing framework for .NET applications

## High level steps / overview
1. Warms up Benchmark
2. Runs Bmark
3. Averages perf #s
4. logs them out to console, etc


## Outputs
composite class is a composition pattern to hide that we can have multiple outputs


## writing a bmark
must write attributes for all the things you want to measure UP FRONT

## Benchmark context
only job is to grab a counter
measure app specific throughputs

## Types of tests
1. Throughput test
    1. tests chunk of code continuously for a period of time (things like calls per second)
    2. grabs data in the background
    1. set # of iterations low or it will take forever
    2. used for testing anything over time
2. Iteration test
    1. runs benchmark N number of times
    1. set # of iterations high
    2. fast iter test is for memory profiling
    3. GC => long running iteration test (inside each iteration has to big enough to create/destroy objects so that GC triggers)

## Setup/ Teardown
only one setup/cleanup method per class, but many bmarks
bmark classes are torn down & rebuilt btw every bmark iteration => only true for iteration test, or between full runs of a given throughput test (e.g. run a throughput test 4 times, setup/teardown will happen 4x)

## Test Modes
ways of running bmarks

1. Test => measurement + assertions
2. Measurement => has no assertions, just collects data

## Modes
1. RunMode
    1. how test is executed / we collect data
    2. defaults to iterations
        1. before / after test
        2. good for measuring things like memory allocation pretty precisely (in 8kb pages)
2. TestMode

## Running a performance test
1. Must have a `[PerformanceBenchmark]` attribute method
    1. can be a void w/o args, or void that takes a benchmark context
2.