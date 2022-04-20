#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#FROM registry.cn-hangzhou.aliyuncs.com/masa/dotnet_sdk:6.0.100
#ENV LANG="zh_CN.UTF-8"
#ENV LANGUAGE="zh_CN:zh"
#ENV ASPNETCORE_URLS=http://0.0.0.0:80
#WORKDIR /app
#COPY . .
#RUN dotnet build src/Web/Masa.Auth.Web.Admin.Server -c Release
#ENTRYPOINT ["dotnet","./src/Web/Masa.Auth.Web.Admin.Server/bin/Release/net6.0/Masa.Auth.Web.Admin.Server.dll"]




FROM registry.cn-hangzhou.aliyuncs.com/masa/dotnet_sdk:6.0.100 AS publish
WORKDIR /src
COPY . .
ENV CSPROJ="src/Web/Masa.Auth.Web.Admin.Server/Masa.Auth.Web.Admin.Server.csproj"
RUN dotnet restore $CSPROJ && dotnet publish $CSPROJ -c Release -o /app/publish

FROM registry.cn-hangzhou.aliyuncs.com/masa/dotnet_aspnet:6.0.0
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["src/Web/Masa.Auth.Web.Admin.Rcl/wwwroot/i18n","wwwroot/i18n"]
ENV ASPNETCORE_URLS=https://0.0.0.0:443
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "Masa.Auth.Web.Admin.Server.dll"]


