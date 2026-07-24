namespace NINJA.EShop.Discount.Grpc.Models
{
    public class Coupon
    {
        public int Id { get; set; }
        public string ProductKey { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
    }
}