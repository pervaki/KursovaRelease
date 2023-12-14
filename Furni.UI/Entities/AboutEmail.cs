using MongoDB.Bson;

namespace Furni.UI.Entities
{
	public class AboutEmail
	{
		public ObjectId _id { get; set; }
		public string Email { get; set; }
		public string Name { get; set; }
	}
}
