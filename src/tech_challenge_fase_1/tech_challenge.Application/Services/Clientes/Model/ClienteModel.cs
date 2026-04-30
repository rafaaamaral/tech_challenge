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
        public string Contato { get; set; } = string.Empty;
    }
}
