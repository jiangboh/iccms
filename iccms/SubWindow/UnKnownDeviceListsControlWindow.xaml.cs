using DataInterface;
using NetController;
using ParameterControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace iccms.SubWindow
{
    ///为未知设备指定站点类
    public class UnKnownDeviceNameStationListClass
    {
        private string iD;
        private string deviceName;
        private string deviceMode;
        private string parentID;
        private List<string> domainFullNamePath = null;

        public UnKnownDeviceNameStationListClass()
        {
            if (DomainFullNamePath == null)
            {
                DomainFullNamePath = new List<string>();
            }
        }

        public string DeviceName
        {
            get
            {
                return deviceName;
            }

            set
            {
                deviceName = value;
            }
        }

        public string ID
        {
            get
            {
                return iD;
            }

            set
            {
                iD = value;
            }
        }

        public List<string> DomainFullNamePath
        {
            get
            {
                return domainFullNamePath;
            }

            set
            {
                domainFullNamePath = value;
            }
        }

        public string DeviceMode
        {
            get
            {
                return deviceMode;
            }

            set
            {
                deviceMode = value;
            }
        }

        public string ParentID
        {
            get
            {
                return parentID;
            }

            set
            {
                parentID = value;
            }
        }
    }

    /// <summary>
    /// 未知设备重命名
    /// </summary>
    public class UnknownDeviceReNameClass : INotifyPropertyChanged
    {
        private string unknownSourceName = string.Empty;
        private string unknownNewName = string.Empty;
        private bool nameOverride = false;
        private bool selectCustomInput = true;
        private bool selectDeviceList = false;
        private Visibility selectedCustomEnable = Visibility.Visible;
        private Visibility selectedListEnable = Visibility.Collapsed;

        private List<SourceDeviceNameOfSelectedStationClass> stationDeviceNameList = null;

        //构造
        public UnknownDeviceReNameClass()
        {
            if (StationDeviceNameList == null)
            {
                StationDeviceNameList = new List<SourceDeviceNameOfSelectedStationClass>();
            }
        }

        public class SourceDeviceNameOfSelectedStationClass : INotifyPropertyChanged
        {
            private string _selfID;
            private string _selfIcon;
            private string _selfName;
            private string _fullName;

            public event PropertyChangedEventHandler PropertyChanged;

            private void NotifyPropertyChanged(string value)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(value));
                }
            }

            public string SelfID
            {
                get
                {
                    return _selfID;
                }

                set
                {
                    _selfID = value;
                    NotifyPropertyChanged("SelfID");
                }
            }

            public string SelfIcon
            {
                get
                {
                    return _selfIcon;
                }

                set
                {
                    _selfIcon = value;
                    NotifyPropertyChanged("SelfIcon");
                }
            }

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

            public string FullName
            {
                get
                {
                    return _fullName;
                }

                set
                {
                    _fullName = value;
                    NotifyPropertyChanged("FullName");
                }
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

        public string UnknownSourceName
        {
            get
            {
                return unknownSourceName;
            }

            set
            {
                unknownSourceName = value;
                NotifyPropertyChanged("UnknownSourceName");
            }
        }

        public string UnknownNewName
        {
            get
            {
                return unknownNewName;
            }

            set
            {
                unknownNewName = value;
                NotifyPropertyChanged("UnknownNewName");
            }
        }

        public bool NameOverride
        {
            get
            {
                return nameOverride;
            }

            set
            {
                nameOverride = value;
                NotifyPropertyChanged("NameOverride");
            }
        }

        public List<SourceDeviceNameOfSelectedStationClass> StationDeviceNameList
        {
            get
            {
                return stationDeviceNameList;
            }

            set
            {
                stationDeviceNameList = value;
            }
        }

        public bool SelectCustomInput
        {
            get
            {
                return selectCustomInput;
            }

            set
            {
                selectCustomInput = value;
                NotifyPropertyChanged("SelectCustomInput");
            }
        }

        public bool SelectDeviceList
        {
            get
            {
                return selectDeviceList;
            }

            set
            {
                selectDeviceList = value;
                NotifyPropertyChanged("SelectDeviceList");
            }
        }

        public Visibility SelectedCustomEnable
        {
            get
            {
                return selectedCustomEnable;
            }

            set
            {
                selectedCustomEnable = value;
                NotifyPropertyChanged("SelectedCustomEnable");
            }
        }

        public Visibility SelectedListEnable
        {
            get
            {
                return selectedListEnable;
            }

            set
            {
                selectedListEnable = value;
                NotifyPropertyChanged("SelectedListEnable");
            }
        }
    }

    /// <summary>
    /// 提交未知设备进度
    /// </summary>
    public class UnknownDeviceSubmitProgressBar : INotifyPropertyChanged
    {
        private int _maxValue = 100;
        private int _stepValue = 0;
        private Visibility _enabled = Visibility.Collapsed;
        private string _tipsColor = "Yellow";

        public event PropertyChangedEventHandler PropertyChanged;

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

        public Visibility Enabled
        {
            get
            {
                return _enabled;
            }

            set
            {
                _enabled = value;
                NotifyPropertyChanged("Enabled");
            }
        }

        public string TipsColor
        {
            get
            {
                return _tipsColor;
            }

            set
            {
                _tipsColor = value;
                NotifyPropertyChanged("TipsColor");
            }
        }

        private void NotifyPropertyChanged(string value)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(value));
            }
        }
    }

    /// <summary>
    /// UnKnownDeviceListsControlWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UnKnownDeviceListsControlWindow : Window
    {
        //未知设备显示表
        public static ObservableCollection<UnKnownDeviceListsParameterClass> ShowUnKnownDeviceLists = new ObservableCollection<UnKnownDeviceListsParameterClass>();
        public static object ShowUnKnownDeviceListsLocked = new object();
        public static int CompleteNumber = -1;
        public static string ReturnStr = string.Empty;

        //已选择的未知设备信息表
        public static List<UnKnownDeviceListsParameterClass> SubmitedUnknownDeviceLists = new List<UnKnownDeviceListsParameterClass>();
        public static UnknownDeviceReNameClass UnknownDeviceReName = new UnknownDeviceReNameClass();
        private System.Timers.Timer ShowUnknownDeviceTatolTimer = null;

        //进度条信息
        private static UnknownDeviceSubmitProgressBar ProgressBarStatus = new UnknownDeviceSubmitProgressBar();

        //===
        public static List<UnKnownDeviceNameStationListClass> UnKnownDeviceNameStationList = new List<UnKnownDeviceNameStationListClass>();
        private List<string> UnknownDeviceToStationSelectedLists = new List<string>();
        //===

        public UnKnownDeviceListsControlWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;

            if (ShowUnknownDeviceTatolTimer == null)
            {
                ShowUnknownDeviceTatolTimer = new System.Timers.Timer();
                ShowUnknownDeviceTatolTimer.Interval = 1000;
                ShowUnknownDeviceTatolTimer.AutoReset = true;
                ShowUnknownDeviceTatolTimer.Elapsed += ShowUnknownDeviceTatolTimer_Elapsed;
            }
        }

        private void ShowUnknownDeviceTatolTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ControlUnknownDeviceCount();
        }

        /// <summary>
        /// 事件监听
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            if (hwndSource != null)
            {
                IntPtr handle = hwndSource.Handle;
                hwndSource.AddHook(new HwndSourceHook(WndProc));
            }
        }

        /// <summary>
        /// 响应Window消息
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            try
            {
                //更新本地未知设备缓存数据
                if (msg == Parameters.WM_UnknownDeviceAddResponse)
                {
                    CompleteNumber = Convert.ToInt32(Marshal.PtrToStringBSTR(lParam));
                    ReturnStr = Marshal.PtrToStringBSTR(wParam);
                    UpdateDeviceTreeViewCaching(CompleteNumber, ReturnStr);
                }
                else if (msg == Parameters.WM_UnknownDeviceDragToAddMessage)
                {
                    DragToAdd();
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("Windows 消息事件：", ex.Message, ex.StackTrace);
            }
            return hwnd;
        }

        /// <summary>
        /// 更新设备列表缓存
        /// </summary>
        private void UpdateDeviceTreeViewCaching(int CompleteNumber, string ReturnStr)
        {
            try
            {
                #region 记录事件日志
                if (CompleteNumber == SubmitedUnknownDeviceLists.Count - 1)
                {
                    JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "添加未知设备[" + SubmitedUnknownDeviceLists[CompleteNumber].ToStation + "." + SubmitedUnknownDeviceLists[CompleteNumber].DeviceName + " 制式:" + SubmitedUnknownDeviceLists[CompleteNumber].Mode + "]", "添加未知设备", ReturnStr.Split(new char[] { '|' })[1]);
                    if (JsonInterFace.ResultMessageList.Length <= 0)
                    {
                        JsonInterFace.ResultMessageList.AppendLine("添加未知设备:");
                        JsonInterFace.ResultMessageList.AppendLine("添加未知设备[" + SubmitedUnknownDeviceLists[CompleteNumber].ToStation + "." + SubmitedUnknownDeviceLists[CompleteNumber].DeviceName + " 制式:" + SubmitedUnknownDeviceLists[CompleteNumber].Mode + "] ------------ [" + ReturnStr.Split(new char[] { '|' })[1] + "]");
                    }
                    else
                    {
                        JsonInterFace.ResultMessageList.AppendLine("添加未知设备[" + SubmitedUnknownDeviceLists[CompleteNumber].ToStation + "." + SubmitedUnknownDeviceLists[CompleteNumber].DeviceName + " 制式:" + SubmitedUnknownDeviceLists[CompleteNumber].Mode + "] ------------ [" + ReturnStr.Split(new char[] { '|' })[1] + "]");
                    }

                    new Thread(() =>
                    {
                        JsonInterFace.ShowMessage(JsonInterFace.ResultMessageList.ToString(), 64);
                    }).Start();
                }
                else
                {
                    if (JsonInterFace.ResultMessageList.Length <= 0)
                    {
                        JsonInterFace.ResultMessageList.AppendLine("添加未知设备:");
                        JsonInterFace.ResultMessageList.AppendLine("添加未知设备[" + SubmitedUnknownDeviceLists[CompleteNumber].ToStation + "." + SubmitedUnknownDeviceLists[CompleteNumber].DeviceName + " 制式:" + SubmitedUnknownDeviceLists[CompleteNumber].Mode + "] ------------ [" + ReturnStr.Split(new char[] { '|' })[1] + "]");
                    }
                    else
                    {
                        JsonInterFace.ResultMessageList.AppendLine("添加未知设备[" + SubmitedUnknownDeviceLists[CompleteNumber].ToStation + "." + SubmitedUnknownDeviceLists[CompleteNumber].DeviceName + " 制式:" + SubmitedUnknownDeviceLists[CompleteNumber].Mode + "] ------------ [" + ReturnStr.Split(new char[] { '|' })[1] + "]");
                    }

                    JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "添加未知设备[" + SubmitedUnknownDeviceLists[CompleteNumber].ToStation + "." + SubmitedUnknownDeviceLists[CompleteNumber].DeviceName + " 制式:" + SubmitedUnknownDeviceLists[CompleteNumber].Mode + "]", "添加未知设备", ReturnStr.Split(new char[] { '|' })[1]);
                }
                #endregion
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("添加未知设备更新数据失败", ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// 发送设备参数信息
        /// </summary>
        private void UpdateDeviceParametersToServer(string DomainFullPathName, string DeviceName, Dictionary<string, string> UpdateParamList)
        {
            try
            {
                if (NetWorkClient.ControllerServer.Connected)
                {
                    Parameters.ConfigType = "Auto";
                    NetWorkClient.ControllerServer.Send(JsonInterFace.AddDeviceNameUpdateParametersRequest(DomainFullPathName, DeviceName, UpdateParamList));
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("设备参数更新至服务端", ex.Message, ex.StackTrace);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 提交未知设备添加到域列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SubmitedUnknownDeviceLists.Clear();
                lock (ShowUnKnownDeviceListsLocked)
                {
                    for (int i = 0; i < ShowUnKnownDeviceLists.Count; i++)
                    {
                        if (ShowUnKnownDeviceLists[i].IsSelected)
                        {
                            //提交该未知设备的详细属性参数
                            UnKnownDeviceListsParameterClass UnknownDeviceInfo = new UnKnownDeviceListsParameterClass();
                            UnknownDeviceInfo.ID = ShowUnKnownDeviceLists[i].ID;
                            UnknownDeviceInfo.ToStation = ShowUnKnownDeviceLists[i].ToStation;
                            UnknownDeviceInfo.DeviceName = ShowUnKnownDeviceLists[i].DeviceName;
                            UnknownDeviceInfo.Mode = ShowUnKnownDeviceLists[i].Mode;
                            UnknownDeviceInfo.SN = ShowUnKnownDeviceLists[i].SN;
                            UnknownDeviceInfo.IpAddr = ShowUnKnownDeviceLists[i].IpAddr;
                            UnknownDeviceInfo.Port = ShowUnKnownDeviceLists[i].Port;
                            UnknownDeviceInfo.Netmask = ShowUnKnownDeviceLists[i].Netmask;
                            UnknownDeviceInfo.InnerType = ShowUnKnownDeviceLists[i].InnerType;
                            UnknownDeviceInfo.APVersion = ShowUnKnownDeviceLists[i].APVersion;
                            UnknownDeviceInfo.Carrier = ShowUnKnownDeviceLists[i].Carrier;
                            UnknownDeviceInfo.IsActive = ShowUnKnownDeviceLists[i].IsActive;
                            UnknownDeviceInfo.StationID = ShowUnKnownDeviceLists[i].StationID;
                            SubmitedUnknownDeviceLists.Add(UnknownDeviceInfo);
                        }
                    }
                }
                int SelectedCount = SubmitedUnknownDeviceLists.Count;

                if (SelectedCount <= 0)
                {
                    MessageBox.Show("未选择任何[未知设备]，请选择[未知设备]并为[未知设备]指定站点后再继续！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                for (int i = 0; i < SubmitedUnknownDeviceLists.Count; i++)
                {
                    if (DeviceType.UnknownType.ToLower() == SubmitedUnknownDeviceLists[i].DeviceName.ToLower())
                    {
                        MessageBox.Show("你选择包含了未命名的[未知设备]，请重命名后再继续！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    else if (SubmitedUnknownDeviceLists[i].ToStation == null && SubmitedUnknownDeviceLists[i].ToStation == "")
                    {
                        MessageBox.Show("你选择包含了未指定站点的[未知设备]，请并为[未知设备]指定站点后再继续！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                //发送
                if (NetWorkClient.ControllerServer.Connected)
                {
                    new Thread(() =>
                    {
                        //清空多类信息列表
                        JsonInterFace.ResultMessageList.Clear();
                        StringBuilder ErrMsg = new StringBuilder();
                        Parameters.CompleteCount = 0;

                        ProgressBarStatus.MaxValue = SubmitedUnknownDeviceLists.Count;
                        ProgressBarStatus.StepValue = 0;
                        ProgressBarStatus.Enabled = Visibility.Visible;

                        for (int i = 0; i < SubmitedUnknownDeviceLists.Count; i++)
                        {
                            //未知设备屏蔽打开
                            Parameters.STRefresh = false;
                            //原信息
                            string SN = SubmitedUnknownDeviceLists[i].SN;
                            string SourceName = Parameters.UnknownDeviceNameDefault;
                            string SourceIPAddr = SubmitedUnknownDeviceLists[i].IpAddr;
                            string SourcePort = SubmitedUnknownDeviceLists[i].Port;

                            //提交的信息
                            string StationFullName = SubmitedUnknownDeviceLists[i].ToStation;
                            string DeviceName = SubmitedUnknownDeviceLists[i].DeviceName;
                            string DeviceMode = SubmitedUnknownDeviceLists[i].Mode;

                            //提交设备数
                            Parameters.ConfigType = "UnknownDeviceToStation:" + SelectedCount.ToString();

                            //进度
                            ProgressBarStatus.StepValue++;

                            if (StationFullName != null && StationFullName != "" && StationFullName != Parameters.ToStationDefault)
                            {
                                //发送
                                NetWorkClient.ControllerServer.Send(
                                                                        JsonInterFace.AddDeviceNameRequest(
                                                                                                            StationFullName,
                                                                                                            DeviceName,
                                                                                                            DeviceMode,
                                                                                                            SourceName,
                                                                                                            SourceIPAddr,
                                                                                                            SourcePort
                                                                                                          )
                                                                   );


                                //删除已成功提交的未知设备
                                while (true)
                                {
                                    if (ReturnStr != null && ReturnStr != "")
                                    {
                                        if (ReturnStr.Split(new char[] { '|' })[0] == "0")
                                        {
                                            #region 1. 将已成功添加的未知设备添加/更新到设备树列表
                                            if (ReturnStr.Split(new char[] { '|' })[0] == "0")
                                            {
                                                //不覆盖(追加)
                                                if (!UnknownDeviceReName.NameOverride)
                                                {
                                                    //将成功添加的未知设备追加到设备树列表
                                                    DataRow rw = JsonInterFace.BindTreeViewClass.DeviceTreeTable.NewRow();
                                                    rw["PathName"] = SubmitedUnknownDeviceLists[CompleteNumber].ToStation + "." + SubmitedUnknownDeviceLists[CompleteNumber].DeviceName;
                                                    rw["SelfID"] = (Convert.ToInt32((JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows.Count - 1][1])) + 1).ToString();
                                                    rw["SelfName"] = SubmitedUnknownDeviceLists[CompleteNumber].DeviceName;
                                                    rw["ParentID"] = SubmitedUnknownDeviceLists[CompleteNumber].StationID;
                                                    rw["IsStation"] = "2";
                                                    rw["NodeContent"] = SubmitedUnknownDeviceLists[CompleteNumber].DeviceName;
                                                    rw["IsDeleted"] = false;
                                                    rw["Permission"] = "Enable";
                                                    rw["NodeType"] = NodeType.LeafNode.ToString();
                                                    rw["CarrierStatus"] = SubmitedUnknownDeviceLists[CompleteNumber].Carrier;
                                                    //是否在线须检测设备类型及载波
                                                    if (SubmitedUnknownDeviceLists[CompleteNumber].Online == "1")
                                                    {
                                                        if (SubmitedUnknownDeviceLists[CompleteNumber].IsActive == "1")
                                                        {
                                                            //单载波类型设备
                                                            if (new Regex(DeviceType.LTE).Match(SubmitedUnknownDeviceLists[CompleteNumber].Mode).Success || SubmitedUnknownDeviceLists[CompleteNumber].Mode == DeviceType.WCDMA || new Regex("TD").Match(SubmitedUnknownDeviceLists[CompleteNumber].Mode).Success)
                                                            {
                                                                rw["NodeIcon"] = new NodeIcon().LeafAllReadyNodeIcon;
                                                            }
                                                            //双载波类型设备
                                                            else
                                                            {
                                                                //载波1
                                                                if (SubmitedUnknownDeviceLists[CompleteNumber].Carrier == "0")
                                                                {
                                                                    rw["NodeIcon"] = new NodeIcon().Carrier_One_ActiveIcon;
                                                                }
                                                                //载波2
                                                                else if (SubmitedUnknownDeviceLists[CompleteNumber].Carrier == "1")
                                                                {
                                                                    rw["NodeIcon"] = new NodeIcon().Carrier_Two_ActiveIcon;
                                                                }
                                                                //全载波
                                                                else
                                                                {
                                                                    rw["NodeIcon"] = new NodeIcon().LeafAllReadyNodeIcon;
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            rw["NodeIcon"] = new NodeIcon().LeafNoActiveNodeIcon;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        rw["NodeIcon"] = new NodeIcon().LeafNoConnectNodeIcon;
                                                    }
                                                    JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows.Add(rw);

                                                    //将成功添加的未知设备追加到设备属性列表
                                                    APATTributes Apattribute = new APATTributes();
                                                    Apattribute.SelfID = (Convert.ToInt32((JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows.Count - 1][1])) + 1).ToString();
                                                    Apattribute.ParentID = SubmitedUnknownDeviceLists[CompleteNumber].StationID;
                                                    Apattribute.SelfName = SubmitedUnknownDeviceLists[CompleteNumber].DeviceName;
                                                    Apattribute.FullName = SubmitedUnknownDeviceLists[CompleteNumber].ToStation + "." + SubmitedUnknownDeviceLists[CompleteNumber].DeviceName;
                                                    Apattribute.Mode = SubmitedUnknownDeviceLists[CompleteNumber].Mode;
                                                    Apattribute.Carrier = SubmitedUnknownDeviceLists[CompleteNumber].Carrier;
                                                    Apattribute.SN = SubmitedUnknownDeviceLists[CompleteNumber].SN;
                                                    Apattribute.IpAddr = SubmitedUnknownDeviceLists[CompleteNumber].IpAddr;
                                                    Apattribute.Port = SubmitedUnknownDeviceLists[CompleteNumber].Port;
                                                    Apattribute.NetMask = SubmitedUnknownDeviceLists[CompleteNumber].Netmask;
                                                    Apattribute.OnLine = SubmitedUnknownDeviceLists[CompleteNumber].Online;
                                                    Apattribute.IsActive = SubmitedUnknownDeviceLists[CompleteNumber].IsActive;
                                                    Apattribute.LastOnline = SubmitedUnknownDeviceLists[CompleteNumber].LastOnline;
                                                    //数据对齐状态
                                                    Apattribute.AlertIcon = new NodeIcon().AlignAlertIcon;
                                                    Apattribute.AlertText = "该设备数据未对齐";
                                                    Apattribute.ALIGN = "-1";
                                                    //白名单自学习状态
                                                    Apattribute.Command = "-1";
                                                    JsonInterFace.APATTributesLists.Add(Apattribute);

                                                    //该设备参数更新至服务器
                                                    Dictionary<string, string> UpdateParamList = new Dictionary<string, string>();
                                                    UpdateParamList.Add("name", Apattribute.SelfName);
                                                    UpdateParamList.Add("mode", Apattribute.Mode);
                                                    UpdateParamList.Add("sn", Apattribute.SN);
                                                    UpdateParamList.Add("carrier", Apattribute.Carrier);
                                                    UpdateParamList.Add("ipAddr", Apattribute.IpAddr);
                                                    UpdateParamList.Add("port", Apattribute.Port);
                                                    UpdateParamList.Add("netmask", Apattribute.NetMask);
                                                    UpdateDeviceParametersToServer(SubmitedUnknownDeviceLists[CompleteNumber].ToStation, SubmitedUnknownDeviceLists[CompleteNumber].DeviceName, UpdateParamList);
                                                }
                                                //覆盖(更新)
                                                else
                                                {
                                                    //将成功添加的未知设备更新到设备树列表
                                                    for (int m = 0; m < JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows.Count; m++)
                                                    {
                                                        if ((SubmitedUnknownDeviceLists[CompleteNumber].ToStation + "." + SubmitedUnknownDeviceLists[CompleteNumber].DeviceName)
                                                            == JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[m]["PathName"].ToString())
                                                        {
                                                            DataRow rw = JsonInterFace.BindTreeViewClass.DeviceTreeTable.NewRow();
                                                            JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[m]["PathName"] = SubmitedUnknownDeviceLists[CompleteNumber].ToStation + "." + SubmitedUnknownDeviceLists[CompleteNumber].DeviceName;
                                                            JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[m]["SelfID"] = Convert.ToInt32(JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[m]["SelfID"].ToString());
                                                            JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[m]["SelfName"] = SubmitedUnknownDeviceLists[CompleteNumber].DeviceName;
                                                            JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[m]["ParentID"] = SubmitedUnknownDeviceLists[CompleteNumber].StationID;
                                                            JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[m]["IsStation"] = "2";
                                                            JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[m]["NodeContent"] = SubmitedUnknownDeviceLists[CompleteNumber].DeviceName;
                                                            JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[m]["IsDeleted"] = false;
                                                            JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[m]["Permission"] = "Enable";
                                                            JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[m]["NodeType"] = NodeType.LeafNode.ToString();
                                                            JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[m]["CarrierStatus"] = SubmitedUnknownDeviceLists[CompleteNumber].Carrier;

                                                            if (SubmitedUnknownDeviceLists[CompleteNumber].Online == "1")
                                                            {
                                                                if (SubmitedUnknownDeviceLists[CompleteNumber].IsActive == "1")
                                                                {
                                                                    //单载波类型设备
                                                                    if (new Regex(DeviceType.LTE).Match(SubmitedUnknownDeviceLists[CompleteNumber].Mode).Success || SubmitedUnknownDeviceLists[CompleteNumber].Mode == DeviceType.WCDMA || new Regex("TD").Match(SubmitedUnknownDeviceLists[CompleteNumber].Mode).Success)
                                                                    {
                                                                        JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[m]["NodeIcon"] = new NodeIcon().LeafAllReadyNodeIcon;
                                                                    }
                                                                    //双载波类型设备
                                                                    else
                                                                    {
                                                                        //载波1
                                                                        if (SubmitedUnknownDeviceLists[CompleteNumber].Carrier == "0")
                                                                        {
                                                                            JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[m]["NodeIcon"] = new NodeIcon().Carrier_One_ActiveIcon;
                                                                        }
                                                                        //载波2
                                                                        else if (SubmitedUnknownDeviceLists[CompleteNumber].Carrier == "1")
                                                                        {
                                                                            JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[m]["NodeIcon"] = new NodeIcon().Carrier_Two_ActiveIcon;
                                                                        }
                                                                        //全载波
                                                                        else
                                                                        {
                                                                            JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[m]["NodeIcon"] = new NodeIcon().LeafAllReadyNodeIcon;
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[m]["NodeIcon"] = new NodeIcon().LeafNoActiveNodeIcon;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[m]["NodeIcon"] = new NodeIcon().LeafNoConnectNodeIcon;
                                                            }

                                                            break;
                                                        }
                                                    }

                                                    //更新到属性
                                                    APATTributes Apattribute = new APATTributes();
                                                    Apattribute.SelfName = SubmitedUnknownDeviceLists[CompleteNumber].DeviceName;
                                                    Apattribute.FullName = SubmitedUnknownDeviceLists[CompleteNumber].ToStation + "." + SubmitedUnknownDeviceLists[CompleteNumber].DeviceName;
                                                    Apattribute.Mode = SubmitedUnknownDeviceLists[CompleteNumber].Mode;
                                                    Apattribute.Carrier = SubmitedUnknownDeviceLists[CompleteNumber].Carrier;
                                                    Apattribute.SN = SubmitedUnknownDeviceLists[CompleteNumber].SN;
                                                    Apattribute.IpAddr = SubmitedUnknownDeviceLists[CompleteNumber].IpAddr;
                                                    Apattribute.Port = SubmitedUnknownDeviceLists[CompleteNumber].Port;
                                                    Apattribute.NetMask = SubmitedUnknownDeviceLists[CompleteNumber].Netmask;
                                                    Apattribute.OnLine = SubmitedUnknownDeviceLists[CompleteNumber].Online;
                                                    Apattribute.IsActive = SubmitedUnknownDeviceLists[CompleteNumber].IsActive;
                                                    Apattribute.LastOnline = SubmitedUnknownDeviceLists[CompleteNumber].LastOnline;
                                                    //数据对齐状态
                                                    Apattribute.AlertIcon = new NodeIcon().AlignAlertIcon;
                                                    Apattribute.AlertText = "该设备数据未对齐";
                                                    Apattribute.ALIGN = "-1";
                                                    //白名单自学习状态
                                                    Apattribute.Command = "-1";
                                                    for (int m = 0; m < JsonInterFace.APATTributesLists.Count; m++)
                                                    {
                                                        if (JsonInterFace.APATTributesLists[m].FullName == Apattribute.FullName)
                                                        {
                                                            JsonInterFace.APATTributesLists[m].SelfName = Apattribute.SelfName;
                                                            JsonInterFace.APATTributesLists[m].FullName = Apattribute.FullName;
                                                            JsonInterFace.APATTributesLists[m].Mode = Apattribute.Mode;
                                                            JsonInterFace.APATTributesLists[m].Carrier = Apattribute.Carrier;
                                                            JsonInterFace.APATTributesLists[m].SN = Apattribute.SN;
                                                            JsonInterFace.APATTributesLists[m].IpAddr = Apattribute.IpAddr;
                                                            JsonInterFace.APATTributesLists[m].Port = Apattribute.Port;
                                                            JsonInterFace.APATTributesLists[m].NetMask = Apattribute.NetMask;
                                                            JsonInterFace.APATTributesLists[m].OnLine = Apattribute.OnLine;
                                                            JsonInterFace.APATTributesLists[m].IsActive = Apattribute.IsActive;
                                                            JsonInterFace.APATTributesLists[m].LastOnline = Apattribute.LastOnline;
                                                            //数据对齐状态
                                                            JsonInterFace.APATTributesLists[m].AlertIcon = Apattribute.AlertIcon;
                                                            JsonInterFace.APATTributesLists[m].AlertText = Apattribute.AlertText;
                                                            JsonInterFace.APATTributesLists[m].ALIGN = Apattribute.ALIGN;
                                                            //白名单自学习状态
                                                            JsonInterFace.APATTributesLists[m].Command = Apattribute.Command;
                                                            break;
                                                        }
                                                    }

                                                    //将该设备信息参数更新至服务器
                                                    Dictionary<string, string> UpdateParamList = new Dictionary<string, string>();
                                                    UpdateParamList.Add("name", Apattribute.SelfName);
                                                    UpdateParamList.Add("mode", Apattribute.Mode);
                                                    UpdateParamList.Add("sn", Apattribute.SN);
                                                    UpdateParamList.Add("carrier", Apattribute.Carrier);
                                                    UpdateParamList.Add("ipAddr", Apattribute.IpAddr);
                                                    UpdateParamList.Add("port", Apattribute.Port);
                                                    UpdateParamList.Add("netmask", Apattribute.NetMask);
                                                    UpdateDeviceParametersToServer(SubmitedUnknownDeviceLists[CompleteNumber].ToStation, Apattribute.SelfName, UpdateParamList);
                                                }
                                            }
                                            #endregion

                                            #region 2. 删除对应成功提交项
                                            for (int j = 0; j < ShowUnKnownDeviceLists.Count; j++)
                                            {
                                                if (SN == ShowUnKnownDeviceLists[j].SN)
                                                {
                                                    if (ShowUnKnownDeviceLists.Count > 0)
                                                    {
                                                        try
                                                        {
                                                            ShowUnKnownDeviceLists.RemoveAt(j);
                                                        }
                                                        catch
                                                        {

                                                        }
                                                    }
                                                    break;
                                                }
                                            }
                                            Dispatcher.Invoke(() =>
                                            {
                                                UnKnownDeviceListsGrid.Items.Refresh();
                                            });
                                            #endregion

                                            //重置参数
                                            CompleteNumber = -1;
                                            ReturnStr = string.Empty;
                                            break;
                                        }
                                        else
                                        {
                                            //重置参数
                                            CompleteNumber = -1;
                                            ReturnStr = string.Empty;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        Thread.Sleep(10);
                                    }
                                }
                            }
                            else
                            {
                                ErrMsg.AppendLine("未知设备[" + DeviceName + "],制式[" + DeviceMode + "]没有指定站点！！");
                            }
                        }

                        //未知设备屏蔽关闭
                        Parameters.STRefresh = true;

                        //未知设备提交完成记数重置
                        Parameters.CompleteCount = 0;

                        //清空已选择未知设备列表
                        SubmitedUnknownDeviceLists.Clear();

                        //进度复位
                        ProgressBarStatus.MaxValue = 100;
                        ProgressBarStatus.StepValue = 0;
                        ProgressBarStatus.Enabled = Visibility.Collapsed;

                        #region 刷新设备树显示
                        Parameters.SendMessage(Parameters.WinHandle, Parameters.WM_DeviceListInfoLoad, 0, 0);
                        #endregion

                        if (ErrMsg.Length > 0)
                        {
                            MessageBox.Show(ErrMsg.ToString(), "提交未知设备提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }).Start();
                }
                else
                {
                    //清空已选择未知设备列表
                    SubmitedUnknownDeviceLists.Clear();
                    MessageBox.Show("提交未知设备失败,网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception Ex)
            {
                //未知设备屏蔽关闭
                Parameters.STRefresh = true;
                Parameters.PrintfLogsExtended("提交未知设备失败", Ex.Message, Ex.StackTrace);
            }
        }

        /// <summary>
        /// 拖动提交未知设备
        /// </summary>
        private void DragToAdd()
        {
            try
            {
                int SelectedCount = SubmitedUnknownDeviceLists.Count;

                //发送
                if (NetWorkClient.ControllerServer.Connected)
                {
                    new Thread(() =>
                    {
                        //清空多类信息列表
                        JsonInterFace.ResultMessageList.Clear();
                        StringBuilder ErrMsg = new StringBuilder();
                        Parameters.CompleteCount = 0;

                        ProgressBarStatus.MaxValue = SubmitedUnknownDeviceLists.Count;
                        ProgressBarStatus.StepValue = 0;
                        ProgressBarStatus.Enabled = Visibility.Visible;

                        for (int i = 0; i < SubmitedUnknownDeviceLists.Count; i++)
                        {
                            //未知设备屏蔽打开
                            Parameters.STRefresh = false;
                            //原信息
                            string SN = SubmitedUnknownDeviceLists[i].SN;
                            string SourceName = Parameters.UnknownDeviceNameDefault;
                            string SourceIPAddr = SubmitedUnknownDeviceLists[i].IpAddr;
                            string SourcePort = SubmitedUnknownDeviceLists[i].Port;

                            //提交的信息
                            string StationFullName = SubmitedUnknownDeviceLists[i].ToStation;
                            string DeviceName = SubmitedUnknownDeviceLists[i].DeviceName;
                            string DeviceMode = SubmitedUnknownDeviceLists[i].Mode;

                            //提交设备数
                            Parameters.ConfigType = "UnknownDeviceToStation:" + SelectedCount.ToString();

                            //进度
                            ProgressBarStatus.StepValue++;

                            if (StationFullName != null && StationFullName != "" && StationFullName != Parameters.ToStationDefault)
                            {
                                //发送
                                NetWorkClient.ControllerServer.Send(
                                                                            JsonInterFace.AddDeviceNameRequest(
                                                                                                                StationFullName,
                                                                                                                DeviceName,
                                                                                                                DeviceMode,
                                                                                                                SourceName,
                                                                                                                SourceIPAddr,
                                                                                                                SourcePort
                                                                                                              )
                                                                       );


                                //删除已成功提交的未知设备
                                while (true)
                                {
                                    if (ReturnStr != null && ReturnStr != "")
                                    {
                                        if (ReturnStr.Split(new char[] { '|' })[0] == "0")
                                        {
                                            #region 1. 将已成功添加的未知设备添加/更新到设备树列表
                                            if (ReturnStr.Split(new char[] { '|' })[0] == "0")
                                            {
                                                //不覆盖(追加)
                                                if (!UnknownDeviceReName.NameOverride)
                                                {
                                                    //将成功添加的未知设备追加到设备树列表
                                                    DataRow rw = JsonInterFace.BindTreeViewClass.DeviceTreeTable.NewRow();
                                                    rw["PathName"] = SubmitedUnknownDeviceLists[CompleteNumber].ToStation + "." + SubmitedUnknownDeviceLists[CompleteNumber].DeviceName;
                                                    rw["SelfID"] = (Convert.ToInt32((JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows.Count - 1][1])) + 1).ToString();
                                                    rw["SelfName"] = SubmitedUnknownDeviceLists[CompleteNumber].DeviceName;
                                                    rw["ParentID"] = SubmitedUnknownDeviceLists[CompleteNumber].StationID;
                                                    rw["IsStation"] = "2";
                                                    rw["NodeContent"] = SubmitedUnknownDeviceLists[CompleteNumber].DeviceName;
                                                    rw["IsDeleted"] = false;
                                                    rw["Permission"] = "Enable";
                                                    rw["NodeType"] = NodeType.LeafNode.ToString();
                                                    rw["CarrierStatus"] = SubmitedUnknownDeviceLists[CompleteNumber].Carrier;
                                                    //是否在线须检测设备类型及载波
                                                    if (SubmitedUnknownDeviceLists[CompleteNumber].Online == "1")
                                                    {
                                                        if (SubmitedUnknownDeviceLists[CompleteNumber].IsActive == "1")
                                                        {
                                                            //单载波类型设备
                                                            if (new Regex(DeviceType.LTE).Match(SubmitedUnknownDeviceLists[CompleteNumber].Mode).Success || SubmitedUnknownDeviceLists[CompleteNumber].Mode == DeviceType.WCDMA || new Regex("TD").Match(SubmitedUnknownDeviceLists[CompleteNumber].Mode).Success)
                                                            {
                                                                rw["NodeIcon"] = new NodeIcon().LeafAllReadyNodeIcon;
                                                            }
                                                            //双载波类型设备
                                                            else
                                                            {
                                                                //载波1
                                                                if (SubmitedUnknownDeviceLists[CompleteNumber].Carrier == "0")
                                                                {
                                                                    rw["NodeIcon"] = new NodeIcon().Carrier_One_ActiveIcon;
                                                                }
                                                                //载波2
                                                                else if (SubmitedUnknownDeviceLists[CompleteNumber].Carrier == "1")
                                                                {
                                                                    rw["NodeIcon"] = new NodeIcon().Carrier_Two_ActiveIcon;
                                                                }
                                                                //全载波
                                                                else
                                                                {
                                                                    rw["NodeIcon"] = new NodeIcon().LeafAllReadyNodeIcon;
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            rw["NodeIcon"] = new NodeIcon().LeafNoActiveNodeIcon;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        rw["NodeIcon"] = new NodeIcon().LeafNoConnectNodeIcon;
                                                    }
                                                    JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows.Add(rw);

                                                    //将成功添加的未知设备追加到设备属性列表
                                                    APATTributes Apattribute = new APATTributes();
                                                    Apattribute.SelfID = (Convert.ToInt32((JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows.Count - 1][1])) + 1).ToString();
                                                    Apattribute.ParentID = SubmitedUnknownDeviceLists[CompleteNumber].StationID;
                                                    Apattribute.SelfName = SubmitedUnknownDeviceLists[CompleteNumber].DeviceName;
                                                    Apattribute.FullName = SubmitedUnknownDeviceLists[CompleteNumber].ToStation + "." + SubmitedUnknownDeviceLists[CompleteNumber].DeviceName;
                                                    Apattribute.Mode = SubmitedUnknownDeviceLists[CompleteNumber].Mode;
                                                    Apattribute.Carrier = SubmitedUnknownDeviceLists[CompleteNumber].Carrier;
                                                    Apattribute.SN = SubmitedUnknownDeviceLists[CompleteNumber].SN;
                                                    Apattribute.IpAddr = SubmitedUnknownDeviceLists[CompleteNumber].IpAddr;
                                                    Apattribute.Port = SubmitedUnknownDeviceLists[CompleteNumber].Port;
                                                    Apattribute.NetMask = SubmitedUnknownDeviceLists[CompleteNumber].Netmask;
                                                    Apattribute.OnLine = SubmitedUnknownDeviceLists[CompleteNumber].Online;
                                                    Apattribute.IsActive = SubmitedUnknownDeviceLists[CompleteNumber].IsActive;
                                                    Apattribute.LastOnline = SubmitedUnknownDeviceLists[CompleteNumber].LastOnline;
                                                    //数据对齐状态
                                                    Apattribute.AlertIcon = new NodeIcon().AlignAlertIcon;
                                                    Apattribute.AlertText = "该设备数据未对齐";
                                                    Apattribute.ALIGN = "-1";
                                                    //白名单自学习状态
                                                    Apattribute.Command = "-1";
                                                    JsonInterFace.APATTributesLists.Add(Apattribute);

                                                    //该设备参数更新至服务器
                                                    Dictionary<string, string> UpdateParamList = new Dictionary<string, string>();
                                                    UpdateParamList.Add("name", Apattribute.SelfName);
                                                    UpdateParamList.Add("mode", Apattribute.Mode);
                                                    UpdateParamList.Add("sn", Apattribute.SN);
                                                    UpdateParamList.Add("carrier", Apattribute.Carrier);
                                                    UpdateParamList.Add("ipAddr", Apattribute.IpAddr);
                                                    UpdateParamList.Add("port", Apattribute.Port);
                                                    UpdateParamList.Add("netmask", Apattribute.NetMask);
                                                    UpdateDeviceParametersToServer(SubmitedUnknownDeviceLists[CompleteNumber].ToStation, SubmitedUnknownDeviceLists[CompleteNumber].DeviceName, UpdateParamList);
                                                }
                                                //覆盖(更新)
                                                else
                                                {
                                                    //将成功添加的未知设备更新到设备树列表
                                                    for (int m = 0; m < JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows.Count; m++)
                                                    {
                                                        if ((SubmitedUnknownDeviceLists[CompleteNumber].ToStation + "." + SubmitedUnknownDeviceLists[CompleteNumber].DeviceName)
                                                            == JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[m]["PathName"].ToString())
                                                        {
                                                            DataRow rw = JsonInterFace.BindTreeViewClass.DeviceTreeTable.NewRow();
                                                            JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[m]["PathName"] = SubmitedUnknownDeviceLists[CompleteNumber].ToStation + "." + SubmitedUnknownDeviceLists[CompleteNumber].DeviceName;
                                                            JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[m]["SelfID"] = Convert.ToInt32(JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[m]["SelfID"].ToString());
                                                            JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[m]["SelfName"] = SubmitedUnknownDeviceLists[CompleteNumber].DeviceName;
                                                            JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[m]["ParentID"] = SubmitedUnknownDeviceLists[CompleteNumber].StationID;
                                                            JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[m]["IsStation"] = "2";
                                                            JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[m]["NodeContent"] = SubmitedUnknownDeviceLists[CompleteNumber].DeviceName;
                                                            JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[m]["IsDeleted"] = false;
                                                            JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[m]["Permission"] = "Enable";
                                                            JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[m]["NodeType"] = NodeType.LeafNode.ToString();
                                                            JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[m]["CarrierStatus"] = SubmitedUnknownDeviceLists[CompleteNumber].Carrier;

                                                            if (SubmitedUnknownDeviceLists[CompleteNumber].Online == "1")
                                                            {
                                                                if (SubmitedUnknownDeviceLists[CompleteNumber].IsActive == "1")
                                                                {
                                                                    //单载波类型设备
                                                                    if (new Regex(DeviceType.LTE).Match(SubmitedUnknownDeviceLists[CompleteNumber].Mode).Success || SubmitedUnknownDeviceLists[CompleteNumber].Mode == DeviceType.WCDMA || new Regex("TD").Match(SubmitedUnknownDeviceLists[CompleteNumber].Mode).Success)
                                                                    {
                                                                        JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[m]["NodeIcon"] = new NodeIcon().LeafAllReadyNodeIcon;
                                                                    }
                                                                    //双载波类型设备
                                                                    else
                                                                    {
                                                                        //载波1
                                                                        if (SubmitedUnknownDeviceLists[CompleteNumber].Carrier == "0")
                                                                        {
                                                                            JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[m]["NodeIcon"] = new NodeIcon().Carrier_One_ActiveIcon;
                                                                        }
                                                                        //载波2
                                                                        else if (SubmitedUnknownDeviceLists[CompleteNumber].Carrier == "1")
                                                                        {
                                                                            JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[m]["NodeIcon"] = new NodeIcon().Carrier_Two_ActiveIcon;
                                                                        }
                                                                        //全载波
                                                                        else
                                                                        {
                                                                            JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[m]["NodeIcon"] = new NodeIcon().LeafAllReadyNodeIcon;
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[m]["NodeIcon"] = new NodeIcon().LeafNoActiveNodeIcon;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[m]["NodeIcon"] = new NodeIcon().LeafNoConnectNodeIcon;
                                                            }

                                                            break;
                                                        }
                                                    }

                                                    //更新到属性
                                                    APATTributes Apattribute = new APATTributes();
                                                    Apattribute.SelfName = SubmitedUnknownDeviceLists[CompleteNumber].DeviceName;
                                                    Apattribute.FullName = SubmitedUnknownDeviceLists[CompleteNumber].ToStation + "." + SubmitedUnknownDeviceLists[CompleteNumber].DeviceName;
                                                    Apattribute.Mode = SubmitedUnknownDeviceLists[CompleteNumber].Mode;
                                                    Apattribute.Carrier = SubmitedUnknownDeviceLists[CompleteNumber].Carrier;
                                                    Apattribute.SN = SubmitedUnknownDeviceLists[CompleteNumber].SN;
                                                    Apattribute.IpAddr = SubmitedUnknownDeviceLists[CompleteNumber].IpAddr;
                                                    Apattribute.Port = SubmitedUnknownDeviceLists[CompleteNumber].Port;
                                                    Apattribute.NetMask = SubmitedUnknownDeviceLists[CompleteNumber].Netmask;
                                                    Apattribute.OnLine = SubmitedUnknownDeviceLists[CompleteNumber].Online;
                                                    Apattribute.IsActive = SubmitedUnknownDeviceLists[CompleteNumber].IsActive;
                                                    Apattribute.LastOnline = SubmitedUnknownDeviceLists[CompleteNumber].LastOnline;
                                                    //数据对齐状态
                                                    Apattribute.AlertIcon = new NodeIcon().AlignAlertIcon;
                                                    Apattribute.AlertText = "该设备数据未对齐";
                                                    Apattribute.ALIGN = "-1";
                                                    //白名单自学习状态
                                                    Apattribute.Command = "-1";
                                                    for (int m = 0; m < JsonInterFace.APATTributesLists.Count; m++)
                                                    {
                                                        if (JsonInterFace.APATTributesLists[m].FullName == Apattribute.FullName)
                                                        {
                                                            JsonInterFace.APATTributesLists[m].SelfName = Apattribute.SelfName;
                                                            JsonInterFace.APATTributesLists[m].FullName = Apattribute.FullName;
                                                            JsonInterFace.APATTributesLists[m].Mode = Apattribute.Mode;
                                                            JsonInterFace.APATTributesLists[m].Carrier = Apattribute.Carrier;
                                                            JsonInterFace.APATTributesLists[m].SN = Apattribute.SN;
                                                            JsonInterFace.APATTributesLists[m].IpAddr = Apattribute.IpAddr;
                                                            JsonInterFace.APATTributesLists[m].Port = Apattribute.Port;
                                                            JsonInterFace.APATTributesLists[m].NetMask = Apattribute.NetMask;
                                                            JsonInterFace.APATTributesLists[m].OnLine = Apattribute.OnLine;
                                                            JsonInterFace.APATTributesLists[m].IsActive = Apattribute.IsActive;
                                                            JsonInterFace.APATTributesLists[m].LastOnline = Apattribute.LastOnline;
                                                            //数据对齐状态
                                                            JsonInterFace.APATTributesLists[m].AlertIcon = Apattribute.AlertIcon;
                                                            JsonInterFace.APATTributesLists[m].AlertText = Apattribute.AlertText;
                                                            JsonInterFace.APATTributesLists[m].ALIGN = Apattribute.ALIGN;
                                                            //白名单自学习状态
                                                            JsonInterFace.APATTributesLists[m].Command = Apattribute.Command;
                                                            break;
                                                        }
                                                    }

                                                    //将该设备信息参数更新至服务器
                                                    Dictionary<string, string> UpdateParamList = new Dictionary<string, string>();
                                                    UpdateParamList.Add("name", Apattribute.SelfName);
                                                    UpdateParamList.Add("mode", Apattribute.Mode);
                                                    UpdateParamList.Add("sn", Apattribute.SN);
                                                    UpdateParamList.Add("carrier", Apattribute.Carrier);
                                                    UpdateParamList.Add("ipAddr", Apattribute.IpAddr);
                                                    UpdateParamList.Add("port", Apattribute.Port);
                                                    UpdateParamList.Add("netmask", Apattribute.NetMask);
                                                    UpdateDeviceParametersToServer(SubmitedUnknownDeviceLists[CompleteNumber].ToStation, Apattribute.SelfName, UpdateParamList);
                                                }
                                            }
                                            #endregion

                                            #region 2. 删除对应成功提交项
                                            for (int j = 0; j < ShowUnKnownDeviceLists.Count; j++)
                                            {
                                                if (SN == ShowUnKnownDeviceLists[j].SN)
                                                {
                                                    if (ShowUnKnownDeviceLists.Count > 0)
                                                    {
                                                        try
                                                        {
                                                            ShowUnKnownDeviceLists.RemoveAt(j);
                                                        }
                                                        catch
                                                        {

                                                        }
                                                    }
                                                    break;
                                                }
                                            }
                                            Dispatcher.Invoke(() =>
                                            {
                                                UnKnownDeviceListsGrid.Items.Refresh();
                                            });
                                            #endregion

                                            //重置参数
                                            CompleteNumber = -1;
                                            ReturnStr = string.Empty;
                                            break;
                                        }
                                        else
                                        {
                                            //重置参数
                                            CompleteNumber = -1;
                                            ReturnStr = string.Empty;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        Thread.Sleep(10);
                                    }
                                }
                            }
                            else
                            {
                                ErrMsg.AppendLine("未知设备[" + DeviceName + "],制式[" + DeviceMode + "]没有指定站点！！");
                            }
                        }

                        //未知设备屏蔽关闭
                        Parameters.STRefresh = true;

                        //未知设备提交完成记数重置
                        Parameters.CompleteCount = 0;

                        //清空已选择未知设备列表
                        SubmitedUnknownDeviceLists.Clear();

                        //进度复位
                        ProgressBarStatus.MaxValue = 100;
                        ProgressBarStatus.StepValue = 0;
                        ProgressBarStatus.Enabled = Visibility.Collapsed;

                        #region 刷新设备树显示
                        Parameters.SendMessage(Parameters.WinHandle, Parameters.WM_DeviceListInfoLoad, 0, 0);
                        #endregion

                        if (ErrMsg.Length > 0)
                        {
                            MessageBox.Show(ErrMsg.ToString(), "提交未知设备提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }).Start();
                }
                else
                {
                    //清空已选择未知设备列表
                    SubmitedUnknownDeviceLists.Clear();
                    MessageBox.Show("提交未知设备失败,网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception Ex)
            {
                //未知设备屏蔽关闭
                Parameters.STRefresh = true;
                Parameters.PrintfLogsExtended("提交未知设备失败", Ex.Message, Ex.StackTrace);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UnKnownDeviceListsGrid.ItemsSource = ShowUnKnownDeviceLists;
            ProgressBarBox.DataContext = ProgressBarStatus;
            lblUnknownDeviceCount.DataContext = JsonInterFace.UnKnownDeviceListsParameter;
            //句柄
            WindowInteropHelper SelfHandle = new WindowInteropHelper(this);
            Parameters.UnknownDeviceWinHandle = SelfHandle.Handle;
            ShowUnknownDeviceTatolTimer.Start();
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        private ChildType FindVisualChild<ChildType>(DependencyObject obj) where ChildType : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is ChildType)
                {
                    return (child as ChildType);
                }
                else
                {
                    ChildType childOfChild = FindVisualChild<ChildType>(child);
                    if (childOfChild != null)
                    {
                        return (childOfChild);
                    }
                }
            }
            return (null);
        }

        private void btnSelectAPStation_Click(object sender, RoutedEventArgs e)
        {
            if (UnKnownDeviceListsGrid.SelectedItem != null)
            {
                UnknownDeviceStationWindow UnknownDeviceStationWin = new UnknownDeviceStationWindow();
                UnknownDeviceStationWin.Left = this.Left;
                UnknownDeviceStationWin.Top = this.Top;
                UnknownDeviceStationWin.ID = (UnKnownDeviceListsGrid.SelectedItem as UnKnownDeviceListsParameterClass).ID;
                UnknownDeviceStationWin.ToStationFullPath = (UnKnownDeviceListsGrid.SelectedItem as UnKnownDeviceListsParameterClass).ToStation;
                UnknownDeviceStationWin.ShowDialog();
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (btnCancel.IsFocused)
                {
                    this.DragMove();
                }
            }
        }

        /// <summary>
        /// ESC退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnCancel.Focus();
        }

        /// <summary>
        /// 操作复选框
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="isChecked"></param>
        public void GetVisualChild(DependencyObject parent, bool isChecked)
        {
            try
            {
                int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
                for (int i = 0; i < numVisuals; i++)
                {
                    DependencyObject Element = VisualTreeHelper.GetChild(parent, i);
                    CheckBox ChildItemCheckBox = Element as CheckBox;

                    if (ChildItemCheckBox == null)
                    {
                        GetVisualChild(Element, isChecked);
                    }
                    else
                    {
                        ChildItemCheckBox.IsChecked = isChecked;
                        return;
                    }
                }
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended(Ex.Message, Ex.StackTrace);
            }
        }

        private void txtDeviceName_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string ID = string.Empty;
            string SN = string.Empty;
            string IPAddr = string.Empty;
            string Port = string.Empty;
            string Mode = string.Empty;
            bool Flag = true;

            try
            {
                if (UnKnownDeviceListsGrid.SelectedItem != null)
                {
                    UnknownDevieReNameWindow UnknownDevieReNameWin = new UnknownDevieReNameWindow();
                    UnknownDeviceReName.UnknownSourceName = (UnKnownDeviceListsGrid.SelectedItem as UnKnownDeviceListsParameterClass).DeviceName;
                    ID = (UnKnownDeviceListsGrid.SelectedItem as UnKnownDeviceListsParameterClass).ID;
                    SN = (UnKnownDeviceListsGrid.SelectedItem as UnKnownDeviceListsParameterClass).SN;
                    IPAddr = (UnKnownDeviceListsGrid.SelectedItem as UnKnownDeviceListsParameterClass).IpAddr;
                    Port = (UnKnownDeviceListsGrid.SelectedItem as UnKnownDeviceListsParameterClass).Port;
                    Mode = (UnKnownDeviceListsGrid.SelectedItem as UnKnownDeviceListsParameterClass).Mode;
                    this.Topmost = false;

                    if ((bool)UnknownDevieReNameWin.ShowDialog())
                    {
                        for (int i = 0; i < UnKnownDeviceNameStationList.Count; i++)
                        {
                            if (UnKnownDeviceNameStationList[i].ID == ID)
                            {
                                UnKnownDeviceNameStationList[i].DeviceName = UnknownDeviceReName.UnknownNewName;
                                UnKnownDeviceNameStationList[i].DeviceMode = Mode;
                                Flag = false;
                                break;
                            }
                            else
                            {
                                Flag = true;
                            }
                        }

                        if (Flag)
                        {
                            UnKnownDeviceNameStationListClass Item = new UnKnownDeviceNameStationListClass();
                            Item.ID = ID;
                            Item.DeviceName = UnknownDeviceReName.UnknownNewName;
                            Item.DeviceMode = Mode;
                            UnKnownDeviceNameStationList.Add(Item);
                        }

                        //更新显示列表
                        lock (JsonInterFace.UnKnownDeviceListsParameter.LockObject)
                        {
                            StatusIcon UnknownDeviceNameInfo = new StatusIcon();
                            Dispatcher.Invoke(() =>
                            {
                                for (int i = 0; i < ShowUnKnownDeviceLists.Count; i++)
                                {
                                    if (ShowUnKnownDeviceLists[i].ID == ID)
                                    {
                                        ShowUnKnownDeviceLists[i].DeviceName = UnknownDeviceReName.UnknownNewName;
                                        ShowUnKnownDeviceLists[i].NodeIcon = UnknownDeviceNameInfo.RenameOk;
                                        ShowUnKnownDeviceLists[i].NodeIconTips = UnknownDeviceNameInfo.RenameOkTips;
                                        break;
                                    }
                                }
                            });
                        }

                        //同时更新缓存列表
                        for (int i = 0; i < JsonInterFace.UnKnownDeviceListsParameter.UnKnownDeviceTab.Rows.Count; i++)
                        {
                            if (JsonInterFace.UnKnownDeviceListsParameter.UnKnownDeviceTab.Rows[i][2].ToString() == SN
                                && JsonInterFace.UnKnownDeviceListsParameter.UnKnownDeviceTab.Rows[i][4].ToString() == IPAddr
                                && JsonInterFace.UnKnownDeviceListsParameter.UnKnownDeviceTab.Rows[i][5].ToString() == Port)
                            {
                                JsonInterFace.UnKnownDeviceListsParameter.UnKnownDeviceTab.Rows[i]["DeviceName"] = UnknownDeviceReName.UnknownNewName;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }

            this.Topmost = true;
        }

        /// <summary>
        /// 单选一个未知设备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnKnownDeviceListsGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                if (UnKnownDeviceListsGrid.CurrentColumn != null)
                {
                    if (UnKnownDeviceListsGrid.CurrentColumn.SortMemberPath == "IsSelected")
                    {
                        int AllCount = ShowUnKnownDeviceLists.Count;
                        int SelectedCount = 0;
                        string SelectedID = (UnKnownDeviceListsGrid.CurrentItem as UnKnownDeviceListsParameterClass).ID;
                        for (int i = 0; i < ShowUnKnownDeviceLists.Count; i++)
                        {
                            if (ShowUnKnownDeviceLists[i].ID == SelectedID)
                            {
                                if (ShowUnKnownDeviceLists[i].IsSelected)
                                {
                                    ShowUnKnownDeviceLists[i].IsSelected = false;
                                }
                                else
                                {
                                    ShowUnKnownDeviceLists[i].IsSelected = true;
                                }
                                break;
                            }
                        }

                        //已选条数
                        for (int i = 0; i < ShowUnKnownDeviceLists.Count; i++)
                        {
                            if (ShowUnKnownDeviceLists[i].IsSelected)
                            {
                                SelectedCount++;
                            }
                        }

                        //是否改变全选状态
                        CheckBox SelectedAllCtrl = Parameters.GetVisualChild<CheckBox>(UnKnownDeviceListsGrid, Item => Item.Name == "UnknownDeviceIsSelectedAll");
                        if (SelectedCount == AllCount)
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
                }
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("选择一个未知设备内部异常", Ex.Message, Ex.StackTrace);
            }
        }

        /// <summary>
        /// 全选未知设备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnknownDeviceIsSelectedAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                for (int i = 0; i < ShowUnKnownDeviceLists.Count; i++)
                {
                    ShowUnKnownDeviceLists[i].IsSelected = (bool)(sender as CheckBox).IsChecked;
                }
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("全选未知设备", Ex.Message, Ex.StackTrace);
            }
        }

        private void ControlUnknownDeviceCount()
        {
            Dispatcher.Invoke(() =>
            {
                try
                {
                    JsonInterFace.UnKnownDeviceListsParameter.Count = UnKnownDeviceListsGrid.Items.Count;
                    if (JsonInterFace.UnKnownDeviceListsParameter.Count <= 0)
                    {
                        btnEnter.IsEnabled = false;
                    }
                    else
                    {
                        btnEnter.IsEnabled = true;
                    }

                    //列表空全选状态复位
                    CheckBox SelectedAllCtrl = Parameters.GetVisualChild<CheckBox>(UnKnownDeviceListsGrid, Item => Item.Name == "UnknownDeviceIsSelectedAll");
                    if (ShowUnKnownDeviceLists.Count <= 0)
                    {
                        if (null != SelectedAllCtrl)
                        {
                            SelectedAllCtrl.IsChecked = false;
                        }
                    }
                }
                catch (Exception Ex)
                {
                    Parameters.PrintfLogsExtended(Ex.Message, Ex.StackTrace);
                }
            });
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            ShowUnknownDeviceTatolTimer.Stop();
        }

        private void mmUnknownDeviceDataReLoad_Click(object sender, RoutedEventArgs e)
        {
            bool STRefreshBack = false;

            if (MessageBox.Show("确定重载未知设备信息？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                return;
            }

            try
            {
                if (NetWorkClient.ControllerServer.Connected)
                {
                    if (Parameters.STRefresh)
                    {
                        STRefreshBack = Parameters.STRefresh;
                        Parameters.STRefresh = false;
                        Thread.Sleep(2000);
                    }

                    lock (JsonInterFace.UnKnownDeviceListsParameter.LockObject)
                    {
                        JsonInterFace.UnKnownDeviceListsParameter.UnKnownDeviceTab.Rows.Clear();
                    }

                    lock (ShowUnKnownDeviceListsLocked)
                    {
                        ShowUnKnownDeviceLists.Clear();
                    }

                    if (STRefreshBack)
                    {
                        Parameters.STRefresh = STRefreshBack;
                    }

                    NavigatePages.DeviceListWindow.SelectedDeviceParameters();
                    NavigatePages.DeviceListWindow.ReloadDeviceList(false);
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("刷新未知设备列表", Ex.Message, Ex.StackTrace);
            }
        }

        private void mmUnknownDeviceAllSameStation_Click(object sender, RoutedEventArgs e)
        {
            string aStation = string.Empty;
            string aID = string.Empty;
            string ID = string.Empty;
            string ToStation = string.Empty;
            string ActionType = "AllSameStation";

            try
            {
                if (ShowUnKnownDeviceLists.Count <= 0)
                {
                    MessageBox.Show("没有任何未知设备！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                //如果已选则应用此已选站点作为站点列表默认已选项
                for (int i = 0; i < ShowUnKnownDeviceLists.Count; i++)
                {
                    if (ShowUnKnownDeviceLists[i].ToStation != null && ShowUnKnownDeviceLists[i].ToStation != "" && ShowUnKnownDeviceLists[i].ToStation != Parameters.ToStationDefault)
                    {
                        ID = ShowUnKnownDeviceLists[i].ID;
                        ToStation = ShowUnKnownDeviceLists[i].ToStation;
                        break;
                    }
                }

                //弹窗选择站点
                UnknownDeviceStationWindow UnknownDeviceStationWin = new UnknownDeviceStationWindow();
                UnknownDeviceStationWin.Left = this.Left;
                UnknownDeviceStationWin.Top = this.Top;
                UnknownDeviceStationWin.Tag = ActionType;
                UnknownDeviceStationWin.ID = ID;
                UnknownDeviceStationWin.ToStationFullPath = ToStation;

                if ((bool)UnknownDeviceStationWin.ShowDialog())
                {
                    if (UnknownDeviceStationWin.Tag != null)
                    {
                        if (UnknownDeviceStationWin.Tag.ToString() != ActionType)
                        {
                            aID = UnknownDeviceStationWin.Tag.ToString().Split(new char[] { '|' })[0]; ;
                            aStation = UnknownDeviceStationWin.Tag.ToString().Split(new char[] { '|' })[1];
                        }
                    }
                    else
                    {
                        aID = ID;
                        aStation = ToStation;
                    }
                }

                //设备全部使用相同站点
                if (aStation != null && aStation != "")
                {
                    for (int i = 0; i < ShowUnKnownDeviceLists.Count; i++)
                    {
                        ShowUnKnownDeviceLists[i].StationID = aID;
                        ShowUnKnownDeviceLists[i].ToStation = aStation;
                        ShowUnKnownDeviceLists[i].StationStatuIcon = new StatusIcon().OK;
                    }
                    JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "全部使用相同站点设置时，站点已设定,所有[未知设备]指定到站点-->[" + aStation + "]", "站点设置", "成功");
                }
                else
                {
                    JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "全部使用相同站点设置时，站点未指定,所有[未知设备]未指定到任何站点...", "站点设置", "未选择任何站点");
                }
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("全部使用相同站点内部异常", Ex.Message, Ex.StackTrace);
            }
        }

        private void mmUnknownDeviceAllSelected_Click(object sender, RoutedEventArgs e)
        {
            int SelectedCount = 0;
            try
            {
                if (ShowUnKnownDeviceLists.Count <= 0)
                {
                    MessageBox.Show("没有任何未知设备！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                //全选
                for (int i = 0; i < ShowUnKnownDeviceLists.Count; i++)
                {
                    ShowUnKnownDeviceLists[i].IsSelected = true;
                    SelectedCount++;
                }

                //是否改变全选状态
                CheckBox SelectedAllCtrl = Parameters.GetVisualChild<CheckBox>(UnKnownDeviceListsGrid, Item => Item.Name == "UnknownDeviceIsSelectedAll");
                if (SelectedCount >= ShowUnKnownDeviceLists.Count)
                {

                    if (null != SelectedAllCtrl)
                    {
                        SelectedAllCtrl.IsChecked = true;
                    }
                }
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("全选未知设备", Ex.Message, Ex.StackTrace);
            }
        }

        private void mmUnknownDeviceAllUnSelected_Click(object sender, RoutedEventArgs e)
        {
            int SelectedCount = 0;
            try
            {
                if (ShowUnKnownDeviceLists.Count <= 0)
                {
                    MessageBox.Show("没有任何未知设备！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                //全不选
                for (int i = 0; i < ShowUnKnownDeviceLists.Count; i++)
                {
                    ShowUnKnownDeviceLists[i].IsSelected = false;
                    SelectedCount++;
                }

                //是否改变全选状态
                CheckBox SelectedAllCtrl = Parameters.GetVisualChild<CheckBox>(UnKnownDeviceListsGrid, Item => Item.Name == "UnknownDeviceIsSelectedAll");
                if (SelectedCount >= ShowUnKnownDeviceLists.Count)
                {

                    if (null != SelectedAllCtrl)
                    {
                        SelectedAllCtrl.IsChecked = false;
                    }
                }
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("全不选未知设备", Ex.Message, Ex.StackTrace);
            }
        }

        private void mmUnknownDeviceAllSameStationReset_Click(object sender, RoutedEventArgs e)
        {
            string aStation = string.Empty;
            try
            {
                if (ShowUnKnownDeviceLists.Count <= 0)
                {
                    MessageBox.Show("没有任何未知设备！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                //全部站点重置
                for (int i = 0; i < ShowUnKnownDeviceLists.Count; i++)
                {
                    ShowUnKnownDeviceLists[i].ToStation = string.Empty;
                    ShowUnKnownDeviceLists[i].StationStatuIcon = new StatusIcon().None;
                }

                JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "所有[未知设备]的指派站点已重置...", "未知设备站点重置", "成功");
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("全部站点重置内部异常", Ex.Message, Ex.StackTrace);
            }
        }

        private void txtDeviceName_MouseEnter(object sender, MouseEventArgs e)
        {
            //自动选择
            try
            {
                if ((sender != null))
                {
                    string Selected_SN = (((sender as TextBlock).Parent as Grid).DataContext as UnKnownDeviceListsParameterClass).SN;
                    e.Handled = true;
                    for (int i = 0; i < UnKnownDeviceListsGrid.Items.Count; i++)
                    {
                        if (Selected_SN == ((UnKnownDeviceListsGrid.Items[i] as UnKnownDeviceListsParameterClass).SN))
                        {
                            UnKnownDeviceListsGrid.Focus();
                            UnKnownDeviceListsGrid.SelectedIndex = i;
                            UnKnownDeviceListsGrid.SelectedItem = UnKnownDeviceListsGrid.Items[i];
                            break;
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended(Ex.Message, Ex.StackTrace);
            }
        }

        private void UnKnownDeviceListsGrid_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (UnKnownDeviceListsGrid.CurrentCell != null)
                {
                    if (UnKnownDeviceListsGrid.CurrentCell.Column.SortMemberPath == "IsSelected")
                    {
                        string SN = (UnKnownDeviceListsGrid.CurrentCell.Item as UnKnownDeviceListsParameterClass).SN;
                        for (int i = 0; i < ShowUnKnownDeviceLists.Count; i++)
                        {
                            if (ShowUnKnownDeviceLists[i].SN == SN)
                            {
                                if (ShowUnKnownDeviceLists[i].IsSelected)
                                {
                                    ShowUnKnownDeviceLists[i].IsSelected = false;
                                }
                                else
                                {
                                    ShowUnKnownDeviceLists[i].IsSelected = true;
                                }
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended(Ex.Message, Ex.StackTrace);
            }
        }

        private void UnKnownDeviceListsGrid_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    SubmitedUnknownDeviceLists.Clear();

                    for (int i = 0; i < ShowUnKnownDeviceLists.Count; i++)
                    {
                        if (ShowUnKnownDeviceLists[i].IsSelected)
                        {
                            SubmitedUnknownDeviceLists.Add(ShowUnKnownDeviceLists[i]);
                        }
                    }

                    if (SubmitedUnknownDeviceLists.Count > 0)
                    {
                        DragDrop.DoDragDrop(UnKnownDeviceListsGrid, SubmitedUnknownDeviceLists, DragDropEffects.Copy);
                    }

                }
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("拖动未知设备内部异常", Ex.Message, Ex.StackTrace);
            }
        }
    }
}
