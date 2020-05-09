using Android.Content;
using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DDKTCKE.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Nastaveni : ContentPage
    {
        public List<Switch> Boxes = new List<Switch>();
        public string historieStr;
        public Nastaveni()
        {
            InitializeComponent();

            StackLayout[] Layouts = Array.ConvertAll(HlavniStackLayout.Children.Where(x => x is StackLayout).ToArray(),
                new Converter<View, StackLayout>(ViewToStackLayout));
            foreach (StackLayout l in Layouts)
            {
                Boxes.Add((Switch)l.Children.Where(x => x is Switch).FirstOrDefault()); //Získá všechny Switche
            }
            if (Boxes.Last() == null)
            {
                Boxes.RemoveAt(Boxes.Count - 1);
            }

            var prefs = Android.App.Application.Context.GetSharedPreferences("DDKTCKE", FileCreationMode.Private);

            if (prefs.GetInt("limitBodu", -1) != -1)
            {
                limitBoduEntry.Text = prefs.GetInt("limitBodu", 0).ToString();
            }
            else
            {
                limitBoduEntry.Text = "50";
            }

            string TypyStr = prefs.GetString("Typy", null);
            String[] Typy;
            if (TypyStr != null)
            {
                Typy = TypyStr.Split(',');
                if (Typy.Length != 7)
                {
                    Typy = new string[] { "True", "True", "True", "True", "True", "True" };
                }

                for (int i = 0; i < Boxes.Count; i++) //Podle toho co je uložené nastaví Switche
                {
                    if (Typy[i] == "True")
                    {
                        Boxes[i].IsToggled = true;

                    }
                    else
                    {
                        Boxes[i].IsToggled = false;
                    }
                }
            }
            else
            {
                foreach (Switch b in Boxes)
                {
                    b.IsToggled = true;
                }
            }

            BindingContext = this;

        }

        public async void Hotovo(Object sender, EventArgs e)
        {

            var prefs = Android.App.Application.Context.GetSharedPreferences("DDKTCKE", FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            string CheckData = "";
            foreach (Switch b in Boxes)
            {
                CheckData += b.IsToggled.ToString() + ",";
            }
            if (!CheckData.Contains("True"))
            {
                Android.Widget.Toast.MakeText(Android.App.Application.Context, "Chyba : Musí být zvolen alespoň jeden typ otázky!", Android.Widget.ToastLength.Long).Show();
            }
            else
            {

                prefEditor.PutString("Typy", CheckData); //Zapíše které typy otázek použít ve formátu Bool,Bool.. Nutné udržení stejného pořadí
                prefEditor.PutInt("limitBodu", Int32.Parse(limitBoduEntry.Text));
                prefEditor.Commit();
                await Navigation.PushAsync(new Pages.MainPage());
            }
        }

        public static Switch ViewToCheckBox(View v)
        {
            return (Switch)v;
        }
        public static StackLayout ViewToStackLayout(View v)
        {
            return (StackLayout)v;
        }
    }
}