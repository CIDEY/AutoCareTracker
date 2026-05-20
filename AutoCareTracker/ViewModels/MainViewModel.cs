using AutoCareTracker.Models;
using AutoCareTracker.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace AutoCareTracker.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly DatabaseService _dbService;

        [ObservableProperty]
        private ObservableCollection<ServiceRecord> records;

        public MainViewModel(DatabaseService dbService)
        {
            _dbService = dbService;
            Records = new ObservableCollection<ServiceRecord>();
        }
        [ObservableProperty]
        private double _totalCost;

        [ObservableProperty]
        private string _oilStatus;

        private void CalculateOilStatus()
        {
            // Ищем последнюю запись про масло
            var lastOilChange = Records.FirstOrDefault(r => r.WorkType == "Замена масла");

            if (lastOilChange == null)
            {
                OilStatus = "Данных о замене масла нет";
                return;
            }

            int currentMileage = Records.Max(x => x.Mileage); // Берем самый большой пробег из всех записей
            int milesSinceOil = currentMileage - lastOilChange.Mileage;
            int remaining = 10000 - milesSinceOil;

            if (remaining <= 0)
                OilStatus = "СРОЧНО ЗАМЕНИТЕ МАСЛО!";
            else
                OilStatus = $"Замена масла через: {remaining} км";
        }

        // Метод для подсчета
        private void CalculateTotalCost()
        {
            TotalCost = Records.Sum(x => x.Cost);
        }

        // И не забудь вызвать CalculateTotalCost() в конце метода LoadRecords!

        // Метод загрузки данных
        [RelayCommand]
        public async Task LoadRecords()
        {
            // 1. Получаем данные из базы
            var items = await _dbService.GetRecordsAsync();

            // 2. Чистим старый список
            Records.Clear();

            // 3. Заполняем новый
            foreach (var item in items)
            {
                Records.Add(item);
            }

            // 4. И только ПОСЛЕ этого считаем сумму
            // Важно: пишем TotalCost с большой буквы!
            TotalCost = Records.Sum(x => x.Cost);

            // Если ты добавил статус масла, вызови и его здесь
            CalculateOilStatus();
        }

        [RelayCommand]
        public async Task EditRecord(ServiceRecord record)
        {
            if (record == null) return;

            // Передаем выбранную запись на страницу редактирования
            var navParam = new Dictionary<string, object>
            {
                { "Record", record }
            };

            // В GoToAsync имя страницы должно совпадать с тем, что ты указал в AppShell
            await Shell.Current.GoToAsync("AddRecordPage", navParam);
        }

        // Команда перехода на страницу добавления
        [RelayCommand]
        public async Task GoToAddPage()
        {
            // Имя должно совпадать с тем, что в AppShell
            await Shell.Current.GoToAsync("AddRecordPage");
        }

        [RelayCommand]
        public async Task DeleteRecord(ServiceRecord record)
        {
            bool answer = await Shell.Current.DisplayAlert("Подтверждение", "Удалить эту запись?", "Да", "Нет");
            if (answer)
            {
                await _dbService.DeleteRecordAsync(record);
                Records.Remove(record); // Удаляем из списка на экране
                CalculateTotalCost();   // Пересчитываем сумму
            }
        }
    }
}
