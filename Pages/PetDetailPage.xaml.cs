using FrontEndHealthPets.Models;
using FrontEndHealthPets.Services;
using FrontEndHealthPets.Helpers;
using System.Collections.ObjectModel;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;

namespace FrontEndHealthPets.Pages
{
    public partial class PetDetailPage : ContentPage
    {
        private Pet _currentPet;
        private ObservableCollection<VaccineRecord> _vaccines = new();
        private ObservableCollection<Appointment> _appointments = new();
        private ObservableCollection<PetPhoto> _photos = new();

        public PetDetailPage(Pet pet)
        {
            InitializeComponent();
            _currentPet = pet;
            BindingContext = _currentPet;
            LoadPetData();
        }

        private async void LoadPetData()
        {
            try
            {
                var petService = ServiceHelper.GetService<IPetService>();
                if (petService != null)
                {
                    var freshPet = await petService.GetPetByIdAsync(_currentPet.Id);
                    if (freshPet != null)
                    {
                        // Update current pet properties to trigger bindings
                        _currentPet.Name = freshPet.Name;
                        _currentPet.Species = freshPet.Species;
                        _currentPet.Breed = freshPet.Breed;
                        _currentPet.BirthDate = freshPet.BirthDate;
                        _currentPet.Weight = freshPet.Weight;
                        _currentPet.Color = freshPet.Color;
                        _currentPet.Gender = freshPet.Gender;
                        _currentPet.PhotoUrl = freshPet.PhotoUrl;
                        _currentPet.MicrochipNumber = freshPet.MicrochipNumber;
                        _currentPet.Notes = freshPet.Notes;
                        
                        // Set gender badge colors manually as they are not currently bound
                        GenderBadge.IsVisible = !string.IsNullOrEmpty(_currentPet.Gender);
                        if (_currentPet.Gender?.ToLower() == "macho")
                        {
                            GenderBadge.BackgroundColor = Color.FromRgb(64, 128, 255);
                        }
                        else if (_currentPet.Gender?.ToLower() == "hembra")
                        {
                            GenderBadge.BackgroundColor = Color.FromRgb(255, 105, 180);
                        }
                    }

                    // Load photos
                    var photos = await petService.GetPetPhotosAsync(_currentPet.Id);
                    _photos = new ObservableCollection<PetPhoto>(photos);
                    PhotosCollectionView.ItemsSource = _photos;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading fresh pet data: {ex.Message}");
            }

            // Load vaccines (mock data for now)
            LoadVaccines();

            // Load appointments (mock data for now)
            LoadAppointments();
        }

        private void LoadVaccines()
        {
            // TODO: Replace with actual API call
            _vaccines = new ObservableCollection<VaccineRecord>
            {
                new VaccineRecord
                {
                    VaccineName = "Rabia",
                    DateAdministered = DateTime.Now.AddMonths(-6),
                    NextDueDate = DateTime.Now.AddMonths(6),
                    VeterinarianName = "Dr. Pérez"
                },
                new VaccineRecord
                {
                    VaccineName = "Parvovirus",
                    DateAdministered = DateTime.Now.AddMonths(-3),
                    NextDueDate = DateTime.Now.AddMonths(9),
                    VeterinarianName = "Dr. García"
                },
                new VaccineRecord
                {
                    VaccineName = "Moquillo",
                    DateAdministered = DateTime.Now.AddMonths(-4),
                    NextDueDate = DateTime.Now.AddDays(-10), // Overdue
                    VeterinarianName = "Dr. Martínez"
                }
            };

            VaccinesCollectionView.ItemsSource = _vaccines;
        }

        private void LoadAppointments()
        {
            // TODO: Replace with actual API call
            _appointments = new ObservableCollection<Appointment>
            {
                new Appointment
                {
                    ClinicName = "Clínica Veterinaria San José",
                    Reason = "Revisión general",
                },
                new Appointment
                {
                    ClinicName = "Hospital Veterinario Central",
                    Reason = "Vacunación anual",
                }
            };

            AppointmentsCollectionView.ItemsSource = _appointments;
        }

        private async void Edit_Clicked(object sender, EventArgs e)
        {
            // TODO: Navigate to edit pet page
            await DisplayAlert("Editar", $"Editar información de {_currentPet.Name}", "OK");
        }

        private async void AddPhoto_Clicked(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet(
                "Agregar Foto",
                "Cancelar",
                null,
                "Tomar Foto",
                "Elegir de Galería"
            );

            if (action == "Tomar Foto")
            {
                try
                {
                    if (!MediaPicker.Default.IsCaptureSupported)
                    {
                        await DisplayAlert("Error", "La cámara no está disponible en este dispositivo", "OK");
                        return;
                    }

                    var photo = await MediaPicker.Default.CapturePhotoAsync();
                    if (photo != null)
                    {
                        await UploadProfilePhoto(photo);
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"No se pudo acceder a la cámara: {ex.Message}", "OK");
                }
            }
            else if (action == "Elegir de Galería")
            {
                try
                {
                    var result = await FilePicker.PickAsync(new PickOptions
                    {
                        PickerTitle = "Selecciona una foto de perfil",
                        FileTypes = FilePickerFileType.Images
                    });

                    if (result != null)
                    {
                        await UploadProfilePhotoFromFile(result);
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"No se pudo seleccionar la foto: {ex.Message}", "OK");
                }
            }
        }

        private async Task UploadProfilePhoto(FileResult photo)
        {
            try
            {
                var petService = ServiceHelper.GetService<IPetService>();
                if (petService == null) return;

                using var stream = await photo.OpenReadAsync();
                var result = await petService.UploadProfileImageAsync(_currentPet.Id, stream, photo.FileName);

                if (result.success)
                {
                    MainThread.BeginInvokeOnMainThread(() => {
                        _currentPet.PhotoUrl = result.photoUrl ?? "";
                        _currentPet.CacheBuster = DateTime.Now.Ticks.ToString();
                    });
                    await DisplayAlert("Éxito", "Foto de perfil actualizada", "OK");
                }
                else
                {
                    await DisplayAlert("Error", result.error, "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo subir la foto: {ex.Message}", "OK");
            }
        }

        private async Task UploadProfilePhotoFromFile(FileResult file)
        {
            try
            {
                var petService = ServiceHelper.GetService<IPetService>();
                if (petService == null) return;

                using var stream = await file.OpenReadAsync();
                var result = await petService.UploadProfileImageAsync(_currentPet.Id, stream, file.FileName);

                if (result.success)
                {
                    MainThread.BeginInvokeOnMainThread(() => {
                        _currentPet.PhotoUrl = result.photoUrl ?? "";
                        _currentPet.CacheBuster = DateTime.Now.Ticks.ToString();
                    });
                    await DisplayAlert("Éxito", "Foto de perfil actualizada", "OK");
                }
                else
                {
                    await DisplayAlert("Error", result.error, "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo subir la foto: {ex.Message}", "OK");
            }
        }

        private async void AddVaccine_Clicked(object sender, EventArgs e)
        {
            // TODO: Navigate to add vaccine page
            await DisplayAlert("Agregar Vacuna", $"Registrar nueva vacuna para {_currentPet.Name}", "OK");
        }

        private async void AddAppointment_Clicked(object sender, EventArgs e)
        {
            // TODO: Navigate to add appointment page
            await DisplayAlert("Agendar Cita", $"Agendar cita para {_currentPet.Name}", "OK");
        }

        private async void ViewAllVaccines_Tapped(object sender, TappedEventArgs e)
        {
            // TODO: Navigate to vaccines list page
            await DisplayAlert("Vacunas", $"Ver todas las vacunas de {_currentPet.Name}", "OK");
        }

        private async void ViewAllAppointments_Tapped(object sender, TappedEventArgs e)
        {
            // TODO: Navigate to appointments list page
            await DisplayAlert("Citas", $"Ver todas las citas de {_currentPet.Name}", "OK");
        }

        private async void DeletePhoto_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is PetPhoto photo)
            {
                bool confirm = await DisplayAlert(
                    "Eliminar Foto",
                    "¿Estás seguro de que deseas eliminar esta foto?",
                    "Eliminar",
                    "Cancelar"
                );

                if (confirm)
                {
                    try
                    {
                        var petService = ServiceHelper.GetService<IPetService>();
                        if (petService != null)
                        {
                            var success = await petService.DeletePetPhotoAsync(_currentPet.Id, photo.Id);
                            if (success)
                            {
                                _photos.Remove(photo);
                            }
                            else
                            {
                                await DisplayAlert("Error", "No se pudo eliminar la foto", "OK");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", $"No se pudo eliminar la foto: {ex.Message}", "OK");
                    }
                }
            }
        }

        private async void DeletePet_Clicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert(
                "Eliminar Mascota",
                $"¿Estás seguro de que deseas eliminar a {_currentPet.Name}? Esta acción no se puede deshacer.",
                "Eliminar",
                "Cancelar"
            );

            if (confirm)
            {
                // TODO: Implement delete pet API call
                await DisplayAlert("Eliminado", $"{_currentPet.Name} ha sido eliminado", "OK");
                await Navigation.PopAsync();
            }
        }

        private double _startTranslationY;
        private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    _startTranslationY = _currentPet.PhotoOffsetY;
                    break;

                case GestureStatus.Running:
                    // Only vertical movement
                    double newOffset = _startTranslationY + e.TotalY;
                    _currentPet.PhotoOffsetY = newOffset;
                    SaveOffsetButton.IsVisible = true;
                    break;

                case GestureStatus.Completed:
                    break;
            }
        }

        private async void SaveOffset_Clicked(object sender, EventArgs e)
        {
            try
            {
                var petService = ServiceHelper.GetService<IPetService>();
                if (petService != null)
                {
                    var result = await petService.UpdatePhotoOffsetAsync(_currentPet.Id, _currentPet.PhotoOffsetY);
                    if (result.success)
                    {
                        SaveOffsetButton.IsVisible = false;
                        await DisplayAlert("Éxito", "Posición de la foto guardada", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Error", $"No se pudo guardar la posición: {result.error}", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
