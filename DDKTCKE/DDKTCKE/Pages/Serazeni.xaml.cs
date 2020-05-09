using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace DDKTCKE.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Serazeni : ContentPage
    {
        public string Ukol { get; set; }
        public string Body { get; set; }
        public string Uspesnost { get; set; }
        public char[] ABC = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };
        public List<Button> ButtonList;
        public List<EventHandler> kontrolaDelegates = new List<EventHandler>();


        public ABCPage()
        {
            InitializeComponent();
            Uspesnost = Statistika.Current.Procent_uspesnost.ToString();
            Otazka otzk = NactiOtazku();
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
            Ukol = otzk.Ukol;




            int index = 1;
            foreach (String moznost in otzk.Moznosti.Moznost)
            {

                kontrolaDelegates.Add(delegate { KontrolaOdpovedi(moznost[0], otzk.Spravna[0]); });

                Button button = new Button
                {
                    Text = moznost.Trim(),
                    VerticalOptions = Xamarin.Forms.LayoutOptions.Center,
                    FontSize = 16,
                    StyleClass = new List<string>() { "Tlacitko_odpoved" }
                };
                myStackLayout.Children.Insert(index, button);
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
            //Pokracovat_butt.IsEnabled = true;
            Pokracovat_butt.IsEnabled = true;
            Pokracovat_butt.BackgroundColor = Xamarin.Forms.Color.LawnGreen;
            Button[] tlacitka = Array.ConvertAll(myStackLayout.Children.Where(x => x is Button).ToArray(),
                new Converter<View, Button>(ViewToButton));
            int index = 0;
            foreach (Button b in tlacitka)
            {
                b.Clicked -= kontrolaDelegates[index];
                index++;

            }
            if (odpoved == spravna) //Odpověd je správná
            {
                Statistika.Current.Spravnych_odpovedi++;
                //Button button = (Button)myStackLayout.Children.Where(x => x is Button).ToArray()[ABC.IndexOf(spravna)];
                tlacitka[ABC.IndexOf(spravna)].BackgroundColor = Xamarin.Forms.Color.LawnGreen; //Udělá tlačítko se správnou odpovědí zelené


            }
            else
            {
                Button button = (Button)myStackLayout.Children.Where(x => x is Button).ToArray()[ABC.IndexOf(odpoved)];
                button.BackgroundColor = Xamarin.Forms.Color.IndianRed;//Udělá tlačítko se špatnou odpovědí červené
                myStackLayout.Children.Where(x => x is Button).ToArray()[ABC.IndexOf(spravna)].BackgroundColor = Color.LawnGreen; //Udělá tlačítko se správnou odpovědí zelené


            }
        }

        private async void Zpet(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MainPage());
        }

        private async void Pokracovat(object sender, EventArgs e)
        {
            Statistika.Current.DalsiPage();
        }

        private async void Reload(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ABCPage());

        }

        public static Button ViewToButton(View v)
        {
            return (Button)v;
        }
    }
}