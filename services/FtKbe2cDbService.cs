using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace ft_kbe2c;

public class FtKbe2cDbService(FtKbe2cDbContext dbContext) : IDbService
{
    private readonly FtKbe2cDbContext _dbContext = dbContext;

    public async Task Add(IEntity model)
    {
        await _dbContext.AddAsync(model);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete<T>(Guid id) where T : class, IEntity
    {
        await _dbContext.Set<T>().Where(x => x.Id == id).ExecuteDeleteAsync();
        await _dbContext.SaveChangesAsync();
    }

    public async Task<T?> GetOneByProperties<T>(params KeyValuePair<string, object>[] properties) where T : class, IEntity
    {
        // return await _dbContext.Set<T>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        Dictionary<string, object> propertiesDict = properties.ToDictionary(k => k.Key, v => v.Value);
        var entities = await _dbContext.Set<T>().AsNoTracking().ToListAsync();

        foreach (var entity in entities)
        {
            int i = 0;
            bool[] checks = new bool[propertiesDict.Count];
            foreach (KeyValuePair<string, object> property in propertiesDict)
            {
                if (entity.GetType().GetProperty(property.Key).GetValue(entity).ToString() == property.Value.ToString())
                {
                    checks[i] = true;
                }
                else
                {
                    checks[i] = false;
                }
                i++;
                if (checks.All(x => x == true))
                {
                    return entity;
                }
            }
        }
        return null;
    }
    
    

    public async Task<List<T>?> GetByPage<T>(int page, int page_size) where T : class, IEntity
    {
        return await _dbContext.Set<T>().AsNoTracking().Skip((page - 1) * page_size).Take(page_size).ToListAsync<T>();
    }

    public async Task Update<T>(Guid id, T entity) where T : class, IEntity
    {
        // await _dbContext.Set<T>().Where(x => x.Id == id).ExecuteUpdateAsync(x => x.SetProperty(c => c.GetType().GetProperty(propertyName).SetValue(c, value)));
        
        IEntity _entity = await _dbContext.Set<T>().FirstOrDefaultAsync(x => x.Id == id);
        // entity.GetType().GetProperty(propertyName).SetValue(entity, value);
        _entity = entity;
        await _dbContext.SaveChangesAsync();
    }
}