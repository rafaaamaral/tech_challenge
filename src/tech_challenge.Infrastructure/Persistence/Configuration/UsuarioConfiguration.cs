using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using tech_challenge.Domain.Aggregates.Usuarios;
using tech_challenge.Infrastructure.Persistence.Configuration.Base;

namespace tech_challenge.Infrastructure.Persistence.Configuration
{
    public class UsuarioConfiguration : AuditConfiguration<Usuario>
    {
        public override void Configure(EntityTypeBuilder<Usuario> builder)
        {
            base.Configure(builder);

            builder.ToTable("Usuario");

            builder.Property(c => c.Nome).IsRequired();
            builder.Property(c => c.Login).IsRequired();
            builder.Property(c => c.Senha).IsRequired();
            builder.Property(c => c.Perfil).IsRequired().HasConversion<int>();
        }
    }
}
