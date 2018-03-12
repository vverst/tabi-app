﻿using System;
using System.Collections.Generic;
using Tabi.DataObjects;

namespace Tabi.DataStorage
{
    public interface ISensorMeasurementSessionRepository : IRepository<SensorMeasurementSession>
    {
        IEnumerable<SensorMeasurementSession> GetRange(DateTimeOffset begin, DateTimeOffset end);
    }
}
