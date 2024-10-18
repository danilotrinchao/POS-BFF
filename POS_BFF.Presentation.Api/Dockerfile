# Build image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia o arquivo de solução
COPY ["POS_BFF.sln", "."]  
COPY ["POS_BFF.Presentation.Api/POS_BFF.Presentation.Api.csproj", "POS_BFF.Presentation.Api/"]
COPY ["POS_BFF.Application/POS_BFF.Application.csproj", "POS_BFF.Application/"]
COPY ["POS_BFF.Core/POS_BFF.Core.Domain.csproj", "POS_BFF.Core/"]
COPY ["POS_BFF.Infra/POS_BFF.Infra.csproj", "POS_BFF.Infra/"]

# Restaura os pacotes
RUN dotnet restore "POS_BFF.sln"

# Copia o resto do código para o container
COPY . .

# Define a pasta de trabalho como a do projeto de apresentação
WORKDIR "/src/POS_BFF.Presentation.Api"

# Builda o projeto de apresentação
RUN dotnet build "POS_BFF.Presentation.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publica o projeto
FROM build AS publish
RUN dotnet publish "POS_BFF.Presentation.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Usa a imagem base para rodar a aplicação
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "POS_BFF.Presentation.Api.dll"]
