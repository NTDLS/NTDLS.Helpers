using System.Collections.Concurrent;

namespace NTDLS.Helpers
{
    /// <summary>
    /// Executes tasks at specified intervals using a delegate-based approach.
    /// </summary>
    public class DelegateScheduler
        : IDisposable
    {
        private readonly ConcurrentDictionary<string, WorkItem> _workItems = new();

        #region WorkItem.

        private class WorkItem : IDisposable
        {
            public DelegateScheduler Scheduler { get; private set; }
            public string Id { get; private set; }
            public TimeSpan? Interval { get; private set; }
            public DateTime? At { get; private set; }

            private readonly Action _action;
            private readonly Timer _timer;
            private int _isRunning = 0;

            /// <summary>
            /// Initializes a new instance of the <see cref="WorkItem"/> class, which schedules and executes a recurring action.
            /// </summary>
            /// <remarks>The <see cref="WorkItem"/> schedules the provided action to run at regular
            /// intervals using a timer. If <paramref name="startupVariability"/> is provided, the initial delay is
            /// randomized to reduce contention caused by multiple timers starting simultaneously.</remarks>
            /// <param name="scheduler">The <see cref="DelegateScheduler"/> responsible for managing the execution of the work item. Cannot be
            /// <see langword="null"/>.</param>
            /// <param name="id">A unique identifier for the work item. This is used to distinguish the work item within the scheduler.</param>
            /// <param name="action">The action to be executed at each interval. Cannot be <see langword="null"/>.</param>
            /// <param name="interval">The time interval between consecutive executions of the action.</param>
            /// <param name="startupVariability">An optional variability applied to the initial delay before the first execution of the action. If
            /// specified, the startup delay will be randomized within a range derived from this value to avoid
            /// simultaneous execution of multiple timers. Defaults to <see cref="TimeSpan.Zero"/> if not provided.</param>
            public WorkItem(DelegateScheduler scheduler, string id, Action action, TimeSpan interval, TimeSpan? startupVariability = null)
            {
                Scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));

                if (startupVariability != null)
                {
                    var upperBound = startupVariability.Value.TotalMilliseconds;
                    var lowerBound = Math.Max(0, upperBound / 4);

                    //Sometimes we want the work item to delay the startup just a bit so that other timers don't start at the same time.
                    startupVariability = TimeSpan.FromMilliseconds(Random.Shared.Next((int)lowerBound, (int)upperBound));
                }
                else
                {
                    startupVariability = TimeSpan.Zero;
                }

                Id = id;
                Interval = interval;
                At = null; // No specific time for recurring execution.
                _action = action;
                _timer = new Timer((o) => TryRun(), null, startupVariability.Value, interval);
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="WorkItem"/> class, representing a one-time scheduled task.
            /// </summary>
            /// <remarks>This constructor schedules a one-time execution of the specified <paramref
            /// name="action"/> at the given <paramref name="at"/> time. The work item is automatically unregistered
            /// from the scheduler after execution.</remarks>
            /// <param name="scheduler">The <see cref="DelegateScheduler"/> responsible for managing the execution of this work item. Cannot be
            /// <see langword="null"/>.</param>
            /// <param name="id">The unique identifier for this work item. Used for registration and tracking within the scheduler.</param>
            /// <param name="action">The action to execute when the scheduled time is reached. Cannot be <see langword="null"/>.</param>
            /// <param name="at">The <see cref="DateTime"/> at which the action should be executed. Must be a valid future time.</param>
            public WorkItem(DelegateScheduler scheduler, string id, Action action, DateTime at)
            {
                Scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));

                Id = id;
                Interval = null; // No interval for one-time execution.
                At = at;
                _action = action;
                _timer = new Timer((o) =>
                {
                    if (DateTime.UtcNow >= At)
                    {
                        Scheduler.TryUnregister(Id); // Unregister after execution.
                        TryRun();
                    }
                }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            }

            public bool TryRun()
            {
                if (Interlocked.CompareExchange(ref _isRunning, 1, 0) == 0)
                {
                    try
                    {
                        _action();
                    }
                    catch
                    {
                        //TODO: Add some logging here? Maybe an event on the Scheduler that can be subscribed to for error handling?
                    }
                    finally
                    {
                        Interlocked.Exchange(ref _isRunning, 0);
                    }
                    return true;
                }
                return false;
            }

