using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// Interface for event wrappers to enable cross-thread event-raising.
/// </summary>
public interface IEventWrapper
{
    void RaiseEvent();
}

public class RBEventQueue
{
    // <summary>
    /// List of all outstanding events to be fired.
    /// </summary>
    private volatile Queue<IEventWrapper> _eventQueue = new Queue<IEventWrapper>();

    /// <summary>
    /// Locked while a process is emptying the event queue.
    /// </summary>
    private readonly object _raiseLock = new object();

    /// <summary>
    /// Adds an event to the event queue.
    /// </summary>
    /// <param name="eventWrapper"></param>
    public void Enqueue(IEventWrapper eventWrapper)
    {
        lock (_raiseLock)
        {
            _eventQueue.Enqueue(eventWrapper);
        }
    }

    /// <summary>
    /// Removes all pending events from the queue.
    /// </summary>
    public void Clear()
    {
        lock (_raiseLock)
        {
            _eventQueue.Clear();
        }
    }

    /// <summary>
    /// Raises all pending events.
    /// </summary>
    public void RaiseEvents()
    {
        // check whether another process is already raising events and lock if not
        if (Monitor.TryEnter(_raiseLock))
        {
            try
            {
                // raise all events in the queue
                while (_eventQueue.Count != 0)
                {
                    // pop the first object in the queue
                    IEventWrapper eventWrapper = null;
                    lock (_raiseLock) eventWrapper = _eventQueue.Dequeue();

                    if (eventWrapper == null) return;

                    // raise the event
                    eventWrapper.RaiseEvent();
                }
            }
            finally
            {
                // release the lock, enable another task to raise events
                Monitor.Exit(_raiseLock);
            }
        }
    }
}
