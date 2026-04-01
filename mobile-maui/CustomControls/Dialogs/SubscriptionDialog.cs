using System;
using System.Collections.Generic;
using Common;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace CustomControls.Dialogs
{
    public enum SubscriptionDialogResult
    {
        Cancel,
        Month,
        Year
    }

    public class SubscriptionDialog : BaseDialog<SubscriptionDialogResult>
    {

        private string MonthPrice { get; set; }
        private string YearPrice { get; set; }
        private double YearPriceNumber { get; set; }

        public SubscriptionDialog(string monthPrice, string yearPrice, double yearPriceNumber) : base()
        {
            MonthPrice = monthPrice;
            YearPrice = yearPrice;
            YearPriceNumber = yearPriceNumber;
            //DialogContent.Spacing = 5;
            InitDialog();
        }

        protected void InitDialog()
        {
            var elements = new List<View>()
            {
                new StackLayout
                {
                    Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.Center, Margin = new Thickness(0, 0, 0, 10),
                    Children =
                    {
                        new Button
                        {
                            HeightRequest = 40, WidthRequest = 40, Opacity = 0, HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.FillAndExpand
                        }, // placeholder
                        new Label
                        {
                            Text = StringResource.AppName, FontSize = 24, HeightRequest = 40,
                            VerticalTextAlignment = TextAlignment.Center, FontAttributes = FontAttributes.Bold,
                            HorizontalTextAlignment = TextAlignment.Center, TextColor = CustomColors.DarkMain,
                            HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.Center
                        }, // header
                        new Label()
                        {
                            Text = "x", BackgroundColor = Colors.Transparent, TextColor = CustomColors.DarkMain,
                            HeightRequest = 15, WidthRequest = 40, Padding = 0, FontSize = 15, VerticalTextAlignment = TextAlignment.Start,
                            HorizontalTextAlignment = TextAlignment.End, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.FillAndExpand,
                            GestureRecognizers = { new TapGestureRecognizer{ Command = new Command(() => Submit(SubscriptionDialogResult.Cancel)) } }
                        }, // close
                    }
                },
                new Image
                {
                    Source = ImageSource.FromFile("inappicon.png"), Margin = new Thickness(0, 0, 0, 10),
                    HeightRequest = 100, WidthRequest = 100, HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.Center
                }, //Logo
                new Label
                {
	                Text = StringResource.AllContentPackageString, FontSize = 18, HeightRequest = 20,
	                VerticalTextAlignment = TextAlignment.Center, FontAttributes = FontAttributes.Bold,
	                HorizontalTextAlignment = TextAlignment.Center, TextColor = Colors.Black,
	                HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.Center,
	                Margin = new Thickness(0, 0, 0, 10)
                }, // sub header
                new Button
                {
                    Text = $"{MonthPrice} / {StringResource.Month}", Margin = new Thickness(0, 0, 0, 10),
                    BackgroundColor = CustomColors.DarkMain, TextColor = Colors.White, CornerRadius = 2, FontSize = 18,
                    HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.Center,
                    Command = new Command(() => Submit(SubscriptionDialogResult.Month))
                }, // monthButton
                new Button
                {
                    Text = $"{YearPrice} / {StringResource.Year}",
                    BackgroundColor = CustomColors.DarkMain, TextColor = Colors.White, CornerRadius = 2, FontSize = 18,
                    HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.Center,
                    Command = new Command(() => Submit(SubscriptionDialogResult.Year))
                }, // yearButton
                new Label
                {
                    Text = string.Format(StringResource.SaveSubscriptionString, (YearPriceNumber/12).ToString(".##")), FontSize = 14, TextColor = Colors.Gray,
                    HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.Center
                }, // saveLabel
            };
            elements.ForEach(x => DialogContent.Children.Add(x));
        }
    }
}
