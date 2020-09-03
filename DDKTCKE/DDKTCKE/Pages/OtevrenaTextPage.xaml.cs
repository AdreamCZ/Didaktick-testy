//using Rg.Plugins.Popup;
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
    public partial class OtevrenaTextPage : ContentPage
    {
        public string Ukol { get; set; }
        public string Body { get; set; }
        public int BodyInt;
        public string Text { get; set; }
        public string Uspesnost { get; set; }
        public string Spravna { get; set; }
        public string NeupravenyText;

        public OtevrenaTextPage()
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
            BodyInt = otzk.Bodu;
            Spravna = otzk.Spravna.Trim().ToLower();
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

            Ukol = otzk.Ukol.Replace("\n", " ").Trim();
            while (Ukol.Contains("  "))
            {
                Ukol = Ukol.Replace("  ", " ");
            }
            NeupravenyText = otzk.Text;
            Text = FormatovacTextu.Uprav(otzk.Text);

            BindingContext = this;
        }

        void KontrolaOdpovedi(object sender, EventArgs args)
        {
            Kontrola.IsEnabled = false;
            Statistika.Current.Celkem_odpovedi++;
            if (!Test.current.probiha)
            {
                Pokracovat_butt.BackgroundColor = Xamarin.Forms.Color.LawnGreen;
            }

            if (Vstup.Text != null)
            {
                bool jeSpravna = false;
                List<bool> Vyhodnoceni = new List<bool>();
                string odpoved = "";

                odpoved = Vstup.Text.ToLower().Trim();

                String[] Spravne = Spravna.Split(',');
                for (int i = 0; i < Spravne.Length; i++)
                {

                    Spravne[i] = Spravne[i].Trim();
                }

                //Zpracování odpovědi
                String[] Odpovedi;
                if (Vstup.Text.Contains(','))
                {
                    Odpovedi = Vstup.Text.Split(',');
                }
                else
                {
                    Odpovedi = Vstup.Text.Split(' ');
                }
                for (int i = 0; i < Odpovedi.Length; i++)
                {
                    Odpovedi[i] = Odpovedi[i].ToLower().Trim();
                }
                //Kontrola
                if (Odpovedi.Length == Spravne.Length)
                {
                    for (int y = 0; y < Odpovedi.Length; y++)
                    {
                        jeSpravna = false;
                        for (int s = 0; s < Spravne.Length; s++)
                        {

                            if (Spravne[s].Contains("/"))
                            {
                                
                                //if (Odpovedi[y].Trim() == Spravne[s].Split('/')[0].Trim() || Odpovedi[y].Trim() == Spravne[s].Split('/')[1].Trim()) //PRO 2 MOŽNOSTI FUNGOVALO DOBŘE
                                if(Spravne[s].Split('/').Select(sp => sp.Trim()).Contains(Odpovedi[y].Trim()))
                                {
                                    jeSpravna = true;
                                }

                            }
                            else if (Odpovedi[y].Trim() == Spravne[s])
                            {
                                jeSpravna = true;
                            }

                        }
                        if (jeSpravna)
                        {
                            Vyhodnoceni.Add(true);
                        }
                        else
                        {
                            Vyhodnoceni.Add(false);
                        }
                    }
                }

                if (!Vyhodnoceni.Contains(false))
                {
                    Statistika.Current.Spravnych_odpovedi++;
                    if (Test.current.probiha)
                    {
                        Test.current.Odpoved(BodyInt, BodyInt, Ukol, odpoved, Spravna);
                    }
                    else
                    {
                        Popups.Spravne();
                        Vstup.TextColor = Color.Green;
                    }
                }
                else
                {
                    if (Test.current.probiha)
                    {
                        Test.current.Odpoved(0, BodyInt, Ukol, odpoved, Spravna);
                    }
                    else
                    {
                        SpravnaOdpoved.Text = "Správně = " + Spravna;
                        Popups.Spatne();
                        Vstup.TextColor = Color.Red;
                    }
                }


            }
            else
            {
                if (Test.current.probiha)
                {
                    Test.current.Odpoved(0, BodyInt, Ukol, " ", Spravna);
                }
            }
        }

        Otazka NactiOtazku()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DDKTCKE.OtevrenaText.xml"))
            {
                XDocument doc = XDocument.Load(stream);
                Random rnd = new Random();
                int indexOtazky = -1;
                if (Statistika.Current.OtevrenaTextHistorie.Count() == doc.Descendants("Otazka").Count())
                {
                    Statistika.Current.OtevrenaTextHistorie = new List<int>();
                }

                if (Statistika.Current.PosledniText != "")
                {
                    List<XElement> OtazkyStejnyText;
                    OtazkyStejnyText = doc.Descendants("Otazka").Where(e => e.Element("Text").Value == Statistika.Current.PosledniText).ToList();
                    if (OtazkyStejnyText.Count > 0)
                    {
                        indexOtazky = doc.Descendants("Otazka").ToList().IndexOf(OtazkyStejnyText.FirstOrDefault());
                        int pricteno = 0;
                        while (Statistika.Current.OtevrenaTextHistorie.Contains(indexOtazky))
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

                    while (Statistika.Current.OtevrenaTextHistorie.Contains(indexOtazky))
                    {
                        indexOtazky = rnd.Next(0, doc.Descendants("Otazka").Count());
                    }
                }

                Statistika.Current.OtevrenaTextHistorie.Add(indexOtazky);

                var prefs = Android.App.Application.Context.GetSharedPreferences("DDKTCKE", FileCreationMode.Private);
                var prefEdit = prefs.Edit();
                string HistorieStr = "";
                foreach (int i in Statistika.Current.OtevrenaTextHistorie)
                {
                    HistorieStr += i.ToString() + ',';
                }
                HistorieStr.TrimEnd(',');
                prefEdit.PutString("OtevrenaTextHistorie", HistorieStr);
                prefEdit.Commit();

                var nahodna = doc.Descendants("Otazka").ToList()[indexOtazky];

                Otazka result = new Otazka
                {
                    Text = nahodna.Element("Text").Value,
                    Typ = nahodna.Element("Typ").Value,
                    Bodu = Convert.ToInt32(nahodna.Element("Bodu").Value),
                    Ukol = nahodna.Element("Ukol").Value,
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
            Statistika.Current.DalsiPage(NeupravenyText);
        }
    }
}