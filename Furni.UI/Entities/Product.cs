using MongoDB.Bson;

namespace Furni.UI.Entities
{
	public class Product
	{
		public ObjectId _id { get; set; }
		public string StringId { get; set; } = "";
		public string Title { get; set; }
		public decimal Price { get; set; }
		public string Image { get; set; }
		public string Email { get; set; }
		public int Count {  get; set; }

	}
}
