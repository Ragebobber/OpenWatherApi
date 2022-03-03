using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Collections.Generic;

namespace OpenWatherApi
{
    class Program
    {
        public class City
        {
            public string Name { get; set; }
        }
       public class WatherInfo
        {
            public List<Days> daily { get; set; }
        }

        public class Temp
        {         
            public double day { get; set; }
            public double min { get; set; }
            public double max { get; set; }
            public double night { get; set; }
        }
        public class Feels_like
        {
            public double day { get; set; }
            public double night { get; set; }
        }
        public class Days
        {         
            public long dt { get; set; } //Время прогнозируемых данных, Unix, UTC
            public long sunrise { get; set; } // Время восхода солнца, Unix, UTC
            public long sunset { get; set; } // Время заката, Unix, UTC
            public long moonrise { get; set; } // Время восхода луны в этот день, Unix, UTC
            public long moonset { get; set; } // Время захода луны для этого дня, Unix, UTC
            public Temp temp { get; set; }
            public Feels_like feels_like { get; set; }

        }

        static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }

        static void Main(string[] args)
        {
            string Key = "";
            float lat = 56.129057f;// Широта 
            float lon = 40.406635f; // Долгота
            string ow_url = "https://api.openweathermap.org/data/2.5/onecall?lat=" + lat + "&lon="+lon + "&units=metric" + "&lang=ru&appid=" + Key;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ow_url);
            HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse();

            string Response = null;

            using (StreamReader streamReader = new StreamReader(webResponse.GetResponseStream()))
            {
                Response = streamReader.ReadToEnd();
            }

            WatherInfo watherJSON = JsonSerializer.Deserialize<WatherInfo>(Response);
            
            double  min = Math.Abs(watherJSON.daily[0].temp.night - watherJSON.daily[0].feels_like.night);
            long    max = watherJSON.daily[0].sunset - watherJSON.daily[0].sunrise;
            double  abs;
            int     day = 0;
            long Max_long = 0;
            int day_max_long = 0;
           
            for (int i = 1; i < watherJSON.daily.Count; i++)
            {
                abs = Math.Abs(watherJSON.daily[i].temp.night - watherJSON.daily[i].feels_like.night);
                Max_long = watherJSON.daily[i].sunset - watherJSON.daily[i].sunrise;
                if (abs <= min)
                {
                    min = abs;
                    day = i;
                }
                if(Max_long >= max)
                {
                    max = Max_long;
                    day_max_long = i;
                }

            }

            DateTime Sday = ConvertFromUnixTimestamp(watherJSON.daily[day].dt);
            DateTime time = ConvertFromUnixTimestamp(watherJSON.daily[day_max_long].dt);

            DateTime delta = ConvertFromUnixTimestamp(max);

            Console.WriteLine("День с минимальной разницей ощущаемой и фактической температурой ночью " + Sday.ToString("dd.MM.yyyy"));
            Console.WriteLine("Максимальная продолжительность светового " + time.ToString("dd.MM.yyyy") + " Длительностью - " + delta.ToString("hh:mm:ss"));

            Console.ReadKey();
        } 

    }
}
