using tech_challenge.Application.Interfaces.Repositories;
using tech_challenge.Domain.Aggregates.Veiculos;
using tech_challenge.Infrastructure.Persistence.Context;
using tech_challenge.Infrastructure.Persistence.Repositories.Base;

namespace tech_challenge.Infrastructure.Persistence.Repositories
{
    public class VeiculoRepository : BaseRepository<Veiculo>, IVeiculoRepository
    {
        public VeiculoRepository(AppDbContext context) : base(context)
        {
        }
    }
}
