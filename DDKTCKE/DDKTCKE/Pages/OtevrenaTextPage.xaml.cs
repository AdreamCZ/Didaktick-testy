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

            Ukol = otzk.Ukol.Replace("\n", "").Trim();
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

            bool jeSpravna = false;
            string odpoved = "";


            if (Vstup.Text != null)
            {
                odpoved = Vstup.Text.ToLower().Trim();
                if (Spravna.Contains(",")) //Pokud je odpověd víceslovná
                {
                    String[] Spravne = Spravna.Split(',');
                    for (int i = 0; i < Spravne.Length; i++)
                    {
                        Spravne[i] = Spravne[i].Trim();
                    }
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
                    if (Odpovedi.Length == Spravne.Length)
                    {
                        int check = 0;
                        for (int y = 0; y < Odpovedi.Length; y++)
                        {
                            if (!Spravne.Contains(Odpovedi[y]))
                            {
                                break;
                            }
                            check = y;
                        }
                        if (check == Odpovedi.Length - 1)
                        {
                            jeSpravna = true;
                        }
                    }
                }
                else if (Spravna.Contains("/"))//Pokud je více přijatelnách odpovědí 
                {
                    String[] Spravne = Spravna.Split('/');
                    for (int i = 0; i < Spravne.Length; i++)
                    {
                        Spravne[i] = Spravne[i].Trim();
                    }


                    foreach (string s in Spravne)
                    {
                        if (s == odpoved)
                        {
                            jeSpravna = true;
                        }
                    }
                }
                else
                {
                    if (odpoved == Spravna.ToLower())
                    {
                        jeSpravna = true;
                    }
                    else
                    {
                        jeSpravna = false;
                    }
                }
            }
            else
            {
                jeSpravna = false;
            }

            if (jeSpravna)
            {
                Statistika.Current.Spravnych_odpovedi++;
                if (Test.current.probiha)
                {
                    Test.current.Odpoved(1, 1, Ukol, odpoved, Spravna);
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
                    Test.current.Odpoved(0, 1, Ukol, odpoved, Spravna);
                }
                else
                {
                    SpravnaOdpoved.Text = "Správně = " + Spravna;
                    Popups.Spatne();
                    Vstup.TextColor = Color.Red;
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