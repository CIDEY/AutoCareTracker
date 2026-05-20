using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AutoCareTracker.Models;
using AutoCareTracker.Services;
using System.Collections.ObjectModel;

namespace AutoCareTracker.ViewModels
{
    public partial class GarageViewModel : ObservableObject
    {
        private readonly DatabaseService _dbService;

        [ObservableProperty]
        private ObservableCollection<Vehicle> _vehicles;

        public GarageViewModel(DatabaseService dbService)
        {
            _dbService = dbService;
            Vehicles = new ObservableCollection<Vehicle>(); 
        }

        [RelayCommand]
        public async Task LoadVehicles()
        {
            var list = await _dbService.GetVehiclesAsync();
            Vehicles.Clear();
            foreach (var v in list) Vehicles.Add(v);
        }

        [RelayCommand]
        public async Task AddVehicle()
        {
            // Упрощенный ввод через диалоговые окна (быстро и удобно)
            string brand = await Shell.Current.DisplayPromptAsync("Новое авто", "Введите марку (напр. BMW):");
            if (string.IsNullOrWhiteSpace(brand)) return;

            string model = await Shell.Current.DisplayPromptAsync("Новое авто", "Введите модель (напр. X5):");
            string plate = await Shell.Current.DisplayPromptAsync("Новое авто", "Введите госномер:");

            var newVehicle = new Vehicle { Brand = brand, Model = model, Plate = plate };
            await _dbService.AddVehicleAsync(newVehicle);
            await LoadVehicles();
        }

        [RelayCommand]
        public async Task SelectVehicle(Vehicle vehicle)
        {
            if (vehicle == null) return;

            // Сохраняем выбор в глобальное состояние
            AppState.SelectedVehicle = vehicle;

            // Переходим на главную страницу с записями ТО
            await Shell.Current.GoToAsync("//MainPage");
        }

        [RelayCommand]
        public async Task DeleteVehicle(Vehicle vehicle)
        {
            bool confirm = await Shell.Current.DisplayAlert("Удаление", $"Удалить {vehicle.FullName} и всю историю ТО?", "Да", "Нет");
            if (confirm)
            {
                await _dbService.DeleteVehicleAsync(vehicle);
                await LoadVehicles();
            }
        }
    }
}