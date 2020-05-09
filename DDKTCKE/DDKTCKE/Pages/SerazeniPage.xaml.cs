using Android.Content;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Syncfusion.XForms.PopupLayout;

namespace DDKTCKE.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SerazeniPage : ContentPage
    {
        public string Ukol { get; set; }
        public string Body { get; set; }
        public string Zdroj { get; set; }
        public string Spravna;

        public string Uspesnost { get; set; }
        public List<string> moznostiSeraz = new List<string>();
        public char[] ABC = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };
        public List<Button> ButtonList;
        public List<EventHandler> kontrolaDelegates = new List<EventHandler>();
        SerazeniPageViewModel viewModel;
        private ObservableCollection<MoznostSerazeni> moznosti;
        public ObservableCollection<MoznostSerazeni> Moznosti;

        SfPopupLayout popupLayout;


        public SerazeniPage()
        {
            InitializeComponent();
            popupLayout = new SfPopupLayout();
            DataTemplate templateView = new DataTemplate(() =>
            {
                Label content = new Label
                {
                    Text = "Ťukněte a podžte prst na části textu, kterou chcete přesunout, po chvíli se uvolní a můžete ji umístit na správné místo. Po seřazení všech částí stlačte tlačítko Kontrola.",
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center
                };
                return content;
            });
            popupLayout.PopupView.ContentTemplate = templateView;
            popupLayout.PopupView.HeaderTitle = "Nápověda";

            myListView.DragDropController.UpdateSource = true;
            myListView.SelectionChanging += itemTapped;

            Otazka otzk = NactiOtazku();
            moznostiSeraz = otzk.Moznosti.Moznost;
            Spravna = otzk.Spravna;
            viewModel = new SerazeniPageViewModel(otzk);

            if (Test.current.probiha)
            {
                viewModel.Uspesnost = Test.current.procentHotovo;
            }
            else
            {
                viewModel.Uspesnost = "Správně : " + Statistika.Current.Procent_uspesnost.ToString() + "%";
                Pokracovat_butt.IsEnabled = true;
            }

            Moznosti = viewModel.Moznosti;


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


            BindingContext = viewModel;
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

        void KontrolaOdpovedi(object sender, EventArgs e)
        {

            Statistika.Current.Celkem_odpovedi++;
            if (!Test.current.probiha)
            {
                Pokracovat_butt.BackgroundColor = Xamarin.Forms.Color.LawnGreen;
            }
            Kontrola_butt.IsEnabled = false;

            string uzivatelPoradi = "";
            for (int i = 0; i < Moznosti.Count; i++)
            {
                uzivatelPoradi += Moznosti[i].Text[0];
                if (Test.current.probiha == false)
                {
                    if (uzivatelPoradi[i] == Spravna[i])
                    {
                        Moznosti[i].Barva = "#FF7CFC00";
                    }
                }
            }

            string uzivatelText = "";
            foreach (char l in uzivatelPoradi)
            {
                uzivatelText += moznostiSeraz["ABCDEFGHIJK".IndexOf(l)].Replace("\n", " ").Replace("  ", " ").Replace("  "," ").Trim();
            }

            if (uzivatelPoradi == Spravna) //Odpověď je správná
            {
                Statistika.Current.Spravnych_odpovedi++;

                if (Test.current.probiha)
                {
                    Test.current.Odpoved(3, 3, Ukol, uzivatelText, uzivatelText);
                }
                else
                {
                Popups.Spravne();
                }
                
            }
            else // Je špatná
            {
                if (Test.current.probiha)
                {
                    string spravneText = "";
                    foreach(char l in Spravna)
                    {
                        spravneText += moznostiSeraz["ABCDEFGHIJK".IndexOf(l)].Replace("\n", " ").Replace("  ", " ").Replace("  ", " ").Trim();
                    }

                    Test.current.Odpoved(0, 3, Ukol, uzivatelText, spravneText);
                }
                else
                {
                    List<char> SpravnaList = Spravna.ToCharArray().ToList();
                    var MoznostiSpravne = from m in Moznosti
                                          let index = SpravnaList.IndexOf(m.Text[0])
                                          orderby (index < 0 ? int.MaxValue : index) // in case it is not in the list
                                          select m;
                    Moznosti = new ObservableCollection<MoznostSerazeni>(MoznostiSpravne);

                    string poradi = "";
                    for (int i = 0; i < Moznosti.Count; i++)
                    {
                        Moznosti[i].Serazeno = true;
                        poradi += Moznosti[i].Text[0];
                    }
                    viewModel.Moznosti = Moznosti;
                    Popups.Spatne();
                }
            }
            return;
        }

        private void itemTapped(object sender, Syncfusion.ListView.XForms.ItemSelectionChangingEventArgs e)
        {
            e.Cancel = true;
        }

        private async void zobrazNavod(object sender, EventArgs e)
        {
            popupLayout.Show();
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

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisedOnPropertyChanged(string _PropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(_PropertyName));
            }
        }




    }
}