using FrontEndHealthPets.Helpers;
using FrontEndHealthPets.Models;
using FrontEndHealthPets.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FrontEndHealthPets.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly IPetService _petService;
        private readonly INavigationService _navigationService;
        private readonly IRecordatorioService? _recordatorioService;

        private string _greeting = "¡Hola!";
        private int _petsCount;
        private int _vaccinesCount;
        private int _appointmentsCount;
        private int _messagesCount;
        private int _remindersCount;
        private ObservableCollection<Pet> _recentPets = new();

        public string Greeting
        {
            get => _greeting;
            set => SetProperty(ref _greeting, value);
        }

        public int PetsCount
        {
            get => _petsCount;
            set => SetProperty(ref _petsCount, value);
        }

        public int VaccinesCount
        {
            get => _vaccinesCount;
            set => SetProperty(ref _vaccinesCount, value);
        }

        public int AppointmentsCount
        {
            get => _appointmentsCount;
            set => SetProperty(ref _appointmentsCount, value);
        }

        public int MessagesCount
        {
            get => _messagesCount;
            set => SetProperty(ref _messagesCount, value);
        }

        public int RemindersCount
        {
            get => _remindersCount;
            set => SetProperty(ref _remindersCount, value);
        }

        public ObservableCollection<Pet> RecentPets
        {
            get => _recentPets;
            set => SetProperty(ref _recentPets, value);
        }

        public ICommand ViewPetCommand { get; }
        public ICommand AddPetCommand { get; }
        public ICommand ViewAllPetsCommand { get; }
        public ICommand ViewRemindersCommand { get; }


        public DashboardViewModel(IPetService petService, INavigationService navigationService)
        {
            _petService = petService;
            _navigationService = navigationService;
            _recordatorioService = ServiceHelper.GetService<IRecordatorioService>();

            Title = "Dashboard";

            ViewPetCommand = new Command<Pet>(async (pet) => await OnViewPet(pet));
            AddPetCommand = new Command(async () => await OnAddPet());
            ViewAllPetsCommand = new Command(async () => await OnViewAllPets());
            ViewRemindersCommand = new Command(async () => await OnViewReminders());

        }

        public override async Task OnAppearing()
        {
            await base.OnAppearing();
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            await ExecuteWithLoading(async () =>
            {
                // Set greeting based on time
                var hour = DateTime.Now.Hour;
                Greeting = hour < 12 ? "¡Buenos días!" :
                          hour < 18 ? "¡Buenas tardes!" :
                          "¡Buenas noches!";

                // Load pets
                var pets = await _petService.GetPetsAsync();
                RecentPets = new ObservableCollection<Pet>(pets.Take(3));
                PetsCount = pets.Count;

                // Load vaccines count (sum from all pets)
                int totalVaccines = 0;
                foreach (var pet in pets)
                {
                    var vaccines = await _petService.GetPetVaccinesAsync(pet.Id);
                    totalVaccines += vaccines.Count(v => v.NextDueDate > DateTime.Now);
                }
                VaccinesCount = totalVaccines;

                // Load appointments count (would need appointments service, using placeholder)
                AppointmentsCount = 0;
                foreach (var pet in pets)
                {
                    var appointments = await _petService.GetPetAppointmentsAsync(pet.Id);
                    AppointmentsCount += appointments.Count;
                }

                // Load reminders count
                if (_recordatorioService != null)
                {
                    var reminders = await _recordatorioService.GetRecordatoriosAsync();
                    RemindersCount = reminders.Count;
                }

                // Messages count (placeholder - would need chat service)
                MessagesCount = 0;
            });
        }

        private async Task OnViewReminders()
        {
            // Navigate to Recordatorios page
            if (Application.Current?.MainPage is FlyoutPage flyout)
            {
                var page = ServiceHelper.GetService<FrontEndHealthPets.Paginas.RecordatoriosPage>();
                if (page != null)
                {
                    flyout.Detail = new NavigationPage(page);
                }
            }
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

        private async Task OnViewAllPets()
        {
            await _navigationService.NavigateToAsync("PetsList");
        }
    }
}
