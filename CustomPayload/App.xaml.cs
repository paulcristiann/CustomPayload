
using Plugin.DeviceInfo;
using System;
using System.Security.Cryptography;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CustomPayload
{
    public partial class App : Application
    {
        public App()
        {
            //Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NzM3NEAzMTM3MmUzNDJlMzBPRm41TTBEL2hiZ0pjbG93dDZPQ0VocmRCWkJHSXlzWFgrUkxrZVlDaUpzPQ==");

            InitializeComponent();

            var device_id = CrossDeviceInfo.Current.Id;
            var firstLaunch = VersionTracking.IsFirstLaunchEver;
            var currentVersion = VersionTracking.CurrentVersion;

            if (firstLaunch)
            {
                Preferences.Set("license_status", "not activated");
                Preferences.Remove("user_license");
            }
            else
            {
                //Look for a key
                var license_stored = Preferences.Get("user_license", null);
                if (string.IsNullOrWhiteSpace(license_stored))
                {
                    Preferences.Set("license_status", "not activated");
                }
                else
                {
                    if (GetMd5Sum(device_id + currentVersion) == license_stored)
                    {
                        Preferences.Set("license_status", "activated");
                    }
                    else
                    {
                        Preferences.Set("license_status", "not activated");
                    }
                }
            }

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
            

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

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
