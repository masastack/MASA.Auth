#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Web/Masa.Auth.Web.Admin.Server/Masa.Auth.Web.Admin.Server.csproj", "src/Web/Masa.Auth.Web.Admin.Server/"]
COPY ["src/Web/Masa.Auth.Web.Admin.Rcl/Masa.Auth.Web.Admin.Rcl.csproj", "src/Web/Masa.Auth.Web.Admin.Rcl/"]
RUN dotnet restore "src/Web/Masa.Auth.Web.Admin.Server/Masa.Auth.Web.Admin.Server.csproj"
COPY . .
WORKDIR "/src/src/Web/Masa.Auth.Web.Admin.Server"
RUN dotnet build "Masa.Auth.Web.Admin.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Masa.Auth.Web.Admin.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Masa.Auth.Web.Admin.Server.dll"]