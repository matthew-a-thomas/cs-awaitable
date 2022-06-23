// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMemberInSuper.Global
namespace Awaitable;

/// <summary>
/// The interface for something that can be awaited and returns nothing.
/// </summary>
/// <remarks>
/// <para>
/// Do not implement this interface explicitly.
/// </para>
/// </remarks>
public interface IAwaitable<out TAwaiter>
where TAwaiter : IAwaiter
{
    /// <summary>
    /// Gets the awaiter.
    /// </summary>
    TAwaiter GetAwaiter();
}

/// <summary>
/// The interface for something that can be awaited and returns an instance of <typeparamref name="TResult"/>.
/// </summary>
/// <remarks>
/// <para>
/// Do not implement this interface explicitly.
/// </para>
/// </remarks>
public interface IAwaitable<TResult, out TAwaiter>
where TAwaiter : IAwaiter<TResult>
{
    /// <summary>
    /// Gets the awaiter.
    /// </summary>
    TAwaiter GetAwaiter();
}