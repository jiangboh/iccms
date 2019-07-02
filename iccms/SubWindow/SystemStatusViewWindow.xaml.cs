using DataInterface;
using ParameterControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace iccms.SubWindow
{
    public class SymbolStatusParameterListClass : INotifyPropertyChanged
    {
        private string _componentSymbolName;
        private string _statusIcon = new NodeIcon().LeafNoConnectNodeIcon;

        public string ComponentSymbolName
        {
            get
            {
                return _componentSymbolName;
            }

            set
            {
                _componentSymbolName = value;
                NotifyPropertyChanged("ComponentSymbolName");
            }
        }

        public string StatusIcon
        {
            get
            {
                return _statusIcon;
            }

            set
            {
                _statusIcon = value;
                NotifyPropertyChanged("StatusIcon");
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
    /// SystemStatusViewWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SystemStatusViewWindow : Window
    {
        public static ObservableCollection<SymbolStatusParameterListClass> SymbolStatusParameterList = new ObservableCollection<SymbolStatusParameterListClass>();
        private Thread CheckSymbolStatusThread = null;

        public SystemStatusViewWindow()
        {
            InitializeComponent();
            InitialSymbolStatuListParameter();

            if (CheckSymbolStatusThread == null)
            {
                CheckSymbolStatusThread = new Thread(new ThreadStart(CheckSymbolStatus));
            }
        }

        //初化模块列表
        private void InitialSymbolStatuListParameter()
        {
            try
            {
                System.Type ClassType = typeof(SystemThreadsStatusParametersClass);
                PropertyInfo[] ItemCount = ClassType.GetProperties();
                foreach (PropertyInfo Item in ItemCount)
                {
                    object[] Obj = Item.GetCustomAttributes(typeof(DescriptionAttribute), true);
                    if (Obj.Length > 0)
                    {
                        string SymbolName = ((DescriptionAttribute)Obj[0]).Description;
                        SymbolStatusParameterList.Add(new SymbolStatusParameterListClass() { ComponentSymbolName = SymbolName, StatusIcon = new NodeIcon().LeafNoActiveNodeIcon });
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        //状态检测
        private void CheckSymbolStatus()
        {
            NodeIcon StatusIco = new NodeIcon();
            while (true)
            {
                for (int i = 0; i < SymbolStatusParameterList.Count; i++)
                {
                    if (SymbolStatusParameterList[i].ComponentSymbolName == JsonInterFace.SystemThreadsStatusParameters.SymbolType.AppHeartThreadName)
                    {
                        if (JsonInterFace.SystemThreadsStatusParameters.AppHeartThreadStatus)
                        {
                            SymbolStatusParameterList[i].StatusIcon = StatusIco.LeafAllReadyNodeIcon;
                        }
                        else
                        {
                            SymbolStatusParameterList[i].StatusIcon = StatusIco.ErrorNodeIcon;
                        }
                    }
                    else if (SymbolStatusParameterList[i].ComponentSymbolName == JsonInterFace.SystemThreadsStatusParameters.SymbolType.StickDataPackageThreadName)
                    {
                        if (JsonInterFace.SystemThreadsStatusParameters.StickDataPackageThreadStatus)
                        {
                            SymbolStatusParameterList[i].StatusIcon = StatusIco.LeafAllReadyNodeIcon;
                        }
                        else
                        {
                            SymbolStatusParameterList[i].StatusIcon = StatusIco.ErrorNodeIcon;
                        }
                    }
                    else if (SymbolStatusParameterList[i].ComponentSymbolName == JsonInterFace.SystemThreadsStatusParameters.SymbolType.AnalysisDataThreadName)
                    {
                        if (JsonInterFace.SystemThreadsStatusParameters.AnalysisDataThreadStatus)
                        {
                            SymbolStatusParameterList[i].StatusIcon = StatusIco.LeafAllReadyNodeIcon;
                        }
                        else
                        {
                            SymbolStatusParameterList[i].StatusIcon = StatusIco.ErrorNodeIcon;
                        }
                    }
                    else if (SymbolStatusParameterList[i].ComponentSymbolName == JsonInterFace.SystemThreadsStatusParameters.SymbolType.InfoStatisticThreadName)
                    {
                        if (JsonInterFace.SystemThreadsStatusParameters.InfoStatisticThreadStatus)
                        {
                            SymbolStatusParameterList[i].StatusIcon = StatusIco.LeafAllReadyNodeIcon;
                        }
                        else
                        {
                            SymbolStatusParameterList[i].StatusIcon = StatusIco.ErrorNodeIcon;
                        }
                    }
                    else if (SymbolStatusParameterList[i].ComponentSymbolName == JsonInterFace.SystemThreadsStatusParameters.SymbolType.GettingUnknownDeviceListThreadName)
                    {
                        if (JsonInterFace.SystemThreadsStatusParameters.GettingUnknownDeviceListThreadStatus)
                        {
                            SymbolStatusParameterList[i].StatusIcon = StatusIco.LeafAllReadyNodeIcon;
                        }
                        else
                        {
                            SymbolStatusParameterList[i].StatusIcon = StatusIco.ErrorNodeIcon;
                        }
                    }
                    else if (SymbolStatusParameterList[i].ComponentSymbolName == JsonInterFace.SystemThreadsStatusParameters.SymbolType.RemoteReconnectTheadName)
                    {
                        if (JsonInterFace.SystemThreadsStatusParameters.RemoteReconnectTheadStatus)
                        {
                            SymbolStatusParameterList[i].StatusIcon = StatusIco.LeafAllReadyNodeIcon;
                        }
                        else
                        {
                            SymbolStatusParameterList[i].StatusIcon = StatusIco.ErrorNodeIcon;
                        }
                    }
                    else if (SymbolStatusParameterList[i].ComponentSymbolName == JsonInterFace.SystemThreadsStatusParameters.SymbolType.SetUnknownTipsWindowThreadName)
                    {
                        if (JsonInterFace.SystemThreadsStatusParameters.SetUnknownTipsWindowThreadStatus)
                        {
                            SymbolStatusParameterList[i].StatusIcon = StatusIco.LeafAllReadyNodeIcon;
                        }
                        else
                        {
                            SymbolStatusParameterList[i].StatusIcon = StatusIco.ErrorNodeIcon;
                        }
                    }
                    else if (SymbolStatusParameterList[i].ComponentSymbolName == JsonInterFace.SystemThreadsStatusParameters.SymbolType.CheckNetWorkInterfaceTheadName)
                    {
                        if (JsonInterFace.SystemThreadsStatusParameters.CheckNetWorkInterfaceTheadStatus)
                        {
                            SymbolStatusParameterList[i].StatusIcon = StatusIco.LeafAllReadyNodeIcon;
                        }
                        else
                        {
                            SymbolStatusParameterList[i].StatusIcon = StatusIco.ErrorNodeIcon;
                        }
                    }
                    else if (SymbolStatusParameterList[i].ComponentSymbolName == JsonInterFace.SystemThreadsStatusParameters.SymbolType.ScannerWarningSoundTheadName)
                    {
                        if (JsonInterFace.SystemThreadsStatusParameters.ScannerWarningSoundTheadStatus)
                        {
                            SymbolStatusParameterList[i].StatusIcon = StatusIco.LeafAllReadyNodeIcon;
                        }
                        else
                        {
                            SymbolStatusParameterList[i].StatusIcon = StatusIco.ErrorNodeIcon;
                        }
                    }
                    else if (SymbolStatusParameterList[i].ComponentSymbolName == JsonInterFace.SystemThreadsStatusParameters.SymbolType.ScannerReportRealTimeThreadName)
                    {
                        if (JsonInterFace.SystemThreadsStatusParameters.ScannerReportRealTimeThreadStatus)
                        {
                            SymbolStatusParameterList[i].StatusIcon = StatusIco.LeafAllReadyNodeIcon;
                        }
                        else
                        {
                            SymbolStatusParameterList[i].StatusIcon = StatusIco.ErrorNodeIcon;
                        }
                    }
                    else if (SymbolStatusParameterList[i].ComponentSymbolName == JsonInterFace.SystemThreadsStatusParameters.SymbolType.MeasReportBlackListThreadName)
                    {
                        if (JsonInterFace.SystemThreadsStatusParameters.MeasReportBlackListThreadStatus)
                        {
                            SymbolStatusParameterList[i].StatusIcon = StatusIco.LeafAllReadyNodeIcon;
                        }
                        else
                        {
                            SymbolStatusParameterList[i].StatusIcon = StatusIco.ErrorNodeIcon;
                        }
                    }
                    else if (SymbolStatusParameterList[i].ComponentSymbolName == JsonInterFace.SystemThreadsStatusParameters.SymbolType.GSMPhoneNumberRecordInfoThreadName)
                    {
                        if (JsonInterFace.SystemThreadsStatusParameters.GSMPhoneNumberRecordInfoThreadStatus)
                        {
                            SymbolStatusParameterList[i].StatusIcon = StatusIco.LeafAllReadyNodeIcon;
                        }
                        else
                        {
                            SymbolStatusParameterList[i].StatusIcon = StatusIco.ErrorNodeIcon;
                        }
                    }
                    else if (SymbolStatusParameterList[i].ComponentSymbolName == JsonInterFace.SystemThreadsStatusParameters.SymbolType.GSMSMSRecordInfoThreadName)
                    {
                        if (JsonInterFace.SystemThreadsStatusParameters.GSMSMSRecordInfoThreadStatus)
                        {
                            SymbolStatusParameterList[i].StatusIcon = StatusIco.LeafAllReadyNodeIcon;
                        }
                        else
                        {
                            SymbolStatusParameterList[i].StatusIcon = StatusIco.ErrorNodeIcon;
                        }
                    }
                }

                Thread.Sleep(5000);
            }
        }

        private void btnQuit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtDataTotalValue.DataContext = JsonInterFace.SystemRuningStickPackageStatus;
            txtDataCachingStatusValue.DataContext = JsonInterFace.SystemRuningStickPackageStatus;
            DataCachingStatusBar.DataContext = JsonInterFace.SystemRuningStickPackageStatus;
            txtDataPoolValue.DataContext = JsonInterFace.SystemRuningStickPackageStatus;

            txtDataAnilaysisgTotalsValue.DataContext = JsonInterFace.SystemRuningAnalysisStatus;
            txtDataAnilaysisgStatusValue.DataContext = JsonInterFace.SystemRuningAnalysisStatus;
            DataAnilaysisgStatusBar.DataContext = JsonInterFace.SystemRuningAnalysisStatus;

            txtNetWorkConnectedStatusValue.DataContext = JsonInterFace.SystemRuningStickPackageStatus;
            imgNetWorkConnectedStatus.DataContext = JsonInterFace.SystemRuningStickPackageStatus;

            dgSymbolStatusList.ItemsSource = SymbolStatusParameterList;

            //启动检测模块状态
            CheckSymbolStatusThread.Start();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnQuit.Focus();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (btnQuit.IsFocused)
                {
                    this.DragMove();
                }
            }
        }

        private void btnToBack_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = false;
            this.WindowState = WindowState.Minimized;
        }

        private void btnToTop_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = true;
            this.WindowState = WindowState.Normal;
        }

        private void btnDataAnilaysisBrowserCaption_Click(object sender, RoutedEventArgs e)
        {
            AnalysisLibJsonStrBrowserWindow AnalysisLibJsonStrBrowserWin = new AnalysisLibJsonStrBrowserWindow();
            AnalysisLibJsonStrBrowserWin.Show();
        }
    }
}
