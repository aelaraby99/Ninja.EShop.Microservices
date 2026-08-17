# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
HEALTHCHECK --interval=5s --timeout=3s --start-period=30s --retries=5 \
  CMD curl -f http://localhost:8080/health || exit 1


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/NINJA.EShop.Basket/NINJA.EShop.Basket.API/NINJA.EShop.Basket.API.csproj", "src/NINJA.EShop.Basket/NINJA.EShop.Basket.API/"]
RUN dotnet restore "./src/NINJA.EShop.Basket/NINJA.EShop.Basket.API/NINJA.EShop.Basket.API.csproj"
COPY . .
WORKDIR "/src/src/NINJA.EShop.Basket/NINJA.EShop.Basket.API"
RUN dotnet build "./NINJA.EShop.Basket.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./NINJA.EShop.Basket.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NINJA.EShop.Basket.API.dll"]