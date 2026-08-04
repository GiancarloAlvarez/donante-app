using Donant_app.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Donant_app.Services
{
    

    public class DatabaseService
    {
        private SQLiteAsyncConnection? _database;

        // ── Inicialización lazy ──────────────────────────────────────
        private async Task Init()
        {
            if (_database is not null) return;

            _database = new SQLiteAsyncConnection(
                Path.Combine(FileSystem.AppDataDirectory, "DonanT.db3"),
                SQLiteOpenFlags.ReadWrite |
                SQLiteOpenFlags.Create |
                SQLiteOpenFlags.SharedCache);

            await _database.CreateTableAsync<Donante>();
            await _database.CreateTableAsync<RegistroDonacion>();
            await _database.CreateTableAsync<SolicitudSangre>();
        }

        // ── DONANTES ─────────────────────────────────────────────────

        public async Task<List<Donante>> GetDonantesAsync()
        {
            await Init();
            return await _database!.Table<Donante>().ToListAsync();
        }

        public async Task<int> SaveDonanteAsync(Donante donante)
        {
            await Init();
            try
            {
                if (donante.Id == 0)
                    return await _database!.InsertAsync(donante);
                else
                    return await _database!.UpdateAsync(donante);
            }
            catch (SQLiteException ex) when (ex.Message.Contains("UNIQUE"))
            {
                throw new Exception("El número de teléfono ya está registrado.");
            }
        }

        public async Task<int> DeleteDonanteAsync(Donante donante)
        {
            await Init();
            return await _database!.DeleteAsync(donante);
        }

        // ── REGISTROS DE DONACIÓN ────────────────────────────────────

        public async Task<List<RegistroDonacion>> GetRegistrosAsync()
        {
            await Init();
            return await _database!.Table<RegistroDonacion>().ToListAsync();
        }

        public async Task<List<RegistroDonacion>> GetRegistrosPorDonanteAsync(int donanteId)
        {
            await Init();
            return await _database!.Table<RegistroDonacion>()
                                   .Where(r => r.DonanteId == donanteId)
                                   .ToListAsync();
        }

        public async Task<int> SaveRegistroAsync(RegistroDonacion registro)
        {
            await Init();
            if (registro.Id == 0)
                return await _database!.InsertAsync(registro);
            else
                return await _database!.UpdateAsync(registro);
        }

        // ── SOLICITUDES DE SANGRE ────────────────────────────────────

        public async Task<List<SolicitudSangre>> GetSolicitudesAsync()
        {
            await Init();
            return await _database!.Table<SolicitudSangre>().ToListAsync();
        }

        public async Task<int> SaveSolicitudAsync(SolicitudSangre solicitud)
        {
            await Init();
            if (solicitud.Id == 0)
                return await _database!.InsertAsync(solicitud);
            else
                return await _database!.UpdateAsync(solicitud);
        }

        public async Task<int> DeleteSolicitudAsync(SolicitudSangre solicitud)
        {
            await Init();
            return await _database!.DeleteAsync(solicitud);
        }
    }
}
