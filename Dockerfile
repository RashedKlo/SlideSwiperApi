FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Api/Api.csproj", "Api/"]
COPY ["src/Application/Application.csproj", "Application/"]
COPY ["src/Domain/Domain.csproj", "Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["tests/Tests/Tests.csproj", "Tests/"]

RUN dotnet restore "Api/Api.csproj"
RUN dotnet restore "Tests/Tests.csproj"

COPY src/ src/
COPY tests/ tests/

RUN dotnet build "Api/Api.csproj" -c Release -o /app/build
RUN dotnet build "Tests/Tests.csproj" -c Release -o /app/tests

# Test stage (optional: run tests during build)
FROM build AS test
WORKDIR /src
RUN dotnet test "Tests/Tests.csproj" -c Release --no-build --logger "console;verbosity=normal"

FROM build AS publish
RUN dotnet publish "Api/Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]
