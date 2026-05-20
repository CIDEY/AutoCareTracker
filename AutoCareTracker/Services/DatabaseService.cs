using AutoCareTracker.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCareTracker.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;

        async Task Init()
        {
            if (_database is not null)
                return;

            // Путь к файлу БД на устройстве
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "AutoCare.db3");

            _database = new SQLiteAsyncConnection(dbPath);

            // Создаем таблицу, если её еще нет
            await _database.CreateTableAsync<ServiceRecord>();
        }

        // CREATE
        public async Task AddRecordAsync(ServiceRecord record)
        {
            await Init();
            await _database.InsertAsync(record);
        }

        // READ
        public async Task<List<ServiceRecord>> GetRecordsAsync()
        {
            await Init();
            return await _database.Table<ServiceRecord>().OrderByDescending(x => x.Date).ToListAsync();
        }

        // UPDATE
        public async Task UpdateRecordAsync(ServiceRecord record)
        {
            await Init();
            await _database.UpdateAsync(record);
        }

        // DELETE
        public async Task DeleteRecordAsync(ServiceRecord record)
        {
            await Init();
            await _database.DeleteAsync(record);
        }
    }
}
