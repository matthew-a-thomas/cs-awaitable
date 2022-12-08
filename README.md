# Awaitable

Interfaces for custom [awaitable expressions](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#11882-awaitable-expressions).

[![GitHub Workflow Status](https://img.shields.io/github/workflow/status/matthew-a-thomas/cs-awaitable/.NET)](https://github.com/matthew-a-thomas/cs-awaitable)

[![Nuget](https://img.shields.io/nuget/v/Awaitable)](https://www.nuget.org/packages/Awaitable)

# Examples

A few examples to get your creative juices flowing:

## Awaitable `TimeSpan`

Turn any `TimeSpan` into a timer that returns itself after its amount of time
passes:

```csharp
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

readonly record struct TimeSpanAwaiter(TaskAwaiter TaskAwaiter, TimeSpan TimeSpan) : IAwaiter<TimeSpan>
{
    public TimeSpan GetResult()
    {
        TaskAwaiter.GetResult();
        return TimeSpan;
    }

    public void OnCompleted(Action continuation) => TaskAwaiter.OnCompleted(continuation);

    public void UnsafeOnCompleted(Action continuation) => TaskAwaiter.UnsafeOnCompleted(continuation);

    public bool IsCompleted => TaskAwaiter.IsCompleted;
}

static class TimeSpanExtensions
{
    public static TimeSpanAwaiter GetAwaiter(this TimeSpan timeSpan) => new(Task.Delay(timeSpan).GetAwaiter(), timeSpan);
}

[Fact]
public async Task ShouldYieldToSynchronizationContext()
{
    var timeSpan = TimeSpan.FromSeconds(1);
    var result = await timeSpan; // Takes 1 second to evaluate
    Assert.Equal(timeSpan, result);
}
```

A simpler variation of this can be found here:<br/>
https://devblogs.microsoft.com/pfxteam/await-anything/#:~:text=1%2Dline%20GetAwaiter%20method%20for%20TimeSpan

## Awaitable `CancellationToken`

Await the cancellation of any `CancellationToken`. A slight tweak will make this
work for any `WaitHandle`:

```csharp
using System;
using System.Threading;

readonly record struct WaitHandleAwaiter(WaitHandle? Handle) : IAwaiter
{
    static readonly ContextCallback InvokeActionContextCallback = state => ((Action)state!)();
    static readonly WaitOrTimerCallback InvokeActionWaitOrTimerCallback = (state, _) => ((Action)state!)();

    public bool IsCompleted => Handle?.WaitOne(0) ?? false;

    public void GetResult()
    {}

    public void OnCompleted(Action continuation)
    {
        var context = ExecutionContext.Capture();
        if (context is not null)
        {
            void WrappedContinuation() => ExecutionContext.Run(context, InvokeActionContextCallback, continuation);
            UnsafeOnCompleted(WrappedContinuation);
        }
        else
        {
            UnsafeOnCompleted(continuation);
        }
    }

    public void UnsafeOnCompleted(Action continuation)
    {
        if (Handle is null)
            return; // Missing handle is treated the same as an infinite wait. The continuation will never execute
        ThreadPool.RegisterWaitForSingleObject(
            Handle,
            InvokeActionWaitOrTimerCallback,
            continuation,
            Timeout.Infinite,
            true
        );
    }
}

static class CancellationTokenExtensions
{
    public static WaitHandleAwaiter GetAwaiter(this CancellationToken cancellationToken) =>
        cancellationToken.CanBeCanceled
            ? new WaitHandleAwaiter(cancellationToken.WaitHandle)
            : default;
}

[Fact]
public async Task ShouldAwaitCancellation()
{
    var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
    var token = cts.Token;
    await token; // Takes 1 second to evaluate
}
```

## Synchronously return 42

Jump through a lot of hoops to synchronously return a constant from an awaitable
expression:

```csharp
using System;
using System.Threading;

sealed class Awaiter<T> : IAwaiter<T>
{
    static readonly ContextCallback ExecuteContinuation = state => ((Action)state!)();

    readonly Func<T> _getResult;
    readonly Func<bool> _isCompleted;
    readonly Action<Action> _unsafeOnCompleted;

    public Awaiter(
        Func<T> getResult,
        Func<bool> isCompleted,
        Action<Action> unsafeOnCompleted)
    {
        _getResult = getResult;
        _isCompleted = isCompleted;
        _unsafeOnCompleted = unsafeOnCompleted;
    }

    public bool IsCompleted => _isCompleted();

    public T GetResult() => _getResult();

    public void OnCompleted(Action continuation)
    {
        var context = ExecutionContext.Capture();
        if (context is not null)
        {
            void WrappedContinuation() => ExecutionContext.Run(context, ExecuteContinuation, continuation);
            UnsafeOnCompleted(WrappedContinuation);
        }
        else
        {
            UnsafeOnCompleted(continuation);
        }
    }

    public void UnsafeOnCompleted(Action continuation) => _unsafeOnCompleted(continuation);
}

record Awaitable<T>(Awaiter<T> Awaiter) : IAwaitable<T, Awaiter<T>>
{
    public Awaiter<T> GetAwaiter() => Awaiter;
}

[Fact]
public async Task ShouldBeFortyTwo()
{
    var awaitableFortyTwo = new Awaitable<int>(new Awaiter<int>(
        getResult: () => 42,
        isCompleted: () => true, // Tell compiler not to bother passing us a continuation
        unsafeOnCompleted: _ => throw new NotImplementedException()
    ));
    Assert.Equal(42, await awaitableFortyTwo);
}
```

# Release history

|Version|Notes|
|-|-|
|0.3.0|Target more frameworks to reduce package dependency graph|
|0.2.0|Fix .csproj and align language features with target framework|
|0.1.2|Improve documentation|
|0.1.1|Strongly name assembly|
|0.1.0|Initial release|