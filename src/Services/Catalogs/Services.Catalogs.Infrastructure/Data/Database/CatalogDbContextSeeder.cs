using Common.Application.Abstractions.Data;
using Services.Catalogs.Domain;
using Services.Catalogs.Domain.Entities.MasterSubjects;
using Services.Catalogs.Domain.Entities.Subjects;
using Services.Catalogs.Domain.Entities.ActivityTypes;
using Services.Catalogs.Domain.Entities.Maps;
using Services.Catalogs.Domain.Entities.MapObjects;
using Services.Catalogs.Domain.Entities.TaskLocations;
using Services.Catalogs.Domain.Entities.ObjectActivityTypes;
using Services.Catalogs.Domain.Entities.ObjectLocations;

namespace Services.Catalogs.Infrastructure.Data.Database;

public class CatalogDbContextSeeder
{
    // ===========================
    // === Fields
    // ===========================
    private readonly IDataSeeder _dataSeeder;

    // ===========================
    // === Constructors
    // ===========================

    public CatalogDbContextSeeder(
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
        _dataSeeder.AddRelativePath<MasterSubject>(AppCts.SeederFilePaths.MasterSubjectFilePath);
        _dataSeeder.AddRelativePath<Subject>(AppCts.SeederFilePaths.SubjectFilePath);
        _dataSeeder.AddRelativePath<ActivityType>(AppCts.SeederFilePaths.ActivityTypeFilePath);
        _dataSeeder.AddRelativePath<Map>(AppCts.SeederFilePaths.MapFilePath);
        _dataSeeder.AddRelativePath<MapObject>(AppCts.SeederFilePaths.MapObjectFilePath);
        _dataSeeder.AddRelativePath<TaskLocation>(AppCts.SeederFilePaths.TaskLocationFilePath);
        _dataSeeder.AddRelativePath<ObjectActivityType>(AppCts.SeederFilePaths.ObjectActivityTypeFilePath);
        _dataSeeder.AddRelativePath<ObjectLocation>(AppCts.SeederFilePaths.ObjectLocationFilePath);

        // seeding all tables
        await _dataSeeder.SeedAllTablesAsync();
    }

    /// <summary>
    /// Seedingh all data in staging mode, table like "activity type" or something then put in here"
    /// </summary>
    /// <returns></returns>
    public async Task SeedStagingAsync()
    {
        // add subsequent files to seed here
        _dataSeeder.AddRelativePath<MasterSubject>(AppCts.ProdSeederFilePaths.MasterSubjectFilePath);
        _dataSeeder.AddRelativePath<Subject>(AppCts.ProdSeederFilePaths.SubjectFilePath);
        _dataSeeder.AddRelativePath<ActivityType>(AppCts.ProdSeederFilePaths.ActivityTypeFilePath);
        _dataSeeder.AddRelativePath<Map>(AppCts.ProdSeederFilePaths.MapFilePath);
        _dataSeeder.AddRelativePath<MapObject>(AppCts.ProdSeederFilePaths.MapObjectFilePath);
        _dataSeeder.AddRelativePath<TaskLocation>(AppCts.ProdSeederFilePaths.TaskLocationFilePath);
        _dataSeeder.AddRelativePath<ObjectActivityType>(AppCts.ProdSeederFilePaths.ObjectActivityTypeFilePath);
        _dataSeeder.AddRelativePath<ObjectLocation>(AppCts.ProdSeederFilePaths.ObjectLocationFilePath);

        // seeding all tables
        await _dataSeeder.SeedAllTablesAsync();
    }

    /// <summary>
    /// Seedingh data for production mode only
    /// </summary>
    /// <returns></returns>
#pragma warning disable S4144 // Methods should not have identical implementations
    public async Task SeedProductionAsync()
#pragma warning restore S4144 // Methods should not have identical implementations
    {
        // add subsequent files to seed here
        _dataSeeder.AddRelativePath<MasterSubject>(AppCts.ProdSeederFilePaths.MasterSubjectFilePath);
        _dataSeeder.AddRelativePath<Subject>(AppCts.ProdSeederFilePaths.SubjectFilePath);
        _dataSeeder.AddRelativePath<ActivityType>(AppCts.ProdSeederFilePaths.ActivityTypeFilePath);
        _dataSeeder.AddRelativePath<Map>(AppCts.ProdSeederFilePaths.MapFilePath);
        _dataSeeder.AddRelativePath<MapObject>(AppCts.ProdSeederFilePaths.MapObjectFilePath);
        _dataSeeder.AddRelativePath<TaskLocation>(AppCts.ProdSeederFilePaths.TaskLocationFilePath);
        _dataSeeder.AddRelativePath<ObjectActivityType>(AppCts.ProdSeederFilePaths.ObjectActivityTypeFilePath);
        _dataSeeder.AddRelativePath<ObjectLocation>(AppCts.ProdSeederFilePaths.ObjectLocationFilePath);

        // seeding all tables
        await _dataSeeder.SeedAllTablesAsync();
    }
}
