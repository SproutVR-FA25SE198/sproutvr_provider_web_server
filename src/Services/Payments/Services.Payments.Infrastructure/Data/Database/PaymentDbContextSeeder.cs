using Common.Application.Abstractions.Data;
using Services.Payments.Domain;
using Services.Payments.Domain.Entities.Payments;

namespace Services.Payments.Infrastructure.Data.Database;
public class PaymentDbContextSeeder
{
    // ===========================
    // === Fields
    // ===========================
    private readonly IDataSeeder _dataSeeder;

    // ===========================
    // === Constructors
    // ===========================

    public PaymentDbContextSeeder(
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
        _dataSeeder.AddRelativePath<PaymentTransaction>(AppCts.SeederFilePaths.PaymentFilePath);

        // seeding all tables
        await _dataSeeder.SeedAllTablesAsync();
    }

    /// <summary>
    /// Seedingh all data in staging mode
    /// </summary>
    /// <returns></returns>
#pragma warning disable S4144 // Methods should not have identical implementations
    public async Task SeedStagingAsync()
    {
        // add subsequent files to seed here
        _dataSeeder.AddRelativePath<PaymentTransaction>(AppCts.SeederFilePaths.PaymentFilePath);

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
        _dataSeeder.AddRelativePath<PaymentTransaction>(AppCts.SeederFilePaths.PaymentFilePath);

        // seeding all tables
        await _dataSeeder.SeedAllTablesAsync();
    }
#pragma warning restore S4144 // Methods should not have identical implementations

}
