using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WarehouseManager.Server.Models;

public partial class WarehouseContext : DbContext
{
    public WarehouseContext()
    {
    }

    public WarehouseContext(DbContextOptions<WarehouseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Balance> Balances { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<DocumentsReceipt> DocumentsReceipts { get; set; }

    public virtual DbSet<DocumentsShipment> DocumentsShipments { get; set; }

    public virtual DbSet<Measure> Measures { get; set; }

    public virtual DbSet<ReceiptResource> ReceiptResources { get; set; }

    public virtual DbSet<Resource> Resources { get; set; }

    public virtual DbSet<ShipmentResource> ShipmentResources { get; set; }

    public virtual DbSet<VReceiptDocumentResource> VReceiptDocumentResources { get; set; }

    public virtual DbSet<VShipmentDocumentResource> VShipmentDocumentResources { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=warehouse;Username=wm_server;Password=wm_server");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Balance>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("balances_pkey");

            entity.ToTable("balances");

            entity.HasIndex(e => e.MeasureId, "idx_balances_measure_id");

            entity.HasIndex(e => e.ResourceId, "idx_balances_resource_id");

            entity.HasIndex(e => new { e.ResourceId, e.MeasureId }, "unique_resource_measure").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.MeasureId).HasColumnName("measure_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.ResourceId).HasColumnName("resource_id");

            entity.HasOne(d => d.Measure).WithMany(p => p.Balances)
                .HasForeignKey(d => d.MeasureId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("balances_measure_id_fkey");

            entity.HasOne(d => d.Resource).WithMany(p => p.Balances)
                .HasForeignKey(d => d.ResourceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("balances_resource_id_fkey");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("clients_pkey");

            entity.ToTable("clients");

            entity.HasIndex(e => e.Name, "clients_name_key").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasColumnName("status");
        });

        modelBuilder.Entity<DocumentsReceipt>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("documents_receipt_pkey");

            entity.ToTable("documents_receipt");

            entity.HasIndex(e => e.Number, "documents_receipt_number_key").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.DateReceipt).HasColumnName("date_receipt");
            entity.Property(e => e.Number)
                .HasMaxLength(50)
                .HasColumnName("number");
        });

        modelBuilder.Entity<DocumentsShipment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("documents_shipment_pkey");

            entity.ToTable("documents_shipment");

            entity.HasIndex(e => e.Number, "documents_shipment_number_key").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.DateShipment).HasColumnName("date_shipment");
            entity.Property(e => e.Number)
                .HasMaxLength(50)
                .HasColumnName("number");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasColumnName("status");

            entity.HasOne(d => d.Client).WithMany(p => p.DocumentsShipments)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("documents_shipment_client_id_fkey");
        });

        modelBuilder.Entity<Measure>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("measures_pkey");

            entity.ToTable("measures");

            entity.HasIndex(e => e.Name, "measures_name_key").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasColumnName("status");
        });

        modelBuilder.Entity<ReceiptResource>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("receipt_resources_pkey");

            entity.ToTable("receipt_resources");

            entity.HasIndex(e => e.MeasureId, "idx_receipt_resources_measure_id");

            entity.HasIndex(e => e.ResourceId, "idx_receipt_resources_resource_id");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.DocumentReceiptId).HasColumnName("document_receipt_id");
            entity.Property(e => e.MeasureId).HasColumnName("measure_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.ResourceId).HasColumnName("resource_id");

            entity.HasOne(d => d.DocumentReceipt).WithMany(p => p.ReceiptResources)
                .HasForeignKey(d => d.DocumentReceiptId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("receipt_resources_document_receipt_id_fkey");

            entity.HasOne(d => d.Measure).WithMany(p => p.ReceiptResources)
                .HasForeignKey(d => d.MeasureId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("receipt_resources_measure_id_fkey");

            entity.HasOne(d => d.Resource).WithMany(p => p.ReceiptResources)
                .HasForeignKey(d => d.ResourceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("receipt_resources_resource_id_fkey");
        });

        modelBuilder.Entity<Resource>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("resources_pkey");

            entity.ToTable("resources");

            entity.HasIndex(e => e.Name, "resources_name_key").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasColumnName("status");
        });

        modelBuilder.Entity<ShipmentResource>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("shipment_resources_pkey");

            entity.ToTable("shipment_resources");

            entity.HasIndex(e => e.MeasureId, "idx_shipment_resources_measure_id");

            entity.HasIndex(e => e.ResourceId, "idx_shipment_resources_resource_id");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.DocumentShipmentId).HasColumnName("document_shipment_id");
            entity.Property(e => e.MeasureId).HasColumnName("measure_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.ResourceId).HasColumnName("resource_id");

            entity.HasOne(d => d.DocumentShipment).WithMany(p => p.ShipmentResources)
                .HasForeignKey(d => d.DocumentShipmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("shipment_resources_document_shipment_id_fkey");

            entity.HasOne(d => d.Measure).WithMany(p => p.ShipmentResources)
                .HasForeignKey(d => d.MeasureId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("shipment_resources_measure_id_fkey");

            entity.HasOne(d => d.Resource).WithMany(p => p.ShipmentResources)
                .HasForeignKey(d => d.ResourceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("shipment_resources_resource_id_fkey");
        });

        modelBuilder.Entity<VReceiptDocumentResource>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_receipt_document_resource");

            entity.Property(e => e.DocumentReceiptId).HasColumnName("document_receipt_id");
            entity.Property(e => e.MeasureId).HasColumnName("measure_id");
            entity.Property(e => e.MeasureName)
                .HasMaxLength(50)
                .HasColumnName("measure_name");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.ReceiptResourceId).HasColumnName("receipt_resource_id");
            entity.Property(e => e.ResourceId).HasColumnName("resource_id");
            entity.Property(e => e.ResourceName)
                .HasMaxLength(50)
                .HasColumnName("resource_name");
        });

        modelBuilder.Entity<VShipmentDocumentResource>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_shipment_document_resource");

            entity.Property(e => e.DocumentShipmentId).HasColumnName("document_shipment_id");
            entity.Property(e => e.MeasureId).HasColumnName("measure_id");
            entity.Property(e => e.MeasureName)
                .HasMaxLength(50)
                .HasColumnName("measure_name");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.ResourceId).HasColumnName("resource_id");
            entity.Property(e => e.ResourceName)
                .HasMaxLength(50)
                .HasColumnName("resource_name");
            entity.Property(e => e.ShipmentResourceId).HasColumnName("shipment_resource_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
