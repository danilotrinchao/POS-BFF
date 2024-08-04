# Base image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files
COPY ["AuthenticationService.Presentation.Api/AuthenticationService.Presentation.Api.csproj", "AuthenticationService.Presentation.Api/"]
COPY ["AuthenticationService.Application/AuthenticationService.Application.csproj", "AuthenticationService.Application/"]
COPY ["AuthenticationService.Domain/AuthenticationService.Core.Domain.csproj", "AuthenticationService.Core/"]
COPY ["AuthenticationService.Infra/AuthenticationService.Infra.csproj", "AuthenticationService.Infra/"]

# Restore dependencies
RUN dotnet restore "AuthenticationService.Presentation.Api/AuthenticationService.Presentation.Api.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR "/src/AuthenticationService.Presentation.Api"
RUN dotnet build "AuthenticationService.Presentation.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "AuthenticationService.Presentation.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image to run the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "AuthenticationService.Presentation.Api.dll"]
CMD ASPNETCORE_URLS="http://*:$PORT" dotnet AuthenticationService.Presentation.Api.dll
