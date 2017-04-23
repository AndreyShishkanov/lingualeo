using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Win32;

namespace LinguaLeo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            var users = new Dictionary<string, string>
            {
                {"user1", "password1"},
                {"user2", "password2"}
            };

            var fileDialog = new OpenFileDialog();
            var result = fileDialog.ShowDialog()??false;
            if (result)
            {
                var file = fileDialog.FileName;
                DataTable tbl = new DataTable();
                using (var pck = new OfficeOpenXml.ExcelPackage())
                {
                    using (var stream = System.IO.File.OpenRead(file))
                    {
                        pck.Load(stream);
                    }
                    var ws = pck.Workbook.Worksheets.First();

                    tbl.Columns.Add(ws.Cells[1, 1, 1, 1].Text);
                    tbl.Columns.Add(ws.Cells[1, 3, 1, 3].Text);

                    var startRow = 2;
                    for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                    {
                        DataRow row = tbl.Rows.Add();
                        row[0] = ws.Cells[rowNum, 1, rowNum, 1].Text;
                        row[1] = ws.Cells[rowNum, 3, rowNum, 3].Text;
                    }
                }

                ProgressBar1.Maximum = tbl.Rows.Count;

                foreach (var user in users)
                {
                    double value = 0;
                    ProgressBar1.Value = value;

                    foreach (DataRow row in tbl.Rows)
                    {
                        if (!await LinguaLeoApi.AddWord(row.ItemArray[0].ToString(), row.ItemArray[1].ToString(),user.Key,user.Value))
                            textBox.Text += row.ItemArray[0] + "\r\n";
                        value++;
                        ProgressBar1.Dispatcher.Invoke(() => ProgressBar1.Value = value, DispatcherPriority.Background);
                    }
                }
            }
        }

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }


}
