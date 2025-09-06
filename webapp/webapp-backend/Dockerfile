# ==============================================
# Stage 1: Build dotnet App (restore + build)
# ==============================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy the solution and project files first (layered caching)
COPY webapp-backend.sln ./

COPY src/model/model.csproj src/model/
COPY src/dataaccess/dataaccess.csproj src/dataaccess/
COPY src/webapp/webapp.csproj src/webapp/
COPY test/database.test/database.test.csproj test/database.test/
COPY test/webapp.test/webapp.test.csproj test/webapp.test/

# Restore dependencies (uses the copied csproj files)
RUN dotnet restore webapp-backend.sln

# Copy rest of source into the image
COPY . .

# Build the solution so test binaries are produced for the test stage
# Use Debug so tests run against Debug outputs (matches dotnet test default)
RUN dotnet build webapp-backend.sln -c Debug

# ==============================================
# Stage 2: Run tests (fail build if tests fail)
# ==============================================
FROM build AS test
WORKDIR /app

# Run tests — no-restore and no-build is safe because we built in the previous stage
RUN dotnet test test/database.test/database.test.csproj --no-restore --no-build --verbosity detailed
RUN dotnet test test/webapp.test/webapp.test.csproj --no-restore --no-build --verbosity detailed

# ==============================================
# Stage 3: Publish the tested code (Release)
# ==============================================
FROM test AS publish
WORKDIR /app

# Publish the webapp project as Release output to /app/publish
WORKDIR /app/src/webapp
RUN dotnet publish webapp.csproj -c Release -o /app/publish

# ==============================================
# Stage 4: Run with dotnet runtime
# ==============================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copy the published output from the publish stage
COPY --from=publish /app/publish .

# Build-time args (do not put real secrets here in your CI)
ARG DB_HOST
ARG DB_PORT
ARG DB_NAME
ARG DB_USER
ARG DB_PASSWORD
ARG CLIENT_URL
ARG ASPNETCORE_URL
ARG EXTERNAL_PORT

# Map args to runtime envs (note: prefer injecting secrets at runtime)
ENV WEBAPP_DB_HOST=${DB_HOST}
ENV WEBAPP_DB_PORT=${DB_PORT}
ENV WEBAPP_DB_NAME=${DB_NAME}
ENV WEBAPP_DB_USER=${DB_USER}
ENV WEBAPP_DB_PASSWORD=${DB_PASSWORD}
ENV WEBAPP_CLIENT_URL=${CLIENT_URL}
ENV WEBAPP_EXPOSED_PORT=${EXTERNAL_PORT}

# Expose and set ASPNETCORE urls
EXPOSE ${WEBAPP_EXPOSED_PORT}
ENV ASPNETCORE_URLS="http://+:${WEBAPP_EXPOSED_PORT}"

# Copy wait-for-it script
#COPY wait-for-it.sh /wait-for-it.sh
#RUN chmod +x /wait-for-it.sh

# Install netcat (nc) for Kubernetes probes if needed
RUN apt-get update && \
    apt-get install -y --no-install-recommends netcat-openbsd && \
    rm -rf /var/lib/apt/lists/*

# Start the application
ENTRYPOINT ["dotnet", "webapp.dll"]
#ENTRYPOINT ["/bin/sh", "-c", "/wait-for-it.sh ${WEBAPP_DB_HOST}:${WEBAPP_DB_PORT} -- dotnet webapp.dll"]

