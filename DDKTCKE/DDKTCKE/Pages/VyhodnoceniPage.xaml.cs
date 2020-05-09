using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DDKTCKE.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VyhodnoceniPage : ContentPage
    {
        public VyhodnoceniPage()
        {
            InitializeComponent();


            foreach (Odpoved o in Test.current.odpovedi)
            {
                /*
                Button button = new Button
                {
                    // Text = o.JeSpravne ? o.Index.ToString() + ". " + "✔"  : o.Index.ToString() + ". "  + "✘" ,
                    Text = o.Index.ToString() + ".",
                    ImageSource = o.JeSpravne ? ImageSource.FromFile("Tick.png"): ImageSource.FromFile("Cross.png"),
                    TextColor = o.JeSpravne ? Color.LimeGreen : Color.OrangeRed,
                    HorizontalOptions = Xamarin.Forms.LayoutOptions.Start,
                    CornerRadius = 90,
                };
                button.Clicked += delegate{ RozbalOdpoved(o); };
                */
                StackLayout layout = new StackLayout { 
                    Orientation = StackOrientation.Horizontal,
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalOptions = LayoutOptions.Start

                };
                Label index = new Label
                {
                    Text = o.Index.ToString() + ". ",
                    TextColor = Color.Black
                };
                layout.Children.Add(index);


                Label symbol = new Label
                {

                    Text = o.JeSpravne == "false" ? "✘" : (o.JeSpravne == "true" ? "✔" : "½"),
                    TextColor = o.JeSpravne == "false" ? Color.OrangeRed : (o.JeSpravne == "true" ? Color.LimeGreen : Color.Orange )
                };
                layout.Children.Add(symbol);

                Frame frame = new Frame
                {
                    Content = layout,
                    HorizontalOptions = LayoutOptions.Start
                };
                var tapRecognizer = new TapGestureRecognizer();
                tapRecognizer.Tapped += (s, e) => {
                    RozbalOdpoved(o);
                };
                frame.GestureRecognizers.Add(tapRecognizer);
                

                //odpovediGrid.Children.Add(frame,(o.Index-1)%2,cislaRadku[o.Index-1]);
                odpovediGrid.Children.Add(frame);

                if(Int32.Parse(Test.current.cas.TrimEnd('′')) > 60)
                {
                    cas.TextColor = Color.OrangeRed;
                }
            }

            BindingContext = Test.current;
        }

        public int rozbalene = -1; //Index rozbalené odpovědi, -1 znamená že žádná není rozbalená
        public View smazaneTlac;

        private void RozbalOdpoved(Odpoved odpoved)
        {
            if(rozbalene == -1)
            {
                smazaneTlac = odpovediGrid.Children.ElementAt(odpoved.Index-1);
                odpovediGrid.Children.Remove(smazaneTlac);
                odpovediGrid.Children.Insert(odpoved.Index-1, vytvorFrameOdpovedi(odpoved));
                Grid.SetColumnSpan(odpovediGrid.Children.ElementAt(odpoved.Index - 1), 2);
                rozbalene = odpoved.Index-1;
            }else if(rozbalene == odpoved.Index-1) //Zabalení odpovědi
            {
                odpovediGrid.Children.RemoveAt(odpoved.Index-1);
                odpovediGrid.Children.Insert(odpoved.Index - 1, smazaneTlac);
                rozbalene = -1;
            }
            else
            {
                odpovediGrid.Children.RemoveAt(rozbalene);
                odpovediGrid.Children.Insert(rozbalene, smazaneTlac);
                Label ukol = new Label
                {
                    Text = odpoved.Ukol
                };
                smazaneTlac = odpovediGrid.Children.ElementAt(odpoved.Index-1);
                odpovediGrid.Children.Remove(smazaneTlac);
                odpovediGrid.Children.Insert(odpoved.Index-1, vytvorFrameOdpovedi(odpoved));
                Grid.SetColumnSpan(odpovediGrid.Children.ElementAt(odpoved.Index - 1), 2);
                rozbalene = odpoved.Index-1;
            }
            
            
        }



        private Frame vytvorFrameOdpovedi(Odpoved o)
        {
            Grid grid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition{Height = GridLength.Auto},
                    new RowDefinition{Height = GridLength.Auto},
                    new RowDefinition{Height = GridLength.Auto},
                    new RowDefinition{Height = GridLength.Auto}
                },
                
                ColumnDefinitions=
                {
                    new ColumnDefinition{Width = new GridLength(0.5,GridUnitType.Star)},
                    new ColumnDefinition { Width = new GridLength(0.5, GridUnitType.Star) }
                }
                
            };
            /*
            if (o.JeSpravne)  //Stačí 1 sloupec se správnou odpovědí
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }
            else//je špatně => musí být 2 sloupce pro vaši a správnou odpověď
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.5, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.5, GridUnitType.Star) });
            }
            */
            Label ukol = new Label
            {
                Text = o.Index.ToString() + ". " + o.Ukol,
                FontSize = 22,
                TextColor = Color.Black
            };
            grid.Children.Add(ukol, 0, 2, 0, 1);
            if (o.JeSpravne != "true")
            {
                Label vaseLabel = new Label
                {
                    Text = "Špatná odpověď : "
                };
                grid.Children.Add(vaseLabel, 0, 1, 1, 2);


                Label vase = new Label
                {
                    Text = o.Zvolena,
                    HorizontalOptions = Xamarin.Forms.LayoutOptions.Start,
                    TextColor = Color.Black
                };
                grid.Children.Add(vase, 0, 1, 2, 3);
            }

            Label spravnaLabel = new Label
            {
                Text = "Správná odpověď : "
            };
            if (o.JeSpravne == "true")
            {
                grid.Children.Add(spravnaLabel, 0, 2,1,2);
            }
            else {
                grid.Children.Add(spravnaLabel, 1, 2, 1, 2);
            }

            Label spravna = new Label
            {
                Text = o.Spravna,
                HorizontalOptions = Xamarin.Forms.LayoutOptions.Start,
                TextColor = Color.Black
            };
            if (o.JeSpravne == "true")
            {
                grid.Children.Add(spravna, 0, 2,2,3);
            }
            else{
                grid.Children.Add(spravna, 1, 2, 2, 3);
            }

            Label symbol = new Label
            {
                Text = o.JeSpravne == "false" ? "✘" : (o.JeSpravne == "true" ? "✔" : "½"),
                FontSize = 25,
                TextColor = o.JeSpravne == "false" ? Color.OrangeRed : (o.JeSpravne == "true" ? Color.LimeGreen : Color.Orange),
                HorizontalOptions = Xamarin.Forms.LayoutOptions.CenterAndExpand
            };
            grid.Children.Add(symbol,0,2,3,4);

            Frame frame = new Frame 
            { 
                Content = grid 
            };
            var tapRecognizer = new TapGestureRecognizer();
            tapRecognizer.Tapped += (s, e) => {
                RozbalOdpoved(o);
            };
            frame.GestureRecognizers.Add(tapRecognizer);

            return frame;
        }

        private async void Domu(object sender, EventArgs e)
        {
            Test.current.Ukoncit();
            await Navigation.PushAsync(new MainPage());
        }
    }
}