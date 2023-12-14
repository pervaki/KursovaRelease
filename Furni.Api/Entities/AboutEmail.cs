using MongoDB.Bson;

namespace Furni.Api.Entities
{
	public class AboutEmail : Entity
	{
		public override ObjectId _id { get; set; }
		public override string Email {  get; set; }
		public string Name {  get; set; }
	}
}
