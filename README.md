# 🧪 PROJETO DEV – Developer Evaluation (Full Stack)

Este projeto foi desenvolvido com o objetivo de avaliar competências práticas de um desenvolvedor full stack, simulando um ambiente real com backend em .NET 8, frontend Angular, mensageria com RabbitMQ, armazenamento com MinIO e banco de dados relacional (PostgreSQL).

O projeto está preparado para rodar localmente via Docker com configuração facilitada, testes automatizados e documentação de apoio.

---

## 📚 Sumário

1. [🎯 Finalidade do Projeto](#-finalidade-do-projeto)
2. [⚙️ Pré-requisitos](#️-pré-requisitos)
3. [🔐 Configurações Necessárias](#-configurações-necessárias)
4. [🚀 Execução Rápida](#-execução-rápida)
5. [🌐 Endpoints e Serviços](#-endpoints-e-serviços)
6. [🔑 Credenciais de Acesso](#-credenciais-de-acesso)
7. [🧪 Testes Automatizados](#-testes-automatizados)
8. [🗂️ Estrutura do Projeto](#️-estrutura-do-projeto)
9. [🚧 Troubleshooting](#-troubleshooting)
10. [🔐 Acesso e Credenciais de Acesso (Ambiente de Demonstração)](#-acesso-e-credenciais-de-acesso-ambiente-de-demonstracao)
---

## 🎯 Finalidade do Projeto

Este projeto demonstra:

- Uso de arquitetura **DDD com .NET 8**
- API RESTful com autenticação JWT
- Frontend Angular integrado via HTTPS
- Execução local via Docker Compose
- RabbitMQ para mensageria e filas
- MinIO para armazenamento de objetos
- Testes automatizados (unitários, integração e funcionais)

---

## ⚙️ Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [Node.js (v18+)](https://nodejs.org/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Postman](https://www.postman.com/)
- [Cypress](https://www.cypress.io/)

---

## 🔐 Geração do Certificado HTTPS

A Web API está configurada para rodar via HTTPS por padrão. Por isso, é necessário gerar um certificado de desenvolvimento antes de subir os containers.

### ▶️ Windows (PowerShell)

```bash
dotnet dev-certs https -ep $env:APPDATA/ASP.NET/Https/Ambev.DeveloperEvaluation.WebApi.pfx -p "@mb3vD3v3l0p3r"
```

### ▶️ Linux/macOS/WSL

```bash
dotnet dev-certs https -ep ~/.aspnet/https/Ambev.DeveloperEvaluation.WebApi.pfx -p "@mb3vD3v3l0p3r"
```

> ⚠️ Certifique-se de que o caminho e senha estão iguais ao configurado em `appsettings.Development.json`.

---

## 🔐 Configurações Necessárias

### ✅ .env

Crie o arquivo `.env` na raiz do projeto com o seguinte conteúdo:

```env
POSTGRES_DB=developer_evaluation
POSTGRES_USER=developer
POSTGRES_PASSWORD=ev@luAt10n

RABBITMQ_USER=developer
RABBITMQ_PASS=ev@luAt10n

MINIO_ROOT_USER=minio
MINIO_ROOT_PASSWORD=minio123

USER_SECRETS_PATH=${APPDATA}/Microsoft/UserSecrets
HTTPS_CERT_PATH=${APPDATA}/ASP.NET/Https
```

> ⚠️ O `.env` é essencial para funcionamento local com Docker.

---

### ✅ appsettings.Development.json

Este arquivo já está incluso no projeto com as configurações completas para desenvolvimento. **Não deve ser usado em produção.**

Certifique-se de que contém:

```json
"Kestrel": {
  "Certificates": {
    "Default": {
      "Path": "/https/Ambev.DeveloperEvaluation.WebApi.pfx",
      "Password": "@mb3vD3v3l0p3r"
    }
  }
},
"MinioSettings": {
  "ApiEndpoint": "ambev.developerevaluation.minio:9000",
  "PublicUrl": "localhost:9000"
}
```
---

## 🚀 Execução Rápida

```bash
git clone https://github.com/halls510/developerstore-sales-api.git
cd developerstore-sales-api

# Gere o certificado HTTPS (obrigatório para a API):
dotnet dev-certs https -ep $env:APPDATA/ASP.NET/Https/Ambev.DeveloperEvaluation.WebApi.pfx -p "@mb3vD3v3l0p3r"

# Suba os containers:
docker-compose -p developerstore up -d
```

---

## 🌐 Endpoints e Serviços

| Serviço            | URL                                                        |
|--------------------|------------------------------------------------------------|
| Web API (HTTP)     | http://localhost:8080/api                                   |
| Web API (HTTPS)    | https://localhost:8081/api                                  |
| Swagger UI         | http://localhost:8080/swagger / https://localhost:8081/swagger |
| Frontend Angular   | http://localhost:4200                                       |
| RabbitMQ Console   | http://localhost:15672                                      |
| MinIO Console      | http://localhost:9001                                       |

---

## 🔑 Credenciais de Acesso

### Web API / Frontend

- **Email:** `admin@example.com`
- **Senha:** `A#g7jfdsd#$%#`

### RabbitMQ (Interface Gráfica)

- **Usuário:** `developer`
- **Senha:** `ev@luAt10n`

### MinIO (Console Gráfico)

- **Usuário:** `minio`
- **Senha:** `minio123`

---

## 🧪 Testes Automatizados

### .NET

```bash
cd tests/Ambev.DeveloperEvaluation.Functional
dotnet test

cd tests/Ambev.DeveloperEvaluation.Integration
dotnet test

cd tests/Ambev.DeveloperEvaluation.Unit
dotnet test
```

### Cypress

```bash
cd src/frontend
npm install
npx cypress open
```

### Postman

Importe a collection:
```
tests/Postman/DeveloperEvaluation.postman_collection.json
```

---

## 🗂️ Estrutura do Projeto

```
root/
├── src/
│   ├── Ambev.DeveloperEvaluation.WebApi/
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json
│   │   └── certs/
│   ├── ...
│   └── frontend/
├── tests/
│   ├── Functional/
│   ├── Integration/
│   ├── Unit/
│   ├── Cypress/
│   └── Postman/
├── .env
├── docker-compose.yml
└── README.md
```

---

## 🚧 Troubleshooting

| Problema                        | Solução                                                                 |
|---------------------------------|--------------------------------------------------------------------------|
| HTTPS não funciona              | Gere o certificado com `dotnet dev-certs https -ep ...`                 |
| Erro de SSL / API não sobe      | Caminho ou senha do certificado incorretos no appsettings               |
| Porta em uso                    | Altere no `docker-compose.override.yml` ou finalize serviços conflitantes |
| Serviços não conectam           | Verifique `.env` e credenciais (RabbitMQ, MinIO)                        |

## 🔐 Acesso e Credenciais de Acesso (Ambiente de Demonstração)

- **RabbitMQ**  
  - URL: https://rabbitmq.hallison.com.br/  
  - Usuário: `devstore`  
  - Senha: `devstore123`

- **MinIO**  
  - URL: https://minio-ui.hallison.com.br/  
  - Usuário: `minioadminstore`  
  - Senha: `minioadminstore`

- **Frontend**  
  - URL: https://store.hallison.com.br/  
  - Usuário de teste: `manager@devstore.com`  
  - Senha: `Manager123!`

- **WebApi**  
  - URL: https://api.hallison.com.br/api  
  - Email: `admin@example.com`  
  - Senha: `A#g7jfdsd#$%#`

- **WebApi Swagger**  
  - URL: https://api.hallison.com.br/swagger  
  - Email: `admin@example.com`  
  - Senha: `A#g7jfdsd#$%#`  

> *Obs: Estes acessos são apenas para fins de avaliação do projeto.*