using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace tech_challenge.Teste.Integration.Support
{
    public class ApiIntegrationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private const string TestAdminConnectionEnv = "TECH_CHALLENGE_TEST_ADMIN_CONNECTION";
        private const string DefaultAdminConnection = "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=postgres";

        private readonly string _databaseName = $"tech_challenge_it_{Guid.NewGuid():N}";
        private string _connectionString = string.Empty;
        private bool _databaseReady;
        private string? _initializationError;

        public string ConnectionString => _connectionString;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");

            builder.ConfigureAppConfiguration((_, configBuilder) =>
            {
                var settings = new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = _connectionString,
                    ["Webhook:ApiKey"] = "MKSLKSmYJ0cwFtrOULmoTPW9TmXG7NHwTKoMhWg3sEt"
                };

                configBuilder.AddInMemoryCollection(settings);
            });
        }

        public async Task InitializeAsync()
        {
            var adminConnection = Environment.GetEnvironmentVariable(TestAdminConnectionEnv);
            var adminConnectionString = string.IsNullOrWhiteSpace(adminConnection)
                ? DefaultAdminConnection
                : adminConnection;

            var builder = new NpgsqlConnectionStringBuilder(adminConnectionString)
            {
                Database = _databaseName
            };

            _connectionString = builder.ConnectionString;

            try
            {
                await using var connection = new NpgsqlConnection(adminConnectionString);
                await connection.OpenAsync();

                await using var command = connection.CreateCommand();
                command.CommandText = $"CREATE DATABASE \"{_databaseName}\"";
                await command.ExecuteNonQueryAsync();
                _databaseReady = true;
            }
            catch (Exception ex)
            {
                _databaseReady = false;
                _initializationError = ex.Message;
            }
        }

        public new async Task DisposeAsync()
        {
            if (!_databaseReady)
            {
                await base.DisposeAsync();
                return;
            }

            var adminConnection = Environment.GetEnvironmentVariable(TestAdminConnectionEnv);
            var adminConnectionString = string.IsNullOrWhiteSpace(adminConnection)
                ? DefaultAdminConnection
                : adminConnection;

            await using var connection = new NpgsqlConnection(adminConnectionString);
            await connection.OpenAsync();

            await using (var terminateCommand = connection.CreateCommand())
            {
                terminateCommand.CommandText =
                    "SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname = @dbName AND pid <> pg_backend_pid();";
                terminateCommand.Parameters.AddWithValue("dbName", _databaseName);
                await terminateCommand.ExecuteNonQueryAsync();
            }

            await using (var dropCommand = connection.CreateCommand())
            {
                dropCommand.CommandText = $"DROP DATABASE IF EXISTS \"{_databaseName}\"";
                await dropCommand.ExecuteNonQueryAsync();
            }

            await base.DisposeAsync();
        }

        public bool IsDatabaseAvailable()
        {
            return _databaseReady;
        }

        public string? GetDatabaseUnavailableReason() => _initializationError;

        public async Task<HttpClient> CreateAuthenticatedClientAsync(string login = "atendimento@techchallengefase1.com", string senha = "123456")
        {
            var client = CreateClient();

            var response = await client.PostAsJsonAsync("/api/Auth/login", new
            {
                Login = login,
                Senha = senha
            });

            response.EnsureSuccessStatusCode();

            var payload = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (payload is null || string.IsNullOrWhiteSpace(payload.AccessToken))
            {
                throw new InvalidOperationException("Não foi possível obter token de autenticação.");
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", payload.AccessToken);
            return client;
        }

        public IServiceScope CreateScope()
        {
            return Services.CreateScope();
        }

        private sealed class LoginResponse
        {
            public string AccessToken { get; set; } = string.Empty;
        }
    }
}
