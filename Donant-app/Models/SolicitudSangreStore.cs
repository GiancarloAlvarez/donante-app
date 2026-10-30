namespace Donant_app.Models;

public static class SolicitudSangreStore
{
    private const string DataFileName = "solicitudes.json";
    private static readonly SemaphoreSlim Gate = new(1, 1);
    private static readonly List<SolicitudSangre> Items = [];
    private static bool _initialized;

    public static IReadOnlyList<SolicitudSangre> Solicitudes => Items;

    public static async Task InitializeAsync()
    {
        if (_initialized) return;

        await Gate.WaitAsync();
        try
        {
            if (_initialized) return;

            Items.Clear();
            Items.AddRange(await JsonFileStore.LoadAsync<SolicitudSangre>(GetDataFilePath()));
            _initialized = true;
        }
        finally
        {
            Gate.Release();
        }
    }

    public static async Task AddAsync(SolicitudSangre solicitud)
    {
        await InitializeAsync();
        await Gate.WaitAsync();
        try
        {
            solicitud.Id = Items.Count == 0 ? 1 : Items.Max(item => item.Id) + 1;
            Items.Add(solicitud);
            try
            {
                await JsonFileStore.SaveAsync(GetDataFilePath(), Items);
            }
            catch
            {
                Items.Remove(solicitud);
                throw;
            }
        }
        finally
        {
            Gate.Release();
        }
    }

    public static async Task MarkAsAttendedAsync(int id)
    {
        await InitializeAsync();
        await Gate.WaitAsync();
        try
        {
            SolicitudSangre? solicitud = Items.FirstOrDefault(item => item.Id == id);
            if (solicitud is null)
            {
                throw new InvalidOperationException("La solicitud ya no existe.");
            }

            bool previousAttended = solicitud.EstaAtendida;
            bool previousUrgent = solicitud.EsUrgente;
            solicitud.EstaAtendida = true;
            solicitud.EsUrgente = false;

            try
            {
                await JsonFileStore.SaveAsync(GetDataFilePath(), Items);
            }
            catch
            {
                solicitud.EstaAtendida = previousAttended;
                solicitud.EsUrgente = previousUrgent;
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
