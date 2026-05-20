using AutoCareTracker.ViewModels;

namespace AutoCareTracker.Views;

public partial class AddRecordPage : ContentPage
{
	public AddRecordPage(AddRecordViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}