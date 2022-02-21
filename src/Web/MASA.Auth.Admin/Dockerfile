#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Web/MASA.Auth.Admin/MASA.Auth.Admin.csproj", "src/Web/MASA.Auth.Admin/"]
COPY ["src/ApiGateways/Caller/MASA.Auth.Caller/MASA.Auth.Caller.csproj", "src/ApiGateways/Caller/MASA.Auth.Caller/"]
RUN dotnet restore "src/Web/MASA.Auth.Admin/MASA.Auth.Admin.csproj"
COPY . .
WORKDIR "/src/src/Web/MASA.Auth.Admin"
RUN dotnet build "MASA.Auth.Admin.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MASA.Auth.Admin.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MASA.Auth.Admin.dll"]