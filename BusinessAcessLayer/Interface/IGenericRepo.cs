using System.Linq.Expressions;
using DataAccessLayer.Constant;
using DataAccessLayer.Models;

namespace BusinessAcessLayer.Interface;

public interface IGenericRepo
{
    string GetLoginLink();
    IEnumerable<T> GetAll<T>(Expression<Func<T, bool>>? predicate, List<Expression<Func<T, object>>>? includes = null, List<Func<IQueryable<T>, IQueryable<T>>>? thenIncludes = null) where T : class;
    T Get<T>(Expression<Func<T, bool>>? predicate, List<Expression<Func<T, object>>>? includes = null, List<Func<IQueryable<T>, IQueryable<T>>>? thenIncludes = null) where T : class;
    bool IsPresentAny<T>(Expression<Func<T, bool>> predicate) where T : class;
    Task AddAsync<T>(T entity) where T : class;
    Task UpdateAsync<T>(T entity) where T : class;
    void Update<T>(T entity) where T : class;
    Task SaveChangesAsync();
}