            public void Dispose()
            {
                GC.SuppressFinalize(this);
                _timer.Dispose();
            }
        }

        #endregion

        /// <summary>
        /// Registers a task to be run at a given interval.
        /// </summary>
        /// <param name="name">Name of the scheduled item.</param>
        /// <param name="interval">Interval at which to execute the given delegate.</param>
        /// <param name="action">Delegate to execute at the given interval.</param>
        /// <param name="startImmediately">Whether to execute the delegate immediately at creation of the timer.</param>
        /// <param name="startupVariability">Optional maximum amount of time to use for a generating a random "first start" time. This is used to assist in preventing timers with the same interval from running at the same time.</param>
        public bool TryRegister(string name, TimeSpan interval, Action action, bool startImmediately = false, TimeSpan? startupVariability = null)
        {
            if (IsRegistered(name))
                return false; // Task with this name already exists.

            var workItem = new WorkItem(this, name, action, interval, startupVariability);

            if (!_workItems.TryAdd(name, workItem))
                throw new InvalidOperationException($"A task named '{name}' is already registered.");

            if (startImmediately)
            {
                workItem.TryRun();
            }

            return true;
        }

        /// <summary>
        /// Registers an unnamed task to be run at a given interval.
        /// </summary>
        /// <param name="interval">Interval at which to execute the given delegate.</param>
        /// <param name="action">Delegate to execute at the given interval.</param>
        /// <param name="startImmediately">Whether to execute the delegate immediately at creation of the timer.</param>
        /// <param name="startupVariability">Optional maximum number of milliseconds to use for a generating a random "first start" time. This is used to assist in preventing timers with the same interval from running at the same time.</param>
        public Guid Register(TimeSpan interval, Action action, bool startImmediately = false, TimeSpan? startupVariability = null)
        {
            var id = Guid.NewGuid();
            Register(id.ToString(), interval, action, startImmediately, startupVariability);
            return id;
        }

        /// <summary>
        /// Registers an action to be executed at a specified time and returns a unique identifier for the registration.
        /// </summary>
        /// <remarks>This method generates a unique identifier for the registration and schedules the
        /// provided action to run at the specified time. Use the returned <see cref="Guid"/> to reference or manage the
        /// registration.</remarks>
        /// <param name="at">The date and time at which the action should be executed.</param>
        /// <param name="action">The action to be executed. Cannot be <see langword="null"/>.</param>
        /// <returns>A <see cref="Guid"/> representing the unique identifier for the registered action.</returns>
        public Guid Register(DateTime at, Action action)
        {
            var id = Guid.NewGuid();
            Register(id.ToString(), at, action);
            return id;
        }

        /// <summary>
        /// Registers a task to be run at a given interval.
        /// </summary>
        /// <param name="name">Name of the scheduled item.</param>
        /// <param name="interval">Interval at which to execute the given delegate.</param>
        /// <param name="action">Delegate to execute at the given interval.</param>
        /// <param name="startImmediately">Whether to execute the delegate immediately at creation of the timer.</param>
        /// <param name="startupVariability">Optional maximum amount of time to use for a generating a random "first start" time. This is used to assist in preventing timers with the same interval from running at the same time.</param>
        public void Register(string name, TimeSpan interval, Action action, bool startImmediately = false, TimeSpan? startupVariability = null)
        {
            var workItem = new WorkItem(this, name, action, interval, startupVariability);

            if (!_workItems.TryAdd(name, workItem))
                throw new InvalidOperationException($"A task named '{name}' is already registered.");

            if (startImmediately)
            {
                workItem.TryRun();
            }
        }

        /// <summary>
        /// Registers a task with the specified name, execution time, and action to be performed.
        /// </summary>
        /// <remarks>The task name must be unique within the current context. Attempting to register a
        /// task with a duplicate name will result in an exception.</remarks>
        /// <param name="name">The unique name of the task to register. Cannot be null or empty.</param>
        /// <param name="at">The date and time at which the task is scheduled to execute.</param>
        /// <param name="action">The action to be performed when the task is executed. Cannot be null.</param>
        /// <exception cref="InvalidOperationException">Thrown if a task with the specified <paramref name="name"/> is already registered.</exception>
        public void Register(string name, DateTime at, Action action)
        {
            var workItem = new WorkItem(this, name, action, at);

            if (!_workItems.TryAdd(name, workItem))
                throw new InvalidOperationException($"A task named '{name}' is already registered.");
        }

        /// <summary>
        /// Returns true if a scheduled delegate exists with the given name.
        /// </summary>
        /// <param name="name"></param>
        public bool IsRegistered(string name)
        {
            return _workItems.ContainsKey(name);
        }

        /// <summary>
        /// Removes a scheduled delegate by its name.
        /// </summary>
        public bool TryUnregister(string name)
        {
            if (_workItems.TryRemove(name, out var item))
            {
                item.Dispose();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Releases all resources used by the current instance of the class.
        /// </summary>
        /// <remarks>Call this method when you are finished using the object to release unmanaged
        /// resources and perform other cleanup operations. After calling Dispose, the object should not be
        /// used.</remarks>
        public void Dispose()
        {
            GC.SuppressFinalize(this);

            foreach (var item in _workItems.Values)
            {
                item.Dispose();
            }
            _workItems.Clear();
        }
    }
}
