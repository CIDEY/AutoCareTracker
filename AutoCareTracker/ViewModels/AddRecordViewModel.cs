using AutoCareTracker.Models;
using AutoCareTracker.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoCareTracker.ViewModels
{
    // Атрибут для получения объекта при переходе (режим редактирования)
    [QueryProperty(nameof(ExistingRecord), "Record")]
    public partial class AddRecordViewModel : ObservableObject
    {
        private readonly DatabaseService _dbService;

        // Поля для формы ввода
        [ObservableProperty] private string _mileage;
        [ObservableProperty] private DateTime _date = DateTime.Now;
        [ObservableProperty] private string _cost;
        [ObservableProperty] private string _notes;
        [ObservableProperty] private string _selectedCategory;

        // Текст кнопки (будет меняться динамически)
        [ObservableProperty] private string _buttonText = "Сохранить";

        // Скрытое свойство для хранения редактируемой записи
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

        // Метод срабатывает автоматически, когда в ViewModel "залетает" запись для редактирования
        partial void OnExistingRecordChanged(ServiceRecord value)
        {
            if (value != null)
            {
                // Заполняем поля данными из пришедшей записи
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
            // Валидация
            if (string.IsNullOrWhiteSpace(SelectedCategory) || string.IsNullOrWhiteSpace(Mileage))
            {
                await Shell.Current.DisplayAlert("Ошибка", "Заполните тип работы и пробег", "OK");
                return;
            }

            if (ExistingRecord == null)
            {
                // ЛОГИКА СОЗДАНИЯ (твой старый код)
                var newRecord = new ServiceRecord
                {
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
                // ЛОГИКА ОБНОВЛЕНИЯ
                ExistingRecord.WorkType = SelectedCategory;
                ExistingRecord.Mileage = int.TryParse(Mileage, out var m) ? m : 0;
                ExistingRecord.Date = Date;
                ExistingRecord.Cost = double.TryParse(Cost, out var c) ? c : 0;
                ExistingRecord.Notes = Notes;

                await _dbService.UpdateRecordAsync(ExistingRecord);
            }

            // Возврат назад
            await Shell.Current.GoToAsync("..");
        }
    }
}