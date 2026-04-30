using System;
using System.Collections.Generic;
using System.Text;
using tech_challenge.Application.Interfaces.Repositories;
using tech_challenge.Domain.Aggregates.Clientes;
using tech_challenge.Infrastructure.Persistence.Context;
using tech_challenge.Infrastructure.Persistence.Repositories.Base;

namespace tech_challenge.Infrastructure.Persistence.Repositories
{
    public class ClienteRepository : BaseRepository<Cliente>, IClienteRepository
    {
        public ClienteRepository(AppDbContext context) : base(context)
        {
        }
    }
}
