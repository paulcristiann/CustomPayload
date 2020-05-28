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
    public partial class StopPage : ContentPage
    {
        public string Coin { get; set; }

        public string Type { get; set; }

        public string Webhook { get; set; }

        private static readonly HttpClient client = new HttpClient();

        public StopPage()
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

            if (license_status != "not activated")
            {
                try
                {
                    var httpContent = new StringContent(payload, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(Webhook, httpContent);
                    await DisplayAlert("Server status", response.StatusCode.ToString(), "Ok");
                }
                catch (Exception e)
                {
                    await DisplayAlert("Error", "An error occured. Message not sent", "Ok");
                }
            }
            else
            {
                await DisplayAlert("Error", "Please activate the software first", "Ok");
            }
        }

        private async void SendToWebhook(object sender, EventArgs e)
        {
            var payload = "";
            if (radioGroup.CheckedItem == null)
            {
                await DisplayAlert("Error", "Please select an action", "Ok");
                return;
            }

            var type = radioGroup.CheckedItem.Text;
            var quantity = quantity_entry.Text;
            if (quantity == null || float.Parse(quantity) < 0 || float.Parse(quantity) > 100)
            {
                await DisplayAlert("Notification", "Please set quantity between 0 and 100", "Ok");
                return;
            }
            var percentage = percentage_entry.Text;
            if (percentage == null || float.Parse(percentage) < 0 || float.Parse(percentage) > 100)
            {
                await DisplayAlert("Notification", "Please set percentage between 0 and 100", "Ok");
                return;
            }
            if (Type == "LONG")
            {
                if (type == "Stop")
                {
                    payload = "symbol=" + Coin + ",type=stop,side=sell,price=" + percentage + ",quantity=" + quantity;
                }
                else
                {
                    payload = "symbol=" + Coin + ",type=bestop,side=sell,price=" + percentage + ",quantity=" + quantity;
                }
            }
            else
            {
                if (type == "Stop")
                {
                    payload = "symbol=" + Coin + ",type=stop,side=buy,price=" + percentage + ",quantity=" + quantity;
                }
                else
                {
                    payload = "symbol=" + Coin + ",type=bestop,side=buy,price=" + percentage + ",quantity=" + quantity;
                }
            }
            if (trail_switch.IsToggled)
            {
                payload = payload + ",trail=1;";
            }
            else
            {
                payload = payload + ";";
            }
            if (float.Parse(quantity) == 0)
            {
                payload = payload.Replace(",quantity=" + quantity, "");
            }
            bool answer1 = await DisplayAlert("Notification", payload, "Confirm", "Cancel");
            if (answer1 == true)
            {
                send_to_webhook(payload);
                _ = Navigation.PopToRootAsync();
            }
            
        }
    }
}