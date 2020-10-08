namespace Weather.OpenWeatherFiveDays
{
    class MainDetail
    {
        public double Temp { get; set; }
        public double Feels_like { get; set; }
        public double Temp_min { get; set; }
        public double Temp_max { get; set; }
        private double _pressure;
        public double Pressure
        {
            get => _pressure;
            set => _pressure = value / 1.3332239;
        }
        public int Humidity { get; set; }
    }
}
