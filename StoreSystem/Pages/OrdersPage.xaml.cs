using System.Text.Json;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace StoreSystem
{
    public partial class OrdersPage : Page
    {
        private List<OrderModel> orders;
        private OrderModel selectedOrder;

        public OrdersPage()
        {
            InitializeComponent();
            LoadOrders();
        }

        private async void LoadOrders()
        {
            var client = new HttpClient();
            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "https://localhost:7072/orders"));

            var json = await response.Content.ReadAsStringAsync();
            
            orders = JsonSerializer.Deserialize<List<OrderModel>>(json);
            Console.WriteLine(orders[0].clientName);
            OrdersListView.ItemsSource = orders;
        }

        private void OrdersListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (OrdersListView.SelectedItem is OrderModel order)
            {
                selectedOrder = order;
                ClientNameTextBox.Text = order.clientName;
                AddressTextBox.Text = order.address;
                OrderDateTextBox.Text = order.orderDate.ToString("yyyy-MM-dd");
                DiscountTextBox.Text = order.discountPercent.ToString();
                TotalAmountTextBox.Text = order.totalAmount.ToString("F2");
                DeliveryDateTextBox.Text = order.deliveryDate?.ToString("yyyy-MM-dd") ?? "";
            }
        }

        private async void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (selectedOrder == null) return;

            selectedOrder.clientName = ClientNameTextBox.Text;
            selectedOrder.address = AddressTextBox.Text;
            selectedOrder.orderDate = DateTime.Parse(OrderDateTextBox.Text);
            selectedOrder.discountPercent = decimal.Parse(DiscountTextBox.Text);
            selectedOrder.totalAmount = decimal.Parse(TotalAmountTextBox.Text);
            selectedOrder.deliveryDate = string.IsNullOrWhiteSpace(DeliveryDateTextBox.Text)
                ? (DateTime?)null
                : DateTime.Parse(DeliveryDateTextBox.Text);

            var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7072/ordersupdate")
            {
                Content = new StringContent(JsonSerializer.Serialize(selectedOrder), Encoding.UTF8, "application/json")
            };

            var response = await UserSession.Instance.SendAuthorizedRequest(() => request);

            if (response.IsSuccessStatusCode)
                MessageBox.Show("Изменения сохранены", "Заказы", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show("Ошибка сохранения", "Заказы", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public class OrderModel
    {
        public int orderId { get; set; }
        public int clientId { get; set; }
        public string clientName { get; set; }
        public string address { get; set; }
        public DateTime orderDate { get; set; }
        public decimal discountPercent { get; set; }
        public decimal totalAmount { get; set; }
        public DateTime? deliveryDate { get; set; }
    }

}

