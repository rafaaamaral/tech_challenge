using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using tech_challenge.Domain.Aggregates.PecaInsumos;
using tech_challenge.Infrastructure.Persistence.Configuration.Base;

namespace tech_challenge.Infrastructure.Persistence.Configuration
{
    public class PecaInsumoConfiguration : AuditConfiguration<PecaInsumo>
    {
        public override void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<PecaInsumo> builder)
        {
            base.Configure(builder);
            builder.ToTable("PecaInsumo");
            builder.Property(c => c.Nome).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Codigo).IsRequired().HasMaxLength(10);
            builder.Property(c => c.Descricao).IsRequired(false).HasMaxLength(500);
            builder.Property(c => c.Tipo).IsRequired().HasConversion<int>();
            builder.Property(c => c.PrecoUnitario).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(c => c.QuantidadeEstoque).IsRequired();
        }
    }
}
