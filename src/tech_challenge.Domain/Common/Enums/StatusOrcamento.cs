using System.ComponentModel;

namespace tech_challenge.Domain.Common.Enums
{
    public enum StatusOrcamento
    {
        [Description("Pendente")]
        Pendente = 1,
        [Description("Aprovado")]
        Aprovado = 2,
        [Description("Reprovado")]
        Reprovado = 3
    }
}
