using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SystemTools.SystemToolsShared;

namespace SystemTools.DatabaseToolsShared;

public /*open*/ class DataSeederRepository : IDataSeederRepository
{
    private readonly DbContext _context;
    private readonly ILogger<DataSeederRepository> _logger;

    // ReSharper disable once ConvertToPrimaryConstructor
    public DataSeederRepository(DbContext ctx, ILogger<DataSeederRepository> logger)
    {
        _context = ctx;
        _logger = logger;
    }

    public List<T> GetAll<T>() where T : class
    {
        return [.. _context.Set<T>()];
    }

    public bool HaveAnyRecord<T>() where T : class
    {
        return _context.Set<T>().Any();
    }

    public bool CreateEntities<T>(List<T> entities) where T : class
    {
        if (entities.Count == 0)
        {
            return true;
        }

        try
        {
            _context.AddRange(entities);
            return SaveChanges();
        }
        catch (Exception e)
        {
            StShared.WriteException(e, $"Error when creating CreateEntities type: {typeof(T)}", true, _logger, false);
            return false;
        }
    }

    public bool DeleteEntities<T>(List<T> entities) where T : class
    {
        if (entities.Count == 0)
        {
            return true;
        }

        try
        {
            foreach (T entity in entities)
            {
                _context.Remove(entity);
            }

            return SaveChanges();
        }
        catch (Exception e)
        {
            StShared.WriteException(e, $"Error when creating CreateEntities type: {typeof(T)}", true, _logger, false);
            return false;
        }
    }

    public bool SaveChanges()
    {
        try
        {
            _context.SaveChanges();
            return true;
        }
        catch (Exception e)
        {
            StShared.WriteException(e, "Error when saving changes", true, _logger, false);
            return false;
        }
    }

    public bool SetUpdates<T>(List<T> forUpdate) where T : class
    {
        if (forUpdate.Count == 0)
        {
            return true;
        }

        try
        {
            foreach (T rec in forUpdate)
            {
                _context.Update(rec);
            }

            return SaveChanges();
        }
        catch (Exception e)
        {
            StShared.WriteException(e, $"Error when SetUpdates type: {typeof(T)}", true, _logger, false);
            return false;
        }
    }

    public bool RemoveNeedlessRecords<T>(List<T> needLessList) where T : class
    {
        if (needLessList.Count == 0)
        {
            return true;
        }

        try
        {
            _context.RemoveRange(needLessList);
            return SaveChanges();
        }
        catch (Exception e)
        {
            StShared.WriteException(e, $"Error when RemoveNeedlessRecords type: {typeof(T)}", true, _logger, false);
            return false;
        }
    }
}
