using CenterOfCeramic.Data;
using CenterOfCeramic.Interfaces;
using CenterOfCeramic.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CenterOfCeramic.Seeder
{
    public class DatabaseSeeder : IDatabaseSeeder
    {

        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DatabaseSeeder(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }

        public async Task InitializeAsync()
        {
            await AddAdministratorAsync();
            await AddBasicUserAsync();
            await _db.SaveChangesAsync();
        }

        private async Task AddAdministratorAsync()
        {
            await Task.Run(async () =>
            {
                //Check if Role Exists
                var adminRole = new IdentityRole(Constants.AdministratorRole);
                var adminRoleInDb = await _roleManager.FindByNameAsync(Constants.AdministratorRole);
                if (adminRoleInDb == null)
                {
                    await _roleManager.CreateAsync(adminRole);
                }
                //Check if User Exists
                var superUser = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "admin@admin.com",
                    UserName = "admin_admin",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    FirstName = "admin",
                    LastName = "admin",
                };
                var superUserInDb = await _userManager.FindByEmailAsync(superUser.Email);
                if (superUserInDb == null)
                {
                    await _userManager.CreateAsync(superUser, Constants.DefaultPassword);
                    var result = await _userManager.AddToRoleAsync(superUser, Constants.AdministratorRole);
                }
            });
        }

        private async Task AddBasicUserAsync()
        {
            await Task.Run(async () =>
            {
                //Check if Role Exists
                var basicRole = new IdentityRole(Constants.BasicRole);
                var basicRoleInDb = await _roleManager.FindByNameAsync(Constants.BasicRole);
                if (basicRoleInDb == null)
                {
                    await _roleManager.CreateAsync(basicRole);
                }
                //Check if User Exists
                var basicUser = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = "kovalkola2@gmail.com",
                    UserName = "Kolya_Koval",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    FirstName = "Kolya",
                    LastName = "Koval"
                };
                var basicUserInDb = await _userManager.FindByEmailAsync(basicUser.Email);
                if (basicUserInDb == null)
                {
                    await _userManager.CreateAsync(basicUser, Constants.DefaultPassword);
                    var result = await _userManager.AddToRoleAsync(basicUser, Constants.BasicRole);
                }
            });
        }
    }
}
