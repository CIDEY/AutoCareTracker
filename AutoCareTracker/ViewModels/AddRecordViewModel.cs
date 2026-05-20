using AutoCareTracker.Models;
using AutoCareTracker.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoCareTracker.ViewModels
{
    [QueryProperty(nameof(ExistingRecord), "Record")]
    public partial class AddRecordViewModel : ObservableObject
    {
        private readonly DatabaseService _dbService;

        [ObservableProperty] private string _mileage;
        [ObservableProperty] private DateTime _date = DateTime.Now;
        [ObservableProperty] private string _cost;
        [ObservableProperty] private string _notes;
        [ObservableProperty] private string _selectedCategory;

        [ObservableProperty] private string _buttonText = "Сохранить";

        [ObservableProperty] private ServiceRecord _existingRecord;

        public AddRecordViewModel(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        [ObservableProperty]
        private List<string> _categories = new()
        {
            "Замена масла", "Фильтры", "Тормоза", "Шиномонтаж", "Подвеска", "Прочее"
        };

        partial void OnExistingRecordChanged(ServiceRecord value)
        {
            if (value != null)
            {
                SelectedCategory = value.WorkType;
                Mileage = value.Mileage.ToString();
                Date = value.Date;
                Cost = value.Cost.ToString();
                Notes = value.Notes;

                ButtonText = "Обновить данные";
            }
        }

        [RelayCommand]
        public async Task SaveRecord()
        {
            if (string.IsNullOrWhiteSpace(SelectedCategory) || string.IsNullOrWhiteSpace(Mileage))
            {
                await Shell.Current.DisplayAlert("Ошибка", "Заполните тип работы и пробег", "OK");
                return;
            }

            if (ExistingRecord == null)
            {
                var newRecord = new ServiceRecord
                {
                    VehicleId = AppState.SelectedVehicle.Id,
                    WorkType = SelectedCategory,
                    Mileage = int.TryParse(Mileage, out var m) ? m : 0,
                    Date = Date,
                    Cost = double.TryParse(Cost, out var c) ? c : 0,
                    Notes = Notes
                };
                await _dbService.AddRecordAsync(newRecord);
            }
            else
            {
                ExistingRecord.WorkType = SelectedCategory;
                ExistingRecord.Mileage = int.TryParse(Mileage, out var m) ? m : 0;
                ExistingRecord.Date = Date;
                ExistingRecord.Cost = double.TryParse(Cost, out var c) ? c : 0;
                ExistingRecord.Notes = Notes;

                await _dbService.UpdateRecordAsync(ExistingRecord);
            }

            await Shell.Current.GoToAsync("..");
        }
    }
}