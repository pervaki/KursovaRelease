using MongoDB.Bson;

namespace Furni.UI.Entities
{
    public class Coupon
    {
        public ObjectId _id { get; set; }
        public string StringId { get; set; } = "";
        public string CouponCode { get; set; }
        public int Procent { get; set; }
        public string Email { get; set; }
    }
}
