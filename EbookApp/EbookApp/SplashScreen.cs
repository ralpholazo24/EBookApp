using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace EbookApp
{
    public class SplashScreen : ContentPage
    {
        Image SplashImage;

        public SplashScreen()
        {
            NavigationPage.SetHasNavigationBar(this, false);

            var sub = new AbsoluteLayout();
            SplashImage = new Image
            {
                Source = "app_logo.png",
                WidthRequest = 200,
                HeightRequest = 200
            };

            AbsoluteLayout.SetLayoutFlags(SplashImage, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(SplashImage, new Rectangle(0.5, 0.5, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

            sub.Children.Add(SplashImage);

            this.BackgroundColor = Color.FromHex("#fff");
            this.Content = sub;

        }

        protected override async void OnAppearing()
        {

            base.OnAppearing();

            await SplashImage.ScaleTo(1, 2000);
            await SplashImage.ScaleTo(0.9, 1500, Easing.Linear);
            await SplashImage.ScaleTo(150, 1200, Easing.Linear);
            Application.Current.MainPage = new Main();
            //await Navigation.PushAsync(new Main(),false);

        }

    }



}
