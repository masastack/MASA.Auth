FROM registry.cn-hangzhou.aliyuncs.com/masa/dotnet_sdk:6.0.100 AS publish
WORKDIR /src
COPY . .
ENV CSPROJ="src/Web/Masa.Auth.Web.Sso/Masa.Auth.Web.Sso.csproj"
RUN dotnet restore $CSPROJ && dotnet publish $CSPROJ -c Release -o /app/publish

FROM registry.cn-hangzhou.aliyuncs.com/masa/dotnet_aspnet:6.0.0
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["src/Web/Masa.Auth.Web.Admin.Rcl/wwwroot/i18n","wwwroot/i18n"]
ENV ASPNETCORE_URLS=https://0.0.0.0:443
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "Masa.Auth.Web.Sso.dll"]


