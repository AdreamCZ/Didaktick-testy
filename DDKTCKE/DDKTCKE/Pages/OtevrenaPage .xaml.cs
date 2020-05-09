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
    public partial class OtevrenaPage : ContentPage
    {
        public string Ukol { get; set; }
        public string Body { get; set; }
        public string Uspesnost { get; set; }
        public string Spravna { get; set; }
        public string ZobrazitSpravnou { get; set; } = "";
        public OtevrenaPage()
        {
            InitializeComponent();
            Uspesnost = Statistika.Current.Procent_uspesnost.ToString();
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

            Ukol = otzk.Ukol;

            BindingContext = this;
        }

        void KontrolaOdpovedi(object sender, EventArgs args)
        {
            Statistika.Current.Celkem_odpovedi++;
            Pokracovat_butt.IsEnabled = true;
            Pokracovat_butt.BackgroundColor = Xamarin.Forms.Color.LawnGreen;

            bool jeSpravna = false;
            string odpoved = "";
            odpoved = Vstup.Text.ToLower();
            if (Vstup.Text != String.Empty)
            {

                if (Spravna.Contains(",")) //Pokud je odpověd víceslovná
                {
                    String[] Spravne = Spravna.Split(',');
                    for (int i = 0; i < Spravne.Length; i++)
                    {
                        Spravne[i] = Spravne[i].Trim();
                    }
                    String[] Odpovedi = Vstup.Text.Split(',');
                    for (int i = 0; i < Odpovedi.Length; i++)
                    {
                        Odpovedi[i] = Odpovedi[i].Trim();
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
                        if (s == Vstup.Text.Trim())
                        {
                            jeSpravna = true;
                        }
                    }
                }
                else
                {
                    if (odpoved.Trim() == Spravna)
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
                ZobrazitSpravnou = Spravna;
                SpravnaMark.Text = "✔";
                SpravnaMark.TextColor = Color.LawnGreen;
                Statistika.Current.Celkem_odpovedi++;
                Statistika.Current.Spravnych_odpovedi++;
            }
            else
            {
                SpravnaMark.Text = "✘";
                SpravnaMark.TextColor = Color.IndianRed;
                SpravnaOdpoved.Text = "Správně = " + Spravna;
                Statistika.Current.Celkem_odpovedi++;
            }

        }

        Otazka NactiOtazku()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DDKTCKE.Otevrena.xml"))
            {
                XDocument doc = XDocument.Load(stream);
                Random rnd = new Random();
                int indexOtazky = rnd.Next(0, doc.Descendants("Otazka").Count());
                if (Statistika.Current.OtevrenaHistorie.Count() == doc.Descendants("Otazka").Count())
                {
                    Statistika.Current.OtevrenaHistorie = new List<int>();
                }
                else
                {
                    while (Statistika.Current.OtevrenaHistorie.Contains(indexOtazky))
                    {
                        indexOtazky = rnd.Next(0, doc.Descendants("Otazka").Count());
                    }
                }
                Statistika.Current.OtevrenaHistorie.Add(indexOtazky);
                var nahodna = doc.Descendants("Otazka").ToList()[indexOtazky];

                Otazka result = new Otazka
                {
                    Typ = nahodna.Element("Typ").Value,
                    Bodu = Convert.ToInt32(nahodna.Element("Bodu").Value),
                    Ukol = nahodna.Element("Ukol").Value,
                    Spravna = nahodna.Element("Spravna").Value
                };
                return result;
            }
        }

        private async void Reload(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ABCTextPage());
        }
        private async void Zpet(object sender, EventArgs e)
        {

            await Navigation.PushAsync(new MainPage());
        }

        private void Pokracovat(object sender, EventArgs e)
        {
            Statistika.Current.DalsiPage();
        }
    }
}