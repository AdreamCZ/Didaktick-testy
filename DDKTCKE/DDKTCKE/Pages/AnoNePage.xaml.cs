using Android.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DDKTCKE.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AnoNePage : ContentPage
    {
        public string Ukol { get; set; }
        public string Body { get; set; }
        public string Uspesnost { get; set; }
        public string Spravne;
        public List<string> moznosti;
        public AnoNePage()
        {
            InitializeComponent();
            if (Test.current.probiha)
            {
                Uspesnost = Test.current.procentHotovo;
            }
            else
            {
                Uspesnost = "Správně : " + Statistika.Current.Procent_uspesnost.ToString() + "%";
                Pokracovat_butt.IsEnabled = true;
            }
            Otazka otzk = NactiOtazku();
            Body = otzk.Bodu.ToString();
            Spravne = otzk.Spravna;

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
            Ukol = otzk.Ukol.Replace("\n", " ").Replace("    ", " ").Trim();
            while (Ukol.Contains("  "))
            {
                Ukol = Ukol.Replace("  ", " ");
            }

            moznosti = new List<string>();
            foreach (string m in otzk.Moznosti.Moznost)
            {
                string moznost = m.Replace("\n", " ");
                while (moznost.Contains("  ") || moznost.Contains("   "))
                {
                    moznost = moznost.Replace("  ", " ").Replace("   ", " ");
                }
                moznosti.Add(moznost);
            }

            int index = 0;
            foreach (String moznost in moznosti)
            {
                StackLayout layout = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal
                };
                FontSizeConverter fontSizeConverter = new FontSizeConverter();
                Label label = new Label
                {
                    Text = moznost.Replace("\n", " ").Replace("  ", " ").Trim(),
                    TextColor = Color.Black,
                    FontSize = (Double)fontSizeConverter.ConvertFromInvariantString("Small"),
                    VerticalOptions = Xamarin.Forms.LayoutOptions.Center,
                    Margin = new Thickness(15, 0, 0, 0),
                    HorizontalOptions = LayoutOptions.StartAndExpand
                };
                layout.Children.Add(label);
                Switch switchA = new Switch();
                layout.Children.Add(switchA);

                HlavniStackLayout.Children.Insert(index + 3, layout);
                index++;



            }
            BindingContext = this;
        }

        void KontrolaOdpovedi(object sender, EventArgs e)
        {
            Statistika.Current.Celkem_odpovedi++;
            List<Switch> switche = new List<Switch>();
            StackLayout[] Layouts = Array.ConvertAll(HlavniStackLayout.Children.Where(x => x is StackLayout).ToArray(),
            new Converter<View, StackLayout>(ViewToStackLayout));
            foreach (StackLayout l in Layouts)
            {
                View sw = l.Children.Where(x => x is Switch).FirstOrDefault();
                if (sw != null)
                {
                    switche.Add((Switch)sw);
                }
            }
            int s = 0;
            int chyb = 0;
            string odpovedi = "";

            string uzivateloviOdpovedi = "";
            string spravneOdpovedi = "";
            foreach (Switch sw in switche)
            {
                if (sw.IsToggled)
                {
                    odpovedi += "A";
                }
                else
                {
                    odpovedi += "N";
                }
                if (odpovedi[s] != Spravne[s])
                {
                    if (Test.current.probiha == false)
                    {
                        if (Spravne[s] == 'A')
                        {
                            sw.IsToggled = true;
                        }
                        else
                        {
                            sw.IsToggled = false;
                        }
                    }
                    else
                    {
                        uzivateloviOdpovedi += moznosti[s] + " - " + odpovedi[s] + "\n";
                        spravneOdpovedi += moznosti[s] + " - " + Spravne[s] + "\n";
                    }
                    chyb++; //Zvětší počítadlo chyb

                }
                s++;
            }
            //Řetězce pro TEST
            if (Test.current.probiha)
            {
                if(chyb == 0) { 
                int cisloOdpovedi = 0;
                    foreach (string ot in moznosti)
                    {
                        spravneOdpovedi += ot + " - " + Spravne[cisloOdpovedi] + "\n";
                        cisloOdpovedi++;
                    }
                }
                Test.current.Odpoved((chyb < 2 ? (chyb == 0 ? 2 : 1) : 0), 2, Ukol, uzivateloviOdpovedi, spravneOdpovedi);
                if (chyb == 0)
                {
                    Statistika.Current.Spravnych_odpovedi++;
                }
            }
            else {
                if (chyb == 0) //Nemá žádne chyby
                {
                    Statistika.Current.Spravnych_odpovedi++;
                    Popups.Spravne();
                }
                else
                {

                    Popups.Spatne();
                }
                if (!Test.current.probiha)
                {
                    Pokracovat_butt.BackgroundColor = Xamarin.Forms.Color.LawnGreen;
                }
                Kontrola_butt.IsEnabled = false;
            }
        }

        Otazka NactiOtazku()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DDKTCKE.AnoNe.xml"))
            {
                Random rnd = new Random();
                XDocument doc = XDocument.Load(stream);
                int indexOtazky = rnd.Next(0, doc.Descendants("Otazka").Count());
                if (Statistika.Current.AnoNeHistorie.Count() == doc.Descendants("Otazka").Count())
                {
                    Statistika.Current.AnoNeHistorie = new List<int>();
                }
                else
                {
                    while (Statistika.Current.AnoNeHistorie.Contains(indexOtazky))
                    {
                        indexOtazky = rnd.Next(0, doc.Descendants("Otazka").Count());
                    }
                }
                Statistika.Current.AnoNeHistorie.Add(indexOtazky);

                var prefs = Android.App.Application.Context.GetSharedPreferences("DDKTCKE", FileCreationMode.Private);
                var prefEdit = prefs.Edit();
                string HistorieStr = "";
                foreach (int i in Statistika.Current.AnoNeHistorie)
                {
                    HistorieStr += i.ToString() + ',';
                }
                HistorieStr.TrimEnd(',');
                prefEdit.PutString("AnoNeHistorie", HistorieStr);
                prefEdit.Commit();

                var nahodna = doc.Descendants("Otazka").ToList()[indexOtazky];

                Otazka result = new Otazka
                {
                    Typ = nahodna.Element("Typ").Value,
                    Bodu = Convert.ToInt32(nahodna.Element("Bodu").Value),
                    Ukol = nahodna.Element("Ukol").Value,
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

        public static Switch ViewToSwitch(View v)
        {
            return (Switch)v;
        }
        public static StackLayout ViewToStackLayout(View v)
        {
            return (StackLayout)v;
        }
    }
}