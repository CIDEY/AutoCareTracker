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

        // Свойство для заголовка (название авто)
        [ObservableProperty]
        private string _currentVehicleHeader;

        // Свойство для подзаголовка (госномер)
        [ObservableProperty]
        private string _currentVehiclePlate;

        [RelayCommand]
        public async Task GoToGarage()
        {
            await Shell.Current.GoToAsync("//GaragePage"); // Возвращаемся в корень на страницу гаража
        }

        // Метод для подсчета
        private void CalculateTotalCost()
        {
            TotalCost = Records.Sum(x => x.Cost);
        }

        [RelayCommand]
        public async Task ExportToCsv()
        {
            try
            {
                // 1. Проверяем, выбрана ли машина (на всякий случай)
                if (AppState.SelectedVehicle == null)
                {
                    await Shell.Current.DisplayAlert("Ошибка", "Сначала выберите автомобиль", "OK");
                    return;
                }

                // 2. ПЕРЕДАЕМ ID выбранной машины в метод (исправление ошибки)
                var items = await _dbService.GetRecordsAsync(AppState.SelectedVehicle.Id);

                if (items == null || items.Count == 0)
                {
                    await Shell.Current.DisplayAlert("Инфо", "Нет данных для экспорта", "OK");
                    return;
                }

                // 3. Формируем текст (добавим название машины в заголовок для крутости)
                var csvContent = new System.Text.StringBuilder();
                csvContent.AppendLine($"Отчет для автомобиля: {AppState.SelectedVehicle.FullName} ({AppState.SelectedVehicle.Plate})");
                csvContent.AppendLine("Дата;Тип работы;Пробег (км);Стоимость (руб);Заметки");

                foreach (var item in items)
                {
                    csvContent.AppendLine($"{item.Date:dd.MM.yyyy};{item.WorkType};{item.Mileage};{item.Cost};{item.Notes}");
                }

                // Далее старый код без изменений...
                string fileName = $"Report_{AppState.SelectedVehicle.Brand}.csv";
                string targetFile = Path.Combine(FileSystem.CacheDirectory, fileName);
                var encoding = new System.Text.UTF8Encoding(true);
                await File.WriteAllTextAsync(targetFile, csvContent.ToString(), encoding);

                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = $"Экспорт ТО: {AppState.SelectedVehicle.FullName}",
                    File = new ShareFile(targetFile)
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Ошибка", $"Не удалось экспортировать: {ex.Message}", "OK");
            }
        }

        // Метод загрузки данных
        [RelayCommand]
        public async Task LoadRecords()
        {
            if (AppState.SelectedVehicle == null) return;

            // Обновляем заголовок данными из AppState
            CurrentVehicleHeader = AppState.SelectedVehicle.FullName;
            CurrentVehiclePlate = AppState.SelectedVehicle.Plate;

            // Загружаем записи только для выбранного авто
            var items = await _dbService.GetRecordsAsync(AppState.SelectedVehicle.Id);

            // Если поиск не пустой — фильтруем список
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                items = items.Where(x => x.WorkType.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            Records.Clear();
            foreach (var item in items) Records.Add(item);

            TotalCost = Records.Sum(x => x.Cost);
            CalculateOilStatus();
        }

        [ObservableProperty]
        private string _searchText;

        // Вызывай метод загрузки при каждом изменении текста поиска
        partial void OnSearchTextChanged(string value)
        {
            LoadRecordsCommand.Execute(null);
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
