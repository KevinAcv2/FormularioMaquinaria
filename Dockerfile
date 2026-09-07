FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

COPY ["FormularioMaquinaria.csproj", "./"]

RUN dotnet restore

COPY . .

RUN dotnet publish "FormularioMaquinaria.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FormularioMaquinaria.dll"]