using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;

namespace DigitalMenu_30_DAL.Data;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using ApplicationDbContext _dbContext =
            new(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

        try
        {
            if (!_dbContext.Users.Any())
            {
                _dbContext.Users.AddRange(
                    new IdentityUser
                    {
                        Id = "0206A018-5AC6-492D-AB99-10105193D384",
                        Email = "rongenmylo@gmail.com",
                        NormalizedEmail = "rongenmylo@gmail.com",
                        UserName = "rongenmylo@gmail.com",
                        NormalizedUserName = "rongenmylo@gmail.com",
                        PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(null, "Password123!"),
                    }
                );
            }

            if (!_dbContext.UserRoles.Any())
            {
                IdentityRole roleAdmin = _dbContext.Roles.First(r => r.Name == "Admin");
                _dbContext.UserRoles.Add(new IdentityUserRole<string>
                {
                    UserId = "0206A018-5AC6-492D-AB99-10105193D384",
                    RoleId = roleAdmin.Id,
                });
            }

            _dbContext.SaveChanges();
        }
        catch (MySqlException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}