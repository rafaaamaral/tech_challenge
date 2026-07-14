using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using tech_challenge.Domain.Aggregates.OrdemServicos;
using tech_challenge.Domain.Common.Enums;
using tech_challenge.Infrastructure.Persistence.Context;
using tech_challenge.Teste.Integration.Support;

namespace tech_challenge.Teste.Integration.OrdemServicos
{
    public class OrdemServicoIntegrationTeste : IClassFixture<ApiIntegrationFactory>
    {
        private const string WebhookApiKey = "MKSLKSmYJ0cwFtrOULmoTPW9TmXG7NHwTKoMhWg3sEt";
        private readonly ApiIntegrationFactory _factory;

        public OrdemServicoIntegrationTeste(ApiIntegrationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task AbrirOrdemServico_DevePersistirERetornarIdentificacao()
        {
            if (!_factory.IsDatabaseAvailable())
            {
                return;
            }

            var client = await _factory.CreateAuthenticatedClientAsync();

            using var scope = _factory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var seed = await IntegrationDataHelper.SeedBaseDataAsync(context);

            var request = CriarRequest(seed.Cliente.Id, seed.Veiculo.Id, seed.Servico.Id, seed.PecaInsumo.Id);

            var response = await client.PostAsJsonAsync("/api/OrdemServico", request);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var payload = await response.Content.ReadFromJsonAsync<OrdemServicoResponse>();
            payload.Should().NotBeNull();
            payload!.Id.Should().BeGreaterThan(0);
            payload.UniqueCode.Should().NotBe(Guid.Empty);

            var persisted = await context.OrdemServicos
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == payload.Id);

            var persistedFallback = persisted ?? await context.Set<OrdemServico>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == payload.Id);

            persistedFallback.Should().NotBeNull();
            persistedFallback!.ClienteId.Should().Be(seed.Cliente.Id);
            persistedFallback.VeiculoId.Should().Be(seed.Veiculo.Id);
        }

