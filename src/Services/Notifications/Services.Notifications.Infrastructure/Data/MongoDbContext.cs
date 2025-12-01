using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Services.Notifications.Domain.Entities;

namespace Services.Notifications.Infrastructure.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("MongoDB") 
            ?? throw new InvalidOperationException("MongoDB connection string is not configured.");
        string databaseName = configuration["MongoDB:DatabaseName"] 
            ?? throw new InvalidOperationException("MongoDB database name is not configured.");

        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<Notification> Notifications => _database.GetCollection<Notification>("notifications");
}

