using Furni.Api.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Furni.Api.Repository
{
	public class GenericRepository<T> : IGenericRepository<T> where T : Entity
    {
		private readonly IMongoCollection<T> mongoCollection;
		public GenericRepository(IMongoDatabase database, string collectionName)
		{
			mongoCollection = database.GetCollection<T>(collectionName);
		}
		public async Task Add(T entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			await mongoCollection.InsertOneAsync(entity);
		}

		public async Task Delete(ObjectId id)
		{
			await mongoCollection.DeleteOneAsync(u => u._id == id);
		}

		public async Task<T> Get(ObjectId id)
		{
			return await mongoCollection.Find(u => u._id == id).FirstOrDefaultAsync();
		}

		public async Task<T> Get(string email) 
		{
			return await mongoCollection.Find(u => u.Email == email).FirstOrDefaultAsync();
		}

		public IEnumerable<T> GetAll()
		{
			return mongoCollection.Find(new BsonDocument()).ToList();
		}

		public async Task Update(T user)
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			await mongoCollection.ReplaceOneAsync(u => u._id == user._id, user);
		}
	}
}
