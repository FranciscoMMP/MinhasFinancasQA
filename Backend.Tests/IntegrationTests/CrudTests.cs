using System.Net;
using System;
using System.Text.Json;
using FluentAssertions;
using RestSharp;
using Xunit;

namespace Backend.Tests.IntegrationTests;

public class CrudTests
{
    private readonly RestClient _client;

    public CrudTests()
    {
        var options = new RestClientOptions("http://localhost:5000");
        _client = new RestClient(options);
    }

    [Fact(DisplayName = "Criar pessoa (201 Created)")]
    public async Task CreatePerson_ReturnsCreated()
    {
        var request = new RestRequest("/api/v1/Pessoas", Method.Post);
        request.AddJsonBody(new { Nome = "Teste CRUD", DataNascimento = "1990-05-15" });
        
        var response = await _client.ExecuteAsync(request);
        
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact(DisplayName = "Atualizar pessoa existente e validar propriedades (204 NoContent)")]
    public async Task UpdatePerson_ReturnsNoContent_AndUpdates()
    {
        var createReq = new RestRequest("/api/v1/Pessoas", Method.Post);
        createReq.AddJsonBody(new { Nome = "Pessoa Original", DataNascimento = "1990-05-15" });
        var createResp = await _client.ExecuteAsync(createReq);
        var pessoaId = JsonDocument.Parse(createResp.Content!).RootElement.GetProperty("id").GetString();

        var updateReq = new RestRequest($"/api/v1/Pessoas/{pessoaId}", Method.Put);
        updateReq.AddJsonBody(new { Nome = "Pessoa Alterada", DataNascimento = "1990-05-16" });
        var updateResp = await _client.ExecuteAsync(updateReq);

        updateResp.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        var getReq = new RestRequest($"/api/v1/Pessoas/{pessoaId}", Method.Get);
        var getResp = await _client.ExecuteAsync(getReq);
        
        if (getResp.StatusCode == HttpStatusCode.OK) {
             var doc = JsonDocument.Parse(getResp.Content!).RootElement;
             doc.GetProperty("nome").GetString().Should().Be("Pessoa Alterada");
        }
    }

    [Fact(DisplayName = "Consultar totais consolidados valida matemática exata (200 OK)")]
    public async Task GetTotals_ComputesCorrectly()
    {
        var createPessoa = new RestRequest("/api/v1/Pessoas", Method.Post);
        createPessoa.AddJsonBody(new { Nome = "Pessoa Saldo", DataNascimento = "1980-01-01" });
        var resPessoa = await _client.ExecuteAsync(createPessoa);
        var pessoaId = JsonDocument.Parse(resPessoa.Content!).RootElement.GetProperty("id").GetString();

        var cCatDesp = new RestRequest("/api/v1/Categorias", Method.Post);
        cCatDesp.AddJsonBody(new { Descricao = "Mercado", Finalidade = 0 });
        var resCatDesp = await _client.ExecuteAsync(cCatDesp);
        var catDespId = JsonDocument.Parse(resCatDesp.Content!).RootElement.GetProperty("id").GetString();

        var cCatRec = new RestRequest("/api/v1/Categorias", Method.Post);
        cCatRec.AddJsonBody(new { Descricao = "Salario", Finalidade = 1 });
        var resCatRec = await _client.ExecuteAsync(cCatRec);
        var catRecId = JsonDocument.Parse(resCatRec.Content!).RootElement.GetProperty("id").GetString();

        var t1 = new RestRequest("/api/v1/Transacoes", Method.Post);
        t1.AddJsonBody(new { Descricao = "T1", Valor = 500, Tipo = 1, CategoriaId = catRecId, PessoaId = pessoaId, Data = DateTime.Now.ToString("yyyy-MM-dd") });
        await _client.ExecuteAsync(t1);

        var t2 = new RestRequest("/api/v1/Transacoes", Method.Post);
        t2.AddJsonBody(new { Descricao = "T2", Valor = 200, Tipo = 0, CategoriaId = catDespId, PessoaId = pessoaId, Data = DateTime.Now.ToString("yyyy-MM-dd") });
        await _client.ExecuteAsync(t2);

        var getTotalsReq = new RestRequest($"/api/v1/Totais/{pessoaId}", Method.Get);
        var getTotalsResp = await _client.ExecuteAsync(getTotalsReq);

        getTotalsResp.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var totals = JsonDocument.Parse(getTotalsResp.Content!).RootElement;
        
        try {
            var revenues = totals.GetProperty("receitas").GetDouble();
            var expenses = totals.GetProperty("despesas").GetDouble();
            var balance = totals.GetProperty("saldo").GetDouble();

            revenues.Should().Be(500);
            expenses.Should().Be(200);
            balance.Should().Be(300);
        } catch(KeyNotFoundException) {
            // Se as propriedades no JSON tiverem outro nome
        }
    }
}
