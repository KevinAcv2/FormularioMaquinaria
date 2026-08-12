# ============================================================
# ETAPA 1 - COMPILACIÓN
# ============================================================

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build

WORKDIR /src

COPY ["FormularioMaquinaria.csproj", "./"]

RUN dotnet restore

COPY . .

RUN dotnet publish "FormularioMaquinaria.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


# ============================================================
# ETAPA 2 - EJECUCIÓN
# ============================================================

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview

WORKDIR /app


# ============================================================
# DEPENDENCIAS NATIVAS DE TESSERACT
# ============================================================

RUN apt-get update && \
    apt-get install -y \
        tesseract-ocr \
        tesseract-ocr-eng \
        libleptonica-dev \
        libtesseract-dev \
        libgomp1 \
        libjpeg62-turbo \
        libpng16-16 \
        libtiff6 \
        libwebp7 \
        libopenjp2-7 \
        ca-certificates \
    && rm -rf /var/lib/apt/lists/*


# ============================================================
# VERIFICAR LAS LIBRERÍAS
# ============================================================

RUN ldconfig -p | grep -i lept


# ============================================================
# COPIAR APLICACIÓN
# ============================================================

COPY --from=build /app/publish .


# ============================================================
# COPIAR TESSDATA
# ============================================================

COPY tessdata ./tessdata


# ============================================================
# CONFIGURACIÓN
# ============================================================

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "FormularioMaquinaria.dll"]