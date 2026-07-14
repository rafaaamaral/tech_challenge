using Microsoft.EntityFrameworkCore;
using tech_challenge.Domain.Aggregates.Clientes;
using tech_challenge.Domain.Aggregates.OrdemServicos;
using tech_challenge.Domain.Aggregates.PecaInsumos;
using tech_challenge.Domain.Aggregates.Servicos;
using tech_challenge.Domain.Aggregates.Usuarios;
using tech_challenge.Domain.Aggregates.Veiculos;
using tech_challenge.Domain.Common.Enums;
using tech_challenge.Infrastructure.Persistence.Context;

namespace tech_challenge.Teste.Integration.Support
{
    public static class IntegrationDataHelper
    {
        public static async Task<BaseSeedData> SeedBaseDataAsync(AppDbContext context)
        {
            var sufixo = Guid.NewGuid().ToString("N")[..8];

            var cliente = Cliente.Criar($"Cliente {sufixo}", GerarDocumentoNumerico(), $"cliente.{sufixo}@email.com", "11999999999");
            await context.Clientes.AddAsync(cliente);
            await context.SaveChangesAsync();

            var veiculo = Veiculo.Criar(cliente.Id, $"TES{sufixo[..4]}", "FIAT", "UNO", 2020);
            await context.Veiculos.AddAsync(veiculo);

            var servico = Servico.Criar($"Servico {sufixo}", $"SVC-{sufixo}", "Troca de óleo", 150m, 60);
            await context.Servicos.AddAsync(servico);

            var pecaInsumo = PecaInsumo.Criar($"Peca {sufixo}", $"PEC-{sufixo}", "Filtro de óleo", TipoPecaInsumo.Peca, 50m, 20);
            await context.PecasInsumos.AddAsync(pecaInsumo);

            await context.SaveChangesAsync();

            return new BaseSeedData(cliente, veiculo, servico, pecaInsumo);
        }

        public static async Task<Usuario> SeedClienteUsuarioAsync(AppDbContext context, Cliente cliente)
        {
            var sufixo = Guid.NewGuid().ToString("N")[..8];
            var usuario = Usuario.Criar(cliente.UniqueCode, $"Cliente Usuario {sufixo}", $"cliente.usuario.{sufixo}@email.com", PerfilUsuario.Cliente);

            await context.Usuarios.AddAsync(usuario);
            await context.SaveChangesAsync();
            return usuario;
        }

        public static async Task<OrdemServico> SeedOrdemServicoAsync(
            AppDbContext context,
            int clienteId,
            int veiculoId,
            int numero,
            StatusOrdemServico status)
        {
            var ordemServico = OrdemServico.Criar(clienteId, veiculoId, numero);
            ordemServico.Status = status;

            await context.OrdemServicos.AddAsync(ordemServico);
            await context.SaveChangesAsync();
            return ordemServico;
        }

        public static async Task<int> NextOrdemNumeroAsync(AppDbContext context)
        {
            var maxNumero = await context.OrdemServicos.MaxAsync(x => (int?)x.Numero) ?? 0;
            return maxNumero + 1;
        }

        private static string GerarDocumentoNumerico()
        {
            var random = new Random();
            return string.Concat(Enumerable.Range(0, 11).Select(_ => random.Next(0, 10).ToString()));
        }
    }

    public record BaseSeedData(Cliente Cliente, Veiculo Veiculo, Servico Servico, PecaInsumo PecaInsumo);
}
