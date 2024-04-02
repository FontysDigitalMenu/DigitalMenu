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

    public DbSet<MenuItemIngredient> MenuItemIngredients { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<OrderMenuItem> OrderMenuItems { get; set; }

    public DbSet<ExcludedIngredientCartItem> ExcludedIngredientCartItems { get; set; }

    public DbSet<Table> Tables { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<OrderMenuItem>()
            .HasKey(om => new { om.OrderId, om.MenuItemId });

        modelBuilder.Entity<OrderMenuItem>()
            .HasOne(om => om.Order)
            .WithMany(o => o.OrderMenuItems)
            .HasForeignKey(om => om.OrderId);

        modelBuilder.Entity<OrderMenuItem>()
            .HasOne(om => om.MenuItem)
            .WithMany()
            .HasForeignKey(om => om.MenuItemId);


        modelBuilder.Entity<MenuItemIngredient>()
            .HasKey(mi => new { mi.MenuItemId, mi.IngredientId });
    }
}