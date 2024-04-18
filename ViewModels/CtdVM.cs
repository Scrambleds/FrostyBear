namespace FrostyBear.ViewModels
{
    public class CtdVM
    {
        public string CartId { get; set; } = null!;

        public string ProductId { get; set; } = null!;
        public string ProductName { get; set; }

        public string? BrandName { get; set; }

        public double? CdtlQty { get; set; }

        public double? CdtlPrice { get; set; }

        public double? CdtlMoney { get; set; }
    }
}
