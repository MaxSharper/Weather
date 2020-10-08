using System;
using System.Windows.Forms;

namespace Weather
{
    public partial class Form2 : Form
    {
        long roundtripTime = 0;
        #region Methods
        public Form2()
        {
            InitializeComponent();
            this.KeyPreview = true;
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            // Check ping to google
            roundtripTime = Form1.GooglePingTripTime();
            // Calculate current date
            var dateNow = DateTime.Now;
            // First button is current day, second button button is next day etc
            button0.Text = CapitalizeFirstLetter(DateFormat(dateNow));
            button1.Text = CapitalizeFirstLetter(DateFormat(dateNow.AddDays(1)));
            button2.Text = CapitalizeFirstLetter(DateFormat(dateNow.AddDays(2)));
            button3.Text = CapitalizeFirstLetter(DateFormat(dateNow.AddDays(3)));
            button4.Text = CapitalizeFirstLetter(DateFormat(dateNow.AddDays(4)));
        }
        // Capitalize first letter of text
        public string CapitalizeFirstLetter(string text) => text.Substring(0, 1).ToUpper() + text.Remove(0, 1);
        // Method DateFormat return date like name of day of week
        private string DateFormat(DateTime date) => date.ToString("dddd");
        private void Button1_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            // Check internet connection
            if (roundtripTime >= 500 || roundtripTime == 0)
                MessageBox.Show("У вас плохой интернет, возможно длительное ожидание", "Плохой интернет", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            // Display Form3
            new Form3(Convert.ToInt32(button.Name.Remove(0, button  .Name.Length - 1)), button.Text).Show();
            // Close this Form
            Close();
        }
        private void Form2_KeyDown(object sender, KeyEventArgs e)
        {
            // If user press escape close form
            if (e.KeyCode == Keys.Escape) Close();
        }
        #endregion
    }
}
