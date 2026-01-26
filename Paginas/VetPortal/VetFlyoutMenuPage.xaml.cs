using FrontEndHealthPets.Helpers;

namespace FrontEndHealthPets.Paginas.VetPortal;

public partial class VetFlyoutMenuPage : ContentPage
{
    public VetFlyoutMenuPage()
    {
        InitializeComponent();
        LoadVetInfo();
    }

    private void LoadVetInfo()
    {
        VetNameLabel.Text = $"Dr. {Settings.VetName}";
        VetSpecialtyLabel.Text = Settings.VetSpecialty ?? "Medicina General";
    }

    // Events to notify the FlyoutPage (VetMainPage)
    public event EventHandler? DashboardTapped;
    public event EventHandler? AppointmentsTapped;
    public event EventHandler? ScheduleTapped;
    public event EventHandler? ProfileTapped;
    public event EventHandler? LogoutClicked;

    private void OnDashboardTapped(object sender, TappedEventArgs e)
    {
        DashboardTapped?.Invoke(this, EventArgs.Empty);
    }

    private void OnAppointmentsTapped(object sender, TappedEventArgs e)
    {
        AppointmentsTapped?.Invoke(this, EventArgs.Empty);
    }

    private void OnScheduleTapped(object sender, TappedEventArgs e)
    {
        ScheduleTapped?.Invoke(this, EventArgs.Empty);
    }

    private void OnProfileTapped(object sender, TappedEventArgs e)
    {
        ProfileTapped?.Invoke(this, EventArgs.Empty);
    }

    private void OnLogoutClicked(object sender, EventArgs e)
    {
        LogoutClicked?.Invoke(this, EventArgs.Empty);
    }
}
