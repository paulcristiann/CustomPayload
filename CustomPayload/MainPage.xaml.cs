using Plugin.DeviceInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CustomPayload
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        async void Button_Pressed(object sender, EventArgs e)
        {
            var coin = radioGroup.CheckedItem;
            var payload_type = radioGroup1.CheckedItem;

            if (coin == null || payload_type == null)
            {
                await DisplayAlert("Error", "Please select coin and type", "Ok");
                return;
            }

            var coin_string = coin.Text;
            var payload_type_string = payload_type.Text;

            if (coin_string != "XBT" && coin_string != "ETH")
            {
                coin_string = coin_string + "H20";
            }
            else
            {
                coin_string = coin_string + "USD";
            }

            var function_page_instance = new FunctionPage();
            function_page_instance.Coin = coin_string;
            function_page_instance.Type = payload_type_string;
            await Navigation.PushAsync(function_page_instance);
        }

        private async void open_webhook_config(object sender, EventArgs e)
        {
            var webhook_page_instance = new WebhookPage();
            await Navigation.PushAsync(webhook_page_instance);
        }

        private async void open_license_config(object sender, EventArgs e)
        {
            var license_page_instance = new LicensePage();
            await Navigation.PushAsync(license_page_instance);
        }
    }
}
