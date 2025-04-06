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
    public partial class EmployeesPage : Page
    {
        private List<EmployeeModel> employees;
        private EmployeeModel selectedEmployee;

        public EmployeesPage()
        {
            InitializeComponent();
            LoadEmployees();
        }

        private async void LoadEmployees()
        {
            var response = await UserSession.Instance.SendAuthorizedRequest(() =>
                new HttpRequestMessage(HttpMethod.Get, "https://localhost:7072/employees"));

            var json = await response.Content.ReadAsStringAsync();
            employees = JsonSerializer.Deserialize<List<EmployeeModel>>(json);
            EmployeesListView.ItemsSource = employees;
        }

        private void EmployeesListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (EmployeesListView.SelectedItem is EmployeeModel emp)
            {
                selectedEmployee = emp;
                EmployeeIdTextBox.Text = emp.employeeId.ToString();
                EmployeeNameTextBox.Text = emp.name;
                ContactInfoTextBox.Text = emp.contactInfo;
                StoreAddressTextBox.Text = emp.storeName;
            }
        }

        private async void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (selectedEmployee == null) return;

            selectedEmployee.name = EmployeeNameTextBox.Text;
            selectedEmployee.contactInfo = ContactInfoTextBox.Text;
            selectedEmployee.storeName = StoreAddressTextBox.Text;

            var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7072/employeesupdate")
            {
                Content = new StringContent(JsonSerializer.Serialize(selectedEmployee), Encoding.UTF8, "application/json")
            };

            var response = await UserSession.Instance.SendAuthorizedRequest(() => request);

            if (response.IsSuccessStatusCode)
                MessageBox.Show("Изменения сохранены", "Сотрудники", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show("Ошибка при сохранении", "Сотрудники", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public class EmployeeModel
    {
        public int employeeId { get; set; }
        public string name { get; set; }
        public int storeId { get; set; }
        public string storeName { get; set; }

        public string contactInfo { get; set; }
        
    }
}
