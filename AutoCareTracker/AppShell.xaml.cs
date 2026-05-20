namespace AutoCareTracker
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("AddRecordPage", typeof(AutoCareTracker.Views.AddRecordPage));
        }
    }
}
