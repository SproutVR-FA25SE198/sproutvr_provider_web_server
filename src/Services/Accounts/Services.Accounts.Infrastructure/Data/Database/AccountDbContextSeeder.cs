using Common.Application.Abstractions.Data;
using Services.Accounts.Application.Abstractions.Data.Seeders;
using Services.Accounts.Domain;
using Services.Accounts.Domain.Entities.OrganizationRegisterRequests;

namespace Services.Accounts.Infrastructure.Data.Database;
public class AccountDbContextSeeder
{
    // ===========================
    // === Fields
    // ===========================
    private readonly IDataSeeder _dataSeeder;
    private readonly IAccountSeeder _accountSeeder;

    // ===========================
    // === Constructors
    // ===========================

    public AccountDbContextSeeder(
        IDataSeeder dataSeeder,
        IAccountSeeder accountSeeder)
    {
        _dataSeeder = dataSeeder;
        _accountSeeder = accountSeeder;
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
        _dataSeeder.AddRelativePath<OrganizationRegisterRequest>(AppCts.SeederFilePaths.OrganizationRegisterRequestFilePath);
        await _accountSeeder.SeedAsync();

        // seeding all tables
        await _dataSeeder.SeedAllTablesAsync();
    }

    /// <summary>
    /// Seedingh all data in staging mode, table like "activity type" or something then put in here"
    /// </summary>
    /// <returns></returns>
#pragma warning disable S4144 // Methods should not have identical implementations
    public async Task SeedStagingAsync()
    {
        // add subsequent files to seed here
        _dataSeeder.AddRelativePath<OrganizationRegisterRequest>(AppCts.SeederFilePaths.OrganizationRegisterRequestFilePath);
        await _accountSeeder.SeedAsync();
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
        _dataSeeder.AddRelativePath<OrganizationRegisterRequest>(AppCts.SeederFilePaths.OrganizationRegisterRequestFilePath);
        await _accountSeeder.SeedAsync();
        // seeding all tables
        await _dataSeeder.SeedAllTablesAsync();
    }
}
#pragma warning restore S4144 // Methods should not have identical implementations

