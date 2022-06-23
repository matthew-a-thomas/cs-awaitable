namespace Awaitable.Tests.Documentation.AwaitableTimeSpan;

using System;
using System.Threading.Tasks;
using Xunit;

public class Test
{
    [Fact]
    public async Task ShouldYieldToSynchronizationContext()
    {
        var timeSpan = TimeSpan.FromSeconds(1);
        var result = await timeSpan; // Takes 1 second to evaluate
        Assert.Equal(timeSpan, result);
    }
}