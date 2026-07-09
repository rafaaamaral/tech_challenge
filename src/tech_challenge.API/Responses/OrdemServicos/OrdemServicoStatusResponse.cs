namespace tech_challenge.API.Responses.OrdemServicos
{
    public class OrdemServicoStatusResponse
    {
        public Guid UniqueCode { get; set; }
        public int Numero { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
