// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Fare;

namespace Masa.Auth.Service.Admin.Infrastructure.Password;

public class PasswordRuleProvider : IPasswordRuleProvider, IScopedDependency
{
    public const string PASSWORDRULECONFIGNAME = "$public.AppSettings:PasswordRule";
    public const string PASSWORDPROMPTCONFIGNAME = "$public.AppSettings:PasswordPrompt";
    public const string DEFAULTPASSWORDRULE = "^\\S*(?=\\S{6,})(?=\\S*\\d)(?=\\S*[A-Za-z])\\S*$";
    public const string DEFAULTPASSWORDPROMPT = "PasswordValidateFailed";

    private readonly IClientConfigRepository _clientConfigRepository;
    private readonly IMasaConfiguration _masaConfiguration;
    private readonly ILogger<PasswordRuleProvider> _logger;

    public PasswordRuleProvider(
        IClientConfigRepository clientConfigRepository,
        IMasaConfiguration masaConfiguration,
        ILogger<PasswordRuleProvider> logger)
    {
        _clientConfigRepository = clientConfigRepository;
        _masaConfiguration = masaConfiguration;
        _logger = logger;
    }

    public async Task<string?> GetFailureAsync(string? password, string? clientId)
    {
        var (rule, prompt) = await GetEffectiveRuleAsync(clientId);
        return ValidatePasswordWithRule(password, rule, prompt);
    }

    public Task<string> GenerateNewPasswordAsync()
    {
        var (passwordRule, _) = GetGlobalPasswordRule();
        return Task.FromResult(GenerateNewPassword(passwordRule));
    }

    private async Task<(string rule, string prompt)> GetEffectiveRuleAsync(string? clientId)
    {
        // 全局 DCC 配置作为回退默认值
        var (rule, prompt) = GetGlobalPasswordRule();

        if (!string.IsNullOrEmpty(clientId))
        {
            var clientConfig = await _clientConfigRepository.FindByClientIdAsync(clientId);
            if (clientConfig is not null && !string.IsNullOrEmpty(clientConfig.PasswordRule))
            {
                rule = clientConfig.PasswordRule;
                if (!string.IsNullOrEmpty(clientConfig.PasswordPrompt))
                {
                    prompt = clientConfig.PasswordPrompt;
                }
            }
        }

        return (rule, prompt);
    }

    private (string rule, string prompt) GetGlobalPasswordRule()
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

    private string GenerateNewPassword(string passwordRule)
    {
        try
        {
            var xeger = new Xeger(passwordRule);
            return xeger.Generate();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成新密码时发生错误");
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

    private string? ValidatePasswordWithRule(string? password, string passwordRule, string passwordPrompt)
    {
        password ??= string.Empty;
        try
        {
            if (Regex.IsMatch(password, passwordRule))
            {
                return null;
            }

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
            return message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "密码验证过程中发生异常");
            return "密码验证过程中发生错误";
        }
    }
}
