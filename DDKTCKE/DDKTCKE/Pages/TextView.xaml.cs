using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DDKTCKE.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TextView : ContentPage
    {
        public string Text { get; set; }
        public TextView(string t)
        {
            InitializeComponent();
            Text = t;
            BindingContext = this;
        }

        public void Zabalit(Object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}