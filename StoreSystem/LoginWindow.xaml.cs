using System;
using System.Collections.Generic;

using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace StoreSystem
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        static HttpClient httpClient = new HttpClient();
        public LoginWindow()
        {
            InitializeComponent();
            
        }
        private async Task Login()
        {
            if (LoginBox.Text=="" || PasswordBox.Text == "")
            {
                ErrorLabel.Content = "Поля должны быть заполнены";
                    return;
            }
            var loginCont = LoginBox.Text;
            var passwordCont = PasswordBox.Text;


            var data = new
            {
                Login = loginCont,
                Password = passwordCont
            };
            string jsonData = JsonSerializer.Serialize(data);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            try
            {
                var response = await httpClient.PostAsync("https://localhost:7072/login", content);
                if (response.IsSuccessStatusCode) {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                    var respData = JsonSerializer.Deserialize<Dictionary<string, string>>(await response.Content.ReadAsStringAsync(), options);

                   
                    UserSession.Instance.Username = respData["username"];
                    UserSession.Instance.Login = respData["login"];
                    UserSession.Instance.Password = respData["password"];
                    
      
                    UserSession.Instance.AccessToken = respData["token"];
                    UserSession.Instance.RefreshToken = respData["refreshToken"];
                    UserSession.Instance.UserId = Convert.ToInt32(respData["userId"]);
                    MainWindow MW = new MainWindow();
                    this.Close();
                    MW.Show();
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) 
                {
                    ErrorLabel.Content = "Неправильный логин или пароль";
                }
            }
            catch (HttpRequestException e)
            {
                ErrorLabel.Content = "Не удалось установить соединение с сервером";
            }
        }
            

        async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LogBut.IsEnabled = false;
            await Login();
            LogBut.IsEnabled = true;
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LogBut.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }

    }
}