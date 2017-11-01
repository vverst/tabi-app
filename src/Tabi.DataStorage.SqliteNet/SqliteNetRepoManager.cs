﻿using System;
using SQLite;
using Tabi.DataObjects;

namespace Tabi.DataStorage.SqliteNet
{
    public class SqliteNetRepoManager : IRepoManager
    {
        private static SQLiteConnection conn;

        public SqliteNetRepoManager(string path)
        {
            if (conn == null)
            {
                conn = new SQLiteConnection(
                    path,
                    SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex,
                    false);
            }

            conn.CreateTable<User>();
            conn.CreateTable<Device>();
            conn.CreateTable<PositionEntry>();
            conn.CreateTable<Stop>();
            conn.CreateTable<StopVisit>();
            conn.CreateTable<BatteryEntry>();
            conn.CreateTable<TrackEntry>();
            conn.CreateTable<LogEntry>();


            PositionEntryRepository = new SqliteNetPositionEntryRepository(conn);
            DeviceRepository = new SqliteNetDeviceRepository(conn);
            StopRepository = new SqliteNetStopRepository(conn);
            StopVisitRepository = new SqliteNetStopVisitRepository(conn);
            BatteryEntryRepository = new SqliteNetBatteryEntryRepository(conn);
            TrackEntryRepository = new SqliteNetTrackEntryRepository(conn);
            LogEntryRepository = new SqliteNetLogEntryRepository(conn);
        }

        public IUserRepository UserRepository { get; private set; }
        public IDeviceRepository DeviceRepository { get; private set; }
        public IPositionEntryRepository PositionEntryRepository { get; private set; }
        public IMotionEntryRepository MotionEntryRepository { get; private set; }
        public IStopRepository StopRepository { get; private set; }
        public IStopVisitRepository StopVisitRepository { get; private set; }
        public IBatteryEntryRepository BatteryEntryRepository { get; private set; }
        public ITrackEntryRepository TrackEntryRepository { get; }
        public ILogEntryRepository LogEntryRepository { get; }


        public void SaveChanges()
        {
            throw new NotImplementedException();
        }
    }
}