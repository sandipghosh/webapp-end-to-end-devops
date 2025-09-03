## Docker images formatted view
```bash
docker ps --format "table {{.ID}}\t{{.Image}}\t{{.Names}}\t{{.Status}}\t{{.Networks}}\t{{.Ports}}"
```

## docker compose up all the containers
```bash
docker compose --env-file secrets.env up -d --build --remove-orphans --force-recreate
```

## docker compose down all the containers
```bash
docker compose --env-file secrets.env down 
docker compose --env-file secrets.env down -v # Remove volume as well
docker compose --env-file secrets.env down --rmi all # Remove all downloaded images
```