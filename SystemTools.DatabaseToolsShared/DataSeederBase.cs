namespace SystemTools.DatabaseToolsShared;

public abstract class DataSeederBase
{
    private readonly bool _checkOnly;

    protected DataSeederBase(bool checkOnly)
    {
        _checkOnly = checkOnly;
    }

    protected bool Use(ITableDataSeeder dataSeeder)
    {
        return dataSeeder.Create(_checkOnly);
    }

    public abstract bool SeedData();
}
