using FrontEndHealthPets.Helpers;
using FrontEndHealthPets.Services;

namespace FrontEndHealthPets.Paginas.VetPortal;

public partial class VetMainPage : FlyoutPage
{
    private readonly IVetAuthenticationService _vetAuthService;
    private readonly IServiceProvider _serviceProvider;

    public VetMainPage(IVetAuthenticationService vetAuthService, IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _vetAuthService = vetAuthService;
        _serviceProvider = serviceProvider;

        // Subscribe to menu events
        flyoutMenu.DashboardTapped += OnDashboardClicked;
        flyoutMenu.AppointmentsTapped += OnAppointmentsClicked;
        flyoutMenu.ScheduleTapped += OnScheduleClicked;
        flyoutMenu.ProfileTapped += OnProfileClicked;
        flyoutMenu.LogoutClicked += OnLogoutClicked;

        // Load dashboard on start
        LoadDashboard();
    }

    private void LoadDashboard()
    {
        var dashboardPage = _serviceProvider.GetService<VetDashboardPage>();
        if (dashboardPage != null)
        {
            Detail = new NavigationPage(dashboardPage)
            {
                BarBackgroundColor = Color.FromArgb("#1a1a2e"),
                BarTextColor = Colors.White
            };
        }
        IsPresented = false;
    }

    private void OnDashboardClicked(object? sender, EventArgs e)
    {
        LoadDashboard();
    }

    private void OnAppointmentsClicked(object? sender, EventArgs e)
    {
        var page = _serviceProvider.GetService<VetMyAppointmentsPage>();
        if (page != null)
        {
            Detail = new NavigationPage(page)
            {
                BarBackgroundColor = Color.FromArgb("#1a1a2e"),
                BarTextColor = Colors.White
            };
        }
        IsPresented = false;
    }

    private void OnScheduleClicked(object? sender, EventArgs e)
    {
        // Use existing schedule management page
        var page = _serviceProvider.GetService<VetScheduleManagementPage>();
        if (page != null)
        {
            Detail = new NavigationPage(page)
            {
                BarBackgroundColor = Color.FromArgb("#1a1a2e"),
                BarTextColor = Colors.White
            };
        }
        IsPresented = false;
    }

    private void OnProfileClicked(object? sender, EventArgs e)
    {
        // Use existing profile edit page
        var page = _serviceProvider.GetService<VetProfileEditPage>();
        if (page != null)
        {
            Detail = new NavigationPage(page)
            {
                BarBackgroundColor = Color.FromArgb("#1a1a2e"),
                BarTextColor = Colors.White
            };
        }
        IsPresented = false;
    }

    private async void OnLogoutClicked(object? sender, EventArgs e)
    {
        bool confirm = await DisplayAlert(
            "Cerrar Sesión",
            "¿Estás seguro de que deseas cerrar sesión?",
            "Sí",
            "Cancelar");

        if (confirm)
        {
            await _vetAuthService.LogoutAsync();
            Application.Current!.MainPage = new AppShell();
            await Shell.Current.GoToAsync("//Login");
        }
    }
}
