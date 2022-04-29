namespace Masa.Auth.Contracts.Admin.Infrastructure.Dtos;

public class CheckItemDto<TId> : IEquatable<CheckItemDto<TId>>
{
    public TId Id { get; set; } = default!;

    public bool Selected { get; set; }

    public string DisplayValue { get; set; } = string.Empty;

    public override bool Equals(object? obj)
    {
        return Equals(obj as CheckItemDto<TId>);
    }

    public bool Equals(CheckItemDto<TId>? other)
    {
        if (other is null)
        {
            return false;
        }
        if (Id is null)
        {
            return other.Id == null;
        }
        return Id.Equals(other.Id);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id);
    }

    public override string ToString()
    {
        return DisplayValue ?? " - ";
    }

    public static bool operator ==(CheckItemDto<TId> left, CheckItemDto<TId> right)
    {
        return EqualityComparer<CheckItemDto<TId>>.Default.Equals(left, right);
    }

    public static bool operator !=(CheckItemDto<TId> left, CheckItemDto<TId> right)
    {
        return !(left == right);
    }
}
