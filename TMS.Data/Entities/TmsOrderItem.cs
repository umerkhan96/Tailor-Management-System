using System;
using System.Collections.Generic;

namespace TMS.Data.Entities
{
    public partial class TmsOrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int TailorId { get; set; }
        public int CutterId { get; set; }
        public string? ShirtLengthSize { get; set; }
        public string? TeraSize { get; set; }
        public string? ArmSize { get; set; }
        public string? NeckSize { get; set; }
        public string? ChestSize { get; set; }
        public string? QamarSize { get; set; }
        public string? PentLengthSize { get; set; }
        public string? PentSize { get; set; }
        public string? FeetSize { get; set; }
        public string? HipsSize { get; set; }
        public string? OtherDetails { get; set; }
        public bool IsCompleted { get; set; }
        public string? Description { get; set; }
        public int? Qty { get; set; }
        public string? ShalwarPocket { get; set; }
        public string? ColorNock { get; set; }
        public string? ColorBan { get; set; }
        public string? Kurta { get; set; }
        public string? Cuff { get; set; }
        public string? FrontPocket { get; set; }
        public string? Shirt { get; set; }
        public string? Patti { get; set; }
        public string? SidePocket { get; set; }

        public virtual AspNetUser Cutter { get; set; } = null!;
        public virtual TmsOrder Order { get; set; } = null!;
        public virtual AspNetUser Tailor { get; set; } = null!;
    }
}
