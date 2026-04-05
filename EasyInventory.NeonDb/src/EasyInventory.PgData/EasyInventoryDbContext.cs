using EasyInventory.PgData.Entities;
using Microsoft.EntityFrameworkCore;

namespace EasyInventory.PgData;

public class EasyInventoryDbContext : DbContext
{
    public EasyInventoryDbContext(DbContextOptions<EasyInventoryDbContext> options) : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<MyBusinessInfo> MyBusinessInfos => Set<MyBusinessInfo>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<PO> POes => Set<PO>();
    public DbSet<POD> PODs => Set<POD>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<SO> SOes => Set<SO>();
    public DbSet<SOD> SODs => Set<SOD>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Customer>(e =>
        {
            e.ToTable("Customer");
            e.Property(x => x.Name).HasMaxLength(50);
            e.Property(x => x.Address).HasMaxLength(100);
            e.Property(x => x.Balance).HasPrecision(18, 2);
            e.Property(x => x.Remarks).HasMaxLength(500);
            e.Property(x => x.bizId).HasMaxLength(50);
        });

        b.Entity<Department>(e =>
        {
            e.ToTable("Department");
            e.Property(x => x.Name).HasMaxLength(50);
            e.Property(x => x.Remarks).HasMaxLength(500);
            e.Property(x => x.bizId).HasMaxLength(50);
        });

