FROM mcr.microsoft.com/dotnet/sdk:10.0 AS publish
WORKDIR /src
COPY . .
ENV CSPROJ="src/Web/Masa.Auth.Web.Sso/Masa.Auth.Web.Sso.csproj"
RUN dotnet restore $CSPROJ && dotnet publish $CSPROJ -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=https://0.0.0.0:443
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "Masa.Auth.Web.Sso.dll"]


