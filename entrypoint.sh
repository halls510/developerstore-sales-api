#!/bin/sh
set -e

echo "Criando o banco de dados: $POSTGRES_DB"

# Aguarda até 2 minutos pelo PostgreSQL
for i in {1..60}; do
    if pg_isready -h ambev.developerevaluation.database -p 5432 -U "$POSTGRES_USER"; then
        echo "PostgreSQL está pronto!"
        break
    fi
    echo "Aguardando o PostgreSQL iniciar... Tentativa $i"
    sleep 2
done

# Tenta conexão novamente após aguardar
psql -v ON_ERROR_STOP=1 --host=ambev.developerevaluation.database --username="$POSTGRES_USER" <<-EOSQL
    SELECT 'Banco de dados já existe' FROM pg_database WHERE datname = '$POSTGRES_DB';
    \gexec
    CREATE DATABASE $POSTGRES_DB;
EOSQL

echo "Banco de dados $POSTGRES_DB criado com sucesso!"
