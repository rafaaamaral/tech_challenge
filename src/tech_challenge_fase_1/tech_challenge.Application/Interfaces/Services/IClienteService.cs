using System;
using System.Collections.Generic;
using System.Text;

namespace tech_challenge.Application.Interfaces.Services
{
    public interface IClienteService
    {
        Task AddAsync(string nome, string documento, string contato);
    }
}
