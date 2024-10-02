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



        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Message> Messages { get; set; }

        

        // Опционально: Настройка схемы базы данных
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);



            modelBuilder.Entity<CompanyUser>()
          .HasBaseType<User>();

            modelBuilder.Entity<BloggerUser>()
                .HasBaseType<User>();

            // Настройка связей многие ко многим между Blogger и Category
            modelBuilder.Entity<CompanyUser>()
                .HasMany(b => b.Categories)
                .WithMany(c => c.companyUsers)

                .UsingEntity(j => j.ToTable("CompanyUsersCategories")); // Таблица пересечений

            modelBuilder.Entity<BloggerUser>()
               .HasMany(b => b.Categories)
               .WithMany(c => c.bloggerUserUsers)
               .UsingEntity(j => j.ToTable("BloggerUsersCategories")); // Таблица пересечений       
            // Пример настройки уникального индекса
            modelBuilder.Entity<CompanyUser>()
                .HasIndex(c => c.Email)
                .IsUnique();
            // Пример настройки свойства с дополнительными ограничениями
            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Message>()
          .HasOne(m => m.Sender)
          .WithMany(u => u.Messages)
          .HasForeignKey(m => m.SenderId).OnDelete(DeleteBehavior.NoAction); 

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId).OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<Message>()
       .Property(m => m.Id)
       .ValueGeneratedOnAdd();
            // Другие настройки моделей...
        }
    }
}
