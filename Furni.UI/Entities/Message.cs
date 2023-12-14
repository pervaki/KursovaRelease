using MongoDB.Bson;

namespace Furni.UI.Entities
{
	public class Message
	{
		public ObjectId _id {  get; set; }
		public string StringId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string MessageAboutProblem { get; set; }
	}
}
