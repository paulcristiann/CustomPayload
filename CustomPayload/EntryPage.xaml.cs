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
    public partial class EntryPage : ContentPage
    {
        public string Coin { get; set; }

        public string Type { get; set; }

        private static readonly HttpClient client = new HttpClient();

        public string Webhook { get; set; }

        public EntryPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            coin_label.Text = "Coin: " + Coin;
            type_label.Text = "Payload type: " + Type;
            percentage_entry.IsVisible = false;
            trail_label.IsVisible = false;
            trail_switch.IsVisible = false;
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

        private void CheckField(object sender, EventArgs e)
        {
            if (radioGroup.CheckedItem.Text == "Limit")
            {
                //percentage_label.IsVisible = true;
                percentage_entry.IsVisible = true;
                trail_label.IsVisible = true;
                trail_switch.IsVisible = true;
            }
            else
            {
                //percentage_label.IsVisible = false;
                percentage_entry.IsVisible = false;
                trail_label.IsVisible = false;
                trail_switch.IsVisible = false;
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
            var maxposition = maxposition_entry.Text;

            if (maxposition == null)
            {
                await DisplayAlert("Notification", "Please set maxposition", "Ok");
                return;
            }

            if (quantity == null || float.Parse(quantity) < 0 || float.Parse(quantity) > 100)
            {
                await DisplayAlert("Notification", "Please set quantity between 0 and 100", "Ok");
                return;
            }

            if (type == "Limit")
            {
                var percentage = percentage_entry.Text;
                if (percentage == null || float.Parse(percentage) < 0 || float.Parse(percentage) > 100)
                {
                    await DisplayAlert("Notification", "Please set percentage between 0 and 100", "Ok");
                    return;
                }
                //send_to_webhook
                if (Type == "LONG")
                {
                    payload = "symbol=" + Coin + ",type=limit,side=buy,price=" + percentage + ",quantity=" + quantity + ",maxposition=" + maxposition;
                }
                else
                {
                    payload = "symbol=" + Coin + ",type=limit,side=sell,price=" + percentage + ",quantity=" + quantity + ",maxposition=" + maxposition;
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
                
                return;
            }
            //send to webhook
            if (Type == "LONG")
            {
                payload = "symbol=" + Coin + ",type=market,side=buy,quantity=" + quantity + ",maxposition=" + maxposition + ";";
            }
            else
            {
                payload = "symbol=" + Coin + ",type=market,side=sell,quantity=" + quantity + ",maxposition=" + maxposition + ";";
            }

            if (float.Parse(quantity) == 0)
            {
                payload = payload.Replace(",quantity=" + quantity, "");
            }
            bool answer2 = await DisplayAlert("Notification", payload, "Confirm", "Cancel");
            if (answer2 == true)
            {
                send_to_webhook(payload);
                _ = Navigation.PopToRootAsync();
            }
            
        }
    }
}