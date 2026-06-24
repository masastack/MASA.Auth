#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS publish
WORKDIR /src
COPY . .
ENV CSPROJ="src/Services/Masa.Auth.Service.Admin/Masa.Auth.Service.Admin.csproj"
RUN dotnet restore $CSPROJ && dotnet publish $CSPROJ -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:80
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "Masa.Auth.Service.Admin.dll"]
