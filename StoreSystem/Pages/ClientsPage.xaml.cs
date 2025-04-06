using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StoreSystem
{
    public partial class ClientsPage : Page
    {
        private List<ClientModel> clients;
        private ClientModel selectedClient;

        public ClientsPage()
        {
            InitializeComponent();
            LoadClients();
        }

        private async void LoadClients()
        {
            var response = await UserSession.Instance.SendAuthorizedRequest(() =>
                new HttpRequestMessage(HttpMethod.Get, "https://localhost:7072/clients"));

            var json = await response.Content.ReadAsStringAsync();
            clients = JsonSerializer.Deserialize<List<ClientModel>>(json);
            
            ClientsListView.ItemsSource = clients;
        }

        private void ClientsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ClientsListView.SelectedItem is ClientModel client)
            {
                selectedClient = client;
                ClientIdTextBox.Text = client.clientId.ToString();
                ClientNameTextBox.Text = client.name;
                ContactInfoTextBox.Text = client.contactInfo;
            }
        }

        private async void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (selectedClient == null) return;

            selectedClient.name = ClientNameTextBox.Text;
            selectedClient.contactInfo = ContactInfoTextBox.Text;

            var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7072/clientsupdate")
            {
                Content = new StringContent(JsonSerializer.Serialize(selectedClient), Encoding.UTF8, "application/json")
            };
            
            var response = await UserSession.Instance.SendAuthorizedRequest(() => request);

            if (response.IsSuccessStatusCode)
                MessageBox.Show("Изменения сохранены", "Клиенты", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show("Ошибка при сохранении", "Клиенты", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public class ClientModel
    {
        public int clientId { get; set; }
        public string name { get; set; }
        public string contactInfo { get; set; }
    }
}

