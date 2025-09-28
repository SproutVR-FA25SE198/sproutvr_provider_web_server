using System.Reflection;
using Common.Application.Abstractions.Data;
using Common.Infrastructure.Data.Seeders;
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
        _dataSeeder.AddAbsoluteProjectPath(InfrastructureReference.AbsoluteProjectPath);

        // add subsequent files to seed here
        _dataSeeder.AddRelativePath<MasterSubject>(AppCts.SeederFilePaths.MasterSubjectFilePath);
        _dataSeeder.AddRelativePath<Subject>(AppCts.SeederFilePaths.SubjectFilePath);
        _dataSeeder.AddRelativePath<ActivityType>(AppCts.SeederFilePaths.ActivityTypeFilePath);


#pragma warning disable S125 // Sections of code should not be commented out
        //_dataSeeder.AddRelativePath<Map>(AppCts.SeederFilePaths.MapFilePath);
        //_dataSeeder.AddRelativePath<MapObject>(AppCts.SeederFilePaths.MapObjectFilePath);
        //_dataSeeder.AddRelativePath<TaskLocation>(AppCts.SeederFilePaths.TaskLocationFilePath);
        //_dataSeeder.AddRelativePath<ObjectActivityType>(AppCts.SeederFilePaths.ObjectActivityTypeFilePath);
        //_dataSeeder.AddRelativePath<ObjectLocation>(AppCts.SeederFilePaths.ObjectLocationFilePath);

        // seeding all tables
        await _dataSeeder.SeedAllTablesAsync();
#pragma warning restore S125 // Sections of code should not be commented out
    }

    /// <summary>
    /// Seedingh all data in staging mode, table like "activity type" or something then put in here"
    /// </summary>
    /// <returns></returns>
    public async Task SeedStagingAsync()
    {
        _dataSeeder.AddAbsoluteProjectPath(InfrastructureReference.AbsoluteProjectPath);

        // add subsequent files to seed here

#pragma warning disable S125
        //_dataSeeder.AddRelativePath<MasterSubject>(AppCts.SeederFilePaths.MasterSubjectFilePath);
        //_dataSeeder.AddRelativePath<Subject>(AppCts.SeederFilePaths.SubjectFilePath);
        //_dataSeeder.AddRelativePath<ActivityType>(AppCts.SeederFilePaths.ActivityTypeFilePath);
        //_dataSeeder.AddRelativePath<Map>(AppCts.SeederFilePaths.MapFilePath);
        //_dataSeeder.AddRelativePath<MapObject>(AppCts.SeederFilePaths.MapObjectFilePath);
        //_dataSeeder.AddRelativePath<TaskLocation>(AppCts.SeederFilePaths.TaskLocationFilePath);
        //_dataSeeder.AddRelativePath<ObjectActivityType>(AppCts.SeederFilePaths.ObjectActivityTypeFilePath);
        //_dataSeeder.AddRelativePath<ObjectLocation>(AppCts.SeederFilePaths.ObjectLocationFilePath);

        // seeding all tables
        await _dataSeeder.SeedAllTablesAsync();
#pragma warning restore S125
    }

    /// <summary>
    /// Seedingh data for production mode only
    /// </summary>
    /// <returns></returns>
    public async Task SeedProductionAsync()
    {
        _dataSeeder.AddAbsoluteProjectPath(InfrastructureReference.AbsoluteProjectPath);

        // add subsequent files to seed here

#pragma warning disable S125 // Sections of code should not be commented out
        //_dataSeeder.AddRelativePath<MasterSubject>(AppCts.SeederFilePaths.MasterSubjectFilePath);
        _dataSeeder.AddRelativePath<Subject>(AppCts.SeederFilePaths.SubjectFilePath);
        //_dataSeeder.AddRelativePath<ActivityType>(AppCts.SeederFilePaths.ActivityTypeFilePath);
        //_dataSeeder.AddRelativePath<Map>(AppCts.SeederFilePaths.MapFilePath);
        //_dataSeeder.AddRelativePath<MapObject>(AppCts.SeederFilePaths.MapObjectFilePath);
        //_dataSeeder.AddRelativePath<TaskLocation>(AppCts.SeederFilePaths.TaskLocationFilePath);
        //_dataSeeder.AddRelativePath<ObjectActivityType>(AppCts.SeederFilePaths.ObjectActivityTypeFilePath);
        //_dataSeeder.AddRelativePath<ObjectLocation>(AppCts.SeederFilePaths.ObjectLocationFilePath);

        // seeding all tables
        await _dataSeeder.SeedAllTablesAsync();
#pragma warning restore S125 // Sections of code should not be commented out
    }
}
