namespace FrontEndHealthPets.Services
{
    /// <summary>
    /// Navigation service interface
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Navigate to page
        /// </summary>
        Task NavigateToAsync(string route);

        /// <summary>
        /// Navigate to page with parameters
        /// </summary>
        Task NavigateToAsync(string route, IDictionary<string, object> parameters);

        /// <summary>
        /// Go back
        /// </summary>
        Task GoBackAsync();

        /// <summary>
        /// Navigate to root/main page
        /// </summary>
        Task NavigateToRootAsync();
    }
}
