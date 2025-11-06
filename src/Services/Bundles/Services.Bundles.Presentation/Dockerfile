# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "."]

COPY ["src/Common/Common.Presentations/Common.Presentation.csproj", "src/Common/Common.Presentations/"]

COPY ["src/Services/Bundles/Services.Bundles.Infrastructure/Services.Bundles.Infrastructure.csproj", "src/Services/Bundles/Services.Bundles.Infrastructure/"]
COPY ["src/Services/Bundles/Services.Bundles.Infrastructure/Services.Bundles.Infrastructure.csproj", "src/Services/Bundles/Services.Bundles.Infrastructure/"]
RUN dotnet restore "./src/Services/Bundles/Services.Bundles.Presentation/Services.Bundles.Presentation.csproj"
COPY . .
WORKDIR "/src/src/Services/Bundles/Services.Bundles.Presentation"
RUN dotnet build "./Services.Bundles.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Services.Bundles.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Services.Bundles.Presentation.dll"]