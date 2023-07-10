using Microsoft.Win32;
using System;
using System.Data;
using System.IO;
using System.Windows;
using TeaTime;
using Wpf.Ui.Controls;

namespace TeaExplorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : FluentWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            //MGrid.DataContext = dt.DefaultView;
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                HandleFile(openFileDialog.FileName);
            }
        }

        private void HandleFile(string file)
        {
            var ext = Path.GetExtension(file);
            if (ext == ".csv")
                OpenCsvFile(file);
            else if (ext == ".tea")
                OpenTeaFile(file);
        }

        private void OpenCsvFile(string fileName)
        {
            using var reader = new StreamReader(fileName);
            var header = reader.ReadLine();
            if (header == null) return;
            var dt = new DataTable("MyTable");
            foreach (var sub in header.Split(',', StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries))
            {
                dt.Columns.Add(sub);
            }
            while ((header = reader.ReadLine()) != null)
            {
                var dataRow = dt.NewRow();
                var ct = 0;
                foreach (var cell in header.Split(',', StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries))
                {
                    dataRow[ct] = cell;
                    ct++;
                }
                dt.Rows.Add(dataRow);
            }
            MGrid.DataContext = dt;
            MGrid.ItemsSource = dt.DefaultView;
        }

        private void OpenTeaFile(string fileName)
        {
            using var tea = TeaFile.OpenRead(fileName);
            var dt = new DataTable("MyTable");
            foreach (var field in tea.Description.ItemDescription.Fields)
            {
                dt.Columns.Add(field.Name);
            }
            foreach (Item item in tea.Items)
            {
                var dataRow = dt.NewRow();
                var ct = 0;
                foreach (var cell in item.Values)
                {
                    dataRow[ct] = cell;
                    ct++;
                }

                dt.Rows.Add(dataRow);
            }
            MGrid.DataContext = dt;
            MGrid.ItemsSource = dt.DefaultView;
        }



        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MGrid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
                HandleFile(files[0]);
            }
        }
    }
}
