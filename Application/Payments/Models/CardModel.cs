namespace Biobrain.Application.Payments.Models
{
	public class CardModel
	{
		public string CardNumber { get; set; }
		public int ExpiryMonth { get; set; }
		public int ExpiryYear { get; set; }
		public string Cvc { get; set; }
		public string CardholderName { get; set; }
		public string AddressLine1 { get; set; }
		public string Country { get; set; }
		public string City { get; set; }
    }
}
