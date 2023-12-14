using MongoDB.Bson;

namespace Furni.Api.Entities
{
	public class Message : Entity
	{
		public override ObjectId _id { get; set; }
		public string StringId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public override string Email { get; set; }
		public string MessageAboutProblem { get; set; }
	}
}
