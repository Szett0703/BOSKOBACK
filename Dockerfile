# Imagen base para construir la API
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos de proyecto
COPY *.csproj ./
RUN dotnet restore

# Copiar todo el proyecto
COPY . .

# Publicar en modo Release
RUN dotnet publish -c Release -o /app/publish

# Imagen final para ejecutar la API
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copiar archivos publicados
COPY --from=build /app/publish .

# Railway SIEMPRE usa el puerto 8080 internamente
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "DBTest_BACK.dll"]
