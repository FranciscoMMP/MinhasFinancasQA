# Automação de Testes - Minhas Finanças

Este repositório contém a suíte de testes desenvolvida para validar as regras de negócio do sistema de controle financeiro. A abordagem utilizada preserva o código original intacto, garantindo a qualidade através de testes unitários, de integração e end-to-end.

## Estrutura do Projeto

*   **Testes Unitários**: Isolam lógicas e validações de componentes do front-end (`/Frontend.Tests/tests/unit`). Framework utilizado: Vitest.
*   **Testes de Integração**: Realizam chamadas autônomas aos endpoints do Backend (`/Backend.Tests/IntegrationTests`). Verificam o processamento das transações, restrições e consolidação de saldos utilizando C# (RestSharp e FluentAssertions).
*   **Testes E2E (End-to-End)**: Validam a comunicação e o fluxo completo do ponto de vista do usuário (`/Frontend.Tests/e2e`). Validam componentes de UI navegando localmente na porta `:5173`. Framework: Playwright.

## Bugs / Erros Identificados

Na execução dos testes automatizados, mapeamos as seguintes inconsistências e falhas nas regras de negócio (mais detalhes podem ser acessados nos cards correspondentes):

1.  [**Bug 01 - Violação de exclusão em cascata (Foreign Key / Orfandade)**](./bugs/bug-01-cascata.md) 
2.  [**Bug 02 - Status 500 no endpoint de relatório (Totais)**](./bugs/bug-02-totais-erro-500.md) 
3.  [**Bug 03 - Retorno Status HTTP 500 para bloqueio de modalidade de categoria**](./bugs/bug-03-categoria-finalidade-erro-500.md)

## Como executar o projeto

Certifique-se de que a aplicação alvo está iniciada (`docker-compose up -d`).

### Executando C# (Backend)
```bash
cd Backend.Tests
dotnet restore
dotnet test
```

### Executando TS (Frontend E2E)
```bash
cd Frontend.Tests
npm install
npx playwright test
```

### Executando TS (Frontend Únitário)
```bash
cd Frontend.Tests
npm run test:unit
```
