using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using System.Security.AccessControl;
using System.Security.Principal;

namespace PC_Timer {

    public partial class MainWindow :Window {
        const string config_file = "config.xml";
        public MainWindow() {
            InitializeComponent();

            //Checks if there is the config file, reads the language and sets it. When there is no config file, it reads the culture from the PC and sets it when its avaible
            string culture;
            if(!File.Exists(config_file)) {
                culture = Thread.CurrentThread.CurrentCulture.Name;
                }
            else {
                XmlDocument doc = new XmlDocument();
                doc.Load(config_file);
                XmlNode node = doc.DocumentElement.SelectSingleNode("/settings/language");
                culture = (node.InnerText);
                }
            switch(culture) {
                    case "de-DE":
                        MenRadio_de.IsChecked = true;
                        write_lang_settings("de-DE");
                        break;
                    case "en-US":
                        MenRadio_en.IsChecked = true;
                        write_lang_settings("en-US");
                        break;
                    default:
                        MenRadio_en.IsChecked = true;
                        write_lang_settings("en-US");
                        break;
                    }
 
            datetimepicker_date.Minimum = DateTime.Now;
            datetimepicker_date.Value = DateTime.Now;
            }

        //required to set the option to prevent sleep
        public enum EXECUTION_STATE :uint {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001,
            }
        internal class NativeMethods {
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);
            }


        private void SetLanguageDictionary(CultureInfo newCulture) {
            //
            //Changes the Language depending on the localisation. Default is en-US
            //
            CultureInfo.DefaultThreadCurrentCulture = newCulture;
            CultureInfo.DefaultThreadCurrentUICulture = newCulture;
            ResourceDictionary dict = new ResourceDictionary();
            switch(Thread.CurrentThread.CurrentCulture.ToString()) {
                case "en-US":
                    dict.Source = new Uri("..\\Resources\\Dictionary_en-US.xaml", UriKind.Relative);
                    write_lang_settings("en-US");
                    break;
                case "de-DE":
                    dict.Source = new Uri("..\\Resources\\Dictionary_de-DE.xaml", UriKind.Relative);
                    write_lang_settings("de-DE");
                    break;
                default:
                    dict.Source = new Uri("..\\Resources\\Dictionary_en-US.xaml", UriKind.Relative);
                    write_lang_settings("en-US");
                    break;
                }
            this.Resources.MergedDictionaries.Add(dict);
            }

        public static void write_lang_settings(string lang) {
                new XDocument(
                new XElement("settings",
                    new XComment("If set to false, no window will show up on startup of this programm"),
                    new XElement("language", lang)
                    )
                )
                .Save(config_file);
            }

        private void MenRadio_en_Checked(object sender, RoutedEventArgs e) {
            SetLanguageDictionary(new CultureInfo("en-US"));
            datetimepicker_date.CultureInfo = new CultureInfo("en-US");
            //string text = "lang=en-US";
            //WriteToConfig(text);

            }

        private void MenRadio_de_Checked(object sender, RoutedEventArgs e) {
            SetLanguageDictionary(new CultureInfo("de-DE"));
            datetimepicker_date.CultureInfo = new CultureInfo("de-DE");
            //string text = "lang=de-DE";
            //WriteToConfig(text);
            }

        private void MenItem_en_Click(object sender, RoutedEventArgs e) {
            MenRadio_en.IsChecked = true;
            }

        private void MenItem_de_Click(object sender, RoutedEventArgs e) {
            MenRadio_de.IsChecked = true;
            }

        private void MenCode_Click(object sender, RoutedEventArgs e) {
            System.Diagnostics.Process.Start("https://github.com/Hundhausen/PC-Timer");
            }

        private void MenError_Click(object sender, RoutedEventArgs e) {
            System.Diagnostics.Process.Start("https://github.com/Hundhausen/PC-Timer/issues");
            }

        private void MenInfo_Click(object sender, RoutedEventArgs e) {
            //TODO better Version
            MessageBox.Show("PC-Timer created by Jean-Pierre Hundhausen\nMy Github: https://github.com/Hundhausen", "Info");
            }

