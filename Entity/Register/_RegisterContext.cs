using Microsoft.EntityFrameworkCore;

namespace Mtd.OrderMaker.Server.Entity
{
    public partial class OrderMakerContext : DbContext
    {
        public virtual DbSet<MtdRegister> MtdRegister { get; set; }
        public virtual DbSet<MtdRegisterField> MtdRegisterField { get; set; }

        public void RegisterModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MtdRegister>(entity =>
            {
                entity.ToTable("mtd_register");

                entity.HasIndex(e => e.Id)
                    .HasDatabaseName("id_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasColumnType("varchar(512)");

                entity.Property(e => e.ParentLimit)
                    .IsRequired()
                    .HasColumnName("parent_limit")
                    .HasColumnType("tinyint(4)");


            });

            modelBuilder.Entity<MtdRegisterField>(entity =>
            {
                entity.ToTable("mtd_register_field");

                entity.HasIndex(e => e.Id)
                    .HasDatabaseName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.MtdRegisterId)
                    .HasDatabaseName("fk_mtd_form_register_idx");


                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.MtdRegisterId)
                    .IsRequired()
                    .HasColumnName("mtd_register_id")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.Income)
                    .IsRequired()
                    .HasColumnName("income")
                    .HasColumnType("tinyint(4)");

                entity.Property(e => e.Expense)
                    .IsRequired()
                    .HasColumnName("expense")
                    .HasColumnType("tinyint(4)");

                entity.HasOne(d => d.MtdRegister)
                    .WithMany(p => p.MtdRegisterFields)
                    .HasForeignKey(d => d.MtdRegisterId)
                    .HasConstraintName("fk_mtd_form_register");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.MtdRegisterField)
                    .HasForeignKey<MtdRegisterField>(d => d.Id)
                    .HasConstraintName("fk_mtd_form_register_field");

            });
        }

    }
}
