# WebApiApp
Baseado no curso MinimalApi.
Uma API Web moderna desenvolvida em **.NET 8** utilizando os princípios da Clean Architecture (Arquitetura Limpa), com mapeamentos via **AutoMapper 15**, testes unitários com **xUnit/Moq** e persistência no **SQL Server**.

## 📐 Diagrama de Arquitetura

![Diagrama de Classes](arquitetura.puml)

## 🚀 Tecnologias Utilizadas

* **Runtime:** .NET 8.0 SDK
* **Banco de Dados:** SQL Server
* **ORM:** Entity Framework Core 8
* **Mapeamento:** AutoMapper 15.x
* **Testes:** xUnit, Moq & FluentAssertions

## 📂 Estrutura do Projeto
O projeto é dividido em camadas isoladas para garantir testabilidade e baixo acoplamento:

* **`WebApiApp.Domain`**: Contém as entidades de negócio (`Produto`, `Categoria`), a classe base `Entity` e as interfaces dos repositórios.
* **`WebApiApp.Application`**: Contém os serviços de aplicação, DTOs, interfaces de serviço e perfis de mapeamento do AutoMapper (`DomainToDTOMappingProfile`).
* **`WebApiApp.Infrastructure`**: Implementação do `DbContext`, configurações do Fluent API, Migrations e Repositories de dados.
* **`WebApiApp.CrossCutting`**: Camada responsável pela inversão de controle (IoC) e registro de dependências do sistema.
* **`WebApiApp.Api`**: Pontos de entrada da aplicação (Controllers), arquivos de configuração (`appsettings.json`) e inicialização (`Program.cs`).

## 🛠️ Como Executar o Projeto

### Pré-requisitos
* [.NET 8 SDK](https://microsoft.com) instalado.
* Instância do LocalDB ou SQL Server ativa.

### 1. Configurar a String de Conexão
Abra o arquivo `src/WebApiApp.Api/appsettings.json` e ajuste as credenciais do seu banco de dados:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=SEU_SERVIDOR;Database=DatabaseName;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 2. Executar as Migrations
Abra o terminal na raiz do projeto e aplique as atualizações do banco de dados:

```bash
dotnet ef database update --project WebApiApp.Infrastructure --startup-project WebApiApp.Api
```

### 3. Rodar a Aplicação
Navegue até a pasta da API e inicie o servidor:

```bash
cd WebApiApp.Api
dotnet run
```
Acesse o Swagger gerado no navegador através da porta indicada no terminal (ex: `http://localhost:5000/swagger`).

## 🧪 Executando os Testes Unitários

A aplicação conta com uma suite de testes de unidade utilizando xUnit, Moq e FluentAssertions. O ambiente simula as configuracoes do AutoMapper 15 de maneira isolada.

Para rodar todos os testes automatizados da solucao, abra o terminal na raiz do projeto e execute:

```bash
dotnet test
```

### Estrutura de Testes Implementada:
* Testes de Servico: Validam os fluxos de listagem, busca por ID e exclusao de registros garantindo que as chamadas aos repositorios ocorram conforme o esperado.
* Testes de Controladores: Validam as respostas HTTP (200 OK, 404 Not Found) retornadas pelos endpoints da API com base no comportamento simulado da camada de aplicacao.
