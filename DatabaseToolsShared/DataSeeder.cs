namespace DatabaseToolsShared;

public abstract class DataSeeder
{
    private readonly bool _checkOnly;

    // ReSharper disable once ConvertToPrimaryConstructor
    protected DataSeeder(bool checkOnly)
    {
        _checkOnly = checkOnly;
    }

    protected bool Use(ITableDataSeeder dataSeeder)
    {
        return dataSeeder.Create(_checkOnly);
    }

    public abstract bool SeedData();
}