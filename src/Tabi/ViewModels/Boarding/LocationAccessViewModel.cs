﻿using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Tabi.Pages.OnBoarding;
using Tabi.Resx;
using Xamarin.Forms;

namespace Tabi.ViewModels
{
    public class LocationAccessViewModel : BaseViewModel
    {
        public LocationAccessViewModel()
        {
            NextCommand = new Command(Next);
            LocationPermissionCommand = new Command(async () => await RequestLocationPermissionAsync());
        }

        private bool _isReady;
        public bool IsReady
        {
            get
            {
                return _isReady;
            }
            set
            {
                SetProperty<bool>(ref _isReady, value);
            }
        }

        private bool _locationPermissionGiven;
        public bool LocationPermissionGiven
        {
            get
            {
                return _locationPermissionGiven;
            }
            set
            {
                SetProperty<bool>(ref _locationPermissionGiven, value);
            }
        }

        private Style _locationButtonStyle = (Style)Application.Current.Resources["defaultButtonStyle"];
        public Style LocationButtonStyle
        {
            get
            {
                return _locationButtonStyle;
            }
            set
            {
                SetProperty<Style>(ref _locationButtonStyle, value);
            }
        }


        private bool _locationPermissionIncorrect;
        public bool LocationPermissionIncorrect
        {
            get
            {
                return _locationPermissionIncorrect;
            }
            set
            {
                SetProperty<bool>(ref _locationPermissionIncorrect, value);
            }
        }

        private string _locationButtonText;
        public string LocationButtonText
        {
            get
            {
                return _locationButtonText;
            }
            set
            {
                SetProperty<string>(ref _locationButtonText, value);
            }
        }


        public INavigation Navigation { get; set; }

        public Page Page { get; set; }

        public ICommand NextCommand { get; set; }

        public ICommand LocationPermissionCommand { get; set; }


        private async void Next()
        {
            PermissionStatus status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Sensors);

            // iOS needs to request motion access:
            if (Device.RuntimePlatform == Device.iOS && status != PermissionStatus.Disabled)
            {
                Navigation.InsertPageBefore(new MotionAccessPage(), Page);
            }
            else if(Device.RuntimePlatform == Device.iOS)
            {
                Navigation.InsertPageBefore(new ThanksPage(), Page);
            }
            else if (Device.RuntimePlatform == Device.Android)
            {
                Navigation.InsertPageBefore(new ThanksPage(), Page);
            }
            await Navigation.PopAsync();
        }

        private void PermissionStatusSetViewModel(PermissionStatus status)
        {
            LocationPermissionGiven = false;

            switch (status)
            {
                case PermissionStatus.Granted:
                    LocationPermissionGiven = true;
                    IsReady = true;
                    LocationPermissionIncorrect = false;
                    LocationButtonText = AppResources.LocationPermissionGiven;
                    LocationButtonStyle = (Style)Application.Current.Resources["successButtonStyle"];

                    break;
                case PermissionStatus.Denied:
                    LocationPermissionIncorrect = true;
                    LocationButtonText = AppResources.LocationTryAgainLabel;
                    LocationButtonStyle = (Style)Application.Current.Resources["warningButtonStyle"];

                    break;
                case PermissionStatus.Restricted:
                    LocationPermissionIncorrect = true;
                    LocationButtonText = AppResources.LocationTryAgainLabel;
                    LocationButtonStyle = (Style)Application.Current.Resources["warningButtonStyle"];

                    break;
                case PermissionStatus.Disabled:
                    LocationButtonText = AppResources.EnableLocationServices;
                    LocationPermissionIncorrect = true;
                    LocationButtonStyle = (Style)Application.Current.Resources["warningButtonStyle"];
                    break;
                case PermissionStatus.Unknown:
                    break;
            }
        }

        public async Task OnAppearingAsync()
        {
            await CheckLocationPermissionAsync();
        }

        public async Task CheckLocationPermissionAsync()
        {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
            PermissionStatusSetViewModel(status);
        }

        private async Task RequestLocationPermissionAsync()
        {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
            if (status != PermissionStatus.Granted)
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                {
                    await Page.DisplayAlert(
                        AppResources.LocationPermissionRationaleTitle,
                        AppResources.LocationPermissionRationaleText,
                        AppResources.OkText);
                }
                if (status == PermissionStatus.Denied && Device.RuntimePlatform == Device.iOS)
                {
                    bool openSettings = await Page.DisplayAlert(
                        AppResources.LocationPermissionDeniedOpenSettingsiOSTitle,
                        AppResources.LocationPermissionDeniedOpenSettingsiOSText,
                        AppResources.Settings, AppResources.CancelText);

                    if (openSettings)
                    {
                        CrossPermissions.Current.OpenAppSettings();
                    }
                }

                var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                //Best practice to always check that the key exists
                if (results.ContainsKey(Permission.Location))
                {
                    status = results[Permission.Location];
                }
            }

            PermissionStatusSetViewModel(status);
        }
    }
}
