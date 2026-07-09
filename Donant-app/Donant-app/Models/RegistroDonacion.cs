using System;
using System.Collections.Generic;
using System.Text;

namespace Donant_app.Models
{
    public class RegistroDonacion
    {
        public int Id { get; set; }
        public int DonanteId { get; set; }
        public string NombreDonante { get; set; } = string.Empty;
        public DateTime FechaDonacion { get; set; } = DateTime.Now;
        public string CentroDonacion { get; set; } = string.Empty;
        public double VolumenMl { get; set; } = 450;

        public string Ciudad { get; set; } = string.Empty;
    }
}
