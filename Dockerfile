# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY PricePulse/PricePulse.csproj PricePulse/
RUN dotnet restore PricePulse/PricePulse.csproj
COPY PricePulse/ PricePulse/
RUN dotnet publish PricePulse/PricePulse.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

# Config never changes in-container, and each watcher burns a host inotify instance from a shared
# limit — exhausting it throws out of CreateBuilder before the app can start.
ENV DOTNET_hostBuilder__reloadConfigOnChange=false

# TLS terminates at the host's edge, so the original scheme survives only in X-Forwarded-Proto. That
# proxy has no fixed address, so this trusts the headers from any caller — nothing else can reach us.
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true

EXPOSE 10000
# Render supplies the listening port in $PORT at runtime (10000 locally). Shell form
# is required so $PORT expands at container start; exec keeps dotnet as PID 1 for clean shutdown.
ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-10000} exec dotnet PricePulse.dll"]
