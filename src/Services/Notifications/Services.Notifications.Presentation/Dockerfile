# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "."]

COPY ["src/Common/Common.Domain/Common.Domain.csproj", "src/Common/Common.Domain/"]
COPY ["src/Common/Common.Application/Common.Application.csproj", "src/Common/Common.Application/"]
COPY ["src/Common/Common.Infrastructure/Common.Infrastructure.csproj", "src/Common/Common.Infrastructure/"]
COPY ["src/Common/Common.Presentation/Common.Presentation.csproj", "src/Common/Common.Presentation/"]

COPY ["src/Services/Notifications/Services.Notifications.Presentation/Services.Notifications.Presentation.csproj", "src/Services/Notifications/Services.Notifications.Presentation/"]
COPY ["src/Services/Notifications/Services.Notifications.Application/Services.Notifications.Application.csproj", "src/Services/Notifications/Services.Notifications.Application/"]
COPY ["src/Services/Notifications/Services.Notifications.Domain/Services.Notifications.Domain.csproj", "src/Services/Notifications/Services.Notifications.Domain/"]

RUN dotnet restore "./src/Services/Notifications/Services.Notifications.Presentation/Services.Notifications.Presentation.csproj"
COPY . .
WORKDIR "/src/src/Services/Notifications/Services.Notifications.Presentation"
RUN dotnet build "./Services.Notifications.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Services.Notifications.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Services.Notifications.Presentation.dll"]