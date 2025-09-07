#!/bin/bash

SERVICE="$1"
IMAGE_TAG="$(echo "${GITHUB_SHA}" | cut -c1-7)"
VALUES_FILE="./infra-chart/values.yaml"

git config --local user.email "gitops@github.com"
git config --local user.name "GitOps Bot"

if ! git diff --quiet "${VALUES_FILE}"; then
  git add "${VALUES_FILE}"
  git commit -m "🚀 Update ${SERVICE} Helm chart to ${IMAGE_TAG} - ECR: retail-store-${SERVICE} - Commit: ${GITHUB_SHA}"

  for i in {1..3}; do
    if git push origin gitops; then
      echo "✅ Successfully pushed Helm update for ${SERVICE}"
      break
    else
      echo "⚠️ Push failed (attempt $i). Retrying..."
      git pull --rebase origin gitops
      sleep 2
    fi

    [[ $i -eq 3 ]] && echo "❌ Failed to push after 3 attempts" && exit 1
  done
else
  echo "📝 No Helm changes to commit for ${SERVICE}"
fi


