using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Entity
{
    public partial class OrderMakerContext : DbContext
    {
        public virtual DbSet<MtdStoreStack> MtdStoreStack { get; set; }
        public virtual DbSet<MtdStoreStackDate> MtdStoreStackDate { get; set; }
        public virtual DbSet<MtdStoreStackDecimal> MtdStoreStackDecimal { get; set; }
        public virtual DbSet<MtdStoreStackFile> MtdStoreStackFile { get; set; }
        public virtual DbSet<MtdStoreStackInt> MtdStoreStackInt { get; set; }
        public virtual DbSet<MtdStoreStackText> MtdStoreStackText { get; set; }


        public void StoreStackModelCreating(ModelBuilder modelBuilder) {


            modelBuilder.Entity<MtdStoreStack>(entity =>
            {
                entity.ToTable("mtd_store_stack");

                entity.HasIndex(e => e.Id)
                    .HasDatabaseName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.MtdFormPartField)
                    .HasDatabaseName("fk_mtd_store_stack_mtd_form_part_field1_idx");

                entity.HasIndex(e => e.MtdStore)
                    .HasDatabaseName("fk_mtd_store_stack_mtd_store_idx");

                entity.HasIndex(e => new { e.MtdStore, e.MtdFormPartField })
                    .HasDatabaseName("IX_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.MtdFormPartField)
                    .IsRequired()
                    .HasColumnName("mtd_form_part_field")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.MtdStore)
                    .IsRequired()
                    .HasColumnName("mtd_store")
                    .HasColumnType("varchar(36)");

                entity.HasOne(d => d.MtdFormPartFieldNavigation)
                    .WithMany(p => p.MtdStoreStack)
                    .HasForeignKey(d => d.MtdFormPartField)
                    .HasConstraintName("fk_mtd_store_stack_mtd_form_part_field1");

                entity.HasOne(d => d.MtdStoreNavigation)
                    .WithMany(p => p.MtdStoreStack)
                    .HasForeignKey(d => d.MtdStore)
                    .HasConstraintName("fk_mtd_store_stack_mtd_store");
            });

            modelBuilder.Entity<MtdStoreStackDate>(entity =>
            {
                entity.ToTable("mtd_store_stack_date");

                entity.HasIndex(e => e.Id)
                    .HasDatabaseName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.Register)
                    .HasDatabaseName("IX_DATESTACK");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Register)
                    .HasColumnName("register")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.MtdStoreStackDate)
                    .HasForeignKey<MtdStoreStackDate>(d => d.Id)
                    .HasConstraintName("fk_date_stack");
            });

            modelBuilder.Entity<MtdStoreStackDecimal>(entity =>
            {
                entity.ToTable("mtd_store_stack_decimal");

                entity.HasIndex(e => e.Id)
                    .HasDatabaseName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.Register)
                    .HasDatabaseName("IX_DECIMALREGISTER");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Register)
                    .HasColumnName("register")
                    .HasColumnType("decimal(20,2)");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.MtdStoreStackDecimal)
                    .HasForeignKey<MtdStoreStackDecimal>(d => d.Id)
                    .HasConstraintName("fk_decimal_stack");
            });

            modelBuilder.Entity<MtdStoreStackFile>(entity =>
            {
                entity.ToTable("mtd_store_stack_file");

                entity.HasIndex(e => e.Id)
                    .HasDatabaseName("id_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasColumnName("file_name")
                    .HasColumnType("varchar(256)");

                entity.Property(e => e.FileSize)
                    .HasColumnName("file_size")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.FileType)
                    .IsRequired()
                    .HasColumnName("file_type")
                    .HasColumnType("varchar(256)");

                entity.Property(e => e.Register)
                    .IsRequired()
                    .HasColumnName("register")
                    .HasColumnType("longblob");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.MtdStoreStackFile)
                    .HasForeignKey<MtdStoreStackFile>(d => d.Id)
                    .HasConstraintName("fk_file_stack");
            });

            modelBuilder.Entity<MtdStoreStackInt>(entity =>
            {
                entity.ToTable("mtd_store_stack_int");

                entity.HasIndex(e => e.Id)
                    .HasDatabaseName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.Register)
                    .HasDatabaseName("IX_INTSTACK");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Register)
                    .HasColumnName("register")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.MtdStoreStackInt)
                    .HasForeignKey<MtdStoreStackInt>(d => d.Id)
                    .HasConstraintName("fk_int_stack");
            });

            modelBuilder.Entity<MtdStoreStackText>(entity =>
            {
                entity.ToTable("mtd_store_stack_text");

                entity.HasIndex(e => e.Id)
                    .HasDatabaseName("category_id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.Register)
                    .HasDatabaseName("IX_REGISTER_TEXT");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Register)
                    .IsRequired()
                    .HasColumnName("register")
                    .HasColumnType("varchar(768)");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.MtdStoreStackText)
                    .HasForeignKey<MtdStoreStackText>(d => d.Id)
                    .HasConstraintName("fk_text_stack");
            });

        }

    }
}
