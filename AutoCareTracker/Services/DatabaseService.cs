using AutoCareTracker.Models;
using SQLite;

namespace AutoCareTracker.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;

        async Task Init()
        {
            if (_database is not null) return;

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "AutoCareV2.db3");
            _database = new SQLiteAsyncConnection(dbPath);

            await _database.CreateTableAsync<Vehicle>();
            await _database.CreateTableAsync<ServiceRecord>();
        }

        // === РАБОТА С АВТОМОБИЛЯМИ (VEHICLES) ===

        public async Task<List<Vehicle>> GetVehiclesAsync()
        {
            await Init();
            return await _database.Table<Vehicle>().ToListAsync();
        }

        public async Task AddVehicleAsync(Vehicle vehicle)
        {
            await Init();
            await _database.InsertAsync(vehicle);
        }

        public async Task DeleteVehicleAsync(Vehicle vehicle)
        {
            await Init();
            await _database.ExecuteAsync("DELETE FROM ServiceRecord WHERE VehicleId = ?", vehicle.Id);
            await _database.DeleteAsync(vehicle);
        }

        // === РАБОТА С ЗАПИСЯМИ ТО (SERVICE RECORDS) ===

        public async Task<List<ServiceRecord>> GetRecordsAsync(int vehicleId)
        {
            await Init();
            return await _database.Table<ServiceRecord>()
                                   .Where(v => v.VehicleId == vehicleId)
                                   .OrderByDescending(x => x.Date)
                                   .ToListAsync();
        }

        public async Task AddRecordAsync(ServiceRecord record)
        {
            await Init();
            await _database.InsertAsync(record);
        }

        public async Task UpdateRecordAsync(ServiceRecord record)
        {
            await Init();
            await _database.UpdateAsync(record);
        }

        public async Task DeleteRecordAsync(ServiceRecord record)
        {
            await Init();
            await _database.DeleteAsync(record);
        }
    }
}