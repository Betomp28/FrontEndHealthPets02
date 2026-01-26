using FrontEndHealthPets.Helpers;
using FrontEndHealthPets.Models;
using FrontEndHealthPets.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FrontEndHealthPets.ViewModels
{
    public class PetDetailViewModel : BaseViewModel
    {
        private readonly IPetService _petService;
        private readonly INavigationService _navigationService;
        private readonly INotificationService _notificationService;

        private Pet _pet;
        private ObservableCollection<VaccineRecord> _vaccines = new();
        private ObservableCollection<Appointment> _appointments = new();
        private ObservableCollection<PetPhoto> _photos = new();

        public Pet Pet
        {
            get => _pet;
            set => SetProperty(ref _pet, value);
        }

        public ObservableCollection<VaccineRecord> Vaccines
        {
            get => _vaccines;
            set => SetProperty(ref _vaccines, value);
        }

        public ObservableCollection<Appointment> Appointments
        {
            get => _appointments;
            set => SetProperty(ref _appointments, value);
        }

        public ObservableCollection<PetPhoto> Photos
        {
            get => _photos;
            set => SetProperty(ref _photos, value);
        }

        public bool HasPhotos => Photos?.Count > 0;

        public ICommand AddPhotoCommand { get; }
        public ICommand AddVaccineCommand { get; }
        public ICommand AddAppointmentCommand { get; }
        public ICommand EditPetCommand { get; }
        public ICommand DeletePetCommand { get; }
        public ICommand ViewAllVaccinesCommand { get; }
        public ICommand ViewAllAppointmentsCommand { get; }
        public ICommand DeletePhotoCommand { get; }

        public PetDetailViewModel(
            IPetService petService,
            INavigationService navigationService,
            INotificationService notificationService)
        {
            _petService = petService;
            _navigationService = navigationService;
            _notificationService = notificationService;

            Title = "Detalle de Mascota";

            AddPhotoCommand = new Command(async () => await OnAddPhoto());
            AddVaccineCommand = new Command(async () => await OnAddVaccine());
            AddAppointmentCommand = new Command(async () => await OnAddAppointment());
            EditPetCommand = new Command(async () => await OnEditPet());
            DeletePetCommand = new Command(async () => await OnDeletePet());
            ViewAllVaccinesCommand = new Command(async () => await OnViewAllVaccines());
            ViewAllAppointmentsCommand = new Command(async () => await OnViewAllAppointments());
            DeletePhotoCommand = new Command<PetPhoto>(async (photo) => await OnDeletePhoto(photo));
        }

        public async Task LoadPetAsync(long petId)
        {
            await ExecuteWithLoading(async () =>
            {
                Pet = await _petService.GetPetByIdAsync(petId);
                
                if (Pet != null)
                {
                    Title = Pet.Name;
                    
                    // Load vaccines and appointments
                    Vaccines = new ObservableCollection<VaccineRecord>(
                        await _petService.GetPetVaccinesAsync(petId)
                    );

                    Appointments = new ObservableCollection<Appointment>(
                        await _petService.GetPetAppointmentsAsync(petId)
                    );

                    // Load photos
                    Photos = new ObservableCollection<PetPhoto>(
                        await _petService.GetPetPhotosAsync(petId)
                    );
                    OnPropertyChanged(nameof(HasPhotos));
                }
            });
        }

        private async Task OnAddPhoto()
        {
            try
            {
                var result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Seleccionar foto"
                });

                if (result != null)
                {
                    IsBusy = true;
                    
                    using var stream = await result.OpenReadAsync();
                    var uploadResult = await _petService.UploadPhotoAsync(Pet.Id, stream, result.FileName);
                    
                    if (uploadResult.success)
                    {
                        // Reload photos
                        Photos = new ObservableCollection<PetPhoto>(
                            await _petService.GetPetPhotosAsync(Pet.Id)
                        );
                        OnPropertyChanged(nameof(HasPhotos));
                        await ShowAlert("Éxito", "Foto agregada correctamente", "OK");
                    }
                    else
                    {
                        await ShowAlert("Error", uploadResult.error, "OK");
                    }
                    
                    IsBusy = false;
                }
            }
            catch (Exception ex)
            {
                await ShowAlert("Error", $"No se pudo agregar la foto: {ex.Message}", "OK");
                IsBusy = false;
            }
        }

        private async Task OnDeletePhoto(PetPhoto photo)
        {
            if (photo == null) return;

            bool confirm = await ShowConfirmation(
                "Eliminar Foto",
                "¿Estás seguro de que deseas eliminar esta foto?",
                "Eliminar",
                "Cancelar"
            );

            if (confirm)
            {
                IsBusy = true;
                var success = await _petService.DeletePetPhotoAsync(Pet.Id, photo.Id);
                
                if (success)
                {
                    Photos.Remove(photo);
                    OnPropertyChanged(nameof(HasPhotos));
                }
                else
                {
                    await ShowAlert("Error", "No se pudo eliminar la foto", "OK");
                }
                IsBusy = false;
            }
        }


        private async Task OnAddVaccine()
        {
            await _navigationService.NavigateToAsync($"AddVaccine?petId={Pet.Id}");
        }

        private async Task OnAddAppointment()
        {
            await _navigationService.NavigateToAsync($"AddAppointment?petId={Pet.Id}");
        }

        private async Task OnEditPet()
        {
            await _navigationService.NavigateToAsync($"EditPet?petId={Pet.Id}");
        }

        private async Task OnDeletePet()
        {
            bool confirm = await ShowConfirmation(
                "Eliminar Mascota",
                $"¿Estás seguro de que deseas eliminar a {Pet.Name}? Esta acción no se puede deshacer.",
                "Eliminar",
                "Cancelar"
            );

            if (confirm)
            {
                await ExecuteWithLoading(async () =>
                {
                    // Cancel all notifications
                    await _notificationService.CancelAllPetNotificationsAsync(Pet.Id);
                    
                    // Delete pet
                    var result = await _petService.DeletePetAsync(Pet.Id);
                    
                    if (result.success)
                    {
                        await ShowAlert("Eliminado", $"{Pet.Name} ha sido eliminado", "OK");
                        await _navigationService.GoBackAsync();
                    }
                    else
                    {
                        await ShowAlert("Error", result.error ?? "No se pudo eliminar", "OK");
                    }
                });
            }
        }

        private async Task OnViewAllVaccines()
        {
            await _navigationService.NavigateToAsync($"Vaccines?petId={Pet.Id}");
        }

        private async Task OnViewAllAppointments()
        {
            await _navigationService.NavigateToAsync($"Appointments?petId={Pet.Id}");
        }
    }
}
