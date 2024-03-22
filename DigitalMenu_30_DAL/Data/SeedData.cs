using DigitalMenu_20_BLL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace DigitalMenu_30_DAL.Data;

public class SeedData
{
    private readonly ApplicationDbContext _dbContext;

    public SeedData(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ResetDatabaseAndSeed()
    {
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.MigrateAsync();
        _dbContext.ChangeTracker.Clear();

        try
        {
            SeedUsers();
            SeedRoles();
            SeedMenuItems();
            await SeedCategories();
            SeedTables();
            SeedCartItems();

            await _dbContext.SaveChangesAsync();
        }
        catch (MySqlException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void SeedCartItems()
    {
        _dbContext.CartItems.AddRange(
            new CartItem
            {
                Id = 1,
                MenuItemId = 1,
                Quantity = 2,
                DeviceId = "90FC58F8-88A0-49A1-A7B5-217A54F8191A",
                Note = "No onions and no pickles",
            },
            new CartItem
            {
                Id = 2,
                MenuItemId = 2,
                Quantity = 1,
                DeviceId = "90FC58F8-88A0-49A1-A7B5-217A54F8191A",
            },
            new CartItem
            {
                Id = 3,
                MenuItemId = 3,
                Quantity = 3,
                DeviceId = "90FC58F8-88A0-49A1-A7B5-217A54F8191A",
            },
            new CartItem
            {
                Id = 4,
                MenuItemId = 4,
                Quantity = 1,
                DeviceId = "90FC58F8-88A0-49A1-A7B5-217A54F8191A",
                Note = "No salt and no ketchup",
            },
            new CartItem
            {
                Id = 5,
                MenuItemId = 5,
                Quantity = 2,
                DeviceId = "90FC58F8-88A0-49A1-A7B5-217A54F8191A",
            },
            new CartItem
            {
                Id = 6,
                MenuItemId = 6,
                Quantity = 1,
                DeviceId = "90FC58F8-88A0-49A1-A7B5-217A54F8191A",
            },
            new CartItem
            {
                Id = 7,
                MenuItemId = 7,
                Quantity = 1,
                DeviceId = "90FC58F8-88A0-49A1-A7B5-217A54F8191A",
            },
            new CartItem
            {
                Id = 8,
                MenuItemId = 8,
                Quantity = 1,
                DeviceId = "90FC58F8-88A0-49A1-A7B5-217A54F8191A",
            },
            new CartItem
            {
                Id = 9,
                MenuItemId = 9,
                Quantity = 1,
                DeviceId = "90FC58F8-88A0-49A1-A7B5-217A54F8191A",
                Note = "No ice and no sugar"
            },
            new CartItem
            {
                Id = 10,
                MenuItemId = 10,
                Quantity = 1,
                DeviceId = "90FC58F8-88A0-49A1-A7B5-217A54F8191A",
                Note = "No ice",
            }
        );
    }

    private void SeedUsers()
    {
        _dbContext.Users.AddRange(
            new IdentityUser
            {
                Id = "0206A018-5AC6-492D-AB99-10105193D384",
                Email = "admin@gmail.com",
                NormalizedEmail = "admin@gmail.com",
                UserName = "admin@gmail.com",
                NormalizedUserName = "admin@gmail.com",
                PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(null, "Password123!"),
            }
        );
    }

    private void SeedRoles()
    {
        _dbContext.Roles.AddRange(
            new IdentityRole
            {
                Id = "8977148E-C765-410F-9A58-0C7D054E4536", Name = "Admin", NormalizedName = "ADMIN",
            },
            new IdentityRole
            {
                Id = "81659B09-5665-4E61-ACB9-5C43E28BE6A4", Name = "Employee", NormalizedName = "EMPLOYEE",
            }
        );

        _dbContext.UserRoles.Add(new IdentityUserRole<string>
        {
            UserId = "0206A018-5AC6-492D-AB99-10105193D384",
            RoleId = "8977148E-C765-410F-9A58-0C7D054E4536",
        });
    }

    private void SeedMenuItems()
    {
        _dbContext.MenuItems.AddRange(
            new MenuItem
            {
                Id = 1,
                Name = "Burger",
                Description = "A delicious burger",
                Price = 1000,
                ImageUrl =
                    "https://www.outofhome-shops.nl/files/202202/dist/3d91e961ea0f0abc6ee29aabe8dddc10.jpg",
            },
            new MenuItem
            {
                Id = 2,
                Name = "Pizza",
                Description = "A delicious pizza",
                Price = 1500,
                ImageUrl =
                    "https://www.moulinex-me.com/medias/?context=bWFzdGVyfHJvb3R8MTQzNTExfGltYWdlL2pwZWd8aGNlL2hmZC8xNTk2ODYyNTc4NjkxMC5qcGd8MmYwYzQ4YTg0MTgzNmVjYTZkMWZkZWZmMDdlMWFlMjRhOGIxMTQ2MTZkNDk4ZDU3ZjlkNDk2MzMzNDA5OWY3OA",
            },
            new MenuItem
            {
                Id = 3,
                Name = "Pasta",
                Description = "A delicious pasta",
                Price = 1200,
                ImageUrl =
                    "https://www.culy.nl/wp-content/uploads/2023/09/3_pasta-all-assassina-recept-1024x683.jpg",
            },
            new MenuItem
            {
                Id = 4,
                Name = "Fries",
                Description = "A delicious fries",
                Price = 500,
                ImageUrl =
                    "https://www.inspiredtaste.net/wp-content/uploads/2023/09/Baked-French-Fries-Video.jpg",
            },
            new MenuItem
            {
                Id = 5,
                Name = "Coke",
                Description = "A delicious coke",
                Price = 200,
                ImageUrl =
                    "https://kentstreetcellars.com.au/cdn/shop/files/coke-can_7bf866c9-bffc-449d-a173-de324ac47905_1200x1200.png?v=1687840069",
            },
            new MenuItem
            {
                Id = 6,
                Name = "Pepsi",
                Description = "A delicious pepsi",
                Price = 200,
                ImageUrl =
                    "https://www.compliment.nl/wp-content/uploads/2018/08/Pepsi-Cola-regular-blik-24x-33cl_jpg.webp",
            },
            new MenuItem
            {
                Id = 7,
                Name = "Sprite",
                Description = "A delicious sprite",
                Price = 200,
                ImageUrl = "https://www.africaproducts.nl/cdn/shop/products/3836_700x.jpg?v=1613150968",
            },
            new MenuItem
            {
                Id = 8,
                Name = "Fanta",
                Description = "A delicious fanta",
                Price = 200,
                ImageUrl =
                    "https://www.frisdrankkoning.nl/wp-content/uploads/2023/01/1d0e3d578e436b391f9d6edf8524f094_Fanta_____Orange_1.jpg",
            },
            new MenuItem
            {
                Id = 9,
                Name = "7up",
                Description = "A delicious 7up",
                Price = 200,
                ImageUrl =
                    "https://goedkoopblikjes.nl/image/cache/catalog/Frisdrank/Blikje%20fris/Light/7_up_free_zero_sugar_blikjes_33cl_tray-800x800.jpg",
            },
            new MenuItem
            {
                Id = 10,
                Name = "Mountain Dew",
                Description = "A delicious mountain dew",
                Price = 200,
                ImageUrl =
                    "https://www.frisenzoetwaren.nl/wp-content/uploads/2023/07/Mountain-Dew-Citrus-Blast-24-x-330-ml-EU.jpg",
            }
        );
    }

    private async Task SeedCategories()
    {
        _dbContext.Categories.AddRange(
            new Category { Id = 1, Name = "Italian" },
            new Category { Id = 2, Name = "American" },
            new Category { Id = 3, Name = "Drinks" }
        );

        await _dbContext.SaveChangesAsync();

        List<int> ids1 = [2, 3];
        List<MenuItem> menuItems1 = _dbContext.MenuItems.Where(mi => ids1.Contains(mi.Id)).ToList();
        _dbContext.Categories.First(c => c.Id == 1).MenuItems = [..menuItems1];

        List<int> ids2 = [1, 4];
        List<MenuItem> menuItems2 = _dbContext.MenuItems.Where(mi => ids2.Contains(mi.Id)).ToList();
        _dbContext.Categories.First(c => c.Id == 2).MenuItems = [..menuItems2];

        List<int> ids3 = [5, 6, 7, 8, 9, 10];
        List<MenuItem> menuItems3 = _dbContext.MenuItems.Where(mi => ids3.Contains(mi.Id)).ToList();
        _dbContext.Categories.First(c => c.Id == 3).MenuItems = [..menuItems3];
    }

    private void SeedTables()
    {
        _dbContext.Tables.AddRange(
            new Table
            {
                Id = "69AC2F65-5DE9-40D4-B930-624CA40D3F13",
                Name = "Table 1",
                QrCode = "n/a",
                CreatedAt = DateTime.Now,
            },
            new Table
            {
                Id = "C158F7B2-3F05-4C9F-BC95-3246D20A8A45",
                Name = "Table 2",
                QrCode = "n/a",
                CreatedAt = DateTime.Now.AddMinutes(10),
            },
            new Table
            {
                Id = "46673854-0888-4309-AD33-71A306C1D026",
                Name = "Table 3",
                QrCode = "n/a",
                CreatedAt = DateTime.Now.AddMinutes(20),
            },
            new Table
            {
                Id = "493FAF89-7344-403C-8D89-C9DF5BDFCB0F",
                Name = "Table 4",
                QrCode = "n/a",
                CreatedAt = DateTime.Now.AddMinutes(30),
            },
            new Table
            {
                Id = "AE15A89C-0CF0-47DA-83F0-6E232B819BBD",
                Name = "Table 5",
                QrCode = "n/a",
                CreatedAt = DateTime.Now.AddMinutes(30),
            }
        );
    }
}