#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Web/Masa.Auth.Admin/Masa.Auth.Admin.csproj", "src/Web/Masa.Auth.Admin/"]
COPY ["src/ApiGateways/Caller/Masa.Auth.Caller/Masa.Auth.Caller.csproj", "src/ApiGateways/Caller/Masa.Auth.Caller/"]
RUN dotnet restore "src/Web/Masa.Auth.Admin/Masa.Auth.Admin.csproj"
COPY . .
WORKDIR "/src/src/Web/Masa.Auth.Admin"
RUN dotnet build "Masa.Auth.Admin.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Masa.Auth.Admin.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Masa.Auth.Admin.dll"]