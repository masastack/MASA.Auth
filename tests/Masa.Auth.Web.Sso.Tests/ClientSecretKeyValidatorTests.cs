// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Nodes;
using Masa.Auth.Contracts.Admin.Sso;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace Masa.Auth.Web.Sso.Tests;

/// <summary>验签公钥 secret 保存前校验；须优先拦截误粘贴的私钥。</summary>
public class ClientSecretKeyValidatorTests
{
    // 手拼 JWK，避免绑定 JsonWebKey 序列化细节
    static string PublicRsaJwk(RSA rsa, string? kid = "k1", Action<JsonObject>? mutate = null)
    {
        var parameters = rsa.ExportParameters(false);
        var node = new JsonObject
        {
            ["kty"] = "RSA",
            ["n"] = Base64UrlEncoder.Encode(parameters.Modulus!),
            ["e"] = Base64UrlEncoder.Encode(parameters.Exponent!)
        };
        if (kid is not null)
        {
            node["kid"] = kid;
        }

        mutate?.Invoke(node);
        return node.ToJsonString();
    }

    static JsonObject PublicEcJwk(ECDsa ec, string curve = "P-256")
    {
        var parameters = ec.ExportParameters(false);
        return new JsonObject
        {
            ["kty"] = "EC",
            ["kid"] = "ec1",
            ["crv"] = curve,
            ["x"] = Base64UrlEncoder.Encode(parameters.Q.X!),
            ["y"] = Base64UrlEncoder.Encode(parameters.Q.Y!)
        };
    }

    [Fact]
    public void Public_rsa_jwk_with_kid_is_accepted()
    {
        using var rsa = RSA.Create(2048);

        Assert.Null(ClientSecretKeyValidator.Validate("JWK", PublicRsaJwk(rsa)));
    }

    [Fact]
    public void Jwk_exported_the_way_the_usage_doc_describes_is_accepted()
    {
        using var rsa = RSA.Create(2048);
        var publicJwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(
            new RsaSecurityKey(rsa.ExportParameters(false)) { KeyId = "domainidentity-2026" });
        var json = System.Text.Json.JsonSerializer.Serialize(publicJwk);

        Assert.Null(ClientSecretKeyValidator.Validate("JWK", json));
    }

    [Fact]
    public void Public_ec_jwk_with_kid_is_accepted()
    {
        using var ec = ECDsa.Create(ECCurve.NamedCurves.nistP256);

        Assert.Null(ClientSecretKeyValidator.Validate("JWK", PublicEcJwk(ec).ToJsonString()));
    }

