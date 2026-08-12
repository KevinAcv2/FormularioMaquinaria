FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build

WORKDIR /src

COPY ["FormularioMaquinaria.csproj", "./"]

RUN dotnet restore

COPY . .

RUN dotnet publish "FormularioMaquinaria.csproj" -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview

WORKDIR /app

RUN apt-get update \
    && apt-get install -y \
        libleptonica-dev \
        libtesseract-dev \
        tesseract-ocr \
    && echo "===== LEPTONICA =====" \
    && ls -la /usr/lib/x86_64-linux-gnu/liblept* \
    && echo "===== TESSERACT =====" \
    && ls -la /usr/lib/x86_64-linux-gnu/libtess* \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

COPY tessdata ./tessdata

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "FormularioMaquinaria.dll"]