using System.IO.Compression;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Common.Application.Abstractions.Data;
using Microsoft.Extensions.Logging;
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

public class MapMetadataGeneratorService : IMapMetadataGeneratorService
{
    private readonly ILogger<MapMetadataGeneratorService> _logger;
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

    public MapMetadataGeneratorService(
        ILogger<MapMetadataGeneratorService> logger,
        IUnitOfWork unitOfWork,
        IBundleGrpcClient bundleGrpcClient)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _bundleGrpcClient = bundleGrpcClient;
        
        // Use local folder in Catalog service (no need for shared path since we send file content via gRPC)
        _outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "MapMetadata");
        Directory.CreateDirectory(_outputDirectory);
        
        _logger.LogInformation("Using local metadata folder: {OutputDirectory}", _outputDirectory);
    }

    public async Task<string> GenerateMapMetadataAsync(Guid mapId)
    {
        Map map = await GetMapWithRelatedDataAsync(mapId);
        MapMetadataData metadataData = await FetchMapMetadataDataAsync(map);

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

    private async Task<MapMetadataData> FetchMapMetadataDataAsync(Map map)
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

        return new MapMetadataData(
            map.Subject.MasterSubject,
            map.Subject,
            activityTypes.ToList(),
            mapObjects.ToList(),
            taskLocations.ToList(),
            objectActivityTypes.ToList(),
            objectLocations.ToList());
    }

    private async Task<string> CreateMetadataZipFileAsync(Map map, MapMetadataData metadataData)
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

    private static async Task GenerateJsonFilesAsync(string outputDir, Map map, MapMetadataData data)
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

        // Send file content via gRPC (not path)
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

internal sealed record MapMetadataData(
    MasterSubject MasterSubject,
    Subject Subject,
    List<ActivityType> ActivityTypes,
    List<MapObject> MapObjects,
    List<TaskLocation> TaskLocations,
    List<ObjectActivityType> ObjectActivityTypes,
    List<ObjectLocation> ObjectLocations);
