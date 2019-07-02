using DataInterface;
using NetController;
using ParameterControl;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace iccms.NavigatePages
{
    /// <summary>
    /// DataManage.xaml 的交互逻辑
    /// </summary>
    public partial class UECallInfoWindow : Window
    {
        private object UECallInfoLanguageClass = null;
        private static bool condition = false;
        private static GSMV2UEReportInfoClass UECallInfo = new GSMV2UEReportInfoClass();
        //数据更新后实时通知界面显示
        private static Thread ShowUECallInfoThread = null;
        private static ObservableCollection<GSMV2UEReportInfoClass> UECallInfoListTree = new ObservableCollection<GSMV2UEReportInfoClass>();
        private static ObservableCollection<GSMV2UEReportInfoClass> ConditionUECallInfoListTree = new ObservableCollection<GSMV2UEReportInfoClass>();
        public UECallInfoWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            if (Parameters.LanguageType.Equals("CN") || Parameters.LanguageType.Equals(""))
            {
                //UECallInfoLanguageClass = new Language_CN.QueryHistoricalData();
            }
            else if (Parameters.LanguageType.Equals("EN"))
            {
                //UECallInfoLanguageClass = new Language_EN.QueryHistoricalData();
            }
            
            this.textbox_minute.Background = Brushes.White;
            this.textbox_second.Background = Brushes.White;
            this.textbox_hour.Background = Brushes.White;
            this.dploreStartTime.SelectedDate = DateTime.Now.Date;
            initParameters();
            dgHistoryTable.Items.Clear();
            if (ShowUECallInfoThread == null)
            {
                ShowUECallInfoThread = new Thread(new ThreadStart(UECallInfoListInfo));
                ShowUECallInfoThread.Start();
            }
        }
        private void UECallInfoListInfo()
        {
            while (true)
            {
                try
                {
                    if (JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows.Count > 0) 
                    {
                        //存在查询条件
                        if (condition)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                for (int i = 0; i < JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows.Count; i++)
                                {
                                    bool existence = true;
                                    if (!UECallInfo.DomainFullPathName.Equals("") && 
                                        !UECallInfo.DomainFullPathName.Equals(JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["ParentFullPathName"].ToString()) ||
                                        !UECallInfo.BOrmType.Equals("") &&
                                        !UECallInfo.BOrmType.Equals(JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["BOrmType"].ToString()) ||
                                        !UECallInfo.BUeId.Equals("") &&
                                        !UECallInfo.BUeId.Equals(JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["BUeId"].ToString()))
                                    {
                                        existence = false;
                                    }

                                    if (existence)
                                    {
                                        bool Flag = true;
                                        GSMV2UEReportInfoClass UEReportInfo = new GSMV2UEReportInfoClass();
                                        UEReportInfo.ReportID = (ConditionUECallInfoListTree.Count + 1).ToString();
                                        UEReportInfo.DomainFullPathName = JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["ParentFullPathName"].ToString();
                                        UEReportInfo.DeviceName = JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["DeviceName"].ToString();
                                        UEReportInfo.BOrmType = JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["BOrmType"].ToString();
                                        UEReportInfo.BUeId = JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["BUeId"].ToString();
                                        UEReportInfo.CRSRP = JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["CRSRP"].ToString();
                                        UEReportInfo.Carrier = JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["Carrier"].ToString();
                                        UEReportInfo.DataTime = JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["DataTime"].ToString();
                                        UEReportInfo.BUeContent = JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["BUeContent"].ToString();
                                        for (int j = 0; j < ConditionUECallInfoListTree.Count; j++)
                                        {
                                            if(UEReportInfo.DomainFullPathName == ConditionUECallInfoListTree[j].DomainFullPathName &&
                                               UEReportInfo.DeviceName == ConditionUECallInfoListTree[j].DeviceName &&
                                               UEReportInfo.BOrmType == ConditionUECallInfoListTree[j].BOrmType &&
                                               UEReportInfo.BUeId == ConditionUECallInfoListTree[j].BUeId &&
                                               UEReportInfo.CRSRP == ConditionUECallInfoListTree[j].CRSRP &&
                                               UEReportInfo.Carrier == ConditionUECallInfoListTree[j].Carrier &&
                                               UEReportInfo.DataTime == ConditionUECallInfoListTree[j].DataTime &&
                                               UEReportInfo.BUeContent == ConditionUECallInfoListTree[j].BUeContent)
                                            {
                                                Flag = false;
                                                break;
                                            }
                                        }
                                        if (Flag)
                                        {
                                            ConditionUECallInfoListTree.Add(UEReportInfo);
                                        }
                                    }
                                }
                            });
                        }
                        //不存在查询条件
                        else
                        {
                            Dispatcher.Invoke(() =>
                            {
                                for (int i = 0; i < JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows.Count; i++)
                                {
                                    if (i == 0 && JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows.Count == UECallInfoListTree.Count)
                                    {
                                        break;
                                    }
                                    else if (i > UECallInfoListTree.Count - 1)
                                    {
                                        GSMV2UEReportInfoClass UEReportInfo = new GSMV2UEReportInfoClass();
                                        UEReportInfo.ReportID = (UECallInfoListTree.Count + 1).ToString();
                                        UEReportInfo.DomainFullPathName = JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["ParentFullPathName"].ToString();
                                        UEReportInfo.DeviceName = JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["DeviceName"].ToString();
                                        UEReportInfo.BOrmType = JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["BOrmType"].ToString();
                                        UEReportInfo.BUeId = JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["BUeId"].ToString();
                                        UEReportInfo.CRSRP = JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["CRSRP"].ToString();
                                        UEReportInfo.Carrier = JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["Carrier"].ToString();
                                        UEReportInfo.DataTime = JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["DataTime"].ToString();
                                        UEReportInfo.BUeContent = JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["BUeContent"].ToString();
                                        UECallInfoListTree.Add(UEReportInfo);
                                    }
                                }
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended("上报主叫信息异常", ex.Message, ex.StackTrace);
                }

                Thread.Sleep(5000);
            }
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
            try
            {
                
            }
            catch (Exception ex)
            {
                Parameters.printfLogs("Windows 消息事件：", ex.Message, ex.StackTrace);
            }
            return hwnd;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //中/英文初始化
            if (Parameters.LanguageType.Equals("CN") || Parameters.LanguageType.Equals(""))
            {
                this.DataContext = (Language_CN.QueryHistoricalData)UECallInfoLanguageClass;
            }
            else
            {
                this.DataContext = (Language_EN.QueryHistoricalData)UECallInfoLanguageClass;
                //txtQHD_ID.DataContext = new Language_EN.QueryHistoricalData();
            }
            //加载域
            addcbFullNameItems();
            //加载数据列表
            dgHistoryTable.ItemsSource = UECallInfoListTree;
            #region 权限
            if (RoleTypeClass.RoleType != null && RoleTypeClass.RoleType != "")
            {
                if (RoleTypeClass.RoleType.Equals("RoleType"))
                {
                    if (!RoleTypeClass.RolePrivilege.ContainsKey("主叫信息"))
                    {
                        mmHistoryDataRefresh.IsEnabled = false;
                    }
                    else
                    {
                        mmHistoryDataRefresh.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["主叫信息"]));
                    }
                }
                else
                {
                    if (int.Parse(RoleTypeClass.RoleType) > 3)
                    {
                        mmHistoryDataRefresh.IsEnabled = false;
                    }
                }
            }
            #endregion
        }
        private void addcbFullNameItems()
        {
            cbFullName.Items.Clear();
            DataTable dt = new DataTable();
            DataTable TempDevicedt = new DataTable();
            TempDevicedt = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Copy();
            DataRow[] dr = TempDevicedt.Select("NodeType='" + NodeType.LeafNode.ToString() + "' and IsStation = '1'");
            for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
            {
                if (JsonInterFace.APATTributesLists[i].InnerType.Equals("CDMA") || JsonInterFace.APATTributesLists[i].InnerType.Equals("GSM_V2"))
                {
                    cbFullName.Items.Add(JsonInterFace.APATTributesLists[i].FullName);
                }
            }
            //if (dr.Length != 0)
            //{
            //    foreach (DataRow drTemp in dr)
            //    {
            //        cbFullName.Items.Add(drTemp[0]);
            //    }
            //}
        }

        private void FrmHistoryDataTable_Activated(object sender, EventArgs e)
        {
            Parameters.HistoryDataWinHandle = (IntPtr)Parameters.FindWindow(null, this.Title);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            ShowUECallInfoThread.Abort();
            ShowUECallInfoThread = null;
            this.Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                System.Environment.Exit(System.Environment.ExitCode);
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (btnClose.IsFocused)
                {
                    this.DragMove();
                }
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnClose.Focus();
        }        

        private void btnExportData_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            //设置文件类型
            saveFileDialog.Filter = "csv files(*.csv)|*.csv|All files(*.*)|*.*";
            //获取或设置一个值，该值指示如果用户省略扩展名，文件对话框是否自动在文件名中添加扩展名。（可以不设置）
            saveFileDialog.AddExtension = true;
            //保存对话框是否记忆上次打开的目录
            saveFileDialog.RestoreDirectory = true;
            if (dgHistoryTable.Items.Count > 0)
            {

                if ((bool)saveFileDialog.ShowDialog())
                {
                    //获得文件路径
                    string localFilePath = saveFileDialog.FileName.ToString();
                    //获取文件名，不带路径
                    string fileNameExt = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1);
                    //获取文件路径，不带文件名
                    string FilePath = localFilePath.Substring(0, localFilePath.LastIndexOf("\\"));
                }
            }
            else
            {
                MessageBox.Show("没有数据，无法导出", "温馨提示", MessageBoxButton.YesNo, MessageBoxImage.Asterisk, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
            }
        }

        /// <summary>
        /// 更改选中状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textbox_hour_SelectionChanged(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                switch (tb.Name)
                {
                    case "textbox_hour":
                        tb.Background = Brushes.Gray;
                        this.textbox_minute.Background = this.Background;
                        this.textbox_second.Background = this.Background;
                        break;
                    case "textbox_minute":
                        tb.Background = Brushes.Gray;
                        this.textbox_hour.Background = this.Background;
                        this.textbox_second.Background = this.Background;
                        break;
                    case "textbox_second":
                        tb.Background = Brushes.Gray;
                        this.textbox_hour.Background = this.Background;
                        this.textbox_minute.Background = this.Background;
                        break;

                }
            }
        }
        /// <summary>
        /// 向上加时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_up_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn.Name.Equals("button_up"))
            {
                if (this.textbox_hour.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textbox_hour.Text);
                    temp++;
                    if (temp > 24)
                    {
                        temp = 0;
                    }
                    this.textbox_hour.Text = temp.ToString();
                }
                else if (this.textbox_minute.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textbox_minute.Text);
                    temp++;
                    if (temp > 60)
                    {
                        temp = 0;
                    }
                    this.textbox_minute.Text = temp.ToString();
                }
                else if (this.textbox_second.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textbox_second.Text);
                    temp++;
                    if (temp > 60)
                    {
                        temp = 0;
                    }
                    this.textbox_second.Text = temp.ToString();
                }
            }
        }
        /// <summary>
        /// 向下减时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_down_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn.Name.Equals("button_down"))
            {
                if (this.textbox_hour.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textbox_hour.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 23;
                    }
                    this.textbox_hour.Text = temp.ToString();
                }
                else if (this.textbox_minute.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textbox_minute.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 59;
                    }
                    this.textbox_minute.Text = temp.ToString();
                }
                else if (this.textbox_second.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textbox_second.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 59;
                    }
                    this.textbox_second.Text = temp.ToString();
                }
            }
        }
        /// <summary>
        /// 初始化参数设置
        /// </summary>
        private void initParameters()
        {
            string strt = System.DateTime.Now.ToString("HH:mm:ss");
            this.textbox_hour.Text = "00";
            this.textbox_minute.Text = "00";
            this.textbox_second.Text = "00";
        }

        /// <summary>
        /// 数字标准化处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numtextboxchanged(object sender, TextChangedEventArgs e)
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

        /// <summary>
        /// 判断是否为数字，是--->true，否--->false
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
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

        private void btnSelectData_Click(object sender, RoutedEventArgs e)
        {
            UECallInfo.DomainFullPathName = cbFullName.Text.Trim();
            UECallInfo.BOrmType = txtIMSI.Text.Trim();
            if (cbCallType.SelectedIndex == 0)
            {
                UECallInfo.BUeId = "";
            }
            else
            {
                UECallInfo.BUeId = cbCallType.Text.Trim();
            }
            if (UECallInfo.DomainFullPathName.Equals("") && cbCallType.SelectedIndex == 0 && UECallInfo.BUeId.Equals("")) 
            {
                condition = false;
                ConditionUECallInfoListTree.Clear();
                UECallInfoListTree.Clear();
                for (int i = 0; i < JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows.Count; i++)
                {
                    GSMV2UEReportInfoClass UEReportInfo = new GSMV2UEReportInfoClass();
                    UEReportInfo.ReportID = (ConditionUECallInfoListTree.Count + 1).ToString();
                    UEReportInfo.DomainFullPathName = JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["ParentFullPathName"].ToString();
                    UEReportInfo.DeviceName = JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["DeviceName"].ToString();
                    UEReportInfo.BOrmType = JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["BOrmType"].ToString();
                    UEReportInfo.BUeId = JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["BUeId"].ToString();
                    UEReportInfo.CRSRP = JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["CRSRP"].ToString();
                    UEReportInfo.Carrier = JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["Carrier"].ToString();
                    UEReportInfo.DataTime = JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["DataTime"].ToString();
                    UEReportInfo.BUeContent = JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["BUeContent"].ToString();
                    UECallInfoListTree.Add(UEReportInfo);
                }
                dgHistoryTable.ItemsSource = UECallInfoListTree;
            }
            else
            {
                condition = true;
                ConditionUECallInfoListTree.Clear();
                UECallInfoListTree.Clear();
                for (int i = 0; i < JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows.Count; i++)
                {
                    bool existence = true;
                    if (!UECallInfo.DomainFullPathName.Equals("") &&
                        !UECallInfo.DomainFullPathName.Equals(JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["ParentFullPathName"].ToString()) ||
                        !UECallInfo.BOrmType.Equals("") &&
                        !UECallInfo.BOrmType.Equals(JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["BOrmType"].ToString()) ||
                        !UECallInfo.BUeId.Equals("") &&
                        !UECallInfo.BUeId.Equals(JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["BUeId"].ToString()))
                    {
                        existence = false;
                    }

                    if (existence)
                    {
                        GSMV2UEReportInfoClass UEReportInfo = new GSMV2UEReportInfoClass();
                        UEReportInfo.ReportID = (ConditionUECallInfoListTree.Count + 1).ToString();
                        UEReportInfo.DomainFullPathName = JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["ParentFullPathName"].ToString();
                        UEReportInfo.DeviceName = JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["DeviceName"].ToString();
                        UEReportInfo.BOrmType = JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["BOrmType"].ToString();
                        UEReportInfo.BUeId = JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["BUeId"].ToString();
                        UEReportInfo.CRSRP = JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["CRSRP"].ToString();
                        UEReportInfo.Carrier = JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["Carrier"].ToString();
                        UEReportInfo.DataTime = JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["DataTime"].ToString();
                        UEReportInfo.BUeContent = JsonInterFace.GSMV2UEReportInfo.UECallInfoDataTab.Rows[i]["BUeContent"].ToString();
                        ConditionUECallInfoListTree.Add(UEReportInfo);
                    }
                }
                dgHistoryTable.ItemsSource = ConditionUECallInfoListTree;
            }
        }
    }
}
