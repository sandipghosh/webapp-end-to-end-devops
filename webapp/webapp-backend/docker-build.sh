# Load variables from .env file
export $(grep -v '^#' *.env | xargs)

# Build passing build args
docker build \
  --build-arg DB_HOST=$DB_HOST \
  --build-arg DB_PORT=$DB_PORT \
  --build-arg DB_NAME=$DB_NAME \
  --build-arg DB_USER=$DB_USER \
  --build-arg DB_PASSWORD=$DB_PASSWORD \
  --build-arg CLIENT_URL=$CLIENT_URL \
  --build-arg ASPNETCORE_URL=$ASPNETCORE_URL \
  --build-arg EXTERNAL_PORT=$EXTERNAL_PORT \
  --no-cache \
  -t $TAG .