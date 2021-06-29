/*
    MTD OrderMaker - http://ordermaker.org
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.

    This file is part of MTD OrderMaker.
    MTD OrderMaker is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see  https://www.gnu.org/licenses/.
*/

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Mtd.OrderMaker.Server.Entity
{
    public partial class OrderMakerContext : DbContext
    {
        public OrderMakerContext()
        {
        }

        public OrderMakerContext(DbContextOptions<OrderMakerContext> options)
            : base(options)
        {
        }


        public virtual DbSet<MtdCategoryForm> MtdCategoryForm { get; set; }

        public virtual DbSet<MtdEventSubscribe> MtdEventSubscribes { get; set; }
        public virtual DbSet<MtdGroup> MtdGroup { get; set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
                        
            ApprovalModelCreating(modelBuilder);

            FormModelCreating(modelBuilder);

            FilterModelCreating(modelBuilder);

            ConfigModelCreating(modelBuilder);

            LogModelCreating(modelBuilder);

            PolicyModelCreating(modelBuilder);

            RegisterModelCreating(modelBuilder);

            StoreModelCreating(modelBuilder);

            StoreStackModelCreating(modelBuilder);

            SystemModelCreating(modelBuilder);

            modelBuilder.Entity<MtdCategoryForm>(entity =>
            {
                entity.ToTable("mtd_category_form");

                entity.HasIndex(e => e.Id)
                    .HasDatabaseName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.Parent)
                    .HasDatabaseName("fk_group_themself_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasColumnType("varchar(512)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(120)");

                entity.Property(e => e.Parent)
                    .IsRequired()
                    .HasColumnName("parent")
                    .HasColumnType("varchar(36)");
            });
                               
            modelBuilder.Entity<MtdGroup>(entity =>
            {
                entity.ToTable("mtd_group");

                entity.HasIndex(e => e.Id)
                    .HasDatabaseName("id_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasColumnType("varchar(512)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(255)");
            });

            modelBuilder.Entity<MtdEventSubscribe>(entity => {

                entity.ToTable("mtd_event_subscribe");

                entity.HasIndex(e => e.Id)
                    .HasDatabaseName("id_unique")
                    .IsUnique();

                entity.HasIndex(e => e.MtdFormId)
                    .HasDatabaseName("fk_mtd_event_mtd_form_idx");

                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("ix_user_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.MtdFormId)
                    .IsRequired()
                    .HasColumnName("mtd_form_id")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("user_id")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.EventCreate)
                   .HasColumnName("event_create")
                   .HasColumnType("tinyint(4)")
                   .HasDefaultValueSql("'0'");

                entity.Property(e => e.EventEdit)
                   .HasColumnName("event_edit")
                   .HasColumnType("tinyint(4)")
                   .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.MtdForm)
                    .WithMany(p => p.MtdEventSubscribes)
                    .HasForeignKey(d => d.MtdFormId)
                    .HasConstraintName("fk_mtd_event_mtd_form");

            });



        }
    }
}
