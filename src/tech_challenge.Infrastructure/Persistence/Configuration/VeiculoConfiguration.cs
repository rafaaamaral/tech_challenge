using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using tech_challenge.Domain.Aggregates.Clientes;
using tech_challenge.Domain.Aggregates.Veiculos;
using tech_challenge.Infrastructure.Persistence.Configuration.Base;

namespace tech_challenge.Infrastructure.Persistence.Configuration
{
    public class VeiculoConfiguration : AuditConfiguration<Veiculo>
    {
        public override void Configure(EntityTypeBuilder<Veiculo> builder)
        {
            base.Configure(builder);

            builder.ToTable("Veiculo");
            builder.Property(c => c.Placa).IsRequired().HasMaxLength(10);
            builder.Property(c => c.Marca).IsRequired().HasMaxLength(50);
            builder.Property(c => c.Modelo).IsRequired().HasMaxLength(50);
            builder.Property(c => c.Ano).IsRequired(false);

            builder.HasOne<Cliente>()
                   .WithMany()
                   .HasForeignKey(c => c.ClienteId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
