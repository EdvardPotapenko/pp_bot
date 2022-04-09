ARG DOTNET_SDK=mcr.microsoft.com/dotnet/sdk:6.0-alpine3.15
ARG ASPNET_RUNTIME=mcr.microsoft.com/dotnet/aspnet:6.0-alpine3.15
ARG BOT_CONFIGURATION=release


# BUILDING HOST
FROM ${DOTNET_SDK} AS build_host_base
WORKDIR /build
COPY src/ .


FROM build_host_base AS build_host_release
RUN dotnet publish pp_bot.Server/pp_bot.Server.csproj -c Release -o /app/publish


FROM build_host_base AS build_host_webhook
RUN dotnet publish pp_bot.Server/pp_bot.Server.csproj -c Webhook -o /app/publish


FROM build_host_${BOT_CONFIGURATION} AS build_host


# BUILDING ACHIEVEMENTS
FROM ${DOTNET_SDK} AS build_achievements
WORKDIR /build
COPY src/ .
RUN dotnet publish pp_bot.Achievements/pp_bot.Achievements.csproj -c Release -o /app/publish/Achievements


# BUILDING COMMANDS
FROM ${DOTNET_SDK} AS build_commands
WORKDIR /build
COPY src/ .
RUN dotnet publish pp_bot.Commands/pp_bot.Commands.csproj -c Release -o /app/publish/Commands


# FINAL STAGE
FROM ${ASPNET_RUNTIME} AS final
WORKDIR /app
COPY --from=build_host /app/publish .
COPY --from=build_achievements /app/publish .
COPY --from=build_commands /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "pp_bot.Server.dll"]