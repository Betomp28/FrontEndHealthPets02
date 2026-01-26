using FrontEndHealthPets.Models;
using FrontEndHealthPets.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FrontEndHealthPets.ViewModels
{
    [QueryProperty(nameof(Pet), "Pet")]
    public class PetProfileViewModel : BaseViewModel
    {
        private readonly IPetService _petService;
        private readonly INotificationService _notificationService;

        private Pet? _pet;
        private ObservableCollection<VaccineRecord> _vaccines = new();
        private ObservableCollection<Appointment> _appointments = new();
        private ObservableCollection<PetPhoto> _galleryPhotos = new();
        private bool _isVaccinesTabVisible = true;
        private bool _isAppointmentsTabVisible = false;
        private bool _isGalleryTabVisible = false;

        public Pet? Pet
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

        public ObservableCollection<PetPhoto> GalleryPhotos
        {
            get => _galleryPhotos;
            set => SetProperty(ref _galleryPhotos, value);
        }

        public bool IsVaccinesTabVisible
        {
            get => _isVaccinesTabVisible;
            set => SetProperty(ref _isVaccinesTabVisible, value);
        }

        public bool IsAppointmentsTabVisible
        {
            get => _isAppointmentsTabVisible;
            set => SetProperty(ref _isAppointmentsTabVisible, value);
        }

        public bool IsGalleryTabVisible
        {
            get => _isGalleryTabVisible;
            set => SetProperty(ref _isGalleryTabVisible, value);
        }

        public ICommand LoadDataCommand { get; }
        public ICommand ShowVaccinesCommand { get; }
        public ICommand ShowAppointmentsCommand { get; }
        public ICommand ShowGalleryCommand { get; }
        public ICommand AddVaccineCommand { get; }
        public ICommand AddAppointmentCommand { get; }
        public ICommand UploadProfilePhotoCommand { get; }
        public ICommand UploadGalleryPhotoCommand { get; }
        public ICommand DeleteGalleryPhotoCommand { get; }

        public PetProfileViewModel(IPetService petService, INotificationService notificationService)
        {
            _petService = petService;
            _notificationService = notificationService;
            Title = "Perfil de Mascota";

            LoadDataCommand = new Command(async () => await LoadDataAsync());
            ShowVaccinesCommand = new Command(ShowVaccines);
            ShowAppointmentsCommand = new Command(ShowAppointments);
            ShowGalleryCommand = new Command(ShowGallery);
            AddVaccineCommand = new Command(async () => await AddVaccine());
            AddAppointmentCommand = new Command(async () => await AddAppointment());
            UploadProfilePhotoCommand = new Command(async () => await UploadProfilePhoto());
            UploadGalleryPhotoCommand = new Command(async () => await UploadGalleryPhoto());
            DeleteGalleryPhotoCommand = new Command<PetPhoto>(async (p) => await DeleteGalleryPhoto(p));
        }

        private async Task LoadDataAsync()
        {
            if (Pet == null) return;

            IsBusy = true;
            try
            {
                var vaccineList = await _petService.GetPetVaccinesAsync(Pet.Id);
                Vaccines = new ObservableCollection<VaccineRecord>(vaccineList);

                var appointmentList = await _petService.GetPetAppointmentsAsync(Pet.Id);
                Appointments = new ObservableCollection<Appointment>(appointmentList);

                var photoList = await _petService.GetPetPhotosAsync(Pet.Id);
                GalleryPhotos = new ObservableCollection<PetPhoto>(photoList);
            }
            catch (Exception ex)
            {
                await ShowAlert("Error", "Error al cargar datos de la mascota");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ShowVaccines()
        {
            IsVaccinesTabVisible = true;
            IsAppointmentsTabVisible = false;
        }

        private void ShowAppointments()
        {
            IsAppointmentsTabVisible = true;
            IsGalleryTabVisible = false;
        }

        private void ShowGallery()
        {
            IsVaccinesTabVisible = false;
            IsAppointmentsTabVisible = false;
            IsGalleryTabVisible = true;
        }

        private async Task AddVaccine()
        {
            await ShowAlert("Próximamente", "Funcionalidad para agregar vacunas estará disponible pronto.");
        }

        private async Task AddAppointment()
        {
            await ShowAlert("Próximamente", "Funcionalidad para agregar citas estará disponible pronto.");
        }

        private async Task UploadProfilePhoto()
        {
            if (Pet == null) return;

            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Selecciona una foto de perfil",
                    FileTypes = FilePickerFileType.Images
                });

                if (result == null) return;

                IsBusy = true;
                using var stream = await result.OpenReadAsync();
                var uploadResult = await _petService.UploadProfileImageAsync(Pet.Id, stream, result.FileName);

                if (uploadResult.success)
                {
                    Pet.PhotoUrl = uploadResult.photoUrl ?? "";
                    Pet.CacheBuster = DateTime.Now.Ticks.ToString();
                    await ShowAlert("Éxito", "Foto de perfil actualizada");
                }
                else
                {
                    await ShowAlert("Error", uploadResult.error);
                }
            }
            catch (Exception ex)
            {
                await ShowAlert("Error", $"No se pudo subir la foto: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task UploadGalleryPhoto()
        {
            if (Pet == null) return;

            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Selecciona una foto para la galería",
                    FileTypes = FilePickerFileType.Images
                });

                if (result == null) return;

                IsBusy = true;
                using var stream = await result.OpenReadAsync();
                var uploadResult = await _petService.UploadPhotoAsync(Pet.Id, stream, result.FileName);

                if (uploadResult.success)
                {
                    // Refresh gallery
                    var photoList = await _petService.GetPetPhotosAsync(Pet.Id);
                    GalleryPhotos = new ObservableCollection<PetPhoto>(photoList);
                    await ShowAlert("Éxito", "Foto agregada a la galería");
                }
                else
                {
                    await ShowAlert("Error", uploadResult.error);
                }
            }
            catch (Exception ex)
            {
                await ShowAlert("Error", $"No se pudo subir la foto: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task DeleteGalleryPhoto(PetPhoto photo)
        {
            if (Pet == null || photo == null) return;

            bool confirm = await Application.Current.MainPage.DisplayAlert("Confirmar", "¿Eliminar esta foto?", "Sí", "No");
            if (!confirm) return;

            IsBusy = true;
            try
            {
                var success = await _petService.DeletePetPhotoAsync(Pet.Id, photo.Id);
                if (success)
                {
                    GalleryPhotos.Remove(photo);
                }
                else
                {
                    await ShowAlert("Error", "No se pudo eliminar la foto");
                }
            }
            catch (Exception ex)
            {
                await ShowAlert("Error", ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
