FROM registry.cn-hangzhou.aliyuncs.com/masa/dotnet_sdk:6.0.403 AS publish
WORKDIR /src
COPY . .
ENV CSPROJ="src/Web/Masa.Auth.Web.Sso/Masa.Auth.Web.Sso.csproj"
RUN dotnet restore $CSPROJ -s "http://devops.lonsid.cn:8080/nuget;https://nuget.cdn.azure.cn/v3/index.json" && dotnet publish $CSPROJ -c Release -o /app/publish

FROM registry.cn-hangzhou.aliyuncs.com/masa/dotnet_aspnet:6.0.4
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=https://0.0.0.0:443
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "Masa.Auth.Web.Sso.dll"]


