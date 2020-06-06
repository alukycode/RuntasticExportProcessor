using System;
using RuntasticExportProcessor.Enums;

namespace RuntasticExportProcessor.Types
{
    public class SportSessionData
    {
        public string Json { get; set; }
        public ParsedJsonData ParsedJson { get; set; }

        public string GpsDataFilePath { get; set; }
        public bool HasGpsData { get; set; }

        public DateTime StartTime { get; set; }
        public ActivityTypeId ActivityTypeId { get; set; }
        public int TimeZone { get; set; }
    }
}
