# resource "argocd_project" "webapp-project" {

# }

resource "argocd_application" "webapp-application" {
  metadata {
    name      = "demo-application-terraform"
    namespace = "argocd"
  }
  spec {
    project = "default"
    source {
      repo_url        = "https://github.com/sandipghosh/webapp-end-to-end-devops.git"
      path            = "webapp-chart"
      target_revision = "main"
      helm {
        release_name = "testing"
        value_files  = ["custom-values.yaml"]
      }
    }
    destination {
      server    = "https://kubernetes.default.svc"
      namespace = "by-terraform"
    }
  }
}   