terraform {
  required_providers {
    argocd = {
      source  = "argoproj-labs/argocd"
      version = "7.11.0"
    }
  }
}

provider "argocd" {
  server_addr = var.host_URL
  username    = var.username
  password    = var.password
}