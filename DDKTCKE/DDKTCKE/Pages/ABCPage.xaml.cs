using Android.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace DDKTCKE.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ABCPage : ContentPage
    {
        public string Ukol { get; set; }
        public string Body { get; set; }
        public int BodyInt;
        public string Uspesnost { get; set; }
        public char[] ABC = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };
        public List<Button> ButtonList;
        public List<EventHandler> kontrolaDelegates = new List<EventHandler>();


        public ABCPage()
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





            int index = 0;
            foreach (String moznost in otzk.Moznosti.Moznost)
            {

                kontrolaDelegates.Add(delegate { KontrolaOdpovedi(moznost.Trim()[0], otzk.Spravna[0]); });

                Button button = new Button
                {
                    Text = Regex.Replace(moznost, @"\s+", " "),
                    VerticalOptions = Xamarin.Forms.LayoutOptions.Center,
                    FontSize = 16,
                    StyleClass = new List<string>() { "Tlacitko_odpoved" }
                };
                myStackLayout.Children.Insert(index + 2, button);
                button.Clicked += kontrolaDelegates[index];
                index++;



            }
            BindingContext = this;
        }

        Otazka NactiOtazku()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DDKTCKE.ABC.xml"))
            {
                Random rnd = new Random();
                XDocument doc = XDocument.Load(stream);
                int indexOtazky = rnd.Next(0, doc.Descendants("Otazka").Count());
                if (Statistika.Current.ABCHistorie.Count() == doc.Descendants("Otazka").Count())
                {
                    Statistika.Current.ABCHistorie = new List<int>();
                }
                else
                {
                    while (Statistika.Current.ABCHistorie.Contains(indexOtazky))
                    {
                        indexOtazky = rnd.Next(0, doc.Descendants("Otazka").Count());
                    }
                }
                Statistika.Current.ABCHistorie.Add(indexOtazky);

                var prefs = Android.App.Application.Context.GetSharedPreferences("DDKTCKE", FileCreationMode.Private);
                var prefEdit = prefs.Edit();
                string ABCHstr = "";
                foreach (int i in Statistika.Current.ABCHistorie)
                {
                    ABCHstr += i.ToString() + ',';
                }
                ABCHstr.TrimEnd(',');
                prefEdit.PutString("ABCHistorie", ABCHstr);
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

        void KontrolaOdpovedi(char odpoved, char spravna)
        {
            Statistika.Current.Celkem_odpovedi++;
            if (Test.current.probiha == false)
            {
                Pokracovat_butt.BackgroundColor = Xamarin.Forms.Color.LawnGreen;
            }
            Button[] tlacitka = Array.ConvertAll(myStackLayout.Children.Where(x => x is Button).ToArray(),
                new Converter<View, Button>(ViewToButton));
            int index = 0;
            foreach (Button b in tlacitka)
            {
                b.Clicked -= kontrolaDelegates[index];
                index++;

            }
            int ziskanoBodu;
            if (odpoved == spravna) //Odpověd je správná
            {
                Statistika.Current.Spravnych_odpovedi++;
                //Button button = (Button)myStackLayout.Children.Where(x => x is Button).ToArray()[ABC.IndexOf(spravna)];

                ziskanoBodu = BodyInt;
                if (Test.current.probiha == true)
                {
                    Test.current.Odpoved(ziskanoBodu, BodyInt, Ukol, tlacitka[ABC.IndexOf(odpoved)].Text, tlacitka[ABC.IndexOf(spravna)].Text);
                }
                else
                {
                    tlacitka[ABC.IndexOf(spravna)].BackgroundColor = Xamarin.Forms.Color.LawnGreen; //Udělá tlačítko se správnou odpovědí zelené
                    Popups.Spravne();
                }

            }
            else
            {
                ziskanoBodu = 0;
                if (Test.current.probiha == true)
                {
                    Test.current.Odpoved(ziskanoBodu, BodyInt, Ukol, tlacitka[ABC.IndexOf(odpoved)].Text, tlacitka[ABC.IndexOf(spravna)].Text);
                }
                else { 
                    Popups.Spatne();
                    Button button = (Button)myStackLayout.Children.Where(x => x is Button).ToArray()[ABC.IndexOf(odpoved)];
                    button.BackgroundColor = Xamarin.Forms.Color.IndianRed;//Udělá tlačítko se špatnou odpovědí červené
                    myStackLayout.Children.Where(x => x is Button).ToArray()[ABC.IndexOf(spravna)].BackgroundColor = Color.LawnGreen; //Udělá tlačítko se správnou odpovědí zelené
                }
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


        public static Button ViewToButton(View v)
        {
            return (Button)v;
        }
    }
}