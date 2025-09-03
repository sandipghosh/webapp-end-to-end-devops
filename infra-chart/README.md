# Important notes
- >All the followign command should be executed form the root directory where the chart directory exists
- >In the current context the working directory is "webapp-devops"

## linting the entire helm chart
```helm lint ./infra-chart```

## Add a host entry for webapp.local
```echo "127.0.0.1 webapp.local" | sudo tee -a /etc/hosts```

## Use kubectl port-forward (if needed):
```sudo kubectl port-forward --namespace <namespace-name> <service-name> <forwarding-port>:<hostport>```

```sudo kubectl port-forward --namespace webapp service/webapp-backend-service 8085:8080```

## Rendering all the template of the entire helm chart (Used for debug purposes)
```helm template <release-name> <chart-root-directory> -f <full path of values file> --namespace <namespace-name> --create-namespace```

```helm install <release-name> <chart-root-directory> -f <full path of values file> --namespace <namespace-name> --create-namespace```

```helm template webapp-chart ./infra-chart -f ./infra-chart/values.yaml --dry-run --debug```

```helm install webapp-chart ./infra-chart -f ./infra-chart/values.yaml --dry-run --debug```

## Rendering all the template of the entire helm chart and export to an output file "rendered-output.yaml"
```helm template webapp-chart ./infra-chart -f ./infra-chart/values.dev.yaml --debug > rendered-output.yaml```

## Installing the helm chart 
```helm install webapp-chart ./infra-chart -f ./infra-chart/values.dev.yaml -n webapp --create-namespace```

## Installing the helm chart with updraded configureation
```helm upgrade --install webapp-chart ./infra-chart -f ./infra-chart/values.dev.yaml -n webapp```


## Spinup testing pod
```kubectl run -i --tty test-connectivity -n dev-env --rm --image=busybox:latest --restart=Never -- /bin/sh```

```kubectl run -i --tty test-connectivity -n webapp --rm --image=ubuntu:latest --restart=Never -- /bin/bash```

## Default DNS of a service
```<service-name>.<namespace>.svc.cluster.local```


## **Why Port Forwarding (or Port Mapping) is Needed in Kind**
  - Kind runs Kubernetes inside Docker containers on your local machine.
  - These containers don’t expose ports like cloud LoadBalancer services.
  - The cluster’s NodePort services expose ports on the container, but those are not automatically exposed to your host machine.
  - Without mapping or forwarding those ports, you cannot access the ingress controller from your host (e.g., browser or curl).

```kubectl port-forward -n ingress-nginx svc/ingress-nginx-controller 8585:80```

