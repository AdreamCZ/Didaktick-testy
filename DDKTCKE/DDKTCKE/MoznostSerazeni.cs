using System.ComponentModel;

namespace DDKTCKE
{
    public class MoznostSerazeni : INotifyPropertyChanged
    {
        private string text;
        private string barva = "#ffffff";
        private bool serazeno = false;
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
            }
        }
        public string Barva
        {
            get { return barva; }
            set
            {
                barva = value;
                this.OnPropertyChanged("Barva");
            }
        }
        public bool Serazeno
        {
            get { return serazeno; }
            set
            {
                serazeno = value;
                this.OnPropertyChanged("Serazeno");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }

}

