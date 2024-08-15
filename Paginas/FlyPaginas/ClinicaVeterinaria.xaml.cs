namespace FrontEndHealthPets.Paginas.FlyPaginas;

public partial class ClinicaVeterinaria : ContentPage
{
	string laURL = "https://localhost:44348/api";
	public ClinicaVeterinaria()
	{
		InitializeComponent();
		

	}
	 private void PickerClinicas_SelectedIndexChanged(object sender, EventArgs e)
        {
           
            
            var selectedClinica = pickerClinicas.SelectedItem as string;
            if (selectedClinica == "Clínica Veterinaria San Francisco")
            {
                lblDireccion.Text = "Dirección: Calle San Francisco, 123";
                lblTelefono.Text = "Teléfono: +506 85602260";
            }
            else if (selectedClinica == "Clínica Veterinaria Los Ángeles")
            {
                lblDireccion.Text = "Dirección: Avenida Los Ángeles, 456";
                lblTelefono.Text = "Teléfono: +506 88910545";
            }
            else if (selectedClinica == "Clínica Veterinaria El Bosque")
            {
                lblDireccion.Text = "Dirección: Calle El Bosque, 789";
                lblTelefono.Text = "Teléfono: +506 83360224";
            }
            else if (selectedClinica == "Clínica Veterinaria La Pradera")
            {
                lblDireccion.Text = "Dirección: Avenida La Pradera, 321";
                lblTelefono.Text = "Teléfono: +506 83360789";
            }
            else if (selectedClinica == "Clínica Veterinaria El Roble")
            {
                lblDireccion.Text = "Dirección: Calle El Roble, 987";
                lblTelefono.Text = "Teléfono: +506 88723938";
            }
            }
        
	private async void Agregar_Clicked(object sender, EventArgs e)
	{
        await DisplayAlert("Aviso", "Se ha agendado la cita", "OK");
	}  

}