using tech_challenge.Application.Interfaces.Repositories;
using tech_challenge.Application.Interfaces.Services;
using tech_challenge.Application.Services.Usuarios.Model;

namespace tech_challenge.Application.Services.Usuarios
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }
        public async Task<UsuarioModel> ObterPorEmailAsync(string email)
        {
            var usuario = await _usuarioRepository.ObterPorEmailAsync(email);

            return new UsuarioModel
            {
                UniqueCode = usuario.UniqueCode,
                Nome = usuario.Nome,
                Login = usuario.Login,
                Senha = usuario.Senha,
                Perfil = usuario.Perfil
            };
        }
    }
}
