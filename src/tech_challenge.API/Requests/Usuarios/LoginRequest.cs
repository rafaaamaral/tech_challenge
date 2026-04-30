namespace tech_challenge.API.Requests.Usuarios
{
    public class LoginRequest
    {
        public required string Login { get; set; }
        public required string Senha { get; set; }
    }
}