        [Fact]
        public async Task ConsultarOrdemServico_DeveRetornarDadosDaOrdem()
        {
            if (!_factory.IsDatabaseAvailable())
            {
                return;
            }

            var client = await _factory.CreateAuthenticatedClientAsync();

            using var scope = _factory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var seed = await IntegrationDataHelper.SeedBaseDataAsync(context);

            var created = await CriarOrdemServicoAsync(client, seed);

            var response = await client.GetAsync($"/api/OrdemServico/{created.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var payload = await response.Content.ReadFromJsonAsync<OrdemServicoResponse>();
            payload.Should().NotBeNull();
            payload!.Id.Should().Be(created.Id);
            payload.ClienteId.Should().Be(seed.Cliente.Id);
            payload.VeiculoId.Should().Be(seed.Veiculo.Id);
            payload.Status.Should().Be(StatusOrdemServico.Recebida.ToString());
        }

        [Fact]
        public async Task WebhookAprovarOrcamento_DeveAlterarStatusParaEmExecucao()
        {
            if (!_factory.IsDatabaseAvailable())
            {
                return;
            }

            var client = await _factory.CreateAuthenticatedClientAsync();

            using var scope = _factory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var seed = await IntegrationDataHelper.SeedBaseDataAsync(context);

            var created = await CriarOrdemServicoAsync(client, seed);

            var gerarOrcamentoResponse = await client.PatchAsync($"/api/OrdemServico/{created.Id}/gerar-orcamento", null);
            gerarOrcamentoResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var webhookRequest = new
            {
                UniqueCode = created.UniqueCode,
                Status = StatusOrcamento.Aprovado
            };

            var webhookMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Webhook/integracoes/orcamentos")
            {
                Content = JsonContent.Create(webhookRequest)
            };
            webhookMessage.Headers.Add("X-Webhook-Key", WebhookApiKey);

            var webhookResponse = await client.SendAsync(webhookMessage);
            webhookResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var consultaResponse = await client.GetAsync($"/api/OrdemServico/{created.Id}");
            consultaResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var payload = await consultaResponse.Content.ReadFromJsonAsync<OrdemServicoResponse>();
            payload.Should().NotBeNull();
            payload!.Status.Should().Be(StatusOrdemServico.EmExecucao.ToString());
            payload.Orcamento.Status.Should().Be(StatusOrcamento.Aprovado.ToString());
        }

        [Fact]
        public async Task WebhookReprovarOrcamento_DeveAlterarStatusDoOrcamento()
        {
            if (!_factory.IsDatabaseAvailable())
            {
                return;
            }

            var client = await _factory.CreateAuthenticatedClientAsync();

            using var scope = _factory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var seed = await IntegrationDataHelper.SeedBaseDataAsync(context);

            var created = await CriarOrdemServicoAsync(client, seed);

            var gerarOrcamentoResponse = await client.PatchAsync($"/api/OrdemServico/{created.Id}/gerar-orcamento", null);
            gerarOrcamentoResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var webhookRequest = new
            {
                UniqueCode = created.UniqueCode,
                Status = StatusOrcamento.Reprovado
            };

            var webhookMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Webhook/integracoes/orcamentos")
            {
                Content = JsonContent.Create(webhookRequest)
            };
            webhookMessage.Headers.Add("X-Webhook-Key", WebhookApiKey);

            var webhookResponse = await client.SendAsync(webhookMessage);
            webhookResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var consultaResponse = await client.GetAsync($"/api/OrdemServico/{created.Id}");
            consultaResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var payload = await consultaResponse.Content.ReadFromJsonAsync<OrdemServicoResponse>();
            payload.Should().NotBeNull();
            payload!.Status.Should().Be(StatusOrdemServico.AguardandoAprovacao.ToString());
            payload.Orcamento.Status.Should().Be(StatusOrcamento.Reprovado.ToString());
        }

        [Fact]
        public async Task ListarOrdensPorCliente_DeveRespeitarOrdenacaoEExcluirFinalizadaEEntregue()
        {
            if (!_factory.IsDatabaseAvailable())
            {
                return;
            }

            using var scope = _factory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var seed = await IntegrationDataHelper.SeedBaseDataAsync(context);
            var usuarioCliente = await IntegrationDataHelper.SeedClienteUsuarioAsync(context, seed.Cliente);

            await IntegrationDataHelper.SeedOrdemServicoAsync(context, seed.Cliente.Id, seed.Veiculo.Id, 10, StatusOrdemServico.Recebida);
            await IntegrationDataHelper.SeedOrdemServicoAsync(context, seed.Cliente.Id, seed.Veiculo.Id, 20, StatusOrdemServico.EmDiagnostico);
            await IntegrationDataHelper.SeedOrdemServicoAsync(context, seed.Cliente.Id, seed.Veiculo.Id, 30, StatusOrdemServico.Finalizada);
            await IntegrationDataHelper.SeedOrdemServicoAsync(context, seed.Cliente.Id, seed.Veiculo.Id, 40, StatusOrdemServico.Entregue);

            var client = await _factory.CreateAuthenticatedClientAsync(usuarioCliente.Login, "123456");

            var response = await client.GetAsync("/api/OrdemServico/ordem-servico/minhas");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var payload = await response.Content.ReadFromJsonAsync<List<OrdemServicoResponse>>();
            payload.Should().NotBeNull();
            payload!.Should().NotBeEmpty();
            payload.Select(x => x.Numero).Should().BeInAscendingOrder();
            payload.Select(x => x.Status).Should().NotContain(StatusOrdemServico.Finalizada.ToString());
            payload.Select(x => x.Status).Should().NotContain(StatusOrdemServico.Entregue.ToString());
        }

        [Fact]
        public async Task FluxoCompleto_DeveCriarConsultarGerarOrcamentoAprovarEConsultarNovamente()
        {
            if (!_factory.IsDatabaseAvailable())
            {
                return;
            }

            var client = await _factory.CreateAuthenticatedClientAsync();

            using var scope = _factory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var seed = await IntegrationDataHelper.SeedBaseDataAsync(context);

            var created = await CriarOrdemServicoAsync(client, seed);

            var consultaInicial = await client.GetAsync($"/api/OrdemServico/{created.Id}");
            consultaInicial.StatusCode.Should().Be(HttpStatusCode.OK);
            var ordemInicial = await consultaInicial.Content.ReadFromJsonAsync<OrdemServicoResponse>();
            ordemInicial!.Status.Should().Be(StatusOrdemServico.Recebida.ToString());

            var gerarOrcamentoResponse = await client.PatchAsync($"/api/OrdemServico/{created.Id}/gerar-orcamento", null);
            gerarOrcamentoResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var webhookMessage = new HttpRequestMessage(HttpMethod.Post, "/api/Webhook/integracoes/orcamentos")
            {
                Content = JsonContent.Create(new
                {
                    UniqueCode = created.UniqueCode,
                    Status = StatusOrcamento.Aprovado
                })
            };
            webhookMessage.Headers.Add("X-Webhook-Key", WebhookApiKey);

            var webhookResponse = await client.SendAsync(webhookMessage);
            webhookResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var consultaFinal = await client.GetAsync($"/api/OrdemServico/{created.Id}");
            consultaFinal.StatusCode.Should().Be(HttpStatusCode.OK);
            var ordemFinal = await consultaFinal.Content.ReadFromJsonAsync<OrdemServicoResponse>();
            ordemFinal.Should().NotBeNull();
            ordemFinal!.Status.Should().Be(StatusOrdemServico.EmExecucao.ToString());
            ordemFinal.Orcamento.Status.Should().Be(StatusOrcamento.Aprovado.ToString());
        }

        private static object CriarRequest(int clienteId, int veiculoId, int servicoId, int pecaInsumoId)
        {
            return new
            {
                ClienteId = clienteId,
                VeiculoId = veiculoId,
                ItensServicos = new[]
                {
                    new { ServicoId = servicoId }
                },
                ItensPecasInsumos = new[]
                {
                    new { PecaInsumoId = pecaInsumoId, Quantidade = 2 }
                }
            };
        }

        private static async Task<OrdemServicoResponse> CriarOrdemServicoAsync(HttpClient client, BaseSeedData seed)
        {
            var response = await client.PostAsJsonAsync("/api/OrdemServico", CriarRequest(seed.Cliente.Id, seed.Veiculo.Id, seed.Servico.Id, seed.PecaInsumo.Id));
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var payload = await response.Content.ReadFromJsonAsync<OrdemServicoResponse>();
            payload.Should().NotBeNull();
            return payload!;
        }

        private sealed class OrdemServicoResponse
        {
            public int Id { get; set; }
            public Guid UniqueCode { get; set; }
            public int ClienteId { get; set; }
            public int VeiculoId { get; set; }
            public int Numero { get; set; }
            public string Status { get; set; } = string.Empty;
            public OrcamentoResponse Orcamento { get; set; } = new();
        }

        private sealed class OrcamentoResponse
        {
            public string Status { get; set; } = string.Empty;
        }
    }
}
