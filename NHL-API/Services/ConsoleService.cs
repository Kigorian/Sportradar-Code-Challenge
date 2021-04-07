﻿using Newtonsoft.Json.Linq;
using NHL_API.Resources.Enums;
using System;
using System.IO;

namespace NHL_API.Services
{
    public class ConsoleService
    {
        /// <summary>
        /// Asks the user for the pipeline type using the console.
        /// </summary>
        /// <returns></returns>
        public static PipelineType GetPipelineType()
        {
            while (true)
            {
                Console.WriteLine("Would you like to query for Team or Player results?");
                var pipelineTypeChoice = Console.ReadLine();

                switch (pipelineTypeChoice.ToLower().Trim().TrimEnd('s'))
                {
                    case "team":
                        return PipelineType.Team;

                    case "player":
                        return PipelineType.Player;

                    default:
                        Console.WriteLine();
                        WriteLineInColor(
                            $"\"{pipelineTypeChoice}\" is not a valid option.",
                            ConsoleColor.Red
                        );
                        break;
                }
            }
        }

        public static int GetEntityId(string pipelineTypeDescription)
        {
            int id;

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine(
                    $"Please enter the ID for the {pipelineTypeDescription} " +
                    $"you would like to view."
                );
                var userInput = Console.ReadLine();

                if (int.TryParse(userInput, out id) && id > 0)
                {
                    break;
                }
                else
                {
                    Console.WriteLine();
                    WriteLineInColor(
                        $"\"{userInput}\" is not a valid option. " +
                        $"Please enter the ID as a positive integer.",
                        ConsoleColor.Red
                    );
                }
            }

            return id;
        }

        public static int GetYear()
        {
            // Set some boundaries.
            var minYear = 1900;

            //var currentYear = DateTime.Now.Year;
            var currentSeasonUrl = "https://statsapi.web.nhl.com/api/v1/seasons/current";
            var currentSeasonJson = ApiService.GetJsonResponse(currentSeasonUrl);
            var currentSeasonJObject = (JObject)JObject.Parse(currentSeasonJson)
                .SelectToken("seasons[0]");

            var currentSeasonIdString = (string)currentSeasonJObject["seasonId"];
            var currentSeasonStartYearString = currentSeasonIdString
                .Substring(0, currentSeasonIdString.Length / 2);
            var currentSeasonStartYear = int.Parse(currentSeasonStartYearString);

            int year;
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Which year would you like to view?");
                var userInput = Console.ReadLine();

                if (int.TryParse(userInput, out year) && year > minYear && year <= currentSeasonStartYear)
                {
                    break;
                }
                else
                {
                    Console.WriteLine();
                    WriteLineInColor(
                        $"\"{userInput}\" is not a valid option. " +
                        $"Please enter a valid year between {minYear} and {currentSeasonStartYear}",
                        ConsoleColor.Red
                    );
                }
            }

            return year;
        }

        public static string GetFilePath()
        {
            string path;

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Please specify the directory to save the file to.");
                path = Console.ReadLine();

                if (Directory.Exists(path))
                {
                    break;
                }
                else
                {
                    Console.WriteLine();
                    WriteLineInColor(
                        $"No directory found at \"{path}\". " +
                        $"Please enter the path to an existing directory.",
                        ConsoleColor.Red
                    );
                }
            }

            return path;
        }

        public static string GetFileName(string directoryPath)
        {
            string name;

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Choose a name for your file (without the extension).");
                name = Console.ReadLine();

                if (File.Exists($"{directoryPath}/{name}.csv"))
                {
                    Console.WriteLine();
                    Console.WriteLine(
                        "A file with that name already exists. " +
                        "Do you wish to overwrite it? (Y/N)"
                    );
                    var isOverwrite = Console.ReadLine();

                    if (isOverwrite.ToLower() == "y" || isOverwrite.ToLower() == "yes")
                    {
                        break;
                    }
                }
                else if (!string.IsNullOrWhiteSpace(name))
                {
                    break;
                }
            }

            return $"{name}.csv";
        }

        /// <summary>
        /// Calls Console.WriteLine(), but prints in the specified color.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="message"></param>
        public static void WriteLineInColor(string value, ConsoleColor color)
        {
            // Keep track of original color so we can set it again.
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(value);
            Console.ForegroundColor = originalColor;
        }
    }
}
