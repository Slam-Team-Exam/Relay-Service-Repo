FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file and restore (keeps restore layer cached)
COPY ["RelayService.csproj", "./"]
RUN dotnet restore "RelayService.csproj"

# Copy the rest of the source
COPY . .

# Build to surface compilation errors early (do not restore again)
RUN dotnet build "RelayService.csproj" -c Release

# Publish without restoring (restore already done) and show diagnostic logs
RUN dotnet publish "RelayService.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Listen on container port 80
ENV ASPNETCORE_URLS=http://+:80

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "RelayService.dll"]
