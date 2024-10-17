# Use the official .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

# Copy the .csproj file and restore dependencies first
COPY ./NewLife_Web_api/NewLife_Web_api.csproj ./NewLife_Web_api/
RUN dotnet restore ./NewLife_Web_api/NewLife_Web_api.csproj

# Copy the rest of the project files and build the release version
COPY ./NewLife_Web_api/. ./NewLife_Web_api/
RUN dotnet publish ./NewLife_Web_api/NewLife_Web_api.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App
COPY --from=build-env /App/out ./

EXPOSE 8080

# Set the entry point to run your application
ENTRYPOINT ["dotnet", "NewLife_Web_api.dll"]
