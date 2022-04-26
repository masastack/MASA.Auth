namespace Masa.Auth.Contracts.Admin.Infrastructure.Dtos;

public class CheckItemDto<T> : IEquatable<CheckItemDto<T>>
{
    public T Id { get; set; } = default!;

    public bool Selected { get; set; }

    public string DisplayValue { get; set; } = string.Empty;

    public override bool Equals(object? obj)
    {
        return Equals(obj as CheckItemDto<T>);
    }

    public bool Equals(CheckItemDto<T>? other)
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

    public static bool operator ==(CheckItemDto<T> left, CheckItemDto<T> right)
    {
        return EqualityComparer<CheckItemDto<T>>.Default.Equals(left, right);
    }

    public static bool operator !=(CheckItemDto<T> left, CheckItemDto<T> right)
    {
        return !(left == right);
    }
}
