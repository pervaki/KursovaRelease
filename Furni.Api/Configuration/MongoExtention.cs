using Furni.Api.Entities;
using Furni.Api.Repository;
using Furni.Api.Settings;
using MongoDB.Driver;

namespace Furni.Api.Configuration
{
	public static class MongoExtention
	{
		public static IServiceCollection AddMongo(this IServiceCollection services)
		{
			services.AddSingleton(serviceProvider =>
			{ 
				var mongoClient = new MongoClient(DatabaseSettings.ConnectionString);
				return mongoClient.GetDatabase(DatabaseSettings.DatabaseName);
			});
			return services;
		}

		public static IServiceCollection AddMongoRepository<T>(this IServiceCollection services, string collectionName) where T : Entity
		{
			services.AddSingleton<IGenericRepository<T>>(serviceProvider =>
			{
				var database = serviceProvider.GetService<IMongoDatabase>();
				return new GenericRepository<T>(database, collectionName);
			});

			return services;
		}
	}
}
