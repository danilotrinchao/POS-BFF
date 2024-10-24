# Build image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia o arquivo de solu��o
COPY ["POS_BFF.sln", "."]
COPY ["POS_BFF.Presentation.Api/POS_BFF.Presentation.Api.csproj", "POS_BFF.Presentation.Api/"]
COPY ["POS_BFF.Application/POS_BFF.Application.csproj", "POS_BFF.Application/"]
COPY ["POS_BFF.Domain/POS_BFF.Core.Domain.csproj", "POS_BFF.Domain/"]
COPY ["POS_BFF.Infra/POS_BFF.Infra.csproj", "POS_BFF.Infra/"]

# Restaura os pacotes
RUN dotnet restore "./POS_BFF.sln"

# Copia o resto do c�digo para o container
COPY . .

# Define a pasta de trabalho como /src onde est� a solu��o
WORKDIR "/src/POS_BFF.Presentation.Api"

# Builda a solu��o completa
RUN dotnet build "POS_BFF.Presentation.Api.csproj" -c Release -o /app/build

# Publica a solu��o completa
FROM build AS publish
RUN dotnet publish "POS_BFF.Presentation.Api.csproj" -c Release -o /app/publish
# Verifique os arquivos publicados
RUN ls -la /app/publish

# Usa a imagem base para rodar a aplica��o
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# ENTRYPOINT para iniciar o servi�o com a DLL correta
ENTRYPOINT ["dotnet", "POS_BFF.Presentation.Api.dll"]
