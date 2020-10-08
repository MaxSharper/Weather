using System.Collections.Generic;

namespace Weather.OpenWeatherFiveDays
{
    class Root
    {
        public string Cod { get; set; }
        public double Message { get; set; }
        public int Cnt { get; set; }
        public List<Forecast> List { get; set; }
        public City City { get; set; }
    }
}
