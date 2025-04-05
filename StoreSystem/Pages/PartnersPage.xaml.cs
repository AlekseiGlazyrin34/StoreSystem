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

    public partial class PartnersPage : Page
    {
        private List<PartnerModel> partners;
        private PartnerModel selectedPartner;

        public PartnersPage()
        {
            InitializeComponent();
            //LoadPartners();
        }

        private async void LoadPartners()
        {
            var response = await UserSession.Instance.SendAuthorizedRequest(() =>
                new HttpRequestMessage(HttpMethod.Get, "https://localhost:7006/api/partners"));

            var json = await response.Content.ReadAsStringAsync();
            partners = JsonSerializer.Deserialize<List<PartnerModel>>(json);
            PartnersListView.ItemsSource = partners;
        }

        private void PartnersListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (PartnersListView.SelectedItem is PartnerModel partner)
            {
                selectedPartner = partner;
                PartnerNameTextBox.Text = partner.Name;
                PartnerContactInfoTextBox.Text = partner.ContactInfo;
            }
        }

        private async void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPartner == null) return;

            selectedPartner.Name = PartnerNameTextBox.Text;
            selectedPartner.ContactInfo = PartnerContactInfoTextBox.Text;

            var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7006/api/partners/update")
            {
                Content = new StringContent(JsonSerializer.Serialize(selectedPartner), Encoding.UTF8, "application/json")
            };

            var response = await UserSession.Instance.SendAuthorizedRequest(() => request);

            if (response.IsSuccessStatusCode)
                MessageBox.Show("Изменения сохранены", "Партнёры", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show("Ошибка при сохранении", "Партнёры", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public class PartnerModel
    {
        public int PartnerId { get; set; }
        public string Name { get; set; }
        public string ContactInfo { get; set; }
    }

}
