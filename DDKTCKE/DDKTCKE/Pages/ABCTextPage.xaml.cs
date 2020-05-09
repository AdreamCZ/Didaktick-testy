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
    public partial class ABCTextPage : ContentPage
    {
        public string Ukol { get; set; }

        public string Body { get; set; }
        public string Uspesnost { get; set; }
        public string HtmlString { get; set; }
        public string Text { get; set; }
        public string NeupravenyText;
        public char[] ABC = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };
        public List<Button> ButtonList;
        public List<EventHandler> kontrolaDelegates = new List<EventHandler>();
        public int BodyInt;


        public ABCTextPage()
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
            NeupravenyText = otzk.Text;
            Text = FormatovacTextu.Uprav(otzk.Text);


            int index = 1;
            foreach (String moznost in otzk.Moznosti.Moznost)
            {

                kontrolaDelegates.Add(delegate { KontrolaOdpovedi(moznost.Trim()[0], otzk.Spravna[0]); });

                Button button = new Button
                {
                    Text = Regex.Replace(moznost, @"\s+", " "),
                    VerticalOptions = Xamarin.Forms.LayoutOptions.Center,
                    StyleClass = new List<string>() { "Tlacitko_odpoved" }
                };
                myStackLayout.Children.Insert(index + 3, button);
                button.Clicked += kontrolaDelegates[index - 1];
                index++;



            }
            BindingContext = this;

        }





        Otazka NactiOtazku()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DDKTCKE.ABCText.xml"))
            {
                XDocument doc = XDocument.Load(stream);
                Random rnd = new Random();
                XElement nahodna;
                int indexOtazky = -1;
                if (Statistika.Current.ABCTextHistorie.Count() == doc.Descendants("Otazka").Count())
                {
                    Statistika.Current.ABCTextHistorie = new List<int>();
                }

                if (Statistika.Current.PosledniText != "")
                {
                    List<XElement> OtazkyStejnyText;
                    OtazkyStejnyText = doc.Descendants("Otazka").Where(e => e.Element("Text").Value == Statistika.Current.PosledniText).ToList();
                    if (OtazkyStejnyText.Count > 0)
                    {
                        indexOtazky = doc.Descendants("Otazka").ToList().IndexOf(OtazkyStejnyText.FirstOrDefault());
                        int pricteno = 0;
                        while (Statistika.Current.ABCTextHistorie.Contains(indexOtazky))
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

                    while (Statistika.Current.ABCTextHistorie.Contains(indexOtazky))
                    {
                        indexOtazky = rnd.Next(0, doc.Descendants("Otazka").Count());
                    }
                }


                Statistika.Current.ABCTextHistorie.Add(indexOtazky);

                var prefs = Android.App.Application.Context.GetSharedPreferences("DDKTCKE", FileCreationMode.Private);
                var prefEdit = prefs.Edit();
                string HistorieStr = "";
                foreach (int i in Statistika.Current.ABCTextHistorie)
                {
                    HistorieStr += i.ToString() + ',';
                }
                HistorieStr.TrimEnd(',');
                prefEdit.PutString("ABCTextHistorie", HistorieStr);
                prefEdit.Commit();

                nahodna = doc.Descendants("Otazka").ToList()[indexOtazky];



                Otazka result = new Otazka
                {
                    Text = nahodna.Element("Text").Value,
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
            if (!Test.current.probiha)
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
            int ziskanoBodu = 0;
            if (odpoved == spravna) //Odpověd je správná
            {
                Statistika.Current.Spravnych_odpovedi++;
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
                Button button = (Button)myStackLayout.Children.Where(x => x is Button).ToArray()[ABC.IndexOf(odpoved)];
                ziskanoBodu = 0;
                if (Test.current.probiha == true)
                {
                    Test.current.Odpoved(ziskanoBodu, BodyInt, Ukol, tlacitka[ABC.IndexOf(odpoved)].Text, tlacitka[ABC.IndexOf(spravna)].Text);
                }
                else
                {
                    button.BackgroundColor = Xamarin.Forms.Color.IndianRed;//Udělá tlačítko se špatnou odpovědí červené
                    myStackLayout.Children.Where(x => x is Button).ToArray()[ABC.IndexOf(spravna)].BackgroundColor = Color.LawnGreen; //Udělá tlačítko se správnou odpovědí zelené
                    Popups.Spatne();
                }
            }

        }

        private void Rozbalit(object sender, EventArgs e)
        {
            Navigation.PushAsync(new Pages.TextView(Text));
        }
        private async void Reload(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new OtevrenaTextPage());
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
            Statistika.Current.DalsiPage(NeupravenyText);

        }

        public static Button ViewToButton(View v)
        {
            return (Button)v;
        }
    }
}