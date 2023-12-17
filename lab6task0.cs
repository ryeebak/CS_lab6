using System.Collections.Generic;
using System.Net.Http;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;

class WeatherInfo
{
    public MainInfo main { get; set; }
    public WeatherDescription[] weather { get; set; }
    public string name { get; set; }
    public SysInfo sys { get; set; }
}

class MainInfo
{
    public double temp { get; set; }
}

class WeatherDescription
{
    public string description { get; set; }
}

class SysInfo
{
    public string country { get; set; }
}

class Weather
{
    public string Country { get; set; }
    public string Name { get; set; }
    public double Temp { get; set; }
    public string Description { get; set; }
}
class lab6task0
{
    static async Task Main(string[] args)
    {
        for (int j = 0; j < 10; j++)
        {
            Console.WriteLine();

            string apiKey = "1079432d32219ee76ef9ff5766bb3924";
            string apiUrl = "https://api.openweathermap.org/data/2.5/weather";

            List<Weather> weatherData = new List<Weather>();
            Random random = new Random();

            Console.Write("Collecting info, please wait for full response:\n$");
            for (int i = 0; i < 50; i++)
            {
                Console.Write($"{i+1} -> ");
                double latitude = random.NextDouble() * (90 - (-90)) + (-90);
                double longitude = random.NextDouble() * (180 - (-180)) + (-180);

                Weather weather = await GetWeatherData(apiUrl, apiKey, latitude, longitude);
                if (weather != null)
                {
                    weatherData.Add(weather);
                }
            }

            var maxTempCountry = weatherData.OrderByDescending(w => w.Temp).FirstOrDefault();
            var minTempCountry = weatherData.OrderBy(w => w.Temp).FirstOrDefault();
            var averageTemp = weatherData.Average(w => w.Temp);
            var distinctCountriesCount = weatherData.Select(w => w.Country).Distinct().Count();

            var allCountries = weatherData.Select(w => w.Country).Distinct().ToList();

            var firstClearSky = weatherData.FirstOrDefault(w => w.Description == "clear sky");
            var firstRain = weatherData.FirstOrDefault(w => w.Description == "rain");
            var firstFewClouds = weatherData.FirstOrDefault(w => w.Description == "few clouds");

            Console.WriteLine($"\n\nThe hottest country: {maxTempCountry?.Country}, Temperature: {maxTempCountry?.Temp}°C");
            Console.WriteLine($"The coldest country: {minTempCountry?.Country}, Temperature: {minTempCountry?.Temp}°C");
            Console.WriteLine($"\nAverage temperature: {averageTemp}°C");
            Console.WriteLine($"Amount of countries used in selection: {distinctCountriesCount}");

            if (firstClearSky != null && firstClearSky.Country != null)
            {
                Console.WriteLine($"\nFirst country with clear sky: {firstClearSky.Country}, Location: {firstClearSky.Name}");
            }
            else
            {
                Console.WriteLine("\nWhere is no country with clear sky");
            }

            if (firstRain != null && firstRain.Country != null)
            {
                Console.WriteLine($"First rainy place: {firstRain.Country}, Location: {firstRain.Name}");
            }
            else
            {
                Console.WriteLine("Its pretty dry");
            }

            if (firstFewClouds != null && firstFewClouds.Country != null)
            {
                Console.WriteLine($"First clowdy place: {firstFewClouds.Country}, Location: {firstFewClouds.Name}");
            }
            else
            {
                Console.WriteLine("Its pretty clear");
            }

            Console.WriteLine("\nAll countries:");
            foreach (var country in allCountries)
            {
                Console.WriteLine(country);
            }
        }
    }

    static async Task<Weather> GetWeatherData(string apiUrl, string apiKey, double latitude, double longitude)
    {
        using (HttpClient client = new HttpClient())
        {
            int maxAttempts = 10;
            int attempt = 0;

            while (attempt < maxAttempts)
            {
                var response = await client.GetStringAsync($"{apiUrl}?lat={latitude}&lon={longitude}&appid={apiKey}&units=metric");
                var weatherInfo = JsonSerializer.Deserialize<WeatherInfo>(response);

                if (weatherInfo != null && !string.IsNullOrEmpty(weatherInfo.sys.country))
                {
                    string country = weatherInfo.sys.country;
                    string name = weatherInfo.name;
                    double temp = weatherInfo.main.temp;
                    string description = weatherInfo.weather[0].description;

                    return new Weather
                    {
                        Country = country,
                        Name = name,
                        Temp = temp,
                        Description = description
                    };
                }

                attempt++;
            }

            return null;
        }
    }
}
