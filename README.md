# Tech Challenge – Leandro Cordeiro e Rafael Bastos Amaral

## 📖 Sobre o Projeto
Este é o MVP da Tech Challenge Fase 1


## 🚀 Subindo o ambiente
> **Pré-requisitos:**
> - Docker instalado → [https://docs.docker.com/get-docker/](https://docs.docker.com/get-docker/)
> - Docker Compose (já incluso no Docker Desktop)

### 1. Clone o repositório
```bash
git clone https://github.com/rafaaamaral/tech_challenge_fase1
cd tech_challenge_fase1
```
### 2. Suba os containers

```bash
docker-compose -f docker/docker-compose.yml up -d
```
### 3. Verifique se o banco está rodando

```bash
docker ps
```

Você deve ver algo como:

```
CONTAINER ID   IMAGE          STATUS          PORTS
123abc456def   postgres:16    Up 5 minutes    0.0.0.0:5432->5432/tcp
```

---

## 🗄️ Banco de Dados

**PostgreSQL 16**

| Configuração | Valor |
|---------------|--------|
| Host | `localhost` |
| Porta | `5432` |
| Banco | `tech-challenge` |
| Usuário | `postgres` |
| Senha | `postgres` |

### Conexão no .NET
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=service-order;Username=postgres;Password=postgres"
  }
}
```

## 📁 Estrutura do Projeto
```
src/
├── tech_challenge.API → Projeto de inicialização (Startup)
├── tech_challenge.Infrastructure → Contém o DbContext e configurações de persistência
├── tech_challenge.Application/ # Casos de uso, DTOs, handlers
├── tech_challenge.Domain → Contém as entidades de domínio
```

## 🛠️ Requisitos
- .NET 10 ou superior
- Ferramentas do EF instaladas:
```
dotnet tool install --global dotnet-ef --version (versao do EF)
```

## 🚀 Criando uma nova migration
Execute no diretório raiz do projeto:
```
dotnet ef migrations add NomeDaMigration -p tech_challenge.Infrastructure -s challenge.API
```
## 🛠️ Atualizando o banco de dados
```
dotnet ef database update -p tech_challenge.Infrastructure -s tech_challenge.API
```

## Observações
- A migration deve sempre ser criada apontando **Infrastructure como projeto de persistência** e **API como startup**.
- Em caso de erro de caminho, confirme se está dentro da pasta correta ao executar o comando.

## 📦 Restaurar dependências
```
dotnet restore
```
## 🧪 Compilar a solução
```
dotnet build
````
## 🏃 Rodar o projeto
```
dotnet run --project src/tech_challenge.API
```

## 🧰 Comandos úteis

## 🐳 Docker
Estar na raiz do repositório

| Ação | Comando |
|------|----------|
| Subir containers | `docker-compose -f docker/docker-compose.yml up -d` |
| Parar containers | `docker-compose -f docker/docker-compose.yml down` |
| Ver logs | `docker-compose -f docker/docker-compose.yml logs -f` |
| Acessar banco via terminal | `docker exec -it postgres_main psql -U postgres -d tech-challenge` |