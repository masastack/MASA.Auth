namespace Masa.Stack.Components.Extensions;

public static class TextFormattingExtensions
{
    public static string MaskPhoneNumber(this string phoneNumber)
    {
        if (phoneNumber.Length == 11)
        {
            return phoneNumber.Substring(0, 3) + "****" + phoneNumber.Substring(7);
        }
        return phoneNumber;
    }

    public static string MaskIdCard(this string idCard)
    {
        if (idCard.Length == 18)
        {
            return idCard.Substring(0, 6) + "********" + idCard.Substring(14);
        }
        return idCard;
    }

    public static string MaskAccount(this string account)
    {
        if (account.Length > 2)
        {
            return account.Substring(0, 1) + new string('*', account.Length - 2) + account.Substring(account.Length - 1);
        }
        else if (account.Length == 2)
        {
            return account.Substring(0, 1) + "*";
        }
        return account;
    }

    public static string MaskSensitiveInfo(this string input, string type)
    {
        return type switch
        {
            "phone" => input.MaskPhoneNumber(),
            "idCard" => input.MaskIdCard(),
            "account" => input.MaskAccount(),
            _ => input
        };
    }
}
