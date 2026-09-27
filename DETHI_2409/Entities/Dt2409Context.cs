using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DETHI_2409.Entities;

public partial class Dt2409Context : DbContext
{
    public Dt2409Context()
    {
    }

    public Dt2409Context(DbContextOptions<Dt2409Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Developer> Developers { get; set; }

    public virtual DbSet<Label> Labels { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<WorkItem> WorkItems { get; set; }

    public virtual DbSet<WorkItemHistory> WorkItemHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Developer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("developers_pkey");

            entity.ToTable("developers");

            entity.HasIndex(e => e.Code, "developers_code_key").IsUnique();

            entity.HasIndex(e => e.Email, "developers_email_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(20)
                .HasColumnName("code");
            entity.Property(e => e.Email)
                .HasMaxLength(200)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(120)
                .HasColumnName("full_name");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Team)
                .HasMaxLength(50)
                .HasColumnName("team");
        });

        modelBuilder.Entity<Label>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("labels_pkey");

            entity.ToTable("labels");

            entity.HasIndex(e => e.Name, "labels_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("projects_pkey");

            entity.ToTable("projects");

            entity.HasIndex(e => e.Code, "projects_code_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(20)
                .HasColumnName("code");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");
        });

        modelBuilder.Entity<WorkItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("work_items_pkey");

            entity.ToTable("work_items");
            entity.HasIndex(e => new { e.AssigneeId, e.Status }, "ix_work_items_assignee_status").HasFilter("(NOT is_deleted)");

            entity.HasIndex(e => new { e.ProjectId, e.CreatedAt }, "ix_work_items_project_created").HasFilter("(NOT is_deleted)");

            entity.HasIndex(e => e.Status, "ix_work_items_status").HasFilter("(NOT is_deleted)");

            entity.HasIndex(e => e.Code, "work_items_code_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AssigneeId).HasColumnName("assignee_id");
            entity.Property(e => e.Code)
                .HasMaxLength(30)
                .HasColumnName("code");
            entity.Property(e => e.CompletedAt).HasColumnName("completed_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.Description)
                .HasMaxLength(2000)
                .HasColumnName("description");
            entity.Property(e => e.DueAt).HasColumnName("due_at");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.Priority)
                .HasMaxLength(20)
                .HasColumnName("priority");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.Assignee).WithMany(p => p.WorkItems)
                .HasForeignKey(d => d.AssigneeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("work_items_assignee_id_fkey");

            entity.HasOne(d => d.Project).WithMany(p => p.WorkItems)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("work_items_project_id_fkey");

            entity.HasMany(d => d.Labels).WithMany(p => p.WorkItems)
                .UsingEntity<Dictionary<string, object>>(
                    "WorkItemLabel",
                    r => r.HasOne<Label>().WithMany()
                        .HasForeignKey("LabelId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("work_item_labels_label_id_fkey"),
                    l => l.HasOne<WorkItem>().WithMany()
                        .HasForeignKey("WorkItemId")
                        .HasConstraintName("work_item_labels_work_item_id_fkey"),
                    j =>
                    {
                        j.HasKey("WorkItemId", "LabelId").HasName("work_item_labels_pkey");
                        j.ToTable("work_item_labels");
                        j.IndexerProperty<long>("WorkItemId").HasColumnName("work_item_id");
                        j.IndexerProperty<long>("LabelId").HasColumnName("label_id");
                    });
        });

        modelBuilder.Entity<WorkItemHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("work_item_histories_pkey");

            entity.ToTable("work_item_histories");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChangedBy)
                .HasMaxLength(120)
                .HasColumnName("changed_by");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.FromStatus)
                .HasMaxLength(20)
                .HasColumnName("from_status");
            entity.Property(e => e.Note)
                .HasMaxLength(1000)
                .HasColumnName("note");
            entity.Property(e => e.ToStatus)
                .HasMaxLength(20)
                .HasColumnName("to_status");
            entity.Property(e => e.WorkItemId).HasColumnName("work_item_id");

            entity.HasOne(d => d.WorkItem).WithMany(p => p.WorkItemHistories)
                .HasForeignKey(d => d.WorkItemId)
                .HasConstraintName("work_item_histories_work_item_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
