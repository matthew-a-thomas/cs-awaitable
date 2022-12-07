// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global
namespace Awaitable
{
    /// <summary>
    /// The interface for an awaiter that can be returned from <see cref="IAwaitable{TAwaiter}.GetAwaiter"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Do not implement this interface explicitly.
    /// </para>
    /// </remarks>
    public interface IAwaiter : IAwaiterCore
    {
        /// <summary>
        /// Synchronously gets the result or throws an exception.
        /// </summary>
        void GetResult();
    }

    /// <summary>
    /// The interface for an awaiter that can be returned from <see cref="IAwaitable{TResult,TAwaiter}.GetAwaiter"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Do not implement this interface explicitly.
    /// </para>
    /// </remarks>
    public interface IAwaiter<out T> : IAwaiterCore
    {
        /// <summary>
        /// Synchronously gets the result or throws an exception.
        /// </summary>
        T GetResult();
    }
}