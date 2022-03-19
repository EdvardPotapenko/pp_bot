FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build

WORKDIR /src
COPY pp_bot.Server/pp_bot.Server.csproj pp_bot.Server/

RUN dotnet restore pp_bot.Server/pp_bot.Server.csproj

COPY . .
WORKDIR /src/pp_bot.Server
RUN dotnet build pp_bot.Server.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish pp_bot.Server.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "pp_bot.Server.dll"]