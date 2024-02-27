using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using TMS.Auth.Dtos;
using TMS.Auth.Repositories.Interfaces;
using TMS.Dtos;

namespace TMS.Auth.Repositories.Actions
{
    internal class UserRepo : IUserRepo
    {
        private readonly SignInManager<ApplicationUser> _signinManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _configuration;

        public UserRepo(SignInManager<ApplicationUser> signinManager, RoleManager<ApplicationRole> roleManager,
            IConfiguration configuration)
        {
            _signinManager = signinManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<UserDto> CreateUser(UserDto user)
        {
            ApplicationUser usr = new ApplicationUser()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.Phone,
                UserName = user.Username
            };

            var res = await _signinManager.UserManager.CreateAsync(usr, user.Password);
            if (res.Succeeded)
            {
                user.Id = user.Id;
                await _signinManager.UserManager.AddToRoleAsync(usr, user.Role);
            }
            return user;
        }

        public async Task<bool> ExistsByEmail(string email, int Id)
        {
            bool flag = false;
            var usr = await _signinManager.UserManager.FindByEmailAsync(email);
            if (usr != null && usr.Id != Id)
            {
                flag = true;
            }
            return flag;
        }

        public async Task<bool> ExistsByUsername(string Username, int Id)
        {
            bool flag = false;
            var usr = await _signinManager.UserManager.FindByNameAsync(Username);
            if (usr != null && usr.Id != Id)
            {
                flag = true;
            }
            return flag;
        }

        public async Task<UserDto> GetUserByID(string ID)
        {
            var usr = await _signinManager.UserManager.FindByIdAsync(ID);
            if (usr == null)
                return null;
            var data = MapUser(usr);
            var roles = await _signinManager.UserManager.GetRolesAsync(usr);
            data.Role = roles.FirstOrDefault() ?? "";
            return data;
        }
        public async Task DeleteUserByID(string ID)
        {
            var usr = await _signinManager.UserManager.FindByIdAsync(ID);
            if (usr != null)
            {
                usr.IsDeleted = true;
                await _signinManager.UserManager.UpdateAsync(usr);
            }
        }

        public async Task<(bool status, string msg, ApplicationUser data)> Login(string username, string password, bool isPersistant)
        {
            var usr = await _signinManager.UserManager.FindByNameAsync(username);
            if (usr == null)
            {
                return (status: false, msg: "User with username not found!", data: null);
            }
            if (usr.IsDeleted)
            {
                return (status: false, msg: "Your account is inactive!", data: null);
            }
            var res = await _signinManager.PasswordSignInAsync(usr, password, isPersistant, false);
            if (!res.Succeeded)
            {
                return (status: false, msg: "Invalid password!", data: null);
            }
            return (status: true, msg: "", data: usr);
        }

        public async Task Logout()
        {
            await _signinManager.SignOutAsync();
        }

        public async Task<UserDto> UpdateUser(UserDto user)
        {
            var usr = await _signinManager.UserManager.FindByIdAsync(user.Id.ToString());
            if (usr != null)
            {
                usr.FirstName = user.FirstName;
                usr.LastName = user.LastName;
                usr.Email = user.Email;
                usr.PhoneNumber = user.Phone;
                usr.UserName = user.Username;
                await _signinManager.UserManager.UpdateAsync(usr);
            }
            return user;
        }

