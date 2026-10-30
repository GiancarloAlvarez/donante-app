using Donant_app.Models;

namespace Donant_app.Tests;

public class DonanteValidatorTests
{
    [Fact]
    public void Validate_AcceptsCompleteDonor() =>
        Assert.Null(DonanteValidator.Validate(CreateValidDonor()));

    [Theory]
    [InlineData(17)]
    [InlineData(101)]
    public void Validate_RejectsAgeOutsideRange(int age)
    {
        Donante donor = CreateValidDonor();
        donor.Edad = age;

        Assert.Contains("edad", DonanteValidator.Validate(donor), StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(49)]
    [InlineData(301)]
    public void Validate_RejectsWeightOutsideRange(double weight)
    {
        Donante donor = CreateValidDonor();
        donor.Peso = weight;

        Assert.Contains("peso", DonanteValidator.Validate(donor), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Validate_RejectsFutureDonationDate()
    {
        Donante donor = CreateValidDonor();
        donor.UltimaDonacion = DateTime.Today.AddDays(1);

        Assert.Contains("futuro", DonanteValidator.Validate(donor), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void NormalizeBloodType_ConvertsUnicodeMinus() =>
        Assert.Equal("A-", DonanteValidator.NormalizeBloodType(" a− "));

    private static Donante CreateValidDonor() => new()
    {
        NombreDonante = "María López",
        TipoSangre = "O+",
        Edad = 28,
        FechaNacimiento = DateTime.Today.AddYears(-28),
        Telefono = "809-555-0101",
        Direccion = "Santo Domingo",
        UltimaDonacion = DateTime.Today.AddMonths(-4),
        Peso = 62
    };
}
