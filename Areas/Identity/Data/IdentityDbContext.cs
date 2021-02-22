using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Mtd.Cpq.Manager.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Areas.Identity.Data;

namespace Mtd.OrderMaker.Server.Entity
{
    public class IdentityDbContext : IdentityDbContext<WebAppUser, WebAppRole, string>
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<MtdCpqTitlesOwner> MtdCpqTitlesOwners { get; set; }
        public virtual DbSet<MtdCpqProposalOwner> MtdCpqProposalOwners { get; set; }
        public virtual DbSet<MtdCpqLogAction> MtdCpqLogActions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<MtdCpqLogAction>(entity =>
            {
                entity.ToTable("mtd_cpq_log_action");

                entity.HasIndex(e => e.Id)
                    .HasDatabaseName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("idx_userid");

                entity.HasIndex(e => e.DocumentId)
                    .HasDatabaseName("idx_documentid");

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
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.ActionTime)
                    .IsRequired()
                    .HasColumnName("action_time")
                    .HasColumnType("datetime");
                
                entity.Property(e => e.ActionType)
                    .IsRequired()
                    .HasColumnName("action_type")
                    .HasColumnType("int");

                entity.Property(e => e.DocumentId)
                    .IsRequired()
                    .HasColumnName("document_id")
                    .HasColumnType("varchar(36)");

            });

            modelBuilder.Entity<MtdCpqTitlesOwner>(entity =>
            {
                entity.ToTable("mtd_cpq_titles_owner");

                entity.HasIndex(e => e.Id)
                    .HasDatabaseName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("idx_userid");

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
                    .HasColumnType("varchar(255)");

            });

            modelBuilder.Entity<MtdCpqProposalOwner>(entity =>
            {
                entity.ToTable("mtd_cpq_proposal_owner");

                entity.HasIndex(e => e.Id)
                    .HasDatabaseName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("idx_userid");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("varchar(36)");
                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("user_id")
                    .HasColumnType("varchar(36)");
                
                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("user_name")
                    .HasColumnType("varchar(255)");

            });
        }
    }
}
