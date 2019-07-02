using DataInterface;
using JsonLib;
using NetController;
using Newtonsoft.Json;
using ParameterControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;

namespace iccms
{
    /// <summary>
    /// 设备树列表结点连线
    /// </summary>
    public class TreeViewLineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            System.Windows.Controls.TreeViewItem item = (System.Windows.Controls.TreeViewItem)value;
            System.Windows.Controls.ItemsControl ic = System.Windows.Controls.ItemsControl.ItemsControlFromItemContainer(item);
            return ic.ItemContainerGenerator.IndexFromContainer(item) == ic.Items.Count - 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return false;
        }
    }

    /// <summary>
    /// 语音播放类
    /// </summary>
    public class EventSound : INotifyPropertyChanged
    {
        private double _soundVolume = 1;

        private bool _playerComplete;

        private MediaPlayer WarningSoundPlayer = null;

        public SpeechSynthesizer Speecher = null; //语音合成对象

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string value)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(value));
            }
        }

        public EventSound()
        {
            if (Speecher == null)
            {
                Speecher = new SpeechSynthesizer();
                Speecher.SpeakCompleted += Speecher_SpeakCompleted;
            }

            if (WarningSoundPlayer == null)
            {
                WarningSoundPlayer = new MediaPlayer();
                PlayerComplete = false;
                WarningSoundPlayer.MediaEnded += WarningSoundPlayer_MediaEnded;
                WarningSoundPlayer.MediaFailed += WarningSoundPlayer_MediaFailed;
                WarningSoundPlayer.MediaOpened += WarningSoundPlayer_MediaOpened;
                WarningSoundPlayer.Changed += WarningSoundPlayer_Changed;
            }
        }

        private void Speecher_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            PlayerComplete = true;
        }

        private void WarningSoundPlayer_Changed(object sender, EventArgs e)
        {
            Parameters.PrintfLogsExtended("媒体文件播放:" + e.ToString());
        }

        private void WarningSoundPlayer_MediaOpened(object sender, EventArgs e)
        {
            Parameters.PrintfLogsExtended("媒体文件开始播放");
        }

        private void WarningSoundPlayer_MediaFailed(object sender, ExceptionEventArgs e)
        {
            Parameters.PrintfLogsExtended("媒体文件播放失败" + e.ErrorException.Message);
        }

        private void WarningSoundPlayer_MediaEnded(object sender, EventArgs e)
        {
            PlayerComplete = true;
            Parameters.PrintfLogsExtended("媒体文件播放结束");
        }

        public EventSound(double SdVolume, int SpVolume, int SpRate)
        {
            if (Speecher == null)
            {
                Volume = SpVolume;
                Rate = SpRate;
                Speecher = new SpeechSynthesizer();
                Speecher.SpeakCompleted += Speecher_SpeakCompleted;
            }

            if (WarningSoundPlayer == null)
            {
                SoundVolume = SdVolume;
                WarningSoundPlayer = new MediaPlayer();
                WarningSoundPlayer.MediaEnded += WarningSoundPlayer_MediaEnded;
                WarningSoundPlayer.MediaFailed += WarningSoundPlayer_MediaFailed;
                WarningSoundPlayer.MediaOpened += WarningSoundPlayer_MediaOpened;
                WarningSoundPlayer.Changed += WarningSoundPlayer_Changed;
            }
        }

        public EventSound(int Volume, int Rate)
        {
            //使用 Speecher 设置朗读音量 [范围 0 ~ 100]
            Speecher.Volume = Volume;
            //使用 Speecher 设置朗读频率 [范围 -10 ~ 10]
            Speecher.Rate = Rate;
        }

        public void SpeakChina(string WordS)
        {
            try
            {
                Speecher.SelectVoice("Microsoft Lili");
                Speecher.Speak(WordS);
            }
            catch
            {
                Speecher.Speak(WordS);
            }
        }

        public void SpeakCancelAll()
        {
            Speecher.SpeakAsyncCancelAll();
        }

        public void SpeakPause()
        {
            Speecher.Pause();
        }

        public void SpeakResume()
        {
            Speecher.Resume();
        }

        public void SpeakChinaAsync(string WordS)
        {
            try
            {
                Speecher.SelectVoice("Microsoft Lili");
                Speecher.SpeakAsync(WordS);
            }
            catch
            {
                Speecher.SpeakAsync(WordS);
            }
        }

        public void SpeakEnglish(string WordS)
        {
            try
            {
                Speecher.SelectVoice("VW Julie");
                Speecher.Speak(WordS);
            }
            catch
            {
                Speecher.Speak(WordS);
            }
        }

        public void SpeakEnglishAsync(string WordS)
        {
            try
            {
                Speecher.SelectVoice("VW Julie");
                Speecher.SpeakAsync(WordS);
            }
            catch
            {
                Speecher.SpeakAsync(WordS);
            }
        }

        public void MediaSoundPlayer(string SoundFile, UriKind Type)
        {
            try
            {
                WarningSoundPlayer.Open(new Uri(@SoundFile, Type));
                WarningSoundPlayer.ScrubbingEnabled = true;
                WarningSoundPlayer.Play();
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("媒体播放", ex.Message, ex.StackTrace);
            }
        }

        private void SoundPlayer_MediaEnded(object sender, EventArgs e)
        {
            PlayerComplete = true;
        }


        public void MediaSoundStop()
        {
            WarningSoundPlayer.Stop();
        }

        public void MediaSoundStart()
        {
            WarningSoundPlayer.Play();
        }

        public void MediaSoundPause()
        {
            WarningSoundPlayer.Pause();
        }

        public int Volume
        {
            get
            {
                return Speecher.Volume;
            }
            set
            {
                Speecher.Volume = value;
            }
        }

        public int Rate
        {
            get
            {
                return Speecher.Rate;
            }
            set
            {
                Speecher.Rate = value;
            }
        }

        public bool MediaSoundComplete
        {
            get { return PlayerComplete; }
        }

        public double SoundVolume
        {
            get
            {
                _soundVolume = WarningSoundPlayer.Volume;
                return _soundVolume;
            }

            set
            {
                _soundVolume = value;
                WarningSoundPlayer.Volume = _soundVolume;
            }
        }

        public bool PlayerComplete
        {
            get
            {
                return _playerComplete;
            }

            set
            {
                _playerComplete = value;
            }
        }
    }

    #region 系统日志缓存类
    public class SysLogsCaching
    {
        private string _dTime;
        private string _object;
        private string _action;
        private string _other;

        public string DTime
        {
            get
            {
                return _dTime;
            }

            set
            {
                _dTime = value;
            }
        }

        public string Object
        {
            get
            {
                return _object;
            }

            set
            {
                _object = value;
            }
        }

        public string Action
        {
            get
            {
                return _action;
            }

            set
            {
                _action = value;
            }
        }

        public string Other
        {
            get
            {
                return _other;
            }

            set
            {
                _other = value;
            }
        }
    }
    #endregion

    #region 系统关闭提示
    public class WindowCloseParameters : INotifyPropertyChanged
    {
        private int _waitMax = 100;
        private int _finished = 0;

        public int WaitMax
        {
            get
            {
                return _waitMax;
            }

            set
            {
                _waitMax = value;
                NotifyPropertyChanged("WaitMax");
            }
        }

        public int Finished
        {
            get
            {
                return _finished;
            }

            set
            {
                _finished = value;
                NotifyPropertyChanged("Finished");
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
    #endregion

    #region 选择列表设备信息
    public class DeviceSelected : INotifyPropertyChanged
    {
        private string _SelfID;
        private string _ParentID;
        private string _SelfName;
        private string _Model;
        private bool _IsOnline;
        private string _ApVersion;
        private string _LongFullNamePath;
        private string _ShortFullNamePath;
        private string _SN;
        private string _Des;
        private string _UpdateTactics;
        private string _CellType;
        private string _IsStation;
        private string _SelfIP;
        private string _SelfNetMask;
        private int _SelfPort;
        private string _InnerType;
        private string _SelfNodeType;
        private CheckBoxTreeModel _SelfParent;

        public string SelfID
        {
            get
            {
                return _SelfID;
            }

            set
            {
                _SelfID = value;
                NotifyPropertyChanged("SelfID");
            }
        }

        public string ParentID
        {
            get
            {
                return _ParentID;
            }

            set
            {
                _ParentID = value;
                NotifyPropertyChanged("ParentID");
            }
        }

        public string SelfName
        {
            get
            {
                return _SelfName;
            }

            set
            {
                _SelfName = value;
                NotifyPropertyChanged("SelfName");
            }
        }

        public string Model
        {
            get
            {
                return _Model;
            }

            set
            {
                _Model = value;
                NotifyPropertyChanged("Mode");
            }
        }

        public bool IsOnline
        {
            get
            {
                return _IsOnline;
            }

            set
            {
                _IsOnline = value;
                NotifyPropertyChanged("IsOnline");
            }
        }

        public string ApVersion
        {
            get
            {
                return _ApVersion;
            }

            set
            {
                _ApVersion = value;
                NotifyPropertyChanged("ApVersion");
            }
        }

        public string LongFullNamePath
        {
            get
            {
                return _LongFullNamePath;
            }

            set
            {
                _LongFullNamePath = value;
                NotifyPropertyChanged("LongFullNamePath");
            }
        }

        public string SN
        {
            get
            {
                return _SN;
            }

            set
            {
                _SN = value;
                NotifyPropertyChanged("SN");
            }
        }

        public string Des
        {
            get
            {
                return _Des;
            }

            set
            {
                _Des = value;
                NotifyPropertyChanged("Des");
            }
        }

        public string UpdateTactics
        {
            get
            {
                return _UpdateTactics;
            }

            set
            {
                _UpdateTactics = value;
                NotifyPropertyChanged("UpdateTactics");
            }
        }

        public string CellType
        {
            get
            {
                return _CellType;
            }

            set
            {
                _CellType = value;
                NotifyPropertyChanged("CellType");
            }
        }

        public string IsStation
        {
            get
            {
                return _IsStation;
            }

            set
            {
                _IsStation = value;
                NotifyPropertyChanged("IsStation");
            }
        }

        public CheckBoxTreeModel SelfParent
        {
            get
            {
                return _SelfParent;
            }

            set
            {
                _SelfParent = value;
                NotifyPropertyChanged("SelfParent");
            }
        }

        public string SelfIP
        {
            get
            {
                return _SelfIP;
            }

            set
            {
                _SelfIP = value;
                NotifyPropertyChanged("SelfIP");
            }
        }

        public string SelfNetMask
        {
            get
            {
                return _SelfNetMask;
            }

            set
            {
                _SelfNetMask = value;
                NotifyPropertyChanged("SelfNetMask");
            }
        }

        public int SelfPort
        {
            get
            {
                return _SelfPort;
            }

            set
            {
                _SelfPort = value;
                NotifyPropertyChanged("SelfPort");
            }
        }

        public string InnerType
        {
            get
            {
                return _InnerType;
            }

            set
            {
                _InnerType = value;
                NotifyPropertyChanged("InnerType");
            }
        }

        public string SelfNodeType
        {
            get
            {
                return _SelfNodeType;
            }

            set
            {
                _SelfNodeType = value;
                NotifyPropertyChanged("SelfNodeType");
            }
        }

        public string ShortFullNamePath
        {
            get
            {
                return _ShortFullNamePath;
            }

            set
            {
                _ShortFullNamePath = value;
                NotifyPropertyChanged("ShortFullNamePath");
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
    #endregion

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //----------------------------------------------------------------------------------------------------------------
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        public static extern bool SetLayeredWindowAttributes(IntPtr handle, int colorKey, int alpha, int flags);
        [DllImport("user32.dll", EntryPoint = "SetWindowText", CharSet = CharSet.Ansi)]
        public static extern int SetWindowText(IntPtr hwnd, string lpString);
        [DllImport("user32.dll", EntryPoint = "GetWindowText", CharSet = CharSet.Ansi)]
        public static extern int GetWindowText(IntPtr hwnd, StringBuilder lpString, int Count);
        [DllImport("user32.dll", EntryPoint = "FindWindowEx", CharSet = CharSet.Ansi)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        //----------------------------------------------------------------------------------------------------------------
        public static List<SystemLogsInformation> SysLogsCachingList = new List<SystemLogsInformation>();

        #region 信息统计类
        /// <summary>
        /// 信息统计类
        /// </summary>
        private class InfoStatisticParameterClass : INotifyPropertyChanged
        {
            private string _welcome = Parameters.WelcomTips;
            private string _loginUser;
            private string _stationTotal;
            private string _deviceTotal;
            private string _deviceOnlineTotal;
            private string _sysLogsTotal;

            public event PropertyChangedEventHandler PropertyChanged;

            private void NotifyPropertyChanged(string value)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(value));
                }
            }

            public string StationTotal
            {
                get
                {
                    return _stationTotal;
                }

                set
                {
                    _stationTotal = value;
                    NotifyPropertyChanged("StationTotal");
                }
            }

            public string DeviceTotal
            {
                get
                {
                    return _deviceTotal;
                }

                set
                {
                    _deviceTotal = value;
                    NotifyPropertyChanged("DeviceTotal");
                }
            }

            public string DeviceOnlineTotal
            {
                get
                {
                    return _deviceOnlineTotal;
                }

                set
                {
                    _deviceOnlineTotal = value;
                    NotifyPropertyChanged("DeviceOnlineTotal");
                }
            }

            public string SysLogsTotal
            {
                get
                {
                    return _sysLogsTotal;
                }

                set
                {
                    _sysLogsTotal = value;
                    NotifyPropertyChanged("SysLogsTotal");
                }
            }

            public string LoginUser
            {
                get
                {
                    return _loginUser;
                }

                set
                {
                    _loginUser = value;
                    NotifyPropertyChanged("LoginUser");
                }
            }

            public string Welcome
            {
                get
                {
                    return _welcome;
                }

                set
                {
                    _welcome = value;
                    NotifyPropertyChanged("Welcome");
                }
            }
        }
        #endregion

        //设备选择器
        public static bool ItemSelected = false;
        public static DeviceSelected aDeviceSelected = new DeviceSelected();

        /// <summary>
        /// 检测网线是否脱落,禁用类
        /// </summary>
        private class CheckNetWorkInterfaceUpDown
        {
            private string _interfaceName;
            private string _status;
            private string _speed;
            private string _mAC;
            private object _operationLock = null;

            public List<CheckNetWorkInterfaceUpDown> NetWorkInterfaceList = null;

            /// <summary>
            /// 构造
            /// </summary>
            public CheckNetWorkInterfaceUpDown()
            {
                if (NetWorkInterfaceList == null)
                {
                    NetWorkInterfaceList = new List<CheckNetWorkInterfaceUpDown>();
                }

                if (OperationLock == null)
                {
                    OperationLock = new object();
                }
            }

            public string InterfaceName
            {
                get
                {
                    return _interfaceName;
                }

                set
                {
                    _interfaceName = value;
                }
            }

            public string Status
            {
                get
                {
                    return _status;
                }

                set
                {
                    _status = value;
                }
            }

            public string Speed
            {
                get
                {
                    return _speed;
                }

                set
                {
                    _speed = value;
                }
            }

            public string MAC
            {
                get
                {
                    return _mAC;
                }

                set
                {
                    _mAC = value;
                }
            }

            public object OperationLock
            {
                get
                {
                    return _operationLock;
                }

                set
                {
                    _operationLock = value;
                }
            }

            //检测网络接口是否禁用，掉线
            public bool GettingNetWorkInterfaceOnline(string MAC)
            {
                lock (OperationLock)
                {
                    if (MAC == "00-00-00-00-00-00") { return true; }

                    for (int i = 0; i < NetWorkInterfaceList.Count; i++)
                    {
                        if (NetWorkInterfaceList[i].MAC == MAC)
                        {
                            if (NetWorkInterfaceList[i].Status == "Up")
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
        }

        //窗体容器
        private Dictionary<string, Uri> AllWindow = new Dictionary<string, Uri>();
        private object LanguageClass = null;
        private static bool _unknownDeviceWindowStatus = true;

        //信息统计
        private Thread InfoStatisticThread = null;
        private static InfoStatisticParameterClass InfoStatisticParameter = new InfoStatisticParameterClass();

        public static System.Timers.Timer GetDeviceListTimer = null;

        private static BindTreeView bindTreeView = new BindTreeView();

        //获取未知设备
        private static Thread GettingUnknownDeviceListThread = null;

        //心跳
        private static Thread AppHeartThread = null;

        //网络重连
        private static Thread RemoteReconnectThead = null;

        //未知设备
        private static Thread SetUnknownTipsWindowThread = null;

        //检测网线是否脱落, 禁用
        CheckNetWorkInterfaceUpDown CheckNetWorkInterface = new CheckNetWorkInterfaceUpDown();
        private static Thread CheckNetWorkInterfaceThead = null;

        //媒体播放器
        private static Thread ScannerWarningSoundThead = null;

        //线程监视
        private static Thread DaemonThread = null;

        //远程基本配置参数获取
        public static System.Timers.Timer LoadRemoteParameterTimer = null;

        //UDP服务器
        private static bool Runing = true;
        private static Socket udpServer = null;
        private static bool Started = false;
        public static int SelfPort = Parameters.UDPPort;
        public static string SelfSVIP = Parameters.UDPHost;
        public static object LogsListsLocker = new object();
        public static ObservableCollection<SystemLogsInformation> SelfSystemLog = new ObservableCollection<SystemLogsInformation>();
        private static SysLogsCaching DuplicationLogs = new SysLogsCaching();
        public static List<SysLogsCaching> SysLogsList = new List<SysLogsCaching>();
        private static DataCaching DataStruct = new DataCaching();
        private static IPEndPoint serverIP = new IPEndPoint(IPAddress.Parse(SelfSVIP), SelfPort);
        private static Thread UDPServerHandle = null;

        //关闭参数
        public static WindowCloseParameters ClosingControlPara = new WindowCloseParameters();

        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SetParent")]
        public extern static IntPtr SetParent(IntPtr childPtr, IntPtr parentPtr);

        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "ShowWindowAsync")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        public static BindTreeView BindTreeView
        {
            get
            {
                return bindTreeView;
            }

            set
            {
                bindTreeView = value;
            }
        }

        public static bool UnknownDeviceWindowStatus
        {
            get
            {
                return _unknownDeviceWindowStatus;
            }

            set
            {
                _unknownDeviceWindowStatus = value;
            }
        }

        /// <summary>
        /// 构造
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            AllWindow.Add("Menu", new Uri("NavigatePages/Menu.xaml", UriKind.Relative));
            AllWindow.Add("DeviceListWindow", new Uri("NavigatePages/DeviceListWindow.xaml", UriKind.Relative));
            AllWindow.Add("DeviceLoadingStatusWindow", new Uri("NavigatePages/DeviceLoadingStatusWindow.xaml", UriKind.Relative));
            AllWindow.Add("UEInfoWindow", new Uri("NavigatePages/UEInfoWindow.xaml", UriKind.Relative));
            AllWindow.Add("SystemLogInfo", new Uri("NavigatePages/SystemLogInfo.xaml", UriKind.Relative));

            if (Parameters.LanguageType.Equals("EN"))
            {
                LanguageClass = new Language_EN.SystemConrol();
            }
            else
            {
                LanguageClass = new Language_CN.SystemConrol();
            }
            if (GetDeviceListTimer == null)
            {
                GetDeviceListTimer = new System.Timers.Timer();
                GetDeviceListTimer.Interval = 1000;
                GetDeviceListTimer.AutoReset = false;
                GetDeviceListTimer.Elapsed += GetDeviceListTimer_Elapsed;
            }

            if (UDPServerHandle == null)
            {
                UDPServerHandle = new Thread(new ThreadStart(Server));
                UDPServerHandle.DisableComObjectEagerCleanup();
            }

            if (AppHeartThread == null)
            {
                AppHeartThread = new Thread(new ThreadStart(AppTOServerHeartRequest));
            }

            if (SetUnknownTipsWindowThread == null)
            {
                SetUnknownTipsWindowThread = new Thread(new ThreadStart(SetUnknownTipsWindow));
                SetUnknownTipsWindowThread.Priority = ThreadPriority.Lowest;
            }

            if (InfoStatisticThread == null)
            {
                InfoStatisticThread = new Thread(new ThreadStart(InfoStatisticGetting));
                InfoStatisticThread.Priority = ThreadPriority.Lowest;
            }

            if (CheckNetWorkInterfaceThead == null)
            {
                CheckNetWorkInterfaceThead = new Thread(new ThreadStart(GettingNetWorkInterfaceStatus));
                CheckNetWorkInterfaceThead.Start();
            }

            if (RemoteReconnectThead == null)
            {
                RemoteReconnectThead = new Thread(new ThreadStart(ReconnectToRemounteServer));
                RemoteReconnectThead.DisableComObjectEagerCleanup();
                RemoteReconnectThead.Start();
            }

            if (GettingUnknownDeviceListThread == null)
            {
                GettingUnknownDeviceListThread = new Thread(new ThreadStart(GettingUnknownDeviceList));
                GettingUnknownDeviceListThread.IsBackground = true;
                GettingUnknownDeviceListThread.Priority = ThreadPriority.Normal;
                GettingUnknownDeviceListThread.Start();
            }

            if (ScannerWarningSoundThead == null)
            {
                ScannerWarningSoundThead = new Thread(new ThreadStart(PlayerSound));
            }

            if (LoadRemoteParameterTimer == null)
            {
                LoadRemoteParameterTimer = new System.Timers.Timer();
                LoadRemoteParameterTimer.AutoReset = false;
                LoadRemoteParameterTimer.Interval = 50;
                LoadRemoteParameterTimer.Elapsed += LoadRemoteParameterTimer_Elapsed;
            }

            if (DaemonThread == null)
            {
                DaemonThread = new Thread(new ThreadStart(CheckingThreads));
            }
        }

        private void LoadRemoteParameterTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ServerBaseParametersRequest();
        }

        /// <summary>
        /// 播放指定媒体声音
        /// </summary>
        private void PlayerSound()
        {
            double Volume = 0;
            EventSound MediaSoundPlayer = null;
            try
            {
                Volume = 0.5;
                MediaSoundPlayer = new EventSound();
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("媒体播放初始化", ex.Message, ex.StackTrace);
            }

            while (true)
            {
                try
                {
                    double SoundDelay = 0;
                    int SoundEnable = 0;
                    string SoundFile = Parameters.ScannerDataControlParameter.SoundFile;
                    int ScannerDataCount = JsonInterFace.ScannerData.ScannerDataTable.Rows.Count;

                    //播放延时
                    if (Parameters.ISSoundTime(Parameters.ScannerDataControlParameter.SoundDelay))
                    {
                        SoundDelay = Convert.ToDouble(Parameters.ScannerDataControlParameter.SoundDelay);
                    }

                    //是否开启播放功能
                    if (Parameters.ISDigital(Parameters.ScannerDataControlParameter.SoundEnable))
                    {
                        SoundEnable = Convert.ToInt32(Parameters.ScannerDataControlParameter.SoundEnable);
                    }

                    //文件是否存在
                    if (!System.IO.File.Exists(SoundFile))
                    {
                        SoundFile = string.Empty;
                    }

                    //在指定时间内只播报一次，在指定时间外有新上报才播报
                    if (ScannerDataCount > 0 && SoundEnable == 1 && SoundDelay > 0 && (SoundFile != "" && SoundFile != null))
                    {
                        bool ReportStatus = CheckScannerReportUserType();

                        if (ReportStatus)
                        {
                            //媒体文件播放
                            if (Parameters.ScannerDataControlParameter.PlayerMode)
                            {
                                MediaSoundPlayer.SpeakCancelAll();

                                Volume = Parameters.ScannerDataControlParameter.Volume;

                                MediaSoundPlayer.SoundVolume = Volume;

                                //播放次数
                                int Loop = Parameters.ScannerDataControlParameter.PlayCount;
                                for (int j = 0; j < Loop; j++)
                                {
                                    MediaSoundPlayer.MediaSoundPlayer(Parameters.ScannerDataControlParameter.SoundFile, UriKind.Absolute);

                                    if (Loop > 1)
                                    {
                                        Thread.Sleep(3000);
                                    }
                                }
                                MediaSoundPlayer.PlayerComplete = false;
                            }
                            //语音合成播放
                            else
                            {
                                MediaSoundPlayer.MediaSoundStop();
                                MediaSoundPlayer.Volume = 100;
                                MediaSoundPlayer.Rate = 1;
                                if (Parameters.ScannerDataControlParameter.SpeeckContent != "" && Parameters.ScannerDataControlParameter.SpeeckContent != null)
                                {
                                    //播放次数
                                    int Loop = Parameters.ScannerDataControlParameter.PlayCount;
                                    for (int j = 0; j < Loop; j++)
                                    {
                                        MediaSoundPlayer.SpeakChina(Parameters.ScannerDataControlParameter.SpeeckContent);
                                        if (Loop > 1)
                                        {
                                            Thread.Sleep(3000);
                                        }
                                    }
                                    MediaSoundPlayer.PlayerComplete = false;
                                }
                            }

                            ReportStatus = false;
                        }

                        //播放音频延时
                        Thread.Sleep(Convert.ToInt32(SoundDelay * 1000));
                    }
                    else
                    {
                        Thread.Sleep(2000);
                    }
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended("播放媒体失败" + ex.Message, ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// 检测是否是根据指定条件播报语音
        /// </summary>
        /// <param name="LastData"></param>
        /// <param name="Reporting"></param>
        /// <returns></returns>
        private bool CheckScannerReportUserType()
        {
            bool result = false;

            try
            {
                //黑名单/白名单/普通用户
                if (Parameters.ScannerDataControlParameter.WhiteListMode && Parameters.ScannerDataControlParameter.BlackListMode && Parameters.ScannerDataControlParameter.OtherListMode)
                {
                    lock (JsonInterFace.FSM.StatusLock)
                    {
                        if (JsonInterFace.FSM.BlackListStatus == 1 || JsonInterFace.FSM.OtherListStauts == 1)
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }

                        JsonInterFace.FSM.Reset(true, false, true);
                    }
                }
                //黑名单/普通用户
                if (!Parameters.ScannerDataControlParameter.WhiteListMode && Parameters.ScannerDataControlParameter.BlackListMode && Parameters.ScannerDataControlParameter.OtherListMode)
                {
                    lock (JsonInterFace.FSM.StatusLock)
                    {
                        if (JsonInterFace.FSM.BlackListStatus == 1 || JsonInterFace.FSM.OtherListStauts == 1)
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }

                        JsonInterFace.FSM.Reset(true, false, true);
                    }
                }
                //白名单/普通用户
                else if (Parameters.ScannerDataControlParameter.WhiteListMode && !Parameters.ScannerDataControlParameter.BlackListMode && Parameters.ScannerDataControlParameter.OtherListMode)
                {
                    lock (JsonInterFace.FSM.StatusLock)
                    {
                        if (JsonInterFace.FSM.WhiteListStatus == 1 || JsonInterFace.FSM.OtherListStauts == 1)
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }
                        JsonInterFace.FSM.Reset(false, true, true);
                    }
                }
                //黑名单/白名单
                else if (Parameters.ScannerDataControlParameter.WhiteListMode && Parameters.ScannerDataControlParameter.BlackListMode && !Parameters.ScannerDataControlParameter.OtherListMode)
                {
                    lock (JsonInterFace.FSM.StatusLock)
                    {
                        if (JsonInterFace.FSM.BlackListStatus == 1 || JsonInterFace.FSM.WhiteListStatus == 1)
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }
                        JsonInterFace.FSM.Reset(false, true, false);
                    }
                }
                //白名单
                else if (Parameters.ScannerDataControlParameter.WhiteListMode && !Parameters.ScannerDataControlParameter.BlackListMode && !Parameters.ScannerDataControlParameter.OtherListMode)
                {
                    lock (JsonInterFace.FSM.StatusLock)
                    {
                        if (JsonInterFace.FSM.WhiteListStatus == 1)
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }
                        JsonInterFace.FSM.Reset(false, true, false);
                    }
                }
                //黑名单
                else if (!Parameters.ScannerDataControlParameter.WhiteListMode && Parameters.ScannerDataControlParameter.BlackListMode && !Parameters.ScannerDataControlParameter.OtherListMode)
                {
                    lock (JsonInterFace.FSM.StatusLock)
                    {
                        if (JsonInterFace.FSM.BlackListStatus == 1)
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }
                        JsonInterFace.FSM.Reset(true, false, false);
                    }
                }

                //普通用户
                else if (!Parameters.ScannerDataControlParameter.WhiteListMode && !Parameters.ScannerDataControlParameter.BlackListMode && Parameters.ScannerDataControlParameter.OtherListMode)
                {
                    lock (JsonInterFace.FSM.StatusLock)
                    {
                        if (JsonInterFace.FSM.OtherListStauts == 1)
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }
                        JsonInterFace.FSM.Reset(false, false, true);
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Parameters.PrintfLogsExtended("播报检测捕号上报", ex.Message, ex.StackTrace);
            }

            return result;
        }

        /// <summary>
        /// 从缓存获到未知设备
        /// </summary>
        private void GettingUnknownDeviceList()
        {
            DataTable TmpUnknownDevice = null;
            while (true)
            {
                //屏蔽期间
                if (!Parameters.STRefresh)
                {
                    lock (JsonInterFace.UnKnownDeviceListsParameter.LockObject)
                    {
                        //未知设备重复丢弃
                        for (int i = 0; i < SubWindow.UnKnownDeviceListsControlWindow.SubmitedUnknownDeviceLists.Count; i++)
                        {
                            for (int j = 0; j < JsonInterFace.UnKnownDeviceListsParameter.UnKnownDeviceTab.Rows.Count; j++)
                            {
                                if ((JsonInterFace.UnKnownDeviceListsParameter.UnKnownDeviceTab.Rows[j]["SN"].ToString()
                                    == SubWindow.UnKnownDeviceListsControlWindow.SubmitedUnknownDeviceLists[i].SN)
                                   )
                                {
                                    JsonInterFace.UnKnownDeviceListsParameter.UnKnownDeviceTab.Rows.RemoveAt(j);
                                    break;
                                }
                            }
                        }
                    }

                    Thread.Sleep(100);
                    continue;
                }

                try
                {
                    if (JsonInterFace.UnKnownDeviceListsParameter.UnKnownDeviceTab.Rows.Count > 0)
                    {
                        lock (JsonInterFace.UnKnownDeviceListsParameter.LockObject)
                        {
                            TmpUnknownDevice = JsonInterFace.UnKnownDeviceListsParameter.UnKnownDeviceTab.Copy();
                            JsonInterFace.UnKnownDeviceListsParameter.UnKnownDeviceTab.Rows.Clear();

                            //未知设备重复丢弃
                            for (int i = 0; i < SubWindow.UnKnownDeviceListsControlWindow.SubmitedUnknownDeviceLists.Count; i++)
                            {
                                for (int j = 0; j < TmpUnknownDevice.Rows.Count; j++)
                                {
                                    if (TmpUnknownDevice.Rows[j]["SN"].ToString()
                                        == SubWindow.UnKnownDeviceListsControlWindow.SubmitedUnknownDeviceLists[i].SN
                                       )
                                    {
                                        TmpUnknownDevice.Rows.RemoveAt(j);
                                        break;
                                    }
                                }
                            }
                        }

                        lock (SubWindow.UnKnownDeviceListsControlWindow.ShowUnKnownDeviceListsLocked)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                for (int i = 0; i < TmpUnknownDevice.Rows.Count; i++)
                                {
                                    bool Flag = true;
                                    StatusIcon UnknownDeviceNameInfo = new StatusIcon();
                                    UnKnownDeviceListsParameterClass Item = new UnKnownDeviceListsParameterClass();
                                    if (SubWindow.UnKnownDeviceListsControlWindow.ShowUnKnownDeviceLists.Count <= 0)
                                    {
                                        Item.ID = (i + 1).ToString();
                                    }
                                    else
                                    {
                                        Item.ID = (int.Parse(SubWindow.UnKnownDeviceListsControlWindow.ShowUnKnownDeviceLists[SubWindow.UnKnownDeviceListsControlWindow.ShowUnKnownDeviceLists.Count - 1].ID) + 1).ToString();
                                    }

                                    Item.IsSelected = false;
                                    Item.DeviceName = TmpUnknownDevice.Rows[i][1].ToString();
                                    Item.NodeIcon = UnknownDeviceNameInfo.RenameNone;
                                    Item.NodeIconTips = UnknownDeviceNameInfo.RenameNoneTips;
                                    Item.SN = TmpUnknownDevice.Rows[i][2].ToString();
                                    Item.Carrier = TmpUnknownDevice.Rows[i][3].ToString();
                                    Item.IpAddr = TmpUnknownDevice.Rows[i][4].ToString();
                                    Item.Port = TmpUnknownDevice.Rows[i][5].ToString();
                                    Item.Netmask = TmpUnknownDevice.Rows[i][6].ToString();
                                    Item.Mode = TmpUnknownDevice.Rows[i][7].ToString();
                                    Item.Online = TmpUnknownDevice.Rows[i][8].ToString();
                                    Item.LastOnline = TmpUnknownDevice.Rows[i][9].ToString();
                                    Item.IsActive = TmpUnknownDevice.Rows[i][10].ToString();
                                    Item.InnerType = TmpUnknownDevice.Rows[i][11].ToString();
                                    Item.ToStation = Parameters.ToStationDefault;
                                    Item.StationStatuIcon = UnknownDeviceNameInfo.None;
                                    Item.StationID = string.Empty;

                                    //检查是否存在
                                    for (int j = 0; j < SubWindow.UnKnownDeviceListsControlWindow.ShowUnKnownDeviceLists.Count; j++)
                                    {
                                        if (SubWindow.UnKnownDeviceListsControlWindow.ShowUnKnownDeviceLists[j].SN == TmpUnknownDevice.Rows[i][2].ToString())
                                        {
                                            Flag = false;
                                            SubWindow.UnKnownDeviceListsControlWindow.ShowUnKnownDeviceLists[i].IpAddr = Item.IpAddr;
                                            SubWindow.UnKnownDeviceListsControlWindow.ShowUnKnownDeviceLists[i].Port = Item.Port;
                                            SubWindow.UnKnownDeviceListsControlWindow.ShowUnKnownDeviceLists[i].Netmask = Item.Netmask;
                                            SubWindow.UnKnownDeviceListsControlWindow.ShowUnKnownDeviceLists[i].Carrier = Item.Carrier;
                                            SubWindow.UnKnownDeviceListsControlWindow.ShowUnKnownDeviceLists[i].Online = Item.Online;
                                            SubWindow.UnKnownDeviceListsControlWindow.ShowUnKnownDeviceLists[i].LastOnline = Item.LastOnline;
                                            SubWindow.UnKnownDeviceListsControlWindow.ShowUnKnownDeviceLists[i].IsActive = Item.IsActive;
                                            SubWindow.UnKnownDeviceListsControlWindow.ShowUnKnownDeviceLists[i].InnerType = Item.InnerType;
                                            break;
                                        }
                                    }

                                    //不存在才能添加
                                    if (Flag)
                                    {
                                        SubWindow.UnKnownDeviceListsControlWindow.ShowUnKnownDeviceLists.Add(Item);
                                    }
                                }

                                TmpUnknownDevice.Rows.Clear();
                            });
                        }

                    }
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended("获取未知设备列表异常", ex.Message, ex.StackTrace);
                }

                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// 监视线程
        /// </summary>
        private void CheckingThreads()
        {
            while (true)
            {
                Thread.Sleep(10000);
                try
                {
                    if (AppHeartThread != null)
                    {
                        if (AppHeartThread.ThreadState == ThreadState.Running)
                        {
                            Parameters.PrintfLogsExtended("心跳事务", "AppHeartThread is Running！IsAlive[" + AppHeartThread.IsAlive.ToString() + "]");
                        }
                        else if (AppHeartThread.ThreadState == ThreadState.Background)
                        {
                            Parameters.PrintfLogsExtended("心跳事务", "AppHeartThread is Background！IsAlive[" + AppHeartThread.IsAlive.ToString() + "]");
                        }
                        else if (AppHeartThread.ThreadState == ThreadState.Stopped)
                        {
                            Parameters.PrintfLogsExtended("心跳事务", "AppHeartThread is Stopped！IsAlive[" + AppHeartThread.IsAlive.ToString() + "]");
                        }
                        else if (AppHeartThread.ThreadState == ThreadState.Suspended)
                        {
                            Parameters.PrintfLogsExtended("心跳事务", "AppHeartThread is Suspended！IsAlive[" + AppHeartThread.IsAlive.ToString() + "]");
                        }
                        else if (AppHeartThread.ThreadState == ThreadState.Unstarted)
                        {
                            Parameters.PrintfLogsExtended("心跳事务", "AppHeartThread is Unstarted！IsAlive[" + AppHeartThread.IsAlive.ToString() + "]");
                        }
                        else if (AppHeartThread.ThreadState == ThreadState.WaitSleepJoin)
                        {
                            Parameters.PrintfLogsExtended("心跳事务", "AppHeartThread is WaitSleepJoin！IsAlive[" + AppHeartThread.IsAlive.ToString() + "]");
                        }
                        else if (AppHeartThread.ThreadState == ThreadState.Aborted)
                        {
                            Parameters.PrintfLogsExtended("心跳事务", "AppHeartThread is Aborted！IsAlive[" + AppHeartThread.IsAlive.ToString() + "]");
                        }
                    }
                    else
                    {
                        Parameters.PrintfLogsExtended("心跳事务", "AppHeartThread is Null！");
                    }
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// 获取所有网络接口状态
        /// </summary>
        private void GettingNetWorkInterfaceStatus()
        {
            try
            {
                while (true)
                {
                    lock (CheckNetWorkInterface.OperationLock)
                    {
                        NetworkInterface[] NetWorkInterfaceInfo = NetworkInterface.GetAllNetworkInterfaces();
                        CheckNetWorkInterface.NetWorkInterfaceList.Clear();
                        foreach (NetworkInterface Item in NetWorkInterfaceInfo)
                        {
                            CheckNetWorkInterfaceUpDown NetWorkInterfaceItems = new CheckNetWorkInterfaceUpDown();
                            NetWorkInterfaceItems.InterfaceName = Item.Name;
                            NetWorkInterfaceItems.Status = Item.OperationalStatus.ToString();
                            NetWorkInterfaceItems.Speed = Item.Speed.ToString();
                            String _MAC = Item.GetPhysicalAddress().ToString();
                            if (_MAC != "")
                            {
                                for (int i = 0; i < _MAC.Trim().Length / 2; i++)
                                {
                                    if (NetWorkInterfaceItems.MAC == "" || NetWorkInterfaceItems.MAC == null)
                                    {
                                        NetWorkInterfaceItems.MAC = _MAC.Substring(i * 2, 2);
                                    }
                                    else
                                    {
                                        NetWorkInterfaceItems.MAC += "-" + _MAC.Substring(i * 2, 2);
                                    }
                                }
                            }
                            else
                            {
                                NetWorkInterfaceItems.MAC = "";
                            }

                            CheckNetWorkInterface.NetWorkInterfaceList.Add(NetWorkInterfaceItems);
                        }
                    }

                    Thread.Sleep(3000);
                }
            }
            catch (Exception ex)
            {
                JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "网络媒体检测异常终止，" + ex.Message, "网络媒体检测", "终止");
            }
        }

        /// <summary>
        /// 信息统计到状态栏
        /// </summary>
        private void InfoStatisticGetting()
        {
            while (true)
            {
                try
                {
                    int StationTotal = 0;
                    int DeviceTotal = 0;
                    int DeviceOnlineTotal = 0;
                    int SysLogsTotal = 0;

                    DataTable DomainInfo = JsonInterFace.BindTreeViewClass.DeviceTreeTable;
                    //站点总数
                    StationTotal = DomainInfo.Select(string.Format("NodeType='{0}' and IsStation='{1}'", NodeType.StructureNode.ToString(), "1")).Length;
                    //设备总数
                    DeviceTotal = DomainInfo.Select(string.Format("NodeType='{0}'", NodeType.LeafNode.ToString())).Length;
                    //日志总数
                    SysLogsTotal = SelfSystemLog.Count;
                    //在线设备总数
                    DeviceOnlineTotal = DomainInfo.Select(string.Format("NodeType='{0}' and NodeIcon<>'{1}'", NodeType.LeafNode.ToString(), new NodeIcon().LeafNoConnectNodeIcon)).Length;

                    InfoStatisticParameter.LoginUser = JsonInterFace.LoginUserInfo[0].LoginUser;
                    InfoStatisticParameter.StationTotal = StationTotal.ToString();
                    InfoStatisticParameter.DeviceTotal = DeviceTotal.ToString();
                    InfoStatisticParameter.DeviceOnlineTotal = DeviceOnlineTotal.ToString();
                    InfoStatisticParameter.SysLogsTotal = SysLogsTotal.ToString();
                }
                catch (Exception ex)
                {
                    JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), ex.Message, ex.StackTrace, "信息统计异常");
                    Parameters.PrintfLogsExtended("信息统计异常", ex.Message, ex.StackTrace);
                }

                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// 递归获取设备在线数量
        /// </summary>
        /// <param name="Children"></param>
        /// <param name="StationList"></param>
        private void AutoGetAllDeviceOnlineTotal(IList<CheckBoxTreeModel> Children, ref int Total)
        {
            if (Children != null)
            {
                foreach (CheckBoxTreeModel itemChild in Children)
                {
                    if (itemChild.Children.Count > 0 && !itemChild.SelfNodeType.Equals(NodeType.LeafNode))
                    {
                        AutoGetAllDeviceOnlineTotal(itemChild.Children, ref Total);
                    }
                    else
                    {
                        if (itemChild.SelfNodeType == NodeType.LeafNode.ToString())
                        {
                            if (itemChild.IsOnLine)
                            {
                                Total++;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 启动未知设备提示器
        /// </summary>
        private void SetUnknownTipsWindow()
        {
            JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "未知设备监听器已启动", "正在监听...", "监听器正常");
            while (true)
            {
                try
                {
                    //自动
                    if (Parameters.UnknownDeviceWindowControlParameters.ActionType == ActionTypeList.Auto.ToString())
                    {
                        if (SubWindow.UnKnownDeviceListsControlWindow.ShowUnKnownDeviceLists.Count > 0)
                        {
                            if (Parameters.UnknownDeviceWindowControlParameters.UnknownDeviceTipsBlockAttribute != Visibility.Visible)
                            {
                                Parameters.UnknownDeviceWindowControlParameters.DevcieTreeRowSpan = 1;
                                Parameters.UnknownDeviceWindowControlParameters.UnknownDeviceTipsBlockAttribute = Visibility.Visible;
                            }
                        }
                        else
                        {
                            if (Parameters.UnknownDeviceWindowControlParameters.UnknownDeviceTipsBlockAttribute != Visibility.Collapsed)
                            {
                                Parameters.UnknownDeviceWindowControlParameters.DevcieTreeRowSpan = 2;
                                Parameters.UnknownDeviceWindowControlParameters.UnknownDeviceTipsBlockAttribute = Visibility.Collapsed;
                            }
                        }
                    }
                    //显示
                    else if (Parameters.UnknownDeviceWindowControlParameters.ActionType == ActionTypeList.Show.ToString())
                    {
                        if (Parameters.UnknownDeviceWindowControlParameters.UnknownDeviceTipsBlockAttribute != Visibility.Visible)
                        {
                            Parameters.UnknownDeviceWindowControlParameters.DevcieTreeRowSpan = 1;
                            Parameters.UnknownDeviceWindowControlParameters.UnknownDeviceTipsBlockAttribute = Visibility.Visible;
                        }
                    }
                    //隐藏
                    else if (Parameters.UnknownDeviceWindowControlParameters.ActionType == ActionTypeList.Hide.ToString())
                    {
                        if (Parameters.UnknownDeviceWindowControlParameters.UnknownDeviceTipsBlockAttribute != Visibility.Collapsed)
                        {
                            Parameters.UnknownDeviceWindowControlParameters.DevcieTreeRowSpan = 2;
                            Parameters.UnknownDeviceWindowControlParameters.UnknownDeviceTipsBlockAttribute = Visibility.Collapsed;
                        }
                    }

                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended("未知设备监听器", ex.Message, ex.StackTrace);
                    JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "Error: 未知设备监听器未就绪(" + ex.Message + ")", "未知设备监听", "失败");
                }
            }
        }

        /// <summary>
        /// APP与服务器之间心跳通讯
        /// </summary>
        private void AppTOServerHeartRequest()
        {
            AppHeartThread.DisableComObjectEagerCleanup();
            while (true)
            {
                try
                {
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        Parameters.PrintfLogsExtended("Heart Status (" + Parameters.HeartStatu.ToString() + ") <==> " + "NetWork Interface Connected(" + NetWorkClient.ControllerServer.Connected.ToString() + ")");
                        JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "Heart Status (" + Parameters.HeartStatu.ToString() + ") <==> " + "NetWork Interface Connected(" + NetWorkClient.ControllerServer.Connected.ToString() + ")", "System Information", "正常");

                        //心跳正常
                        if (Parameters.HeartStatu == 1)
                        {
                            try
                            {
                                Parameters.HeartStatu = 0;
                                NetWorkClient.ControllerServer.Send(JsonInterFace.AppTOServerHeartRequest(JsonInterFace.LoginUserInfo[0].LoginUser, JsonInterFace.LoginUserInfo[0].WorkGroup));
                                JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "[" + Parameters.HeartTime + "秒/次]心跳信息发送到服务器[" + JsonInterFace.RemoteHost + ":" + JsonInterFace.RemotePort + "]", "发送心跳消息", "正在通讯...");
                            }
                            catch (Exception ex)
                            {
                                Parameters.PrintfLogsExtended("心跳通讯异常", ex.Message, ex.StackTrace);
                                JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "心跳通讯异常," + ex.Message, "发送心跳", "失败");
                            }
                        }
                        else
                        {
                            try
                            {
                                JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "客户端与服务器断开,偿试重连开始...", "重连接服务器", "正在连接...");
                                RemoteReconnectThead.Suspend();
                                NetWorkClient.ControllerServer.Close();
                                new NetWorkClient();
                                Thread.Sleep(5 * 1000);
                                Parameters.HeartStatu = 1;
                                RemoteReconnectThead.Resume();
                                continue;
                            }
                            catch (Exception ex)
                            {
                                Parameters.PrintfLogsExtended("心跳超时,重连服务器产生异常", ex.Message, ex.StackTrace);
                            }
                        }
                    }
                    else
                    {
                        Parameters.PrintfLogsExtended("Heart Status (" + Parameters.HeartStatu.ToString() + ") <==> " + "NetWork Interface Connected(" + NetWorkClient.ControllerServer.Connected.ToString() + ")");
                        JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "Heart Status (" + Parameters.HeartStatu.ToString() + ") <==> " + "NetWork Interface Connected(" + NetWorkClient.ControllerServer.Connected.ToString() + ")", "System Information", "失败");
                    }
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended("心跳任务异常已终止", ex.Message, ex.StackTrace);
                    JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "心跳任务异常已终止," + ex.Message, "心跳任务终止", "异常");
                }

                Thread.Sleep(Parameters.HeartTime * 1000);
            }
        }

        /// <summary>
        /// 网络断网重连和检测
        /// </summary>
        private void ReconnectToRemounteServer()
        {
            Byte Flag = 1;
            while (true)
            {
                Thread.Sleep(Parameters.NetCheckDelayFactor * 1000);

                try
                {
                    //检测网络是否禁用或网线是否脱落
                    if (CheckNetWorkInterface.GettingNetWorkInterfaceOnline(Parameters.GetMacAddress(JsonInterFace.ClientIP)))
                    {
                        try
                        {
                            if (!NetWorkClient.ControllerServer.Connected)
                            {
                                //断网导至所有设备下线(暂时不启用这项功能)
                                //NavigatePages.DeviceListWindow.SettingAllDivceOffline(JsonInterFace.UsrdomainData);

                                Flag = 0;
                                JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "客户端与服务器断开,偿试重连开始...", "重连接服务器", "正在连接...");
                                NetWorkClient.ControllerServer.Close();
                                new NetWorkClient();
                            }
                            else
                            {
                                if (Flag == 0)
                                {
                                    Flag++;
                                    JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "客户端与服务器恢复连接成功！", "重连服务器", "连接成功");

                                    //重载入设备
                                    NavigatePages.DeviceListWindow.ReloadDeviceList(false);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "客户端与服务器重连," + ex.Message, "重连接服务器", "重连异常");
                        }
                    }
                    else
                    {
                        NetWorkClient.ControllerServer.Close();
                        JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "检测到本地网络已经禁用或网线已脱落...", "本地网络接口异常", "网络检测...");

                        //断网导至所有设备下线
                        NavigatePages.DeviceListWindow.SettingAllDivceOffline(JsonInterFace.UsrdomainData);
                    }

                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended("网络断连事务异常已终止", ex.Message, ex.StackTrace);
                    JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "网络断连事务异常已终止," + ex.Message, "网络断连终止", "异常");
                }
            }
        }

        //获取句柄请求域
        private void GetDeviceListTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            int DelayTimes = 0;
            try
            {
                Dispatcher.Invoke(() =>
                {
                    while (true)
                    {
                        if (this.Title != "" && this.Title != null)
                        {
                            WindowInteropHelper SelfWindowHelper = new WindowInteropHelper(this);
                            Parameters.WinHandle = SelfWindowHelper.Handle;

                            if (Parameters.WinHandle != IntPtr.Zero)
                            {
                                SubmitRequestDomainLists();
                                break;
                            }
                        }
                        else
                        {
                            DelayTimes += 500;
                            Thread.Sleep(500);
                        }

                        if (DelayTimes > 5 * 60 * 1000)
                        {
                            JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "[" + (this.Title == "" ? "Main window not found！" : this.Title) + "]域列表请求异常!", "域列表请求", "失败");
                            break;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void SubmitRequestDomainLists()
        {
            Parameters.ConfigType = "AutoGettingDeviceDomainList";
            NavigatePages.DeviceListWindow.GetDomainNameListRequest();
        }

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
            //请求获取设备列表
            try
            {
                //主窗口显示
                if (msg == Parameters.WM_MainWindowShowMessage)
                {
                    Dispatcher.Invoke(() => { this.Visibility = Visibility.Visible; });
                }
                //获取所有设备响应
                else if (msg == Parameters.WM_RequestDeviceLists)
                {
                    NavigatePages.DeviceListWindow.GetDeviceListsRequest();
                }
                //自动获取设备详细信息
                else if (msg == Parameters.WM_GettingAllDeviceDetailMessage)
                {
                    NavigatePages.DeviceListWindow.AllDeviceDitailRequestTimer.Start();
                }
                //加载设备列表显示
                else if (msg == Parameters.WM_DeviceListInfoLoad)
                {
                    NavigatePages.DeviceListWindow.LoadDeviceListTreeViewTimer.Start();
                }
                //添加域名
                else if (msg == Parameters.WM_AddDomainNameResponse)
                {
                    NavigatePages.DeviceListWindow.AddNewDoaminNameTOTreeView();
                }
                //重命名域名
                else if (msg == Parameters.WM_ReNameDomainNameResponse)
                {
                    NavigatePages.DeviceListWindow.ReNameDomainNameUpdateTOTreeView();
                }
                //删除域名
                else if (msg == Parameters.WM_DeleteDomainNameResponse)
                {
                    NavigatePages.DeviceListWindow.DeleteDomainNameUpdateToTreeView();
                }
                //添加设备信息
                else if (msg == Parameters.WM_AddDeviceNameResponse)
                {
                    NavigatePages.DeviceListWindow.AddNewDeviceTOTreeView();
                }
                //删除设备信息
                else if (msg == Parameters.WM_DeleteDeviceNameResponse)
                {
                    NavigatePages.DeviceListWindow.DeleteDeviceTOTreeView();
                }
                //设备名称更新
                else if (msg == Parameters.WM_UpdateDeviceInfoResponse)
                {
                    NavigatePages.DeviceListWindow.UpdateDeviceInfoTOTreeView();
                }
                //小区信息更新
                else if (msg == Parameters.WM_UpdateCellNeighConfigrationResponse)
                {
                    NavigatePages.DeviceListWindow.CellNeighConfigrationUpdateTOTreeView();
                }
                //GPS信息更新
                else if (msg == Parameters.WM_GPSConfigrationResponse)
                {
                    NavigatePages.DeviceListWindow.GPSConfigrationUpdateTOTreeView();
                }
                //NTP信息更新
                else if (msg == Parameters.WM_NTPConfigrationResponse)
                {
                    NavigatePages.DeviceListWindow.NTPConfigrationUpdateTOTreeView();
                }
                //同步源信息更新
                else if (msg == Parameters.WM_SyncSourceConfigrationResponse)
                {
                    NavigatePages.DeviceListWindow.SyncSourceConfigrationUpdateTOTreeView();
                }
                //时间段控制信息更新
                else if (msg == Parameters.WM_APPeriodTimeConfigrationResponse)
                {
                    NavigatePages.DeviceListWindow.APPeriodTimeConfigrationUpdateTOTreeView();
                }
                //AP激活响应
                else if (msg == Parameters.WM_APActiveResponse)
                {
                    NavigatePages.DeviceListWindow.ApActiveResponseTimer.Start();
                }
                //AP去激活响应
                else if (msg == Parameters.WM_APNoActiveResponse)
                {
                    NavigatePages.DeviceListWindow.ApUnActiveResponseTimer.Start();
                }
                //捕号窗口颜色
                else if (msg == Parameters.WM_ChangeScannerDataRowsBackGroundColor)
                {
                    NavigatePages.UEInfoWindow.ApplayDataRowBackGroundColor();
                }
                //重启媒体播放器
                else if (msg == Parameters.WM_RestartPlayerThreadRequest)
                {
                    if (ScannerWarningSoundThead == null)
                    {
                        try
                        {
                            ScannerWarningSoundThead.Abort();
                            ScannerWarningSoundThead.Join();
                            Thread.Sleep(1000);
                        }
                        catch (Exception ex)
                        {
                            Parameters.PrintfLogsExtended("启动媒体播放器失败", ex.Message, ex.StackTrace);
                        }
                        finally
                        {
                            ScannerWarningSoundThead = new Thread(new ThreadStart(PlayerSound));
                            ScannerWarningSoundThead.Start();
                        }
                    }
                }
                //GSM激活或去激活后获取上次正确配置
                else if (msg == Parameters.WM_GSMCarrierActionResponse)
                {
                    NavigatePages.DeviceListWindow.GettingGSMLastConfigure();
                }
                //界面显示控制 捕号窗口
                else if (msg == Parameters.WM_ShowScannerWindowControl)
                {
                    ShowScannerWindowControl();
                }
                //测量报告窗口
                else if (msg == Parameters.WM_ShowMeasurementReportWindowControl)
                {
                    ShowMeasurementReportWindowControl();
                }
                //通话记录窗口
                else if (msg == Parameters.WM_ShowCallRecordsWindowControl)
                {
                    ShowCallRecordsWindowControl();
                }
                //短信记录窗口
                else if (msg == Parameters.WM_ShowSMSRecordsWindowControl)
                {
                    ShowSMSRecordsWindowControl();
                }
                //系统日志状态窗口
                else if (msg == Parameters.WM_ShowSystemLogsInfoWindowControl)
                {
                    ShowSystemLogsInfoWindowControl();
                }
                //默认显示
                else if (msg == Parameters.WM_ShowDefaultWindowControl)
                {
                    ShowDefaultWindowControl();
                }
                //删除对应黑名单追踪表IMSI
                else if (msg == Parameters.WM_DeleteMeasureReportsIMSIRequest)
                {
                    DeleteMeasureReports_IMSI(lParam);
                }
                //重载设备列表
                else if (msg == Parameters.WM_ReLoadDeviceListMessage)
                {
                    NavigatePages.DeviceListWindow.ReloadDeviceList(false);
                }
                //启动信息栏统计
                else if (msg == Parameters.WM_InfoStatisticMessage)
                {
                    if (InfoStatisticThread.ThreadState == ThreadState.Unstarted)
                    {
                        InfoStatisticThread.Start();
                    }
                }
                //服务端通知客户端清理未知设备
                else if (msg == Parameters.WM_UnknownDeviceAutoUpdate)
                {
                    try
                    {
                        if (lParam != IntPtr.Zero)
                        {
                            string LParm = Marshal.PtrToStringBSTR(lParam);
                            if (Parameters.ISDigital(LParm))
                            {
                                if (Convert.ToInt32(LParm) <= 0)
                                {
                                    lock (JsonInterFace.UnKnownDeviceListsParameter.LockObject)
                                    {
                                        Dispatcher.Invoke(() =>
                                        {
                                            SubWindow.UnKnownDeviceListsControlWindow.ShowUnKnownDeviceLists.Clear();
                                            SubWindow.UnKnownDeviceListsControlWindow.UnKnownDeviceNameStationList.Clear();
                                            JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "服务端通知清理未知设备成功...", "清理未知设备", "成功");
                                        });
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Parameters.PrintfLogsExtended("客户端清理未知设备失败！", e.Message, e.StackTrace);
                    }
                }
                else if (msg == Parameters.WM_SubWindowsConfigurationMessage)
                {
                    MainWindowsConfiguration();
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("响应Windows消息事件：", ex.Message, ex.StackTrace);
            }
            return hwnd;
        }

        //删除黑名单追踪IMSI
        private void DeleteMeasureReports_IMSI(IntPtr IMSIHandle)
        {
            try
            {
                string IMSI = Marshal.PtrToStringAnsi(IMSIHandle);
                for (int i = 0; i < NavigatePages.UEInfoWindow.SelfMeasReportBlackList.Count; i++)
                {
                    if (IMSI == NavigatePages.UEInfoWindow.SelfMeasReportBlackList[i].IMSI)
                    {
                        NavigatePages.UEInfoWindow.SelfMeasReportBlackList.RemoveAt(i);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //初始化语言
                if (Parameters.LanguageType.Equals("EN"))
                {
                    this.DataContext = (Language_EN.SystemConrol)LanguageClass;
                    btnClose.DataContext = (Language_EN.SystemConrol)LanguageClass;
                    this.Title = new Language_EN.SystemConrol().Title;
                }
                else
                {
                    this.DataContext = (Language_CN.SystemConrol)LanguageClass;
                    btnClose.DataContext = (Language_CN.SystemConrol)LanguageClass;
                    this.Title = new Language_CN.SystemConrol().Title;
                }

                SystemLogsInformation LogsTemp = new SystemLogsInformation();
                LogsTemp.DTime = System.DateTime.Now.ToString();
                LogsTemp.Object = "用户[" + JsonInterFace.LoginUserInfo[0].LoginUser + "]系统登录";
                LogsTemp.Action = "系统登录";
                LogsTemp.Other = "成功";
                SysLogsCachingList.Add(LogsTemp);

                //载入菜单
                MenuFram.Navigate(AllWindow["Menu"]);
                //载入设备列表
                DeviceListBarFram.Navigate(AllWindow["DeviceListWindow"]);
                //载入UE数据显示
                UEDataFram.Navigate(AllWindow["UEInfoWindow"]);
                //载入系统日志显示
                SysLogDataFram.Navigate(AllWindow["SystemLogInfo"]);

                //启动UDP服务器
                UDPServerStart();

                #region ----------------配置主界面标题栏样式----------------
                var hwnd = new WindowInteropHelper(this).Handle;

                //去除标题栏系统菜单
                /*
                SetWindowLong(
                                hwnd,
                                WindowsAPIStyleEventsParameterClass.GWL_STYLE,
                                GetWindowLong(hwnd, WindowsAPIStyleEventsParameterClass.GWL_STYLE)
                                & ~WindowsAPIStyleEventsParameterClass.WS_SYSMENU
                             );

                */

                //设置题栏背景色
                SetWindowLong(hwnd, 1, 1);
                #endregion -------------------------------------------------

                //设备列表
                DeviceListLoadingStatuFram.DataContext = JsonInterFace.DeviceListRequestCompleteStatus;

                //状态栏信息
                txtWelcome.DataContext = InfoStatisticParameter;
                txtLoginUserNameValue.DataContext = InfoStatisticParameter;
                txtStationTotal.DataContext = InfoStatisticParameter;
                txtDeviceTotal.DataContext = InfoStatisticParameter;
                txtDeviceOnlineTotal.DataContext = InfoStatisticParameter;
                txtSysLogsListTotal.DataContext = InfoStatisticParameter;
                txtScannerTotal.DataContext = JsonInterFace.ScannerData;

                //启动设备载入进度窗口
                DeviceListLoadingStatuFram.Navigate(AllWindow["DeviceLoadingStatusWindow"]);

                //启动心跳线程
                AppHeartThread.Start();

                //启动获取设备列表
                GetDeviceListTimer.Start();

                //启动未知设备提示线程
                SetUnknownTipsWindowThread.Start();

                //窗口显示属性
                UEInfoWindow.DataContext = Parameters.DataReportWindowsControl;
                SystemLogsWindow.DataContext = Parameters.DataReportWindowsControl;
                SystemStatusBar.DataContext = Parameters.DataReportWindowsControl;

                //语音播报
                ScannerWarningSoundThead.Start();

                //线程监视
                DaemonThread.Start();

                //电围用户组显示
                if (JsonInterFace.LoginUserInfo[0].WorkGroup.Equals("电围组"))
                {
                    DeviceListWindowResizeLine.Visibility = Visibility.Collapsed;
                    SystemLogsWindowResizeLine.Visibility = Visibility.Collapsed;
                    UEDataFram.Visibility = Visibility.Collapsed;
                    UEInfoWindow.BorderBrush = null;

                    System.Windows.Controls.Grid.SetRow(DeviceListWindow, 1);
                    System.Windows.Controls.Grid.SetColumn(DeviceListWindow, 0);
                    System.Windows.Controls.Grid.SetRowSpan(DeviceListWindow, 1);
                    System.Windows.Controls.Grid.SetColumnSpan(DeviceListWindow, 4);
                }

                //主窗口按配置显示
                new Thread(() => { MainWindowsConfiguration(); }).Start();
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("主界面初始化失败", ex.Message, ex.StackTrace);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            ((System.Windows.Controls.Button)sender).Tag = 1;
            if (MessageBox.Show("确定退出智能通讯管控管理系统？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                this.Close();
            }
            else
            {
                ((System.Windows.Controls.Button)sender).Tag = 0;
            }
        }

        private void Window_Terminate(object sender, EventArgs e)
        {
            bool[] Finished = new bool[] { false, false, false };
            object valueLoack = new object();
            Parameters.NetCheckDelayFactor = 0xFF;
            this.IsEnabled = false;
            IntPtr TipsWinHandle = IntPtr.Zero;
            //提示窗
            WaitForCloseWindow TipsWin = new WaitForCloseWindow();
            //提示窗句柄
            new Thread(() =>
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    TipsWin.Show();
                    Parameters.TipsWinCloseHandle = new WindowInteropHelper(TipsWin).Handle;
                }));
            }).Start();

            //结束线程
            if (AppHeartThread != null)
            {
                if (AppHeartThread.ThreadState == ThreadState.Running || AppHeartThread.ThreadState == ThreadState.WaitSleepJoin)
                {
                    new Thread(() =>
                    {
                        AppHeartThread.Abort();
                        AppHeartThread.Join();
                    }).Start();

                    new Thread(() =>
                    {
                        while (true)
                        {
                            if (AppHeartThread.ThreadState == ThreadState.Stopped
                            || AppHeartThread.ThreadState == ThreadState.Aborted)
                            {
                                AppHeartThread = null;
                                Finished[0] = true;
                                lock (valueLoack)
                                {
                                    ClosingControlPara.Finished++;
                                }
                                break;
                            }
                            Thread.Sleep(100);
                        }
                    }).Start();
                }
            }

            if (RemoteReconnectThead != null)
            {
                if (RemoteReconnectThead.ThreadState == ThreadState.Running || RemoteReconnectThead.ThreadState == ThreadState.WaitSleepJoin)
                {
                    new Thread(() =>
                    {
                        RemoteReconnectThead.Abort();
                        RemoteReconnectThead.Join();
                    }).Start();

                    new Thread(() =>
                    {
                        while (true)
                        {
                            if (RemoteReconnectThead.ThreadState == ThreadState.Stopped
                            || RemoteReconnectThead.ThreadState == ThreadState.Aborted)
                            {
                                RemoteReconnectThead = null;
                                Finished[1] = true;
                                lock (valueLoack)
                                {
                                    ClosingControlPara.Finished++;
                                }
                                break;
                            }
                            Thread.Sleep(200);
                        }
                    }).Start();
                }
            }

            //关闭网络
            new Thread(() =>
            {
                if (NetWorkClient.ControllerServer.Connected)
                {
                    NetWorkClient.ControllerServer.Close();
                }
            }).Start();

            new Thread(() =>
            {
                while (true)
                {
                    if (!NetWorkClient.ControllerServer.Connected)
                    {
                        Finished[2] = true;
                        lock (valueLoack)
                        {
                            ClosingControlPara.Finished++;
                        }
                        break;
                    }
                    Thread.Sleep(300);
                }

            }).Start();


            new Thread(() =>
            {
                while (true)
                {
                    if (Finished[0] && Finished[1] && Finished[2])
                    {
                        Parameters.SendMessage(Parameters.TipsWinCloseHandle, Parameters.WM_TipsWinCloseMessage, 0, 0);
                        break;
                    }
                    Thread.Sleep(500);
                }
            }).Start();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Parameters.NetCheckDelayFactor = 0xFF;

            if (AppHeartThread != null)
            {
                if (AppHeartThread.ThreadState == ThreadState.Running || AppHeartThread.ThreadState == ThreadState.WaitSleepJoin)
                {
                    AppHeartThread.Abort();
                    AppHeartThread.Join();
                }
            }

            if (RemoteReconnectThead != null)
            {
                if (RemoteReconnectThead.ThreadState == ThreadState.Running || RemoteReconnectThead.ThreadState == ThreadState.WaitSleepJoin)
                {
                    RemoteReconnectThead.Abort();
                    RemoteReconnectThead.Join();
                }
            }

            //关闭网络
            if (NetWorkClient.ControllerServer.Connected)
            {
                NetWorkClient.ControllerServer.Close();
            }
            System.Environment.Exit(System.Environment.ExitCode);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (btnClose.Tag != null)
            {
                //点击右上角关闭
                if (btnClose.Tag.ToString() != "1")
                {
                    if (MessageBox.Show("确定退出智能通讯管控管理系统？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        e.Cancel = false;
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
            }
            //点击菜单退出关闭
            else
            {
                if (MessageBox.Show("确定退出智能通讯管控管理系统？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    e.Cancel = false;
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        //基本参数请求
        private void ServerBaseParametersRequest()
        {
            if (NetWorkClient.ControllerServer.Connected)
            {
                NetWorkClient.ControllerServer.Send(JsonInterFace.ServerBaseParameterRequest());
            }
        }

        #region UDP服务器
        /// <summary>
        /// UDP服务器
        /// </summary>
        public void Server()
        {
            IPEndPoint IpEp = null;
            EndPoint Remote = null;
            DataCaching DataStruct = null;
            try
            {
                udpServer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                udpServer.Bind(serverIP);
                IpEp = new IPEndPoint(IPAddress.Any, Parameters.UDPPort);
                Remote = (EndPoint)IpEp;
                DataStruct = new DataCaching();
                Started = true;
            }
            catch (Exception Ex)
            {
                Runing = false;
                Started = false;
                Parameters.PrintfLogsExtended("UDP服务器启动失败", Ex.Message, Ex.StackTrace);
                return;
            }

            while (Runing)
            {
                byte[] data = new byte[4096];
                int Length = 0;
                try
                {
                    Length = udpServer.ReceiveFrom(data, ref Remote);
                    PrintSysLogs(data, Length);
                }

                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended(string.Format("UDP服务器出现异常：{0}", ex.Message), ex.StackTrace);
                    break;
                }
            }

            try
            {
                if (udpServer != null)
                {
                    if (udpServer.Connected)
                    {
                        JsonInterFace.UDPClient.Close();
                        udpServer.Close();
                    }
                }
                Started = false;
                Thread.Sleep(2000);
                Parameters.PrintfLogsExtended(string.Format("UDP服务器停止：{0}", DateTime.Now.ToString()));
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended(string.Format("UDP服务器停止失败：{0},{1}", Ex.Message, DateTime.Now.ToString()));
            }
        }

        /// <summary>
        /// 启动UDP服务器
        /// </summary>
        public void UDPServerStart()
        {
            try
            {
                if (!Started)
                {
                    if (UDPServerHandle.ThreadState == ThreadState.Running
                        || UDPServerHandle.ThreadState == ThreadState.WaitSleepJoin)
                    {
                        UDPServerHandle.Abort();
                        UDPServerHandle.Join();
                        UDPServerHandle = null;
                        UDPServerHandle = new Thread(new ThreadStart(Server));
                        UDPServerHandle.IsBackground = true;
                        UDPServerHandle.Start();
                    }
                    else
                    {
                        UDPServerHandle = new Thread(new ThreadStart(Server));
                        UDPServerHandle.IsBackground = true;
                        UDPServerHandle.Start();
                    }
                }

                //客户端启动连接
                JsonInterFace.UDPClient.Start();
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("UDP服务器启动失败", ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// 停止UPD服务器
        /// </summary>
        public void UDPServerStop()
        {
            try
            {
                JsonInterFace.UDPClient.Close();
                UDPServerHandle.Priority = ThreadPriority.Lowest;
                UDPServerHandle.Abort();
                UDPServerHandle.Join();
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("停止UPD服务器异常", Ex.Message, Ex.StackTrace);
            }
            finally
            {
                UDPServerHandle = null;
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
            }
        }
        #endregion

        #region 显示系统日志
        public void PrintSysLogs(byte[] lParam, int wParam)
        {
            #region 不显示日志
            if (Parameters.SysLogsTotal == 0)
            {
                lock (LogsListsLocker)
                {
                    Dispatcher.Invoke(delegate ()
                    {
                        if (SelfSystemLog.Count > 0)
                        {
                            SelfSystemLog.Clear();
                        }
                    });
                }
                return;
            }

            if (Parameters.WinHandle == IntPtr.Zero) { return; }
            #endregion

            try
            {
                string DTime = string.Empty;
                string Object = string.Empty;
                string Action = string.Empty;
                string Other = string.Empty;


                //读取缓存---------------------------
                if (SysLogsList.Count > 0)
                {
                    Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        for (int i = 0; i < SysLogsList.Count; i++)
                        {
                            if (SelfSystemLog.Count > 0)
                            {
                                SelfSystemLog.Insert(
                                                        0,
                                                        new SystemLogsInformation()
                                                        {
                                                            DTime = SysLogsList[i].DTime,
                                                            Object = SysLogsList[i].Object,
                                                            Action = SysLogsList[i].Action,
                                                            Other = SysLogsList[i].Other
                                                        }
                                                    );
                            }
                            else
                            {
                                SelfSystemLog.Add(
                                                        new SystemLogsInformation()
                                                        {
                                                            DTime = SysLogsList[i].DTime,
                                                            Object = SysLogsList[i].Object,
                                                            Action = SysLogsList[i].Action,
                                                            Other = SysLogsList[i].Other
                                                        }
                                                 );
                            }
                        }
                        SysLogsList.Clear();
                    });
                }

                if (MainWindow.SysLogsCachingList.Count > 0)
                {
                    Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        for (int i = 0; i < MainWindow.SysLogsCachingList.Count; i++)
                        {
                            if (SelfSystemLog.Count > 0)
                            {
                                SelfSystemLog.Insert(
                                                        0,
                                                        new SystemLogsInformation()
                                                        {
                                                            DTime = MainWindow.SysLogsCachingList[i].DTime,
                                                            Object = MainWindow.SysLogsCachingList[i].Object,
                                                            Action = MainWindow.SysLogsCachingList[i].Action,
                                                            Other = MainWindow.SysLogsCachingList[i].Other
                                                        }
                                                    );
                            }
                            else
                            {
                                SelfSystemLog.Add(
                                                    new SystemLogsInformation()
                                                    {
                                                        DTime = MainWindow.SysLogsCachingList[i].DTime,
                                                        Object = MainWindow.SysLogsCachingList[i].Object,
                                                        Action = MainWindow.SysLogsCachingList[i].Action,
                                                        Other = MainWindow.SysLogsCachingList[i].Other
                                                    }
                                                 );
                            }
                        }
                        MainWindow.SysLogsCachingList.Clear();
                    });
                }
                //----------------------------------------

                //获取数据
                byte[] LogsBuff = new byte[wParam];
                Buffer.BlockCopy(lParam, 0, LogsBuff, 0, wParam);
                string JosnStr = Encoding.UTF8.GetString(LogsBuff);

                //解析JSON串
                DataStruct.ResultJsonData = JsonConvert.DeserializeObject<MsgStruct.InterModuleMsgStruct>(JosnStr);
                JsonInterFace.AnalysisSysLogsStruct(DataStruct.ResultJsonData, ref DTime, ref Object, ref Action, ref Other);

                //不重复则显示
                if (DuplicationLogs.Object != Object)
                {
                    DuplicationLogs.Object = Object;
                    DuplicationLogs.Action = Action;
                    DuplicationLogs.Other = Other;

                    lock (LogsListsLocker)
                    {
                        if (SelfSystemLog.Count < Parameters.SysLogsTotal)
                        {
                            Dispatcher.BeginInvoke((Action)delegate ()
                            {
                                if (SelfSystemLog.Count > 0)
                                {
                                    SelfSystemLog.Insert(
                                                            0,
                                                            new SystemLogsInformation()
                                                            {
                                                                DTime = DTime,
                                                                Object = Object,
                                                                Action = Action,
                                                                Other = Other
                                                            }
                                                        );
                                }
                                else
                                {
                                    SelfSystemLog.Add(
                                                        new SystemLogsInformation()
                                                        {
                                                            DTime = DTime,
                                                            Object = Object,
                                                            Action = Action,
                                                            Other = Other
                                                        }
                                                      );
                                }
                            });
                        }
                        else
                        {
                            Dispatcher.BeginInvoke((Action)delegate ()
                            {
                                //应截减条数
                                if (SelfSystemLog.Count > Parameters.SysLogsTotal)
                                {
                                    for (int i = 0; i <= (SelfSystemLog.Count - (Parameters.SysLogsTotal - 1)); i++)
                                    {
                                        SelfSystemLog.RemoveAt(SelfSystemLog.Count - 1);
                                    }

                                    SelfSystemLog.Insert(
                                                            0,
                                                            new SystemLogsInformation()
                                                            {
                                                                DTime = DTime,
                                                                Object = Object,
                                                                Action = Action,
                                                                Other = Other
                                                            }
                                                        );
                                }
                                //正常
                                else if (SelfSystemLog.Count == Parameters.SysLogsTotal)
                                {
                                    SelfSystemLog.RemoveAt(SelfSystemLog.Count - 1);
                                    SelfSystemLog.Insert(
                                                            0,
                                                            new SystemLogsInformation()
                                                            {
                                                                DTime = DTime,
                                                                Object = Object,
                                                                Action = Action,
                                                                Other = Other
                                                            }
                                                        );
                                }
                            });
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("打印系统日志", ex.Message, ex.StackTrace);
            }
        }
        #endregion

        #region 主窗显示相关
        //设备列表窗口
        private void ShowDeviceListWindowControl()
        {
            Dispatcher.Invoke(() =>
            {
                DeviceListWindow.Visibility = Visibility.Visible;
                DeviceListWindowResizeLine.Visibility = Visibility.Collapsed;
                SystemLogsWindowResizeLine.Visibility = Visibility.Collapsed;
                SystemLogsWindow.Visibility = Visibility.Collapsed;
                UEInfoWindow.Visibility = Visibility.Collapsed;

                System.Windows.Controls.Grid.SetRow(DeviceListWindow, 1);
                System.Windows.Controls.Grid.SetColumn(DeviceListWindow, 0);
                System.Windows.Controls.Grid.SetRowSpan(DeviceListWindow, 3);
                System.Windows.Controls.Grid.SetColumnSpan(DeviceListWindow, 4);
            });
        }

        //捕号窗口
        private void ShowScannerWindowControl()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    DeviceListWindow.Visibility = Visibility.Collapsed;
                    DeviceListWindowResizeLine.Visibility = Visibility.Collapsed;
                    SystemLogsWindowResizeLine.Visibility = Visibility.Collapsed;
                    SystemLogsWindow.Visibility = Visibility.Collapsed;

                    UEInfoWindow.Visibility = Visibility.Visible;
                    System.Windows.Controls.Grid.SetRow(UEInfoWindow, 1);
                    System.Windows.Controls.Grid.SetColumn(UEInfoWindow, 0);
                    System.Windows.Controls.Grid.SetRowSpan(UEInfoWindow, 3);
                    System.Windows.Controls.Grid.SetColumnSpan(UEInfoWindow, 4);
                });

                Parameters.DataReportWindowsControl.ScannerWindowControl();
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("捕号窗口设置内部异常", Ex.Message, Ex.StackTrace);
            }
        }

        //测量报告窗口
        private void ShowMeasurementReportWindowControl()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    DeviceListWindow.Visibility = Visibility.Collapsed;
                    DeviceListWindowResizeLine.Visibility = Visibility.Collapsed;
                    SystemLogsWindowResizeLine.Visibility = Visibility.Collapsed;
                    SystemLogsWindow.Visibility = Visibility.Collapsed;

                    UEInfoWindow.Visibility = Visibility.Visible;
                    System.Windows.Controls.Grid.SetRow(UEInfoWindow, 1);
                    System.Windows.Controls.Grid.SetColumn(UEInfoWindow, 0);
                    System.Windows.Controls.Grid.SetRowSpan(UEInfoWindow, 3);
                    System.Windows.Controls.Grid.SetColumnSpan(UEInfoWindow, 4);
                });

                Parameters.DataReportWindowsControl.FunctionWindowsAreaControl(0, Visibility.Visible, Visibility.Collapsed, Visibility.Collapsed);
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("测量报告窗口", Ex.Message, Ex.StackTrace);
            }
        }

        //通话记录窗口
        private void ShowCallRecordsWindowControl()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    DeviceListWindow.Visibility = Visibility.Collapsed;
                    DeviceListWindowResizeLine.Visibility = Visibility.Collapsed;
                    SystemLogsWindowResizeLine.Visibility = Visibility.Collapsed;
                    SystemLogsWindow.Visibility = Visibility.Collapsed;
                    UEInfoWindow.Visibility = Visibility.Visible;

                    System.Windows.Controls.Grid.SetRow(UEInfoWindow, 1);
                    System.Windows.Controls.Grid.SetColumn(UEInfoWindow, 0);
                    System.Windows.Controls.Grid.SetRowSpan(UEInfoWindow, 3);
                    System.Windows.Controls.Grid.SetColumnSpan(UEInfoWindow, 4);
                });

                Parameters.DataReportWindowsControl.FunctionWindowsAreaControl(2, Visibility.Collapsed, Visibility.Visible, Visibility.Collapsed);
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("通话记录窗口", Ex.Message, Ex.StackTrace);
            }
        }

        //短信记录窗口
        private void ShowSMSRecordsWindowControl()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    DeviceListWindow.Visibility = Visibility.Collapsed;
                    DeviceListWindowResizeLine.Visibility = Visibility.Collapsed;
                    SystemLogsWindowResizeLine.Visibility = Visibility.Collapsed;
                    SystemLogsWindow.Visibility = Visibility.Collapsed;
                    UEInfoWindow.Visibility = Visibility.Visible;

                    System.Windows.Controls.Grid.SetRow(UEInfoWindow, 1);
                    System.Windows.Controls.Grid.SetColumn(UEInfoWindow, 0);
                    System.Windows.Controls.Grid.SetRowSpan(UEInfoWindow, 3);
                    System.Windows.Controls.Grid.SetColumnSpan(UEInfoWindow, 4);
                });

                Parameters.DataReportWindowsControl.FunctionWindowsAreaControl(3, Visibility.Collapsed, Visibility.Collapsed, Visibility.Visible);
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("短信记录窗口", Ex.Message, Ex.StackTrace);
            }
        }

        //系统日志状态窗口
        private void ShowSystemLogsInfoWindowControl()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    DeviceListWindow.Visibility = Visibility.Collapsed;
                    DeviceListWindowResizeLine.Visibility = Visibility.Collapsed;
                    SystemLogsWindowResizeLine.Visibility = Visibility.Collapsed;
                    UEInfoWindow.Visibility = Visibility.Collapsed;
                    SystemLogsWindow.Visibility = Visibility.Visible;

                    System.Windows.Controls.Grid.SetRow(SystemLogsWindow, 1);
                    System.Windows.Controls.Grid.SetColumn(SystemLogsWindow, 0);
                    System.Windows.Controls.Grid.SetRowSpan(SystemLogsWindow, 3);
                    System.Windows.Controls.Grid.SetColumnSpan(SystemLogsWindow, 4);
                });
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("显示系统日志状态窗口内部故障", Ex.Message, Ex.StackTrace);
            }
        }

        //窗口默认显示
        private void ShowDefaultWindowControl()
        {
            try
            {
                #region 框架数据
                Dispatcher.Invoke(() =>
                {
                    System.Windows.Controls.Grid.SetRow(DeviceListWindow, 1);
                    System.Windows.Controls.Grid.SetColumn(DeviceListWindow, 0);
                    System.Windows.Controls.Grid.SetRowSpan(DeviceListWindow, 1);
                    System.Windows.Controls.Grid.SetColumnSpan(DeviceListWindow, 1);

                    System.Windows.Controls.Grid.SetRow(UEInfoWindow, 1);
                    System.Windows.Controls.Grid.SetColumn(UEInfoWindow, 2);
                    System.Windows.Controls.Grid.SetRowSpan(UEInfoWindow, 1);
                    System.Windows.Controls.Grid.SetColumnSpan(UEInfoWindow, 2);

                    System.Windows.Controls.Grid.SetRow(DeviceListWindowResizeLine, 1);
                    System.Windows.Controls.Grid.SetColumn(DeviceListWindowResizeLine, 1);
                    System.Windows.Controls.Grid.SetRowSpan(DeviceListWindowResizeLine, 1);
                    System.Windows.Controls.Grid.SetColumnSpan(DeviceListWindowResizeLine, 1);

                    System.Windows.Controls.Grid.SetRow(SystemLogsWindowResizeLine, 2);
                    System.Windows.Controls.Grid.SetColumn(SystemLogsWindowResizeLine, 0);
                    System.Windows.Controls.Grid.SetRowSpan(SystemLogsWindowResizeLine, 1);
                    System.Windows.Controls.Grid.SetColumnSpan(SystemLogsWindowResizeLine, 4);

                    System.Windows.Controls.Grid.SetRow(SystemLogsWindow, 3);
                    System.Windows.Controls.Grid.SetColumn(SystemLogsWindow, 0);
                    System.Windows.Controls.Grid.SetRowSpan(SystemLogsWindow, 1);
                    System.Windows.Controls.Grid.SetColumnSpan(SystemLogsWindow, 4);

                    DeviceListWindow.Visibility = Visibility.Visible;
                    DeviceListWindowResizeLine.Visibility = Visibility.Visible;
                    SystemLogsWindowResizeLine.Visibility = Visibility.Visible;
                    UEInfoWindow.Visibility = Visibility.Visible;
                    SystemLogsWindow.Visibility = Visibility.Visible;
                });
                #endregion

                Parameters.DataReportWindowsControl.AllElementNormalShow();
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("窗口默认显示内部故障", Ex.Message, Ex.StackTrace);
            }
        }

        //设备列表窗口显示 + 捕号窗口显示
        private void ShowDeviceListWinAndScannerWin()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    SystemLogsWindowResizeLine.Visibility = Visibility.Collapsed;
                    SystemLogsWindow.Visibility = Visibility.Collapsed;

                    DeviceListWindow.Visibility = Visibility.Visible;
                    DeviceListWindowResizeLine.Visibility = Visibility.Visible;
                    UEInfoWindow.Visibility = Visibility.Visible;

                    System.Windows.Controls.Grid.SetRow(DeviceListWindow, 1);
                    System.Windows.Controls.Grid.SetColumn(DeviceListWindow, 0);
                    System.Windows.Controls.Grid.SetRowSpan(DeviceListWindow, 3);
                    System.Windows.Controls.Grid.SetColumnSpan(DeviceListWindow, 1);

                    System.Windows.Controls.Grid.SetRow(DeviceListWindowResizeLine, 1);
                    System.Windows.Controls.Grid.SetColumn(DeviceListWindowResizeLine, 1);
                    System.Windows.Controls.Grid.SetRowSpan(DeviceListWindowResizeLine, 3);
                    System.Windows.Controls.Grid.SetColumnSpan(DeviceListWindowResizeLine, 1);

                    System.Windows.Controls.Grid.SetRow(UEInfoWindow, 1);
                    System.Windows.Controls.Grid.SetColumn(UEInfoWindow, 2);
                    System.Windows.Controls.Grid.SetRowSpan(UEInfoWindow, 3);
                    System.Windows.Controls.Grid.SetColumnSpan(UEInfoWindow, 2);
                });

                Parameters.DataReportWindowsControl.ScannerWindowControl();
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("设备列表窗口与捕号窗口显示模式内部故障", Ex.Message, Ex.StackTrace);
            }
        }

        //设备列表窗口显示 + 黑名单窗口显示
        private void ShowDeviceListWinAndMeasurementReportWin()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    SystemLogsWindowResizeLine.Visibility = Visibility.Collapsed;
                    SystemLogsWindow.Visibility = Visibility.Collapsed;

                    DeviceListWindow.Visibility = Visibility.Visible;
                    DeviceListWindowResizeLine.Visibility = Visibility.Visible;
                    UEInfoWindow.Visibility = Visibility.Visible;

                    System.Windows.Controls.Grid.SetRow(DeviceListWindow, 1);
                    System.Windows.Controls.Grid.SetColumn(DeviceListWindow, 0);
                    System.Windows.Controls.Grid.SetRowSpan(DeviceListWindow, 3);
                    System.Windows.Controls.Grid.SetColumnSpan(DeviceListWindow, 1);

                    System.Windows.Controls.Grid.SetRow(DeviceListWindowResizeLine, 1);
                    System.Windows.Controls.Grid.SetColumn(DeviceListWindowResizeLine, 1);
                    System.Windows.Controls.Grid.SetRowSpan(DeviceListWindowResizeLine, 3);
                    System.Windows.Controls.Grid.SetColumnSpan(DeviceListWindowResizeLine, 1);

                    System.Windows.Controls.Grid.SetRow(UEInfoWindow, 1);
                    System.Windows.Controls.Grid.SetColumn(UEInfoWindow, 2);
                    System.Windows.Controls.Grid.SetRowSpan(UEInfoWindow, 3);
                    System.Windows.Controls.Grid.SetColumnSpan(UEInfoWindow, 2);
                });

                Parameters.DataReportWindowsControl.FunctionWindowsAreaControl(0, Visibility.Visible, Visibility.Visible, Visibility.Visible);
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("设备列表窗口与黑名单窗口显示模式内部故障", Ex.Message, Ex.StackTrace);
            }
        }

        //设备列表窗口显示 + 系统日志窗口显示
        private void ShowDeviceListWinAndSystemLogsWin()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    DeviceListWindow.Visibility = Visibility.Visible;
                    SystemLogsWindowResizeLine.Visibility = Visibility.Visible;
                    SystemLogsWindow.Visibility = Visibility.Visible;

                    DeviceListWindowResizeLine.Visibility = Visibility.Collapsed;
                    UEInfoWindow.Visibility = Visibility.Collapsed;

                    System.Windows.Controls.Grid.SetRow(DeviceListWindow, 1);
                    System.Windows.Controls.Grid.SetColumn(DeviceListWindow, 0);
                    System.Windows.Controls.Grid.SetRowSpan(DeviceListWindow, 1);
                    System.Windows.Controls.Grid.SetColumnSpan(DeviceListWindow, 4);

                    System.Windows.Controls.Grid.SetRow(SystemLogsWindowResizeLine, 2);
                    System.Windows.Controls.Grid.SetColumn(SystemLogsWindowResizeLine, 0);
                    System.Windows.Controls.Grid.SetRowSpan(SystemLogsWindowResizeLine, 1);
                    System.Windows.Controls.Grid.SetColumnSpan(SystemLogsWindowResizeLine, 4);

                    System.Windows.Controls.Grid.SetRow(SystemLogsWindow, 3);
                    System.Windows.Controls.Grid.SetColumn(SystemLogsWindow, 0);
                    System.Windows.Controls.Grid.SetRowSpan(SystemLogsWindow, 1);
                    System.Windows.Controls.Grid.SetColumnSpan(SystemLogsWindow, 4);
                });
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("设备列表窗口与系统日志窗口显示模式内部故障", Ex.Message, Ex.StackTrace);
            }
        }

        //捕号窗口显示 + 黑名单窗口显示
        private void ShowScannerWinAndMeasurementReportWin()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    DeviceListWindow.Visibility = Visibility.Collapsed;
                    DeviceListWindowResizeLine.Visibility = Visibility.Collapsed;
                    SystemLogsWindowResizeLine.Visibility = Visibility.Collapsed;
                    SystemLogsWindow.Visibility = Visibility.Collapsed;
                    UEInfoWindow.Visibility = Visibility.Visible;

                    System.Windows.Controls.Grid.SetRow(UEInfoWindow, 1);
                    System.Windows.Controls.Grid.SetColumn(UEInfoWindow, 0);
                    System.Windows.Controls.Grid.SetRowSpan(UEInfoWindow, 3);
                    System.Windows.Controls.Grid.SetColumnSpan(UEInfoWindow, 4);
                });

                Parameters.DataReportWindowsControl.AllElementNormalShow();
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("捕号窗口与黑名单窗口显示模式内部故障", Ex.Message, Ex.StackTrace);
            }
        }

        //捕号窗口显示 + 系统日志窗口显示
        private void ShowScannerWinAndSystemLogsWin()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    SystemLogsWindowResizeLine.Visibility = Visibility.Visible;
                    SystemLogsWindow.Visibility = Visibility.Visible;

                    DeviceListWindow.Visibility = Visibility.Collapsed;
                    DeviceListWindowResizeLine.Visibility = Visibility.Collapsed;
                    UEInfoWindow.Visibility = Visibility.Visible;

                    System.Windows.Controls.Grid.SetRow(UEInfoWindow, 1);
                    System.Windows.Controls.Grid.SetColumn(UEInfoWindow, 0);
                    System.Windows.Controls.Grid.SetRowSpan(UEInfoWindow, 1);
                    System.Windows.Controls.Grid.SetColumnSpan(UEInfoWindow, 4);

                    System.Windows.Controls.Grid.SetRow(SystemLogsWindowResizeLine, 2);
                    System.Windows.Controls.Grid.SetColumn(SystemLogsWindowResizeLine, 0);
                    System.Windows.Controls.Grid.SetRowSpan(SystemLogsWindowResizeLine, 1);
                    System.Windows.Controls.Grid.SetColumnSpan(SystemLogsWindowResizeLine, 4);

                    System.Windows.Controls.Grid.SetRow(SystemLogsWindow, 3);
                    System.Windows.Controls.Grid.SetColumn(SystemLogsWindow, 0);
                    System.Windows.Controls.Grid.SetRowSpan(SystemLogsWindow, 1);
                    System.Windows.Controls.Grid.SetColumnSpan(SystemLogsWindow, 4);
                });

                Parameters.DataReportWindowsControl.ScannerWindowControl();
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("捕号窗口与系统日志窗口显示模式内部故障", Ex.Message, Ex.StackTrace);
            }
        }

        //黑名单窗口显示 + 系统日志窗口显示
        private void ShowMeasurementReportWinAndSystemLogsWin()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    SystemLogsWindowResizeLine.Visibility = Visibility.Visible;
                    SystemLogsWindow.Visibility = Visibility.Visible;

                    DeviceListWindow.Visibility = Visibility.Collapsed;
                    DeviceListWindowResizeLine.Visibility = Visibility.Collapsed;
                    UEInfoWindow.Visibility = Visibility.Visible;

                    System.Windows.Controls.Grid.SetRow(UEInfoWindow, 1);
                    System.Windows.Controls.Grid.SetColumn(UEInfoWindow, 0);
                    System.Windows.Controls.Grid.SetRowSpan(UEInfoWindow, 1);
                    System.Windows.Controls.Grid.SetColumnSpan(UEInfoWindow, 4);

                    System.Windows.Controls.Grid.SetRow(SystemLogsWindowResizeLine, 2);
                    System.Windows.Controls.Grid.SetColumn(SystemLogsWindowResizeLine, 0);
                    System.Windows.Controls.Grid.SetRowSpan(SystemLogsWindowResizeLine, 1);
                    System.Windows.Controls.Grid.SetColumnSpan(SystemLogsWindowResizeLine, 4);

                    System.Windows.Controls.Grid.SetRow(SystemLogsWindow, 3);
                    System.Windows.Controls.Grid.SetColumn(SystemLogsWindow, 0);
                    System.Windows.Controls.Grid.SetRowSpan(SystemLogsWindow, 1);
                    System.Windows.Controls.Grid.SetColumnSpan(SystemLogsWindow, 4);
                });

                Parameters.DataReportWindowsControl.FunctionWindowsAreaControl(0, Visibility.Visible, Visibility.Visible, Visibility.Visible);
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("黑名单窗口与系统日志窗口显示模式内部故障", Ex.Message, Ex.StackTrace);
            }
        }

        //设备列表窗口显示 + 捕号窗口显示 + 黑名单窗口显示
        private void ShowDeviceListWinAndUEInfoWindow()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    SystemLogsWindowResizeLine.Visibility = Visibility.Collapsed;
                    SystemLogsWindow.Visibility = Visibility.Collapsed;

                    DeviceListWindow.Visibility = Visibility.Visible;
                    DeviceListWindowResizeLine.Visibility = Visibility.Visible;
                    UEInfoWindow.Visibility = Visibility.Visible;

                    System.Windows.Controls.Grid.SetRow(DeviceListWindow, 1);
                    System.Windows.Controls.Grid.SetColumn(DeviceListWindow, 0);
                    System.Windows.Controls.Grid.SetRowSpan(DeviceListWindow, 3);
                    System.Windows.Controls.Grid.SetColumnSpan(DeviceListWindow, 1);

                    System.Windows.Controls.Grid.SetRow(DeviceListWindowResizeLine, 1);
                    System.Windows.Controls.Grid.SetColumn(DeviceListWindowResizeLine, 1);
                    System.Windows.Controls.Grid.SetRowSpan(DeviceListWindowResizeLine, 3);
                    System.Windows.Controls.Grid.SetColumnSpan(DeviceListWindowResizeLine, 1);

                    System.Windows.Controls.Grid.SetRow(UEInfoWindow, 1);
                    System.Windows.Controls.Grid.SetColumn(UEInfoWindow, 2);
                    System.Windows.Controls.Grid.SetRowSpan(UEInfoWindow, 3);
                    System.Windows.Controls.Grid.SetColumnSpan(UEInfoWindow, 2);
                });

                Parameters.DataReportWindowsControl.AllElementNormalShow();
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("设备列表窗口 + 捕号窗口 + 黑名单窗口显示模式内部故障", Ex.Message, Ex.StackTrace);
            }
        }

        //设备列表窗口显示 + 捕号窗口显示 + 系统日志窗口显示
        private void ShowDeviceListWinAndScannerWinAndSystemLogsWin()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    System.Windows.Controls.Grid.SetRow(DeviceListWindow, 1);
                    System.Windows.Controls.Grid.SetColumn(DeviceListWindow, 0);
                    System.Windows.Controls.Grid.SetRowSpan(DeviceListWindow, 1);
                    System.Windows.Controls.Grid.SetColumnSpan(DeviceListWindow, 1);

                    System.Windows.Controls.Grid.SetRow(UEInfoWindow, 1);
                    System.Windows.Controls.Grid.SetColumn(UEInfoWindow, 2);
                    System.Windows.Controls.Grid.SetRowSpan(UEInfoWindow, 1);
                    System.Windows.Controls.Grid.SetColumnSpan(UEInfoWindow, 2);

                    System.Windows.Controls.Grid.SetRow(SystemLogsWindow, 3);
                    System.Windows.Controls.Grid.SetColumn(SystemLogsWindow, 0);
                    System.Windows.Controls.Grid.SetRowSpan(SystemLogsWindow, 1);
                    System.Windows.Controls.Grid.SetColumnSpan(SystemLogsWindow, 4);

                    System.Windows.Controls.Grid.SetRow(DeviceListWindowResizeLine, 1);
                    System.Windows.Controls.Grid.SetColumn(DeviceListWindowResizeLine, 1);
                    System.Windows.Controls.Grid.SetRowSpan(DeviceListWindowResizeLine, 1);
                    System.Windows.Controls.Grid.SetColumnSpan(DeviceListWindowResizeLine, 1);

                    System.Windows.Controls.Grid.SetRow(SystemLogsWindowResizeLine, 2);
                    System.Windows.Controls.Grid.SetColumn(SystemLogsWindowResizeLine, 0);
                    System.Windows.Controls.Grid.SetRowSpan(SystemLogsWindowResizeLine, 1);
                    System.Windows.Controls.Grid.SetColumnSpan(SystemLogsWindowResizeLine, 4);

                    DeviceListWindow.Visibility = Visibility.Visible;
                    DeviceListWindowResizeLine.Visibility = Visibility.Visible;
                    SystemLogsWindowResizeLine.Visibility = Visibility.Visible;
                    UEInfoWindow.Visibility = Visibility.Visible;
                    SystemLogsWindow.Visibility = Visibility.Visible;
                });

                Parameters.DataReportWindowsControl.ScannerWindowControl();
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("设备列表窗口 + 捕号窗口 + 系统日志窗口显示模式内部故障", Ex.Message, Ex.StackTrace);
            }
        }

        //设备列表窗口显示 + 黑名单窗口显示 + 系统日志窗口显示
        private void ShowDeviceListWinAndMeasurementReportWinAndSystemLogsWin()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    System.Windows.Controls.Grid.SetRow(DeviceListWindow, 1);
                    System.Windows.Controls.Grid.SetColumn(DeviceListWindow, 0);
                    System.Windows.Controls.Grid.SetRowSpan(DeviceListWindow, 1);
                    System.Windows.Controls.Grid.SetColumnSpan(DeviceListWindow, 1);

                    System.Windows.Controls.Grid.SetRow(UEInfoWindow, 1);
                    System.Windows.Controls.Grid.SetColumn(UEInfoWindow, 2);
                    System.Windows.Controls.Grid.SetRowSpan(UEInfoWindow, 1);
                    System.Windows.Controls.Grid.SetColumnSpan(UEInfoWindow, 2);

                    System.Windows.Controls.Grid.SetRow(SystemLogsWindow, 3);
                    System.Windows.Controls.Grid.SetColumn(SystemLogsWindow, 0);
                    System.Windows.Controls.Grid.SetRowSpan(SystemLogsWindow, 1);
                    System.Windows.Controls.Grid.SetColumnSpan(SystemLogsWindow, 4);

                    System.Windows.Controls.Grid.SetRow(DeviceListWindowResizeLine, 1);
                    System.Windows.Controls.Grid.SetColumn(DeviceListWindowResizeLine, 1);
                    System.Windows.Controls.Grid.SetRowSpan(DeviceListWindowResizeLine, 1);
                    System.Windows.Controls.Grid.SetColumnSpan(DeviceListWindowResizeLine, 1);

                    System.Windows.Controls.Grid.SetRow(SystemLogsWindowResizeLine, 2);
                    System.Windows.Controls.Grid.SetColumn(SystemLogsWindowResizeLine, 0);
                    System.Windows.Controls.Grid.SetRowSpan(SystemLogsWindowResizeLine, 1);
                    System.Windows.Controls.Grid.SetColumnSpan(SystemLogsWindowResizeLine, 4);

                    DeviceListWindow.Visibility = Visibility.Visible;
                    DeviceListWindowResizeLine.Visibility = Visibility.Visible;
                    SystemLogsWindowResizeLine.Visibility = Visibility.Visible;
                    UEInfoWindow.Visibility = Visibility.Visible;
                    SystemLogsWindow.Visibility = Visibility.Visible;
                });

                Parameters.DataReportWindowsControl.FunctionWindowsAreaControl(0, Visibility.Visible, Visibility.Visible, Visibility.Visible);
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("设备列表窗口 + 黑名单窗口 + 系统日志窗口显示模式内部故障", Ex.Message, Ex.StackTrace);
            }
        }

        //捕号窗口显示 + 黑名单窗口显示 + 系统日志窗口显示
        private void ShowScannerWinAndMeasurementReportWinAndSystemLogsWin()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    DeviceListWindow.Visibility = Visibility.Collapsed;
                    DeviceListWindowResizeLine.Visibility = Visibility.Collapsed;

                    SystemLogsWindowResizeLine.Visibility = Visibility.Visible;
                    SystemLogsWindow.Visibility = Visibility.Visible;
                    UEInfoWindow.Visibility = Visibility.Visible;

                    System.Windows.Controls.Grid.SetRow(UEInfoWindow, 1);
                    System.Windows.Controls.Grid.SetColumn(UEInfoWindow, 0);
                    System.Windows.Controls.Grid.SetRowSpan(UEInfoWindow, 1);
                    System.Windows.Controls.Grid.SetColumnSpan(UEInfoWindow, 4);

                    System.Windows.Controls.Grid.SetRow(SystemLogsWindowResizeLine, 2);
                    System.Windows.Controls.Grid.SetColumn(SystemLogsWindowResizeLine, 0);
                    System.Windows.Controls.Grid.SetRowSpan(SystemLogsWindowResizeLine, 1);
                    System.Windows.Controls.Grid.SetColumnSpan(SystemLogsWindowResizeLine, 4);

                    System.Windows.Controls.Grid.SetRow(SystemLogsWindow, 3);
                    System.Windows.Controls.Grid.SetColumn(SystemLogsWindow, 0);
                    System.Windows.Controls.Grid.SetRowSpan(SystemLogsWindow, 1);
                    System.Windows.Controls.Grid.SetColumnSpan(SystemLogsWindow, 4);
                });

                Parameters.DataReportWindowsControl.AllElementNormalShow();
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("捕号窗口 + 黑名单窗口 + 系统日志窗口显示模式内部故障", Ex.Message, Ex.StackTrace);
            }
        }

        #region 主窗口按配置显示
        private void MainWindowsConfiguration()
        {
            int Value = Parameters.MainWinControlParameter.GettingStatu();
            switch (Value)
            {
                case 1:
                    #region 全部窗口显示
                    ShowDefaultWindowControl();
                    #endregion
                    break;

                case 2:
                    #region 设备列表窗口显示
                    ShowDeviceListWindowControl();
                    #endregion
                    break;

                case 3:
                    #region 捕号窗口显示
                    ShowScannerWindowControl();
                    #endregion
                    break;

                case 4:
                    #region 黑名单窗口显示
                    ShowMeasurementReportWindowControl();
                    #endregion
                    break;

                case 5:
                    #region 系统日志窗口显示
                    ShowSystemLogsInfoWindowControl();
                    #endregion
                    break;

                case 6:
                    #region 设备列表窗口显示 + 捕号窗口显示
                    ShowDeviceListWinAndScannerWin();
                    #endregion
                    break;

                case 7:
                    #region 设备列表窗口显示 + 黑名单窗口显示
                    ShowDeviceListWinAndMeasurementReportWin();
                    #endregion
                    break;

                case 8:
                    #region 设备列表窗口显示 + 系统日志窗口显示
                    ShowDeviceListWinAndSystemLogsWin();
                    #endregion
                    break;

                case 9:
                    #region 捕号窗口显示 + 黑名单窗口显示
                    ShowScannerWinAndMeasurementReportWin();
                    #endregion
                    break;

                case 10:
                    #region 捕号窗口显示 + 系统日志窗口显示
                    ShowScannerWinAndSystemLogsWin();
                    #endregion
                    break;

                case 11:
                    #region 黑名单窗口显示 + 系统日志窗口显示
                    ShowMeasurementReportWinAndSystemLogsWin();
                    #endregion
                    break;

                case 12:
                    #region 设备列表窗口显示 + 捕号窗口显示 + 黑名单窗口显示
                    ShowDeviceListWinAndUEInfoWindow();
                    #endregion
                    break;

                case 13:
                    #region 设备列表窗口显示 + 捕号窗口显示 + 系统日志窗口显示
                    ShowDeviceListWinAndScannerWinAndSystemLogsWin();
                    #endregion
                    break;

                case 14:
                    #region 设备列表窗口显示 + 黑名单窗口显示 + 系统日志窗口显示
                    ShowDeviceListWinAndMeasurementReportWinAndSystemLogsWin();
                    #endregion
                    break;

                case 15:
                    #region  捕号窗口显示 + 黑名单窗口显示 + 系统日志窗口显示
                    ShowScannerWinAndMeasurementReportWinAndSystemLogsWin();
                    #endregion
                    break;

                default:

                    break;
            }
        }
        #endregion

        #endregion

        private void lblDeviceManage_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Parameters.ConfigType = DeviceType.UnknownType;
            NavigatePages.DeviceManagerWindow DeviceManagerFrom = new NavigatePages.DeviceManagerWindow();
            DeviceManagerFrom.ShowDialog();
        }

        private void lblNameListManage_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                NavigatePages.NameListManage NameListManageFrom = new NavigatePages.NameListManage();
                NameListManageFrom.LoadDeviceListTreeView();
                #region 权限 
                if (RoleTypeClass.RoleType != null && RoleTypeClass.RoleType != "")
                {
                    if (RoleTypeClass.RoleType.Equals("RoleType"))
                    {

                    }
                    else
                    {
                        if (Parameters.ISDigital(RoleTypeClass.RoleType))
                        {
                            if (int.Parse(RoleTypeClass.RoleType) > 3)
                            {
                                //黑白名单
                                NameListManageFrom.btnBlackAdd.Visibility = System.Windows.Visibility.Collapsed;
                                NameListManageFrom.btnBlackEdit.Visibility = System.Windows.Visibility.Collapsed;
                                NameListManageFrom.btnBlackDelete.Visibility = System.Windows.Visibility.Collapsed;
                                NameListManageFrom.btnWhiteAdd.Visibility = System.Windows.Visibility.Collapsed;
                                NameListManageFrom.btnWhiteEdit.Visibility = System.Windows.Visibility.Collapsed;
                                NameListManageFrom.btnWhiteDelete.Visibility = System.Windows.Visibility.Collapsed;
                            }
                        }
                    }
                }
                #endregion
                NameListManageFrom.ShowDialog();
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void lblMapManage_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SubWindow.ShowDBMapWindow ShowMapViewWin = new SubWindow.ShowDBMapWindow();
            ShowMapViewWin.LoadDeviceListTreeView();
            ShowMapViewWin.ShowDialog();
        }

        private void lblStatisticalManage_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SubWindow.StatisticalAnalysisWindow StatisticalAnalysisViewWin = new SubWindow.StatisticalAnalysisWindow();
            StatisticalAnalysisViewWin.LoadDeviceListTreeView();
            StatisticalAnalysisViewWin.ShowDialog();
        }

        private void lblDomainManage_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            NavigatePages.DomainManage ManageFieldFrm = new NavigatePages.DomainManage();
            ManageFieldFrm.ShowDialog();
        }

        private void lblDataManage_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            NavigatePages.DataManage DataManageFrm = new NavigatePages.DataManage();
            DataManageFrm.ShowDialog();
        }

        private void lblConfiguration_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SubWindow.BaseParameterSettingWindow BaseParameterSettingWin = new SubWindow.BaseParameterSettingWindow();
            BaseParameterSettingWin.Show();
        }
    }
}
