ARG DOTNET_SDK=mcr.microsoft.com/dotnet/sdk:6.0-alpine3.15
ARG ASPNET_RUNTIME=mcr.microsoft.com/dotnet/aspnet:6.0-alpine3.15


FROM ${DOTNET_SDK} AS build

WORKDIR /build
COPY src/ .

RUN dotnet publish pp_bot.Server/pp_bot.Server.csproj -c Release -o /app/publish

FROM ${ASPNET_RUNTIME} AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "pp_bot.Server.dll"]