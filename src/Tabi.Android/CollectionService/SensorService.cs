﻿using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Hardware;
using Tabi.DataStorage;
using Tabi.DataObjects;

namespace Tabi.Droid.CollectionService
{
    [Service]
    public class SensorService : Service, ISensorEventListener
    {
        private readonly IHeadingRepository _headingRepository;
        private readonly ISensorRepository<Accelerometer> _accelerometerRepository;
        private readonly ISensorRepository<Gyroscope> _gyroscopeRepository;
        private readonly ISensorRepository<Magnetometer> _magnetometerRepository;
        private readonly ISensorRepository<LinearAcceleration> _linearAccelerationRepository;
        private readonly ISensorRepository<RotationVector> _rotationVectorRepository;
        private readonly ISensorRepository<Quaternion> _quaternionRepository;
        private readonly ISensorRepository<Gravity> _gravityRepository;

        private SensorServiceBinder _binder;

        public SensorService()
        {
            _headingRepository = App.RepoManager.HeadingRepository;
            _accelerometerRepository = App.RepoManager.AccelerometerRepository;
            _gyroscopeRepository = App.RepoManager.GyroscopeRepository;
            _magnetometerRepository = App.RepoManager.MagnetometerRepository;
            _linearAccelerationRepository = App.RepoManager.LinearAccelerationRepository;
            _rotationVectorRepository = App.RepoManager.RotationVectorRepository;
            _quaternionRepository = App.RepoManager.QuaternionRepository;
            _gravityRepository = App.RepoManager.GravityRepository;
        }


        public override IBinder OnBind(Intent intent)
        {
            // service binder is used to communicate with the service
            _binder = new SensorServiceBinder(this);
            return _binder;
        }

        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            //register sensors for listening
            
            var sensorManager = (SensorManager)Application.Context.GetSystemService(Context.SensorService);

            Sensor accelerometer = sensorManager.GetDefaultSensor(SensorType.Accelerometer);
            sensorManager.RegisterListener(this, accelerometer, SensorDelay.Normal);

            Sensor gyroscope = sensorManager.GetDefaultSensor(SensorType.Gyroscope);
            sensorManager.RegisterListener(this, gyroscope, SensorDelay.Normal);

            Sensor magnetometer = sensorManager.GetDefaultSensor(SensorType.MagneticField);
            sensorManager.RegisterListener(this, magnetometer, SensorDelay.Normal);


            //sensor fusion
            Sensor rotationVector = sensorManager.GetDefaultSensor(SensorType.RotationVector);
            sensorManager.RegisterListener(this, rotationVector, SensorDelay.Normal);
            
            Sensor linearAcceleration = sensorManager.GetDefaultSensor(SensorType.LinearAcceleration);
            sensorManager.RegisterListener(this, linearAcceleration, SensorDelay.Normal);

            Sensor gravity = sensorManager.GetDefaultSensor(SensorType.Gravity);
            sensorManager.RegisterListener(this, gravity, SensorDelay.Normal);

            Sensor heading = sensorManager.GetDefaultSensor(SensorType.Orientation);
            sensorManager.RegisterListener(this, heading, SensorDelay.Normal);


            return StartCommandResult.Sticky;
        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            
        }

        public void OnSensorChanged(SensorEvent e)
        {
            //start gathering data and push to SqliteDB
            switch (e.Sensor.Type)
            {
                case SensorType.Accelerometer:
                    _accelerometerRepository.Add(new Accelerometer()
                    {
                        Timestamp = DateTimeOffset.Now,
                        X = e.Values[0],
                        Y = e.Values[1],
                        Z = e.Values[2],
                    });
                    break;

                case SensorType.Gyroscope:
                    _gyroscopeRepository.Add(new Gyroscope()
                    {
                        Timestamp = DateTimeOffset.Now,
                        X = e.Values[0],
                        Y = e.Values[1],
                        Z = e.Values[2],
                    });
                    break;

                case SensorType.MagneticField:
                    _magnetometerRepository.Add(new Magnetometer()
                    {
                        Timestamp = DateTimeOffset.Now,
                        X = e.Values[0],
                        Y = e.Values[1],
                        Z = e.Values[2],
                    });
                    break;

                case SensorType.RotationVector:
                    _rotationVectorRepository.Add(new RotationVector()
                    {
                        Timestamp = DateTimeOffset.Now,
                        X = e.Values[0],
                        Y = e.Values[1],
                        Z = e.Values[2]
                    });

                    //quaternion?

                    break;

                case SensorType.Gravity:
                    _gravityRepository.Add(new Gravity()
                    {
                        Timestamp = DateTimeOffset.Now,
                        X = e.Values[0],
                        Y = e.Values[1],
                        Z = e.Values[2]
                    });
                    break;

                case SensorType.LinearAcceleration:
                    _linearAccelerationRepository.Add(new LinearAcceleration()
                    {
                        Timestamp = DateTimeOffset.Now,
                        X = e.Values[0],
                        Y = e.Values[1],
                        Z = e.Values[2]
                    });
                    break;

                case SensorType.Orientation:
                    _headingRepository.Add(new Heading()
                    {
                        Timestamp = DateTimeOffset.Now,
                        Value = e.Values[0]
                    });
                    break;

                default:
                    break;
            }
        }

        void SessionMeasurementTrigger(TriggerEvent e)
        {

        }
    }
}