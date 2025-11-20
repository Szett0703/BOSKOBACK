# Imagen base para construir la API
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos del proyecto
COPY *.csproj ./
RUN dotnet restore

# Copiar el resto
COPY . .

# Publicar en modo Release
RUN dotnet publish -c Release -o /app/publish

# Imagen final para ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copiar archivos publicados
COPY --from=build /app/publish .

# Railway usa puerto dinámico
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}

# Expón el puerto dinámico
EXPOSE ${PORT}

# Ejecutar la API (Asegúrate de que el nombre está bien)
ENTRYPOINT ["dotnet", "DBTest_BACK.dll"]
