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
    public partial class DataManage : Window
    {
        private object DataManageLanguageClass = null;
        string localFilePath, fileNameExt, FilePath;

        //历史记录查询
        private static Thread ShowHistoryDataThread = null;
        private static ObservableCollection<HistoryDataClass> HistoryDataListTree = new ObservableCollection<HistoryDataClass>();

        //通话记录查询
        private static Thread ShowCallInfoThread = null;
        private static ObservableCollection<PhoneHistoryDataClass> CallInfoListTree = new ObservableCollection<PhoneHistoryDataClass>();

        //短信记录查询
        private static Thread ShowSMSInfoThread = null;
        private static ObservableCollection<SMSHistoryDataClass> SMSInfoListTree = new ObservableCollection<SMSHistoryDataClass>();
        public DataManage()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            if (Parameters.LanguageType.Equals("CN") || Parameters.LanguageType.Equals(""))
            {
                DataManageLanguageClass = new Language_CN.QueryHistoricalData();
            }
            else if (Parameters.LanguageType.Equals("EN"))
            {
                DataManageLanguageClass = new Language_EN.QueryHistoricalData();
            }

            txtCurPageIndex.IsEnabled = false;
            btnBNextPage.IsEnabled = false;
            btnBBackPage.IsEnabled = false;
            btnBFirstPage.IsEnabled = false;
            btnBLastPage.IsEnabled = false;
            this.textbox_minute.Background = Brushes.White;
            this.textbox_second.Background = Brushes.White;
            this.textbox_hour.Background = Brushes.White;
            this.dploreStartTime.SelectedDate = DateTime.Now.Date;
            this.dploreEndTime.SelectedDate = DateTime.Now.Date;
            dpkCallStartTime.SelectedDate = DateTime.Now.Date;
            dpkCallEndTime.SelectedDate = DateTime.Now.Date;
            dpkSMSStartTime.SelectedDate = DateTime.Now.Date;
            dpkSMSEndTime.SelectedDate = DateTime.Now.Date;
            initParameters();
            if (cbDeviceManager.SelectedIndex == 1)
            {
                cbDomain.SelectedIndex = -1;
                cbDevice.IsEnabled = true;
                cbDomain.IsEnabled = false;
            }
            else if (cbDeviceManager.SelectedIndex == 2)
            {
                cbDevice.SelectedIndex = -1;
                cbDevice.IsEnabled = false;
                cbDomain.IsEnabled = true;
            }
            else
            {
                cbDevice.IsEnabled = false;
                cbDomain.IsEnabled = false;
            }
            //清除上一次查询的数据缓存
            HistoryDataListTree.Clear();
            CallInfoListTree.Clear();
            SMSInfoListTree.Clear();
            dgHistoryTable.Items.Clear();
            CallInfoGrid.Items.Clear();
            SMSInfoGrid.Items.Clear();
            if (ShowHistoryDataThread == null)
            {
                ShowHistoryDataThread = new Thread(new ThreadStart(HistoryDataListInfo));
                ShowHistoryDataThread.Start();
            }
            if (ShowCallInfoThread == null)
            {
                ShowCallInfoThread = new Thread(new ThreadStart(CallHistoryInfo));
                ShowCallInfoThread.Start();
            }
            if (ShowSMSInfoThread == null)
            {
                ShowSMSInfoThread = new Thread(new ThreadStart(SMSHistoryInfo));
                ShowSMSInfoThread.Start();
            }
        }
        private void HistoryDataListInfo()
        {
            while (true)
            {
                try
                {
                    bool tempIMSI = true;
                    if (JsonInterFace.HistoryDataList.HistoryDataTable.Rows.Count > 0)
                    {
                        if (int.Parse(JsonInterFace.HistoryDataList.CurPageIndex.Split(new char[] { ':' })[0]) < int.Parse(JsonInterFace.HistoryDataList.CurPageIndex.Split(new char[] { ':' })[1]))
                        {
                            if (JsonInterFace.HistoryDataList.HistoryDataTable.Rows.Count < int.Parse(JsonInterFace.HistoryDataList.PageSize))
                            {
                                tempIMSI = false;
                            }
                        }
                        else if (int.Parse(JsonInterFace.HistoryDataList.CurPageIndex.Split(new char[] { ':' })[0]) >= int.Parse(JsonInterFace.HistoryDataList.CurPageIndex.Split(new char[] { ':' })[1]))
                        {
                            if (JsonInterFace.HistoryDataList.HistoryDataTable.Rows.Count < (int.Parse(JsonInterFace.HistoryDataList.TotalRecords) - (int.Parse(JsonInterFace.HistoryDataList.CurPageIndex.Split(new char[] { ':' })[0]) - 1) * int.Parse(JsonInterFace.HistoryDataList.PageSize)))
                            {
                                tempIMSI = false;
                            }
                        }
                        if (tempIMSI)
                        {
                            for (int i = 0; i < JsonInterFace.HistoryDataList.HistoryDataTable.Rows.Count; i++)
                            {
                                DataRow dr = JsonInterFace.HistoryDataList.HistoryDataTable.Rows[i];
                                Dispatcher.Invoke(() =>
                                {
                                    if (i == 0)
                                    {
                                        HistoryDataListTree.Clear();
                                    }
                                    HistoryDataListTree.Add(new HistoryDataClass()
                                    {
                                        ID = dr["Id"].ToString(),
                                        IMSI = dr["IMSI"].ToString(),
                                        Time = dr["Time"].ToString(),
                                        UserType = dr["UserType"].ToString(),
                                        TMSI = dr["TMSI"].ToString(),
                                        Operators = dr["Operators"].ToString(),
                                        Domain = dr["Domain"].ToString(),
                                        Device = dr["Device"].ToString(),
                                        Des = dr["Des"].ToString()
                                    });
                                });
                            }
                            if (JsonInterFace.HistoryDataList.HistoryDataTable.Rows.Count > 0)
                            {
                                JsonInterFace.HistoryDataList.HistoryDataTable.Rows.Clear();
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended("历史数据查询", ex.Message, ex.StackTrace);
                }

                Thread.Sleep(100);
            }
        }
        private void CallHistoryInfo()
        {
            while (true)
            {
                try
                {
                    bool tempIMSI = true;
                    if (JsonInterFace.PhoneHistoryData.PhoneHistoryDataTable.Rows.Count > 0)
                    {
                        if (int.Parse(JsonInterFace.PhoneHistoryData.CurPageIndex.Split(new char[] { ':' })[0]) < int.Parse(JsonInterFace.PhoneHistoryData.CurPageIndex.Split(new char[] { ':' })[1]))
                        {
                            if (JsonInterFace.PhoneHistoryData.PhoneHistoryDataTable.Rows.Count < int.Parse(JsonInterFace.PhoneHistoryData.PageSize))
                            {
                                tempIMSI = false;
                            }
                        }
                        else if (int.Parse(JsonInterFace.PhoneHistoryData.CurPageIndex.Split(new char[] { ':' })[0]) >= int.Parse(JsonInterFace.PhoneHistoryData.CurPageIndex.Split(new char[] { ':' })[1]))
                        {
                            if (JsonInterFace.PhoneHistoryData.PhoneHistoryDataTable.Rows.Count < (int.Parse(JsonInterFace.PhoneHistoryData.TotalRecords) - (int.Parse(JsonInterFace.PhoneHistoryData.CurPageIndex.Split(new char[] { ':' })[0]) - 1) * int.Parse(JsonInterFace.PhoneHistoryData.PageSize)))
                            {
                                tempIMSI = false;
                            }
                        }
                        if (tempIMSI)
                        {
                            for (int i = 0; i < JsonInterFace.PhoneHistoryData.PhoneHistoryDataTable.Rows.Count; i++)
                            {
                                DataRow dr = JsonInterFace.PhoneHistoryData.PhoneHistoryDataTable.Rows[i];
                                Dispatcher.Invoke(() =>
                                {
                                    if (i == 0)
                                    {
                                        CallInfoListTree.Clear();
                                    }
                                    CallInfoListTree.Add(new PhoneHistoryDataClass()
                                    {
                                        ID = dr["Id"].ToString(),
                                        IMSI = dr["IMSI"].ToString(),
                                        PhoneNumber = dr["PhoneNumber"].ToString(),
                                        Time = dr["Time"].ToString(),
                                        Device = cbCallDevice.Text.Trim()
                                    });
                                });
                            }
                            if (JsonInterFace.PhoneHistoryData.PhoneHistoryDataTable.Rows.Count > 0)
                            {
                                JsonInterFace.PhoneHistoryData.PhoneHistoryDataTable.Rows.Clear();
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended("通话记录查询", ex.Message, ex.StackTrace);
                }

                Thread.Sleep(100);
            }
        }
        private void SMSHistoryInfo()
        {
            while (true)
            {
                try
                {
                    bool tempIMSI = true;
                    if (JsonInterFace.SMSHistoryData.SMSHistoryDataTable.Rows.Count > 0)
                    {
                        if (int.Parse(JsonInterFace.SMSHistoryData.CurPageIndex.Split(new char[] { ':' })[0]) < int.Parse(JsonInterFace.SMSHistoryData.CurPageIndex.Split(new char[] { ':' })[1]))
                        {
                            if (JsonInterFace.SMSHistoryData.SMSHistoryDataTable.Rows.Count < int.Parse(JsonInterFace.SMSHistoryData.PageSize))
                            {
                                tempIMSI = false;
                            }
                        }
                        else if (int.Parse(JsonInterFace.SMSHistoryData.CurPageIndex.Split(new char[] { ':' })[0]) >= int.Parse(JsonInterFace.SMSHistoryData.CurPageIndex.Split(new char[] { ':' })[1]))
                        {
                            if (JsonInterFace.SMSHistoryData.SMSHistoryDataTable.Rows.Count < (int.Parse(JsonInterFace.SMSHistoryData.TotalRecords) - (int.Parse(JsonInterFace.SMSHistoryData.CurPageIndex.Split(new char[] { ':' })[0]) - 1) * int.Parse(JsonInterFace.SMSHistoryData.PageSize)))
                            {
                                tempIMSI = false;
                            }
                        }
                        if (tempIMSI)
                        {
                            for (int i = 0; i < JsonInterFace.SMSHistoryData.SMSHistoryDataTable.Rows.Count; i++)
                            {
                                DataRow dr = JsonInterFace.SMSHistoryData.SMSHistoryDataTable.Rows[i];
                                Dispatcher.Invoke(() =>
                                {
                                    if (i == 0)
                                    {
                                        SMSInfoListTree.Clear();
                                    }
                                    SMSInfoListTree.Add(new SMSHistoryDataClass()
                                    {
                                        ID = dr["Id"].ToString(),
                                        IMSI = dr["IMSI"].ToString(),
                                        PhoneNumber = dr["PhoneNumber"].ToString(),
                                        Codetype = dr["Codetype"].ToString(),
                                        SMSdataInfo = dr["SMSdataInfo"].ToString(),
                                        Time = dr["Time"].ToString(),
                                        Device = cbSMSDevice.Text.Trim()
                                    });
                                });
                            }
                            if (JsonInterFace.SMSHistoryData.SMSHistoryDataTable.Rows.Count > 0)
                            {
                                JsonInterFace.SMSHistoryData.SMSHistoryDataTable.Rows.Clear();
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended("短信记录查询", ex.Message, ex.StackTrace);
                }

                Thread.Sleep(100);
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
                //历史数据查询
                if (msg == Parameters.WM_GetHistoryDataResponse)
                {
                    //MessageBox.Show("总共获取到" + JsonInterFace.HistoryDataList.TotalRecords.ToString() + "条历史数据", "温馨提示", MessageBoxButton.OK);
                }
                //连接ftp导出CSV文件
                if (msg == Parameters.WM_GetHistoryDataToCSVResponse)
                {
                    DownLoadCSVFile();
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("Windows 消息事件：", ex.Message, ex.StackTrace);
            }
            return hwnd;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //中/英文初始化
            if (Parameters.LanguageType.Equals("CN") || Parameters.LanguageType.Equals(""))
            {
                this.DataContext = (Language_CN.QueryHistoricalData)DataManageLanguageClass;
                txtQHD_ID.DataContext = new Language_CN.QueryHistoricalData();
                txtQHD_IMSI.DataContext = new Language_CN.QueryHistoricalData();
                txtQHD_Time.DataContext = new Language_CN.QueryHistoricalData();
                txtQHD_CustomerType.DataContext = new Language_CN.QueryHistoricalData();
                txtQHD_TMSI.DataContext = new Language_CN.QueryHistoricalData();
                txtQHD_Operator.DataContext = new Language_CN.QueryHistoricalData();
                txtQHD_Region.DataContext = new Language_CN.QueryHistoricalData();
                txtQHD_Device.DataContext = new Language_CN.QueryHistoricalData();
                txtQHD_Des.DataContext = new Language_CN.QueryHistoricalData();
            }
            else
            {
                this.DataContext = (Language_EN.QueryHistoricalData)DataManageLanguageClass;
                txtQHD_ID.DataContext = new Language_EN.QueryHistoricalData();
                txtQHD_IMSI.DataContext = new Language_EN.QueryHistoricalData();
                txtQHD_Time.DataContext = new Language_EN.QueryHistoricalData();
                txtQHD_CustomerType.DataContext = new Language_EN.QueryHistoricalData();
                txtQHD_TMSI.DataContext = new Language_EN.QueryHistoricalData();
                txtQHD_Operator.DataContext = new Language_EN.QueryHistoricalData();
                txtQHD_Region.DataContext = new Language_EN.QueryHistoricalData();
                txtQHD_Device.DataContext = new Language_EN.QueryHistoricalData();
                txtQHD_Des.DataContext = new Language_CN.QueryHistoricalData();
            }
            //加载域
            addcbDomainItems();
            //加载设备
            addcbDeviceItems();
            //加载数据列表
            dgHistoryTable.ItemsSource = HistoryDataListTree;
            txtHistoryDataPageIndex.DataContext = JsonInterFace.HistoryDataList;
            txtCurPageIndex.DataContext = JsonInterFace.HistoryDataList;

            CallInfoGrid.ItemsSource = CallInfoListTree;
            txtCallPageIndex.DataContext = JsonInterFace.PhoneHistoryData;
            txtCallCurPageIndex.DataContext = JsonInterFace.PhoneHistoryData;

            SMSInfoGrid.ItemsSource = SMSInfoListTree;
            txtSMSPageIndex.DataContext = JsonInterFace.SMSHistoryData;
            txtSMSCurPageIndex.DataContext = JsonInterFace.SMSHistoryData;
            #region 权限
            if (RoleTypeClass.RoleType != null && RoleTypeClass.RoleType != "")
            {
                if (RoleTypeClass.RoleType.Equals("RoleType"))
                {
                    if (!RoleTypeClass.RolePrivilege.ContainsKey("历史记录管理"))
                    {
                        btnExportData.IsEnabled = false;
                        mmHistoryDataRefresh.IsEnabled = false;
                    }
                    else
                    {
                        btnExportData.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["历史记录管理"]));
                        mmHistoryDataRefresh.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["历史记录管理"]));
                    }
                }
                else
                {
                    if (int.Parse(RoleTypeClass.RoleType) > 3)
                    {
                        btnExportData.IsEnabled = false;
                        mmHistoryDataRefresh.IsEnabled = false;
                    }
                }
            }
            #endregion
        }
        private void addcbDeviceItems()
        {
            cbDevice.Items.Clear();
            DataTable dt = new DataTable();
            dt = JsonInterFace.BindTreeViewClass.DeviceTreeTable;
            DataRow[] dr = dt.Select("NodeType='LeafNode'");//判断是否为设备
            if (dr.Length != 0)
            {
                foreach (DataRow drTemp in dr)
                {
                    cbDevice.Items.Add(drTemp[0]);
                }
            }
        }
        private void addcbDomainItems()
        {
            cbDomain.Items.Clear();
            cbCallDomain.Items.Clear();
            cbSMSDomain.Items.Clear();
            DataTable dt = new DataTable();
            DataTable TempDevicedt = new DataTable();
            TempDevicedt = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Copy();
            DataRow[] dr = TempDevicedt.Select("NodeType='" + NodeType.StructureNode.ToString() + "' and IsStation = '1'");
            if (dr.Length != 0)
            {
                foreach (DataRow drTemp in dr)
                {
                    cbDomain.Items.Add(drTemp[0]);
                    cbCallDomain.Items.Add(drTemp[0]);
                    cbSMSDomain.Items.Add(drTemp[0]);
                }
            }
        }

        private void btnSelectData_Click(object sender, RoutedEventArgs e)
        {
            string bwListApplyTo = string.Empty;
            string deviceFullPathName = string.Empty;
            string domainFullPathName = string.Empty;
            string imsi = string.Empty;
            string imei = string.Empty;
            string bwFlag = string.Empty;
            string timeStart = string.Empty;
            string timeEnded = string.Empty;
            string RmDupFlag = "1";
            imsi = txtIMSI.Text.Trim();
            imei = txtIMEI.Text.Trim();
            //设置名单类型
            if (cbBwFlag.SelectedIndex == 1)
            {
                bwFlag = "white";
            }
            else if (cbBwFlag.SelectedIndex == 2)
            {
                bwFlag = "black";
            }
            else if (cbBwFlag.SelectedIndex == 3)
            {
                bwFlag = "other";
            }
            else
            {
                bwFlag = "";
            }
            //设置开始和结束时间System.Int32.Parse(this.textbox_minute.Text);.PadLeft(2, '0')
            if (!dploreStartTime.Text.Equals(""))
            {
                timeStart = Convert.ToDateTime(dploreStartTime.Text).ToString("yyyy-MM-dd") + " " + System.Int32.Parse(textbox_hour.Text.Trim()).ToString().PadLeft(2, '0') + ":" + System.Int32.Parse(textbox_minute.Text.Trim()).ToString().PadLeft(2, '0') + ":" + System.Int32.Parse(textbox_second.Text.Trim()).ToString().PadLeft(2, '0');
            }
            if (!dploreEndTime.Text.Equals(""))
            {
                timeEnded = Convert.ToDateTime(dploreEndTime.Text).ToString("yyyy-MM-dd") + " " + System.Int32.Parse(textendbox_hour.Text.Trim()).ToString().PadLeft(2, '0') + ":" + System.Int32.Parse(textendbox_minute.Text.Trim()).ToString().PadLeft(2, '0') + ":" + System.Int32.Parse(textendbox_second.Text.Trim()).ToString().PadLeft(2, '0');
            }
            //去重
            if (rbRmDupFlag.IsChecked == true)
            {
                RmDupFlag = "1";
            }
            else
            {
                RmDupFlag = "0";
            }
            //判断选中设备还是域或其它
            if (cbDeviceManager.SelectedIndex == 1)
            {
                bwListApplyTo = "device";
                deviceFullPathName = cbDevice.Text.Trim();
            }
            else if (cbDeviceManager.SelectedIndex == 2)
            {
                bwListApplyTo = "domain";
                domainFullPathName = cbDomain.Text.Trim();
            }
            else
            {
                bwListApplyTo = "none";
            }
            try
            {
                if (NetWorkClient.ControllerServer.Connected)
                {
                    ////清除上一次查询的数据缓存
                    HistoryDataListTree.Clear();
                    dgHistoryTable.Items.Refresh();
                    txtCurPageIndex.IsEnabled = true;
                    btnBNextPage.IsEnabled = true;
                    btnBBackPage.IsEnabled = true;
                    btnBFirstPage.IsEnabled = true;
                    btnBLastPage.IsEnabled = true;
                    NetWorkClient.ControllerServer.Send(JsonInterFace.App_history_record__Request(bwListApplyTo, deviceFullPathName, domainFullPathName,
                                                                                                  imsi, imei, bwFlag, timeStart, timeEnded, RmDupFlag));//请求用户组
                }
                else
                {
                    Parameters.PrintfLogsExtended("向服务器请求历史记录:", "Connected: Failed!");
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void btnBNextPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //string PageIndexStart = JsonInterFace.HistoryDataList.CurPageIndex.Split(new char[] { ':' })[0];
                string PageIndexStart = txtCurPageIndex.Text.Trim();
                string PageIndexEnd = JsonInterFace.HistoryDataList.CurPageIndex.Split(new char[] { ':' })[1];

                if (int.Parse(PageIndexStart) + 1 > int.Parse(PageIndexEnd))
                {
                    MessageBox.Show("当前已经是最后一页", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (int.Parse(PageIndexStart) + 1 <= int.Parse(PageIndexEnd))
                {
                    if (NetController.NetWorkClient.ControllerServer.Connected)
                    {
                        string PageIndex = (int.Parse(PageIndexStart) + 1).ToString() + ":" + PageIndexEnd;
                        NetWorkClient.ControllerServer.Send(JsonInterFace.App_history_record_next_page_Request(PageIndex));
                        HistoryDataListTree.Clear();
                    }
                    else
                    {
                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void btnBBackPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //string PageIndexStart = JsonInterFace.HistoryDataList.CurPageIndex.Split(new char[] { ':' })[0];
                string PageIndexStart = txtCurPageIndex.Text.Trim();
                string PageIndexEnd = JsonInterFace.HistoryDataList.CurPageIndex.Split(new char[] { ':' })[1];

                if (int.Parse(PageIndexStart) - 1 <= 0)
                {
                    MessageBox.Show("当前已经是第一页", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (int.Parse(PageIndexStart) - 1 >= 0)
                {
                    if (NetController.NetWorkClient.ControllerServer.Connected)
                    {
                        string PageIndex = (int.Parse(PageIndexStart) - 1).ToString() + ":" + PageIndexEnd;
                        NetWorkClient.ControllerServer.Send(JsonInterFace.App_history_record_next_page_Request(PageIndex));
                        HistoryDataListTree.Clear();
                    }
                    else
                    {
                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void FrmHistoryDataTable_Activated(object sender, EventArgs e)
        {
            Parameters.HistoryDataWinHandle = (IntPtr)Parameters.FindWindow(null, this.Title);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            ShowHistoryDataThread.Abort();
            ShowHistoryDataThread = null;
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


        private void cbDevice_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                string[] tmpstr;
                string _tmpstr = string.Empty;
                tmpstr = cbDevice.Text.Trim().Split(new char[] { '.' });
                for (int i = 0; i < tmpstr.Length - 2; i++)
                {
                    _tmpstr += tmpstr[i] + ".";
                }
                _tmpstr += tmpstr[tmpstr.Length - 2];
                if (cbDomain.IsEnabled)
                {
                    if (cbDomain.Items.Count > 0)
                    {
                        cbDomain.SelectedItem = _tmpstr;
                    }
                }
            }
            catch
            {
                cbDomain.SelectedIndex = -1;
            }
        }

        private void cbDeviceManager_DropDownClosed(object sender, EventArgs e)
        {
            if (cbDeviceManager.SelectedIndex == 1)
            {
                cbDomain.SelectedIndex = -1;
                cbDevice.IsEnabled = true;
                cbDomain.IsEnabled = false;
            }
            else if (cbDeviceManager.SelectedIndex == 2)
            {
                cbDevice.SelectedIndex = -1;
                cbDevice.IsEnabled = false;
                cbDomain.IsEnabled = true;
            }
            else
            {
                cbDomain.SelectedIndex = -1;
                cbDevice.SelectedIndex = -1;
                cbDevice.IsEnabled = false;
                cbDomain.IsEnabled = false;
            }
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
                    localFilePath = saveFileDialog.FileName.ToString();
                    //获取文件名，不带路径
                    fileNameExt = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1);
                    //获取文件路径，不带文件名
                    FilePath = localFilePath.Substring(0, localFilePath.LastIndexOf("\\"));
                    if (NetController.NetWorkClient.ControllerServer.Connected)
                    {
                        NetWorkClient.ControllerServer.Send(JsonInterFace.App_history_record_export_csv__Request(fileNameExt));
                    }
                    else
                    {
                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            else
            {
                MessageBox.Show("没有数据，无法导出", "温馨提示", MessageBoxButton.YesNo, MessageBoxImage.Asterisk, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
            }
        }

        private void DownLoadCSVFile()
        {

            FtpHelper FTPServerConnetion = new FtpHelper(JsonInterFace.HistoryDataToCSVFile.FtpServerIp,
                                                         JsonInterFace.HistoryDataToCSVFile.FtpRootDir,
                                                         JsonInterFace.HistoryDataToCSVFile.FtpUsrName,
                                                         JsonInterFace.HistoryDataToCSVFile.FtpPwd,
                                                         int.Parse(JsonInterFace.HistoryDataToCSVFile.FtpPort));

            if (FTPServerConnetion.Connected)
            {
                FTPServerConnetion.Get(JsonInterFace.HistoryDataToCSVFile.FileName, FilePath, fileNameExt);
                //判断文件是否下载成功
                string[] files = System.IO.Directory.GetFiles(FilePath);
                bool downFilebol = false;
                foreach (string file in files)
                {
                    string strNonExtentsion = System.IO.Path.GetFileNameWithoutExtension(file) + ".csv";
                    if (strNonExtentsion == fileNameExt)
                    {
                        downFilebol = true;
                    }
                }
                if (downFilebol)
                {
                    MessageBox.Show("数据下载成功", "提示", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                }
                else
                {
                    MessageBox.Show("数据下载失败,请重新点击下载按钮", "提示", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                }
            }
            else
            {
                MessageBox.Show("连接FTP服务器不成功！文件下载失败", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    case "textendbox_hour":
                        tb.Background = Brushes.Gray;
                        this.textendbox_minute.Background = this.Background;
                        this.textendbox_second.Background = this.Background;
                        break;
                    case "textendbox_minute":
                        tb.Background = Brushes.Gray;
                        this.textendbox_hour.Background = this.Background;
                        this.textendbox_second.Background = this.Background;
                        break;
                    case "textendbox_second":
                        tb.Background = Brushes.Gray;
                        this.textendbox_hour.Background = this.Background;
                        this.textendbox_minute.Background = this.Background;
                        break;
                    case "txtCallbox_hour":
                        tb.Background = Brushes.Gray;
                        this.txtCallbox_minute.Background = this.Background;
                        this.txtCallbox_second.Background = this.Background;
                        break;
                    case "txtCallbox_minute":
                        tb.Background = Brushes.Gray;
                        this.txtCallbox_hour.Background = this.Background;
                        this.txtCallbox_second.Background = this.Background;
                        break;
                    case "txtCallbox_second":
                        tb.Background = Brushes.Gray;
                        this.txtCallbox_hour.Background = this.Background;
                        this.txtCallbox_minute.Background = this.Background;
                        break;
                    case "txtCallendbox_hour":
                        tb.Background = Brushes.Gray;
                        this.txtCallendbox_minute.Background = this.Background;
                        this.txtCallendbox_second.Background = this.Background;
                        break;
                    case "txtCallendbox_minute":
                        tb.Background = Brushes.Gray;
                        this.txtCallendbox_hour.Background = this.Background;
                        this.txtCallendbox_second.Background = this.Background;
                        break;
                    case "txtCallendbox_second":
                        tb.Background = Brushes.Gray;
                        this.txtCallendbox_hour.Background = this.Background;
                        this.txtCallendbox_minute.Background = this.Background;
                        break;
                    case "txtSMSbox_hour":
                        tb.Background = Brushes.Gray;
                        this.txtSMSbox_minute.Background = this.Background;
                        this.txtSMSbox_second.Background = this.Background;
                        break;
                    case "txtSMSbox_minute":
                        tb.Background = Brushes.Gray;
                        this.txtSMSbox_hour.Background = this.Background;
                        this.txtSMSbox_second.Background = this.Background;
                        break;
                    case "txtSMSbox_second":
                        tb.Background = Brushes.Gray;
                        this.txtSMSbox_hour.Background = this.Background;
                        this.txtSMSbox_minute.Background = this.Background;
                        break;
                    case "txtSMSendbox_hour":
                        tb.Background = Brushes.Gray;
                        this.txtSMSendbox_minute.Background = this.Background;
                        this.txtSMSendbox_second.Background = this.Background;
                        break;
                    case "txtSMSendbox_minute":
                        tb.Background = Brushes.Gray;
                        this.txtSMSendbox_hour.Background = this.Background;
                        this.txtSMSendbox_second.Background = this.Background;
                        break;
                    case "txtSMSendbox_second":
                        tb.Background = Brushes.Gray;
                        this.txtSMSendbox_hour.Background = this.Background;
                        this.txtSMSendbox_minute.Background = this.Background;
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
            else if (btn.Name.Equals("button_upend"))
            {
                if (this.textendbox_hour.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textendbox_hour.Text);
                    temp++;
                    if (temp > 24)
                    {
                        temp = 0;
                    }
                    this.textendbox_hour.Text = temp.ToString();
                }
                else if (this.textendbox_minute.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textendbox_minute.Text);
                    temp++;
                    if (temp > 60)
                    {
                        temp = 0;
                    }
                    this.textendbox_minute.Text = temp.ToString();
                }
                else if (this.textendbox_second.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textendbox_second.Text);
                    temp++;
                    if (temp > 60)
                    {
                        temp = 0;
                    }
                    this.textendbox_second.Text = temp.ToString();
                }
            }
            else if (btn.Name.Equals("btnCall_up"))
            {
                if (this.txtCallbox_hour.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.txtCallbox_hour.Text);
                    temp++;
                    if (temp > 24)
                    {
                        temp = 0;
                    }
                    this.txtCallbox_hour.Text = temp.ToString();
                }
                else if (this.txtCallbox_minute.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.txtCallbox_minute.Text);
                    temp++;
                    if (temp > 60)
                    {
                        temp = 0;
                    }
                    this.txtCallbox_minute.Text = temp.ToString();
                }
                else if (this.txtCallbox_second.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.txtCallbox_second.Text);
                    temp++;
                    if (temp > 60)
                    {
                        temp = 0;
                    }
                    this.txtCallbox_second.Text = temp.ToString();
                }
            }
            else if (btn.Name.Equals("btnCall_upend"))
            {
                if (this.txtCallendbox_hour.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.txtCallendbox_hour.Text);
                    temp++;
                    if (temp > 24)
                    {
                        temp = 0;
                    }
                    this.txtCallendbox_hour.Text = temp.ToString();
                }
                else if (this.txtCallendbox_minute.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.txtCallendbox_minute.Text);
                    temp++;
                    if (temp > 60)
                    {
                        temp = 0;
                    }
                    this.txtCallendbox_minute.Text = temp.ToString();
                }
                else if (this.txtCallendbox_second.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.txtCallendbox_second.Text);
                    temp++;
                    if (temp > 60)
                    {
                        temp = 0;
                    }
                    this.txtCallendbox_second.Text = temp.ToString();
                }
            }
            else if (btn.Name.Equals("btnSMS_up"))
            {
                if (this.txtSMSbox_hour.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.txtSMSbox_hour.Text);
                    temp++;
                    if (temp > 24)
                    {
                        temp = 0;
                    }
                    this.txtSMSbox_hour.Text = temp.ToString();
                }
                else if (this.txtSMSbox_minute.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.txtSMSbox_minute.Text);
                    temp++;
                    if (temp > 60)
                    {
                        temp = 0;
                    }
                    this.txtSMSbox_minute.Text = temp.ToString();
                }
                else if (this.txtSMSbox_second.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.txtSMSbox_second.Text);
                    temp++;
                    if (temp > 60)
                    {
                        temp = 0;
                    }
                    this.txtSMSbox_second.Text = temp.ToString();
                }
            }
            else if (btn.Name.Equals("btnCall_upend"))
            {
                if (this.txtSMSendbox_hour.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.txtSMSendbox_hour.Text);
                    temp++;
                    if (temp > 24)
                    {
                        temp = 0;
                    }
                    this.txtSMSendbox_hour.Text = temp.ToString();
                }
                else if (this.txtSMSendbox_minute.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.txtSMSendbox_minute.Text);
                    temp++;
                    if (temp > 60)
                    {
                        temp = 0;
                    }
                    this.txtSMSendbox_minute.Text = temp.ToString();
                }
                else if (this.txtSMSendbox_second.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.txtSMSendbox_second.Text);
                    temp++;
                    if (temp > 60)
                    {
                        temp = 0;
                    }
                    this.txtSMSendbox_second.Text = temp.ToString();
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
                        temp = 24;
                    }
                    this.textbox_hour.Text = temp.ToString();
                }
                else if (this.textbox_minute.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textbox_minute.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 60;
                    }
                    this.textbox_minute.Text = temp.ToString();
                }
                else if (this.textbox_second.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textbox_second.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 60;
                    }
                    this.textbox_second.Text = temp.ToString();
                }
            }
            else if (btn.Name.Equals("button_downend"))
            {
                if (this.textendbox_hour.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textendbox_hour.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 24;
                    }
                    this.textendbox_hour.Text = temp.ToString();
                }
                else if (this.textendbox_minute.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textendbox_minute.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 60;
                    }
                    this.textendbox_minute.Text = temp.ToString();
                }
                else if (this.textendbox_second.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textendbox_second.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 60;
                    }
                    this.textendbox_second.Text = temp.ToString();
                }
            }
            else if (btn.Name.Equals("btnCall_down"))
            {
                if (this.txtCallbox_hour.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.txtCallbox_hour.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 24;
                    }
                    this.txtCallbox_hour.Text = temp.ToString();
                }
                else if (this.txtCallbox_minute.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.txtCallbox_minute.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 60;
                    }
                    this.txtCallbox_minute.Text = temp.ToString();
                }
                else if (this.txtCallbox_second.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.txtCallbox_second.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 60;
                    }
                    this.txtCallbox_second.Text = temp.ToString();
                }
            }
            else if (btn.Name.Equals("btnCall_downend"))
            {
                if (this.txtCallendbox_hour.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.txtCallendbox_hour.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 24;
                    }
                    this.txtCallendbox_hour.Text = temp.ToString();
                }
                else if (this.txtCallendbox_minute.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.txtCallendbox_minute.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 60;
                    }
                    this.txtCallendbox_minute.Text = temp.ToString();
                }
                else if (this.txtCallendbox_second.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.txtCallendbox_second.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 60;
                    }
                    this.txtCallendbox_second.Text = temp.ToString();
                }
            }
            else if (btn.Name.Equals("btnSMS_down"))
            {
                if (this.txtSMSbox_hour.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.txtSMSbox_hour.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 24;
                    }
                    this.txtSMSbox_hour.Text = temp.ToString();
                }
                else if (this.txtSMSbox_minute.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.txtSMSbox_minute.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 60;
                    }
                    this.txtSMSbox_minute.Text = temp.ToString();
                }
                else if (this.txtSMSbox_second.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.txtSMSbox_second.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 60;
                    }
                    this.txtSMSbox_second.Text = temp.ToString();
                }
            }
            else if (btn.Name.Equals("btnSMS_downend"))
            {
                if (this.txtSMSendbox_hour.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.txtSMSendbox_hour.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 24;
                    }
                    this.txtSMSendbox_hour.Text = temp.ToString();
                }
                else if (this.txtSMSendbox_minute.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.txtSMSendbox_minute.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 60;
                    }
                    this.txtSMSendbox_minute.Text = temp.ToString();
                }
                else if (this.txtSMSendbox_second.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.txtSMSendbox_second.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 60;
                    }
                    this.txtSMSendbox_second.Text = temp.ToString();
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

            this.textendbox_hour.Text = "23";
            this.textendbox_minute.Text = "59";
            this.textendbox_second.Text = "59";

            this.txtCallbox_hour.Text = "00";
            this.txtCallbox_minute.Text = "00";
            this.txtCallbox_second.Text = "00";

            this.txtCallendbox_hour.Text = "23";
            this.txtCallendbox_minute.Text = "59";
            this.txtCallendbox_second.Text = "59";

            this.txtSMSbox_hour.Text = "00";
            this.txtSMSbox_minute.Text = "00";
            this.txtSMSbox_second.Text = "00";

            this.txtSMSendbox_hour.Text = "23";
            this.txtSMSendbox_minute.Text = "59";
            this.txtSMSendbox_second.Text = "59";
        }

        private void txtCurPageIndex_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox textbox = new TextBox();
            textbox = sender as TextBox;
            string PageIndex = JsonInterFace.HistoryDataList.CurPageIndex.Split(new char[] { ':' })[1];
            if (e.Key == Key.Enter)
            {
                if (int.Parse(textbox.Text.Trim()) <= int.Parse(PageIndex))
                {
                    if (NetController.NetWorkClient.ControllerServer.Connected)
                    {
                        string tmpPageIndex = textbox.Text.Trim().ToString() + ":" + PageIndex;
                        NetWorkClient.ControllerServer.Send(JsonInterFace.App_history_record_next_page_Request(tmpPageIndex));
                        HistoryDataListTree.Clear();
                    }
                    else
                    {
                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }

        private void txtCurPageIndex_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9]+");
            e.Handled = re.IsMatch(e.Text);
        }

        private void btnBFirstPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string PageIndexEnd = JsonInterFace.HistoryDataList.CurPageIndex.Split(new char[] { ':' })[1];
                if (NetController.NetWorkClient.ControllerServer.Connected)
                {
                    string PageIndex = ("1:" + PageIndexEnd);
                    NetWorkClient.ControllerServer.Send(JsonInterFace.App_history_record_next_page_Request(PageIndex));
                    HistoryDataListTree.Clear();
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void btnBLastPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string PageIndexEnd = JsonInterFace.HistoryDataList.CurPageIndex.Split(new char[] { ':' })[1];
                if (NetController.NetWorkClient.ControllerServer.Connected)
                {
                    string PageIndex = (PageIndexEnd + ":" + PageIndexEnd);
                    NetWorkClient.ControllerServer.Send(JsonInterFace.App_history_record_next_page_Request(PageIndex));
                    HistoryDataListTree.Clear();
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void btnSelectCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string deviceFullPathName = string.Empty;
                string device = string.Empty;
                string Carrier = string.Empty;
                string imsi = string.Empty;
                string phone = string.Empty;
                string timeStart = string.Empty;
                string timeEnded = string.Empty;
                if (!((bool)cbCallCarrierOne.IsChecked || (bool)cbCallCarrierTwo.IsChecked))
                {
                    MessageBox.Show("请选择需要查询的载波！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                //设置开始和结束时间System.Int32.Parse(this.textbox_minute.Text);.PadLeft(2, '0')
                if (!dpkCallStartTime.Text.Equals(""))
                {
                    timeStart = Convert.ToDateTime(dpkCallStartTime.Text).ToString("yyyy-MM-dd") + " " + System.Int32.Parse(txtCallbox_hour.Text.Trim()).ToString().PadLeft(2, '0') + ":" + System.Int32.Parse(txtCallbox_minute.Text.Trim()).ToString().PadLeft(2, '0') + ":" + System.Int32.Parse(txtCallbox_second.Text.Trim()).ToString().PadLeft(2, '0');
                }
                if (!dpkCallEndTime.Text.Equals(""))
                {
                    timeEnded = Convert.ToDateTime(dpkCallEndTime.Text).ToString("yyyy-MM-dd") + " " + System.Int32.Parse(txtCallendbox_hour.Text.Trim()).ToString().PadLeft(2, '0') + ":" + System.Int32.Parse(txtCallendbox_minute.Text.Trim()).ToString().PadLeft(2, '0') + ":" + System.Int32.Parse(txtCallendbox_second.Text.Trim()).ToString().PadLeft(2, '0');
                }
                deviceFullPathName = cbCallDomain.Text.Trim();
                device = cbCallDevice.SelectedIndex > 0 ? cbCallDevice.Text.Trim() : "";
                if (cbCallCarrierOne.IsEnabled && cbCallCarrierTwo.IsEnabled)
                {
                    Carrier = (bool)cbCallCarrierOne.IsChecked ? "0" : "1";
                }
                else
                {
                    Carrier = "-1";
                }
                imsi = txtCallIMSI.Text.Trim();
                phone = txtCallNumber.Text.Trim();
                if (NetWorkClient.ControllerServer.Connected)
                {
                    ////清除上一次查询的数据缓存
                    CallInfoListTree.Clear();
                    CallInfoGrid.Items.Refresh();
                    txtCallCurPageIndex.IsEnabled = true;
                    btnCallNextPage.IsEnabled = true;
                    btnCallBackPage.IsEnabled = true;
                    btnCallFirstPage.IsEnabled = true;
                    btnCallLastPage.IsEnabled = true;
                    NetWorkClient.ControllerServer.Send(JsonInterFace.App_get_MsCall_Request__Request(deviceFullPathName, device, Carrier, imsi, phone, timeStart, timeEnded));
                }
                else
                {
                    Parameters.PrintfLogsExtended("向服务器请求通话记录:", "Connected: Failed!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("通话记录查询有误", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void cbCallCarrierOne_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)cbCallCarrierOne.IsChecked)
            {
                cbCallCarrierTwo.IsChecked = false;
            }
            else
            {
                if (!(bool)cbCallCarrierTwo.IsChecked)
                {
                    cbCallCarrierOne.IsChecked = true;
                }
            }
        }

        private void cbCallCarrierTwo_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)cbCallCarrierTwo.IsChecked)
            {
                cbCallCarrierOne.IsChecked = false;
            }
            else
            {
                if (!(bool)cbCallCarrierOne.IsChecked)
                {
                    cbCallCarrierTwo.IsChecked = true;
                }
            }
        }

        private void cbCallDomain_DropDownClosed(object sender, EventArgs e)
        {
            ComboBox cbb = sender as ComboBox;
            if (cbb.Name.Equals("cbCallDomain"))
            {
                cbCallDevice.IsEnabled = true;
                cbCallDevice.Items.Clear();
                cbCallDevice.Items.Add("域中所有设备");
                for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                {
                    string domain = string.Empty;
                    string[] _domain = JsonInterFace.APATTributesLists[i].FullName.ToString().Split(new char[] { '.' });
                    for (int j = 0; j < _domain.Length - 1; j++)
                    {
                        if (domain == "" || domain == null)
                        {
                            domain = _domain[j];
                        }
                        else
                        {
                            domain += "." + _domain[j];
                        }
                    }
                    if (domain.Equals(cbCallDomain.Text.Trim()))
                    {
                        cbCallDevice.Items.Add(JsonInterFace.APATTributesLists[i].SelfName);
                    }
                }
            }
            else if (cbb.Name.Equals("cbSMSDomain"))
            {
                cbSMSDevice.IsEnabled = true;
                cbSMSDevice.Items.Clear();
                cbSMSDevice.Items.Add("域中所有设备");
                DataTable dt = new DataTable();
                dt = JsonInterFace.BindTreeViewClass.DeviceTreeTable;
                DataRow[] dr = dt.Select("NodeType='LeafNode'");//判断是否为设备
                if (dr.Length != 0)
                {
                    foreach (DataRow drTemp in dr)
                    {
                        string domain = string.Empty;
                        string[] _domain = drTemp[0].ToString().Split(new char[] { '.' });
                        for (int i = 0; i < _domain.Length - 1; i++)
                        {
                            if (domain == "" || domain == null)
                            {
                                domain = _domain[i];
                            }
                            else
                            {
                                domain += "." + _domain[i];
                            }
                        }
                        if (domain.Equals(cbSMSDomain.Text.Trim()))
                        {
                            cbSMSDevice.Items.Add(drTemp[0].ToString().Split(new char[] { '.' })[drTemp[0].ToString().Split(new char[] { '.' }).Length - 1]);
                        }
                    }
                }
            }
        }

        private void btnSelectSMS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string deviceFullPathName = string.Empty;
                string device = string.Empty;
                string Carrier = string.Empty;
                string imsi = string.Empty;
                string phone = string.Empty;
                string data = string.Empty;
                string timeStart = string.Empty;
                string timeEnded = string.Empty;

                if (!((bool)cbSMSCarrierOne.IsChecked || (bool)cbSMSCarrierTwo.IsChecked))
                {
                    MessageBox.Show("请选择需要查询的载波！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                //设置开始和结束时间System.Int32.Parse(this.textbox_minute.Text);.PadLeft(2, '0')
                if (!dpkSMSStartTime.Text.Equals(""))
                {
                    timeStart = Convert.ToDateTime(dpkSMSStartTime.Text).ToString("yyyy-MM-dd") + " " + System.Int32.Parse(txtSMSbox_hour.Text.Trim()).ToString().PadLeft(2, '0') + ":" + System.Int32.Parse(txtSMSbox_minute.Text.Trim()).ToString().PadLeft(2, '0') + ":" + System.Int32.Parse(txtSMSbox_second.Text.Trim()).ToString().PadLeft(2, '0');
                }
                if (!dpkCallEndTime.Text.Equals(""))
                {
                    timeEnded = Convert.ToDateTime(dpkSMSEndTime.Text).ToString("yyyy-MM-dd") + " " + System.Int32.Parse(txtSMSendbox_hour.Text.Trim()).ToString().PadLeft(2, '0') + ":" + System.Int32.Parse(txtSMSendbox_minute.Text.Trim()).ToString().PadLeft(2, '0') + ":" + System.Int32.Parse(txtSMSendbox_second.Text.Trim()).ToString().PadLeft(2, '0');
                }
                deviceFullPathName = cbSMSDomain.Text.Trim();
                device = cbSMSDevice.SelectedIndex > 0 ? cbSMSDevice.Text.Trim() : "";
                if (cbSMSCarrierOne.IsEnabled && cbSMSCarrierTwo.IsEnabled)
                {
                    Carrier = (bool)cbSMSCarrierOne.IsChecked ? "0" : "1";
                }
                else
                {
                    Carrier = "-1";
                }
                imsi = txtSMSIMSI.Text.Trim();
                phone = txtSMSNumber.Text.Trim();
                data = txtSMSData.Text.Trim();
                if (NetWorkClient.ControllerServer.Connected)
                {
                    ////清除上一次查询的数据缓存
                    SMSInfoListTree.Clear();
                    SMSInfoGrid.Items.Refresh();
                    txtSMSCurPageIndex.IsEnabled = true;
                    btnSMSNextPage.IsEnabled = true;
                    btnSMSBackPage.IsEnabled = true;
                    btnSMSFirstPage.IsEnabled = true;
                    btnSMSLastPage.IsEnabled = true;
                    NetWorkClient.ControllerServer.Send(JsonInterFace.App_get_MsSms_Request__Request(deviceFullPathName, device, Carrier, imsi, phone, data, timeStart, timeEnded));
                }
                else
                {
                    Parameters.PrintfLogsExtended("向服务器请求短信记录:", "Connected: Failed!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("短信记录查询有误", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void txtCallPageIndex_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox textbox = new TextBox();
            textbox = sender as TextBox;
            string PageIndex = JsonInterFace.PhoneHistoryData.CurPageIndex.Split(new char[] { ':' })[1];
            if (e.Key == Key.Enter)
            {
                if (int.Parse(textbox.Text.Trim()) <= int.Parse(PageIndex))
                {
                    if (NetController.NetWorkClient.ControllerServer.Connected)
                    {
                        string tmpPageIndex = textbox.Text.Trim().ToString() + ":" + PageIndex;
                        NetWorkClient.ControllerServer.Send(JsonInterFace.App_get_MsCall_NextPage_Request(tmpPageIndex));
                        CallInfoListTree.Clear();
                    }
                    else
                    {
                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }

        private void txtSMSPageIndex_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox textbox = new TextBox();
            textbox = sender as TextBox;
            string PageIndex = JsonInterFace.SMSHistoryData.CurPageIndex.Split(new char[] { ':' })[1];
            if (e.Key == Key.Enter)
            {
                if (int.Parse(textbox.Text.Trim()) <= int.Parse(PageIndex))
                {
                    if (NetController.NetWorkClient.ControllerServer.Connected)
                    {
                        string tmpPageIndex = textbox.Text.Trim().ToString() + ":" + PageIndex;
                        NetWorkClient.ControllerServer.Send(JsonInterFace.App_get_MsSms_NextPage_Request(tmpPageIndex));
                        SMSInfoListTree.Clear();
                    }
                    else
                    {
                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }

        private void btnCallNextPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string PageIndexStart = txtCallPageIndex.Text.Trim();
                string PageIndexEnd = JsonInterFace.PhoneHistoryData.CurPageIndex.Split(new char[] { ':' })[1];

                if (int.Parse(PageIndexStart) + 1 > int.Parse(PageIndexEnd))
                {
                    MessageBox.Show("当前已经是最后一页", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (int.Parse(PageIndexStart) + 1 <= int.Parse(PageIndexEnd))
                {
                    if (NetController.NetWorkClient.ControllerServer.Connected)
                    {
                        string PageIndex = (int.Parse(PageIndexStart) + 1).ToString() + ":" + PageIndexEnd;
                        NetWorkClient.ControllerServer.Send(JsonInterFace.App_get_MsCall_NextPage_Request(PageIndex));
                        CallInfoListTree.Clear();
                    }
                    else
                    {
                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void btnSMSNextPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string PageIndexStart = txtSMSPageIndex.Text.Trim();
                string PageIndexEnd = JsonInterFace.SMSHistoryData.CurPageIndex.Split(new char[] { ':' })[1];

                if (int.Parse(PageIndexStart) + 1 > int.Parse(PageIndexEnd))
                {
                    MessageBox.Show("当前已经是最后一页", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (int.Parse(PageIndexStart) + 1 <= int.Parse(PageIndexEnd))
                {
                    if (NetController.NetWorkClient.ControllerServer.Connected)
                    {
                        string PageIndex = (int.Parse(PageIndexStart) + 1).ToString() + ":" + PageIndexEnd;
                        NetWorkClient.ControllerServer.Send(JsonInterFace.App_get_MsSms_NextPage_Request(PageIndex));
                        SMSInfoListTree.Clear();
                    }
                    else
                    {
                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void btnCallBackPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string PageIndexStart = txtCallPageIndex.Text.Trim();
                string PageIndexEnd = JsonInterFace.PhoneHistoryData.CurPageIndex.Split(new char[] { ':' })[1];

                if (int.Parse(PageIndexStart) - 1 <= 0)
                {
                    MessageBox.Show("当前已经是第一页", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (int.Parse(PageIndexStart) - 1 >= 0)
                {
                    if (NetController.NetWorkClient.ControllerServer.Connected)
                    {
                        string PageIndex = (int.Parse(PageIndexStart) - 1).ToString() + ":" + PageIndexEnd;
                        NetWorkClient.ControllerServer.Send(JsonInterFace.App_get_MsCall_NextPage_Request(PageIndex));
                        CallInfoListTree.Clear();
                    }
                    else
                    {
                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void btnSMSBackPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string PageIndexStart = txtSMSPageIndex.Text.Trim();
                string PageIndexEnd = JsonInterFace.SMSHistoryData.CurPageIndex.Split(new char[] { ':' })[1];

                if (int.Parse(PageIndexStart) - 1 <= 0)
                {
                    MessageBox.Show("当前已经是第一页", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (int.Parse(PageIndexStart) - 1 >= 0)
                {
                    if (NetController.NetWorkClient.ControllerServer.Connected)
                    {
                        string PageIndex = (int.Parse(PageIndexStart) - 1).ToString() + ":" + PageIndexEnd;
                        NetWorkClient.ControllerServer.Send(JsonInterFace.App_get_MsSms_NextPage_Request(PageIndex));
                        SMSInfoListTree.Clear();
                    }
                    else
                    {
                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void btnSMSFirstPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string PageIndexEnd = JsonInterFace.SMSHistoryData.CurPageIndex.Split(new char[] { ':' })[1];
                if (NetController.NetWorkClient.ControllerServer.Connected)
                {
                    string PageIndex = ("1:" + PageIndexEnd);
                    NetWorkClient.ControllerServer.Send(JsonInterFace.App_get_MsSms_NextPage_Request(PageIndex));
                    SMSInfoListTree.Clear();
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void btnSMSLastPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string PageIndexEnd = JsonInterFace.SMSHistoryData.CurPageIndex.Split(new char[] { ':' })[1];
                if (NetController.NetWorkClient.ControllerServer.Connected)
                {
                    string PageIndex = (PageIndexEnd + ":" + PageIndexEnd);
                    NetWorkClient.ControllerServer.Send(JsonInterFace.App_get_MsSms_NextPage_Request(PageIndex));
                    SMSInfoListTree.Clear();
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void btnCallFirstPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string PageIndexEnd = JsonInterFace.PhoneHistoryData.CurPageIndex.Split(new char[] { ':' })[1];
                if (NetController.NetWorkClient.ControllerServer.Connected)
                {
                    string PageIndex = ("1:" + PageIndexEnd);
                    NetWorkClient.ControllerServer.Send(JsonInterFace.App_get_MsCall_NextPage_Request(PageIndex));
                    CallInfoListTree.Clear();
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void btnCallLastPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string PageIndexEnd = JsonInterFace.PhoneHistoryData.CurPageIndex.Split(new char[] { ':' })[1];
                if (NetController.NetWorkClient.ControllerServer.Connected)
                {
                    string PageIndex = (PageIndexEnd + ":" + PageIndexEnd);
                    NetWorkClient.ControllerServer.Send(JsonInterFace.App_get_MsCall_NextPage_Request(PageIndex));
                    CallInfoListTree.Clear();
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void cbSMSCarrierTwo_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)cbSMSCarrierTwo.IsChecked)
            {
                cbSMSCarrierOne.IsChecked = false;
            }
            else
            {
                if (!(bool)cbSMSCarrierOne.IsChecked)
                {
                    cbSMSCarrierTwo.IsChecked = true;
                }
            }
        }

        private void cbSMSCarrierOne_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)cbSMSCarrierOne.IsChecked)
            {
                cbSMSCarrierTwo.IsChecked = false;
            }
            else
            {
                if (!(bool)cbSMSCarrierTwo.IsChecked)
                {
                    cbSMSCarrierOne.IsChecked = true;
                }
            }
        }

        private void cbCallDevice_DropDownClosed(object sender, EventArgs e)
        {
            ComboBox cbb = new ComboBox();
            cbb = sender as ComboBox;
            if (cbb.SelectedIndex > -1)
            {
                if (cbb.Name.Equals("cbCallDevice"))
                {
                    string _fullName = cbCallDomain.Text.Trim() + "." + cbCallDevice.Text.Trim();
                    for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                    {
                        string FullName = JsonInterFace.APATTributesLists[i].FullName.ToString();
                        if (_fullName == FullName)
                        {
                            if (JsonInterFace.APATTributesLists[i].Mode.Replace("_", "-").Equals(DeviceType.CDMA))
                            {
                                cbCallCarrierOne.IsChecked = true;
                                cbCallCarrierTwo.IsChecked = false;
                                cbCallCarrierOne.IsEnabled = false;
                                cbCallCarrierTwo.IsEnabled = false;
                            }
                            else
                            {
                                cbCallCarrierOne.IsChecked = true;
                                cbCallCarrierTwo.IsChecked = false;
                                cbCallCarrierOne.IsEnabled = true;
                                cbCallCarrierTwo.IsEnabled = true;
                            }
                            break;
                        }
                    }
                }
                else if (cbb.Name.Equals("cbSMSDevice"))
                {
                    string _fullName = cbSMSDomain.Text.Trim() + "." + cbSMSDevice.Text.Trim();
                    for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                    {
                        string FullName = JsonInterFace.APATTributesLists[i].FullName.ToString();
                        if (_fullName == FullName)
                        {
                            if (JsonInterFace.APATTributesLists[i].Mode.Replace("_", "-").Equals(DeviceType.CDMA))
                            {
                                cbSMSCarrierOne.IsChecked = true;
                                cbSMSCarrierTwo.IsChecked = false;
                                cbSMSCarrierOne.IsEnabled = false;
                                cbSMSCarrierTwo.IsEnabled = false;
                            }
                            else
                            {
                                cbSMSCarrierOne.IsChecked = true;
                                cbSMSCarrierTwo.IsChecked = false;
                                cbSMSCarrierOne.IsEnabled = true;
                                cbSMSCarrierTwo.IsEnabled = true;
                            }
                            break;
                        }
                    }
                }
            }
        }

        private void mmItemExportCallData_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            //设置文件类型
            saveFileDialog.Filter = "csv files(*.csv)|*.csv|All files(*.*)|*.*";
            //获取或设置一个值，该值指示如果用户省略扩展名，文件对话框是否自动在文件名中添加扩展名。（可以不设置）
            saveFileDialog.AddExtension = true;
            //保存对话框是否记忆上次打开的目录
            saveFileDialog.RestoreDirectory = true;
            if (CallInfoGrid.Items.Count > 0)
            {

                if ((bool)saveFileDialog.ShowDialog())
                {
                    //获得文件路径
                    localFilePath = saveFileDialog.FileName.ToString();
                    //获取文件名，不带路径
                    fileNameExt = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1);
                    //获取文件路径，不带文件名
                    FilePath = localFilePath.Substring(0, localFilePath.LastIndexOf("\\"));
                    if (NetController.NetWorkClient.ControllerServer.Connected)
                    {
                        NetWorkClient.ControllerServer.Send(JsonInterFace.App_get_MsCall_ExportCSV_Request(fileNameExt));
                    }
                    else
                    {
                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            else
            {
                MessageBox.Show("没有数据，无法导出", "温馨提示", MessageBoxButton.YesNo, MessageBoxImage.Asterisk, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
            }
        }

        private void mmItemExportSMSData_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            //设置文件类型
            saveFileDialog.Filter = "csv files(*.csv)|*.csv|All files(*.*)|*.*";
            //获取或设置一个值，该值指示如果用户省略扩展名，文件对话框是否自动在文件名中添加扩展名。（可以不设置）
            saveFileDialog.AddExtension = true;
            //保存对话框是否记忆上次打开的目录
            saveFileDialog.RestoreDirectory = true;
            if (SMSInfoGrid.Items.Count > 0)
            {

                if ((bool)saveFileDialog.ShowDialog())
                {
                    //获得文件路径
                    localFilePath = saveFileDialog.FileName.ToString();
                    //获取文件名，不带路径
                    fileNameExt = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1);
                    //获取文件路径，不带文件名
                    FilePath = localFilePath.Substring(0, localFilePath.LastIndexOf("\\"));
                    if (NetController.NetWorkClient.ControllerServer.Connected)
                    {
                        NetWorkClient.ControllerServer.Send(JsonInterFace.App_get_MsSms_ExportCSV_Request(fileNameExt));
                    }
                    else
                    {
                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            else
            {
                MessageBox.Show("没有数据，无法导出", "温馨提示", MessageBoxButton.YesNo, MessageBoxImage.Asterisk, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
            }
        }

        private void cbCallAllDevice_Click(object sender, RoutedEventArgs e)
        {
            CheckBox ckbox = sender as CheckBox;
            if (ckbox.Name.Equals("cbCallAllDevice"))
            {
                if ((bool)ckbox.IsChecked)
                {
                    cbCallDomain.SelectedIndex = -1;
                    cbCallDevice.SelectedIndex = -1;
                    cbCallDomain.IsEnabled = false;
                    cbCallDevice.IsEnabled = false;
                }
                else
                {
                    cbCallDomain.IsEnabled = true;
                    cbCallDevice.IsEnabled = true;
                }
                cbCallAllDevice.IsChecked = ckbox.IsChecked;
            }
            else if (ckbox.Name.Equals("cbSMSAllDevice"))
            {
                if ((bool)ckbox.IsChecked)
                {
                    cbSMSDomain.SelectedIndex = -1;
                    cbSMSDevice.SelectedIndex = -1;
                    cbSMSDomain.IsEnabled = false;
                    cbSMSDevice.IsEnabled = false;
                }
                else
                {
                    cbSMSDomain.IsEnabled = true;
                    cbSMSDevice.IsEnabled = true;
                }
                cbSMSAllDevice.IsChecked = ckbox.IsChecked;
            }

        }

        private void btnExportPhoneRecord_Click(object sender, RoutedEventArgs e)
        {
            mmItemExportCallData_Click(sender, e);
        }

        private void btnExportSMS_Click(object sender, RoutedEventArgs e)
        {
            mmItemExportSMSData_Click(sender, e);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                if (NetWorkClient.ControllerServer.Connected)
                {
                    NetWorkClient.ControllerServer.Send(JsonInterFace.HistoryRecordQueryExit());
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void btnDeleteData_Click(object sender, RoutedEventArgs e)
        {
            string timeStart = "";
            string timeEnded = "";
            try
            {
                if (MessageBox.Show("清除IMSI历史记录只和开始时间和结束时间有关", "删除IMSI提示", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {
                    if (!dploreStartTime.Text.Equals(""))
                    {
                        timeStart = Convert.ToDateTime(dploreStartTime.Text).ToString("yyyy-MM-dd") + " " + System.Int32.Parse(textbox_hour.Text.Trim()).ToString().PadLeft(2, '0') + ":" + System.Int32.Parse(textbox_minute.Text.Trim()).ToString().PadLeft(2, '0') + ":" + System.Int32.Parse(textbox_second.Text.Trim()).ToString().PadLeft(2, '0');
                    }
                    if (!dploreEndTime.Text.Equals(""))
                    {
                        timeEnded = Convert.ToDateTime(dploreEndTime.Text).ToString("yyyy-MM-dd") + " " + System.Int32.Parse(textendbox_hour.Text.Trim()).ToString().PadLeft(2, '0') + ":" + System.Int32.Parse(textendbox_minute.Text.Trim()).ToString().PadLeft(2, '0') + ":" + System.Int32.Parse(textendbox_second.Text.Trim()).ToString().PadLeft(2, '0');
                    }
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        NetWorkClient.ControllerServer.Send(JsonInterFace.App_history_record_delete_request(timeStart, timeEnded));
                    }
                    else
                    {
                        Parameters.PrintfLogsExtended("向服务器请求历史记录:", "Connected: Failed!");
                    }
                }
            }
            catch(Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
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

    }
}
