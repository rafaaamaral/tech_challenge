using tech_challenge.Application.Interfaces.Repositories;
using tech_challenge.Domain.Aggregates.PecaInsumos;
using tech_challenge.Infrastructure.Persistence.Context;
using tech_challenge.Infrastructure.Persistence.Repositories.Base;

namespace tech_challenge.Infrastructure.Persistence.Repositories
{
    public class PecaInsumoRepository : BaseRepository<PecaInsumo>, IPecaInsumoRepository
    {
        public PecaInsumoRepository(AppDbContext context) : base(context)
        {
        }
    }
}
