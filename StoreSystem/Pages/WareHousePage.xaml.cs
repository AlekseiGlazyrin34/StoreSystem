using System.Text.Json;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StoreSystem
{
    public partial class WareHousePage : Page
    {
        private List<WarehouseItem> items;
        private WarehouseItem selectedItem;

        public WareHousePage()
        {
            InitializeComponent();
            LoadWarehouseItems();
        }

        private async void LoadWarehouseItems()
        {
            var response = await UserSession.Instance.SendAuthorizedRequest(() =>
                new HttpRequestMessage(HttpMethod.Get, "https://localhost:7072/warehouse"));

            var json = await response.Content.ReadAsStringAsync();
            items = JsonSerializer.Deserialize<List<WarehouseItem>>(json);
            WarehouseListView.ItemsSource = items;
        }

        private void WarehouseListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (WarehouseListView.SelectedItem is WarehouseItem item)
            {
                selectedItem = item;
                StoreNameTextBox.Text = item.storeName;
                ProductNameTextBox.Text = item.productName;
                QuantityTextBox.Text = item.quantity.ToString();
            }
        }

        private async void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (selectedItem == null) return;

            selectedItem.storeName = StoreNameTextBox.Text;
            selectedItem.productName = ProductNameTextBox.Text;
            selectedItem.quantity = int.Parse(QuantityTextBox.Text);

            var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7072/warehouseupdate")
            {
                Content = new StringContent(JsonSerializer.Serialize(selectedItem), Encoding.UTF8, "application/json")
            };

            var response = await UserSession.Instance.SendAuthorizedRequest(() => request);

            if (response.IsSuccessStatusCode)
                MessageBox.Show("Изменения сохранены", "Склад", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show("Ошибка сохранения", "Склад", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public class WarehouseItem
    {
        public int id { get; set; } // Для обновления записи
        public string storeName { get; set; }
        public string productName { get; set; }
        public int quantity { get; set; }
    }
}

