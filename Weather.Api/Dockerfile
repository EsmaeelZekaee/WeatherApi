## See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Stage 1 - Base
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8182
EXPOSE 8183

# نصب curl و filebeat
USER root
RUN apt-get update && \
    apt-get install -y curl gnupg && \
    curl -L https://artifacts.elastic.co/GPG-KEY-elasticsearch | apt-key add - && \
    echo "deb https://artifacts.elastic.co/packages/8.x/apt stable main" > /etc/apt/sources.list.d/elastic-8.x.list && \
    apt-get update && \
    apt-get install -y filebeat && \
    apt-get clean

# کپی کانفیگ filebeat
COPY filebeat/filebeat.yml /etc/filebeat/filebeat.yml
RUN chmod go-w /etc/filebeat/filebeat.yml

# Stage 2 - Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Weather.Api.csproj", "./"]
RUN dotnet restore "./Weather.Api.csproj"
COPY . .
RUN dotnet build "./Weather.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet tool install --global dotnet-trace
# Stage 3 - Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Weather.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 4 - Final
FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

# اسکریپت entrypoint که هم API و هم Filebeat را اجرا می‌کند
COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

ENTRYPOINT ["/entrypoint.sh"]
