namespace Awaitable.Tests.Documentation.AwaitableTimeSpan;

using System;
using System.Runtime.CompilerServices;

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