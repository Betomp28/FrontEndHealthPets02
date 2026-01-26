using FrontEndHealthPets.Helpers;
using FrontEndHealthPets.Models;
using System.Collections.ObjectModel;

namespace FrontEndHealthPets.Pages
{
    public partial class DashboardPage : ContentPage
    {
        private ObservableCollection<Pet> _pets = new();

        public DashboardPage()
        {
            InitializeComponent();
            LoadData();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                // Load user info
                UserNameLabel.Text = Settings.UserName;
                
                // Set greeting based on time
                var hour = DateTime.Now.Hour;
                string greeting = hour < 12 ? "¡Buenos días!" :
                                hour < 18 ? "¡Buenas tardes!" :
                                "¡Buenas noches!";
                GreetingLabel.Text = greeting + " 👋";



                // Load pets from API
                await LoadPets();

                // Load stats
                PetsCountLabel.Text = _pets.Count.ToString();
                VaccinesCountLabel.Text = "0"; // TODO: Load from API
                AppointmentsCountLabel.Text = "0"; // TODO: Load from API
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DashboardPage] LoadData error: {ex.Message}");
            }
        }

        private async Task LoadPets()
        {
            try
            {
                var petService = ServiceHelper.GetService<Services.IPetService>() ?? new Services.PetService();
                var pets = await petService.GetPetsAsync();
                
                if (pets != null && pets.Count > 0)
                {
                    _pets = new ObservableCollection<Pet>(pets);
                }
                else
                {
                    _pets = new ObservableCollection<Pet>();
                }

                PetsCollectionView.ItemsSource = _pets;
                
                // Show empty state if no pets
                EmptyPetsState.IsVisible = _pets.Count == 0;
                PetsCollectionView.IsVisible = _pets.Count > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DashboardPage] LoadPets error: {ex.Message}");
                _pets = new ObservableCollection<Pet>();
                PetsCollectionView.ItemsSource = _pets;
                EmptyPetsState.IsVisible = true;
                PetsCollectionView.IsVisible = false;
            }
        }

        private async void Settings_Clicked(object sender, EventArgs e)
        {
            // TODO: Navigate to settings page
            await DisplayAlert("Configuración", "Página de configuración próximamente", "OK");
        }

        private async void ViewAllPets_Tapped(object sender, TappedEventArgs e)
        {
            // TODO: Navigate to pets list page
            await DisplayAlert("Mis Mascotas", "Lista completa de mascotas próximamente", "OK");
        }

        private async void Pet_Selected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count == 0)
                return;

            var pet = e.CurrentSelection[0] as Pet;
            if (pet != null)
            {
                // TODO: Navigate to pet detail page
                await DisplayAlert("Detalle", $"Ver detalle de {pet.Name}", "OK");
            }

            // Clear selection
            ((CollectionView)sender).SelectedItem = null;
        }

        private async void AddPet_Clicked(object sender, EventArgs e)
        {


            // TODO: Navigate to add pet page
            await DisplayAlert("Agregar Mascota", "Formulario de nueva mascota próximamente", "OK");
        }

        private async void AddVaccine_Clicked(object sender, EventArgs e)
        {
            if (_pets.Count == 0)
            {
                await DisplayAlert("Sin Mascotas", "Primero debes agregar una mascota", "OK");
                return;
            }

            // TODO: Navigate to add vaccine page
            await DisplayAlert("Registrar Vacuna", "Formulario de vacuna próximamente", "OK");
        }

        private async void AddAppointment_Clicked(object sender, EventArgs e)
        {
            if (_pets.Count == 0)
            {
                await DisplayAlert("Sin Mascotas", "Primero debes agregar una mascota", "OK");
                return;
            }

            // TODO: Navigate to add appointment page
            await DisplayAlert("Agendar Cita", "Formulario de cita próximamente", "OK");
        }

        private async void Medications_Clicked(object sender, EventArgs e)
        {
            if (_pets.Count == 0)
            {
                await DisplayAlert("Sin Mascotas", "Primero debes agregar una mascota para ver sus medicamentos", "OK");
                return;
            }

            await Shell.Current.GoToAsync(nameof(Paginas.MedicationPage));
        }

    }
}
