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
    public partial class SetupTradePage : ContentPage
    {
        private static readonly HttpClient client = new HttpClient();

        public string Webhook { get; set; }

        public string Coin { get; set; }

        public string Type { get; set; }

        public SetupTradePage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            coin_label.Text = "Coin: " + Coin;
            type_label.Text = "Payload type: " + Type;
            Webhook = Preferences.Get("user_webhook", "https://denyss.requestcatcher.com/test");
        }

        private async void Button_Pressed(object sender, EventArgs e)
        {
            List<string> entries = new List<string>();
            List<string> exits = new List<string>();
            
            //Craft entries
            if (entry1.Text != null)
            {
                entries.Add(entry1.Text); 
            }
            if (entry2.Text != null)
            {
                entries.Add(entry2.Text);
            }
            if (entry3.Text != null)
            {
                entries.Add(entry3.Text);
            }
            if (entry4.Text != null)
            {
                entries.Add(entry4.Text);
            }
            //Craft exits
            if (exit1.Text != null)
            {
                exits.Add(exit1.Text);
            }
            if (exit2.Text != null)
            {
                exits.Add(exit2.Text);
            }
            if (exit3.Text != null)
            {
                exits.Add(exit3.Text);
            }
            if (exit4.Text != null)
            {
                exits.Add(exit4.Text);
            }
            if (exit5.Text != null)
            {
                exits.Add(exit5.Text);
            }
            if (exit6.Text != null)
            {
                exits.Add(exit6.Text);
            }
            //Get stop loss
            if (stoploss.Text == null)
            {
                await DisplayAlert("Error", "Please input stop loss", "Ok");
                return;
            }

            if(exits.Count == 0)
            {
                await DisplayAlert("Error", "Not enough parameters set", "Ok");
                return;
            }

            string payload = "#pssignal_bitmex_" + Coin + "_" + Type + "_";

            foreach(var entry in entries)
            {
                payload = payload + entry + "_";
            }

            payload = payload + "exit_";

            foreach (var exit in exits)
            {
                payload = payload + exit + "_";
            }

            payload = payload + "stop_" + stoploss.Text;

            bool answer = await DisplayAlert("Notification", payload, "Confirm", "Cancel");
            if (answer == true)
            {
                send_to_webhook(payload);
                _ = Navigation.PopToRootAsync();
            }
            
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
                    //await DisplayAlert("Error", e.ToString(), "Ok");
                    await DisplayAlert("Error", "An error occured. Message not sent", "Ok");
                }
            }
            else
            {
                await DisplayAlert("Error", "Please activate the software first", "Ok");
            }
        }
    }
}