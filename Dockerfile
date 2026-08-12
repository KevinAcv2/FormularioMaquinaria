# ============================================================
# BUILD
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
# RUNTIME
# ============================================================

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "FormularioMaquinaria.dll"]