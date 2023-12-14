using MongoDB.Bson;

namespace Furni.Api.Repository
{
    public interface IGenericRepository<T>
    {

        IEnumerable<T> GetAll();
        Task Add(T user);
        Task Update(T user);
        Task Delete(ObjectId id);
        Task<T> Get(ObjectId id);
        Task<T> Get(string email);

	}
}
