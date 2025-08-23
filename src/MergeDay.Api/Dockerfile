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

# Final runtime image (with Tesseract installed)
FROM base AS final

# Install Tesseract and English language data (switch to root temporarily)
USER root
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
        tesseract-ocr \
        tesseract-ocr-ces \ 
        tesseract-ocr-eng \
        tesseract-ocr-osd && \
    rm -rf /var/lib/apt/lists/*
# Return to the non-root user used by the ASP.NET image
USER app

WORKDIR /app
COPY --from=publish /app/publish .

# Optional: explicitly bind to 8080 inside container
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

ENTRYPOINT ["dotnet", "MergeDay.Api.dll"]
