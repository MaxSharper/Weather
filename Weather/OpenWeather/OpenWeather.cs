
namespace Weather.OpenWeather
{
    class OpenWeather
    {
        public Coord coord = new Coord();
        public Weather[] weather = { };
        public string Id { get; set; }
        public Main main = new Main();
        public double Visibility { get; set; }
        public Wind wind = new Wind();
        public Sys sys = new Sys();
        public string Name { get; set; }
    }
}
