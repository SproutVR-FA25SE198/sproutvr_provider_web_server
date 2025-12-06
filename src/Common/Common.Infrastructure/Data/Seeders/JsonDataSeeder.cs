using Common.Application.Abstractions.Data;
using Common.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Common.Infrastructure.Data.Seeders;

public class JsonDataSeeder<TDbContext> : IDataSeeder
    where TDbContext : DbContext
{
    // =====================================
    // === Fields & Props
    // =====================================

    private readonly IFileReader _fileReader;
    private readonly List<(string relativeFilePath, Type entityType)> _seedFileInfors = new();
    private readonly TDbContext _dbContext;
    private readonly ILogger<JsonDataSeeder<TDbContext>> _logger;

    // =====================================
    // === Constructors
    // =====================================

    public JsonDataSeeder(IFileReader fileReader, TDbContext dbContext, ILogger<JsonDataSeeder<TDbContext>> logger)
    {
        _fileReader = fileReader;
        _dbContext = dbContext;
        _logger = logger;
    }

    // =====================================
    // === Methods
    // =====================================

    /// <summary>
    /// Add the relative path of the json file as longh as the entity type
    /// </summary>
    /// <param name="relativefilePath"></param>
    public void AddRelativePath<T>(string relativefilePath) where T : BaseEntity
    {
        _seedFileInfors.Add((relativefilePath, typeof(T)));
    }

    /// <summary>
    /// Parsing the json file into the list object with the specific entity type
    /// </summary>
    /// <param name="absoluteFilePath"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private async Task<IEnumerable<object>> ParseJsonToObject(string absoluteFilePath, Type entityType)
    {
        try
        {
            string json = await _fileReader.ReadFileAsync(absoluteFilePath);
            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Include,
                MissingMemberHandling = MissingMemberHandling.Error,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            };

            Type listType = typeof(List<>).MakeGenericType(entityType);
            var data = JsonConvert.DeserializeObject(json, listType, settings) as IEnumerable<object>;

            return data ?? Enumerable.Empty<object>();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Seed all entities from json files
    /// </summary>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    public async Task SeedAllTablesAsync()
    {
        // If no path provided, return
        if (!_seedFileInfors.Any())
        {
            _logger.LogWarning("Does not have files");
            return;
        }

        // Seed data based on entity
        if (await _dbContext.Database.CanConnectAsync())
        {
            foreach ((string relativeFilePath, Type entityType) in _seedFileInfors)
            {
                // base directory
                // /app in Docker
                // /bin/Debug/net9.0 in local
                string absoluteFilePath = Path.Combine(AppContext.BaseDirectory, relativeFilePath);

                // Using reflection to call the genericMethod method
                System.Reflection.MethodInfo? method = typeof(DbContext).GetMethod("Set", Type.EmptyTypes);
                System.Reflection.MethodInfo? genericMethod = method?.MakeGenericMethod(entityType);
                object? dbSet = genericMethod?.Invoke(_dbContext, null);
                IEnumerable<object> entities = await ParseJsonToObject(absoluteFilePath, entityType);

                // Skip seeding if there are existing records
                dynamic queryable = dbSet as IQueryable;
                if (await EntityFrameworkQueryableExtensions.AnyAsync(queryable))
                {
                    continue;
                }

                // reflect to get the AddRange method
                System.Reflection.MethodInfo? addRangeMethod = dbSet?.GetType().GetMethod("AddRange", new[] { typeof(IEnumerable<>).MakeGenericType(entityType) });
                if (addRangeMethod is null)
                {
                    throw new InvalidOperationException($"Cannot find AddRange method for type {entityType.Name}");
                }
                addRangeMethod.Invoke(dbSet, new object[] { entities });
            }
        }

        // Save change to the database
        await _dbContext.SaveChangesAsync();
    }
}
