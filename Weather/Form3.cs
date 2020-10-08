using System;
using System.Text.RegularExpressions;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Windows.Forms;
using Weather.OpenWeatherFiveDays;

namespace Weather
{
    public partial class Form3 : Form
    {
        #region Variables
        // Object of Form2
        private readonly Form2 form2 = new Form2();
        // Variable shows how many days we should add to current date to get the date which user asks (need it to display date)
        private readonly int days = 0;
        // Counters need for correct work of information displayer
        private int labelCounter = 1;
        private int pictureBoxCounter = 1;
        // Latitude and longitude
        private string latitude, longitude = "";
        // City from first form
        private string city = Form1.city;
        // Selected day is the text of button that user press (need it to display the name of day of week)
        private readonly string selectedDay = "";
        // Time is weather time (00:00,03:00,06:00 etc)
        private string url, answerJson, time = "";
        // Main variable which contains all weather info
        private Root deserializedJson = new Root();
        #endregion
        #region Constructor
        public Form3(int days, string selectedDay)
        {
            InitializeComponent();
            // Initialize variables
            this.days = days;
            this.KeyPreview = true;
            this.selectedDay = selectedDay;
        }
        #endregion
        #region Methods
        private void Form3_Load(object sender, EventArgs e)
        {
            // Set title
            title.Text = $"Прогноз погоды на {DateTime.Now.AddDays(days).ToShortDateString()} ({selectedDay})";
            // Add event: when user press ctrl + s he call method which open the google with our coord
            this.KeyDown += (s, a) =>
            {
                if (a.Modifiers == Keys.Control && a.KeyCode == Keys.S)
                    Country1_LinkClicked(null, null);
            };
            // Call main method which display info, initialize variables
            PropertyInitializer();
            // Set tooltip to not initialize labels
            PopUpHelper();
        }
        private void PropertyInitializer()
        {
            // Set counters to default
            labelCounter = 1;
            pictureBoxCounter = 1;
            // Set textbox which represents user city to form1 city
            cityName.Text = city;
            // API URL
            url = $"http://api.openweathermap.org/data/2.5/forecast?q={city}&lang=ru&units=metric&appid={Form1.APPID}";
            // Download string
            answerJson = new WebClient() { Encoding = Encoding.UTF8 }.DownloadString(url);
            // Deserialize answer
            deserializedJson = JsonConvert.DeserializeObject<Root>(answerJson);
            // Initialize latitude and longitude variables
            latitude = deserializedJson.City.Coord.Lat.ToString().Replace(",", ".");
            longitude = deserializedJson.City.Coord.Lon.ToString().Replace(",", ".");
            // Set label which display country and coord with necessary values
            country1.Text = $"{deserializedJson.City.Country} ({latitude}, {longitude})";
            // Iterate in deserialized JSON
            foreach (var forecast in deserializedJson.List)
            {
                // If date which user's chosen is equal to date in weather list
                if (Convert.ToDateTime(DateTime.Now.AddDays(days).ToString("yyyy-MM-dd")).ToShortDateString() == Convert.ToDateTime(forecast.Dt_txt).ToShortDateString())
                {
                    // Call the method that display pictures
                    pictureBoxCounter = PanelDisplayer(pictureBoxCounter, forecast);
                    // Calculate the time
                    time = Regex.Match(forecast.Dt_txt, ".*?([0-9]{2}:00:00)").Groups[1].Value;
                    time = time.Remove(time.Length - 3);
                    // Call the method that display info on labels
                    labelCounter = LabelDisplayer(labelCounter, forecast);
                }
                continue;
            }
        }
        private int PanelDisplayer(int panelCounter, Forecast forecast)
        {
            // Iterate all picture box
            foreach (PictureBox pb in Controls.Cast<Control>().Where(x => x is PictureBox).Select(x => x as PictureBox))
            {
                // If picture box name contains panel counter (need for sequential display)
                if (pb.Name.Contains(panelCounter.ToString()))
                {
                    // Set icon
                    pb.BackgroundImage = forecast.Weather[0].Icon;
                    // Add one to panel counter
                    panelCounter++;
                }
                continue;
            }
            return panelCounter;
        }
        public static string DegressConverter(int degrees)
        {
            string result = "";
            // Depending on degrees that we've gotten we return necessary side of the world
            switch (degrees)
            {
                case 90:
                    result = "В";
                    break;
                case 180:
                    result = "Ю";
                    break;
                case int n when (n > 0 && n < 90):
                    result += "С-В";
                    break;
                case int n when (n > 90 && n < 180):
                    result += "Ю-В";
                    break;
                case int n when (n > 180 && n < 270):
                    result += "Ю-З";
                    break;
                case int n when (n > 270 && n < 360):
                    result += "С-З";
                    break;
                default:
                    result = "С";
                    break;
            }
            return result;
        }
        private void Country1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => System.Diagnostics.Process.Start($"https://www.google.com/search?q={latitude}%2C{longitude}");
        private void CityName_KeyDown(object sender, KeyEventArgs e)
        {
            // If user press enter
            if (e.KeyCode == Keys.Enter)
            {
                // Disable this awful sound
                e.SuppressKeyPress = true;
                // Initialize city
                city = cityName.Text.Trim();
                // Call the method which display info again
                PropertyInitializer();
            }
        }
        private int LabelDisplayer(int labelCounter, Forecast forecast)
        {
            // Iterating on all labels
            foreach (Label lb in Controls.Cast<Control>().Where(x => x is Label).Select(x => x as Label))
            {
                // If it contains counter
                if (lb.Name.Contains(labelCounter.ToString()))
                {
                    // Depending on it time we display neccessary information
                    if (lb.Name.Contains("time"))
                        lb.Text = time;
                    else if (lb.Name.Contains("description"))
                    {
                        string description = forecast.Weather[0].Description;
                        if (description.Length >= 18)
                            lb.Font = new System.Drawing.Font("Calibri", 8f);
                        lb.Text = description.Length == 4 ? $"        {form2.CapitalizeFirstLetter(description)}" : description.Length == 8 ? $"   {form2.CapitalizeFirstLetter(description)}" : form2.CapitalizeFirstLetter(description);
                        labelCounter++;
                    }
                    else if (lb.Name.Contains("temp"))
                        lb.Text = $"{forecast.Main.Temp}°C";
                    else if (lb.Name.Contains("maxTemp"))
                        lb.Text = $"{forecast.Main.Temp_max}°C";
                    else if (lb.Name.Contains("minTemp"))
                        lb.Text = $"{forecast.Main.Temp_min}°C";
                    else if (lb.Name.Contains("feelsLike"))
                        lb.Text = $"{forecast.Main.Feels_like}°C";
                    else if (lb.Name.Contains("deg"))
                        lb.Text = $"{forecast.Wind.Deg}° - {DegressConverter((int)forecast.Wind.Deg)}";
                    else if (lb.Name.Contains("humidity"))
                        lb.Text = $"{forecast.Main.Humidity}";
                    else if (lb.Name.Contains("pressure"))
                        lb.Text = $"{Math.Round(forecast.Main.Pressure, 2)}";
                    else if (lb.Name.Contains("windSpeed"))
                        lb.Text = $"{forecast.Wind.Speed}";
                    continue;
                }
            }
            return labelCounter;
        }
        private void PopUpHelper()
        {
            // Set up tool tip
            var toolTip = new ToolTip
            {
                ToolTipIcon = ToolTipIcon.Info,
                ToolTipTitle = "Здесь пока ничего нет",
                InitialDelay = 10
            };
            foreach (Label lb in Controls.Cast<Control>().Where(x => x is Label).Select(x => x as Label))
            {
                // If label wasn't initialized
                if (lb.Text.Contains("some"))
                {
                    // Set text "None"
                    lb.Text = "  None";
                    // Set tool tip
                    toolTip.SetToolTip(lb, "Загляните завтра пораньше!");
                }
            }
            // Update Form
            Update();
        }
        private void BackPanel_Click(object sender, EventArgs e)
        {
            // If user press Esc or picture with arrow
            // Close this form
            Close();
            // Show Form2
            form2.Show();
        }
        private void Form3_KeyPress(object sender, KeyPressEventArgs e)
        {
            // If user press Esc call the method that returns to Form2
            if (e.KeyChar == 27)
                BackPanel_Click(null, null);
        }
        #endregion
    }
}