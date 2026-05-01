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

            builder.Property(c => c.Nome).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Login).IsRequired().HasMaxLength(50);
            builder.Property(c => c.Senha).IsRequired().HasMaxLength(200);
            builder.Property(c => c.Perfil).IsRequired().HasConversion<int>();
        }
    }
}
