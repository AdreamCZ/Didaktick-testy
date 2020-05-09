using Android.Content;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace DDKTCKE
{


    public class Statistika : INotifyPropertyChanged
    {
        // Singleton
        public static Statistika Current = new Statistika();
        //public List<Type> typyOtazek = new List<Type>() { typeof(Pages.ABCPage), typeof(Pages.ABCTextPage), typeof(Pages.OtevrenaTextPage) };

        public bool probihaTest = false;
        private int _Spravnych_odpovedi = 0;
        private int _Celkem_odpovedi = 0;
        private List<int> _ABCHistorie = new List<int>();
        private List<int> _ABCTextHistorie = new List<int>();
        private List<int> _OtevrenaTextHistorie = new List<int>();
        private List<int> _OtevrenaHistorie = new List<int>();
        private List<int> _SerazeniHistorie = new List<int>();
        private List<int> _AnoNeHistorie = new List<int>();
        private List<int> _AnoNeTextHistorie = new List<int>();
        private string _PosledniText;

        public Statistika()
        {
            nactiHistorii();
        }

        public void nactiHistorii()
        {

            var prefs = Android.App.Application.Context.GetSharedPreferences("DDKTCKE", FileCreationMode.Private);
            if (prefs.GetString("ABCHistorie", null) != null)
            {
                ABCHistorie = prefs.GetString("ABCHistorie", null).TrimEnd(',').Split(',').ToList().Select(int.Parse).ToList(); //převede z list<string> na list<int>
            }
            else
            {
                ABCHistorie = new List<int>();
            }

            if (prefs.GetString("ABCTextHistorie", null) != null)
            {
                string res = prefs.GetString("ABCTextHistorie", null).TrimEnd(',');
                List<string> ABCTxtStr = res.Split(',').ToList();
                ABCTextHistorie = ABCTxtStr.Select(int.Parse).ToList(); //převede z list<string> na list<int>
            }
            else
            {
                ABCTextHistorie = new List<int>();
            }

            if (prefs.GetString("OtevrenaTextHistorie", null) != null)
            {
                OtevrenaTextHistorie = prefs.GetString("OtevrenaTextHistorie", null).TrimEnd(',').Split(',').ToList().Select(int.Parse).ToList(); //převede z list<string> na list<int>
            }
            else
            {
                OtevrenaTextHistorie = new List<int>();
            }

            if (prefs.GetString("SerazeniHistorie", null) != null)
            {
                SerazeniHistorie = prefs.GetString("SerazeniHistorie", null).TrimEnd(',').Split(',').ToList().Select(int.Parse).ToList(); //převede z list<string> na list<int>
            }
            else
            {
                SerazeniHistorie = new List<int>();
            }

            if (prefs.GetString("AnoNeHistorie", null) != null)
            {
                AnoNeHistorie = prefs.GetString("AnoNeHistorie", null).TrimEnd(',').Split(',').ToList().Select(int.Parse).ToList(); //převede z list<string> na list<int>
            }
            else
            {
                AnoNeHistorie = new List<int>();
            }

            if (prefs.GetString("AnoNeTextHistorie", null) != null)
            {
                AnoNeTextHistorie = prefs.GetString("AnoNeTextHistorie", null).TrimEnd(',').Split(',').ToList().Select(int.Parse).ToList(); //převede z list<string> na list<int>
            }
            else
            {
                AnoNeTextHistorie = new List<int>();
            }
        }

        public async void DalsiPage(string text = "")

        {
            if (_Celkem_odpovedi != 0)
            {
                ZapisDat();
            }

            if (text == "")
            {
                Current.PosledniText = "";
            }
            else
            {
                Current.PosledniText = text;
            }
            var prefs = Android.App.Application.Context.GetSharedPreferences("DDKTCKE", FileCreationMode.Private);
            string TypyStr = prefs.GetString("Typy", null);
            String[] Typy;
            if (TypyStr == null)
            {
                Typy = new String[] { "True", "True", "True", "True", "True", "True" };
            }
            else
            {
                Typy = TypyStr.Split(',');
            }
            Page soucasnaPage = Application.Current.MainPage.Navigation.NavigationStack.LastOrDefault();
            try
            {
                //Pořadí typů otázek uložených v preferencích ABC,ABCtxt,OtevrenaTxt,AnoNeTxt,Serazeni,Ano/Ne
                Random rnd = new Random();
                int p = rnd.Next(0, 6);
                while (Typy[p] == "False")
                {
                    p = rnd.Next(0, 6);
                }
                switch (p)
                {
                    case 0:
                        await soucasnaPage.Navigation.PushAsync(new Pages.ABCPage());
                        break;
                    case 1:
                        await soucasnaPage.Navigation.PushAsync(new Pages.ABCTextPage());
                        break;
                    case 2:
                        await soucasnaPage.Navigation.PushAsync(new Pages.OtevrenaTextPage());
                        break;
                    case 3:
                        await soucasnaPage.Navigation.PushAsync(new Pages.AnoNeTextPage());
                        break;
                    case 4:
                        await soucasnaPage.Navigation.PushAsync(new Pages.SerazeniPage());
                        break;
                    case 5:
                        await soucasnaPage.Navigation.PushAsync(new Pages.AnoNePage());
                        break;
                }
            }
            catch (Exception ex)
            {
                Android.Widget.Toast.MakeText(Android.App.Application.Context, ex.InnerException.ToString(), Android.Widget.ToastLength.Short).Show();
            }
        }
        public void ZapisDat()
        {
            //Zápis úspěšnosti
            string procent;
            if (Current.Procent_uspesnost < 10) //Vytvoření řetězce procent pro zapisování tak že udržuje stále stejnou délku záznamu což umožňuje jejich přepisování
            {
                procent = "00" + Current.Procent_uspesnost.ToString();
            }
            else if (Current.Procent_uspesnost < 100)
            {
                procent = "0" + Current.Procent_uspesnost.ToString();
            }
            else
            {
                procent = Current.Procent_uspesnost.ToString();
            }

            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, "UspesnostLog.txt");
            string log;
            using (StreamReader sr = new StreamReader(filePath))
            {
                log = sr.ReadToEnd();
                sr.Close();
            }
            if (log != "")
            {
                string[] logLines = log.Split(';');
                string posledni = logLines[logLines.Length - 2];
                string posledniDate = posledni.Substring(0, posledni.IndexOf("@")).Trim();
                if (posledniDate == DateTime.Today.ToString("dd:MM:yyyy"))
                { //Pokud je poslední záznam z tohoto dne přepiš ho
                    using (StreamWriter sw = new StreamWriter(filePath, false))
                    {
                        for (int i = 0; i < logLines.Length - 2; i++)
                        {
                            sw.Write(logLines[i] + ";"); //Zapíše předchozí záznamy (až na ten z dnešního dne)

                        }
                        sw.Write(DateTime.Today.ToString("dd:MM:yyyy") + "@" + procent + ";"); //Přepíše poslední záznam tím aktuálním
                        sw.Flush();
                        sw.Close();
                    }
                }
                else //Pokud není poslední záznam z dneška udělej nový
                {
                    using (StreamWriter sw = new StreamWriter(filePath, true))
                    {
                        sw.Write(DateTime.Today.ToString("dd:MM:yyyy") + "@" + procent + ";"); //Připíše poslední záznam
                        sw.Flush();
                        sw.Close();
                    }
                }
            }
            else //Pokud nejsou žádné záznamy udělej nový
            {
                using (StreamWriter sw = new StreamWriter(filePath, true))
                {
                    sw.Write(DateTime.Today.ToString("dd:MM:yyyy") + "@" + procent + ";"); //Připíše poslední záznam
                    sw.Flush();
                    sw.Close();
                }
            }


        }
        /*
        public ConstructorInfo getConstructorDalsiOtazky()
        {
            Random rnd = new Random();
            Type pageType = Statistika.Current.typyOtazek[rnd.Next(0, Current.typyOtazek.Count)];
            var constructor = pageType.GetConstructor(Type.EmptyTypes);
            return constructor;
        }
        */

        public List<int> ABCHistorie
        {
            get { return _ABCHistorie; }
            set
            {
                _ABCHistorie = value;
            }

        }

        public List<int> ABCTextHistorie
        {
            get { return _ABCHistorie; }
            set
            {
                _ABCHistorie = value;
            }

        }

        public List<int> OtevrenaTextHistorie
        {
            get { return _OtevrenaTextHistorie; }
            set
            {
                _OtevrenaTextHistorie = value;
            }

        }

        public List<int> OtevrenaHistorie
        {
            get { return _OtevrenaHistorie; }
            set
            {
                _OtevrenaHistorie = value;
            }

        }

        public List<int> SerazeniHistorie
        {
            get { return _SerazeniHistorie; }
            set
            {
                _SerazeniHistorie = value;
            }

        }

        public List<int> AnoNeHistorie
        {
            get { return _AnoNeHistorie; }
            set
            {
                _AnoNeHistorie = value;
            }

        }

        public List<int> AnoNeTextHistorie
        {
            get { return _AnoNeTextHistorie; }
            set
            {
                _AnoNeTextHistorie = value;
            }

        }


        public int Spravnych_odpovedi
        {
            get { return _Spravnych_odpovedi; }
            set
            {
                _Spravnych_odpovedi = value;
                OnPropertyChanged("Spravnych_odpovedi");
            }
        }

        public int Celkem_odpovedi
        {
            get { return _Celkem_odpovedi; }
            set
            {
                _Celkem_odpovedi = value;
                OnPropertyChanged("Celkem_odpovedi");
            }
        }

        public int Procent_uspesnost
        {
            get
            {
                if (_Celkem_odpovedi == 0) { return 0; }
                else
                {
                    float Desetine = ((float)_Spravnych_odpovedi / (float)_Celkem_odpovedi) * 100;
                    return (int)Desetine;
                }
            }
        }

        public string PosledniText
        {
            get { return _PosledniText; }
            set
            {
                _PosledniText = value;
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
                return;

            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
