using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Auth.Dtos;

namespace TMS.Auth.Seeders
{
    public class SeederManager
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public SeederManager(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task SeedData()
        {
            await SeedRoles();
            await SeedUser();
        }

        private async Task SeedUser()
        {
            var usrName = _configuration.GetSection("SeederData:DefaultUsername").Value;
            var email = _configuration.GetSection("SeederData:DefaultEmail").Value;
            var pass = _configuration.GetSection("SeederData:DefaultPassword").Value;
            var fname = _configuration.GetSection("SeederData:DefaultFirstName").Value;
            var lname = _configuration.GetSection("SeederData:DefaultLastName").Value;
            var usr = await _userManager.FindByNameAsync(usrName);
            if (usr == null)
            {
                usr = new ApplicationUser()
                {
                    FirstName = fname,
                    LastName = lname,
                    Email = email,
                    UserName = usrName
                };
                await _userManager.CreateAsync(usr, pass);
                usr = await _userManager.FindByNameAsync(usrName);
                await _userManager.AddToRoleAsync(usr, _configuration.GetSection("SeederData:DefaultRoleAdmin").Value);
            }
        }

        private async Task SeedRoles()
        {
            List<string> lstRoles = new List<string>()
            {
                _configuration.GetSection("SeederData:DefaultRoleAdmin").Value,
                _configuration.GetSection("SeederData:DefaultRoleCutter").Value,
                _configuration.GetSection("SeederData:DefaultRoleTailor").Value,
                _configuration.GetSection("SeederData:DefaultRoleCustomer").Value,
                _configuration.GetSection("SeederData:DefaultRoleOther").Value
            };

            foreach (var x in lstRoles)
            {
                var role = await _roleManager.FindByNameAsync(x);
                if (role == null)
                {
                    await _roleManager.CreateAsync(new ApplicationRole(x));
                }
            }
        }

    }
}
