using DataInterface;
using ParameterControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace iccms.SubWindow
{
    public class SMSIMSIParameterClass : INotifyPropertyChanged
    {
        private string _iMSI;
        private bool _used;

        public string IMSI
        {
            get
            {
                return _iMSI;
            }

            set
            {
                _iMSI = value;
                NotifyPropertyChanged("IMSI");
            }
        }

        public bool Used
        {
            get
            {
                return _used;
            }

            set
            {
                _used = value;
                NotifyPropertyChanged("Used");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string value)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(value));
            }
        }
    }

    /// <summary>
    /// GSMV2SMSIMSIListWindow.xaml 的交互逻辑
    /// </summary>
    public partial class GSMV2SMSIMSIListWindow : Window
    {
        public string SelfDeviceName = string.Empty;
        static ObservableCollection<SMSIMSIParameterClass> SelfIMSIList = new ObservableCollection<SMSIMSIParameterClass>();
        private Thread LoadingIMSIThread = null;
        int ID = -1;

        public GSMV2SMSIMSIListWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;

            if (LoadingIMSIThread == null)
            {
                LoadingIMSIThread = new Thread(new ThreadStart(LoadIMSI));
                LoadingIMSIThread.Priority = ThreadPriority.BelowNormal;
            }
        }

        private void LoadIMSI()
        {
            //初始化
            DataTable SelfIMSITab = null;
            lock (JsonInterFace.CDMADeviceParameter.SMSIMSITabLock)
            {
                SelfIMSITab = JsonInterFace.CDMADeviceParameter.SMSIMSITab.Clone();
            }

            while (true)
            {
                try
                {
                    if (SelfDeviceName != null && SelfDeviceName != "")
                    {

                        Dispatcher.Invoke(() =>
                        {
                            if (ID != -1)
                            {
                                lock (JsonInterFace.CDMADeviceParameter.SMSIMSITabLock)
                                {
                                    SelfIMSITab = JsonInterFace.APATTributesLists[ID].SMSIMSITab.Copy();
                                }
                            }
                            for (int i = 0; i < SelfIMSITab.Rows.Count; i++)
                            {
                                string imsi = SelfIMSITab.Rows[i]["IMSI"].ToString();
                                bool used = Convert.ToBoolean(SelfIMSITab.Rows[i]["Used"].ToString());
                                bool Flag = true;

                                for (int j = 0; j < SelfIMSIList.Count; j++)
                                {
                                    if (imsi != SelfIMSIList[j].IMSI)
                                    {
                                        Flag = true;
                                    }
                                    else
                                    {
                                        Flag = false;
                                        break;
                                    }
                                }

                                if (Flag)
                                {

                                    SelfIMSIList.Add(
                                                        new SMSIMSIParameterClass()
                                                        {
                                                            IMSI = imsi,
                                                            Used = used
                                                        }
                                                    );
                                }
                            }

                            SelfIMSITab.Rows.Clear();
                        });
                    }
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
                }

                Thread.Sleep(1000);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                txtDeviceName.Text = SelfDeviceName;
                SMSIMSIList.ItemsSource = SelfIMSIList;
                lock (JsonInterFace.APATTributesParameter.APATTributesLock)
                {
                    for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                    {
                        if (JsonInterFace.APATTributesLists[i].FullName == SelfDeviceName)
                        {
                            ID = i;
                            break;
                        }
                    }
                }

                LoadingIMSIThread.Start();
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("短信息群发列表", ex.Message, ex.StackTrace);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (LoadingIMSIThread != null)
            {
                if (LoadingIMSIThread.ThreadState == ThreadState.Running || LoadingIMSIThread.ThreadState == ThreadState.WaitSleepJoin)
                {
                    LoadingIMSIThread.Abort();
                    LoadingIMSIThread.Join();
                }
            }

            SelfIMSIList.Clear();
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            bool Flag = true;
            string IMSIS = string.Empty;

            for (int i = 0; i < SelfIMSIList.Count; i++)
            {
                if (SelfIMSIList[i].Used)
                {
                    if (IMSIS == null || IMSIS == "")
                    {
                        IMSIS = SelfIMSIList[i].IMSI;
                    }
                    else
                    {

                        IMSIS += "," + SelfIMSIList[i].IMSI;
                    }
                    Flag = false;
                }
            }

            if (Flag)
            {
                MessageBox.Show("请选择IMSI号！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);

                if (ID != -1)
                {
                    lock (JsonInterFace.CDMADeviceParameter.SMSIMSITabLock)
                    {
                        for (int i = 0; i < SelfIMSIList.Count; i++)
                        {
                            for (int j = 0; j < JsonInterFace.APATTributesLists[ID].SMSIMSITab.Rows.Count; j++)
                            {
                                if (SelfIMSIList[i].IMSI == JsonInterFace.APATTributesLists[ID].SMSIMSITab.Rows[j]["IMSI"].ToString())
                                {
                                    JsonInterFace.APATTributesLists[ID].SMSIMSITab.Rows[j]["Used"] = SelfIMSIList[i].Used;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (IMSIS != "" && IMSIS != null)
                {
                    NavigatePages.DeviceManagerWindow.SelfIMSIList = IMSIS;
                }

                if (ID != -1)
                {
                    lock (JsonInterFace.CDMADeviceParameter.SMSIMSITabLock)
                    {
                        for (int i = 0; i < SelfIMSIList.Count; i++)
                        {
                            for (int j = 0; j < JsonInterFace.APATTributesLists[ID].SMSIMSITab.Rows.Count; j++)
                            {
                                if (SelfIMSIList[i].IMSI == JsonInterFace.APATTributesLists[ID].SMSIMSITab.Rows[j]["IMSI"].ToString())
                                {
                                    JsonInterFace.APATTributesLists[ID].SMSIMSITab.Rows[j]["Used"] = SelfIMSIList[i].Used;
                                    break;
                                }
                            }
                        }
                    }
                }

                this.Close();
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (btnEnter.IsFocused)
                {
                    this.DragMove();
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnEnter.Focus();
        }
    }
}
