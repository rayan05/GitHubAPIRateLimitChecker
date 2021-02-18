FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["APIRateLimitChecker/APIRateLimitChecker.csproj", "APIRateLimitChecker/"]
COPY ["APIRateLimitCheckerTest/APIRateLimitCheckerTest.csproj", "APIRateLimitCheckerTest/"]
RUN dotnet restore "APIRateLimitChecker/APIRateLimitChecker.csproj"
COPY . .
RUN dotnet build -c Release -o /app/build

# build the unit tests
FROM build AS testrunner
WORKDIR /src/APIRateLimitCheckerTest
CMD ["dotnet", "test", "--logger:trx"]

# run the unit tests
FROM build AS test
WORKDIR /src/APIRateLimitCheckerTest
RUN dotnet test --logger:trx


FROM build AS publish
WORKDIR /src/APIRateLimitChecker
RUN dotnet publish "APIRateLimitChecker.csproj" -c Release -r linux-x64 -o /app/publish -p:PublishReadyToRun=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "APIRateLimitChecker.dll"]