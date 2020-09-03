using System;
using Xamarin.Forms;

namespace DDKTCKE.Controls
{
    public class AdBannerek : View
    {
        public enum Sizes { Standardbanner, LargeBanner, MediumRectangle, FullBanner, Leaderboard, SmartBannerPortrait }
        public Sizes Size { get; set; }
        public AdBannerek()
        {
            this.BackgroundColor = Color.Accent;
        }
    }


}

