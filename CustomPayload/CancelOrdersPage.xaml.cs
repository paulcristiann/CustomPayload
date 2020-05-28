using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CustomPayload
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CancelOrdersPage : ContentPage
    {
        public string Coin { get; set; }

        public string Type { get; set; }

        public string Webhook { get; set; }

        private static readonly HttpClient client = new HttpClient();

        public CancelOrdersPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            coin_label.Text = "Coin: " + Coin;
            type_label.Text = "Payload type: " + Type;
            Webhook = Preferences.Get("user_webhook", "https://denyss.requestcatcher.com/test");
        }

        protected async void send_to_webhook(string payload)
        {
            var license_status = Preferences.Get("license_status", "not activated");

            if(license_status != "not activated")
            {
                try
                {
                    var httpContent = new StringContent(payload, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(Webhook, httpContent);
                    await DisplayAlert("Server status", response.StatusCode.ToString(), "Ok");
                }
                catch (Exception e)
                {
                    //await DisplayAlert("Error", e.ToString(), "Ok");
                    await DisplayAlert("Error", "An error occured. Message not sent", "Ok");
                }
            }
            else
            {
                await DisplayAlert("Error", "Please activate the software first", "Ok");
            }
        }

        private async void Cancel_All_Pressed(object sender, EventArgs e)
        {
            var payload = "symbol=" + Coin + ",type=cancelorders;";
            bool answer = await DisplayAlert("Notification", payload, "Confirm", "Cancel");
            if (answer == true)
            {
                send_to_webhook(payload);
                _ = Navigation.PopToRootAsync();
            }
            
        }

        private async void Cancel_Limit_Pressed(object sender, EventArgs e)
        {
            var payload = "";
            if (Type == "LONG")
            {
                payload = "symbol=" + Coin + ",type=cancelorders,canceltype=limit,cancelside=buy;";
            }
            else
            {
                payload = "symbol=" + Coin + ",type=cancelorders,canceltype=limit,cancelside=sell;";
            }

            bool answer = await DisplayAlert("Notification", payload, "Confirm", "Cancel");
            if (answer == true)
            {
                send_to_webhook(payload);
                _ = Navigation.PopToRootAsync();
            }
            
        }

        private async void Cancel_Limit_Closed_Pressed(object sender, EventArgs e)
        {
            var payload = "";
            if (Type == "LONG")
            {
                payload = "symbol=" + Coin + ",type=cancelorders,canceltype=limitclose,cancelside=buy;";
            }
            else
            {
                payload = "symbol=" + Coin + ",type=cancelorders,canceltype=limitclose,cancelside=sell;";
            }

            bool answer = await DisplayAlert("Notification", payload, "Confirm", "Cancel");
            if (answer == true)
            {
                send_to_webhook(payload);
                _ = Navigation.PopToRootAsync();
            }
            
        }

        private async void Cancel_Stop_Pressed(object sender, EventArgs e)
        {
            var payload = "";
            if (Type == "LONG")
            {
                payload = "symbol=" + Coin + ",type=cancelorders,canceltype=stop,cancelside=sell;";
            }
            else
            {
                payload = "symbol=" + Coin + ",type=cancelorders,canceltype=stop,cancelside=buy;";
            }

            bool answer = await DisplayAlert("Notification", payload, "Confirm", "Cancel");
            if (answer == true)
            {
                send_to_webhook(payload);
                _ = Navigation.PopToRootAsync();
            }
            
        }
    }
}