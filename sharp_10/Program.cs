using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Data.SqlClient;
using System.Net.Http;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace lab10New
{
    class Program
    {
        static async Task Main(string[] args)
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            CreateDb();
            ChekerCondition();
        }

        static void ChekerCondition()
        {
            Console.WriteLine("Ticker");
            string tickerName = Console.ReadLine();
            while (tickerName != "stop")
            {
                using (DataBase db = new DataBase())
                {
                    var unit = (from price in db.Prices.Include(p => p.Ticker)
                                 where price.Ticker.Ticker1 == tickerName
                                 select price).ToList();
                    var condition = (from con in db.TodayConditions.Include(p => p.Ticker)
                                     where con.Ticker.Ticker1 == tickerName
                                     select con).ToList();
                    foreach (var elem in unit)
                        Console.WriteLine($"{elem.Price1}");

                    if (unit[1].Price1 - unit[0].Price1 > 0)
                    {
                        Console.WriteLine($"The price has risen");
                    }
                    if (unit[1].Price1 - unit[0].Price1 < 0)
                    {
                        Console.WriteLine($"The price has decreased");
                    }
                    if (unit[1].Price1 - unit[0].Price1 == 0)
                    {
                        Console.WriteLine($"The price has not changed");
                    }
                }
                Console.WriteLine("\nNew ticker or stop");
                tickerName = Console.ReadLine();
            }
        }

        static async void CreateDb()
        {
            string path = "ticker.txt";
            int IdTicker = 1;
            int IdPrice = 1;
            using (DataBase db = new DataBase())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                using (StreamReader reader = File.OpenText(path))
                {
                    while (true)
                    {
                        string line = reader.ReadLine();
                        if (line == null)
                        {
                            break;
                        }
                        var client = new HttpClient();
                        var nowTime = DateTimeOffset.Now.ToUnixTimeSeconds();
                        var yesterdayTime = nowTime - 4 * 86400;
                        try
                        {
                            var content = await client.GetStringAsync($"https://query1.finance.yahoo.com/v7/finance/download/{line}?period1={yesterdayTime}&period2={nowTime}&interval=1d&events=history&includeAdjustedClose=true");
                            string[] lines = content.Split('\n');

                            Console.WriteLine(line);
                            var newTicker = new Ticker()
                            {
                                Id = IdTicker,
                                Ticker1 = line
                            };
                            ++IdTicker;
                            var priceForTicker1 = new Price()
                            {
                                Id = IdPrice,
                                Ticker = newTicker,
                                TickerId = newTicker.Id,
                                Price1 = GetAverageNum(lines[1]),
                                Date = DateTime.Now
                            };
                            ++IdPrice;
                            var priceForTicker2 = new Price()
                            {
                                Id = IdPrice,
                                Ticker = newTicker,
                                TickerId = newTicker.Id,
                                Price1 = GetAverageNum(lines[2]),
                                Date = DateTime.Now.AddDays(-1)
                            };
                            ++IdPrice;
                            Console.WriteLine($"Check {line}");
                            db.Prices.Add(priceForTicker1);
                            db.Prices.Add(priceForTicker2);
                            db.Tickers.Add(newTicker);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            continue;
                        }
                    }
                    db.SaveChanges();
                }
            }
        }
        static float GetAverageNum(string data)
        {
            double HighPrice = 0;
            double LowPrice = 0;
            string[] arr = new string[20];
            arr = data.Split(',');
            HighPrice = double.Parse(arr[3]);
            LowPrice = double.Parse(arr[4]);
            return (float)((HighPrice + LowPrice) / 2);
        }
    }
}