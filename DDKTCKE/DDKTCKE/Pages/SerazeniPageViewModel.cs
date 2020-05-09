using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace DDKTCKE.Pages
{
    class SerazeniPageViewModel : INotifyPropertyChanged
    {

        private ObservableCollection<MoznostSerazeni> moznosti;

        public string Uspesnost { get; set; }
        public string Ukol { get; set; }
        public string Body { get; set; }
        public string Zdroj { get; set; }


        public ObservableCollection<MoznostSerazeni> Moznosti
        {
            get
            {
                return moznosti;
            }
            set
            {
                moznosti = value;
                this.RaisedOnPropertyChanged("Moznosti");
            }
        }

        public SerazeniPageViewModel(Otazka otzk)
        {

            Moznosti = new ObservableCollection<MoznostSerazeni>();
            for (int i = 0; i < otzk.Moznosti.Moznost.Count; i++)
            {
                Moznosti.Add(new MoznostSerazeni());
                Moznosti.Last().Text = otzk.Moznosti.Moznost[i].Replace("\n", " ").Replace("  ", " ").Trim();

            }
            Body = otzk.Bodu.ToString() + " Body";
            Zdroj = otzk.Zdroj;
            //Ukol = otzk.Ukol.Replace("\n", "").Replace("  "," ").Trim();
            Ukol = "Seřaďtě části textu tak, aby navazovali.";

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
