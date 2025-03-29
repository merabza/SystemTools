using System.Collections.Generic;

namespace DatabaseToolsShared;

public interface IDataSeederRepository
{
    bool HaveAnyRecord<T>() where T : class;
    bool CreateEntities<T>(List<T> entities) where T : class;
    bool DeleteEntities<T>(List<T> entities) where T : class;
    List<T> GetAll<T>() where T : class;
    string GetTableName<T>() where T : class;
    bool SetUpdates<T>(List<T> forUpdate) where T : class;
    bool RemoveNeedlessRecords<T>(List<T> needLessList) where T : class;
    bool SaveChanges();
}