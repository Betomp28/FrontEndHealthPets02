using FrontEndHealthPets.Helpers;
using FrontEndHealthPets.Services;

namespace FrontEndHealthPets.Controls
{
    public partial class AdBannerView : ContentView
    {
        public static readonly BindableProperty ShouldShowAdsProperty =
            BindableProperty.Create(nameof(ShouldShowAds), typeof(bool), typeof(AdBannerView), true);

        public bool ShouldShowAds => false;

        public AdBannerView()
        {
            InitializeComponent();
            BindingContext = this;
        }



        protected override void OnParentSet()
        {
            base.OnParentSet();
            // Refresh visibility when parent changes
            OnPropertyChanged(nameof(ShouldShowAds));
        }
    }
}
