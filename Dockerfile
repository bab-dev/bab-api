# Build stage
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

# Copy the solution file and restore dependencies
COPY ["API_BABWebApp.sln", "./"]
COPY . .
RUN dotnet restore

# Copy the project files and build the application
COPY ["API_BABWebApp/API_BABWebApp.csproj", "API_BABWebApp/"]
RUN dotnet build "API_BABWebApp/API_BABWebApp.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "API_BABWebApp/API_BABWebApp.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 5000
EXPOSE 5001

# Copy the XML file to the container
COPY ["API_BABWebApp/API_BABWebApp.xml", "/app/"]

# Copy the published application
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "API_BABWebApp.dll"]