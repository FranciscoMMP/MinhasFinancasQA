# Bug 01 - Falha na Exclusão em Cascata de Transações

**Descrição:** 
A regra estipula exclusão em cascata: ao deletar uma pessoa, as transações vinculadas à ela também devem ser excluídas automaticamente. Na execução dos testes, após deletar uma pessoa existente com sucesso (via `DELETE /api/v1/Pessoas/{id}`), as transações criadas anteriormente continuaram acessíveis isoladamente na base de dados (a consulta da transação não foi proscrita, retornando Status 200 ao invés do esperado 404).

**Passos para reproduzir:**
1. Criar novo usuário `POST /api/v1/Pessoas`.
2. Registrar transação referenciando essa pessoa através do `POST /api/v1/Transacoes`.
3. Executar soft/hard delete do usuário chamando `DELETE /api/v1/Pessoas/{id}`.
4. Consultar a respectiva transação aguardando a exclusão efetivada.

**Comportamento Esperado:** 
Transação deve ser apagada da persistência, retornando Http Status `404 Not Found` na tentativa de consulta pós-deleção.

**Comportamento Encontrado:**
Transação permaneceu ativa no banco de dados (retornando `200 OK`), o que fere drasticamente a restrição imposta pela regra de negócio estipulada (Orfandade).

**Análise Recomendada:**
Ajustar o Entity Framework Core Configurations para `OnDelete(DeleteBehavior.Cascade)` no relacionamento da entidade Pessoa, ou aplicar limpeza explícita de repositório na instrução `PessoaService`.
