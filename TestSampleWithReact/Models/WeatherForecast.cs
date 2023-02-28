namespace TestSampleWithReact.Models
{
    public class WeatherForecast
    {
        public DateOnly Date { get; set; }

        public double TemperatureC { get; set; }

        public double TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }
    }
}