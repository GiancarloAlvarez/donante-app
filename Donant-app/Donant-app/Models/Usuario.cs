using System;
using System.Collections.Generic;
using System.Text;

namespace Donant_app.Models
{
    internal class Usuario
    {

    
        
            public int Id { get; set; }
            public string Username { get; set; } = string.Empty;

            public string Password { get; set; }
            public string PasswordHash { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Role { get; set; } = "donante";   
            public int? DonorId { get; set; }              
            //public bool RecordarSesion { get; set; } = false;
            //public DateTime LastLogin { get; set; }
        
    }





}
