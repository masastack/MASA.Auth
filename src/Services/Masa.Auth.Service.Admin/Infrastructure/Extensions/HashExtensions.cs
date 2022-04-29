namespace Masa.Auth.Service.Admin.Infrastructure.Extensions;

public static class HashExtensions
{
    public static string Sha256(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = sha.ComputeHash(bytes);

        return Convert.ToBase64String(hash);
    }

    public static string Sha512(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        using var sha = SHA512.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = sha.ComputeHash(bytes);

        return Convert.ToBase64String(hash);
    }
}
