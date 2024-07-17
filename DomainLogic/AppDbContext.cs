using DomainLogic.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using File = DomainLogic.Entities.File;

namespace DomainLogic
{
    public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<TelegramUser> TelegramUsers { get; set; }
        public DbSet<File> Files { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TelegramUser>(entity => 
            {
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<File>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne<TelegramUser>().WithMany().HasForeignKey(e => e.TelegramUserId);
                entity.Property(p => p.FileType).HasConversion(new EnumToStringConverter<FileType>());
                entity.OwnsMany(e => e.OtherPhotoSizes, b =>
                {
                    b.ToJson();
                });
            });
        }
    }
}
