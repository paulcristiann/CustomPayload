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
    public partial class FunctionPage : ContentPage
    {
        private static readonly HttpClient client = new HttpClient();

        public string Webhook { get; set; }

        public string Coin { get; set; }

        public string Type { get; set; }

        public FunctionPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            coin_label.Text = "Coin: " + Coin;
            type_label.Text = "Payload type: " + Type;
            Webhook = Preferences.Get("user_webhook", "https://denyss.requestcatcher.com/test");
        }

        async void Button_Pressed(object sender, EventArgs e)
        {
            if (radioGroup.CheckedItem == null)
            {
                await DisplayAlert("Error", "Please select an action", "Ok");
                return;
            }

            var action_entered = radioGroup.CheckedItem.Text;

            if (action_entered == "Cancel orders")
            {
                var cancel_orders_page_instance = new CancelOrdersPage();
                cancel_orders_page_instance.Coin = Coin.ToString();
                cancel_orders_page_instance.Type = Type.ToString();
                await Navigation.PushAsync(cancel_orders_page_instance);
            }
            if (action_entered == "Entry")
            {
                var entry_page_instance = new EntryPage();
                entry_page_instance.Coin = Coin.ToString();
                entry_page_instance.Type = Type.ToString();
                await Navigation.PushAsync(entry_page_instance);
            }
            if (action_entered == "Exit")
            {
                var exit_page_instance = new ExitPage();
                exit_page_instance.Coin = Coin.ToString();
                exit_page_instance.Type = Type.ToString();
                await Navigation.PushAsync(exit_page_instance);
            }
            if (action_entered == "Stop")
            {
                var stop_page_instance = new StopPage();
                stop_page_instance.Coin = Coin.ToString();
                stop_page_instance.Type = Type.ToString();
                await Navigation.PushAsync(stop_page_instance);
            }
            if (action_entered == "Setup trade")
            {
                var setup_trade_page_instance = new SetupTradePage();
                setup_trade_page_instance.Coin = Coin.ToString();
                setup_trade_page_instance.Type = Type.ToString();
                await Navigation.PushAsync(setup_trade_page_instance);
            }
        }

        private async void Close_position_Pressed(object sender, EventArgs e)
        {
            var payload = "";
            if(Type.ToUpper() == "SHORT")
            {
                payload = "symbol=" + Coin + ",type=cancelorders;symbol=" + Coin + ",type=limitclose,side=sell,trail=1;";
            }
            else
            {
                payload = "symbol=" + Coin + ",type=cancelorders;symbol=" + Coin + ",type=limitclose,side=buy,trail=1;";
            }
            bool answer = await DisplayAlert("Notification", payload, "Confirm", "Cancel");
            if (answer == true)
            {
                send_to_webhook(payload);
            }
            _ = Navigation.PopToRootAsync();
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