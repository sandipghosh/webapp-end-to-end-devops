#!/bin/bash

SERVICE="$1"
IMAGE_REPO="${ECR_REPO}"
IMAGE_TAG="$(echo "${GITHUB_SHA}" | cut -c1-7)"
VALUES_FILE="./infra-chart/values.yaml"

echo "Updating Helm values for the service: ${SERVICE}"
echo "Repository: ${IMAGE_REPO}"
echo "Tag: ${IMAGE_TAG}"
echo "Values file: ${VALUES_FILE}"

[[ ! -f "${VALUES_FILE}" || && echo "Values file not found: ${VALUES_FILE}" && exit 1 ]]

# Backup the helm values file before making any changes

awk -v repo="$IMAGE_REPO" -v tag="$IMAGE_TAG" -v target_host="$SERVICE" '
  # detect top-level section name like "database: or backend: or frontend:", capture section name
  /^[^[:space:]].*:[[:space:]]*$/ {
    # reset per-section state
    in_section = 1
    section = $1                      # $1 contains the name followed by colon
    sub(/:.*/,"",section)             # remove the colon and trailing text to get section name
    host_match = 0
    in_image = 0
    print
    next
  }

  # if we hit another top-level (non-empty non-space-starting), also handle
  /^[^[:space:]]/ && !/^[^[:space:]].*:[[:space:]]*$/ {
    # This is a top-level line that is not a simple "name:" line
    in_section = 0
    section = ""
    host_match = 0
    in_image = 0
    print
    next
  }

  # inside some top-level section: capture hostName
  in_section && /^[[:space:]]*hostName:[[:space:]]*/ {
    val = $0
    sub(/^[[:space:]]*hostName:[[:space:]]*/,"",val)   # remove leading key text
    gsub(/^[ \t]+|[ \t]+$/,"",val)                     # trim spaces
    # strip surrounding quotes if present
    if (val ~ /^".*"$/ || val ~ /^\x27.*\x27$/) {
      val = substr(val,2,length(val)-2)
    }
    if (val == target_host) {
      host_match = 1
    } else {
      host_match = 0
    }
    print; next
  }

  # enter image block under the current section
  in_section && /^[[:space:]]*image:[[:space:]]*$/ {
    in_image = 1
    print; next
  }

  # if we see another top-level key inside a section, leave image & section scope
  in_section && /^[^[:space:]]/ {
    in_image = 0
    in_section = 0
    host_match = 0
    section = ""
    print; next
  }

  # replace repository if we are in the image block and host matches
  in_image && host_match && /^[[:space:]]*repository:[[:space:]]*/ {
    # preserve indentation prefix up to the "repository"
    pos = index($0,"repository")
    if (pos == 0) {
      # fallback if index fails, just use two-space indentation
      prefix = substr($0,1,match($0,/[^[:space:]]/)-1)
    } else {
      prefix = substr($0,1,pos-1)
    }
    print prefix "repository: " repo
    next
  }

  # replace tag if we are in the image block and host matches
  in_image && host_match && /^[[:space:]]*tag:[[:space:]]*/ {
    pos = index($0,"tag")
    if (pos == 0) {
      prefix = substr($0,1,match($0,/[^[:space:]]/)-1)
    } else {
      prefix = substr($0,1,pos-1)
    }
    # write tag with quotes to match your sample; remove quotes if you prefer unquoted
    print prefix "tag: \"" tag "\""
    next
  }

  # default: print unchanged
  { print }
' ${VALUES_FILE} > ${VALUES_FILE}.backup

cat ${VALUES_FILE}.backup