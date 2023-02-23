using LiteDB;
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
using System.Windows.Shapes;
using static Billing_tool.Window1;

namespace Billing_tool
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        String src = AppDomain.CurrentDomain.BaseDirectory + "\\Data.db";
        public Window2()
        {
            InitializeComponent();
        }

        public class FacBedrijfS
        {
            public int Id { get; set; }
            public String facBedrijf { get; set; }
            public String facAdres { get; set; }
            public int facPostcode { get; set; }
            public String facStad { get; set; }
            public String facLand { get; set; }
            public String facTelefoon { get; set; }
            public String facEmail { get; set; }
            public String facWebsite { get; set; }
            public String facBTW { get; set; }
            public String facIBAN { get; set; }
            public String facBIC { get; set; }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (txtBedrijf.Text != String.Empty &&
                           txtAdres.Text != String.Empty &&
                           txtPostcode.Text != String.Empty &&
                           txtStad.Text != String.Empty &&
                           txtLand.Text != String.Empty &&
                           txtTelefoon.Text != String.Empty &&
                           txtEmail.Text != String.Empty &&
                           txtWebsite.Text != String.Empty &&
                           txtBTW.Text != String.Empty &&
                           txtIBAN.Text != String.Empty &&
                           txtBIC.Text != String.Empty)
            {
                int iPostcode;
                bool test = Int32.TryParse(txtPostcode.Text, out iPostcode);
                if (test)
                {
                    using (var db = new LiteDatabase(src))
                    {
                        var col = db.GetCollection<FacBedrijfS>("BedrijvenSettings");

                        // Create your new customer instance
                        var bedrijf = new FacBedrijfS
                        {
                            facBedrijf = txtBedrijf.Text,
                            facAdres = txtAdres.Text,
                            facPostcode = iPostcode,
                            facStad = txtStad.Text,
                            facLand = txtLand.Text,
                            facTelefoon = txtTelefoon.Text,
                            facEmail = txtEmail.Text,
                            facWebsite = txtWebsite.Text,
                            facBTW = txtBTW.Text,
                            facIBAN = txtIBAN.Text,
                            facBIC = txtBIC.Text,
                        };

                        // Insert new customer document (Id will be auto-incremented)
                        col.Insert(bedrijf);
                    }
                (Application.Current.MainWindow as MainWindow).presetUpdateS();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Please enter a correct postal code.");
                }
            }
            else
            {
                MessageBox.Show("Vul alle velden in aub.");
            }
        }
    }
}
