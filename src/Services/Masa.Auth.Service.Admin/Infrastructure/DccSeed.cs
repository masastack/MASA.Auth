// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure;

public class DccSeed
{
    public async Task SeedAsync(WebApplicationBuilder builder)
    {
        var services = builder.Services.BuildServiceProvider();
        var env = services.GetRequiredService<IWebHostEnvironment>();
        var contentRootPath = env.ContentRootPath;
        var configurationApiManage = services.GetRequiredService<IConfigurationApiManage>();
        var environment = builder.Environment.EnvironmentName;

        await configurationApiManage.AddAsync(environment, "default", "public-$Config", new Dictionary<string, string>
        {
            { "$public.RedisConfig",GetRedisConfig(contentRootPath,environment) },
            { "$public.AppSettings",GetAppSettings(contentRootPath,environment) },
            { "$public.Oidc",GetOidc(contentRootPath,environment) },
            { "$public.Oss",GetOss(contentRootPath,environment) },
            { "$public.ES.UserAutoComplete",GetESUserAutoComplete(contentRootPath,environment) },
            { "$public.AliyunPhoneNumberLogin",GetAliyunPhoneNumberLogin(contentRootPath,environment) },
            { "$public.Email",GetEmail(contentRootPath,environment) },
            { "$public.Sms",GetSms(contentRootPath,environment) },
        });

        await configurationApiManage.AddAsync(environment, "default", "masa-auth-service-admin", new Dictionary<string, string>
        {
            {"ClientSeed",GetClient(contentRootPath,environment)  }
        });
    }

    private string GetRedisConfig(string contentRootPath, string environment)
    {
        var filePath = CombineFilePath(contentRootPath, "$public.RedisConfig.json", environment);
        return File.ReadAllText(filePath);
    }

    private string GetAppSettings(string contentRootPath, string environment)
    {
        var filePath = CombineFilePath(contentRootPath, "$public.AppSettings.json", environment);
        return File.ReadAllText(filePath);
    }

    private string GetOidc(string contentRootPath, string environment)
    {
        var filePath = CombineFilePath(contentRootPath, "$public.Oidc.json", environment);
        return File.ReadAllText(filePath);
    }

    private string GetOss(string contentRootPath, string environment)
    {
        var filePath = CombineFilePath(contentRootPath, "$public.Oss.json", environment);
        return File.ReadAllText(filePath);
    }

    private string GetClient(string contentRootPath, string environment)
    {
        var filePath = CombineFilePath(contentRootPath, "ClientSeed.json", environment);
        return File.ReadAllText(filePath);
    }

    private string GetESUserAutoComplete(string contentRootPath, string environment)
    {
        var filePath = CombineFilePath(contentRootPath, "$public.ES.UserAutoComplete.json", environment);
        return File.ReadAllText(filePath);
    }

    private string GetAliyunPhoneNumberLogin(string contentRootPath, string environment)
    {
        var filePath = CombineFilePath(contentRootPath, "$public.AliyunPhoneNumberLogin.json", environment);
        return File.ReadAllText(filePath);
    }

    private string GetEmail(string contentRootPath, string environment)
    {
        var filePath = CombineFilePath(contentRootPath, "$public.Email.json", environment);
        return File.ReadAllText(filePath);
    }

    private string GetSms(string contentRootPath, string environment)
    {
        var filePath = CombineFilePath(contentRootPath, "$public.Sms.json", environment);
        return File.ReadAllText(filePath);
    }

    private string CombineFilePath(string contentRootPath, string fileName, string environment)
    {
        var extension = Path.GetExtension(fileName);
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        var environmentFileName = $"{fileNameWithoutExtension}.{environment}{extension}";
        var environmentFilePath = Path.Combine(contentRootPath, "Setup", environmentFileName);
        if (File.Exists(environmentFilePath))
        {
            return environmentFilePath;
        }
        return Path.Combine(contentRootPath, "Setup", fileName);
    }
}
