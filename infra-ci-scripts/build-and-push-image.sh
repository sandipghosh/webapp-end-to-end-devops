#!/bin/bash

SERVICE=$1
TAG="$(echo "${GITHUB_SHA}")"
AWS_ACCOUNT_ID="${AWS_ACCOUNT_ID}"

echo "${TAG}"   