    [Theory]
    [InlineData("d")]
    [InlineData("p")]
    [InlineData("q")]
    [InlineData("dp")]
    [InlineData("dq")]
    [InlineData("qi")]
    public void Jwk_carrying_private_rsa_parameters_is_rejected(string member)
    {
        using var rsa = RSA.Create(2048);
        var parameters = rsa.ExportParameters(true);
        var secret = member switch
        {
            "d" => parameters.D!,
            "p" => parameters.P!,
            "q" => parameters.Q!,
            "dp" => parameters.DP!,
            "dq" => parameters.DQ!,
            _ => parameters.InverseQ!
        };
        var encoded = Base64UrlEncoder.Encode(secret);

        var reason = ClientSecretKeyValidator.Validate("JWK", PublicRsaJwk(rsa, mutate: n => n[member] = encoded));

        Assert.NotNull(reason);
        Assert.Contains("private", reason, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(encoded, reason);
    }

    [Fact]
    public void Empty_collections_written_by_the_serializer_are_not_private_key_material()
    {
        // 序列化导出的空 oth/key_ops/x5c 不算私钥材料
        using var rsa = RSA.Create(2048);

        var reason = ClientSecretKeyValidator.Validate("JWK", PublicRsaJwk(rsa, mutate: n =>
        {
            n["oth"] = new System.Text.Json.Nodes.JsonArray();
            n["key_ops"] = new System.Text.Json.Nodes.JsonArray();
            n["x5c"] = new System.Text.Json.Nodes.JsonArray();
            n["d"] = null;
        }));

        Assert.Null(reason);
    }

    [Fact]
    public void Non_empty_oth_is_rejected_as_private_key_material()
    {
        using var rsa = RSA.Create(2048);

        var reason = ClientSecretKeyValidator.Validate("JWK", PublicRsaJwk(rsa, mutate: n =>
        {
            var prime = new System.Text.Json.Nodes.JsonObject
            {
                ["r"] = "AQAB",
                ["d"] = "AQAB",
                ["t"] = "AQAB"
            };
            n["oth"] = new System.Text.Json.Nodes.JsonArray(prime);
        }));

        Assert.NotNull(reason);
        Assert.Contains("'oth'", reason);
    }

    [Fact]
    public void Ec_jwk_carrying_the_private_scalar_is_rejected()
    {
        using var ec = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var jwk = PublicEcJwk(ec);
        jwk["d"] = Base64UrlEncoder.Encode(ec.ExportParameters(true).D!);

        var reason = ClientSecretKeyValidator.Validate("JWK", jwk.ToJsonString());

        Assert.NotNull(reason);
        Assert.Contains("private", reason, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Symmetric_oct_jwk_is_rejected(bool includeKeyMaterial)
    {
        var jwk = new JsonObject { ["kty"] = "oct", ["kid"] = "k1" };
        if (includeKeyMaterial)
        {
            jwk["k"] = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(32));
        }

        Assert.NotNull(ClientSecretKeyValidator.Validate("JWK", jwk.ToJsonString()));
    }

    [Fact]
    public void Jwk_without_kid_is_rejected()
    {
        using var rsa = RSA.Create(2048);

        var reason = ClientSecretKeyValidator.Validate("JWK", PublicRsaJwk(rsa, kid: null));

        Assert.NotNull(reason);
        Assert.Contains("kid", reason);
    }

    [Fact]
    public void Jwk_with_non_signature_use_is_rejected()
    {
        using var rsa = RSA.Create(2048);

        Assert.NotNull(ClientSecretKeyValidator.Validate("JWK", PublicRsaJwk(rsa, mutate: n => n["use"] = "enc")));
        Assert.Null(ClientSecretKeyValidator.Validate("JWK", PublicRsaJwk(rsa, mutate: n => n["use"] = "sig")));
    }

    [Fact]
    public void Jwk_set_is_rejected_because_each_secret_holds_one_key()
    {
        using var rsa = RSA.Create(2048);
        var set = new JsonObject
        {
            ["keys"] = new JsonArray(JsonNode.Parse(PublicRsaJwk(rsa)))
        }.ToJsonString();

        Assert.NotNull(ClientSecretKeyValidator.Validate("JWK", set));
    }

    [Fact]
    public void Rsa_key_shorter_than_2048_bits_is_rejected()
    {
        using var rsa = RSA.Create(1024);

        var reason = ClientSecretKeyValidator.Validate("JWK", PublicRsaJwk(rsa));

        Assert.NotNull(reason);
        Assert.Contains("2048", reason);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not json")]
    [InlineData("[]")]
    [InlineData("\"just a string\"")]
    [InlineData("{}")]
    [InlineData("{\"kty\":\"RSA\",\"kid\":\"k1\"}")]
    [InlineData("{\"kty\":\"RSA\",\"kid\":\"k1\",\"n\":\"!!!\",\"e\":\"AQAB\"}")]
    [InlineData("{\"kty\":\"EC\",\"kid\":\"k1\",\"crv\":\"secp256k1\",\"x\":\"AA\",\"y\":\"AA\"}")]
    [InlineData("{\"kty\":\"EC\",\"kid\":\"k1\",\"crv\":\"P-256\"}")]
    public void Malformed_jwk_is_rejected(string value)
    {
        Assert.NotNull(ClientSecretKeyValidator.Validate("JWK", value));
    }

    [Fact]
    public void Secret_type_is_matched_case_insensitively()
    {
        Assert.NotNull(ClientSecretKeyValidator.Validate("jwk", "not json"));
        Assert.NotNull(ClientSecretKeyValidator.Validate("x509certificatebase64", "AAAA"));
    }

    [Theory]
    [InlineData("SharedSecret", "anything")]
    [InlineData("X509Thumbprint", "ABCDEF")]
    [InlineData("X509Name", "CN=client")]
    [InlineData(null, "anything")]
    public void Non_key_secret_types_are_not_this_validators_concern(string? type, string value)
    {
        Assert.Null(ClientSecretKeyValidator.Validate(type, value));
    }

    [Fact]
    public void Public_certificate_is_accepted()
    {
        using var rsa = RSA.Create(2048);
        using var cert = SelfSigned(rsa);

        Assert.Null(ClientSecretKeyValidator.Validate(
            "X509CertificateBase64", Convert.ToBase64String(cert.Export(X509ContentType.Cert))));
    }

    [Fact]
    public void Pfx_with_private_key_is_rejected()
    {
        using var rsa = RSA.Create(2048);
        using var cert = SelfSigned(rsa);

        var reason = ClientSecretKeyValidator.Validate(
            "X509CertificateBase64", Convert.ToBase64String(cert.Export(X509ContentType.Pfx)));

        Assert.NotNull(reason);
    }

    [Theory]
    [InlineData("")]
    [InlineData("%%%not-base64%%%")]
    [InlineData("AAAA")]
    public void Garbage_certificate_is_rejected(string value)
    {
        Assert.NotNull(ClientSecretKeyValidator.Validate("X509CertificateBase64", value));
    }

    [Fact]
    public void Weak_rsa_certificate_is_rejected()
    {
        using var rsa = RSA.Create(1024);
        using var cert = SelfSigned(rsa);

        var reason = ClientSecretKeyValidator.Validate(
            "X509CertificateBase64", Convert.ToBase64String(cert.Export(X509ContentType.Cert)));

        Assert.NotNull(reason);
    }

    internal static X509Certificate2 SelfSigned(RSA rsa)
    {
        var request = new CertificateRequest("CN=assertion-test", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return request.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(30));
    }
}
