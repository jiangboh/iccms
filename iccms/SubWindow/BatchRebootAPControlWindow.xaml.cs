using DataInterface;
using NetController;
using ParameterControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace iccms.SubWindow
{
    public class RebootAPPara : INotifyPropertyChanged
    {
        private bool _isSelected = false;
        private string _selfName;
        private string _sN;
        private string _ipAddr;
        private string _port;
        private string _netMask;
        private string _mode;
        private string _isOnline;
        private string _innerType;
        private string _icon;

        public string SelfName
        {
            get
            {
                return _selfName;
            }

            set
            {
                _selfName = value;
                NotifyPropertyChanged("SelfName");
            }
        }

        public string SN
        {
            get
            {
                return _sN;
            }

            set
            {
                _sN = value;
                NotifyPropertyChanged("SN");
            }
        }

        public string IpAddr
        {
            get
            {
                return _ipAddr;
            }

            set
            {
                _ipAddr = value;
                NotifyPropertyChanged("IpAddr");
            }
        }

        public string Port
        {
            get
            {
                return _port;
            }

            set
            {
                _port = value;
                NotifyPropertyChanged("StringPort");
            }
        }

        public string NetMask
        {
            get
            {
                return _netMask;
            }

            set
            {
                _netMask = value;
                NotifyPropertyChanged("NetMask");
            }
        }

        public string Mode
        {
            get
            {
                return _mode;
            }

            set
            {
                _mode = value;
                NotifyPropertyChanged("Mode");
            }
        }

        public string IsOnline
        {
            get
            {
                return _isOnline;
            }

            set
            {
                _isOnline = value;
                NotifyPropertyChanged("IsOnline");
            }
        }

        public string InnerType
        {
            get
            {
                return _innerType;
            }

            set
            {
                _innerType = value;
                NotifyPropertyChanged("InnerType");
            }
        }

        public string Icon
        {
            get
            {
                return _icon;
            }

            set
            {
                _icon = value;
                NotifyPropertyChanged("Icon");
            }
        }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }

            set
            {
                _isSelected = value;
                NotifyPropertyChanged("IsSelected");
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

    public class ProgressBarPara : INotifyPropertyChanged
    {
        private int _maxValue = 100;
        private int _stepValue = 0;
        private bool _enable;

        public int MaxValue
        {
            get
            {
                return _maxValue;
            }

            set
            {
                _maxValue = value;
                NotifyPropertyChanged("MaxValue");
            }
        }

        public int StepValue
        {
            get
            {
                return _stepValue;
            }

            set
            {
                _stepValue = value;
                NotifyPropertyChanged("StepValue");
            }
        }

        public bool Enable
        {
            get
            {
                return _enable;
            }

            set
            {
                _enable = value;
                NotifyPropertyChanged("Enable");
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
    /// BatchRebootAPControlWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BatchRebootAPControlWindow : Window
    {
        private static ObservableCollection<RebootAPPara> BatchRebootAPList = new ObservableCollection<RebootAPPara>();
        private static ObservableCollection<RebootAPPara> BatchRemoveAPList = new ObservableCollection<RebootAPPara>();
        private static ProgressBarPara ProgressBarParameter = new ProgressBarPara();
        private static int BatchRebootApCount = 0;
        private StringBuilder BatchRebootResult = new StringBuilder();
        private static System.Timers.Timer CheckTimedOut = null;
        private static int BROV = 60000;
        private System.Timers.Timer CheckSelectedAllItemTimer = null;
        private object BatchRebootAPListDataLock = new object();
        public BatchRebootAPControlWindow()
        {
            InitializeComponent();

            if (CheckTimedOut == null)
            {
                CheckTimedOut = new System.Timers.Timer();
                CheckTimedOut.AutoReset = false;
                CheckTimedOut.Interval = BROV;
                CheckTimedOut.Elapsed += CheckTimedOut_Elapsed;
            }

            if (CheckSelectedAllItemTimer == null)
            {
                CheckSelectedAllItemTimer = new System.Timers.Timer();
                CheckSelectedAllItemTimer.AutoReset = true;
                CheckSelectedAllItemTimer.Interval = 300;
                CheckSelectedAllItemTimer.Elapsed += CheckSelectedAllItemTimer_Elapsed;
            }
        }

        private void CheckSelectedAllItemTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                CheckSelectedAllItem(sender, new EventArgs());
            });

        }

        private void CheckTimedOut_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            MessageBox.Show("设备批量重启超时！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
        }

        /// <summary>
        /// 重新实现 OnSourceInitialized
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource hwndsource = PresentationSource.FromVisual(this) as HwndSource;
            hwndsource.AddHook(new HwndSourceHook(WndProc));
        }

        /// <summary>
        /// 响应Window消息
        /// </summary>
        /// <param name="whnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        IntPtr WndProc(IntPtr whnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == Parameters.WM_BatchApRebootCompleteMessage)
            {
                RebootApRebootComplete(lParam);
                handled = true;
            }
            return whnd;
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

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnEnter.Focus();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {

                if (this.Tag != null)
                {
                    if (this.Tag.ToString() == "ApReboot")
                    {
                        btnEnter.Content = "批量重启";
                        BatchRebootAPList.Clear();
                        dgRebootAPList.ItemsSource = BatchRebootAPList;
                        pgbComplete.DataContext = ProgressBarParameter;
                        InitData();
                        Parameters.BatchRebootApWinHandle = new WindowInteropHelper(this).Handle;
                    }
                    else if (this.Tag.ToString() == "ApRemove")
                    {
                        btnEnter.Content = "批量删除";
                        BatchRebootAPList.Clear();
                        dgRebootAPList.ItemsSource = BatchRemoveAPList;
                        pgbComplete.DataContext = ProgressBarParameter;
                        InitData();
                        Parameters.BatchRebootApWinHandle = new WindowInteropHelper(this).Handle;
                    }
                }
                else
                {
                    this.Close();
                }

                CheckSelectedAllItemTimer.Start();
            }
            catch (Exception Ex)
            {
                BatchRebootAPList.Clear();
                BatchRemoveAPList.Clear();
                Parameters.PrintfLogsExtended("批量设备重启初始化内部故障", Ex.Message, Ex.StackTrace);
                JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "批量设备重启初始化内部故障，" + Ex.Message, "参数初始化", "异常");
            }
        }

        private void InitData()
        {
            try
            {
                for (int i = 0; i < NavigatePages.DeviceListWindow.BatchRebootAPList.DeviceNameList.Count; i++)
                {
                    if (NavigatePages.DeviceListWindow.BatchRebootAPList.DeviceNameList[i].IsOnLine)
                    {
                        RebootAPPara Item = new RebootAPPara();
                        Item.IsSelected = true;
                        Item.SelfName = NavigatePages.DeviceListWindow.BatchRebootAPList.DeviceNameList[i].Name;
                        Item.SN = NavigatePages.DeviceListWindow.BatchRebootAPList.DeviceNameList[i].SN;
                        Item.IpAddr = NavigatePages.DeviceListWindow.BatchRebootAPList.DeviceNameList[i].IPAddr;
                        Item.Port = NavigatePages.DeviceListWindow.BatchRebootAPList.DeviceNameList[i].Port.ToString();
                        Item.NetMask = NavigatePages.DeviceListWindow.BatchRebootAPList.DeviceNameList[i].NetMask;
                        Item.Mode = NavigatePages.DeviceListWindow.BatchRebootAPList.DeviceNameList[i].Mode;
                        Item.IsOnline = NavigatePages.DeviceListWindow.BatchRebootAPList.DeviceNameList[i].IsOnLine.ToString();
                        Item.InnerType = NavigatePages.DeviceListWindow.BatchRebootAPList.DeviceNameList[i].InnerType;
                        Item.Icon = NavigatePages.DeviceListWindow.BatchRebootAPList.DeviceNameList[i].Icon;
                        BatchRebootAPList.Add(Item);
                    }
                }
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("初始化批量重启设备失败", Ex.Message, Ex.StackTrace);
            }
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BatchRebootApCount = 0;
                lock (BatchRebootAPListDataLock)
                {
                    for (int i = 0; i < BatchRebootAPList.Count; i++)
                    {
                        if (BatchRebootAPList[i].IsSelected && BatchRebootAPList[i].IsOnline.ToLower() == "true")
                        {
                            BatchRebootApCount++;
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("获取批量重启AP数量失败", Ex.Message, Ex.StackTrace);
            }

            if (BatchRebootApCount <= 0)
            {
                MessageBox.Show("请选择需要重启的设备名称！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else
            {
                BROV = BatchRebootApCount * 10 * 1000;
            }

            new Thread(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    ObservableCollection<RebootAPPara> BatchRebootAPList = null;
                    lock (BatchRebootAPListDataLock)
                    {
                        BatchRebootAPList = BatchRebootAPControlWindow.BatchRebootAPList;
                    }
                    ProgressBarParameter.MaxValue = BatchRebootAPList.Count;
                    ProgressBarParameter.StepValue = 0;
                    CheckTimedOut.Start();
                    for (int i = 0; i < BatchRebootAPList.Count; i++)
                    {
                        try
                        {
                            if (BatchRebootAPList[i].IsSelected && BatchRebootAPList[i].IsOnline.ToLower() == "true")
                            {
                                Parameters.ConfigType = "BatchApReboot";

                                if (new Regex(DeviceType.LTE).Match(BatchRebootAPList[i].Mode).Success)
                                {
                                    JsonInterFace.LteDeviceParameter.DomainFullPathName = txtStation.Text;
                                    JsonInterFace.LteDeviceParameter.DeviceName = BatchRebootAPList[i].SelfName;
                                    JsonInterFace.LteDeviceParameter.IpAddr = BatchRebootAPList[i].IpAddr;
                                    JsonInterFace.LteDeviceParameter.Port = BatchRebootAPList[i].Port;
                                    JsonInterFace.LteDeviceParameter.InnerType = BatchRebootAPList[i].InnerType;
                                    JsonInterFace.LteDeviceParameter.SN = BatchRebootAPList[i].SN;
                                    RebootApTasck(0);
                                }
                                else if (DeviceType.WCDMA == BatchRebootAPList[i].Mode)
                                {
                                    JsonInterFace.WCDMADeviceParameter.DomainFullPathName = txtStation.Text;
                                    JsonInterFace.WCDMADeviceParameter.DeviceName = BatchRebootAPList[i].SelfName;
                                    JsonInterFace.WCDMADeviceParameter.IpAddr = BatchRebootAPList[i].IpAddr;
                                    JsonInterFace.WCDMADeviceParameter.Port = BatchRebootAPList[i].Port;
                                    JsonInterFace.WCDMADeviceParameter.InnerType = BatchRebootAPList[i].InnerType;
                                    JsonInterFace.WCDMADeviceParameter.SN = BatchRebootAPList[i].SN;
                                    RebootApTasck(1);
                                }
                                else if (DeviceType.GSM == BatchRebootAPList[i].Mode)
                                {
                                    JsonInterFace.GSMDeviceParameter.DomainFullPathName = txtStation.Text;
                                    JsonInterFace.GSMDeviceParameter.DeviceName = BatchRebootAPList[i].SelfName;
                                    JsonInterFace.GSMDeviceParameter.IpAddr = BatchRebootAPList[i].IpAddr;
                                    JsonInterFace.GSMDeviceParameter.Port = BatchRebootAPList[i].Port;
                                    JsonInterFace.GSMDeviceParameter.InnerType = BatchRebootAPList[i].InnerType;
                                    JsonInterFace.GSMDeviceParameter.SN = BatchRebootAPList[i].SN;
                                    RebootApTasck(2);
                                }
                                else if (DeviceType.CDMA == BatchRebootAPList[i].Mode)
                                {
                                    JsonInterFace.CDMADeviceParameter.DomainFullPathName = txtStation.Text;
                                    JsonInterFace.CDMADeviceParameter.DeviceName = BatchRebootAPList[i].SelfName;
                                    JsonInterFace.CDMADeviceParameter.IpAddr = BatchRebootAPList[i].IpAddr;
                                    JsonInterFace.CDMADeviceParameter.Port = BatchRebootAPList[i].Port;
                                    JsonInterFace.CDMADeviceParameter.InnerType = BatchRebootAPList[i].InnerType;
                                    JsonInterFace.CDMADeviceParameter.SN = BatchRebootAPList[i].SN;
                                    RebootApTasck(3);
                                }
                                else if (DeviceType.GSMV2 == BatchRebootAPList[i].Mode)
                                {
                                    JsonInterFace.GSMV2DeviceParameter.DomainFullPathName = txtStation.Text;
                                    JsonInterFace.GSMV2DeviceParameter.DeviceName = BatchRebootAPList[i].SelfName;
                                    JsonInterFace.GSMV2DeviceParameter.IpAddr = BatchRebootAPList[i].IpAddr;
                                    JsonInterFace.GSMV2DeviceParameter.Port = BatchRebootAPList[i].Port;
                                    JsonInterFace.GSMV2DeviceParameter.InnerType = BatchRebootAPList[i].InnerType;
                                    JsonInterFace.GSMV2DeviceParameter.SN = BatchRebootAPList[i].SN;
                                    RebootApTasck(4);
                                }
                                else if (DeviceType.TD_SCDMA == BatchRebootAPList[i].Mode)
                                {
                                    JsonInterFace.TDSDeviceParameter.DomainFullPathName = txtStation.Text;
                                    JsonInterFace.TDSDeviceParameter.DeviceName = BatchRebootAPList[i].SelfName;
                                    JsonInterFace.TDSDeviceParameter.IpAddr = BatchRebootAPList[i].IpAddr;
                                    JsonInterFace.TDSDeviceParameter.Port = BatchRebootAPList[i].Port;
                                    JsonInterFace.TDSDeviceParameter.InnerType = BatchRebootAPList[i].InnerType;
                                    JsonInterFace.TDSDeviceParameter.SN = BatchRebootAPList[i].SN;
                                    RebootApTasck(5);
                                }
                            }
                        }
                        catch (Exception Ex)
                        {
                            JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "重启动设备[" + txtStation.Text + "." + BatchRebootAPList[i].SelfName + "]，" + Ex.Message, "重启动设备", "内部故障");
                            Parameters.PrintfLogsExtended("重启动设备[" + txtStation.Text + "." + BatchRebootAPList[i].SelfName + "]内部故障", Ex.Message, Ex.StackTrace);
                        }
                    }
                });
            }).Start();
        }

        //批量提交设备重启
        private void RebootApTasck(byte TaskType)
        {
            try
            {
                switch (TaskType)
                {
                    case 0:
                        JsonInterFace.SystemLogsInfo.Input(
                                    DateTime.Now.ToString(),
                                    "正在重启动设备[" + JsonInterFace.LteDeviceParameter.DomainFullPathName + "." + JsonInterFace.LteDeviceParameter.DeviceName + "，SN=" + JsonInterFace.LteDeviceParameter.SN + "]",
                                    "设备重启",
                                    "正在通讯..."
                                    );
                        NetWorkClient.ControllerServer.Send(JsonInterFace.APRestartRequest(
                                            JsonInterFace.LteDeviceParameter.DomainFullPathName + "." + JsonInterFace.LteDeviceParameter.DeviceName,
                                            JsonInterFace.LteDeviceParameter.DeviceName,
                                            JsonInterFace.LteDeviceParameter.IpAddr,
                                            int.Parse(JsonInterFace.LteDeviceParameter.Port),
                                            JsonInterFace.LteDeviceParameter.InnerType,
                                            JsonInterFace.LteDeviceParameter.SN
                                      ));
                        break;
                    case 1:
                        JsonInterFace.SystemLogsInfo.Input(
                                    DateTime.Now.ToString(),
                                    "正在重启动设备[" + JsonInterFace.WCDMADeviceParameter.DomainFullPathName + "." + JsonInterFace.WCDMADeviceParameter.DeviceName + "，SN=" + JsonInterFace.WCDMADeviceParameter.SN + "]",
                                    "设备重启",
                                    "正在通讯..."
                                  );
                        NetWorkClient.ControllerServer.Send(JsonInterFace.APRestartRequest(
                                            JsonInterFace.WCDMADeviceParameter.DomainFullPathName + "." + JsonInterFace.WCDMADeviceParameter.DeviceName,
                                            JsonInterFace.WCDMADeviceParameter.DeviceName,
                                            JsonInterFace.WCDMADeviceParameter.IpAddr,
                                            int.Parse(JsonInterFace.WCDMADeviceParameter.Port),
                                            JsonInterFace.WCDMADeviceParameter.InnerType,
                                            JsonInterFace.WCDMADeviceParameter.SN
                                      ));
                        break;
                    case 2:
                        JsonInterFace.SystemLogsInfo.Input(
                                    DateTime.Now.ToString(),
                                    "正在重启动设备[" + JsonInterFace.GSMDeviceParameter.DomainFullPathName + "." + JsonInterFace.GSMDeviceParameter.DeviceName + "，SN=" + JsonInterFace.GSMDeviceParameter.SN + "]",
                                    "设备重启",
                                    "正在通讯..."
                                  );
                        NetWorkClient.ControllerServer.Send(JsonInterFace.APRestartRequest(
                                            JsonInterFace.GSMDeviceParameter.DomainFullPathName + "." + JsonInterFace.GSMDeviceParameter.DeviceName,
                                            JsonInterFace.GSMDeviceParameter.DeviceName,
                                            JsonInterFace.GSMDeviceParameter.IpAddr,
                                            int.Parse(JsonInterFace.GSMDeviceParameter.Port),
                                            JsonInterFace.GSMDeviceParameter.InnerType,
                                            JsonInterFace.GSMDeviceParameter.SN
                                      ));
                        break;
                    case 3:
                        JsonInterFace.SystemLogsInfo.Input(
                                    DateTime.Now.ToString(),
                                    "正在重启动设备[" + JsonInterFace.CDMADeviceParameter.DomainFullPathName + "." + JsonInterFace.CDMADeviceParameter.DeviceName + "，SN=" + JsonInterFace.CDMADeviceParameter.SN + "]",
                                    "设备重启",
                                    "正在通讯..."
                                  );
                        NetWorkClient.ControllerServer.Send(JsonInterFace.APRestartRequest(
                                            JsonInterFace.CDMADeviceParameter.DomainFullPathName + "." + JsonInterFace.CDMADeviceParameter.DeviceName,
                                            JsonInterFace.CDMADeviceParameter.DeviceName,
                                            JsonInterFace.CDMADeviceParameter.IpAddr,
                                            int.Parse(JsonInterFace.CDMADeviceParameter.Port),
                                            JsonInterFace.CDMADeviceParameter.InnerType,
                                            JsonInterFace.CDMADeviceParameter.SN
                                      ));
                        break;
                    case 4:
                        JsonInterFace.SystemLogsInfo.Input(
                                    DateTime.Now.ToString(),
                                    "正在重启动设备[" + JsonInterFace.GSMV2DeviceParameter.DomainFullPathName + "." + JsonInterFace.GSMV2DeviceParameter.DeviceName + "，SN=" + JsonInterFace.GSMV2DeviceParameter.SN + "]",
                                    "设备重启",
                                    "正在通讯..."
                                  );
                        NetWorkClient.ControllerServer.Send(JsonInterFace.APRestartRequest(
                                            JsonInterFace.GSMV2DeviceParameter.DomainFullPathName + "." + JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                            JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                            JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                            int.Parse(JsonInterFace.GSMV2DeviceParameter.Port),
                                            JsonInterFace.GSMV2DeviceParameter.InnerType,
                                            JsonInterFace.GSMV2DeviceParameter.SN
                                      ));
                        break;
                    case 5:
                        JsonInterFace.SystemLogsInfo.Input(
                                    DateTime.Now.ToString(),
                                    "正在重启动设备[" + JsonInterFace.TDSDeviceParameter.DomainFullPathName + "." + JsonInterFace.TDSDeviceParameter.DeviceName + "，SN=" + JsonInterFace.TDSDeviceParameter.SN + "]",
                                    "设备重启",
                                    "正在通讯..."
                                  );
                        NetWorkClient.ControllerServer.Send(JsonInterFace.APRestartRequest(
                                            JsonInterFace.TDSDeviceParameter.DomainFullPathName + "." + JsonInterFace.TDSDeviceParameter.DeviceName,
                                            JsonInterFace.TDSDeviceParameter.DeviceName,
                                            JsonInterFace.TDSDeviceParameter.IpAddr,
                                            int.Parse(JsonInterFace.TDSDeviceParameter.Port),
                                            JsonInterFace.TDSDeviceParameter.InnerType,
                                            JsonInterFace.TDSDeviceParameter.SN
                                      ));
                        break;
                    default:
                        JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "设备重启未找到对应的类型", "设备重启", "内部故障");
                        break;
                }
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("提交设备重启", Ex.Message, Ex.StackTrace);
            }
        }

        //批量提交设备重启进度
        private void RebootApRebootComplete(IntPtr lParam)
        {
            string resultStr = (Marshal.PtrToStringBSTR(lParam));
            BatchRebootResult.AppendLine(resultStr);
            ProgressBarParameter.StepValue++;

            if (new Regex("错误").Match(resultStr).Success)
            {
                JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), resultStr, "设备重启", "错误");
            }
            else
            {
                JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), resultStr, "设备重启", "成功");
            }

            Dispatcher.Invoke(() =>
            {

                if (BatchRebootAPList.Count > 0)
                {
                    lock (BatchRebootAPListDataLock)
                    {
                        for (int i = 0; i < BatchRebootAPList.Count; i++)
                        {
                            if (new Regex(BatchRebootAPList[i].SN).Match(resultStr).Success)
                            {
                                BatchRebootAPList.RemoveAt(i);
                                break;
                            }
                            else if (new Regex(BatchRebootAPList[i].IpAddr + ":" + BatchRebootAPList[i].Port).Match(resultStr).Success)
                            {
                                BatchRebootAPList.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }
            });

            if (BatchRebootApCount == ProgressBarParameter.StepValue)
            {
                CheckTimedOut.Stop();
                ProgressBarParameter.StepValue = 0;
                Dispatcher.Invoke(() =>
                {
                    lock (BatchRebootAPListDataLock)
                    {
                        if (BatchRebootAPList.Count > 0)
                        {
                            BatchRebootAPList.Clear();
                        }
                    }
                });
                MessageBox.Show(BatchRebootResult.ToString(), "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void RebootDeviceIsSelectedAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                for (int i = 0; i < BatchRebootAPList.Count; i++)
                {
                    BatchRebootAPList[i].IsSelected = Convert.ToBoolean((sender as CheckBox).IsChecked);
                }
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended(Ex.Message, Ex.StackTrace);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            this.Tag = string.Empty;
            CheckSelectedAllItemTimer.Stop();
        }

        /// <summary>
        /// 检测是否全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckSelectedAllItem(object sender, EventArgs e)
        {
            try
            {
                int SelectedItemCount = 0;
                lock (BatchRebootAPListDataLock)
                {
                    for (int i = 0; i < BatchRebootAPList.Count; i++)
                    {
                        if (BatchRebootAPList[i].IsSelected)
                        {
                            SelectedItemCount++;
                        }
                    }
                }

                //是否改变全选状态
                CheckBox SelectedAllCtrl = Parameters.GetVisualChild<CheckBox>(dgRebootAPList, Item => Item.Name == "RebootDeviceIsSelectedAll");
                if (SelectedItemCount == BatchRebootAPList.Count && BatchRebootAPList.Count > 0)
                {
                    if (null != SelectedAllCtrl)
                    {
                        SelectedAllCtrl.IsChecked = true;
                    }
                }
                else
                {
                    SelectedAllCtrl.IsChecked = false;
                }
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended(Ex.Message, Ex.StackTrace);
            }
        }

        private void mmAllSelected_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                for (int i = 0; i < BatchRebootAPList.Count; i++)
                {
                    BatchRebootAPList[i].IsSelected = true;
                }
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended(Ex.Message, Ex.StackTrace);
            }
        }

        private void mmAllUnSelected_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                for (int i = 0; i < BatchRebootAPList.Count; i++)
                {
                    BatchRebootAPList[i].IsSelected = false;
                }
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended(Ex.Message, Ex.StackTrace);
            }
        }
    }
}