        public async Task<(int Total, List<UserDto> Data)> GetStaffUsers(string searchTerm = "", int PageSize = 10, int PageNumber = 0, string Role = "", string OrderColumn = "FirstName", string OrderDir = "asc", bool? Status = null)
        {
            List<UserDto> data = new List<UserDto>();
            int total = 0;
            int Index = PageSize * PageNumber;
            //List<ApplicationUser> users = new List<ApplicationUser>();
            if (string.IsNullOrEmpty(Role))
            {

                var crol = _configuration.GetSection("SeederData:DefaultRoleCutter").Value;
                var trol = _configuration.GetSection("SeederData:DefaultRoleTailor").Value;
                var orol = _configuration.GetSection("SeederData:DefaultRoleOther").Value;
                var tailorUsers = await _signinManager.UserManager.GetUsersInRoleAsync(trol);
                var cutterUsers = await _signinManager.UserManager.GetUsersInRoleAsync(crol);
                var otherUsers = await _signinManager.UserManager.GetUsersInRoleAsync(orol);
                //users.AddRange(tailorUsers);
                //users.AddRange(cutterUsers);
                //users.AddRange(otherUsers);

                tailorUsers.ToList().ForEach(x =>
                {
                    var us = MapUser(x);
                    us.Index = ++Index;
                    us.Role = trol;
                    data.Add(us);
                });

                cutterUsers.ToList().ForEach(x =>
                {
                    var us = MapUser(x);
                    us.Index = ++Index;
                    us.Role = crol;
                    data.Add(us);
                });

                otherUsers.ToList().ForEach(x =>
                {
                    var us = MapUser(x);
                    us.Index = ++Index;
                    us.Role = orol;
                    data.Add(us);
                });
            }
            else
            {
                var rsers = await _signinManager.UserManager.GetUsersInRoleAsync(Role);
                rsers.ToList().ForEach(x =>
                {
                    var us = MapUser(x);
                    us.Index = ++Index;
                    us.Role = Role;
                    data.Add(us);
                });
                //users.AddRange(rsers);
            }

            if (Status.HasValue)
                data = data.Where(x => x.IsDeleted == Status.Value).ToList();

            total = data.Count;
            if (data.Count > 0 && !string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                data = data.Where(x => x.FirstName.ToLower().Contains(searchTerm)
                || x.LastName.ToLower().Contains(searchTerm)
                || x.Username.ToLower().Contains(searchTerm)
                || x.Email.ToLower().Contains(searchTerm)
                || x.Phone.ToLower().Contains(searchTerm)
                ).ToList();
            }

            switch (OrderColumn)
            {
                case "id":
                    data = OrderDir == "desc" ? data.OrderByDescending(x => x.Id).ToList() : data.OrderBy(x => x.Id).ToList();
                    break;
                case "firstName":
                    data = OrderDir == "desc" ? data.OrderByDescending(x => x.FirstName).ToList() : data.OrderBy(x => x.FirstName).ToList();
                    break;
                case "lastName":
                    data = OrderDir == "desc" ? data.OrderByDescending(x => x.LastName).ToList() : data.OrderBy(x => x.LastName).ToList();
                    break;
                case "email":
                    data = OrderDir == "desc" ? data.OrderByDescending(x => x.Email).ToList() : data.OrderBy(x => x.Email).ToList();
                    break;
            }

            data = data.Skip(PageSize * PageNumber).Take(PageSize).ToList();

            return (total, data);
        }

        public UserDto MapUser(ApplicationUser user)
        {
            return new UserDto()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.PhoneNumber,
                Username = user.UserName,
                IsDeleted = user.IsDeleted
            };
        }

        public async Task RestoreUserByID(string ID)
        {
            var usr = await _signinManager.UserManager.FindByIdAsync(ID);
            if (usr != null)
            {
                usr.IsDeleted = false;
                await _signinManager.UserManager.UpdateAsync(usr);
            }
        }

        public async Task<List<UserDto>> GetUsersByRole(string Role)
        {
            List<UserDto> users = new List<UserDto>();
            var rsers = await _signinManager.UserManager.GetUsersInRoleAsync(Role);
            rsers.ToList().ForEach(x =>
            {
                var us = MapUser(x);
                users.Add(us);
            });
            return users;
        }

        public async Task<(bool status, string msg)> UpdatePassword(PasswordDto model)
        {
            var usr = await _signinManager.UserManager.FindByIdAsync(model.Id.ToString());
            if (usr == null)
            {
                return (false, "Invalid user details!");
            }

            var rs = await _signinManager.UserManager.CheckPasswordAsync(usr, model.OldPassword);
            if (!rs)
                return (false, "Incorrect old password!");

            var res = await _signinManager.UserManager.ChangePasswordAsync(usr, model.OldPassword, model.Password);
            if (res.Succeeded)
                return (true, "Password updated successfully!");
            else
                return (false, res?.Errors?.FirstOrDefault()?.Description ?? "");
        }
    }
}
