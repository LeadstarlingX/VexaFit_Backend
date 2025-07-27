# Stage 1: Build the application
# Using .NET 8.0 SDK. Change the version if you use a different one.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /source

# Copy the solution and project files first to leverage Docker's layer cache.
# This step only re-runs if your project dependencies change.
COPY *.sln .
# COPY each project file. Replace with your actual project folder names.
COPY API/*.csproj ./API/
COPY Infrastructure/*.csproj ./Infrastructure/
COPY Application/*.csproj ./Application/
COPY Domain/*.csproj ./Domain/

# Restore dependencies for all projects in the solution
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Publish the API project in Release configuration
# *** REPLACE 'API/API.csproj' with the path to your API's .csproj file ***
WORKDIR /source/API
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Create the final runtime image
# Use the lightweight ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app/publish .

# Expose the port for HTTP traffic. The ASP.NET runtime image defaults to port 80.
EXPOSE 80

# Set the entrypoint for the container
# *** REPLACE 'YourApiProject.dll' with the name of your API's DLL file ***
ENTRYPOINT ["dotnet", "API.dll"]