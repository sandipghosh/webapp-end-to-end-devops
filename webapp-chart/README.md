# Important notes
- >All the followign command should be executed form the root directory where the chart directory exists
- >In the current context the working directory is "webapp-devops"

## linting the entire helm chart
```helm lint ./webapp-chart```

## Add a host entry for webapp.local
```echo "127.0.0.1 webapp.local" | sudo tee -a /etc/hosts```

## Use kubectl port-forward (if needed):
```sudo kubectl port-forward --namespace <namespace-name> <service-name> <forwarding-port>:<hostport>```

```sudo kubectl port-forward --namespace webapp service/webapp-backend-service 8085:8080```

## Rendering all the template of the entire helm chart (Used for debug purposes)
```helm template <release-name> <chart-root-directory> -f <full path of values file> --namespace <namespace-name> --create-namespace```

```helm install <release-name> <chart-root-directory> -f <full path of values file> --namespace <namespace-name> --create-namespace```

```helm template webapp-chart ./webapp-chart -f ./webapp-chart/values.dev.yaml --dry-run --debug```

```helm install webapp-chart ./webapp-chart -f ./webapp-chart/values.dev.yaml --dry-run --debug```

## Rendering all the template of the entire helm chart and export to an output file "rendered-output.yaml"
```helm template webapp-chart ./webapp-chart -f ./webapp-chart/values.dev.yaml --debug > rendered-output.yaml```

## Installing the helm chart 
```helm install webapp-chart ./webapp-chart -f ./webapp-chart/values.dev.yaml -n webapp --create-namespace```

## Installing the helm chart with updraded configureation
```helm upgrade --install webapp-chart ./webapp-chart -f ./webapp-chart/values.dev.yaml -n webapp```


## Spinup testing pod
```kubectl run -i --tty test-connectivity -n dev-env --rm --image=busybox:latest --restart=Never -- /bin/sh```

```kubectl run -i --tty test-connectivity -n webapp --rm --image=ubuntu:latest --restart=Never -- /bin/bash```

## Default DNS of a service
```<service-name>.<namespace>.svc.cluster.local```


kubectl create configmap mysql-init-script \
  --from-file=init.sql=./init.sql \
  --namespace=webapp \
  --dry-run=client -o yaml > database-init-configmap.yaml


# Setup and configure nginx imgress controller in Kind cluster
1. The kind cluser configuration file should have "extraPortMappings" section maps localhost ports 80 and 443 to Kind node ports 80 and 443, so no need for port forwarding later

2. Install NGINX Ingress Controller via manifest (or Helm)

    ```kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.10.0/deploy/static/provider/kind/deploy.yaml```

3. Verify Service type

    ```kubectl get svc -n ingress-nginx```

4. **Why Port Forwarding (or Port Mapping) is Needed in Kind**
  - Kind runs Kubernetes inside Docker containers on your local machine.
  - These containers don’t expose ports like cloud LoadBalancer services.
  - The cluster’s NodePort services expose ports on the container, but those are not automatically exposed to your host machine.
  - Without mapping or forwarding those ports, you cannot access the ingress controller from your host (e.g., browser or curl).

    ```kubectl port-forward -n ingress-nginx svc/ingress-nginx-controller 8080:80```

# Step-by-Step: Install Metrics Server on Kind
1. Apply the official Metrics Server release from GitHub:
  
    ```kubectl apply -f https://github.com/kubernetes-sigs/metrics-server/releases/latest/download/components.yaml```

2. Modify Arguments for Kind Compatibility
  
    ```kubectl edit deployment metrics-server -n kube-system```

    ```yaml
    containers:
    - name: metrics-server
      args:
        - --kubelet-insecure-tls
        - --kubelet-preferred-address-types=InternalIP,Hostname,InternalDNS,ExternalDNS,ExternalIP

