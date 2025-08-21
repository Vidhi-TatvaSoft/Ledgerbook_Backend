using System.Linq.Expressions;
using BusinessAcessLayer.Interface;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BusinessAcessLayer.Services;

public class GenericRepo : IGenericRepo
{
    private LedgerBookDbContext _context;
    private readonly IConfiguration _configuration;

    public GenericRepo(LedgerBookDbContext _context, IConfiguration configuration)
    {
        this._context = _context;
        _configuration = configuration;
    }

    public string GetLoginLink()
    {
        return _configuration["FrontendSettings:FrontEndBaseURL"];
    }

    public IEnumerable<T> GetAll<T>(Expression<Func<T, bool>>? predicate,
    List<Expression<Func<T, object>>>? includes = null, List<Func<IQueryable<T>,
    IQueryable<T>>>? thenIncludes = null) where T : class
    {
        IQueryable<T> query = _context.Set<T>();
        if (predicate != null)
        {
            query = query.Where(predicate);
        }
        if (includes != null)
        {
            foreach (Expression<Func<T, object>>? include in includes)
            {
                query = query.Include(include);
            }
        }

        if (thenIncludes != null)
        {
            foreach (var thenInclude in thenIncludes)
            {
                query = thenInclude(query);
            }
        }
        return query.ToList() ?? Enumerable.Empty<T>();
    }

    public T Get<T>(Expression<Func<T, bool>>? predicate, List<Expression<Func<T, object>>>? includes = null, List<Func<IQueryable<T>, IQueryable<T>>>? thenIncludes = null) where T : class
    {
        IQueryable<T> query = _context.Set<T>();
        if (predicate != null)
        {
            query = query.Where(predicate);
        }
        if (includes != null)
        {
            foreach (Expression<Func<T, object>>? include in includes)
            {
                query = query.Include(include);
            }
        }

        if (thenIncludes != null)
        {
            foreach (var thenInclude in thenIncludes)
            {
                query = thenInclude(query);
            }
        }
        return query.FirstOrDefault(predicate);
    }

    public bool IsPresentAny<T>(Expression<Func<T, bool>> predicate) where T : class
    {
        return _context.Set<T>().Any(predicate);
    }

    public async Task AddAsync<T>(T entity) where T : class
    {
        await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync<T>(T entity) where T : class
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
    }

    public void Update<T>(T entity) where T : class
    {
        _context.Set<T>().Update(entity);
        _context.SaveChanges();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
