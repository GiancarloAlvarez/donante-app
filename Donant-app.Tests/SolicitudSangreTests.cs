using Donant_app.Models;

namespace Donant_app.Tests;

public class SolicitudSangreTests
{
    [Fact]
    public void Estado_UrgenteMientrasEsteActiva()
    {
        var request = new SolicitudSangre { EsUrgente = true };

        Assert.Equal("Urgente", request.Estado);
    }

    [Fact]
    public void Estado_PendienteCuandoNoEsUrgente()
    {
        var request = new SolicitudSangre { EsUrgente = false };

        Assert.Equal("Pendiente", request.Estado);
    }

    [Fact]
    public void Estado_AtendidaTienePrioridadSobreUrgencia()
    {
        var request = new SolicitudSangre
        {
            EsUrgente = true,
            EstaAtendida = true
        };

        Assert.Equal("Atendida", request.Estado);
    }
}