        b.Entity<Employee>(e =>
        {
            e.ToTable("Employee");
            e.Property(x => x.FirstName).HasMaxLength(50);
            e.Property(x => x.LastName).HasMaxLength(50);
            e.Property(x => x.Gender).HasMaxLength(50);
            e.Property(x => x.Login).HasMaxLength(50).IsRequired();
            e.Property(x => x.Password).HasMaxLength(50).IsRequired();
            e.Property(x => x.Email).HasMaxLength(50);
            e.Property(x => x.EmployeeTypeId).HasPrecision(3, 0);
            e.Property(x => x.RightId).HasPrecision(4, 0);
            e.Property(x => x.Designation).HasMaxLength(50);
            e.Property(x => x.Casual).HasPrecision(3, 0);
            e.Property(x => x.Earned).HasPrecision(3, 0);
            e.Property(x => x.IsActive).HasPrecision(2, 0);
            e.Property(x => x.bizId).HasMaxLength(50);
            e.HasOne(x => x.Department).WithMany(d => d.Employees).HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.Restrict);
        });

        b.Entity<Location>(e =>
        {
            e.ToTable("Location");
            e.Property(x => x.Name).HasMaxLength(50);
            e.Property(x => x.Remarks).HasMaxLength(500);
            e.Property(x => x.bizId).HasMaxLength(50);
        });

        b.Entity<MyBusinessInfo>(e =>
        {
            e.ToTable("MyBusinessInfo");
            e.Property(x => x.Id).HasPrecision(18, 0);
            e.Property(x => x.BizName).HasMaxLength(500);
            e.Property(x => x.BizAddress).HasMaxLength(500);
            e.Property(x => x.Mobile).HasMaxLength(500);
            e.Property(x => x.Email).HasMaxLength(500);
            e.Property(x => x.Website).HasMaxLength(500);
            e.Property(x => x.Tagline).HasMaxLength(500);
        });

        b.Entity<Payment>(e =>
        {
            e.ToTable("Payment");
            e.Property(x => x.SOId).HasMaxLength(50);
            e.Property(x => x.PaymentMethod).HasMaxLength(50);
            e.Property(x => x.PaymentAmount).HasPrecision(10, 2);
            e.Property(x => x.Remarks).HasMaxLength(500);
            e.HasOne(x => x.SO).WithMany(s => s.Payments).HasForeignKey(x => x.SOId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
        });

        b.Entity<PO>(e =>
        {
            e.ToTable("PO");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasMaxLength(50);
            e.Property(x => x.BillAmount).HasPrecision(18, 2);
            e.Property(x => x.BillPaid).HasPrecision(18, 2);
            e.Property(x => x.Discount).HasPrecision(18, 2);
            e.Property(x => x.Balance).HasPrecision(18, 2);
            e.Property(x => x.PrevBalance).HasPrecision(18, 2);
            e.Property(x => x.PODId).HasPrecision(18, 0);
            e.Property(x => x.PurchaseOrderAmount).HasPrecision(18, 2);
            e.Property(x => x.PurchaseReturnAmount).HasPrecision(18, 2);
            e.Property(x => x.PurchaseOrderQty).HasPrecision(18, 2);
            e.Property(x => x.PurchaseReturnQty).HasPrecision(18, 2);
            e.Property(x => x.PaymentMethod).HasMaxLength(100);
            e.Property(x => x.PaymentDetail).HasMaxLength(500);
            e.Property(x => x.Remarks).HasMaxLength(500);
            e.Property(x => x.Remarks2).HasMaxLength(500);
            e.HasOne(x => x.Employee).WithMany(u => u.POes).HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(x => x.Supplier).WithMany(s => s.POes).HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.SetNull);
        });

        b.Entity<POD>(e =>
        {
            e.ToTable("POD");
            e.HasKey(x => x.Auto);
            e.Property(x => x.Auto).UseIdentityColumn();
            e.Property(x => x.POId).HasMaxLength(50);
            e.Property(x => x.OpeningStock).HasPrecision(18, 2);
            e.Property(x => x.PurchasePrice).HasPrecision(18, 2);
            e.Property(x => x.PerPack).HasPrecision(18, 0);
            e.Property(x => x.Remarks).HasMaxLength(500);
            e.HasOne(x => x.PO).WithMany(p => p.PODs).HasForeignKey(x => x.POId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Product).WithMany(p => p.PODs).HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.SetNull);
        });

        b.Entity<Product>(e =>
        {
            e.ToTable("Product");
            e.Property(x => x.Name).HasMaxLength(500);
            e.Property(x => x.PurchasePrice).HasPrecision(18, 2);
            e.Property(x => x.SalePrice).HasPrecision(18, 2);
            e.Property(x => x.Stock).HasPrecision(18, 2);
            e.Property(x => x.totalPiece).HasPrecision(18, 2);
            e.Property(x => x.RackPosition).HasMaxLength(100);
            e.Property(x => x.Image).HasMaxLength(500);
            e.Property(x => x.Remarks).HasMaxLength(1000);
            e.Property(x => x.BarCode).HasMaxLength(100);
            e.HasOne(x => x.Supplier).WithMany(s => s.Products).HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Location).WithMany(l => l.Products).HasForeignKey(x => x.LocationId).OnDelete(DeleteBehavior.SetNull);
        });

        b.Entity<SO>(e =>
        {
            e.ToTable("SO");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasMaxLength(50);
            e.Property(x => x.BillAmount).HasPrecision(18, 2);
            e.Property(x => x.BillPaid).HasPrecision(18, 2);
            e.Property(x => x.Discount).HasPrecision(18, 2);
            e.Property(x => x.Balance).HasPrecision(18, 2);
            e.Property(x => x.PrevBalance).HasPrecision(18, 2);
            e.Property(x => x.SODId).HasPrecision(18, 0);
            e.Property(x => x.SaleOrderAmount).HasPrecision(18, 2);
            e.Property(x => x.SaleReturnAmount).HasPrecision(18, 2);
            e.Property(x => x.SaleOrderQty).HasPrecision(18, 2);
            e.Property(x => x.SaleReturnQty).HasPrecision(18, 2);
            e.Property(x => x.Profit).HasPrecision(18, 2);
            e.Property(x => x.PaymentMethod).HasMaxLength(100);
            e.Property(x => x.PaymentDetail).HasMaxLength(500);
            e.Property(x => x.Remarks).HasMaxLength(500);
            e.Property(x => x.Remarks2).HasMaxLength(500);
            e.HasOne(x => x.Customer).WithMany(c => c.SOes).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(x => x.Employee).WithMany(u => u.SOes).HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.SetNull);
        });

        b.Entity<SOD>(e =>
        {
            e.ToTable("SOD");
            e.HasKey(x => x.Auto);
            e.Property(x => x.Auto).UseIdentityColumn();
            e.Property(x => x.SOId).HasMaxLength(50);
            e.Property(x => x.OpeningStock).HasPrecision(18, 2);
            e.Property(x => x.SalePrice).HasPrecision(18, 2);
            e.Property(x => x.PurchasePrice).HasPrecision(18, 2);
            e.Property(x => x.PerPack).HasPrecision(18, 0);
            e.Property(x => x.Profit).HasPrecision(18, 2);
            e.Property(x => x.Remarks).HasMaxLength(500);
            e.HasOne(x => x.SO).WithMany(s => s.SODs).HasForeignKey(x => x.SOId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Product).WithMany(p => p.SODs).HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.SetNull);
        });

        b.Entity<Supplier>(e =>
        {
            e.ToTable("Supplier");
            e.Property(x => x.Name).HasMaxLength(50);
            e.Property(x => x.Address).HasMaxLength(100);
            e.Property(x => x.Balance).HasPrecision(10, 2);
            e.Property(x => x.bizId).HasMaxLength(50);
        });
    }
}
