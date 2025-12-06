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

COPY ["src/Common/Common.Domain/Common.Domain.csproj", "src/Common/Common.Domain/"]
COPY ["src/Common/Common.Application/Common.Application.csproj", "src/Common/Common.Application/"]
COPY ["src/Common/Common.Infrastructure/Common.Infrastructure.csproj", "src/Common/Common.Infrastructure/"]
COPY ["src/Common/Common.Presentations/Common.Presentation.csproj", "src/Common/Common.Presentations/"]

COPY ["src/Services/Accounts/Services.Accounts.Presentation/Services.Accounts.Presentation.csproj", "src/Services/Accounts/Services.Accounts.Presentation/"]
COPY ["src/Services/Accounts/Services.Accounts.Application/Services.Accounts.Application.csproj", "src/Services/Accounts/Services.Accounts.Application/"]
COPY ["src/Services/Accounts/Services.Accounts.Domain/Services.Accounts.Domain.csproj", "src/Services/Accounts/Services.Accounts.Domain/"]
COPY ["src/Services/Accounts/Services.Accounts.Infrastructure/Services.Accounts.Infrastructure.csproj", "src/Services/Accounts/Services.Accounts.Infrastructure/"]

RUN dotnet restore "./src/Services/Accounts/Services.Accounts.Presentation/Services.Accounts.Presentation.csproj"
COPY . .
WORKDIR "/src/src/Services/Accounts/Services.Accounts.Presentation"
RUN dotnet build "./Services.Accounts.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Services.Accounts.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Services.Accounts.Presentation.dll"]