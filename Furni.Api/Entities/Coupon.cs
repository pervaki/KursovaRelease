using MongoDB.Bson;

namespace Furni.Api.Entities
{
	public class Coupon : Entity
	{
		public override ObjectId _id { get; set; }
		public string StringId { get; set; } = "";
		public string CouponCode {  get; set; }
		public int Procent {  get; set; }
        public override string Email { get; set; }
    }
}
