FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY global.json ./
COPY Directory.Build.props ./
COPY Directory.Packages.props ./
COPY TrueCodeTest.sln ./

COPY src/Shared ./src/Shared
COPY src/Users ./src/Users
COPY src/Finance ./src/Finance
COPY src/Migrator ./src/Migrator
COPY src/CurrencyFetcher ./src/CurrencyFetcher
COPY src/Gateway ./src/Gateway
COPY tests ./tests

RUN dotnet restore src/Migrator/TrueCodeTest.Migrator/TrueCodeTest.Migrator.csproj
RUN dotnet publish src/Migrator/TrueCodeTest.Migrator/TrueCodeTest.Migrator.csproj \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "TrueCodeTest.Migrator.dll"]
