namespace tech_challenge.API.Model.Clientes
{
    public class CriarClienteRequest
    {
        public required string Nome { get; set; }
        public required string Documento { get; set; }
        public required string Email { get; set; }
        public string? Telefone { get; set; }
    }
}
