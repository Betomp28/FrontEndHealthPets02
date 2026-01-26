using FrontEndHealthPets.Helpers;
using FrontEndHealthPets.Models;
using FrontEndHealthPets.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FrontEndHealthPets.ViewModels
{
    public class PetsListViewModel : BaseViewModel
    {
        private readonly IPetService _petService;
        private readonly INavigationService _navigationService;

        private ObservableCollection<Pet> _pets = new();
        private ObservableCollection<Pet> _filteredPets = new();
        private string _searchText = string.Empty;
        private string _selectedFilter = "Todos";

        public ObservableCollection<Pet> Pets
        {
            get => _pets;
            set => SetProperty(ref _pets, value);
        }

        public ObservableCollection<Pet> FilteredPets
        {
            get => _filteredPets;
            set => SetProperty(ref _filteredPets, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    ApplyFilters();
                }
            }
        }

        public string SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                if (SetProperty(ref _selectedFilter, value))
                {
                    ApplyFilters();
                }
            }
        }

        public ICommand ViewPetCommand { get; }
        public ICommand AddPetCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand FilterCommand { get; }

        public PetsListViewModel(IPetService petService, INavigationService navigationService)
        {
            _petService = petService;
            _navigationService = navigationService;

            Title = "Mis Mascotas";

            ViewPetCommand = new Command<Pet>(async (pet) => await OnViewPet(pet));
            AddPetCommand = new Command(async () => await OnAddPet());
            RefreshCommand = new Command(async () => await LoadPetsAsync());
            FilterCommand = new Command<string>((filter) => SelectedFilter = filter);
        }

        public override async Task OnAppearing()
        {
            await base.OnAppearing();
            await LoadPetsAsync();
        }

        private async Task LoadPetsAsync()
        {
            await ExecuteWithLoading(async () =>
            {
                var pets = await _petService.GetPetsAsync();
                Pets = new ObservableCollection<Pet>(pets);
                ApplyFilters();
            });
        }

        private void ApplyFilters()
        {
            var filtered = Pets.AsEnumerable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(p =>
                    p.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    p.Breed.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                );
            }

            // Apply category filter
            if (SelectedFilter != "Todos")
            {
                // TODO: Add Species property to Pet model
                // filtered = filtered.Where(p => p.Species == SelectedFilter);
            }

            FilteredPets = new ObservableCollection<Pet>(filtered);
        }

        private async Task OnViewPet(Pet pet)
        {
            if (pet == null) return;
            var parameters = new Dictionary<string, object> { { "Pet", pet } };
            await _navigationService.NavigateToAsync(nameof(Paginas.FlyPaginas.PetProfilePage), parameters);
        }

        private async Task OnAddPet()
        {
            await _navigationService.NavigateToAsync("AddPet");
        }
    }
}
