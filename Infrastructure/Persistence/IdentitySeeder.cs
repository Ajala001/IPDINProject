using App.Core.Entities;
using App.Infrastructure.Data;
using App.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace App.Infrastructure.Persistence
{
    public class IdentitySeeder(UserManager<User> userManager, RoleManager<Role> roleManager, IPDINDbContext dbContext)
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

            // Seed RegistrationType
            Level level = await dbContext.Levels.FirstOrDefaultAsync(l => l.Name == "Fellow");

            if (level == null)
            {
                level = new Level
                {
                    Id = Guid.NewGuid(),
                    Name = "Fellow",
                    Dues = 2000,
                    CreatedBy = "Admin",
                    CreatedOn = DateTime.Now
                };

                await dbContext.Levels.AddAsync(level);
                await dbContext.SaveChangesAsync();
            }



            // Seed users
            if (await _userManager.FindByNameAsync("admin@example.com") == null)
            {
                var admin = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Abdbaasit",
                    LastName = "Ajala",
                    UserName = "admin@example.com",
                    Email = "admin@example.com",
                    LevelId = level!.Id,
                    EmailConfirmed = true,
                    Gender = Core.Enums.Gender.Male,
                    DateOfBirth = DateTime.Now,
                    Country = "Nigeria",
                    StateOfResidence = "Ogun",
                    DriverLicenseNo = "0123HG",
                    YearIssued = 2024,
                    ExpiringDate = DateTime.Now,
                    YearsOfExperience = 5,
                    NameOfCurrentDrivingSchool = "IPDIN",
                    Level = level,
                    MembershipNumber = "ADM/2025/0001",
                    HasPaidDues = true,
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
