using System.Reflection;
using Common.Application.Abstractions.Data;
using Common.Infrastructure.Data.Seeders;
using Services.Catalogs.Domain;
using Services.Catalogs.Domain.Entities.Map;

namespace Services.Catalogs.Infrastructure.Data.Database;

public class CatalogDbContextSeeder
{
    private readonly IDataSeeder _dataSeeder;

    public CatalogDbContextSeeder(
        IDataSeeder dataSeeder)
    {
        _dataSeeder = dataSeeder;
    }
    public async Task SeedAsync()
    {
        _dataSeeder.AddAbsoluteProjectPath(InfrastructureReference.AbsoluteProjectPath);

        // add subsequent files to seed here
        _dataSeeder.AddRelativePath<Map>(AppCts.SeederFilePaths.MapFilePath);

        // seeding all tables
        await _dataSeeder.SeedAllTablesAsync();
    }
}
