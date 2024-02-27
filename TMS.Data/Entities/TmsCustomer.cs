using System;
using System.Collections.Generic;

namespace TMS.Data.Entities
{
    public partial class TmsCustomer
    {
        public TmsCustomer()
        {
            TmsOrders = new HashSet<TmsOrder>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string MobileNumber { get; set; } = null!;
        public string? Address { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDelete { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }

        public virtual ICollection<TmsOrder> TmsOrders { get; set; }
    }
}
