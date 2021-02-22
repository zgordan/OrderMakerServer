using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Entity
{
    public partial class OrderMakerContext : DbContext
    {
        public virtual DbSet<MtdStore> MtdStore { get; set; }
        public virtual DbSet<MtdStoreActivity> MtdStoreActivites { get; set; }
        public virtual DbSet<MtdStoreApproval> MtdStoreApproval { get; set; }
        public virtual DbSet<MtdStoreLink> MtdStoreLink { get; set; }
        public virtual DbSet<MtdStoreOwner> MtdStoreOwner { get; set; }
        public virtual DbSet<MtdStoreTask> MtdStoreTasks { get; set; }

        public void StoreModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<MtdStore>(entity =>
            {
                entity.ToTable("mtd_store");

                entity.HasIndex(e => e.Id)
                    .HasDatabaseName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.MtdForm)
                    .HasDatabaseName("fk_mtd_store_mtd_form1_idx");

                entity.HasIndex(e => e.Parent);

                entity.HasIndex(e => e.Timecr)
                    .HasDatabaseName("IX_TIMECR");

                entity.HasIndex(e => new { e.MtdForm, e.Sequence })
                    .HasDatabaseName("Seq_Unique")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.Active)
                    .HasColumnName("active")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.MtdForm)
                    .IsRequired()
                    .HasColumnName("mtd_form")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.Parent)
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.Sequence)
                    .HasColumnName("sequence")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Timecr)
                    .HasColumnName("timecr")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.MtdFormNavigation)
                    .WithMany(p => p.MtdStore)
                    .HasForeignKey(d => d.MtdForm)
                    .HasConstraintName("fk_mtd_store_mtd_form1");

                entity.HasOne(d => d.ParentNavigation)
                    .WithMany(p => p.InverseParentNavigation)
                    .HasForeignKey(d => d.Parent)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_mtd_store_parent");
            });

            modelBuilder.Entity<MtdStoreActivity>(entity =>
            {
                entity.ToTable("mtd_store_activity");

                entity.HasIndex(e => e.Id)
                    .HasDatabaseName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.MtdStoreId)
                    .HasDatabaseName("fk_store_activity_store_idx");

                entity.HasIndex(e => e.MtdFormActivityId)
                    .HasDatabaseName("fk_store_activity_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.MtdFormActivityId)
                    .IsRequired()
                    .HasColumnName("mtd_form_activity_id")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.MtdStoreId)
                    .IsRequired()
                    .HasColumnName("mtd_store_id")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.Comment)
                    .IsRequired()
                    .HasColumnName("app_comment")
                    .HasColumnType("varchar(512)");

                entity.Property(e => e.TimeCr)
                    .IsRequired()
                    .HasColumnName("timecr")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("user_id")
                    .HasColumnType("varchar(36)");

                entity.HasOne(d => d.MtdStore)
                    .WithMany(p => p.MtdStoreActitvites)
                    .HasForeignKey(d => d.MtdStoreId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_store_activity_store");

                entity.HasOne(d => d.MtdFormActitvity)
                    .WithMany(p => p.MtdStoreActivites)
                    .HasForeignKey(d => d.MtdFormActivityId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_store_activity_idx");
            });

            modelBuilder.Entity<MtdStoreApproval>(entity =>
            {
                entity.ToTable("mtd_store_approval");

                entity.HasIndex(e => e.Complete)
                    .HasDatabaseName("IX_APPROVED");

                entity.HasIndex(e => e.Id)
                    .HasDatabaseName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.MtdApproveStage)
                    .HasDatabaseName("fk_store_approve_stage_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.Complete)
                    .HasColumnName("complete")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.MtdApproveStage)
                    .HasColumnName("md_approve_stage")
                    .HasColumnType("int(11)");

                entity.Property(e => e.PartsApproved)
                    .IsRequired()
                    .HasColumnName("parts_approved")
                    .HasColumnType("longtext");

                entity.Property(e => e.Result)
                    .HasColumnName("result")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.SignChain)
                    .HasColumnName("sign_chain")
                    .HasColumnType("longtext");

                entity.Property(e => e.LastEventTime)
                    .IsRequired()
                    .HasColumnName("last_event_time")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.MtdStoreApproval)
                    .HasForeignKey<MtdStoreApproval>(d => d.Id)
                    .HasConstraintName("fk_store_approve");

                entity.HasOne(d => d.MtdApproveStageNavigation)
                    .WithMany(p => p.MtdStoreApproval)
                    .HasForeignKey(d => d.MtdApproveStage)
                    .HasConstraintName("fk_store_approve_stage");
            });

            modelBuilder.Entity<MtdStoreLink>(entity =>
            {
                entity.ToTable("mtd_store_link");

                entity.HasIndex(e => e.Id)
                    .HasDatabaseName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.MtdStore)
                    .HasDatabaseName("fk_mtd_store_link_mtd_store1_idx");

                entity.HasIndex(e => new { e.MtdStore, e.Id })
                    .HasDatabaseName("ix_unique")
                    .IsUnique();

                entity.HasIndex(e => e.Register)
                    .HasDatabaseName("ix_register");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.MtdStore)
                    .IsRequired()
                    .HasColumnName("mtd_store")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.Register)
                    .IsRequired()
                    .HasColumnType("varchar(768)");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.MtdStoreLink)
                    .HasForeignKey<MtdStoreLink>(d => d.Id)
                    .HasConstraintName("fk_mtd_store_link_mtd_store_stack");

                entity.HasOne(d => d.MtdStoreNavigation)
                    .WithMany(p => p.MtdStoreLink)
                    .HasForeignKey(d => d.MtdStore)
                    .HasConstraintName("fk_mtd_store_link_mtd_store1");
            });

            modelBuilder.Entity<MtdStoreOwner>(entity =>
            {
                entity.ToTable("mtd_store_owner");

                entity.HasIndex(e => e.Id)
                    .HasDatabaseName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("IX_USER");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("user_id")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnName("user_name")
                    .HasColumnType("varchar(256)");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.MtdStoreOwner)
                    .HasForeignKey<MtdStoreOwner>(d => d.Id)
                    .HasConstraintName("fk_owner_store");
            });

            modelBuilder.Entity<MtdStoreTask>(entity =>
            {
                entity.ToTable("mtd_store_task");

                entity.HasIndex(e => e.Id)
                    .HasDatabaseName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.MtdStoreId)
                    .HasDatabaseName("fk_mtd_store_task_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.MtdStoreId)
                    .IsRequired()
                    .HasColumnName("mtd_store_id")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(250)");

                entity.Property(e => e.Deadline)
                    .IsRequired()
                    .HasColumnName("deadline")
                    .HasColumnType("datetime");


                entity.Property(e => e.Initiator)
                    .IsRequired()
                    .HasColumnName("iniciator")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.InitNote)
                    .IsRequired()
                    .HasColumnName("init_note")
                    .HasColumnType("varchar(250)");

                entity.Property(e => e.InitTimeCr)
                    .IsRequired()
                    .HasColumnName("init_timecr")
                    .HasColumnType("datetime");

                entity.Property(e => e.Executor)
                    .IsRequired()
                    .HasColumnName("executor")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.ExecNote)
                    .IsRequired()
                    .HasColumnName("exec_note")
                    .HasColumnType("varchar(250)");

                entity.Property(e => e.ExecTimeCr)
                    .IsRequired()
                    .HasColumnName("exec_timecr")
                    .HasColumnType("datetime");


                entity.Property(e => e.Complete)
                    .IsRequired()
                    .HasColumnName("complete")
                    .HasColumnType("int(11)");


                entity.Property(e => e.PrivateTask)
                    .IsRequired()
                    .HasColumnName("private_task")
                    .HasColumnType("tinyint(4)");

                entity.Property(e => e.LastEventTime)
                    .IsRequired()
                    .HasColumnName("last_evant_time")
                    .HasColumnType("datetime");


                entity.HasOne(d => d.MtdStore)
                    .WithMany(p => p.MtdStoreTasks)
                    .HasForeignKey(d => d.MtdStoreId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_mtd_store_task");
            });

        }
    }
}
