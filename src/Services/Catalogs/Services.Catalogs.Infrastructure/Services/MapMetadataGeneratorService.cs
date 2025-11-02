using System.IO.Compression;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Common.Application.Abstractions.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services.Catalogs.Application.Abstractions.Services;
using Services.Catalogs.Application.BusinessLogics.MapObjects.Specifications;
using Services.Catalogs.Application.BusinessLogics.Maps.Specifications;
using Services.Catalogs.Application.BusinessLogics.ObjectActivityTypes.Specifications;
using Services.Catalogs.Application.BusinessLogics.ObjectLocations.Specifications;
using Services.Catalogs.Application.BusinessLogics.TaskLocations.Specifications;
using Services.Catalogs.Domain.Entities.ActivityTypes;
using Services.Catalogs.Domain.Entities.MapObjects;
using Services.Catalogs.Domain.Entities.Maps;
using Services.Catalogs.Domain.Entities.MasterSubjects;
using Services.Catalogs.Domain.Entities.ObjectActivityTypes;
using Services.Catalogs.Domain.Entities.ObjectLocations;
using Services.Catalogs.Domain.Entities.Subjects;
using Services.Catalogs.Domain.Entities.TaskLocations;

namespace Services.Catalogs.Infrastructure.Services;

public class MapMetadataGeneratorService : IMapMetadataGeneratorService
{
    private readonly ILogger<MapMetadataGeneratorService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly string _outputDirectory;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        WriteIndented = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    public MapMetadataGeneratorService(
        ILogger<MapMetadataGeneratorService> logger,
        IConfiguration configuration,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _outputDirectory = configuration["MetadataOutputDirectory"] ?? Path.Combine(Path.GetTempPath(), "MapMetadata");

        // Ensure output directory exists
        Directory.CreateDirectory(_outputDirectory);
    }

    public async Task<string> GenerateMapMetadataAsync(Guid mapId)
    {

        // Get map with all related entities using Specification
        var mapSpec = new MapSpecification(mapId, false);
        Map map = await _unitOfWork.Repository<Map>().GetEntityWithSpec(mapSpec);

        if (map == null)
        {
            throw new InvalidOperationException($"Map {mapId} not found");
        }

        // Get all related entities using Specifications and Generic Repository
        var mapObjectSpec = new MapObjectSpecification(mapId);
        IReadOnlyList<MapObject> mapObjects = await _unitOfWork.Repository<MapObject>().ListAsync(mapObjectSpec);

        var taskLocationSpec = new TaskLocationSpecification(mapId);
        IReadOnlyList<TaskLocation> taskLocations = await _unitOfWork.Repository<TaskLocation>().ListAsync(taskLocationSpec);

        var mapObjectIds = mapObjects.Select(mo => mo.Id).ToList();

        var objectActivityTypeSpec = new ObjectActivityTypeSpecification(mapObjectIds);
        IReadOnlyList<ObjectActivityType> objectActivityTypes = await _unitOfWork.Repository<ObjectActivityType>().ListAsync(objectActivityTypeSpec);

        var objectLocationSpec = new ObjectLocationSpecification(mapObjectIds);
        IReadOnlyList<ObjectLocation> objectLocations = await _unitOfWork.Repository<ObjectLocation>().ListAsync(objectLocationSpec);

        // Get all activity types using ListAllAsync
        IReadOnlyList<ActivityType> activityTypes = await _unitOfWork.Repository<ActivityType>().ListAllAsync();

        // Create temporary directory for this map's metadata
        string mapTempDir = Path.Combine(_outputDirectory, $"Map_{mapId}_{DateTime.UtcNow:yyyyMMdd_HHmmss}");
        Directory.CreateDirectory(mapTempDir);

        // Generate 8 JSON files with data from database
        await GenerateMasterSubjectJson(mapTempDir, map.Subject.MasterSubject);
        await GenerateSubjectJson(mapTempDir, map.Subject);
        await GenerateActivityTypeJson(mapTempDir, activityTypes.ToList());
        await GenerateMapJson(mapTempDir, map);
        await GenerateMapObjectJson(mapTempDir, mapObjects.ToList());
        await GenerateTaskLocationJson(mapTempDir, taskLocations.ToList());
        await GenerateObjectActivityTypeJson(mapTempDir, objectActivityTypes.ToList());
        await GenerateObjectLocationJson(mapTempDir, objectLocations.ToList());

        string zipFileName = $"{map.MapCode}.zip";
        string zipFilePath = Path.Combine(_outputDirectory, zipFileName);

        if (File.Exists(zipFilePath))
        {
            try
            {
                File.Delete(zipFilePath);
            }
            catch (Exception ex)
            {
                Directory.Delete(mapTempDir, true);

                throw new IOException($"Cannot override old file: {zipFilePath}", ex);
            }
        }

        // Create zip file
        await CreateZipFile(mapTempDir, zipFilePath);

        // Clean up temporary directory
        Directory.Delete(mapTempDir, true);

        _logger.LogInformation("Successfully generated metadata zip file: {ZipFilePath}", zipFilePath);
        return zipFilePath;

    }

