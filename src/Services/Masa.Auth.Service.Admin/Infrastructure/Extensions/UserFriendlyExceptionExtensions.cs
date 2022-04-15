namespace Masa.Auth.Service.Admin.Infrastructure.Extensions;

public static class UserFriendlyExceptionExtensions
{
    public static void ThrowIfEmpty(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new UserFriendlyException($"the value of { nameof(value) } can not be empty");
        }
    }
}
