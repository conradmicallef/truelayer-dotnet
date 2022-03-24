namespace TrueLayer.AcceptanceTests;


public class paymentResponseModel
{
    public class User
    {
        public string id { get; set; } = null!;
    }

    public class CreatePaymentResponseModel
    {
        public string id { get; set; }= null!;
        public User user { get; set; }= null!;
        public string resource_token { get; set; }= null!;
    }
}
