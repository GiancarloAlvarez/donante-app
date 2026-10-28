namespace Donant_app.Models;

public sealed class CentroDonacionItem
{
    public string Nombre { get; init; } = string.Empty;
    public string Ciudad { get; init; } = string.Empty;
    public int Donaciones { get; init; }
    public DateTime UltimaDonacion { get; init; }

    public string Resumen => string.IsNullOrWhiteSpace(Ciudad)
        ? $"{Donaciones} donación(es) registrada(s)"
        : $"{Ciudad} · {Donaciones} donación(es)";

    public string UltimaVisita => $"Última visita: {UltimaDonacion:dd MMM yyyy}";
}
