using Common.Application.Abstractions.Data;
using Services.Orders.Domain;
using Services.Orders.Domain.Entities.Orders;

namespace Services.Orders.Infrastructure.Data.Database;
public class OrderDbContextSeeder
{
    // ===========================
    // === Fields
    // ===========================
    private readonly IDataSeeder _dataSeeder;

    // ===========================
    // === Constructors
    // ===========================

    public OrderDbContextSeeder(
        IDataSeeder dataSeeder)
    {
        _dataSeeder = dataSeeder;
    }

    // ===========================
    // === Methods
    // ===========================

    /// <summary>
    /// Seeding all data in development mode
    /// </summary>
    /// <returns></returns>
    public async Task SeedDevelopmentAsync()
    {
        // add subsequent files to seed here
        _dataSeeder.AddRelativePath<Order>(AppCts.SeederFilePaths.OrderFilePath);
        _dataSeeder.AddRelativePath<OrderItem>(AppCts.SeederFilePaths.OrderItemFilePath);

        // seeding all tables
        await _dataSeeder.SeedAllTablesAsync();
    }

    /// <summary>
    /// Seedingh all data in staging mode
    /// </summary>
    /// <returns></returns>
    public async Task SeedStagingAsync()
    {
        // add subsequent files to seed here

        // seeding all tables
        await _dataSeeder.SeedAllTablesAsync();
    }

    /// <summary>
    /// Seedingh data for production mode only
    /// </summary>
    /// <returns></returns>
    public async Task SeedProductionAsync()
    {
        // add subsequent files to seed here

        // seeding all tables
        await _dataSeeder.SeedAllTablesAsync();
    }
}

