namespace Masa.Auth.Contracts.Admin.Infrastructure.Dtos;

public class SelectItemDto<T>
{
    public T Value { get; set; } = default!;

    public string Text { get; set; } = string.Empty;
}
