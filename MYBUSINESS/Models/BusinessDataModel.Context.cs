using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace MYBUSINESS.Models
{
    public partial class BusinessContext : DbContext
    {
        public BusinessContext()
            : base("name=BusinessContext")
        {
            Configuration.LazyLoadingEnabled = true;
            Database.SetInitializer<BusinessContext>(null);
        }

        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<PO> POes { get; set; }
        public virtual DbSet<POD> PODs { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<SO> SOes { get; set; }
        public virtual DbSet<SOD> SODs { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<MyBusinessInfo> MyBusinessInfoes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // PostgreSQL business tables live in the public schema, not EF6's default dbo schema.
            modelBuilder.HasDefaultSchema("public");
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Customer>()
                .ToTable("Customer")
                .Property(e => e.Name).HasMaxLength(50);
            modelBuilder.Entity<Customer>().Property(e => e.Address).HasMaxLength(100);
            modelBuilder.Entity<Customer>().Property(e => e.Balance).HasPrecision(18, 2);
            modelBuilder.Entity<Customer>().Property(e => e.Remarks).HasMaxLength(500);
            modelBuilder.Entity<Customer>().Property(e => e.bizId).HasMaxLength(50);

            modelBuilder.Entity<Department>()
                .ToTable("Department")
                .Property(e => e.Name).HasMaxLength(50);
            modelBuilder.Entity<Department>().Property(e => e.Remarks).HasMaxLength(500);
            modelBuilder.Entity<Department>().Property(e => e.bizId).HasMaxLength(50);

            modelBuilder.Entity<Employee>()
                .ToTable("Employee")
                .Property(e => e.FirstName).HasMaxLength(50);
            modelBuilder.Entity<Employee>().Property(e => e.LastName).HasMaxLength(50);
            modelBuilder.Entity<Employee>().Property(e => e.Gender).HasMaxLength(50);
            modelBuilder.Entity<Employee>().Property(e => e.Login).HasMaxLength(50).IsRequired();
            modelBuilder.Entity<Employee>().Property(e => e.Password).HasMaxLength(50).IsRequired();
            modelBuilder.Entity<Employee>().Property(e => e.Email).HasMaxLength(50);
            modelBuilder.Entity<Employee>().Property(e => e.EmployeeTypeId).HasPrecision(3, 0);
            modelBuilder.Entity<Employee>().Property(e => e.RightId).HasPrecision(4, 0);
            modelBuilder.Entity<Employee>().Property(e => e.Designation).HasMaxLength(50);
            modelBuilder.Entity<Employee>().Property(e => e.Casual).HasPrecision(3, 0);
            modelBuilder.Entity<Employee>().Property(e => e.Earned).HasPrecision(3, 0);
            modelBuilder.Entity<Employee>().Property(e => e.IsActive).HasPrecision(2, 0);
            modelBuilder.Entity<Employee>().Property(e => e.bizId).HasMaxLength(50);
            modelBuilder.Entity<Employee>()
                .HasRequired(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId);

            modelBuilder.Entity<Location>()
                .ToTable("Location")
                .Property(e => e.Name).HasMaxLength(50);
            modelBuilder.Entity<Location>().Property(e => e.Remarks).HasMaxLength(500);
            modelBuilder.Entity<Location>().Property(e => e.bizId).HasMaxLength(50);

            modelBuilder.Entity<MyBusinessInfo>()
                .ToTable("MyBusinessInfo")
                .Property(e => e.Id).HasPrecision(18, 0);
            modelBuilder.Entity<MyBusinessInfo>().Property(e => e.BizName).HasMaxLength(500);
            modelBuilder.Entity<MyBusinessInfo>().Property(e => e.BizAddress).HasMaxLength(500);
            modelBuilder.Entity<MyBusinessInfo>().Property(e => e.Mobile).HasMaxLength(500);
            modelBuilder.Entity<MyBusinessInfo>().Property(e => e.Email).HasMaxLength(500);
            modelBuilder.Entity<MyBusinessInfo>().Property(e => e.Website).HasMaxLength(500);
            modelBuilder.Entity<MyBusinessInfo>().Property(e => e.Tagline).HasMaxLength(500);

            modelBuilder.Entity<Payment>()
                .ToTable("Payment")
                .Property(e => e.SOId).HasMaxLength(50);
            modelBuilder.Entity<Payment>().Property(e => e.PaymentMethod).HasMaxLength(50);
            modelBuilder.Entity<Payment>().Property(e => e.PaymentAmount).HasPrecision(10, 2);
            modelBuilder.Entity<Payment>().Property(e => e.Remarks).HasMaxLength(500);
            modelBuilder.Entity<Payment>()
                .HasOptional(p => p.SO)
                .WithMany(s => s.Payments)
                .HasForeignKey(p => p.SOId);

            modelBuilder.Entity<PO>()
                .ToTable("PO")
                .HasKey(p => p.Id);
            modelBuilder.Entity<PO>().Property(p => p.Id).HasMaxLength(50);
            modelBuilder.Entity<PO>().Property(p => p.BillAmount).HasPrecision(18, 2);
            modelBuilder.Entity<PO>().Property(p => p.BillPaid).HasPrecision(18, 2);
            modelBuilder.Entity<PO>().Property(p => p.Discount).HasPrecision(18, 2);
            modelBuilder.Entity<PO>().Property(p => p.Balance).HasPrecision(18, 2);
            modelBuilder.Entity<PO>().Property(p => p.PrevBalance).HasPrecision(18, 2);
            modelBuilder.Entity<PO>().Property(p => p.PODId).HasPrecision(18, 0);
            modelBuilder.Entity<PO>().Property(p => p.PurchaseOrderAmount).HasPrecision(18, 2);
            modelBuilder.Entity<PO>().Property(p => p.PurchaseReturnAmount).HasPrecision(18, 2);
            modelBuilder.Entity<PO>().Property(p => p.PurchaseOrderQty).HasPrecision(18, 2);
            modelBuilder.Entity<PO>().Property(p => p.PurchaseReturnQty).HasPrecision(18, 2);
            modelBuilder.Entity<PO>().Property(p => p.PaymentMethod).HasMaxLength(100);
            modelBuilder.Entity<PO>().Property(p => p.PaymentDetail).HasMaxLength(500);
            modelBuilder.Entity<PO>().Property(p => p.Remarks).HasMaxLength(500);
            modelBuilder.Entity<PO>().Property(p => p.Remarks2).HasMaxLength(500);
            modelBuilder.Entity<PO>()
                .HasOptional(p => p.Employee)
                .WithMany(e => e.POes)
                .HasForeignKey(p => p.EmployeeId);
            modelBuilder.Entity<PO>()
                .HasOptional(p => p.Supplier)
                .WithMany(s => s.POes)
                .HasForeignKey(p => p.SupplierId);

            modelBuilder.Entity<POD>()
                .ToTable("POD")
                .HasKey(p => p.Auto);
            modelBuilder.Entity<POD>().Property(p => p.Auto).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<POD>().Property(p => p.POId).HasMaxLength(50);
            modelBuilder.Entity<POD>().Property(p => p.OpeningStock).HasPrecision(18, 2);
            modelBuilder.Entity<POD>().Property(p => p.PurchasePrice).HasPrecision(18, 2);
            modelBuilder.Entity<POD>().Property(p => p.PerPack).HasPrecision(18, 0);
            modelBuilder.Entity<POD>().Property(p => p.Remarks).HasMaxLength(500);
            modelBuilder.Entity<POD>()
                .HasOptional(p => p.PO)
                .WithMany(o => o.PODs)
                .HasForeignKey(p => p.POId)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<POD>()
                .HasOptional(p => p.Product)
                .WithMany(p => p.PODs)
                .HasForeignKey(p => p.ProductId);

            modelBuilder.Entity<Product>()
                .ToTable("Product")
                .Property(p => p.Name).HasMaxLength(500);
            modelBuilder.Entity<Product>().Property(p => p.PurchasePrice).HasPrecision(18, 2);
            modelBuilder.Entity<Product>().Property(p => p.SalePrice).HasPrecision(18, 2);
            modelBuilder.Entity<Product>().Property(p => p.Stock).HasPrecision(18, 2);
            modelBuilder.Entity<Product>().Property(p => p.totalPiece).HasPrecision(18, 2);
            modelBuilder.Entity<Product>().Property(p => p.RackPosition).HasMaxLength(100);
            modelBuilder.Entity<Product>().Property(p => p.Image).HasMaxLength(500);
            modelBuilder.Entity<Product>().Property(p => p.Remarks).HasMaxLength(1000);
            modelBuilder.Entity<Product>().Property(p => p.BarCode).HasMaxLength(100);
            modelBuilder.Entity<Product>()
                .HasRequired(p => p.Supplier)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SupplierId);
            modelBuilder.Entity<Product>()
                .HasOptional(p => p.Location)
                .WithMany(l => l.Products)
                .HasForeignKey(p => p.LocationId);

            modelBuilder.Entity<SO>()
                .ToTable("SO")
                .HasKey(s => s.Id);
            modelBuilder.Entity<SO>().Property(s => s.Id).HasMaxLength(50);
            modelBuilder.Entity<SO>().Property(s => s.BillAmount).HasPrecision(18, 2);
            modelBuilder.Entity<SO>().Property(s => s.BillPaid).HasPrecision(18, 2);
            modelBuilder.Entity<SO>().Property(s => s.Discount).HasPrecision(18, 2);
            modelBuilder.Entity<SO>().Property(s => s.Balance).HasPrecision(18, 2);
            modelBuilder.Entity<SO>().Property(s => s.PrevBalance).HasPrecision(18, 2);
            modelBuilder.Entity<SO>().Property(s => s.SODId).HasPrecision(18, 0);
            modelBuilder.Entity<SO>().Property(s => s.SaleOrderAmount).HasPrecision(18, 2);
            modelBuilder.Entity<SO>().Property(s => s.SaleReturnAmount).HasPrecision(18, 2);
            modelBuilder.Entity<SO>().Property(s => s.SaleOrderQty).HasPrecision(18, 2);
            modelBuilder.Entity<SO>().Property(s => s.SaleReturnQty).HasPrecision(18, 2);
            modelBuilder.Entity<SO>().Property(s => s.Profit).HasPrecision(18, 2);
            modelBuilder.Entity<SO>().Property(s => s.PaymentMethod).HasMaxLength(100);
            modelBuilder.Entity<SO>().Property(s => s.PaymentDetail).HasMaxLength(500);
            modelBuilder.Entity<SO>().Property(s => s.Remarks).HasMaxLength(500);
            modelBuilder.Entity<SO>().Property(s => s.Remarks2).HasMaxLength(500);
            modelBuilder.Entity<SO>()
                .HasOptional(s => s.Customer)
                .WithMany(c => c.SOes)
                .HasForeignKey(s => s.CustomerId);
            modelBuilder.Entity<SO>()
                .HasOptional(s => s.Employee)
                .WithMany(e => e.SOes)
                .HasForeignKey(s => s.EmployeeId);

            modelBuilder.Entity<SOD>()
                .ToTable("SOD")
                .HasKey(s => s.Auto);
            modelBuilder.Entity<SOD>().Property(s => s.Auto).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<SOD>().Property(s => s.SOId).HasMaxLength(50);
            modelBuilder.Entity<SOD>().Property(s => s.OpeningStock).HasPrecision(18, 2);
            modelBuilder.Entity<SOD>().Property(s => s.SalePrice).HasPrecision(18, 2);
            modelBuilder.Entity<SOD>().Property(s => s.PurchasePrice).HasPrecision(18, 2);
            modelBuilder.Entity<SOD>().Property(s => s.PerPack).HasPrecision(18, 0);
            modelBuilder.Entity<SOD>().Property(s => s.Profit).HasPrecision(18, 2);
            modelBuilder.Entity<SOD>().Property(s => s.Remarks).HasMaxLength(500);
            modelBuilder.Entity<SOD>()
                .HasOptional(s => s.SO)
                .WithMany(o => o.SODs)
                .HasForeignKey(s => s.SOId)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<SOD>()
                .HasOptional(s => s.Product)
                .WithMany(p => p.SODs)
                .HasForeignKey(s => s.ProductId);

            modelBuilder.Entity<Supplier>()
                .ToTable("Supplier")
                .Property(s => s.Name).HasMaxLength(50);
            modelBuilder.Entity<Supplier>().Property(s => s.Address).HasMaxLength(100);
            modelBuilder.Entity<Supplier>().Property(s => s.Balance).HasPrecision(10, 2);
            modelBuilder.Entity<Supplier>().Property(s => s.bizId).HasMaxLength(50);
        }
    }
}
