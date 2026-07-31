using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Q1.Models;

public partial class PePrn26spP7Context : DbContext
{
    public PePrn26spP7Context()
    {
    }

    public PePrn26spP7Context(DbContextOptions<PePrn26spP7Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Batch> Batches { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductSupplier> ProductSuppliers { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Batch>(entity =>
        {
            entity.HasKey(e => e.BatchId).HasName("PK__Batches__5D55CE38CFA16A98");

            entity.Property(e => e.BatchId).HasColumnName("BatchID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Quarter).HasMaxLength(20);
            entity.Property(e => e.WarehouseCode).HasMaxLength(20);

            entity.HasOne(d => d.Product).WithMany(p => p.Batches)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Batches__Product__2D27B809");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64B8BDDE0123");

            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.CustomerName).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BAF987A80FC");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.BatchId).HasColumnName("BatchID");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

            entity.HasOne(d => d.Batch).WithMany(p => p.Orders)
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("FK__Orders__BatchID__32E0915F");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Orders__Customer__31EC6D26");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6ED24C984BB");

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.ProductName).HasMaxLength(200);
        });

        modelBuilder.Entity<ProductSupplier>(entity =>
        {
            entity.HasKey(e => new { e.ProductId, e.SupplierId }).HasName("PK__ProductS__E0B2A084210E9AA9");

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");
            entity.Property(e => e.SupplyDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductSuppliers)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductSu__Produ__29572725");

            entity.HasOne(d => d.Supplier).WithMany(p => p.ProductSuppliers)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductSu__Suppl__2A4B4B5E");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PK__Supplier__4BE666942F9DC551");

            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");
            entity.Property(e => e.Specialty).HasMaxLength(200);
            entity.Property(e => e.SupplierName).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
