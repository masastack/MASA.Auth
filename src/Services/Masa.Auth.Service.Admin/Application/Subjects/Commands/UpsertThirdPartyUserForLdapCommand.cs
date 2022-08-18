// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public record UpsertThirdPartyUserForLdapCommand : Command
{
    public Guid ThirdPartyIdpId { get; set; }

    public string ThridPartyIdentity { get; set; } = "";

    public string ExtendedData { get; set; } = "";

    public string? Name { get; set; }

    public string? DisplayName { get; set; }

    public string PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? Account { get; set; }

    public string Password { get; set; }

    public string JobNumber { get; set; }

    public UpsertThirdPartyUserForLdapCommand()
    {
        PhoneNumber = "";
        Password = "";
        JobNumber = "";
    }

    public UpsertThirdPartyUserForLdapCommand(Guid thirdPartyIdpId, string thridPartyIdentity, string extendedData, string? name, string? displayName, string phoneNumber, string? email, string? account, string password, string jobNumber)
    {
        ThirdPartyIdpId = thirdPartyIdpId;
        ThridPartyIdentity = thridPartyIdentity;
        ExtendedData = extendedData;
        Name = name;
        DisplayName = displayName;
        PhoneNumber = phoneNumber;
        Email = email;
        Account = account;
        Password = password;
        JobNumber = jobNumber;
    }
}
