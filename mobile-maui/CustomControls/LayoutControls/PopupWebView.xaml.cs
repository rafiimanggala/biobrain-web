using System;
using Microsoft.Maui.Controls;

namespace CustomControls.LayoutControls
{
    public partial class PopupWebView
    {
        private object parameter;
        private bool isPopupPostback;

        public event EventHandler<PopupCloseEventArgs> Closed;
        public event EventHandler<PopupContentChangigEventArgs> ContentChaging;

        public PopupWebView()
        {
            InitializeComponent();
            IsVisible = false;
        }

        public void InitPopup(PopupStyles type, object param = null)
        {
            PopupType = type;
            IsVisible = true;
            parameter = param;
        }

        private void CloseButton_OnClicked(object sender, EventArgs e)
        {
            OnClosed();
        }

        private void PopupWebView_OnNavigating(object sender, WebNavigatingEventArgs e)
        {
            if (isPopupPostback)
            {
                isPopupPostback = false;
                e.Cancel = true;
                OnContentChanging(e.Url);
            }
            else
                isPopupPostback = true;
        }

        private void OnContentChanging(string param)
        {
            ContentChaging?.Invoke(this, new PopupContentChangigEventArgs(param));
        }

        private void OnClosed()
        {
            Closed?.Invoke(this, new PopupCloseEventArgs(parameter));
            IsVisible = false;
            Correctness.Source = null;
            PopupButton.IsVisible = false;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            if (MaxWidth > 0 && width > MaxWidth)
            {
                width = MaxWidth;
                WidthRequest = MaxWidth;
                HorizontalOptions = LayoutOptions.CenterAndExpand;
            }
            if (MaxHeight > 0 && height > MaxHeight)
            {
                height = MaxHeight;
                HeightRequest = MaxHeight;
                VerticalOptions = LayoutOptions.CenterAndExpand;
            }
            base.OnSizeAllocated(width, height);
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName != nameof(IsVisible)) return;
            if (IsVisible)
            {
                ContentWebView.Source = WebSource;
                return;
            }
            isPopupPostback = false;
            parameter = null;
        }

        #region Properties

        public static BindableProperty MaxWidthProperty =
           BindableProperty.Create(nameof(MaxWidth), typeof(int), typeof(PopupWebView), -1, BindingMode.TwoWay,
               propertyChanged: (bindable, oldValue, newValue) =>
               {
                   var ctrl = (PopupWebView)bindable;
                   ctrl.MaxWidth = (int)newValue;
               });

        public int MaxWidth
        {
            get => (int)GetValue(MaxWidthProperty);
            set => SetValue(MaxWidthProperty, value);
        }

        public static BindableProperty MaxHeightProperty =
           BindableProperty.Create(nameof(MaxHeight), typeof(int), typeof(PopupWebView), -1, BindingMode.TwoWay,
               propertyChanged: (bindable, oldValue, newValue) =>
               {
                   var ctrl = (PopupWebView)bindable;
                   ctrl.MaxHeight = (int)newValue;
               });

        public int MaxHeight
        {
            get => (int)GetValue(MaxHeightProperty);
            set => SetValue(MaxHeightProperty, value);
        }

        public static BindableProperty WebSourceProperty =
           BindableProperty.Create(nameof(WebSource), typeof(WebViewSource), typeof(PopupWebView), null, BindingMode.TwoWay,
               propertyChanged: (bindable, oldValue, newValue) =>
               {
                   var ctrl = (PopupWebView)bindable;
                   ctrl.WebSource = (WebViewSource)newValue;
               });

        public WebViewSource WebSource
        {
            get => (WebViewSource)GetValue(WebSourceProperty);
            set
            {
                SetValue(WebSourceProperty, value);
                ContentWebView.Source = value;
            }
        }

        public static BindableProperty IsCorrectProperty =
           BindableProperty.Create(nameof(IsCorrect), typeof(bool), typeof(PopupWebView), false, BindingMode.TwoWay,
               propertyChanged: (bindable, oldValue, newValue) =>
               {
                   var ctrl = (PopupWebView)bindable;
                   ctrl.IsCorrect = (bool)newValue;
               });

        public bool IsCorrect
        {
            get => (bool)GetValue(IsCorrectProperty);
            set
            {
                SetValue(IsCorrectProperty, value);
                Correctness.Source = new FileImageSource { File = value ? "greentick" : "redcross" };
            }
        }

        public static BindableProperty PopupTypeProperty =
           BindableProperty.Create(nameof(PopupType), typeof(PopupStyles), typeof(PopupWebView), PopupStyles.Hint, BindingMode.TwoWay,
               propertyChanged: (bindable, oldValue, newValue) =>
               {
                   var ctrl = (PopupWebView)bindable;
                   ctrl.PopupType = (PopupStyles)newValue;
               });

        public PopupStyles PopupType
        {
            get => (PopupStyles)GetValue(PopupTypeProperty);
            set
            {
                SetValue(PopupTypeProperty, value);
                ContentContainer.Padding = new Thickness(3, 15, 2, 0);
                SpeechBubbleImage.IsVisible = false;

                switch (value)
                {
                    case PopupStyles.Hint:
                        ContentWebView.Margin = DeviceInfo.Platform == DevicePlatform.Android ? new Thickness(15, 40, 15, 10) : new Thickness(15, 40, 15, 0);
                        PopupButton.IsVisible = false;
                        Correctness.Source = new FileImageSource { File = "hintcross" };
                        SpeechBubbleImage.IsVisible = true;
                        ContentContainer.Padding = DeviceInfo.Platform == DevicePlatform.Android ? new Thickness(3, 15, 2, 28) : new Thickness(3, 15, 2, 26);
                        break;
                    case PopupStyles.Glossary:
                        ContentWebView.Margin = DeviceInfo.Platform == DevicePlatform.Android ? new Thickness(15, 40, 15, 10) : new Thickness(15, 40, 15, 0);
                        PopupButton.IsVisible = false;
                        Correctness.Source = new FileImageSource { File = "hintcross" };
                        break;
                    case PopupStyles.Answer:
                        ContentWebView.Margin = new Thickness(15, 40, 15, 0);
                        PopupButton.Source = ImageSource.FromFile("popupicon.png");
                        PopupButton.IsVisible = true;
                        Correctness.Source = new FileImageSource { File = IsCorrect ? "greentick" : "redcross" };
                        break;
                    case PopupStyles.Message:
                        ContentWebView.Margin = DeviceInfo.Platform == DevicePlatform.Android ? new Thickness(15, 40, 15, 10) : new Thickness(15, 40, 15, 0);
                        PopupButton.IsVisible = false;
                        Correctness.Source = new FileImageSource { File = "hintcross" };
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(PopupType), PopupType, null);
                }
                Correctness.IsVisible = true;
            }
        }

        #endregion // Properties

        private void Correctness_OnOnTouchesEnded(object sender, EventArgs eventArgs)
        {
            OnClosed();
        }
    }
}
