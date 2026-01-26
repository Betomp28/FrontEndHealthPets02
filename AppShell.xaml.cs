namespace FrontEndHealthPets
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            
            // Register navigation routes
            Routing.RegisterRoute(nameof(Paginas.FlyPaginas.PetProfilePage), typeof(Paginas.FlyPaginas.PetProfilePage));

            Routing.RegisterRoute(nameof(Paginas.VeterinaryDirectoryPage), typeof(Paginas.VeterinaryDirectoryPage));
            Routing.RegisterRoute(nameof(Paginas.VeterinarianProfilePage), typeof(Paginas.VeterinarianProfilePage));
            Routing.RegisterRoute(nameof(Paginas.MedicationPage), typeof(Paginas.MedicationPage));
            Routing.RegisterRoute(nameof(Paginas.WeightTrackingPage), typeof(Paginas.WeightTrackingPage));

            // Vet Portal Routes
            Routing.RegisterRoute("VetLogin", typeof(Paginas.VetPortal.VetLoginPage));
        }
    }
}
