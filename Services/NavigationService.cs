using FrontEndHealthPets.Helpers;
using FrontEndHealthPets.Paginas;
using FrontEndHealthPets.Paginas.FlyPaginas;
using FrontEndHealthPets.ViewModels;

namespace FrontEndHealthPets.Services
{
    /// <summary>
    /// Navigation service implementation using Shell
    /// </summary>
    public class NavigationService : INavigationService
    {
        public async Task NavigateToAsync(string route)
        {
            if (Application.Current?.MainPage is NavigationPage navPage)
            {
                // Simple mapping for routes without Shell
                Page? page = route switch
                {

                    "Veterinarias" => ServiceHelper.GetService<VeterinaryDirectoryPage>(),
                    "VeterinariansList" => ServiceHelper.GetService<VeterinariansPage>(),
                    "MyAppointments" => ServiceHelper.GetService<MyAppointmentsPage>(),
                    "AddPet" => new IngresarMascotas(),
                    "PetsList" => new MisMascotas(),
                    "ForgotPassword" => ServiceHelper.GetService<ForgotPasswordPage>(),
                    "ResetPassword" => ServiceHelper.GetService<ResetPasswordPage>(),
                    _ => null
                };

                if (page != null)
                {
                    await navPage.PushAsync(page);
                }
            }
            else if (Application.Current?.MainPage is FlyoutPage flyout)
            {
                if (flyout.Detail is NavigationPage detailNav)
                {
                Page? page = route switch
                {

                    "Veterinarias" => ServiceHelper.GetService<VeterinaryDirectoryPage>(),
                    "VeterinariansList" => ServiceHelper.GetService<VeterinariansPage>(),
                    "MyAppointments" => ServiceHelper.GetService<MyAppointmentsPage>(),
                    "AddPet" => new IngresarMascotas(),
                    "PetsList" => new MisMascotas(),
                    _ => null
                };

                if (page != null)
                {
                    await detailNav.PushAsync(page);
                    
                    // Try to hide flyout safely to avoid crashes on desktop/split modes
                    try 
                    { 
                        if (flyout.FlyoutLayoutBehavior == FlyoutLayoutBehavior.Popover || DeviceInfo.Platform == DevicePlatform.Android || DeviceInfo.Platform == DevicePlatform.iOS)
                        {
                            flyout.IsPresented = false; 
                        }
                    } 
                    catch { }
                }
                }
            }
        }

        public async Task NavigateToAsync(string route, IDictionary<string, object> parameters)
        {
            // Handle ResetPassword with Email parameter specially
            if (route == "ResetPassword" && parameters.TryGetValue("Email", out var emailObj) && emailObj is string email)
            {
                var viewModel = ServiceHelper.GetService<ResetPasswordViewModel>();
                if (viewModel != null)
                {
                    viewModel.Email = email;
                    var page = new ResetPasswordPage(viewModel);
                    
                    if (Application.Current?.MainPage is NavigationPage navPage)
                    {
                        await navPage.PushAsync(page);
                    }
                    else if (Application.Current?.MainPage is FlyoutPage flyout && flyout.Detail is NavigationPage detailNav)
                    {
                        await detailNav.PushAsync(page);
                    }
                    return;
                }
            }
            
            // Fallback for other routes
            await NavigateToAsync(route);
        }

        public async Task GoBackAsync()
        {
            if (Application.Current?.MainPage is NavigationPage navPage)
            {
                await navPage.PopAsync();
            }
            else if (Application.Current?.MainPage is FlyoutPage flyout && flyout.Detail is NavigationPage detailNav)
            {
                await detailNav.PopAsync();
            }
        }

        public async Task NavigateToRootAsync()
        {
            if (Application.Current?.MainPage is NavigationPage navPage)
            {
                await navPage.PopToRootAsync();
            }
            else if (Application.Current?.MainPage is FlyoutPage flyout && flyout.Detail is NavigationPage detailNav)
            {
                await detailNav.PopToRootAsync();
            }
        }
    }
}
