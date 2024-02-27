using System;
using System.Collections.Generic;

namespace TMS.Data.Entities
{
    public partial class TmsOrder
    {
        public TmsOrder()
        {
            TmsOrderItems = new HashSet<TmsOrderItem>();
        }

        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public bool IsCollected { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool IsReady { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        public bool IsSmsSent { get; set; }

        public virtual TmsCustomer Customer { get; set; } = null!;
        public virtual ICollection<TmsOrderItem> TmsOrderItems { get; set; }
    }
}
