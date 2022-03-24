namespace TrueLayer.AcceptanceTests;


public class paymentRequestModel
{
    public class ProviderSelection
    {
        public string type { get; set; } =null !;
    }

    public class Beneficiary
    {
        public string type { get; set; } =null !;
        public string name { get; set; } =null !;
        public string reference { get; set; } =null !;
        public string merchant_account_id { get; set; } =null !;
    }

    public class PaymentMethod
    {
        public string type { get; set; } =null !;
        public ProviderSelection provider_selection { get; set; } =null !;
        public Beneficiary beneficiary { get; set; } =null !;
    }

    public class User
    {
        public string id { get; set; } =null !;
        public string name { get; set; } =null !;
        public string email { get; set; } =null !;
    }

    public class paymentRequest
    {
        public int amount_in_minor { get; set; }
        public string currency { get; set; } =null !;
        public PaymentMethod payment_method { get; set; } =null !;
        public User user { get; set; } =null !;
    }
}
