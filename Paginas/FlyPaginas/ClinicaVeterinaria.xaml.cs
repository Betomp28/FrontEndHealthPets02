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
            if (selectedClinica == "Cl�nica Veterinaria San Francisco")
            {
                lblDireccion.Text = "Direcci�n: Calle San Francisco, 123";
                lblTelefono.Text = "Tel�fono: +506 85602260";
            }
            else if (selectedClinica == "Cl�nica Veterinaria Los �ngeles")
            {
                lblDireccion.Text = "Direcci�n: Avenida Los �ngeles, 456";
                lblTelefono.Text = "Tel�fono: +506 88910545";
            }
            else if (selectedClinica == "Cl�nica Veterinaria El Bosque")
            {
                lblDireccion.Text = "Direcci�n: Calle El Bosque, 789";
                lblTelefono.Text = "Tel�fono: +506 83360224";
            }
            else if (selectedClinica == "Cl�nica Veterinaria La Pradera")
            {
                lblDireccion.Text = "Direcci�n: Avenida La Pradera, 321";
                lblTelefono.Text = "Tel�fono: +506 83360789";
            }
            else if (selectedClinica == "Cl�nica Veterinaria El Roble")
            {
                lblDireccion.Text = "Direcci�n: Calle El Roble, 987";
                lblTelefono.Text = "Tel�fono: +506 88723938";
            }
            }
        
	private async void Agregar_Clicked(object sender, EventArgs e)
	{
        await DisplayAlert("Aviso", "Se ha agendado la cita", "OK");
	}  

}