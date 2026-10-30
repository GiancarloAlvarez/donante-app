namespace Donant_app.Models;

public static class DemoDataSeeder
{
    public static async Task SeedIfApplicableAsync()
    {
        Donante? maria = DonanteStore.Donantes.FirstOrDefault(donor =>
            donor.Id == 1 &&
            donor.NombreDonante == "María López" &&
            donor.Telefono == "809-555-0101");
        Donante? carlos = DonanteStore.Donantes.FirstOrDefault(donor =>
            donor.Id == 2 &&
            donor.NombreDonante == "Carlos Pérez" &&
            donor.Telefono == "809-555-0202");

        if (maria is null || carlos is null || DonanteStore.Donantes.Count != 2)
        {
            return;
        }

        if (RegistroDonacionStore.Registros.Count == 0)
        {
            await RegistroDonacionStore.AddAsync(new RegistroDonacion
            {
                DonanteId = maria.Id,
                NombreDonante = maria.NombreDonante,
                FechaDonacion = maria.UltimaDonacion ?? DateTime.Today.AddMonths(-4),
                CentroDonacion = "Hospital General Plaza de la Salud",
                Ciudad = "Santo Domingo",
                VolumenMl = 450
            });

            await RegistroDonacionStore.AddAsync(new RegistroDonacion
            {
                DonanteId = carlos.Id,
                NombreDonante = carlos.NombreDonante,
                FechaDonacion = carlos.UltimaDonacion ?? DateTime.Today.AddMonths(-1),
                CentroDonacion = "Banco de Sangre Regional",
                Ciudad = "Santiago",
                VolumenMl = 450
            });
        }

        if (SolicitudSangreStore.Solicitudes.Count == 0)
        {
            await SolicitudSangreStore.AddAsync(new SolicitudSangre
            {
                NombrePaciente = "Lucía Martínez",
                TipoSangreNecesario = "O-",
                Hospital = "Hospital General Plaza de la Salud",
                EsUrgente = true,
                NumeroContacto = "809-555-0303",
                FechaSolicitud = DateTime.Today,
                Ciudad = "Santo Domingo"
            });
        }
    }
}
