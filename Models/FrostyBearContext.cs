using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FrostyBear.Models;

public partial class FrostyBearContext : DbContext
{
    public FrostyBearContext()
    {
    }

    public FrostyBearContext(DbContextOptions<FrostyBearContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<BuyDtl> BuyDtls { get; set; }

    public virtual DbSet<Buying> Buyings { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartDtl> CartDtls { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Delivery> Deliveries { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Sale> Sales { get; set; }

    public virtual DbSet<SalesDaily> SalesDailies { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=127.0.0.1;Initial Catalog=FrostyBear;User ID=sa;Password=hello13;Encrypt=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.BrandId).HasName("PK__Brands__5E5A8E270E572762");

            entity.Property(e => e.BrandId).HasColumnName("brand_id");
            entity.Property(e => e.BrandName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("brand_name");
        });

        modelBuilder.Entity<BuyDtl>(entity =>
        {
            entity.HasKey(e => new { e.BuyId, e.PdId }).HasName("PK_BuyDtl");

            entity.Property(e => e.BuyId)
                .HasMaxLength(50)
                .HasColumnName("BuyID");
            entity.Property(e => e.PdId)
                .HasMaxLength(50)
                .HasColumnName("PdID");
            entity.Property(e => e.BdtlMoney).HasColumnName("BDtlMoney");
            entity.Property(e => e.BdtlPrice).HasColumnName("BDtlPrice");
            entity.Property(e => e.BdtlQty).HasColumnName("BDtlQty");
        });

        modelBuilder.Entity<Buying>(entity =>
        {
            entity.HasKey(e => e.BuyId);

            entity.ToTable("Buying");

            entity.Property(e => e.BuyId).HasMaxLength(50);
            entity.Property(e => e.BuyDocId)
                .HasMaxLength(50)
                .HasColumnName("BuyDocID");
            entity.Property(e => e.BuyRemark).HasColumnType("text");
            entity.Property(e => e.EmployeeId)
                .HasMaxLength(50)
                .HasColumnName("employee_id");
            entity.Property(e => e.Saleman).HasMaxLength(50);
            entity.Property(e => e.SupId)
                .HasMaxLength(50)
                .HasColumnName("SupID");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.Property(e => e.CartId)
                .HasMaxLength(50)
                .HasColumnName("CartID");
            entity.Property(e => e.CartCf)
                .HasMaxLength(1)
                .HasDefaultValue("N")
                .IsFixedLength()
                .HasColumnName("CartCF");
            entity.Property(e => e.CartPay)
                .HasMaxLength(1)
                .HasDefaultValue("N")
                .IsFixedLength();
            entity.Property(e => e.CartSend)
                .HasMaxLength(1)
                .IsFixedLength();
            entity.Property(e => e.CartVoid)
                .HasMaxLength(1)
                .HasDefaultValue("N")
                .IsFixedLength();
            entity.Property(e => e.CustomerId)
                .HasMaxLength(50)
                .HasColumnName("customer_id");
        });

        modelBuilder.Entity<CartDtl>(entity =>
        {
            entity.HasKey(e => new { e.CartId, e.ProductId }).HasName("PK_CartDtl");

            entity.Property(e => e.CartId)
                .HasMaxLength(50)
                .HasColumnName("CartID");
            entity.Property(e => e.ProductId)
                .HasMaxLength(50)
                .HasColumnName("product_id");
            entity.Property(e => e.CdtlMoney).HasColumnName("CDtlMoney");
            entity.Property(e => e.CdtlPrice).HasColumnName("CDtlPrice");
            entity.Property(e => e.CdtlQty).HasColumnName("CDtlQty");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__D54EE9B48F105298");

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("category_name");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__CD65CB85339FE440");

            entity.Property(e => e.CustomerId)
                .HasMaxLength(50)
                .HasColumnName("customer_id");
            entity.Property(e => e.CustomerAddress)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("customer_address");
            entity.Property(e => e.CustomerContact)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("customer_contact");
            entity.Property(e => e.CustomerName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("customer_name");
            entity.Property(e => e.CustomerPassword)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("customer_password");
            entity.Property(e => e.CustomerUsername)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("customer_username");
            entity.Property(e => e.Lastlogin).HasColumnName("lastlogin");
            entity.Property(e => e.Startdate).HasColumnName("startdate");
        });

        modelBuilder.Entity<Delivery>(entity =>
        {
            entity.HasKey(e => e.DeliveryId).HasName("PK__Deliveri__1C5CF4F5D78A702D");

            entity.Property(e => e.DeliveryId).HasColumnName("delivery_id");
            entity.Property(e => e.DeliveryDate)
                .HasColumnType("datetime")
                .HasColumnName("delivery_date");
            entity.Property(e => e.SaleId).HasColumnName("sale_id");

            entity.HasOne(d => d.Sale).WithMany(p => p.Deliveries)
                .HasForeignKey(d => d.SaleId)
                .HasConstraintName("FK__Deliverie__sale___4F7CD00D");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__C52E0BA8DCAC15FE");

            entity.Property(e => e.EmployeeId)
                .HasMaxLength(50)
                .HasColumnName("employee_id");
            entity.Property(e => e.EmployeeContact)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("employee_contact");
            entity.Property(e => e.EmployeeName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("employee_name");
            entity.Property(e => e.EmployeePassword)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("employee_password");
            entity.Property(e => e.EmployeeUsername)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("employee_username");
            entity.Property(e => e.PositionId)
                .HasMaxLength(50)
                .HasColumnName("position_id");

            entity.HasOne(d => d.Position).WithMany(p => p.Employees)
                .HasForeignKey(d => d.PositionId)
                .HasConstraintName("FK__Employees__posit__5070F446");
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => e.PositionId).HasName("PK__Position__99A0E7A422BD3173");

            entity.Property(e => e.PositionId)
                .HasMaxLength(50)
                .HasColumnName("position_id");
            entity.Property(e => e.PositionName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("position_name");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__47027DF5F2EF5134");

            entity.Property(e => e.ProductId)
                .HasMaxLength(50)
                .HasColumnName("product_id");
            entity.Property(e => e.BrandId).HasColumnName("brand_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Detail)
                .HasColumnType("text")
                .HasColumnName("detail");
            entity.Property(e => e.ProductCost).HasColumnName("product_cost");
            entity.Property(e => e.ProductName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("product_name");
            entity.Property(e => e.ProductPrice).HasColumnName("product_price");
            entity.Property(e => e.ProductStock).HasColumnName("product_stock");

            entity.HasOne(d => d.Brand).WithMany(p => p.Products)
                .HasForeignKey(d => d.BrandId)
                .HasConstraintName("FK__Products__brand___52593CB8");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Products__catego__5165187F");
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.SaleId).HasName("PK__Sales__E1EB00B27061783A");

            entity.Property(e => e.SaleId).HasColumnName("sale_id");
            entity.Property(e => e.CustomerId)
                .HasMaxLength(50)
                .HasColumnName("customer_id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.ProductId)
                .HasMaxLength(50)
                .HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.SaleDate)
                .HasColumnType("datetime")
                .HasColumnName("sale_date");

            entity.HasOne(d => d.Customer).WithMany(p => p.Sales)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Sales__customer___5441852A");

            entity.HasOne(d => d.Product).WithMany(p => p.Sales)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Sales__product_i__534D60F1");
        });

        modelBuilder.Entity<SalesDaily>(entity =>
        {
            entity.HasKey(e => e.SaleDate).HasName("PK__Sales_Da__387C7FF89D3B31C3");

            entity.ToTable("Sales_Daily");

            entity.Property(e => e.SaleDate).HasColumnName("sale_date");
            entity.Property(e => e.TotalSale)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_sale");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupId);

            entity.Property(e => e.SupId)
                .HasMaxLength(50)
                .HasColumnName("SupID");
            entity.Property(e => e.SupAdd).HasColumnType("text");
            entity.Property(e => e.SupContact).HasMaxLength(100);
            entity.Property(e => e.SupEmail).HasMaxLength(50);
            entity.Property(e => e.SupName).HasMaxLength(100);
            entity.Property(e => e.SupRemark).HasColumnType("text");
            entity.Property(e => e.SupTel).HasMaxLength(50);
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__Transact__85C600AFD66854BA");

            entity.Property(e => e.TransactionId)
                .ValueGeneratedNever()
                .HasColumnName("transaction_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.CustomerId)
                .HasMaxLength(50)
                .HasColumnName("customer_id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.SaleDate).HasColumnName("sale_date");
            entity.Property(e => e.SaleId).HasColumnName("sale_id");

            entity.HasOne(d => d.Customer).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Transacti__custo__5812160E");

            entity.HasOne(d => d.Sale).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.SaleId)
                .HasConstraintName("FK__Transacti__sale___5535A963");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
