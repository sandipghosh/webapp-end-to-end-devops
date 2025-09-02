## docker compose up all the containers
```docker compose --env-file secrets.env up -d --build```

## docker compose down all the containers
```bash
docker compose down 
docker compose down -v # Remove voulume as well
docker compose down --rmi all # Remove all downloaded images
```