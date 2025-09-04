#!/bin/bash
SERVICES=("webapp-database" "webapp-backend" "webapp-frontend")
CHANGED_SERVICES=()

echo "Checking for changes in the services"

# SERVICES[@] expand all the array elements
for service in "${SERVICES[@]}"; do
    if git diff --name-only HEAD~1 HEAD | grep -Ei "^webapp/${service}/" || \
        [ "${GITHUB_EVENT_NAME}" == "workflow_dispatch" ]; then

        CHANGED_SERVICES+=("${service}")
        echo "Changes detected in: ${service}."
    else
        echo "NO changes detected in: ${service}."
    fi
done

# "#" in front of array variable returns the element count
if [ "${#CHANGED_SERVICES[@]}" -eq 0 ]; then
    echo "No service has changed."
    echo "has-changes=false" >> "$GITHUB_OUTPUT"
    exit 0  
fi

# Expected value would be ["webapp-backend","webapp-frontend"]
MATRIX_JSON="["
# "!" in front of array variable expand the list of indexes instead of element values
for index in "${!CHANGED_SERVICES[@]}"; do
    [[ $index -gt 0 ]] && MATRIX_JSON+=","
    MATRIX_JSON+="\"${CHANGED_SERVICES[$index]}\""
done
MATRIX_JSON+="]"

echo "has-changes=true" >> "$GITHUB_OUTPUT"

# Expected value to be written in the output would be "webapp-backend,webapp-frontend"
# CHANGED_SERVICES[*] flatten the all array elements in a text format
echo "changed-services=${CHANGED_SERVICES[*]}" >> "$GITHUB_OUTPUT"

# Expected value to be written in the output would be {services: ["webapp-backend","webapp-frontend"]}
echo "matrix={\"services\":${MATRIX_JSON}}" >> "$GITHUB_OUTPUT"

echo "Service to build: ${CHANGED_SERVICES[*]}" 
echo "Generated matrix: ${MATRIX_JSON}" 