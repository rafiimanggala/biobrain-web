using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;

namespace CustomControls.LayoutControls
{
    public enum ShareType
    {
        Email,
        Facebook,
        Twitter,
        Whatsapp,
        Linkedin,
        WhatsUpError
    }

    public class ShareControlEventArgs
    {
        public ShareControlEventArgs(ShareType type)
        {
            Type = type;
        }

        public ShareType Type { get; }
    }

    public partial class ShareButtonsPoup
    {
        public ShareButtonsPoup()
        {
            InitializeComponent();
        }

        public static BindableProperty LinkProperty =
            BindableProperty.Create(nameof(Link), typeof(string), typeof(ShareButtonsPoup), string.Empty, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (ShareButtonsPoup)bindable;
                    ctrl.Link = (string)newValue;
                });

        public string Link
        {
            get { return (string)GetValue(LinkProperty); }
            set
            {
                SetValue(LinkProperty, value);
            }
        }

        public static BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(ShareButtonsPoup), string.Empty, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var ctrl = (ShareButtonsPoup)bindable;
                    ctrl.Text = (string)newValue;
                });

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        public event EventHandler<ShareControlEventArgs> Share;

        public void InitPopup()
        {
            this.IsVisible = true;
        }

        public void ClosePopup()
        {
            this.IsVisible = false;
        }

        private async void MessageButton_OnClicked(object sender, EventArgs e)
        {
            ClosePopup();
            OnShare(new ShareControlEventArgs(ShareType.Email));
            var str = $"sms://?body={Text}%20{Link}";
            await Launcher.OpenAsync(new Uri(str));
        }

        private async void MailButton_OnClicked(object sender, EventArgs e)
        {
            ClosePopup();
            OnShare(new ShareControlEventArgs(ShareType.Email));
            await Launcher.OpenAsync(new Uri($"mailto:?subject=BioBrain&body={Text}%20%0A{Link}"));
        }

        private async void FacebookButton_OnClicked(object sender, EventArgs e)
        {
            ClosePopup();
            OnShare(new ShareControlEventArgs(ShareType.Facebook));
            await Launcher.OpenAsync(new Uri($"http://facebook.com/sharer.php?u={Link}&quote={Text}"));
        }

        private async void TwitterButton_OnClicked(object sender, EventArgs e)
        {
            ClosePopup();
            await Launcher.OpenAsync(new Uri($"https://twitter.com/intent/tweet?url={Link}&text={Text}"));
            OnShare(new ShareControlEventArgs(ShareType.Twitter));
        }

        private async void LinkedinButton_OnClicked(object sender, EventArgs e)
        {
            ClosePopup();
            await Launcher.OpenAsync(new Uri($"https://www.linkedin.com/shareArticle?mini=true&url={Link}&title=&summary=&source="));
            OnShare(new ShareControlEventArgs(ShareType.Linkedin));
        }

        private async void WhatsappButton_OnClicked(object sender, EventArgs e)
        {
            ClosePopup();
            try
            {
                await Launcher.OpenAsync(new Uri($"whatsapp://send?text={Text}%20%0A{Link}"));
            }
            catch (Exception)
            {
                OnShare(new ShareControlEventArgs(ShareType.WhatsUpError));
                return;
            }

            OnShare(new ShareControlEventArgs(ShareType.Whatsapp));
        }

        protected virtual void OnShare(ShareControlEventArgs e)
        {
            Share?.Invoke(this, e);
        }
    }
}
