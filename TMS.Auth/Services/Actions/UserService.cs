using TMS.Auth.Dtos;
using TMS.Auth.Repositories.Interfaces;
using TMS.Auth.Services.Interfaces;
using TMS.Dtos;

namespace TMS.Auth.Services.Actions
{
    internal class UserService : IUserService
    {
        private readonly IUserRepo _userRepo;

        public UserService(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<UserDto> CreateUser(UserDto user)
        {
            return await _userRepo.CreateUser(user);
        }

        public async Task<UserDto> UpdateUser(UserDto user)
        {
            return await _userRepo.UpdateUser(user);
        }

        public async Task<bool> ExistsByEmail(string Email, int Id)
        {
            return await _userRepo.ExistsByEmail(Email, Id);
        }

        public async Task<bool> ExistsByUsername(string Username, int Id)
        {
            return await _userRepo.ExistsByUsername(Username, Id);
        }

        public async Task<UserDto> GetUserByID(string ID)
        {
            return await _userRepo.GetUserByID(ID);
        }

        public async Task<(bool status, string msg, ApplicationUser data)> Login(string username, string password, bool isPersistant)
        {
            return await _userRepo.Login(username, password, isPersistant);
        }

        public async Task Logout()
        {
            await _userRepo.Logout();
        }

        public async Task<(int Total, List<UserDto> Data)> GetStaffUsers(string searchTerm = "", int PageSize = 10, int PageNumber = 0, string Role = "", string OrderColumn = "FirstName", string OrderDir = "asc", bool? Status = null)
        {
            return await _userRepo.GetStaffUsers(searchTerm, PageSize, PageNumber, Role, OrderColumn, OrderDir, Status);
        }

        public async Task DeleteUserByID(string ID)
        {
            await _userRepo.DeleteUserByID(ID);
        }

        public async Task RestoreUserByID(string ID)
        {
            await _userRepo.RestoreUserByID(ID);
        }

        public async Task<List<UserDto>> GetUsersByRole(string Role)
        {
            return await _userRepo.GetUsersByRole(Role);
        }

        public async Task<(bool status, string msg)> UpdatePassword(PasswordDto model)
        {
            return await _userRepo.UpdatePassword(model);
        }
    }
}
