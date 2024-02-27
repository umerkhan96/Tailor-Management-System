using TMS.Dtos;

namespace TMS.Business.Services
{
    public interface ICustomerService
    {
        Task<CustomerDto> CreateAsync(CustomerDto customer);
        Task<CustomerDto> UpdateAsync(CustomerDto customer);
        Task<List<CustomerDto>> GetAllAsync();
        Task<CustomerDto> GetByIDAsync(int ID);
        Task<int> GetTotalCountAsync();
        Task DeleteByIDAsync(int ID, int UserID);
        Task RestoreByIDAsync(int ID, int UserID);
        Task<(int Total, List<CustomerDto> Data)> Paginate(int PageIndex = 0, int PageSize = 10, string searchTitle = "", string orderBy = "", string orderDir = "", bool? status = null);
    }
}
