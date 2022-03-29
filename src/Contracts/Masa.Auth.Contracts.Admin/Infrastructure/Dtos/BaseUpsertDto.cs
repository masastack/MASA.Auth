namespace Masa.Auth.Contracts.Admin.Infrastructure.Dtos;

public class BaseUpsertDto<T> where T : IComparable
{
    public T Id { get; set; } = default!;

    [JsonIgnore]
    public bool HasValue => Id.Equals(default(T));
}
