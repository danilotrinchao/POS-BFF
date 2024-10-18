# Build image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files
COPY ["POS_BFF.Presentation.Api/POS_BFF.Presentation.Api.csproj", "POS_BFF.Presentation.Api/"]
COPY ["POS_BFF.Application/POS_BFF.Application.csproj", "POS_BFF.Application/"]
COPY ["POS_BFF.Domain/POS_BFF.Core.Domain.csproj", "POS_BFF.Core/"]
COPY ["POS_BFF.Infra/POS_BFF.Infra.csproj", "POS_BFF.Infra/"]

# Restore dependencies
RUN dotnet restore "POS_BFF.Presentation.Api/POS_BFF.Presentation.Api.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR "/src/POS_BFF.Presentation.Api"
RUN dotnet build "POS_BFF.Presentation.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "POS_BFF.Presentation.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image to run the application
# Usa a imagem base para rodar a aplicação
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "POS_BFF.Presentation.Api.dll"]
