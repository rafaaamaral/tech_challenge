using System;
using System.Collections.Generic;
using System.Text;

namespace tech_challenge.Application.Services.Clientes.Model
{
    public class ClienteModel
    {
        public Guid UniqueCode { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Telefone { get; set; }
        public bool Ativo { get; set; }
    }
}
