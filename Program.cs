using HtmlAgilityPack;
using CsvHelper;
using System.Globalization;
using Microsoft.Data.Sqlite;

namespace SimpleWebScraper
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // creating the HAP object
            HtmlWeb web = new HtmlWeb();

            // creating lists for saving scraped data
            List<string> AvgTempDataList = new List<string>();
            List<string> MinTempDataList = new List<string>();
            List<string> MaxTempDataList = new List<string>();
            List<string> PrecipitationDataList = new List<string>();
            List<string> HumidityDataList = new List<string>();
            List<string> RainyDaysCountDataList = new List<string>();
            List<string> SunHrsAvgDataList = new List<string>();

            // defining list for holding row id's to update database rows
            List<int> idList = new List<int>();

            // defining path to database file
            string database = "/Users/lauri/source/soil_data_denmark.sqlite";

            // checking to see if database path is correct
            if (!File.Exists(database))
            {
                Console.WriteLine("Error locating database.");
                System.Environment.Exit(1);
            }

            // defines new sqlite connection
            var connection = new SqliteConnection("Data Source=" + database);

            // opens or catches new connection
            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Database Connection Unsuccessfull.");
                Console.WriteLine(ex.Message);
                System.Environment.Exit(1);
            }

            // reads and saves id's from adress data to runtime list
            try
            {
                var command = connection.CreateCommand();
                command.CommandText =
                    @"
                        SELECT wineyard_id
                        FROM address_data_denmark
                    ";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var id = reader.GetInt32(0);

                        idList.Add(id);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading data.");
                Console.WriteLine(ex.Message);
                System.Environment.Exit(1);
            }

            // looping through URL list to the target web page
            for (int UrlIndex = 0; UrlIndex < DataURLs.Count; UrlIndex++)
            {
                var html = $"{DataURLs[UrlIndex]}";
                var htmlDoc = web.Load(html);

                // selecting node for naming geographical location
                var nameNode = htmlDoc.DocumentNode.SelectSingleNode($"/html/body/div[1]/div[1]/div[1]/div/div[4]/div/ol/li[7]/span/span");
                string areaName = nameNode.InnerText;


                // selecting nodes and extracting text for AvgTemp
                for (int month = 1; month <= 12; month++)
                {
                    var AvgTempNode = htmlDoc.DocumentNode.SelectSingleNode($"/html/body/div[1]/div[1]/div[2]/div/div/div[1]/div/div/article/div/section[3]/table[2]/tbody/tr[1]/td[{month + 1}]/p[1]");

                    AvgTempDataList.Add(AvgTempNode.InnerText);
                }

                // extracting nodes for MinTemp
                for (int month = 1; month <= 12; month++)
                {
                    var MinTempNode = htmlDoc.DocumentNode.SelectSingleNode($"/html/body/div[1]/div[1]/div[2]/div/div/div[1]/div/div/article/div/section[3]/table[2]/tbody/tr[2]/td[{month + 1}]/p[1]");

                    MinTempDataList.Add(MinTempNode.InnerText);
                }

                // extracting nodes for MaxTemp
                for (int month = 1; month <= 12; month++)
                {
                    var MaxTempNode = htmlDoc.DocumentNode.SelectSingleNode($"/html/body/div[1]/div[1]/div[2]/div/div/div[1]/div/div/article/div/section[3]/table[2]/tbody/tr[3]/td[{month + 1}]/p[1]");

                    MaxTempDataList.Add(MaxTempNode.InnerText);
                }

                // extracting nodes for Precipitation
                for (int month = 1; month <= 12; month++)
                {
                    var PrecipitationTempNode = htmlDoc.DocumentNode.SelectSingleNode($"/html/body/div[1]/div[1]/div[2]/div/div/div[1]/div/div/article/div/section[3]/table[2]/tbody/tr[4]/td[{month + 1}]/p[1]");

                    PrecipitationDataList.Add(PrecipitationTempNode.InnerText);
                }

                // extracting nodes for Humidity
                for (int month = 1; month <= 12; month++)
                {
                    var HumidityTempNode = htmlDoc.DocumentNode.SelectSingleNode($"/html/body/div[1]/div[1]/div[2]/div/div/div[1]/div/div/article/div/section[3]/table[2]/tbody/tr[5]/td[{month + 1}]");

                    HumidityDataList.Add(HumidityTempNode.InnerText);
                }

                // extracting nodes for RainyDays
                for (int month = 1; month <= 12; month++)
                {
                    var RainyDaysCountTempNode = htmlDoc.DocumentNode.SelectSingleNode($"/html/body/div[1]/div[1]/div[2]/div/div/div[1]/div/div/article/div/section[3]/table[2]/tbody/tr[6]/td[{month + 1}]");

                    RainyDaysCountDataList.Add(RainyDaysCountTempNode.InnerText);
                }

                // extracting nodes for SunHours
                for (int month = 1; month <= 12; month++)
                {
                    var SunHrsAvgTempNode = htmlDoc.DocumentNode.SelectSingleNode($"/html/body/div[1]/div[1]/div[2]/div/div/div[1]/div/div/article/div/section[3]/table[2]/tbody/tr[7]/td[{month + 1}]");

                    SunHrsAvgDataList.Add(SunHrsAvgTempNode.InnerText);
                }

                //TODO update to write newly scraped data to correct database
                try
                {
                    var command = connection.CreateCommand();

                    for (int listIndex = 0; listIndex < AvgSunHoursMonthsList.Count; listIndex++)
                    {
                        command.CommandText =
                            @$"
                                INSERT INTO climate_data_denmark ({AvgTempMonthsList[listIndex]}, {MinTempMonthsList[listIndex]}, {MaxTempMonthsList[listIndex]}, {PrecipitationMonthsList[listIndex]}, {HumidityMonthsList[listIndex]}, {RainyDaysMonthsList[listIndex]}, {AvgSunHoursMonthsList[listIndex]})
                                VALUES
                                ({AvgTempDataList[listIndex]}, {MinTempDataList[listIndex]}, {MaxTempDataList[listIndex]}, {PrecipitationDataList[listIndex]}, {HumidityDataList[listIndex]}, {RainyDaysCountDataList[listIndex]}, {AvgSunHoursMonthsList[listIndex]})
                            ";

                        /*using (var cmd = new SqliteCommand(command.CommandText, connection))
                        {
                            cmd.Parameters.AddWithValue(AvgTempMonthsList[listIndex], AvgTemp[listIndex]);
                            cmd.Parameters.AddWithValue(MinTempMonthsList[listIndex], AvgTemp[listIndex]);
                            cmd.Parameters.AddWithValue(MaxTempMonthsList[listIndex], AvgTemp[listIndex]);
                            cmd.Parameters.AddWithValue(PrecipitationMonthsList[listIndex], AvgTemp[listIndex]);
                            cmd.Parameters.AddWithValue(HumidityMonthsList[listIndex], AvgTemp[listIndex]);
                            cmd.Parameters.AddWithValue(RainyDaysMonthsList[listIndex], AvgTemp[listIndex]);
                            cmd.Parameters.AddWithValue(AvgSunHoursMonthsList[listIndex], AvgTemp[listIndex]);

                            cmd.ExecuteNonQuery();
                        }*/
                        command.ExecuteNonQuery();
                    }
                    Console.WriteLine("Data inserted successfully");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error inserting data.");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.HelpLink);
                    System.Environment.Exit(1);
                }
            }
            connection.Close();
        }

        public static List<string> AvgTempMonthsList = new List<string>()
        {
            "jan_avg_temp_c = $jan_avg_temp_c",
            "feb_avg_temp_c = $feb_avg_temp_c",
            "mar_avg_temp_c = $mar_avg_temp_c",
            "apr_avg_temp_c = $apr_avg_temp_c",
            "may_avg_temp_c = $may_avg_temp_c",
            "jun_avg_temp_c = $jun_avg_temp_c",
            "jul_avg_temp_c = $jul_avg_temp_c",
            "aug_avg_temp_c = $aug_avg_temp_c",
            "sep_avg_temp_c = $sep_avg_temp_c",
            "oct_avg_temp_c = $oct_avg_temp_c",
            "nov_avg_temp_c = $nov_avg_temp_c",
            "dec_avg_temp_c = $dec_avg_temp_c",
        };

        public static List<string> MinTempMonthsList = new List<string>()
        {
            "jan_min_temp_c = $jan_min_temp_c",
            "feb_min_temp_c = $feb_min_temp_c",
            "mar_min_temp_c = $mar_min_temp_c",
            "apr_min_temp_c = $apr_min_temp_c",
            "may_min_temp_c = $may_min_temp_c",
            "jun_min_temp_c = $jun_min_temp_c",
            "jul_min_temp_c = $jul_min_temp_c",
            "aug_min_temp_c = $aug_min_temp_c",
            "sep_min_temp_c = $sep_min_temp_c",
            "oct_min_temp_c = $oct_min_temp_c",
            "nov_min_temp_c = $nov_min_temp_c",
            "dec_min_temp_c = $dec_min_temp_c",
        };

        public static List<string> MaxTempMonthsList = new List<string>()
        {
            "jan_max_temp_c = $jan_max_temp_c",
            "feb_max_temp_c = $feb_max_temp_c",
            "mar_max_temp_c = $mar_max_temp_c",
            "apr_max_temp_c = $apr_max_temp_c",
            "may_max_temp_c = $may_max_temp_c",
            "jun_max_temp_c = $jun_max_temp_c",
            "jul_max_temp_c = $jul_max_temp_c",
            "aug_max_temp_c = $aug_max_temp_c",
            "sep_max_temp_c = $sep_max_temp_c",
            "oct_max_temp_c = $oct_max_temp_c",
            "nov_max_temp_c = $nov_max_temp_c",
            "dec_max_temp_c = $dec_max_temp_c",
        };

        public static List<string> PrecipitationMonthsList = new List<string>()
        {
            "jan_precipitation_mm = $jan_precipitation_mm",
            "feb_precipitation_mm = $feb_precipitation_mm",
            "mar_precipitation_mm = $mar_precipitation_mm",
            "apr_precipitation_mm = $apr_precipitation_mm",
            "may_precipitation_mm = $may_precipitation_mm",
            "jun_precipitation_mm = $jun_precipitation_mm",
            "jul_precipitation_mm = $jul_precipitation_mm",
            "aug_precipitation_mm = $aug_precipitation_mm",
            "sep_precipitation_mm = $sep_precipitation_mm",
            "oct_precipitation_mm = $oct_precipitation_mm",
            "nov_precipitation_mm = $nov_precipitation_mm",
            "dec_precipitation_mm = $dec_precipitation_mm",
        };

        public static List<string> HumidityMonthsList = new List<string>()
        {
            "jan_humidity_percent = $jan_humidity_percent",
            "feb_humidity_percent = $feb_humidity_percent",
            "mar_humidity_percent = $mar_humidity_percent",
            "apr_humidity_percent = $apr_humidity_percent",
            "may_humidity_percent = $may_humidity_percent",
            "jun_humidity_percent = $jun_humidity_percent",
            "jul_humidity_percent = $jul_humidity_percent",
            "aug_humidity_percent = $aug_humidity_percent",
            "sep_humidity_percent = $sep_humidity_percent",
            "oct_humidity_percent = $oct_humidity_percent",
            "nov_humidity_percent = $nov_humidity_percent",
            "dec_humidity_percent = $dec_humidity_percent",
        };

        public static List<string> RainyDaysMonthsList = new List<string>()
        {
            "jan_rainy_days = $jan_rainy_days",
            "feb_rainy_days = $feb_rainy_days",
            "mar_rainy_days = $mar_rainy_days",
            "apr_rainy_days = $apr_rainy_days",
            "may_rainy_days = $may_rainy_days",
            "jun_rainy_days = $jun_rainy_days",
            "jul_rainy_days = $jul_rainy_days",
            "aug_rainy_days = $aug_rainy_days",
            "sep_rainy_days = $sep_rainy_days",
            "oct_rainy_days = $oct_rainy_days",
            "nov_rainy_days = $nov_rainy_days",
            "dec_rainy_days = $dec_rainy_days",
        };

        public static List<string> AvgSunHoursMonthsList = new List<string>()
        {
            "jan_avg_sun_hours = $jan_avg_sun_hours",
            "feb_avg_sun_hours = $feb_avg_sun_hours",
            "mar_avg_sun_hours = $mar_avg_sun_hours",
            "apr_avg_sun_hours = $apr_avg_sun_hours",
            "may_avg_sun_hours = $may_avg_sun_hours",
            "jun_avg_sun_hours = $jun_avg_sun_hours",
            "jul_avg_sun_hours = $jul_avg_sun_hours",
            "aug_avg_sun_hours = $aug_avg_sun_hours",
            "sep_avg_sun_hours = $sep_avg_sun_hours",
            "oct_avg_sun_hours = $oct_avg_sun_hours",
            "nov_avg_sun_hours = $nov_avg_sun_hours",
            "dec_avg_sun_hours = $dec_avg_sun_hours",
        };

        public static List<string> DataURLs = new List<string>()
        {
            "https://en.climate-data.org/europe/denmark/north-denmark-region/godthab-436465/",
            "https://en.climate-data.org/europe/denmark/north-denmark-region/godthab-436465/",
            "https://en.climate-data.org/europe/denmark/north-denmark-region/fr%c3%b8slev-892261/",
            "https://en.climate-data.org/europe/denmark/north-denmark-region/ranum-860590/",
            "https://en.climate-data.org/europe/denmark/north-denmark-region/ranum-860590/",
            "https://en.climate-data.org/europe/denmark/north-denmark-region/stae-1000028/",
            "https://en.climate-data.org/europe/denmark/central-denmark-region/sunds-861252/",
            "https://en.climate-data.org/europe/denmark/central-denmark-region/kjellerup-695187/",
            "https://en.climate-data.org/europe/denmark/central-denmark-region/hammel-7475/",
            "https://en.climate-data.org/europe/denmark/central-denmark-region/skanderborg-7511/",
            "https://en.climate-data.org/europe/denmark/central-denmark-region/l%c3%b8gten-64515/",
            "https://en.climate-data.org/europe/denmark/central-denmark-region/hammersh%c3%b8j-892259/",
            "https://en.climate-data.org/europe/denmark/central-denmark-region/videb%c3%a6k-861258/",
            "https://en.climate-data.org/europe/denmark/central-denmark-region/r%c3%b8nde-7447/",
            "https://en.climate-data.org/europe/denmark/region-of-southern-denmark/vamdrup-64581/",
            "https://en.climate-data.org/europe/denmark/region-of-southern-denmark/assens-892190/",
            "https://en.climate-data.org/europe/denmark/region-of-southern-denmark/kliplev-184825/",
            "https://en.climate-data.org/europe/denmark/region-of-southern-denmark/skelde-892166/",
            "https://en.climate-data.org/europe/denmark/region-of-southern-denmark/almind-183673/",
            "https://en.climate-data.org/europe/denmark/region-of-southern-denmark/gredstedbro-554330/",
            "https://en.climate-data.org/europe/denmark/region-of-southern-denmark/vilslev-146324/",
            "https://en.climate-data.org/europe/denmark/region-of-southern-denmark/vilslev-146324/",
            "https://en.climate-data.org/europe/denmark/region-of-southern-denmark/gredstedbro-554330/",
            "https://en.climate-data.org/europe/denmark/region-of-southern-denmark/haderslev-7490/",
            "https://en.climate-data.org/europe/denmark/region-of-southern-denmark/oure-223910/",
            "https://en.climate-data.org/europe/denmark/region-of-southern-denmark/vamdrup-64581/",
            "https://en.climate-data.org/europe/denmark/region-of-southern-denmark/%c3%b8rb%c3%a6k-194253/",
            "https://en.climate-data.org/europe/denmark/region-of-southern-denmark/s%c3%b8nder-n%c3%a6ra-896453/",
            "https://en.climate-data.org/europe/denmark/region-of-southern-denmark/ullerslev-64452/",
            "https://en.climate-data.org/europe/denmark/region-of-southern-denmark/haarby-64508/",
            "https://en.climate-data.org/europe/denmark/region-of-southern-denmark/snave-214614/",
            "https://en.climate-data.org/europe/denmark/region-of-southern-denmark/skarup-224016/",
            "https://en.climate-data.org/europe/denmark/region-of-southern-denmark/horne-194174/",
            "https://en.climate-data.org/europe/denmark/region-of-southern-denmark/fr%c3%b8rup-643101/",
            "https://en.climate-data.org/europe/denmark/region-zealand/karreb%c3%a6k-214260/",
            "https://en.climate-data.org/europe/denmark/region-zealand/pr%c3%a6st%c3%b8-64542/",
            "https://en.climate-data.org/europe/denmark/region-zealand/gislinge-119648/",
            "https://en.climate-data.org/europe/denmark/region-zealand/stokkemarke-73088/",
            "https://en.climate-data.org/europe/denmark/region-zealand/sandby-73089/",
            "https://en.climate-data.org/europe/denmark/region-zealand/kalundborg-7523/",
            "https://en.climate-data.org/europe/denmark/region-zealand/holb%c3%a6k-6830/",
            "https://en.climate-data.org/europe/denmark/region-zealand/havns%c3%b8-703113/",
            "https://en.climate-data.org/europe/denmark/region-zealand/r%c3%b8rvig-72840/",
            "https://en.climate-data.org/europe/denmark/region-zealand/kalundborg-7523/",
            "https://en.climate-data.org/europe/denmark/region-zealand/tappern%c3%b8je-313877/",
            "https://en.climate-data.org/europe/denmark/region-zealand/stokkemarke-73088/",
            "https://en.climate-data.org/europe/denmark/region-zealand/lumsas-61409/",
            "https://en.climate-data.org/europe/denmark/region-zealand/fensmark-64543/",
            "https://en.climate-data.org/europe/denmark/capital-region-of-denmark/dalby-509425/",
            "https://en.climate-data.org/europe/denmark/capital-region-of-denmark/helsinge-7450/",
            "https://en.climate-data.org/europe/denmark/capital-region-of-denmark/helsinge-7450/",
            "https://en.climate-data.org/europe/denmark/capital-region-of-denmark/kvistgard-277387/",
            "https://en.climate-data.org/europe/denmark/capital-region-of-denmark/helsinge-7450/",
            "https://en.climate-data.org/europe/denmark/capital-region-of-denmark/hvidovre-29480/",
            "https://en.climate-data.org/europe/denmark/capital-region-of-denmark/slangerup-64493/",
            "https://en.climate-data.org/europe/denmark/capital-region-of-denmark/pedersker-216186/",
        };
    }
}



