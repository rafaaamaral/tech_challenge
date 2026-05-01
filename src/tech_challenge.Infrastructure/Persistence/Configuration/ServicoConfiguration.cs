using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using tech_challenge.Domain.Aggregates.Servicos;
using tech_challenge.Infrastructure.Persistence.Configuration.Base;

namespace tech_challenge.Infrastructure.Persistence.Configuration
{
    public class ServicoConfiguration : AuditConfiguration<Servico>
    {
        public override void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Servico> builder)
        {
            base.Configure(builder);
            builder.ToTable("Servico");
            builder.Property(c => c.Nome).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Descricao).IsRequired(false).HasMaxLength(500);
            builder.Property(c => c.PrecoBase).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(c => c.TempoEstimado).IsRequired(false);
        }
    }
}
