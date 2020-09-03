using Android.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;
using Xamarin.Forms.Internals;

namespace DDKTCKE.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PrirazeniPage : ContentPage
    {

        public string Ukol { get; set; }
        public string Body { get; set; }
        public int BodyInt;
        public string Uspesnost { get; set; }
        public string Spravna;
        public List<Picker> pickers = new List<Picker>();
        public ObservableCollection<string> MoznostiView = new ObservableCollection<string>();
        public ObservableCollection<string> PodukolyView = new ObservableCollection<string>();
        public char[] ABC = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };
        public PrirazeniPage()
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
                otzk.Moznosti.Moznost[i] = otzk.Moznosti.Moznost[i].Replace("\n", "").Trim();
                MoznostiView.Add(otzk.Moznosti.Moznost[i]);
            }

            Grid ContentGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                }
                
            };
            

            for (int i = 0; i < otzk.PodUkoly.Podukol.Count; i++)
            {
                ContentGrid.RowDefinitions.Add(new RowDefinition {Height = GridLength.Auto });
                while (otzk.PodUkoly.Podukol[i].Contains("  ")) {
                    otzk.PodUkoly.Podukol[i] = otzk.PodUkoly.Podukol[i].Replace("\n"," ").Replace("  ", " ");
                    
                }
                PodukolyView.Add(otzk.PodUkoly.Podukol[i]);
                StackLayout stackLayout = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal
                };
                Label label = new Label
                {
                    Text = otzk.PodUkoly.Podukol[i].Replace("\n"," "),
                    TextColor = Color.Black
                };

                Picker picker = new Picker ();

                picker.ItemsSource = MoznostiView;

                pickers.Add(picker);
                

                ContentGrid.Children.Add(label,0,i);
                ContentGrid.Children.Add(picker,1,i);

            }
            ContentScroll.Content = ContentGrid;
            BindingContext = this;
            

        }
        public string Odpoved = "";
        public string SpravnaOdpoved = "";
        void KontrolaOdpovedi(object sender, EventArgs args)
        {

            string uzivatelovaOdpoved = "";
            int i = 0;
            int chyb = 0;
            foreach(Picker p in pickers)
            {
                if (p.SelectedItem == null)
                {
                    Android.Widget.Toast.MakeText(Android.App.Application.Context, "Chyba : Všechna pole musí být vyplněna!", Android.Widget.ToastLength.Long).Show();
                    return;
                }
                else
                {
                    char pismeno = p.SelectedItem.ToString()[0];
                    uzivatelovaOdpoved += pismeno;
                    if (pismeno == Spravna[i] && !Test.current.probiha)
                    {
                        p.BackgroundColor = Color.LimeGreen;
                       
                    }
                    else
                    {
                        if (!Test.current.probiha)
                        {
                            p.SelectedItem = MoznostiView[ABC.IndexOf(Spravna[i])];
                        }
                        chyb++;
                    }

                    if (Test.current.probiha)
                    {
                        Odpoved += PodukolyView[i] + " = " + MoznostiView[ABC.IndexOf(pismeno)] + "\n";
                        SpravnaOdpoved += PodukolyView[i] + " = " + MoznostiView[ABC.IndexOf(Spravna[i])] + "\n";
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

            if (uzivatelovaOdpoved == Spravna)
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
                    int dostalBodu = 0;
                    dostalBodu = BodyInt - chyb;
                    dostalBodu = dostalBodu < 0 ? 0 : dostalBodu;
                    Test.current.Odpoved(dostalBodu, BodyInt, Ukol, Odpoved, SpravnaOdpoved);
                }
                Popups.Spatne();
            }
            
        }

        Otazka NactiOtazku()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DDKTCKE.Prirazeni.xml"))
            {
                XDocument doc = XDocument.Load(stream);
                Random rnd = new Random();
                XElement nahodna;
                int indexOtazky = -1;
                if (Statistika.Current.PrirazeniHistorie.Count() == doc.Descendants("Otazka").Count())
                {
                    Statistika.Current.PrirazeniHistorie = new List<int>();
                }

                indexOtazky = rnd.Next(0, doc.Descendants("Otazka").Count());

                while (Statistika.Current.PrirazeniHistorie.Contains(indexOtazky))
                {
                    indexOtazky = rnd.Next(0, doc.Descendants("Otazka").Count());
                }
                


                Statistika.Current.PrirazeniHistorie.Add(indexOtazky);

                var prefs = Android.App.Application.Context.GetSharedPreferences("DDKTCKE", FileCreationMode.Private);
                var prefEdit = prefs.Edit();
                string HistorieStr = "";
                foreach (int i in Statistika.Current.PrirazeniHistorie)
                {
                    HistorieStr += i.ToString() + ',';
                }
                HistorieStr.TrimEnd(',');
                prefEdit.PutString("Prirazeni", HistorieStr);
                prefEdit.Commit();

                nahodna = doc.Descendants("Otazka").ToList()[indexOtazky];



                Otazka result = new Otazka
                {
                    Typ = nahodna.Element("Typ").Value,
                    Ukol = nahodna.Element("Ukol").Value,
                    Bodu = Convert.ToInt32(nahodna.Element("Bodu").Value),
                    PodUkoly = new Podukoly(nahodna.Element("Podukoly").Elements().Select(x => x.Value).ToList()),
                    Moznosti = new Moznosti(nahodna.Element("Moznosti").Elements().Select(x => x.Value).ToList()),
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