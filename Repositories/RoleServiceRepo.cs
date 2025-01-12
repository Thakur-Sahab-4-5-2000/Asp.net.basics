using Microsoft.EntityFrameworkCore;
using Learning_Backend.Databases;
using Learning_Backend.Models.LearningDatabaseModels;

public class RoleService
{
    private readonly LearningDatabase _context;
    private readonly CacheService _cacheService;

    public RoleService(LearningDatabase context, CacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
    }

    public async Task<List<Roles>> GetRolesAsync()
    {
        string cacheKey = "RolesList";

        var cachedRoles = _cacheService.GetCache<List<Roles>>(cacheKey);

        if (cachedRoles != null)
        {
            return cachedRoles;
        }

        var roles = await _context.Roles.ToListAsync();

        _cacheService.AddOrUpdateCache(cacheKey, roles, TimeSpan.FromMinutes(30));

        return roles;
    }
}