using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
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

namespace StoreSystem
{

    public partial class StoresPage : Page
    {
        private List<StoreModel> stores;
        private StoreModel selectedStore;

        public StoresPage()
        {
            InitializeComponent();
            LoadStores();
        }

        private async void LoadStores()
        {
            var response = await UserSession.Instance.SendAuthorizedRequest(() =>
                new HttpRequestMessage(HttpMethod.Get, "https://localhost:7072/stores"));

            var json = await response.Content.ReadAsStringAsync();
            stores = JsonSerializer.Deserialize<List<StoreModel>>(json);
            StoresListView.ItemsSource = stores;
        }

        private void StoresListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (StoresListView.SelectedItem is StoreModel store)
            {
                selectedStore = store;
                StoreAddressTextBox.Text = store.address;
            }
        }

        private async void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (selectedStore == null) return;

            selectedStore.address = StoreAddressTextBox.Text;

            var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7072/storesupdate")
            {
                Content = new StringContent(JsonSerializer.Serialize(selectedStore), Encoding.UTF8, "application/json")
            };

            var response = await UserSession.Instance.SendAuthorizedRequest(() => request);

            if (response.IsSuccessStatusCode)
                MessageBox.Show("Изменения сохранены", "Магазины", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show("Ошибка при сохранении", "Магазины", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public class StoreModel
    {
        public int storeId { get; set; }
        public string address { get; set; }
    }

}
