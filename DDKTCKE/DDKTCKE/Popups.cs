
using global::Android.Views;
using global::Android.Widget;




namespace DDKTCKE
{
    class Popups
    {
        public static void Spravne()
        {
            Toast toast = Toast.MakeText(Android.App.Application.Context, "✔", ToastLength.Short);
            TextView text = toast.View.FindViewById<TextView>(Android.Resource.Id.Message);
            text.SetTextColor(Android.Graphics.Color.Green);
            text.SetTextSize(Android.Util.ComplexUnitType.Pt, 30);
            text.SetPadding(0, 0, 0, 5);
            toast.SetGravity(GravityFlags.Center, 0, 0);
            toast.Show();



        }

        public static void Spatne()
        {
            Toast toast = Toast.MakeText(Android.App.Application.Context, "✘", ToastLength.Short);
            TextView text = toast.View.FindViewById<TextView>(Android.Resource.Id.Message);
            text.SetTextColor(Android.Graphics.Color.Red);
            text.SetTextSize(Android.Util.ComplexUnitType.Pt, 30);
            text.SetPadding(0, 0, 0, 5);
            toast.SetGravity(GravityFlags.Center, 0, 0);
            toast.Show();
        }

    }
}
