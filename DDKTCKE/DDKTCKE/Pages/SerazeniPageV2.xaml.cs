using Android.Content;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace DDKTCKE.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SerazeniPageV2 : ContentPage
    {

             public string Ukol { get; set; }
        public string Uspesnost { get; set; }
        public string Body { get; set; }
        public int BodyInt;
        public string Spravna;
        public List<String> moznosti = new List<string>();
        public List<Picker> pickers = new List<Picker>();
        public ObservableCollection<string> PoradiView = new ObservableCollection<string>();
        public char[] ABC = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };
        public string[] Poradi = { "1.","2.","3.","4.","5.","6.","7.","8.","9." };
        public SerazeniPageV2()
        {
            InitializeComponent();

            Otazka otzk = NactiOtazku();
            Spravna = otzk.Spravna;
            BodyInt = otzk.Bodu;
            Body = otzk.Bodu.ToString();
            if (otzk.Bodu == 1)
            {
                Body += " bod";
            }
            else if (otzk.Bodu < 5)
            {
                Body += " body";
            }
            else
            {
                Body += " bodů";
            }

            Ukol = otzk.Ukol.Replace("\n", "").Trim();
            while (Ukol.Contains("  "))
            {
                Ukol = Ukol.Replace("  ", " ");
            }

            if (Test.current.probiha)
            {
                Uspesnost = Test.current.procentHotovo;
            }
            else
            {
                Uspesnost = "Správně : " + Statistika.Current.Procent_uspesnost.ToString() + "%";
                Pokracovat_butt.IsEnabled = true;
            }

            //Picker

            for (int i = 0; i < otzk.Moznosti.Moznost.Count; i++)
            {
                otzk.Moznosti.Moznost[i] = otzk.Moznosti.Moznost[i].Replace("\n", " ").Trim();
                while(otzk.Moznosti.Moznost[i].Contains("  ")) {
                    otzk.Moznosti.Moznost[i].Replace("  ", " ");
                }
                moznosti.Add(otzk.Moznosti.Moznost[i]);
                PoradiView.Add(Poradi[i]);
            }

            Grid ContentGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(7, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                }

            };


            for (int i = 0; i < otzk.Moznosti.Moznost.Count; i++)
            {
                ContentGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });


                StackLayout stackLayout = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal
                };
                Label label = new Label
                {
                    Text = otzk.Moznosti.Moznost[i],
                    TextColor = Color.Black
                };

                Picker picker = new Picker();

                picker.ItemsSource = PoradiView;

                pickers.Add(picker);


                ContentGrid.Children.Add(label, 0, i);
                ContentGrid.Children.Add(picker, 1, i);

            }
            ContentScroll.Content = ContentGrid;
            BindingContext = this;


        }
        public string Odpoved = "";
        public string SpravnaOdpoved = "";
        void KontrolaOdpovedi(object sender, EventArgs args)
        {
            bool spatne = false;

            int i = 0;
            foreach (Picker p in pickers)
            {
                if (p.SelectedItem == null)
                {
                    Android.Widget.Toast.MakeText(Android.App.Application.Context, "Chyba : Všechna pole musí být vyplněna!", Android.Widget.ToastLength.Long).Show();
                    return;
                }
                else
                {
                    int cislo = Int32.Parse(p.SelectedItem.ToString()[0].ToString());
                    char pismeno = moznosti[cislo-1][0];
                    if (Test.current.probiha)
                    {
                        Odpoved += moznosti[cislo - 1];
                        SpravnaOdpoved += moznosti[ABC.IndexOf(Spravna[i])];
                        
                    }

                    if (cislo == ABC.IndexOf(Spravna[i])+1)
                    {
                        p.BackgroundColor = Color.LimeGreen;
                    }
                    else
                    {
                        p.SelectedItem = PoradiView[ABC.IndexOf(Spravna[i])];
                        spatne = true;
                    }
                    i++;
                }
            }


            Statistika.Current.Celkem_odpovedi++;
            Kontrola.IsEnabled = false;
            if (!Test.current.probiha)
            {
                Pokracovat_butt.BackgroundColor = Xamarin.Forms.Color.LawnGreen;
            }

            if (!spatne)
            {
                Statistika.Current.Spravnych_odpovedi++;
                if (Test.current.probiha)
                {
                    Test.current.Odpoved(BodyInt, BodyInt, Ukol, Odpoved, SpravnaOdpoved);
                }
                Popups.Spravne();
            }
            else
            {
                if (Test.current.probiha)
                {
                    Test.current.Odpoved(0, BodyInt, Ukol, Odpoved, SpravnaOdpoved);
                }
                Popups.Spatne();
            }

        }

        Otazka NactiOtazku()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DDKTCKE.Serazeni.xml"))
            {
                Random rnd = new Random();
                XDocument doc = XDocument.Load(stream);
                int indexOtazky = rnd.Next(0, doc.Descendants("Otazka").Count());
                if (Statistika.Current.SerazeniHistorie.Count() == doc.Descendants("Otazka").Count())
                {
                    Statistika.Current.SerazeniHistorie = new List<int>();
                }
                else
                {
                    while (Statistika.Current.SerazeniHistorie.Contains(indexOtazky))
                    {
                        indexOtazky = rnd.Next(0, doc.Descendants("Otazka").Count());
                    }
                }
                Statistika.Current.SerazeniHistorie.Add(indexOtazky);

                var prefs = Android.App.Application.Context.GetSharedPreferences("DDKTCKE", FileCreationMode.Private);
                var prefEdit = prefs.Edit();
                string HistorieStr = "";
                foreach (int i in Statistika.Current.SerazeniHistorie)
                {
                    HistorieStr += i.ToString() + ',';
                }
                HistorieStr.TrimEnd(',');
                prefEdit.PutString("SerazeniHistorie", HistorieStr);
                prefEdit.Commit();

                var nahodna = doc.Descendants("Otazka").ToList()[indexOtazky];

                Otazka result = new Otazka
                {
                    Typ = nahodna.Element("Typ").Value,
                    Bodu = Convert.ToInt32(nahodna.Element("Bodu").Value),
                    Ukol = nahodna.Element("Ukol").Value,
                    Moznosti = new Moznosti(nahodna.Element("Moznosti").Elements().Select(x => x.Value).ToList()),
                    Zdroj = nahodna.Element("Zdroj").Value,
                    Spravna = nahodna.Element("Spravna").Value

                };


                return result;

            }
        }

        private async void Zpet(object sender, EventArgs e)
        {
            if (Test.current.probiha)
            {
                Test.current.Ukoncit();
            }
            await Navigation.PushAsync(new MainPage());
        }

        private async void Pokracovat(object sender, EventArgs e)
        {
            Statistika.Current.DalsiPage();

        }
    }
}