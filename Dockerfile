FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files into the same structure as your local 'src' and 'tests' folders
COPY ["src/Api/Api.csproj", "src/Api/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
COPY ["tests/Tests/Tests.csproj", "tests/Tests/"]

# Restore using the new paths
RUN dotnet restore "src/Api/Api.csproj"
RUN dotnet restore "tests/Tests/Tests.csproj"

# Copy the rest of the source code
COPY src/ src/
COPY tests/ tests/

# Build using the correct paths
RUN dotnet build "src/Api/Api.csproj" -c Release -o /app/build
RUN dotnet build "tests/Tests/Tests.csproj" -c Release -o /app/tests

# Test stage
FROM build AS test
WORKDIR /src
RUN dotnet test "tests/Tests/Tests.csproj" -c Release --no-build --logger "console;verbosity=normal"

# Publish stage
FROM build AS publish
RUN dotnet publish "src/Api/Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]
