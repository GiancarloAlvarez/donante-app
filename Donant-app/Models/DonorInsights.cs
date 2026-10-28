namespace Donant_app.Models;

public static class DonorInsights
{
    public const int RequiredRestDays = 56;

    public static DateTime? GetLastDonation(
        Donante donor,
        IEnumerable<RegistroDonacion> records)
    {
        DateTime? lastRecordedDonation = records
            .Where(record => record.DonanteId == donor.Id)
            .Select(record => (DateTime?)record.FechaDonacion.Date)
            .Max();

        if (lastRecordedDonation is null)
        {
            return donor.UltimaDonacion?.Date;
        }

        if (donor.UltimaDonacion is null)
        {
            return lastRecordedDonation;
        }

        return lastRecordedDonation > donor.UltimaDonacion.Value.Date
            ? lastRecordedDonation
            : donor.UltimaDonacion.Value.Date;
    }

    public static DateTime? GetNextEligibleDate(
        Donante donor,
        IEnumerable<RegistroDonacion> records)
    {
        return GetLastDonation(donor, records)?.AddDays(RequiredRestDays);
    }

    public static bool IsEligible(
        Donante donor,
        IEnumerable<RegistroDonacion> records,
        DateTime today)
    {
        if (!donor.IsAvailable)
        {
            return false;
        }

        DateTime? nextEligibleDate = GetNextEligibleDate(donor, records);
        return nextEligibleDate is null || nextEligibleDate.Value.Date <= today.Date;
    }

    public static string GetLevel(int donationCount) => donationCount switch
    {
        >= 20 => "Donante platino",
        >= 10 => "Donante de oro",
        >= 5 => "Donante de plata",
        >= 1 => "Donante activo",
        _ => "Nuevo donante"
    };
}
