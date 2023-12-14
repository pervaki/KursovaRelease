using MongoDB.Bson;

namespace Furni.Api.Entities
{
	public class User : Entity
	{
		public override ObjectId _id { get; set; }
		public string StringId { get; set; } = "";
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public override string Email { get; set; }
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
