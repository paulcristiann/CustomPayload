using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CustomPayload
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WebhookPage : ContentPage
    {
        public WebhookPage()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            var user_webhook = Preferences.Get("user_webhook", "http://test-webhook.com");
            webhook_label.Text = user_webhook;
        }

        private async void save_webhook(object sender, EventArgs e)
        {
            if(webhook_field.Text == null)
            {
                await DisplayAlert("Error", "Please type the webhook", "Ok");
                return;
            }
            else
            {
                Preferences.Set("user_webhook", webhook_field.Text);
            }
            _ = Navigation.PopToRootAsync();
        }
    }
}