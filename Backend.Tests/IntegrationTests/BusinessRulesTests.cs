using System.Net;
using System.Text.Json;
using FluentAssertions;
using RestSharp;
using Xunit;

namespace Backend.Tests.IntegrationTests;

public class BusinessRulesTests
{
    private readonly RestClient _client;

    public BusinessRulesTests()
    {
        var options = new RestClientOptions("http://localhost:5000");
        _client = new RestClient(options);
    }

    [Fact(DisplayName = "Regra 1: Menores de 18 anos não podem registrar receitas.")]
    public async Task Minor_Cannot_Register_Revenues()
    {
        var createPessoaReq = new RestRequest("/api/v1/Pessoas", Method.Post);
        createPessoaReq.AddJsonBody(new { Nome = "Crianca Teste", DataNascimento = DateTime.Now.AddYears(-10).ToString("yyyy-MM-dd") });
        var pessoaResponse = await _client.ExecuteAsync(createPessoaReq);
        pessoaResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var pessoaId = JsonDocument.Parse(pessoaResponse.Content!).RootElement.GetProperty("id").GetString();

        var createCategoriaReq = new RestRequest("/api/v1/Categorias", Method.Post);
        createCategoriaReq.AddJsonBody(new { Descricao = "Mesada", Finalidade = 1 });
        var categoriaResponse = await _client.ExecuteAsync(createCategoriaReq);
        var categoriaId = JsonDocument.Parse(categoriaResponse.Content!).RootElement.GetProperty("id").GetString();

        var createTransacaoReq = new RestRequest("/api/v1/Transacoes", Method.Post);
        createTransacaoReq.AddJsonBody(new {
            Descricao = "Recebimento",
            Valor = 100.50,
            Tipo = 1,
            CategoriaId = categoriaId,
            PessoaId = pessoaId,
            Data = DateTime.Now.ToString("yyyy-MM-dd")
        });

        var transacaoResponse = await _client.ExecuteAsync(createTransacaoReq);

        transacaoResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        transacaoResponse.Content.Should().Contain("Menores de 18 anos não podem registrar receitas");
    }

    [Fact(DisplayName = "Regra 2: Categoria só pode ser usada conforme sua finalidade.")]
    public async Task Category_Must_Respect_Its_Purpose()
    {
        var createCategoriaReq = new RestRequest("/api/v1/Categorias", Method.Post);
        createCategoriaReq.AddJsonBody(new { Descricao = "Luz", Finalidade = 0 });
        var categoriaResponse = await _client.ExecuteAsync(createCategoriaReq);
        var categoriaId = JsonDocument.Parse(categoriaResponse.Content!).RootElement.GetProperty("id").GetString();

        var createPessoaReq = new RestRequest("/api/v1/Pessoas", Method.Post);
        createPessoaReq.AddJsonBody(new { Nome = "Adulto Teste", DataNascimento = DateTime.Now.AddYears(-25).ToString("yyyy-MM-dd") });
        var pessoaResponse = await _client.ExecuteAsync(createPessoaReq);
        var pessoaId = JsonDocument.Parse(pessoaResponse.Content!).RootElement.GetProperty("id").GetString();

        var createTransacaoReq = new RestRequest("/api/v1/Transacoes", Method.Post);
        createTransacaoReq.AddJsonBody(new {
            Descricao = "Salario Errado",
            Valor = 5000,
            Tipo = 1,
            CategoriaId = categoriaId,
            PessoaId = pessoaId,
            Data = DateTime.Now.ToString("yyyy-MM-dd")
        });

        var transacaoResponse = await _client.ExecuteAsync(createTransacaoReq);

        transacaoResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        transacaoResponse.Content.Should().Contain("Não é possível registrar receita em categoria de despesa");
    }

    [Fact(DisplayName = "Regra 3: Exclusão em cascata de transações ao excluir pessoa")]
    public async Task Cascading_Delete_Removes_Transactions_When_Person_Is_Deleted()
    {
        var createCategoriaReq = new RestRequest("/api/v1/Categorias", Method.Post);
        createCategoriaReq.AddJsonBody(new { Descricao = "Mercado", Finalidade = 0 });
        var categoriaResponse = await _client.ExecuteAsync(createCategoriaReq);
        var categoriaId = JsonDocument.Parse(categoriaResponse.Content!).RootElement.GetProperty("id").GetString();

        var createPessoaReq = new RestRequest("/api/v1/Pessoas", Method.Post);
        createPessoaReq.AddJsonBody(new { Nome = "Pessoa Cascata", DataNascimento = DateTime.Now.AddYears(-30).ToString("yyyy-MM-dd") });
        var pessoaResponse = await _client.ExecuteAsync(createPessoaReq);
        var pessoaId = JsonDocument.Parse(pessoaResponse.Content!).RootElement.GetProperty("id").GetString();

        var createTransacaoReq = new RestRequest("/api/v1/Transacoes", Method.Post);
        createTransacaoReq.AddJsonBody(new {
            Descricao = "Compra",
            Valor = 300,
            Tipo = 0,
            CategoriaId = categoriaId,
            PessoaId = pessoaId,
            Data = DateTime.Now.ToString("yyyy-MM-dd")
        });
        var transacaoResponse = await _client.ExecuteAsync(createTransacaoReq);
        var transacaoId = JsonDocument.Parse(transacaoResponse.Content!).RootElement.GetProperty("id").GetString();

        var deleteReq = new RestRequest($"/api/v1/Pessoas/{pessoaId}", Method.Delete);
        var deleteResponse = await _client.ExecuteAsync(deleteReq);

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getTransacaoReq = new RestRequest($"/api/v1/Transacoes/{transacaoId}", Method.Get);
        var getTransacaoResponse = await _client.ExecuteAsync(getTransacaoReq);

        getTransacaoResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
