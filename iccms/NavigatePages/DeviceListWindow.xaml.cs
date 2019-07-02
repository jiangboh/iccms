using DataInterface;
using NetController;
using ParameterControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace iccms.NavigatePages
{
    #region 数据上下文类型
    /// <summary>
    /// 数据上下文类型
    /// </summary>
    public class TreeViewItems
    {
        private DataColumn selfID;
        private DataColumn selfName;
        private DataColumn parentID;

        public DataColumn SelfID
        {
            get
            {
                return selfID;
            }

            set
            {
                selfID = value;
            }
        }

        public DataColumn SelfName
        {
            get
            {
                return selfName;
            }

            set
            {
                selfName = value;
            }
        }

        public DataColumn ParentID
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

        public TreeViewItems()
        {
            SelfID = new DataColumn("SelfID", System.Type.GetType("System.Int32"));
            SelfName = new DataColumn("SelfName", System.Type.GetType("System.String"));
            ParentID = new DataColumn("ParentID", System.Type.GetType("System.Int32"));
        }
    }
    #endregion

    #region 批量重启动设备
    public class BatchRebootAP : INotifyPropertyChanged
    {
        private string _stationFullName;
        private List<CheckBoxTreeModel> _deviceNameList = new List<CheckBoxTreeModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        public string StationFullName
        {
            get
            {
                return _stationFullName;
            }

            set
            {
                _stationFullName = value;
                NotifyPropertyChanged("StationFullName");
            }
        }

        public List<CheckBoxTreeModel> DeviceNameList
        {
            get
            {
                return _deviceNameList;
            }

            set
            {
                _deviceNameList = value;
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
    #endregion

    /// <summary>
    /// DeviceListWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceListWindow : Page
    {
        private object TreeViewLanguageClass = null;
        private object TreeViewLanguageContentMenuClass = null;
        private static System.Timers.Timer allDeviceDitailRequestTimer = null;
        private static System.Timers.Timer loadDeviceListTreeViewTimer = null;
        private static InputWindow inputWin = null;
        private Thread DeviceUnknownAlertColorControl = null;

        private TreeViewItem OldTreeViewItem = null;
        System.Windows.Input.MouseButtonEventArgs eButton = null;
        private static string Model = string.Empty;
        private string SelfID = string.Empty;
        private static string ParentID = string.Empty;
        private string SelfName = string.Empty;
        private static string FullName = string.Empty;
        public bool IsOnline = false;
        private static string IsStation = string.Empty;
        private string ApVersion = string.Empty;
        private string Des = string.Empty;
        public static BatchRebootAP BatchRebootAPList = new BatchRebootAP();

        //激活去激活请求的载波
        public static List<string> CarrierList = null;

        private static System.Timers.Timer apActiveResponseTimer = null;
        private static System.Timers.Timer apUnActiveResponseTimer = null;

        private static System.Timers.Timer gSMActiveEventsTimer = null;
        private static System.Timers.Timer gSMUnActiveEventsTimer = null;

        public static System.Timers.Timer LoadDeviceListTreeViewTimer
        {
            get
            {
                return loadDeviceListTreeViewTimer;
            }

            set
            {
                loadDeviceListTreeViewTimer = value;
            }
        }

        public static System.Timers.Timer AllDeviceDitailRequestTimer
        {
            get
            {
                return allDeviceDitailRequestTimer;
            }

            set
            {
                allDeviceDitailRequestTimer = value;
            }
        }

        public static System.Timers.Timer ApActiveResponseTimer
        {
            get
            {
                return apActiveResponseTimer;
            }

            set
            {
                apActiveResponseTimer = value;
            }
        }

        public static System.Timers.Timer ApUnActiveResponseTimer
        {
            get
            {
                return apUnActiveResponseTimer;
            }

            set
            {
                apUnActiveResponseTimer = value;
            }
        }

        public static System.Timers.Timer GSMActiveEventsTimer
        {
            get
            {
                return gSMActiveEventsTimer;
            }

            set
            {
                gSMActiveEventsTimer = value;
            }
        }

        public static System.Timers.Timer GSMUnActiveEventsTimer
        {
            get
            {
                return gSMUnActiveEventsTimer;
            }

            set
            {
                gSMUnActiveEventsTimer = value;
            }
        }

        #region 构造
        public DeviceListWindow()
        {
            InitializeComponent();

            if (Parameters.LanguageType.Equals("CN") || Parameters.LanguageType.Equals(""))
            {
                TreeViewLanguageClass = new Language_CN.DeviceTree.TreeViewContent();
                TreeViewLanguageContentMenuClass = new Language_CN.DeviceTree.DeviceTree_ContentMenu();
            }
            else if (Parameters.LanguageType.Equals("EN"))
            {
                TreeViewLanguageClass = new Language_EN.DeviceTree.TreeViewContent();
                TreeViewLanguageContentMenuClass = new Language_EN.DeviceTree.DeviceTree_ContentMenu();
            }

            if (LoadDeviceListTreeViewTimer == null)
            {
                LoadDeviceListTreeViewTimer = new System.Timers.Timer();
                LoadDeviceListTreeViewTimer.Interval = 1;
                LoadDeviceListTreeViewTimer.AutoReset = false;
                LoadDeviceListTreeViewTimer.Elapsed += LoadDeviceListTreeViewTimer_Elapsed;
            }

            if (AllDeviceDitailRequestTimer == null)
            {
                AllDeviceDitailRequestTimer = new System.Timers.Timer();
                AllDeviceDitailRequestTimer.Interval = 1;
                AllDeviceDitailRequestTimer.AutoReset = false;
                AllDeviceDitailRequestTimer.Elapsed += AllDeviceDitailRequestTimer_Elapsed;
            }

            if (ApActiveResponseTimer == null)
            {
                ApActiveResponseTimer = new System.Timers.Timer();
                ApActiveResponseTimer.AutoReset = false;
                ApActiveResponseTimer.Interval = 1;
                ApActiveResponseTimer.Elapsed += ApActiveResponseTimer_Elapsed;
            }

            if (ApUnActiveResponseTimer == null)
            {
                ApUnActiveResponseTimer = new System.Timers.Timer();
                ApUnActiveResponseTimer.AutoReset = false;
                ApUnActiveResponseTimer.Interval = 1;
                ApUnActiveResponseTimer.Elapsed += ApUnActiveResponseTimer_Elapsed;
            }

            if (GSMActiveEventsTimer == null)
            {
                GSMActiveEventsTimer = new System.Timers.Timer();
                GSMActiveEventsTimer.AutoReset = false;
                GSMActiveEventsTimer.Interval = 1;
                GSMActiveEventsTimer.Elapsed += GSMActiveEventsTimer_Elapsed;
            }

            if (GSMUnActiveEventsTimer == null)
            {
                GSMUnActiveEventsTimer = new System.Timers.Timer();
                GSMUnActiveEventsTimer.AutoReset = false;
                GSMUnActiveEventsTimer.Interval = 1;
                GSMUnActiveEventsTimer.Elapsed += GSMUnActiveEventsTimer_Elapsed;
            }

            if (DeviceUnknownAlertColorControl == null)
            {
                DeviceUnknownAlertColorControl = new Thread(new ThreadStart(UnknownDeviceAlertColor));
                DeviceUnknownAlertColorControl.Start();
            }

            if (CarrierList == null)
            {
                CarrierList = new List<string>();
            }
        }

        private void GSMUnActiveEventsTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                GSMActiveEventStart("0");
            });
        }

        private void GSMActiveEventsTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                GSMActiveEventStart("1");
            });

        }

        private void ApUnActiveResponseTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            APUnActiveResponse();
        }

        private void ApActiveResponseTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            APActiveResponse();
        }

        private void AllDeviceDitailRequestTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            new Thread(() => { AutoGetAllDeviceDitailRequest(); }).Start();
        }

        private void LoadDeviceListTreeViewTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            new Thread(() => { LoadDeviceListTreeView(); }).Start();
        }
        #endregion

        #region 未知设备提示器样式与显示
        /// <summary>
        /// 未知设备提示样式
        /// </summary>
        private void UnknownDeviceAlertColor()
        {
            double ballSize = 10;
            while (true)
            {
                try
                {
                    if (Parameters.UnknownDeviceWindowControlParameters.UnknownDeviceTipsBlockAttribute == Visibility.Visible)
                    {
                        ballSize = 10 * Parameters.UnknownDeviceWindowControlParameters.ElementScaleCoefficient;
                        if (SubWindow.UnKnownDeviceListsControlWindow.ShowUnKnownDeviceLists.Count > 0)
                        {
                            Parameters.UnknownDeviceWindowControlParameters.BackGroundTincture = "LightPink";
                            Parameters.UnknownDeviceWindowControlParameters.ToolTipContent = "系统有新的未知设备...";

                            Parameters.UnknownDeviceWindowControlParameters.BackGroundColor = "#FF831111";
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5SHide = Visibility.Collapsed;
                            ballSize += 1.6;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueX = ballSize;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueY = ballSize;
                            Thread.Sleep(200);

                            Parameters.UnknownDeviceWindowControlParameters.BackGroundColor = "#FF911313";
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5SHide = Visibility.Collapsed;
                            ballSize += 1.6;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueX = ballSize;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueY = ballSize;
                            Thread.Sleep(200);

                            Parameters.UnknownDeviceWindowControlParameters.BackGroundColor = "#FFA11515";
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5SHide = Visibility.Collapsed;
                            ballSize += 1.6;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueX = ballSize;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueY = ballSize;
                            Thread.Sleep(200);

                            Parameters.UnknownDeviceWindowControlParameters.BackGroundColor = "#FFB31717";
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5SHide = Visibility.Collapsed;
                            ballSize += 1.6;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueX = ballSize;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueY = ballSize;
                            Thread.Sleep(200);

                            Parameters.UnknownDeviceWindowControlParameters.BackGroundColor = "#FFC71919";
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5SHide = Visibility.Collapsed;
                            ballSize += 1.6;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueX = ballSize;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueY = ballSize;
                            Thread.Sleep(200);
                            Parameters.UnknownDeviceWindowControlParameters.BackGroundColor = "#FFDB1B1B";
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5SHide = Visibility.Visible;
                            ballSize += 1.6;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueX = ballSize;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueY = ballSize;
                            Thread.Sleep(200);

                            Parameters.UnknownDeviceWindowControlParameters.BackGroundColor = "#FFEF1D1D";
                            Thread.Sleep(1000);

                            Parameters.UnknownDeviceWindowControlParameters.BackGroundColor = "#FFDB1B1B";
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0SHide = Visibility.Visible;
                            ballSize -= 1.6;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueX = ballSize;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueY = ballSize;
                            Thread.Sleep(200);

                            Parameters.UnknownDeviceWindowControlParameters.BackGroundColor = "#FFC71919";
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0SHide = Visibility.Visible;
                            ballSize -= 1.6;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueX = ballSize;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueY = ballSize;
                            Thread.Sleep(200);

                            Parameters.UnknownDeviceWindowControlParameters.BackGroundColor = "#FFB31717";
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0SHide = Visibility.Visible;
                            ballSize -= 1.6;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueX = ballSize;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueY = ballSize;
                            Thread.Sleep(200);

                            Parameters.UnknownDeviceWindowControlParameters.BackGroundColor = "#FFA11515";
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0SHide = Visibility.Visible;
                            ballSize -= 1.6;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueX = ballSize;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueY = ballSize;
                            Thread.Sleep(200);

                            Parameters.UnknownDeviceWindowControlParameters.BackGroundColor = "#FF911313";
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0SHide = Visibility.Visible;
                            ballSize -= 1.6;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueX = ballSize;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueY = ballSize;
                            Thread.Sleep(200);

                            Parameters.UnknownDeviceWindowControlParameters.BackGroundColor = "#FF831111";
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0SHide = Visibility.Visible;
                            ballSize -= 1.6;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueX = ballSize;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueY = ballSize;
                            Thread.Sleep(200);
                        }
                        else if (SubWindow.UnKnownDeviceListsControlWindow.ShowUnKnownDeviceLists.Count <= 0)
                        {
                            Parameters.UnknownDeviceWindowControlParameters.BackGroundTincture = "#6AFF6A";
                            Parameters.UnknownDeviceWindowControlParameters.ToolTipContent = "系统无新的未知设备...";

                            Parameters.UnknownDeviceWindowControlParameters.BackGroundColor = "Green";
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5SHide = Visibility.Collapsed;
                            ballSize += 1.6;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueX = ballSize;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueY = ballSize;
                            Thread.Sleep(200);

                            Parameters.UnknownDeviceWindowControlParameters.BackGroundColor = "Green";
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5SHide = Visibility.Collapsed;
                            ballSize += 1.6;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueX = ballSize;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueY = ballSize;
                            Thread.Sleep(200);

                            Parameters.UnknownDeviceWindowControlParameters.BackGroundColor = "Green";
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5SHide = Visibility.Collapsed;
                            ballSize += 1.6;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueX = ballSize;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueY = ballSize;
                            Thread.Sleep(200);

                            Parameters.UnknownDeviceWindowControlParameters.BackGroundColor = "Green";
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5SHide = Visibility.Collapsed;
                            ballSize += 1.6;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueX = ballSize;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueY = ballSize;
                            Thread.Sleep(200);

                            Parameters.UnknownDeviceWindowControlParameters.BackGroundColor = "Green";
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5SHide = Visibility.Collapsed;
                            ballSize += 1.6;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueX = ballSize;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueY = ballSize;
                            Thread.Sleep(200);

                            Parameters.UnknownDeviceWindowControlParameters.BackGroundColor = "Green";
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5SHide = Visibility.Visible;
                            ballSize += 1.6;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueX = ballSize;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueY = ballSize;
                            Thread.Sleep(200);

                            Parameters.UnknownDeviceWindowControlParameters.BackGroundColor = "Green";
                            Thread.Sleep(1000);

                            Parameters.UnknownDeviceWindowControlParameters.BackGroundColor = "Green";
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0SHide = Visibility.Visible;
                            ballSize -= 1.6;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueX = ballSize;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueY = ballSize;
                            Thread.Sleep(200);

                            Parameters.UnknownDeviceWindowControlParameters.BackGroundColor = "Green";
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0SHide = Visibility.Visible;
                            ballSize -= 1.6;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueX = ballSize;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueY = ballSize;
                            Thread.Sleep(200);

                            Parameters.UnknownDeviceWindowControlParameters.BackGroundColor = "Green";
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0SHide = Visibility.Visible;
                            ballSize -= 1.6;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueX = ballSize;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueY = ballSize;
                            Thread.Sleep(200);

                            Parameters.UnknownDeviceWindowControlParameters.BackGroundColor = "Green";
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0SHide = Visibility.Visible;
                            ballSize -= 1.6;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueX = ballSize;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueY = ballSize;
                            Thread.Sleep(200);

                            Parameters.UnknownDeviceWindowControlParameters.BackGroundColor = "Green";
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1SHide = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0SHide = Visibility.Visible;
                            ballSize -= 1.6;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueX = ballSize;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueY = ballSize;
                            Thread.Sleep(200);

                            Parameters.UnknownDeviceWindowControlParameters.BackGroundColor = "Green";
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine5SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine4SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine3SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine2SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1Show = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine1SHide = Visibility.Collapsed;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0Show = Visibility.Visible;
                            Parameters.UnknownDeviceWindowControlParameters.SignleLine0SHide = Visibility.Visible;
                            ballSize -= 1.6;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueX = ballSize;
                            Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueY = ballSize;
                            Thread.Sleep(200);
                        }
                    }
                    else
                    {
                        Parameters.UnknownDeviceWindowControlParameters.BackGroundTincture = "#6AFF6A";
                        Parameters.UnknownDeviceWindowControlParameters.BackGroundColor = "Green";
                        Parameters.UnknownDeviceWindowControlParameters.ToolTipContent = "系统无新的未知设备...";
                        ballSize = 20 * Parameters.UnknownDeviceWindowControlParameters.ElementScaleCoefficient;
                        Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueX = ballSize;
                        Parameters.UnknownDeviceWindowControlParameters.EllipseRadiueY = ballSize;
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception Ex)
                {
                    Parameters.PrintfLogsExtended("未知设备提示器参数配置内部故障", Ex.Message, Ex.StackTrace);
                    Thread.Sleep(1000);
                }
            }
        }
        #endregion

        /// <summary>
        /// 自动获取所有设备详细参数请求
        /// </summary>
        private void AutoGetAllDeviceDitailRequest()
        {
            int DelayTime = 5;
            string Tips = string.Empty;
            System.Timers.Timer WaitResultTimer = new System.Timers.Timer();
            WaitResultTimer.AutoReset = true;
            WaitResultTimer.Interval = 3000;
            WaitResultTimer.Elapsed += WaitResultTimer_Elapsed;

            //非GSM设备列表
            List<string> StationLists = new List<string>();
            //GSM设备列表
            List<string> GSMDeviceLists = new List<string>();

            //统计AP
            lock (Parameters.UseObject)
            {
                for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                {
                    if (JsonInterFace.APATTributesLists[i].Mode != DeviceType.GSM && JsonInterFace.APATTributesLists[i].Mode != DeviceType.GSMV2)
                    {
                        if (JsonInterFace.APATTributesLists[i].FullName != "" && JsonInterFace.APATTributesLists[i].FullName != null)
                        {
                            StationLists.Add(JsonInterFace.APATTributesLists[i].FullName);
                        }
                    }
                    else
                    {
                        if (JsonInterFace.APATTributesLists[i].FullName != "" && JsonInterFace.APATTributesLists[i].FullName != null)
                        {
                            GSMDeviceLists.Add(JsonInterFace.APATTributesLists[i].FullName);
                        }
                    }
                }
            }

            JsonInterFace.ActionResultStatus.APCount = StationLists.Count + GSMDeviceLists.Count;
            Parameters.UniversalCounter = 1;
            JsonInterFace.DeviceListRequestCompleteStatus.MaxLoading = StationLists.Count - 1;
            JsonInterFace.DeviceListRequestCompleteStatus.ValueLoading = 0;
            string DomainFullPathName = string.Empty;
            string DeviceName = string.Empty;

            //打开显示日志
            JsonInterFace.LoadedFinish = true;

            #region 非GSM设备详细信息请求
            for (int i = 0; i < StationLists.Count; i++)
            {
                DelayTime = 5;
                while (true)
                {
                    if (JsonInterFace.ActionResultStatus.Finished)
                    {
                        WaitResultTimer.Stop();
                        lock (JsonInterFace.ActionResultStatus.FinishedLock)
                        {
                            JsonInterFace.ActionResultStatus.Finished = false;
                        }
                        JsonInterFace.ActionResultStatus.NoResultErrorDeviceName = false;

                        if (i > 0)
                        {
                            JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(),
                                "获取设备[" + DomainFullPathName + "." + DeviceName + "]详细信息"
                                + JsonInterFace.ActionResultStatus.ResoultStatus + " ------ [" + Parameters.UniversalCounter.ToString() + "/"
                                + JsonInterFace.ActionResultStatus.APCount.ToString() + "]", "获取设备详细信息",
                                JsonInterFace.ActionResultStatus.ResoultStatus);

                            Parameters.UniversalCounter++;
                            JsonInterFace.DeviceListRequestCompleteStatus.ValueLoading++;
                        }

                        break;
                    }
                    //超时
                    else if (JsonInterFace.ActionResultStatus.NoResultErrorDeviceName)
                    {
                        WaitResultTimer.Stop();
                        lock (JsonInterFace.ActionResultStatus.FinishedLock)
                        {
                            JsonInterFace.ActionResultStatus.Finished = false;
                        }

                        JsonInterFace.ActionResultStatus.NoResultErrorDeviceName = false;

                        JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(),
                            "获取设备[" + DomainFullPathName + "." + DeviceName + "]详细信息"
                            + "超时" + " ------ [" + Parameters.UniversalCounter.ToString() + "/"
                            + JsonInterFace.ActionResultStatus.APCount.ToString() + "]", "获取设备详细信息",
                            "超时");

                        Parameters.UniversalCounter++;
                        JsonInterFace.DeviceListRequestCompleteStatus.ValueLoading++;

                        break;
                    }
                    else
                    {
                        Thread.Sleep(DelayTime += 5);
                    }
                }

                if (i >= StationLists.Count) { break; }

                WaitResultTimer.Start();

                DomainFullPathName = string.Empty;
                string[] DeviceFullNameField = StationLists[i].Split(new char[] { '.' });
                for (int j = 0; j < DeviceFullNameField.Length - 1; j++)
                {
                    if (DomainFullPathName == "" || DomainFullPathName == null)
                    {
                        DomainFullPathName = DeviceFullNameField[j];
                    }
                    else
                    {
                        DomainFullPathName += "." + DeviceFullNameField[j];
                    }
                }

                DeviceName = DeviceFullNameField[DeviceFullNameField.Length - 1];

                if (NetWorkClient.ControllerServer.Connected)
                {
                    Parameters.ConfigType = "LoadedDevice";
                    if (DomainFullPathName != "" && DomainFullPathName != null)
                    {
                        JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "自动获取[" + DomainFullPathName + "." + DeviceName + "]详细信息开始...", "获取设备详细信息", "正在通讯...");
                        NetWorkClient.ControllerServer.Send(JsonInterFace.GetDeviceDetailRequest(DomainFullPathName, DeviceName));
                    }
                }
                else
                {
                    JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "自动获取[" + DomainFullPathName + "." + DeviceName + "]详细信息开始...", "客户端与服务器网络断开", "无网络连接...");
                }
            }
            #endregion

            #region GSM设备详细信息请求
            if (GSMDeviceLists.Count > 0)
            {
                JsonInterFace.DeviceListRequestCompleteStatus.MaxLoading = GSMDeviceLists.Count - 1;
                JsonInterFace.DeviceListRequestCompleteStatus.ValueLoading = 0;
                for (int i = 0; i <= GSMDeviceLists.Count; i++)
                {
                    DelayTime = 5;
                    while (true)
                    {
                        if (JsonInterFace.ActionResultStatus.Finished)
                        {
                            WaitResultTimer.Stop();
                            lock (JsonInterFace.ActionResultStatus.FinishedLock)
                            {
                                JsonInterFace.ActionResultStatus.Finished = false;
                            }

                            JsonInterFace.ActionResultStatus.NoResultErrorDeviceName = false;

                            if (i > 0)
                            {
                                Parameters.UniversalCounter++;
                                JsonInterFace.DeviceListRequestCompleteStatus.ValueLoading++;

                                JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(),
                                    "获取设备[" + DomainFullPathName + "." + DeviceName + "]载波(" + JsonInterFace.ActionResultStatus.Carrier + ")通用参数"
                                    + JsonInterFace.ActionResultStatus.ResoultStatus
                                    + " ------ [" + Parameters.UniversalCounter.ToString() + "/"
                                    + JsonInterFace.ActionResultStatus.APCount.ToString() + "]", "获取载波(" + JsonInterFace.ActionResultStatus.Carrier + ")参数",
                                    JsonInterFace.ActionResultStatus.ResoultStatus);
                            }
                            break;
                        }
                        //超时
                        else if (JsonInterFace.ActionResultStatus.NoResultErrorDeviceName)
                        {
                            WaitResultTimer.Stop();

                            lock (JsonInterFace.ActionResultStatus.FinishedLock)
                            {
                                JsonInterFace.ActionResultStatus.Finished = false;
                            }

                            JsonInterFace.ActionResultStatus.NoResultErrorDeviceName = false;

                            Parameters.UniversalCounter++;
                            JsonInterFace.DeviceListRequestCompleteStatus.ValueLoading++;

                            JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(),
                                "获取设备[" + DomainFullPathName + "." + DeviceName + "]载波(" + JsonInterFace.ActionResultStatus.Carrier + ")通用参数"
                                + "超时"
                                + " ------ [" + Parameters.UniversalCounter.ToString() + "/"
                                + JsonInterFace.ActionResultStatus.APCount.ToString() + "]", "获取载波(" + JsonInterFace.ActionResultStatus.Carrier + ")参数",
                                "超时");

                            break;
                        }
                        else
                        {
                            Thread.Sleep(DelayTime += 5);
                        }
                    }

                    if (i >= GSMDeviceLists.Count) { break; }

                    WaitResultTimer.Start();

                    DomainFullPathName = string.Empty;
                    string[] DeviceFullNameField = GSMDeviceLists[i].Split(new char[] { '.' });
                    for (int j = 0; j < DeviceFullNameField.Length - 1; j++)
                    {
                        if (DomainFullPathName == "" || DomainFullPathName == null)
                        {
                            DomainFullPathName = DeviceFullNameField[j];
                        }
                        else
                        {
                            DomainFullPathName += "." + DeviceFullNameField[j];
                        }
                    }

                    DeviceName = DeviceFullNameField[DeviceFullNameField.Length - 1];

                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        //设备通用参数
                        for (int j = 0; j < 2; j++)
                        {
                            if (DomainFullPathName != "" && DomainFullPathName != null)
                            {
                                Parameters.ConfigType = "LoadedDevice";
                                JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "自动获取[" + DomainFullPathName + "." + DeviceName + "]载波(" + (j + 1).ToString() + ")通用参数开始...", "获取载波(" + (j + 1).ToString() + ")详细信息", "正在通讯...");
                                NetWorkClient.ControllerServer.Send(JsonInterFace.GetGSMCarrierOneGenParaRequest(DomainFullPathName, DeviceName, j.ToString()));
                            }
                        }

                        //设备详细信息
                        Parameters.ConfigType = "LoadedDevice";
                        JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "自动获取[" + DomainFullPathName + "." + DeviceName + "]详细信息开始...", "获取设备详细信息", "正在通讯...");
                        NetWorkClient.ControllerServer.Send(JsonInterFace.GetDeviceDetailRequest(DomainFullPathName, DeviceName));
                    }
                    else
                    {
                        JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "自动获取[" + DomainFullPathName + "." + DeviceName + "]详细信息开始...", "客户端与服务器网络断开", "无网络连接...");
                    }
                }
            }
            #endregion

            //自动获取设备详细信息,通用参数完毕，显示设备树列表
            Parameters.UniversalCounter = 0;
            JsonInterFace.DeviceListRequestCompleteStatus.ValueLoading = 0;
            Parameters.ConfigType = "AutoGettingDeviceDomainList";
            Parameters.SendMessage(Parameters.WinHandle, Parameters.WM_DeviceListInfoLoad, 0, 0);
        }

        /// <summary>
        /// 递归获取所有AP设备名称
        /// </summary>
        /// <param name="Children"></param>
        /// <param name="StationList"></param>
        private void AutoGetAllDeviceDitailFun(IList<CheckBoxTreeModel> Children, ref List<string> StationList)
        {
            if (Children != null)
            {
                foreach (CheckBoxTreeModel itemChild in Children)
                {
                    if (itemChild.Children.Count > 0 && !itemChild.SelfNodeType.Equals(NodeType.LeafNode))
                    {
                        AutoGetAllDeviceDitailFun(itemChild.Children, ref StationList);
                    }
                    else
                    {
                        if (itemChild.SelfNodeType == NodeType.LeafNode.ToString())
                        {
                            StationList.Add(itemChild.FullName);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取所有指定点下所有设备详细参数
        /// </summary>
        /// <param name="Children"></param>
        /// <param name="StationList"></param>
        private void AutoGetAllDeviceDitailList(IList<CheckBoxTreeModel> Children, string StationName, ref List<CheckBoxTreeModel> StationList)
        {
            if (Children != null)
            {
                foreach (CheckBoxTreeModel itemChild in Children)
                {
                    try
                    {
                        if (itemChild.Children.Count > 0 && itemChild.IsStation != "1")
                        {
                            AutoGetAllDeviceDitailList(itemChild.Children, StationName, ref StationList);
                        }
                        else
                        {
                            //站点所有设备参数
                            if (itemChild.IsStation == "1" && itemChild.FullName == StationName)
                            {
                                for (int i = 0; i < itemChild.Children.Count; i++)
                                {
                                    CheckBoxTreeModel Item = new CheckBoxTreeModel();
                                    Item.FullName = itemChild.Children[i].FullName;
                                    Item.Name = itemChild.Children[i].Name;
                                    Item.SN = itemChild.Children[i].SN;
                                    Item.IPAddr = itemChild.Children[i].IPAddr;
                                    Item.Port = itemChild.Children[i].Port;
                                    Item.NetMask = itemChild.Children[i].NetMask;
                                    Item.Mode = itemChild.Children[i].Mode;
                                    Item.InnerType = itemChild.Children[i].InnerType;
                                    Item.IsOnLine = itemChild.Children[i].IsOnLine;
                                    Item.Icon = itemChild.Children[i].Icon;
                                    StationList.Add(Item);
                                }
                                return;
                            }
                        }
                    }
                    catch (Exception Ex)
                    {
                        Parameters.PrintfLogsExtended("获取所有指定点下所有设备详细参数", Ex.Message, Ex.StackTrace);
                    }
                }
            }
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Parameters.LanguageType.Equals("CN") || Parameters.LanguageType.Equals(""))
            {
                DeviceListTreeView.DataContext = (Language_CN.DeviceTree.TreeViewContent)TreeViewLanguageClass;
                DeviceListTreeView.ContextMenu.DataContext = (Language_CN.DeviceTree.DeviceTree_ContentMenu)TreeViewLanguageContentMenuClass;
            }
            else
            {
                DeviceListTreeView.DataContext = (Language_EN.DeviceTree.TreeViewContent)TreeViewLanguageClass;
                DeviceListTreeView.ContextMenu.DataContext = (Language_EN.DeviceTree.DeviceTree_ContentMenu)TreeViewLanguageContentMenuClass;
            }

            DeviceTreeListWindow.DataContext = Parameters.UnknownDeviceWindowControlParameters;
            UnknonwDeviceTipsBlockWindow.DataContext = Parameters.UnknownDeviceWindowControlParameters;
        }

        /// <summary>
        /// 展开结点
        /// </summary>
        private void ExpandTree()
        {
            if (this.DeviceListTreeView.Items != null && this.DeviceListTreeView.Items.Count > 0)
            {
                foreach (var item in this.DeviceListTreeView.Items)
                {
                    DependencyObject dependencyObject = this.DeviceListTreeView.ItemContainerGenerator.ContainerFromItem(item);
                    if (dependencyObject != null)
                    {
                        ((TreeViewItem)dependencyObject).ExpandSubtree();
                    }
                }
            }
        }

        //设备激活操作
        private void miActive_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                IntPtr BarWinHandle = (IntPtr)Parameters.FindWindow(null, "VolumeActiveProgressBar");
                if (BarWinHandle != IntPtr.Zero)
                {
                    MessageBox.Show(SubWindow.VolumeActiveOrUnActiveProgressBarWindow.ProgressBarParameter.Tips, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                CarrierList.Clear();
                if (DeviceListTreeView.SelectedItem != null)
                {
                    //单站点激活
                    if ((DeviceListTreeView.SelectedItem as CheckBoxTreeModel).IsStation == "0")
                    {
                        if (!(DeviceListTreeView.SelectedItem as CheckBoxTreeModel).IsOnLine)
                        {
                            MessageBox.Show("该设备不在线！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        SelectedDeviceParameters();

                        if (DeviceType.GSM == (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).Mode)
                        {
                            if (MessageBox.Show("确定激活AP吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                            {
                                if (NetWorkClient.ControllerServer.Connected)
                                {
                                    SubWindow.GSMCarrierChoiseWindow GSMCarrierChoiseWin = new SubWindow.GSMCarrierChoiseWindow(DeviceType.GSM);
                                    GSMCarrierChoiseWin.Left = Parameters.UserMousePosition.X + 30;
                                    GSMCarrierChoiseWin.Top = Parameters.UserMousePosition.Y + 30;

                                    if ((bool)GSMCarrierChoiseWin.ShowDialog())
                                    {
                                        //请求的载波
                                        string Carrier = string.Empty;
                                        if (JsonInterFace.GSMCarrierParameter.CarrierOne)
                                        {
                                            Carrier = (Convert.ToInt32(JsonInterFace.GSMCarrierParameter.CarrierOne) - 1).ToString();
                                            CarrierList.Add(Carrier);
                                            JsonInterFace.GSMCarrierParameter.CarrierTotal = 0;
                                        }

                                        if (JsonInterFace.GSMCarrierParameter.CarrierTwo)
                                        {
                                            Carrier = (Convert.ToInt32(JsonInterFace.GSMCarrierParameter.CarrierTwo)).ToString();
                                            CarrierList.Add(Carrier);
                                            JsonInterFace.GSMCarrierParameter.CarrierTotal = 1;
                                        }

                                        if (CarrierList.Count <= 0)
                                        {
                                            JsonInterFace.GSMCarrierParameter.CarrierTotal = -1;
                                            MessageBox.Show("GSM载波参数有误！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                            return;
                                        }

                                        if (CarrierList.Count > 1)
                                        {
                                            JsonInterFace.GSMCarrierParameter.CarrierTotal = 2;
                                        }

                                        JsonInterFace.ResultMessageList.Clear();
                                        JsonInterFace.GSMCarrierParameter.SubmitCount = 0;
                                        //激活
                                        GSMActiveEventsTimer.Start();
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                                }
                            }
                        }
                        else if (new Regex(DeviceType.LTE).Match((DeviceListTreeView.SelectedItem as CheckBoxTreeModel).Mode).Success)
                        {
                            if (MessageBox.Show("确定激活AP吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                            {
                                if (NetWorkClient.ControllerServer.Connected)
                                {
                                    SelectedDeviceParameters();
                                    Parameters.ConfigType = "APActive";
                                    NetWorkClient.ControllerServer.Send(JsonInterFace.APActiveRequest(
                                                                                                        JsonInterFace.LteDeviceParameter.DomainFullPathName,
                                                                                                        JsonInterFace.LteDeviceParameter.DeviceName,
                                                                                                        JsonInterFace.LteDeviceParameter.IpAddr,
                                                                                                        int.Parse(JsonInterFace.LteDeviceParameter.Port),
                                                                                                        JsonInterFace.LteDeviceParameter.InnerType,
                                                                                                        JsonInterFace.LteDeviceParameter.SN
                                                                                                      ));
                                }
                                else
                                {
                                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                                }
                            }
                        }
                        else if (DeviceType.WCDMA == (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).Mode)
                        {
                            if (MessageBox.Show("确定激活AP吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                            {
                                if (NetWorkClient.ControllerServer.Connected)
                                {
                                    Parameters.ConfigType = "APActive";
                                    NetWorkClient.ControllerServer.Send(JsonInterFace.APActiveRequest(
                                                                                                        JsonInterFace.WCDMADeviceParameter.DomainFullPathName,
                                                                                                        JsonInterFace.WCDMADeviceParameter.DeviceName,
                                                                                                        JsonInterFace.WCDMADeviceParameter.IpAddr,
                                                                                                        int.Parse(JsonInterFace.WCDMADeviceParameter.Port),
                                                                                                        JsonInterFace.WCDMADeviceParameter.InnerType,
                                                                                                        JsonInterFace.WCDMADeviceParameter.SN
                                                                                                      ));
                                }
                                else
                                {
                                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                                }
                            }
                        }
                        else if (DeviceType.CDMA == (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).Mode)
                        {
                            if (MessageBox.Show("确定激活AP吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                            {
                                if (NetWorkClient.ControllerServer.Connected)
                                {
                                    NetWorkClient.ControllerServer.Send(JsonInterFace.CDMAActiveRequest(
                                                                                                        JsonInterFace.CDMADeviceParameter.DomainFullPathName,
                                                                                                        JsonInterFace.CDMADeviceParameter.DeviceName,
                                                                                                        JsonInterFace.CDMADeviceParameter.IpAddr,
                                                                                                        JsonInterFace.CDMADeviceParameter.Port,
                                                                                                        JsonInterFace.CDMADeviceParameter.InnerType,
                                                                                                        JsonInterFace.CDMADeviceParameter.SN
                                                                                                      ));
                                }
                                else
                                {
                                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                                }
                            }
                        }
                        else if (DeviceType.GSMV2 == (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).Mode)
                        {
                            if (MessageBox.Show("确定激活AP吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                            {
                                if (NetWorkClient.ControllerServer.Connected)
                                {
                                    SubWindow.GSMCarrierChoiseWindow GSMV2CarrierChoiseWin = new SubWindow.GSMCarrierChoiseWindow(DeviceType.GSMV2);
                                    GSMV2CarrierChoiseWin.Left = Parameters.UserMousePosition.X + 30;
                                    GSMV2CarrierChoiseWin.Top = Parameters.UserMousePosition.Y + 30;

                                    if ((bool)GSMV2CarrierChoiseWin.ShowDialog())
                                    {
                                        //请求的载波
                                        string Carrier = string.Empty;
                                        if (JsonInterFace.GSMV2CarrierParameter.CarrierOne && JsonInterFace.GSMV2CarrierParameter.CarrierTwo)
                                        {
                                            JsonInterFace.GSMV2CarrierParameter.CarrierTotal = 2;
                                        }
                                        else
                                        {
                                            if (JsonInterFace.GSMV2CarrierParameter.CarrierOne)
                                            {
                                                JsonInterFace.GSMV2CarrierParameter.CarrierTotal = 0;
                                            }

                                            if (JsonInterFace.GSMV2CarrierParameter.CarrierTwo)
                                            {
                                                JsonInterFace.GSMV2CarrierParameter.CarrierTotal = 1;
                                            }
                                        }

                                        JsonInterFace.GSMV2CarrierParameter.SubmitCount = 0;

                                        JsonInterFace.ResultMessageList.Clear();
                                        if (JsonInterFace.GSMV2CarrierParameter.CarrierOne)
                                        {
                                            Carrier = (Convert.ToInt32(JsonInterFace.GSMV2CarrierParameter.CarrierOne) - 1).ToString();
                                            NetWorkClient.ControllerServer.Send(JsonInterFace.GSMV2ActiveRequest(
                                                                                                        JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                                        JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                                        JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                                                                                        JsonInterFace.GSMV2DeviceParameter.Port,
                                                                                                        JsonInterFace.GSMV2DeviceParameter.InnerType,
                                                                                                        JsonInterFace.GSMV2DeviceParameter.SN,
                                                                                                        Carrier
                                                                                                      ));
                                        }

                                        if (JsonInterFace.GSMV2CarrierParameter.CarrierOne && JsonInterFace.GSMV2CarrierParameter.CarrierTwo) { Thread.Sleep(1000); }

                                        if (JsonInterFace.GSMV2CarrierParameter.CarrierTwo)
                                        {
                                            Carrier = (Convert.ToInt32(JsonInterFace.GSMV2CarrierParameter.CarrierTwo)).ToString();
                                            NetWorkClient.ControllerServer.Send(JsonInterFace.GSMV2ActiveRequest(
                                                                                                        JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                                        JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                                        JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                                                                                        JsonInterFace.GSMV2DeviceParameter.Port,
                                                                                                        JsonInterFace.GSMV2DeviceParameter.InnerType,
                                                                                                        JsonInterFace.GSMV2DeviceParameter.SN,
                                                                                                        Carrier
                                                                                                      ));
                                        }
                                        if (!JsonInterFace.GSMV2CarrierParameter.CarrierOne && !JsonInterFace.GSMV2CarrierParameter.CarrierTwo)
                                        {
                                            MessageBox.Show("GSMV2载波参数有误！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                            return;
                                        }

                                    }
                                }
                                else
                                {
                                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                                }
                            }
                        }
                        else if (DeviceType.TD_SCDMA == (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).Mode)
                        {
                            if (MessageBox.Show("确定激活AP吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                            {
                                if (NetWorkClient.ControllerServer.Connected)
                                {
                                    Parameters.ConfigType = "APActive";
                                    NetWorkClient.ControllerServer.Send(JsonInterFace.APActiveRequest(
                                                                                                        JsonInterFace.TDSDeviceParameter.DomainFullPathName,
                                                                                                        JsonInterFace.TDSDeviceParameter.DeviceName,
                                                                                                        JsonInterFace.TDSDeviceParameter.IpAddr,
                                                                                                        int.Parse(JsonInterFace.TDSDeviceParameter.Port),
                                                                                                        JsonInterFace.TDSDeviceParameter.InnerType,
                                                                                                        JsonInterFace.TDSDeviceParameter.SN
                                                                                                      ));
                                }
                                else
                                {
                                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                                }
                            }
                        }
                    }
                    //按站点激活
                    else if ((DeviceListTreeView.SelectedItem as CheckBoxTreeModel).IsStation == "1")
                    {
                        if (MessageBox.Show("确定激活该站点下的所有AP吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                        {
                            if (NetWorkClient.ControllerServer.Connected)
                            {
                                SubWindow.VolumeActiveOrUnActiveProgressBarWindow VolumeActiveOrUnActiveProgressBarWin = new SubWindow.VolumeActiveOrUnActiveProgressBarWindow();
                                VolumeActiveOrUnActiveProgressBarWin.Left = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width - VolumeActiveOrUnActiveProgressBarWin.Width - 50;
                                VolumeActiveOrUnActiveProgressBarWin.Top = 50;
                                SubWindow.VolumeActiveOrUnActiveProgressBarWindow.ProgressBarParameter.Tips = "正在批量激活设备，请稍后...";
                                VolumeActiveOrUnActiveProgressBarWin.Show();

                                string FullName = (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).FullName;

                                LoadDeviceListTreeView();
                                new Thread(() =>
                                {
                                    VolumeActivationDevices(JsonInterFace.UsrdomainData, FullName);
                                }).Start();
                            }
                            else
                            {
                                MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("请选择左边列表的设备！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    return;
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        //批量激活设备
        private void VolumeActivationDevices(IList<CheckBoxTreeModel> SelectedItem, String StationFullName)
        {
            StringBuilder ErrorList = new StringBuilder();
            StringBuilder NormalList = new StringBuilder();
            string Tips = string.Empty;
            JsonInterFace.ActionResultStatus.Finished = true;

            System.Timers.Timer WaitResultTimer = new System.Timers.Timer();
            WaitResultTimer.AutoReset = true;
            WaitResultTimer.Interval = 5000;
            WaitResultTimer.Elapsed += WaitResultTimer_Elapsed;

            try
            {
                if (StationFullName == "" || StationFullName == null)
                {
                    Parameters.SendMessage(Parameters.VolumeActiveWinHandle, Parameters.WM_ProgressBarWindowClose, 0, 0);
                    MessageBox.Show("请选择站点！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                IList<CheckBoxTreeModel> SelectChildrens = null;
                GettingAllDevices(SelectedItem, StationFullName, ref SelectChildrens);

                if (SelectChildrens != null)
                {
                    CarrierList.Add("0");
                    CarrierList.Add("1");
                    JsonInterFace.GSMCarrierParameter.CarrierOne = true;
                    JsonInterFace.GSMCarrierParameter.CarrierTwo = true;

                    SubWindow.VolumeActiveOrUnActiveProgressBarWindow.ProgressBarParameter.FinishedBarMax = SelectChildrens.Count;
                    SubWindow.VolumeActiveOrUnActiveProgressBarWindow.ProgressBarParameter.FinishedBarStep = 0;

                    for (int i = 0; i <= SelectChildrens.Count; i++)
                    {
                        while (true)
                        {
                            if (JsonInterFace.ActionResultStatus.Finished)
                            {
                                lock (JsonInterFace.ActionResultStatus.FinishedLock)
                                {
                                    WaitResultTimer.Stop();
                                    if (Tips != "" && Tips != null)
                                    {
                                        NormalList.AppendLine(Tips + "[成功]");
                                    }

                                    JsonInterFace.ActionResultStatus.Finished = false;
                                    JsonInterFace.ActionResultStatus.NoResultErrorDeviceName = false;
                                }
                                break;
                            }
                            else if (JsonInterFace.ActionResultStatus.NoResultErrorDeviceName)
                            {
                                //记录问题项
                                WaitResultTimer.Stop();
                                if (Tips != "" && Tips != null)
                                {
                                    ErrorList.AppendLine(Tips + "[超时]");
                                }
                                JsonInterFace.ActionResultStatus.Finished = false;
                                JsonInterFace.ActionResultStatus.NoResultErrorDeviceName = false;
                                break;
                            }
                            else
                            {
                                Thread.Sleep(625);
                            }
                        }

                        if (i == SelectChildrens.Count) { break; }

                        WaitResultTimer.Start();
                        JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "正在激活设备[" + SelectChildrens[i].FullName + "]", "请求激活", "正在通讯...");
                        Tips = "设备[" + SelectChildrens[i].Name + "]激活 ------ ";

                        string FullName = string.Empty;
                        string[] _FullName = SelectChildrens[i].FullName.Split(new char[] { '.' });
                        for (int j = 0; j < _FullName.Length - 1; j++)
                        {
                            if (FullName == "" || FullName == null)
                            {
                                FullName = _FullName[j];
                            }
                            else
                            {
                                FullName += "." + _FullName[j];
                            }
                        }

                        //LTE
                        if (new Regex(DeviceType.LTE).Match(SelectChildrens[i].Mode).Success)
                        {
                            JsonInterFace.LteDeviceParameter.DomainFullPathName = SelectChildrens[i].FullName;
                            JsonInterFace.LteDeviceParameter.DeviceName = SelectChildrens[i].Name;
                            JsonInterFace.LteDeviceParameter.IpAddr = SelectChildrens[i].IPAddr;
                            JsonInterFace.LteDeviceParameter.Port = SelectChildrens[i].Port.ToString();
                            JsonInterFace.LteDeviceParameter.InnerType = SelectChildrens[i].InnerType;
                            JsonInterFace.LteDeviceParameter.SN = SelectChildrens[i].SN;

                            JsonInterFace.ResultMessageList.Clear();
                            Parameters.ConfigType = "APActive";
                            JsonInterFace.ActionResultStatus.ConfigType = "APVolumeActive";
                            NetWorkClient.ControllerServer.Send(JsonInterFace.APActiveRequest(
                                                                                                SelectChildrens[i].FullName,
                                                                                                SelectChildrens[i].Name,
                                                                                                SelectChildrens[i].IPAddr,
                                                                                                SelectChildrens[i].Port,
                                                                                                SelectChildrens[i].InnerType,
                                                                                                SelectChildrens[i].SN
                                                                                              ));

                        }
                        //WCDMA
                        else if (DeviceType.WCDMA == SelectChildrens[i].Mode)
                        {
                            JsonInterFace.WCDMADeviceParameter.DomainFullPathName = SelectChildrens[i].FullName;
                            JsonInterFace.WCDMADeviceParameter.DeviceName = SelectChildrens[i].Name;
                            JsonInterFace.WCDMADeviceParameter.IpAddr = SelectChildrens[i].IPAddr;
                            JsonInterFace.WCDMADeviceParameter.Port = SelectChildrens[i].Port.ToString();
                            JsonInterFace.WCDMADeviceParameter.InnerType = SelectChildrens[i].InnerType;
                            JsonInterFace.WCDMADeviceParameter.SN = SelectChildrens[i].SN;

                            JsonInterFace.ResultMessageList.Clear();
                            Parameters.ConfigType = "APActive";
                            JsonInterFace.ActionResultStatus.ConfigType = "APVolumeActive";
                            NetWorkClient.ControllerServer.Send(JsonInterFace.APActiveRequest(
                                                                                                SelectChildrens[i].FullName,
                                                                                                SelectChildrens[i].Name,
                                                                                                SelectChildrens[i].IPAddr,
                                                                                                SelectChildrens[i].Port,
                                                                                                SelectChildrens[i].InnerType,
                                                                                                SelectChildrens[i].SN
                                                                                              ));

                        }
                        //GSM
                        else if (DeviceType.GSM == SelectChildrens[i].Mode)
                        {
                            JsonInterFace.GSMDeviceParameter.DomainFullPathName = FullName;
                            JsonInterFace.GSMDeviceParameter.DeviceName = SelectChildrens[i].Name;
                            JsonInterFace.GSMDeviceParameter.IpAddr = SelectChildrens[i].IPAddr;
                            JsonInterFace.GSMDeviceParameter.Port = SelectChildrens[i].Port.ToString();
                            JsonInterFace.GSMDeviceParameter.InnerType = SelectChildrens[i].InnerType;
                            JsonInterFace.GSMDeviceParameter.SN = SelectChildrens[i].SN;

                            CarrierList.Add("0");
                            CarrierList.Add("1");
                            JsonInterFace.GSMCarrierParameter.CarrierOne = true;
                            JsonInterFace.GSMCarrierParameter.CarrierTwo = true;
                            JsonInterFace.GSMCarrierParameter.CarrierTotal = 2;
                            JsonInterFace.ResultMessageList.Clear();
                            JsonInterFace.GSMCarrierParameter.SubmitCount = 1;
                            JsonInterFace.ActionResultStatus.ConfigType = "APVolumeActive";

                            //激活
                            GSMActiveEventsTimer.Start();
                        }
                        //CDMA
                        else if (DeviceType.CDMA == SelectChildrens[i].Mode)
                        {
                            JsonInterFace.CDMADeviceParameter.DomainFullPathName = FullName;
                            JsonInterFace.CDMADeviceParameter.DeviceName = SelectChildrens[i].Name;
                            JsonInterFace.CDMADeviceParameter.IpAddr = SelectChildrens[i].IPAddr;
                            JsonInterFace.CDMADeviceParameter.Port = SelectChildrens[i].Port.ToString();
                            JsonInterFace.CDMADeviceParameter.InnerType = SelectChildrens[i].InnerType;
                            JsonInterFace.CDMADeviceParameter.SN = SelectChildrens[i].SN;

                            JsonInterFace.ActionResultStatus.ConfigType = "APVolumeActive";
                            JsonInterFace.ResultMessageList.Clear();
                            NetWorkClient.ControllerServer.Send(JsonInterFace.CDMAActiveRequest(
                                                                                                    FullName,
                                                                                                    SelectChildrens[i].Name,
                                                                                                    SelectChildrens[i].IPAddr,
                                                                                                    (SelectChildrens[i].Port).ToString(),
                                                                                                    SelectChildrens[i].InnerType,
                                                                                                    SelectChildrens[i].SN
                                                                                               ));
                        }
                        //GSMV2
                        else if (DeviceType.GSMV2 == SelectChildrens[i].Mode)
                        {
                            JsonInterFace.GSMV2DeviceParameter.DomainFullPathName = FullName;
                            JsonInterFace.GSMV2DeviceParameter.DeviceName = SelectChildrens[i].Name;
                            JsonInterFace.GSMV2DeviceParameter.IpAddr = SelectChildrens[i].IPAddr;
                            JsonInterFace.GSMV2DeviceParameter.Port = SelectChildrens[i].Port.ToString();
                            JsonInterFace.GSMV2DeviceParameter.InnerType = SelectChildrens[i].InnerType;
                            JsonInterFace.GSMV2DeviceParameter.SN = SelectChildrens[i].SN;

                            JsonInterFace.GSMV2CarrierParameter.CarrierOne = true;
                            JsonInterFace.GSMV2CarrierParameter.CarrierTwo = true;
                            JsonInterFace.GSMV2CarrierParameter.CarrierTotal = 2;
                            JsonInterFace.ResultMessageList.Clear();
                            JsonInterFace.GSMV2CarrierParameter.SubmitCount = 0;
                            JsonInterFace.ActionResultStatus.ConfigType = "APVolumeActive";

                            for (int j = 0; j < JsonInterFace.GSMV2CarrierParameter.CarrierTotal; j++)
                            {
                                string Carrier = j.ToString();
                                Parameters.ConfigType = "APActive";
                                NetWorkClient.ControllerServer.Send(JsonInterFace.GSMV2ActiveRequest(
                                                                                                        FullName,
                                                                                                        SelectChildrens[i].Name,
                                                                                                        SelectChildrens[i].IPAddr,
                                                                                                        (SelectChildrens[i].Port).ToString(),
                                                                                                        SelectChildrens[i].InnerType,
                                                                                                        SelectChildrens[i].SN,
                                                                                                        Carrier
                                                                                                    ));

                                Thread.Sleep(1000);
                            }
                        }
                        //TDS
                        else if (DeviceType.TD_SCDMA == SelectChildrens[i].Mode)
                        {
                            JsonInterFace.TDSDeviceParameter.DomainFullPathName = SelectChildrens[i].FullName;
                            JsonInterFace.TDSDeviceParameter.DeviceName = SelectChildrens[i].Name;
                            JsonInterFace.TDSDeviceParameter.IpAddr = SelectChildrens[i].IPAddr;
                            JsonInterFace.TDSDeviceParameter.Port = SelectChildrens[i].Port.ToString();
                            JsonInterFace.TDSDeviceParameter.InnerType = SelectChildrens[i].InnerType;
                            JsonInterFace.TDSDeviceParameter.SN = SelectChildrens[i].SN;

                            JsonInterFace.ResultMessageList.Clear();
                            Parameters.ConfigType = "APActive";
                            JsonInterFace.ActionResultStatus.ConfigType = "APVolumeActive";
                            NetWorkClient.ControllerServer.Send(JsonInterFace.APActiveRequest(
                                                                                                SelectChildrens[i].FullName,
                                                                                                SelectChildrens[i].Name,
                                                                                                SelectChildrens[i].IPAddr,
                                                                                                SelectChildrens[i].Port,
                                                                                                SelectChildrens[i].InnerType,
                                                                                                SelectChildrens[i].SN
                                                                                              ));

                        }

                        SubWindow.VolumeActiveOrUnActiveProgressBarWindow.ProgressBarParameter.FinishedBarStep++;
                    }

                    CarrierList.Clear();
                    JsonInterFace.ActionResultStatus.Finished = true;
                    JsonInterFace.GSMCarrierParameter.CarrierOne = false;
                    JsonInterFace.GSMCarrierParameter.CarrierTwo = false;
                    JsonInterFace.ActionResultStatus.ConfigType = string.Empty;
                    Parameters.ConfigType = "null";

                    Parameters.SendMessage(Parameters.VolumeActiveWinHandle, Parameters.WM_ProgressBarWindowClose, 0, 0);
                    MessageBox.Show("站点[" + StationFullName + "]批量设备激活结果："
                                   + Environment.NewLine
                                   + NormalList.ToString()
                                   + ErrorList.ToString()
                                   , "批量设备激活"
                                   , MessageBoxButton.OK
                                   , MessageBoxImage.Information);
                }
                else
                {
                    Parameters.SendMessage(Parameters.VolumeActiveWinHandle, Parameters.WM_ProgressBarWindowClose, 0, 0);
                    MessageBox.Show("站点中无任何设备！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("站点下批量激活设备", ex.Message, ex.StackTrace);
            }
        }

        //批量去激活设备
        private void VolumeUnActivationDevices(IList<CheckBoxTreeModel> SelectedItem, String StationFullName)
        {
            StringBuilder ErrorList = new StringBuilder();
            StringBuilder NormalList = new StringBuilder();
            string Tips = string.Empty;
            JsonInterFace.ActionResultStatus.Finished = true;

            System.Timers.Timer WaitResultTimer = new System.Timers.Timer();
            WaitResultTimer.AutoReset = true;
            WaitResultTimer.Interval = 5000;
            WaitResultTimer.Elapsed += WaitResultTimer_Elapsed;

            try
            {
                if (StationFullName == "" || StationFullName == null)
                {
                    Parameters.SendMessage(Parameters.VolumeActiveWinHandle, Parameters.WM_ProgressBarWindowClose, 0, 0);
                    MessageBox.Show("请选择站点！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                IList<CheckBoxTreeModel> SelectChildrens = null;
                GettingAllDevices(SelectedItem, StationFullName, ref SelectChildrens);

                if (SelectChildrens != null)
                {
                    CarrierList.Add("0");
                    CarrierList.Add("1");
                    JsonInterFace.GSMCarrierParameter.CarrierOne = true;
                    JsonInterFace.GSMCarrierParameter.CarrierTwo = true;

                    SubWindow.VolumeActiveOrUnActiveProgressBarWindow.ProgressBarParameter.FinishedBarMax = SelectChildrens.Count;
                    SubWindow.VolumeActiveOrUnActiveProgressBarWindow.ProgressBarParameter.FinishedBarStep = 0;

                    for (int i = 0; i < SelectChildrens.Count; i++)
                    {
                        while (true)
                        {
                            if (JsonInterFace.ActionResultStatus.Finished)
                            {
                                lock (JsonInterFace.ActionResultStatus.FinishedLock)
                                {
                                    WaitResultTimer.Stop();
                                    if (Tips != "" && Tips != null)
                                    {
                                        NormalList.AppendLine(Tips + "[成功]");
                                    }

                                    JsonInterFace.ActionResultStatus.Finished = false;
                                    JsonInterFace.ActionResultStatus.NoResultErrorDeviceName = false;
                                }
                                break;
                            }
                            else if (JsonInterFace.ActionResultStatus.NoResultErrorDeviceName)
                            {
                                //记录问题项
                                WaitResultTimer.Stop();
                                if (Tips != "" && Tips != null)
                                {
                                    ErrorList.AppendLine(Tips + "[超时]");
                                }
                                JsonInterFace.ActionResultStatus.Finished = false;
                                JsonInterFace.ActionResultStatus.NoResultErrorDeviceName = false;
                                break;
                            }
                            else
                            {
                                Thread.Sleep(625);
                            }
                        }

                        if (i == SelectChildrens.Count) { break; }

                        WaitResultTimer.Start();
                        JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "正在去激活设备[" + SelectChildrens[i].FullName + "]", "请求去激活", "正在通讯...");
                        Tips = "设备[" + SelectChildrens[i].Name + "]去激活 ------ ";

                        string FullName = string.Empty;
                        string[] _FullName = SelectChildrens[i].FullName.Split(new char[] { '.' });
                        for (int j = 0; j < _FullName.Length - 1; j++)
                        {
                            if (FullName == "" || FullName == null)
                            {
                                FullName = _FullName[j];
                            }
                            else
                            {
                                FullName += "." + _FullName[j];
                            }
                        }

                        //LTE
                        if (new Regex(DeviceType.LTE).Match(SelectChildrens[i].Mode).Success)
                        {
                            JsonInterFace.LteDeviceParameter.DomainFullPathName = SelectChildrens[i].FullName;
                            JsonInterFace.LteDeviceParameter.DeviceName = SelectChildrens[i].Name;
                            JsonInterFace.LteDeviceParameter.IpAddr = SelectChildrens[i].IPAddr;
                            JsonInterFace.LteDeviceParameter.Port = SelectChildrens[i].Port.ToString();
                            JsonInterFace.LteDeviceParameter.InnerType = SelectChildrens[i].InnerType;
                            JsonInterFace.LteDeviceParameter.SN = SelectChildrens[i].SN;

                            JsonInterFace.ResultMessageList.Clear();
                            Parameters.ConfigType = "APUnActive";
                            JsonInterFace.ActionResultStatus.ConfigType = "APVolumeUnActive";
                            NetWorkClient.ControllerServer.Send(JsonInterFace.APUnActiveRequest(
                                                                                                SelectChildrens[i].FullName,
                                                                                                SelectChildrens[i].Name,
                                                                                                SelectChildrens[i].IPAddr,
                                                                                                SelectChildrens[i].Port,
                                                                                                SelectChildrens[i].InnerType,
                                                                                                SelectChildrens[i].SN
                                                                                              ));

                        }
                        //WCDMA
                        else if (DeviceType.WCDMA == SelectChildrens[i].Mode)
                        {
                            JsonInterFace.WCDMADeviceParameter.DomainFullPathName = SelectChildrens[i].FullName;
                            JsonInterFace.WCDMADeviceParameter.DeviceName = SelectChildrens[i].Name;
                            JsonInterFace.WCDMADeviceParameter.IpAddr = SelectChildrens[i].IPAddr;
                            JsonInterFace.WCDMADeviceParameter.Port = SelectChildrens[i].Port.ToString();
                            JsonInterFace.WCDMADeviceParameter.InnerType = SelectChildrens[i].InnerType;
                            JsonInterFace.WCDMADeviceParameter.SN = SelectChildrens[i].SN;

                            JsonInterFace.ResultMessageList.Clear();
                            Parameters.ConfigType = "APUnActive";
                            JsonInterFace.ActionResultStatus.ConfigType = "APVolumeUnActive";
                            NetWorkClient.ControllerServer.Send(JsonInterFace.APUnActiveRequest(
                                                                                                SelectChildrens[i].FullName,
                                                                                                SelectChildrens[i].Name,
                                                                                                SelectChildrens[i].IPAddr,
                                                                                                SelectChildrens[i].Port,
                                                                                                SelectChildrens[i].InnerType,
                                                                                                SelectChildrens[i].SN
                                                                                              ));

                        }
                        //GSM
                        else if (DeviceType.GSM == SelectChildrens[i].Mode)
                        {
                            JsonInterFace.GSMDeviceParameter.DomainFullPathName = FullName;
                            JsonInterFace.GSMDeviceParameter.DeviceName = SelectChildrens[i].Name;
                            JsonInterFace.GSMDeviceParameter.IpAddr = SelectChildrens[i].IPAddr;
                            JsonInterFace.GSMDeviceParameter.Port = SelectChildrens[i].Port.ToString();
                            JsonInterFace.GSMDeviceParameter.InnerType = SelectChildrens[i].InnerType;
                            JsonInterFace.GSMDeviceParameter.SN = SelectChildrens[i].SN;

                            CarrierList.Add("0");
                            CarrierList.Add("1");
                            JsonInterFace.GSMCarrierParameter.CarrierOne = true;
                            JsonInterFace.GSMCarrierParameter.CarrierTwo = true;
                            JsonInterFace.GSMCarrierParameter.CarrierTotal = 2;
                            JsonInterFace.ResultMessageList.Clear();
                            JsonInterFace.GSMCarrierParameter.SubmitCount = 1;
                            JsonInterFace.ActionResultStatus.ConfigType = "APVolumeUnActive";

                            //去激活
                            GSMUnActiveEventsTimer.Start();
                        }
                        //CDMA
                        else if (DeviceType.CDMA == SelectChildrens[i].Mode)
                        {
                            JsonInterFace.CDMADeviceParameter.DomainFullPathName = FullName;
                            JsonInterFace.CDMADeviceParameter.DeviceName = SelectChildrens[i].Name;
                            JsonInterFace.CDMADeviceParameter.IpAddr = SelectChildrens[i].IPAddr;
                            JsonInterFace.CDMADeviceParameter.Port = SelectChildrens[i].Port.ToString();
                            JsonInterFace.CDMADeviceParameter.InnerType = SelectChildrens[i].InnerType;
                            JsonInterFace.CDMADeviceParameter.SN = SelectChildrens[i].SN;

                            JsonInterFace.ActionResultStatus.ConfigType = "APVolumeUnActive";
                            JsonInterFace.ResultMessageList.Clear();
                            NetWorkClient.ControllerServer.Send(JsonInterFace.CDMAUnActiveRequest(
                                                                                                    FullName,
                                                                                                    SelectChildrens[i].Name,
                                                                                                    SelectChildrens[i].IPAddr,
                                                                                                    (SelectChildrens[i].Port).ToString(),
                                                                                                    SelectChildrens[i].InnerType,
                                                                                                    SelectChildrens[i].SN
                                                                                               ));
                        }
                        //GSMV2
                        else if (DeviceType.GSMV2 == SelectChildrens[i].Mode)
                        {
                            JsonInterFace.GSMV2DeviceParameter.DomainFullPathName = FullName;
                            JsonInterFace.GSMV2DeviceParameter.DeviceName = SelectChildrens[i].Name;
                            JsonInterFace.GSMV2DeviceParameter.IpAddr = SelectChildrens[i].IPAddr;
                            JsonInterFace.GSMV2DeviceParameter.Port = SelectChildrens[i].Port.ToString();
                            JsonInterFace.GSMV2DeviceParameter.InnerType = SelectChildrens[i].InnerType;
                            JsonInterFace.GSMV2DeviceParameter.SN = SelectChildrens[i].SN;

                            JsonInterFace.GSMV2CarrierParameter.CarrierOne = true;
                            JsonInterFace.GSMV2CarrierParameter.CarrierTwo = true;
                            JsonInterFace.GSMV2CarrierParameter.CarrierTotal = 2;
                            JsonInterFace.ResultMessageList.Clear();
                            JsonInterFace.GSMV2CarrierParameter.SubmitCount = 0;
                            JsonInterFace.ActionResultStatus.ConfigType = "APVolumeUnActive";

                            for (int j = 0; j < JsonInterFace.GSMV2CarrierParameter.CarrierTotal; j++)
                            {
                                string Carrier = j.ToString();
                                Parameters.ConfigType = "APUnActive";
                                NetWorkClient.ControllerServer.Send(JsonInterFace.GSMV2UnActiveRequest(
                                                                                                        FullName,
                                                                                                        SelectChildrens[i].Name,
                                                                                                        SelectChildrens[i].IPAddr,
                                                                                                        (SelectChildrens[i].Port).ToString(),
                                                                                                        SelectChildrens[i].InnerType,
                                                                                                        SelectChildrens[i].SN,
                                                                                                        Carrier
                                                                                                    ));

                                Thread.Sleep(1000);
                            }
                        }
                        //TDS
                        else if (DeviceType.TD_SCDMA == SelectChildrens[i].Mode)
                        {
                            JsonInterFace.TDSDeviceParameter.DomainFullPathName = SelectChildrens[i].FullName;
                            JsonInterFace.TDSDeviceParameter.DeviceName = SelectChildrens[i].Name;
                            JsonInterFace.TDSDeviceParameter.IpAddr = SelectChildrens[i].IPAddr;
                            JsonInterFace.TDSDeviceParameter.Port = SelectChildrens[i].Port.ToString();
                            JsonInterFace.TDSDeviceParameter.InnerType = SelectChildrens[i].InnerType;
                            JsonInterFace.TDSDeviceParameter.SN = SelectChildrens[i].SN;

                            JsonInterFace.ResultMessageList.Clear();
                            Parameters.ConfigType = "APUnActive";
                            JsonInterFace.ActionResultStatus.ConfigType = "APVolumeUnActive";
                            NetWorkClient.ControllerServer.Send(JsonInterFace.APUnActiveRequest(
                                                                                                SelectChildrens[i].FullName,
                                                                                                SelectChildrens[i].Name,
                                                                                                SelectChildrens[i].IPAddr,
                                                                                                SelectChildrens[i].Port,
                                                                                                SelectChildrens[i].InnerType,
                                                                                                SelectChildrens[i].SN
                                                                                              ));

                        }
                        SubWindow.VolumeActiveOrUnActiveProgressBarWindow.ProgressBarParameter.FinishedBarStep++;
                    }

                    CarrierList.Clear();
                    JsonInterFace.ActionResultStatus.Finished = true;
                    JsonInterFace.GSMCarrierParameter.CarrierOne = false;
                    JsonInterFace.GSMCarrierParameter.CarrierTwo = false;
                    JsonInterFace.ActionResultStatus.ConfigType = string.Empty;
                    Parameters.ConfigType = "null";

                    Parameters.SendMessage(Parameters.VolumeActiveWinHandle, Parameters.WM_ProgressBarWindowClose, 0, 0);
                    MessageBox.Show("站点[" + StationFullName + "]批量设备去激活结果："
                                   + Environment.NewLine
                                   + NormalList.ToString()
                                   + ErrorList.ToString()
                                   , "批量设备去激活"
                                   , MessageBoxButton.OK
                                   , MessageBoxImage.Information);
                }
                else
                {
                    Parameters.SendMessage(Parameters.VolumeActiveWinHandle, Parameters.WM_ProgressBarWindowClose, 0, 0);
                    MessageBox.Show("站点中无任何设备！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("站点下批量去激活设备", ex.Message, ex.StackTrace);
            }
        }

        private void WaitResultTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            JsonInterFace.ActionResultStatus.NoResultErrorDeviceName = true;
        }

        //递归获取所选站点的全部设备
        private void GettingAllDevices(IList<CheckBoxTreeModel> Children, string StationFullName, ref IList<CheckBoxTreeModel> Childrens)
        {
            if (FullName != null && FullName != "")
            {
                foreach (CheckBoxTreeModel child in Children)
                {
                    if ((child.Children.Count > 0) && (FullName == child.FullName))
                    {
                        Childrens = child.Children;
                        break;
                    }
                    else
                    {
                        GettingAllDevices(child.Children, StationFullName, ref Childrens);
                    }
                }
            }
        }

        private void miTreeNodeExpande_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                if (DeviceListTreeView.SelectedItem != null)
                {
                    bool IsExpanded = (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).IsExpanded;
                    if ((DeviceListTreeView.SelectedItem as CheckBoxTreeModel).SelfNodeType.Equals(NodeType.StructureNode.ToString()))
                    {
                        if (IsExpanded)
                        {
                            (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).IsExpanded = false;
                        }
                        else
                        {
                            (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).IsExpanded = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("请选择域名！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void miReStart_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DeviceListTreeView.SelectedItem != null)
            {
                if (MessageBox.Show("确定重启AP吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        SelectedDeviceParameters();

                        if ((DeviceListTreeView.SelectedItem as CheckBoxTreeModel).Mode != null
                            && (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).Mode != "")
                        {
                            if (new Regex(DeviceType.LTE).Match((DeviceListTreeView.SelectedItem as CheckBoxTreeModel).Mode).Success)
                            {
                                Parameters.ConfigType = "APReboot";
                                NetWorkClient.ControllerServer.Send(JsonInterFace.APRestartRequest(
                                                                                                    JsonInterFace.LteDeviceParameter.DomainFullPathName,
                                                                                                    JsonInterFace.LteDeviceParameter.DeviceName,
                                                                                                    JsonInterFace.LteDeviceParameter.IpAddr,
                                                                                                    int.Parse(JsonInterFace.LteDeviceParameter.Port),
                                                                                                    JsonInterFace.LteDeviceParameter.InnerType,
                                                                                                    JsonInterFace.LteDeviceParameter.SN
                                                                                                  ));
                            }
                            else if (DeviceType.WCDMA == (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).Mode)
                            {
                                Parameters.ConfigType = "APReboot";
                                NetWorkClient.ControllerServer.Send(JsonInterFace.APRestartRequest(
                                                                                                    JsonInterFace.WCDMADeviceParameter.DomainFullPathName,
                                                                                                    JsonInterFace.WCDMADeviceParameter.DeviceName,
                                                                                                    JsonInterFace.WCDMADeviceParameter.IpAddr,
                                                                                                    int.Parse(JsonInterFace.WCDMADeviceParameter.Port),
                                                                                                    JsonInterFace.WCDMADeviceParameter.InnerType,
                                                                                                    JsonInterFace.WCDMADeviceParameter.SN
                                                                                                  ));
                            }
                            else if (DeviceType.CDMA == (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).Mode)
                            {
                                NetWorkClient.ControllerServer.Send(JsonInterFace.CDMARebootRequest(
                                                                                                    JsonInterFace.CDMADeviceParameter.DomainFullPathName,
                                                                                                    JsonInterFace.CDMADeviceParameter.DeviceName,
                                                                                                    JsonInterFace.CDMADeviceParameter.IpAddr,
                                                                                                    JsonInterFace.CDMADeviceParameter.Port,
                                                                                                    JsonInterFace.CDMADeviceParameter.InnerType,
                                                                                                    JsonInterFace.CDMADeviceParameter.SN
                                                                                                  ));
                            }
                            else if (DeviceType.GSMV2 == (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).Mode)
                            {
                                NetWorkClient.ControllerServer.Send(JsonInterFace.CDMARebootRequest(
                                                                                                    JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.Port,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.InnerType,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.SN
                                                                                                  ));
                            }
                            else if (DeviceType.TD_SCDMA == (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).Mode)
                            {
                                Parameters.ConfigType = "APReboot";
                                NetWorkClient.ControllerServer.Send(JsonInterFace.APRestartRequest(
                                                                                                    JsonInterFace.TDSDeviceParameter.DomainFullPathName,
                                                                                                    JsonInterFace.TDSDeviceParameter.DeviceName,
                                                                                                    JsonInterFace.TDSDeviceParameter.IpAddr,
                                                                                                    int.Parse(JsonInterFace.TDSDeviceParameter.Port),
                                                                                                    JsonInterFace.TDSDeviceParameter.InnerType,
                                                                                                    JsonInterFace.TDSDeviceParameter.SN
                                                                                                  ));
                            }
                        }

                        //站点下全部设备重启
                        else if (IsStation == "1")
                        {
                            //弹窗
                            SubWindow.BatchRebootAPControlWindow BatchRebootAPControlWin = new SubWindow.BatchRebootAPControlWindow();
                            BatchRebootAPControlWin.Tag = "ApReboot";
                            BatchRebootAPControlWin.txtStation.Text = BatchRebootAPList.StationFullName;
                            BatchRebootAPControlWin.ShowDialog();
                        }
                    }
                    else
                    {
                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择左边列表的设备！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                return;
            }
        }

        private void miStatisticalInfo_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigatePages.StatisticalInfoWindow statistcalInfoWin = new StatisticalInfoWindow();
            SelectedDeviceParameters();
            statistcalInfoWin.ShowDialog();
        }

        /// <summary>
        /// 获取扫频结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miScannerInfo_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string SelfID = string.Empty;
            string SelfName = string.Empty;
            string DomainFullPathName = string.Empty;
            bool Online = false;

            if (DeviceListTreeView.SelectedItem != null)
            {
                try
                {
                    SelfID = (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).Id;
                    SelfName = (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).Name;
                    Online = (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).IsOnLine;

                    SelectedDeviceParameters();

                    if (Online && (new Regex(DeviceType.LTE).Match((DeviceListTreeView.SelectedItem as CheckBoxTreeModel).Mode).Success))
                    {
                        ScannerInfoWindow scannerInfoWin = new ScannerInfoWindow();
                        for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                        {
                            if (JsonInterFace.APATTributesLists[i].SelfID.Equals(SelfID)
                                && JsonInterFace.APATTributesLists[i].SelfName.Equals(SelfName))
                            {
                                string[] DomainFullPathNameTmp = JsonInterFace.APATTributesLists[i].FullName.ToString().Split(new char[] { '.' });
                                for (int j = 0; j < DomainFullPathNameTmp.Length - 1; j++)
                                {
                                    if (DomainFullPathName.Equals(""))
                                    {
                                        DomainFullPathName = DomainFullPathNameTmp[j];
                                    }
                                    else
                                    {
                                        DomainFullPathName += "." + DomainFullPathNameTmp[j];
                                    }
                                }
                                if (NetWorkClient.ControllerServer.Connected)
                                {
                                    Parameters.ConfigType = "SelectScanner";
                                    //获取扫频结果数据
                                    NetWorkClient.ControllerServer.Send(JsonInterFace.Get_son_earfcn_Request(JsonInterFace.LteDeviceParameter.DomainFullPathName,
                                                                                                            JsonInterFace.LteDeviceParameter.DeviceName,
                                                                                                            JsonInterFace.LteDeviceParameter.IpAddr,
                                                                                                            int.Parse(JsonInterFace.LteDeviceParameter.Port),
                                                                                                            JsonInterFace.LteDeviceParameter.InnerType,
                                                                                                            JsonInterFace.LteDeviceParameter.SN));
                                }
                                else
                                {
                                    Parameters.PrintfLogsExtended("向服务器请求扫频结果:", "Connected Server Failed!");
                                }
                            }
                        }
                        scannerInfoWin.ShowDialog();
                    }
                    else if (Online && (new Regex(DeviceType.WCDMA).Match((DeviceListTreeView.SelectedItem as CheckBoxTreeModel).Mode).Success))
                    {
                        ScannerInfoWindow scannerInfoWin = new ScannerInfoWindow();
                        for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                        {
                            if (JsonInterFace.APATTributesLists[i].SelfID.Equals(SelfID)
                                && JsonInterFace.APATTributesLists[i].SelfName.Equals(SelfName))
                            {
                                string[] DomainFullPathNameTmp = JsonInterFace.APATTributesLists[i].FullName.ToString().Split(new char[] { '.' });
                                for (int j = 0; j < DomainFullPathNameTmp.Length - 1; j++)
                                {
                                    if (DomainFullPathName.Equals(""))
                                    {
                                        DomainFullPathName = DomainFullPathNameTmp[j];
                                    }
                                    else
                                    {
                                        DomainFullPathName += "." + DomainFullPathNameTmp[j];
                                    }
                                }
                                if (NetWorkClient.ControllerServer.Connected)
                                {
                                    Parameters.ConfigType = "SelectScanner";
                                    //获取扫频结果数据
                                    NetWorkClient.ControllerServer.Send(JsonInterFace.Get_son_earfcn_Request(JsonInterFace.WCDMADeviceParameter.DomainFullPathName,
                                                                                                            JsonInterFace.WCDMADeviceParameter.DeviceName,
                                                                                                            JsonInterFace.WCDMADeviceParameter.IpAddr,
                                                                                                            int.Parse(JsonInterFace.WCDMADeviceParameter.Port),
                                                                                                            JsonInterFace.WCDMADeviceParameter.InnerType,
                                                                                                            JsonInterFace.WCDMADeviceParameter.SN));
                                }
                                else
                                {
                                    Parameters.PrintfLogsExtended("向服务器请求扫频结果:", "Connected Server Failed!");
                                }
                            }
                        }
                        scannerInfoWin.ShowDialog();
                    }
                    else if (Online && (new Regex(DeviceType.TD_SCDMA).Match((DeviceListTreeView.SelectedItem as CheckBoxTreeModel).Mode).Success))
                    {
                        TDSScannerInfoWindow TDSscannerInfoWin = new TDSScannerInfoWindow();
                        for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                        {
                            if (JsonInterFace.APATTributesLists[i].SelfID.Equals(SelfID)
                                && JsonInterFace.APATTributesLists[i].SelfName.Equals(SelfName))
                            {
                                string[] DomainFullPathNameTmp = JsonInterFace.APATTributesLists[i].FullName.ToString().Split(new char[] { '.' });
                                for (int j = 0; j < DomainFullPathNameTmp.Length - 1; j++)
                                {
                                    if (DomainFullPathName.Equals(""))
                                    {
                                        DomainFullPathName = DomainFullPathNameTmp[j];
                                    }
                                    else
                                    {
                                        DomainFullPathName += "." + DomainFullPathNameTmp[j];
                                    }
                                }
                                if (NetWorkClient.ControllerServer.Connected)
                                {
                                    Parameters.ConfigType = "SelectScanner";
                                    //获取扫频结果数据
                                    NetWorkClient.ControllerServer.Send(JsonInterFace.Get_son_earfcn_Request(JsonInterFace.TDSDeviceParameter.DomainFullPathName,
                                                                                                            JsonInterFace.TDSDeviceParameter.DeviceName,
                                                                                                            JsonInterFace.TDSDeviceParameter.IpAddr,
                                                                                                            int.Parse(JsonInterFace.TDSDeviceParameter.Port),
                                                                                                            JsonInterFace.TDSDeviceParameter.InnerType,
                                                                                                            JsonInterFace.TDSDeviceParameter.SN));
                                }
                                else
                                {
                                    Parameters.PrintfLogsExtended("向服务器请求扫频结果:", "Connected Server Failed!");
                                }
                            }
                        }
                        TDSscannerInfoWin.ShowDialog();
                    }
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// 获取设备详细信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miDevicedetail_Click(object sender, RoutedEventArgs e)
        {
            string SelfID = string.Empty;
            string SelfName = string.Empty;
            string DomainFullPathName = string.Empty;
            string DeviceName = string.Empty;
            string SelfNodeType = string.Empty;
            string Model = string.Empty;
            bool Online = false;

            //选中AP信息
            if (DeviceListTreeView.SelectedItem != null)
            {
                try
                {
                    SelfID = (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).Id;
                    SelfName = (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).Name;
                    Model = (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).Mode;
                    Online = (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).IsOnLine;

                    if (!Online)
                    {
                        MessageBox.Show("该设备不在线！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    SelectedDeviceParameters();

                    for (int i = 0; i < JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows.Count; i++)
                    {
                        if (JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][8].ToString().Equals(NodeType.StructureNode.ToString()))
                        {
                            continue;
                        }

                        if (JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][1].ToString().Equals(SelfID)
                            && JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][2].ToString().Equals(SelfName))
                        {
                            string[] DomainFullPathNameTmp = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][0].ToString().Split(new char[] { '.' });
                            for (int j = 0; j < DomainFullPathNameTmp.Length - 1; j++)
                            {
                                if (DomainFullPathName.Equals(""))
                                {
                                    DomainFullPathName = DomainFullPathNameTmp[j];
                                }
                                else
                                {
                                    DomainFullPathName += "." + DomainFullPathNameTmp[j];
                                }
                            }
                            DeviceName = SelfName;
                            SelfNodeType = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][8].ToString();
                            break;
                        }
                    }

                    //设备
                    if (SelfNodeType.Equals(NodeType.LeafNode.ToString()))
                    {
                        //请求参数
                        if (NetWorkClient.ControllerServer.Connected)
                        {
                            if (new Regex(DeviceType.LTE).Match(Model).Success)
                            {
                                if (NetWorkClient.ControllerServer.Connected)
                                {
                                    Parameters.ConfigType = "Manul";
                                    NetWorkClient.ControllerServer.Send(JsonInterFace.GetAPGenParaRequest(DomainFullPathName, DeviceName));
                                    NetWorkClient.ControllerServer.Send(JsonInterFace.GetDeviceDetailRequest(DomainFullPathName, DeviceName));
                                }
                                //显示设备详细信息
                                JsonInterFace.LTEDeviceDetail.VersionInfo = ApVersion;
                                SubWindow.DeviceDetailWindow deviceDetailWin = new SubWindow.DeviceDetailWindow();
                                deviceDetailWin.Left = Parameters.UserMousePosition.X + 30;
                                deviceDetailWin.Top = Parameters.UserMousePosition.Y + 30;
                                deviceDetailWin.OperationType = "LTE";
                                deviceDetailWin.ShowDialog();
                            }
                            else if (DeviceType.GSMV2 == Model)
                            {
                                if (NetWorkClient.ControllerServer.Connected)
                                {
                                    string IPAddr = string.Empty;
                                    string Port = string.Empty;
                                    string InnerType = string.Empty;
                                    string SN = string.Empty;

                                    Parameters.ConfigType = "Manul";
                                    NetWorkClient.ControllerServer.Send(JsonInterFace.GetDeviceDetailRequest(DomainFullPathName, DeviceName));
                                    //这两个参数在小区信息中得到
                                    for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                                    {
                                        if (JsonInterFace.APATTributesLists[i].SelfID == SelfID
                                            && JsonInterFace.APATTributesLists[i].SelfName == DeviceName)
                                        {
                                            IPAddr = JsonInterFace.APATTributesLists[i].IpAddr;
                                            Port = JsonInterFace.APATTributesLists[i].Port;
                                            InnerType = JsonInterFace.APATTributesLists[i].InnerType;
                                            SN = JsonInterFace.APATTributesLists[i].SN;
                                            break;
                                        }
                                    }
                                    Parameters.ConfigType = "Auto";
                                    NetWorkClient.ControllerServer.Send(JsonInterFace.GSMV2CellPARAMRequest(DomainFullPathName, DeviceName, IPAddr, Port, InnerType, SN, "0"));
                                    NetWorkClient.ControllerServer.Send(JsonInterFace.GSMV2CellPARAMRequest(DomainFullPathName, DeviceName, IPAddr, Port, InnerType, SN, "1"));
                                }
                                //显示设备详细信息
                                JsonInterFace.CDMADeviceDetail.VersionInfo = ApVersion;
                                SubWindow.DeviceDetailWindow deviceDetailWin = new SubWindow.DeviceDetailWindow();
                                deviceDetailWin.Left = Parameters.UserMousePosition.X + 30;
                                deviceDetailWin.Top = Parameters.UserMousePosition.Y + 30;
                                deviceDetailWin.OperationType = DeviceType.GSMV2;
                                deviceDetailWin.ShowDialog();
                            }
                            else if (DeviceType.GSM == Model)
                            {
                                if (NetWorkClient.ControllerServer.Connected)
                                {
                                    NetWorkClient.ControllerServer.Send(JsonInterFace.GetGSMCarrierOneGenParaRequest(DomainFullPathName, DeviceName, "0"));
                                    NetWorkClient.ControllerServer.Send(JsonInterFace.GetGSMCarrierOneGenParaRequest(DomainFullPathName, DeviceName, "1"));
                                }
                                SubWindow.DeviceDetailWindow GSNDevuceDetaukWin = new SubWindow.DeviceDetailWindow();
                                GSNDevuceDetaukWin.Left = Parameters.UserMousePosition.X + 30;
                                GSNDevuceDetaukWin.Top = Parameters.UserMousePosition.Y + 30;
                                GSNDevuceDetaukWin.OperationType = DeviceType.GSM;
                                GSNDevuceDetaukWin.ShowDialog();
                            }
                            else if (DeviceType.WCDMA == Model)
                            {
                                if (NetWorkClient.ControllerServer.Connected)
                                {
                                    Parameters.ConfigType = "Manul";
                                    NetWorkClient.ControllerServer.Send(JsonInterFace.GetAPGenParaRequest(DomainFullPathName, DeviceName));
                                    NetWorkClient.ControllerServer.Send(JsonInterFace.GetDeviceDetailRequest(DomainFullPathName, DeviceName));
                                }
                                //显示设备详细信息
                                JsonInterFace.WCDMADeviceDetail.VersionInfo = ApVersion;
                                SubWindow.DeviceDetailWindow deviceDetailWin = new SubWindow.DeviceDetailWindow();
                                deviceDetailWin.Left = Parameters.UserMousePosition.X + 30;
                                deviceDetailWin.Top = Parameters.UserMousePosition.Y + 30;
                                deviceDetailWin.OperationType = "WCDMA";
                                deviceDetailWin.ShowDialog();
                            }
                            else if (DeviceType.CDMA == Model)
                            {
                                if (NetWorkClient.ControllerServer.Connected)
                                {
                                    string IPAddr = string.Empty;
                                    string Port = string.Empty;
                                    string InnerType = string.Empty;
                                    string SN = string.Empty;

                                    Parameters.ConfigType = "Manul";
                                    NetWorkClient.ControllerServer.Send(JsonInterFace.GetDeviceDetailRequest(DomainFullPathName, DeviceName));

                                    //这两个参数在小区信息中得到
                                    for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                                    {
                                        if (JsonInterFace.APATTributesLists[i].SelfID == SelfID
                                            && JsonInterFace.APATTributesLists[i].SelfName == DeviceName)
                                        {
                                            IPAddr = JsonInterFace.APATTributesLists[i].IpAddr;
                                            Port = JsonInterFace.APATTributesLists[i].Port;
                                            InnerType = JsonInterFace.APATTributesLists[i].InnerType;
                                            SN = JsonInterFace.APATTributesLists[i].SN;
                                            break;
                                        }
                                    }

                                    Parameters.ConfigType = "Auto";
                                    NetWorkClient.ControllerServer.Send(
                                                                        JsonInterFace.CDMACellPARAMRequest(
                                                                                                            DomainFullPathName,
                                                                                                            DeviceName,
                                                                                                            IPAddr,
                                                                                                            Port,
                                                                                                            InnerType,
                                                                                                            SN
                                                                                                          )
                                                                       );
                                }
                                //显示设备详细信息
                                JsonInterFace.CDMADeviceDetail.VersionInfo = ApVersion;
                                SubWindow.DeviceDetailWindow deviceDetailWin = new SubWindow.DeviceDetailWindow();
                                deviceDetailWin.Left = Parameters.UserMousePosition.X + 30;
                                deviceDetailWin.Top = Parameters.UserMousePosition.Y + 30;
                                deviceDetailWin.OperationType = "CDMA";
                                deviceDetailWin.ShowDialog();
                            }
                            else if (DeviceType.TD_SCDMA == Model)
                            {
                                if (NetWorkClient.ControllerServer.Connected)
                                {
                                    Parameters.ConfigType = "Manul";
                                    NetWorkClient.ControllerServer.Send(JsonInterFace.GetAPGenParaRequest(DomainFullPathName, DeviceName));
                                    NetWorkClient.ControllerServer.Send(JsonInterFace.GetDeviceDetailRequest(DomainFullPathName, DeviceName));
                                }
                                //显示设备详细信息
                                JsonInterFace.TDSDeviceDetail.VersionInfo = ApVersion;
                                SubWindow.DeviceDetailWindow deviceDetailWin = new SubWindow.DeviceDetailWindow();
                                deviceDetailWin.Left = Parameters.UserMousePosition.X + 30;
                                deviceDetailWin.Top = Parameters.UserMousePosition.Y + 30;
                                deviceDetailWin.OperationType = "TD-SCDMA";
                                deviceDetailWin.ShowDialog();
                            }
                        }
                        else
                        {
                            MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("请选择左边列表的设备！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
                }
            }
        }

        private void miDeviceManage_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Parameters.ConfigType = Model;
            DeviceManagerWindow DeviceManagerFrom = new DeviceManagerWindow();
            DeviceManagerFrom.SelfDevicePara.SelfID = SelfID;
            DeviceManagerFrom.SelfDevicePara.ParentID = ParentID;
            DeviceManagerFrom.SelfDevicePara.DomainFullNamePath = FullName;
            DeviceManagerFrom.SelfDevicePara.DeviceName = SelfName;
            DeviceManagerFrom.SelfDevicePara.IsOneline = IsOnline;
            SelectedDeviceParameters();
            DeviceManagerFrom.ShowDialog();
        }

        /// <summary>
        /// 右键菜单控制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeviceListTreeView_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                //右键时坐标
                Parameters.UserMousePosition.X = e.GetPosition((sender as TreeViewItem)).X;
                Parameters.UserMousePosition.Y = e.GetPosition((sender as TreeViewItem)).Y;

                if (DeviceListTreeView.SelectedItem != null)
                {
                    if (((CheckBoxTreeModel)DeviceListTreeView.SelectedItem).IsChecked)
                    {
                        ((CheckBoxTreeModel)DeviceListTreeView.SelectedItem).IsChecked = false;
                    }
                }

                if (DeviceListTreeView.SelectedItem == null)
                {

                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[0]).IsEnabled = false;
                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[1]).IsEnabled = false;
                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[2]).IsEnabled = false;
                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[3]).IsEnabled = false;
                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[4]).IsEnabled = false;

                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[6]).IsEnabled = false;
                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[7]).IsEnabled = false;
                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[8]).IsEnabled = false;

                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[10]).IsEnabled = false;
                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[11]).IsEnabled = false;
                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[12]).IsEnabled = false;
                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[13]).IsEnabled = false;

                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[15]).IsEnabled = true;
                }
                else if (DeviceListTreeView.SelectedItem != null)
                {
                    string SelectNodteType = (DeviceListTreeView.SelectedItem as DataInterface.CheckBoxTreeModel).SelfNodeType;
                    string Model = (DeviceListTreeView.SelectedItem as DataInterface.CheckBoxTreeModel).Mode;
                    if (SelectNodteType.Equals(NodeType.RootNode.ToString()))
                    {
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[0]).IsEnabled = true;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[1]).IsEnabled = true;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[2]).IsEnabled = true;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[3]).IsEnabled = true;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[4]).IsEnabled = true;

                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[6]).IsEnabled = false;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[7]).IsEnabled = false;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[8]).IsEnabled = false;

                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[10]).IsEnabled = true;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[11]).IsEnabled = false;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[12]).IsEnabled = false;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[13]).IsEnabled = false;

                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[14]).IsEnabled = true;
                    }
                    else if (SelectNodteType.Equals(NodeType.StructureNode.ToString()))
                    {
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[0]).IsEnabled = true;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[1]).IsEnabled = true;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[2]).IsEnabled = true;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[3]).IsEnabled = true;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[4]).IsEnabled = true;

                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[6]).IsEnabled = false;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[7]).IsEnabled = false;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[8]).IsEnabled = false;

                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[10]).IsEnabled = true;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[11]).IsEnabled = true;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[12]).IsEnabled = true;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[13]).IsEnabled = false;

                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[15]).IsEnabled = true;
                    }
                    else if (SelectNodteType.Equals(NodeType.LeafNode.ToString()))
                    {
                        if (DeviceType.GSM == Model)
                        {
                            ((MenuItem)DeviceListTreeView.ContextMenu.Items[2]).IsEnabled = false;
                        }
                        else
                        {
                            ((MenuItem)DeviceListTreeView.ContextMenu.Items[2]).IsEnabled = true;
                        }
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[0]).IsEnabled = true;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[1]).IsEnabled = true;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[3]).IsEnabled = false;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[4]).IsEnabled = false;

                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[6]).IsEnabled = true;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[7]).IsEnabled = true;

                        if (DeviceType.GSM == Model)
                        {
                            ((MenuItem)DeviceListTreeView.ContextMenu.Items[8]).IsEnabled = false;
                        }
                        else
                        {
                            ((MenuItem)DeviceListTreeView.ContextMenu.Items[8]).IsEnabled = true;
                        }

                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[10]).IsEnabled = false;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[11]).IsEnabled = true;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[12]).IsEnabled = true;

                        if (new Regex(DeviceType.LTE).Match(Model).Success)
                        {
                            ((MenuItem)DeviceListTreeView.ContextMenu.Items[13]).IsEnabled = true;
                        }
                        else
                        {
                            ((MenuItem)DeviceListTreeView.ContextMenu.Items[13]).IsEnabled = false;
                        }
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[15]).IsEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("设备列表右键菜单控制", ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// 请求域名列表
        /// </summary>
        public static void GetDomainNameListRequest()
        {
            try
            {
                if (NetWorkClient.ControllerServer.Connected)
                {
                    NetWorkClient.ControllerServer.Send(JsonInterFace.All_DomainLists_Request());
                    JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "[设备域]列表正在请求传输......", "请求设备域", "正在通讯...");
                }
                else
                {
                    Parameters.PrintfLogsExtended("网络与服务器已断开！");
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// 请求获取设备
        /// </summary>
        public static void GetDeviceListsRequest()
        {
            try
            {
                new Thread(() =>
                {
                    //初始化完成进度
                    JsonInterFace.DeviceListRequestCompleteStatus.InitStatus();

                    string StationFullName = string.Empty;
                    JsonInterFace.DeviceListRequestCompleteStatus.MaxLoading = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows.Count - 1;
                    JsonInterFace.DeviceListRequestCompleteStatus.ValueLoading = 0;
                    for (int i = 0; i < JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows.Count; i++)
                    {
                        if (JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][4].ToString().Equals("1")
                            && JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][8].ToString().Equals(NodeType.StructureNode.ToString()))
                        {
                            StationFullName = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][0].ToString();

                            if (NetWorkClient.ControllerServer.Connected)
                            {
                                JsonInterFace.DeviceListRequestCompleteStatus.StationCount++;
                                Parameters.ConfigType = "AutoLoadDeviceLists";
                                NetWorkClient.ControllerServer.Send(JsonInterFace.All_DeviceLists_Request(StationFullName));
                                JsonInterFace.DeviceListRequestCompleteStatus.ValueLoading++;
                            }
                        }
                    }
                }).Start();
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// 增加一个新的域结点(与服务端交互成功)
        /// </summary>
        public static void AddNewDoaminNameTOTreeView()
        {
            try
            {
                new Thread(() =>
                {
                    DataRow rw = JsonInterFace.BindTreeViewClass.DeviceTreeTable.NewRow();
                    rw[0] = Parameters.DomainActionInfoClass.PathName + "." + Parameters.DomainActionInfoClass.SelfName;
                    rw[1] = Parameters.DomainActionInfoClass.SelfID;
                    rw[2] = Parameters.DomainActionInfoClass.SelfName;
                    rw[3] = Parameters.DomainActionInfoClass.ParentID;
                    rw[4] = Parameters.DomainActionInfoClass.IsStation;
                    rw[5] = Parameters.DomainActionInfoClass.NodeContent;
                    rw[6] = Parameters.DomainActionInfoClass.IsDeleted;
                    rw[7] = Parameters.DomainActionInfoClass.Permission;
                    rw[8] = Parameters.DomainActionInfoClass.NodeType;
                    rw[9] = Parameters.DomainActionInfoClass.NodeIcon;
                    JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows.Add(rw);

                    LoadDeviceListTreeViewTimer.Start();
                }).Start();
            }
            catch (Exception ex)
            {

                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// 增加一个新的设备(与服务端交互成功)
        /// </summary>
        public static void AddNewDeviceTOTreeView()
        {
            try
            {
                DataRow rw = JsonInterFace.BindTreeViewClass.DeviceTreeTable.NewRow();
                rw[0] = JsonInterFace.LteDeviceParameter.DomainFullPathName + "." + JsonInterFace.LteDeviceParameter.DeviceName;
                rw[1] = JsonInterFace.LteDeviceParameter.SelfID;
                rw[2] = JsonInterFace.LteDeviceParameter.DeviceName;
                rw[3] = JsonInterFace.LteDeviceParameter.ParentID;
                rw[4] = "0";
                rw[5] = JsonInterFace.LteDeviceParameter.DeviceName;
                rw[6] = false;
                rw[7] = "Enable";
                rw[8] = NodeType.LeafNode.ToString();
                rw[9] = new NodeIcon().LeafNoConnectNodeIcon;
                JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows.Add(rw);

                APATTributes Apattribute = new APATTributes();
                Apattribute.SelfID = JsonInterFace.LteDeviceParameter.SelfID;
                Apattribute.ParentID = JsonInterFace.LteDeviceParameter.ParentID;
                Apattribute.SelfName = JsonInterFace.LteDeviceParameter.DeviceName;
                Apattribute.FullName = JsonInterFace.LteDeviceParameter.DomainFullPathName + "." + JsonInterFace.LteDeviceParameter.DeviceName;
                Apattribute.Mode = JsonInterFace.LteDeviceParameter.DeviceMode;
                Apattribute.SN = JsonInterFace.LteDeviceParameter.SN;
                Apattribute.IpAddr = JsonInterFace.LteDeviceParameter.IpAddr;
                Apattribute.Port = JsonInterFace.LteDeviceParameter.Port;
                Apattribute.NetMask = JsonInterFace.LteDeviceParameter.NetMask;
                Apattribute.IsActive = string.Empty;
                Apattribute.OnLine = "0";
                Apattribute.LastOnline = string.Empty;
                JsonInterFace.APATTributesLists.Add(Apattribute);

                //更新参数
                Dictionary<string, string> UpdateParamList = new Dictionary<string, string>();
                UpdateParamList.Add("name", JsonInterFace.LteDeviceParameter.DeviceName);
                UpdateParamList.Add("mode", JsonInterFace.LteDeviceParameter.DeviceMode);
                UpdateParamList.Add("sn", JsonInterFace.LteDeviceParameter.SN);
                UpdateParamList.Add("carrier", "0");
                UpdateParamList.Add("ipAddr", JsonInterFace.LteDeviceParameter.IpAddr);
                UpdateParamList.Add("port", JsonInterFace.LteDeviceParameter.Port);
                UpdateParamList.Add("netmask", JsonInterFace.LteDeviceParameter.NetMask);

                if (NetWorkClient.ControllerServer.Connected)
                {
                    Parameters.ConfigType = "Auto";
                    NetWorkClient.ControllerServer.Send(JsonInterFace.AddDeviceNameUpdateParametersRequest(JsonInterFace.LteDeviceParameter.DomainFullPathName, JsonInterFace.LteDeviceParameter.DeviceName, UpdateParamList));
                }

                //刷新设备树
                LoadDeviceListTreeViewTimer.Start();
                JsonInterFace.LteDeviceParameter.SelfID = (int.Parse(JsonInterFace.LteDeviceParameter.SelfID) + 1).ToString();
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// 删除一个设备(与服务端交互成功)
        /// </summary>
        public static void DeleteDeviceTOTreeView()
        {
            try
            {
                //设备
                for (int i = 0; i < JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows.Count; i++)
                {
                    if (JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][1].ToString().Equals(JsonInterFace.LteDeviceParameter.SelfID)
                        && JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][3].ToString().Equals(JsonInterFace.LteDeviceParameter.ParentID))
                    {
                        JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i].Delete();
                        break;
                    }
                }

                //设备参数
                for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                {
                    if (JsonInterFace.APATTributesLists[i].SelfID.Equals(JsonInterFace.LteDeviceParameter.SelfID)
                        && JsonInterFace.APATTributesLists[i].ParentID.Equals(JsonInterFace.LteDeviceParameter.ParentID))
                    {
                        JsonInterFace.APATTributesLists.RemoveAt(i);
                        break;
                    }
                }

                LoadDeviceListTreeViewTimer.Start();
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// 更新一个设备(与服务端交互成功)
        /// </summary>
        public static void UpdateDeviceInfoTOTreeView()
        {
            try
            {
                //域名
                for (int i = 0; i < JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows.Count; i++)
                {
                    if (JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][1].ToString().Equals(JsonInterFace.LteDeviceParameter.SelfID)
                        && JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][3].ToString().Equals(JsonInterFace.LteDeviceParameter.ParentID))
                    {
                        JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][0] = JsonInterFace.LteDeviceParameter.DomainFullPathName + "." + JsonInterFace.LteDeviceParameter.DeviceName;
                        JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][2] = JsonInterFace.LteDeviceParameter.DeviceName;
                        JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][5] = JsonInterFace.LteDeviceParameter.DeviceName;
                        break;
                    }
                }

                //属性
                for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                {
                    if (JsonInterFace.APATTributesLists[i].SelfID.ToString().Equals(JsonInterFace.LteDeviceParameter.SelfID)
                        && JsonInterFace.APATTributesLists[i].ParentID.ToString().Equals(JsonInterFace.LteDeviceParameter.ParentID))
                    {
                        JsonInterFace.APATTributesLists[i].FullName = JsonInterFace.LteDeviceParameter.DomainFullPathName + "." + JsonInterFace.LteDeviceParameter.DeviceName;
                        JsonInterFace.APATTributesLists[i].SelfName = JsonInterFace.LteDeviceParameter.DeviceName;
                        JsonInterFace.APATTributesLists[i].IpAddr = JsonInterFace.LteDeviceParameter.IpAddr;
                        JsonInterFace.APATTributesLists[i].Port = JsonInterFace.LteDeviceParameter.Port;
                        JsonInterFace.APATTributesLists[i].NetMask = JsonInterFace.LteDeviceParameter.NetMask;
                        JsonInterFace.APATTributesLists[i].SN = JsonInterFace.LteDeviceParameter.SN;
                        JsonInterFace.APATTributesLists[i].Mode = JsonInterFace.LteDeviceParameter.DeviceMode;
                        break;
                    }
                }

                //重新刷新设备树
                LoadDeviceListTreeViewTimer.Start();
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// 重命名子节点的PathName
        /// </summary>
        public static void ReNameDomainChildNameUpdateTOTreeView(int selfID, string newSelfName, string oldSelfName)
        {
            try
            {
                for (int i = 0; i < JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows.Count; i++)
                {
                    if (int.Parse(JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][3].ToString()).Equals(selfID))
                    {
                        string tmpPathName = string.Empty;
                        string[] tmpDomainName = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][0].ToString().Split(new char[] { '.' });
                        for (int j = 0; j < tmpDomainName.Length; j++)
                        {
                            //找到需要修改的那个域名
                            if (tmpDomainName[j].Equals(oldSelfName))
                            {
                                tmpDomainName[j] = newSelfName;
                                break;
                            }
                        }
                        for (int k = 0; k < tmpDomainName.Length; k++)
                        {
                            if (!tmpPathName.Trim().Equals(""))
                            {
                                tmpPathName += "." + tmpDomainName[k];
                            }
                            else
                            {
                                tmpPathName += tmpDomainName[k];
                            }
                        }
                        JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][0] = tmpPathName;
                        ReNameDomainChildNameUpdateTOTreeView(int.Parse(JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][1].ToString()), newSelfName, oldSelfName);
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// 重命名域名结点(与服务端交互成功)
        /// </summary>
        public static void ReNameDomainNameUpdateTOTreeView()
        {
            try
            {
                string[] oldDomain = Parameters.DomainActionInfoClass.OldFullDomainName.Split(new char[] { '.' });
                ReNameDomainChildNameUpdateTOTreeView(Parameters.DomainActionInfoClass.SelfID, Parameters.DomainActionInfoClass.SelfName, oldDomain[oldDomain.Length - 1].ToString());
                for (int i = 0; i < JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows.Count; i++)
                {
                    if (int.Parse(JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][1].ToString()).Equals(Parameters.DomainActionInfoClass.SelfID) && int.Parse(JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][3].ToString()).Equals(Parameters.DomainActionInfoClass.ParentID))
                    {
                        JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][0] = Parameters.DomainActionInfoClass.PathName;
                        JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][2] = Parameters.DomainActionInfoClass.SelfName;
                        JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][4] = Parameters.DomainActionInfoClass.IsStation;
                        JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][5] = Parameters.DomainActionInfoClass.NodeContent;
                        break;
                    }
                }

                LoadDeviceListTreeViewTimer.Start();
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        #region  递归删除所选节点以及子节点
        /// <summary>
        ///  遍历删除所选节点以及子节点|Datatable 中有 ParentID、SelfID
        /// </summary>
        private static void DeleteDomainNode(int SelfID, DataTable TableSource)
        {
            Dictionary<int, int> ChildNodes = new Dictionary<int, int>();
            try
            {
                // 查询此节点下面的所有的子节点
                for (int i = 0; i < TableSource.Rows.Count; i++)
                {
                    if (TableSource.Rows[i][3].ToString().Equals(SelfID.ToString()))
                    {
                        ChildNodes.Add(int.Parse(TableSource.Rows[i][1].ToString()), int.Parse(TableSource.Rows[i][3].ToString()));
                    }
                }

                // 若此节点下面没有子节点
                if (ChildNodes.Count <= 0)
                {
                    //删除本结点
                    for (int i = 0; i < TableSource.Rows.Count; i++)
                    {
                        if (TableSource.Rows[i][1].ToString().Equals(SelfID.ToString()))
                        {
                            TableSource.Rows.RemoveAt(i);
                            break;
                        }
                    }
                }
                //递归删除节点
                else
                {
                    for (int i = 0; i < TableSource.Rows.Count; i++)
                    {
                        if (TableSource.Rows[i][1].ToString().Equals(SelfID.ToString()))
                        {
                            TableSource.Rows.RemoveAt(i);
                            break;
                        }
                    }

                    foreach (KeyValuePair<int, int> Items in ChildNodes)
                    {
                        DeleteDomainNode(Items.Key, TableSource);
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }
        #endregion

        #region  查找所选节点以及子节点是否是站点
        /// <summary>
        ///  查找所选节点以及子节点是否是站点
        /// </summary>

        private static void GetDeviceParentID(int SelfID, DataTable TableSource, ref Dictionary<string, int> DeviceParentIDList)
        {
            try
            {
                Dictionary<int, int> ChildNodes = new Dictionary<int, int>();

                // 查询此节点下面的所有的子节点
                for (int i = 0; i < TableSource.Rows.Count; i++)
                {
                    if (TableSource.Rows[i][3].ToString().Equals(SelfID.ToString()))
                    {
                        ChildNodes.Add(int.Parse(TableSource.Rows[i][1].ToString()), int.Parse(TableSource.Rows[i][3].ToString()));
                    }
                }

                // 若此节点下面没有子节点
                if (ChildNodes.Count <= 0)
                {
                    //删除本结点
                    for (int i = 0; i < TableSource.Rows.Count; i++)
                    {
                        if (TableSource.Rows[i][1].ToString().Equals(SelfID.ToString())
                            && TableSource.Rows[i][4].ToString().Equals("1")
                            && TableSource.Rows[i][8].ToString().Equals(NodeType.StructureNode.ToString()))
                        {
                            DeviceParentIDList.Add(TableSource.Rows[i][2].ToString(), int.Parse(TableSource.Rows[i][1].ToString()));
                            break;
                        }
                    }
                }
                //递归删除节点
                else
                {
                    for (int i = 0; i < TableSource.Rows.Count; i++)
                    {
                        if (TableSource.Rows[i][1].ToString().Equals(SelfID.ToString())
                            && TableSource.Rows[i][4].ToString().Equals("1")
                            && TableSource.Rows[i][8].ToString().Equals(NodeType.StructureNode.ToString()))
                        {
                            DeviceParentIDList.Add(TableSource.Rows[i][2].ToString(), int.Parse(TableSource.Rows[i][1].ToString()));
                            break;
                        }
                    }

                    foreach (KeyValuePair<int, int> Items in ChildNodes)
                    {
                        GetDeviceParentID(Items.Key, TableSource, ref DeviceParentIDList);
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }
        #endregion

        /// <summary>
        /// 删除域名结点(与服务端交互成功)
        /// </summary>
        public static void DeleteDomainNameUpdateToTreeView()
        {
            new Thread(() =>
            {
                Dictionary<string, int> DeviceParentIDList = new Dictionary<string, int>();

                try
                {
                    //获到设备列表ParentID
                    GetDeviceParentID(Parameters.DomainActionInfoClass.SelfID, JsonInterFace.BindTreeViewClass.DeviceTreeTable, ref DeviceParentIDList);

                    //调用递归删除域名包括:选定的域结 ,子域结点 , 设备结点
                    DeleteDomainNode(Parameters.DomainActionInfoClass.SelfID, JsonInterFace.BindTreeViewClass.DeviceTreeTable);

                    //若设备存在也一并删除(设备列表)
                    foreach (KeyValuePair<string, int> Item in DeviceParentIDList)
                    {
                        int APATTRCount = 0;
                        for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                        {
                            if (Item.Value.ToString().Equals(JsonInterFace.APATTributesLists[i].ParentID))
                            {
                                APATTRCount += 1;
                            }
                        }

                        while (APATTRCount >= 0)
                        {
                            --APATTRCount;

                            for (int j = 0; j < JsonInterFace.APATTributesLists.Count; j++)
                            {
                                if (JsonInterFace.APATTributesLists[j].ParentID.Equals(Item.Value.ToString()))
                                {
                                    JsonInterFace.APATTributesLists.RemoveAt(j);
                                    break;
                                }
                            }
                        }
                    }

                    LoadDeviceListTreeViewTimer.Start();
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended("删除域名异常", ex.Message, ex.StackTrace);
                }
            }).Start();
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miTreeUpdate_Click(object sender, RoutedEventArgs e)
        {
            LoadDeviceListTreeView();
        }

        /// <summary>
        /// 重命名域结点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miEdition_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DeviceListTreeView.SelectedItem != null)
                {
                    string NodeName = ((CheckBoxTreeModel)DeviceListTreeView.SelectedItem).Name;
                    int SelfID = Convert.ToInt32(((CheckBoxTreeModel)DeviceListTreeView.SelectedItem).Id);
                    int ParentID = 0;
                    IList<CheckBoxTreeModel> Children = (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).Children;
                    string FullNodeName = string.Empty;
                    bool IsStation = false;
                    string deviceNameFlagMode = string.Empty;
                    SelectedDeviceParameters();

                    for (int i = 0; i < JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows.Count; i++)
                    {
                        if (JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][2].ToString().Equals(NodeName) && int.Parse(JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][1].ToString()).Equals(SelfID))
                        {
                            FullNodeName = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][0].ToString();
                            ParentID = Convert.ToInt32(JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][3].ToString());
                            IsStation = Convert.ToBoolean(int.Parse(JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][4].ToString()));
                            Parameters.DomainActionInfoClass.OldFullDomainName = FullNodeName;
                            Parameters.DomainActionInfoClass.NodeType = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][8].ToString();
                            deviceNameFlagMode = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][0].ToString();
                            break;
                        }
                    }
                    //获取原始坐标
                    if (IsStation)
                    {
                        string ParentName = string.Empty;
                        for (int i = 0; i < FullNodeName.Split(new char[] { '.' }).Length - 1; i++)
                        {
                            if (i == 0)
                                ParentName += FullNodeName.Split(new char[] { '.' })[i];
                            else
                                ParentName += "." + FullNodeName.Split(new char[] { '.' })[i];
                        }
                        NetWorkClient.ControllerServer.Send(JsonInterFace.Get_station_location_Request(ParentName, NodeName));
                    }
                    if (Parameters.DomainActionInfoClass.NodeType.Equals(NodeType.RootNode.ToString()))
                    {
                        MessageBox.Show("[设备]项不能修改！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    //重命名域名称
                    else if (Parameters.DomainActionInfoClass.NodeType.Equals(NodeType.StructureNode.ToString()))
                    {
                        inputWin = new InputWindow();
                        Parameters.DomainActionInfoClass.SelfID = SelfID;
                        Parameters.DomainActionInfoClass.ParentID = ParentID;
                        inputWin.TreeViewNodeInfoClass.NodeName = NodeName;
                        inputWin.TreeViewNodeInfoClass.DesInfo = Des;
                        inputWin.TreeViewNodeInfoClass.FullNodeName = FullNodeName;
                        inputWin.TreeViewNodeInfoClass.IsStation = IsStation;
                        inputWin.TreeViewNodeInfoClass.Children = Children;
                        inputWin.TreeViewNodeInfoClass.Operation = DeviceTreeOperation.DomainReName;
                        inputWin.lblstationSetting.Visibility = Visibility.Hidden;
                        inputWin.grdCheckBox.Visibility = Visibility.Hidden;
                        inputWin.ShowDialog();
                    }
                    //重命名设备名称
                    else if (Parameters.DomainActionInfoClass.NodeType.Equals(NodeType.LeafNode.ToString()))
                    {
                        AddDeviceWindow deviceAddWin = new AddDeviceWindow();
                        //设备属性
                        for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                        {
                            if (JsonInterFace.APATTributesLists[i].SelfID.Equals(SelfID.ToString()))
                            {
                                JsonInterFace.LteDeviceParameter.Operation = DeviceTreeOperation.DeviceUpdate;
                                string[] DeviceFullNameTmp = JsonInterFace.APATTributesLists[i].FullName.Split(new char[] { '.' });
                                JsonInterFace.LteDeviceParameter.DomainFullPathName = string.Empty;
                                for (int j = 0; j < DeviceFullNameTmp.Length - 1; j++)
                                {
                                    if (JsonInterFace.LteDeviceParameter.DomainFullPathName.Trim().Equals(""))
                                    {
                                        JsonInterFace.LteDeviceParameter.DomainFullPathName = DeviceFullNameTmp[j];
                                    }
                                    else
                                    {
                                        JsonInterFace.LteDeviceParameter.DomainFullPathName += "." + DeviceFullNameTmp[j];
                                    }
                                }

                                JsonInterFace.LteDeviceParameter.SelfID = SelfID.ToString();
                                JsonInterFace.LteDeviceParameter.ParentID = ParentID.ToString();
                                JsonInterFace.LteDeviceParameter.DeviceName = JsonInterFace.APATTributesLists[i].SelfName;
                                JsonInterFace.LteDeviceParameter.Station = DeviceFullNameTmp[DeviceFullNameTmp.Length - 2];
                                JsonInterFace.LteDeviceParameter.SN = JsonInterFace.APATTributesLists[i].SN;
                                JsonInterFace.LteDeviceParameter.IpAddr = JsonInterFace.APATTributesLists[i].IpAddr;
                                JsonInterFace.LteDeviceParameter.Port = JsonInterFace.APATTributesLists[i].Port;
                                JsonInterFace.LteDeviceParameter.NetMask = JsonInterFace.APATTributesLists[i].NetMask;
                                JsonInterFace.LteDeviceParameter.DeviceMode = JsonInterFace.APATTributesLists[i].Mode;
                                JsonInterFace.LteDeviceParameter.Des = JsonInterFace.APATTributesLists[i].Des;
                                break;
                            }
                        }
                        deviceAddWin.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// 删除域名,设备名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DeviceListTreeView.SelectedItem != null)
                {
                    string NodeName = ((CheckBoxTreeModel)DeviceListTreeView.SelectedItem).Name;
                    int SelfID = Convert.ToInt32(((CheckBoxTreeModel)DeviceListTreeView.SelectedItem).Id);
                    int ParentID = 0;
                    string FullNodeName = string.Empty;
                    bool isStation = false;
                    string TreeNodeType = string.Empty;
                    SelectedDeviceParameters();

                    for (int i = 0; i < JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows.Count; i++)
                    {
                        if (JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][2].ToString().Equals(NodeName) && int.Parse(JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][1].ToString()).Equals(SelfID))
                        {
                            FullNodeName = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][0].ToString();
                            ParentID = Convert.ToInt32(JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][3].ToString());
                            isStation = Convert.ToBoolean(int.Parse(JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][4].ToString()));
                            TreeNodeType = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][8].ToString();
                            break;
                        }
                    }

                    if (TreeNodeType.Equals(NodeType.StructureNode.ToString()))
                    {
                        try
                        {
                            if (MessageBox.Show("确定要删除吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning).Equals(MessageBoxResult.OK))
                            {
                                Parameters.DomainActionInfoClass.PathName = FullNodeName;
                                Parameters.DomainActionInfoClass.SelfID = SelfID;
                                Parameters.DomainActionInfoClass.SelfName = NodeName;
                                Parameters.DomainActionInfoClass.ParentID = ParentID;

                                NetWorkClient.ControllerServer.Send(JsonInterFace.DeleteDomainNameRequest(FullNodeName));
                            }
                        }
                        catch (Exception ex)
                        {
                            Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
                        }
                    }
                    else if (TreeNodeType.Equals(NodeType.LeafNode.ToString()))
                    {
                        try
                        {
                            JsonInterFace.LteDeviceParameter.SelfID = SelfID.ToString();
                            JsonInterFace.LteDeviceParameter.ParentID = ParentID.ToString();
                            JsonInterFace.LteDeviceParameter.DeviceName = NodeName;
                            JsonInterFace.LteDeviceParameter.DomainFullPathName = string.Empty;

                            string[] DomainFullNameTmp = FullNodeName.Split(new char[] { '.' });
                            JsonInterFace.LteDeviceParameter.Station = DomainFullNameTmp[DomainFullNameTmp.Length - 2];

                            for (int i = 0; i < DomainFullNameTmp.Length - 1; i++)
                            {
                                if (JsonInterFace.LteDeviceParameter.DomainFullPathName.Trim().Equals(""))
                                {
                                    JsonInterFace.LteDeviceParameter.DomainFullPathName = DomainFullNameTmp[i];
                                }
                                else
                                {
                                    JsonInterFace.LteDeviceParameter.DomainFullPathName += "." + DomainFullNameTmp[i];
                                }
                            }

                            JsonInterFace.LteDeviceParameter.Operation = DeviceTreeOperation.DeviceDelete;

                            if (MessageBox.Show("确定删除设备[" + JsonInterFace.LteDeviceParameter.DeviceName + "]?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                            {
                                if (NetWorkClient.ControllerServer.Connected)
                                {
                                    NetWorkClient.ControllerServer.Send(JsonInterFace.DeleteDeviceNameRequest(JsonInterFace.LteDeviceParameter.DomainFullPathName, JsonInterFace.LteDeviceParameter.DeviceName));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// 添加域名,设备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miAdd_Click(object sender, RoutedEventArgs e)
        {
            if (DeviceListTreeView.SelectedItem != null)
            {
                string NodeName = ((CheckBoxTreeModel)DeviceListTreeView.SelectedItem).Name;
                string SelfID = ((CheckBoxTreeModel)DeviceListTreeView.SelectedItem).Id.ToString();
                SelectedDeviceParameters();
                for (int i = 0; i < JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows.Count; i++)
                {
                    if (JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][2].ToString().Equals(NodeName) && JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][1].ToString().Equals(SelfID))
                    {
                        Parameters.DomainActionInfoClass.SelfID = Convert.ToInt32(JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows.Count - 1][1].ToString());
                        Parameters.DomainActionInfoClass.SelfName = string.Empty;
                        Parameters.DomainActionInfoClass.NodeContent = string.Empty;
                        Parameters.DomainActionInfoClass.ParentID = Convert.ToInt32(JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][1].ToString());
                        Parameters.DomainActionInfoClass.PathName = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][0].ToString();
                        Parameters.DomainActionInfoClass.IsDeleted = false;
                        Parameters.DomainActionInfoClass.NodeType = NodeType.StructureNode.ToString();
                        Parameters.DomainActionInfoClass.Permission = "Enable";
                        Parameters.DomainActionInfoClass.NodeIcon = new NodeIcon().StructureCloseNodeIcon;
                        Parameters.DomainActionInfoClass.IsStation = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][4].ToString();
                        break;
                    }
                }

                //根下添加域结点
                if (Parameters.DomainActionInfoClass.NodeType.Equals(NodeType.RootNode.ToString()))
                {
                    inputWin = new InputWindow();
                    inputWin.TreeViewNodeInfoClass.NodeName = Parameters.DomainActionInfoClass.SelfName;
                    inputWin.TreeViewNodeInfoClass.FullNodeName = Parameters.DomainActionInfoClass.PathName;
                    inputWin.TreeViewNodeInfoClass.Operation = DeviceTreeOperation.DomainAdd;
                    inputWin.ShowDialog();
                    inputWin = null;
                }
                //域下添加域结点
                else if (Parameters.DomainActionInfoClass.NodeType.Equals(NodeType.StructureNode.ToString())
                    && Parameters.DomainActionInfoClass.IsStation.Equals("0"))
                {
                    inputWin = new InputWindow();
                    inputWin.TreeViewNodeInfoClass.NodeName = Parameters.DomainActionInfoClass.SelfName;
                    inputWin.TreeViewNodeInfoClass.FullNodeName = Parameters.DomainActionInfoClass.PathName;
                    inputWin.TreeViewNodeInfoClass.Operation = DeviceTreeOperation.DomainAdd;
                    inputWin.ShowDialog();
                    inputWin = null;
                }
                //站点下添加设备
                else if (Parameters.DomainActionInfoClass.NodeType.Equals(NodeType.StructureNode.ToString())
                    && Parameters.DomainActionInfoClass.IsStation.Equals("1"))
                {
                    AddDeviceWindow inputAddDeviceWin = new AddDeviceWindow();
                    JsonInterFace.LteDeviceParameter.SelfID = (Parameters.DomainActionInfoClass.SelfID + 1).ToString();
                    JsonInterFace.LteDeviceParameter.ParentID = SelfID;
                    JsonInterFace.LteDeviceParameter.DeviceName = string.Empty;
                    JsonInterFace.LteDeviceParameter.Station = NodeName;
                    JsonInterFace.LteDeviceParameter.DomainFullPathName = string.Empty;
                    string[] DomainFullNameTmp = Parameters.DomainActionInfoClass.PathName.Split(new char[] { '.' });
                    for (int i = 0; i < DomainFullNameTmp.Length; i++)
                    {
                        if (JsonInterFace.LteDeviceParameter.DomainFullPathName.Trim().Equals(""))
                        {
                            JsonInterFace.LteDeviceParameter.DomainFullPathName = DomainFullNameTmp[i];
                        }
                        else
                        {
                            JsonInterFace.LteDeviceParameter.DomainFullPathName += "." + DomainFullNameTmp[i];
                        }
                    }
                    JsonInterFace.LteDeviceParameter.Operation = DeviceTreeOperation.DeviceAdd;
                    inputAddDeviceWin.ShowDialog();
                }
                else if (Parameters.DomainActionInfoClass.NodeType.Equals(NodeType.LeafNode.ToString()))
                {
                    JsonInterFace.ShowMessage("设备名称不能添加子项！", 48);
                }
            }
        }

        /// <summary>
        /// 小区信息配置成功更新
        /// </summary>
        public static void CellNeighConfigrationUpdateTOTreeView()
        {
            try
            {
                for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                {
                    if (JsonInterFace.APATTributesLists[i].SelfID.Equals(JsonInterFace.LteCellNeighParameter.SelfID)
                        && JsonInterFace.APATTributesLists[i].ParentID.Equals(JsonInterFace.LteCellNeighParameter.ParentID))
                    {
                        JsonInterFace.APATTributesLists[i].PLMN = JsonInterFace.LteCellNeighParameter.PLMN;

                        JsonInterFace.APATTributesLists[i].FrequencyPoint = JsonInterFace.LteCellNeighParameter.FrequencyPoint;

                        JsonInterFace.APATTributesLists[i].BandWidth = JsonInterFace.LteCellNeighParameter.BandWidth;

                        JsonInterFace.APATTributesLists[i].PowerAttenuation = JsonInterFace.LteCellNeighParameter.PowerAttenuation;

                        JsonInterFace.APATTributesLists[i].FrequencyChioceModeAuto = JsonInterFace.LteSetWorkModeParameter.FrequencyChioceModeAuto;
                        JsonInterFace.APATTributesLists[i].FrequencyChioceModeManul = JsonInterFace.LteSetWorkModeParameter.FrequencyChioceModeManul;

                        JsonInterFace.APATTributesLists[i].Scrambler = JsonInterFace.LteCellNeighParameter.Scrambler;

                        JsonInterFace.APATTributesLists[i].TacLac = JsonInterFace.LteCellNeighParameter.TacLac;

                        JsonInterFace.APATTributesLists[i].Period = JsonInterFace.LteCellNeighParameter.Period;

                        JsonInterFace.APATTributesLists[i].RebootModeAuto = JsonInterFace.LteSetWorkModeParameter.RebootModeAuto;
                        JsonInterFace.APATTributesLists[i].RebootModeManul = JsonInterFace.LteSetWorkModeParameter.RebootModeManul;
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
        /// GPS信息配置成功更新
        /// </summary>
        public static void GPSConfigrationUpdateTOTreeView()
        {
            try
            {
                for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                {
                    if (JsonInterFace.APATTributesLists[i].SelfID.Equals(JsonInterFace.LteDeviceAdvanceSettingParameter.SelfID)
                        && JsonInterFace.APATTributesLists[i].ParentID.Equals(JsonInterFace.LteDeviceAdvanceSettingParameter.ParentID))
                    {
                        JsonInterFace.APATTributesLists[i].GPSStatusConfig = JsonInterFace.LteDeviceAdvanceSettingParameter.GPSStatusConfig;
                        JsonInterFace.APATTributesLists[i].GPSStatusNoneConfig = JsonInterFace.LteDeviceAdvanceSettingParameter.GPSStatusNoneConfig;
                        JsonInterFace.APATTributesLists[i].FrequencyOffsetList = JsonInterFace.LteDeviceAdvanceSettingParameter.FrequencyOffsetList;
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
        /// NTP信息配置更新
        /// </summary>
        public static void NTPConfigrationUpdateTOTreeView()
        {
            try
            {
                for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                {
                    if (JsonInterFace.APATTributesLists[i].SelfID.Equals(JsonInterFace.LteDeviceAdvanceSettingParameter.SelfID)
                        && JsonInterFace.APATTributesLists[i].ParentID.Equals(JsonInterFace.LteDeviceAdvanceSettingParameter.ParentID))
                    {
                        JsonInterFace.APATTributesLists[i].NTPServerIP = JsonInterFace.LteDeviceAdvanceSettingParameter.NTPServerIP;
                        JsonInterFace.APATTributesLists[i].NTPLevel = JsonInterFace.LteDeviceAdvanceSettingParameter.NTPLevel;
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
        /// 同频源信息配置更新
        /// </summary>
        public static void SyncSourceConfigrationUpdateTOTreeView()
        {
            try
            {
                for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                {
                    if (JsonInterFace.APATTributesLists[i].SelfID.Equals(JsonInterFace.LteDeviceAdvanceSettingParameter.SelfID)
                        && JsonInterFace.APATTributesLists[i].ParentID.Equals(JsonInterFace.LteDeviceAdvanceSettingParameter.ParentID))
                    {
                        JsonInterFace.APATTributesLists[i].SyncSourceWithGPS = JsonInterFace.LteDeviceAdvanceSettingParameter.SyncSourceWithGPS;
                        JsonInterFace.APATTributesLists[i].SyncSourceWithKongKou = JsonInterFace.LteDeviceAdvanceSettingParameter.SyncSourceWithKongKou;

                        JsonInterFace.APATTributesLists[i].AppointNeighNoneConfig = JsonInterFace.LteDeviceAdvanceSettingParameter.AppointNeighNoneConfig;
                        JsonInterFace.APATTributesLists[i].AppointNeighConfig = JsonInterFace.LteDeviceAdvanceSettingParameter.AppointNeighConfig;

                        JsonInterFace.APATTributesLists[i].AppointNeighList = JsonInterFace.LteDeviceAdvanceSettingParameter.AppointNeighList;
                        JsonInterFace.APATTributesLists[i].AppointNeighPci = JsonInterFace.LteDeviceAdvanceSettingParameter.AppointNeighPci;
                        JsonInterFace.APATTributesLists[i].AppointNeighBandWidth = JsonInterFace.LteDeviceAdvanceSettingParameter.AppointNeighBandWidth;
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
        /// 时间段信息配置更新
        /// </summary>
        public static void APPeriodTimeConfigrationUpdateTOTreeView()
        {

        }

        //设备列表邦定加载显示
        private void LoadDeviceListTreeView()
        {
            try
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    DataInterface.BindCheckBoxTreeView devicetreeview = new DataInterface.BindCheckBoxTreeView();
                    DataInterface.CheckBoxTreeModel treeModel = new DataInterface.CheckBoxTreeModel();
                    devicetreeview.Dt = JsonInterFace.BindTreeViewClass.DeviceTreeTable;
                    devicetreeview.DeviceTreeViewBind(ref treeModel);
                    lock (JsonInterFace.UsrdomainDataLock)
                    {
                        JsonInterFace.UsrdomainData.Clear();
                        JsonInterFace.UsrdomainData.Add(treeModel);
                    }

                    DeviceListTreeView.ItemsSource = null;
                    DeviceListTreeView.Items.Clear();
                    DeviceListTreeView.Items.Refresh();
                    DeviceListTreeView.ItemsSource = JsonInterFace.UsrdomainData;

                    if (Parameters.ConfigType != null)
                    {
                        if (new Regex("AutoGettingDeviceDomainList").Match(Parameters.ConfigType).Success)
                        {
                            //关闭进度窗口
                            JsonInterFace.DeviceListRequestCompleteStatus.LoadingWindowStatu = Visibility.Collapsed;

                            JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "[设备列表]传输完成", "[设备列]初始化", "完成");
                            Parameters.ConfigType = "null";

                            //加载远程参数
                            MainWindow.LoadRemoteParameterTimer.Start();

                            //登录完成才解析实时上报显示
                            JsonInterFace.LoginFinish = true;

                            //启动信息统计
                            Parameters.SendMessage(Parameters.WinHandle, Parameters.WM_InfoStatisticMessage, 0, 0);
                        }
                    }

                    //关闭弹窗
                    if (inputWin != null)
                    {
                        //inputWin.Close();
                    }
                }));

                //设备管理窗口通知
                if (Parameters.DeviceManageWinHandle != IntPtr.Zero)
                {
                    Parameters.SendMessage(Parameters.DeviceManageWinHandle, Parameters.WM_DeviceManageWinTreeViewReLoade, 0, 0);
                }

                //通知域管理窗口
                if (Parameters.DomainManageWinHandle != IntPtr.Zero)
                {
                    Parameters.SendMessage(Parameters.DomainManageWinHandle, Parameters.WM_DomainManageDeviceReloadEven, 0, 0);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        //设备去激活
        private void miUnActive_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IntPtr BarWinHandle = (IntPtr)Parameters.FindWindow(null, "VolumeActiveProgressBar");
                if (BarWinHandle != IntPtr.Zero)
                {
                    MessageBox.Show(SubWindow.VolumeActiveOrUnActiveProgressBarWindow.ProgressBarParameter.Tips, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                CarrierList.Clear();
                if (DeviceListTreeView.SelectedItem != null)
                {
                    if ((DeviceListTreeView.SelectedItem as CheckBoxTreeModel).IsStation != "1")
                    {
                        if (!(DeviceListTreeView.SelectedItem as CheckBoxTreeModel).IsOnLine)
                        {
                            MessageBox.Show("该设备不在线！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        SelectedDeviceParameters();
                        if (DeviceType.GSM == (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).Mode)
                        {
                            if (MessageBox.Show("确定去激活吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                            {
                                if (NetWorkClient.ControllerServer.Connected)
                                {
                                    SubWindow.GSMCarrierChoiseWindow GSMCarrierChoiseWin = new SubWindow.GSMCarrierChoiseWindow(DeviceType.GSM);
                                    GSMCarrierChoiseWin.Left = Parameters.UserMousePosition.X + 30;
                                    GSMCarrierChoiseWin.Top = Parameters.UserMousePosition.Y + 30;

                                    if ((bool)GSMCarrierChoiseWin.ShowDialog())
                                    {
                                        //请求的载波
                                        string Carrier = string.Empty;
                                        if (JsonInterFace.GSMCarrierParameter.CarrierOne)
                                        {
                                            Carrier = (Convert.ToInt32(JsonInterFace.GSMCarrierParameter.CarrierOne) - 1).ToString();
                                            CarrierList.Add(Carrier);
                                            JsonInterFace.GSMCarrierParameter.CarrierTotal = 0;
                                        }

                                        if (JsonInterFace.GSMCarrierParameter.CarrierTwo)
                                        {
                                            Carrier = (Convert.ToInt32(JsonInterFace.GSMCarrierParameter.CarrierTwo)).ToString();
                                            CarrierList.Add(Carrier);
                                            JsonInterFace.GSMCarrierParameter.CarrierTotal = 1;
                                        }

                                        if (CarrierList.Count <= 0)
                                        {
                                            JsonInterFace.GSMCarrierParameter.CarrierTotal = -1;
                                            MessageBox.Show("GSM载波参数有误！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                            return;
                                        }
                                        else if (CarrierList.Count > 1)
                                        {
                                            JsonInterFace.GSMCarrierParameter.CarrierTotal = 2;
                                        }

                                        JsonInterFace.ResultMessageList.Clear();
                                        JsonInterFace.GSMCarrierParameter.SubmitCount = 0;

                                        //去激活
                                        GSMUnActiveEventsTimer.Start();
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                                }
                            }
                        }
                        else if (new Regex(DeviceType.LTE).Match((DeviceListTreeView.SelectedItem as CheckBoxTreeModel).Mode).Success)
                        {
                            if (MessageBox.Show("确定去激活吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                            {
                                if (NetWorkClient.ControllerServer.Connected)
                                {
                                    Parameters.ConfigType = "APUnActive";
                                    NetWorkClient.ControllerServer.Send(JsonInterFace.APUnActiveRequest(
                                                                                                        JsonInterFace.LteDeviceParameter.DomainFullPathName,
                                                                                                        JsonInterFace.LteDeviceParameter.DeviceName,
                                                                                                        JsonInterFace.LteDeviceParameter.IpAddr,
                                                                                                        int.Parse(JsonInterFace.LteDeviceParameter.Port),
                                                                                                        JsonInterFace.LteDeviceParameter.InnerType,
                                                                                                        JsonInterFace.LteDeviceParameter.SN
                                                                                                      ));
                                }
                                else
                                {
                                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                                }
                            }
                        }
                        else if (DeviceType.WCDMA == (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).Mode)
                        {
                            if (MessageBox.Show("确定去激活吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                            {
                                if (NetWorkClient.ControllerServer.Connected)
                                {
                                    Parameters.ConfigType = "APUnActive";
                                    NetWorkClient.ControllerServer.Send(JsonInterFace.APUnActiveRequest(
                                                                                                        JsonInterFace.WCDMADeviceParameter.DomainFullPathName,
                                                                                                        JsonInterFace.WCDMADeviceParameter.DeviceName,
                                                                                                        JsonInterFace.WCDMADeviceParameter.IpAddr,
                                                                                                        int.Parse(JsonInterFace.WCDMADeviceParameter.Port),
                                                                                                        JsonInterFace.WCDMADeviceParameter.InnerType,
                                                                                                        JsonInterFace.WCDMADeviceParameter.SN
                                                                                                      ));
                                }
                                else
                                {
                                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                                }
                            }
                        }
                        else if (DeviceType.CDMA == (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).Mode)
                        {
                            if (MessageBox.Show("确定去激活吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                            {
                                if (NetWorkClient.ControllerServer.Connected)
                                {
                                    Parameters.ConfigType = "APUnActive";
                                    NetWorkClient.ControllerServer.Send(JsonInterFace.CDMAUnActiveRequest(
                                                                                                            JsonInterFace.CDMADeviceParameter.DomainFullPathName,
                                                                                                            JsonInterFace.CDMADeviceParameter.DeviceName,
                                                                                                            JsonInterFace.CDMADeviceParameter.IpAddr,
                                                                                                            JsonInterFace.CDMADeviceParameter.Port,
                                                                                                            JsonInterFace.CDMADeviceParameter.InnerType,
                                                                                                            JsonInterFace.CDMADeviceParameter.SN
                                                                                                          ));
                                }
                                else
                                {
                                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                                }
                            }
                        }
                        else if (DeviceType.GSMV2 == (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).Mode)
                        {
                            if (MessageBox.Show("确定去激活吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                            {
                                if (NetWorkClient.ControllerServer.Connected)
                                {
                                    SubWindow.GSMCarrierChoiseWindow GSMV2CarrierChoiseWin = new SubWindow.GSMCarrierChoiseWindow(DeviceType.GSMV2);
                                    GSMV2CarrierChoiseWin.Left = Parameters.UserMousePosition.X + 30;
                                    GSMV2CarrierChoiseWin.Top = Parameters.UserMousePosition.Y + 30;

                                    if ((bool)GSMV2CarrierChoiseWin.ShowDialog())
                                    {
                                        //请求的载波
                                        string Carrier = string.Empty;
                                        if (JsonInterFace.GSMV2CarrierParameter.CarrierOne && JsonInterFace.GSMV2CarrierParameter.CarrierTwo)
                                        {
                                            JsonInterFace.GSMV2CarrierParameter.CarrierTotal = 2;
                                        }
                                        else
                                        {
                                            if (JsonInterFace.GSMV2CarrierParameter.CarrierOne)
                                            {
                                                JsonInterFace.GSMV2CarrierParameter.CarrierTotal = 0;
                                            }

                                            if (JsonInterFace.GSMV2CarrierParameter.CarrierTwo)
                                            {
                                                JsonInterFace.GSMV2CarrierParameter.CarrierTotal = 1;
                                            }
                                        }

                                        JsonInterFace.GSMV2CarrierParameter.SubmitCount = 0;

                                        JsonInterFace.ResultMessageList.Clear();
                                        if (JsonInterFace.GSMV2CarrierParameter.CarrierOne)
                                        {
                                            Carrier = (Convert.ToInt32(JsonInterFace.GSMV2CarrierParameter.CarrierOne) - 1).ToString();
                                            Parameters.ConfigType = "APUnActive";
                                            NetWorkClient.ControllerServer.Send(JsonInterFace.GSMV2UnActiveRequest(
                                                                                                                    JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                                                    JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                                                    JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                                                                                                    JsonInterFace.GSMV2DeviceParameter.Port,
                                                                                                                    JsonInterFace.GSMV2DeviceParameter.InnerType,
                                                                                                                    JsonInterFace.GSMV2DeviceParameter.SN,
                                                                                                                    Carrier
                                                                                                                ));
                                        }

                                        if (JsonInterFace.GSMV2CarrierParameter.CarrierOne && JsonInterFace.GSMV2CarrierParameter.CarrierTwo) { Thread.Sleep(1000); }

                                        if (JsonInterFace.GSMV2CarrierParameter.CarrierTwo)
                                        {
                                            Carrier = (Convert.ToInt32(JsonInterFace.GSMV2CarrierParameter.CarrierTwo)).ToString();
                                            Parameters.ConfigType = "APUnActive";
                                            NetWorkClient.ControllerServer.Send(JsonInterFace.GSMV2UnActiveRequest(
                                                                                                                    JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                                                    JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                                                    JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                                                                                                    JsonInterFace.GSMV2DeviceParameter.Port,
                                                                                                                    JsonInterFace.GSMV2DeviceParameter.InnerType,
                                                                                                                    JsonInterFace.GSMV2DeviceParameter.SN,
                                                                                                                    Carrier
                                                                                                                ));
                                        }
                                        if (!JsonInterFace.GSMV2CarrierParameter.CarrierOne && !JsonInterFace.GSMV2CarrierParameter.CarrierTwo)
                                        {
                                            MessageBox.Show("GSMV2载波参数有误！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                            return;
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                                }
                            }
                        }
                        //TDS
                        else if (DeviceType.TD_SCDMA == (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).Mode)
                        {
                            if (MessageBox.Show("确定去激活吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                            {
                                if (NetWorkClient.ControllerServer.Connected)
                                {
                                    Parameters.ConfigType = "APUnActive";
                                    NetWorkClient.ControllerServer.Send(JsonInterFace.APUnActiveRequest(
                                                                                                        JsonInterFace.TDSDeviceParameter.DomainFullPathName,
                                                                                                        JsonInterFace.TDSDeviceParameter.DeviceName,
                                                                                                        JsonInterFace.TDSDeviceParameter.IpAddr,
                                                                                                        int.Parse(JsonInterFace.TDSDeviceParameter.Port),
                                                                                                        JsonInterFace.TDSDeviceParameter.InnerType,
                                                                                                        JsonInterFace.TDSDeviceParameter.SN
                                                                                                      ));
                                }
                                else
                                {
                                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                                }
                            }
                        }
                    }
                    //按站点去激活
                    else if ((DeviceListTreeView.SelectedItem as CheckBoxTreeModel).IsStation == "1")
                    {
                        if (MessageBox.Show("确定去激活该站点下的所有AP吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                        {
                            if (NetWorkClient.ControllerServer.Connected)
                            {
                                SubWindow.VolumeActiveOrUnActiveProgressBarWindow VolumeActiveOrUnActiveProgressBarWin = new SubWindow.VolumeActiveOrUnActiveProgressBarWindow();
                                VolumeActiveOrUnActiveProgressBarWin.Left = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width - VolumeActiveOrUnActiveProgressBarWin.Width - 50;
                                VolumeActiveOrUnActiveProgressBarWin.Top = 50;
                                SubWindow.VolumeActiveOrUnActiveProgressBarWindow.ProgressBarParameter.Tips = "正在批量去激活设备，请稍后...";
                                VolumeActiveOrUnActiveProgressBarWin.Show();

                                string FullName = (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).FullName;

                                LoadDeviceListTreeView();
                                new Thread(() =>
                                {
                                    VolumeUnActivationDevices(JsonInterFace.UsrdomainData, FullName);
                                }).Start();
                            }
                            else
                            {
                                MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("请选择左边列表的设备！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// 激活响应
        /// </summary>
        private void APActiveResponse()
        {
            //修改属性
            for (int i = 0; i < JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows.Count; i++)
            {
                if (DeviceType.WCDMA == Model)
                {
                    if (JsonInterFace.WCDMADeviceParameter.DomainFullPathName.Equals(JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][0].ToString()))
                    {
                        for (int j = 0; j < JsonInterFace.UsrdomainData.Count; j++)
                        {
                            ChangeDivceListAttrActive(JsonInterFace.UsrdomainData[j].Children, JsonInterFace.WCDMADeviceParameter.DomainFullPathName);
                        }

                        for (int j = 0; j < JsonInterFace.APATTributesLists.Count; j++)
                        {
                            if (JsonInterFace.APATTributesLists[j].FullName.Equals(JsonInterFace.WCDMADeviceParameter.DomainFullPathName))
                            {
                                JsonInterFace.APATTributesLists[j].OnLine = "1";
                                break;
                            }
                        }

                        JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][9] = new NodeIcon().LeafAllReadyNodeIcon;

                        LoadDeviceListTreeView();
                        break;
                    }
                }
                else if (DeviceType.TD_SCDMA == Model)
                {
                    if (JsonInterFace.TDSDeviceParameter.DomainFullPathName.Equals(JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][0].ToString()))
                    {
                        for (int j = 0; j < JsonInterFace.UsrdomainData.Count; j++)
                        {
                            ChangeDivceListAttrActive(JsonInterFace.UsrdomainData[j].Children, JsonInterFace.TDSDeviceParameter.DomainFullPathName);
                        }

                        for (int j = 0; j < JsonInterFace.APATTributesLists.Count; j++)
                        {
                            if (JsonInterFace.APATTributesLists[j].FullName.Equals(JsonInterFace.TDSDeviceParameter.DomainFullPathName))
                            {
                                JsonInterFace.APATTributesLists[j].OnLine = "1";
                                break;
                            }
                        }

                        JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][9] = new NodeIcon().LeafAllReadyNodeIcon;

                        LoadDeviceListTreeView();
                        break;
                    }
                }
                else
                {
                    if (JsonInterFace.LteDeviceParameter.DomainFullPathName.Equals(JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][0].ToString()))
                    {
                        for (int j = 0; j < JsonInterFace.UsrdomainData.Count; j++)
                        {
                            ChangeDivceListAttrActive(JsonInterFace.UsrdomainData[j].Children, JsonInterFace.LteDeviceParameter.DomainFullPathName);
                        }

                        for (int j = 0; j < JsonInterFace.APATTributesLists.Count; j++)
                        {
                            if (JsonInterFace.APATTributesLists[j].FullName.Equals(JsonInterFace.LteDeviceParameter.DomainFullPathName))
                            {
                                JsonInterFace.APATTributesLists[j].OnLine = "1";
                                break;
                            }
                        }

                        JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][9] = new NodeIcon().LeafAllReadyNodeIcon;

                        LoadDeviceListTreeView();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 去激活
        /// </summary>
        private void APUnActiveResponse()
        {
            //修改属性
            for (int i = 0; i < JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows.Count; i++)
            {
                if (DeviceType.WCDMA == Model)
                {
                    if (JsonInterFace.WCDMADeviceParameter.DomainFullPathName == (JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][0].ToString()))
                    {
                        for (int j = 0; j < JsonInterFace.UsrdomainData.Count; j++)
                        {
                            ChangeDivceListAttrUnActive(JsonInterFace.UsrdomainData[j].Children, JsonInterFace.WCDMADeviceParameter.DomainFullPathName);
                        }

                        for (int j = 0; j < JsonInterFace.APATTributesLists.Count; j++)
                        {
                            if (JsonInterFace.APATTributesLists[j].FullName == (JsonInterFace.WCDMADeviceParameter.DomainFullPathName))
                            {
                                JsonInterFace.APATTributesLists[j].OnLine = "2";
                                break;
                            }
                        }

                        JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][9] = new NodeIcon().LeafNoActiveNodeIcon;

                        LoadDeviceListTreeView();
                        break;
                    }
                }
                else if (DeviceType.TD_SCDMA == Model)
                {
                    if (JsonInterFace.TDSDeviceParameter.DomainFullPathName == (JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][0].ToString()))
                    {
                        for (int j = 0; j < JsonInterFace.UsrdomainData.Count; j++)
                        {
                            ChangeDivceListAttrUnActive(JsonInterFace.UsrdomainData[j].Children, JsonInterFace.TDSDeviceParameter.DomainFullPathName);
                        }

                        for (int j = 0; j < JsonInterFace.APATTributesLists.Count; j++)
                        {
                            if (JsonInterFace.APATTributesLists[j].FullName == (JsonInterFace.TDSDeviceParameter.DomainFullPathName))
                            {
                                JsonInterFace.APATTributesLists[j].OnLine = "2";
                                break;
                            }
                        }

                        JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][9] = new NodeIcon().LeafNoActiveNodeIcon;

                        LoadDeviceListTreeView();
                        break;
                    }
                }
                else
                {
                    if (JsonInterFace.LteDeviceParameter.DomainFullPathName == (JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][0].ToString()))
                    {
                        for (int j = 0; j < JsonInterFace.UsrdomainData.Count; j++)
                        {
                            ChangeDivceListAttrUnActive(JsonInterFace.UsrdomainData[j].Children, JsonInterFace.LteDeviceParameter.DomainFullPathName);
                        }

                        for (int j = 0; j < JsonInterFace.APATTributesLists.Count; j++)
                        {
                            if (JsonInterFace.APATTributesLists[j].FullName.Equals(JsonInterFace.LteDeviceParameter.DomainFullPathName))
                            {
                                JsonInterFace.APATTributesLists[j].OnLine = "2";
                                break;
                            }
                        }

                        JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][9] = new NodeIcon().LeafNoActiveNodeIcon;

                        LoadDeviceListTreeView();
                        break;
                    }
                }
            }
        }

        //递归处理上线
        private void ChangeDivceListAttrActive(IList<CheckBoxTreeModel> Children, string FullName)
        {
            if (FullName != null && FullName != "")
            {
                foreach (CheckBoxTreeModel child in Children)
                {
                    if (child.Children.Count > 0)
                    {
                        ChangeDivceListAttrActive(child.Children, FullName);
                    }
                    else
                    {
                        if (FullName == child.FullName)
                        {
                            child.Icon = new NodeIcon().LeafAllReadyNodeIcon;
                            break;
                        }
                    }
                }

                JsonInterFace.UsrdomainData = Children;
            }
        }

        //递归处理未激活
        private void ChangeDivceListAttrUnActive(IList<CheckBoxTreeModel> Children, string FullName)
        {
            if (FullName != null && FullName != "")
            {
                foreach (CheckBoxTreeModel child in Children)
                {
                    if (child.Children.Count > 0)
                    {
                        ChangeDivceListAttrUnActive(child.Children, FullName);
                    }
                    else
                    {
                        if (FullName == child.FullName)
                        {
                            child.Icon = new NodeIcon().LeafNoActiveNodeIcon;
                            break;
                        }
                    }
                }

                JsonInterFace.UsrdomainData = Children;
            }
        }

        //递归处理全部下线
        public static void SettingAllDivceOffline(IList<CheckBoxTreeModel> Children)
        {
            foreach (CheckBoxTreeModel child in Children)
            {
                if (child.Children.Count > 0)
                {
                    SettingAllDivceOffline(child.Children);
                }
                else if (child.SelfNodeType == NodeType.LeafNode.ToString())
                {
                    child.Icon = new NodeIcon().LeafNoConnectNodeIcon;

                    for (int j = 0; j < JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows.Count; j++)
                    {
                        if (child.FullName == JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[j]["PathName"].ToString())
                        {
                            JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[j]["NodeIcon"] = new NodeIcon().LeafNoConnectNodeIcon;
                            for (int k = 0; k < JsonInterFace.APATTributesLists.Count; k++)
                            {
                                if (JsonInterFace.APATTributesLists[k].FullName == child.FullName)
                                {
                                    JsonInterFace.APATTributesLists[k].OnLine = "0";
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }

        //GSM激活,GSM去激活
        private void GSMActiveEventStart(string rfEnable)
        {
            new Thread(() =>
            {
                try
                {
                    //参数类型列表
                    List<string> GSMMsgType = new List<string>();
                    GSMMsgType.Add("RECV_RF_PARA");

                    //参数列表
                    List<Dictionary<string, string>> GSMParametersLists = new List<Dictionary<string, string>>();

                    //射频参数(双载波或单载波) JsonInterFace.GSMDeviceParameter.DomainFullPathName
                    if (JsonInterFace.GSMCarrierParameter.CarrierOne && JsonInterFace.GSMCarrierParameter.CarrierTwo)
                    {
                        for (int i = 0; i < CarrierList.Count; i++)
                        {
                            JsonInterFace.GSMRadioFrequencyParameter.RfFreq = null;
                            JsonInterFace.GSMRadioFrequencyParameter.RfPwr = null;
                            int Waitfor = 10;

                            //获取参数
                            if (NetWorkClient.ControllerServer.Connected)
                            {
                                Parameters.ConfigType = "Manul";
                                JsonInterFace.ResultMessageList.Clear();
                                NetWorkClient.ControllerServer.Send(
                                                                    JsonInterFace.GetGSMCarrierOneGenParaRequest(
                                                                                                                    JsonInterFace.GSMDeviceParameter.DomainFullPathName,
                                                                                                                    JsonInterFace.GSMDeviceParameter.DeviceName,
                                                                                                                    CarrierList[i]
                                                                                                                )
                                                                    );


                            }
                            else
                            {
                                MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }

                            //等待参数返回
                            while (true)
                            {
                                if (JsonInterFace.GSMRadioFrequencyParameter.RfFreq != null
                                    && JsonInterFace.GSMRadioFrequencyParameter.RfPwr != null)
                                {
                                    break;
                                }
                                else
                                {
                                    Thread.Sleep(Waitfor);
                                    Waitfor += 10;
                                    if (Waitfor >= 3000)
                                    {
                                        MessageBox.Show("[" + JsonInterFace.GSMDeviceParameter.DomainFullPathName + "." + JsonInterFace.GSMDeviceParameter.DeviceName + "]" + rfEnable == "1" ? "激活超时！" : "去激活超时！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                        return;
                                    }
                                }
                            }
                            Dictionary<string, string> RF_n_dic = new Dictionary<string, string>();
                            RF_n_dic.Add("rfEnable", rfEnable);  // 0 关射频  1 开射频
                            RF_n_dic.Add("rfFreq", JsonInterFace.GSMRadioFrequencyParameter.RfFreq);
                            RF_n_dic.Add("rfPwr", JsonInterFace.GSMRadioFrequencyParameter.RfPwr);
                            GSMParametersLists.Add(RF_n_dic);
                        }
                    }
                    else
                    {
                        if (JsonInterFace.GSMCarrierParameter.CarrierOne)
                        {
                            JsonInterFace.GSMRadioFrequencyParameter.RfFreq = null;
                            JsonInterFace.GSMRadioFrequencyParameter.RfPwr = null;
                            int Waitfor = 10;

                            //获取参数
                            if (NetWorkClient.ControllerServer.Connected)
                            {
                                Parameters.ConfigType = "Manul";
                                JsonInterFace.ResultMessageList.Clear();
                                NetWorkClient.ControllerServer.Send(
                                                                    JsonInterFace.GetGSMCarrierOneGenParaRequest(
                                                                                                                    JsonInterFace.GSMDeviceParameter.DomainFullPathName,
                                                                                                                    JsonInterFace.GSMDeviceParameter.DeviceName,
                                                                                                                    "0"
                                                                                                                )
                                                                    );


                            }
                            else
                            {
                                MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }

                            //等待参数返回
                            while (true)
                            {
                                if (JsonInterFace.GSMRadioFrequencyParameter.RfFreq != null
                                    && JsonInterFace.GSMRadioFrequencyParameter.RfPwr != null)
                                {
                                    break;
                                }
                                else
                                {
                                    Thread.Sleep(Waitfor);
                                    Waitfor += 10;
                                    if (Waitfor >= 200)
                                    {
                                        MessageBox.Show("[" + JsonInterFace.GSMDeviceParameter.DomainFullPathName + "." + JsonInterFace.GSMDeviceParameter.DeviceName + "]" + rfEnable == "1" ? "激活超时！" : "去激活超时！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                        return;
                                    }
                                }
                            }

                            Dictionary<string, string> RF_n_dic = new Dictionary<string, string>();
                            RF_n_dic.Add("rfEnable", rfEnable);
                            RF_n_dic.Add("rfFreq", JsonInterFace.GSMRadioFrequencyParameter.RfFreq);
                            RF_n_dic.Add("rfPwr", JsonInterFace.GSMRadioFrequencyParameter.RfPwr);
                            GSMParametersLists.Add(RF_n_dic);
                        }
                        else if (JsonInterFace.GSMCarrierParameter.CarrierTwo)
                        {

                            JsonInterFace.GSMRadioFrequencyParameter.RfFreq = null;
                            JsonInterFace.GSMRadioFrequencyParameter.RfPwr = null;
                            int Waitfor = 10;

                            //获取参数
                            if (NetWorkClient.ControllerServer.Connected)
                            {
                                Parameters.ConfigType = "Manul";
                                JsonInterFace.ResultMessageList.Clear();
                                NetWorkClient.ControllerServer.Send(
                                                                    JsonInterFace.GetGSMCarrierOneGenParaRequest(
                                                                                                                    JsonInterFace.GSMDeviceParameter.DomainFullPathName,
                                                                                                                    JsonInterFace.GSMDeviceParameter.DeviceName,
                                                                                                                    "1"
                                                                                                                )
                                                                    );


                            }
                            else
                            {
                                MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }

                            //等待参数返回
                            while (true)
                            {
                                if (JsonInterFace.GSMRadioFrequencyParameter.RfFreq != null
                                    && JsonInterFace.GSMRadioFrequencyParameter.RfPwr != null)
                                {
                                    break;
                                }
                                else
                                {
                                    Thread.Sleep(Waitfor);
                                    Waitfor += 10;
                                    if (Waitfor >= 200)
                                    {
                                        MessageBox.Show("[" + JsonInterFace.GSMDeviceParameter.DomainFullPathName + "." + JsonInterFace.GSMDeviceParameter.DeviceName + "]" + rfEnable == "1" ? "激活超时！" : "去激活超时！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                        return;
                                    }
                                }
                            }

                            Dictionary<string, string> RF_n_dic = new Dictionary<string, string>();
                            RF_n_dic.Add("rfEnable", rfEnable);
                            RF_n_dic.Add("rfFreq", JsonInterFace.GSMRadioFrequencyParameter.RfFreq);
                            RF_n_dic.Add("rfPwr", JsonInterFace.GSMRadioFrequencyParameter.RfPwr);
                            GSMParametersLists.Add(RF_n_dic);
                        }
                    }

                    if (rfEnable == "1")
                    {
                        Parameters.ConfigType = "Active";
                    }
                    else
                    {
                        Parameters.ConfigType = "UnActive";
                    }

                    //激活
                    if (JsonInterFace.GSMCarrierParameter.CarrierOne && JsonInterFace.GSMCarrierParameter.CarrierTwo)
                    {
                        for (int i = 0; i < GSMParametersLists.Count; i++)
                        {
                            if (NetWorkClient.ControllerServer.Connected)
                            {
                                NetWorkClient.ControllerServer.Send(JsonInterFace.GSMAPSettingRequest(
                                                                                                        JsonInterFace.GSMDeviceParameter.DomainFullPathName,
                                                                                                        JsonInterFace.GSMDeviceParameter.DeviceName,
                                                                                                        JsonInterFace.GSMDeviceParameter.IpAddr,
                                                                                                        JsonInterFace.GSMDeviceParameter.Port,
                                                                                                        JsonInterFace.GSMDeviceParameter.InnerType,
                                                                                                        JsonInterFace.GSMDeviceParameter.SN,
                                                                                                        GSMParametersLists[i],
                                                                                                        GSMMsgType[0],
                                                                                                        CarrierList[i]
                                                                                                    ));
                                Thread.Sleep(1000);
                            }
                            else
                            {
                                MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }
                        }
                    }
                    else if (JsonInterFace.GSMCarrierParameter.CarrierOne || JsonInterFace.GSMCarrierParameter.CarrierTwo)
                    {
                        NetWorkClient.ControllerServer.Send(JsonInterFace.GSMAPSettingRequest(
                                                                                                JsonInterFace.GSMDeviceParameter.DomainFullPathName,
                                                                                                JsonInterFace.GSMDeviceParameter.DeviceName,
                                                                                                JsonInterFace.GSMDeviceParameter.IpAddr,
                                                                                                JsonInterFace.GSMDeviceParameter.Port,
                                                                                                JsonInterFace.GSMDeviceParameter.InnerType,
                                                                                                JsonInterFace.GSMDeviceParameter.SN,
                                                                                                GSMParametersLists[0],
                                                                                                GSMMsgType[0],
                                                                                                CarrierList[0]
                                                                                            ));
                    }
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended("GSM设备[激活,去激活]失败", ex.Message, ex.StackTrace);
                }
            }).Start();
        }

        //LTE白名单自学习功能
        private void miWhiteListSelfLearning_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SelectedDeviceParameters();
                if (JsonInterFace.LteDeviceParameter.OnLine == "1")
                {
                    if (JsonInterFace.LteDeviceParameter.DomainFullPathName == "")
                    {
                        MessageBox.Show("设备全名为空", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (!new Regex(JsonInterFace.LteDeviceParameter.DeviceName).Match(JsonInterFace.LteDeviceParameter.DomainFullPathName).Success)
                    {
                        MessageBox.Show("设备全名称不正确：[" + JsonInterFace.LteDeviceParameter.DomainFullPathName + "],请选择设备！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (JsonInterFace.LteDeviceParameter.DeviceName == "")
                    {
                        MessageBox.Show("设备名称为空", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (JsonInterFace.LteDeviceParameter.IpAddr == "")
                    {
                        MessageBox.Show("设备IP为空", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (JsonInterFace.LteDeviceParameter.Port == "")
                    {
                        MessageBox.Show("设备端口为空", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (JsonInterFace.LteDeviceParameter.InnerType == "")
                    {
                        MessageBox.Show("设备[InnerType]为空", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (JsonInterFace.LteDeviceParameter.SN == "")
                    {
                        MessageBox.Show("设备[SN]为空", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    SubWindow.WhiteListSelfLearningSettingWindow WhiteListSelfLearningSettingWin = new SubWindow.WhiteListSelfLearningSettingWindow();
                    WhiteListSelfLearningSettingWin.RemoteAPInfoParameter.DomainFullPathName = JsonInterFace.LteDeviceParameter.DomainFullPathName;
                    WhiteListSelfLearningSettingWin.RemoteAPInfoParameter.DeviceName = JsonInterFace.LteDeviceParameter.DeviceName;
                    WhiteListSelfLearningSettingWin.RemoteAPInfoParameter.IP = JsonInterFace.LteDeviceParameter.IpAddr;
                    WhiteListSelfLearningSettingWin.RemoteAPInfoParameter.Port = JsonInterFace.LteDeviceParameter.Port;
                    WhiteListSelfLearningSettingWin.RemoteAPInfoParameter.InnerType = JsonInterFace.LteDeviceParameter.InnerType;
                    WhiteListSelfLearningSettingWin.RemoteAPInfoParameter.SN = JsonInterFace.LteDeviceParameter.SN;
                    WhiteListSelfLearningSettingWin.ShowDialog();
                }
                else
                {
                    MessageBox.Show("该设备不在线！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// 右键选择结点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T))
                source = VisualTreeHelper.GetParent(source);

            return source;
        }

        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                var treeViewItem = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
                if (treeViewItem != null)
                {
                    treeViewItem.Focus();
                    e.Handled = true;
                    eButton = e;
                    OldTreeViewItem = treeViewItem;
                }
                else
                {
                    if (OldTreeViewItem != null && eButton != null)
                    {
                        OldTreeViewItem.IsSelected = false;
                        eButton.Handled = false;
                        OldTreeViewItem = null;
                        eButton = null;
                    }
                }

                if (treeViewItem == null && sender != null)
                {
                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[0]).IsEnabled = false;
                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[1]).IsEnabled = false;
                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[2]).IsEnabled = false;
                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[3]).IsEnabled = false;
                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[4]).IsEnabled = false;

                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[6]).IsEnabled = false;
                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[7]).IsEnabled = false;
                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[8]).IsEnabled = false;

                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[10]).IsEnabled = false;
                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[11]).IsEnabled = false;
                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[12]).IsEnabled = false;
                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[13]).IsEnabled = false;

                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[15]).IsEnabled = true;
                    return;
                }

                //右键时坐标
                Parameters.UserMousePosition.X = e.GetPosition((sender as TreeViewItem)).X;
                Parameters.UserMousePosition.Y = e.GetPosition((sender as TreeViewItem)).Y;

                if (DeviceListTreeView.SelectedItem == null)
                {
                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[0]).IsEnabled = false;
                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[1]).IsEnabled = false;
                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[2]).IsEnabled = false;
                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[3]).IsEnabled = false;
                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[4]).IsEnabled = false;

                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[6]).IsEnabled = false;
                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[7]).IsEnabled = false;
                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[8]).IsEnabled = false;

                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[10]).IsEnabled = false;
                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[11]).IsEnabled = false;
                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[12]).IsEnabled = false;
                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[13]).IsEnabled = false;

                    ((MenuItem)DeviceListTreeView.ContextMenu.Items[15]).IsEnabled = true;
                }
                else if (DeviceListTreeView.SelectedItem != null)
                {
                    //选择设备树元素
                    DeveiceListTreeView_PreviewMouseLeftButtonUp(sender, e);

                    string SelectNodteType = (DeviceListTreeView.SelectedItem as DataInterface.CheckBoxTreeModel).SelfNodeType;

                    if (SelectNodteType == (NodeType.RootNode.ToString()))
                    {
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[0]).IsEnabled = true;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[1]).IsEnabled = true;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[2]).IsEnabled = true;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[3]).IsEnabled = true;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[4]).IsEnabled = true;

                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[6]).IsEnabled = false;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[7]).IsEnabled = false;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[8]).IsEnabled = false;

                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[10]).IsEnabled = true;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[11]).IsEnabled = false;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[12]).IsEnabled = false;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[13]).IsEnabled = false;

                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[15]).IsEnabled = true;
                    }
                    else if (SelectNodteType == (NodeType.StructureNode.ToString()))
                    {
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[0]).IsEnabled = true;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[1]).IsEnabled = true;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[2]).IsEnabled = true;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[3]).IsEnabled = true;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[4]).IsEnabled = true;

                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[6]).IsEnabled = false;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[7]).IsEnabled = false;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[8]).IsEnabled = false;

                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[10]).IsEnabled = true;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[11]).IsEnabled = true;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[12]).IsEnabled = true;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[13]).IsEnabled = false;

                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[15]).IsEnabled = true;
                    }
                    else if (SelectNodteType == (NodeType.LeafNode.ToString()))
                    {

                        if (DeviceType.GSM == Model)
                        {
                            ((MenuItem)DeviceListTreeView.ContextMenu.Items[2]).IsEnabled = false;
                        }
                        else
                        {
                            ((MenuItem)DeviceListTreeView.ContextMenu.Items[2]).IsEnabled = true;
                        }
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[0]).IsEnabled = true;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[1]).IsEnabled = true;

                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[3]).IsEnabled = false;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[4]).IsEnabled = false;

                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[6]).IsEnabled = true;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[7]).IsEnabled = true;


                        if (DeviceType.GSM == Model || DeviceType.CDMA == Model || DeviceType.GSMV2 == Model)
                        {
                            ((MenuItem)DeviceListTreeView.ContextMenu.Items[8]).IsEnabled = false;
                        }
                        else
                        {
                            ((MenuItem)DeviceListTreeView.ContextMenu.Items[8]).IsEnabled = true;
                        }

                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[10]).IsEnabled = false;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[11]).IsEnabled = true;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[12]).IsEnabled = true;

                        if (new Regex(DeviceType.LTE).Match(Model).Success)
                        {
                            ((MenuItem)DeviceListTreeView.ContextMenu.Items[13]).IsEnabled = true;
                        }
                        else
                        {
                            ((MenuItem)DeviceListTreeView.ContextMenu.Items[13]).IsEnabled = false;
                        }
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[15]).IsEnabled = true;
                    }
                }
                #region 权限 
                if (RoleTypeClass.RoleType.Equals("RoleType"))
                {

                }
                else
                {
                    if (int.Parse(RoleTypeClass.RoleType) > 2)
                    {
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[8]).Visibility = System.Windows.Visibility.Collapsed;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[10]).Visibility = System.Windows.Visibility.Collapsed;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[11]).Visibility = System.Windows.Visibility.Collapsed;
                    }
                    if (int.Parse(RoleTypeClass.RoleType) > 3)
                    {
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[0]).Visibility = System.Windows.Visibility.Collapsed;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[1]).Visibility = System.Windows.Visibility.Collapsed;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[2]).Visibility = System.Windows.Visibility.Collapsed;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[3]).Visibility = System.Windows.Visibility.Collapsed;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[4]).Visibility = System.Windows.Visibility.Collapsed;


                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[11]).Visibility = System.Windows.Visibility.Collapsed;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[12]).Visibility = System.Windows.Visibility.Collapsed;
                        ((MenuItem)DeviceListTreeView.ContextMenu.Items[13]).Visibility = System.Windows.Visibility.Collapsed;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("设备列表属性异常", ex.Message, ex.StackTrace);
                MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeveiceListTreeView_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                lock (JsonInterFace.UsrdomainDataLock)
                {
                    if (DeviceListTreeView.SelectedItem != null)
                    {
                        if (((CheckBoxTreeModel)DeviceListTreeView.SelectedItem).IsChecked)
                        {
                            ((CheckBoxTreeModel)DeviceListTreeView.SelectedItem).IsChecked = false;
                        }
                        SelfID = (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).Id;
                        ParentID = (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).ParentID;
                        SelfName = (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).Name;
                        FullName = (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).FullName;
                        Model = (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).Mode;
                        IsOnline = (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).IsOnLine;
                        IsStation = (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).IsStation;
                        ApVersion = (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).ApVersion;
                        Des = (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).Des;
                        Parameters.ConfigType = Model;

                        if (IsStation == "1")
                        {
                            BatchRebootAPList.StationFullName = FullName;
                            List<CheckBoxTreeModel> TmpData = new List<CheckBoxTreeModel>();
                            AutoGetAllDeviceDitailList(JsonInterFace.UsrdomainData, BatchRebootAPList.StationFullName, ref TmpData);
                            BatchRebootAPList.DeviceNameList = TmpData;
                        }
                        else
                        {
                            BatchRebootAPList.DeviceNameList.Clear();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("选取设备列表", ex.Message, ex.StackTrace);
                MessageBox.Show("选取设备列表，" + ex.Message + "！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SelectedDeviceInfoParaReseting()
        {
            MainWindow.aDeviceSelected.SelfID = string.Empty;
            MainWindow.aDeviceSelected.ParentID = string.Empty;
            MainWindow.aDeviceSelected.SelfName = string.Empty;
            MainWindow.aDeviceSelected.LongFullNamePath = string.Empty;
            MainWindow.aDeviceSelected.ShortFullNamePath = string.Empty;
            MainWindow.aDeviceSelected.SN = string.Empty;
            MainWindow.aDeviceSelected.Des = string.Empty;
            MainWindow.aDeviceSelected.IsOnline = false;
            MainWindow.aDeviceSelected.IsStation = string.Empty;
            MainWindow.aDeviceSelected.InnerType = string.Empty;
            MainWindow.aDeviceSelected.SelfIP = string.Empty;
            MainWindow.aDeviceSelected.SelfNetMask = string.Empty;
            MainWindow.aDeviceSelected.SelfPort = 0;
        }

        public static void SelectedDeviceParameters()
        {
            string DomainFullPathName = string.Empty;
            string[] DomainFullName = FullName.ToString().Split(new char[] { '.' });
            for (int j = 0; j < DomainFullName.Length - 1; j++)
            {
                if (DomainFullPathName == "" || DomainFullPathName == null)
                {
                    DomainFullPathName = DomainFullName[j];
                }
                else
                {
                    DomainFullPathName += "." + DomainFullName[j];
                }
            }

            for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
            {
                if (JsonInterFace.APATTributesLists[i].ParentID.Equals(ParentID)
                    && JsonInterFace.APATTributesLists[i].FullName.Equals(FullName)
                   )
                {
                    if (new Regex(DeviceType.LTE).Match(JsonInterFace.APATTributesLists[i].Mode).Success)
                    {
                        JsonInterFace.LteDeviceParameter.SelfID = JsonInterFace.APATTributesLists[i].SelfID;
                        JsonInterFace.LteDeviceParameter.ParentID = JsonInterFace.APATTributesLists[i].ParentID;
                        JsonInterFace.LteDeviceParameter.DeviceName = JsonInterFace.APATTributesLists[i].SelfName;
                        JsonInterFace.LteDeviceParameter.DomainFullPathName = JsonInterFace.APATTributesLists[i].FullName;

                        string[] DomainFullNameTmp = JsonInterFace.APATTributesLists[i].FullName.Split(new char[] { '.' });
                        JsonInterFace.LteDeviceParameter.Station = DomainFullNameTmp[DomainFullNameTmp.Length - 2];
                        JsonInterFace.LteDeviceParameter.DeviceMode = JsonInterFace.APATTributesLists[i].Mode;
                        JsonInterFace.LteDeviceParameter.OnLine = JsonInterFace.APATTributesLists[i].OnLine;
                        JsonInterFace.LteDeviceParameter.InnerType = JsonInterFace.APATTributesLists[i].InnerType;
                        JsonInterFace.LteDeviceParameter.StaticIPMode = true;                       //没有返回值
                        JsonInterFace.LteDeviceParameter.DynamicIPMode = false;                     //没有返回值

                        JsonInterFace.LteDeviceParameter.IpAddr = JsonInterFace.APATTributesLists[i].IpAddr;
                        JsonInterFace.LteDeviceParameter.Port = JsonInterFace.APATTributesLists[i].Port;
                        JsonInterFace.LteDeviceParameter.NetMask = JsonInterFace.APATTributesLists[i].NetMask;
                        JsonInterFace.LteDeviceParameter.SN = JsonInterFace.APATTributesLists[i].SN;
                        JsonInterFace.LteDeviceParameter.DeviceIdentificationMode = "设备名称";      //没有返回值
                    }
                    else if (DeviceType.GSMV2 == JsonInterFace.APATTributesLists[i].Mode)
                    {
                        JsonInterFace.GSMV2DeviceParameter.SelfID = JsonInterFace.APATTributesLists[i].SelfID;
                        JsonInterFace.GSMV2DeviceParameter.ParentID = JsonInterFace.APATTributesLists[i].ParentID;
                        JsonInterFace.GSMV2DeviceParameter.DeviceName = JsonInterFace.APATTributesLists[i].SelfName;
                        JsonInterFace.GSMV2DeviceParameter.DomainFullPathName = DomainFullPathName;

                        string[] DomainFullNameTmp = JsonInterFace.APATTributesLists[i].FullName.Split(new char[] { '.' });
                        JsonInterFace.GSMV2DeviceParameter.Station = DomainFullNameTmp[DomainFullNameTmp.Length - 2];
                        JsonInterFace.GSMV2DeviceParameter.DeviceMode = JsonInterFace.APATTributesLists[i].Mode;
                        JsonInterFace.GSMV2DeviceParameter.InnerType = JsonInterFace.APATTributesLists[i].InnerType;
                        JsonInterFace.GSMV2DeviceParameter.StaticIPMode = true;                     //没有返回值
                        JsonInterFace.GSMV2DeviceParameter.DynamicIPMode = false;                   //没有返回值

                        JsonInterFace.GSMV2DeviceParameter.IpAddr = JsonInterFace.APATTributesLists[i].IpAddr;
                        JsonInterFace.GSMV2DeviceParameter.Port = JsonInterFace.APATTributesLists[i].Port;
                        JsonInterFace.GSMV2DeviceParameter.NetMask = JsonInterFace.APATTributesLists[i].NetMask;
                        JsonInterFace.GSMV2DeviceParameter.SN = JsonInterFace.APATTributesLists[i].SN;
                        JsonInterFace.GSMV2DeviceParameter.DeviceIdentificationMode = "设备名称";    //没有返回值
                    }
                    else if (DeviceType.GSM == JsonInterFace.APATTributesLists[i].Mode)
                    {
                        JsonInterFace.GSMDeviceParameter.SelfID = JsonInterFace.APATTributesLists[i].SelfID;
                        JsonInterFace.GSMDeviceParameter.ParentID = JsonInterFace.APATTributesLists[i].ParentID;
                        JsonInterFace.GSMDeviceParameter.DeviceName = JsonInterFace.APATTributesLists[i].SelfName;
                        JsonInterFace.GSMDeviceParameter.DomainFullPathName = DomainFullPathName;

                        string[] DomainFullNameTmp = JsonInterFace.APATTributesLists[i].FullName.Split(new char[] { '.' });
                        JsonInterFace.GSMDeviceParameter.Station = DomainFullNameTmp[DomainFullNameTmp.Length - 2];
                        JsonInterFace.GSMDeviceParameter.DeviceMode = JsonInterFace.APATTributesLists[i].Mode;
                        JsonInterFace.GSMDeviceParameter.InnerType = JsonInterFace.APATTributesLists[i].InnerType;
                        JsonInterFace.GSMDeviceParameter.StaticIPMode = true;                      //没有返回值
                        JsonInterFace.GSMDeviceParameter.DynamicIPMode = false;                    //没有返回值

                        JsonInterFace.GSMDeviceParameter.IpAddr = JsonInterFace.APATTributesLists[i].IpAddr;
                        JsonInterFace.GSMDeviceParameter.Port = JsonInterFace.APATTributesLists[i].Port;
                        JsonInterFace.GSMDeviceParameter.NetMask = JsonInterFace.APATTributesLists[i].NetMask;
                        JsonInterFace.GSMDeviceParameter.SN = JsonInterFace.APATTributesLists[i].SN;
                        JsonInterFace.GSMDeviceParameter.DeviceIdentificationMode = "设备名称";     //没有返回值
                    }
                    else if (DeviceType.WCDMA == JsonInterFace.APATTributesLists[i].Mode)
                    {
                        JsonInterFace.WCDMADeviceParameter.SelfID = JsonInterFace.APATTributesLists[i].SelfID;
                        JsonInterFace.WCDMADeviceParameter.ParentID = JsonInterFace.APATTributesLists[i].ParentID;
                        JsonInterFace.WCDMADeviceParameter.DeviceName = JsonInterFace.APATTributesLists[i].SelfName;
                        JsonInterFace.WCDMADeviceParameter.DomainFullPathName = JsonInterFace.APATTributesLists[i].FullName;

                        string[] DomainFullNameTmp = JsonInterFace.APATTributesLists[i].FullName.Split(new char[] { '.' });
                        JsonInterFace.WCDMADeviceParameter.Station = DomainFullNameTmp[DomainFullNameTmp.Length - 2];
                        JsonInterFace.WCDMADeviceParameter.DeviceMode = JsonInterFace.APATTributesLists[i].Mode;
                        JsonInterFace.WCDMADeviceParameter.InnerType = JsonInterFace.APATTributesLists[i].InnerType;
                        JsonInterFace.WCDMADeviceParameter.StaticIPMode = true;                     //没有返回值
                        JsonInterFace.WCDMADeviceParameter.DynamicIPMode = false;                   //没有返回值

                        JsonInterFace.WCDMADeviceParameter.IpAddr = JsonInterFace.APATTributesLists[i].IpAddr;
                        JsonInterFace.WCDMADeviceParameter.Port = JsonInterFace.APATTributesLists[i].Port;
                        JsonInterFace.WCDMADeviceParameter.NetMask = JsonInterFace.APATTributesLists[i].NetMask;
                        JsonInterFace.WCDMADeviceParameter.SN = JsonInterFace.APATTributesLists[i].SN;
                        JsonInterFace.WCDMADeviceParameter.DeviceIdentificationMode = "设备名称";    //没有返回值
                    }
                    else if (DeviceType.CDMA == JsonInterFace.APATTributesLists[i].Mode)
                    {
                        JsonInterFace.CDMADeviceParameter.SelfID = JsonInterFace.APATTributesLists[i].SelfID;
                        JsonInterFace.CDMADeviceParameter.ParentID = JsonInterFace.APATTributesLists[i].ParentID;
                        JsonInterFace.CDMADeviceParameter.DeviceName = JsonInterFace.APATTributesLists[i].SelfName;
                        JsonInterFace.CDMADeviceParameter.DomainFullPathName = JsonInterFace.APATTributesLists[i].FullName;

                        string[] DomainFullNameTmp = JsonInterFace.APATTributesLists[i].FullName.Split(new char[] { '.' });
                        JsonInterFace.CDMADeviceParameter.Station = DomainFullNameTmp[DomainFullNameTmp.Length - 2];
                        JsonInterFace.CDMADeviceParameter.DeviceMode = JsonInterFace.APATTributesLists[i].Mode;
                        JsonInterFace.CDMADeviceParameter.InnerType = JsonInterFace.APATTributesLists[i].InnerType;
                        JsonInterFace.CDMADeviceParameter.StaticIPMode = true;                     //没有返回值
                        JsonInterFace.CDMADeviceParameter.DynamicIPMode = false;                   //没有返回值

                        JsonInterFace.CDMADeviceParameter.IpAddr = JsonInterFace.APATTributesLists[i].IpAddr;
                        JsonInterFace.CDMADeviceParameter.Port = JsonInterFace.APATTributesLists[i].Port;
                        JsonInterFace.CDMADeviceParameter.NetMask = JsonInterFace.APATTributesLists[i].NetMask;
                        JsonInterFace.CDMADeviceParameter.SN = JsonInterFace.APATTributesLists[i].SN;
                        JsonInterFace.CDMADeviceParameter.DeviceIdentificationMode = "设备名称";    //没有返回值
                    }
                    else if (DeviceType.TD_SCDMA == JsonInterFace.APATTributesLists[i].Mode)
                    {
                        JsonInterFace.TDSDeviceParameter.SelfID = JsonInterFace.APATTributesLists[i].SelfID;
                        JsonInterFace.TDSDeviceParameter.ParentID = JsonInterFace.APATTributesLists[i].ParentID;
                        JsonInterFace.TDSDeviceParameter.DeviceName = JsonInterFace.APATTributesLists[i].SelfName;
                        JsonInterFace.TDSDeviceParameter.DomainFullPathName = JsonInterFace.APATTributesLists[i].FullName;

                        string[] DomainFullNameTmp = JsonInterFace.APATTributesLists[i].FullName.Split(new char[] { '.' });
                        JsonInterFace.TDSDeviceParameter.Station = DomainFullNameTmp[DomainFullNameTmp.Length - 2];
                        JsonInterFace.TDSDeviceParameter.DeviceMode = JsonInterFace.APATTributesLists[i].Mode;
                        JsonInterFace.TDSDeviceParameter.InnerType = JsonInterFace.APATTributesLists[i].InnerType;
                        JsonInterFace.TDSDeviceParameter.StaticIPMode = true;                     //没有返回值
                        JsonInterFace.TDSDeviceParameter.DynamicIPMode = false;                   //没有返回值

                        JsonInterFace.TDSDeviceParameter.IpAddr = JsonInterFace.APATTributesLists[i].IpAddr;
                        JsonInterFace.TDSDeviceParameter.Port = JsonInterFace.APATTributesLists[i].Port;
                        JsonInterFace.TDSDeviceParameter.NetMask = JsonInterFace.APATTributesLists[i].NetMask;
                        JsonInterFace.TDSDeviceParameter.SN = JsonInterFace.APATTributesLists[i].SN;
                        JsonInterFace.TDSDeviceParameter.DeviceIdentificationMode = "设备名称";    //没有返回值
                    }
                    break;
                }
                else
                {
                    //LTE
                    JsonInterFace.LteDeviceParameter.SelfID = null;
                    JsonInterFace.LteDeviceParameter.ParentID = null;
                    JsonInterFace.LteDeviceParameter.DeviceName = null;
                    JsonInterFace.LteDeviceParameter.DomainFullPathName = null;

                    JsonInterFace.LteDeviceParameter.Station = null;
                    JsonInterFace.LteDeviceParameter.DeviceMode = null;
                    JsonInterFace.LteDeviceParameter.OnLine = null;
                    JsonInterFace.LteDeviceParameter.InnerType = null;
                    JsonInterFace.LteDeviceParameter.StaticIPMode = true;                        //没有返回值
                    JsonInterFace.LteDeviceParameter.DynamicIPMode = false;                      //没有返回值

                    JsonInterFace.LteDeviceParameter.IpAddr = null;
                    JsonInterFace.LteDeviceParameter.Port = null;
                    JsonInterFace.LteDeviceParameter.NetMask = null;
                    JsonInterFace.LteDeviceParameter.SN = null;
                    JsonInterFace.LteDeviceParameter.DeviceIdentificationMode = null;            //没有返回值

                    //GSM
                    JsonInterFace.GSMDeviceParameter.SelfID = null;
                    JsonInterFace.GSMDeviceParameter.ParentID = null;
                    JsonInterFace.GSMDeviceParameter.DeviceName = null;
                    JsonInterFace.GSMDeviceParameter.DomainFullPathName = null;

                    JsonInterFace.GSMDeviceParameter.Station = null;
                    JsonInterFace.GSMDeviceParameter.DeviceMode = null;
                    JsonInterFace.GSMDeviceParameter.InnerType = null;
                    JsonInterFace.GSMDeviceParameter.StaticIPMode = true;                        //没有返回值
                    JsonInterFace.GSMDeviceParameter.DynamicIPMode = false;                      //没有返回值

                    JsonInterFace.GSMDeviceParameter.IpAddr = null;
                    JsonInterFace.GSMDeviceParameter.Port = null;
                    JsonInterFace.GSMDeviceParameter.NetMask = null;
                    JsonInterFace.GSMDeviceParameter.SN = null;
                    JsonInterFace.GSMDeviceParameter.DeviceIdentificationMode = null;           //没有返回值

                    //CDMA
                    JsonInterFace.CDMADeviceParameter.SelfID = null;
                    JsonInterFace.CDMADeviceParameter.ParentID = null;
                    JsonInterFace.CDMADeviceParameter.DeviceName = null;
                    JsonInterFace.CDMADeviceParameter.DomainFullPathName = null;

                    JsonInterFace.CDMADeviceParameter.Station = null;
                    JsonInterFace.CDMADeviceParameter.DeviceMode = null;
                    JsonInterFace.CDMADeviceParameter.InnerType = null;
                    JsonInterFace.CDMADeviceParameter.StaticIPMode = true;                        //没有返回值
                    JsonInterFace.CDMADeviceParameter.DynamicIPMode = false;                      //没有返回值

                    JsonInterFace.CDMADeviceParameter.IpAddr = null;
                    JsonInterFace.CDMADeviceParameter.Port = null;
                    JsonInterFace.CDMADeviceParameter.NetMask = null;
                    JsonInterFace.CDMADeviceParameter.SN = null;
                    JsonInterFace.CDMADeviceParameter.DeviceIdentificationMode = null;           //没有返回值

                    //GSMV2
                    JsonInterFace.GSMV2DeviceParameter.SelfID = null;
                    JsonInterFace.GSMV2DeviceParameter.ParentID = null;
                    JsonInterFace.GSMV2DeviceParameter.DeviceName = null;
                    JsonInterFace.GSMV2DeviceParameter.DomainFullPathName = null;

                    JsonInterFace.GSMV2DeviceParameter.Station = null;
                    JsonInterFace.GSMV2DeviceParameter.DeviceMode = null;
                    JsonInterFace.GSMV2DeviceParameter.InnerType = null;
                    JsonInterFace.GSMV2DeviceParameter.StaticIPMode = true;                        //没有返回值
                    JsonInterFace.GSMV2DeviceParameter.DynamicIPMode = false;                      //没有返回值

                    JsonInterFace.GSMV2DeviceParameter.IpAddr = null;
                    JsonInterFace.GSMV2DeviceParameter.Port = null;
                    JsonInterFace.GSMV2DeviceParameter.NetMask = null;
                    JsonInterFace.GSMV2DeviceParameter.SN = null;
                    JsonInterFace.GSMV2DeviceParameter.DeviceIdentificationMode = null;           //没有返回值

                    //WCDMA
                    JsonInterFace.WCDMADeviceParameter.SelfID = null;
                    JsonInterFace.WCDMADeviceParameter.ParentID = null;
                    JsonInterFace.WCDMADeviceParameter.DeviceName = null;
                    JsonInterFace.WCDMADeviceParameter.DomainFullPathName = null;

                    JsonInterFace.WCDMADeviceParameter.Station = null;
                    JsonInterFace.WCDMADeviceParameter.DeviceMode = null;
                    JsonInterFace.WCDMADeviceParameter.OnLine = null;
                    JsonInterFace.WCDMADeviceParameter.InnerType = null;
                    JsonInterFace.WCDMADeviceParameter.StaticIPMode = true;                        //没有返回值
                    JsonInterFace.WCDMADeviceParameter.DynamicIPMode = false;                      //没有返回值

                    JsonInterFace.WCDMADeviceParameter.IpAddr = null;
                    JsonInterFace.WCDMADeviceParameter.Port = null;
                    JsonInterFace.WCDMADeviceParameter.NetMask = null;
                    JsonInterFace.WCDMADeviceParameter.SN = null;
                    JsonInterFace.WCDMADeviceParameter.DeviceIdentificationMode = null;            //没有返回值

                    //TDS
                    JsonInterFace.TDSDeviceParameter.SelfID = null;
                    JsonInterFace.TDSDeviceParameter.ParentID = null;
                    JsonInterFace.TDSDeviceParameter.DeviceName = null;
                    JsonInterFace.TDSDeviceParameter.DomainFullPathName = null;

                    JsonInterFace.TDSDeviceParameter.Station = null;
                    JsonInterFace.TDSDeviceParameter.DeviceMode = null;
                    JsonInterFace.TDSDeviceParameter.OnLine = null;
                    JsonInterFace.TDSDeviceParameter.InnerType = null;
                    JsonInterFace.TDSDeviceParameter.StaticIPMode = true;                        //没有返回值
                    JsonInterFace.TDSDeviceParameter.DynamicIPMode = false;                      //没有返回值

                    JsonInterFace.TDSDeviceParameter.IpAddr = null;
                    JsonInterFace.TDSDeviceParameter.Port = null;
                    JsonInterFace.TDSDeviceParameter.NetMask = null;
                    JsonInterFace.TDSDeviceParameter.SN = null;
                    JsonInterFace.TDSDeviceParameter.DeviceIdentificationMode = null;            //没有返回值
                }
            }
        }

        //重载设备
        private void miReloadDeviceList_Click(object sender, RoutedEventArgs e)
        {
            if (NetWorkClient.ControllerServer.Connected)
            {
                SelectedDeviceParameters();
                ReloadDeviceList(true);
            }
            else
            {
                MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        //重新加载设备列表
        public static void ReloadDeviceList(bool Action)
        {
            if (Action)
            {
                if (MessageBox.Show("确定重载设备列表？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                {
                    return;
                }
            }

            JsonInterFace.ActionResultStatus.Finished = true;

            JsonInterFace.APATTributesLists.Clear();
            JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows.Clear();
            JsonInterFace.UsrdomainData.Clear();

            JsonInterFace.DeviceListRequestCompleteStatus.ValueLoading = 0;
            JsonInterFace.DeviceListRequestCompleteStatus.LoadingWindowStatu = Visibility.Visible;
            MainWindow.GetDeviceListTimer.Start();
        }

        private void UnknonwDeviceTipsBlock_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                SubWindow.UnKnownDeviceListsControlWindow UnKnownDeviceListsControlWin = new SubWindow.UnKnownDeviceListsControlWindow();
                UnKnownDeviceListsControlWin.Show();
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("打开未知设备管理", Ex.Message, Ex.StackTrace);
            }
        }

        /// <summary>
        /// 获取上次正确的配置
        /// </summary>
        public static void GettingGSMLastConfigure()
        {
            new Thread(() =>
            {
                for (int i = 0; i < CarrierList.Count; i++)
                {
                    NetWorkClient.ControllerServer.Send(
                                                        JsonInterFace.GetGSMCarrierOneGenParaRequest(
                                                                                                      JsonInterFace.GSMDeviceParameter.DomainFullPathName,
                                                                                                      JsonInterFace.GSMDeviceParameter.DeviceName,
                                                                                                      CarrierList[i]
                                                                                                    )
                                                       );

                    if (CarrierList.Count > 1)
                    {
                        Thread.Sleep(1000);
                    }
                }
                CarrierList.Clear();
            }).Start();
        }

        private void DeviceListTreeView_PreviewMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DeviceListTreeView.SelectedItem != null)
            {
                string SelectedFullName = (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).FullName;
                string SelectedNodeType = (DeviceListTreeView.SelectedItem as CheckBoxTreeModel).SelfNodeType;

                //------后期采用这种方式------
                MainWindow.aDeviceSelected.SelfName = ((CheckBoxTreeModel)DeviceListTreeView.SelectedItem).Name;
                MainWindow.aDeviceSelected.LongFullNamePath = ((CheckBoxTreeModel)DeviceListTreeView.SelectedItem).FullName;
                string[] TmpFullNamePath = MainWindow.aDeviceSelected.LongFullNamePath.Split(new char[] { '.' });
                for (int i = 0; i < TmpFullNamePath.Length - 1; i++)
                {
                    if (MainWindow.aDeviceSelected.ShortFullNamePath == null || MainWindow.aDeviceSelected.ShortFullNamePath == "")
                    {
                        MainWindow.aDeviceSelected.ShortFullNamePath = TmpFullNamePath[i];
                    }
                    else
                    {
                        MainWindow.aDeviceSelected.ShortFullNamePath += "." + TmpFullNamePath[i];
                    }
                }
                MainWindow.aDeviceSelected.SelfID = ((CheckBoxTreeModel)DeviceListTreeView.SelectedItem).Id;
                MainWindow.aDeviceSelected.ParentID = ((CheckBoxTreeModel)DeviceListTreeView.SelectedItem).ParentID;
                MainWindow.aDeviceSelected.SelfNodeType = ((CheckBoxTreeModel)DeviceListTreeView.SelectedItem).SelfNodeType;
                MainWindow.aDeviceSelected.IsStation = ((CheckBoxTreeModel)DeviceListTreeView.SelectedItem).IsStation;
                MainWindow.aDeviceSelected.IsOnline = ((CheckBoxTreeModel)DeviceListTreeView.SelectedItem).IsOnLine;
                MainWindow.aDeviceSelected.Model = ((CheckBoxTreeModel)DeviceListTreeView.SelectedItem).Mode;

                MainWindow.aDeviceSelected.SN = ((CheckBoxTreeModel)DeviceListTreeView.SelectedItem).SN;
                MainWindow.aDeviceSelected.SelfIP = ((CheckBoxTreeModel)DeviceListTreeView.SelectedItem).IPAddr;
                MainWindow.aDeviceSelected.SelfNetMask = ((CheckBoxTreeModel)DeviceListTreeView.SelectedItem).NetMask;
                MainWindow.aDeviceSelected.SelfPort = ((CheckBoxTreeModel)DeviceListTreeView.SelectedItem).Port;
                MainWindow.aDeviceSelected.InnerType = ((CheckBoxTreeModel)DeviceListTreeView.SelectedItem).InnerType;
                //-------------------------

                if (FullName == SelectedFullName && SelectedNodeType == NodeType.LeafNode.ToString())
                {
                    miDeviceManage_Click(sender, new RoutedEventArgs());
                }
            }
        }

        private void SelfTreeViewItem_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MainWindow.ItemSelected = true;
        }

        private void SelfTreeViewItem_DragEnter(object sender, DragEventArgs e)
        {

        }

        private void SelfTreeViewItem_Drop(object sender, DragEventArgs e)
        {
            try
            {
                string IsStation = ((e.Source as TextBlock).DataContext as CheckBoxTreeModel).IsStation;
                string StationID = ((e.Source as TextBlock).DataContext as CheckBoxTreeModel).Id;
                string StationNodeName = ((e.Source as TextBlock).DataContext as CheckBoxTreeModel).FullName;
                if (IsStation != "1")
                {
                    MessageBox.Show("不接受该操作！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (StationNodeName == "" || StationNodeName == null || StationID == "" || StationID == null)
                {
                    MessageBox.Show("站点获取失败！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                List<UnKnownDeviceListsParameterClass> SubmitedUnknownDeviceLists = (e.Data.GetData(typeof(List<UnKnownDeviceListsParameterClass>)) as List<UnKnownDeviceListsParameterClass>);
                if (SubmitedUnknownDeviceLists != null)
                {
                    if (SubmitedUnknownDeviceLists.Count > 0)
                    {
                        for (int i = 0; i < SubmitedUnknownDeviceLists.Count; i++)
                        {
                            SubmitedUnknownDeviceLists[i].StationID = StationID;
                            SubmitedUnknownDeviceLists[i].ToStation = StationNodeName;
                        }

                        SubWindow.UnKnownDeviceListsControlWindow.SubmitedUnknownDeviceLists = SubmitedUnknownDeviceLists;


                        for (int i = 0; i < SubmitedUnknownDeviceLists.Count; i++)
                        {
                            if (DeviceType.UnknownType.ToLower() == SubmitedUnknownDeviceLists[i].DeviceName.ToLower())
                            {
                                MessageBox.Show("你选择包含了未命名的[未知设备]，请重命名后再继续！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }
                            else if (SubmitedUnknownDeviceLists[i].DeviceName == "" || SubmitedUnknownDeviceLists[i].DeviceName == null)
                            {
                                MessageBox.Show("你选择包含了空名称的[未知设备]，请重命名后再继续！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }
                        }

                        if (NetWorkClient.ControllerServer.Connected)
                        {
                            Parameters.SendMessage(Parameters.UnknownDeviceWinHandle, Parameters.WM_UnknownDeviceDragToAddMessage, 0, 0);
                        }
                        else
                        {
                            MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("要添加[未知设备]到站点[" + StationNodeName + "]，请先选择[未知设备...]项！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "要添加的[未知设备...]到站点[" + StationNodeName + "]", "未知设备不存在", "失败");
                    MessageBox.Show("不接受该操作！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception Ex)
            {
                JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "添加未知设备到站点,接收拖放数据内部异常," + Ex.Message, "接收数据", "内部故障");
                Parameters.PrintfLogsExtended("接收拖放数据内部异常", Ex.Message, Ex.StackTrace);
            }
        }

        private void UnknonwDeviceTipsBlockWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //缩放比列系数
            try
            {
                double scaleCoefficient = Math.Round(((UnknonwDeviceTipsBlockWindow.ActualHeight * UnknonwDeviceTipsBlockWindow.ActualWidth) / 2) / ((50 * 280) / 2), 2);
                Parameters.UnknownDeviceWindowControlParameters.ScaleElement(scaleCoefficient);
                UnknonwDeviceTipsBox.Width *= scaleCoefficient;
                UnknonwDeviceTipsBox.Height *= scaleCoefficient;
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("缩放设备提示器内部故障", Ex.Message, Ex.StackTrace);
            }
        }
    }
}
