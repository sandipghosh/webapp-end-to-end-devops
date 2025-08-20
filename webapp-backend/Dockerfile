# ============================
# Stage 1: Build dotnet App
# ============================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy the solution file
COPY webapp-backend.sln ./

# Copy all the project files (helps with caching restore step)
COPY src/model/model.csproj src/model/
COPY src/dataaccess/dataaccess.csproj src/dataaccess/
COPY src/webapp/webapp.csproj src/webapp/

# Restore dependencies
RUN dotnet restore webapp-backend.sln

# Copy the entire source code
COPY . .

# Build and publish the webapp project
WORKDIR /app/src/webapp
RUN dotnet publish webapp.csproj -c Release -o /app/publish

# ============================
# Stage 2: Serve with dotnet runtime
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copy the published output from build stage
COPY --from=build /app/publish .

# Set env vars for runtime
ENV WEBAPP_DB_HOST=localhost
ENV WEBAPP_DB_PORT=3306
ENV WEBAPP_DB_NAME=app_db
ENV WEBAPP_DB_USER=sandip
ENV WEBAPP_DB_PASSWORD=sandev@1984
ENV WEBAPP_CLIENT_URL="http://localhost:4200"

# Expose the port used by the webapp
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

# Copy wait-for-it script
COPY wait-for-it.sh /wait-for-it.sh
RUN chmod +x /wait-for-it.sh

# Start the application
#ENTRYPOINT ["dotnet", "webapp.dll"]
ENTRYPOINT ["/wait-for-it.sh", "localhost:3306", "--", "dotnet", "webapp.dll"]
