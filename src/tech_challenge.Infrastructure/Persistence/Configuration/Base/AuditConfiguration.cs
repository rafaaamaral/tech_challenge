using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using tech_challenge.Domain.Common.Entities;

namespace tech_challenge.Infrastructure.Persistence.Configuration.Base
{
    public abstract class AuditConfiguration<T> : IEntityTypeConfiguration<T> where T : Audit
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(c => c.Id).IsRequired().UseIdentityAlwaysColumn();
            builder.Property(c => c.UniqueCode).IsRequired();
            builder.Property(c => c.Ativo).IsRequired();
            builder.Property(c => c.CriadoPor).IsRequired();
            builder.Property(c => c.DataCriacao).IsRequired();
            builder.Property(c => c.AlteradoPor);
            builder.Property(c => c.DataAlteracao);
        }
    }
}
