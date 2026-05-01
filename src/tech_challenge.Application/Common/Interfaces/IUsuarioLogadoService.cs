using System;
using System.Collections.Generic;
using System.Text;

namespace tech_challenge.Application.Common.Interfaces
{
    public interface IUsuarioLogadoService
    {
        Guid? UniqueCode { get; }
        string? Nome { get; }
        string? Email { get; }
        string? Perfil { get; }
    }
}
