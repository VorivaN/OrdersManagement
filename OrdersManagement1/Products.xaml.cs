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
    /// Interaction logic for Products.xaml
    /// </summary>
    public partial class Products : Window
    {
        public Products()
        {
            InitializeComponent();
        }

        private void UpdateForm()
        {
            using (var model = new OrdersDataClassesDataContext(ConfigurationManager.ConnectionStrings["OrdersManagement1.Properties.Settings.OrdersManagementConnectionString"].ConnectionString))
            {
                var products = from prod in model.PRODUCTs
                               select new { Article = prod.Article, Cost = prod.Cost, Quantity = prod.Quantity };
                ProductsDataGrid.ItemsSource = products;
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateForm();
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ArticleTextBox.Text))
            {

                MessageBox.Show("Article must be filled in");
                return;
            }
            int cost;
            if (!int.TryParse(CostTextBox.Text, out cost))
            {
                MessageBox.Show("Cost must be integer");
                return;
            }

            int quantity;
            if (!int.TryParse(QuantityTextBox.Text, out quantity))
            {
                MessageBox.Show("Quantity must be integer");
                return;
            }

            using (var model = new OrdersDataClassesDataContext(ConfigurationManager.ConnectionStrings["OrdersManagement1.Properties.Settings.OrdersManagementConnectionString"].ConnectionString))
            {
                var prods = from prod in model.PRODUCTs
                            where prod.Article.Equals(ArticleTextBox.Text)
                            select prod;
                if (prods.Any())
                {
                    MessageBox.Show("Product with same articul already exists");
                    return;
                }

                var newProd = new PRODUCT();
                newProd.Article = ArticleTextBox.Text;
                newProd.Cost = cost;
                newProd.Quantity = quantity;

                model.PRODUCTs.InsertOnSubmit(newProd);
                model.SubmitChanges();
            }
            UpdateForm();
        }
        private static T Cast<T>(T typeHolder, Object x)
        {
            // typeHolder above is just for compiler magic
            // to infer the type to cast x to
            return (T)x;
        }

        private void IncButton_Click(object sender, RoutedEventArgs e)
        {
            int quantity;
            if (!int.TryParse(QuantityTextBox.Text, out quantity))
            {
                MessageBox.Show("Quantity must be integer");
                return;
            }

            if (ProductsDataGrid.SelectedItem == null || ProductsDataGrid.SelectedItems.Count != 1)
            {
                MessageBox.Show("Select one order!");
                return;
            }

            var row = new
            {
                Article = "",
                Cost = 0L,
                Quantity = 0L
            };

            row = Cast(row, ProductsDataGrid.SelectedItem);

            using (var model = new OrdersDataClassesDataContext(ConfigurationManager.ConnectionStrings["OrdersManagement1.Properties.Settings.OrdersManagementConnectionString"].ConnectionString))
            {
                var prod = (from pr in model.PRODUCTs
                            where pr.Article.Equals(row.Article)
                            select pr).FirstOrDefault();

                prod.Quantity += quantity;
                model.SubmitChanges();
            }

            UpdateForm();
        }
    }
}
