namespace Awaitable.Tests.Documentation.AwaitableTimeSpan;

using System;
using System.Threading.Tasks;

static class TimeSpanExtensions
{
    public static TimeSpanAwaiter GetAwaiter(this TimeSpan timeSpan) => new(Task.Delay(timeSpan).GetAwaiter(), timeSpan);
}