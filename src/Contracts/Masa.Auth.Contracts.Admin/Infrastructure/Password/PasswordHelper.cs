// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Password;

public class PasswordHelper : ISingletonDependency
{
    public const string PASSWORDRULECONFIGNAME = "$public.AppSettings:PasswordRule";
    public const string PASSWORDPROMPTCONFIGNAME = "$public.AppSettings:PasswordPrompt";
    public const string DEFAULTPASSWORDRULE = "^\\S*(?=\\S{6,})(?=\\S*\\d)(?=\\S*[A-Za-z])\\S*$";
    public const string DEFAULTPASSWORDPROMPT = "PasswordValidateFailed";

    private readonly IMasaConfiguration _masaConfiguration;
    private readonly ILogger<PasswordHelper> _logger;

    public PasswordHelper(IMasaConfiguration masaConfiguration, ILogger<PasswordHelper> logger)
    {
        _masaConfiguration = masaConfiguration;
        _logger = logger;
    }

    /// <summary>
    /// 生成新的密码
    /// </summary>
    /// <returns>生成的密码</returns>
    public string GenerateNewPassword()
    {
        try
        {
            var (passwordRule, _) = GetPasswordRule();
            var xeger = new Xeger(passwordRule);
            return xeger.Generate();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成新密码时发生错误");
            // 如果生成失败，返回默认密码规则生成的密码
            try
            {
                var defaultXeger = new Xeger(DEFAULTPASSWORDRULE);
                return defaultXeger.Generate();
            }
            catch (Exception fallbackEx)
            {
                _logger.LogError(fallbackEx, "使用默认密码规则生成密码时也发生错误");
                throw new InvalidOperationException("无法生成密码", fallbackEx);
            }
        }
    }

    /// <summary>
    /// 获取密码规则和提示信息
    /// </summary>
    /// <returns>密码规则和提示信息的元组</returns>
    public (string rule, string prompt) GetPasswordRule()
    {
        var passwordRule = DEFAULTPASSWORDRULE;
        var passwordPrompt = DEFAULTPASSWORDPROMPT;
        if (_masaConfiguration != null)
        {
            try
            {
                passwordRule = _masaConfiguration.ConfigurationApi.GetPublic().GetValue(PASSWORDRULECONFIGNAME, DEFAULTPASSWORDRULE);
                passwordPrompt = _masaConfiguration.ConfigurationApi.GetPublic().GetValue(PASSWORDPROMPTCONFIGNAME, DEFAULTPASSWORDPROMPT);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取密码规则配置时发生错误，使用默认配置。配置名称: {PasswordRuleConfigName}, {PasswordPromptConfigName}", 
                    PASSWORDRULECONFIGNAME, PASSWORDPROMPTCONFIGNAME);
            }
        }
        else
        {
            _logger.LogWarning("MasaConfiguration 为空，使用默认密码规则配置");
        }
        return (passwordRule, passwordPrompt);
    }

    /// <summary>
    /// 验证密码格式
    /// </summary>
    /// <param name="password">待验证的密码</param>
    /// <param name="context">验证上下文</param>
    public void ValidatePassword(string? password, ValidationContext<string?> context)
    {
        if (password == null)
        {
            password = string.Empty;
        }

        try
        {
            var (passwordRule, passwordPrompt) = GetPasswordRule();
            if (!Regex.IsMatch(password, passwordRule))
            {
                _logger.LogWarning("密码验证失败，密码不符合规则。规则: {PasswordRule}", passwordRule);
                var message = DEFAULTPASSWORDRULE.Equals(passwordRule) ? "PasswordFormatVerificationPrompt" : passwordPrompt;
                try
                {
                    message = I18n.T(message);
                }
                catch (Exception i18nEx)
                {
                    _logger.LogWarning(i18nEx, "获取密码验证失败提示的本地化文本时发生错误，消息键: {MessageKey}", message);
                }
                finally
                {
                    context.AddFailure(message);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "密码验证过程中发生异常");
            context.AddFailure("密码验证过程中发生错误");
        }
    }
}