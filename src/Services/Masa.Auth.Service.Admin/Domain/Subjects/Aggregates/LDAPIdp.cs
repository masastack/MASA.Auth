namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class LDAPIdp : IdentityProvider
{
    public string ServerAddress { get; private set; } = null!;

    public int ServerPort { get; set; }

    public bool IsSSL { get; set; }

    public string BaseDn { get; set; } = null!;

    public string UserSearchBaseDn { get; private set; } = string.Empty;

    public string GroupSearchBaseDn { get; private set; } = string.Empty;

    public string RootUserDn { get; set; } = null!;

    public string RootUserPassword { get; set; } = null!;

    private LDAPIdp()
    {
        Name = "LDAP";
        DisplayName = "LDAP";
        IdentificationType = IdentificationTypes.PhoneNumber;
    }
}
