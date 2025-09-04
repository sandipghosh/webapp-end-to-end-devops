## CI pipeline structure 

./.github
└── workflows
    ├── build.yaml
    ├── deploy.yaml
    ├── detect-changes.yaml
    ├── main.yaml
    ├── README.md
    └── summary.yaml
./infra-ci-scripts
├── application-change-detection.sh
├── build-and-push-image.sh
├── commit-helm-image-changes.sh
├── README.md
└── update-helm-values.sh