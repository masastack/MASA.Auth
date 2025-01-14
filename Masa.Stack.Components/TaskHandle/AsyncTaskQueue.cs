namespace Masa.Stack.Components.TaskHandle
{
    public class AsyncTaskQueue : IDisposable
    {
        public AsyncTaskQueue()
        {
            _autoResetEvent = new AutoResetEvent(false);
            _thread = new Thread(InternalRunning) { IsBackground = true };
            _thread.Start();
        }

        public async Task<(bool IsValid, T result)> ExecuteAsync<T>(Func<Task<T>> func)
        {
            var task = GetExecutableTask(func);
            var result = await await task;
            if (!task.IsValid)
            {
                result = default(T);
            }
            return (task.IsValid, result);
        }

        public async Task<bool> ExecuteAsync<T>(Func<Task> func)
        {
            var task = GetExecutableTask(func);
            await await task;
            return task.IsValid;
        }

        private AwaitableTask GetExecutableTask(Action action)
        {
            var awaitableTask = new AwaitableTask(new Task(action));
            AddPenddingTaskToQueue(awaitableTask);
            return awaitableTask;
        }

        private AwaitableTask<TResult> GetExecutableTask<TResult>(Func<TResult> function)
        {
            var awaitableTask = new AwaitableTask<TResult>(new Task<TResult>(function));
            AddPenddingTaskToQueue(awaitableTask);
            return awaitableTask;
        }

        private void AddPenddingTaskToQueue(AwaitableTask task)
        {
            lock (_queue)
            {
                _queue.Enqueue(task);
                _autoResetEvent.Set();
            }
        }

        private void InternalRunning()
        {
            while (!_isDisposed)
            {
                if (_queue.Count == 0)
                {
                    _autoResetEvent.WaitOne();
                }
                while (TryGetNextTask(out var task))
                {
                    if (task.NotExecutable) continue;

                    if (UseSingleThread)
                    {
                        task.RunSynchronously();
                    }
                    else
                    {
                        task.Start();
                    }
                }
            }
        }

        private AwaitableTask _lastDoingTask;
        private bool TryGetNextTask(out AwaitableTask task)
        {
            task = default;
            while (_queue.Count > 0)
            {
                if (_queue.TryDequeue(out task) && (!AutoCancelPreviousTask || _queue.Count == 0))
                {
                    _lastDoingTask?.MarkTaskValid();
                    _lastDoingTask = task;
                    return true;
                }
                task.SetNotExecutable();
            }
            return false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AsyncTaskQueue() => Dispose(false);

        private void Dispose(bool disposing)
        {
            if (_isDisposed) return;
            if (disposing)
            {
                _autoResetEvent.Dispose();
            }
            _thread = null;
            _autoResetEvent = null;
            _isDisposed = true;
        }

        public bool UseSingleThread { get; set; } = true;

        public bool AutoCancelPreviousTask { get; set; } = false;

        private bool _isDisposed;
        private readonly ConcurrentQueue<AwaitableTask> _queue = new ConcurrentQueue<AwaitableTask>();
        private Thread _thread;
        private AutoResetEvent _autoResetEvent;
    }
}