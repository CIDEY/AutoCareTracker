namespace AutoCareTracker
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Регистрируем страницу (создадим её чуть позже)
            Routing.RegisterRoute("AddRecordPage", typeof(AutoCareTracker.Views.AddRecordPage));
        }
    }
}
