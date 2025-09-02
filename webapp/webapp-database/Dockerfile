# Use the official MySQL base image
FROM mysql:8.0

ARG DATABASE_NAME
ARG USER_NAME
ARG USER_PASSWORD 
ARG ROOT_PASSWORD
ARG DATABASE_PORT

# Set environment variables (⚠️ do not use for sensitive values in real projects)
ENV MYSQL_DATABASE=${DATABASE_NAME} 
ENV MYSQL_USER=${USER_NAME} 
ENV MYSQL_PASSWORD=${USER_PASSWORD} 
ENV MYSQL_ROOT_PASSWORD=${ROOT_PASSWORD}

# Copy initialization SQL scripts (optional)
# Any .sql or .sh files here will be executed on first startup
COPY ./init.sql /docker-entrypoint-initdb.d/

# Expose MySQL port
EXPOSE ${DATABASE_PORT}