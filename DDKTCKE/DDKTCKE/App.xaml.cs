using System.IO;
using Xamarin.Forms;

namespace DDKTCKE
{
    public partial class App : Application
    {
        public static App Instance;
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjUzNDI3QDMxMzgyZTMxMmUzMFRFSlBKS1pFK3Q0U1FEbDQ0anNCc3h4d2t6R21OR25FQ1hmdGV0bnNma009");

            InitializeComponent();
           

            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, "UspesnostLog.txt");
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Dispose();
            }
            MainPage = new NavigationPage(new Pages.MainPage());
            BindingContext = this;
        }


        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
