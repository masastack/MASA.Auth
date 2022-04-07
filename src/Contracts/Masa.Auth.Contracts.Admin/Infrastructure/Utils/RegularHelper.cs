namespace Masa.Auth.Contracts.Admin.Infrastructure.Utils;

public static class RegularHelper
{
    public const string CHINESE = "^\\s{0}$|^[\u4e00-\u9fa5]+$";
    public const string NUMBER = "^\\s{0}$|^[0-9]+$";
    public const string LETTER = "^\\s{0}$|^[a-zA-Z]+$";
    public const string LOWER_LETTER = "^\\s{0}$|^[a-z]+$";
    public const string UPPER_LETTER = "^\\s{0}$|^[A-Z]+$";
    public const string LETTER_NUMBER = "^\\s{0}$|^[A-Z0-9]+$";
    public const string CHINESE_LETTER_NUMBER = "^\\s{0}$|^[\u4e00-\u9fa5_a-zA-Z0-9]+$";
    public const string CHINESE_LETTER = "^\\s{0}$|^[\u4e00-\u9fa5_a-zA-Z]+$";
    public const string PHONE = "^\\s{0}$|^((17[0-9])|(14[0-9])|(13[0-9])|(15[^4,\\D])|(19[^4,\\D])|(18[0,1,2,5-9]))\\d{8}$";
    public const string IDCARD = "^\\s{0}$|(^\\d{15}$)|(^\\d{17}([0-9]|X|x)$)";
    public const string EMAIL = "^\\s{0}$|^[\\w-]+@[\\w-]+\\.(com|net|org|edu|mil|tv|biz|info)$";
    public const string URL = "^\\s{0}$|[a-zA-z]+://[^s]*";
}

