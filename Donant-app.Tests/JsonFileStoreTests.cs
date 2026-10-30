using Donant_app.Models;

namespace Donant_app.Tests;

public class JsonFileStoreTests : IDisposable
{
    private readonly string _testDirectory = Path.Combine(
        Path.GetTempPath(),
        $"donant-app-tests-{Guid.NewGuid():N}");

    [Fact]
    public async Task SaveAndLoadAsync_PreservesDonorData()
    {
        string path = Path.Combine(_testDirectory, "donantes.json");
        var donors = new List<Donante>
        {
            new()
            {
                Id = 4,
                NombreDonante = "Ana Martínez",
                TipoSangre = "AB-",
                Edad = 31,
                FechaNacimiento = DateTime.Today.AddYears(-31),
                Telefono = "809-555-0199",
                Direccion = "Santiago",
                Peso = 67,
                IsAvailable = false
            }
        };

        await JsonFileStore.SaveAsync(path, donors);
        IReadOnlyList<Donante> loaded = await JsonFileStore.LoadAsync<Donante>(path);

        Donante donor = Assert.Single(loaded);
        Assert.Equal(4, donor.Id);
        Assert.Equal("Ana Martínez", donor.NombreDonante);
        Assert.Equal("AB-", donor.TipoSangre);
        Assert.False(donor.IsAvailable);
    }

    [Fact]
    public async Task LoadAsync_MovesCorruptDataAndReturnsEmptyList()
    {
        Directory.CreateDirectory(_testDirectory);
        string path = Path.Combine(_testDirectory, "donantes.json");
        await File.WriteAllTextAsync(path, "{not-valid-json");

        IReadOnlyList<Donante> loaded = await JsonFileStore.LoadAsync<Donante>(path);

        Assert.Empty(loaded);
        Assert.False(File.Exists(path));
        Assert.Single(Directory.GetFiles(_testDirectory, "donantes.json.corrupt-*"));
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, true);
        }
    }
}
