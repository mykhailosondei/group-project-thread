using ApplicationDAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ApplicationBLL.Extentions;

public static class CustomEntityIncluder
{
    public static IQueryable<Comment> CustomInclude(this IQueryable<Comment> source, int depth)
    {
        if (depth == 0)
        {
            return source.Include(c => c.Post).ThenInclude(p => p.Author).ThenInclude(u => u.Avatar)
                .Include(c => c.Post).ThenInclude(p => p.Images);
        }

        var result = source.Include(c => c.ParentComment);

        for (int i = 0; i < depth - 1; i++)
        {
            result = result.ThenInclude(c => c.ParentComment);
        }

        result = result.ThenInclude(c => c.Post).ThenInclude(p => p.Author.Avatar).Include(c => c.ParentComment);
        
        for (int i = 0; i < depth - 1; i++)
        {
            result = result.ThenInclude(c => c.ParentComment);
        }
        
        return result.ThenInclude(c => c.Post).ThenInclude(p => p.Images);
    }
}