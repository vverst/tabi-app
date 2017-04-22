﻿using System;
using Android.Content;
using Android.Hardware;
using Android.Runtime;
using Xamarin.Forms;

namespace Tabi.Droid.CollectionService
{
    public class SensorCollection : Java.Lang.Object, ISensorEventListener
    {
        private SensorManager mSensorManager;
        private Sensor gravitySensor;
        private Sensor linearAccSensor;
        private Sensor significantMotionSensor;

        public SensorCollection()
        {
            mSensorManager = (SensorManager)Android.App.Application.Context.GetSystemService(Context.SensorService);
            linearAccSensor = mSensorManager.GetDefaultSensor(SensorType.LinearAcceleration);
            gravitySensor = mSensorManager.GetDefaultSensor(SensorType.Gravity);
            significantMotionSensor = mSensorManager.GetDefaultSensor(SensorType.SignificantMotion);
            if (significantMotionSensor != null)
            {
                mSensorManager.RequestTriggerSensor(new SignificantMotionTriggerListener(mSensorManager, significantMotionSensor), significantMotionSensor);
            }
            //mSensorManager.RegisterListener(this, linearAccSensor, SensorDelay.Normal);
        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            //System.Diagnostics.Debug.WriteLine(accuracy);

        }

        public void OnSensorChanged(SensorEvent e)
        {
            System.Diagnostics.Debug.WriteLine($"{e.Accuracy}");
            foreach (float f in e.Values)
            {
                System.Diagnostics.Debug.WriteLine($"Value : {f}");
            }
        }
    }
}
