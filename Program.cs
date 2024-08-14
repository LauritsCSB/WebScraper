﻿using HtmlAgilityPack;
using CsvHelper;
using System.Globalization;

namespace SimpleWebScraper
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // creating the HAP object
            HtmlWeb web = new HtmlWeb();

            // looping through URL list to the target web page
            for (int UrlIndex = 0; UrlIndex < DataURLs.Count; UrlIndex++)
            {
                var html = $"{DataURLs[UrlIndex]}";
                var htmlDoc = web.Load(html);

                // selecting node for naming geographical location
                var nameNode = htmlDoc.DocumentNode.SelectSingleNode($"/html/body/div[1]/div[1]/div[1]/div/div[4]/div/ol/li[7]/span/span");

                // creating lists for saving scraped data
                string areaName = nameNode.InnerText;
                List<string> AvgTemp = new List<string>();
                List<string> MinTemp = new List<string>();
                List<string> MaxTemp = new List<string>();
                List<string> Precipitation = new List<string>();
                List<string> Humidity = new List<string>();
                List<string> RainyDaysCount = new List<string>();
                List<string> AvgSunHrs = new List<string>();

                // selecting nodes and extracting text for AvgTemp
                for (int month = 1; month <= 12; month++)
                {
                    var AvgTempNode = htmlDoc.DocumentNode.SelectSingleNode($"/html/body/div[1]/div[1]/div[2]/div/div/div[1]/div/div/article/div/section[3]/table[2]/tbody/tr[1]/td[{month+1}]/p[1]");

                    AvgTemp.Add(AvgTempNode.InnerText);
                }

                // TODO: select correct node
                for (int month = 1; month <= 12; month++)
                {
                    var MinTempNode = htmlDoc.DocumentNode.SelectSingleNode($"/html/body/div[1]/div[1]/div[2]/div/div/div[1]/div/div/article/div/section[3]/table[2]/tbody/tr[1]/td[{month + 1}]/p[1]");

                    MinTemp.Add(MinTempNode.InnerText);
                }

                // TODO: select correct node
                for (int month = 1; month <= 12; month++)
                {
                    var MinTempNode = htmlDoc.DocumentNode.SelectSingleNode($"/html/body/div[1]/div[1]/div[2]/div/div/div[1]/div/div/article/div/section[3]/table[2]/tbody/tr[1]/td[{month + 1}]/p[1]");

                    MinTemp.Add(MinTempNode.InnerText);
                }

                // TODO: select correct node
                for (int month = 1; month <= 12; month++)
                {
                    var MinTempNode = htmlDoc.DocumentNode.SelectSingleNode($"/html/body/div[1]/div[1]/div[2]/div/div/div[1]/div/div/article/div/section[3]/table[2]/tbody/tr[1]/td[{month + 1}]/p[1]");

                    MinTemp.Add(MinTempNode.InnerText);
                }

                // TODO: select correct node
                for (int month = 1; month <= 12; month++)
                {
                    var MinTempNode = htmlDoc.DocumentNode.SelectSingleNode($"/html/body/div[1]/div[1]/div[2]/div/div/div[1]/div/div/article/div/section[3]/table[2]/tbody/tr[1]/td[{month + 1}]/p[1]");

                    MinTemp.Add(MinTempNode.InnerText);
                }

                // TODO: select correct node
                for (int month = 1; month <= 12; month++)
                {
                    var MinTempNode = htmlDoc.DocumentNode.SelectSingleNode($"/html/body/div[1]/div[1]/div[2]/div/div/div[1]/div/div/article/div/section[3]/table[2]/tbody/tr[1]/td[{month + 1}]/p[1]");

                    MinTemp.Add(MinTempNode.InnerText);
                }

                // TODO: select correct node
                for (int month = 1; month <= 12; month++)
                {
                    var MinTempNode = htmlDoc.DocumentNode.SelectSingleNode($"/html/body/div[1]/div[1]/div[2]/div/div/div[1]/div/div/article/div/section[3]/table[2]/tbody/tr[1]/td[{month + 1}]/p[1]");

                    MinTemp.Add(MinTempNode.InnerText);
                }
            }


            /*// iterating over the list of product HTML elements
            foreach (var productElement in productHTMLElements)
            {
                // scraping logic
                var url = HtmlEntity.DeEntitize(productElement.QuerySelector("a").Attributes["href"].Value);
                var image = HtmlEntity.DeEntitize(productElement.QuerySelector("img").Attributes["src"].Value);
                var name = HtmlEntity.DeEntitize(productElement.QuerySelector("h2").InnerText);
                var price = HtmlEntity.DeEntitize(productElement.QuerySelector(".price").InnerText);

                //instancing a new product object
                var product = new Product()
                {
                    Url = url,
                    Image = image,
                    Name = name,
                    Price = price
                };

                // adding the object containing the scraped data to the list
                products.Add(product);
            }

            // creating the CSV output file
            using (var writer = new StreamWriter("products.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                // populating the csv file
                csv.WriteRecords(products);
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



