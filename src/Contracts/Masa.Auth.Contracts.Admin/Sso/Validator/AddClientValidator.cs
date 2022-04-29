namespace Masa.Auth.Contracts.Admin.Sso.Validator;

internal class AddClientValidator : AbstractValidator<AddClientDto>
{
    public AddClientValidator()
    {
        RuleFor(client => client.ClientId).Required();
        RuleFor(client => client.ClientName).Required();
    }
}
