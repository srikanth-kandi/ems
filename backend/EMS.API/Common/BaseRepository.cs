using Microsoft.EntityFrameworkCore;
using EMS.API.Data;
using EMS.API.Interfaces;
using System.Linq.Expressions;

namespace EMS.API.Common;

public abstract class BaseRepository<TEntity, TDto> where TEntity : class
{
    protected readonly EMSDbContext _context;
    protected readonly ILogger _logger;

    protected BaseRepository(EMSDbContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    protected abstract IQueryable<TEntity> GetBaseQuery();
    protected abstract TDto MapToDto(TEntity entity);
    protected abstract TEntity MapToEntity(TDto dto);
    protected abstract void UpdateEntity(TEntity existing, TDto dto);
    protected abstract Expression<Func<TEntity, TDto>> GetMapToDtoExpression();

    public virtual async Task<IEnumerable<TDto>> GetAllAsync()
    {
        return await GetBaseQuery()
            .Select(GetMapToDtoExpression())
            .ToListAsync();
    }

    public virtual async Task<TDto?> GetByIdAsync(int id)
    {
        var entity = await GetBaseQuery()
            .FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        
        return entity != null ? MapToDto(entity) : default(TDto);
    }

    public virtual async Task<bool> ExistsAsync(int id)
    {
        return await GetBaseQuery()
            .AnyAsync(e => EF.Property<int>(e, "Id") == id);
    }

    protected virtual async Task<int> GetTotalCountAsync()
    {
        return await GetBaseQuery().CountAsync();
    }
}
