using Donant_app.Models;

namespace Donant_app.Tests;

public class DonorInsightsTests
{
    [Fact]
    public void GetLastDonation_UsesNewestAvailableDate()
    {
        Donante donor = new()
        {
            Id = 7,
            UltimaDonacion = new DateTime(2026, 1, 10)
        };
        RegistroDonacion[] records =
        [
            new() { DonanteId = 7, FechaDonacion = new DateTime(2026, 3, 15) },
            new() { DonanteId = 8, FechaDonacion = new DateTime(2026, 5, 1) }
        ];

        DateTime? result = DonorInsights.GetLastDonation(donor, records);

        Assert.Equal(new DateTime(2026, 3, 15), result);
    }

    [Fact]
    public void IsEligible_RequiresFiftySixDaysOfRest()
    {
        Donante donor = new()
        {
            Id = 4,
            IsAvailable = true,
            UltimaDonacion = new DateTime(2026, 1, 1)
        };

        Assert.False(DonorInsights.IsEligible(donor, [], new DateTime(2026, 2, 25)));
        Assert.True(DonorInsights.IsEligible(donor, [], new DateTime(2026, 2, 26)));
    }

    [Theory]
    [InlineData(0, "Nuevo donante")]
    [InlineData(1, "Donante activo")]
    [InlineData(5, "Donante de plata")]
    [InlineData(10, "Donante de oro")]
    [InlineData(20, "Donante platino")]
    public void GetLevel_UsesDonationMilestones(int donationCount, string expected)
    {
        Assert.Equal(expected, DonorInsights.GetLevel(donationCount));
    }
}
