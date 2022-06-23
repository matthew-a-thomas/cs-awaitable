namespace Awaitable.Tests.Documentation.CancellationTokens;

using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class Test
{
    [Fact]
    public async Task ShouldAwaitCancellation()
    {
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
        var token = cts.Token;
        await token; // Takes 1 second to evaluate
    }
}