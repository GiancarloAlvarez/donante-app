namespace Donant_app.Models;

public static class DonanteValidator
{
    private static readonly HashSet<string> BloodTypes =
    [
        "O+", "O-", "A+", "A-", "B+", "B-", "AB+", "AB-"
    ];

    public static string? Validate(Donante donante)
    {
        if (string.IsNullOrWhiteSpace(donante.NombreDonante) || donante.NombreDonante.Trim().Length < 3)
        {
            return "Escribe el nombre completo del donante.";
        }

        if (!BloodTypes.Contains(NormalizeBloodType(donante.TipoSangre)))
        {
            return "Selecciona un tipo de sangre válido.";
        }

        if (donante.Edad is < 18 or > 100)
        {
            return "La edad debe estar entre 18 y 100 años.";
        }

        int phoneDigits = donante.Telefono.Count(char.IsDigit);
        if (phoneDigits is < 7 or > 15)
        {
            return "Escribe un teléfono válido de 7 a 15 dígitos.";
        }

        if (string.IsNullOrWhiteSpace(donante.Direccion))
        {
            return "La dirección es obligatoria.";
        }

        if (donante.Peso is < 50 or > 300)
        {
            return "El peso debe estar entre 50 y 300 kg.";
        }

        if (donante.FechaNacimiento.Date > DateTime.Today.AddYears(-18))
        {
            return "El donante debe tener al menos 18 años.";
        }

        if (donante.UltimaDonacion.HasValue && donante.UltimaDonacion.Value.Date > DateTime.Today)
        {
            return "La última donación no puede estar en el futuro.";
        }

        return null;
    }

    public static string NormalizeBloodType(string value) =>
        value.Trim().Replace('−', '-').ToUpperInvariant();
}
