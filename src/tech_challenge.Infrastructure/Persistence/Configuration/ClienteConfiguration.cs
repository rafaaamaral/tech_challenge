using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using tech_challenge.Domain.Aggregates.Clientes;
using tech_challenge.Infrastructure.Persistence.Configuration.Base;

namespace tech_challenge.Infrastructure.Persistence.Configuration
{
    public class ClienteConfiguration : AuditConfiguration<Cliente>
    {
        public override void Configure(EntityTypeBuilder<Cliente> builder)
        {
            base.Configure(builder);

            builder.ToTable("Cliente");

            builder.Property(c => c.Nome).IsRequired().HasMaxLength(100);
            builder.OwnsOne(c => c.Documento, d =>
            {
                d.Property(p => p.Valor).IsRequired().HasMaxLength(20).HasColumnName("Documento");
            });
            builder.Property(c => c.Email).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Telefone).HasMaxLength(20);
        }
    }
}
