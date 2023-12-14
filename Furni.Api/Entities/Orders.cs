using MongoDB.Bson;

namespace Furni.Api.Entities
{
	public class Order : Entity
	{
		public override ObjectId _id { get; set; }
		public string StringId { get; set; } = "";
		public string IdUser { get; set; }
		public List<string> Products { get; set; }
		public List<int> CountProduct {  get; set; }
		public override string Email { get; set; }
		public decimal Total { get; set; }
	}
}
