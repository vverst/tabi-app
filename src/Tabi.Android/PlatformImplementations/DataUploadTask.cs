﻿using System;
using System.Threading.Tasks;
using Android.Content;
using Tabi.DataSync;
using Tabi.Helpers;

namespace Tabi.Droid.PlatformImplementations
{
    public class DataUploadTask : IDataUploadTask
    {
        private readonly Context _androidContext;
        private readonly TabiConfiguration _configuration;

        public DataUploadTask(TabiConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _androidContext = Android.App.Application.Context;
        }

        public Task Start()
        {
            Intent dataSyncIntent = new Intent(_androidContext, typeof(DataUploadService));
            dataSyncIntent.PutExtra("interval", _configuration.Api.SyncInterval);
            dataSyncIntent.PutExtra("autoUpload", Settings.Current.WifiOnly);
            _androidContext.StartService(dataSyncIntent);

            return Task.CompletedTask;
        }
    }
}
