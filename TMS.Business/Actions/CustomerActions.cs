using Microsoft.EntityFrameworkCore;
using System.Net;
using TMS.Business.Repository;
using TMS.Business.Services;
using TMS.Data;
using TMS.Data.Entities;
using TMS.Dtos;

namespace TMS.Business.Actions
{
    internal class CustomerActions : Repository<TmsCustomer>, ICustomerService
    {
        public CustomerActions(TmsDbContext context) : base(context)
        {

        }

        public async Task<CustomerDto> CreateAsync(CustomerDto customer)
        {
            var obj = new TmsCustomer()
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Address = customer.Address,
                MobileNumber = customer.MobileNumber,
                CreatedBy = customer.CreatedBy,
                CreatedDate = customer.CreatedDate
            };
            await Add(obj);
            customer.Id = obj.Id;
            return customer;
        }

        public async Task DeleteByIDAsync(int ID, int UserID)
        {
            var usr = await GetAll(x => !x.IsDelete && x.Id == ID).FirstOrDefaultAsync();
            if (usr != null)
            {
                usr.IsDelete = true;
                usr.DeletedBy = UserID;
                usr.DeletedDate = DateTime.Now;
                await Save();
            }
        }

        public async Task<List<CustomerDto>> GetAllAsync()
        {
            var data = await GetAll(x => !x.IsDelete).Select(x => new CustomerDto()
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
            }).ToListAsync();
            return data;
        }
        
        public async Task<int> GetTotalCountAsync()
        {
            var data = await GetAll(x => !x.IsDelete).CountAsync();
            return data;
        }

        public async Task<CustomerDto> GetByIDAsync(int ID)
        {
            var obj = await GetAll(x => !x.IsDelete && x.Id == ID)
                .Select(x => new CustomerDto()
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Address = x.Address,
                    MobileNumber = x.MobileNumber
                })
                .FirstOrDefaultAsync();
            return obj;

        }

        public async Task<(int Total, List<CustomerDto> Data)> Paginate(int PageIndex = 0, int PageSize = 10, string searchTitle = "", string orderBy = "", string orderDir = "", bool? status = null)
        {
            var data = GetAll();
            if (status.HasValue)
            {
                data = data.Where(x => x.IsDelete == status.Value);
            }

            if (!string.IsNullOrEmpty(searchTitle))
            {
                searchTitle = searchTitle.Trim().ToLower();
                data = data.Where(x =>
                    x.FirstName.ToLower().Contains(searchTitle)
                    || x.LastName.ToLower().Contains(searchTitle)
                    || (!string.IsNullOrEmpty(x.Address) && x.Address.ToLower().Contains(searchTitle)));
            }

            switch (orderBy)
            {
                case "firstName":
                    data = orderDir == "asc" ? data.OrderBy(x => x.FirstName) : data.OrderByDescending(x => x.FirstName);
                    break;
                case "lastName":
                    data = orderDir == "asc" ? data.OrderBy(x => x.LastName) : data.OrderByDescending(x => x.LastName);
                    break;
                case "address":
                    data = orderDir == "asc" ? data.OrderBy(x => x.Address) : data.OrderByDescending(x => x.Address);
                    break;
            }
            int index = PageSize * PageIndex;
            data = data.Skip(index).Take(PageSize);
            int total = await data.CountAsync();
            var model = await data.AsNoTracking().Select(x => new CustomerDto()
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Address = x.Address,
                MobileNumber = x.MobileNumber,
                IsDelete = x.IsDelete
            }).ToListAsync();
            model.ForEach(x => x.Index = ++index);
            return (total, model);
        }

        public async Task RestoreByIDAsync(int ID, int UserID)
        {
            var usr = await GetAll(x => x.IsDelete && x.Id == ID).FirstOrDefaultAsync();
            if (usr != null)
            {
                usr.IsDelete = false;
                usr.DeletedBy = null;
                usr.DeletedDate = null;
                await Save();
            }
        }

        public async Task<CustomerDto> UpdateAsync(CustomerDto customer)
        {
            var obj = await GetAll(x => x.Id == customer.Id).FirstOrDefaultAsync();
            if (obj != null)
            {
                obj.FirstName = customer.FirstName;
                obj.LastName = customer.LastName;
                obj.Address = customer.Address;
                obj.MobileNumber = customer.MobileNumber;
                obj.UpdatedBy = customer.UpdatedBy;
                obj.UpdatedDate = customer.UpdatedDate;
                await Save();
            }
            return customer;
        }
    }
}
