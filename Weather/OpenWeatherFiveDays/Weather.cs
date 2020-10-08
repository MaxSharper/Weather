namespace Weather.OpenWeatherFiveDays
{
    class Weather
    {
        public int Id { get; set; }
        public string Main { get; set; }
        public string icon = "";
        public System.Drawing.Bitmap Icon { get => new System.Drawing.Bitmap(System.Drawing.Image.FromFile($"icons/{icon}.png")); }
        public string Description { get; set; }
    }
}
