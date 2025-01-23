# Use the .NET SDK as a base image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the entire source code to the container
COPY . .

# Restore dependencies, build, and publish the application
RUN dotnet restore "IdentityAPI.csproj" --disable-parallel
RUN dotnet publish "IdentityAPI.csproj" -c Release -o /app --no-restore

# Use the ASP.NET runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app ./

# Expose the required port and set the entry point
EXPOSE 8080
ENTRYPOINT ["dotnet", "IdentityAPI.dll"]
