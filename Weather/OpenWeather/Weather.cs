
namespace Weather.OpenWeather
{
    class Weather
    {
        public string Description { get; set; }
        public string icon = "";
        public System.Drawing.Bitmap Icon { get => new System.Drawing.Bitmap(System.Drawing.Image.FromFile($"Icons\\{icon}.png"));}
    }
}
