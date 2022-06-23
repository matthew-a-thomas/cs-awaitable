namespace Awaitable.Tests.Documentation.SynchronouslyReturn42;

record Awaitable<T>(Awaiter<T> Awaiter) : IAwaitable<T, Awaiter<T>>
{
    public Awaiter<T> GetAwaiter() => Awaiter;
}