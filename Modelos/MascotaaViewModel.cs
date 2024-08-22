using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using FrontEndHealthPets.Modelos;
using Microsoft.Maui.Controls;
using FrontEndHealthPets.Paginas.FlyPaginas;

namespace FrontEndHealthPets.Modelos
{
    public class MascotasViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<PerfilMascota> perfilMascotas;

        public ObservableCollection<PerfilMascota> PerfilMascotas
        {
            get => perfilMascotas;
            set
            {
                perfilMascotas = value;
                OnPropertyChanged(nameof(PerfilMascotas));
            }
        }

        public ICommand VerDetallesCommand { get; }

        public MascotasViewModel()
        {
            PerfilMascotas = new ObservableCollection<PerfilMascota>();
            VerDetallesCommand = new Command<PerfilMascota>(OnVerDetalles);
        }

        public void AgregarPerfilMascota(PerfilMascota nuevoPerfil)
        {
            PerfilMascotas.Add(nuevoPerfil);
        }

        private async void OnVerDetalles(PerfilMascota mascota)
        {
            if (mascota != null)
            {
                await Application.Current.MainPage.Navigation.PushAsync(new DetallesMascotaPage(mascota));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
