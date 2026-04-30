using System;
using System.Collections.Generic;
using System.Text;

namespace tech_challenge.Domain.Common.Entities
{
    public class Audit : BaseDomain
    {
        public bool IsActive { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
