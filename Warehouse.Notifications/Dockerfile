# syntax=docker/dockerfile:1
# Build context for this Dockerfile is the "Warehouse.Notifications" solution folder
# (the one containing Warehouse.Notifications.sln, Warehouse.Notifications.Api/, etc.)

# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Warehouse.Notifications.Api/Warehouse.Notifications.Api.csproj Warehouse.Notifications.Api/
COPY Warehouse.Notifications.Domain/Warehouse.Notifications.Domain.csproj Warehouse.Notifications.Domain/
COPY Warehouse.Notifications.Application/Warehouse.Notifications.Application.csproj Warehouse.Notifications.Application/
COPY Warehouse.Notifications.Infrastructure/Warehouse.Notifications.Infrastructure.csproj Warehouse.Notifications.Infrastructure/

RUN dotnet restore Warehouse.Notifications.Api/Warehouse.Notifications.Api.csproj

COPY Warehouse.Notifications.Domain/ Warehouse.Notifications.Domain/
COPY Warehouse.Notifications.Application/ Warehouse.Notifications.Application/
COPY Warehouse.Notifications.Infrastructure/ Warehouse.Notifications.Infrastructure/
COPY Warehouse.Notifications.Api/ Warehouse.Notifications.Api/

WORKDIR /src/Warehouse.Notifications.Api
RUN dotnet publish Warehouse.Notifications.Api.csproj -c Release -o /app/publish --no-restore

# ---------- Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Warehouse.Notifications.Api.dll"]
