using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Weather
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            this.KeyPreview = true;
            InitializeComponent();
        }
        #region Variables
        // Variable which contains pictures names which was edited
        private string lostPicturesNames = "";
        // Contain the status of images equal
        private bool isImagesEqual = true;
        // SHA256 hashes for pictures to check them
        private readonly string[] sha256Hashes =
{
"4d97d68ba45f75d6f63fea2575659c8d48ae087894f58adce61cab400845dba2",
"7bd4657936b44fb4e8f568b6c09fbdc1a7936df1ceb1407fc46c24c7ef3d7848",
"7b1e76d8ec4dccd369491186ce1ec49ac0598bf30e158fb52244174ce30b2f72",
"6a455a7db1db6bc488967d4a15195c759da6d49b725a751078b51fe20d616440",
"d67ed35d7dbf10d139bf85b2632fffaaa2e338177d56f0240bce6d3a401ba9f0",
"d67ed35d7dbf10d139bf85b2632fffaaa2e338177d56f0240bce6d3a401ba9f0",
"5b93d1d05564bfdedf759cd96adff916da7b9af18fb30064f5a99a5270d599f0",
"5b93d1d05564bfdedf759cd96adff916da7b9af18fb30064f5a99a5270d599f0",
"f4abef242db956eb428ffc52e4c1e9565f7ea14b81716646f4f431ecd40f64ab",
"f4abef242db956eb428ffc52e4c1e9565f7ea14b81716646f4f431ecd40f64ab",
"649bddef1d5b18d1ad2a9bcc9394f9a21c06617a5a1530f6c258ed75d2de5ede",
"45f3c1e87773087c6dfe8a2bcd84f140d16155faba03c5e38b2be11a010426c7",
"6946c18d88bcb20930f07bc7a130593a0ff4a13f54ade73197cf3a6221a91a79",
"6946c18d88bcb20930f07bc7a130593a0ff4a13f54ade73197cf3a6221a91a79",
"056914371793153412a413db888143a67b3d32baaecabea75fa1052af9202ec5",
"056914371793153412a413db888143a67b3d32baaecabea75fa1052af9202ec5",
"f962e7602c0b5b0949d3f46524223dea2290503eee3964b81c7a6335d208fc7d",
"f962e7602c0b5b0949d3f46524223dea2290503eee3964b81c7a6335d208fc7d",
};
        // Icon names
        private readonly string[] iconNames = { "01d", "01n", "02d", "02n", "03d", "03n", "04d", "04n", "09d", "09n", "10d",
            "10n", "11d", "11n", "13d", "13n", "50d", "50n" };
        // Ping in milliseconds to google
        private long roundtripTime = 0;
        // API key
        public const string APPID = "ba763e0d8fe014c21a17e4c81b548f6b";
        // City is a city that user entered
        public static string city = "";
        // URL is URL API for request, answer is JSON string that we downloaded
        private string url, answer, longitude, latitude = "";
        // Variable for calculating SHA256 hash
        private readonly SHA256 sha256 = SHA256.Create();
        #endregion
        #region Methods
        #region SHA256Hash
        private byte[] GetSHA256Hash(string filename)
        {
            if (File.Exists(filename))
            {
                using (FileStream stream = File.OpenRead(filename))
                    return sha256.ComputeHash(stream);
            }
            else return new byte[] { 0 };
        }
        public static string BytesToString(byte[] bytes)
        {
            string result = "";
            foreach (byte b in bytes) result += b.ToString("x2");
            return result;
        }
        #endregion
        private void Form1_Load(object sender, EventArgs e)
        {
            // Check internet connection
            new System.Threading.Thread(() =>
            {
                roundtripTime = GooglePingTripTime();
                if (roundtripTime >= 500 || roundtripTime == 0)
                    MessageBox.Show("У вас плохой интернет, не удивляйтесь если запросы будут грузиться более 5, а то и 10 секунд", "Медленный интернет", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }).Start();
            // Check if images equal and not download it
            CheckPictureStatus();      
            // Check if app is already launched
            IsAppLaunched();
            // Method that sends the API request and display information
            WebDownloader();
        }
        private void CheckPictureStatus()
        {
            // Check SHA256 hash of pictures
            for (int i = 0; i < iconNames.Length; i++)
            {
                if (BytesToString(GetSHA256Hash($"Icons\\{iconNames[i]}.png")) != sha256Hashes[i])
                {
                    lostPicturesNames += $"{iconNames[i]}, ";
                    isImagesEqual = false;
                }
            }
            // If it's different
            if (!isImagesEqual)
            {
                // Check if directory Icons wasn't deleted and if yes create it
                new System.Threading.Thread(() =>
                {
                    if (!Directory.Exists("Icons"))
                        Directory.CreateDirectory("Icons");
                }).Start();
                // Display the message and if user press yes
                if (MessageBox.Show($"Кажется, что такие png, как: {lostPicturesNames.Remove(lostPicturesNames.Length - 2)} были изменены. " +
                    "Их скачивание может занять определённое время. Хотите сделать это прямо сейчас?", "Отсутствуют(ет) иконки(а)", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    == DialogResult.Yes)
                {
                    // Iterating over the string to get lost picture names
                    foreach (var i in lostPicturesNames.Split(','))
                    {
                        if (i != " ")
                            // Download it
                            new WebClient().DownloadFile($"http://openweathermap.org/img/wn/{i.Replace(" ", "")}@2x.png",
                                $@"Icons\{i.Replace(" ", "")}.png");
                    }
                }
                // If user press no - exit
                else Application.Exit();
            }
        }
        private void IsAppLaunched()
        {
            if (System.Diagnostics.Process.GetProcessesByName(Application.ProductName).Length > 1)
            {
                MessageBox.Show("Приложение уже запущено!", "Приложение уже запущено!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }
        public static long GooglePingTripTime()
        {
            try
            {
                return new System.Net.NetworkInformation.Ping().Send("www.google.com").RoundtripTime;
            }
            catch (System.Net.NetworkInformation.PingException)
            {
                MessageBox.Show("Проверьте подключение к интернету!", "Отсутствует подключение к интернету", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return -1;
            }
        }
        private void LinkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => new Form2().Show();
        private void CityName_KeyDown(object sender, KeyEventArgs e)
        {
            // When user press enter 
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                WebDownloader();
            }
        }
        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // If user press Esc offer user to exit
            if (e.KeyChar == 27 && MessageBox.Show("Вы действительно хотите выйти?", "Выход", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                Application.Exit();
            // If user press key "5" we open form
            if (e.KeyChar == 53)
            {
                e.Handled = true;
                new Form2().Show();
            }
        }
        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Open the google with latitude and longitude
            System.Diagnostics.Process.Start($"https://www.google.com/search?q={latitude}%2C{longitude}");
        }
        private void InformationDisplayer()
        {
            // Deserialize API answer
            var openWeather = JsonConvert.DeserializeObject<OpenWeather.OpenWeather>(answer);
            // Initialize longitude and latitude
            longitude = openWeather.coord.Lon.ToString().Replace(",", ".");
            latitude = openWeather.coord.Lat.ToString().Replace(",", ".");
            // Icon
            panel1.BackgroundImage = openWeather.weather[0].Icon;
            // Display the information to the labels
            label2.Text = openWeather.weather[0].Description.Substring(0, 1).ToUpper() + openWeather.weather[0].Description.Remove(0, 1);
            label3.Text = $"Температура: {openWeather.main.Temp:0.##}°C";
            label4.Text = $"Чувствуется как: {openWeather.main.Feels_Like}°C";
            label5.Text = $"Скорость ветра: {openWeather.wind.Speed} м/с";
            label6.Text = $"Стор.света: {Form3.DegressConverter(openWeather.wind.Deg)}";
            label7.Text = $"Влажность: {openWeather.main.Humidity}%";
            label8.Text = $"Давление: {Math.Round(openWeather.main.Pressure, 2)} мм";
            label9.Text = $"Долгота: {longitude}";
            label10.Text = $"Широта: {latitude}";
            label11.Text = $"Код страны: {openWeather.Id}";
            label12.Text = $"Мин.температура: {openWeather.main.Temp_min}°C";
            label13.Text = $"Макс.температура: {openWeather.main.Temp_max}°C";
            label14.Text = $"Страна: {openWeather.sys.Country}";
        }
        private void WebDownloader()
        {
            // If city name isn't a null or spaces
            if (!string.IsNullOrWhiteSpace(cityName.Text))
            {
                // Initialize variable city with user entry
                city = cityName.Text.Trim();
                // URL
                url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={APPID}&lang=ru&units=metric";
                // Using the class WebClient to download string
                using (var wc = new WebClient())
                {
                    // Put Encoding to UTF8
                    wc.Encoding = Encoding.UTF8;
                    // Download json 
                    answer = wc.DownloadString(url);
                }
                // Call the method which display informations
                InformationDisplayer();
            }   
        }
        #endregion
    }
}
