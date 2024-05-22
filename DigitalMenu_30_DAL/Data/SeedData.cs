using DigitalMenu_20_BLL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Newtonsoft.Json;

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
            await SeedMailTranslations();

            await dbContext.SaveChangesAsync();
        }
        catch (MySqlException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private async Task SeedMailTranslations()
    {
        dbContext.MailTranslations.AddRange(
            new MailTranslation
            {
                Type = "reservation-created",
                Body = JsonConvert.SerializeObject(new
                {
                    title = "Reservation Created Successfully",
                    salutation = "Dear Customer,",
                    instruction =
                        "Your reservation has been created successfully. Please use the following code when you scan the QR-Code:",
                    thankYou = "Thank you for choosing our service. We look forward to serving you.",
                    bestRegards = "Best Regards,",
                    companyName = "Digital Menu",
                }),
                Language = "en",
                Subject = "Reservation Created Successfully",
            }, new MailTranslation
            {
                Type = "reservation-created",
                Body = JsonConvert.SerializeObject(new
                {
                    title = "Reservering succesvol aangemaakt",
                    salutation = "Beste klant,",
                    instruction =
                        "Uw reservering is succesvol aangemaakt. Gebruik de volgende code wanneer u de QR-code scant:",
                    thankYou =
                        "Bedankt voor het kiezen van onze service. We kijken ernaar uit om u van dienst te zijn.",
                    bestRegards = "Met vriendelijke groet,",
                    companyName = "Digital Menu",
                }),
                Language = "nl",
                Subject = "Reservering succesvol aangemaakt",
            }, new MailTranslation
            {
                Type = "reservation-created",
                Body = JsonConvert.SerializeObject(new
                {
                    title = "Reservierung erfolgreich erstellt",
                    salutation = "Sehr geehrter Kunde,",
                    instruction =
                        "Ihre Reservierung wurde erfolgreich erstellt. Bitte verwenden Sie den folgenden Code, wenn Sie den QR-Code scannen:",
                    thankYou =
                        "Vielen Dank, dass Sie unseren Service gewählt haben. Wir freuen uns darauf, Sie zu bedienen.",
                    bestRegards = "Mit freundlichen Grüßen,",
                    companyName = "Digital Menu",
                }),
                Language = "de",
                Subject = "Reservierung erfolgreich erstellt",
            }, new MailTranslation
            {
                Type = "reservation-created",
                Body = JsonConvert.SerializeObject(new
                {
                    title = "예약이 성공적으로 완료되었습니다",
                    salutation = "고객님,",
                    instruction =
                        "예약이 성공적으로 완료되었습니다. QR 코드를 스캔할 때 다음 코드를 사용하세요:",
                    thankYou = "저희 서비스를 선택해 주셔서 감사합니다. 저희는 고객님을 모시게 되어 기쁩니다.",
                    bestRegards = "감사합니다,",
                    companyName = "디지털 메뉴",
                }),
                Language = "ko",
                Subject = "예약이 성공적으로 완료되었습니다",
            });

        await dbContext.SaveChangesAsync();
    }

    private async Task SeedOrders()
    {
        dbContext.Orders.AddRange(
            new Order
            {
                Id = "12d7eaff-5f3c-456d-92c4-7de2220b2d05",
                TableId = "69AC2F65-5DE9-40D4-B930-624CA40D3F13",
                SessionId = "0449DB90-66AF-4E17-8086-C1452270B52D",
                TotalAmount = 6400,
                OrderDate = DateTime.Parse("2024-04-09 14:54:02"),
                OrderNumber = "092404jnWK",
            },
            new Order
            {
                Id = "6befb25e-0207-4a54-82e8-2d29b8b616c0",
                TableId = "69AC2F65-5DE9-40D4-B930-624CA40D3F13",
                SessionId = "0449DB90-66AF-4E17-8086-C1452270B52D",
                TotalAmount = 2600,
                OrderDate = DateTime.Parse("2024-04-16 14:54:02"),
                OrderNumber = "162404QKAK",
            },
            new Order
            {
                Id = "897dbd82-dd96-476e-83a4-5d9ca59fc8d7",
                TableId = "69AC2F65-5DE9-40D4-B930-624CA40D3F13",
                SessionId = "CBF261E5-A710-4611-A423-87943EB5DC32",
                TotalAmount = 700,
                OrderDate = DateTime.Parse("2024-04-16 14:55:02"),
                OrderNumber = "162404gzjE",
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
            },
            new OrderMenuItem
            {
                OrderId = "6befb25e-0207-4a54-82e8-2d29b8b616c0",
                MenuItemId = 1,
                Quantity = 2,
            },
            new OrderMenuItem
            {
                OrderId = "6befb25e-0207-4a54-82e8-2d29b8b616c0",
                MenuItemId = 5,
                Quantity = 3,
            },
            new OrderMenuItem
            {
                OrderId = "897dbd82-dd96-476e-83a4-5d9ca59fc8d7",
                MenuItemId = 4,
                Quantity = 1,
            },
            new OrderMenuItem
            {
                OrderId = "897dbd82-dd96-476e-83a4-5d9ca59fc8d7",
                MenuItemId = 8,
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
                TableSessionId = "90FC58F8-88A0-49A1-A7B5-217A54F8191A",
                Note = "No onions and no pickles",
            },
            new CartItem
            {
                Id = 2,
                MenuItemId = 2,
                Quantity = 1,
                TableSessionId = "90FC58F8-88A0-49A1-A7B5-217A54F8191A",
            },
            new CartItem
            {
                Id = 3,
                MenuItemId = 3,
                Quantity = 3,
                TableSessionId = "90FC58F8-88A0-49A1-A7B5-217A54F8191A",
            },
            new CartItem
            {
                Id = 4,
                MenuItemId = 4,
                Quantity = 1,
                TableSessionId = "90FC58F8-88A0-49A1-A7B5-217A54F8191A",
                Note = "No salt and no ketchup",
            },
            new CartItem
            {
                Id = 5,
                MenuItemId = 5,
                Quantity = 2,
                TableSessionId = "90FC58F8-88A0-49A1-A7B5-217A54F8191A",
            },
            new CartItem
            {
                Id = 6,
                MenuItemId = 6,
                Quantity = 1,
                TableSessionId = "90FC58F8-88A0-49A1-A7B5-217A54F8191A",
            },
            new CartItem
            {
                Id = 7,
                MenuItemId = 7,
                Quantity = 1,
                TableSessionId = "90FC58F8-88A0-49A1-A7B5-217A54F8191A",
            },
            new CartItem
            {
                Id = 8,
                MenuItemId = 8,
                Quantity = 1,
                TableSessionId = "90FC58F8-88A0-49A1-A7B5-217A54F8191A",
            },
            new CartItem
            {
                Id = 9,
                MenuItemId = 9,
                Quantity = 1,
                TableSessionId = "90FC58F8-88A0-49A1-A7B5-217A54F8191A",
                Note = "No ice and no sugar",
            },
            new CartItem
            {
                Id = 10,
                MenuItemId = 10,
                Quantity = 1,
                TableSessionId = "90FC58F8-88A0-49A1-A7B5-217A54F8191A",
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
            },
            new IdentityUser
            {
                Id = "B8B80F1A-BC51-4246-8895-C33B83D0CA32",
                Email = "employee@gmail.com",
                NormalizedEmail = "employee@gmail.com",
                UserName = "employee@gmail.com",
                NormalizedUserName = "employee@gmail.com",
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

        dbContext.UserRoles.AddRange(new IdentityUserRole<string>
            {
                UserId = "0206A018-5AC6-492D-AB99-10105193D384",
                RoleId = "8977148E-C765-410F-9A58-0C7D054E4536",
            }, new IdentityUserRole<string>
            {
                UserId = "B8B80F1A-BC51-4246-8895-C33B83D0CA32",
                RoleId = "81659B09-5665-4E61-ACB9-5C43E28BE6A4",
            }
        );
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
                ImageUrl = "https://www.outofhome-shops.nl/files/202202/dist/3d91e961ea0f0abc6ee29aabe8dddc10.jpg",
                Translations = new List<MenuItemTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Hamburger", Description = "Heerlijke hamburger" },
                    new() { LanguageCode = "de", Name = "Burger", Description = "Ein köstlicher Burger" },
                    new() { LanguageCode = "ko", Name = "버거", Description = "맛있는 버거" },
                },
            },
            new MenuItem
            {
                Id = 2,
                Name = "Pizza",
                Description = "A delicious pizza",
                Price = 1500,
                ImageUrl =
                    "https://www.moulinex-me.com/medias/?context=bWFzdGVyfHJvb3R8MTQzNTExfGltYWdlL2pwZWd8aGNlL2hmZC8xNTk2ODYyNTc4NjkxMC5qcGd8MmYwYzQ4YTg0MTgzNmVjYTZkMWZkZWZmMDdlMWFlMjRhOGIxMTQ2MTZkNDk4ZDU3ZjlkNDk2MzMzNDA5OWY3OA",
                Translations = new List<MenuItemTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Pizza", Description = "Heerlijke pizza" },
                    new() { LanguageCode = "de", Name = "Pizza", Description = "Eine köstliche Pizza" },
                    new() { LanguageCode = "ko", Name = "피자", Description = "맛있는 피자" },
                },
            },
            new MenuItem
            {
                Id = 3,
                Name = "Pasta",
                Description = "A delicious pasta",
                Price = 1200,
                ImageUrl = "https://www.culy.nl/wp-content/uploads/2023/09/3_pasta-all-assassina-recept-1024x683.jpg",
                Translations = new List<MenuItemTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Pasta", Description = "Heerlijke pasta" },
                    new() { LanguageCode = "de", Name = "Pasta", Description = "Eine köstliche Pasta" },
                    new() { LanguageCode = "ko", Name = "파스타", Description = "맛있는 파스타" },
                },
            },
            new MenuItem
            {
                Id = 4,
                Name = "Fries",
                Description = "A delicious fries",
                Price = 500,
                ImageUrl = "https://www.inspiredtaste.net/wp-content/uploads/2023/09/Baked-French-Fries-Video.jpg",
                Translations = new List<MenuItemTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Frietjes", Description = "Heerlijke frietjes" },
                    new() { LanguageCode = "de", Name = "Pommes", Description = "Köstliche Pommes" },
                    new() { LanguageCode = "ko", Name = "감자 튀김", Description = "맛있는 감자 튀김" },
                },
            },
            new MenuItem
            {
                Id = 5,
                Name = "Coke",
                Description = "A delicious coke",
                Price = 200,
                ImageUrl =
                    "https://kentstreetcellars.com.au/cdn/shop/files/coke-can_7bf866c9-bffc-449d-a173-de324ac47905_1200x1200.png?v=1687840069",
                Translations = new List<MenuItemTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Cola", Description = "Heerlijke cola" },
                    new() { LanguageCode = "de", Name = "Cola", Description = "Eine köstliche Cola" },
                    new() { LanguageCode = "ko", Name = "콜라", Description = "맛있는 콜라" },
                },
            },
            new MenuItem
            {
                Id = 6,
                Name = "Pepsi",
                Description = "A delicious pepsi",
                Price = 200,
                ImageUrl =
                    "https://www.compliment.nl/wp-content/uploads/2018/08/Pepsi-Cola-regular-blik-24x-33cl_jpg.webp",
                Translations = new List<MenuItemTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Pepsi", Description = "Heerlijke pepsi" },
                    new() { LanguageCode = "de", Name = "Pepsi", Description = "Ein köstliches Pepsi" },
                    new() { LanguageCode = "ko", Name = "펩시", Description = "맛있는 펩시" },
                },
            },
            new MenuItem
            {
                Id = 7,
                Name = "Sprite",
                Description = "A delicious sprite",
                Price = 200,
                ImageUrl = "https://www.africaproducts.nl/cdn/shop/products/3836_700x.jpg?v=1613150968",
                Translations = new List<MenuItemTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Sprite", Description = "Heerlijke sprite" },
                    new() { LanguageCode = "de", Name = "Sprite", Description = "Ein köstliches Sprite" },
                    new() { LanguageCode = "ko", Name = "스프라이트", Description = "맛있는 스프라이트" },
                },
            },
            new MenuItem
            {
                Id = 8,
                Name = "Fanta",
                Description = "A delicious fanta",
                Price = 200,
                ImageUrl =
                    "https://www.frisdrankkoning.nl/wp-content/uploads/2023/01/1d0e3d578e436b391f9d6edf8524f094_Fanta_____Orange_1.jpg",
                Translations = new List<MenuItemTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Fanta", Description = "Heerlijke fanta" },
                    new() { LanguageCode = "de", Name = "Fanta", Description = "Eine köstliche Fanta" },
                    new() { LanguageCode = "ko", Name = "환타", Description = "맛있는 환타" },
                },
            },
            new MenuItem
            {
                Id = 9,
                Name = "7up",
                Description = "A delicious 7up",
                Price = 200,
                ImageUrl =
                    "https://goedkoopblikjes.nl/image/cache/catalog/Frisdrank/Blikje%20fris/Light/7_up_free_zero_sugar_blikjes_33cl_tray-800x800.jpg",
                Translations = new List<MenuItemTranslation>
                {
                    new() { LanguageCode = "nl", Name = "7up", Description = "Heerlijke 7up" },
                    new() { LanguageCode = "de", Name = "7up", Description = "Ein köstliches 7up" },
                    new() { LanguageCode = "ko", Name = "세븐업", Description = "맛있는 세븐업" },
                },
            },
            new MenuItem
            {
                Id = 10,
                Name = "Mountain Dew",
                Description = "A delicious mountain dew",
                Price = 200,
                ImageUrl =
                    "https://www.frisenzoetwaren.nl/wp-content/uploads/2023/07/Mountain-Dew-Citrus-Blast-24-x-330-ml-EU.jpg",
                Translations = new List<MenuItemTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Mountain Dew", Description = "Heerlijke Mountain Dew" },
                    new() { LanguageCode = "de", Name = "Mountain Dew", Description = "Ein köstlicher Mountain Dew" },
                    new() { LanguageCode = "ko", Name = "마운틴듀", Description = "맛있는 마운틴듀" },
                },
            }
        );
        await dbContext.SaveChangesAsync();
    }

    private async Task SeedIngredients()
    {
        dbContext.Ingredients.AddRange(
            new Ingredient
            {
                Id = 1, Name = "Beef Patty", Stock = 16, Translations = new List<IngredientTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Rundergehakt" },
                    new() { LanguageCode = "de", Name = "Rindfleischfrikadelle" },
                    new() { LanguageCode = "ko", Name = "소고기 패티" },
                },
            },
            new Ingredient
            {
                Id = 2, Name = "Hamburger Bun", Stock = 8, Translations = new List<IngredientTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Hamburgerbroodje" },
                    new() { LanguageCode = "de", Name = "Hamburgerbrötchen" },
                    new() { LanguageCode = "ko", Name = "햄버거 번" },
                },
            },
            new Ingredient
            {
                Id = 3, Name = "Lettuce", Stock = 20, Translations = new List<IngredientTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Sla" },
                    new() { LanguageCode = "de", Name = "Salat" },
                    new() { LanguageCode = "ko", Name = "상추" },
                },
            },
            new Ingredient
            {
                Id = 4, Name = "Tomato Slices", Stock = 26, Translations = new List<IngredientTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Tomaat" },
                    new() { LanguageCode = "de", Name = "Tomatenscheiben" },
                    new() { LanguageCode = "ko", Name = "토마토 슬라이스" },
                },
            },
            new Ingredient
            {
                Id = 5, Name = "Onion Slices", Stock = 9, Translations = new List<IngredientTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Ui" },
                    new() { LanguageCode = "de", Name = "Zwiebelscheiben" },
                    new() { LanguageCode = "ko", Name = "양파 슬라이스" },
                },
            },
            new Ingredient
            {
                Id = 6, Name = "Pickles", Stock = 20, Translations = new List<IngredientTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Augurken" },
                    new() { LanguageCode = "de", Name = "Essiggurken" },
                    new() { LanguageCode = "ko", Name = "피클" },
                },
            },
            new Ingredient
            {
                Id = 7, Name = "Cheese", Stock = 10, Translations = new List<IngredientTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Kaas" },
                    new() { LanguageCode = "de", Name = "Käse" },
                    new() { LanguageCode = "ko", Name = "치즈" },
                },
            },
            new Ingredient
            {
                Id = 8, Name = "Bacon", Stock = 10, Translations = new List<IngredientTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Spek" },
                    new() { LanguageCode = "de", Name = "Speck" },
                    new() { LanguageCode = "ko", Name = "베이컨" },
                },
            },
            new Ingredient
            {
                Id = 9, Name = "Ketchup", Stock = 10, Translations = new List<IngredientTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Ketchup" },
                    new() { LanguageCode = "de", Name = "Ketchup" },
                    new() { LanguageCode = "ko", Name = "케첩" },
                },
            },
            new Ingredient
            {
                Id = 10, Name = "Mustard", Stock = 18, Translations = new List<IngredientTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Mosterd" },
                    new() { LanguageCode = "de", Name = "Senf" },
                    new() { LanguageCode = "ko", Name = "머스타드" },
                },
            },
            new Ingredient
            {
                Id = 11, Name = "Mayonnaise", Stock = 4, Translations = new List<IngredientTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Mayonaise" },
                    new() { LanguageCode = "de", Name = "Mayonnaise" },
                    new() { LanguageCode = "ko", Name = "마요네즈" },
                },
            },
            new Ingredient
            {
                Id = 12, Name = "Pizza Dough", Stock = 14, Translations = new List<IngredientTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Pizzadeeg" },
                    new() { LanguageCode = "de", Name = "Pizzateig" },
                    new() { LanguageCode = "ko", Name = "피자 반죽" },
                },
            },
            new Ingredient
            {
                Id = 13, Name = "Tomato Sauce", Stock = 19, Translations = new List<IngredientTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Tomatensaus" },
                    new() { LanguageCode = "de", Name = "Tomatensauce" },
                    new() { LanguageCode = "ko", Name = "토마토 소스" },
                },
            },
            new Ingredient
            {
                Id = 14, Name = "Mozzarella Cheese", Stock = 10, Translations = new List<IngredientTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Mozzarella kaas" },
                    new() { LanguageCode = "de", Name = "Mozzarella-Käse" },
                    new() { LanguageCode = "ko", Name = "모짜렐라 치즈" },
                },
            },
            new Ingredient
            {
                Id = 15, Name = "Pepperoni", Stock = 30, Translations = new List<IngredientTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Pepperoni" },
                    new() { LanguageCode = "de", Name = "Pepperoni" },
                    new() { LanguageCode = "ko", Name = "페퍼로니" },
                },
            },
            new Ingredient
            {
                Id = 16, Name = "Mushrooms", Stock = 25, Translations = new List<IngredientTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Champignons" },
                    new() { LanguageCode = "de", Name = "Pilze" },
                    new() { LanguageCode = "ko", Name = "버섯" },
                },
            },
            new Ingredient
            {
                Id = 17, Name = "Bell Peppers", Stock = 17, Translations = new List<IngredientTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Paprika" },
                    new() { LanguageCode = "de", Name = "Paprika" },
                    new() { LanguageCode = "ko", Name = "파프리카" },
                },
            },
            new Ingredient
            {
                Id = 18, Name = "Onions", Stock = 14, Translations = new List<IngredientTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Uien" },
                    new() { LanguageCode = "de", Name = "Zwiebeln" },
                    new() { LanguageCode = "ko", Name = "양파" },
                },
            },
            new Ingredient
            {
                Id = 19, Name = "Olives", Stock = 12, Translations = new List<IngredientTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Olijven" },
                    new() { LanguageCode = "de", Name = "Oliven" },
                    new() { LanguageCode = "ko", Name = "올리브" },
                },
            },
            new Ingredient
            {
                Id = 20, Name = "Basil", Stock = 10, Translations = new List<IngredientTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Basilicum" },
                    new() { LanguageCode = "de", Name = "Basilikum" },
                    new() { LanguageCode = "ko", Name = "바질" },
                },
            },
            new Ingredient
            {
                Id = 21, Name = "Pasta", Stock = 21, Translations = new List<IngredientTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Pasta" },
                    new() { LanguageCode = "de", Name = "Pasta" },
                    new() { LanguageCode = "ko", Name = "파스타" },
                },
            },
            new Ingredient
            {
                Id = 22, Name = "Garlic", Stock = 21, Translations = new List<IngredientTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Knoflook" },
                    new() { LanguageCode = "de", Name = "Knoblauch" },
                    new() { LanguageCode = "ko", Name = "마늘" },
                },
            },
            new Ingredient
            {
                Id = 23, Name = "Olive Oil", Stock = 21, Translations = new List<IngredientTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Olijfolie" },
                    new() { LanguageCode = "de", Name = "Olivenöl" },
                    new() { LanguageCode = "ko", Name = "올리브 오일" },
                },
            },
            new Ingredient
            {
                Id = 24, Name = "Parmesan Cheese", Stock = 21, Translations = new List<IngredientTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Parmezaanse kaas" },
                    new() { LanguageCode = "de", Name = "Parmesankäse" },
                    new() { LanguageCode = "ko", Name = "파르미 산 치즈" },
                },
            },
            new Ingredient
            {
                Id = 25, Name = "Potatoes", Stock = 21, Translations = new List<IngredientTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Aardappelen" },
                    new() { LanguageCode = "de", Name = "Kartoffeln" },
                    new() { LanguageCode = "ko", Name = "감자" },
                },
            },
            new Ingredient
            {
                Id = 26, Name = "Salt", Stock = 10, Translations = new List<IngredientTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Zout" },
                    new() { LanguageCode = "de", Name = "Salz" },
                    new() { LanguageCode = "ko", Name = "소금" },
                },
            },
            new Ingredient
            {
                Id = 27, Name = "Oil", Stock = 10, Translations = new List<IngredientTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Olie" },
                    new() { LanguageCode = "de", Name = "Öl" },
                    new() { LanguageCode = "ko", Name = "유" },
                },
            }
        );
        await dbContext.SaveChangesAsync();
    }

    private async Task SeedIngredientsToMenuItem()
    {
        dbContext.MenuItemIngredients.AddRange(
            new MenuItemIngredient { MenuItemId = 1, IngredientId = 1, Pieces = 1 },
            new MenuItemIngredient { MenuItemId = 1, IngredientId = 2, Pieces = 1 },
            new MenuItemIngredient { MenuItemId = 1, IngredientId = 3, Pieces = 1 },
            new MenuItemIngredient { MenuItemId = 1, IngredientId = 4, Pieces = 1 },
            new MenuItemIngredient { MenuItemId = 1, IngredientId = 5, Pieces = 1 },
            new MenuItemIngredient { MenuItemId = 1, IngredientId = 6, Pieces = 1 },
            new MenuItemIngredient { MenuItemId = 1, IngredientId = 7, Pieces = 1 },
            new MenuItemIngredient { MenuItemId = 1, IngredientId = 8, Pieces = 1 },
            new MenuItemIngredient { MenuItemId = 1, IngredientId = 9, Pieces = 1 },
            new MenuItemIngredient { MenuItemId = 1, IngredientId = 10, Pieces = 1 },
            new MenuItemIngredient { MenuItemId = 1, IngredientId = 11, Pieces = 1 },
            new MenuItemIngredient { MenuItemId = 2, IngredientId = 12, Pieces = 1 },
            new MenuItemIngredient { MenuItemId = 2, IngredientId = 13, Pieces = 1 },
            new MenuItemIngredient { MenuItemId = 2, IngredientId = 14, Pieces = 1 },
            new MenuItemIngredient { MenuItemId = 2, IngredientId = 15, Pieces = 1 },
            new MenuItemIngredient { MenuItemId = 2, IngredientId = 16, Pieces = 1 },
            new MenuItemIngredient { MenuItemId = 2, IngredientId = 17, Pieces = 1 },
            new MenuItemIngredient { MenuItemId = 2, IngredientId = 18, Pieces = 1 },
            new MenuItemIngredient { MenuItemId = 2, IngredientId = 19, Pieces = 1 },
            new MenuItemIngredient { MenuItemId = 2, IngredientId = 20, Pieces = 1 },
            new MenuItemIngredient { MenuItemId = 3, IngredientId = 21, Pieces = 1 },
            new MenuItemIngredient { MenuItemId = 3, IngredientId = 13, Pieces = 1 },
            new MenuItemIngredient { MenuItemId = 3, IngredientId = 22, Pieces = 1 },
            new MenuItemIngredient { MenuItemId = 3, IngredientId = 23, Pieces = 1 },
            new MenuItemIngredient { MenuItemId = 3, IngredientId = 20, Pieces = 1 },
            new MenuItemIngredient { MenuItemId = 3, IngredientId = 24, Pieces = 1 },
            new MenuItemIngredient { MenuItemId = 4, IngredientId = 25, Pieces = 1 },
            new MenuItemIngredient { MenuItemId = 4, IngredientId = 26, Pieces = 1 },
            new MenuItemIngredient { MenuItemId = 4, IngredientId = 27, Pieces = 1 }
        );
        await dbContext.SaveChangesAsync();
    }

    private async Task SeedCategories()
    {
        dbContext.Categories.AddRange(
            new Category
            {
                Id = 1, Name = "Italian", Translations = new List<CategoryTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Italiaans" },
                    new() { LanguageCode = "de", Name = "Italienisch" },
                    new() { LanguageCode = "ko", Name = "이탈리아" },
                },
            },
            new Category
            {
                Id = 2, Name = "American", Translations = new List<CategoryTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Amerikaans" },
                    new() { LanguageCode = "de", Name = "Amerikanisch" },
                    new() { LanguageCode = "ko", Name = "미국" },
                },
            },
            new Category
            {
                Id = 3, Name = "Drinks", Translations = new List<CategoryTranslation>
                {
                    new() { LanguageCode = "nl", Name = "Dranken" },
                    new() { LanguageCode = "de", Name = "Getränke" },
                    new() { LanguageCode = "ko", Name = "음료" },
                },
            }
        );
        await dbContext.SaveChangesAsync();

        dbContext.CategoryMenuItems.AddRange(new List<CategoryMenuItem>
        {
            new() { CategoryId = 1, MenuItemId = 2 },
            new() { CategoryId = 1, MenuItemId = 3 },
            new() { CategoryId = 2, MenuItemId = 1 },
            new() { CategoryId = 2, MenuItemId = 4 },
            new() { CategoryId = 3, MenuItemId = 5 },
            new() { CategoryId = 3, MenuItemId = 6 },
            new() { CategoryId = 3, MenuItemId = 7 },
            new() { CategoryId = 3, MenuItemId = 8 },
            new() { CategoryId = 3, MenuItemId = 9 },
            new() { CategoryId = 3, MenuItemId = 10 },
        });

        await dbContext.SaveChangesAsync();
    }

    private async Task SeedTables()
    {
        dbContext.Tables.AddRange(
            new Table
            {
                Id = "69AC2F65-5DE9-40D4-B930-624CA40D3F13",
                Name = "Table 1",
                SessionId = "0449DB90-66AF-4E17-8086-C1452270B52D",
                QrCode = "n/a",
                CreatedAt = DateTime.Parse("2023-09-01 12:00:00"),
                IsReservable = true,
            },
            new Table
            {
                Id = "C158F7B2-3F05-4C9F-BC95-3246D20A8A45",
                Name = "Table 2",
                SessionId = "7EAE8F7F-C969-4FCD-869E-07AC2E62EB44",
                QrCode = "n/a",
                CreatedAt = DateTime.Parse("2023-09-01 12:00:00").AddMinutes(10),
                IsReservable = true,
            },
            new Table
            {
                Id = "46673854-0888-4309-AD33-71A306C1D026",
                Name = "Table 3",
                SessionId = "CBF261E5-A710-4611-A423-87943EB5DC32",
                QrCode = "n/a",
                CreatedAt = DateTime.Parse("2023-09-01 12:00:00").AddMinutes(20),
                IsReservable = true,
            },
            new Table
            {
                Id = "493FAF89-7344-403C-8D89-C9DF5BDFCB0F",
                Name = "Table 4",
                SessionId = "F2740A4E-2E7F-4E5E-A8BE-5680599D4357",
                QrCode = "n/a",
                CreatedAt = DateTime.Parse("2023-09-01 12:00:00").AddMinutes(30),
            },
            new Table
            {
                Id = "AE15A89C-0CF0-47DA-83F0-6E232B819BBD",
                Name = "Table 5",
                SessionId = "FD9EADC3-1CBC-417D-9CFB-62202E242356",
                QrCode = "n/a",
                CreatedAt = DateTime.Parse("2023-09-01 12:00:00").AddMinutes(30),
            }
        );
        await dbContext.SaveChangesAsync();
    }
}