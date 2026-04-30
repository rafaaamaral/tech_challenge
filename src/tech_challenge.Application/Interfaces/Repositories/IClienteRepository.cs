using System;
using System.Collections.Generic;
using System.Text;
using tech_challenge.Application.Interfaces.Repositories.Base;
using tech_challenge.Domain.Aggregates.Clientes;

namespace tech_challenge.Application.Interfaces.Repositories
{
    public interface IClienteRepository : IBaseRepository<Cliente>
    {
    }
}
