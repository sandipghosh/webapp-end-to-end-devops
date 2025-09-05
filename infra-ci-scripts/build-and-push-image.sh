#!/bin/bash

SERVICE=$1
NAMESPACE="webapp"
AWS_ACCOUNT_ID="${AWS_ACCOUNT_ID}"
REPO_NAME="${NAMESPACE}/${SERVICE}"

# ECR repo naming format
# <aws_account_id>.dkr.ecr.<region>.amazonaws.com/webapp/frontend:tag
IMAGE_NAME="${AWS_ACCOUNT_ID}.dkr.ecr.${AWS_REGION}.amazonaws.com/${REPO_NAME}"
IMAGE_TAG="$(echo "${GITHUB_SHA}" | cut -c1-7)"

echo "Building docker image: ${SERVICE}:${IMAGE_TAG}"

# Redirects the result of the "ecr describe-repositories" standard error (stderr) to /dev/null
# /dev/null is a “black hole” device in Linux/Unix → anything written there is discarded.
# Every process has a few special "channels" for input/output, each represented by a number:
# 0 --> stdin -->   Standard input (what the program reads from, e.g., keyboard or pipe)
# 1 --> stdout -->  Standard output (normal program output)
# 2 --> stderr -->  Standard error (error messages)
# "process 2>/dev/null" means standard error to “black hole” 
# "process > /dev/null 2>&1" means standard output to “black hole” and send stderr to the same location as stdout.

#aws ecr describe-repositories \
#    --repository-names "${ECR_REPO}" 2>/dev/null || \
#aws ecr create-repository \
#    --repository-name "${ECR_REPO}" \
#    --image-scanning-configuration scanOnPush=true \
#    --encryption-configuration encryptionType=AES256
    
if !aws ecr describe-repositories \
    --region "${AWS_REGION}" \
    --repository-names "${REPO_NAME}" > /dev/null 2>&1; then
    echo "ECR Repository ${REPO_NAME} not found; creating a new repository ${REPO_NAME}"

    aws ecr create-repository \
        --region "${AWS_REGION}" \
        --repository-name "${REPO_NAME}" \
        --image-scanning-configuration scanOnPush=true \
        --encryption-configuration encryptionType=AES256
else
    echo "ECR Repository ${REPO_NAME} exists.."
fi

# building docker image with 2 tags (1st one is from git commit SHA and the 2nd one is latest)
docker build -t "${IMAGE_NAME}:${IMAGE_TAG}" -t "${IMAGE_NAME}:latest" "./webapp/${SERVICE}"
echo "Build docker images with 2 tags (1st one is from git commit SHA and the 2nd one is latest)"

docker push "${IMAGE_NAME}:${IMAGE_TAG}"
echo "Pushed the image ${IMAGE_NAME}:${IMAGE_TAG} to AWS ECR"

docker push "${IMAGE_NAME}:latest" 
echo "Pushed the image ${IMAGE_NAME}:latest to AWS ECR"

echo "ECR_REPO=${IMAGE_NAME}" >> "$GITHUB_ENV"
echo "Saving the container image URL into GitHub environment context"