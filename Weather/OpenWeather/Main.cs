
namespace Weather.OpenWeather
{
    class Main
    {
        public double Temp { get; set; }
        public double Temp_min { get; set; }
        public double Temp_max { get; set; }
        public double Feels_Like { get; set; }
        private double _pressure;
        public double Pressure
        {
            get => _pressure;
            set => _pressure = value / 1.3332239;
        }
        public double Humidity { get; set; }
    }
}
