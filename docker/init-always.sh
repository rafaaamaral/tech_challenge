#!/bin/bash
set -e

echo "⏳ Executando script init.sql no PostgreSQL existente..."

# Aguarda o banco estar pronto
until pg_isready -U postgres > /dev/null 2>&1; do
  sleep 1
done

# Executa o script SQL (ignora erros caso o banco já exista)
if [ -f /init.sql ]; then
  psql -U postgres -f /init.sql || true
  echo "✅ Script init.sql executado com sucesso!"
else
  echo "⚠️ Nenhum init.sql encontrado."
fi

wait
