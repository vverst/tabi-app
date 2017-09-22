﻿using System;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace Tabi
{
    public class Settings : BaseViewModel
    {
        private static Settings instance;

        public static Settings Current
        {
            get
            {
                if (instance == null)
                {
                    instance = new Settings();
                }
                return instance;
            }
        }
        public Settings() { }

        public bool Tracking
        {
            get => AppSettings.GetValueOrDefault(nameof(Tracking), false);
            set
            {
                if (value == Tracking)
                    return;

                AppSettings.AddOrUpdateValue(nameof(Tracking), value);
                OnPropertyChanged();
            }
        }

        public bool PermissionsGranted
        {
            get => AppSettings.GetValueOrDefault(nameof(PermissionsGranted), false);
            set
            {
                if (value == PermissionsGranted)
                    return;

                AppSettings.AddOrUpdateValue(nameof(PermissionsGranted), value);
                OnPropertyChanged();
            }
        }

        public bool Developer
        {
            get => AppSettings.GetValueOrDefault(nameof(Developer), false);
            set
            {
                if (value == Developer)
                    return;

                AppSettings.AddOrUpdateValue(nameof(Developer), value);
                OnPropertyChanged();
            }
        }

        public string Device
        {
            get => AppSettings.GetValueOrDefault(nameof(Device), string.Empty);
            set
            {
                if (value == Device)
                    return;

                AppSettings.AddOrUpdateValue(nameof(Device), value);
                OnPropertyChanged();
            }
        }

        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }
    }
}