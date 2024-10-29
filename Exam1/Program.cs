
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace Exam1
{
    public class Program
    {
        private static readonly string apiKey = "9d405423f104efd571556ebbbb2cad71";
        private static readonly string baseUrl = "http://api.openweathermap.org/data/2.5/weather";

        static async Task Main(string[] args)
        {
            Console.Write("Enter city name:");
            string city = Console.ReadLine();

            await GetWeatherJson(city);
            await GetWeatherXml(city);

            Console.ReadLine();
        }

        #region PROCESS JSON
        private static async Task GetWeatherJson(string city)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = $"{baseUrl}?q={city}&appid={apiKey}";
                var response = await client.GetStringAsync(url);

                // Deserialize JSON response
                WeatherResponse weather = JsonConvert.DeserializeObject<WeatherResponse>(response);

                Console.WriteLine("\nJSON Response:");
                Console.WriteLine($"City: {weather.Name}");
                Console.WriteLine($"Temperature: {KelvinToCelsius(weather.Main.Temp)} C");
                Console.WriteLine($"Weather: {weather.Weather[0].Description}");
            }
        }
        #endregion

        #region PROCESS XML
        private static async Task GetWeatherXml(string city)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = $"{baseUrl}?q={city}&appid={apiKey}&mode=xml";
                var response = await client.GetStringAsync(url);

                // Deserialize XML response
                XmlSerializer serializer = new XmlSerializer(typeof(XmlWeatherResponse));
                using (StringReader reader = new StringReader(response))
                {
                    XmlWeatherResponse weather = (XmlWeatherResponse)serializer.Deserialize(reader);

                    Console.WriteLine("\nXML Response:");
                    Console.WriteLine($"City: {weather.City.Name}");
                    Console.WriteLine($"Temperature: {KelvinToCelsius(weather.Temperature.Value)} C");
                    Console.WriteLine($"Weather: {weather.Weather.Value}");
                }
            }
        }
        #endregion

        static string KelvinToCelsius(double kelvin)
        {
            return Math.Round(kelvin - 273.15, 2).ToString();
        }
    }
    #region CLASS FOR JSON
    public class WeatherResponse
    {
        public Main Main { get; set; }
        public Weather[] Weather { get; set; }
        public string Name { get; set; }
    }
    public class Main
    {
        public double Temp { get; set; }
    }

    public class Weather
    {
        public string Description { get; set; }
    }
    #endregion

    #region CLASS FOR XML
    [XmlRoot("current")]
    public class XmlWeatherResponse
    {
        [XmlElement("city")]
        public City City { get; set; }

        [XmlElement("temperature")]
        public Temperature Temperature { get; set; }

        [XmlElement("weather")]
        public XmlWeather Weather { get; set; }
    }
    public class City
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
    }

    public class Temperature
    {
        [XmlAttribute("value")]
        public double Value { get; set; }
    }

    public class XmlWeather
    {
        [XmlAttribute("value")]
        public string Value { get; set; }
    }
    #endregion


}
