using LiteDB;
using System;
using System.Windows;

namespace Billing_tool
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        String src = AppDomain.CurrentDomain.BaseDirectory + "\\Data.db";
        public Window1()
        {
            InitializeComponent();
        }

        public class FacBedrijf
        {
            public int Id { get; set; }
            public String facBedrijf { get; set; }
            public String facContact { get; set; }
            public String facAdres { get; set; }
            public int facPostcode { get; set; }
            public String facStad { get; set; }
            public String facLand { get; set; }
            public String facBTW { get; set; }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (txtBedrijf.Text != String.Empty &&
                txtContactpersoon.Text != String.Empty &&
                txtAdres.Text != String.Empty &&
                txtPostcode.Text != String.Empty &&
                txtStad.Text != String.Empty &&
                txtLand.Text != String.Empty &&
                txtBTW.Text != String.Empty)
            {
                int iPostcode;
                bool test = Int32.TryParse(txtPostcode.Text, out iPostcode);
                if (test)
                {
                    using (var db = new LiteDatabase(src))
                    {
                        var col = db.GetCollection<FacBedrijf>("Bedrijven");

                        // Create your new customer instance
                        var bedrijf = new FacBedrijf
                        {
                            facBedrijf = txtBedrijf.Text,
                            facContact = txtContactpersoon.Text,
                            facAdres = txtAdres.Text,
                            facPostcode = iPostcode,
                            facStad = txtStad.Text,
                            facLand = txtLand.Text,
                            facBTW = txtBTW.Text
                        };

                        // Insert new customer document (Id will be auto-incremented)
                        col.Insert(bedrijf);
                    }
                (Application.Current.MainWindow as MainWindow).presetUpdate();
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
