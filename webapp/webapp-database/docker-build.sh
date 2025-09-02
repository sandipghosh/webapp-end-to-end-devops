# Load variables from .env file
export $(grep -v '^#' *.env | xargs)

# Build passing build args
docker build \
  --build-arg DATABASE_NAME=$DATABASE_NAME \
  --build-arg USER_NAME=$USER_NAME \
  --build-arg USER_PASSWORD=$USER_PASSWORD \
  --build-arg ROOT_PASSWORD=$ROOT_PASSWORD \
  --build-arg DATABASE_PORT=$DATABASE_PORT \
  --no-cache \
  -t $TAG .
