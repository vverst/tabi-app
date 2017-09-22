﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Tabi.DataObjects;
using Tabi.DataStorage;
using Tabi.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Tabi
{
    public partial class StopDetailPage : ContentPage
    {
        StopDetailViewModel ViewModel => vm ?? (vm = BindingContext as StopDetailViewModel);
        StopDetailViewModel vm;

        IStopRepository stopRepository = App.RepoManager.StopRepository;

        public StopDetailPage(StopVisit sv)
        {
            InitializeComponent();
            routeMap.HeightRequest = App.ScreenHeight * 0.30;
            routeMap.ClearMap();
            SetupPage(sv);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            SaveStop();
        }

        private void SetupPage(StopVisit sv)
        {

            SetMapLocation(sv.Stop);
            BindingContext = new StopDetailViewModel(sv);
        }



        private void SetMapLocation(Stop s)
        {
            if (s == null)
            {
                return;
            }
            var pos = new Xamarin.Forms.Maps.Position(s.Latitude, s.Longitude);

            string labelPin = "Stop";
            if (s.Name != null)
            {
                labelPin = s.Name;
            }

            routeMap.Pins.Add(new Xamarin.Forms.Maps.Pin() { Label = labelPin, Position = pos });
            routeMap.MoveToRegion(MapSpan.FromCenterAndRadius(pos, Distance.FromMeters(300)));
        }

        private void SaveStop()
        {
            stopRepository.Update(ViewModel.Stop);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            vm = null;
        }
    }
}