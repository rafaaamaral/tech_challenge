using Microsoft.EntityFrameworkCore;
using tech_challenge.Application.Interfaces.Repositories;
using tech_challenge.Domain.Aggregates.OrdemServicos;
using tech_challenge.Domain.Common.Enums;
using tech_challenge.Infrastructure.Persistence.Context;
using tech_challenge.Infrastructure.Persistence.Repositories.Base;

namespace tech_challenge.Infrastructure.Persistence.Repositories
{
    public class OrdemServicoRepository : BaseRepository<OrdemServico>, IOrdemServicoRepository
    {
        public OrdemServicoRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<OrdemServico>> ListarComItensAsync()
        {
            return await _context.OrdemServicos
                .AsNoTracking()
                .Include(x => x.ItensServicos)
                .Include(x => x.ItensPecasInsumos)
                .ToListAsync();
        }

        public async Task<List<OrdemServico>> ListarPorClienteAsync(Guid clienteUniqueCode)
        {
            var listStatus = new List<StatusOrdemServico>
            {
                StatusOrdemServico.Finalizada,
                StatusOrdemServico.Entregue
            };

            return await _context.OrdemServicos
                .AsNoTracking()
                .Include(x => x.ItensServicos)
                .Include(x => x.ItensPecasInsumos)
                .Include(x => x.Cliente)
                .Where(x => x.Cliente.UniqueCode == clienteUniqueCode && !listStatus.Contains(x.Status))

                .ToListAsync();
        }

        public async Task<OrdemServico?> ObterPorIdComItensAsync(int id)
        {
            return await _context.OrdemServicos
                .AsNoTracking()
                .Include(x => x.Cliente)
                .Include(x => x.ItensServicos)
                .Include(x => x.ItensPecasInsumos)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<OrdemServico?> ObterPorUniqueCodeAsync(Guid uniqueCode)
        {
            return await _context.OrdemServicos
                .AsNoTracking()
                .Include(x => x.Cliente)
                .FirstOrDefaultAsync(x => x.UniqueCode == uniqueCode);
        }
    }
}
