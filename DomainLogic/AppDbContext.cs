using DomainLogic.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DomainLogic
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<TelegramUser> TelegramUsers { get; set; }
        public DbSet<UserFile> UserFiles { get; set; }
        public DbSet<FileMetadata> FileMetadata { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TelegramUser>(entity => 
            {
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<UserFile>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne<TelegramUser>()
                    .WithMany()
                    .HasForeignKey(e => e.TelegramUserId);
                entity.HasOne<FileMetadata>(e => e.Content)
                    .WithMany()
                    .HasForeignKey(f => f.FileUniqueId);
            });
            modelBuilder.Entity<FileMetadata>(entity =>
            {
                entity.HasKey(e => e.FileUniqueId);
                entity.Property(p => p.FileType).HasConversion(new EnumToStringConverter<FileType>());
                entity.OwnsMany(e => e.OtherPhotoSizes, b =>
                {
                    b.ToJson();
                });
            });
        }
    }
}
