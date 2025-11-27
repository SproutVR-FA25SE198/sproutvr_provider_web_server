using System.IO.Compression;
using Common.Application.Abstractions.Data;
using Common.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Services.Catalogs.Application.Abstractions.Services;
using Services.Catalogs.Domain.Entities.ActivityTypes;
using Services.Catalogs.Domain.Entities.MapObjects;
using Services.Catalogs.Domain.Entities.Maps;
using Services.Catalogs.Domain.Entities.MasterSubjects;
using Services.Catalogs.Domain.Entities.ObjectActivityTypes;
using Services.Catalogs.Domain.Entities.ObjectLocations;
using Services.Catalogs.Domain.Entities.Subjects;
using Services.Catalogs.Domain.Entities.TaskLocations;

namespace Services.Catalogs.Application.BusinessLogics.Maps.Features.CreateMap;

public sealed class CreateMapCommandHandler : IRequestHandler<CreateMapCommand, CreateMapResponseDto>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMapMetadataService _mapMetadataService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly string _outputDirectory;
    private string _tempExtractedPath;
    private readonly ILogger<CreateMapCommandHandler> _logger;
    public CreateMapCommandHandler(
        IConfiguration configuration,
        IServiceProvider serviceProvider,
        IMapMetadataService mapMetadataService,
        IUnitOfWork unitOfWork,
        ILogger<CreateMapCommandHandler> logger)
    {
        _serviceProvider = serviceProvider;
        _mapMetadataService = mapMetadataService;
        _unitOfWork = unitOfWork;
        _logger = logger;
        string directoryPath = configuration["TempImportDirectory"] ?? "TempImport";
        _outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), directoryPath);
    }
    public async Task<CreateMapResponseDto> Handle(CreateMapCommand request, CancellationToken cancellationToken)
    {
        // 1. Currently, create a temp directory for seeding
        Directory.CreateDirectory(_outputDirectory);

        // 2. Download the map bundle from the given URL
        _tempExtractedPath = Path.Combine(_outputDirectory, $"Map_{DateTime.UtcNow:yyyyMMdd_HHmmss}_extracted");
        await UnzipMapBundleAsync(request, _tempExtractedPath, cancellationToken);

        _logger.LogDebug("Successfully downloaded and extracted to {Path}", _tempExtractedPath);

        // 3. Process the extracted files and seed the map into the system
        Guid? mapId = await SeedDataFromFolderAsync(cancellationToken);

        // 4. Cleanup temp seeding folder after successfilly seeding
        CleanUp(_tempExtractedPath);

        Map map = await _unitOfWork.Repository<Map>().GetByIdAsync(mapId.Value);

        // update map with data inputted by admin
        map.Status = MapStatus.Active;
        map.Price = decimal.Parse(
            request.Price,
            System.Globalization.NumberStyles.Currency,
            System.Globalization.CultureInfo.InvariantCulture
        );
        map.SubjectId = Guid.Parse(request.SubjectId);

        _unitOfWork.Repository<Map>().Update(map);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateMapResponseDto() { MapId = mapId };
    }

    private async Task<Guid?> SeedDataFromFolderAsync(CancellationToken cancellationToken)
    {
        using IServiceScope scope = _serviceProvider.CreateScope();
        await using IDbContextTransaction transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            // Seed .json file data into the database
            await _mapMetadataService.SeedSingleFileForMapBundleAsync<MasterSubject>(Path.Combine(_tempExtractedPath, "MasterSubject.json"));
            await _mapMetadataService.SeedSingleFileForMapBundleAsync<Subject>(Path.Combine(_tempExtractedPath, "Subject.json"));
            await _mapMetadataService.SeedSingleFileForMapBundleAsync<ActivityType>(Path.Combine(_tempExtractedPath, "ActivityType.json"));
            Guid? mapId = await _mapMetadataService.SeedSingleFileForMapBundleAsync<Map>(Path.Combine(_tempExtractedPath, "Map.json"));
            await _mapMetadataService.SeedSingleFileForMapBundleAsync<MapObject>(Path.Combine(_tempExtractedPath, "MapObject.json"));
            await _mapMetadataService.SeedSingleFileForMapBundleAsync<TaskLocation>(Path.Combine(_tempExtractedPath, "TaskLocation.json"));
            await _mapMetadataService.SeedSingleFileForMapBundleAsync<ObjectActivityType>(Path.Combine(_tempExtractedPath, "ObjectActivityType.json"));
            await _mapMetadataService.SeedSingleFileForMapBundleAsync<ObjectLocation>(Path.Combine(_tempExtractedPath, "ObjectLocation.json"));

            // Save changes and commit transaction to DB
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(transaction, cancellationToken);
            return mapId;
        }
        catch (Exception ex)
        {
            CleanUp(_tempExtractedPath);
            await _unitOfWork.RollbackTransactionAsync(transaction, cancellationToken);
            throw new OperationFailedException($"Failed to seed map data from folder '{_tempExtractedPath}': {ex.Message}");
        }
    }

    private async Task UnzipMapBundleAsync(
    CreateMapCommand request,
    string extractedPath,
    CancellationToken cancellationToken)
    {
        // Kiểm tra và tạo thư mục đích nếu chưa tồn tại
        if (string.IsNullOrWhiteSpace(extractedPath))
        {
            throw new ArgumentException("Extracted path cannot be null or empty.", nameof(extractedPath));
        }
        Directory.CreateDirectory(extractedPath);

        IFormFile bundleFile = request.MapDataFile;

        try
        {
            if (bundleFile == null || bundleFile.Length == 0)
            {
                throw new DirectoryNotFoundException("Map data file not found!");
            }

            if (!Path.GetExtension(bundleFile.FileName)
                .Equals(".zip", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidDataException("Map data file must be in .zip format");
            }

            // read file from request
            using var memoryStream = new MemoryStream();
            await bundleFile.CopyToAsync(memoryStream, cancellationToken);
            memoryStream.Position = 0;

            // extract zip file
            using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read);

            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                string destinationFileName = Path.GetFileName(entry.FullName);
                if (string.IsNullOrEmpty(destinationFileName))
                {
                    continue;
                }

                string destinationPath = Path.Combine(extractedPath, destinationFileName);

                using var destinationStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None);
                await entry.Open().CopyToAsync(destinationStream, cancellationToken);
            }

        }
        catch (DirectoryNotFoundException)
        {
            CleanUp(extractedPath);
            throw;
        }
        catch (InvalidDataException)
        {
            CleanUp(extractedPath);
            throw;
        }
        catch (Exception ex)
        {
            CleanUp(extractedPath);
            throw new OperationFailedException($"Failed to unzip the map bundle: {ex.Message}");
        }
    }

    private void CleanUp(string path)
    {
        if (Directory.Exists(path))
        {
            try
            {
                Directory.Delete(path, recursive: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot cleanup directory");
            }
        }
    }
}
