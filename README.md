## 🧰 Comandos úteis

| Ação | Comando |
|------|----------|
| Subir containers | `docker-compose -f docker/docker-compose.yml up -d` |
| Parar containers | `docker-compose -f docker/docker-compose.yml down` |
| Ver logs | `docker-compose -f docker/docker-compose.yml logs -f` |
| Acessar banco via terminal | `docker exec -it postgres_main psql -U postgres -d service-order` |