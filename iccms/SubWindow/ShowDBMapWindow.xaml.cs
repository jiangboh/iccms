using DataInterface;
using iccms.NavigatePages;
using NetController;
using ParameterControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
    public class StructureInfoCalss : INotifyPropertyChanged
    {
        private string tmplng;
        private string tmplat;
        private string markerTitle;
        private string parentName;
        private bool isStation;
        private string des;
        private bool setExistence;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string value)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(value));
            }
        }

        public string Tmplng
        {
            get { return tmplng; }
            set
            {
                tmplng = value;
                NotifyPropertyChanged("Tmplng");
            }
        }
        public string Tmplat
        {
            get { return tmplat; }
            set
            {
                tmplat = value;
                NotifyPropertyChanged("Tmplat");
            }
        }
        public string MarkerTitle
        {
            get { return markerTitle; }
            set
            {
                markerTitle = value;
                NotifyPropertyChanged("MarkerTitle");
            }
        }
        public string ParentName
        {
            get { return parentName; }
            set
            {
                parentName = value;
                NotifyPropertyChanged("ParentName");
            }
        }
        public bool IsStation
        {
            get { return isStation; }
            set
            {
                isStation = value;
                NotifyPropertyChanged("IsStation");
            }
        }
        public string Des
        {
            get { return des; }
            set
            {
                des = value;
                NotifyPropertyChanged("Des");
            }
        }
        public bool SetExistence
        {
            get { return setExistence; }
            set
            {
                setExistence = value;
                NotifyPropertyChanged("SetExistence");
            }
        }
    }
    /// <summary>
    /// ShowDBMapWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ShowDBMapWindow : Window
    {
        //界面容器
        private Dictionary<string, Uri> SubWindows = new Dictionary<string, Uri>();

        [System.Runtime.InteropServices.ComVisible(true)]

        public static IList<CheckBoxTreeModel> UsrdomainData = new List<CheckBoxTreeModel>();
        private static object[] DeviceTreeViewItem = new object[2];
        private List<object> Overlay = new List<object>();
        private Thread AddStructureNodeThread = null;
        private StructureInfoCalss StructureInfo = new StructureInfoCalss();
        private bool ThreadFlag = true;

        public ShowDBMapWindow()
        {
            InitializeComponent();

            SubWindows.Add("StatisticsInfoWindow", new Uri("NavigatePages/StatisticsInfoWindow.xaml", UriKind.Relative));

            if (AddStructureNodeThread == null)
            {
                AddStructureNodeThread = new Thread(new ThreadStart(AddStructureNode));
            }
        }
        public void LoadDeviceListTreeView()
        {
            new Thread(() =>
            {
                BindCheckBoxTreeView devicetreeview = new BindCheckBoxTreeView();
                CheckBoxTreeModel treeModel = new CheckBoxTreeModel();
                devicetreeview.Dt = JsonInterFace.BindTreeViewClass.DeviceTreeTable;
                devicetreeview.DeviceTreeViewBind(ref treeModel);
                UsrdomainData.Clear();
                UsrdomainData.Add(treeModel);
            }).Start();
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FrmShowDBMapWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
        private void chkTreeViewItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //实现单选效果
                if ((DeviceTreeViewItem[0] as CheckBox) == null)
                {
                    DeviceTreeViewItem[0] = sender;
                }
                else
                {
                    if (sender != DeviceTreeViewItem[0])
                    {
                        ((CheckBoxTreeModel)(DeviceTreeViewItem[0] as CheckBox).DataContext).IsChecked = false;
                        (DeviceTreeViewItem[0] as CheckBox).IsChecked = false;

                        ((CheckBoxTreeModel)(sender as CheckBox).DataContext).IsChecked = true;
                        (sender as CheckBox).IsChecked = true;
                    }
                    DeviceTreeViewItem[0] = sender;
                }

                mapweb.InvokeScript("deleteAllMarker");
                mapweb.InvokeScript("existenceLngLat", new Object[] { true });
                string SelfID = ((CheckBoxTreeModel)(sender as CheckBox).DataContext).Id;
                string Model = ((CheckBoxTreeModel)(sender as CheckBox).DataContext).Mode;
                string NodeName = ((CheckBoxTreeModel)(sender as CheckBox).DataContext).Name;
                string FullName = ((CheckBoxTreeModel)(sender as CheckBox).DataContext).FullName;
                string IsStation = ((CheckBoxTreeModel)(sender as CheckBox).DataContext).IsStation;
                string SelfNodeType = ((CheckBoxTreeModel)(sender as CheckBox).DataContext).SelfNodeType;
                Boolean NodeChecked = ((CheckBoxTreeModel)(sender as CheckBox).DataContext).IsChecked;
                if (!NodeChecked)
                {
                    return;
                }
                if (NodeChecked && IsStation == "1" && SelfNodeType.Equals(NodeType.StructureNode.ToString()))
                {
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        string[] name = FullName.Split(new char[] { '.' });
                        string ParentName = string.Empty;
                        for (int i = 0; i < name.Length - 1; i++)
                        {
                            if (i == 0)
                                ParentName += name[i];
                            else
                                ParentName += "." + name[i];
                        }
                        NetWorkClient.ControllerServer.Send(JsonInterFace.Get_station_location_Request(ParentName, NodeName));
                    }
                    else
                    {
                        Parameters.PrintfLogsExtended("网络与服务器断开！", "Connected: Failed!");
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("百度地图显示", ex.Message, ex.StackTrace);
            }
        }

        private void FrmShowDBMapWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                dpStartTime.SelectedDate = DateTime.Now.Date;
                dpEndTime.SelectedDate = DateTime.Now.Date;
                txtStartHour.Text = "00";
                txtStartMinute.Text = "00";
                txtStartSecond.Text = "00";

                txtEndHour.Text = "23";
                txtEndMinute.Text = "59";
                txtEndSecond.Text = "59";

                tvSpecialListDeviceTree.ItemsSource = UsrdomainData;
                String sURL = AppDomain.CurrentDomain.BaseDirectory + "baiDuMap.html";
                Uri uri = new Uri(sURL);
                mapweb.Navigate(uri);
                txtLng.DataContext = StructureInfo;
                txtLat.DataContext = StructureInfo;

                //启动线程
                if (!AddStructureNodeThread.IsAlive)
                    AddStructureNodeThread.Start();
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("地图显示初始化", ex.Message, ex.StackTrace);
            }
        }

        private void FrmShowDBMapWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (btnClose.IsFocused)
                {
                    this.DragMove();
                }
            }
        }

        private void FrmShowDBMapWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (btnClose.IsFocused)
                {
                    this.DragMove();
                }
            }
        }

        private void btnSelectIMSI_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string imsi = string.Empty;
                string timeStart = string.Empty;
                string timeEnd = string.Empty;
                timeStart = Convert.ToDateTime(dpStartTime.Text).ToString("yyyy-MM-dd") + " "
                    + System.Int32.Parse(txtStartHour.Text.Trim()).ToString().PadLeft(2, '0') + ":"
                    + System.Int32.Parse(txtStartMinute.Text.Trim()).ToString().PadLeft(2, '0') + ":"
                    + System.Int32.Parse(txtStartSecond.Text.Trim()).ToString().PadLeft(2, '0');

                timeEnd = Convert.ToDateTime(dpEndTime.Text).ToString("yyyy-MM-dd") + " "
                    + System.Int32.Parse(txtEndHour.Text.Trim()).ToString().PadLeft(2, '0') + ":"
                    + System.Int32.Parse(txtEndMinute.Text.Trim()).ToString().PadLeft(2, '0') + ":"
                    + System.Int32.Parse(txtEndSecond.Text.Trim()).ToString().PadLeft(2, '0');
                imsi = txtIMSI.Text.Trim();
                if (NetWorkClient.ControllerServer.Connected)
                {
                    NetWorkClient.ControllerServer.Send(JsonInterFace.Get_imsi_path_Request(timeStart, timeEnd, imsi));
                }
                else
                {
                    Parameters.PrintfLogsExtended("网络与服务器断开！", "Connected: Failed!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("轨迹查询" + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            string marker = mapweb.InvokeScript("GetAllMarker").ToString();
            string[] listMarker = marker.Split(new char[] { '|' });
            for (int i = 0; i < listMarker.Length - 1; i++)
            {
                string point = listMarker[i];
            }
        }
        private void AddStructureNode()
        {
            Thread.Sleep(5000);
            while (true)
            {
                try
                {
                    if (ThreadFlag)
                    {
                        Dispatcher.Invoke(() =>
                        {

                            string Point = mapweb.InvokeScript("getPoint").ToString();
                            if (Point != null && Point.Length > 0)
                            {
                                StructureInfo.Tmplng = Point.Split(new char[] { ',' })[0];
                                StructureInfo.Tmplat = Point.Split(new char[] { ',' })[1];
                            }
                            string StructureInfostr = mapweb.InvokeScript("getPointInfo").ToString();
                            if (StructureInfostr != null && StructureInfostr.Length > 0)
                            {
                                if (StructureInfostr.Split(new char[] { ',' }).Length > 1)
                                {
                                    StructureInfo.ParentName = ((CheckBoxTreeModel)(DeviceTreeViewItem[0] as CheckBox).DataContext).FullName;
                                    StructureInfo.MarkerTitle = ((CheckBoxTreeModel)(DeviceTreeViewItem[0] as CheckBox).DataContext).Name;
                                    string[] name = StructureInfo.ParentName.Split(new char[] { '.' });
                                    string ParentName = string.Empty;
                                    for (int i = 0; i < name.Length - 1; i++)
                                    {
                                        if (i == 0)
                                            ParentName += name[i];
                                        else
                                            ParentName += "." + name[i];
                                    }
                                    StructureInfo.Tmplng = StructureInfostr.Split(new char[] { ',' })[0];
                                    StructureInfo.Tmplat = StructureInfostr.Split(new char[] { ',' })[1];
                                    StructureInfo.SetExistence = Convert.ToBoolean(StructureInfostr.Split(new char[] { ',' })[2]);
                                    if (NetWorkClient.ControllerServer.Connected)
                                    {
                                        //添加域(不重复实现)
                                        //NetWorkClient.ControllerServer.Send(JsonInterFace.AddDomainNodeName(StructureInfo.ParentName, StructureInfo.MarkerTitle, Convert.ToInt32(StructureInfo.IsStation), StructureInfo.Des));
                                        //如果添加站点，则保存位置
                                        if (StructureInfo.SetExistence)
                                            NetWorkClient.ControllerServer.Send(JsonInterFace.Set_station_location_Request(ParentName, StructureInfo.MarkerTitle, StructureInfo.Tmplng, StructureInfo.Tmplat));
                                    }
                                    else
                                    {
                                        Parameters.PrintfLogsExtended("网络与服务器断开！", "Connected: Failed!");
                                    }
                                }
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended("添加站点错误", ex.Message, ex.StackTrace);
                }
                Thread.Sleep(50);
            }
        }

        private void lblMapInfoCaption_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

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
        /// 
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            //地图上显示标注
            try
            {
                //地图上显示标注
                if (msg == Parameters.WM_StationLocationResponse)
                {
                    showMarkerOnMap();
                }
                //IMSI轨迹
                else if (msg == Parameters.WM_IMSIPathResponse)
                {
                    getIMSIPath();
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("Windows 消息事件：", ex.Message, ex.StackTrace);
            }
            return hwnd;
        }

        private void showMarkerOnMap()
        {
            string[] name = ((CheckBoxTreeModel)(DeviceTreeViewItem[0] as CheckBox).DataContext).FullName.Split(new char[] { '.' });
            string ParentName = string.Empty;
            for (int i = 0; i < name.Length - 1; i++)
            {
                if (i == 0)
                    ParentName += name[i];
                else
                    ParentName += "." + name[i];
            }
            object fullName = name[name.Length - 2] + "." + name[name.Length - 1];
            //设置提示框标题   域名+站点名
            mapweb.InvokeScript("setMarkerTitle", new Object[] { fullName });
            //不存在定位信息
            if ((JsonInterFace.StationMap.Lng == null || JsonInterFace.StationMap.Lng == "") &&
                (JsonInterFace.StationMap.Lat == null || JsonInterFace.StationMap.Lat == ""))
            {
                mapweb.InvokeScript("existenceLngLat", new Object[] { false });
            }
            else
            {
                object Jindd = JsonInterFace.StationMap.Lng;
                object Weidd = JsonInterFace.StationMap.Lat;
                mapweb.InvokeScript("setStructureNodePoint", new Object[] { Jindd, Weidd, fullName });
                Overlay.Add(Jindd + "," + Weidd);
            }
        }
        private void getIMSIPath()
        {
            try
            {
                //Random random = new Random();
                ////清楚地图上的标注
                //mapweb.InvokeScript("deleteAllMarker");
                //for (int i = 0; i < 10; i++)
                //{
                //    double jind = (double)random.Next(7340, 13523) / (double)100;
                //    double weid = (double)random.Next(2000, 5333) / (double)100;
                //    object Jindd = jind;
                //    object Weidd = weid;
                //    object fullName = "深圳市.南山区.飞亚达大厦";
                //    mapweb.InvokeScript("setIMSI", new Object[] { fullName, Jindd, Weidd });
                //    Thread.Sleep(100);
                //}
                //mapweb.InvokeScript("addOverlays");
                if (JsonInterFace.IMSIPath.DTIMSIPath.Rows.Count > 0)
                {
                    //清楚地图上的标注
                    mapweb.InvokeScript("deleteAllMarker");
                    for (int i = 0; i < JsonInterFace.IMSIPath.DTIMSIPath.Rows.Count; i++)
                    {
                        object fullName = JsonInterFace.IMSIPath.DTIMSIPath.Rows[i][1];
                        object Jindd = JsonInterFace.IMSIPath.DTIMSIPath.Rows[i][2];
                        object Weidd = JsonInterFace.IMSIPath.DTIMSIPath.Rows[i][3];
                        mapweb.InvokeScript("setIMSI", new Object[] { fullName, Jindd, Weidd });
                    }
                    mapweb.InvokeScript("addOverlays");
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }
        private void FrmShowDBMapWindow_Activated(object sender, EventArgs e)
        {
            WindowInteropHelper GetWindowHandleHelper = new WindowInteropHelper(this);
            Parameters.ShowDBMapWinHandle = GetWindowHandleHelper.Handle;
        }

        private void FrmShowDBMapWindow_Closed(object sender, EventArgs e)
        {
            try
            {
                mapweb.Dispose();
                ThreadFlag = false;
                AddStructureNodeThread.Abort();
                AddStructureNodeThread.Join();
                //AddStructureNodeThread = null;
            }
            catch(Exception ex)
            {
                Parameters.PrintfLogsExtended("退出地图功能异常：", ex.Message, ex.StackTrace);
            }
        }

        private void btnSetLngLat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (StructureInfo.ParentName != "" || StructureInfo.ParentName != null)
                {
                    mapweb.InvokeScript("deleteAllMarker");
                    mapweb.InvokeScript("setStructureNodePoint", new Object[] { StructureInfo.Tmplng, StructureInfo.Tmplat, StructureInfo.MarkerTitle });
                    var ParentName = string.Empty;
                    for (int i = 0; i < StructureInfo.ParentName.Split(new char[] { '.' }).Length - 1; i++)
                    {
                        if (i == 0)
                            ParentName += StructureInfo.ParentName.Split(new char[] { '.' })[i];
                        else
                            ParentName += "." + StructureInfo.ParentName.Split(new char[] { '.' })[i];
                    }
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        NetWorkClient.ControllerServer.Send(JsonInterFace.Set_station_location_Request(ParentName, StructureInfo.MarkerTitle, StructureInfo.Tmplng, StructureInfo.Tmplat));
                    }
                    else
                    {
                        Parameters.PrintfLogsExtended("网络与服务器断开！", "Connected: Failed!");
                    }
                }
                else
                {
                    MessageBox.Show("请先选择站点");
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("手动设置站点位置事件：", ex.Message, ex.StackTrace);
            }
        }

        private void txtStartHour_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                if ((this.isNum(tb.Text) == false) || (tb.Text.Length > 2))
                {
                    tb.Text = "00";
                    MessageBox.Show("请输入正确的时间！", "警告！");
                    return;
                }
            }
        }
        private bool isNum(string str)
        {
            bool ret = true;
            foreach (char c in str)
            {
                if ((c < 48) || (c > 57))
                {
                    return false;
                }
            }
            return ret;
        }

        private void txtStartHour_SelectionChanged(object sender, RoutedEventArgs e)
        {
            txtStartHour.Background = Brushes.DodgerBlue;
            txtStartMinute.Background = this.Background;
            txtStartSecond.Background = this.Background;
            txtStartHour.Tag = 1;
            txtStartMinute.Tag = 0;
            txtStartSecond.Tag = 0;
        }

        private void txtStartMinute_SelectionChanged(object sender, RoutedEventArgs e)
        {
            txtStartMinute.Background = Brushes.DodgerBlue;
            txtStartHour.Background = this.Background;
            txtStartSecond.Background = this.Background;
            txtStartHour.Tag = 0;
            txtStartMinute.Tag = 1;
            txtStartSecond.Tag = 0;
        }

        private void txtStartSecond_SelectionChanged(object sender, RoutedEventArgs e)
        {
            txtStartSecond.Background = Brushes.DodgerBlue;
            txtStartMinute.Background = this.Background;
            txtStartHour.Background = this.Background;
            txtStartHour.Tag = 0;
            txtStartMinute.Tag = 0;
            txtStartSecond.Tag = 1;
        }

        private void txtEndHour_SelectionChanged(object sender, RoutedEventArgs e)
        {
            txtEndHour.Background = Brushes.DodgerBlue;
            txtEndMinute.Background = this.Background;
            txtEndSecond.Background = this.Background;
            txtEndHour.Tag = 1;
            txtEndMinute.Tag = 0;
            txtEndSecond.Tag = 0;
        }

        private void txtEndMinute_SelectionChanged(object sender, RoutedEventArgs e)
        {
            txtEndMinute.Background = Brushes.DodgerBlue;
            txtEndHour.Background = this.Background;
            txtEndSecond.Background = this.Background;
            txtEndHour.Tag = 0;
            txtEndMinute.Tag = 1;
            txtEndSecond.Tag = 0;
        }

        private void txtEndSecond_SelectionChanged(object sender, RoutedEventArgs e)
        {
            txtEndSecond.Background = Brushes.DodgerBlue;
            txtEndMinute.Background = this.Background;
            txtEndHour.Background = this.Background;
            txtEndHour.Tag = 0;
            txtEndMinute.Tag = 0;
            txtEndSecond.Tag = 1;
        }

        private void btnStartup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.Parse(txtStartHour.Tag.ToString()) == 1)
                {
                    int temp = System.Int32.Parse(this.txtStartHour.Text);
                    temp++;
                    if (temp > 24)
                    {
                        temp = 0;
                    }
                    txtStartHour.Text = temp.ToString();
                }
                else if (int.Parse(txtStartMinute.Tag.ToString()) == 1)
                {
                    int temp = System.Int32.Parse(txtStartMinute.Text);
                    temp++;
                    if (temp > 60)
                    {
                        temp = 0;
                    }
                    txtStartMinute.Text = temp.ToString();
                }
                else if (int.Parse(txtStartSecond.Tag.ToString()) == 1)
                {
                    int temp = System.Int32.Parse(txtStartSecond.Text);
                    temp++;
                    if (temp > 60)
                    {
                        temp = 0;
                    }
                    txtStartSecond.Text = temp.ToString();
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void btnStartdown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.Parse(txtStartHour.Tag.ToString()) == 1)
                {
                    int temp = System.Int32.Parse(this.txtStartHour.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 24;
                    }
                    txtStartHour.Text = temp.ToString();
                }
                else if (int.Parse(txtStartMinute.Tag.ToString()) == 1)
                {
                    int temp = System.Int32.Parse(txtStartMinute.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 60;
                    }
                    txtStartMinute.Text = temp.ToString();
                }
                else if (int.Parse(txtStartSecond.Tag.ToString()) == 1)
                {
                    int temp = System.Int32.Parse(txtStartSecond.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 60;
                    }
                    txtStartSecond.Text = temp.ToString();
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void btnEndup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.Parse(txtEndHour.Tag.ToString()) == 1)
                {
                    int temp = System.Int32.Parse(this.txtEndHour.Text);
                    temp++;
                    if (temp > 24)
                    {
                        temp = 0;
                    }
                    txtEndHour.Text = temp.ToString();
                }
                else if (int.Parse(txtEndMinute.Tag.ToString()) == 1)
                {
                    int temp = System.Int32.Parse(txtEndMinute.Text);
                    temp++;
                    if (temp > 60)
                    {
                        temp = 0;
                    }
                    txtEndMinute.Text = temp.ToString();
                }
                else if (int.Parse(txtEndSecond.Tag.ToString()) == 1)
                {
                    int temp = System.Int32.Parse(txtEndSecond.Text);
                    temp++;
                    if (temp > 60)
                    {
                        temp = 0;
                    }
                    txtEndSecond.Text = temp.ToString();
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void btnEnddown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.Parse(txtEndHour.Tag.ToString()) == 1)
                {
                    int temp = System.Int32.Parse(this.txtEndHour.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 24;
                    }
                    txtEndHour.Text = temp.ToString();
                }
                else if (int.Parse(txtEndMinute.Tag.ToString()) == 1)
                {
                    int temp = System.Int32.Parse(txtEndMinute.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 60;
                    }
                    txtEndMinute.Text = temp.ToString();
                }
                else if (int.Parse(txtEndSecond.Tag.ToString()) == 1)
                {
                    int temp = System.Int32.Parse(txtEndSecond.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 60;
                    }
                    txtEndSecond.Text = temp.ToString();
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }
    }
}
