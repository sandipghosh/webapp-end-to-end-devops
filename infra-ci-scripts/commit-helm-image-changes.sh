#!/bin/bash

SERVICE="${1:-}"
if [ -z "$SERVICE" ]; then
  echo "Usage: $0 <service-name>"
  exit 2
fi

# Prefer GITHUB_SHA if set (CI), otherwise use current HEAD
IMAGE_TAG="$( (echo "${GITHUB_SHA:-}" | cut -c1-7) || true )"
if [ -z "$IMAGE_TAG" ]; then
    # Parses and prints first 7 characters revision identifiers 
    # (40-character SHA-1 commit hash) of the current HEAD commit
    IMAGE_TAG="$(git rev-parse --short=7 HEAD)"
fi

VALUES_FILE="./infra-chart/values.yaml"
BRANCH="gitops"
MAX_ATTEMPTS=3

git config --local user.email "gitops@github.com"
git config --local user.name "GitOps Bot"

# Ensure we have up-to-date refs
git fetch origin "$BRANCH" || true

# Checkout the branch (create local tracking branch if necessary)
if git show-ref --verify --quiet "refs/heads/$BRANCH"; then
  git checkout "$BRANCH"
else
  # try to track remote branch, otherwise create new local branch
  if git show-ref --verify --quiet "refs/remotes/origin/$BRANCH"; then
    git checkout -b "$BRANCH" "origin/$BRANCH"
  else
    git checkout -b "$BRANCH"
  fi
fi

# If there are no differences (both unstaged and staged) for the VALUES_FILE, exit early
if git diff --quiet -- "$VALUES_FILE" && git diff --cached --quiet -- "$VALUES_FILE"; then
  echo "📝 No Helm changes to commit for ${SERVICE}"
  exit 0
fi

# Stage and commit the specific values file only
git add -- "$VALUES_FILE"
# If there's nothing to commit git commit will fail — use || true to continue (we still may have staged changes)
if git commit -m "🚀 Update ${SERVICE} Helm chart to ${IMAGE_TAG} - ECR: retail-store-${SERVICE} - Commit: ${GITHUB_SHA:-$(git rev-parse --verify HEAD)}"; then
  echo "📝 Committed ${VALUES_FILE}"
else
  echo "⚠️ Nothing new to commit (commit returned non-zero). Continuing to push attempt."
fi

# Try to push, retrying up to MAX_ATTEMPTS; on push failure, attempt to rebase with autostash (or fallback to manual stash)
for i in $(seq 1 $MAX_ATTEMPTS); do
  if git push origin "$BRANCH"; then
    echo "✅ Successfully pushed Helm update for ${SERVICE}"
    exit 0
  else
    echo "⚠️ Push failed (attempt $i). Will try to integrate remote changes and retry..."

    # First try: use built-in autostash (works on modern git)
    if git pull --rebase --autostash origin "$BRANCH"; then
      echo "🔁 Successfully pulled & rebased (autostash) remote changes. Retrying push..."
      sleep 1
      continue
    fi

    # Fallback: manual stash/pull/rebase/pop (safer for older git)
    echo "🔁 --autostash failed or not supported. Trying manual stash + rebase..."
    STASH_NAME="gitops-autostash-$(date +%s)"
    # stash everything (including untracked) - if nothing to stash this returns non-zero, so ignore errors
    git stash push -u -m "$STASH_NAME" || true

    if git pull --rebase origin "$BRANCH"; then
      # attempt to restore stash; if conflicts occur, print message and exit non-zero
      if git stash list | grep -q "$STASH_NAME"; then
        if ! git stash pop; then
          echo "❌ Conflicts occurred while popping stash. Manual resolution required."
          echo "Hint: run 'git status' and resolve conflicts, then 'git rebase --continue' or 'git stash drop'."
          # leave working tree for manual resolution
          exit 1
        fi
      fi
      echo "🔁 Pulled & rebased remote changes with manual stash. Retrying push..."
      sleep 1
      continue
    else
      echo "❌ Failed to pull & rebase remote changes (even after stashing). Manual intervention required."
      # try to restore stash if present and then exit
      if git stash list | grep -q "$STASH_NAME"; then
        echo "Attempting to pop stash to restore local changes..."
        git stash pop || echo "Could not pop stash automatically; run 'git stash list' and pop manually."
      fi
      exit 1
    fi
  fi

  # if reached last attempt, fail
  if [ "$i" -eq "$MAX_ATTEMPTS" ]; then
    echo "❌ Failed to push after $MAX_ATTEMPTS attempts"
    exit 1
  fi
done

# safety fallback
echo "❌ Reached end of script unexpectedly"
exit 1