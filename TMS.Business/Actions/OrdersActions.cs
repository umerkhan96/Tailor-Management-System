using Microsoft.EntityFrameworkCore;
using System.Drawing.Imaging;
using System.IO;
using TMS.Business.Repository;
using TMS.Business.Services;
using TMS.Data.Entities;
using TMS.Dtos;

namespace TMS.Business.Actions
{
    internal class OrdersActions : Repository<TmsOrder>, IOrdersService
    {
        public OrdersActions(TmsDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<OrdersDto> CreateAsync(OrdersDto model)
        {
            var obj = new TmsOrder()
            {
                CustomerId = model.CustomerId,
                OrderDate = model.OrderDate,
                ReturnDate = model.ReturnDate,
                TotalAmount = model.TotalAmount,
                PaidAmount = model.PaidAmount ?? 0,
                IsSmsSent = model.IsSmsSent,
                CreatedBy = model.CreatedBy,
                CreatedDate = model.CreatedDate
            };
            if (model.OrderItem != null)
            {
                var x = model.OrderItem;
                List<TmsOrderItem> orderItems = new List<TmsOrderItem>();
                TmsOrderItem itm = new TmsOrderItem()
                {
                    TailorId = x.TailorId,
                    CutterId = x.CutterId,
                    ShirtLengthSize = x.ShirtLengthSize,
                    TeraSize = x.TeraSize,
                    ArmSize = x.ArmSize,
                    NeckSize = x.NeckSize,
                    ChestSize = x.ChestSize,
                    QamarSize = x.QamarSize,
                    PentLengthSize = x.PentLengthSize,
                    PentSize = x.PentSize,
                    FeetSize = x.FeetSize,
                    HipsSize = x.HipsSize,
                    OtherDetails = x.OtherDetails,
                    IsCompleted = x.IsCompleted,
                    Description = x.Description,
                    Qty = x.Qty,
                    ShalwarPocket = x.ShalwarPocket,
                    ColorNock = x.ColorNock,
                    ColorBan = x.ColorBan,
                    Kurta = x.Kurta,
                    Cuff = x.Cuff,
                    FrontPocket = x.FrontPocket,
                    Shirt = x.Shirt,
                    Patti = x.Patti,
                    SidePocket = x.SidePocket,
                };
                orderItems.Add(itm);

                obj.TmsOrderItems = orderItems;
            }
            await Add(obj);
            model.Id = obj.Id;
            return model;
        }
        public async Task DeleteByIDAsync(int ID, int UserID)
        {
            var usr = await GetAll(x => !x.IsDeleted && x.Id == ID).FirstOrDefaultAsync();
            if (usr != null)
            {
                usr.IsDeleted = true;
                usr.DeletedBy = UserID;
                usr.DeletedDate = DateTime.Now;
                await Save();
            }
        }

        public async Task<OrdersDto> GetByIDAsync(int ID)
        {
            var obj = await GetAll(x => !x.IsDeleted && x.Id == ID)
                .Select(x => new OrdersDto()
                {
                    Id = x.Id,
                    CustomerId = x.CustomerId,
                    CustomerName = x.Customer.FirstName + " " + x.Customer.LastName,
                    CustomerAddress = x.Customer.Address,
                    CustomerMobile = x.Customer.MobileNumber,
                    IsCollected = x.IsCollected,
                    IsDeleted = x.IsDeleted,
                    OrderDate = x.OrderDate,
                    IsReady = x.IsReady,
                    ReturnDate = x.ReturnDate,
                    TotalAmount = x.TotalAmount,
                    PaidAmount = x.PaidAmount,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    OrderItem = x.TmsOrderItems.Select(o => new OrderItemDto()
                    {
                        Id = o.Id,
                        OrderId = o.OrderId,
                        TailorId = o.TailorId,
                        CutterId = o.CutterId,
                        ShirtLengthSize = o.ShirtLengthSize ?? string.Empty,
                        TeraSize = o.TeraSize ?? string.Empty,
                        ArmSize = o.ArmSize ?? string.Empty,
                        NeckSize = o.NeckSize ?? string.Empty,
                        ChestSize = o.ChestSize ?? string.Empty,
                        QamarSize = o.QamarSize ?? string.Empty,
                        PentLengthSize = o.PentLengthSize ?? string.Empty,
                        PentSize = o.PentSize ?? string.Empty,
                        FeetSize = o.FeetSize ?? string.Empty,
                        HipsSize = o.HipsSize ?? string.Empty,
                        OtherDetails = o.OtherDetails,
                        Description = o.Description,
                        IsCompleted = o.IsCompleted,
                        TailorName = o.Tailor.FirstName + " " + o.Tailor.LastName,
                        CutterName = o.Cutter.FirstName + " " + o.Cutter.LastName,
                        Qty = o.Qty ?? 0,
                        ShalwarPocket = o.ShalwarPocket,
                        ColorNock = o.ColorNock,
                        ColorBan = o.ColorBan,
                        Kurta = o.Kurta,
                        Cuff = o.Cuff,
                        FrontPocket = o.FrontPocket,
                        Shirt = o.Shirt,
                        Patti = o.Patti,
                        SidePocket = o.SidePocket,

                    }).FirstOrDefault()
                }).FirstOrDefaultAsync();
            return obj;

        }

        public async Task<(int Total, List<OrdersDto> Data)> Paginate(int PageIndex = 0, int PageSize = 10, string searchTitle = "", string orderBy = "", string orderDir = "", bool? IsCollected = null, DateTime? dtFrom = null, DateTime? dtTo = null)
        {
            var data = GetAll();
            if (IsCollected.HasValue)
            {
                data = data.Where(x => x.IsCollected == IsCollected.Value);
            }

            if (!string.IsNullOrEmpty(searchTitle))
            {
                searchTitle = searchTitle.Trim().ToLower();
                data = data.Where(x =>
                    x.Customer.FirstName.ToLower().Contains(searchTitle)
                    || x.Customer.LastName.ToLower().Contains(searchTitle));
            }

            int index = PageSize * PageIndex;
            data = data.Skip(index).Take(PageSize);
            int total = await data.CountAsync();
            var model = await data.AsNoTracking().Select(x => new OrdersDto()
            {
                Id = x.Id,
                CustomerId = x.CustomerId,
                CustomerName = x.Customer.FirstName + " " + x.Customer.LastName,
                IsCollected = x.IsCollected,
                IsDeleted = x.IsDeleted,
                OrderDate = x.OrderDate,
                ReturnDate = x.ReturnDate,
                TotalAmount = x.TotalAmount,
                PaidAmount = x.PaidAmount,
                IsReady = x.IsReady,
                CreatedBy = x.CreatedBy,
                CreatedDate = x.CreatedDate
            }).OrderByDescending(x => x.OrderDate).ThenByDescending(x => x.ReturnDate).ToListAsync();
            model.ForEach(x =>
            {
                x.Index = ++index;
                x.OrderDateStr = x.OrderDate.ToString("dd MMM, yyyy");
                x.ReturnDateStr = x.ReturnDate.ToString("dd MMM, yyyy");
                if (x.PaidAmount.HasValue && x.PaidAmount.Value > 0)
                {
                    x.BalanceAmount = x.TotalAmount - x.PaidAmount.Value;
                }
                else
                {
                    x.BalanceAmount = x.TotalAmount;
                }
            });
            return (total, model);
        }

        public async Task MarkAsReady(int ID)
        {
            var order = await GetAll(x => x.Id == ID).FirstOrDefaultAsync();
            if (order != null)
            {
                order.IsReady = true;
                await Save();
            }
        }

        public async Task MarkAsCollected(int ID)
        {
            var order = await GetAll(x => x.Id == ID).FirstOrDefaultAsync();
            if (order != null)
            {
                order.IsCollected = true;
                await Save();
            }
        }

        public async Task<int> GetTotalCountAsync()
        {
            var data = await GetAll(x => !x.IsDeleted).CountAsync();
            return data;
        }

        public async Task<int> GetTodayTakenCountAsync()
        {
            var data = await GetAll(x => x.OrderDate.Date == DateTime.Now.Date).CountAsync();
            return data;
        }

        public async Task<int> GetTodayReturnCountAsync()
        {
            var data = await GetAll(x => x.ReturnDate.Date == DateTime.Now.Date).CountAsync();
            return data;
        }

        public async Task<List<OrdersDto>> GetCalanderOrdersDetail()
        {
            DateTime currentDate = DateTime.Now;
            DateTime firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            var data = await GetAll()
                .Where(o => o.ReturnDate >= firstDayOfMonth && o.ReturnDate <= lastDayOfMonth)
                .Select(x => new OrdersDto()
                {
                    Id = x.Id,
                    ReturnDate = x.ReturnDate,
                    IsCollected = x.IsCollected,
                }).ToListAsync();
            return data;
        }

        public async Task<OrdersDto> UpdateAsync(OrdersDto model)
        {

            var obj = await GetAll(x => x.Id == model.Id).Include(x => x.TmsOrderItems).FirstOrDefaultAsync();
            if (obj == null)
                return model;
            obj.OrderDate = model.OrderDate;
            obj.ReturnDate = model.ReturnDate;
            obj.TotalAmount = model.TotalAmount;
            obj.PaidAmount = model.PaidAmount ?? 0;
            obj.IsSmsSent = model.IsSmsSent;
            obj.UpdatedBy = model.UpdatedBy;
            obj.UpdatedDate = model.UpdatedDate;
            if (model.OrderItem != null)
            {
                var x = model.OrderItem;
                var itm = obj.TmsOrderItems.FirstOrDefault();
                if (itm == null)
                    return model;
                List<TmsOrderItem> orderItems = new List<TmsOrderItem>();
                itm.TailorId = x.TailorId;
                itm.CutterId = x.CutterId;
                itm.ShirtLengthSize = x.ShirtLengthSize;
                itm.TeraSize = x.TeraSize;
                itm.ArmSize = x.ArmSize;
                itm.NeckSize = x.NeckSize;
                itm.ChestSize = x.ChestSize;
                itm.QamarSize = x.QamarSize;
                itm.PentLengthSize = x.PentLengthSize;
                itm.PentSize = x.PentSize;
                itm.FeetSize = x.FeetSize;
                itm.HipsSize = x.HipsSize;
                itm.OtherDetails = x.OtherDetails;
                itm.IsCompleted = x.IsCompleted;
                itm.Description = x.Description;
                itm.Qty = x.Qty;
                itm.ShalwarPocket = x.ShalwarPocket;
                itm.ColorNock = x.ColorNock;
                itm.ColorBan = x.ColorBan;
                itm.Kurta = x.Kurta;
                itm.Cuff = x.Cuff;
                itm.FrontPocket = x.FrontPocket;
                itm.Shirt = x.Shirt;
                itm.Patti = x.Patti;
                itm.SidePocket = x.SidePocket;
                orderItems.Add(itm);
                obj.TmsOrderItems = orderItems;
            }
            await Change(obj);
            model.Id = obj.Id;
            return model;
        }

        public List<OrdersDto> GetBalanceSheetData(int CustomerID = 0, int OrderID = 0, DateTime? dtFrom = null, DateTime? dtTo = null)
        {
            var data = GetAll(x => !x.IsDeleted);
            if (CustomerID > 0)
                data = data.Where(x => x.CustomerId == CustomerID);
            if (OrderID > 0)
                data = data.Where(x => x.Id == OrderID);
            if (dtFrom.HasValue)
                data = data.Where(x => x.OrderDate.Date >= dtFrom.Value.Date);
            if (dtTo.HasValue)
                data = data.Where(x => x.OrderDate.Date <= dtTo.Value.Date);

            var orders = data.Select(x => new OrdersDto()
            {
                Id = x.Id,
                CustomerId = x.CustomerId,
                CustomerName = x.Customer.FirstName + " " + x.Customer.LastName,
                IsCollected = x.IsCollected,
                OrderDate = x.OrderDate,
                ReturnDate = x.ReturnDate,
                TotalAmount = x.TotalAmount,
                PaidAmount = x.PaidAmount,
                BalanceAmount = x.TotalAmount - x.BalanceAmount
            }).ToList();
            return orders;
        }
    }
}
