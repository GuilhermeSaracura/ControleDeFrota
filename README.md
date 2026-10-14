# Controle de Frota

API interna para gestão da frota de veículos e usuários da empresa.

## Estrutura

- `src/ControleDeFrota.Api`: aplicação ASP.NET Core com endpoints REST.
- `src/ControleDeFrota.Tests`: testes unitários da camada de repositórios em memória.

## Tecnologias

- ASP.NET Core
- .NET 10
- Repositório em memória para persistência de dados em memória

## Como executar

```bash
dotnet restore
 dotnet build
 dotnet run --project src/ControleDeFrota.Api
```

## Endpoints principais

- `GET /api/veiculos`
- `GET /api/veiculos/{id}`
- `POST /api/veiculos`
- `PUT /api/veiculos/{id}`
- `DELETE /api/veiculos/{id}`
- `GET /api/usuarios`
- `GET /api/usuarios/{id}`
- `POST /api/usuarios`
- `PUT /api/usuarios/{id}`
- `DELETE /api/usuarios/{id}`

## Observações

Esta implementação inicial usa armazenamento em memória e foi criada como base para evoluções futuras, sem dependências de banco de dados externo ou ORM.
