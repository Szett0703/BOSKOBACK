# Imagen base para construir la API
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos de proyecto
COPY *.csproj ./
RUN dotnet restore

# Copiar el resto del proyecto
COPY . .

# Publicar en modo Release
RUN dotnet publish -c Release -o /app/publish

# Imagen final para ejecutar la API
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copiar los archivos publicados
COPY --from=build /app/publish .

# Railway usa puerto dinámico ($PORT)
ENV ASPNETCORE_URLS=http://0.0.0.0:$PORT

# Exponer un puerto genérico
EXPOSE 8080

# Ejecutar EL DLL CORRECTO
ENTRYPOINT ["dotnet", "DBTest-BACK.dll"]
