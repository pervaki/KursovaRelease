using MongoDB.Bson;

namespace Furni.Api.Entities
{
	public class Product : Entity
	{
		public override ObjectId _id { get; set; }
		public string StringId { get; set; } = "";
		public string Title { get; set; }
		public decimal Price { get; set; }
		public string Image { get; set; }
        public override string Email { get; set; }
		public int Count { get; set; }

	}
}
