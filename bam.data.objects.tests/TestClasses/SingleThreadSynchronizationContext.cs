using System.Collections.Concurrent;

namespace Bam.Data.Objects.Tests.TestClasses;

/// <summary>
/// A synchronization context that runs every posted callback on one dedicated thread, the way a UI
/// thread or Blazor's renderer dispatcher does.  A blocking wait on that thread for an async call
/// whose continuation was posted back to this context never finishes: that is the deadlock the
/// repository's synchronous methods must not have.
/// </summary>
public sealed class SingleThreadSynchronizationContext : SynchronizationContext, IDisposable
{
    private readonly BlockingCollection<(SendOrPostCallback Callback, object? State)> _queue = new BlockingCollection<(SendOrPostCallback, object?)>();
    private readonly Thread _thread;

    public SingleThreadSynchronizationContext()
    {
        _thread = new Thread(Pump) { IsBackground = true, Name = nameof(SingleThreadSynchronizationContext) };
        _thread.Start();
    }

    /// <inheritdoc />
    public override void Post(SendOrPostCallback d, object? state)
    {
        _queue.Add((d, state));
    }

    /// <inheritdoc />
    public override void Send(SendOrPostCallback d, object? state)
    {
        throw new NotSupportedException("Send is not used by these tests.");
    }

    /// <summary>
    /// Runs <paramref name="work"/> on the context's thread and waits up to <paramref name="timeout"/>.
    /// Returns false when it did not finish in time (a deadlock), and rethrows what it threw.
    /// </summary>
    public bool Run(Action work, TimeSpan timeout)
    {
        Exception? thrown = null;
        // Not disposed: after a timeout the work may still finish later and set it.
        ManualResetEventSlim done = new ManualResetEventSlim(false);
        Post(_ =>
        {
            try
            {
                work();
            }
            catch (Exception e)
            {
                thrown = e;
            }
            finally
            {
                done.Set();
            }
        }, null);
        if (!done.Wait(timeout))
        {
            return false;
        }
        if (thrown is not null)
        {
            throw new InvalidOperationException("the work threw on the synchronization context", thrown);
        }
        return true;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _queue.CompleteAdding();
    }

    private void Pump()
    {
        SetSynchronizationContext(this);
        foreach ((SendOrPostCallback callback, object? state) in _queue.GetConsumingEnumerable())
        {
            callback(state);
        }
    }
}
