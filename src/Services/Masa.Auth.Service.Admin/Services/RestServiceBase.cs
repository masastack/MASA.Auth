using Masa.Auth.Contracts.Admin.Infrastructure.Utils;

namespace Masa.Auth.Service.Admin.Services;

public abstract class RestServiceBase : ServiceBase
{
    public RestServiceBase(IServiceCollection services, string baseUri) : base(services, baseUri)
    {
        var type = GetType();
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var method in methods)
        {
            if (method.Name.EndsWith("Async"))
            {
                var @delegate = TypeDescriptor.ConvertToDelegateType(method, this);
                if (method.Name.StartsWith("Get")) MapGet(@delegate);
                else if (method.Name.StartsWith("Add")) MapPost(@delegate);
                else if (method.Name.StartsWith("Update")) MapPut(@delegate);
                else if (method.Name.StartsWith("Remove")) MapDelete(@delegate);
            }
        }
    }
}

