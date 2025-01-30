FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY ["PostAPI/PostAPI.csproj", "PostAPI/"]
RUN dotnet restore "PostAPI/PostAPI.csproj"

COPY . .

WORKDIR "/app/PostAPI"
RUN dotnet publish "PostAPI.csproj" -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

EXPOSE 8080
ENTRYPOINT ["dotnet", "PostAPI.dll"]
