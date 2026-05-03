using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using tech_challenge.Domain.Exceptions;

namespace tech_challenge.Domain.Aggregates.Veiculos
{
    public class Placa
    {
        public string Valor { get; private set; } = string.Empty;

        public Placa(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new DomainException("Placa inválida");

            if(!ValidarPlaca(valor))
                throw new DomainException("Placa inválida. Formato esperado: AAA1234 ou AAA1A23.");

            Valor = valor.Trim().ToUpper();
        }

        public bool ValidarPlaca(string placa)
        {
            if (string.IsNullOrWhiteSpace(placa)) return false;

            // Remove hífens e espaços para padronizar
            string placaLimpa = placa.Replace("-", "").Trim();

            // Regex: 
            // ^[A-Z]{3}[0-9]{4}$ -> Antiga: AAA1234
            // ^[A-Z]{3}[0-9][A-Z][0-9]{2}$ -> Mercosul: AAA1A23
            string padrao = @"^[a-zA-Z]{3}[0-9]{4}$|^[a-zA-Z]{3}[0-9][a-zA-Z][0-9]{2}$";

            return Regex.IsMatch(placaLimpa, padrao);
        }
    }
}
