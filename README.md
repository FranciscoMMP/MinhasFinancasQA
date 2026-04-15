# Automação de Testes - Minhas Finanças (QA)

Este repositório contém a suíte de testes desenvolvida para validar as regras de negócio do sistema de controle financeiro. A abordagem que adotei preserva o código original intacto, cumprindo todos os requisitos listados no desafio técnico.

---

## 1. Como você estruturou a pirâmide
A arquitetura foi implementada em suas três camadas fundamentais com pastas totalmente desacopladas do repositório principal do ecossistema:

* **Base (Testes Unitários):** Isolam as regras matemáticas e verificações de negócio puras (front-end) em `/Frontend.Tests/tests/unit`.
* **Meio (Testes de Integração):** Realizam chamadas de API autônomas diretamente no projeto C# em `/Backend.Tests/IntegrationTests`. Focam na persistência dos dados e cálculos computados do servidor.
* **Topo (Testes End-to-End):** Validam a comunicação e o fluxo completo do ponto de vista visual do navegador web em `/Frontend.Tests/e2e`.

## 2. Justificativa das escolhas de testes
Como o escopo do desafio determina as tecnologias exigidas e proíbe a alteração do código-fonte da aplicação, o meu plano de testes foi inteiramente guiado pela abordagem **Black-Box**. A alocação da cobertura de testes para cada camada teve as seguintes justificativas táticas:

* **Testes Unitários:** Optei por focar na validação estrita dos delimitadores lógicos (idade vs. receita e modalidade de categorias) de forma isolada no client-side. A justificativa é que validar a base condicional em milissegundos sem depender da rede blinda a aplicação logo na sua camada mais barata.
* **Testes de Integração:** A decisão metodológica central foi cobrir os cenários mais complexos do backend (Ex: Deleção em Cascata e Somatório dos Totais) nesta camada. Testei matemática financeira e integridade referencial batendo diretamente na API real (`localhost:5000`), evitando assim a fragilidade (flakiness) comum aos testes de navegador, atestando com precisão a eficácia do Banco de Dados e dos Controllers.
* **Testes E2E:** Foquei a automação estritamente nas regras *core* de jornada do usuário final. A minha decisão foi não tentar atingir 100% de cobertura estética para manter a leveza da pirâmide. Os testes modelam o fluxo real simulando restrições de negócio ativadas proativamente pela Interface antes da submissão.

---

## 3. Bugs encontrados (qual regra falhou)
A partir da execução crua da pirâmide de testes automáticos, investiguei e mapeei três falhas severas nas regras de negócio (amplamente documentadas nos cards da pasta `/bugs`):

1. **Regra de exclusão em cascata (Falha na Persistência):** 
   * [Bug 01 - Violação de exclusão em cascata (Orfandade de Transações)](./bugs/bug-01-cascata.md) 
2. **Regra de consultas de totais por pessoa (Falhou no Status de Retorno HTTP):** 
   * [Bug 02 - Falha e Status 500 no endpoint de relatório matematico (Totais)](./bugs/bug-02-totais-erro-500.md) 
3. **Regra de "Categoria só pode ser usada conforme finalidade" (Falhou no Exception Handling):** 
   * [Bug 03 - Vazamento de Exceção de Domínio retornando HTTP 500 em vez do BadRequest](./bugs/bug-03-categoria-finalidade-erro-500.md)

---

## 4. Como rodar cada tipo de teste

**Pré-requisito (Backend e Frontend onlines):** 
Certifique-se de que disponibilizou os containers no Docker pela sua máquina.
```bash
docker-compose up -d
```

### Testes de Integração (C# Backend)
```bash
cd Backend.Tests
dotnet restore
dotnet test
```

### Testes Unitários Vitest (TypeScript)
```bash
cd Frontend.Tests
npm install
npm run test:unit
```

### Testes E2E Playwright (TypeScript/UI)
```bash
cd Frontend.Tests
npm install
npx playwright install
npx playwright test
```
