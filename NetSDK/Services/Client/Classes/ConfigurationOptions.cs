﻿
namespace Splitio.Services.Client.Classes
{
    public class ConfigurationOptions
    {
        public string Endpoint { get; set; }
        public string EventsEndpoint { get; set; }
        public string LocalhostFilePath { get; set; }
        public int? FeaturesRefreshRate { get; set; } 
        public int? SegmentsRefreshRate { get; set; } 
        public bool RandomizeRefreshRates { get; set; } 
        public int? ImpressionsRefreshRate { get; set; }
        public int? MaxImpressionsLogSize { get; set; }  
        public long? ConnectionTimeout { get; set; } 
        public long? ReadTimeout { get; set; } 
        public int? Ready { get; set; }  
        public int? MaxMetricsCountCallsBeforeFlush { get; set; } 
        public int? MetricsRefreshRate { get; set; } 
        public int? SplitsStorageConcurrencyLevel { get; set; } 
        public string SdkMachineName { get; set; }
        public string SdkMachineIP{ get; set; }
        public int? NumberOfParalellSegmentTasks { get; set; }
        public bool LabelsEnabled { get; set; } 
    }
}
