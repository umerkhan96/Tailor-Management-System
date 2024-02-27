using TMS.Dtos;

namespace TMS.Business.Services
{
    public interface IOrdersService
    {
        Task<OrdersDto> CreateAsync(OrdersDto model);
        Task<OrdersDto> UpdateAsync(OrdersDto model);
        Task DeleteByIDAsync(int ID, int UserID);
        Task<OrdersDto> GetByIDAsync(int ID);
        Task MarkAsReady(int ID);
        Task MarkAsCollected(int ID);
        Task<int> GetTotalCountAsync();
        Task<int> GetTodayTakenCountAsync();
        Task<int> GetTodayReturnCountAsync();
        Task<List<OrdersDto>> GetCalanderOrdersDetail();
        List<OrdersDto> GetBalanceSheetData(int CustomerID = 0, int OrderID = 0, DateTime? dtFrom = null, DateTime? dtTo = null);
        Task<(int Total, List<OrdersDto> Data)> Paginate(int PageIndex = 0, int PageSize = 10, string searchTitle = "", string orderBy = "", string orderDir = "", bool? IsCollected = null, DateTime? dtFrom = null, DateTime? dtTo = null);
    }
}
