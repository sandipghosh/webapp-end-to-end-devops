#!/bin/bash

CHANGED_SERVICES="${1:-}"
if [ -z "$CHANGED_SERVICES" ]; then
  echo "Changed services value not set."
  exit 2
fi

HAS_CHANGES="${2:-}"
if [ -z "$HAS_CHANGES" ]; then
  echo "Has changes value not set."
  exit 2
fi

echo "## 🚀 Deployment Summary" >> $GITHUB_STEP_SUMMARY
echo "**Commit:** ${GITHUB_SHA}" >> $GITHUB_STEP_SUMMARY
echo "**Branch:** ${GITHUB_REF_NAME}" >> $GITHUB_STEP_SUMMARY
echo "**Triggered by:** ${GITHUB_ACTOR}" >> $GITHUB_STEP_SUMMARY
echo "" >> $GITHUB_STEP_SUMMARY

if [[ "${HAS_CHANGES}" == "true" ]]; then
    echo "**Changed Services:** ${CHANGED_SERVICES}" >> $GITHUB_STEP_SUMMARY
    echo "✅ **Status:** Deployment attempted" >> $GITHUB_STEP_SUMMARY
else
    echo "ℹ️ **Status:** No services changed - no deployment needed" >> $GITHUB_STEP_SUMMARY
fi
