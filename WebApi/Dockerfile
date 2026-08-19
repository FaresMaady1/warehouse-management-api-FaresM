# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY WebApi/WebApi.csproj WebApi/
COPY Warehouse.Domain/Warehouse.Domain.csproj Warehouse.Domain/
COPY Warehouse.Application/Warehouse.Application.csproj Warehouse.Application/
COPY Warehouse.Infrastructure/Warehouse.Infrastructure.csproj Warehouse.Infrastructure/

RUN dotnet restore WebApi/WebApi.csproj


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
