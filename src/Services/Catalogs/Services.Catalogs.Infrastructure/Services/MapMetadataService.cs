using System.IO.Compression;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Common.Application.Abstractions.Data;
using Common.Domain.Entities;
using Common.Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Services.Catalogs.Application.Abstractions.Grpc.Clients;
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

public class MapMetadataService : IMapMetadataService
{
    private readonly ILogger<MapMetadataService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBundleGrpcClient _bundleGrpcClient;
    private readonly string _outputDirectory;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        WriteIndented = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    public MapMetadataService(
        IConfiguration configuration,
        ILogger<MapMetadataService> logger,
        IUnitOfWork unitOfWork,
        IBundleGrpcClient bundleGrpcClient)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _bundleGrpcClient = bundleGrpcClient;
        
        string directoryPath = configuration["MetadataOutputDirectory"] ?? "MapMetadata";
        _outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), directoryPath);
        Directory.CreateDirectory(_outputDirectory);
        
        _logger.LogInformation("Using local metadata folder: {OutputDirectory}", _outputDirectory);
    }

    public async Task<string> GenerateMapMetadataAsync(Guid mapId)
    {
        Map map = await GetMapWithRelatedDataAsync(mapId);
        MapMetadata metadataData = await FetchMapMetadataDataAsync(map);

        string zipFilePath = await CreateMetadataZipFileAsync(map, metadataData);

        try
        {
            string storagePath = await UploadAndUpdateMapAsync(map, zipFilePath);
            return storagePath;
        }
        finally
        {
            DeleteFileIfExists(zipFilePath);
        }
    }

    private async Task<Map> GetMapWithRelatedDataAsync(Guid mapId)
    {
        var mapSpec = new MapSpecification(mapId, false);
        Map? map = await _unitOfWork.Repository<Map>().GetEntityWithSpec(mapSpec);

        if (map == null)
        {
            throw new InvalidOperationException($"Map {mapId} not found");
        }

        return map;
    }

    private async Task<MapMetadata> FetchMapMetadataDataAsync(Map map)
    {
        var mapObjectSpec = new MapObjectSpecification(map.Id);
        IReadOnlyList<MapObject> mapObjects = await _unitOfWork.Repository<MapObject>().ListAsync(mapObjectSpec);

        var taskLocationSpec = new TaskLocationSpecification(map.Id);
        IReadOnlyList<TaskLocation> taskLocations = await _unitOfWork.Repository<TaskLocation>().ListAsync(taskLocationSpec);

        var mapObjectIds = mapObjects.Select(mo => mo.Id).ToList();

        var objectActivityTypeSpec = new ObjectActivityTypeSpecification(mapObjectIds);
        IReadOnlyList<ObjectActivityType> objectActivityTypes = await _unitOfWork.Repository<ObjectActivityType>().ListAsync(objectActivityTypeSpec);

        var objectLocationSpec = new ObjectLocationSpecification(mapObjectIds);
        IReadOnlyList<ObjectLocation> objectLocations = await _unitOfWork.Repository<ObjectLocation>().ListAsync(objectLocationSpec);

        IReadOnlyList<ActivityType> activityTypes = await _unitOfWork.Repository<ActivityType>().ListAllAsync();

        return new MapMetadata(
            map.Subject.MasterSubject,
            map.Subject,
            activityTypes.ToList(),
            mapObjects.ToList(),
            taskLocations.ToList(),
            objectActivityTypes.ToList(),
            objectLocations.ToList());
    }

    private async Task<string> CreateMetadataZipFileAsync(Map map, MapMetadata metadataData)
    {
        string mapTempDir = Path.Combine(_outputDirectory, $"Map_{map.Id}_{DateTime.UtcNow:yyyyMMdd_HHmmss}");
        Directory.CreateDirectory(mapTempDir);

        try
        {
            await GenerateJsonFilesAsync(mapTempDir, map, metadataData);

            string zipFilePath = Path.Combine(_outputDirectory, $"{map.MapCode}.zip");
            DeleteFileIfExists(zipFilePath);

            await CreateZipFile(mapTempDir, zipFilePath);

            return zipFilePath;
        }
        finally
        {
            Directory.Delete(mapTempDir, true);
        }
    }

    private static async Task GenerateJsonFilesAsync(string outputDir, Map map, MapMetadata data)
    {
        await GenerateMasterSubjectJson(outputDir, data.MasterSubject);
        await GenerateSubjectJson(outputDir, data.Subject);
        await GenerateActivityTypeJson(outputDir, data.ActivityTypes);
        await GenerateMapJson(outputDir, map);
        await GenerateMapObjectJson(outputDir, data.MapObjects);
        await GenerateTaskLocationJson(outputDir, data.TaskLocations);
        await GenerateObjectActivityTypeJson(outputDir, data.ObjectActivityTypes);
        await GenerateObjectLocationJson(outputDir, data.ObjectLocations);
    }

    private async Task<string> UploadAndUpdateMapAsync(Map map, string zipFilePath)
    {
        string zipFileName = Path.GetFileName(zipFilePath);

        // Read ZIP file content as bytes
        byte[] fileContent = await File.ReadAllBytesAsync(zipFilePath);
        
        _logger.LogInformation(
            "Read ZIP file for map {MapCode}. Size: {FileSize} bytes", 
            map.MapCode, 
            fileContent.Length);

        // Send file content via gRPC
        BundleUploadResult uploadResult = await _bundleGrpcClient.UploadMetadataAsync(
            fileContent,
            zipFileName,
            map.MapCode);

        if (!uploadResult.Success)
        {
            throw new InvalidOperationException($"Failed to upload metadata: {uploadResult.ErrorMessage}");
        }

        map.MetadataStoragePath = uploadResult.StoragePath;
        _unitOfWork.Repository<Map>().Update(map);
        await _unitOfWork.SaveChangesAsync();

        return uploadResult.StoragePath;
    }

    private void DeleteFileIfExists(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete file: {FilePath}", filePath);
        }
    }

    private static async Task GenerateMasterSubjectJson(string outputDir, MasterSubject masterSubject)
    {
        MasterSubject[] data = [masterSubject];
        string jsonContent = System.Text.Json.JsonSerializer.Serialize(data, _jsonOptions);
        await File.WriteAllTextAsync(Path.Combine(outputDir, "MasterSubject.json"), jsonContent, Encoding.UTF8);
    }

    private static async Task GenerateSubjectJson(string outputDir, Subject subject)
    {
        Subject[] data = [subject];
        string jsonContent = System.Text.Json.JsonSerializer.Serialize(data, _jsonOptions);
        await File.WriteAllTextAsync(Path.Combine(outputDir, "Subject.json"), jsonContent, Encoding.UTF8);
    }

    private static async Task GenerateActivityTypeJson(string outputDir, List<ActivityType> activityTypes)
    {
        string jsonContent = System.Text.Json.JsonSerializer.Serialize(activityTypes, _jsonOptions);
        await File.WriteAllTextAsync(Path.Combine(outputDir, "ActivityType.json"), jsonContent, Encoding.UTF8);
    }

    private static async Task GenerateMapJson(string outputDir, Map map)
    {
        Map[] data = new[] { map };
        string jsonContent = System.Text.Json.JsonSerializer.Serialize(data, _jsonOptions);
        await File.WriteAllTextAsync(Path.Combine(outputDir, "Map.json"), jsonContent, Encoding.UTF8);
    }

    private static async Task GenerateMapObjectJson(string outputDir, List<MapObject> mapObjects)
    {
        string jsonContent = System.Text.Json.JsonSerializer.Serialize(mapObjects, _jsonOptions);
        await File.WriteAllTextAsync(Path.Combine(outputDir, "MapObject.json"), jsonContent, Encoding.UTF8);
    }

    private static async Task GenerateTaskLocationJson(string outputDir, List<TaskLocation> taskLocations)
    {
        string jsonContent = System.Text.Json.JsonSerializer.Serialize(taskLocations, _jsonOptions);
        await File.WriteAllTextAsync(Path.Combine(outputDir, "TaskLocation.json"), jsonContent, Encoding.UTF8);
    }

    private static async Task GenerateObjectActivityTypeJson(string outputDir, List<ObjectActivityType> objectActivityTypes)
    {
        var dataToSerialize = objectActivityTypes.Select(oat => new
        {
            oat.MapObjectId,
            oat.ActivityTypeId
        });
        string jsonContent = System.Text.Json.JsonSerializer.Serialize(dataToSerialize, _jsonOptions);
        await File.WriteAllTextAsync(Path.Combine(outputDir, "ObjectActivityType.json"), jsonContent, Encoding.UTF8);
    }

    private static async Task GenerateObjectLocationJson(string outputDir, List<ObjectLocation> objectLocations)
    {
        var dataToSerialize = objectLocations.Select(oat => new
        {
            oat.ObjectId,
            oat.TaskLocationId
        });
        string jsonContent = System.Text.Json.JsonSerializer.Serialize(dataToSerialize, _jsonOptions);
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

    public async Task<Guid?> SeedSingleFileForMapBundleAsync<T>(string absoluteFilePath) where T : BaseEntity
    {
        Guid mapId = Guid.Empty;
        // 1. If not found, throw exception
        if (!File.Exists(absoluteFilePath))
        {
            throw new FileUploadException($"Seed file not found: {absoluteFilePath}");
        }

        string jsonContent = await File.ReadAllTextAsync(absoluteFilePath);
        var settings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Include,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
        };

        List<T>? entities = JsonConvert.DeserializeObject<List<T>>(jsonContent, settings);

        Type t = typeof(T);

        // check if T is Map then get mapId
        if (t == typeof(Map))
        {
            mapId = (entities?.FirstOrDefault() as Map)!.Id;
        }

        // 2. If file doens't contain data, throw exception
        if (entities is null || !entities.Any())
        {
            return null;
        }

        // 2. Exceptional case for MasterSubject, Subject, and Activity Type,
        if (entities.Any() && (t == typeof(MasterSubject)
                || t == typeof(Subject)
                || t == typeof(ActivityType)))
        {
            foreach (T e in entities)
            {
                // If existing, don't seed it, else add it to the school db
                Guid entityId = (e as BaseEntity)!.Id;
                T? existingEntity = await _unitOfWork.Repository<T>().GetByIdAsync(entityId);
                if (existingEntity != null)
                {
                    continue;
                }

                _unitOfWork.Repository<T>().Add(e);
            }
        }
        else if (entities.Any())
        {
            _unitOfWork.Repository<T>().AddRange(entities);
            
        }

        await _unitOfWork.SaveChangesAsync();
        return mapId;
    }
}

internal sealed record MapMetadata(
    MasterSubject MasterSubject,
    Subject Subject,
    List<ActivityType> ActivityTypes,
    List<MapObject> MapObjects,
    List<TaskLocation> TaskLocations,
    List<ObjectActivityType> ObjectActivityTypes,
    List<ObjectLocation> ObjectLocations);
