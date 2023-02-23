using LiteDB;
using Microsoft.Office.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using static Billing_tool.Window1;
using Excel = Microsoft.Office.Interop.Excel;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Timer = System.Timers.Timer;

namespace Billing_tool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IniFile MyIni = new IniFile("Settings.ini");
        Timer timer = new Timer(1000);
        String strDatum, strFoto, facnummercheck, strFile;
        Double dblTotFinal = 0;
        List<Factuur> users = new List<Factuur>();
        String strDatumFix = String.Empty;
        String facDatacheck;
        String src = AppDomain.CurrentDomain.BaseDirectory + "\\Data.db";
        public MainWindow()
        {
            InitializeComponent();
            grdInfo.Visibility = Visibility.Hidden;
            grdSettings.Visibility = Visibility.Hidden;
            settingsChange();
            cmbTax.Items.Add("21%");
            cmbTax.Items.Add("15%");
            cmbTax.Items.Add("6%");
            cmbTax.Items.Add("0%");
            cmbTax.SelectedIndex = 0;
            dtDatum.SelectedDate = DateTime.Today;
            strDatum = dtDatum.SelectedDate.ToString();
            strDatum = strDatum.Remove(strDatum.Length - 8);
            txtFactuurnummer.Text = "test";
            txtFactuurnummer.IsEnabled = false;
            txtTotaal.IsEnabled = false;
            txtLand.Text = "België";
            btnGen.IsEnabled = false;
            timer.Start();
            timer.Elapsed += OnTick; // Which can also be written as += new ElapsedEventHandler(OnTick);
            fcNummerUpdate();
            presetUpdate();
            presetUpdateS();
            presetItems();
        }

        public void presetItems()
        {
            try
            {
                cmbOmschrijving.Items.Clear();
                using (var db = new LiteDatabase(src))
                {
                    var col = db.GetCollection<FacItems>("Items");
                    int i = col.Count();
                    for (int i2 = 1; i2 <= i; i2++)
                    {
                        using (var bsonReader = db.Execute("SELECT facOmschrijving FROM Items WHERE _id = " + i2 + ";"))
                        {
                            var output = String.Empty;
                            while (bsonReader.Read())
                            {
                                output = bsonReader.Current.ToString();
                            }
                            output = output.Substring(20, output.Length - 22);
                            cmbOmschrijving.Items.Add(output);
                        }
                    }
                    if (i == 1)
                    {
                        lblItems.Content = "There is " + i + " item in the list.";
                    }
                    else
                    {
                        lblItems.Content = "There are " + i + " items in the list.";
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

        public void presetUpdate()
        {
            cmbInfo.Items.Clear();
            using (var db = new LiteDatabase(src))
            {
                var col = db.GetCollection<FacBedrijf>("Bedrijven");
                int i = col.Count();
                for (int i2 = 1; i2 <= i; i2++)
                {
                    using (var bsonReader = db.Execute("SELECT facBedrijf FROM Bedrijven WHERE _id = " + i2 + ";"))
                    {
                        var output = String.Empty;
                        while (bsonReader.Read())
                        {
                            output = bsonReader.Current.ToString();
                        }
                        output = output.Substring(15, output.Length - 17);
                        cmbInfo.Items.Add(output);
                    }
                }
            }
        }

        public void presetUpdateS()
        {
            cmbSettings.Items.Clear();
            using (var db = new LiteDatabase(src))
            {
                var col = db.GetCollection<FacBedrijfS>("BedrijvenSettings");
                int i = col.Count();
                for (int i2 = 1; i2 <= i; i2++)
                {
                    using (var bsonReader = db.Execute("SELECT facBedrijf FROM BedrijvenSettings WHERE _id = " + i2 + ";"))
                    {
                        var output = String.Empty;
                        while (bsonReader.Read())
                        {
                            output = bsonReader.Current.ToString();
                        }
                        output = output.Substring(15, output.Length - 17);
                        cmbSettings.Items.Add(output);
                    }
                }
            }
        }
        private void dbUpdate()
        {
            using (var db = new LiteDatabase(src))
            {
                var col = db.GetCollection<FacData>("_" + facDatacheck);

                // Create your new customer instance
                var factuur = new FacData
                {
                    facNummer = Convert.ToInt32(facnummercheck),
                    facKlant = txtBedrijf.Text
                };

                // Insert new customer document (Id will be auto-incremented)
                col.Insert(factuur);
            }
        }
        private void btnGen_Click(object sender, RoutedEventArgs e)
        {
            btnGen.IsEnabled = false;
            Excel.Application openExcel;
            Excel._Workbook openWerkboek;
            Excel._Worksheet openBlad;
            Excel.Range openRange;

            try
            {
                //Start excel
                openExcel = new Excel.Application();
                openExcel.Visible = false;

                //Nieuw werkboek
                openWerkboek = (Excel._Workbook)(openExcel.Workbooks.Add(Missing.Value));
                openBlad = (Excel._Worksheet)openWerkboek.ActiveSheet;

                //standaardinfo
                openBlad.Cells[1, 1] = txtBedrijfS.Text;
                openBlad.Cells[2, 1] = txtAdresS.Text;
                openBlad.Cells[3, 1] = txtPostcodeS.Text + txtAdresS.Text;
                openBlad.Cells[4, 1] = txtLandS.Text;

                openBlad.Cells[6, 1] = txtTelefoonS.Text;
                openBlad.Cells[7, 1] = txtEmailS.Text;
                openBlad.Cells[8, 1] = txtWebsiteS.Text;

                openBlad.Cells[10, 1] = txtBTWS.Text;
                openBlad.Cells[11, 1] = txtIBANS.Text;
                openBlad.Cells[12, 1] = txtBICS.Text;

                openBlad.Cells[10, 5] = txtBedrijf.Text;
                openBlad.Cells[11, 5] = txtContactpersoon.Text;
                openBlad.Cells[12, 5] = txtAdres.Text;
                openBlad.Cells[13, 5] = txtStad.Text;
                openBlad.Cells[14, 5] = txtLand.Text;
                openBlad.Cells[15, 5] = txtBTW.Text;

                //factuurinfo
                openBlad.Cells[15, 1] = "Factuur";
                openBlad.Cells[16, 1] = "Factuurnummer: " + txtFactuurnummer.Text;
                openBlad.Cells[17, 1] = "Factuurdatum: " + strDatum;

                openBlad.Cells[19, 1] = "Aantal";
                openBlad.Cells[19, 2] = "Omschrijving";
                openBlad.Cells[19, 3] = "Eenheidsprijs";
                openBlad.Cells[19, 4] = "Subtotaal";
                openBlad.Cells[19, 5] = "BTW%";
                openBlad.Cells[19, 6] = "BTW";
                openBlad.Cells[19, 7] = "Totaal";

                //fill with values
                //openBlad.get_Range("A2", "B6").Value = saNames;
                int i = 20;
                var Cell1 = String.Empty;
                foreach (var item in users)
                {
                    for (int i2 = 0; i2 < 7; i2++)
                    {
                        switch (i2)
                        {
                            case 0:
                                Cell1 = "A" + i;
                                openBlad.get_Range(Cell1).Value = item.Aantal;
                                break;
                            case 1:
                                Cell1 = "B" + i;
                                openBlad.get_Range(Cell1).Value = item.Omschrijving;
                                break;
                            case 2:
                                Cell1 = "C" + i;
                                openBlad.get_Range(Cell1).Value = Convert.ToDouble(item.Eenheidsprijs);
                                openBlad.get_Range(Cell1).Style = "Currency";
                                break;
                            case 3:
                                Cell1 = "D" + i;
                                openBlad.get_Range(Cell1).Value = Convert.ToDouble(item.Subtotaal);
                                openBlad.get_Range(Cell1).Style = "Currency";
                                break;
                            case 4:
                                Cell1 = "E" + i;
                                openBlad.get_Range(Cell1).Value = item.BTW;
                                break;
                            case 5:
                                Cell1 = "F" + i;
                                openBlad.get_Range(Cell1).Value = Convert.ToDouble(item.BTW_Prijs);
                                openBlad.get_Range(Cell1).Style = "Currency";
                                break;
                            case 6:
                                Cell1 = "G" + i;
                                openBlad.get_Range(Cell1).Value = Convert.ToDouble(item.Totaal);
                                openBlad.get_Range(Cell1).Style = "Currency";
                                break;
                        }
                    }
                    i++;
                }
                //calculate total
                i++;
                Cell1 = "G" + i;
                var Cell2 = "F" + i;
                openBlad.get_Range(Cell2).Value = "Totaal";
                openBlad.get_Range(Cell1).Value = dblTotFinal;
                openBlad.get_Range(Cell1).Style = "Currency";

                //fit columns
                openRange = openBlad.get_Range("C1", "D1");
                openRange.EntireColumn.AutoFit();
                openRange = openBlad.get_Range("A1", "G1");
                //openRange.EntireColumn.HorizontalAlignment = HorizontalAlignment.Left;
                openRange = openBlad.get_Range("B1");
                openRange.EntireColumn.ColumnWidth = 27;
                openRange = openBlad.get_Range("A1");
                openRange.EntireColumn.ColumnWidth = 6;

                //design
                openBlad.get_Range("A19", "G19").Font.Bold = true;
                openBlad.get_Range("A15").Font.Size = 16;
                openBlad.get_Range("A16", "A17").Font.Size = 12;
                openBlad.get_Range(Cell2).Font.Size = 14;
                openBlad.get_Range(Cell2).Font.Bold = true;
                openBlad.get_Range("A19", "G19").Interior.Color = Excel.XlRgbColor.rgbLightSteelBlue;
                Cell1 = "G" + (i - 2);
                openBlad.get_Range("A20", Cell1).Interior.Color = Excel.XlRgbColor.rgbLavender;
                openBlad.Shapes.AddPicture(strFoto, MsoTriState.msoFalse, MsoTriState.msoTrue, 250, 0, (float)imgFoto.Width, (float)imgFoto.ActualHeight);

                //file save
                strFile = facnummercheck + " " + txtBedrijf.Text;
                openWerkboek.ExportAsFixedFormat2(Excel.XlFixedFormatType.xlTypePDF, MyIni.Read("BillPath") + "\\" + strFile);
                openWerkboek.Saved = true;
                openWerkboek.Close();
                openExcel.Quit();

                //write to source
                dbUpdate();
                //update factuurnummer voor volgende
                fcNummerUpdate();

                //open pdf
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                process.StartInfo = startInfo;
                startInfo.FileName = MyIni.Read("BillPath") + "\\" + strFile + ".pdf";
                startInfo.WorkingDirectory = MyIni.Read("BillPath") + "\\";
                startInfo.UseShellExecute = true;
                process.Start();
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

        private void fcNummerUpdate()
        {

            var charsToRemove = new string[] { "/" };
            foreach (var c in charsToRemove)
            {
                strDatumFix = strDatum.Replace(c, string.Empty);
            }
            if (strDatumFix.Length < 8)
            {
                strDatumFix = "0" + strDatumFix;
            }

            facDatacheck = strDatumFix.Substring(6, 2) + strDatumFix.Substring(2, 2) + strDatumFix.Substring(0, 2);
            using (var db = new LiteDatabase(src))
            {
                var col = db.GetCollection<FacData>("_" + facDatacheck);

                // Create your new customer instance
                var factuur = new FacData
                {
                    facNummer = 1,
                    facKlant = String.Empty
                };
                col.Insert(factuur);
                int i = col.Count();
                col.Delete(i);

                i = col.Count();
                facnummercheck = facDatacheck + (i + 1);
                txtFactuurnummer.Text = facnummercheck.ToString();
            }
        }

        private void btnToevoegen_Click(object sender, RoutedEventArgs e)
        {
            if (txtAantal.Text == "" || cmbOmschrijving.Text == "" || txtPrijs.Text == "")
            {
                MessageBox.Show("Vul aub alle gegevens in");
                return;
            }
            Double dblTotaal, dblProcent, dblBTW, dblTot;
            Double.TryParse(txtTotaal.Text, out dblTotaal);
            dblProcent = 0;
            switch (cmbTax.SelectedIndex)
            {
                case 0:
                    dblProcent = 21;
                    break;
                case 1:
                    dblProcent = 15;
                    break;
                case 2:
                    dblProcent = 6;
                    break;
                case 3:
                    dblProcent = 0;
                    break;
            }
            dblBTW = Math.Round(dblTotaal / 100 * dblProcent, 3);
            dblTot = dblTotaal + dblBTW;
            dblTotFinal += dblTot;
            users.Add(new Factuur
            {
                Aantal = txtAantal.Text,
                Omschrijving = cmbOmschrijving.Text,
                Eenheidsprijs = txtPrijs.Text,
                Subtotaal = txtTotaal.Text,
                BTW = cmbTax.SelectedItem.ToString(),
                BTW_Prijs = dblBTW.ToString(),
                Totaal = dblTot.ToString()
            });
            dgFactuur.ItemsSource = null;
            dgFactuur.ItemsSource = users;
        }

        public class FacItems
        {
            public int Id { get; set; }
            public String facOmschrijving { get; set; }
            public Double facPrijs { get; set; }
        }

        public class Factuur
        {
            public string Aantal { get; set; }
            public string Omschrijving { get; set; }
            public string Eenheidsprijs { get; set; }
            public string Subtotaal { get; set; }
            public string BTW { get; set; }
            public string BTW_Prijs { get; set; }
            public string Totaal { get; set; }

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

        private void btnFoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;...";
            dialog.InitialDirectory = @"C:\";
            dialog.Title = "Please select an image file.";
            Nullable<bool> pad = dialog.ShowDialog();
            if (pad == true)
            {
                strFoto = dialog.FileName;
            }
            txtLogo.Text = strFoto;
            imgFoto.Source = new BitmapImage(new Uri(strFoto));
            MyIni.Write("LogoSource", strFoto);
        }

        private void btnVerwijderen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                users.RemoveAt(dgFactuur.SelectedIndex);
                dgFactuur.ItemsSource = null;
                dgFactuur.ItemsSource = users;
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

        private void cmbInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (var db = new LiteDatabase(src))
            {
                var col = db.GetCollection<FacBedrijf>("Bedrijven");
                int i = cmbInfo.SelectedIndex + 1;
                using (var bsonReader = db.Execute("SELECT * FROM Bedrijven WHERE _id = " + i + "; "))
                {
                    var output = String.Empty;
                    while (bsonReader.Read())
                    {
                        output = bsonReader.Current.ToString();
                    }
                    string[] items = output.Split(':');
                    String itm;
                    int i2 = 0;
                    foreach (string item in items)
                    {
                        switch (i2)
                        {
                            case 3:
                                itm = item.Substring(1, item.Length - 15);
                                txtBedrijf.Text = itm;
                                i2++;
                                break;
                            case 4:
                                itm = item.Substring(1, item.Length - 13);
                                txtContactpersoon.Text = itm;
                                i2++;
                                break;
                            case 5:
                                itm = item.Substring(1, item.Length - 16);
                                txtAdres.Text = itm;
                                i2++;
                                break;
                            case 6:
                                itm = item.Substring(0, item.Length - 10);
                                txtStad.Text = itm;
                                i2++;
                                break;
                            case 7:
                                itm = item.Substring(1, item.Length - 12);
                                txtStad.Text += " " + itm;
                                i2++;
                                break;
                            case 8:
                                itm = item.Substring(1, item.Length - 11);
                                txtLand.Text = itm;
                                i2++;
                                break;
                            case 9:
                                itm = item.Substring(1, item.Length - 5);
                                txtBTW.Text = itm;
                                i2++;
                                break;
                            default:
                                i2++;
                                break;
                        }
                    }
                    i2 = 0;
                }
            }
        }

        private void btnInfoAdd_Click(object sender, RoutedEventArgs e)
        {
            Window1 window = new Window1();
            window.Show();
        }

        public class FacData
        {
            public int Id { get; set; }
            public int facNummer { get; set; }
            public String facKlant { get; set; }
        }

        private void btnBill_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.InitialDirectory = @"C:\";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MyIni.Write("BillPath", dialog.SelectedPath);
                txtBillPath.Text = MyIni.Read("BillPath");
            }
        }

        private void settingsChange()
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Settings.ini") == false)
            {
                bool test = Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\Bills");
                if (!test)
                {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\Bills");
                    MyIni.Write("BillPath", AppDomain.CurrentDomain.BaseDirectory + "\\Bills");
                }
                MyIni.Write("LogoSource", AppDomain.CurrentDomain.BaseDirectory + "\\E-17_CRONOS-GROEP_BLUE-POS_W.png");
            }

            txtBillPath.Text = MyIni.Read("BillPath");

            try
            {
                strFoto = MyIni.Read("LogoSource");
                txtLogo.Text = strFoto;
                imgFoto.Source = new BitmapImage(new Uri(strFoto));
                imgFoto.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                imgFoto.Arrange(new Rect(new Point(0, 0), imgFoto.DesiredSize));
            }
            catch (Exception theException)
            {
                String errorMessage;
                errorMessage = "Error: ";
                errorMessage = String.Concat(errorMessage, theException.Message);
                errorMessage = String.Concat(errorMessage, " Line: ");
                errorMessage = String.Concat(errorMessage, theException.Source);
                errorMessage = String.Concat(errorMessage, " Settings file missing or a setting is set incorrectly.");
                System.Windows.MessageBox.Show(errorMessage, "Error");
            }
        }

        private void dtDatum_CalendarClosed(object sender, RoutedEventArgs e)
        {
            strDatum = dtDatum.SelectedDate.ToString();
            strDatum = strDatum.Remove(strDatum.Length - 8);
            fcNummerUpdate();
        }

        private void txtAantal_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtPrijs.Text != "")
            {
                Double dblAantal, dblPrijs;
                Double.TryParse(txtAantal.Text, out dblAantal);
                Double.TryParse(txtPrijs.Text, out dblPrijs);
                txtTotaal.Text = (dblAantal * dblPrijs).ToString();
            }
        }

        private void txtPrijs_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtAantal.Text != "")
            {
                Double dblAantal, dblPrijs;
                Double.TryParse(txtAantal.Text, out dblAantal);
                Double.TryParse(txtPrijs.Text, out dblPrijs);
                txtTotaal.Text = (dblAantal * dblPrijs).ToString();
            }
        }


        private void cmbSettings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (var db = new LiteDatabase(src))
            {
                var col = db.GetCollection<FacBedrijfS>("BedrijvenSettings");
                int i = cmbSettings.SelectedIndex + 1;
                using (var bsonReader = db.Execute("SELECT * FROM BedrijvenSettings WHERE _id = " + i + "; "))
                {
                    var output = String.Empty;
                    while (bsonReader.Read())
                    {
                        output = bsonReader.Current.ToString();
                    }
                    string[] items = output.Split(':');
                    String itm;
                    int i2 = 0;
                    foreach (string item in items)
                    {
                        switch (i2)
                        {
                            case 3:
                                itm = item.Substring(1, item.Length - 13);
                                txtBedrijfS.Text = itm;
                                i2++;
                                break;
                            case 4:
                                itm = item.Substring(1, item.Length - 16);
                                txtAdresS.Text = itm;
                                i2++;
                                break;
                            case 5:
                                itm = item.Substring(0, item.Length - 10);
                                txtPostcodeS.Text = itm;
                                i2++;
                                break;
                            case 6:
                                itm = item.Substring(1, item.Length - 12);
                                txtStadS.Text = itm;
                                i2++;
                                break;
                            case 7:
                                itm = item.Substring(1, item.Length - 16);
                                txtLandS.Text = itm;
                                i2++;
                                break;
                            case 8:
                                itm = item.Substring(1, item.Length - 13);
                                txtTelefoonS.Text = itm;
                                i2++;
                                break;
                            case 9:
                                itm = item.Substring(1, item.Length - 15);
                                txtEmailS.Text = itm;
                                i2++;
                                break;
                            case 10:
                                itm = item.Substring(1, item.Length - 11);
                                txtWebsiteS.Text = itm;
                                i2++;
                                break;
                            case 11:
                                itm = item.Substring(1, item.Length - 12);
                                txtBTWS.Text = itm;
                                i2++;
                                break;
                            case 12:
                                itm = item.Substring(1, item.Length - 11);
                                txtIBANS.Text = itm;
                                i2++;
                                break;
                            case 13:
                                itm = item.Substring(1, item.Length - 5);
                                txtBICS.Text = itm;
                                i2++;
                                break;
                            default:
                                i2++;
                                break;
                        }
                    }
                    i2 = 0;
                }
            }
        }

        private void btnItems_Click(object sender, RoutedEventArgs e)
        {
            Items window = new Items();
            window.Show();
        }

        private void btnSettingsAdd_Click(object sender, RoutedEventArgs e)
        {
            Window2 window = new Window2();
            window.Show();
        }

        private void btnFactuur_Click(object sender, RoutedEventArgs e)
        {
            grdFactuur.Visibility = Visibility.Visible;
            grdInfo.Visibility = Visibility.Hidden;
            grdSettings.Visibility = Visibility.Hidden;
        }

        private void cmbOmschrijving_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                txtPrijs.Clear();
                using (var db = new LiteDatabase(src))
                {
                    var col = db.GetCollection<FacItems>("Items");
                    int id = cmbOmschrijving.SelectedIndex + 1;
                    using (var bsonReader = db.Execute("SELECT facPrijs FROM Items WHERE _id = " + id + ";"))
                    {
                        var output = String.Empty;
                        while (bsonReader.Read())
                        {
                            output = bsonReader.Current.ToString();
                        }
                        output = output.Substring(12, output.Length - 13);
                        txtPrijs.Text = output;
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

        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {
            grdFactuur.Visibility = Visibility.Hidden;
            grdInfo.Visibility = Visibility.Visible;
            grdSettings.Visibility = Visibility.Hidden;
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            grdFactuur.Visibility = Visibility.Hidden;
            grdInfo.Visibility = Visibility.Hidden;
            grdSettings.Visibility = Visibility.Visible;
        }
        public void OnTick(object source, ElapsedEventArgs e)
        {

            this.Dispatcher.Invoke(() =>
            {
                if ((dgFactuur.Items.Count != 0) &&
                    (txtBedrijf.Text != String.Empty) &&
                    (txtContactpersoon.Text != String.Empty) &&
                    (txtAdres.Text != String.Empty) &&
                    (txtStad.Text != String.Empty) &&
                    (txtLand.Text != String.Empty) &&
                    (txtBTW.Text != String.Empty) &&
                    (txtAdresS.Text != String.Empty) &&
                    (txtPostcodeS.Text != String.Empty) &&
                    (txtStadS.Text != String.Empty) &&
                    (txtLandS.Text != String.Empty) &&
                    (txtTelefoonS.Text != String.Empty) &&
                    (txtEmailS.Text != String.Empty) &&
                    (txtWebsiteS.Text != String.Empty) &&
                    (txtBTWS.Text != String.Empty) &&
                    (txtIBANS.Text != String.Empty) &&
                    (txtBICS.Text != String.Empty)
                    )
                {
                    btnGen.IsEnabled = true;
                }
            });
        }
    }
}
