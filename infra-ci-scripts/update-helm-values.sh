#!/bin/bash

SERVICE="$1"
IMAGE_TAG="$(echo "${GITHUB_SHA}" | cut -c1-7)"
IMAGE_REPO="${ECR_REPO}"

echo "Getting ECR repo: ${IMAGE_REPO} from ENV"