using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using tech_challenge.Teste.Integration.Support;

namespace tech_challenge.Teste.Integration.Autenticacao
{
    public class AutenticacaoIntegrationTeste : IClassFixture<ApiIntegrationFactory>
    {
        private readonly ApiIntegrationFactory _factory;

        public AutenticacaoIntegrationTeste(ApiIntegrationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task LoginValido_DeveRetornarToken()
        {
            if (!_factory.IsDatabaseAvailable())
            {
                return;
            }

            var client = _factory.CreateClient();

            var response = await client.PostAsJsonAsync("/api/Auth/login", new
            {
                Login = "atendimento@techchallengefase1.com",
                Senha = "123456"
            });

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var payload = await response.Content.ReadFromJsonAsync<LoginResponse>();
            payload.Should().NotBeNull();
            payload!.AccessToken.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task LoginInvalido_DeveRetornarUnauthorized()
        {
            if (!_factory.IsDatabaseAvailable())
            {
                return;
            }

            var client = _factory.CreateClient();

            var response = await client.PostAsJsonAsync("/api/Auth/login", new
            {
                Login = "atendimento@techchallengefase1.com",
                Senha = "senha-invalida"
            });

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task EndpointProtegido_SemToken_DeveRetornarUnauthorized()
        {
            if (!_factory.IsDatabaseAvailable())
            {
                return;
            }

            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/OrdemServico");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        private sealed class LoginResponse
        {
            public string AccessToken { get; set; } = string.Empty;
        }
    }
}
