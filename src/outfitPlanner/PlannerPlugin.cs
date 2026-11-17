using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace outfitPlanner
{
    public class PlannerPlugin(HttpClient httpClient)
    {
        [KernelFunction("GetWeather")]
        [Description("To get the hourly weather of a specific location")]
        public async Task<string> GetWeather(string city)
        {
            Console.WriteLine($"[Tool] GetWeather({city})");
            try
            {
                string weather_apikey = Environment.GetEnvironmentVariable("weather_api");

                if (string.IsNullOrEmpty(weather_apikey))
                {
                    Console.WriteLine("Weather API is missing");
                    return "Weather API is missing";
                }

                string url = $"https://api.weatherapi.com/v1/forecast.json?key={weather_apikey}&q={city}&days=1";
                Console.WriteLine(url);
                HttpResponseMessage response= await httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Failed retrieving weather");
                    return "Failed retrieving the weather";
                }

                string json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<WeatherApiResponse>(json);
                 var today = data.Forecast?.ForecastDay?.FirstOrDefault()?.Day;

            return $"""
                Location: {data.Location.Name}, {data.Location.Country} (Local time: {data.Location.LocalTime})
                
                [Current Weather]
                - Condition: {data.Current.Condition.Text} ({(data.Current.IsDay == 1 ? "Day" : "Night")})
                - Temperature: {data.Current.TempC}°C (Feels like {data.Current.FeelsLikeC}°C)
                - Humidity: {data.Current.Humidity}%, Wind: {data.Current.WindKph} km/h, UV Index: {data.Current.Uv}
                
                [Today's Forecast ({data.Forecast?.ForecastDay?.FirstOrDefault()?.Date})]
                - High / Low: {today?.MaxTempC}°C / {today?.MinTempC}°C
                - Overall Condition: {today?.Condition.Text}
                - Chance of Rain: {today?.DailyChanceOfRain}% (Total precip: {today?.TotalPrecipMm} mm)
                """;
        }
        catch (Exception ex)
        {
            return $"Error fetching weather for '{city}': {ex.Message}";
        }

        }

        public record WeatherApiResponse(
            [property: JsonPropertyName("location")] LocationInfo Location,
            [property: JsonPropertyName("current")] CurrentWeather Current,
            [property: JsonPropertyName("forecast")] ForecastInfo? Forecast
        );

        public record LocationInfo(
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("region")] string Region,
            [property: JsonPropertyName("country")] string Country,
            [property: JsonPropertyName("localtime")] string LocalTime
        );

        public record ConditionInfo(
            [property: JsonPropertyName("text")] string Text,
            [property: JsonPropertyName("code")] int Code
        );

        public record CurrentWeather(
            [property: JsonPropertyName("last_updated")] string LastUpdated,
            [property: JsonPropertyName("temp_c")] double TempC,
            [property: JsonPropertyName("feelslike_c")] double FeelsLikeC,
            [property: JsonPropertyName("is_day")] int IsDay,
            [property: JsonPropertyName("condition")] ConditionInfo Condition,
            [property: JsonPropertyName("wind_kph")] double WindKph,
            [property: JsonPropertyName("precip_mm")] double PrecipMm,
            [property: JsonPropertyName("humidity")] int Humidity,
            [property: JsonPropertyName("uv")] double Uv
        );

        public record ForecastInfo(
            [property: JsonPropertyName("forecastday")] List<ForecastDayItem> ForecastDay
        );

        public record ForecastDayItem(
            [property: JsonPropertyName("date")] string Date,
            [property: JsonPropertyName("day")] DayForecast Day,
            [property: JsonPropertyName("hour")] List<HourForecast> Hour
        );

        public record DayForecast(
            [property: JsonPropertyName("maxtemp_c")] double MaxTempC,
            [property: JsonPropertyName("mintemp_c")] double MinTempC,
            [property: JsonPropertyName("avgtemp_c")] double AvgTempC,
            [property: JsonPropertyName("maxwind_kph")] double MaxWindKph,
            [property: JsonPropertyName("totalprecip_mm")] double TotalPrecipMm,
            [property: JsonPropertyName("daily_will_it_rain")] int DailyWillItRain,
            [property: JsonPropertyName("daily_chance_of_rain")] int DailyChanceOfRain,
            [property: JsonPropertyName("daily_will_it_snow")] int DailyWillItSnow,
            [property: JsonPropertyName("daily_chance_of_snow")] int DailyChanceOfSnow,
            [property: JsonPropertyName("condition")] ConditionInfo Condition,
            [property: JsonPropertyName("uv")] double Uv
        );

        public record HourForecast(
            [property: JsonPropertyName("time")] string Time,
            [property: JsonPropertyName("temp_c")] double TempC,
            [property: JsonPropertyName("feelslike_c")] double FeelsLikeC,
            [property: JsonPropertyName("is_day")] int IsDay,
            [property: JsonPropertyName("condition")] ConditionInfo Condition,
            [property: JsonPropertyName("wind_kph")] double WindKph,
            [property: JsonPropertyName("precip_mm")] double PrecipMm,
            [property: JsonPropertyName("humidity")] int Humidity,
            [property: JsonPropertyName("will_it_rain")] int WillItRain,
            [property: JsonPropertyName("chance_of_rain")] int ChanceOfRain,
            [property: JsonPropertyName("will_it_snow")] int WillItSnow,
            [property: JsonPropertyName("chance_of_snow")] int ChanceOfSnow,
            [property: JsonPropertyName("uv")] double Uv
        );

    }
}