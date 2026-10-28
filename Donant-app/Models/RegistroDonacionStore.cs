namespace Donant_app.Models;

public static class RegistroDonacionStore
{
    private const string DataFileName = "donaciones.json";
    private static readonly SemaphoreSlim Gate = new(1, 1);
    private static readonly List<RegistroDonacion> Items = [];
    private static bool _initialized;

    public static IReadOnlyList<RegistroDonacion> Registros => Items;

    public static async Task InitializeAsync()
    {
        if (_initialized) return;

        await Gate.WaitAsync();
        try
        {
            if (_initialized) return;

            Items.Clear();
            Items.AddRange(await JsonFileStore.LoadAsync<RegistroDonacion>(GetDataFilePath()));
            _initialized = true;
        }
        finally
        {
            Gate.Release();
        }
    }

    public static async Task AddAsync(RegistroDonacion registro)
    {
        await InitializeAsync();
        await Gate.WaitAsync();
        try
        {
            registro.Id = Items.Count == 0 ? 1 : Items.Max(item => item.Id) + 1;
            Items.Add(registro);
            try
            {
                await JsonFileStore.SaveAsync(GetDataFilePath(), Items);
            }
            catch
            {
                Items.Remove(registro);
                throw;
            }
        }
        finally
        {
            Gate.Release();
        }
    }

    private static string GetDataFilePath() =>
        Path.Combine(FileSystem.AppDataDirectory, DataFileName);
}
