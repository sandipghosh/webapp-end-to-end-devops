

## Execute the following command to run a local instance of mysql datatbase.
```bash
#Option 1
docker run -d -p 3306:3306 mysql-local:latest
#Option 2 (with environment file)
docker run --env-file secret.env --name mysql-local -d -p 3306:3306 mysql-local:latest 
```