using MongoDB.Bson;

namespace Furni.Api.Entities
{
    public abstract class Entity
    {
        public virtual ObjectId _id { get; set; }
		public virtual string Email { get; set; }
    }
}
