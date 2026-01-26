using FrontEndHealthPets.Models;
using FrontEndHealthPets.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FrontEndHealthPets.ViewModels
{
    public class VeterinaryDirectoryViewModel : BaseViewModel
    {
        private readonly IVeterinaryService _veterinaryService;

        private ObservableCollection<Veterinary> _veterinaries = new();
        private ObservableCollection<Veterinary> _featuredVeterinaries = new();
        private string _searchText = string.Empty;
        private bool _isRefreshing;
        private Veterinary? _selectedVeterinary;

        // User's current location (default: San José, Costa Rica)
        private double _userLatitude = 9.9281;
        private double _userLongitude = -84.0907;

        public ObservableCollection<Veterinary> Veterinaries
        {
            get => _veterinaries;
            set => SetProperty(ref _veterinaries, value);
        }

        public ObservableCollection<Veterinary> FeaturedVeterinaries
        {
            get => _featuredVeterinaries;
            set => SetProperty(ref _featuredVeterinaries, value);
        }

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public Veterinary? SelectedVeterinary
        {
            get => _selectedVeterinary;
            set => SetProperty(ref _selectedVeterinary, value);
        }

        public ICommand LoadDataCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand CallVeterinaryCommand { get; }
        public ICommand OpenMapsCommand { get; }
        public ICommand ViewDetailsCommand { get; }

        public VeterinaryDirectoryViewModel(IVeterinaryService veterinaryService)
        {
            _veterinaryService = veterinaryService;
            Title = "Veterinarias Cercanas";

            LoadDataCommand = new Command(async () => await LoadDataAsync());
            SearchCommand = new Command(async () => await SearchAsync());
            RefreshCommand = new Command(async () => await RefreshAsync());
            CallVeterinaryCommand = new Command<Veterinary>(async (vet) => await CallVeterinary(vet));
            OpenMapsCommand = new Command<Veterinary>(async (vet) => await OpenMaps(vet));
            ViewDetailsCommand = new Command<Veterinary>(async (vet) => await ViewDetails(vet));
        }

        private async Task LoadDataAsync()
        {
            IsBusy = true;
            try
            {
                // Get user location (if available)
                await TryGetUserLocation();

                // Load nearby veterinaries
                var nearby = await _veterinaryService.GetNearbyVeterinariesAsync(_userLatitude, _userLongitude);
                Veterinaries = new ObservableCollection<Veterinary>(nearby);

                // Load featured
                var featured = await _veterinaryService.GetFeaturedVeterinariesAsync();
                FeaturedVeterinaries = new ObservableCollection<Veterinary>(featured);
            }
            catch (Exception ex)
            {
                await ShowAlert("Error", "No se pudieron cargar las veterinarias");
                System.Diagnostics.Debug.WriteLine($"Error loading veterinaries: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        private async Task SearchAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                await LoadDataAsync();
                return;
            }

            IsBusy = true;
            try
            {
                var results = await _veterinaryService.SearchVeterinariesAsync(SearchText);
                Veterinaries = new ObservableCollection<Veterinary>(results);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task RefreshAsync()
        {
            IsRefreshing = true;
            await LoadDataAsync();
        }

        private async Task CallVeterinary(Veterinary? vet)
        {
            if (vet == null || string.IsNullOrEmpty(vet.Telefono)) return;

            try
            {
                // Clean phone number
                var phone = vet.Telefono.Replace(" ", "").Replace("-", "");
                await Launcher.OpenAsync(new Uri($"tel:{phone}"));
            }
            catch (Exception ex)
            {
                await ShowAlert("Error", "No se pudo realizar la llamada");
                System.Diagnostics.Debug.WriteLine($"Error calling: {ex.Message}");
            }
        }

        private async Task OpenMaps(Veterinary? vet)
        {
            if (vet == null) return;

            try
            {
                var location = new Location(vet.Latitud, vet.Longitud);
                var options = new MapLaunchOptions
                {
                    Name = vet.Nombre,
                    NavigationMode = NavigationMode.Driving
                };
                await Map.OpenAsync(location, options);
            }
            catch (Exception ex)
            {
                await ShowAlert("Error", "No se pudo abrir el mapa");
                System.Diagnostics.Debug.WriteLine($"Error opening maps: {ex.Message}");
            }
        }

        private async Task ViewDetails(Veterinary? vet)
        {
            if (vet == null) return;
            
            // Show details in an alert for now
            var specialties = string.Join(", ", vet.Especialidades);
            await ShowAlert(
                vet.Nombre,
                $"📍 {vet.Direccion}\n" +
                $"📞 {vet.Telefono}\n" +
                $"🕐 {vet.HorarioAtencion}\n" +
                $"⭐ {vet.Rating:F1} ({vet.ReviewCount} reseñas)\n" +
                $"🏥 {specialties}"
            );
        }

        private async Task TryGetUserLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                var location = await Geolocation.GetLocationAsync(request);
                
                if (location != null)
                {
                    _userLatitude = location.Latitude;
                    _userLongitude = location.Longitude;
                }
            }
            catch (Exception ex)
            {
                // Use default location if geolocation fails
                System.Diagnostics.Debug.WriteLine($"Geolocation failed: {ex.Message}");
            }
        }
    }
}
