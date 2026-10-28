using System;
using System.Collections.Generic;
using System.Text;

namespace Donant_app.Models
{
   
        public class SolicitudSangre
        {
            public int Id { get; set; }
            public string NombrePaciente { get; set; } = string.Empty;
            public string TipoSangreNecesario { get; set; } = string.Empty;
            public string Hospital { get; set; } = string.Empty;
            public bool EsUrgente { get; set; }
            public string NumeroContacto { get; set; } = string.Empty;
            public DateTime FechaSolicitud { get; set; } = DateTime.Now;

            public string Ciudad { get; set; } = string.Empty;

            public string FechaMes => FechaSolicitud.ToString("MMM").ToUpperInvariant();
            public string FechaDia => FechaSolicitud.ToString("dd");
            public string Resumen => $"{Hospital} · Tipo {TipoSangreNecesario}";
            public string Estado => EsUrgente ? "Urgente" : "Pendiente";
    }
    }
