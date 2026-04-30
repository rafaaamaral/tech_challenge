using System;
using System.Collections.Generic;
using System.Text;

namespace tech_challenge.Domain.Common.Entities
{
    public class BaseDomain
    {
        public int Id { get; set; }
        public Guid UniqueCode { get; set; } = Guid.NewGuid();
    }
}
