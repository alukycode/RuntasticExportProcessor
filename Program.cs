using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using RuntasticExportProcessor.Enums;
using RuntasticExportProcessor.Types;

namespace RuntasticExportProcessor
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var folderPaths = GetFolderPaths(args);

            var list = ReadDataFromFiles(folderPaths);

            OutputDataToConsole(list);

            CreateResultFiles(list, folderPaths.Result);

            Console.WriteLine("Done!");
            Console.ReadLine();
        }

        private static FolderPathsData GetFolderPaths(string[] args)
        {
            var rootPath = args.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(rootPath))
            {
                Console.Write("Enter the path to the folder with exported data (leave empty to use current folder): ");
                rootPath = Console.ReadLine();
            }

            if (string.IsNullOrWhiteSpace(rootPath))
            {
                rootPath = Environment.CurrentDirectory;
            }

            Console.WriteLine($"Root folder path: {rootPath}");

            var sportSessionsFolderPath = Path.Combine(rootPath, "Sport-sessions");
            var gpsDataFolderPath = Path.Combine(sportSessionsFolderPath, "GPS-data");

            var isGpsDataFolderExists = Directory.Exists(gpsDataFolderPath);
            if (!isGpsDataFolderExists)
            {
                throw new ApplicationException("'Sport-sessions\\GPS-data' folder is not found.");
            }

            var resultFolderPath = Path.Combine(rootPath, $"_Processed {DateTime.Now:yyyy-MM-dd HH-mm-ss}");

            Directory.CreateDirectory(resultFolderPath);

            Console.WriteLine($"Result folder path: {resultFolderPath}");

            Console.WriteLine();

            return new FolderPathsData
            {
                Root = rootPath,
                SportSessions = sportSessionsFolderPath,
                GpsData = gpsDataFolderPath,
                Result = resultFolderPath,
            };
        }

        private static List<SportSessionData> ReadDataFromFiles(FolderPathsData folderPaths)
        {
            var sportSessionFilePaths = Directory.GetFiles(folderPaths.SportSessions, "*.json");

            var results = new List<SportSessionData>();

            foreach (var sportSessionFilePath in sportSessionFilePaths)
            {
                var rawJson = File.ReadAllText(sportSessionFilePath);

                var parsedJson = JsonConvert.DeserializeObject<ParsedJsonData>(rawJson);

                var sportSessionFileName = Path.GetFileName(sportSessionFilePath);
                var gpsDataFileName = Path.ChangeExtension(sportSessionFileName, ".gpx");
                var gpsDataFilePath = Path.Combine(folderPaths.GpsData, gpsDataFileName);
                var hasGpsRoute = File.Exists(gpsDataFilePath);

                var sportTypeId = Enum.Parse<ActivityTypeId>(parsedJson.sport_type_id);
                var startTime = UnixTimeStampToDateTime(parsedJson.start_time / 1000.0);
                var timeZone = parsedJson.start_time_timezone_offset / 60 / 60 / 1000;

                var session = new SportSessionData
                {
                    ParsedJson = parsedJson,
                    Json = rawJson,
                    GpsDataFilePath = gpsDataFilePath,
                    HasGpsData = hasGpsRoute,

                    ActivityTypeId = sportTypeId,
                    StartTime = startTime,
                    TimeZone = (int) timeZone,
                };

                results.Add(session);
            }

            return results;
        }

        private static void OutputDataToConsole(List<SportSessionData> list)
        {
            foreach (var item in list.OrderByDescending(x => x.StartTime))
            {
                var gpsMessage = string.Empty;

                if (!item.HasGpsData)
                {
                    gpsMessage = "(no GPS)";
                }

                Console.WriteLine($"{item.StartTime:yyyy-MM-dd HH:mm} {item.TimeZone:+0;-#}: {item.ActivityTypeId} {gpsMessage}");
            }

            Console.WriteLine();
        }

        private static void CreateResultFiles(List<SportSessionData> list, string resultFolderPath)
        {
            // list = dictionary[sportType][year]
            var dictionary = list
                .GroupBy(x => x.ActivityTypeId)
                .ToDictionary(
                    g => g.Key,
                    g => g
                        .GroupBy(x => x.StartTime.Year)
                        .ToDictionary(
                            g2 => g2.Key,
                            g2 => g2.OrderBy(x => x.StartTime).ToList()));


            foreach (var sportType in dictionary.Keys)
            {
                foreach (var year in dictionary[sportType].Keys)
                {
                    var items = dictionary[sportType][year];

                    if (items.All(x => x.HasGpsData))
                    {
                        var resultFileContentBuilder = new StringBuilder();

                        foreach (var item in items)
                        {
                            var gpsDataFileContent = File.ReadAllText(item.GpsDataFilePath);
                            resultFileContentBuilder.Append(gpsDataFileContent);
                        }

                        var resultFileContent = Regex.Replace(resultFileContentBuilder.ToString(), "</trk>(.|\r|\n)+?<trk>", "</trk>\r\n<trk>");

                        var resultFileName = $"{year} {sportType}.gpx";

                        var resultPath = Path.Combine(resultFolderPath, resultFileName);

                        File.WriteAllText(resultPath, resultFileContent);

                        Console.WriteLine($"File created: {resultFileName}");
                    }
                    else if (items.All(x => !x.HasGpsData))
                    {
                        continue;
                    }
                    else
                    {
                        throw new Exception($"{sportType} sometimes does have GPS data and sometimes doesn't, wtf?");
                    }
                }
            }

            Console.WriteLine();
        }

        private static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            var epochStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var result = epochStart.AddSeconds(unixTimeStamp).ToLocalTime();
            return result;
        }
    }
}