using FrontEndHealthPets.Helpers;

namespace FrontEndHealthPets.Services
{
    /// <summary>
    /// Ad service implementation
    /// Uses test IDs by default - replace with real IDs for production
    /// </summary>
    public class AdService : IAdService
    {
        // Test Ad Unit IDs (Google's official test IDs)
        private const string TestBannerAdUnitId = "ca-app-pub-3940256099942544/6300978111";
        private const string TestInterstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";
        
        // TODO: Replace with your real AdMob IDs
        private const string ProductionBannerAdUnitId = "YOUR_BANNER_AD_UNIT_ID";
        private const string ProductionInterstitialAdUnitId = "YOUR_INTERSTITIAL_AD_UNIT_ID";

        private readonly Dictionary<string, int> _actionCounts = new();
        private const int InterstitialFrequency = 3; // Show interstitial every N actions
        
        private bool _isInitialized;
        private bool _isBannerVisible;

        public bool ShouldShowAds => false; // Ads disabled for everyone

        public void Initialize()
        {
            if (_isInitialized) return;

            try
            {
                // TODO: Initialize AdMob SDK
                // MobileAds.Initialize(Platform.CurrentActivity); // Android
                // Or use Plugin.MauiMTAdmob
                
                _isInitialized = true;
                System.Diagnostics.Debug.WriteLine("AdService initialized");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AdService initialization failed: {ex.Message}");
            }
        }

        public void ShowBanner(AdPosition position = AdPosition.Bottom)
        {
            if (!ShouldShowAds || !_isInitialized)
            {
                System.Diagnostics.Debug.WriteLine("Ads disabled or not initialized");
                return;
            }

            try
            {
                // TODO: Implement actual banner display with Plugin.MauiMTAdmob
                // CrossMTAdmob.Current.ShowBanner();
                
                _isBannerVisible = true;
                System.Diagnostics.Debug.WriteLine($"Banner shown at {position}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to show banner: {ex.Message}");
            }
        }

        public void HideBanner()
        {
            if (!_isBannerVisible) return;

            try
            {
                // TODO: Implement with Plugin.MauiMTAdmob
                // CrossMTAdmob.Current.HideBanner();
                
                _isBannerVisible = false;
                System.Diagnostics.Debug.WriteLine("Banner hidden");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to hide banner: {ex.Message}");
            }
        }

        public async Task<bool> ShowInterstitialAsync()
        {
            if (!ShouldShowAds || !_isInitialized)
            {
                return false;
            }

            try
            {
                // TODO: Implement with Plugin.MauiMTAdmob
                // await CrossMTAdmob.Current.ShowInterstitial();
                
                // Simulate interstitial display
                await Task.Delay(100);
                System.Diagnostics.Debug.WriteLine("Interstitial shown");
                
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to show interstitial: {ex.Message}");
                return false;
            }
        }

        public void PreloadInterstitial()
        {
            if (!ShouldShowAds || !_isInitialized) return;

            try
            {
                // TODO: Implement with Plugin.MauiMTAdmob
                // CrossMTAdmob.Current.LoadInterstitial(TestInterstitialAdUnitId);
                
                System.Diagnostics.Debug.WriteLine("Interstitial preloaded");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to preload interstitial: {ex.Message}");
            }
        }

        public void TrackAction(string actionName)
        {
            if (!ShouldShowAds) return;

            if (!_actionCounts.ContainsKey(actionName))
            {
                _actionCounts[actionName] = 0;
            }

            _actionCounts[actionName]++;

            // Show interstitial every N actions
            if (_actionCounts[actionName] >= InterstitialFrequency)
            {
                _actionCounts[actionName] = 0;
                _ = ShowInterstitialAsync(); // Fire and forget
            }
        }

        /// <summary>
        /// Get the appropriate ad unit ID based on build configuration
        /// </summary>
        private string GetBannerAdUnitId()
        {
#if DEBUG
            return TestBannerAdUnitId;
#else
            return ProductionBannerAdUnitId;
#endif
        }

        private string GetInterstitialAdUnitId()
        {
#if DEBUG
            return TestInterstitialAdUnitId;
#else
            return ProductionInterstitialAdUnitId;
#endif
        }
    }
}
