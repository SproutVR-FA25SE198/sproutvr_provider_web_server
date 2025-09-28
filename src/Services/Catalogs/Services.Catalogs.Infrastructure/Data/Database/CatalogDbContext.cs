using Common.Application.Abstractions.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Services.Catalogs.Domain.Entities.MasterSubjects;
using Services.Catalogs.Domain.Entities.Subjects;
using Services.Catalogs.Domain.Entities.ActivityTypes;
using Services.Catalogs.Domain.Entities.Maps;
using Services.Catalogs.Domain.Entities.MapObjects;
using Services.Catalogs.Domain.Entities.TaskLocations;
using Services.Catalogs.Domain.Entities.ObjectActivityTypes;
using Services.Catalogs.Domain.Entities.ObjectLocations;

namespace Services.Catalogs.Infrastructure.Data.Database;

public sealed class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<MasterSubject> MasterSubjects { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<ActivityType> ActivityTypes { get; set; }
    public DbSet<Map> Maps { get; set; }
    public DbSet<MapObject> MapObjects { get; set; }
    public DbSet<TaskLocation> TaskLocations { get; set; }
    public DbSet<ObjectActivityType> ObjectActivityTypes { get; set; }
    public DbSet<ObjectLocation> ObjectLocations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Add Outbox Pattern
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}
