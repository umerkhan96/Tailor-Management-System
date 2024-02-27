using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TMS.Common.CustomAttributes;

namespace TMS.Dtos
{
    public class OrdersDto
    {
        public int Index { get; set; }
        public int Id { get; set; }
        [Required(ErrorMessage = "کسٹمر منتخب کریں۔")]
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAddress { get; set; }
        public string? CustomerMobile { get; set; }
        [Required(ErrorMessage = "آرڈر کی تاریخ منتخب کریں۔")]
        public DateTime OrderDate { get; set; }
        public string? OrderDateStr { get; set; }
        [Required(ErrorMessage = "واپسی کی تاریخ منتخب کریں۔")]
        public DateTime ReturnDate { get; set; }
        public string? ReturnDateStr { get; set; }
        public bool IsCollected { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool IsReady { get; set; }
        [Required(ErrorMessage = "کل رقم درج کریں۔")]
        public decimal TotalAmount { get; set; }
        [Required(ErrorMessage = "ادا شدہ رقم درج کریں۔")]
        public decimal? PaidAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        public bool IsSmsSent { get; set; }
        public bool IsForDownload { get; set; }

        public List<CustomerDto> Customers { get; set; }
        public List<UserDto> Tailors { get; set; }
        public List<UserDto> Cutters { get; set; }
        public OrderItemDto OrderItem { get; set; }
        //public List<OrderItemDto> OrderItems { get; set; }
    }

    public class OrderItemDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }

        [Required(ErrorMessage = "درزی کو منتخب کریں")]
        public int TailorId { get; set; }

        [Required(ErrorMessage = "کٹر کو منتخب کریں")]
        public int CutterId { get; set; }

        [Required(ErrorMessage = "لمبائی قمیض کی پیمائش درج کریں")]
        
        public string ShirtLengthSize { get; set; }

        [Required(ErrorMessage = "تیرا کی پیمائش درج کریں")]
        
        public string TeraSize { get; set; }

        [Required(ErrorMessage = "بازو کی پیمائش درج کریں")]
        
        public string ArmSize { get; set; }

        [Required(ErrorMessage = "گلا کی پیمائش درج کریں")]
        
        public string NeckSize { get; set; }

        [Required(ErrorMessage = "چھاتی کی پیمائش درج کریں")]
        
        public string ChestSize { get; set; }

        [Required(ErrorMessage = "کمر کی پیمائش درج کریں")]
        
        public string QamarSize { get; set; }

        [Required(ErrorMessage = "شلوار لمبائی کی پیمائش درج کریں")]
        
        public string PentLengthSize { get; set; }

        [Required(ErrorMessage = "شلوار گھیرہ کی پیمائش درج کریں")]
        
        public string PentSize { get; set; }

        [Required(ErrorMessage = "پائنچہ کی پیمائش درج کریں")]
        
        public string FeetSize { get; set; }

        [Required(ErrorMessage = "گھیرہ کی پیمائش درج کریں")]
        
        public string HipsSize { get; set; }

        [MaxLength(500, ErrorMessage = "زیادہ سے زیادہ 500 حروف کی اجازت ہے۔")]
        public string? OtherDetails { get; set; }

        [Required(ErrorMessage = "تفصیل درج کریں۔")]
        public string Description { get; set; }
        public string? TailorName { get; set; }
        public string? CutterName { get; set; }
        public bool IsCompleted { get; set; }

        [Required(ErrorMessage = "مقدار درج کریں۔")]
        public int Qty { get; set; }

        [Required(ErrorMessage = "شلوار پاکٹ کی تفصیلات درج کریں۔")]
        public string? ShalwarPocket { get; set; }

        [Required(ErrorMessage = "کالر نوک کی تفصیلات درج کریں۔")]
        public string? ColorNock { get; set; }

        [Required(ErrorMessage = "کالر بین کی تفصیلات درج کریں۔")]
        public string? ColorBan { get; set; }

        [Required(ErrorMessage = "کرتا کی تفصیلات درج کریں۔")]
        public string? Kurta { get; set; }

        [Required(ErrorMessage = "کف چوڑائی کی تفصیلات درج کریں۔")]
        public string? Cuff { get; set; }

        [Required(ErrorMessage = "سامنے پاکٹ کی تفصیلات درج کریں۔")]
        public string? FrontPocket { get; set; }

        [Required(ErrorMessage = "شرٹ کی تفصیلات درج کریں۔")]
        public string? Shirt { get; set; }

        [Required(ErrorMessage = "پٹی چوڑائی کی تفصیلات درج کریں۔")]
        public string? Patti { get; set; }

        [Required(ErrorMessage = "سائیڈ پاکٹ کی تفصیلات درج کریں۔")]
        public string? SidePocket { get; set; }
    }
}
