using Donant_app.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Donant_app.Services
{
    internal class InMemoryDataService
    {
        

        
            // ── Singleton ────────────────────────────────────────────────────
            private static InMemoryDataService? _instance;
        public static InMemoryDataService Instance =>
        _instance ??= new InMemoryDataService();

        // ── Sesión activa ────────────────────────────────────────────────
        public Usuario? CurrentUser { get; private set; }
            public bool IsLoggedIn => CurrentUser != null;

            // ── Credenciales fijas ───────────────────────────────────────────
            private readonly List<Usuario> _users = new()
        {
            new Usuario { Id = 1, Username = "admin",   Password = "admin123",  Role = "admin" },
            new Usuario { Id = 2, Username = "donante",  Password = "dona123",   Role = "donor" }
        };

            // ── Colecciones en memoria ───────────────────────────────────────
            private readonly List<Donante> _donante = new();
            private readonly List<SolicitudSangre> _requests = new();
            private readonly List<RegistroDonacion> _records = new();

            private int _donorIdSeq = 1;
            private int _requestIdSeq = 1;
            private int _recordIdSeq = 1;

            private InMemoryDataService()
            {
                SeedData();
            }

            // ── Seed: datos de ejemplo ───────────────────────────────────────
            private void SeedData()
            {
                _donante.AddRange(new[]
                {
                new Donante { Id = _donorIdSeq++, NombreDonante = "María López",
                    TipoSangre = "O+", Edad = 28, Telefono = "809-555-0101",
                    Direccion = "Santo Domingo", Peso = 62, IsAvailable = true,
                    UltimaDonacion = DateTime.Now.AddMonths(-4) },
                new Donante { Id = _donorIdSeq++, NombreDonante = "Carlos Pérez",
                    TipoSangre = "A-", Edad = 35, Telefono = "809-555-0202",
                    Direccion = "Santiago", Peso = 78, IsAvailable = false,
                    UltimaDonacion = DateTime.Now.AddMonths(-1) }
            });

                _requests.AddRange(new[]
                {
                new SolicitudSangre { Id = _requestIdSeq++, NombrePaciente = "Juan Díaz",
                    TipoSangreNecesario = "O+", Hospital = "Hospital Darío Contreras",
                    Ciudad = "Santo Domingo", EsUrgente = true,
                    NumeroContacto = "809-555-0303" },
                new SolicitudSangre { Id = _requestIdSeq++, NombrePaciente = "Ana Martínez",
                    TipoSangreNecesario = "B+", Hospital = "HOMS",
                    Ciudad = "Santiago", EsUrgente = false,
                    NumeroContacto = "809-555-0404" }
            });

                _records.AddRange(new[]
                {
                new RegistroDonacion { Id = _recordIdSeq++, DonanteId = 1,
                    NombreDonante = "María López", CentroDonacion = "Banco de Sangre Nacional",
                    Ciudad = "Santo Domingo", VolumenMl = 450,
                    FechaEnQueDonanteDonó = DateTime.Now.AddMonths(-4) },
                new RegistroDonacion { Id = _recordIdSeq++, DonanteId = 2,
                    NombreDonante = "Carlos Pérez", CentroDonacion = "Cruz Roja Santiago",
                    Ciudad = "Santiago", VolumenMl = 450,
                    FechaEnQueDonanteDonó = DateTime.Now.AddMonths(-1) }
            });
            }

            // ── Auth ─────────────────────────────────────────────────────────
            public bool Login(string username, string password)
            {
                var user = _users.FirstOrDefault(u =>
                    u.Username == username && u.Password == password);
                CurrentUser = user;
                return user != null;
            }

            public void Logout() => CurrentUser = null;

            // ── Donors CRUD ──────────────────────────────────────────────────
            public List<Donante> GetDonors() => _donante.ToList();

            public void AddDonor(Donante donante)
            {
                donante.Id = _donorIdSeq++;
                donante.CreatedAt = DateTime.Now;
                _donante.Add(donante);
            }

            public void UpdateDonor(Donante donante)
            {
                var idx = _donante.FindIndex(d => d.Id == donante.Id);
                if (idx >= 0) _donante[idx] = donante;
            }

            public void DeleteDonor(int id) =>
                _donante.RemoveAll(d => d.Id == id);

            // ── BloodRequests CRUD ───────────────────────────────────────────
            public List<SolicitudSangre> GetRequests() => _requests.ToList();

            public void AddRequest(SolicitudSangre req)
            {
                req.Id = _requestIdSeq++;
                req.FechaSolicitud = DateTime.Now;
                _requests.Add(req);
            }

            // ── DonationRecords CRUD ─────────────────────────────────────────
            public List<RegistroDonacion> GetRecords() => _records.ToList();

            public void AddRecord(RegistroDonacion record)
            {
                record.Id = _recordIdSeq++;
                _records.Add(record);
            }
        }
    }

