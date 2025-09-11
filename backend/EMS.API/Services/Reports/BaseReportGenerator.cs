using Microsoft.EntityFrameworkCore;
using EMS.API.Data;
using EMS.API.Interfaces;

namespace EMS.API.Services.Reports;

public abstract class BaseReportGenerator : IReportGenerator
{
    protected readonly EMSDbContext _context;
    protected readonly IMemoryCache _cache;
    protected readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(30);

    protected BaseReportGenerator(EMSDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public abstract Task<byte[]> GenerateAsync();
    public abstract string GetContentType();
    public abstract string GetFileExtension();

    protected async Task<byte[]> GetCachedOrGenerateAsync(string cacheKey, Func<Task<byte[]>> generateFunc)
    {
        if (_cache.TryGetValue(cacheKey, out byte[]? cachedData))
        {
            return cachedData!;
        }

        var result = await generateFunc();
        _cache.Set(cacheKey, result, _cacheExpiration);
        return result;
    }
}
