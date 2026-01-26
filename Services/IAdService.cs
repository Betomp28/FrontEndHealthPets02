namespace FrontEndHealthPets.Services
{
    /// <summary>
    /// Interface for advertisement management
    /// </summary>
    public interface IAdService
    {
        /// <summary>
        /// Initialize the ad service
        /// </summary>
        void Initialize();

        /// <summary>
        /// Show a banner ad at specified position
        /// </summary>
        void ShowBanner(AdPosition position = AdPosition.Bottom);

        /// <summary>
        /// Hide the banner ad
        /// </summary>
        void HideBanner();

        /// <summary>
        /// Show an interstitial (full-screen) ad
        /// </summary>
        Task<bool> ShowInterstitialAsync();

        /// <summary>
        /// Preload an interstitial ad for later display
        /// </summary>
        void PreloadInterstitial();

        /// <summary>
        /// Check if ads should be shown (false if premium)
        /// </summary>
        bool ShouldShowAds { get; }

        /// <summary>
        /// Track an action for interstitial frequency
        /// </summary>
        void TrackAction(string actionName);
    }

    public enum AdPosition
    {
        Top,
        Bottom
    }
}
