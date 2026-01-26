using FrontEndHealthPets.Paginas.FlyPaginas;

namespace FrontEndHealthPets.Paginas
{
    public partial class PagFlyPrincipal : FlyoutPage
    {
        public PagFlyPrincipal()
        {
            InitializeComponent();
            
            // Set initial detail page
            Detail = new NavigationPage(new MisMascotas());
        }
    }
}
