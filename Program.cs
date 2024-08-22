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
            List<string> AvgTemp = new List<string>();
            List<string> MinTemp = new List<string>();
            List<string> MaxTemp = new List<string>();
            List<string> Precipitation = new List<string>();
            List<string> Humidity = new List<string>();
            List<string> RainyDaysCount = new List<string>();
            List<string> AvgSunHrs = new List<string>();

            // defining list for holding row id's to update database rows
            List<int> idList = new List<int>();

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

                    AvgTemp.Add(AvgTempNode.InnerText);
                }

                // extracting nodes for MinTemp
                for (int month = 1; month <= 12; month++)
                {
                    var MinTempNode = htmlDoc.DocumentNode.SelectSingleNode($"/html/body/div[1]/div[1]/div[2]/div/div/div[1]/div/div/article/div/section[3]/table[2]/tbody/tr[2]/td[{month + 1}]/p[1]");

                    MinTemp.Add(MinTempNode.InnerText);
                }

                // extracting nodes for MaxTemp
                for (int month = 1; month <= 12; month++)
                {
                    var MaxTempNode = htmlDoc.DocumentNode.SelectSingleNode($"/html/body/div[1]/div[1]/div[2]/div/div/div[1]/div/div/article/div/section[3]/table[2]/tbody/tr[3]/td[{month + 1}]/p[1]");

                    MaxTemp.Add(MaxTempNode.InnerText);
                }

                // extracting nodes for Precipitation
                for (int month = 1; month <= 12; month++)
                {
                    var PrecipitationTempNode = htmlDoc.DocumentNode.SelectSingleNode($"/html/body/div[1]/div[1]/div[2]/div/div/div[1]/div/div/article/div/section[3]/table[2]/tbody/tr[4]/td[{month + 1}]/p[1]");

                    Precipitation.Add(PrecipitationTempNode.InnerText);
                }

                // extracting nodes for Humidity
                for (int month = 1; month <= 12; month++)
                {
                    var HumidityTempNode = htmlDoc.DocumentNode.SelectSingleNode($"/html/body/div[1]/div[1]/div[2]/div/div/div[1]/div/div/article/div/section[3]/table[2]/tbody/tr[5]/td[{month + 1}]");

                    Humidity.Add(HumidityTempNode.InnerText);
                }

                // extracting nodes for RainyDays
                for (int month = 1; month <= 12; month++)
                {
                    var RainyDaysCountTempNode = htmlDoc.DocumentNode.SelectSingleNode($"/html/body/div[1]/div[1]/div[2]/div/div/div[1]/div/div/article/div/section[3]/table[2]/tbody/tr[6]/td[{month + 1}]");

                    RainyDaysCount.Add(RainyDaysCountTempNode.InnerText);
                }

                // extracting nodes for RainyDays
                for (int month = 1; month <= 12; month++)
                {
                    var AvgSunHrsTempNode = htmlDoc.DocumentNode.SelectSingleNode($"/html/body/div[1]/div[1]/div[2]/div/div/div[1]/div/div/article/div/section[3]/table[2]/tbody/tr[7]/td[{month + 1}]");

                    AvgSunHrs.Add(AvgSunHrsTempNode.InnerText);
                }

            }

            // defining path to database file
            string database = "/Users/lauri/source/soil_data_denmark.sqlite";

            // checking to see if path is correct
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
            /*
            //TODO update to write newly scraped data to correct database
            try
            {
                var command = connection.CreateCommand();
                command.CommandText =
                    @"
                        UPDATE climate_data_denmark
                        SET street_name = $street_name,
                            house_number = $house_number,
                            city = $city,
                            postal_code = $postal_code
                        WHERE id = $id
                    ";

                for (int listIndex = 0; listIndex < idList.Count; listIndex++)
                {
                    using (var cmd = new SqliteCommand(command.CommandText, connection))
                    {
                        cmd.Parameters.AddWithValue("$street_name", streetAddressList[listIndex]);
                        cmd.Parameters.AddWithValue("$house_number", houseNumberList[listIndex]);
                        cmd.Parameters.AddWithValue("$city", cityList[listIndex]);
                        cmd.Parameters.AddWithValue("$postal_code", postalCodeList[listIndex]);
                        cmd.Parameters.AddWithValue("$id", idList[listIndex]);
                        cmd.ExecuteNonQuery();
                    }
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
            finally
            {
                connection.Close();

            }*/
        }

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



