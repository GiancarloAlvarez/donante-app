namespace Donant_app.Models;

public static class DonanteStore
{
    private const string DataFileName = "donantes.json";
    private static readonly SemaphoreSlim Gate = new(1, 1);
    private static readonly List<Donante> Items = [];
    private static bool _initialized;

    public static IReadOnlyList<Donante> Donantes => Items;

    public static async Task InitializeAsync()
    {
        if (_initialized)
        {
            return;
        }

        await Gate.WaitAsync();
        try
        {
            if (_initialized)
            {
                return;
            }

            string path = GetDataFilePath();
            bool isFirstRun = !File.Exists(path);
            Items.Clear();
            Items.AddRange(await JsonFileStore.LoadAsync<Donante>(path));

            if (isFirstRun)
            {
                Items.AddRange(CreateDemoData());
                await SaveUnsafeAsync();
            }

            _initialized = true;
        }
        finally
        {
            Gate.Release();
        }
    }

    public static async Task AddAsync(Donante donante)
    {
        await InitializeAsync();
        await Gate.WaitAsync();
        try
        {
            if (Items.Any(existing =>
                string.Equals(existing.NombreDonante, donante.NombreDonante, StringComparison.CurrentCultureIgnoreCase) &&
                NormalizePhone(existing.Telefono) == NormalizePhone(donante.Telefono)))
            {
                throw new InvalidOperationException("Ya existe un donante con el mismo nombre y teléfono.");
            }

            donante.Id = NextId();
            donante.CreatedAt = DateTime.Now;
            Items.Add(donante);
            try
            {
                await SaveUnsafeAsync();
            }
            catch
            {
                Items.Remove(donante);
                throw;
            }
        }
        finally
        {
            Gate.Release();
        }
    }

    public static async Task UpdateAsync(Donante donante)
    {
        await InitializeAsync();
        await Gate.WaitAsync();
        try
        {
            int index = Items.FindIndex(item => item.Id == donante.Id);
            if (index < 0)
            {
                throw new InvalidOperationException("El donante ya no existe.");
            }

            if (Items.Any(existing =>
                existing.Id != donante.Id &&
                string.Equals(existing.NombreDonante, donante.NombreDonante, StringComparison.CurrentCultureIgnoreCase) &&
                NormalizePhone(existing.Telefono) == NormalizePhone(donante.Telefono)))
            {
                throw new InvalidOperationException("Ya existe otro donante con el mismo nombre y teléfono.");
            }

            Donante previous = Items[index];
            Items[index] = donante;
            try
            {
                await SaveUnsafeAsync();
            }
            catch
            {
                Items[index] = previous;
                throw;
            }
        }
        finally
        {
            Gate.Release();
        }
    }

    public static async Task DeleteAsync(int id)
    {
        await InitializeAsync();
        await Gate.WaitAsync();
        try
        {
            int index = Items.FindIndex(donante => donante.Id == id);
            if (index < 0)
            {
                return;
            }

            Donante item = Items[index];
            Items.RemoveAt(index);
            try
            {
                await SaveUnsafeAsync();
            }
            catch
            {
                Items.Insert(index, item);
                throw;
            }
        }
        finally
        {
            Gate.Release();
        }
    }

    public static IReadOnlyList<Donante> Search(string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Items.ToList();
        }

        string term = query.Trim();
        return Items.Where(donante =>
                donante.NombreDonante.Contains(term, StringComparison.CurrentCultureIgnoreCase) ||
                donante.TipoSangre.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                donante.Direccion.Contains(term, StringComparison.CurrentCultureIgnoreCase))
            .ToList();
    }

    private static int NextId() => Items.Count == 0 ? 1 : Items.Max(item => item.Id) + 1;

    private static Task SaveUnsafeAsync() =>
        JsonFileStore.SaveAsync(GetDataFilePath(), Items);

    private static string GetDataFilePath() =>
        Path.Combine(FileSystem.AppDataDirectory, DataFileName);

    private static string NormalizePhone(string value) =>
        new(value.Where(char.IsDigit).ToArray());

    private static IEnumerable<Donante> CreateDemoData() =>
    [
        new()
        {
            Id = 1,
            NombreDonante = "María López",
            TipoSangre = "O+",
            Edad = 28,
            FechaNacimiento = DateTime.Today.AddYears(-28),
            Telefono = "809-555-0101",
            Direccion = "Santo Domingo",
            Peso = 62,
            IsAvailable = true,
            UltimaDonacion = DateTime.Today.AddMonths(-4)
        },
        new()
        {
            Id = 2,
            NombreDonante = "Carlos Pérez",
            TipoSangre = "A-",
            Edad = 35,
            FechaNacimiento = DateTime.Today.AddYears(-35),
            Telefono = "809-555-0202",
            Direccion = "Santiago",
            Peso = 78,
            IsAvailable = false,
            UltimaDonacion = DateTime.Today.AddMonths(-1)
        }
    ];
}
