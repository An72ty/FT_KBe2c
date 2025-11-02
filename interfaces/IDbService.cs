using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;

namespace ft_kbe2c;

public interface IDbService
{
    public Task<T?> GetOneByProperties<T>(params KeyValuePair<string, object>[] properties) where T : class, IEntity;
    // public Task<List<T>?> GetManyByProperties<T>(params KeyValuePair<string, object>[] properties) where T : class, IEntity;
    public Task<List<T>?> GetByPage<T>(int page, int page_size) where T : class, IEntity;
    public Task Update<T>(Guid id, T entity) where T : class, IEntity;
    public Task Add(IEntity model);
    public Task Delete<T>(Guid id) where T : class, IEntity;
}