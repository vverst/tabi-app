﻿using System;
using MvvmHelpers;
using SQLite;

namespace Tabi.DataObjects
{
    public abstract class MotionSensor : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }

        public int TrackEntryId { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }
}
