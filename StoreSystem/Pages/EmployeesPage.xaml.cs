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
            //LoadEmployees();
        }

        private async void LoadEmployees()
        {
            var response = await UserSession.Instance.SendAuthorizedRequest(() =>
                new HttpRequestMessage(HttpMethod.Get, "https://localhost:7006/api/employees"));

            var json = await response.Content.ReadAsStringAsync();
            employees = JsonSerializer.Deserialize<List<EmployeeModel>>(json);
            EmployeesListView.ItemsSource = employees;
        }

        private void EmployeesListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (EmployeesListView.SelectedItem is EmployeeModel emp)
            {
                selectedEmployee = emp;
                EmployeeIdTextBox.Text = emp.EmployeeId.ToString();
                EmployeeNameTextBox.Text = emp.Name;
                ContactInfoTextBox.Text = emp.ContactInfo;
                StoreAddressTextBox.Text = emp.StoreAddress;
            }
        }

        private async void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (selectedEmployee == null) return;

            selectedEmployee.Name = EmployeeNameTextBox.Text;
            selectedEmployee.ContactInfo = ContactInfoTextBox.Text;
            selectedEmployee.StoreAddress = StoreAddressTextBox.Text;

            var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7006/api/employees/update")
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
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public int StoreId { get; set; }
        public string ContactInfo { get; set; }
        public string StoreAddress { get; set; }
    }
}
