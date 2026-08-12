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
# DEPENDENCIAS NATIVAS PARA TESSERACT
# ============================================================

RUN apt-get update && \
    apt-get install -y \
        tesseract-ocr \
        tesseract-ocr-eng \
        libleptonica-dev \
        libtesseract-dev \
        ca-certificates \
        libgomp1 \
        libjpeg62-turbo \
        libpng16-16 \
        libtiff6 \
        libwebp7 \
        libopenjp2-7 \
    && rm -rf /var/lib/apt/lists/*


# ============================================================
# CREAR ALIAS DE LEPTONICA
# Tesseract 5.2.0 busca específicamente:
#
# libleptonica-1.82.0.so
# ============================================================

RUN LEPT_LIB=$(find /usr/lib /lib \
        -type f \
        \( -name "liblept.so.*" -o -name "libleptonica.so.*" \) \
        | head -n 1) \
    && echo "Leptonica encontrada: $LEPT_LIB" \
    && if [ -n "$LEPT_LIB" ]; then \
        ln -sf "$LEPT_LIB" /usr/lib/x86_64-linux-gnu/libleptonica-1.82.0.so; \
       else \
        echo "ERROR: No se encontró Leptonica"; \
        exit 1; \
       fi


# ============================================================
# COPIAR LA APLICACIÓN
# ============================================================

COPY --from=build /app/publish .


# ============================================================
# COPIAR TESSDATA
# ============================================================

COPY tessdata ./tessdata


# ============================================================
# CONFIGURACIÓN ASP.NET
# ============================================================

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "FormularioMaquinaria.dll"]