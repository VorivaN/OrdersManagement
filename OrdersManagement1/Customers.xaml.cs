using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OrdersManagement1
{
    /// <summary>
    /// Interaction logic for Customers.xaml
    /// </summary>
    public partial class Customers : Window
    {
        public Customers()
        {
            InitializeComponent();
        }

        private void UpdateForm()
        {
            using (var model = new OrdersDataClassesDataContext(ConfigurationManager.ConnectionStrings["OrdersManagement1.Properties.Settings.OrdersManagementConnectionString"].ConnectionString))
            {
                var customers = from cust in model.CUSTOMERs
                                select new { Id = cust.Id, FullName = cust.FullName, Money = cust.Money };
                CustomersDataGrid.ItemsSource = customers;
            }
        }
        private void NewCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            int a;
            if (!int.TryParse(MoneyTextBox.Text, out a))
            {
                MessageBox.Show("Money must be integer");
                return;
            }
            if (string.IsNullOrEmpty(FullNameTextBox.Text))
            {
                MessageBox.Show("FullName must be filled in");
                return;
            }
            using (var model = new OrdersDataClassesDataContext(ConfigurationManager.ConnectionStrings["OrdersManagement1.Properties.Settings.OrdersManagementConnectionString"].ConnectionString))
            {
                var newCustomer = new CUSTOMER();
                newCustomer.FullName = FullNameTextBox.Text;
                newCustomer.Money = a;

                model.CUSTOMERs.InsertOnSubmit(newCustomer);
                model.SubmitChanges();
            }
            UpdateForm();
        }

        private void CustomersDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateForm();
        }
        private static T Cast<T>(T typeHolder, Object x)
        {
            // typeHolder above is just for compiler magic
            // to infer the type to cast x to
            return (T)x;
        }
        private void DeleteCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            if (CustomersDataGrid.SelectedItem == null || CustomersDataGrid.SelectedItems.Count != 1)
            {
                MessageBox.Show("Select one order!");
                return;
            }

            var row = new
            {
                Id = 0,
                FullName = "",
                Money = 0L
            };

            row = Cast(row, CustomersDataGrid.SelectedItem);

            using (var model = new OrdersDataClassesDataContext(ConfigurationManager.ConnectionStrings["OrdersManagement1.Properties.Settings.OrdersManagementConnectionString"].ConnectionString))
            {
                var deleted = (from cust in model.CUSTOMERs
                               where cust.Id == row.Id
                               select cust).FirstOrDefault();
                model.CUSTOMERs.DeleteOnSubmit(deleted);

                try
                {
                    model.SubmitChanges();
                }
                catch
                {
                    MessageBox.Show("Cannot delete this customer, because he already have orders");
                }
            }
            UpdateForm();
        }
        private void AddMoneyButton_Click(object sender, RoutedEventArgs e)
        {
            int a;
            if (!int.TryParse(MoneyTextBox.Text, out a))
            {
                MessageBox.Show("Money must be integer");
                return;
            }

            if (CustomersDataGrid.SelectedItem == null || CustomersDataGrid.SelectedItems.Count != 1)
            {
                MessageBox.Show("Select one order!");
                return;
            }

            var row = new
            {
                Id = 0,
                FullName = "",
                Money = 0L
            };

            row = Cast(row, CustomersDataGrid.SelectedItem);

            using (var model = new OrdersDataClassesDataContext(ConfigurationManager.ConnectionStrings["OrdersManagement1.Properties.Settings.OrdersManagementConnectionString"].ConnectionString))
            {
                var addMoney = (from cust in model.CUSTOMERs
                                where cust.Id == row.Id
                                select cust).FirstOrDefault();

                addMoney.Money += a;
                model.SubmitChanges();
            }
            UpdateForm();
        }
    }
}
