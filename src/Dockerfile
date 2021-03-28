FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app
ENV PP_BOT_DB_CONNECTION="Host=pp_bot_postgres;Port=5432;UserId=pp_bot;Password=_PP_IS_C23;Database=pp_bot;CommandTimeout=300;"

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

COPY ["src/pp_bot.csproj", "src/"]
RUN dotnet restore "src/pp_bot.csproj"
COPY . .
WORKDIR "/src/src"
RUN dotnet build "pp_bot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "pp_bot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "pp_bot.dll"]
