namespace Awaitable.Tests.Documentation.CancellationTokens;

using System.Threading;

static class CancellationTokenExtensions
{
    public static WaitHandleAwaiter GetAwaiter(this CancellationToken cancellationToken) =>
        cancellationToken.CanBeCanceled
            ? new WaitHandleAwaiter(cancellationToken.WaitHandle)
            : default;
}