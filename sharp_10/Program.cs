using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Data.SqlClient;
using System.Net.Http;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

namespace lab10New
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ChekerCondition();
        }

        static void CreateConditions()
        {
            using (DataBase db = new DataBase())
            {
                var tickers = db.Tickers.ToList();
                int IdConditions = 1;
                foreach (Ticker ticker in tickers)
                {
                    var prices = (from price in db.Prices.Include(p => p.Ticker)
                                  where price.Ticker == ticker
                                  select price).ToList();
                    if (prices[0].Price1 < prices[1].Price1)
                    {
                        var priceConditions = new TodayCondition()
                        {
                            Id = IdConditions++,
                            TickerId = ticker.Id,
                            Ticker = ticker,
                            State = "down"
                        };

                        db.TodayConditions.Add(priceConditions);
                    }
                    else if (prices[0].Price1 > prices[1].Price1)
                    {
                        var priceConditions = new TodayCondition()
                        {
                            Id = IdConditions++,
                            TickerId = ticker.Id,
                            Ticker = ticker,
                            State = "up"
                        };

                        db.TodayConditions.Add(priceConditions);
                    }
                    else
                    {
                        var priceConditions = new TodayCondition()
                        {
                            Id = IdConditions++,
                            TickerId = ticker.Id,
                            Ticker = ticker,
                            State = "not change"
                        };

                        db.TodayConditions.Add(priceConditions);
                    }
                }
                db.SaveChanges();
            }
        }
        static void ChekerCondition()
        {
            Console.WriteLine("input ticker");
            string tickerName = Console.ReadLine();
            while (tickerName != "exit")
            {
                using (DataBase db = new DataBase())
                {
                    var users = (from price in db.Prices.Include(p => p.Ticker)
                                 where price.Ticker.Ticker1 == tickerName
                                 select price).ToList();
                    var condition = (from con in db.TodayConditions.Include(p => p.Ticker)
                                     where con.Ticker.Ticker1 == tickerName
                                     select con).ToList();
                    foreach (var us in users)
                        Console.WriteLine($"{us.Price1}");
                    if (users[1].Price1 - users[0].Price1 > 0)
                    {
                        Console.WriteLine($"The price has risen");
                    }
                    if (users[1].Price1 - users[0].Price1 < 0)
                    {
                        Console.WriteLine($"The price has decreased");
                    }
                    else
                    {
                        Console.WriteLine($"The price has not changed");
                    }


                }
                Console.WriteLine("\ninput ticker or exit");
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
                        Console.WriteLine(line);
                        if (line == null)
                        {
                            break;
                        }
                        var client = new HttpClient();
                        var dateNow = (int)(DateTime.Now - new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds - 86400;
                        var lastDay = dateNow - 86400;
                        try
                        {
                            var content = await client.GetStringAsync($"https://query1.finance.yahoo.com/v7/finance/download/{line}?period1={lastDay}&period2={dateNow}&interval=1d&events=history&includeAdjustedClose=true");
                            string[] lines = content.Split('\n');
                            List<double> listAvNum = new List<double>();
                            double summa = 0;
                            for (int i = 1; i < lines.Length; ++i)
                            {
                                listAvNum.Add(GetAverageNum(lines[i]));
                            }
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
                            Console.WriteLine($"Correct for {line}");
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

        static void Reader()
        {
            using (DataBase db = new DataBase())
            {
                // получаем объект
                var tickers = db.Tickers.ToList();
                foreach (Ticker ticker in tickers)
                {
                    Console.WriteLine($"{ticker.Id}.{ticker.Ticker1}");
                }
            }
        }

        static float GetAverageNum(string data)
        {
            double High = 0;
            double Low = 0;
            int countOfDel = 0;
            string temp = "";
            for (int i = 0; i < data.Length; ++i)
            {
                if (data[i] == ',')
                {
                    ++countOfDel;
                    if (countOfDel == 3)
                    {
                        if (temp == "null")
                        {
                            High = 0;
                        }
                        else
                        {
                            temp = temp.Replace('.', ',');
                            High = Convert.ToDouble(temp);
                        }
                    }
                    if (countOfDel == 4)
                    {
                        if (temp == "null")
                        {
                            Low = 0;
                        }
                        else
                        {
                            temp = temp.Replace('.', ',');
                            Low = Convert.ToDouble(temp);
                        }
                    }
                    temp = "";
                }
                else
                {
                    temp += data[i];
                }
            }
            return (float)((High + Low) / 2);
        }
        static int DeleteProducts(string name)
        {
            using (var db = new DataBase())
            {
                IEnumerable<Ticker> products = db.Tickers.Where(p => p.Ticker1.StartsWith(name));

                db.Tickers.RemoveRange(products);
                return db.SaveChanges();
            }
        }
        static int DeletePrice(string name)
        {
            using (var db = new DataBase())
            {
                IEnumerable<Price> products = db.Prices.Where(p => p.Ticker.Ticker1.StartsWith(name));

                db.Prices.RemoveRange(products);
                return db.SaveChanges();
            }
        }

        static bool AddPrice(DataBase db, int tickerID, string ticker, float price, DateTime dateTime)
        {
            var newTicker = new Ticker()
            {
                Id = tickerID,
                Ticker1 = ticker
            };

            var priceForTicker = new Price()
            {
                Id = tickerID,
                Ticker = newTicker,
                TickerId = newTicker.Id,
                Price1 = price,
                Date = dateTime
            };

            db.Prices.Add(priceForTicker);
            int affected = db.SaveChanges();
            return (affected == 1);
        }
        static bool AddTicker(DataBase db, int tickerID, string ticker)
        {
            var newTicker = new Ticker()
            {
                Id = tickerID,
                Ticker1 = ticker
            };

            db.Tickers.Add(newTicker);
            int affected = db.SaveChanges();
            return (affected == 1);
        }
        static void DataLoadingSchemasLazy()
        {
            using (var db = new DataBase())
            {
                var loggerFactory = db.GetService<ILoggerFactory>();
                loggerFactory.AddProvider(new ConsoleLoggerProvider());

                IQueryable<Price> prices = db.Prices;

                foreach (var c in prices)
                {
                    Console.WriteLine($"{c.Ticker} has {c.Price1}$");
                }
            }
        }
    }
}