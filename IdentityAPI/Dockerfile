FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY ["IdentityAPI/IdentityAPI.csproj", "IdentityAPI/"]
RUN dotnet restore "IdentityAPI/IdentityAPI.csproj"

COPY . .

WORKDIR "/app/IdentityAPI"
RUN dotnet publish "IdentityAPI.csproj" -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

EXPOSE 8080
ENTRYPOINT ["dotnet", "IdentityAPI.dll"]
