namespace FrontEndHealthPets.Paginas.FlyPaginas
{
    public partial class BañosPage : ContentPage
    {
        public BañosPage()
        {
            InitializeComponent();
        }

        private async void OnAddNewBathClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AgregarBañoPage());
        }
    }
}
