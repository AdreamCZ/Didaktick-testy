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
    public partial class AnoNeTextPage : ContentPage
    {
        public string Ukol { get; set; }
        public string Body { get; set; }
        public string Uspesnost { get; set; }
        public List<string> moznosti { get; set; } = new List<string>();
        public string Text { get; set; }
        public string Spravne;
        public string NeupravenyText;
        public AnoNeTextPage()
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
            NeupravenyText = otzk.Text;
            Text = FormatovacTextu.Uprav(otzk.Text);

            foreach (string m in otzk.Moznosti.Moznost)
            {
                string moznost = m.Replace("\n", " ");
                while (moznost.Contains("  ") || moznost.Contains("   "))
                {
                    moznost = moznost.Replace("  ", " ").Replace("   ", " ");
                }
                moznosti.Add(moznost);
            }
            //Vložení úkolů a switchů pro volbu
            int index = 0;
            foreach (String moznost in moznosti)
            {
                StackLayout layout = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand
                };
                FontSizeConverter fontSizeConverter = new FontSizeConverter();
                Label label = new Label
                {
                    Text = moznost.Replace("\n", " ").Replace("  ", " ").Trim(),
                    TextColor = Color.Black,
                    FontSize = (Double)fontSizeConverter.ConvertFromInvariantString("Small"),
                    //VerticalOptions = Xamarin.Forms.LayoutOptions.Center,
                    Margin = new Thickness(15, 1, 0, 0),
                    HorizontalOptions = LayoutOptions.StartAndExpand
                };
                layout.Children.Add(label);
                Switch switchA = new Switch();
                layout.Children.Add(switchA);

                HlavniStackLayout.Children.Insert(index + 5, layout);
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
            int chyb = 0 ;
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
                    chyb++;
                }
                s++;
            }
            //Řetězce pro TEST
            if (Test.current.probiha)
            {
                if (chyb == 0)
                {
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
            else
            {
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
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DDKTCKE.AnoNeTxt.xml"))
            {
                XDocument doc = XDocument.Load(stream);
                Random rnd = new Random();
                XElement nahodna;
                int indexOtazky = -1;
                if (Statistika.Current.AnoNeTextHistorie.Count() == doc.Descendants("Otazka").Count())
                {
                    Statistika.Current.AnoNeTextHistorie = new List<int>();
                }

                if (Statistika.Current.PosledniText != "")
                {
                    List<XElement> OtazkyStejnyText;
                    OtazkyStejnyText = doc.Descendants("Otazka").Where(e => e.Element("Text").Value == Statistika.Current.PosledniText).ToList();
                    if (OtazkyStejnyText.Count > 0)
                    {
                        indexOtazky = doc.Descendants("Otazka").ToList().IndexOf(OtazkyStejnyText.FirstOrDefault());
                        int pricteno = 0;
                        while (Statistika.Current.AnoNeTextHistorie.Contains(indexOtazky))
                        {
                            indexOtazky++;
                            pricteno++;
                        }
                        if (pricteno >= OtazkyStejnyText.Count - 1)
                        {
                            indexOtazky = -1;
                        }
                    }
                }
                if (Statistika.Current.PosledniText == "" || indexOtazky < 0)
                {
                    indexOtazky = rnd.Next(0, doc.Descendants("Otazka").Count());

                    while (Statistika.Current.AnoNeTextHistorie.Contains(indexOtazky))
                    {
                        indexOtazky = rnd.Next(0, doc.Descendants("Otazka").Count());
                    }
                }
                Statistika.Current.AnoNeTextHistorie.Add(indexOtazky);

                var prefs = Android.App.Application.Context.GetSharedPreferences("DDKTCKE", FileCreationMode.Private);
                var prefEdit = prefs.Edit();
                string HistorieStr = "";
                foreach (int i in Statistika.Current.AnoNeTextHistorie)
                {
                    HistorieStr += i.ToString() + ',';
                }
                HistorieStr.TrimEnd(',');
                prefEdit.PutString("AnoNeTextHistorie", HistorieStr);
                prefEdit.Commit();

                nahodna = doc.Descendants("Otazka").ToList()[indexOtazky];

                Otazka result = new Otazka
                {
                    Typ = nahodna.Element("Typ").Value,
                    Text = nahodna.Element("Text").Value,
                    Bodu = Convert.ToInt32(nahodna.Element("Bodu").Value),
                    Ukol = nahodna.Element("Ukol").Value,
                    Moznosti = new Moznosti(nahodna.Element("Moznosti").Elements().Select(x => x.Value).ToList()),
                    Spravna = nahodna.Element("Spravna").Value

                };
                return result;
            }
        }
        private void Rozbalit(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Pages.TextView(Text));
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