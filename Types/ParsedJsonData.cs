using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace RuntasticExportProcessor.Types
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class ParsedJsonData
    {
        public long start_time                 { get; set; } // 1564946198000,
        public long end_time                   { get; set; } // 1564951085000,
        public long created_at                 { get; set; } // 1564950930000,
        public long updated_at                 { get; set; } // 1564950974000,
        public long start_time_timezone_offset { get; set; } // 10800000,
        public long end_time_timezone_offset   { get; set; } // 10800000,
        public long distance                   { get; set; } // 10608,
        public long duration                   { get; set; } // 4886118,
        public long elevation_gain             { get; set; } // 75,
        public long elevation_loss             { get; set; } // 89,
        public double average_speed            { get; set; } // 7.816,
        public long calories                   { get; set; } // 289,
        public double longitude                { get; set; } // 27.639856,
        public double latitude                 { get; set; } // 53.872658,
        public double max_speed                { get; set; } // 25.833298,
        public long pause_duration             { get; set; } // 0,
        public double duration_per_km          { get; set; } // 460607,
        public double temperature              { get; set; } // 12.0,
        public long pulse_avg                  { get; set; } // 0,
        public long pulse_max                  { get; set; } // 0,
        public long avg_cadence                { get; set; } // 0,
        public long max_cadence                { get; set; } // 0,
        public bool manual                     { get; set; } // false,
        public bool edited                     { get; set; } // false,
        public bool completed                  { get; set; } // true,
        public bool live_tracking_active       { get; set; } // false,
        public bool live_tracking_enabled      { get; set; } // false,
        public bool cheering_enabled           { get; set; } // false,
        public bool indoor                     { get; set; } // false,
        public string id                       { get; set; } // "2ee4c55b-7656-4a1d-839e-b1a54eec0459"
        public string weather_condition_id     { get; set; } // "5",
        public string surface_id               { get; set; } // "1",
        public string subjective_feeling_id    { get; set; } // "5",
        public string sport_type_id            { get; set; } // "3"
    }
}
