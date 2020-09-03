using System;
//using AdMob;
using Android.Gms.Ads;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
//using AdMob.Droid.CustomRenders;
using Android.Content;
using DDKTCKE;
using DDKTCKE.Controls;

[assembly: ExportRenderer(typeof(AdBannerek), typeof(AdBanner_Droid))]
namespace DDKTCKE
{
    public class AdBanner_Droid : ViewRenderer<AdBannerek, Android.Gms.Ads.AdView>
    {
        Context context;
        public AdBanner_Droid(Context _context) : base(_context)
        {
            context = _context;
        }
        protected override void OnElementChanged(ElementChangedEventArgs<AdBannerek> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement == null)
            {
                var adView = new AdView(Context);
                switch ((Element as AdBannerek).Size)
                {
                    case AdBannerek.Sizes.Standardbanner:
                        adView.AdSize = AdSize.Banner;
                        break;
                    case AdBannerek.Sizes.LargeBanner:
                        adView.AdSize = AdSize.LargeBanner;
                        break;
                    case AdBannerek.Sizes.MediumRectangle:
                        adView.AdSize = AdSize.MediumRectangle;
                        break;
                    case AdBannerek.Sizes.FullBanner:
                        adView.AdSize = AdSize.FullBanner;
                        break;
                    case AdBannerek.Sizes.Leaderboard:
                        adView.AdSize = AdSize.Leaderboard;
                        break;
                    case AdBannerek.Sizes.SmartBannerPortrait:
                        adView.AdSize = AdSize.SmartBanner;
                        break;
                    default:
                        adView.AdSize = AdSize.Banner;
                        break;
                }
                // TODO: change this id to your admob id  
                adView.AdUnitId = "ca-app-pub-3940256099942544/6300978111";
                var requestbuilder = new AdRequest.Builder();
                adView.LoadAd(requestbuilder.Build());
                SetNativeControl(adView);
            }
        }
    }
}
