using DigitalMenu_20_BLL.Enums;
using DigitalMenu_20_BLL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace DigitalMenu_30_DAL.Data;

public class SeedData(ApplicationDbContext dbContext)
{
    public async Task ResetDatabaseAndSeed()
    {
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.MigrateAsync();
        dbContext.ChangeTracker.Clear();

        try
        {
            await SeedUsers();
            await SeedRoles();
            await SeedMenuItems();
            await SeedIngredients();
            await SeedIngredientsToMenuItem();
            await SeedCategories();
            await SeedTables();
            await SeedCartItems();
            await SeedOrders();

            await dbContext.SaveChangesAsync();
        }
        catch (MySqlException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private async Task SeedOrders()
    {
        dbContext.Orders.AddRange(
            new Order
            {
                Id = "12d7eaff-5f3c-456d-92c4-7de2220b2d05",
                DeviceId = "90FC58F8-88A0-49A1-A7B5-217A54F8191A",
                TableId = "69AC2F65-5DE9-40D4-B930-624CA40D3F13",
                TotalAmount = 6400,
                PaymentStatus = PaymentStatus.Paid,
                ExternalPaymentId = "tr_294TYYppc4",
                OrderDate = DateTime.Parse("2024-04-09 14:54:02"),
                OrderNumber = "092404jnWK",
            }
        );
        await dbContext.SaveChangesAsync();

        dbContext.OrderMenuItems.AddRange(
            new OrderMenuItem
            {
                OrderId = "12d7eaff-5f3c-456d-92c4-7de2220b2d05",
                MenuItemId = 1,
                Quantity = 2,
            },
            new OrderMenuItem
            {
                OrderId = "12d7eaff-5f3c-456d-92c4-7de2220b2d05",
                MenuItemId = 2,
                Quantity = 1,
            },
            new OrderMenuItem
            {
                OrderId = "12d7eaff-5f3c-456d-92c4-7de2220b2d05",
                MenuItemId = 3,
                Quantity = 2,
            },
            new OrderMenuItem
            {
                OrderId = "12d7eaff-5f3c-456d-92c4-7de2220b2d05",
                MenuItemId = 4,
                Quantity = 1,
            }
        );
        await dbContext.SaveChangesAsync();
    }

    private async Task SeedCartItems()
    {
        dbContext.CartItems.AddRange(
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
                Note = "No ice and no sugar",
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
        await dbContext.SaveChangesAsync();
    }

    private async Task SeedUsers()
    {
        dbContext.Users.AddRange(
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
        await dbContext.SaveChangesAsync();
    }

    private async Task SeedRoles()
    {
        dbContext.Roles.AddRange(
            new IdentityRole
            {
                Id = "8977148E-C765-410F-9A58-0C7D054E4536", Name = "Admin", NormalizedName = "ADMIN",
            },
            new IdentityRole
            {
                Id = "81659B09-5665-4E61-ACB9-5C43E28BE6A4", Name = "Employee", NormalizedName = "EMPLOYEE",
            }
        );
        await dbContext.SaveChangesAsync();

        dbContext.UserRoles.Add(new IdentityUserRole<string>
        {
            UserId = "0206A018-5AC6-492D-AB99-10105193D384",
            RoleId = "8977148E-C765-410F-9A58-0C7D054E4536",
        });
        await dbContext.SaveChangesAsync();
    }

    private async Task SeedMenuItems()
    {
        dbContext.MenuItems.AddRange(
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
        await dbContext.SaveChangesAsync();
    }

    private async Task SeedIngredients()
    {
        dbContext.Ingredients.AddRange(
            new Ingredient { Id = 1, Name = "Beef Patty" },
            new Ingredient { Id = 2, Name = "Hamburger Bun" },
            new Ingredient { Id = 3, Name = "Lettuce" },
            new Ingredient { Id = 4, Name = "Tomato Slices" },
            new Ingredient { Id = 5, Name = "Onion Slices" },
            new Ingredient { Id = 6, Name = "Pickles" },
            new Ingredient { Id = 7, Name = "Cheese" },
            new Ingredient { Id = 8, Name = "Bacon" },
            new Ingredient { Id = 9, Name = "Ketchup" },
            new Ingredient { Id = 10, Name = "Mustard" },
            new Ingredient { Id = 11, Name = "Mayonnaise" },
            new Ingredient { Id = 12, Name = "Pizza Dough" },
            new Ingredient { Id = 13, Name = "Tomato Sauce" },
            new Ingredient { Id = 14, Name = "Mozzarella Cheese" },
            new Ingredient { Id = 15, Name = "Pepperoni" },
            new Ingredient { Id = 16, Name = "Mushrooms" },
            new Ingredient { Id = 17, Name = "Bell Peppers" },
            new Ingredient { Id = 18, Name = "Onions" },
            new Ingredient { Id = 19, Name = "Olives" },
            new Ingredient { Id = 20, Name = "Basil" },
            new Ingredient { Id = 21, Name = "Pasta" },
            new Ingredient { Id = 22, Name = "Garlic" },
            new Ingredient { Id = 23, Name = "Olive Oil" },
            new Ingredient { Id = 24, Name = "Parmesan Cheese" },
            new Ingredient { Id = 25, Name = "Potatoes" },
            new Ingredient { Id = 26, Name = "Salt" },
            new Ingredient { Id = 27, Name = "Oil" }
        );
        await dbContext.SaveChangesAsync();
    }

    private async Task SeedIngredientsToMenuItem()
    {
        dbContext.MenuItemIngredients.AddRange(
            new MenuItemIngredient { MenuItemId = 1, IngredientId = 1 },
            new MenuItemIngredient { MenuItemId = 1, IngredientId = 2 },
            new MenuItemIngredient { MenuItemId = 1, IngredientId = 3 },
            new MenuItemIngredient { MenuItemId = 1, IngredientId = 4 },
            new MenuItemIngredient { MenuItemId = 1, IngredientId = 5 },
            new MenuItemIngredient { MenuItemId = 1, IngredientId = 6 },
            new MenuItemIngredient { MenuItemId = 1, IngredientId = 7 },
            new MenuItemIngredient { MenuItemId = 1, IngredientId = 8 },
            new MenuItemIngredient { MenuItemId = 1, IngredientId = 9 },
            new MenuItemIngredient { MenuItemId = 1, IngredientId = 10 },
            new MenuItemIngredient { MenuItemId = 1, IngredientId = 11 },
            new MenuItemIngredient { MenuItemId = 2, IngredientId = 12 },
            new MenuItemIngredient { MenuItemId = 2, IngredientId = 13 },
            new MenuItemIngredient { MenuItemId = 2, IngredientId = 14 },
            new MenuItemIngredient { MenuItemId = 2, IngredientId = 15 },
            new MenuItemIngredient { MenuItemId = 2, IngredientId = 16 },
            new MenuItemIngredient { MenuItemId = 2, IngredientId = 17 },
            new MenuItemIngredient { MenuItemId = 2, IngredientId = 18 },
            new MenuItemIngredient { MenuItemId = 2, IngredientId = 19 },
            new MenuItemIngredient { MenuItemId = 2, IngredientId = 20 },
            new MenuItemIngredient { MenuItemId = 3, IngredientId = 21 },
            new MenuItemIngredient { MenuItemId = 3, IngredientId = 13 },
            new MenuItemIngredient { MenuItemId = 3, IngredientId = 22 },
            new MenuItemIngredient { MenuItemId = 3, IngredientId = 23 },
            new MenuItemIngredient { MenuItemId = 3, IngredientId = 20 },
            new MenuItemIngredient { MenuItemId = 3, IngredientId = 24 },
            new MenuItemIngredient { MenuItemId = 4, IngredientId = 25 },
            new MenuItemIngredient { MenuItemId = 4, IngredientId = 26 },
            new MenuItemIngredient { MenuItemId = 4, IngredientId = 27 }
        );
        await dbContext.SaveChangesAsync();
    }

    private async Task SeedCategories()
    {
        dbContext.Categories.AddRange(
            new Category { Id = 1, Name = "Italian" },
            new Category { Id = 2, Name = "American" },
            new Category { Id = 3, Name = "Drinks" }
        );
        await dbContext.SaveChangesAsync();

        List<int> ids1 = [2, 3];
        List<MenuItem> menuItems1 = dbContext.MenuItems.Where(mi => ids1.Contains(mi.Id)).ToList();
        dbContext.Categories.First(c => c.Id == 1).MenuItems = [..menuItems1];

        List<int> ids2 = [1, 4];
        List<MenuItem> menuItems2 = dbContext.MenuItems.Where(mi => ids2.Contains(mi.Id)).ToList();
        dbContext.Categories.First(c => c.Id == 2).MenuItems = [..menuItems2];

        List<int> ids3 = [5, 6, 7, 8, 9, 10];
        List<MenuItem> menuItems3 = dbContext.MenuItems.Where(mi => ids3.Contains(mi.Id)).ToList();
        dbContext.Categories.First(c => c.Id == 3).MenuItems = [..menuItems3];

        await dbContext.SaveChangesAsync();
    }

    private async Task SeedTables()
    {
        dbContext.Tables.AddRange(
            new Table
            {
                Id = "69AC2F65-5DE9-40D4-B930-624CA40D3F13",
                Name = "Table 1",
                QrCode = "n/a",
                CreatedAt = DateTime.Parse("2023-09-01 12:00:00"),
            },
            new Table
            {
                Id = "C158F7B2-3F05-4C9F-BC95-3246D20A8A45",
                Name = "Table 2",
                QrCode = "n/a",
                CreatedAt = DateTime.Parse("2023-09-01 12:00:00").AddMinutes(10),
            },
            new Table
            {
                Id = "46673854-0888-4309-AD33-71A306C1D026",
                Name = "Table 3",
                QrCode = "n/a",
                CreatedAt = DateTime.Parse("2023-09-01 12:00:00").AddMinutes(20),
            },
            new Table
            {
                Id = "493FAF89-7344-403C-8D89-C9DF5BDFCB0F",
                Name = "Table 4",
                QrCode = "n/a",
                CreatedAt = DateTime.Parse("2023-09-01 12:00:00").AddMinutes(30),
            },
            new Table
            {
                Id = "AE15A89C-0CF0-47DA-83F0-6E232B819BBD",
                Name = "Table 5",
                QrCode = "n/a",
                CreatedAt = DateTime.Parse("2023-09-01 12:00:00").AddMinutes(30),
            }
        );
        await dbContext.SaveChangesAsync();
    }
}