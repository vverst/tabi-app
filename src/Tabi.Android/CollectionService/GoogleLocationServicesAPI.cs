﻿using System;
using Android.App;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Locations;
using Android.OS;
using Autofac;
using Tabi.Collection;
using Tabi.DataObjects;
using Tabi.DataObjects.CollectionProfile;
using Tabi.DataStorage;
using Tabi.Droid.CollectionService;
using Tabi.Helpers;
using Tabi.Logging;
using ILocationListener = Android.Gms.Location.ILocationListener;
using Object = Java.Lang.Object;

namespace Tabi.Droid
{
    public class GoogleLocationServicesAPI : Object, IAndroidLocation, ILocationListener,
        GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
    {
        private GoogleApiClient client;
        private readonly IPositionEntryRepository positionEntryRepository;
        private readonly IRepoManager _repoManager;
        private readonly BatteryHelper _batteryHelper;
        private readonly ProfileAndroid currentProfile;
        private LocationRequestProfile currentRequestProfile;
        private readonly PositionCache positionCache = new PositionCache();
        private PositionEntry previousPositionEntry;

        public GoogleLocationServicesAPI()
        {
            _repoManager = App.Container.Resolve<IRepoManager>();
            _batteryHelper = App.Container.Resolve<BatteryHelper>();

            positionEntryRepository = _repoManager.PositionEntryRepository;


            currentProfile = App.CollectionProfile.AndroidProfile;
        }

        public void RequestLocationUpdates()
        {
            Log.Info("GoogleApi: Requested location updates");

            client = GetGoogleAPIClient();
            client.Connect();
        }


        public void StopLocationUpdates()
        {
            LocationServices.FusedLocationApi.RemoveLocationUpdates(client, this);
        }

        public void RequestUpdateNow()
        {
            Log.Debug("RequestUpdateNow: HighTrackingProfile");
            currentRequestProfile = currentProfile.HighTrackingProfile;
            RestartLocationUpdates();

        }

        void GoogleApiClient.IConnectionCallbacks.OnConnected(Bundle connectionHint)
        {
            Log.Info("GoogleApi: Connected!");
            currentRequestProfile = currentProfile.HighTrackingProfile;
            StartLocationUpdates();
        }

        void GoogleApiClient.IConnectionCallbacks.OnConnectionSuspended(int cause)
        {
        }

        public void OnLocationChanged(Location location)
        {
            _batteryHelper.CheckStoreBatteryLevel(TimeSpan.FromMinutes(10));

            positionCache.Distance = currentProfile.DistanceDeltaLowTracking;
            PositionEntry positionEntry = location.ToPositionEntry();

            positionCache.Add(positionEntry);
            Log.Trace($"GoogleApi: Location was changed {positionCache.PreviousDistance}");

            positionEntry.DistanceBetweenPreviousPosition = positionCache.PreviousDistance;

            if (positionCache.Check(TimeSpan.FromSeconds(currentProfile.TimePeriodLowTracking)))
            {
                // Enable Low Tracking if not already enabled
                if (currentRequestProfile != currentProfile.LowTrackingProfile)
                {
                    Log.Info("LowTracking");
                    currentRequestProfile = currentProfile.LowTrackingProfile;
                    RestartLocationUpdates();
                }
            }
            else if (currentRequestProfile != currentProfile.HighTrackingProfile)
            {
                Log.Info("HighTracking");
                currentRequestProfile = currentProfile.HighTrackingProfile;
                RestartLocationUpdates();
            }

            if (!positionEntry.Equals(previousPositionEntry))
            {
                positionEntryRepository.Add(positionEntry);
            }
        }

        void GoogleApiClient.IOnConnectionFailedListener.OnConnectionFailed(ConnectionResult result)
        {
            Log.Info($"GoogleApi: Failed! {result.ErrorMessage}");
        }


        public void StartLocationUpdates(LocationRequestProfile profile)
        {
            LocationRequest req = CreateLocationRequest(profile);
            LocationServices.FusedLocationApi.RequestLocationUpdates(client, req, this);
        }

        public void StartLocationUpdates()
        {
            StartLocationUpdates(currentRequestProfile);
        }

        public void RestartLocationUpdates()
        {
            StopLocationUpdates();
            StartLocationUpdates();
        }

        private GoogleApiClient GetGoogleAPIClient()
        {
            return new GoogleApiClient.Builder(Application.Context)
                .AddApi(LocationServices.API)
                .AddConnectionCallbacks(this)
                .AddOnConnectionFailedListener(this)
                .Build();
        }

        private LocationRequest CreateLocationRequest(LocationRequestProfile profile)
        {
            var request = new LocationRequest();
            request.SetPriority(profile.Priority);
            request.SetInterval(profile.Interval);

            return request;
        }
    }
}