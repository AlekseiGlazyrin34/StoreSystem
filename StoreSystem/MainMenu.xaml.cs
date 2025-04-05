using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
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

namespace StoreSystem
{
    /// <summary>
    /// Логика взаимодействия для Request.xaml
    /// </summary>
    public partial class MainMenu : Page
    {
        

        private OrdersPage ordersPage = new OrdersPage();
        private WareHousePage warehousePage = new WareHousePage();
        private ClientsPage clientsPage = new ClientsPage();
        private EmployeesPage employeesPage = new EmployeesPage();
        private PartnersPage partnersPage = new PartnersPage();
        private StoresPage storesPage = new StoresPage();
        

        public MainMenu()
        {
            InitializeComponent();
            RequestFrame.Content = ordersPage;
        }

        private void OrdersTab_Click(object sender, RoutedEventArgs e)
        {
            RequestFrame.Content = ordersPage;
        }

        private void WarehouseTab_Click(object sender, RoutedEventArgs e)
        {
            RequestFrame.Content = warehousePage;
        }

        private void ClientsTab_Click(object sender, RoutedEventArgs e)
        {
            RequestFrame.Content = clientsPage;
        }

        private void EmployeesTab_Click(object sender, RoutedEventArgs e)
        {
            RequestFrame.Content = employeesPage;
        }

        private void PartnersTab_Click(object sender, RoutedEventArgs e)
        {
            RequestFrame.Content = partnersPage;
        }

        private void StoresTab_Click(object sender, RoutedEventArgs e)
        {
            RequestFrame.Content = storesPage;
        }

       
    }
}
