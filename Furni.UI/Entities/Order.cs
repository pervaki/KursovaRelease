using MongoDB.Bson;

namespace Furni.UI.Entities
{
	public class Order
	{
		public ObjectId _id { get; set; }
		public string StringId { get; set; } = "";
		public string IdUser { get; set; }

		public List<string> Products { get; set; }

		public List<int> CountProduct { get; set; }
		public string Email { get; set; }
		public decimal Total { get; set; }
	}
}
