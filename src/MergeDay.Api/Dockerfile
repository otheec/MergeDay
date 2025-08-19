# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj and restore
COPY ["src/MergeDay.Api/MergeDay.Api.csproj", "src/MergeDay.Api/"]
RUN dotnet restore "src/MergeDay.Api/MergeDay.Api.csproj"

# Copy the rest of the source code
COPY . .

# Build
WORKDIR "/src/src/MergeDay.Api"
RUN dotnet build "MergeDay.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MergeDay.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MergeDay.Api.dll"]
