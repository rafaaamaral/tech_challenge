using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using tech_challenge.Application.Services.Usuarios;
using tech_challenge.Application.Services.Usuarios.Model;
using tech_challenge.Domain.Common.Enums;

namespace tech_challenge.Teste.Application.Usuarios
{
    public class TokenServiceTeste
    {
        private readonly TokenService _service;

        public TokenServiceTeste()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Issuer"] = "TechChallenge",
                    ["Jwt:Audience"] = "TechChallenge",
                    ["Jwt:Secret"] = "MINHA_CHAVE_SECRETA_GRANDE_PARA_TESTES_123456",
                    ["Jwt:ExpirationMinutes"] = "60"
                })
                .Build();

            _service = new TokenService(configuration);
        }

        [Fact]
        public void GerarToken_DeveRetornarTokenValido()
        {
            // Arrange
            var usuario = new UsuarioModel
            {
                UniqueCode = Guid.NewGuid(),
                Nome = "Rafael Amaral",
                Login = "rafael@email.com",
                Perfil = PerfilUsuario.Cliente,
                Senha = "senha123"
            };

            // Act
            var token = _service.GerarToken(usuario);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(token));

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            Assert.Equal("TechChallenge", jwtToken.Issuer);
            Assert.Equal("TechChallenge", jwtToken.Audiences.First());

            Assert.Contains(jwtToken.Claims, c =>
                c.Type == ClaimTypes.NameIdentifier &&
                c.Value == usuario.UniqueCode.ToString());

            Assert.Contains(jwtToken.Claims, c =>
                c.Type == ClaimTypes.Name &&
                c.Value == usuario.Nome);

            Assert.Contains(jwtToken.Claims, c =>
                c.Type == ClaimTypes.Email &&
                c.Value == usuario.Login);

            Assert.Contains(jwtToken.Claims, c =>
                c.Type == ClaimTypes.Role &&
                c.Value == usuario.Perfil.ToString());
        }

        [Fact]
        public void GerarToken_DeveGerarTokenComDataDeExpiracao()
        {
            // Arrange
            var usuario = new UsuarioModel
            {
                UniqueCode = Guid.NewGuid(),
                Nome = "Rafael Amaral",
                Login = "rafael@email.com",
                Perfil = PerfilUsuario.Cliente,
                Senha = "senha123"
            };

            // Act
            var token = _service.GerarToken(usuario);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            Assert.True(jwtToken.ValidTo > DateTime.UtcNow);
            Assert.True(jwtToken.ValidTo <= DateTime.UtcNow.AddMinutes(61));
        }
    }
}
