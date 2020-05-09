using Android.Content;
using Java.Sql;
using Java.Util.Prefs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace DDKTCKE
{
    class Test : INotifyPropertyChanged
    {
        public static Test current = new Test();
        private bool _probiha = false;
        private int _ziskanychBodu = 0;
        private int _celkemBodu = 0;
        private string _statusDokonceni = "Test dokončen";
        private List<Odpoved> _odpovedi;
        private int indexOdpovedi = 0;
        public DateTime zacatek;
        
        public int limitBodu = 50; //Mělo by být dělitelné 2
         
        public Test()
        {
            _ziskanychBodu = 0;
            _celkemBodu = 0;
            _odpovedi = new List<Odpoved>();
        }

        public void Zahajit()
        {
            _probiha = true;
            var prefs = Android.App.Application.Context.GetSharedPreferences("DDKTCKE", FileCreationMode.Private);
            if (prefs.GetInt("limitBodu", -1) != -1)
            {
                limitBodu = prefs.GetInt("limitBodu", -1);
            }
            else
            {
                limitBodu = 50;
            }
            System.Timers.Timer timer = new System.Timers.Timer();
            zacatek = DateTime.Now;
            Statistika.Current.DalsiPage();
        }

        public void Odpoved(int ziskanoBodu,int maxBodu,string ukol,string odpoved, string spravna)
        {
            indexOdpovedi++;
            _ziskanychBodu += ziskanoBodu;
            _celkemBodu += maxBodu;

            _odpovedi.Add(new Odpoved
            {
                Ukol = ukol,
                Zvolena = odpoved,
                Spravna = spravna,
                JeSpravne = ziskanoBodu > 0 ? (ziskanoBodu < maxBodu ? "half":"true") : "false",
                Index = indexOdpovedi
            }); 

            if(_celkemBodu < limitBodu)
            {
                Statistika.Current.DalsiPage();
            }
            else
            {
                if(((float)_ziskanychBodu / (float)limitBodu) * 100 > 44)
                {
                    statusDokonceni = "Test úspěšně dokončen";
                }
                else
                {
                    statusDokonceni = "Test dokončen";
                }
                Page soucasnaPage = Application.Current.MainPage.Navigation.NavigationStack.LastOrDefault();
                soucasnaPage.Navigation.PushAsync(new Pages.VyhodnoceniPage());
            }
        }



        public void Ukoncit()
        {
            _probiha = false;
            _ziskanychBodu = 0;
            _celkemBodu = 0;
            _odpovedi = new List<Odpoved>();
            indexOdpovedi = 0;

        }

        public string procentHotovo
        {
            get
            {
                return "Dokončeno " + (((float)_celkemBodu / (float)limitBodu) * 100).ToString() + "% testu";
            }
        }

        public string cas
        {
            get {
                return ((int)(DateTime.Now - zacatek).TotalMinutes).ToString() + "′";
                    }
        }

        public string statusDokonceni
        {
            get { return _statusDokonceni; }
            set
            {
                _statusDokonceni = value;
            }
        }
        public List<Odpoved> odpovedi
        {
            get
            {
                return _odpovedi;
            }
            set
            {
                _odpovedi = value;
            }
        }

        public string ziskanychBodu
        {
            get
            {
                return _ziskanychBodu.ToString();
            }
            set { _ziskanychBodu = Int32.Parse(value); }
        }

        public string procentSpravne 
        {
            get
            {
                return ((((float)_ziskanychBodu/(float)limitBodu)*100).ToString() + "%");
            }
            
        }
        
        public string prospel
        {
            get
            {
                if ((_ziskanychBodu / _celkemBodu) * 100 >= 44 && (((int)(DateTime.Now - zacatek).TotalMinutes) < 60)) //current.procentSpravne ?????
                {
                    return "✔";
                }
                else
                {
                    return "✘";
                }
            }
        }
        
        public bool probiha
        {
            get { return _probiha; }
            set { _probiha = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
                return;

            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }

    class Odpoved
    {
        public string Ukol;
        public string Zvolena;
        public string Spravna;
        public string JeSpravne;
        public int Index;

    }
}
