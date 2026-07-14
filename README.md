# Tech Challenge – Leandro Cordeiro e Rafael Bastos Amaral

## Sobre o Projeto
Este repositório contém a entrega da **Fase 2** do Tech Challenge da pós-graduação em Arquitetura de Software.

O projeto foi desenvolvido em **.NET 10**, com foco em boas práticas de arquitetura, domínio de negócio e operação em ambiente conteinerizado, mantendo uma abordagem orientada a portfólio profissional.

## Tecnologias
- .NET 10
- Arquitetura Hexagonal
- DDD
- EF Core
- PostgreSQL 16
- JWT
- Docker
- Docker Compose
- Kubernetes
- Terraform
- GitHub Actions
- xUnit

## Arquitetura
- Arquitetura Hexagonal
- Services representam os casos de uso
- Sem CQRS
- Aggregate Roots:
  - Cliente
  - OrdemServico
  - Estoque

## Funcionalidades implementadas
- Cadastro de clientes
- Cadastro de veículos
- Cadastro de estoque
- Abertura de Ordem de Serviço
- Consulta de Ordem de Serviço
- Listagem de Ordens de Serviço
- Aprovação/Reprovação de orçamento via Webhook
- Envio de e-mail para aprovação de orçamento
- Alteração automática do status da OS após aprovação/reprovação
- Autenticação JWT

## Arquitetura da Solução
```text
Cliente
  ↓
API REST
  ↓
Application
  ↓
Domain
  ↓
Infrastructure
  ↓
PostgreSQL
```

## Estrutura do Projeto
```text
src/
├── tech_challenge.API
├── tech_challenge.Application
├── tech_challenge.Domain
├── tech_challenge.Infrastructure
└── tech_challenge.Teste
```

Descrição das camadas da Arquitetura Hexagonal:

- **API**: camada de entrada HTTP (controllers, requests/responses, autenticação, middlewares).
- **Application**: casos de uso da aplicação (services), orquestração das regras e contratos (interfaces).
- **Domain**: núcleo do negócio, entidades, agregados, enums e validações de domínio.
- **Infrastructure**: persistência com EF Core/PostgreSQL, implementações de repositórios e integrações externas.
- **Teste**: testes automatizados unitários e de integração.

## Infraestrutura
O projeto possui estrutura completa para execução e operação:

- **Docker**: empacotamento da aplicação.
- **Docker Compose**: orquestração local da API e banco PostgreSQL.
- **Kubernetes**: manifests de Deployments, Services, ConfigMap, Secret e HPA.
- **Terraform**: provisionamento de infraestrutura.
- **GitHub Actions**: automação de CI/CD.

## CI/CD
Fluxo resumido da pipeline:

```text
GitHub
  ↓
GitHub Actions
  ↓
Build
  ↓
Testes
  ↓
Docker Build
  ↓
Docker Push
  ↓
Deploy Kubernetes
```

A pipeline executa:
- Restore
- Build
- Testes automatizados
- Build da imagem Docker
- Push da imagem
- Deploy dos manifestos Kubernetes

## Escalabilidade
A API utiliza **Horizontal Pod Autoscaler (HPA)** configurado para escalar automaticamente entre **2 e 5 réplicas**, conforme utilização de CPU.

## Execução Local

### 1. Clonar o projeto
```bash
git clone https://github.com/rafaaamaral/tech_challenge_fase1
cd tech_challenge_fase1
```

### 2. Restaurar dependências
```bash
dotnet restore
```

### 3. Executar via Docker Compose
```bash
docker compose -f docker/docker-compose.yml up -d --build
```

### 4. Acessar Swagger
Com a aplicação em execução, acesse:

- `http://localhost:8080/swagger`

## Execução no Kubernetes

### 1. Aplicar manifestos
```bash
kubectl apply -f k8s/namespace.yaml
kubectl apply -f k8s/postgres.yaml
kubectl apply -f k8s/api.yaml
```

### 2. Verificar Pods
```bash
kubectl get pods -n tech-challenge
```

### 3. Verificar Services
```bash
kubectl get svc -n tech-challenge
```

### 4. Verificar HPA
```bash
kubectl get hpa -n tech-challenge
```

### 5. Acessar a aplicação
Exponha o service conforme sua estratégia de cluster (LoadBalancer/NodePort/Ingress) e acesse o endpoint público da API.

## Banco de Dados

**PostgreSQL 16**

| Configuração | Valor |
|---|---|
| Host | `localhost` |
| Porta | `5432` |
| Banco | `tech-challenge` |
| Usuário | `postgres` |
| Senha | `postgres` |

Exemplo de connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=tech-challenge;Username=postgres;Password=postgres"
  }
}
```

## Testes
O projeto possui testes automatizados com **xUnit**, cobrindo:

- **Testes unitários**: validação de regras de domínio e casos de uso da aplicação.
- **Testes de integração**: validação ponta a ponta da API (HTTP → Controller → Service → Repository → Banco), incluindo fluxos críticos de autenticação e Ordem de Serviço.

Para executar os testes:

```bash
dotnet test
```

## Migrations e Banco

Criar migration (na raiz do projeto):

```bash
dotnet ef migrations add NomeDaMigration -p src/tech_challenge.Infrastructure -s src/tech_challenge.API
```

Atualizar banco de dados:

```bash
dotnet ef database update -p src/tech_challenge.Infrastructure -s src/tech_challenge.API
```

Observações:
- Utilize sempre **Infrastructure** como projeto de persistência.
- Utilize sempre **API** como startup project.

## Comandos úteis

### Docker
| Ação | Comando |
|---|---|
| Subir containers | `docker compose -f docker/docker-compose.yml up -d --build` |
| Parar containers | `docker compose -f docker/docker-compose.yml down` |
| Ver logs | `docker compose -f docker/docker-compose.yml logs -f` |
| Acessar banco via terminal | `docker exec -it postgres_db psql -U postgres -d tech-challenge` |

### Terraform
```bash
cd terraform
terraform init
terraform plan
terraform apply
```

> Ajuste o caminho do kubeconfig em `terraform/main.tf` conforme o seu cluster.
