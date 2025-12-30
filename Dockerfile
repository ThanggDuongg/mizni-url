FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build

WORKDIR /src

COPY URLService/Directory.Build.props URLService/
COPY URLService/Directory.Packages.props URLService/
COPY URLService/URLService.sln URLService/

COPY URLService/WebAPI/WebAPI.csproj URLService/WebAPI/
COPY URLService/Application/Application.csproj URLService/Application/
COPY URLService/Domain/Domain.csproj URLService/Domain/
COPY URLService/Infrastructure/Infrastructure.csproj URLService/Infrastructure/
COPY URLService/Shared/Shared.csproj URLService/Shared/
COPY Contracts/Contracts.csproj Contracts/

RUN dotnet restore URLService/URLService.sln

COPY URLService/ URLService/
COPY Contracts/ Contracts/

WORKDIR /src/URLService/WebAPI
RUN dotnet publish \
    --no-restore \
    --configuration Release \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime

LABEL maintainer="thangduc.duong14@gmail.com" \
      version="0.0.1" \
      description="URL Service WebAPI"

RUN apk add --no-cache \
    wget \
    icu-libs \
    icu-data-full \
    tzdata

WORKDIR /app

RUN addgroup -g 1000 appuser && \
    adduser -D -u 1000 -G appuser appuser && \
    mkdir -p /app/logs && \
    chown -R appuser:appuser /app

COPY --from=build --chown=appuser:appuser /app/publish .

USER appuser

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    TZ=Asia/Ho_Chi_Minh

HEALTHCHECK --interval=30s --timeout=3s --start-period=15s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "WebAPI.dll"]