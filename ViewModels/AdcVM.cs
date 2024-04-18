namespace FrostyBear.ViewModels
{
    public class AdcVM
    {
        public string CustomerId { get; set; } = null!;

        public string CustomerName { get; set; } = null!;

        public string? CustomerAddress { get; set; }

        public string? CustomerContact { get; set; }

        public string CustomerUsername { get; set; } = null!;

        public string CustomerPassword { get; set; } = null!;

        public DateOnly? Startdate { get; set; }

        public DateOnly? Lastlogin { get; set; }

    }
}
