# Bug 02 - Falha na Consulta de Relatório de Totais (HTTP 500)

**Descrição:** 
O endpoint de consolidação de saldo por pessoa (`GET /api/v1/Totais/{pessoaId}`) apresenta inconsistência quando consultado para um usuário que possua transações vinculadas. Em vez de retornar um `200 OK` contendo os dados matemáticos calculados de Saldo, Receitas e Despesas, a requisição é abortada e retorna erro no servidor.

**Passos para reproduzir:**
1. Realizar criação de usuário via `POST /api/v1/Pessoas`.
2. Registrar transações de Despesa e Receita no endpoint `POST /api/v1/Transacoes` para o usuário criado.
3. Consultar o cálculo matemático em `GET /api/v1/Totais/{pessoaId}`.

**Comportamento Esperado:** 
Transação com Status `200 OK`, descrevendo o objeto JSON com os cálculos exatos numéricos.

**Comportamento Encontrado:**
Status HTTP `500 Internal Server Error`, resultando falha do servidor e inviabilização da consulta.  

**Análise Recomendada:**
Revisar lógicas de agregação matemática (SQL Groups ou Nullable Properties) no respectivo Request Pipeline, pois o somatório de transações agrupadas parece estar estourando exceções não tratadas.
