using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Call_API_from.net_Framework_4._8
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // Attach the event handler to the TextChanged event of each TextBox
            txtUsername.TextChanged += TextBoxes_TextChanged;
            txtPassword.TextChanged += TextBoxes_TextChanged;
            txtAPI.TextChanged += TextBoxes_TextChanged;

            // Initial validation
            ValidateLoginButton();
        }


        private void TextBoxes_TextChanged(object sender, EventArgs e)
        {
            ValidateLoginButton();
        }

        private void ValidateLoginButton()
        {
            // Disable btnLogin if any of the TextBoxes are empty or null
            btnLogin.Enabled = !string.IsNullOrWhiteSpace(txtUsername.Text) &&
                               !string.IsNullOrWhiteSpace(txtPassword.Text) &&
                               !string.IsNullOrWhiteSpace(txtAPI.Text);
        }


        private static readonly HttpClient client = new HttpClient();

        private async void btnLogin_Click(object sender1, EventArgs e)
        {
            btnLogin.Enabled = false;

            System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) =>
                    true; //Accept All Certificates(Not Recommended for Production)
            try
            {
                txtConsole.Text = @"Calling API...";
                var data = new
                {
                    username = txtUsername.Text,
                    password = txtPassword.Text
                };


                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(txtAPI.Text, content);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    txtConsole.Text = result;
                }
                else
                {
                    txtConsole.Text = $@"Error: {response.StatusCode} {await response.Content.ReadAsStringAsync()}";
                }

                btnLogin.Enabled = true;
            }
            catch (HttpRequestException er)
            {
                string errorMessage = $"Request error: {er.Message}";

                if (er.InnerException != null)
                {
                    errorMessage += $"\nInner Exception: {er.InnerException.Message}";
                }

                MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnLogin.Enabled = true;
            }
        }
    }
}