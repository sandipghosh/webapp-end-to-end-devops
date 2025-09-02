## Docker images formatted view
```bash
docker ps --format "table {{.ID}}\t{{.Image}}\t{{.Names}}\t{{.Status}}\t{{.Networks}}\t{{.Ports}}"
```

## docker compose up all the containers
```docker compose --env-file secrets.env up -d --build```

## docker compose down all the containers
```bash
docker compose down 
docker compose down -v # Remove volume as well
docker compose down --rmi all # Remove all downloaded images
```