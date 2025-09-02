# ==============================================
# Stage 1: Build dotnet App
# ==============================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy the solution file
COPY webapp-backend.sln ./

# Copy all the project files (helps with caching restore step)
COPY src/model/model.csproj src/model/
COPY src/dataaccess/dataaccess.csproj src/dataaccess/
COPY src/webapp/webapp.csproj src/webapp/
COPY test/database.test/database.test.csproj test/database.test/
COPY test/webapp.test/webapp.test.csproj test/webapp.test/

# Restore dependencies
RUN dotnet restore webapp-backend.sln

# Copy the entire source code
COPY . .

# ==============================================
# Stage 2: Run tests (fail build if tests fail)
# ==============================================
FROM build AS test
WORKDIR /app

# run tests (this will stop build if any test fails)
RUN dotnet test test/database.test/database.test.csproj --no-restore --no-build --verbosity normal
RUN dotnet test test/webapp.test/webapp.test.csproj --no-restore --no-build --verbosity normal

# ==============================================
# Stage 2: Publishing the tested code
# ==============================================
FROM test AS publish
WORKDIR /app

# Build and publish the webapp project
WORKDIR /app/src/webapp
RUN dotnet publish webapp.csproj -c Release -o /app/publish

# ============================
# Stage 2: Serve with dotnet runtime
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copy the published output from build stage
COPY --from=publish /app/publish .

ARG DB_HOST
ARG DB_PORT
ARG DB_NAME
ARG DB_USER
ARG DB_PASSWORD
ARG CLIENT_URL
ARG ASPNETCORE_URL
ARG EXTERNAL_PORT

# Set env vars for runtime
ENV WEBAPP_DB_HOST=${DB_HOST}
ENV WEBAPP_DB_PORT=${DB_PORT}
ENV WEBAPP_DB_NAME=${DB_NAME}
ENV WEBAPP_DB_USER=${DB_USER}
ENV WEBAPP_DB_PASSWORD=${DB_PASSWORD}
ENV WEBAPP_CLIENT_URL=${CLIENT_URL}
ENV WEBAPP_EXPOSSED_PORT=${EXTERNAL_PORT}

# Expose the port used by the webapp
EXPOSE ${WEBAPP_EXPOSSED_PORT}
ENV ASPNETCORE_URLS="http://+:${WEBAPP_EXPOSSED_PORT}"

# Copy wait-for-it script
COPY wait-for-it.sh /wait-for-it.sh
RUN chmod +x /wait-for-it.sh

# Start the application
#ENTRYPOINT ["dotnet", "webapp.dll"]
#ENTRYPOINT ["/wait-for-it.sh", "${WEBAPP_DB_HOST}:${WEBAPP_DB_PORT}", "--", "dotnet", "webapp.dll"]
ENTRYPOINT ["/bin/sh", "-c", "/wait-for-it.sh ${WEBAPP_DB_HOST}:${WEBAPP_DB_PORT} -- dotnet webapp.dll"]

