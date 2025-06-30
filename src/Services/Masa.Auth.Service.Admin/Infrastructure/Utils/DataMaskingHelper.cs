// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Utils;

/// <summary>
/// Data masking helper class
/// </summary>
public static class DataMaskingHelper
{
    /// <summary>
    /// Mask phone number, replace middle four digits with *
    /// </summary>
    /// <param name="phoneNumber">Phone number</param>
    /// <returns>Masked phone number</returns>
    public static string? MaskPhoneNumber(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber) || phoneNumber.Length < 7)
            return phoneNumber;

        // Keep first 3 digits and last 4 digits, replace middle with *
        if (phoneNumber.Length == 11)
        {
            return phoneNumber.Substring(0, 3) + "****" + phoneNumber.Substring(7);
        }

        // For other length phone numbers, keep first and last 2 digits
        if (phoneNumber.Length >= 4)
        {
            var maskLength = phoneNumber.Length - 4;
            return phoneNumber.Substring(0, 2) + new string('*', maskLength) + phoneNumber.Substring(phoneNumber.Length - 2);
        }

        return phoneNumber;
    }

    /// <summary>
    /// Mask ID card, only show last four digits, others show as *
    /// </summary>
    /// <param name="idCard">ID card number</param>
    /// <returns>Masked ID card number</returns>
    public static string? MaskIdCard(string? idCard)
    {
        if (string.IsNullOrWhiteSpace(idCard) || idCard.Length < 4)
            return idCard;

        // Only keep last 4 digits, replace front with *
        var maskLength = idCard.Length - 4;
        return new string('*', maskLength) + idCard.Substring(idCard.Length - 4);
    }

    /// <summary>
    /// Mask email, keep first 2 characters before @ and content after @
    /// </summary>
    /// <param name="email">Email address</param>
    /// <returns>Masked email address</returns>
    public static string? MaskEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            return email;

        var parts = email.Split('@');
        if (parts.Length != 2 || parts[0].Length < 2)
            return email;

        var localPart = parts[0];
        var domainPart = parts[1];

        // Keep first 2 characters, replace middle with *
        var maskedLocal = localPart.Length <= 2 ? localPart :
            localPart.Substring(0, 2) + new string('*', localPart.Length - 2);

        return maskedLocal + "@" + domainPart;
    }

    /// <summary>
    /// Mask account, intelligently detect if account is phone number and apply appropriate masking
    /// </summary>
    /// <param name="account">Account string</param>
    /// <returns>Masked account string</returns>
    public static string? MaskAccount(string? account)
    {
        if (string.IsNullOrWhiteSpace(account))
            return account;

        // Check if account is a phone number (11 digits starting with 1)
        if (account.Length == 11 && account.All(char.IsDigit) && account.StartsWith("1"))
        {
            return MaskPhoneNumber(account);
        }

        // Check if account is an email
        if (account.Contains('@'))
        {
            return MaskEmail(account);
        }

        // For other account types, mask middle part
        if (account.Length <= 4)
        {
            return account;
        }

        // Keep first 2 and last 2 characters, mask middle
        var maskLength = account.Length - 4;
        return account.Substring(0, 2) + new string('*', maskLength) + account.Substring(account.Length - 2);
    }
}