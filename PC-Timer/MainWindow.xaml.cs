using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace PC_Timer
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //Checks wich Lang is set and checks the radiobutton
            switch (Thread.CurrentThread.CurrentCulture.Name)
            {
                case "de-DE":
                    MenRadio_de.IsChecked = true;
                    break;
                case "en-US":
                    MenRadio_en.IsChecked = true;
                    break;
                default:
                    MenRadio_en.IsChecked = true;
                    break;
            }
        }


        private void SetLanguageDictionary(CultureInfo newCulture)
        {
            //
            //Changes the Language depending on the localisation. Default is en-US
            //
            CultureInfo.DefaultThreadCurrentCulture = newCulture;
            CultureInfo.DefaultThreadCurrentUICulture = newCulture;
            ResourceDictionary dict = new ResourceDictionary();
            switch (Thread.CurrentThread.CurrentCulture.ToString())
            {
                case "en-US":
                    dict.Source = new Uri("..\\Resources\\Dictionary_en-US.xaml", UriKind.Relative);
                    break;
                case "de-DE":
                    dict.Source = new Uri("..\\Resources\\Dictionary_de-DE.xaml", UriKind.Relative);
                    break;
                default:
                    dict.Source = new Uri("..\\Resources\\Dictionary_en-US.xaml", UriKind.Relative);
                    break;
            }
            this.Resources.MergedDictionaries.Add(dict);
        }

        // TODO Does not work right. Is creating new lines instead of editing a line. I do this later or not...

        //private void WriteToConfig(string text)
        //{
        //    //This Function should find a given setting and then replace it. When there is no file, it gets created and the line gets written
        //    //
        //    //I hate this

        //    string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\PC-Timer";
        //    string file = "\\settings.config";
        //    if (!Directory.Exists(path))
        //    {
        //        Directory.CreateDirectory(path);
        //    }
        //    if (!File.Exists(path + file))
        //    {
        //        FileStream fs = new FileStream(path + file, FileMode.OpenOrCreate);
        //        StreamWriter sw = new StreamWriter(fs);
        //        sw.WriteLine("#");
        //        sw.WriteLine("#Config for PC-Timer");
        //        sw.WriteLine("#");
        //        sw.WriteLine(text);


        //        sw.Close();
        //        fs.Close();
        //    }
        //    else
        //    {
        //        String line;
        //        bool foundLine = false;
        //        FileStream fs = new FileStream(path + file, FileMode.OpenOrCreate);
        //        StreamReader sr = new StreamReader(fs);
        //        StreamWriter sw = new StreamWriter(fs);
        //        line = sr.ReadLine();

        //        while (line != null)
        //        {
        //            string textcheck = text.Substring(0, text.IndexOf("="));
        //            string linecheck = line;
        //            if (linecheck.Substring(0, 1) != "#")
        //            {
        //                linecheck = linecheck.Substring(0, line.IndexOf("="));
        //                if (linecheck == textcheck)
        //                {
        //                    sw.Flush();
        //                    sw.WriteLine(text);
        //                    foundLine = true;
        //                    break;
        //                }
        //            }
        //            line = sr.ReadLine();
        //        }
        //        if (foundLine == false)
        //        {
        //            sw.WriteLine(text);
        //        }
        //        sw.Close();
        //        sr.Close();
        //        fs.Close();
        //    }

        //}

        private void MenRadio_en_Checked(object sender, RoutedEventArgs e)
        {
            SetLanguageDictionary(new CultureInfo("en-US"));
            //string text = "lang=en-US";
            //WriteToConfig(text);

        }

        private void MenRadio_de_Checked(object sender, RoutedEventArgs e)
        {
            SetLanguageDictionary(new CultureInfo("de-DE"));
            //string text = "lang=de-DE";
            //WriteToConfig(text);
        }

        private void MenItem_en_Click(object sender, RoutedEventArgs e)
        {
            MenRadio_en.IsChecked = true;
        }

        private void MenItem_de_Click(object sender, RoutedEventArgs e)
        {
            MenRadio_de.IsChecked = true;
        }

        private void MenCode_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Hundhausen/PC-Timer");
        }

        private void MenError_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Hundhausen/PC-Timer/issues");
        }

        private void MenInfo_Click(object sender, RoutedEventArgs e)
        {
            //TODO better Version
            MessageBox.Show("PC-Timer created by Jean-Pierre Hundhausen\nMy Github: https://github.com/Hundhausen", "Info");
        }
    }
}
