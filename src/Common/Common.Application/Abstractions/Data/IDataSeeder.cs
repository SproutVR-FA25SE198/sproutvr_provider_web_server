using Common.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Common.Application.Abstractions.Data;

public interface IDataSeeder
{
    void AddRelativePath<T>(string relativefilePath) where T : BaseEntity;
    Task SeedAllTablesAsync();
}
