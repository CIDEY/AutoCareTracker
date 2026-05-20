using AutoCareTracker.ViewModels;

namespace AutoCareTracker
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm; // Без этой строки привязки (Binding) работать не будут
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // Вызываем команду загрузки данных из ViewModel
            var vm = BindingContext as ViewModels.MainViewModel;
            if (vm != null)
            {
                await vm.LoadRecordsCommand.ExecuteAsync(null);
            }
        }
    }

}
