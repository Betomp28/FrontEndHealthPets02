using FrontEndHealthPets.ViewModels;

namespace FrontEndHealthPets.Paginas;

public partial class RecordatoriosPage : ContentPage
{
    public RecordatoriosPage(RecordatoriosViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is RecordatoriosViewModel vm)
        {
            await vm.LoadRecordatoriosCommand.ExecuteAsync(null);
        }
    }
}
