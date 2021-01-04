using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace YAMSW
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string serverFileName = System.IO.Directory.GetCurrentDirectory() + @"\server.jar";
        public static int minMemory = 1024;
        public static int maxMemory = 2048;
        public static int instanceNo = 0;
        public static char unit = 'M';
        public static bool hasNewInput = false;
        public static bool hasNewOutput = false;
        public static string newInput;
        public static bool isRunning;
        public static string lastOut;
        public static TextBlock outputTb;
        public static bool programOpen;

        public static void startServer() {

            System.Diagnostics.Process serverProcess = new System.Diagnostics.Process();
            
            serverProcess.StartInfo.FileName = "java";
            serverProcess.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(serverFileName);
            serverProcess.StartInfo.Arguments = "-Xms" + minMemory + unit + " -Xmx" + maxMemory + unit + " -jar " + serverFileName + " -nogui";
            serverProcess.StartInfo.UseShellExecute = false;
            serverProcess.StartInfo.RedirectStandardOutput = true;
            serverProcess.StartInfo.RedirectStandardInput = true;
            serverProcess.StartInfo.CreateNoWindow = true;

            serverProcess.Start();
            isRunning = true;
            programOpen = true;
            

            Thread inputThread = new Thread(new ParameterizedThreadStart(inputWrite));
            inputThread.Start(serverProcess.StandardInput);
            Thread outputThread = new Thread(new ParameterizedThreadStart(outputRead));
            outputThread.Start(serverProcess.StandardOutput);

            inputThread.Join();
            outputThread.Join();
            serverProcess.WaitForExit();
            programOpen = false;





        }
        public static void inputWrite(object streamWriter) {
            System.IO.StreamWriter writer = (System.IO.StreamWriter)streamWriter;
            while (isRunning) {
                if (hasNewInput) {
                    writer.WriteLine(newInput);
                    hasNewInput = false;
                    if (newInput == "stop")
                        break;
                }
            }
            writer.WriteLine (" ");
            
        }
        public static void outputRead(object streamReader)
        {
            System.IO.StreamReader reader = (System.IO.StreamReader)streamReader;
            string temp;
            while (isRunning)
            {
                temp = reader.ReadLine();
                if (temp != null)
                {
                    lastOut = temp;

                    YAMSW.MainWindow.Instances[instanceNo].Dispatcher.BeginInvoke(new ThreadStart(() => YAMSW.MainWindow.Instances[instanceNo].CommandLineTextBlock.Text += temp));
                }

            }

        }
        public ServerInfo parseServerInfo(string str)
        {
            Regex regex = new Regex(@"\[([0-9]{2}:[0-9]{2}:[0-9]{2})\] \[([\w-]*)\/([A-Z]*)\]: (.*)", RegexOptions.Compiled);
            MatchCollection matches = regex.Matches(str);
            GroupCollection groups = matches[0].Groups;
            return new ServerInfo(groups[0].Value, groups[1].Value, groups[2].Value, groups[3].Value);
        }
        public class ServerInfo
        {
            public string time { get; private set; }
            public string thread { get; private set; }
            public string logLevel { get; private set; }
            public string feedback { get; private set; }
            public ServerInfo(string time, string thread, string logLevel, string feedback)
            {
                this.time = time;
                this.thread = thread;
                this.logLevel = logLevel;
                this.feedback = feedback;
            }
        }
    }
}
