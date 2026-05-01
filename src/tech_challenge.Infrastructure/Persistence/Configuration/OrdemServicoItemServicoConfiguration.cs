using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using tech_challenge.Domain.Aggregates.OrdemServicos;
using tech_challenge.Domain.Aggregates.Servicos;
using tech_challenge.Infrastructure.Persistence.Configuration.Base;

namespace tech_challenge.Infrastructure.Persistence.Configuration
{
    public class OrdemServicoItemServicoConfiguration : AuditConfiguration<OrdemServicoItemServico>
    {
        public override void Configure(EntityTypeBuilder<OrdemServicoItemServico> builder)
        {
            base.Configure(builder);
            builder.ToTable("OrdemServicoItemServico");
            builder.Property(x => x.ServicoId).IsRequired();
            builder.Property(x => x.OrdemServicoId).IsRequired();
            builder.Property(x => x.Descricao).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Valor).IsRequired().HasColumnType("decimal(18,2)");

            builder.HasOne<OrdemServico>()
                .WithMany(o => o.ItensServicos)
                .HasForeignKey(i => i.OrdemServicoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Servico>()
                .WithMany()
                .HasForeignKey(i => i.ServicoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
