using tech_challenge.Application.Interfaces.Repositories;
using tech_challenge.Domain.Aggregates.Servicos;
using tech_challenge.Infrastructure.Persistence.Context;
using tech_challenge.Infrastructure.Persistence.Repositories.Base;

namespace tech_challenge.Infrastructure.Persistence.Repositories
{
    public class ServicoRepository : BaseRepository<Servico>, IServicoRepository
    {
        public ServicoRepository(AppDbContext context) : base(context)
        {
        }
    }
}
