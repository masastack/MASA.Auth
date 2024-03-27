#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM registry.cn-hangzhou.aliyuncs.com/masa/dotnet_sdk:6.0.403 AS publish
WORKDIR /src
COPY . .
ENV CSPROJ="src/Services/Masa.Auth.Service.Admin/Masa.Auth.Service.Admin.csproj"
RUN dotnet restore $CSPROJ -s "http://devops.lonsid.cn:8080/nuget;https://repo.huaweicloud.com/repository/nuget/v3/index.json" && dotnet publish $CSPROJ -c Release -o /app/publish


FROM registry.cn-hangzhou.aliyuncs.com/masa/dotnet_aspnet:6.0.4
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:80
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "Masa.Auth.Service.Admin.dll"]