    private static async Task GenerateMasterSubjectJson(string outputDir, MasterSubject masterSubject)
    {
        MasterSubject[] data = [masterSubject];
        string jsonContent = JsonSerializer.Serialize(data, _jsonOptions);
        await File.WriteAllTextAsync(Path.Combine(outputDir, "MasterSubject.json"), jsonContent, Encoding.UTF8);
    }

    private static async Task GenerateSubjectJson(string outputDir, Subject subject)
    {
        Subject[] data = [subject];
        string jsonContent = JsonSerializer.Serialize(data, _jsonOptions);
        await File.WriteAllTextAsync(Path.Combine(outputDir, "Subject.json"), jsonContent, Encoding.UTF8);
    }

    private static async Task GenerateActivityTypeJson(string outputDir, List<ActivityType> activityTypes)
    {
        string jsonContent = JsonSerializer.Serialize(activityTypes, _jsonOptions);
        await File.WriteAllTextAsync(Path.Combine(outputDir, "ActivityType.json"), jsonContent, Encoding.UTF8);
    }

    private static async Task GenerateMapJson(string outputDir, Map map)
    {
        Map[] data = new[] { map };
        string jsonContent = JsonSerializer.Serialize(data, _jsonOptions);
        await File.WriteAllTextAsync(Path.Combine(outputDir, "Map.json"), jsonContent, Encoding.UTF8);
    }

    private static async Task GenerateMapObjectJson(string outputDir, List<MapObject> mapObjects)
    {
        string jsonContent = JsonSerializer.Serialize(mapObjects, _jsonOptions);
        await File.WriteAllTextAsync(Path.Combine(outputDir, "MapObject.json"), jsonContent, Encoding.UTF8);
    }

    private static async Task GenerateTaskLocationJson(string outputDir, List<TaskLocation> taskLocations)
    {
        string jsonContent = JsonSerializer.Serialize(taskLocations, _jsonOptions);
        await File.WriteAllTextAsync(Path.Combine(outputDir, "TaskLocation.json"), jsonContent, Encoding.UTF8);
    }

    private static async Task GenerateObjectActivityTypeJson(string outputDir, List<ObjectActivityType> objectActivityTypes)
    {
        var dataToSerialize = objectActivityTypes.Select(oat => new
        {
            oat.MapObjectId,
            oat.ActivityTypeId
        });
        string jsonContent = JsonSerializer.Serialize(dataToSerialize, _jsonOptions);
        await File.WriteAllTextAsync(Path.Combine(outputDir, "ObjectActivityType.json"), jsonContent, Encoding.UTF8);
    }

    private static async Task GenerateObjectLocationJson(string outputDir, List<ObjectLocation> objectLocations)
    {
        var dataToSerialize = objectLocations.Select(oat => new
        {
            oat.ObjectId,
            oat.TaskLocationId
        });
        string jsonContent = JsonSerializer.Serialize(dataToSerialize, _jsonOptions);
        await File.WriteAllTextAsync(Path.Combine(outputDir, "ObjectLocation.json"), jsonContent, Encoding.UTF8);
    }

    private static async Task CreateZipFile(string sourceDirectory, string zipFilePath)
    {
        using var fileStream = new FileStream(zipFilePath, FileMode.Create);
        using var archive = new ZipArchive(fileStream, ZipArchiveMode.Create);

        foreach (string file in Directory.GetFiles(sourceDirectory))
        {
            string entryName = Path.GetFileName(file);
            ZipArchiveEntry entry = archive.CreateEntry(entryName);

            using Stream entryStream = entry.Open();
            using var fileStreamToZip = new FileStream(file, FileMode.Open);
            await fileStreamToZip.CopyToAsync(entryStream);
        }
    }
}
