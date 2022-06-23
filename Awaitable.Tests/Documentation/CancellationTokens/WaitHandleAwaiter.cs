namespace Awaitable.Tests.Documentation.CancellationTokens;

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