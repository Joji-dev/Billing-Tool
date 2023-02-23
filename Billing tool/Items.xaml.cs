using LiteDB;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using static Billing_tool.MainWindow;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace Billing_tool
{
    /// <summary>
    /// Interaction logic for Items.xaml
    /// </summary>
    public partial class Items : Window
    {
        String src = AppDomain.CurrentDomain.BaseDirectory + "\\Data.db";
        int id;

        public Items()
        {
            InitializeComponent();
            listUpdate();
        }

        public class FacItems
        {
            public int Id { get; set; }
            public String facOmschrijving { get; set; }
            public Double facPrijs { get; set; }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (txtOmschrijving.Text != String.Empty &&
                           txtPrijs.Text != String.Empty)
            {
                Double dblPrijs;
                bool test = Double.TryParse(txtPrijs.Text, out dblPrijs);
                if (test)
                {
                    using (var db = new LiteDatabase(src))
                    {
                        var col = db.GetCollection<FacItems>("Items");

                        // Create your new customer instance
                        var item = new FacItems
                        {
                            facOmschrijving = txtOmschrijving.Text,
                            facPrijs = dblPrijs
                        };

                        // Insert new customer document (Id will be auto-incremented)
                        col.Insert(item);
                    }
                (Application.Current.MainWindow as MainWindow).presetItems();
                    listUpdate();
                }
                else
                {
                    MessageBox.Show("Please enter a correct price.");
                }
            }
            else
            {
                MessageBox.Show("Vul alle velden in aub.");
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new LiteDatabase(src))
                {
                    var col = db.GetCollection<FacItems>("Items");
                    //db.Execute("DELETE Items WHERE _id = " + id + ";");
                    db.RenameCollection("Items", "ItemsOld");
                    var col2 = db.GetCollection<FacItems>("ItemsOld");
                    int i = col2.Count();
                    for (int j = 1; j <= i; j++)
                    {
                        if(id != j)
                        {
                            col.Insert(col2.FindOne(Query.All()));
                            //col.Update(col.FindOne(Query.Ascending()));
                            col2.Delete(j);
                        }
                        else
                        {
                            col2.Delete(j);
                        }
                        //col.Insert(col2.FindOne("_id = *"));
                        //col.InsertBulk(db.Execute("SELECT $ FROM ItemsOld LIMIT 1;"));

                        //col2.Delete(col2.FindOne(x => x.Id == "*").Id);
                    }
                    //col.InsertBulk(col2.FindAll());

                    //int[] v = new int[] {1,2,3,4};
                    //col2.UpdateMany("*", ["_id"] = v);
                    //col2.Update()
                    //RENAME COLLECTION Items TO ItemsOld
                }
            }
            catch (Exception theException)
            {
                String errorMessage;
                errorMessage = "Error: ";
                errorMessage = String.Concat(errorMessage, theException.Message);
                errorMessage = String.Concat(errorMessage, " Line: ");
                errorMessage = String.Concat(errorMessage, theException.Source);

                System.Windows.MessageBox.Show(errorMessage, "Error");
            }
            listUpdate();
        }

        private void lstItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            id = lstItems.SelectedIndex + 1;
        }

        private void listUpdate()
        {
            try
            {
                lstItems.Items.Clear();
                using (var db = new LiteDatabase(src))
                {
                    var col = db.GetCollection<FacItems>("Items");
                    int i = col.Count();
                    for (int i2 = 1; i2 <= i; i2++)
                    {
                        using (var bsonReader = db.Execute("SELECT facOmschrijving, facPrijs FROM Items WHERE _id = " + i2 + ";"))
                        {
                            var output = String.Empty;
                            while (bsonReader.Read())
                            {
                                output = bsonReader.Current.ToString();
                            }
                            String[] iteml = output.Split(":");
                            output = String.Empty;
                            output += iteml[1].Substring(1, iteml[1].Length - 13);
                            output += " - € " + iteml[2].Substring(0, iteml[2].Length - 1);
                            lstItems.Items.Add(output);
                        }
                    }
                }
            }
            catch (Exception theException)
            {
                String errorMessage;
                errorMessage = "Error: ";
                errorMessage = String.Concat(errorMessage, theException.Message);
                errorMessage = String.Concat(errorMessage, " Line: ");
                errorMessage = String.Concat(errorMessage, theException.Source);

                System.Windows.MessageBox.Show(errorMessage, "Error");
            }
        }
    }
}
