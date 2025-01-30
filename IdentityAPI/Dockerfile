# Użyj obrazu .NET SDK do budowania aplikacji
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Skopiuj całą aplikację do kontenera
COPY . .

# Przywróć zależności, zbuduj i opublikuj aplikację
RUN dotnet restore "IdentityAPI.csproj"
RUN dotnet publish "IdentityAPI.csproj" -c Release -o /app --no-restore

# Użyj obrazu runtime do uruchamiania aplikacji
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app ./

# Otwórz porty 8080 i 5001
EXPOSE 8080 5001
ENTRYPOINT ["dotnet", "IdentityAPI.dll"]
