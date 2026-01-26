using FrontEndHealthPets.Models;
using FrontEndHealthPets.Services;

namespace FrontEndHealthPets.Paginas
{
    public partial class IngresarMascotas : ContentPage
    {
        private string _selectedGender = "";
        private string? _selectedPhotoPath = null;
        private readonly IPetService _petService;

        public IngresarMascotas()
        {
            InitializeComponent();
            _petService = new PetService();
            
            // Set default date to 1 year ago
            FechaNacimiento.Date = DateTime.Today.AddYears(-1);
            FechaNacimiento.MaximumDate = DateTime.Today;
        }

        private async void OnSelectPhotoTapped(object sender, EventArgs e)
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Selecciona una foto"
                });

                if (result != null)
                {
                    _selectedPhotoPath = result.FullPath;
                    imgPerfil.Source = ImageSource.FromFile(result.FullPath);
                }
            }
            catch (Exception ex)
            {
                // Check if camera/gallery permission is available
                var status = await Permissions.CheckStatusAsync<Permissions.Photos>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.Photos>();
                }
                
                if (status != PermissionStatus.Granted)
                {
                    await DisplayAlert("Permiso Requerido", "Se necesita acceso a las fotos para seleccionar una imagen", "OK");
                }
                else
                {
                    await DisplayAlert("Error", $"No se pudo seleccionar la foto: {ex.Message}", "OK");
                }
            }
        }

        private void OnMaleTapped(object sender, EventArgs e)
        {
            _selectedGender = "Macho";
            MaleFrame.BackgroundColor = Microsoft.Maui.Graphics.Color.FromArgb("#2196F3");
            MaleFrame.BorderColor = Microsoft.Maui.Graphics.Color.FromArgb("#2196F3");
            if (MaleFrame.Content is HorizontalStackLayout maleStack && maleStack.Children.Count > 1 && maleStack.Children[1] is Label maleLabel)
            {
                maleLabel.TextColor = Colors.White;
            }
            
            FemaleFrame.BackgroundColor = Microsoft.Maui.Graphics.Color.FromArgb("#FFE8EC");
            FemaleFrame.BorderColor = Colors.Transparent;
            if (FemaleFrame.Content is HorizontalStackLayout femaleStack && femaleStack.Children.Count > 1 && femaleStack.Children[1] is Label femaleLabel)
            {
                femaleLabel.TextColor = Microsoft.Maui.Graphics.Color.FromArgb("#E91E63");
            }
        }

        private void OnFemaleTapped(object sender, EventArgs e)
        {
            _selectedGender = "Hembra";
            FemaleFrame.BackgroundColor = Microsoft.Maui.Graphics.Color.FromArgb("#E91E63");
            FemaleFrame.BorderColor = Microsoft.Maui.Graphics.Color.FromArgb("#E91E63");
            if (FemaleFrame.Content is HorizontalStackLayout femaleStack && femaleStack.Children.Count > 1 && femaleStack.Children[1] is Label femaleLabel)
            {
                femaleLabel.TextColor = Colors.White;
            }
            
            MaleFrame.BackgroundColor = Microsoft.Maui.Graphics.Color.FromArgb("#E8F5FF");
            MaleFrame.BorderColor = Colors.Transparent;
            if (MaleFrame.Content is HorizontalStackLayout maleStack && maleStack.Children.Count > 1 && maleStack.Children[1] is Label maleLabel)
            {
                maleLabel.TextColor = Microsoft.Maui.Graphics.Color.FromArgb("#2196F3");
            }
        }

        private async void btRegistrar_Clicked(object sender, EventArgs e)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(Nombre.Text))
            {
                NombreError.Text = "El nombre es requerido";
                NombreError.IsVisible = true;
                Nombre.Focus();
                return;
            }
            NombreError.IsVisible = false;

            if (EspeciePicker.SelectedIndex < 0)
            {
                await DisplayAlert("Campo Requerido", "Por favor selecciona la especie", "OK");
                return;
            }

            // Show loading
            LoadingOverlay.IsVisible = true;
            btRegistrar.IsEnabled = false;

            try
            {
                // Get species without emoji
                var especieText = EspeciePicker.SelectedItem?.ToString() ?? "";
                var especie = especieText.Length > 2 ? especieText.Substring(2).Trim() : especieText;

                // Parse weight
                double peso = 0;
                if (!string.IsNullOrWhiteSpace(Peso.Text))
                {
                    double.TryParse(Peso.Text.Replace(",", "."), System.Globalization.NumberStyles.Any, 
                        System.Globalization.CultureInfo.InvariantCulture, out peso);
                }

                var pet = new Pet
                {
                    Name = Nombre.Text.Trim(),
                    Species = especie,
                    Breed = Raza.Text?.Trim() ?? "",
                    BirthDate = FechaNacimiento.Date,
                    Gender = _selectedGender,
                    Weight = peso,
                    Color = ColorEntry.Text?.Trim() ?? "",
                    Notes = Notas.Text?.Trim() ?? "",
                    PhotoUrl = _selectedPhotoPath ?? ""
                };

                var (success, savedPet, error) = await _petService.AddPetAsync(pet);

                if (success)
                {
                    await DisplayAlert("¡Éxito!", $"{pet.Name} ha sido registrado exitosamente", "OK");
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Error", error ?? "No se pudo guardar la mascota", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
            }
            finally
            {
                LoadingOverlay.IsVisible = false;
                btRegistrar.IsEnabled = true;
            }
        }

        private async void btCancelar_Clicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert(
                "Cancelar",
                "¿Estás seguro de que quieres cancelar? Los datos no guardados se perderán.",
                "Sí, cancelar",
                "No"
            );

            if (confirm)
            {
                await Navigation.PopAsync();
            }
        }

        // Legacy method for backwards compatibility
        private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            OnSelectPhotoTapped(sender, e);
        }
    }
}