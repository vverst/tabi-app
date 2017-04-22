﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using Xamarin.Forms;
using PCLStorage;
using Xamarin.Forms.Xaml;
using System.ComponentModel;
using Tabi.Helpers;
using System.Net.NetworkInformation;
using Plugin.Permissions.Abstractions;

namespace Tabi
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PermissionsPage : ContentPage
    {
        IPermissions permissionsPlugin = Plugin.Permissions.CrossPermissions.Current;

        async void Grant_Clicked(object sender, System.EventArgs e)
        {
            var status = await permissionsPlugin.CheckPermissionStatusAsync(Permission.Location);

            // On iOS direct user to Settings if the user has denied access.
            if (status == PermissionStatus.Denied && Device.RuntimePlatform == Device.iOS)
            {
                var openSettingsAnswer = await DisplayAlert("Locatiebepaling vereist", "Zet alstublieft locatiebepaling aan.", "Open instellingen", "Annuleer");
                if (openSettingsAnswer)
                {
                    permissionsPlugin.OpenAppSettings();
                }
            }
            else
            {
                var resultDict = await permissionsPlugin.RequestPermissionsAsync(Permission.Location);
                PermissionStatus resultStatus = resultDict[Permission.Location];

                if (resultStatus == PermissionStatus.Granted)
                {
                    Settings.Current.PermissionsGranted = true;
                    await Navigation.PopModalAsync();
                }

                //            foreach (KeyValuePair<Permission, PermissionStatus> entry in result)
                //{
                //                Debug.WriteLine(entry.Key + " " + entry.Value);

                //}

            }

        }

        SettingsViewModel ViewModel => vm ?? (vm = BindingContext as SettingsViewModel);
        SettingsViewModel vm;

        public PermissionsPage()
        {
            InitializeComponent();
            BindingContext = new SettingsViewModel(Navigation);


        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();


        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ViewModel.Settings.PropertyChanged -= Settings_PropertyChanged;
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        async void SqliteOpen(object sender, EventArgs e)
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            var t = await rootFolder.GetFileAsync("tabi");
            DependencyService.Get<IShareFile>().ShareFile(t.Path);
        }
    }
}
