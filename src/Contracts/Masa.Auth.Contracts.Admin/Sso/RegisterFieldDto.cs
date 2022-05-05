namespace Masa.Auth.Contracts.Admin.Sso;

public class RegisterFieldDto
{
    public RegisterFieldTypes RegisterFieldType { get; private set; }

    public int Sort { get; private set; }

    public bool Required { get; private set; }

    public RegisterFieldDto()
    {
    }

    public RegisterFieldDto(RegisterFieldTypes registerFieldType, int sort, bool required)
    {
        RegisterFieldType = registerFieldType;
        Sort = sort;
        Required = required;
    }
}

