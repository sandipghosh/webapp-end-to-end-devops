# Use the official MySQL base image
FROM mysql:8.0

# Set environment variables (⚠️ do not use for sensitive values in real projects)
ENV MYSQL_DATABASE=app_db \
	MYSQL_USER=sandip \
	MYSQL_PASSWORD=sandev@1984 \
	MYSQL_ROOT_PASSWORD=root

# Copy initialization SQL scripts (optional)
# Any .sql or .sh files here will be executed on first startup
COPY ./init.sql /docker-entrypoint-initdb.d/

# Expose MySQL port
EXPOSE 3306
