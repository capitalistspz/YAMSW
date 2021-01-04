using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace YAMSW
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static List<MainWindow> Instances = new List<MainWindow>();
        public static int instanceCount = 0;
        private int instanceNo;
        public MainWindow()
        {
            InitializeComponent();
            Instances.Add(this);
            instanceNo = instanceCount++;
        }

        private void fileLocationBox_Drop(object sender, DragEventArgs e)
        {

        }

        private void serverStartButton_Click(object sender, RoutedEventArgs e)
        {
            Button startButton = (Button)sender;
            if (App.isRunning)
            {
                startButton.Content = "Stopping server...";
                startButton.IsEnabled = false;

                App.hasNewInput = true;
                App.newInput = "stop";

                while (App.isRunning) {
                }

                startButton.Content = "Start server";
            }
            else
            {
                startButton.Content = "Starting server...";
                startButton.IsEnabled = false;
                this.CommandLineTextBlock.Text = "\0";
                App.outputTb = CommandLineTextBlock;
                if (fileLocationBox.Text != null) {
                    App.serverFileName = fileLocationBox.Text;
                }
                else
                {
                    App.serverFileName = @"\server.jar";
                }
                if (Int32.TryParse(minRAMTextBox.Text, out App.minMemory) && Int32.TryParse(maxRAMTextBox.Text, out App.maxMemory)) 
                {
                    if (MemButton_1.IsChecked == true)
                        App.unit = '\0';

                    else if (MemButton_3.IsChecked == true)
                        App.unit = 'G';

                    else
                        App.unit = 'M';
                }
                else
                {
                    App.minMemory = 1024;
                    App.maxMemory = 1024;
                    App.unit = 'M';
                }
                App.instanceNo = this.instanceNo;

                App.startServer();
                startButton.Content = "Stop server";
            }
        }
        public void updateOutputBox(string str)
        {
            CommandLineTextBlock.Text += str;
        }
        ~MainWindow()
        {
            --instanceCount;
        }

        private void fileLocationButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Jar files (*.jar)|*.jar|All files (*.*)|*.*",
                InitialDirectory = System.IO.Directory.GetCurrentDirectory(),
                Multiselect = false
            };
            if (openFileDialog.ShowDialog() == true)
                fileLocationBox.Text = openFileDialog.FileName;
        }

        private int[] remSpaces(string str) {
            List <int> pos = new List<int>();
            Regex regex = new Regex(@"\s");
            MatchCollection matches = regex.Matches(str);
            int count = matches.Count;
            int it = 0;
            while (it < count)
            {
                str.Remove(matches[it].Index);
                --it; --count;
            }

            return pos.ToArray();
        }

    }
}
