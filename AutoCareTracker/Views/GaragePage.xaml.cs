using AutoCareTracker.ViewModels;

namespace AutoCareTracker.Views;

public partial class GaragePage : ContentPage
{
	public GaragePage(GarageViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var vm = BindingContext as GarageViewModel;
        if (vm != null)
        {
            await vm.LoadVehiclesCommand.ExecuteAsync(null);
        }
    }
}