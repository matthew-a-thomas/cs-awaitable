// ReSharper disable StaticMemberInGenericType
namespace Awaitable.Tests;

using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class Documentation
{
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
            isCompleted: () => true,
            unsafeOnCompleted: _ => throw new NotImplementedException()
        ));
        Assert.Equal(42, await awaitableFortyTwo);
    }
}