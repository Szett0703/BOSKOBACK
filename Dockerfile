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

# Railway usa PUERTOS DINÁMICOS ($PORT)
ENV ASPNETCORE_URLS=http://0.0.0.0:$PORT

# Exponer un puerto estándar
EXPOSE 8080

# Ejecutar la API - ESTE ES EL NOMBRE CORRECTO DEL DLL
ENTRYPOINT ["dotnet", "DBTest-BACK.dll"]
