// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public record UpsertLdapUserCommand : Command
{
    public Guid Id { get; set; }

    public string ThridPartyUserIdentity { get; set; } = "";

    public string ExtendedData { get; set; } = "";

    public string? Name { get; set; }

    public string? DisplayName { get; set; }

    public string PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? Account { get; set; }

    public string JobNumber { get; set; }

    public UserModel Result { get; set; } = new UserModel();

    public UpsertLdapUserCommand()
    {
        PhoneNumber = "";
        JobNumber = "";
    }

    public UpsertLdapUserCommand(string thridPartyUserIdentity, string extendedData, string? name, string? displayName, string phoneNumber, string? email, string? account, string jobNumber)
    {
        ThridPartyUserIdentity = thridPartyUserIdentity;
        ExtendedData = extendedData;
        Name = name;
        DisplayName = displayName;
        PhoneNumber = phoneNumber;
        Email = email;
        Account = account;
        JobNumber = jobNumber;
    }

    public UpsertLdapUserCommand(Guid id, string thridPartyIdentity, string extendedData, string? name, string? displayName, string phoneNumber, string? email, string? account, string jobNumber) : this(thridPartyIdentity, extendedData, name, displayName, phoneNumber, email, account, jobNumber)
    {
        Id = id;
    }
}
