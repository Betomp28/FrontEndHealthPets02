namespace FrontEndHealthPets.Paginas.FlyPaginas
{
    public partial class BañosPage : ContentPage
    {
        public BañosPage()
        {
            InitializeComponent();
        }

        // Controlador del evento para el botón "Agregar nuevo baño"
        private void OnAddBathClicked(object sender, EventArgs e)
        {
            // Aquí puedes navegar a la página para agregar un baño o ejecutar otra lógica
            Navigation.PushAsync(new AgregarBañoPage());
        }
    }
}
