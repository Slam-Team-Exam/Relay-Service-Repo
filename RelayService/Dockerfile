# Use Microsoft’s official .NET SDK image to build the project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy everything and restore dependencies
COPY . .
RUN dotnet restore

# Build and publish the app
RUN dotnet publish -c Release -o out

# Use a lightweight runtime image for the final container
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .

# Expose the port your app runs on
EXPOSE 7282

# Run the application
ENTRYPOINT ["dotnet", "Login-System.dll"]
