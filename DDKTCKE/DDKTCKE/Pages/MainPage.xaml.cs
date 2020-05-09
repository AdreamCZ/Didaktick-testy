using Syncfusion.SfChart.XForms;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using Xamarin.Forms;
using Xamarin.Essentials;
using Android.Widget;

namespace DDKTCKE.Pages
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]

    public partial class MainPage : ContentPage
    {
        public ObservableCollection<ChartDataPoint> UspesnostData { get; set; }
        public string Zaznamu { get; set; } = "";
        public MainPage()
        {
            InitializeComponent();


            UspesnostData = new ObservableCollection<ChartDataPoint>();

            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, "UspesnostLog.txt");
            string log;
            using (StreamReader sr = new StreamReader(filePath))
            {
                log = sr.ReadToEnd();
                sr.Close();
            }
            Zaznamu = log;
            if (log != "")
            {
                string[] logLines = log.Split(';');
                Array.Resize(ref logLines, logLines.Length - 1);
                foreach (string s in logLines)
                {
                    string ds = s.Substring(0, s.IndexOf("@")).Trim();
                    DateTime d = DateTime.ParseExact(ds, "dd:MM:yyyy", CultureInfo.InvariantCulture);
                    float p = float.Parse(s.Substring(s.IndexOf("@") + 1, 3));
                    UspesnostData.Add(new ChartDataPoint(d, p * 0.01));
                }
            }

            BindingContext = this;
        }

        private void StartProcvicovani_onClicked(object sender, EventArgs e)
        {
            try
            {
                Statistika.Current.DalsiPage();
            }
            catch(Exception ex)
            {
                Toast toast = Toast.MakeText(Android.App.Application.Context, ex.InnerException.ToString(), ToastLength.Long);
                toast.Show();
            }

        }

        private void StartTest_onClicked(object sender, EventArgs e)
        {
            Test.current.Zahajit();

        }

        private async void OtevriNastaveni(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Pages.Nastaveni());
        }
    }
}
