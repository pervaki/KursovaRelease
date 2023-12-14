using Furni.Api.Email;
using Furni.Api.Entities;
using Furni.Api.Settings;

namespace Furni.Api.Configuration
{
	public static class DependencySturtup
	{
		public static void AddDependency(this WebApplicationBuilder builder)
		{
			builder.Configuration.SetDatabaseSettings();

			builder.Services.AddMongo()
				.AddMongoRepository<User>(DatabaseSettings.CollectionNameUser)
				.AddMongoRepository<Order>(DatabaseSettings.CollectionNameOrders)
				.AddMongoRepository<Product>(DatabaseSettings.CollectionNameProduct)
				.AddMongoRepository<AboutEmail>(DatabaseSettings.CollectionNameEmails)
				.AddMongoRepository<Message>(DatabaseSettings.CollectionNameMessage)
				.AddMongoRepository<Coupon>(DatabaseSettings.CollectionNameCoupons);

			builder.Services.AddTransient<IEmailSender, EmailSender>();
		}

		private static void SetDatabaseSettings(this IConfiguration configuration)
		{
			DatabaseSettings.ConnectionString = configuration.GetValue<string>("ConnectionString:MongoDB");
			DatabaseSettings.DatabaseName = configuration.GetValue<string>("ConnectionString:DataBase");
			DatabaseSettings.CollectionNameUser = configuration.GetValue<string>("MongoCollections:Users");
			DatabaseSettings.CollectionNameOrders = configuration.GetValue<string>("MongoCollections:Orders");
			DatabaseSettings.CollectionNameProduct = configuration.GetValue<string>("MongoCollections:Products");
			DatabaseSettings.CollectionNameCoupons = configuration.GetValue<string>("MongoCollections:Coupons");
			DatabaseSettings.CollectionNameEmails = configuration.GetValue<string>("MongoCollections:Emails");
			DatabaseSettings.CollectionNameMessage = configuration.GetValue<string>("MongoCollections:Messages");

		}
	}
}
