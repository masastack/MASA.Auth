// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System.Net.Sockets;

namespace Masa.Auth.Service.Admin.Infrastructure.Ldap;

/// <summary>
/// LDAP 连接校验，按失败阶段返回明确提示：
/// 1. TCP 地址/端口不可达 → <see cref="UserFriendlyExceptionCodes.LDAP_SERVER_UNREACHABLE"/>
/// 2. LDAP/LDAPS 握手失败 → <see cref="UserFriendlyExceptionCodes.LDAP_SSL_MISMATCH"/>
/// 3. LDAP result code 49 → <see cref="UserFriendlyExceptionCodes.LDAP_CREDENTIALS_INVALID"/>
/// 4. 其它 Bind 失败 → <see cref="UserFriendlyExceptionCodes.LDAP_BIND_FAILED"/>
/// </summary>
public static class LdapAuthenticateHelper
{
    /// <summary>
    /// LDAP result code: invalidCredentials (RFC 4511).
    /// </summary>
    const int LdapInvalidCredentials = 49;

    /// <summary>
    /// Connect timeout for LDAP connect-test / save validation (milliseconds).
    /// </summary>
    const int ConnectionTimeoutMs = 10_000;

    public static async Task AuthenticateOrThrowAsync(LdapDetailDto ldapDetailDto)
    {
        await EnsureTcpConnectionAsync(ldapDetailDto);

        using var connection = new LdapConnection
        {
            SecureSocketLayer = ldapDetailDto.IsLdaps,
            ConnectionTimeout = ConnectionTimeoutMs
        };

        // Stage 1: TCP / TLS connect — address、port、LDAP vs LDAPS
        try
        {
            await connection.ConnectAsync(ldapDetailDto.ServerAddress, ldapDetailDto.ServerPort);
        }
        catch (Exception ex)
        {
            // TCP 已经连通，此时失败发生在 LDAP 明文协议或 TLS 握手阶段。
            // 不再把它误报为“地址/端口不可达”。
            throw CreateProtocolException(ldapDetailDto);
        }

        // Stage 2: Bind — credentials
        try
        {
            await connection.BindAsync(ldapDetailDto.RootUserDn, ldapDetailDto.RootUserPassword);
        }
        catch (LdapException ex) when (ex.ResultCode == LdapInvalidCredentials)
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.LDAP_CREDENTIALS_INVALID);
        }
        catch (LdapException)
        {
            // 只有 result code 49 才能确定是账号或密码错误。
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.LDAP_BIND_FAILED);
        }
        catch (Exception)
        {
            // Bind 阶段连接中断、服务端策略异常等，不能归类为密码错误。
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.LDAP_BIND_FAILED);
        }
    }

    static async Task EnsureTcpConnectionAsync(LdapDetailDto ldapDetailDto)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromMilliseconds(ConnectionTimeoutMs));
        using var tcpClient = new TcpClient();

        try
        {
            await tcpClient.ConnectAsync(
                ldapDetailDto.ServerAddress,
                ldapDetailDto.ServerPort,
                timeout.Token);
        }
        catch (Exception ex) when (ex is SocketException or OperationCanceledException)
        {
            throw new UserFriendlyException(
                UserFriendlyExceptionCodes.LDAP_SERVER_UNREACHABLE,
                ldapDetailDto.ServerAddress,
                ldapDetailDto.ServerPort.ToString());
        }
    }

    static UserFriendlyException CreateProtocolException(LdapDetailDto ldapDetailDto)
    {
        var mode = ldapDetailDto.IsLdaps ? "LDAPS" : "LDAP";
        return new UserFriendlyException(
            UserFriendlyExceptionCodes.LDAP_SSL_MISMATCH,
            mode,
            ldapDetailDto.ServerAddress,
            ldapDetailDto.ServerPort.ToString());
    }
}
