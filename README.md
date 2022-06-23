# Awaitable

Interfaces for custom [awaitable expressions](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#11882-awaitable-expressions).

[![GitHub Workflow Status](https://img.shields.io/github/workflow/status/matthew-a-thomas/cs-awaitable/.NET)](https://github.com/matthew-a-thomas/cs-awaitable)

[![Nuget](https://img.shields.io/nuget/v/Awaitable)](https://www.nuget.org/packages/Awaitable)

# Examples

## Synchronously return 42

```csharp
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
|0.1.0|Initial release|
|0.1.1|Strongly sign assembly|