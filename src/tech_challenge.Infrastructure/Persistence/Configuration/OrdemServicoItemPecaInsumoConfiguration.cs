using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using tech_challenge.Domain.Aggregates.OrdemServicos;
using tech_challenge.Domain.Aggregates.PecaInsumos;
using tech_challenge.Infrastructure.Persistence.Configuration.Base;

namespace tech_challenge.Infrastructure.Persistence.Configuration
{
    public class OrdemServicoItemPecaInsumoConfiguration : AuditConfiguration<OrdemServicoItemPecaInsumo>
    {
        public override void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<OrdemServicoItemPecaInsumo> builder)
        {
            base.Configure(builder);
            builder.ToTable("OrdemServicoItemPecaInsumo");
            builder.Property(x => x.PecaInsumoId).IsRequired();
            builder.Property(x => x.OrdemServicoId).IsRequired();
            builder.Property(x => x.Descricao).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Quantidade).IsRequired();
            builder.Property(x => x.ValorUnitario).IsRequired().HasColumnType("decimal(18,2)");
            builder.HasOne<OrdemServico>()
                .WithMany(o => o.ItensPecasInsumos)
                .HasForeignKey(i => i.OrdemServicoId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne<PecaInsumo>()
                .WithMany()
                .HasForeignKey(i => i.PecaInsumoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
