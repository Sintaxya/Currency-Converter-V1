using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace WPF_CurrencyConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Root val = new Root();
        public class Root
        {
            public Rate rates { get; set; }
            public long timestamp;
            public string license;

        }

        public class Rate
        {
            public double INR { get; set; }
            public double JPY { get; set; }
            public double USD { get; set; }
            public double EUR { get; set; }
            public double CAD { get; set; }
            public double ISK { get; set; }
            public double PHP { get; set; }
            public double DKK { get; set; }
            public double CZK { get; set; }
            public double HUF { get; set; }

        }

        public static async Task<Root> GetData<T>(string url)
        {
            var myRoot = new Root();
            try
            {
                using (var client = new HttpClient()) // HttpClient a class that provides sending and recieving for HTTP requests and responses from a URL.
                {
                    client.Timeout = TimeSpan.FromMinutes(1); 
                    HttpResponseMessage response = await client.GetAsync(url); // HttpResponse is a way of returning msg
                    if (response.StatusCode == System.Net.HttpStatusCode.OK) // Check API status
                    {
                        var ResponseString = await response.Content.ReadAsStringAsync(); //
                        var ResponseObject = JsonConvert.DeserializeObject<Root>(ResponseString); //

                        //MessageBox.Show("Time Stamp: " + ResponseObject.timestamp, "Information", MessageBoxButton.OK, MessageBoxImage.Question);

                        return ResponseObject; // return API response.
                    }
                    return myRoot;
                }
            }
            catch
            {
                return myRoot;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            GetValue();
        }

        private async void GetValue()
        {
            val = await GetData<Root>("https://openexchangerates.org/api/latest.json?app_id=7ac9ee5cde5f4b689cbeea66de6834f8"); //API access URL
            BindCurrency();
        }

        private void BindCurrency()
        {
            DataTable dtCurrency = new DataTable();
            dtCurrency.Columns.Add("Text");
            dtCurrency.Columns.Add("Value");
            // // //
            dtCurrency.Rows.Add("SELECT", 0);
            dtCurrency.Rows.Add("INR", val.rates.INR);
            dtCurrency.Rows.Add("JPY", val.rates.JPY);
            dtCurrency.Rows.Add("USD", val.rates.USD);
            dtCurrency.Rows.Add("EUR", val.rates.EUR);
            dtCurrency.Rows.Add("CAD", val.rates.CAD);
            dtCurrency.Rows.Add("ISK", val.rates.ISK);
            dtCurrency.Rows.Add("PHP", val.rates.PHP);
            dtCurrency.Rows.Add("DKK", val.rates.DKK);
            dtCurrency.Rows.Add("CZK", val.rates.CZK);
            dtCurrency.Rows.Add("HUF", val.rates.HUF);
            //////
            cmbFromCurrency.ItemsSource = dtCurrency.DefaultView;
            cmbFromCurrency.DisplayMemberPath = "Text";
            cmbFromCurrency.SelectedValuePath = "Value";
            cmbFromCurrency.SelectedIndex = 0;

            cmbToCurrency.ItemsSource = dtCurrency.DefaultView;
            cmbToCurrency.DisplayMemberPath = "Text";
            cmbToCurrency.SelectedValuePath = "Value";
            cmbToCurrency.SelectedIndex = 0;
        }


        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearControls();
        }

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            double ConvertedValue;

            if (txtCurrency == null || txtCurrency.Text.Trim() == " ")
            {
                MessageBox.Show("Please enter currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                txtCurrency.Focus();
                return;
            }
            else if ( cmbFromCurrency.SelectedValue == null || cmbFromCurrency.SelectedIndex == 0)
            {
                MessageBox.Show("Please select the 'from' currency!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                cmbFromCurrency.Focus();
                return;
            }
            else if (cmbToCurrency == null || cmbToCurrency.SelectedIndex == 0)
            {
                MessageBox.Show("Please select the 'from' currency!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                cmbToCurrency.Focus();
                return;
            }
            if (cmbFromCurrency.Text == cmbToCurrency.Text)
            {
                ConvertedValue = double.Parse(txtCurrency.Text);
                lblCurrency.Content = cmbToCurrency + " " + ConvertedValue.ToString("N3");
            }
            else
            {
                double x, y, z = 0;
                x = double.Parse(cmbFromCurrency.SelectedValue.ToString());
                y = double.Parse(cmbToCurrency.SelectedValue.ToString());
                z = double.Parse(txtCurrency.Text);
                ConvertedValue = (y * z) / x;

                lblCurrency.Content = ConvertedValue.ToString("N3") + " " + cmbToCurrency.Text;
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ClearControls()
        {
            txtCurrency.Text = string.Empty;
            if (cmbFromCurrency.Items.Count > 0) { cmbFromCurrency.SelectedIndex = 0; }
            if (cmbToCurrency.Items.Count > 0) { cmbToCurrency.SelectedIndex = 0; }
            lblCurrency.Content = "";
            txtCurrency.Focus();
        }
    }
}
