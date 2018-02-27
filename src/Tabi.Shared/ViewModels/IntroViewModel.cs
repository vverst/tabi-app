﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Tabi.Pages;
using Tabi.Shared.IntroViews;
using Tabi.Shared.Resx;
using TabiApiClient;
using Xamarin.Forms;

namespace Tabi.Shared.ViewModels
{
    public class IntroViewModel : BaseViewModel
    {
        IntroPage introPage;

        public List<View> Views { get; set; } = new List<View>();
        private int nextView = 0;

        public ICommand NextCommand { get; set; }
        public ICommand PermissionCheckCommand { get; set; }
        public ICommand PermissionsCommand { get; set; }
        public ICommand LoginCommand { protected set; get; }
        public ICommand SensorPermissionCommand { get; set; }

        
        public View NextView
        {
            get
            {
                View view;
                if (nextView < Views.Count)
                {
                    view = Views[nextView];
                    nextView++;
                }
                else
                {
                    view = Views.Last();
                }
                return view;
            }
        }

        public bool isLoading;
        public bool IsLoading
        {
            get
            {
                return isLoading;
            }
            set
            {
                isLoading = value;
                OnPropertyChanged();
            }
        }

        public string username;
        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                username = value;
                OnPropertyChanged();
            }
        }

        public string password;
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
                OnPropertyChanged();
            }
        }

        public Color permissionLocationButtonColor = (Color)Application.Current.Resources["blueButtonColor"];
        public Color PermissionLocationButtonColor
        {
            get
            {
                return permissionLocationButtonColor;
            }
            set
            {
                permissionLocationButtonColor = value;
                OnPropertyChanged();
            }
        }


        public Color permissionCheckButtonColor = Color.Gray;
        public Color PermissionCheckButtonColor
        {
            get
            {
                return permissionCheckButtonColor;
            }
            set
            {
                permissionCheckButtonColor = value;
                OnPropertyChanged();
            }
        }

        public Color permissionSensorButtonColor = (Color)Application.Current.Resources["blueButtonColor"];
        public Color PermissionSensorButtonColor
        {
            get
            {
                return permissionSensorButtonColor;
            }
            set
            {
                permissionSensorButtonColor = value;
                OnPropertyChanged();
            }
        }

        private bool permissionsGiven;
        public bool PermissionsGiven
        {
            get
            {
                return permissionsGiven;
            }
            set
            {
                permissionsGiven = value;
                OnPropertyChanged();
            }
        }

        private bool locationPermissionGiven;
        public bool LocationPermissionGiven
        {
            get
            {
                return locationPermissionGiven;
            }
            set
            {
                locationPermissionGiven = value;
                OnPropertyChanged();
            }
        }

        private bool sensorPermissionGiven;
        public bool SensorPermissionGiven
        {
            get
            {
                return sensorPermissionGiven;
            }
            set
            {
                sensorPermissionGiven = value;
                OnPropertyChanged();
            }
        }

        private void GoNextView()
        {
            introPage.Content = this.NextView;
        }


        public IntroViewModel(IntroPage ip)
        {
            introPage = ip;

            Views.Add(new FirstIntroView());
            Views.Add(new LoginIntroView());

            PermIntroView permIntroView = new PermIntroView();
            InitMotionPermissionButton(permIntroView);
            Views.Add(permIntroView);

            
            LoginCommand = new Command(async (obj) =>
            {
                ApiClient ac = new ApiClient(App.Configuration["api-url"]);
                TokenResult tokenResult = null;
                try
                {
                    IsLoading = true;
                    tokenResult = await ac.Authenticate(username, password);
                    IsLoading = false;

                }
                catch (Exception exc)
                {
                    await introPage.DisplayAlert("Error occured", $"Error: {exc}", "Ok");
                    return;
                }
                if (tokenResult != null)
                {
                    Settings.Current.Username = username;
                    Settings.Current.Password = password;

                    if (await ac.GetDevice(Settings.Current.Device) != null)
                    {
                        // Device is already registered
                    }
                    else
                    {
                        bool success = await ac.RegisterDevice(Settings.Current.Device);
                        if (!success)
                        {
                            await introPage.DisplayAlert(AppResources.ErrorOccurredTitle, "Problem registering device", "OK");

                        }
                    }

                    GoNextView();
                }
                else
                {
                    await introPage.DisplayAlert(AppResources.LoginFailureTitle, AppResources.LoginFailureText, "OK");
                }
            });

            PermissionsCommand = new Command(async (obj) =>
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                    {
                        await introPage.DisplayAlert(
                            AppResources.LocationPermissionRationaleTitle,
                            AppResources.LocationPermissionRationaleText,
                            AppResources.OkText);
                    }
                    if (status == PermissionStatus.Denied && Device.RuntimePlatform == Device.iOS)
                    {
                        await introPage.DisplayAlert(
                            AppResources.LocationPermissionDeniedOpenSettingsiOSTitle,
                            AppResources.LocationPermissionDeniedOpenSettingsiOSText,
                            AppResources.OkText);

                        CrossPermissions.Current.OpenAppSettings();
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                    //Best practice to always check that the key exists
                    if (results.ContainsKey(Permission.Location))
                        status = results[Permission.Location];
                }

                if (status == PermissionStatus.Granted)
                {
                    PermissionLocationButtonColor = (Color)Application.Current.Resources["greenButtonColor"];
                    LocationPermissionGiven = true;
                    CheckPermissionsGiven();
                }
            });

            PermissionCheckCommand = new Command((obj) =>
            {
                //triggered when?
                if (PermissionsGiven)
                {
                    introPage.Navigation.PopModalAsync();
                    Settings.Current.PermissionsGranted = true;
                    Settings.Current.Tracking = true;
                }
            });

            NextCommand = new Command((obj) => { GoNextView(); });

            SensorPermissionCommand = new Command(async (obj) => {
                //permission from iOS for usage of pedometer
                // double check if OS is iOS
                if (Device.RuntimePlatform == Device.iOS)
                {
                    var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Sensors);

                    switch (status)
                    {
                        case PermissionStatus.Granted:
                            PermissionSensorButtonColor = (Color)Application.Current.Resources["greenButtonColor"];
                            SensorPermissionGiven = true;
                            CheckPermissionsGiven();
                            break;

                        default:
                            if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Sensors))
                            {
                                await introPage.DisplayAlert(
                                    AppResources.SensorPermissionRationaleTitle,
                                    AppResources.SensorPermissionRationaleText,
                                    AppResources.OkText);
                            }
                            if (status == PermissionStatus.Denied)
                            {
                                await introPage.DisplayAlert(
                                    AppResources.SensorPermissionDeniedOpenSettingsiOSTitle,
                                    AppResources.SensorPermissionDeniedOpenSettingsiOSText,
                                    AppResources.OkText);

                                CrossPermissions.Current.OpenAppSettings();
                            }
                            var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Sensors);
                            //Best practice to always check that the key exists
                            if (results.ContainsKey(Permission.Sensors))
                                status = results[Permission.Sensors];
                            break;
                    }
                }
                else
                {
                    PermissionSensorButtonColor = (Color)Application.Current.Resources["greenButtonColor"];
                    SensorPermissionGiven = true;
                    CheckPermissionsGiven();
                }
            });
            GoNextView();
        }

        private void CheckPermissionsGiven()
        {
            if (LocationPermissionGiven && SensorPermissionGiven)
            {
                PermissionsGiven = true;
                PermissionCheckButtonColor = (Color)Application.Current.Resources["blueButtonColor"];
            }
        }

        private void InitMotionPermissionButton(View view)
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                // show button for permission
                var topLayout = view.FindByName<StackLayout>("Stacklayout_top");

                topLayout.Children.Add(new Button()
                {
                    Margin = new Thickness(30, 0, 30, 30),
                    Text = (string)Application.Current.Resources["SensorPermissionButton"],
                    BackgroundColor = (Color)Application.Current.Resources["blueButtonColor"],
                    Command = SensorPermissionCommand
                });
            }
            else
            {
                // don't show button for permission 
                // set 
                SensorPermissionGiven = true;
            }
        }
    }
}
