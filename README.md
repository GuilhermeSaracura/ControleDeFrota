# Controle de Frota

API interna em ASP.NET Core para gestão da frota de veículos e dos usuários vinculados à empresa. O projeto foi estruturado para manter a lógica simples, sem dependência de banco de dados externo ou ORM, utilizando armazenamento em memória para representar o cenário de uso interno da organização.

## Linguagem e versões

- C#
- ASP.NET Core Web API
- .NET 10.0
- xUnit para testes unitários
- FluentAssertions para asserções mais legíveis

## Estrutura do projeto

```text
ControleDeFrota/
├── .gitignore
├── README.md
├── ControleDeFrota.sln
├── src/
│   ├── ControleDeFrota.Api/
│   │   ├── Controllers/
│   │   ├── Dtos/
│   │   ├── Models/
│   │   ├── Repositories/
│   │   ├── Utils/
│   │   ├── Program.cs
│   │   └── ControleDeFrota.Api.csproj
│   └── ControleDeFrota.Tests/
│       ├── RegraFrotaUtilsTests.cs
│       ├── InMemoryUsuarioRepositoryTests.cs
│       ├── InMemoryVeiculoRepositoryTests.cs
│       └── ControleDeFrota.Tests.csproj
└── .vs/
```

## Pré-requisitos

Antes de executar o projeto, certifique-se de que os itens abaixo estejam instalados:

- .NET SDK 10.0 ou superior
- Git
- Editor ou IDE como Visual Studio 2022 ou VS Code

## Compilação

Na raiz do repositório, execute:

```bash
dotnet restore
```

```bash
dotnet build ControleDeFrota.sln
```

## Execução da API

Para iniciar a API localmente:

```bash
dotnet run --project src/ControleDeFrota.Api
```

A API fica disponível em:

- http://localhost:5000
- http://localhost:5062

Obs.: a porta pode variar conforme configuração local do ambiente.

## Acesso ao Swagger

A API já está configurada para expor a documentação OpenAPI/Swagger.

Após iniciar a aplicação, acesse no navegador:

```text
http://localhost:5062/swagger
```

Ou, se estiver usando HTTP local:

```text
http://localhost:5000/swagger
```

## Endpoints

### Usuarios

- GET /api/usuarios
- GET /api/usuarios/{id}
- POST /api/usuarios
- PUT /api/usuarios/{id}
- DELETE /api/usuarios/{id}

### Veiculos

- GET /api/veiculos
- GET /api/veiculos/{id}
- POST /api/veiculos
- PUT /api/veiculos/{id}
- DELETE /api/veiculos/{id}

## Exemplos de payload

### 1) Criar usuário

Endpoint:

```http
POST /api/usuarios
Content-Type: application/json
```

Exemplo de body:

```json
{
 "nome": "Maria Silva",
 "email": "maria@empresa.com",
 "departamento": "Logística",
 "ativo": true,
 "tipoContratacao": 0,
 "numeroHabilitacao": "12345678901",
 "categoriasHabilitacao": 1,
 "veiculoId": null
}
```

### 2) Atualizar usuário

Endpoint:

```http
PUT /api/usuarios/{id}
Content-Type: application/json
```

Exemplo:

```json
{
 "nome": "Maria Silva",
 "email": "maria@empresa.com",
 "departamento": "Logística",
 "ativo": true,
 "tipoContratacao": 0,
 "numeroHabilitacao": "12345678901",
 "categoriasHabilitacao": 1,
 "veiculoId": "d5d0fe28-3f61-4b9a-b5ee-19f67cbf6ed3"
}
```

### 3) Criar veículo

Endpoint:

```http
POST /api/veiculos
Content-Type: application/json
```

Exemplo de body:

```json
{
 "placa": "ABC-1234",
 "modelo": "Fiat Uno",
 "ano": 2024,
 "cor": "Branco",
 "disponivel": true,
 "categoria": 1
}
```

### 4) Atualizar veículo

Endpoint:

```http
PUT /api/veiculos/{id}
Content-Type: application/json
```

Exemplo:

```json
{
 "placa": "ABC-1234",
 "modelo": "Fiat Uno",
 "ano": 2024,
 "cor": "Branco",
 "disponivel": true,
 "categoria": 1,
 "usuarioId": "e7fe9d44-fb19-4d65-9e7f-672e4c7d6b22"
}
```

## Tipos de contratação

O enum `TipoContratacao` contém:

```csharp
public enum TipoContratacao
{
   Funcionario = 0,
   Estagiario = 1,
   Aprendiz = 2
}
```

## Categorias de habilitação

O enum `CategoriaHabilitacao` usa flags para representar combinações:

```csharp
[Flags]
public enum CategoriaHabilitacao
{
   Nenhuma = 0,
   B = 1,
   C = 2,
   BEC = B | C
}
```

## Regras de negócio aplicadas

- Funcionários precisam ter habilitação e categoria compatível com o veículo.
- Usuários sem habilitação não podem ficar vinculados a veículo.
- Estagiários e aprendizes não podem conduzir veículos nem ter veículo vinculado.
- O vínculo entre usuário e veículo é validado pelo utilitário `RegraFrotaUtils`.
- O relacionamento também é sincronizado em operações de atualização de usuário e veículo.

## Executando os testes

Na raiz do projeto, execute:

```bash
dotnet test src/ControleDeFrota.Tests/ControleDeFrota.Tests.csproj
```

Para uma execução com saída mais detalhada:

```bash
dotnet test src/ControleDeFrota.Tests/ControleDeFrota.Tests.csproj --nologo -v minimal
```

## Observações

- A persistência é em memória e não depende de banco de dados externo.
- A API foi criada como base para evolução interna do sistema de frota.
- Os testes cobrem regras de negócio e a camada de repositórios em memória.
