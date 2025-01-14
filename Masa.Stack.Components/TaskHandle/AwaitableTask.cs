namespace Masa.Stack.Components.TaskHandle
{
    public class AwaitableTask
    {
        public bool NotExecutable { get; private set; }

        public void SetNotExecutable()
        {
            NotExecutable = true;
        }

        public bool IsValid { get; private set; } = true;

        public void MarkTaskValid()
        {
            IsValid = false;
        }

        private readonly Task _task;

        public AwaitableTask(Task task) => _task = task;

        public bool IsCompleted => _task.IsCompleted;

        public int TaskId => _task.Id;

        public void Start() => _task.Start();

        public void RunSynchronously() => _task.RunSynchronously();

        public TaskAwaiter GetAwaiter() => new(this);

        public readonly struct TaskAwaiter : INotifyCompletion
        {
            private readonly AwaitableTask _task;

            public TaskAwaiter(AwaitableTask awaitableTask) => _task = awaitableTask;

            public bool IsCompleted => _task._task.IsCompleted;

            public void OnCompleted(Action continuation)
            {
                var This = this;
                _task._task.ContinueWith(t =>
                {
                    if (!This._task.NotExecutable) continuation?.Invoke();
                });
            }

            public void GetResult() => _task._task.Wait();
        }
    }

    public class AwaitableTask<TResult> : AwaitableTask
    {
        private readonly Task<TResult> _task;

        public AwaitableTask(Task<TResult> task) : base(task) => _task = task;

        public new TaskAwaiter GetAwaiter() => new TaskAwaiter(this);

        public new readonly struct TaskAwaiter : INotifyCompletion
        {
            private readonly AwaitableTask<TResult> _task;

            public TaskAwaiter(AwaitableTask<TResult> awaitableTask) => _task = awaitableTask;

            public bool IsCompleted => _task._task.IsCompleted;

            public void OnCompleted(Action continuation)
            {
                var This = this;
                _task._task.ContinueWith(t =>
                {
                    if (!This._task.NotExecutable) continuation?.Invoke();
                });
            }

            public TResult GetResult() => _task._task.Result;
        }
    }
}
