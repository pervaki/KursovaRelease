//using Furni.Api.Mongo.Model;
using MongoDB.Bson;

namespace Furni.UI.Entities
{
	public class User 
	{
		public ObjectId _id { get; set; }
		public string StringId { get; set; } = "";
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public string Number { get; set; }
		public TypeUser TypeUser { get; set; }
		public List<string> Products { get; set; }
		public List<string> Orders { get; set; }
		public User(TypeUser typeUser)
		{
			TypeUser = typeUser;
		}

	}
	public enum TypeUser
	{
		Client,
		Admin
	}
}
