﻿using Android.Locations;
using Tabi.DataObjects;

namespace Tabi.Droid
{
    public static class LocationExtensionMethods
    {
        public static PositionEntry ToPositionEntry(this Location location)
        {
			PositionEntry p = new PositionEntry();
			p.Accuracy = location.Accuracy;
			p.Latitude = location.Latitude;
			p.Longitude = location.Longitude;

            p.Course = location.Bearing;

            p.Altitude = location.Altitude;
            p.VerticalAccuracy = location.VerticalAccuracyMeters;
			p.Speed = location.Speed;
	        p.Timestamp = Util.TimeLongToDateTime(location.Time);
            return p;
        }
    }
}
