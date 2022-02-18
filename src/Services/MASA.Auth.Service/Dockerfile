#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Services/MASA.Auth.RolePermission.Service/MASA.Auth.RolePermission.csproj", "src/Services/MASA.Auth.RolePermission.Service/"]
RUN dotnet restore "src/Services/MASA.Auth.RolePermission.Service/MASA.Auth.RolePermission.csproj"
COPY . .
WORKDIR "/src/src/Services/MASA.Auth.RolePermission.Service"
RUN dotnet build "MASA.Auth.RolePermission.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MASA.Auth.RolePermission.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MASA.Auth.RolePermission.dll"]