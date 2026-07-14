# GitHub Actions

## Secrets necessários

Configure os seguintes secrets no repositório do GitHub:

- DOCKERHUB_USERNAME
- DOCKERHUB_TOKEN
- KUBE_CONFIG

## Fluxo

1. Push ou pull request para main/master/develop
2. CI executa restore, build e testes
3. Em push para main/master, a imagem Docker é publicada
4. Em push para main/master, os manifests do Kubernetes são aplicados
