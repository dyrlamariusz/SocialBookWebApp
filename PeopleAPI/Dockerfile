﻿FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY ["PeopleAPI/PeopleAPI.csproj", "PeopleAPI/"]
RUN dotnet restore "PeopleAPI/PeopleAPI.csproj"

COPY . .

WORKDIR "/app/PeopleAPI"
RUN dotnet publish "PeopleAPI.csproj" -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

EXPOSE 8080
ENTRYPOINT ["dotnet", "PeopleAPI.dll"]
