namespace PriceSettingsLibrary
{
    public class PriceSettings
    {
        public decimal PricePerProduct { get; set; }
        public decimal VatRate { get; set; } // VAT rate (e.g., 25 for 25%)
    }

}