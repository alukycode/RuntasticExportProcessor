using System;
using RuntasticExportProcessor.Enums;

namespace RuntasticExportProcessor.Types
{
    public class TotalData
    {
        public ParsedJsonData ParsedJson { get; set; }
        public string Json { get; set; }

        public bool HasGpsRoute { get; set; }

        public DateTime StartTime { get; set; }
        public SportsTypeIds SportTypeId { get; set; }
        public int TimeZone { get; set; }
    }
}
