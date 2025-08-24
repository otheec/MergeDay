# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

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

# Final runtime image (with Tesseract installed)
FROM base AS final

# Install Tesseract + languages (ENG + Czech + OSD)
USER root
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
        tesseract-ocr \
        tesseract-ocr-eng \
        tesseract-ocr-ces \
        tesseract-ocr-osd && \
    rm -rf /var/lib/apt/lists/*

# Default OCR configuration via env vars (your app will read these)
ENV OCR_LANG=eng+ces \
    OCR_PSM=6 \
    OCR_OEM=3 \
    OCR_PRESERVE_SPACES=1 \
    OCR_FORMAT=text \
    ASPNETCORE_URLS=http://0.0.0.0:8080

# Drop back to non-root user in ASP.NET image
USER app

WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "MergeDay.Api.dll"]
