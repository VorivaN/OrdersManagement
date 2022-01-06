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
    /// Interaction logic for NewOrder.xaml
    /// </summary>
    public partial class NewOrder : Window
    {
        public NewOrder()
        {
            InitializeComponent();
        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (var model = new OrdersDataClassesDataContext(ConfigurationManager.ConnectionStrings["OrdersManagement1.Properties.Settings.OrdersManagementConnectionString"].ConnectionString))
            {
                var customers = from customer in model.CUSTOMERs
                                select customer;

                var products = from product in model.PRODUCTs
                               select product;

                CustomerComboBox.ItemsSource = customers;
                ProductComboBox.ItemsSource = products;
            }
        }

        List<ORDER_DETAIL> details = new List<ORDER_DETAIL>();

        private void AddDetailButton_Click(object sender, RoutedEventArgs e)
        {
            if(ProductComboBox.SelectedItem == null)
            {
                MessageBox.Show("Select product");
                return;
            }
            int a;
            if (!int.TryParse(QuantityTextBox.Text, out a))
            {
                MessageBox.Show("Value in Quantity text box is not integer");
                return;
            }

            if (details.Where(c => c.PRODUCT_Article.Equals(((PRODUCT)ProductComboBox.SelectedItem).Article)).Any())
            {
                MessageBox.Show("This product already exists in order");
                return;
            }

            details.Add(new ORDER_DETAIL { PRODUCT = (PRODUCT)ProductComboBox.SelectedItem, Quantity = int.Parse(QuantityTextBox.Text) });

            DetailsDataGrid.ItemsSource = from detail in details
                                          select new { Product = detail.PRODUCT_Article, Quantity = detail.Quantity };
        }

        private static T Cast<T>(T typeHolder, Object x)
        {
            // typeHolder above is just for compiler magic
            // to infer the type to cast x to
            return (T)x;
        }


        private void DeleteSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            if (DetailsDataGrid.SelectedItem == null || DetailsDataGrid.SelectedItems.Count != 1)
            {
                MessageBox.Show("Select one detail");
                return;
            }
            var row = new
            {
                Product = "",
                Quantity = 0L
            };
            row = Cast(row, DetailsDataGrid.SelectedItem);

            details.RemoveAll(c => c.PRODUCT_Article.Equals(row.Product));

            DetailsDataGrid.ItemsSource = from detail in details
                                          select new { Product = detail.PRODUCT_Article, Quantity = detail.Quantity };
        }

        private void CreateOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (details.Count < 1)
            {
                MessageBox.Show("Select one or more details");
                return;
            }
            if (CustomerComboBox.SelectedItem == null)
            {
                MessageBox.Show("Select customer");
                return;
            }

            using (var model = new OrdersDataClassesDataContext(ConfigurationManager.ConnectionStrings["OrdersManagement1.Properties.Settings.OrdersManagementConnectionString"].ConnectionString))
            {
                var newOrder = new ORDER();
                newOrder.CUSTOMER_Id = ((CUSTOMER)CustomerComboBox.SelectedItem).Id;
                newOrder.Date = DateTime.Now;
                newOrder.Status = "Wait";

                model.ORDERs.InsertOnSubmit(newOrder);
                model.SubmitChanges();

                foreach (var detail in details)
                {
                    var newDetail = new ORDER_DETAIL();
                    newDetail.ORDER = newOrder;
                    newDetail.PRODUCT_Article = detail.PRODUCT_Article;
                    newDetail.Quantity = detail.Quantity;
                    model.ORDER_DETAILs.InsertOnSubmit(newDetail);
                }
                model.SubmitChanges();
            }

            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
