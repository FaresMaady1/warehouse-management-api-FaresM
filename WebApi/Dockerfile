# syntax=docker/dockerfile:1
# Build context for this Dockerfile is the "WebApi" solution folder
# (the one containing WebApi.sln, WebApi/, Warehouse.Domain/, Warehouse.Application/, Warehouse.Infrastructure/)

# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only the .csproj files first so `dotnet restore` is cached across
# rebuilds unless a dependency actually changes.
COPY WebApi/WebApi.csproj WebApi/
COPY Warehouse.Domain/Warehouse.Domain.csproj Warehouse.Domain/
COPY Warehouse.Application/Warehouse.Application.csproj Warehouse.Application/
COPY Warehouse.Infrastructure/Warehouse.Infrastructure.csproj Warehouse.Infrastructure/

RUN dotnet restore WebApi/WebApi.csproj

# Now copy the actual source for the four projects the API needs.
# (tests/ is intentionally not copied - it's not needed to run the API.)
COPY Warehouse.Domain/ Warehouse.Domain/
COPY Warehouse.Application/ Warehouse.Application/
COPY Warehouse.Infrastructure/ Warehouse.Infrastructure/
COPY WebApi/ WebApi/

WORKDIR /src/WebApi
RUN dotnet publish WebApi.csproj -c Release -o /app/publish --no-restore

# ---------- Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "WebApi.dll"]
