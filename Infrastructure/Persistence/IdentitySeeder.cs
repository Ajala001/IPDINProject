using App.Core.Entities;
using App.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
namespace App.Infrastructure.Persistence
{
    public class IdentitySeeder(UserManager<User> userManager, RoleManager<Role> roleManager)
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly RoleManager<Role> _roleManager = roleManager;

        public async Task SeedAsync()
        {
            // Seed roles
            List<Role> roles =
            [ //Collection initializer
                new Role
                {
                    Id = Guid.NewGuid(),
                    Name = "Admin",
                    CreatedBy = "Admin",
                    CreatedOn = DateTime.Now,
                },

                new Role
                {
                    Id = Guid.NewGuid(),
                    Name = "Member",
                    CreatedBy = "Admin",
                    CreatedOn = DateTime.Now,
                },
            ];

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role.Name))
                {
                    var result = await _roleManager.CreateAsync(role);
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to create role {role}");
                    }
                }
            }

            // Seed users
            if (await _userManager.FindByNameAsync("admin@gmail.com") == null)
            {
                var admin = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Abdbaasit",
                    LastName = "Ajala",
                    UserName = "admin@example.com",
                    Email = "admin@example.com",
                    EmailConfirmed = true,
                    Gender = Core.Enums.Gender.Male,
                    DateOfBirth = DateTime.Now,
                    CreatedOn = DateTime.Now,
                    CreatedBy = "Admin"
                };
                var result = await _userManager.CreateAsync(admin, "Admin@123");

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }
    }
}
