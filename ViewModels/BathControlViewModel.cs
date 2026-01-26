using FrontEndHealthPets.Models;
using FrontEndHealthPets.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FrontEndHealthPets.ViewModels
{
    public class BathControlViewModel : BindableObject
    {
        private readonly IBathService _bathService;
        private readonly IPetService _petService;
        
        // State
        private int _selectedPetIndex = -1;
        private Pet _selectedPet;
        private bool _isBusy;

        public BathControlViewModel()
        {
            // Manual DI resolver since constructor injection might not be fully set up for ViewModels in this project style yet
            // If we use DI properly, we should inject these. Assuming ServiceHelper or manual resolution for now if needed, 
            // but let's try Constructor Injection as MauiProgram registers it.
            _bathService = Helpers.ServiceHelper.GetService<IBathService>() ?? new BathService();
            _petService = Helpers.ServiceHelper.GetService<IPetService>() ?? new PetService();

            Pets = new ObservableCollection<Pet>();
            BathHistory = new ObservableCollection<BathRecord>();

            LoadPetsCommand = new Command(async () => await LoadPetsAsync());
            AddBathCommand = new Command(async () => await AddBathAsync());
            DeleteBathCommand = new Command<int>(async (id) => await DeleteBathAsync(id));
        }

        // Properties
        public ObservableCollection<Pet> Pets { get; }
        public ObservableCollection<BathRecord> BathHistory { get; }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged();
                    // Force commands to re-evaluate their CanExecute
                    (AddBathCommand as Command)?.ChangeCanExecute();
                }
            }
        }

        public Pet SelectedPet
        {
            get => _selectedPet;
            set
            {
                if (_selectedPet != value)
                {
                    _selectedPet = value;
                    OnPropertyChanged();
                    if (_selectedPet != null)
                    {
                        LoadHistoryAsync(_selectedPet.Id);
                    }
                    else
                    {
                        BathHistory.Clear();
                    }
                }
            }
        }
        
        public int SelectedPetIndex
        {
            get => _selectedPetIndex;
            set
            {
                if (_selectedPetIndex != value)
                {
                    _selectedPetIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        // Commands
        public ICommand LoadPetsCommand { get; }
        public ICommand AddBathCommand { get; }
        public ICommand DeleteBathCommand { get; }

        public async Task InitializeAsync()
        {
            await LoadPetsAsync();
        }

        private async Task LoadPetsAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                Pets.Clear();
                var pets = await _petService.GetPetsAsync();
                foreach (var pet in pets)
                {
                    Pets.Add(pet);
                }

                if (Pets.Count > 0)
                {
                    SelectedPet = Pets[0];
                    SelectedPetIndex = 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading pets: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "No se pudieron cargar las mascotas", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadHistoryAsync(long petId)
        {
            IsBusy = true;
            try
            {
                BathHistory.Clear();
                var history = await _bathService.GetBathHistoryAsync(petId);
                foreach (var record in history)
                {
                    BathHistory.Add(record);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading bath history: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task AddBathAsync()
        {
            if (SelectedPet == null) 
            {
                await Application.Current.MainPage.DisplayAlert("Aviso", "Selecciona una mascota primero", "OK");
                return;
            }

            // Simple input for MVP - in a real app this should be a separate page or modal
            string groomer = await Application.Current.MainPage.DisplayPromptAsync("Nuevo Baño", "Lugar / Groomer:", maxLength: 50);
            if (string.IsNullOrEmpty(groomer)) return;

            string notes = await Application.Current.MainPage.DisplayPromptAsync("Nuevo Baño", "Notas (opcional):", maxLength: 100);

            var record = new BathRecord
            {
                PetId = SelectedPet.Id,
                Date = DateTime.Now,
                GroomerName = groomer,
                Notes = notes,
                NextBathDate = DateTime.Now.AddDays(30) // Default suggestion
            };

            await _bathService.AddBathRecordAsync(record);
            await LoadHistoryAsync(SelectedPet.Id);
        }

        private async Task DeleteBathAsync(int id)
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert("Confirmar", "¿Eliminar este registro?", "Sí", "No");
            if (confirm)
            {
                await _bathService.DeleteBathRecordAsync(id);
                if (SelectedPet != null)
                {
                    await LoadHistoryAsync(SelectedPet.Id);
                }
            }
        }
    }
}
