﻿using GlucoseTrayCore.Enums;
using Serilog.Events;
using System;

namespace GlucoseTrayCore
{
    public class GlucoseTraySettings
    {
        public FetchMethod FetchMethod { get; set; }
        public string NightscoutUrl { get; set; }
        public DexcomServerLocation DexcomServer { get; set; }
        public string DexcomUsername { get; set; }
        public string DexcomPassword { get; set; }
        public string AccessToken { get; set; }
        public GlucoseUnitType GlucoseUnit { get; set; }
        public double WarningHighBg { get; set; }
        public double HighBg { get; set; }
        public double WarningLowBg { get; set; }
        public double LowBg { get; set; }
        public double CriticalLowBg { get; set; }
        public int PollingThreshold { get; set; }
        public TimeSpan PollingThresholdTimeSpan => TimeSpan.FromSeconds(PollingThreshold);
        public string DatabaseLocation { get; set; }
        public bool EnableDebugMode { get; set; }
        public LogEventLevel LogLevel { get; set; }
        public int StaleResultsThreshold { get; set; }
    }
}