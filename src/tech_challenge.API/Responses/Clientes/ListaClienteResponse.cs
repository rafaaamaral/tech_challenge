namespace tech_challenge.API.Responses.Clientes
{
    public class ListaClienteResponse
    {
        public Guid UniqueCode { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
    }
}
