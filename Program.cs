using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using RuntasticExportProcessor.Enums;
using RuntasticExportProcessor.Types;

namespace RuntasticExportProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            //var rootPath = args.FirstOrDefault();

            //if (string.IsNullOrWhiteSpace(rootPath))
            //{
            //    Console.Write("Provide a parameter with the path to the folder with exported data:");
            //    rootPath = Console.ReadLine();
            //}

            var rootPath = @"C:\Users\Anton\OneDrive\runtastic\runtastic export 2020 05 31";

            var sportSessionsFolderPath = Path.Combine(rootPath, "Sport-sessions");

            var gpsDataFolderPath = Path.Combine(sportSessionsFolderPath, "GPS-data");

            var sportSessionFiles = Directory.GetFiles(sportSessionsFolderPath, "*.json");

            var list = new List<TotalData>();

            foreach (var sportSessionFile in sportSessionFiles)
            {
                var rawJson = File.ReadAllText(sportSessionFile);

                var parsed = JsonConvert.DeserializeObject<ParsedJsonData>(rawJson);

                var sportSessionFilename = Path.GetFileNameWithoutExtension(sportSessionFile);

                var gpsDataFilename = sportSessionFilename + ".gpx";

                var gpsDataFile = Path.Combine(gpsDataFolderPath, gpsDataFilename);

                var hasGpsRoute = File.Exists(gpsDataFile);

                var sportTypeId = Enum.Parse<SportsTypeIds>(parsed.sport_type_id);
                var startTime = UnixTimeStampToDateTime(parsed.start_time / 1000);
                var timeZone = parsed.start_time_timezone_offset / 60 / 60 / 1000;

                var total = new TotalData
                {
                    ParsedJson = parsed,
                    Json = rawJson,
                    HasGpsRoute = hasGpsRoute,
                    SportTypeId = sportTypeId,
                    StartTime = startTime,
                    TimeZone = (int)timeZone,
                };


                list.Add(total);
            }

            var consoleColor = Console.ForegroundColor;

            foreach (var item in list.OrderByDescending(x => x.StartTime))
            {
                if (item.HasGpsRoute)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }

                Console.WriteLine($"{nameof(item.SportTypeId)}: {item.SportTypeId} ({item.ParsedJson.sport_type_id})");
                Console.WriteLine($"{nameof(item.StartTime)}: {item.StartTime} ({item.ParsedJson.start_time})");
                Console.WriteLine($"{nameof(item.TimeZone)}: {item.TimeZone } ({item.ParsedJson.start_time_timezone_offset})");
                Console.WriteLine($"HasGpsRoute: {item.HasGpsRoute}");

                Console.WriteLine();
            }

            Console.ForegroundColor = consoleColor;
        }

        private static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}