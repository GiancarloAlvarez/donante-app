using System;
using System.Collections.Generic;
using System.Text;

namespace Donant_app.Models
{
    public class Donante
    {
        
            public int Id { get; set; }
            public string NombreDonante { get; set; } = string.Empty;
            public string TipoSangre { get; set; } = string.Empty;  
            public int Edad { get; set; }

            public DateTime FechaNacimiento { get; set; }
            public string Telefono { get; set; } = string.Empty;
            public string Direccion { get; set; } = string.Empty;
            public DateTime? UltimaDonacion { get; set; }
            public double Peso { get; set; }
            public bool IsAvailable { get; set; } = true;
            public string? PhotoUrl { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.Now;
        
    }





}

