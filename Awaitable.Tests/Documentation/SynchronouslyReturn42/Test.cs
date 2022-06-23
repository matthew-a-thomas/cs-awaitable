// ReSharper disable StaticMemberInGenericType
namespace Awaitable.Tests.Documentation.SynchronouslyReturn42;

using System;
using System.Threading.Tasks;
using Xunit;

public class Test
{
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