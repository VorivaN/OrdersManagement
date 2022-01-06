using System;
using System.Collections.Generic;
using System.Linq;
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

namespace OrdersManagement1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            var orders = new Orders();
            orders.ShowDialog();
        }

        private void CustomersButton_Click(object sender, RoutedEventArgs e)
        {
            var customers = new Customers();
            customers.ShowDialog();
        }

        private void ProductsButton_Click(object sender, RoutedEventArgs e)
        {
            var products = new Products();
            products.ShowDialog();
        }
    }
}
