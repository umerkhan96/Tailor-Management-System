using TMS.Auth.Dtos;
using TMS.Dtos;

namespace TMS.Auth.Services.Interfaces
{
    public interface IUserService
    {
        Task<(bool status, string msg, ApplicationUser data)> Login(string username, string password, bool isPersistant);
        Task<UserDto> GetUserByID(string ID);
        Task<List<UserDto>> GetUsersByRole(string Role);
        Task Logout();
        Task<UserDto> CreateUser(UserDto user);
        Task<UserDto> UpdateUser(UserDto user);
        Task<bool> ExistsByUsername(string Username, int Id);
        Task<bool> ExistsByEmail(string Email, int Id);
        Task DeleteUserByID(string ID);
        Task RestoreUserByID(string ID);
        Task<(bool status, string msg)> UpdatePassword(PasswordDto model);
        Task<(int Total, List<UserDto> Data)> GetStaffUsers(string searchTerm = "", int PageSize = 10, int PageNumber = 0, string Role = "", string OrderColumn = "FirstName", string OrderDir = "asc", bool? Status = null);
    }
}
