using AutoCareTracker.ViewModels;

namespace AutoCareTracker
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var vm = BindingContext as ViewModels.MainViewModel;
            if (vm != null)
            {
                await vm.LoadRecordsCommand.ExecuteAsync(null);
            }
        }
    }

}
