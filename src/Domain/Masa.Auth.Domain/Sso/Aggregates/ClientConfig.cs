// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Domain.Sso.Aggregates;

/// <summary>
/// Per-client business configuration: password rule and the message template
/// matrix (scene x channel x template). Keyed by the OIDC client_id (string).
/// A missing value means "fall back to the global DCC configuration".
/// </summary>
public class ClientConfig : FullAggregateRoot<int, Guid>
{
    private List<ClientMessageTemplate> _messageTemplates = new();

    public string ClientId { get; private set; } = "";

    public string? PasswordRule { get; private set; }

    public string? PasswordPrompt { get; private set; }

    public ClientPasswordRuleConfig? PasswordRuleConfig { get; private set; }

    public IReadOnlyCollection<ClientMessageTemplate> MessageTemplates => _messageTemplates;

    private ClientConfig() { }

    public ClientConfig(string clientId)
    {
        ClientId = clientId;
    }

    public void UpdatePasswordRule(string? passwordRule, string? passwordPrompt, ClientPasswordRuleConfig? passwordRuleConfig)
    {
        PasswordRule = passwordRule;
        PasswordPrompt = passwordPrompt;
        PasswordRuleConfig = passwordRuleConfig;
    }

    /// <summary>
    /// Replace the whole message template matrix.
    /// </summary>
    public void SetMessageTemplates(IEnumerable<ClientMessageTemplate> messageTemplates)
    {
        _messageTemplates = messageTemplates.ToList();
    }
}