        private void btn_start_Click(object sender, RoutedEventArgs e) {
            int hour, min, sec, time = 0;
            if(tab_time.IsSelected) {
                try {
                    //failsafe. if nothing is set in a textbox, vaule = 0. When not an integer can be ´parse into an integer, it should crash
                    if(txtbox_hour.Text == null || txtbox_hour.Text == "") {
                        hour = 0;
                        }
                    else {
                        hour = int.Parse(txtbox_hour.Text);
                        }
                    if(txtbox_min.Text == null || txtbox_min.Text == "") {
                        min = 0;
                        }
                    else {
                        min = int.Parse(txtbox_min.Text);
                        }
                    if(txtbox_sec.Text == null || txtbox_sec.Text == "") {
                        sec = 0;
                        }
                    else {
                        sec = int.Parse(txtbox_sec.Text);
                        }
                    }
                catch(Exception) {
                    MessageBox.Show(Application.Current.FindResource("msgbox_noint_txt").ToString() + "\n\n" + e.ToString(), Application.Current.FindResource("msgbox_fail_head").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                    }
                time = sec + (min * 60) + (hour * 60 * 60);
                }
            else if(tab_date.IsSelected) {
                DateTime sel_date;
                if(!DateTime.TryParse(datetimepicker_date.Text, out sel_date)) {
                    MessageBox.Show(Application.Current.FindResource("msgbox_fail_txt").ToString(), Application.Current.FindResource("msgbox_fail_head").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                DateTime cur_date = DateTime.Now;
                if(sel_date < cur_date) {
                    MessageBox.Show(Application.Current.FindResource("msgbox_dateoutofrange_txt").ToString(), Application.Current.FindResource("msgbox_fail_head").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                    }
                else {
                    TimeSpan span = sel_date.Subtract(cur_date);
                    time = (int)span.TotalSeconds;
                    }
                }
            else {
                MessageBox.Show(Application.Current.FindResource("msgbox_fail_txt").ToString(), Application.Current.FindResource("msgbox_fail_head").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                }

            //case is depending on the position and default is only a fail safe. 0 is shutdown, 1 restart and so on
            if(checkBox_sleep.IsEnabled) {
                if(checkBox_display.IsEnabled) {
                    NativeMethods.SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_SYSTEM_REQUIRED | EXECUTION_STATE.ES_AWAYMODE_REQUIRED | EXECUTION_STATE.ES_DISPLAY_REQUIRED);
                    }
                else {
                    NativeMethods.SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_SYSTEM_REQUIRED | EXECUTION_STATE.ES_AWAYMODE_REQUIRED);
                    }
                }

            switch(combo_art.SelectedIndex) {
                case 0:
                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                    startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = "/C shutdown /s /f /t " + time;
                    process.StartInfo = startInfo;
                    process.Start();
                    MessageBox.Show(Application.Current.FindResource("msgbox_start_shutodown_txt").ToString(), Application.Current.FindResource("msgbox_start_head").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);

                    break;
                case 1:
                    System.Diagnostics.Process process_1 = new System.Diagnostics.Process();
                    System.Diagnostics.ProcessStartInfo startInfo_1 = new System.Diagnostics.ProcessStartInfo();
                    startInfo_1.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    startInfo_1.FileName = "cmd.exe";
                    startInfo_1.Arguments = "/C shutdown /r /f /t " + time;
                    process_1.StartInfo = startInfo_1;
                    process_1.Start();
                    MessageBox.Show(Application.Current.FindResource("msgbox_start_restart_txt").ToString(), Application.Current.FindResource("msgbox_start_head").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case 2:
                    System.Diagnostics.Process process_2 = new System.Diagnostics.Process();
                    System.Diagnostics.ProcessStartInfo startInfo_2 = new System.Diagnostics.ProcessStartInfo();
                    startInfo_2.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    startInfo_2.FileName = "cmd.exe";
                    startInfo_2.Arguments = "/C shutdown /h /f /t " + time;
                    process_2.StartInfo = startInfo_2;
                    process_2.Start();
                    MessageBox.Show(Application.Current.FindResource("msgbox_start_suspend_txt").ToString(), Application.Current.FindResource("msgbox_start_head").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case 3:
                    System.Diagnostics.Process process_3 = new System.Diagnostics.Process();
                    System.Diagnostics.ProcessStartInfo startInfo_3 = new System.Diagnostics.ProcessStartInfo();
                    startInfo_3.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    startInfo_3.FileName = "cmd.exe";
                    startInfo_3.Arguments = "/C shutdown /l /f /t " + time;
                    process_3.StartInfo = startInfo_3;
                    process_3.Start();
                    MessageBox.Show(Application.Current.FindResource("msgbox_start_logout_txt").ToString(), Application.Current.FindResource("msgbox_start_head").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                default:
                    MessageBox.Show(Application.Current.FindResource("msgbox_fail_txt").ToString(), Application.Current.FindResource("msgbox_fail_head").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                    break;

                }

            }

        //Display Checkbox is not allowed to be checked alone because then the code will not be executed and also it makes no sense to me to keep the display active when the pc is allowed to go into sleep
        private void checkBox_display_Checked(object sender, RoutedEventArgs e) {
            checkBox_sleep.IsChecked = true;
            }

        private void checkBox_sleep_Unchecked(object sender, RoutedEventArgs e) {
            checkBox_display.IsChecked = false;
            }

        private void btn_stop_Click(object sender, RoutedEventArgs e) {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C shutdown /a";
            process.StartInfo = startInfo;
            process.Start();
            MessageBox.Show(Application.Current.FindResource("msgbox_abort_txt").ToString(), Application.Current.FindResource("msgbox_abort_head").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
