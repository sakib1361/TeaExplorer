using Microsoft.Win32;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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

        private async void HandleFile(string file)
        {
            PBar.Visibility = Visibility.Visible;
            MGrid.ItemsSource = null;
            MGrid.Columns.Clear();
            var ext = Path.GetExtension(file);
            if (ext == ".csv")
                await OpenCsvFile(file);
            else if (ext == ".tea")
               await OpenTeaFile(file);
            PBar.Visibility = Visibility.Collapsed;
        }

        private async Task OpenCsvFile(string fileName)
        {
            var reader = await File.ReadAllLinesAsync(fileName);
            var header = reader[0];
            
            var head = 0;
            foreach (var sub in header.Split(',', StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries))
            {
                MGrid.Columns.Add(new DataGridTextColumn() { Header = sub, Binding = new Binding($"[{head}]") });
                head++;
            }
            var allLines = reader.Skip(1).Select(x=>x.Split(',' , StringSplitOptions.TrimEntries|StringSplitOptions.RemoveEmptyEntries));
            MGrid.ItemsSource = allLines;
        }

        private async Task OpenTeaFile(string fileName)
        {
            using var tea = TeaFile.OpenRead(fileName);
            var head = 0;
            foreach (var field in tea.Description.ItemDescription.Fields)
            {
                MGrid.Columns.Add(new DataGridTextColumn() { Header = field.Name, Binding = new Binding($"[{head}]") });
                head++;
            }
            var dataSet = await Task.Run(() =>
            {
                return tea.Items.Select(x => x.Values).ToArray();
            });
            MGrid.ItemsSource = dataSet;
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
