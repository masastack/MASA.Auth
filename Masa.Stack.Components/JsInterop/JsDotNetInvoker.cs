namespace Masa.Stack.Components.JsInterop;

public class JsDotNetInvoker : IDisposable, IScopedDependency
{
    private readonly IJSRuntime _jsRuntime;
    private readonly List<DotNetObjectReference<Invoker>> _references = new();

    public JsDotNetInvoker(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task ResizeObserver(string selector, Func<Task> func)
    {
        var invoker = DotNetObjectReference.Create(new Invoker(func));

        _references.Add(invoker);

        await _jsRuntime.InvokeVoidAsync(
            "MasaStackComponents.resizeObserver",
            selector,
            invoker
        );
    }

    public async Task IntersectionObserver(string selector, Func<Task> func)
    {
        var invoker = DotNetObjectReference.Create(new Invoker(func));

        _references.Add(invoker);

        await _jsRuntime.InvokeVoidAsync(
            "MasaStackComponents.intersectionObserver",
            selector,
            invoker);
    }

    public void Dispose()
    {
        foreach (var reference in _references)
        {
            reference.Dispose();
        }
    }

    private class Invoker
    {
        private readonly Func<Task>? _func;

        public Invoker(Func<Task> func)
        {
            _func = func;
        }

        [JSInvokable]
        public async Task Invoke()
        {
            if (_func is not null)
            {
                await _func.Invoke();
            }
        }
    }
}
