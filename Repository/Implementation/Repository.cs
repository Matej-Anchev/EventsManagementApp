using System.Linq.Expressions;
using System.Net.Mime;
using Domain.Common;
using Domain.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Repository.Interface;

namespace Repository.Implementation;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _context;
    private readonly DbSet<T> entities;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        entities = _context.Set<T>();
    }

    public async Task<T> InsertAsync(T entity)
    {
        _context.AddRange(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<ICollection<T>> InsertManyAsync(ICollection<T> entity)
    {
        _context.AddRange(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<T> UpdateAsync(T entity)
    {
        _context.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<T> DeleteAsync(T entity)
    {
        _context.Remove(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<TE?> Get<TE>(Expression<Func<T, TE>> selector, Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
    {
        IQueryable<T> query = entities;
        if (include != null)
        {
            query = include(query);
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        return await query.Select(selector).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<TE>> GetAllAsync<TE>(Expression<Func<T, TE>> selector,
        Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        int? take = null)
    {
        IQueryable<T> query = entities;

        if (include != null)
        {
            query = include(query);
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (orderBy != null)
        {
            orderBy(query);
        }

        if (take.HasValue)
        {
            query = query.Take(take.Value);
        }


        return await query.Select(selector).ToListAsync();
    }

    public async Task<PaginatedResult<TE>> GetAllPagedAsync<TE>(Expression<Func<T, TE>> selector, int pageNumber,
        int pageSize, Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, bool asNoTracking = false)
    {
        IQueryable<T> query = entities;

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        if (include != null)
        {
            query = include(query);
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        IQueryable<TE> projectedQuery = query.Select(selector);

        var totalCount = await projectedQuery.CountAsync();


        var items = await projectedQuery
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PaginatedResult<TE>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = totalPages
        };
    }
}