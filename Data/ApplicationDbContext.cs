using Instadvert.CZ.Data.ViewModels;
using Instadvert.CZ.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Instadvert.CZ.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        // Defining DbSet properties for the application's main entities (Blog, Category, Transaction, Message)
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Message> Messages { get; set; }

        // Optional: Database schema configuration
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // This line ensures the base method of IdentityDbContext is called so that identity-related models are configured properly.
            base.OnModelCreating(modelBuilder);

            // Setting up inheritance where CompanyUser and BloggerUser are derived from User entity
            modelBuilder.Entity<CompanyUser>()
                .HasBaseType<User>();

            modelBuilder.Entity<BloggerUser>()
                .HasBaseType<User>();

            // Configuring many-to-many relationships between CompanyUser and Category
            modelBuilder.Entity<CompanyUser>()
                .HasMany(b => b.Categories)
                .WithMany(c => c.companyUsers)
                .UsingEntity(j => j.ToTable("CompanyUsersCategories")); // Intersection table for CompanyUser and Category

            // Configuring many-to-many relationships between BloggerUser and Category
            modelBuilder.Entity<BloggerUser>()
               .HasMany(b => b.Categories)
               .WithMany(c => c.bloggerUserUsers)
               .UsingEntity(j => j.ToTable("BloggerUsersCategories")); // Intersection table for BloggerUser and Category

            // Example of setting a unique index on the CompanyUser's Email property to ensure no duplicate emails.
            modelBuilder.Entity<CompanyUser>()
                .HasIndex(c => c.Email)
                .IsUnique();

            // Example of configuring a column type (decimal) for the Amount property in the Transaction entity.
            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasColumnType("decimal(18, 2)");

            // Configuring relationships for the Message entity:
            // One-to-many relationship where a User (Sender) can have many Messages, but a Message has one Sender.
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.Messages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.NoAction); // Prevent deletion of messages if the sender is deleted.

            // Configuring a one-to-many relationship where a Message has one Receiver, but the relationship is not navigable from the Receiver's side.
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany() // No navigation property for the receiver.
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.NoAction); // Prevent deletion of messages if the receiver is deleted.

            // Configuring the primary key for the Message entity to be auto-generated.
            modelBuilder.Entity<Message>()
                .Property(m => m.Id)
                .ValueGeneratedOnAdd();

            // Additional model configurations can be added here...
        }
    }
}
