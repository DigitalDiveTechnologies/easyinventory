namespace EasyInventory.PgData.Entities;

public class Customer
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public decimal? Balance { get; set; }
    public string? Remarks { get; set; }
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string? bizId { get; set; }

    public ICollection<SO> SOes { get; set; } = new List<SO>();
}

public class Department
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Remarks { get; set; }
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string? bizId { get; set; }

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}

public class Employee
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Gender { get; set; }
    public string Login { get; set; } = "";
    public string Password { get; set; } = "";
    public string? Email { get; set; }
    public decimal EmployeeTypeId { get; set; }
    public decimal? RightId { get; set; }
    public byte? RankId { get; set; }
    public int DepartmentId { get; set; }
    public string? Designation { get; set; }
    public byte? Probation { get; set; }
    public DateTime? RegistrationDate { get; set; }
    public decimal? Casual { get; set; }
    public decimal? Earned { get; set; }
    public decimal? IsActive { get; set; }
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string? bizId { get; set; }

    public Department? Department { get; set; }
    public ICollection<PO> POes { get; set; } = new List<PO>();
    public ICollection<SO> SOes { get; set; } = new List<SO>();
}

public class Location
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Remarks { get; set; }
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string? bizId { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}

public class MyBusinessInfo
{
    public decimal Id { get; set; }
    public string? BizName { get; set; }
    public string? BizAddress { get; set; }
    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? Tagline { get; set; }
}

public class Payment
{
    public int Id { get; set; }
    public string? SOId { get; set; }
    public string? PaymentMethod { get; set; }
    public decimal PaymentAmount { get; set; }
    public DateTime ReceivedDate { get; set; }
    public string? Remarks { get; set; }

    public SO? SO { get; set; }
}

public class PO
{
    public string Id { get; set; } = "";
    public int? POSerial { get; set; }
    public decimal BillAmount { get; set; }
    public decimal BillPaid { get; set; }
    public decimal? Discount { get; set; }
    public decimal? Balance { get; set; }
    public decimal? PrevBalance { get; set; }
    public DateTime? Date { get; set; }
    public bool? PurchaseReturn { get; set; }
    public int? SupplierId { get; set; }
    public decimal? PODId { get; set; }
    public decimal? PurchaseOrderAmount { get; set; }
    public decimal? PurchaseReturnAmount { get; set; }
    public decimal? PurchaseOrderQty { get; set; }
    public decimal? PurchaseReturnQty { get; set; }
    public string? PaymentMethod { get; set; }
    public string? PaymentDetail { get; set; }
    public string? Remarks { get; set; }
    public string? Remarks2 { get; set; }
    public int? EmployeeId { get; set; }

    public Employee? Employee { get; set; }
    public Supplier? Supplier { get; set; }
    public ICollection<POD> PODs { get; set; } = new List<POD>();
}

public class POD
{
    public long Auto { get; set; }
    public string? POId { get; set; }
    public int? PODId { get; set; }
    public int? ProductId { get; set; }
    public decimal? OpeningStock { get; set; }
    public int? Quantity { get; set; }
    public decimal? PurchasePrice { get; set; }
    public decimal? PerPack { get; set; }
    public bool? IsPack { get; set; }
    public bool? SaleType { get; set; }
    public string? Remarks { get; set; }

    public PO? PO { get; set; }
    public Product? Product { get; set; }
}

public class Product
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SalePrice { get; set; }
    public decimal? Stock { get; set; }
    public int? PerPack { get; set; }
    public decimal totalPiece { get; set; }
    public bool Saleable { get; set; }
    public string? RackPosition { get; set; }
    public int SupplierId { get; set; }
    public string? Image { get; set; }
    public string? Remarks { get; set; }
    public string? BarCode { get; set; }
    public int? ReOrder { get; set; }
    public int? LocationId { get; set; }
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }

    public Supplier? Supplier { get; set; }
    public Location? Location { get; set; }
    public ICollection<POD> PODs { get; set; } = new List<POD>();
    public ICollection<SOD> SODs { get; set; } = new List<SOD>();
}

public class SO
{
    public string Id { get; set; } = "";
    public int? SOSerial { get; set; }
    public decimal BillAmount { get; set; }
    public decimal BillPaid { get; set; }
    public decimal? Discount { get; set; }
    public decimal? Balance { get; set; }
    public decimal? PrevBalance { get; set; }
    public DateTime? Date { get; set; }
    public bool? SaleReturn { get; set; }
    public int? CustomerId { get; set; }
    public decimal? SODId { get; set; }
    public decimal? SaleOrderAmount { get; set; }
    public decimal? SaleReturnAmount { get; set; }
    public decimal? SaleOrderQty { get; set; }
    public decimal? SaleReturnQty { get; set; }
    public decimal? Profit { get; set; }
    public string? PaymentMethod { get; set; }
    public string? PaymentDetail { get; set; }
    public string? Remarks { get; set; }
    public string? Remarks2 { get; set; }
    public int? EmployeeId { get; set; }

    public Customer? Customer { get; set; }
    public Employee? Employee { get; set; }
    public ICollection<SOD> SODs { get; set; } = new List<SOD>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

public class SOD
{
    public long Auto { get; set; }
    public string? SOId { get; set; }
    public int? SODId { get; set; }
    public int? ProductId { get; set; }
    public decimal? OpeningStock { get; set; }
    public int? Quantity { get; set; }
    public decimal? SalePrice { get; set; }
    public decimal? PurchasePrice { get; set; }
    public decimal? PerPack { get; set; }
    public bool? IsPack { get; set; }
    public bool? SaleType { get; set; }
    public decimal? Profit { get; set; }
    public string? Remarks { get; set; }

    public Product? Product { get; set; }
    public SO? SO { get; set; }
}

public class Supplier
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public decimal? Balance { get; set; }
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string? bizId { get; set; }

    public ICollection<PO> POes { get; set; } = new List<PO>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
