using System;
using System.Collections.Generic;
using System.Text;
using tech_challenge.Domain.Exceptions;

namespace tech_challenge.Domain.Aggregates.Clientes
{
    public class Documento
    {
        public string Valor { get; private set; }

        public Documento(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new DomainException("Documento inválido");

            var somenteNumeros = Limpar(valor);

            if (!EhCpfValido(somenteNumeros) && !EhCnpjValido(somenteNumeros))
                throw new DomainException("Documento inválido.");

            Valor = somenteNumeros;
        }

        public override string ToString() => Valor;

        private static string Limpar(string valor)
        => new string(valor.Where(char.IsDigit).ToArray());

        private static bool EhCpfValido(string cpf)
        {
            if (cpf.Length != 11 || cpf.Distinct().Count() == 1)
                return false;

            var soma = 0;
            for (int i = 0; i < 9; i++)
                soma += (cpf[i] - '0') * (10 - i);

            var resto = soma % 11;
            var dig1 = resto < 2 ? 0 : 11 - resto;

            if (cpf[9] - '0' != dig1)
                return false;

            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += (cpf[i] - '0') * (11 - i);

            resto = soma % 11;
            var dig2 = resto < 2 ? 0 : 11 - resto;

            return cpf[10] - '0' == dig2;
        }

        private static bool EhCnpjValido(string cnpj)
        {
            if (cnpj.Length != 14 || cnpj.Distinct().Count() == 1)
                return false;

            int[] multiplicador1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            var temp = cnpj.Substring(0, 12);

            var soma = temp.Select((t, i) => (t - '0') * multiplicador1[i]).Sum();
            var resto = soma % 11;
            var dig1 = resto < 2 ? 0 : 11 - resto;

            temp += dig1;

            soma = temp.Select((t, i) => (t - '0') * multiplicador2[i]).Sum();
            resto = soma % 11;
            var dig2 = resto < 2 ? 0 : 11 - resto;

            return cnpj.EndsWith($"{dig1}{dig2}");
        }
    }
}
