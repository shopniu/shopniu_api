# 1. Imagen base para la ejecución (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

# libSkiaSharp.so enlaza dinámicamente libfontconfig (falta en la imagen slim);
# sin ella la confirmación de imágenes revienta con DllNotFoundException.
RUN apt-get update \
    && apt-get install -y --no-install-recommends libfontconfig1 \
    && rm -rf /var/lib/apt/lists/*

EXPOSE 8080
EXPOSE 8081

# 2. Imagen SDK para compilación
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG NUGET_GITHUB_TOKEN
ARG GITHUB_ACTOR
WORKDIR /src

# Copiar archivos .csproj para restaurar dependencias
COPY ["shopniu-api.csproj", "./"]
COPY ["nuget.config", "./"]

# Autenticarse contra GitHub Packages para poder restaurar el paquete privado
RUN dotnet nuget update source github --username $GITHUB_ACTOR --password $NUGET_GITHUB_TOKEN --store-password-in-clear-text

# Restaurar dependencias del proyecto principal
RUN dotnet restore "shopniu-api.csproj"

# Copiar el código fuente
COPY . .

# Compilar la API
RUN dotnet build "shopniu-api.csproj" -c Release -o /app/build

# 3. Publicación
FROM build AS publish
RUN dotnet publish "shopniu-api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. Imagen final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "shopniu-api.dll"]