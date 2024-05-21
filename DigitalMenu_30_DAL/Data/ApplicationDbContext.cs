using DigitalMenu_20_BLL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DigitalMenu_30_DAL.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<CartItem> CartItems { get; set; }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Ingredient> Ingredients { get; set; }

    public DbSet<MenuItem> MenuItems { get; set; }

    public DbSet<CategoryMenuItem> CategoryMenuItems { get; set; }

    public DbSet<MenuItemIngredient> MenuItemIngredients { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<OrderMenuItem> OrderMenuItems { get; set; }

    public DbSet<ExcludedIngredientOrderMenuItem> ExcludedIngredientOrderMenuItems { get; set; }

    public DbSet<ExcludedIngredientCartItem> ExcludedIngredientCartItems { get; set; }

    public DbSet<Table> Tables { get; set; }

    public DbSet<Split> Splits { get; set; }

    public DbSet<CategoryTranslation> CategoryTranslations { get; set; }

    public DbSet<IngredientTranslation> IngredientTranslations { get; set; }

    public DbSet<MenuItemTranslation> MenuItemTranslations { get; set; }

    public DbSet<Reservation> Reservations { get; set; }
}