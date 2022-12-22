using System.Net.Sockets;
using System.Text;
using System.Net;
using System.Collections.Generic;
using lab10New;
using Microsoft.EntityFrameworkCore;

var server = new TcpListener(IPAddress.Any, 1433);

try
{
    server.Start();
    Console.WriteLine("server is ready...");

    while (true)
    {
        using var tcpClient = await server.AcceptTcpClientAsync();
        var stream = tcpClient.GetStream();
        int bytesRead = 0;
        while (true)
        {
            var data = new List<byte>();
            while ((bytesRead = stream.ReadByte()) != -1)
            {
                data.Add(Convert.ToByte(bytesRead));
            }
            var word = Encoding.UTF8.GetString(data.ToArray());
            float priceToday = 0;
            if (word == "exit")
            {
                break;
            }

            Console.WriteLine($"Ticker is {word}");

            using (DataBase db = new DataBase())
            {
                var tickers = db.Tickers.ToList();
                var prices = (from price in db.Prices.Include(p => p.Ticker)
                              where price.Ticker.Ticker1 == word
                              orderby price
                              select price).ToList();

                priceToday = prices[0].Price1;
            }

            byte[] buffer = Encoding.UTF8.GetBytes(Convert.ToString(priceToday) + '\n');
            await stream.WriteAsync(buffer, 0, buffer.Length);
        }
    }
}
finally
{
    server.Stop();
}