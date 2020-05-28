using Plugin.DeviceInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CustomPayload
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LicensePage : ContentPage
    {
        public LicensePage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            var id = CrossDeviceInfo.Current.Id;
            var currentVersion = VersionTracking.CurrentVersion;

            device_id.Text = "Your ID: " + id;
            app_version.Text = "App Version: " + currentVersion;
            
            var license_status_var = Preferences.Get("license_status", "not activated");

            if(license_status_var == "not activated")
            {
                license_status.Text = "Not activated";
                license_status.TextColor = Xamarin.Forms.Color.Red;
                license_key.IsVisible = false;
            }
            else
            {
                activation_key_field.IsVisible = false;
                license_button.IsVisible = false;
                license_key.Text = "License: " + FormatLicenseKey(GetMd5Sum(id));
            }
        }

        static string GetMd5Sum(string productIdentifier)
        {
            System.Text.Encoder enc = System.Text.Encoding.Unicode.GetEncoder();
            byte[] unicodeText = new byte[productIdentifier.Length * 2];
            enc.GetBytes(productIdentifier.ToCharArray(), 0, productIdentifier.Length, unicodeText, 0, true);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(unicodeText);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                sb.Append(result[i].ToString("X2"));
            }
            return sb.ToString();
        }

        static string FormatLicenseKey(string productIdentifier)
        {
            productIdentifier = productIdentifier.Substring(0, 28).ToUpper();
            char[] serialArray = productIdentifier.ToCharArray();
            StringBuilder licenseKey = new StringBuilder();

            int j = 0;
            for (int i = 0; i < 28; i++)
            {
                for (j = i; j < 4 + i; j++)
                {
                    licenseKey.Append(serialArray[j]);
                }
                if (j == 28)
                {
                    break;
                }
                else
                {
                    i = (j) - 1;
                    licenseKey.Append("-");
                }
            }
            return licenseKey.ToString();
        }

        private async void copy_id(object sender, EventArgs e)
        {
            var id = device_id.Text;
            await Clipboard.SetTextAsync(id.Replace("Your ID: ", ""));
        }

        private void activate_license(object sender, EventArgs e)
        {
            if(activation_key_field.Text == null)
            {
                DisplayAlert("Warning", "Please enter a key", "Ok");
                return;
            }
            else
            {
                var entered_key = activation_key_field.Text;
                var id = CrossDeviceInfo.Current.Id;
                var currentVersion = VersionTracking.CurrentVersion;
                if (GetMd5Sum(id + currentVersion) == entered_key.Replace(" ", ""))
                {
                    Preferences.Set("license_status", "activated");
                    Preferences.Set("user_license", entered_key.Replace(" ", ""));
                    DisplayAlert("Information", "Activated succesfully", "Ok");
                    _ = Navigation.PopToRootAsync();
                }
                else
                {
                    DisplayAlert("Information", "Wrong Key", "Ok");
                }
            }
        }
    }
}