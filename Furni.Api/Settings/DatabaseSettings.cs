using MongoDB.Bson.Serialization.Serializers;

namespace Furni.Api.Settings
{
	public static class DatabaseSettings
	{
		public static string ConnectionString { get; set; }
		public static string DatabaseName { get; set; }
		public static string CollectionNameUser { get; set; }
		public static string CollectionNameOrders { get; set; }
		public static string CollectionNameProduct { get; set; }
        public static string CollectionNameCoupons { get; set; }
		public static string CollectionNameEmails { get; set; }
		public static string CollectionNameMessage { get; set; }

    }
}
