using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using tech_challenge.Domain.Aggregates.Clientes;
using tech_challenge.Domain.Aggregates.OrdemServicos;
using tech_challenge.Domain.Aggregates.Veiculos;
using tech_challenge.Infrastructure.Persistence.Configuration.Base;

namespace tech_challenge.Infrastructure.Persistence.Configuration
{
    public class OrdemServicoConfiguration : AuditConfiguration<OrdemServico>
    {
        public override void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<OrdemServico> builder)
        {
            base.Configure(builder);
            builder.ToTable("OrdemServico");
            builder.Property(c => c.ClienteId).IsRequired();
            builder.Property(c => c.VeiculoId).IsRequired();
            builder.Property(c => c.Numero).IsRequired();
            builder.Property(c => c.Status).IsRequired().HasConversion<int>();

            builder.OwnsOne(o => o.Orcamento, x =>
            {
                x.Property(p => p.ValorServicos);
                x.Property(p => p.ValorPecasInsumos);
                x.Property(p => p.ValorTotal);
                x.Property(p => p.Status).HasColumnName("StatusOrcamento").HasConversion<int>();
            });

            builder.HasOne<Cliente>()
                .WithMany()
                .HasForeignKey(o => o.ClienteId);

            builder.HasOne<Veiculo>()
                .WithMany()
                .HasForeignKey(o => o.VeiculoId);
        }
    }
}
