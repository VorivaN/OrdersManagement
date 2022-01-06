using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Windows.Shapes;

namespace OrdersManagement1
{
    /// <summary>
    /// Interaction logic for Orders.xaml
    /// </summary>
    public partial class Orders : Window
    {
        public Orders()
        {
            InitializeComponent();
        }

        private void UpdateForm()
        {
            using (var model = new OrdersDataClassesDataContext(ConfigurationManager.ConnectionStrings["OrdersManagement1.Properties.Settings.OrdersManagementConnectionString"].ConnectionString))
            {
                var items = from order in model.ORDERs
                            select new
                            {
                                Id = order.Id,
                                Date = order.Date.ToShortDateString(),
                                Status = order.Status,
                                Customer = order.CUSTOMER.FullName
                            };
                OrdersDataGrid.ItemsSource = items;
            }
            DetailsDataGrid.ItemsSource = null;
        }

        private void OrdersDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateForm();
        }

        private void OrdersDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem == null) return;
            var row = new
            {
                Id = 0,
                Date = "",
                Status = "",
                Customer = ""
            };

            row = Cast(row, OrdersDataGrid.SelectedItem);

            using (var model = new OrdersDataClassesDataContext(ConfigurationManager.ConnectionStrings["OrdersManagement1.Properties.Settings.OrdersManagementConnectionString"].ConnectionString))
            {
                var items = from det in model.ORDER_DETAILs
                            where det.ORDER_Id == row.Id
                            select new
                            {
                                Order = det.ORDER_Id,
                                Article = det.PRODUCT_Article,
                                Quantity = det.Quantity
                            };

                DetailsDataGrid.ItemsSource = items;
            }
        }

        private static T Cast<T>(T typeHolder, Object x)
        {
            // typeHolder above is just for compiler magic
            // to infer the type to cast x to
            return (T)x;
        }

        private void ApplyOrder_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem == null || OrdersDataGrid.SelectedItems.Count != 1)
            {
                MessageBox.Show("Select one order!");
                return;
            }

            var row = new
            {
                Id = 0,
                Date = "",
                Status = "",
                Customer = ""
            };

            row = Cast(row, OrdersDataGrid.SelectedItem);

            if (!row.Status.Equals("Wait"))
            {
                MessageBox.Show("Status must be 'Wait'");
                return;
            }


            using (var model = new OrdersDataClassesDataContext(ConfigurationManager.ConnectionStrings["OrdersManagement1.Properties.Settings.OrdersManagementConnectionString"].ConnectionString))
            {
                var order = (from ord in model.ORDERs
                             where ord.Id == row.Id
                             select ord).FirstOrDefault();

                var productInStock = from prod in model.PRODUCTs
                                     select prod;

                var orderDetails = from det in model.ORDER_DETAILs
                                   where det.ORDER_Id == order.Id
                                   select det;
                var deficitProductsArticles = from def in orderDetails
                                              where def.Quantity > productInStock.Where(c => c.Article == def.PRODUCT_Article).FirstOrDefault().Quantity
                                              select def.PRODUCT_Article;

                if (deficitProductsArticles.Any())
                {
                    MessageBox.Show(string.Format("Not anough products in stock (articles: {0})", string.Join(",", deficitProductsArticles)));
                    return;
                }

                long sum = (from det in orderDetails
                            select det.Quantity * det.PRODUCT.Cost).Sum();

                if (order.CUSTOMER.Money < sum)
                {
                    MessageBox.Show(string.Format("Customer cannot buy this order (customer money - {0}, sum of order - {1}).", order.CUSTOMER.Money, sum));
                    return;
                }


                order.CUSTOMER.Money -= sum;
                order.Status = "Applied";

                foreach (var det in orderDetails)
                {
                    var prod = productInStock.Where(c => c.Article == det.PRODUCT_Article).FirstOrDefault();
                    prod.Quantity -= det.Quantity;
                }

                model.SubmitChanges();
                UpdateForm();
            }
        }

        private void NewOrder_Click(object sender, RoutedEventArgs e)
        {
            var newOrder = new NewOrder();
            newOrder.ShowDialog();
            UpdateForm();
        }
    }
}
