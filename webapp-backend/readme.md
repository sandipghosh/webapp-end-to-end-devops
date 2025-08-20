execute the following command to create migration 

Step 1:
========================================
export DB_SERVER=localhost
export DB_PORT=3306
export DB_NAME=app_db
export DB_USER=sandip
export DB_PASSWORD=sandev@1984

dotnet ef migrations add InitialCreate --startup-project src/webapp/webapp.csproj --project src/dataaccess/dataaccess.csproj --context ApplicationDbContext

dotnet ef migrations script --startup-project src/webapp/webapp.csproj --project src/dataaccess/dataaccess.csproj --context ApplicationDbContext


Step 2:
========================================

dotnet ef database update --project src/dataaccess/dataaccess.csproj --context ApplicationDbContext


Stand alone docker run
========================================
docker run -d -p 8080:80 --name webapp-backend --network test-network -e WEBAPP_DB_HOST=webapp-database -e WEBAPP_CLIENT_URL="http://webapp-frontend:82,http://localhost:82,http://localhost:4200" webapp-backend



