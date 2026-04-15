# Bug 03 - Status Code Incorreto em Regra de Domínio (Categoria)

**Descrição:** 
A regra "Categoria só pode ser usada conforme sua finalidade" atua efetivamente bloqueando transações maliciosas no back-end. Contudo, em casos de bloqueio, o retorno está gerando `500 Internal Server Error` no lugar de `400 BadRequest`.

**Passos para reproduzir:**
1. Crie uma categoria vinculada estritamente à finalidade "Despesa" via `POST /api/v1/Categorias`.
2. Registre uma nova transação através do `POST /api/v1/Transacoes` preenchendo o payload com o ID da categoria acima, porém definindo a flag do Tipo como "Receita" (1).

**Comportamento Esperado:** 
Cenário de bloqueio amigável alertando violação de negócio (`400 BadRequest`), permitindo que a aplicação *client* leia o retorno sem crash global.

**Comportamento Encontrado:**
A ação quebra o fluxo de resposta gerando Exception global (`500 Internal Server Error`).

**Análise Recomendada:**
Adicionar tratamento ou Exception Filter Attribute em middleware para capturar devidas exceptions (Domain e Business) efetuando swap para HTTP 400 na saída JSON.
