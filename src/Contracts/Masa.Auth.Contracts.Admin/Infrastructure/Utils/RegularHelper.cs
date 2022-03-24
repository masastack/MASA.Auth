namespace Masa.Auth.Contracts.Admin.Infrastructure.Utils;

public static class RegularHelper
{
    public const string CHINESE = "^[\u4e00-\u9fa5]+$";
    public const string NUMBER = "^[0-9]+$";
    public const string LETTER = "^[a-zA-Z]+$";
    public const string LOWER_LETTER = "^[a-z]+$";
    public const string UPPER_LETTER = "^[A-Z]+$";
    public const string LETTER_NUMBER = "^[A-Z0-9]+$";
    public const string CHINESE_LETTER_NUMBER = "^[\u4e00-\u9fa5_a-zA-Z0-9]+$";
    public const string CHINESE_LETTER = "^[\u4e00-\u9fa5_a-zA-Z]+$";
    public const string PHONE = "^((13[0-9])|(14[0-9])|(15[0-9])|(17[0-9])|(18[0-9]))\\d{8}$";
    public const string IDCARD = "(^\\d{15}$)|(^\\d{17}([0-9]|X|x)$)";
    public const string EMAIL = "^[\\w-]+@[\\w-]+\\.(com|net|org|edu|mil|tv|biz|info)$";
    public const string URL = "[a-zA-z]+://[^s]*";
}

