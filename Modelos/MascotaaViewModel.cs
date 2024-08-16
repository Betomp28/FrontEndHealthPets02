using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using FrontEndHealthPets.Modelos;

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

        public MascotasViewModel()
        {
            PerfilMascotas = new ObservableCollection<PerfilMascota>();
        }

        public void AgregarPerfilMascota(PerfilMascota nuevoPerfil)
        {
            PerfilMascotas.Add(nuevoPerfil);
        }

        private int id_Mascota;
        private Int64 id_Usuario;
        private string nombre;
        private string especie;
        private string raza;
        private DateTime fecha_Nacimiento;

        public int Id_Mascota
        {
            get => id_Mascota;
            set
            {
                id_Mascota = value;
                OnPropertyChanged(nameof(Id_Mascota));
            }
        }

        public Int64 Id_Usuario
        {
            get => id_Usuario;
            set
            {
                id_Usuario = value;
                OnPropertyChanged(nameof(Id_Usuario));
            }
        }

        public string Nombre
        {
            get => nombre;
            set
            {
                nombre = value;
                OnPropertyChanged(nameof(Nombre));
            }
        }

        public string Especie
        {
            get => especie;
            set
            {
                especie = value;
                OnPropertyChanged(nameof(Especie));
            }
        }

        public string Raza
        {
            get => raza;
            set
            {
                raza = value;
                OnPropertyChanged(nameof(Raza));
            }
        }

        public DateTime Fecha_Nacimiento
        {
            get => fecha_Nacimiento;
            set
            {
                fecha_Nacimiento = value;
                OnPropertyChanged(nameof(Fecha_Nacimiento));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}