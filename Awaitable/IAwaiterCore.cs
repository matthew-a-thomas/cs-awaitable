// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global
namespace Awaitable;

using System.Runtime.CompilerServices;

/// <summary>
/// Properties and methods that all awaiters must implement.
/// </summary>
/// <remarks>
/// <para>
/// Do not implement this interface explicitly.
/// </para>
/// </remarks>
public interface IAwaiterCore : ICriticalNotifyCompletion
{
    /// <summary>
    /// Returns <c>false</c> if the result is not yet ready and a continuation should be given to
    /// <see cref="INotifyCompletion.OnCompleted"/> or <see cref="ICriticalNotifyCompletion.UnsafeOnCompleted"/>.
    /// </summary>
    bool IsCompleted { get; }
}