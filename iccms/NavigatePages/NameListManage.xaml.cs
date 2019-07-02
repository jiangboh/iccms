using DataInterface;
using IODataControl;
using NetController;
using ParameterControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Runtime.InteropServices;
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
    /// NameListManage.xaml 的交互逻辑
    /// </summary>
    public partial class NameListManage : Window
    {
        private object NameListManageLanguageClass = null;
        private Dictionary<string, string> BlackListItemDataEdit = new Dictionary<string, string>();
        private Dictionary<string, string> WhiteListItemDataEdit = new Dictionary<string, string>();
        private Dictionary<string, string> CustomListItemDataEdit = new Dictionary<string, string>();
        private List<List<BlackListClass>> BlackListItemsDataDelete = new List<List<BlackListClass>>();
        private List<List<WhiteListClass>> WhiteListItemsDataDelete = new List<List<WhiteListClass>>();
        private List<List<CustomListClass>> CustomListItemsDataDelete = new List<List<CustomListClass>>();
        public static IList<CheckBoxTreeModel> _usrdomainData = new List<CheckBoxTreeModel>();
        private SubWindow.ProgressBarWindow ProgressBarWin = null;

        //黑名单,白名单, 普通用户
        private static Thread ShowBlackListDataThread = null;
        private static Thread ShowWhiteListDataThread = null;
        private static Thread ShowCustomListDataThread = null;
        private static ObservableCollection<BlackListClass> SelfBlackLists = new ObservableCollection<BlackListClass>();
        private static ObservableCollection<BlackListClass> SelfWhiteLists = new ObservableCollection<BlackListClass>();
        private static ObservableCollection<CustomListClass> SelfCustomLists = new ObservableCollection<CustomListClass>();

        //重定向
        private static Thread ShowRedirectInfoThread = null;
        private static ObservableCollection<ReDirectionClass> SelfRedirectionList = new ObservableCollection<ReDirectionClass>();

        public NameListManage()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;

            if (Parameters.LanguageType.Equals("CN") || Parameters.LanguageType.Equals(""))
            {
                NameListManageLanguageClass = new Language_CN.SpeciallistManageClass();
            }
            else if (Parameters.LanguageType.Equals("EN"))
            {
                NameListManageLanguageClass = new Language_EN.SpeciallistManageClass();
            }

            if (ProgressBarWin == null)
            {
                ProgressBarWin = new SubWindow.ProgressBarWindow();
            }

            if (ShowBlackListDataThread == null)
            {
                ShowBlackListDataThread = new Thread(new ThreadStart(ShowBlackListInfo));
            }

            if (ShowWhiteListDataThread == null)
            {
                ShowWhiteListDataThread = new Thread(new ThreadStart(ShowWhiteListInfo));
            }

            if (ShowCustomListDataThread == null)
            {
                ShowCustomListDataThread = new Thread(new ThreadStart(ShowCustomListInfo));
            }

            if (ShowRedirectInfoThread == null)
            {
                ShowRedirectInfoThread = new Thread(new ThreadStart(ShowRedirectionInfo));
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
                _usrdomainData.Clear();
                _usrdomainData.Add(treeModel);
            }).Start();
        }

        //显示黑名单
        private void ShowBlackListInfo()
        {
            DataTable BListTab = null;
            while (true)
            {
                if (JsonInterFace.BlackList.BlackListTable.Rows != null)
                {
                    if (JsonInterFace.BlackList.BlackListTable.Rows.Count > 0)
                    {
                        try
                        {
                            lock (JsonInterFace.BlackList.BDataLock)
                            {
                                BListTab = JsonInterFace.BlackList.BlackListTable.Copy();
                            }

                            for (int i = 0; i < BListTab.Rows.Count; i++)
                            {
                                DataRow bdr = BListTab.Rows[i];
                                string _ID = string.Empty;
                                Dispatcher.Invoke(() =>
                                {
                                    if (i == 0)
                                    {
                                        _ID = bdr["ID"].ToString();
                                    }
                                    else
                                    {
                                        _ID = (Convert.ToInt64((SelfBlackLists[SelfBlackLists.Count - 1] as BlackListClass).ID) + 1).ToString();
                                    }

                                    SelfBlackLists.Add(new BlackListClass() { ID = _ID, IMSI = bdr["IMSI"].ToString(), IMEI = bdr["IMEI"].ToString(), AliasName = bdr["AliasName"].ToString(), Resourcese = bdr["Resourcese"].ToString(), Station = bdr["Station"].ToString() });
                                });

                                //清理
                                for (int j = 0; j < JsonInterFace.BlackList.BlackListTable.Rows.Count; j++)
                                {
                                    if (_ID == JsonInterFace.BlackList.BlackListTable.Rows[j]["ID"].ToString())
                                    {
                                        JsonInterFace.BlackList.BlackListTable.Rows.RemoveAt(j);
                                        break;
                                    }
                                }
                            }

                            BListTab.Clear();
                            System.GC.Collect();

                            if (Convert.ToInt64(JsonInterFace.BlackList.TotalRecords) > Convert.ToInt32(JsonInterFace.BlackList.PageSize))
                            {
                                if (SelfBlackLists.Count >= Convert.ToInt32(JsonInterFace.BlackList.PageSize))
                                {
                                    new Thread(() =>
                                    {
                                        MessageBox.Show("获取黑名单成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                                    }).Start();
                                }
                            }
                            else if (Convert.ToInt64(JsonInterFace.BlackList.TotalRecords) <= SelfBlackLists.Count)
                            {
                                new Thread(() =>
                                {
                                    MessageBox.Show("获取黑名单成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                                }).Start();
                            }
                        }
                        catch (Exception ex)
                        {
                            Parameters.PrintfLogsExtended("黑名单查询", ex.Message, ex.StackTrace);
                            Thread.Sleep(1000);
                        }
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
        }

        //显示白名单
        private void ShowWhiteListInfo()
        {
            //-------白名单--------
            DataTable WListTab = null;
            while (true)
            {
                if (JsonInterFace.WhiteList.WhiteListTable.Rows != null)
                {
                    if (JsonInterFace.WhiteList.WhiteListTable.Rows.Count > 0)
                    {
                        try
                        {
                            lock (JsonInterFace.WhiteList.TableLock)
                            {
                                WListTab = JsonInterFace.WhiteList.WhiteListTable.Copy();
                            }

                            for (int i = 0; i < WListTab.Rows.Count; i++)
                            {
                                DataRow wdr = WListTab.Rows[i];
                                string _ID = string.Empty;
                                Dispatcher.Invoke(() =>
                                {
                                    if (i == 0)
                                    {
                                        _ID = wdr["ID"].ToString();
                                    }
                                    else
                                    {
                                        _ID = (Convert.ToInt64((SelfWhiteLists[SelfWhiteLists.Count - 1] as WhiteListClass).ID) + 1).ToString();
                                    }

                                    SelfWhiteLists.Add(new WhiteListClass() { ID = _ID, IMSI = wdr["IMSI"].ToString(), IMEI = wdr["IMEI"].ToString(), AliasName = wdr["AliasName"].ToString(), Station = wdr["Station"].ToString() });
                                });

                                //清理
                                for (int j = 0; j < JsonInterFace.WhiteList.WhiteListTable.Rows.Count; j++)
                                {
                                    if (_ID == JsonInterFace.WhiteList.WhiteListTable.Rows[j]["ID"].ToString())
                                    {
                                        JsonInterFace.WhiteList.WhiteListTable.Rows.RemoveAt(j);
                                        break;
                                    }
                                }
                            }

                            WListTab.Clear();
                            System.GC.Collect();

                            if (Convert.ToInt64(JsonInterFace.WhiteList.TotalRecords) > Convert.ToInt32(JsonInterFace.WhiteList.PageSize))
                            {
                                if (SelfWhiteLists.Count >= Convert.ToInt32(JsonInterFace.WhiteList.PageSize))
                                {
                                    new Thread(() =>
                                    {
                                        MessageBox.Show("获取白名单成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                                    }).Start();
                                }
                            }
                            else if (Convert.ToInt64(JsonInterFace.WhiteList.TotalRecords) <= SelfWhiteLists.Count)
                            {
                                new Thread(() =>
                                {
                                    MessageBox.Show("获取白名单成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                                }).Start();
                            }
                        }
                        catch (Exception ex)
                        {
                            Parameters.PrintfLogsExtended("白名单查询", ex.Message, ex.StackTrace);
                            Thread.Sleep(1000);
                        }
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
        }

        //显示普通用户
        private void ShowCustomListInfo()
        {
            DataTable CListTab = null;
            while (true)
            {
                if (JsonInterFace.CustomList.CustomListTable.Rows != null)
                {
                    if (JsonInterFace.CustomList.CustomListTable.Rows.Count > 0)
                    {
                        try
                        {
                            lock (JsonInterFace.CustomList.CDataLock)
                            {
                                CListTab = JsonInterFace.CustomList.CustomListTable.Copy();
                            }

                            for (int i = 0; i < CListTab.Rows.Count; i++)
                            {
                                DataRow cdr = CListTab.Rows[i];
                                string _ID = string.Empty;
                                Dispatcher.Invoke(() =>
                                {
                                    if (i == 0)
                                    {
                                        _ID = cdr["ID"].ToString();
                                    }
                                    else
                                    {
                                        _ID = (Convert.ToInt64((SelfCustomLists[SelfCustomLists.Count - 1] as CustomListClass).ID) + 1).ToString();
                                    }

                                    SelfCustomLists.Add(new CustomListClass() { ID = _ID, IMSI = cdr["IMSI"].ToString(), IMEI = cdr["IMEI"].ToString(), AliasName = cdr["AliasName"].ToString(), Resourcese = cdr["Resourcese"].ToString(), Station = cdr["Station"].ToString() });
                                });

                                //清理
                                for (int j = 0; j < JsonInterFace.CustomList.CustomListTable.Rows.Count; j++)
                                {
                                    if (_ID == JsonInterFace.CustomList.CustomListTable.Rows[j]["ID"].ToString())
                                    {
                                        JsonInterFace.CustomList.CustomListTable.Rows.RemoveAt(j);
                                        break;
                                    }
                                }
                            }

                            CListTab.Clear();
                            System.GC.Collect();

                            if (Convert.ToInt64(JsonInterFace.CustomList.TotalRecords) > Convert.ToInt32(JsonInterFace.CustomList.PageSize))
                            {
                                if (SelfCustomLists.Count >= Convert.ToInt32(JsonInterFace.CustomList.PageSize))
                                {
                                    new Thread(() =>
                                    {
                                        MessageBox.Show("获取普通用户成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                                    }).Start();
                                }
                            }
                            else if (Convert.ToInt64(JsonInterFace.CustomList.TotalRecords) <= SelfCustomLists.Count)
                            {
                                new Thread(() =>
                                {
                                    MessageBox.Show("获取普通用户成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                                }).Start();
                            }
                        }
                        catch (Exception ex)
                        {
                            Parameters.PrintfLogsExtended("普通用户查询", ex.Message, ex.StackTrace);
                            Thread.Sleep(1000);
                        }
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
        }

        //显示用户策略
        private void ShowRedirectionInfo()
        {
            while (true)
            {
                if (JsonInterFace.ReDirection.RedirectionTable.Rows != null)
                {
                    if (JsonInterFace.ReDirection.RedirectionTable.Rows.Count > 0)
                    {
                        try
                        {
                            int value = JsonInterFace.ReDirection.RedirectionTable.Rows.Count;
                            for (int i = 0; i < JsonInterFace.ReDirection.RedirectionTable.Rows.Count; i++)
                            {
                                DataRow bdr = JsonInterFace.ReDirection.RedirectionTable.Rows[i];
                                Dispatcher.Invoke(() =>
                                {
                                    if (i == 0)
                                    {
                                        SelfRedirectionList.Clear();
                                    }
                                    SelfRedirectionList.Add(new ReDirectionClass() { UserType = bdr["UserType"].ToString(), Optimization = bdr["Optimization"].ToString(), Priority = bdr["Priority"].ToString(), RejectMethod = bdr["RejectMethod"].ToString(), Frequency = bdr["Frequency"].ToString(), AddtionFrequency = bdr["AddtionFrequency"].ToString(), Operation = bdr["Operation"].ToString() });
                                });
                            }
                            if (JsonInterFace.ReDirection.RedirectionTable.Rows.Count > 0)
                            {
                                JsonInterFace.ReDirection.RedirectionTable.Rows.Clear();
                            }
                        }
                        catch (Exception ex)
                        {
                            Parameters.PrintfLogsExtended("重定向", ex.Message, ex.StackTrace);
                            Thread.Sleep(1000);
                        }
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
                else
                {
                    Thread.Sleep(1000);
                }
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
            //请求获取设备列表
            try
            {
                //黑名单删除响应
                if (msg == Parameters.WM_BlackListDeleteResponse)
                {
                    BlackListDeleteResponse();
                    BlackListItemsDataDelete.Clear();
                }
                //白名单删除响应
                else if (msg == Parameters.WM_WhiteListDeleteResponse)
                {
                    WhiteListDeleteResponse();
                    WhiteListItemsDataDelete.Clear();
                }
                //黑名单查询为空
                else if (msg == Parameters.WM_BlackListQueryResponse)
                {
                    SelfBlackLists.Clear();
                }
                //白名单查询为空
                else if (msg == Parameters.WM_WhiteListQueryResponse)
                {
                    SelfWhiteLists.Clear();
                }
                //普通用户查询响应
                else if (msg == Parameters.WM_CustomListQueryResponse)
                {
                    SelfCustomLists.Clear();
                }
                //普通用户删除响应
                else if (msg == Parameters.WM_CustomListDeleteResponse)
                {
                    CustomListDeleteResponse();
                    CustomListItemsDataDelete.Clear();
                }
                //用户策略操作响应
                else if (msg == Parameters.WM_RedirectListQueryResponse)
                {
                    SelfRedirectionList.Clear();
                }
                else if (msg == Parameters.WM_RedirectConfigurationResponse)
                {
                    RedirectConfigurationResponse();
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("Windows 消息事件：", ex.Message, ex.StackTrace);
            }
            return hwnd;
        }

        //执行选定项查询
        private void QueryTask()
        {
            if (tabControl.SelectedIndex == 0)
            {
                BlackListQuery();
            }
            else if (tabControl.SelectedIndex == 1)
            {
                WhiteListQuery();
            }
            else if (tabControl.SelectedIndex == 2)
            {
                CustomListQuery();
            }
            else if (tabControl.SelectedIndex == 3)
            {
                Parameters.ConfigType = "Redirect";
                RedirectListQuery();
            }
        }

        //删除白名单
        private void WhiteListDeleteResponse()
        {
            try
            {
                for (int i = 0; i < WhiteListItemsDataDelete.Count; i++)
                {
                    for (int k = 0; k < WhiteListItemsDataDelete[i].Count; k++)
                    {
                        for (int j = 0; j < SelfWhiteLists.Count; j++)
                        {
                            if (WhiteListItemsDataDelete[i][k].IMSI == SelfWhiteLists[j].IMSI)
                            {
                                if (Parameters.ConfigType == "WhiteList")
                                {
                                    JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "删除白名单[" + SelfWhiteLists[j].IMSI + "]成功", "删除白名单", "成功");
                                }
                                SelfWhiteLists.RemoveAt(j);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("删除白名单", ex.Message, ex.StackTrace);
            }
        }

        //删除黑名单
        private void BlackListDeleteResponse()
        {
            try
            {
                for (int i = 0; i < BlackListItemsDataDelete.Count; i++)
                {
                    for (int k = 0; k < BlackListItemsDataDelete[i].Count; k++)
                    { 
                        for (int j = 0; j < SelfBlackLists.Count; j++)
                        {
                            if (BlackListItemsDataDelete[i][k].IMSI == SelfBlackLists[j].IMSI)
                            {
                                JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "删除黑名单[" + SelfBlackLists[j].IMSI + "]成功", "删除黑名单", "成功");
                                SelfBlackLists.RemoveAt(j);

                                //删除对应黑名单追踪表名单
                                IntPtr IMSIHandle = Marshal.StringToCoTaskMemAnsi(BlackListItemsDataDelete[i][k].IMSI);
                                Parameters.SendMessage(Parameters.WinHandle, Parameters.WM_DeleteMeasureReportsIMSIRequest, 0, IMSIHandle.ToInt32());
                                break;
                            }
                        }
                }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("删除黑名单", ex.Message, ex.StackTrace);
            }
        }

        //删除普通用户
        private void CustomListDeleteResponse()
        {
            try
            {
                for (int i = 0; i < CustomListItemsDataDelete.Count; i++)
                {
                    for (int k = 0; k < CustomListItemsDataDelete[i].Count; k++)
                    {
                        for (int j = 0; j < SelfCustomLists.Count; j++)
                        {
                            if (CustomListItemsDataDelete[i][k].IMSI == SelfCustomLists[j].IMSI)
                            {
                                JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "删除普通用户[" + SelfCustomLists[j].IMSI + "]成功", "删除普通用户", "成功");
                                SelfCustomLists.RemoveAt(j);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("删除普通用户", ex.Message, ex.StackTrace);
            }
        }

        private void RedirectConfigurationResponse()
        {
            JsonInterFace.ReDirection.Optimization = "";
            JsonInterFace.ReDirection.Priority = "";
            JsonInterFace.ReDirection.RejectMethod = "";
            JsonInterFace.ReDirection.Frequency = "";
            JsonInterFace.ReDirection.AddtionFrequency = "";
            JsonInterFace.ReDirection.UserType = "";
            JsonInterFace.ReDirection.UserType = "3";
            Parameters.ConfigType = "UnShowRedirect";
            RedirectListQuery();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //中/英文初始化
            try
            {
                if (Parameters.LanguageType.Equals("EN"))
                {
                    this.DataContext = new Language_EN.SpeciallistManageClass();

                    //黑名单表
                    txtIMSI.DataContext = new Language_EN.SpeciallistManageClass();
                    txtIMEI.DataContext = new Language_EN.SpeciallistManageClass();
                    txtAlias.DataContext = new Language_EN.SpeciallistManageClass();
                    txtResourcese.DataContext = new Language_EN.SpeciallistManageClass();
                    txtStation.DataContext = new Language_EN.SpeciallistManageClass();

                    //白名单表
                    txtwIMSI.DataContext = new Language_EN.SpeciallistManageClass();
                    txtwIMEI.DataContext = new Language_EN.SpeciallistManageClass();
                    txtwAlias.DataContext = new Language_EN.SpeciallistManageClass();
                    txtwStation.DataContext = new Language_EN.SpeciallistManageClass();

                    //自定义名单
                    txtCustomListIMSI.DataContext = new Language_EN.SpeciallistManageClass();
                    txtCustomListIMEI.DataContext = new Language_EN.SpeciallistManageClass();
                    txtCustomListAlias.DataContext = new Language_EN.SpeciallistManageClass();
                    txtCustomListResourcese.DataContext = new Language_EN.SpeciallistManageClass();
                    txtCustomListStation.DataContext = new Language_EN.SpeciallistManageClass();

                    //重定向表
                    txtUserType.DataContext = new Language_EN.SpeciallistManageClass();
                    txtReDirection.DataContext = new Language_EN.SpeciallistManageClass();
                    txtOptimization.DataContext = new Language_EN.SpeciallistManageClass();
                    txtRejectMethod.DataContext = new Language_EN.SpeciallistManageClass();
                    txtFrequencyPoint.DataContext = new Language_EN.SpeciallistManageClass();
                    txtAddtionFrequencyPoint.DataContext = new Language_EN.SpeciallistManageClass();
                    txtOperation.DataContext = new Language_EN.SpeciallistManageClass();
                }
                else
                {
                    this.DataContext = new Language_CN.SpeciallistManageClass();

                    //黑名单表
                    txtIMSI.DataContext = new Language_CN.SpeciallistManageClass();
                    txtIMEI.DataContext = new Language_CN.SpeciallistManageClass();
                    txtAlias.DataContext = new Language_CN.SpeciallistManageClass();
                    txtResourcese.DataContext = new Language_CN.SpeciallistManageClass();
                    txtStation.DataContext = new Language_CN.SpeciallistManageClass();

                    //白名单表
                    txtwIMSI.DataContext = new Language_CN.SpeciallistManageClass();
                    txtwIMEI.DataContext = new Language_CN.SpeciallistManageClass();
                    txtwAlias.DataContext = new Language_CN.SpeciallistManageClass();
                    txtwStation.DataContext = new Language_CN.SpeciallistManageClass();

                    //自定义名单
                    txtCustomListIMSI.DataContext = new Language_CN.SpeciallistManageClass();
                    txtCustomListIMEI.DataContext = new Language_CN.SpeciallistManageClass();
                    txtCustomListAlias.DataContext = new Language_CN.SpeciallistManageClass();
                    txtCustomListResourcese.DataContext = new Language_CN.SpeciallistManageClass();
                    txtCustomListStation.DataContext = new Language_CN.SpeciallistManageClass();

                    //重定向表
                    txtUserType.DataContext = new Language_CN.SpeciallistManageClass();
                    txtReDirection.DataContext = new Language_CN.SpeciallistManageClass();
                    txtOptimization.DataContext = new Language_CN.SpeciallistManageClass();
                    txtRejectMethod.DataContext = new Language_CN.SpeciallistManageClass();
                    txtFrequencyPoint.DataContext = new Language_CN.SpeciallistManageClass();
                    txtAddtionFrequencyPoint.DataContext = new Language_CN.SpeciallistManageClass();
                    txtOperation.DataContext = new Language_CN.SpeciallistManageClass();
                }

                #region 权限
                if (RoleTypeClass.RoleType != null && RoleTypeClass.RoleType != "")
                {
                    if (RoleTypeClass.RoleType.Equals("RoleType"))
                    {
                        if (!RoleTypeClass.RolePrivilege.ContainsKey("黑名单"))
                        {
                            tiBlacklist.Visibility = System.Windows.Visibility.Collapsed;
                            tabControl.SelectedIndex = 1;
                        }
                        else
                        {
                            tabControl.SelectedIndex = 0;
                            tiBlacklist.Visibility = System.Windows.Visibility.Visible;
                            btnBlackAdd.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["黑名单"]));
                            btnBlackEdit.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["黑名单"]));
                            btnBlackDelete.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["黑名单"]));
                            btnBlackExport.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["黑名单"]));
                            btnBlackClear.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["黑名单"]));
                        }
                        if (!RoleTypeClass.RolePrivilege.ContainsKey("白名单"))
                        {
                            tiWhitelist.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            tiWhitelist.Visibility = System.Windows.Visibility.Visible;
                            btnWhiteAdd.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["白名单"]));
                            btnWhiteEdit.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["白名单"]));
                            btnWhiteDelete.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["白名单"]));
                            btnWhilteExport.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["白名单"]));
                            btnWhiteClear.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["白名单"]));
                        }
                        if (!RoleTypeClass.RolePrivilege.ContainsKey("重定向设置"))
                        {
                            tiSetting.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            tiSetting.Visibility = System.Windows.Visibility.Visible;
                        }
                        if (!RoleTypeClass.RolePrivilege.ContainsKey("普通用户"))
                        {
                            tiCustomList.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            tiCustomList.Visibility = System.Windows.Visibility.Visible;
                            btnCustomListAdd.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["普通用户"]));
                            btnCustomListEdit.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["普通用户"]));
                            btnCustomListDelete.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["普通用户"]));
                            btnCustomListExport.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["普通用户"]));
                            btnCustomListClear.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["普通用户"]));
                        }
                    }
                    else
                    {
                        if (int.Parse(RoleTypeClass.RoleType) > 3)
                        {
                            //黑白名单曾删改
                            this.btnBlackAdd.Visibility = System.Windows.Visibility.Collapsed;
                            this.btnBlackEdit.Visibility = System.Windows.Visibility.Collapsed;
                            this.btnBlackDelete.Visibility = System.Windows.Visibility.Collapsed;
                            this.btnWhiteAdd.Visibility = System.Windows.Visibility.Collapsed;
                            this.btnWhiteEdit.Visibility = System.Windows.Visibility.Collapsed;
                            this.btnWhiteDelete.Visibility = System.Windows.Visibility.Collapsed;
                        }
                    }
                }
                #endregion

                tvSpecialListDeviceTree.ItemsSource = _usrdomainData;

                //默认显示黑名单列表
                tabControl.SelectedIndex = 0;
                JsonInterFace.BlackList.TabControlItemName = ((TabItem)tabControl.Items[tabControl.SelectedIndex]).Name;
                JsonInterFace.WhiteList.TabControlItemName = null;
                JsonInterFace.CustomList.TabControlItemName = null;

                txtBlackListPageFirstIndex.DataContext = JsonInterFace.BlackList;
                txtBlackListPageSecondIndex.DataContext = JsonInterFace.BlackList;
                txtWhiteListPageFirstIndex.DataContext = JsonInterFace.WhiteList;
                txtWhiteListPageSecondIndex.DataContext = JsonInterFace.WhiteList;
                txtCustomListPageFirstIndex.DataContext = JsonInterFace.CustomList;
                txtCustomListPageSecondIndex.DataContext = JsonInterFace.CustomList;
                txtAdditionalFreq.DataContext = JsonInterFace.ReDirection;
                txtFrequency.DataContext = JsonInterFace.ReDirection;
                cmbRejectMethod.DataContext = JsonInterFace.ReDirection;

                dgBlacklist.ItemsSource = SelfBlackLists;
                dgWhitelist.ItemsSource = SelfWhiteLists;
                dgCustomList.ItemsSource = SelfCustomLists;
                dgRedirect.ItemsSource = SelfRedirectionList;

                //启动线程
                ShowBlackListDataThread.Start();
                ShowWhiteListDataThread.Start();
                ShowCustomListDataThread.Start();
                ShowRedirectInfoThread.Start();
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("特殊名单管理初始化", ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckAll_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)(((CheckBox)sender).IsChecked))
            {
                if (tiBlacklist.IsSelected)
                {
                    BlackListItemsDataDelete.Clear();
                    BlackListItemDataEdit.Clear();
                }
                else if (tiWhitelist.IsSelected)
                {
                    WhiteListItemsDataDelete.Clear();
                    WhiteListItemDataEdit.Clear();
                }

                if (tiBlacklist.IsSelected)
                {
                    for (int i = 0; i < dgBlacklist.Items.Count; i++)
                    {
                        bool flag = true;
                        BlackListClass BlackListParam = new BlackListClass();
                        DataGridRow DGR = (DataGridRow)dgBlacklist.ItemContainerGenerator.ContainerFromIndex(i);
                        if (DGR != null)
                        {
                            var itemChkBox = dgBlacklist.Columns[0].GetCellContent(DGR);

                            GetVisualChild(itemChkBox, true);

                            BlackListParam.IMSI = (dgBlacklist.Items.GetItemAt(i) as BlackListClass).IMSI;
                            BlackListParam.AliasName = (dgBlacklist.Items.GetItemAt(i) as BlackListClass).AliasName;
                            BlackListParam.Resourcese = (dgBlacklist.Items.GetItemAt(i) as BlackListClass).Resourcese;
                            BlackListParam.Station = (dgBlacklist.Items.GetItemAt(i) as BlackListClass).Station;

                            BlackListItemDataEdit.Clear();
                            BlackListItemDataEdit.Add("ismi", BlackListParam.IMSI);
                            BlackListItemDataEdit.Add("des", BlackListParam.AliasName);
                            BlackListItemDataEdit.Add("rbStart", BlackListParam.Resourcese.Split(new char[] { '-' })[0]);
                            BlackListItemDataEdit.Add("rbEnd", BlackListParam.Resourcese.Split(new char[] { '-' })[1]);
                            BlackListItemDataEdit.Add("parentFullPathName", BlackListParam.Station);
                            if (BlackListItemsDataDelete.Count <= 0)
                            {
                                BlackListItemsDataDelete.Add(new List<BlackListClass>
                                { 
                                    BlackListParam
                                });
                            }
                            else
                            {
                                for (int j = 0; j < BlackListItemsDataDelete.Count; j++)
                                {
                                    for (int k = 0; k < BlackListItemsDataDelete[j].Count; k++)
                                    {
                                        if (BlackListItemsDataDelete[j][k].Station == BlackListParam.Station)
                                        {
                                            BlackListItemsDataDelete[j].Add(BlackListParam);
                                            flag = false;
                                            break;
                                        }
                                    }
                                    if (!flag)
                                        break;
                                }
                                if (flag)
                                {
                                    BlackListItemsDataDelete.Add(new List<BlackListClass>
                                        {
                                            BlackListParam
                                        });
                                }
                            }
                        }
                    }
                }
                else if (tiWhitelist.IsSelected)
                {
                    for (int i = 0; i < dgWhitelist.Items.Count; i++)
                    {
                        bool flag = true;
                        WhiteListClass WhiteListParam = new WhiteListClass();
                        DataGridRow DGR = (DataGridRow)dgWhitelist.ItemContainerGenerator.ContainerFromIndex(i);
                        if (DGR != null)
                        {
                            var itemChkBox = dgWhitelist.Columns[0].GetCellContent(DGR);

                            GetVisualChild(itemChkBox, true);

                            WhiteListParam.IMSI = (dgWhitelist.Items.GetItemAt(i) as WhiteListClass).IMSI;
                            WhiteListParam.AliasName = (dgWhitelist.Items.GetItemAt(i) as WhiteListClass).AliasName;
                            WhiteListParam.Station = (dgWhitelist.Items.GetItemAt(i) as WhiteListClass).Station;

                            WhiteListItemDataEdit.Clear();
                            WhiteListItemDataEdit.Add("ismi", WhiteListParam.IMSI);
                            WhiteListItemDataEdit.Add("des", WhiteListParam.AliasName);
                            WhiteListItemDataEdit.Add("parentFullPathName", WhiteListParam.Station);
                            if (WhiteListItemsDataDelete.Count <= 0)
                            {
                                WhiteListItemsDataDelete.Add(new List<WhiteListClass>
                                {
                                    WhiteListParam
                                });
                            }
                            else
                            {
                                for (int j = 0; j < WhiteListItemsDataDelete.Count; j++)
                                {
                                    for (int k = 0; k < WhiteListItemsDataDelete[j].Count; k++)
                                    {
                                        if (WhiteListItemsDataDelete[j][k].Station == WhiteListParam.Station)
                                        {
                                            WhiteListItemsDataDelete[j].Add(WhiteListParam);
                                            flag = false;
                                            break;
                                        }
                                    }
                                    if (!flag)
                                        break;
                                }
                                if (flag)
                                {
                                    WhiteListItemsDataDelete.Add(new List<WhiteListClass>
                                        {
                                            WhiteListParam
                                        });
                                }
                            }
                        }
                    }
                }
                else if (tiCustomList.IsSelected)
                {
                    for (int i = 0; i < dgCustomList.Items.Count; i++)
                    {
                        bool flag = true;
                        CustomListClass CustomListParam = new CustomListClass();
                        DataGridRow DGR = (DataGridRow)dgCustomList.ItemContainerGenerator.ContainerFromIndex(i);
                        if (DGR != null)
                        {
                            var itemChkBox = dgCustomList.Columns[0].GetCellContent(DGR);

                            GetVisualChild(itemChkBox, true);

                            CustomListParam.IMSI = (dgCustomList.Items.GetItemAt(i) as CustomListClass).IMSI;
                            CustomListParam.AliasName = (dgCustomList.Items.GetItemAt(i) as CustomListClass).AliasName;
                            CustomListParam.Resourcese = (dgCustomList.Items.GetItemAt(i) as CustomListClass).Resourcese;
                            CustomListParam.Station = (dgCustomList.Items.GetItemAt(i) as CustomListClass).Station;

                            CustomListItemDataEdit.Clear();
                            CustomListItemDataEdit.Add("ismi", CustomListParam.IMSI);
                            CustomListItemDataEdit.Add("des", CustomListParam.AliasName);
                            CustomListItemDataEdit.Add("rbStart", CustomListParam.Resourcese.Split(new char[] { '-' })[0]);
                            CustomListItemDataEdit.Add("rbEnd", CustomListParam.Resourcese.Split(new char[] { '-' })[1]);
                            CustomListItemDataEdit.Add("parentFullPathName", CustomListParam.Station);
                            if (CustomListItemsDataDelete.Count <= 0)
                            {
                                CustomListItemsDataDelete.Add(new List<CustomListClass>
                                {
                                    CustomListParam
                                });
                            }
                            else
                            {
                                for (int j = 0; j < CustomListItemsDataDelete.Count; j++)
                                {
                                    for (int k = 0; k < CustomListItemsDataDelete[j].Count; k++)
                                    {
                                        if (CustomListItemsDataDelete[j][k].Station == CustomListParam.Station)
                                        {
                                            CustomListItemsDataDelete[j].Add(CustomListParam);
                                            flag = false;
                                            break;
                                        }
                                    }
                                    if (!flag)
                                        break;
                                }
                                if (flag)
                                {
                                    CustomListItemsDataDelete.Add(new List<CustomListClass>
                                        {
                                            CustomListParam
                                        });
                                }
                            }
                        }
                    }
                }
            }
            //返选
            else
            {
                //黑名单
                if (tiBlacklist.IsSelected)
                {
                    for (int i = 0; i < dgBlacklist.Items.Count; i++)
                    {
                        DataGridRow DGR = (DataGridRow)dgBlacklist.ItemContainerGenerator.ContainerFromIndex(i);
                        if (DGR != null)
                        {
                            var itemChkBox = dgBlacklist.Columns[0].GetCellContent(DGR);
                            GetVisualChild(itemChkBox, false);
                        }
                    }

                    BlackListItemsDataDelete.Clear();
                    BlackListItemDataEdit.Clear();
                }
                //白名单
                else if (tiWhitelist.IsSelected)
                {
                    for (int i = 0; i < dgWhitelist.Items.Count; i++)
                    {
                        DataGridRow DGR = (DataGridRow)dgWhitelist.ItemContainerGenerator.ContainerFromIndex(i);
                        if (DGR != null)
                        {
                            var itemChkBox = dgWhitelist.Columns[0].GetCellContent(DGR);
                            GetVisualChild(itemChkBox, false);
                        }
                    }

                    WhiteListItemsDataDelete.Clear();
                    WhiteListItemDataEdit.Clear();
                }
                //普通用户
                else if (tiCustomList.IsSelected)
                {
                    for (int i = 0; i < dgCustomList.Items.Count; i++)
                    {
                        DataGridRow DGR = (DataGridRow)dgCustomList.ItemContainerGenerator.ContainerFromIndex(i);
                        if (DGR != null)
                        {
                            var itemChkBox = dgCustomList.Columns[0].GetCellContent(DGR);
                            GetVisualChild(itemChkBox, false);
                        }
                    }

                    CustomListItemsDataDelete.Clear();
                    CustomListItemDataEdit.Clear();
                }
            }
        }

        /// <summary>
        /// 单项选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IsCheckAll_Click(object sender, RoutedEventArgs e)
        {
            BlackListClass BlackListParam = new BlackListClass();
            WhiteListClass WhiteListParam = new WhiteListClass();
            CustomListClass CustomListParam = new CustomListClass();
            bool flag = true;
            try
            {
                if ((bool)((CheckBox)sender).IsChecked)
                {
                    if (tiBlacklist.IsSelected)
                    {
                        BlackListParam.IMSI = (dgBlacklist.SelectedItem as BlackListClass).IMSI;
                        BlackListParam.AliasName = (dgBlacklist.SelectedItem as BlackListClass).AliasName;
                        BlackListParam.Resourcese = (dgBlacklist.SelectedItem as BlackListClass).Resourcese;
                        BlackListParam.Station = (dgBlacklist.SelectedItem as BlackListClass).Station;

                        BlackListItemDataEdit.Clear();
                        BlackListItemDataEdit.Add("ismi", BlackListParam.IMSI);
                        BlackListItemDataEdit.Add("des", BlackListParam.AliasName);
                        BlackListItemDataEdit.Add("rbStart", BlackListParam.Resourcese.Split(new char[] { '-' })[0]);
                        BlackListItemDataEdit.Add("rbEnd", BlackListParam.Resourcese.Split(new char[] { '-' })[1]);
                        BlackListItemDataEdit.Add("parentFullPathName", BlackListParam.Station);

                        if (BlackListItemsDataDelete.Count <= 0)
                        {
                            BlackListItemsDataDelete.Add(new List<BlackListClass>
                                {
                                    BlackListParam
                                });
                        }
                        else
                        {
                            for (int j = 0; j < BlackListItemsDataDelete.Count; j++)
                            {
                                for (int k = 0; k < BlackListItemsDataDelete[j].Count; k++)
                                {
                                    if (BlackListItemsDataDelete[j][k].Station == BlackListParam.Station)
                                    {
                                        BlackListItemsDataDelete[j].Add(BlackListParam);
                                        flag = false;
                                        break;
                                    }
                                }
                                if (!flag)
                                    break;
                            }
                            if (flag)
                            {
                                BlackListItemsDataDelete.Add(new List<BlackListClass>
                                        {
                                            BlackListParam
                                        });
                            }
                        }
                    }
                    else if (tiWhitelist.IsSelected)
                    {
                        WhiteListParam.IMSI = (dgWhitelist.SelectedItem as WhiteListClass).IMSI;
                        WhiteListParam.AliasName = (dgWhitelist.SelectedItem as WhiteListClass).AliasName;
                        WhiteListParam.Station = (dgWhitelist.SelectedItem as WhiteListClass).Station;

                        WhiteListItemDataEdit.Clear();
                        WhiteListItemDataEdit.Add("ismi", WhiteListParam.IMSI);
                        WhiteListItemDataEdit.Add("des", WhiteListParam.AliasName);
                        WhiteListItemDataEdit.Add("parentFullPathName", WhiteListParam.Station);

                        if (WhiteListItemsDataDelete.Count <= 0)
                        {
                            WhiteListItemsDataDelete.Add(new List<WhiteListClass>
                                {
                                    WhiteListParam
                                });
                        }
                        else
                        {
                            for (int j = 0; j < WhiteListItemsDataDelete.Count; j++)
                            {
                                for (int k = 0; k < WhiteListItemsDataDelete[j].Count; k++)
                                {
                                    if (WhiteListItemsDataDelete[j][k].Station == WhiteListParam.Station)
                                    {
                                        WhiteListItemsDataDelete[j].Add(WhiteListParam);
                                        flag = false;
                                        break;
                                    }
                                }
                                if (!flag)
                                    break;
                            }
                            if (flag)
                            {
                                WhiteListItemsDataDelete.Add(new List<WhiteListClass>
                                        {
                                            WhiteListParam
                                        });
                            }
                        }
                    }
                    else if (tiCustomList.IsSelected)
                    {
                        CustomListParam.IMSI = (dgCustomList.SelectedItem as CustomListClass).IMSI;
                        CustomListParam.AliasName = (dgCustomList.SelectedItem as CustomListClass).AliasName;
                        CustomListParam.Station = (dgCustomList.SelectedItem as CustomListClass).Station;

                        CustomListItemDataEdit.Clear();
                        CustomListItemDataEdit.Add("ismi", CustomListParam.IMSI);
                        CustomListItemDataEdit.Add("des", CustomListParam.AliasName);
                        CustomListItemDataEdit.Add("parentFullPathName", CustomListParam.Station);

                        if (CustomListItemsDataDelete.Count <= 0)
                        {
                            CustomListItemsDataDelete.Add(new List<CustomListClass>
                                {
                                    CustomListParam
                                });
                        }
                        else
                        {
                            for (int j = 0; j < CustomListItemsDataDelete.Count; j++)
                            {
                                for (int k = 0; k < CustomListItemsDataDelete[j].Count; k++)
                                {
                                    if (CustomListItemsDataDelete[j][k].Station == CustomListParam.Station)
                                    {
                                        CustomListItemsDataDelete[j].Add(CustomListParam);
                                        flag = false;
                                        break;
                                    }
                                }
                                if (!flag)
                                    break;
                            }
                            if (flag)
                            {
                                CustomListItemsDataDelete.Add(new List<CustomListClass>
                                        {
                                            CustomListParam
                                        });
                            }
                        }
                    }
                }
                else
                {
                    if (tiBlacklist.IsSelected)
                    {
                        BlackListParam.IMSI = (dgBlacklist.SelectedItem as BlackListClass).IMSI;
                        BlackListParam.AliasName = (dgBlacklist.SelectedItem as BlackListClass).AliasName;
                        BlackListParam.Resourcese = (dgBlacklist.SelectedItem as BlackListClass).Resourcese;
                        BlackListParam.Station = (dgBlacklist.SelectedItem as BlackListClass).Station;

                        BlackListItemDataEdit.Clear();

                        for (int i = 0; i < BlackListItemsDataDelete.Count; i++)
                        {
                            for (int j = 0; j < BlackListItemsDataDelete[i].Count; j++)
                            {
                                if (BlackListItemsDataDelete[i][j].IMSI.Equals(BlackListParam.IMSI)
                                && BlackListItemsDataDelete[i][j].AliasName.Equals(BlackListParam.AliasName)
                                && BlackListItemsDataDelete[i][j].Resourcese.Equals(BlackListParam.Resourcese)
                                && BlackListItemsDataDelete[i][j].Station.Equals(BlackListParam.Station))
                                {
                                    BlackListItemsDataDelete[i].RemoveAt(j);
                                    break;
                                }
                            }
                        }
                    }
                    else if (tiWhitelist.IsSelected)
                    {
                        WhiteListParam.IMSI = (dgWhitelist.SelectedItem as WhiteListClass).IMSI;
                        WhiteListParam.AliasName = (dgWhitelist.SelectedItem as WhiteListClass).AliasName;
                        WhiteListParam.Station = (dgWhitelist.SelectedItem as WhiteListClass).Station;

                        WhiteListItemDataEdit.Clear();

                        for (int i = 0; i < WhiteListItemsDataDelete.Count; i++)
                        {
                            for (int j = 0; j < WhiteListItemsDataDelete[i].Count; j++)
                            {
                                if (WhiteListItemsDataDelete[i][j].IMSI.Equals(WhiteListParam.IMSI)
                                && WhiteListItemsDataDelete[i][j].AliasName.Equals(WhiteListParam.AliasName)
                               && WhiteListItemsDataDelete[i][j].Station.Equals(WhiteListParam.Station))
                                {
                                    WhiteListItemsDataDelete[i].RemoveAt(j);
                                    break;
                                }
                            }
                        }
                    }
                    else if (tiCustomList.IsSelected)
                    {
                        CustomListParam.IMSI = (dgCustomList.SelectedItem as CustomListClass).IMSI;
                        CustomListParam.AliasName = (dgCustomList.SelectedItem as CustomListClass).AliasName;
                        CustomListParam.Station = (dgCustomList.SelectedItem as CustomListClass).Station;

                        CustomListItemDataEdit.Clear();

                        for (int i = 0; i < CustomListItemsDataDelete.Count; i++)
                        {
                            for (int j = 0; j < CustomListItemsDataDelete[i].Count; j++)
                            {
                                if (CustomListItemsDataDelete[i][j].IMSI.Equals(CustomListParam.IMSI)
                                && CustomListItemsDataDelete[i][j].AliasName.Equals(CustomListParam.AliasName)
                               && CustomListItemsDataDelete[i][j].Station.Equals(CustomListParam.Station))
                                {
                                    CustomListItemsDataDelete[i].RemoveAt(j);
                                    break;
                                }
                            }
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
        /// 按ESC退出
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

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void tiBlacklist_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void tiWhitelist_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void tiSetting_Loaded(object sender, RoutedEventArgs e)
        {

        }

        //黑名单查询
        private void btnBlackFilter_Click(object sender, RoutedEventArgs e)
        {
            BlackListQuery();
        }

        //黑名单查询
        public void BlackListQuery()
        {
            try
            {
                JsonInterFace.BlackList.BlackListTable.Rows.Clear();
                SelfBlackLists.Clear();
                if (JsonInterFace.BlackList.ParameterList.Count > 0)
                {
                    for (int i = 0; i < JsonInterFace.BlackList.ParameterList.Count; i++)
                    {
                        if (NetWorkClient.ControllerServer.Connected)
                        {
                            Parameters.ConfigType = "BlackList";
                            NetWorkClient.ControllerServer.Send(
                                                                JsonInterFace.APBlackListRequest(
                                                                                                    JsonInterFace.BlackList.ParameterList[i].NodeType,
                                                                                                    JsonInterFace.BlackList.ParameterList[i].DeviceFullPathName,
                                                                                                    JsonInterFace.BlackList.ParameterList[i].DomainFullPathName,
                                                                                                    "black",
                                                                                                    txtBlackIMSI.Text,
                                                                                                    "",
                                                                                                    txtBlackOtherName.Text,
                                                                                                    "",
                                                                                                    "")
                                                               );
                        }
                    }
                }
                else
                {
                    MessageBox.Show("请先选择左边列表中的站点或设备名称！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("黑名单查询失败", ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// 称动窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        private void FrmSpeciallistManagement_Activated(object sender, EventArgs e)
        {
            WindowInteropHelper GetWindowHandleHelper = new WindowInteropHelper(this);
            Parameters.SpeciallistWinHandle = GetWindowHandleHelper.Handle;
        }

        /// <summary>
        /// 白名单查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnWhiteFilter_Click(object sender, RoutedEventArgs e)
        {
            WhiteListQuery();
        }

        //白名单查询
        public void WhiteListQuery()
        {
            try
            {
                JsonInterFace.WhiteList.WhiteListTable.Rows.Clear();
                SelfWhiteLists.Clear();
                if (JsonInterFace.WhiteList.ParameterList.Count > 0)
                {
                    for (int i = 0; i < JsonInterFace.WhiteList.ParameterList.Count; i++)
                    {
                        if (NetWorkClient.ControllerServer.Connected)
                        {
                            Parameters.ConfigType = "WhiteList";
                            NetWorkClient.ControllerServer.Send(
                                                                JsonInterFace.APWhiteListRequest(
                                                                                                JsonInterFace.WhiteList.ParameterList[i].NodeType,
                                                                                                JsonInterFace.WhiteList.ParameterList[i].DeviceFullPathName,
                                                                                                JsonInterFace.WhiteList.ParameterList[i].DomainFullPathName,
                                                                                                "white",
                                                                                                txtWhiteIMSI.Text,
                                                                                                "",
                                                                                                txtWhiteOtherName.Text,
                                                                                                "",
                                                                                                "")
                                                                );
                        }
                    }
                }
                else
                {
                    MessageBox.Show("请先选择左边列表中的站点或设备名称！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("白名单查询失败", ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// 黑名单上一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBBackPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string PageIndexStart = JsonInterFace.BlackList.CurPageIndex.Split(new char[] { ':' })[0];
                string PageIndexEnd = JsonInterFace.BlackList.CurPageIndex.Split(new char[] { ':' })[1];

                if (int.Parse(PageIndexStart) - 1 <= 0)
                {
                    MessageBox.Show("当前已经是第一页", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (int.Parse(PageIndexStart) - 1 >= 0)
                {
                    if (NetController.NetWorkClient.ControllerServer.Connected)
                    {
                        Parameters.ConfigType = "BlackList";
                        string PageIndex = (int.Parse(PageIndexStart) - 1).ToString() + ":" + PageIndexEnd;
                        SelfBlackLists.Clear();
                        NetWorkClient.ControllerServer.Send(JsonInterFace.APBWListDataPageRequest(PageIndex));
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

        /// <summary>
        /// 黑名单下一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBNextPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string PageIndexStart = JsonInterFace.BlackList.CurPageIndex.Split(new char[] { ':' })[0];
                string PageIndexEnd = JsonInterFace.BlackList.CurPageIndex.Split(new char[] { ':' })[1];

                if (int.Parse(PageIndexStart) + 1 > int.Parse(PageIndexEnd))
                {
                    MessageBox.Show("当前已经是最后一页", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (int.Parse(PageIndexStart) + 1 <= int.Parse(PageIndexEnd))
                {
                    if (NetController.NetWorkClient.ControllerServer.Connected)
                    {
                        Parameters.ConfigType = "BlackList";
                        string PageIndex = (int.Parse(PageIndexStart) + 1).ToString() + ":" + PageIndexEnd;
                        SelfBlackLists.Clear();
                        NetWorkClient.ControllerServer.Send(JsonInterFace.APBWListDataPageRequest(PageIndex));
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

        /// <summary>
        /// 白名单上一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBackPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string PageIndexStart = JsonInterFace.WhiteList.CurPageIndex.Split(new char[] { ':' })[0];
                string PageIndexEnd = JsonInterFace.WhiteList.CurPageIndex.Split(new char[] { ':' })[1];

                if (int.Parse(PageIndexStart) - 1 <= 0)
                {
                    MessageBox.Show("当前已经是第一页", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (int.Parse(PageIndexStart) - 1 >= 0)
                {
                    if (NetController.NetWorkClient.ControllerServer.Connected)
                    {
                        Parameters.ConfigType = "WhiteList";
                        string PageIndex = (int.Parse(PageIndexStart) - 1).ToString() + ":" + PageIndexEnd;
                        SelfWhiteLists.Clear();
                        NetWorkClient.ControllerServer.Send(JsonInterFace.APBWListDataPageRequest(PageIndex));
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

        /// <summary>
        /// 白名单下一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string PageIndexStart = JsonInterFace.WhiteList.CurPageIndex.Split(new char[] { ':' })[0];
                string PageIndexEnd = JsonInterFace.WhiteList.CurPageIndex.Split(new char[] { ':' })[1];

                if (int.Parse(PageIndexStart) + 1 > int.Parse(PageIndexEnd))
                {
                    MessageBox.Show("当前已经是最后一页", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (int.Parse(PageIndexStart) + 1 <= int.Parse(PageIndexEnd))
                {
                    if (NetController.NetWorkClient.ControllerServer.Connected)
                    {
                        Parameters.ConfigType = "WhiteList";
                        string PageIndex = (int.Parse(PageIndexStart) + 1).ToString() + ":" + PageIndexEnd;
                        SelfWhiteLists.Clear();
                        NetWorkClient.ControllerServer.Send(JsonInterFace.APBWListDataPageRequest(PageIndex));
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

        private void btnBlackAdd_Click(object sender, RoutedEventArgs e)
        {
            if (tiBlacklist.IsSelected)
            {
                if (JsonInterFace.BlackList.ParameterList.Count <= 0)
                {
                    MessageBox.Show("请选择左边列表的站点或设备！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            SpecialListManage.BWListAddWindow BWListAddWin = new SpecialListManage.BWListAddWindow();
            Parameters.ConfigType = "BlackList";
            BWListAddWin.ShowDialog();
            BlackListQuery();
        }

        /// <summary>
        /// 删除黑名单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBlackDelete_Click(object sender, RoutedEventArgs e)
        {
            string TipsContent = string.Empty;
            int DelayTime = 5;
            bool Flag = false;

            System.Timers.Timer WaitResultTimer = new System.Timers.Timer();
            WaitResultTimer.AutoReset = true;
            WaitResultTimer.Interval = Parameters.SpecialListInputDelay * 1000;
            WaitResultTimer.Elapsed += WaitResultTimer_Elapsed;

            JsonInterFace.ActionResultStatus.APCount = 0;
            Parameters.UniversalCounter = 0;
            JsonInterFace.DeviceListRequestCompleteStatus.MaxLoading = 0;
            JsonInterFace.DeviceListRequestCompleteStatus.ValueLoading = 0;
            string DomainFullPathName = string.Empty;
            string DeviceName = string.Empty;
            JsonInterFace.GSMV2IMSIControlInfo.IsSucess = true;

            List<List<List<Dictionary<string, string>>>> DataList = null;
            List<string> SpecialListID = new List<string>();
            List<APATTributes> APATTributeList = new List<APATTributes>();

            try
            {
                if (BlackListItemsDataDelete.Count <= 0)
                {
                    MessageBox.Show("请选择要删除的黑名单！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (MessageBox.Show("确定删除吗？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        SpecialListID.Clear();
                        Parameters.ConfigType = "BlackList";
                        GetDataToDelete<BlackListClass>(ref DataList, BlackListItemsDataDelete, APATTributeList, Parameters.ConfigType);

                        //===========启动进度条窗口===========
                        if (SubWindow.ProgressBarWindow.BWOProgressBarParameter == null)
                        {
                            ProgressBarWin = new SubWindow.ProgressBarWindow();
                        }
                        SubWindow.ProgressBarWindow.BWOProgressBarParameter.TipsContent = "正在删除黑名单, 请稍后....";
                        SubWindow.ProgressBarWindow.BWOProgressBarParameter.SubmitTotal = APATTributeList.Count;
                        SubWindow.ProgressBarWindow.BWOProgressBarParameter.CompleteTotal = APATTributeList.Count;
                        SubWindow.ProgressBarWindow.BWOProgressBarParameter.SubmitValue = 0;
                        SubWindow.ProgressBarWindow.BWOProgressBarParameter.CompleteValue = 0;

                        Dispatcher.Invoke(() =>
                        {
                            ProgressBarWin.Show();
                        });
                        //==================================

                        //提交
                        new Thread(() =>
                        {
                            //提交设备
                            for (int j = 0; j < DataList.Count; j++)
                            {
                                if (APATTributeList[j].OnLine != "1")
                                    continue;
                                //获取参数
                                JsonInterFace.ActionResultStatus.ResoultStatus = string.Empty;
                                JsonInterFace.ActionResultStatus.Finished = true;
                                //数据组
                                for (int i = 0; i <= DataList[j].Count; i++)
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
                                            //完成进度
                                            SubWindow.ProgressBarWindow.BWOProgressBarParameter.CompleteValue++;

                                            //打印状态消息
                                            if (JsonInterFace.ActionResultStatus.ResoultStatus != null && JsonInterFace.ActionResultStatus.ResoultStatus != "")
                                            {
                                                string DelStatus = string.Empty;
                                                switch (Convert.ToInt32(JsonInterFace.ActionResultStatus.ResoultStatus))
                                                {
                                                    case 0:
                                                        DelStatus = "成功";
                                                        break;
                                                    case 1:
                                                        Flag = true;
                                                        DelStatus = "失败";
                                                        break;
                                                    default:
                                                        Flag = true;
                                                        DelStatus = "未知错误";
                                                        break;
                                                }
                                                for (int k = 0; k < DataList[j][i > DataList[j].Count - 1 ? DataList[j].Count - 1 : i - 1].Count; k++)
                                                {
                                                    string IMSI = string.Empty;
                                                    foreach (KeyValuePair<string, string> Item in DataList[j][i > DataList[j].Count - 1 ? DataList[j].Count - 1 : i - 1][k])
                                                    {
                                                        if (Item.Key == "imsi")
                                                        {
                                                            IMSI = Item.Value;
                                                            SpecialListID.Add(IMSI);
                                                            break;
                                                        }
                                                    }
                                                    JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "设备[" + APATTributeList[j].FullName + "]删除黑名单[" + IMSI + "]", "删除黑名单", DelStatus);
                                                }
                                            }

                                            break;
                                        }
                                        //超时
                                        else if (JsonInterFace.ActionResultStatus.NoResultErrorDeviceName)
                                        {
                                            Flag = true;
                                            WaitResultTimer.Stop();
                                            lock (JsonInterFace.ActionResultStatus.FinishedLock)
                                            {
                                                JsonInterFace.ActionResultStatus.Finished = false;
                                            }
                                            JsonInterFace.ActionResultStatus.NoResultErrorDeviceName = false;
                                            SubWindow.ProgressBarWindow.BWOProgressBarParameter.CompleteValue++;

                                            //打印状态消息
                                            if (JsonInterFace.ActionResultStatus.ResoultStatus != null || JsonInterFace.ActionResultStatus.ResoultStatus != "")
                                            {
                                                string DelStatus = "超时";
                                                for (int k = 0; k < DataList[j][i > DataList[j].Count - 1 ? DataList[j].Count - 1 : i - 1].Count; k++)
                                                {
                                                    string IMSI = string.Empty;
                                                    foreach (KeyValuePair<string, string> Item in DataList[j][i > DataList[j].Count - 1 ? DataList[j].Count - 1 : i - 1][k])
                                                    {
                                                        if (Item.Key == "imsi")
                                                        {
                                                            IMSI = Item.Value;
                                                            break;
                                                        }
                                                    }
                                                    JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "设备[" + APATTributeList[j].FullName + "]删除黑名单[" + IMSI + "]", "删除黑名单", DelStatus);
                                                }
                                            }
                                            break;
                                        }
                                        else
                                        {
                                            Thread.Sleep(DelayTime += 5);
                                        }
                                    }

                                    if (i == DataList[j].Count) { break; }

                                    WaitResultTimer.Start();

                                    #region GSMV2 and CDMA
                                    if (APATTributeList[j].Mode == DeviceType.GSMV2 || APATTributeList[j].Mode == DeviceType.CDMA)
                                    {
                                        //数据组
                                        Dictionary<string, string> ParaList = new Dictionary<string, string>();
                                        Parameters.ActionType = "Delete Special";
                                        ParaList.Add("wTotalImsi", BlackListItemsDataDelete[j].Count.ToString());
                                        ParaList.Add("bActionType", "2");
                                        for (int k = 0; k < BlackListItemsDataDelete[j].Count; k++)
                                        {
                                            string IMSI = string.Empty;
                                            IMSI = BlackListItemsDataDelete[j][k].IMSI;
                                            ParaList.Add("bIMSI_#" + k.ToString() + "#", IMSI);
                                            ParaList.Add("bUeActionFlag_#" + k.ToString() + "#", "5");
                                        }
                                        JsonInterFace.ResultMessageList.Clear();
                                        NetWorkClient.ControllerServer.Send(
                                                                            JsonInterFace.GSMV2IMSIActionRequest(
                                                                                                                APATTributeList[j].FullName,
                                                                                                                APATTributeList[j].DeviceName,
                                                                                                                APATTributeList[j].IpAddr,
                                                                                                                APATTributeList[j].Port,
                                                                                                                APATTributeList[j].InnerType,
                                                                                                                APATTributeList[j].SN,
                                                                                                                ParaList,
                                                                                                                "0"
                                                                                                               )
                                                                           );
                                        break;
                                    }
                                    #endregion
                                    else if (APATTributeList[j].Mode != DeviceType.GSM)
                                    {
                                        //发送
                                        NetWorkClient.ControllerServer.Send(
                                            JsonInterFace.APBlackListDataDeleteRequest(
                                                "device",
                                                APATTributeList[j].FullName,
                                                APATTributeList[j].DomainFullPathName,
                                                DataList[j][i]
                                            ));
                                    }
                                    //提交进度
                                    SubWindow.ProgressBarWindow.BWOProgressBarParameter.SubmitValue++;
                                }
                            }

                            //完成后关闭进度窗口
                            Parameters.SendMessage(SubWindow.ProgressBarWindow.BWOProgressBarParameter.SelfHandle, Parameters.WM_ProgressBarWindowClose, 0, 0);

                            //清理
                            for (int i = 0; i < SpecialListID.Count; i++)
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    for (int j = 0; j < SelfBlackLists.Count; j++)
                                    {
                                        if (SpecialListID[i] == (SelfBlackLists[j] as BlackListClass).IMSI)
                                        {
                                            SelfBlackLists.RemoveAt(j);
                                            break;
                                        }
                                    }
                                });
                            }

                            Dispatcher.Invoke(() =>
                            {
                                if (Flag)
                                {
                                    MessageBox.Show("删除黑名单失败,详见日志！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                }
                                else
                                {
                                    MessageBox.Show("删除黑名单成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                BlackListQuery();
                            });
                            //清空选中IMSI
                            BlackListItemsDataDelete.Clear();
                        }).Start();
                    }
                    else
                    {
                        MessageBox.Show("网络与服服器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        //待实现
        private void btnBlackEdit_Click(object sender, RoutedEventArgs e)
        {
            //SpecialListManage.BWListEditorWindow BWListEidtorWin = new SpecialListManage.BWListEditorWindow();
            //BWListEidtorWin.ShowDialog();
        }

        /// <summary>
        /// 操作复选框
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="isChecked"></param>
        public void GetVisualChild(DependencyObject parent, bool isChecked)
        {
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                DependencyObject Element = (DependencyObject)VisualTreeHelper.GetChild(parent, i);
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

        private void btnWhiteAdd_Click(object sender, RoutedEventArgs e)
        {
            if (tiWhitelist.IsSelected)
            {
                if (JsonInterFace.WhiteList.ParameterList.Count <= 0)
                {
                    MessageBox.Show("请选择左边列表的站点或设备！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            SpecialListManage.BWListAddWindow BWListAddWin = new SpecialListManage.BWListAddWindow();
            Parameters.ConfigType = "WhiteList";
            BWListAddWin.ShowDialog();
            WhiteListQuery();
        }

        //删除白名单
        private void btnWhiteDelete_Click(object sender, RoutedEventArgs e)
        {
            string TipsContent = string.Empty;
            int DelayTime = 5;
            bool Flag = false;

            System.Timers.Timer WaitResultTimer = new System.Timers.Timer();
            WaitResultTimer.AutoReset = true;
            WaitResultTimer.Interval = Parameters.SpecialListInputDelay * 1000;
            WaitResultTimer.Elapsed += WaitResultTimer_Elapsed;

            JsonInterFace.ActionResultStatus.APCount = 0;
            Parameters.UniversalCounter = 0;
            JsonInterFace.DeviceListRequestCompleteStatus.MaxLoading = 0;
            JsonInterFace.DeviceListRequestCompleteStatus.ValueLoading = 0;
            string DomainFullPathName = string.Empty;
            string DeviceName = string.Empty;
            JsonInterFace.GSMV2IMSIControlInfo.IsSucess = true;

            List<List<List<Dictionary<string, string>>>> DataList = null;
            List<string> SpecialListID = new List<string>();
            List<APATTributes> APATTributeList = new List<APATTributes>();

            try
            {
                if (WhiteListItemsDataDelete.Count <= 0)
                {
                    MessageBox.Show("请选择要删除的白名单！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (MessageBox.Show("确定删除吗？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        SpecialListID.Clear();
                        Parameters.ConfigType = "WhiteList";
                        GetDataToDelete<WhiteListClass>(ref DataList, WhiteListItemsDataDelete, APATTributeList, Parameters.ConfigType);

                        //===========启动进度条窗口===========
                        if (SubWindow.ProgressBarWindow.BWOProgressBarParameter == null)
                        {
                            ProgressBarWin = new SubWindow.ProgressBarWindow();
                        }
                        SubWindow.ProgressBarWindow.BWOProgressBarParameter.TipsContent = "正在删除白名单, 请稍后....";
                        SubWindow.ProgressBarWindow.BWOProgressBarParameter.SubmitTotal = JsonInterFace.WhiteList.Count * JsonInterFace.WhiteList.ParameterList.Count;
                        SubWindow.ProgressBarWindow.BWOProgressBarParameter.CompleteTotal = JsonInterFace.WhiteList.Count * JsonInterFace.WhiteList.ParameterList.Count;
                        SubWindow.ProgressBarWindow.BWOProgressBarParameter.SubmitValue = 0;
                        SubWindow.ProgressBarWindow.BWOProgressBarParameter.CompleteValue = 0;

                        Dispatcher.Invoke(() =>
                        {
                            ProgressBarWin.Show();
                        });
                        //==================================

                        //提交
                        new Thread(() =>
                        {
                            //提交到站点或设备
                            for (int j = 0; j < DataList.Count; j++)
                            {
                                if (APATTributeList[j].OnLine != "1")
                                    continue;
                                //获取参数
                                JsonInterFace.ActionResultStatus.ResoultStatus = string.Empty;
                                JsonInterFace.ActionResultStatus.Finished = true;
                                //数据组
                                for (int i = 0; i <= DataList[j].Count; i++)
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
                                            //完成进度
                                            SubWindow.ProgressBarWindow.BWOProgressBarParameter.CompleteValue++;

                                            //打印状态消息
                                            if (JsonInterFace.ActionResultStatus.ResoultStatus != null && JsonInterFace.ActionResultStatus.ResoultStatus != "")
                                            {
                                                string DelStatus = string.Empty;
                                                switch (Convert.ToInt32(JsonInterFace.ActionResultStatus.ResoultStatus))
                                                {
                                                    case 0:
                                                        DelStatus = "成功";
                                                        break;
                                                    case 1:
                                                        Flag = true;
                                                        DelStatus = "失败";
                                                        break;
                                                    default:
                                                        Flag = true;
                                                        DelStatus = "未知错误";
                                                        break;
                                                }
                                                for (int k = 0; k < DataList[j][i > DataList[j].Count - 1 ? DataList[j].Count - 1 : i - 1].Count; k++)
                                                {
                                                    string IMSI = string.Empty;
                                                    foreach (KeyValuePair<string, string> Item in DataList[j][i > DataList[j].Count - 1 ? DataList[j].Count - 1 : i - 1][k])
                                                    {
                                                        if (Item.Key == "imsi")
                                                        {
                                                            IMSI = Item.Value;
                                                            SpecialListID.Add(IMSI);
                                                            break;
                                                        }
                                                    }
                                                    JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "设备[" + APATTributeList[j].FullName + "]删除白名单[" + IMSI + "]", "删除白名单", DelStatus);
                                                }
                                            }

                                            break;
                                        }
                                        //超时
                                        else if (JsonInterFace.ActionResultStatus.NoResultErrorDeviceName)
                                        {
                                            Flag = true;
                                            WaitResultTimer.Stop();
                                            lock (JsonInterFace.ActionResultStatus.FinishedLock)
                                            {
                                                JsonInterFace.ActionResultStatus.Finished = false;
                                            }
                                            JsonInterFace.ActionResultStatus.NoResultErrorDeviceName = false;
                                            SubWindow.ProgressBarWindow.BWOProgressBarParameter.CompleteValue++;

                                            //打印状态消息
                                            if (JsonInterFace.ActionResultStatus.ResoultStatus != null || JsonInterFace.ActionResultStatus.ResoultStatus != "")
                                            {
                                                string DelStatus = "超时";
                                                for (int k = 0; k < DataList[j][i > DataList[j].Count - 1 ? DataList[j].Count - 1 : i - 1].Count; k++)
                                                {
                                                    string IMSI = string.Empty;
                                                    foreach (KeyValuePair<string, string> Item in DataList[j][i > DataList[j].Count - 1 ? DataList[j].Count - 1 : i - 1][k])
                                                    {
                                                        if (Item.Key == "imsi")
                                                        {
                                                            IMSI = Item.Value;
                                                            break;
                                                        }
                                                    }
                                                    JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "设备[" + APATTributeList[j].FullName + "]删除白名单[" + IMSI + "]", "删除白名单", DelStatus);
                                                }
                                            }
                                            break;
                                        }
                                        else
                                        {
                                            Thread.Sleep(DelayTime += 5);
                                        }
                                    }

                                    if (i == DataList[j].Count) { break; }

                                    WaitResultTimer.Start();
                                    #region GSMV2 and CDMA
                                    if (APATTributeList[j].Mode == DeviceType.GSMV2 || APATTributeList[j].Mode == DeviceType.CDMA)
                                    {
                                        //数据组
                                        Dictionary<string, string> ParaList = new Dictionary<string, string>();
                                        Parameters.ActionType = "Delete Special";
                                        ParaList.Add("wTotalImsi", WhiteListItemsDataDelete[j].Count.ToString());
                                        ParaList.Add("bActionType", "2");
                                        for (int k = 0; k < WhiteListItemsDataDelete[j].Count; k++)
                                        {
                                            string IMSI = string.Empty;
                                            IMSI = WhiteListItemsDataDelete[j][k].IMSI;
                                            ParaList.Add("bIMSI_#" + k.ToString() + "#", IMSI);
                                            ParaList.Add("bUeActionFlag_#" + k.ToString() + "#", "1");
                                        }
                                        JsonInterFace.ResultMessageList.Clear();
                                        NetWorkClient.ControllerServer.Send(
                                                                            JsonInterFace.GSMV2IMSIActionRequest(
                                                                                                                APATTributeList[j].FullName,
                                                                                                                APATTributeList[j].DeviceName,
                                                                                                                APATTributeList[j].IpAddr,
                                                                                                                APATTributeList[j].Port,
                                                                                                                APATTributeList[j].InnerType,
                                                                                                                APATTributeList[j].SN,
                                                                                                                ParaList,
                                                                                                                "0"
                                                                                                               )
                                                                           );
                                        break;

                                    }
                                    #endregion
                                    else if (APATTributeList[j].Mode != DeviceType.GSM)
                                    {
                                        //发送
                                        NetWorkClient.ControllerServer.Send(
                                            JsonInterFace.APWhiteListDataDeleteRequest(
                                                "device",
                                                APATTributeList[j].FullName,
                                                APATTributeList[j].DomainFullPathName,
                                                DataList[j][i]
                                            ));

                                        //提交进度
                                        SubWindow.ProgressBarWindow.BWOProgressBarParameter.SubmitValue++;
                                    }
                                }
                            }

                            //完成后关闭进度窗口
                            Parameters.SendMessage(SubWindow.ProgressBarWindow.BWOProgressBarParameter.SelfHandle, Parameters.WM_ProgressBarWindowClose, 0, 0);

                            //清理
                            for (int i = 0; i < SpecialListID.Count; i++)
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    for (int j = 0; j < SelfWhiteLists.Count; j++)
                                    {
                                        if (SpecialListID[i] == (SelfWhiteLists[j] as WhiteListClass).IMSI)
                                        {
                                            SelfWhiteLists.RemoveAt(j);
                                            break;
                                        }
                                    }
                                });
                            }

                            Dispatcher.Invoke(() =>
                            {
                                if (Flag)
                                {
                                    MessageBox.Show("删除白名单失败,详见日志！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                }
                                else
                                {
                                    MessageBox.Show("删除白名单成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                WhiteListQuery();
                            });
                            //清除白名单
                            WhiteListItemsDataDelete.Clear();
                        }).Start();
                    }
                    else
                    {
                        MessageBox.Show("网络与服服器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void WaitResultTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            JsonInterFace.ActionResultStatus.NoResultErrorDeviceName = true;
        }

        private void GetDataToDelete<T>(ref List<List<List<Dictionary<string, string>>>> DataList, List<List<T>> SelectedDataList, List<APATTributes> APATTributeList, string Type)
        {
            #region 黑名单
            if (new Regex(Type).Match("BlackList").Success)
            {
                
                DataList = new List<List<List<Dictionary<string, string>>>>();
                DataList.Clear();
                APATTributeList.Clear();
                for (int n = 0; n < SelectedDataList.Count; n++)
                {
                    if ((SelectedDataList[n].Count % Parameters.BWListIMSIConfigurationTotal) == 0)
                    {
                        JsonInterFace.BlackList.Count = (SelectedDataList[n].Count / Parameters.BWListIMSIConfigurationTotal);
                    }
                    else
                    {
                        JsonInterFace.BlackList.Count = (SelectedDataList[n].Count / Parameters.BWListIMSIConfigurationTotal) + 1;
                    }
                    List<List<Dictionary<string, string>>> tmpDataList = new List<List<Dictionary<string, string>>>();
                    for (int j = 0, i = 0; j < JsonInterFace.BlackList.Count; j++)
                    {
                        List<Dictionary<string, string>> BItem = new List<Dictionary<string, string>>();
                        for (int k = 0; i < SelectedDataList[n].Count; i++, k++)
                        {
                            Dictionary<string, string> Item = new Dictionary<string, string>();
                            if ((SelectedDataList[n][i] as BlackListClass).IMSI != "")
                            {
                                Item.Add("imsi", (SelectedDataList[n][i] as BlackListClass).IMSI);
                            }

                            if ((SelectedDataList[n][i] as BlackListClass).IMEI != "")
                            {
                                Item.Add("imei", (SelectedDataList[n][i] as BlackListClass).IMEI);
                            }

                            if (k == 0)
                            {
                                BItem.Add(Item);
                                continue;
                            }

                            if (k % (Parameters.BWListIMSIConfigurationTotal) != 0)
                            {
                                BItem.Add(Item);
                            }
                            else if (k % (Parameters.BWListIMSIConfigurationTotal) == 0)
                            {
                                break;
                            }
                        }
                        tmpDataList.Add(BItem);
                    }
                    DataList.Add(tmpDataList);

                    //当前选定的(站点总数或设备数量)
                    for (int j = 0; j < JsonInterFace.BlackList.ParameterList.Count; j++)
                    {
                        //站点
                        if (JsonInterFace.BlackList.ParameterList[j].NodeType == "domain")
                        {
                            JsonInterFace.BlackList.StationCount++;
                            for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                            {
                                for (int k = 0; k < SelectedDataList[n].Count; k++)
                                {
                                    string TmepDomainFullPathName = string.Empty;
                                    string[] TmepDomainFullPathNameList = (SelectedDataList[n][k] as BlackListClass).Station.Split(new char[] { '.' });
                                    TmepDomainFullPathName += TmepDomainFullPathNameList[1];
                                    TmepDomainFullPathName = JsonInterFace.BlackList.ParameterList[j].DomainFullPathName + "." + TmepDomainFullPathNameList[1];
                                    if (JsonInterFace.APATTributesLists[i].FullName == TmepDomainFullPathName)
                                    {
                                        if(!APATTributeList.Contains(JsonInterFace.APATTributesLists[i]))
                                            APATTributeList.Add(JsonInterFace.APATTributesLists[i]);
                                        break;
                                    }
                                }
                            }
                        }
                        //设备
                        else
                        {
                            JsonInterFace.BlackList.DeviceCount++;
                            for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                            {
                                if (JsonInterFace.APATTributesLists[i].FullName == JsonInterFace.BlackList.ParameterList[j].DeviceFullPathName)
                                {
                                    if (!APATTributeList.Contains(JsonInterFace.APATTributesLists[i]))
                                        APATTributeList.Add(JsonInterFace.APATTributesLists[i]);
                                }
                            }
                        }
                    }
                }

                return;
            }
            #endregion
            #region 白名单
            if (new Regex(Type).Match("WhiteList").Success)
            {
                DataList = new List<List<List<Dictionary<string, string>>>>();
                DataList.Clear();
                APATTributeList.Clear();
                for (int n = 0; n < SelectedDataList.Count; n++)
                {
                    if ((SelectedDataList[n].Count % Parameters.BWListIMSIConfigurationTotal) == 0)
                    {
                        JsonInterFace.WhiteList.Count = (SelectedDataList[n].Count / Parameters.BWListIMSIConfigurationTotal);
                    }
                    else
                    {
                        JsonInterFace.WhiteList.Count = (SelectedDataList[n].Count / Parameters.BWListIMSIConfigurationTotal) + 1;
                    }
                    List<List<Dictionary<string, string>>> tmpDataList = new List<List<Dictionary<string, string>>>();
                    for (int j = 0, i = 0; j < JsonInterFace.WhiteList.Count; j++)
                    {
                        List<Dictionary<string, string>> WItem = new List<Dictionary<string, string>>();
                        for (int k = 0; i < SelectedDataList[n].Count; i++, k++)
                        {
                            Dictionary<string, string> Item = new Dictionary<string, string>();
                            if ((SelectedDataList[n][i] as WhiteListClass).IMSI != "")
                            {
                                Item.Add("imsi", (SelectedDataList[n][i] as WhiteListClass).IMSI);
                            }

                            if ((SelectedDataList[n][i] as WhiteListClass).IMEI != "")
                            {
                                Item.Add("imei", (SelectedDataList[n][i] as WhiteListClass).IMEI);
                            }

                            if (k == 0)
                            {
                                WItem.Add(Item);
                                continue;
                            }

                            if (k % (Parameters.BWListIMSIConfigurationTotal) != 0)
                            {
                                WItem.Add(Item);
                            }
                            else if (k % (Parameters.BWListIMSIConfigurationTotal) == 0)
                            {
                                break;
                            }
                        }
                        tmpDataList.Add(WItem);
                    }
                    DataList.Add(tmpDataList);

                    //当前选定的(站点总数或设备数量)
                    for (int j = 0; j < JsonInterFace.WhiteList.ParameterList.Count; j++)
                    {
                        //站点
                        if (JsonInterFace.WhiteList.ParameterList[j].NodeType == "domain")
                        {
                            JsonInterFace.WhiteList.StationCount++;
                            for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                            {
                                for (int k = 0; k < SelectedDataList[n].Count; k++)
                                {
                                    string TmepDomainFullPathName = string.Empty;
                                    string[] TmepDomainFullPathNameList = (SelectedDataList[n][k] as BlackListClass).Station.Split(new char[] { '.' });
                                    TmepDomainFullPathName += TmepDomainFullPathNameList[1];
                                    TmepDomainFullPathName = JsonInterFace.WhiteList.ParameterList[j].DomainFullPathName + "." + TmepDomainFullPathNameList[1];
                                    if (JsonInterFace.APATTributesLists[i].FullName == TmepDomainFullPathName)
                                    {
                                        if (!APATTributeList.Contains(JsonInterFace.APATTributesLists[i]))
                                            APATTributeList.Add(JsonInterFace.APATTributesLists[i]);
                                        break;
                                    }
                                }
                            }
                        }
                        //设备
                        else
                        {
                            JsonInterFace.WhiteList.DeviceCount++;
                            for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                            {
                                if (JsonInterFace.APATTributesLists[i].FullName == JsonInterFace.WhiteList.ParameterList[j].DeviceFullPathName)
                                {
                                    if (!APATTributeList.Contains(JsonInterFace.APATTributesLists[i]))
                                        APATTributeList.Add(JsonInterFace.APATTributesLists[i]);
                                }
                            }
                        }
                    }
                }

                return;
            }
            #endregion
            #region 普通用户
            if (new Regex(Type).Match("OtherList").Success)
            {
                DataList = new List<List<List<Dictionary<string, string>>>>();
                DataList.Clear();
                APATTributeList.Clear();
                for (int n = 0; n < SelectedDataList.Count; n++)
                {
                    if ((SelectedDataList[n].Count % Parameters.BWListIMSIConfigurationTotal) == 0)
                    {
                        JsonInterFace.CustomList.Count = (SelectedDataList[n].Count / Parameters.BWListIMSIConfigurationTotal);
                    }
                    else
                    {
                        JsonInterFace.CustomList.Count = (SelectedDataList[n].Count / Parameters.BWListIMSIConfigurationTotal) + 1;
                    }
                    List<List<Dictionary<string, string>>> tmpDataList = new List<List<Dictionary<string, string>>>();
                    for (int j = 0, i = 0; j < JsonInterFace.CustomList.Count; j++)
                    {
                        List<Dictionary<string, string>> OItem = new List<Dictionary<string, string>>();
                        for (int k = 0; i < SelectedDataList[n].Count; i++, k++)
                        {
                            Dictionary<string, string> Item = new Dictionary<string, string>();
                            if ((SelectedDataList[n][i] as CustomListClass).IMSI != "")
                            {
                                Item.Add("imsi", (SelectedDataList[n][i] as CustomListClass).IMSI);
                            }

                            if ((SelectedDataList[n][i] as CustomListClass).IMEI != "")
                            {
                                Item.Add("imei", (SelectedDataList[n][i] as CustomListClass).IMEI);
                            }

                            if (k == 0)
                            {
                                OItem.Add(Item);
                                continue;
                            }

                            if (k % (Parameters.BWListIMSIConfigurationTotal) != 0)
                            {
                                OItem.Add(Item);
                            }
                            else if (k % (Parameters.BWListIMSIConfigurationTotal) == 0)
                            {
                                break;
                            }
                        }
                        tmpDataList.Add(OItem);
                    }
                    DataList.Add(tmpDataList);

                    //当前选定的(站点总数或设备数量)
                    for (int j = 0; j < JsonInterFace.CustomList.ParameterList.Count; j++)
                    {
                        //站点
                        if (JsonInterFace.CustomList.ParameterList[j].NodeType == "domain")
                        {
                            JsonInterFace.CustomList.StationCount++;
                            for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                            {
                                for (int k = 0; k < SelectedDataList[n].Count; k++)
                                {
                                    string TmepDomainFullPathName = string.Empty;
                                    string[] TmepDomainFullPathNameList = (SelectedDataList[n][k] as BlackListClass).Station.Split(new char[] { '.' });
                                    TmepDomainFullPathName += TmepDomainFullPathNameList[1];
                                    TmepDomainFullPathName = JsonInterFace.CustomList.ParameterList[j].DomainFullPathName + "." + TmepDomainFullPathNameList[1];
                                    if (JsonInterFace.APATTributesLists[i].FullName == TmepDomainFullPathName)
                                    {
                                        if (!APATTributeList.Contains(JsonInterFace.APATTributesLists[i]))
                                            APATTributeList.Add(JsonInterFace.APATTributesLists[i]);
                                        break;
                                    }
                                }
                            }
                        }
                        //设备
                        else
                        {
                            JsonInterFace.CustomList.DeviceCount++;
                            for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                            {
                                if (JsonInterFace.APATTributesLists[i].FullName == JsonInterFace.CustomList.ParameterList[j].DeviceFullPathName)
                                {
                                    if (!APATTributeList.Contains(JsonInterFace.APATTributesLists[i]))
                                        APATTributeList.Add(JsonInterFace.APATTributesLists[i]);
                                }
                            }
                        }
                    }
                }

                return;
            }
            #endregion
        }

        private void mmRefresh_Click(object sender, RoutedEventArgs e)
        {

        }

        private void mmDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FrmSpeciallistManagement_Closed(object sender, EventArgs e)
        {
            JsonInterFace.BlackList.TabControlItemName = null;
            JsonInterFace.WhiteList.TabControlItemName = null;
            JsonInterFace.CustomList.TabControlItemName = null;
            JsonInterFace.ReDirection.TabControlItemName = null;

            JsonInterFace.BlackList.BlackListTable.Rows.Clear();
            JsonInterFace.WhiteList.WhiteListTable.Rows.Clear();
            JsonInterFace.CustomList.CustomListTable.Rows.Clear();
            JsonInterFace.ReDirection.RedirectionTable.Rows.Clear();

            JsonInterFace.WhiteList.ParameterList.Clear();
            JsonInterFace.BlackList.ParameterList.Clear();
            JsonInterFace.CustomList.ParameterList.Clear();
            JsonInterFace.ReDirection.FullName = null;

            SelfBlackLists.Clear();
            SelfWhiteLists.Clear();
            SelfCustomLists.Clear();
            SelfRedirectionList.Clear();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dgRedirect.SelectedCells.Count > 0)
            {
                ReDirectionClass redirectData = (ReDirectionClass)dgRedirect.SelectedItem;
                JsonInterFace.ReDirection.Optimization = redirectData.Optimization;
                JsonInterFace.ReDirection.Priority = redirectData.Priority;
                JsonInterFace.ReDirection.RejectMethod = redirectData.RejectMethod;
                JsonInterFace.ReDirection.Frequency = redirectData.Frequency;
                JsonInterFace.ReDirection.AddtionFrequency = redirectData.AddtionFrequency;
                JsonInterFace.ReDirection.UserType = redirectData.UserType.Trim();
                if (JsonInterFace.ReDirection.UserType != null)
                {
                    if (JsonInterFace.ReDirection.UserType.Equals("0") || JsonInterFace.ReDirection.UserType.Equals("白名单"))
                    {
                        WhiteName.IsChecked = true;
                    }
                    else if (JsonInterFace.ReDirection.UserType.Equals("1") || JsonInterFace.ReDirection.UserType.Equals("黑名单"))
                    {
                        BlackName.IsChecked = true;
                    }
                    else
                    {
                        OtherName.IsChecked = true;
                    }
                }
                if (JsonInterFace.ReDirection.Priority != null)
                {
                    if (JsonInterFace.ReDirection.Priority.Equals("2"))
                    {
                        rbGeranRedirect.IsChecked = true;
                        txtFrequency.IsEnabled = true;
                    }
                    else if (JsonInterFace.ReDirection.Priority.Equals("3"))
                    {
                        rbUtranRedirect.IsChecked = true;
                        txtFrequency.IsEnabled = true;
                    }
                    else if (JsonInterFace.ReDirection.Priority.Equals("4"))
                    {
                        rbEutranRedirect.IsChecked = true;
                        txtFrequency.IsEnabled = true;
                    }
                    else
                    {
                        rbOtherRedirect.IsChecked = true;
                        txtFrequency.IsEnabled = false;
                    }
                }
            }
        }

        private void menuAddRedirectParam_Click(object sender, RoutedEventArgs e)
        {
            SubWindow.RedirectParamWindow RedirectParamWin = new SubWindow.RedirectParamWindow();
            RedirectParamWin.ShowDialog();
        }

        private void btnBlackExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog svd = new Microsoft.Win32.SaveFileDialog();
                svd.Filter = "(Excel文件 *.xls)|*.xls|(Excel文件 *.xlsx)|*.xlsx";
                svd.AddExtension = true;
                svd.CheckPathExists = true;

                if (!(bool)svd.ShowDialog())
                {
                    return;
                }

                using (IODataControl.ExcelHelper excelHelper = new ExcelHelper(@svd.FileName))//定义一个范围，在范围结束时处理对象
                {
                    DataTable exportDr = JsonInterFace.BlackList.BlackListTable.Clone();
                    exportDr.Rows.Clear();
                    for (int i = 0; i < SelfBlackLists.Count; i++)
                    {
                        DataRow dr = exportDr.NewRow();
                        dr[0] = SelfBlackLists[i].ID;
                        dr[1] = SelfBlackLists[i].IMSI;
                        dr[2] = SelfBlackLists[i].AliasName;
                        dr[3] = SelfBlackLists[i].Resourcese;
                        dr[4] = SelfBlackLists[i].Station;
                        exportDr.Rows.Add(dr);
                    }

                    int res = excelHelper.DataTableToExcelForBlackList(exportDr, "Sheet1", true);
                    if (res != -1)
                    {
                        MessageBox.Show("导出到Excel文件成功,共[" + res.ToString() + "]条数据！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出到Excel文件失败," + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnWhiteExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog svd = new Microsoft.Win32.SaveFileDialog();
                svd.Filter = "(Excel文件 *.xls)|*.xls|(Excel文件 *.xlsx)|*.xlsx";
                svd.AddExtension = true;
                svd.CheckPathExists = true;

                if (!(bool)svd.ShowDialog())
                {
                    return;
                }

                using (IODataControl.ExcelHelper excelHelper = new ExcelHelper(@svd.FileName))//定义一个范围，在范围结束时处理对象
                {
                    DataTable exportDr = JsonInterFace.WhiteList.WhiteListTable.Clone();
                    exportDr.Rows.Clear();
                    for (int i = 0; i < SelfWhiteLists.Count; i++)
                    {
                        DataRow dr = exportDr.NewRow();
                        dr[0] = SelfWhiteLists[i].ID;
                        dr[1] = SelfWhiteLists[i].IMSI;
                        dr[2] = SelfWhiteLists[i].AliasName;
                        dr[3] = SelfWhiteLists[i].Station;

                        exportDr.Rows.Add(dr);
                    }

                    int res = excelHelper.DataTableToExcelForWhiteList(exportDr, "Sheet1", true);
                    if (res != -1)
                    {
                        MessageBox.Show("导出到Excel文件成功,共[" + res.ToString() + "]条数据！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出到Excel文件失败," + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// 选择自定义名单标签
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tiCustomList_Loaded(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 自定义查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCustomFilter_Click(object sender, RoutedEventArgs e)
        {
            CustomListQuery();
        }

        /// <summary>
        /// 自定义名单查询
        /// </summary>
        public void CustomListQuery()
        {
            try
            {
                JsonInterFace.CustomList.CustomListTable.Rows.Clear();
                SelfCustomLists.Clear();
                if (JsonInterFace.CustomList.ParameterList.Count > 0)
                {
                    for (int i = 0; i < JsonInterFace.CustomList.ParameterList.Count; i++)
                    {
                        if (NetWorkClient.ControllerServer.Connected)
                        {
                            Parameters.ConfigType = "OtherList";
                            NetWorkClient.ControllerServer.Send(
                                                                JsonInterFace.APCustomListRequest(
                                                                                                    JsonInterFace.CustomList.ParameterList[i].NodeType,
                                                                                                    JsonInterFace.CustomList.ParameterList[i].DeviceFullPathName,
                                                                                                    JsonInterFace.CustomList.ParameterList[i].DomainFullPathName,
                                                                                                    "other",
                                                                                                    txtCustomIMSI.Text,
                                                                                                    "",
                                                                                                    txtCustomDeviceName.Text,
                                                                                                    "",
                                                                                                    ""
                                                                                                  )
                                                              );
                        }
                    }
                }
                else
                {
                    MessageBox.Show("请先选择左边列表中的站点或设备名称！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("普通用户查询失败", ex.Message, ex.StackTrace);
            }
        }

        private void btnCustomListBackPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string PageIndexStart = JsonInterFace.CustomList.CurPageIndex.Split(new char[] { ':' })[0];
                string PageIndexEnd = JsonInterFace.CustomList.CurPageIndex.Split(new char[] { ':' })[1];

                if (int.Parse(PageIndexStart) - 1 <= 0)
                {
                    MessageBox.Show("当前已经是第一页", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (int.Parse(PageIndexStart) - 1 >= 0)
                {
                    if (NetController.NetWorkClient.ControllerServer.Connected)
                    {
                        Parameters.ConfigType = "OtherList";
                        string PageIndex = (int.Parse(PageIndexStart) - 1).ToString() + ":" + PageIndexEnd;
                        SelfCustomLists.Clear();
                        NetWorkClient.ControllerServer.Send(JsonInterFace.APBWListDataPageRequest(PageIndex));
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

        private void btnCustomListNextPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string PageIndexStart = JsonInterFace.CustomList.CurPageIndex.Split(new char[] { ':' })[0];
                string PageIndexEnd = JsonInterFace.CustomList.CurPageIndex.Split(new char[] { ':' })[1];

                if (int.Parse(PageIndexStart) + 1 > int.Parse(PageIndexEnd))
                {
                    MessageBox.Show("当前已经是最后一页", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (int.Parse(PageIndexStart) + 1 <= int.Parse(PageIndexEnd))
                {
                    if (NetController.NetWorkClient.ControllerServer.Connected)
                    {
                        Parameters.ConfigType = "OtherList";
                        string PageIndex = (int.Parse(PageIndexStart) + 1).ToString() + ":" + PageIndexEnd;
                        SelfCustomLists.Clear();
                        NetWorkClient.ControllerServer.Send(JsonInterFace.APBWListDataPageRequest(PageIndex));
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

        private void btnCustomListAdd_Click(object sender, RoutedEventArgs e)
        {
            if (tiCustomList.IsSelected)
            {
                if (JsonInterFace.CustomList.ParameterList.Count <= 0)
                {
                    MessageBox.Show("请选择左边列表的站点或设备！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            SpecialListManage.BWListAddWindow CustomListListAddWin = new SpecialListManage.BWListAddWindow();
            Parameters.ConfigType = "OtherList";
            CustomListListAddWin.ShowDialog();
            CustomListQuery();
        }

        /// <summary>
        /// 删除普通用户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCustomListDelete_Click(object sender, RoutedEventArgs e)
        {
            string TipsContent = string.Empty;
            int DelayTime = 5;
            bool Flag = false;

            System.Timers.Timer WaitResultTimer = new System.Timers.Timer();
            WaitResultTimer.AutoReset = true;
            WaitResultTimer.Interval = Parameters.SpecialListInputDelay * 1000;
            WaitResultTimer.Elapsed += WaitResultTimer_Elapsed;

            JsonInterFace.ActionResultStatus.APCount = 0;
            Parameters.UniversalCounter = 0;
            JsonInterFace.DeviceListRequestCompleteStatus.MaxLoading = 0;
            JsonInterFace.DeviceListRequestCompleteStatus.ValueLoading = 0;
            string DomainFullPathName = string.Empty;
            string DeviceName = string.Empty;

            List<List<List<Dictionary<string, string>>>> DataList = null;
            List<string> SpecialListID = new List<string>();
            List<APATTributes> APATTributeList = new List<APATTributes>();

            try
            {
                if (CustomListItemsDataDelete.Count <= 0)
                {
                    MessageBox.Show("请选择要删除的普通用户！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (MessageBox.Show("确定删除吗？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        SpecialListID.Clear();
                        Parameters.ConfigType = "OtherList";
                        GetDataToDelete<CustomListClass>(ref DataList, CustomListItemsDataDelete, APATTributeList, Parameters.ConfigType);

                        //===========启动进度条窗口===========
                        if (SubWindow.ProgressBarWindow.BWOProgressBarParameter == null)
                        {
                            ProgressBarWin = new SubWindow.ProgressBarWindow();
                        }
                        SubWindow.ProgressBarWindow.BWOProgressBarParameter.TipsContent = "正在删除普通用户, 请稍后....";
                        SubWindow.ProgressBarWindow.BWOProgressBarParameter.SubmitTotal = JsonInterFace.CustomList.Count * JsonInterFace.CustomList.ParameterList.Count;
                        SubWindow.ProgressBarWindow.BWOProgressBarParameter.CompleteTotal = JsonInterFace.CustomList.Count * JsonInterFace.CustomList.ParameterList.Count;
                        SubWindow.ProgressBarWindow.BWOProgressBarParameter.SubmitValue = 0;
                        SubWindow.ProgressBarWindow.BWOProgressBarParameter.CompleteValue = 0;

                        Dispatcher.Invoke(() =>
                        {
                            ProgressBarWin.Show();
                        });
                        //==================================

                        //提交
                        new Thread(() =>
                        {
                            //提交到站点或设备
                            for (int j = 0; j < DataList.Count; j++)
                            {
                                if (APATTributeList[j].OnLine != "1")
                                    continue;
                                //获取参数
                                JsonInterFace.ActionResultStatus.ResoultStatus = string.Empty;
                                JsonInterFace.ActionResultStatus.Finished = true;
                                //数据组
                                for (int i = 0; i <= DataList[j].Count; i++)
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
                                            //完成进度
                                            SubWindow.ProgressBarWindow.BWOProgressBarParameter.CompleteValue++;

                                            //打印状态消息
                                            if (JsonInterFace.ActionResultStatus.ResoultStatus != null && JsonInterFace.ActionResultStatus.ResoultStatus != "")
                                            {
                                                string DelStatus = string.Empty;
                                                switch (Convert.ToInt32(JsonInterFace.ActionResultStatus.ResoultStatus))
                                                {
                                                    case 0:
                                                        DelStatus = "成功";
                                                        break;
                                                    case 1:
                                                        Flag = true;
                                                        DelStatus = "失败";
                                                        break;
                                                    default:
                                                        Flag = true;
                                                        DelStatus = "未知错误";
                                                        break;
                                                }
                                                for (int k = 0; k < DataList[j][i > DataList[j].Count - 1 ? DataList[j].Count - 1 : i - 1].Count; k++)
                                                {
                                                    string IMSI = string.Empty;
                                                    foreach (KeyValuePair<string, string> Item in DataList[j][i > DataList[j].Count - 1 ? DataList[j].Count - 1 : i - 1][k])
                                                    {
                                                        if (Item.Key == "imsi")
                                                        {
                                                            IMSI = Item.Value;
                                                            SpecialListID.Add(IMSI);
                                                            break;
                                                        }
                                                    }
                                                    JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "设备[" + APATTributeList[j].FullName + "]删除普通用[" + IMSI + "]", "删除普通用", DelStatus);
                                                }
                                            }

                                            break;
                                        }
                                        //超时
                                        else if (JsonInterFace.ActionResultStatus.NoResultErrorDeviceName)
                                        {
                                            Flag = true;
                                            WaitResultTimer.Stop();
                                            lock (JsonInterFace.ActionResultStatus.FinishedLock)
                                            {
                                                JsonInterFace.ActionResultStatus.Finished = false;
                                            }
                                            JsonInterFace.ActionResultStatus.NoResultErrorDeviceName = false;
                                            SubWindow.ProgressBarWindow.BWOProgressBarParameter.CompleteValue++;

                                            //打印状态消息
                                            if (JsonInterFace.ActionResultStatus.ResoultStatus != null || JsonInterFace.ActionResultStatus.ResoultStatus != "")
                                            {
                                                string DelStatus = "超时";
                                                for (int k = 0; k < DataList[j][i > DataList[j].Count - 1 ? DataList[j].Count - 1 : i - 1].Count; k++)
                                                {
                                                    string IMSI = string.Empty;
                                                    foreach (KeyValuePair<string, string> Item in DataList[j][i > DataList[j].Count - 1 ? DataList[j].Count - 1 : i - 1][k])
                                                    {
                                                        if (Item.Key == "imsi")
                                                        {
                                                            IMSI = Item.Value;
                                                            break;
                                                        }
                                                    }
                                                    JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "设备[" + APATTributeList[j].FullName + "]删除普通用户[" + IMSI + "]", "删除普通用户", DelStatus);
                                                }
                                            }
                                            break;
                                        }
                                        else
                                        {
                                            Thread.Sleep(DelayTime += 5);
                                        }
                                    }

                                    if (i == DataList[j].Count) { break; }

                                    WaitResultTimer.Start();
                                    if (APATTributeList[j].Mode != DeviceType.GSM || APATTributeList[j].Mode != DeviceType.GSMV2 || APATTributeList[j].Mode != DeviceType.CDMA)
                                    {
                                        //发送
                                        NetWorkClient.ControllerServer.Send(
                                        JsonInterFace.APCustomListDataDeleteRequest(
                                                "device",
                                                APATTributeList[j].FullName,
                                                APATTributeList[j].DomainFullPathName,
                                                DataList[j][i]
                                        ));
                                    }
                                    //提交进度
                                    SubWindow.ProgressBarWindow.BWOProgressBarParameter.SubmitValue++;
                                }
                            }

                            //完成后关闭进度窗口
                            Parameters.SendMessage(SubWindow.ProgressBarWindow.BWOProgressBarParameter.SelfHandle, Parameters.WM_ProgressBarWindowClose, 0, 0);

                            //清理
                            for (int i = 0; i < SpecialListID.Count; i++)
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    for (int j = 0; j < SelfCustomLists.Count; j++)
                                    {
                                        if (SpecialListID[i] == (SelfCustomLists[j] as CustomListClass).IMSI)
                                        {
                                            SelfCustomLists.RemoveAt(j);
                                            break;
                                        }
                                    }
                                });
                            }

                            Dispatcher.Invoke(() =>
                            {
                                if (Flag)
                                {
                                    MessageBox.Show("删除普通用户失败,详见日志！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                }
                                else
                                {
                                    MessageBox.Show("删除普通用户成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                CustomListQuery();

                                //清除普通用户
                                CustomListItemsDataDelete.Clear();
                            });
                        }).Start();
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

        private void btnCustomListExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog svd = new Microsoft.Win32.SaveFileDialog();
                svd.Filter = "(Excel文件 *.xls)|*.xls|(Excel文件 *.xlsx)|*.xlsx";
                svd.AddExtension = true;
                svd.CheckPathExists = true;

                if (!(bool)svd.ShowDialog())
                {
                    return;
                }

                using (IODataControl.ExcelHelper excelHelper = new ExcelHelper(@svd.FileName))//定义一个范围，在范围结束时处理对象
                {
                    DataTable exportDr = JsonInterFace.CustomList.CustomListTable.Clone();
                    exportDr.Rows.Clear();
                    for (int i = 0; i < SelfCustomLists.Count; i++)
                    {
                        DataRow dr = exportDr.NewRow();
                        dr[0] = SelfCustomLists[i].ID;
                        dr[1] = SelfCustomLists[i].IMSI;
                        dr[2] = SelfCustomLists[i].AliasName;
                        dr[3] = SelfCustomLists[i].Resourcese;
                        dr[4] = SelfCustomLists[i].Station;
                        exportDr.Rows.Add(dr);
                    }

                    int res = excelHelper.DataTableToExcelForWhiteList(exportDr, "Sheet1", true);
                    if (res != -1)
                    {
                        MessageBox.Show("导出到Excel文件成功,共[" + res.ToString() + "]条数据！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出到Excel文件失败," + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// 清空白名单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnWhiteClear_Click(object sender, RoutedEventArgs e)
        {
            List<APATTributes> fullName = null;
            int DelayTime = 5;
            bool Flag = false;

            System.Timers.Timer WaitResultTimer = new System.Timers.Timer();
            WaitResultTimer.AutoReset = true;
            WaitResultTimer.Interval = Parameters.SpecialListInputDelay * 1000;
            WaitResultTimer.Elapsed += WaitResultTimer_Elapsed;

            if (MessageBox.Show("确定清空所有白名单吗？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Parameters.ConfigType = "WhiteList";
                GetFullName(ref fullName, Parameters.ConfigType);
                //===========启动进度条窗口===========
                if (SubWindow.ProgressBarWindow.BWOProgressBarParameter == null)
                {
                    ProgressBarWin = new SubWindow.ProgressBarWindow();
                }
                SubWindow.ProgressBarWindow.BWOProgressBarParameter.TipsContent = "正在清空白名单, 请稍后....";
                SubWindow.ProgressBarWindow.BWOProgressBarParameter.SubmitTotal = fullName.Count;
                SubWindow.ProgressBarWindow.BWOProgressBarParameter.CompleteTotal = fullName.Count;
                SubWindow.ProgressBarWindow.BWOProgressBarParameter.SubmitValue = 0;
                SubWindow.ProgressBarWindow.BWOProgressBarParameter.CompleteValue = 0;

                Dispatcher.Invoke(() =>
                {
                    ProgressBarWin.Show();
                });
                //==================================

                new Thread(() =>
                {
                    try
                    {
                        switch (JsonInterFace.WhiteList.ParameterList.Count)
                        {
                            case 0:
                                MessageBox.Show("请选择列表中的设备或站点！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                        }

                        //获取参数
                        JsonInterFace.ActionResultStatus.ResoultStatus = string.Empty;
                        JsonInterFace.ActionResultStatus.Finished = true;
                        for (int j = 0; j < fullName.Count; j++)
                        {
                            if (fullName[j].OnLine != "1")
                                continue;
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
                                    //完成进度
                                    SubWindow.ProgressBarWindow.BWOProgressBarParameter.CompleteValue++;

                                    //打印状态消息
                                    if (JsonInterFace.ActionResultStatus.ResoultStatus != null && JsonInterFace.ActionResultStatus.ResoultStatus != "")
                                    {
                                        string DelStatus = string.Empty;
                                        switch (Convert.ToInt32(JsonInterFace.ActionResultStatus.ResoultStatus))
                                        {
                                            case 0:
                                                DelStatus = "成功";
                                                break;
                                            case 1:
                                                Flag = true;
                                                DelStatus = "失败";
                                                break;
                                            default:
                                                Flag = true;
                                                DelStatus = "未知错误";
                                                break;
                                        }
                                        JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "设备[" + fullName[j].FullName + "]清空白名单", "清空白名单", DelStatus);
                                    }

                                    break;
                                }
                                //超时
                                else if (JsonInterFace.ActionResultStatus.NoResultErrorDeviceName)
                                {
                                    Flag = true;
                                    WaitResultTimer.Stop();
                                    lock (JsonInterFace.ActionResultStatus.FinishedLock)
                                    {
                                        JsonInterFace.ActionResultStatus.Finished = false;
                                    }
                                    JsonInterFace.ActionResultStatus.NoResultErrorDeviceName = false;
                                    SubWindow.ProgressBarWindow.BWOProgressBarParameter.CompleteValue++;

                                    //打印状态消息
                                    if (JsonInterFace.ActionResultStatus.ResoultStatus != null || JsonInterFace.ActionResultStatus.ResoultStatus != "")
                                    {
                                        string DelStatus = "超时";
                                        JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "设备[" + fullName[j].FullName + "]清空白名单", "清空白名单", DelStatus);
                                    }
                                    break;
                                }
                                else
                                {
                                    Thread.Sleep(DelayTime += 5);
                                }
                            }

                            if (j == fullName.Count) { break; }

                            WaitResultTimer.Start();

                            if (NetWorkClient.ControllerServer.Connected)
                            {
                                #region GSMV2 and CDMA
                                if (fullName[j].Mode == DeviceType.GSMV2 || fullName[j].Mode == DeviceType.CDMA)
                                {
                                    //数据组
                                    JsonInterFace.GSMV2IMSIControlInfo.IsSucess = true;
                                    Dictionary<string, string> ParaList = new Dictionary<string, string>();
                                    Parameters.ActionType = "Delete All";
                                    ParaList.Clear();
                                    ParaList.Add("wTotalImsi", "0");
                                    ParaList.Add("bSegmentType", "0");
                                    ParaList.Add("bActionType", "1");

                                    if (NetWorkClient.ControllerServer.Connected)
                                    {
                                        JsonInterFace.ResultMessageList.Clear();
                                        NetWorkClient.ControllerServer.Send(
                                                                            JsonInterFace.GSMV2IMSIActionRequest(
                                                                                                                fullName[j].FullName,
                                                                                                                fullName[j].DeviceName,
                                                                                                                fullName[j].IpAddr,
                                                                                                                fullName[j].Port,
                                                                                                                fullName[j].InnerType,
                                                                                                                fullName[j].SN,
                                                                                                                ParaList,
                                                                                                                "0"
                                                                                                               )
                                                                           );
                                    }
                                    else
                                    {
                                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                        return;
                                    }
                                }
                                #endregion
                                else if (!fullName[j].Mode.Equals(DeviceType.GSM))
                                {
                                    //类型
                                    Parameters.ConfigType = "ClearWhiteList";
                                    string Command = "123456789abcdef";

                                    NetWorkClient.ControllerServer.Send(
                                                                            JsonInterFace.APWhiteListDataClearRequest(
                                                                                                                        "device",
                                                                                                                        fullName[j].FullName,
                                                                                                                        Command
                                                                                                                     )
                                                                       );
                                }
                            }
                            else
                            {
                                MessageBox.Show("网络与服服器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }
                        }
                        //完成后关闭进度窗口
                        Parameters.SendMessage(SubWindow.ProgressBarWindow.BWOProgressBarParameter.SelfHandle, Parameters.WM_ProgressBarWindowClose, 0, 0);

                        Dispatcher.Invoke(() =>
                        {
                            if (Flag)
                            {
                                MessageBox.Show("清空白名单失败,详见日志！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            else
                            {
                                MessageBox.Show("清空白名单成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            WhiteListQuery();
                        });
                    }
                    catch (Exception ex)
                    {
                        Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
                    }

                    System.GC.Collect();

                }).Start();
            }
        }

        /// <summary>
        /// 清空黑名单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBlackClear_Click(object sender, RoutedEventArgs e)
        {
            List<APATTributes> fullName = null;
            int DelayTime = 5;
            bool Flag = false;

            System.Timers.Timer WaitResultTimer = new System.Timers.Timer();
            WaitResultTimer.AutoReset = true;
            WaitResultTimer.Interval = Parameters.SpecialListInputDelay * 1000;
            WaitResultTimer.Elapsed += WaitResultTimer_Elapsed;

            if (MessageBox.Show("确定清空所有黑名单吗？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Parameters.ConfigType = "BlackList";
                GetFullName(ref fullName, Parameters.ConfigType);
                //===========启动进度条窗口===========
                if (SubWindow.ProgressBarWindow.BWOProgressBarParameter == null)
                {
                    ProgressBarWin = new SubWindow.ProgressBarWindow();
                }
                SubWindow.ProgressBarWindow.BWOProgressBarParameter.TipsContent = "正在清空黑名单, 请稍后....";
                SubWindow.ProgressBarWindow.BWOProgressBarParameter.SubmitTotal = fullName.Count;
                SubWindow.ProgressBarWindow.BWOProgressBarParameter.CompleteTotal = fullName.Count;
                SubWindow.ProgressBarWindow.BWOProgressBarParameter.SubmitValue = 0;
                SubWindow.ProgressBarWindow.BWOProgressBarParameter.CompleteValue = 0;

                Dispatcher.Invoke(() =>
                {
                    ProgressBarWin.Show();
                });
                //==================================

                new Thread(() =>
                {
                    try
                    {
                        switch (JsonInterFace.BlackList.ParameterList.Count)
                        {
                            case 0:
                                MessageBox.Show("请选择列表中的设备或站点！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                        }

                        //获取参数
                        JsonInterFace.ActionResultStatus.ResoultStatus = string.Empty;
                        JsonInterFace.ActionResultStatus.Finished = true;
                        for (int j = 0; j < fullName.Count; j++)
                        {
                            if (fullName[j].OnLine != "1")
                                continue;
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
                                    //完成进度
                                    SubWindow.ProgressBarWindow.BWOProgressBarParameter.CompleteValue++;

                                    //打印状态消息
                                    if (JsonInterFace.ActionResultStatus.ResoultStatus != null && JsonInterFace.ActionResultStatus.ResoultStatus != "")
                                    {
                                        string DelStatus = string.Empty;
                                        switch (Convert.ToInt32(JsonInterFace.ActionResultStatus.ResoultStatus))
                                        {
                                            case 0:
                                                DelStatus = "成功";
                                                break;
                                            case 1:
                                                Flag = true;
                                                DelStatus = "失败";
                                                break;
                                            default:
                                                Flag = true;
                                                DelStatus = "未知错误";
                                                break;
                                        }
                                        JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "设备[" + fullName[j].FullName + "]清空黑名单", "清空黑名单", DelStatus);
                                    }

                                    break;
                                }
                                //超时
                                else if (JsonInterFace.ActionResultStatus.NoResultErrorDeviceName)
                                {
                                    Flag = true;
                                    WaitResultTimer.Stop();
                                    lock (JsonInterFace.ActionResultStatus.FinishedLock)
                                    {
                                        JsonInterFace.ActionResultStatus.Finished = false;
                                    }
                                    JsonInterFace.ActionResultStatus.NoResultErrorDeviceName = false;
                                    SubWindow.ProgressBarWindow.BWOProgressBarParameter.CompleteValue++;

                                    //打印状态消息
                                    if (JsonInterFace.ActionResultStatus.ResoultStatus != null || JsonInterFace.ActionResultStatus.ResoultStatus != "")
                                    {
                                        string DelStatus = "超时";
                                        JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "设备[" + fullName[j].FullName + "]清空黑名单", "清空黑名单", DelStatus);                                       
                                    }
                                    break;
                                }
                                else
                                {
                                    Thread.Sleep(DelayTime += 5);
                                }
                            }

                            if (j == fullName.Count) { break; }

                            WaitResultTimer.Start();

                            if (NetWorkClient.ControllerServer.Connected)
                            {
                                #region GSMV2 and CDMA
                                if (fullName[j].Mode == DeviceType.GSMV2 || fullName[j].Mode == DeviceType.CDMA)
                                {
                                    //数据组
                                    JsonInterFace.GSMV2IMSIControlInfo.IsSucess = true;
                                    Dictionary<string, string> ParaList = new Dictionary<string, string>();
                                    Parameters.ActionType = "Delete All";
                                    ParaList.Clear();
                                    ParaList.Add("wTotalImsi", "0");
                                    ParaList.Add("bSegmentType", "0");
                                    ParaList.Add("bActionType", "1");

                                    if (NetWorkClient.ControllerServer.Connected)
                                    {
                                        JsonInterFace.ResultMessageList.Clear();
                                        NetWorkClient.ControllerServer.Send(
                                                                            JsonInterFace.GSMV2IMSIActionRequest(
                                                                                                                fullName[j].FullName,
                                                                                                                fullName[j].DeviceName,
                                                                                                                fullName[j].IpAddr,
                                                                                                                fullName[j].Port,
                                                                                                                fullName[j].InnerType,
                                                                                                                fullName[j].SN,
                                                                                                                ParaList,
                                                                                                                "0"
                                                                                                               )
                                                                           );
                                    }
                                    else
                                    {
                                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                        return;
                                    }
                                }
                                #endregion
                                else if (!fullName[j].Mode.Equals(DeviceType.GSM))
                                {
                                    //类型
                                    Parameters.ConfigType = "ClearBlackList";
                                    string Command = "123456789abcdef";

                                    NetWorkClient.ControllerServer.Send(
                                                                            JsonInterFace.APBlackListDataClearRequest(
                                                                                                                        "device",
                                                                                                                        fullName[j].FullName,
                                                                                                                        Command
                                                                                                                     )
                                                                       );
                                }
                            }
                            else
                            {
                                MessageBox.Show("网络与服服器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }
                        }
                        //完成后关闭进度窗口
                        Parameters.SendMessage(SubWindow.ProgressBarWindow.BWOProgressBarParameter.SelfHandle, Parameters.WM_ProgressBarWindowClose, 0, 0);

                        Dispatcher.Invoke(() =>
                        {
                            if (Flag)
                            {
                                MessageBox.Show("清空黑名单失败,详见日志！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            else
                            {
                                MessageBox.Show("清空黑名单成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            BlackListQuery();
                        });
                    }
                    catch (Exception ex)
                    {
                        Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
                    }

                    System.GC.Collect();

                }).Start();
            }
        }

        /// <summary>
        /// 清空普通用户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCustomListClear_Click(object sender, RoutedEventArgs e)
        {
            List<APATTributes> fullName = null;
            int DelayTime = 5;
            bool Flag = false;

            System.Timers.Timer WaitResultTimer = new System.Timers.Timer();
            WaitResultTimer.AutoReset = true;
            WaitResultTimer.Interval = Parameters.SpecialListInputDelay * 1000;
            WaitResultTimer.Elapsed += WaitResultTimer_Elapsed;

            if (MessageBox.Show("确定清空所有普通用户吗？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Parameters.ConfigType = "CustomList";
                GetFullName(ref fullName, Parameters.ConfigType);
                //===========启动进度条窗口===========
                if (SubWindow.ProgressBarWindow.BWOProgressBarParameter == null)
                {
                    ProgressBarWin = new SubWindow.ProgressBarWindow();
                }
                SubWindow.ProgressBarWindow.BWOProgressBarParameter.TipsContent = "正在清空普通用户, 请稍后....";
                SubWindow.ProgressBarWindow.BWOProgressBarParameter.SubmitTotal = fullName.Count;
                SubWindow.ProgressBarWindow.BWOProgressBarParameter.CompleteTotal = fullName.Count;
                SubWindow.ProgressBarWindow.BWOProgressBarParameter.SubmitValue = 0;
                SubWindow.ProgressBarWindow.BWOProgressBarParameter.CompleteValue = 0;

                Dispatcher.Invoke(() =>
                {
                    ProgressBarWin.Show();
                });
                //==================================

                new Thread(() =>
                {
                    try
                    {
                        switch (JsonInterFace.CustomList.ParameterList.Count)
                        {
                            case 0:
                                MessageBox.Show("请选择列表中的设备或站点！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                        }

                        //获取参数
                        JsonInterFace.ActionResultStatus.ResoultStatus = string.Empty;
                        JsonInterFace.ActionResultStatus.Finished = true;
                        for (int j = 0; j < fullName.Count; j++)
                        {
                            if (fullName[j].OnLine != "1")
                                continue;
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
                                    //完成进度
                                    SubWindow.ProgressBarWindow.BWOProgressBarParameter.CompleteValue++;

                                    //打印状态消息
                                    if (JsonInterFace.ActionResultStatus.ResoultStatus != null && JsonInterFace.ActionResultStatus.ResoultStatus != "")
                                    {
                                        string DelStatus = string.Empty;
                                        switch (Convert.ToInt32(JsonInterFace.ActionResultStatus.ResoultStatus))
                                        {
                                            case 0:
                                                DelStatus = "成功";
                                                break;
                                            case 1:
                                                Flag = true;
                                                DelStatus = "失败";
                                                break;
                                            default:
                                                Flag = true;
                                                DelStatus = "未知错误";
                                                break;
                                        }
                                        JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "设备[" + fullName[j].FullName + "]清空普通用户", "清空普通用户", DelStatus);
                                    }

                                    break;
                                }
                                //超时
                                else if (JsonInterFace.ActionResultStatus.NoResultErrorDeviceName)
                                {
                                    Flag = true;
                                    WaitResultTimer.Stop();
                                    lock (JsonInterFace.ActionResultStatus.FinishedLock)
                                    {
                                        JsonInterFace.ActionResultStatus.Finished = false;
                                    }
                                    JsonInterFace.ActionResultStatus.NoResultErrorDeviceName = false;
                                    SubWindow.ProgressBarWindow.BWOProgressBarParameter.CompleteValue++;

                                    //打印状态消息
                                    if (JsonInterFace.ActionResultStatus.ResoultStatus != null || JsonInterFace.ActionResultStatus.ResoultStatus != "")
                                    {
                                        string DelStatus = "超时";
                                        JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "设备[" + fullName[j].FullName + "]清空普通用户", "清空普通用户", DelStatus);
                                    }
                                    break;
                                }
                                else
                                {
                                    Thread.Sleep(DelayTime += 5);
                                }
                            }

                            if (j == fullName.Count) { break; }

                            WaitResultTimer.Start();

                            if (NetWorkClient.ControllerServer.Connected)
                            {
                                #region GSMV2 and CDMA
                                if (fullName[j].Mode == DeviceType.GSMV2 || fullName[j].Mode == DeviceType.CDMA)
                                {
                                    //数据组
                                    JsonInterFace.GSMV2IMSIControlInfo.IsSucess = true;
                                    Dictionary<string, string> ParaList = new Dictionary<string, string>();
                                    Parameters.ActionType = "Delete All";
                                    ParaList.Clear();
                                    ParaList.Add("wTotalImsi", "0");
                                    ParaList.Add("bSegmentType", "0");
                                    ParaList.Add("bActionType", "1");

                                    if (NetWorkClient.ControllerServer.Connected)
                                    {
                                        JsonInterFace.ResultMessageList.Clear();
                                        NetWorkClient.ControllerServer.Send(
                                                                            JsonInterFace.GSMV2IMSIActionRequest(
                                                                                                                fullName[j].FullName,
                                                                                                                fullName[j].DeviceName,
                                                                                                                fullName[j].IpAddr,
                                                                                                                fullName[j].Port,
                                                                                                                fullName[j].InnerType,
                                                                                                                fullName[j].SN,
                                                                                                                ParaList,
                                                                                                                "0"
                                                                                                               )
                                                                           );
                                    }
                                    else
                                    {
                                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                        return;
                                    }
                                }
                                #endregion
                                else if (!fullName[j].Mode.Equals(DeviceType.GSM))
                                {
                                    //类型
                                    Parameters.ConfigType = "ClearCustomList";
                                    string Command = "123456789abcdef";

                                    NetWorkClient.ControllerServer.Send(
                                                                            JsonInterFace.APCustomListDataClearRequest(
                                                                                                                        "device",
                                                                                                                        fullName[j].FullName,
                                                                                                                        Command
                                                                                                                     )
                                                                       );
                                }
                            }
                            else
                            {
                                MessageBox.Show("网络与服服器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }
                        }
                        //完成后关闭进度窗口
                        Parameters.SendMessage(SubWindow.ProgressBarWindow.BWOProgressBarParameter.SelfHandle, Parameters.WM_ProgressBarWindowClose, 0, 0);

                        Dispatcher.Invoke(() =>
                        {
                            if (Flag)
                            {
                                MessageBox.Show("清除普通用户失败,详见日志！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            else
                            {
                                MessageBox.Show("清除普通用户成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            WhiteListQuery();
                        });
                    }
                    catch (Exception ex)
                    {
                        Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
                    }

                    System.GC.Collect();

                }).Start();
            }
        }

        private void GetFullName( ref List<APATTributes> fullName, string Type)
        {
            fullName = new List<APATTributes>();
            fullName.Clear();
            #region 黑名单
            if (new Regex(Type).Match("BlackList").Success)
            {
                //当前选定的(站点总数或设备数量)
                for (int j = 0; j < JsonInterFace.BlackList.ParameterList.Count; j++)
                {
                    //站点
                    if (JsonInterFace.BlackList.ParameterList[j].NodeType == "domain")
                    {
                        for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                        {
                            string TmepDomainFullPathName = JsonInterFace.BlackList.ParameterList[j].DomainFullPathName + "." + JsonInterFace.APATTributesLists[i].SelfName;

                            if (JsonInterFace.APATTributesLists[i].FullName == TmepDomainFullPathName)
                            {
                                if (!fullName.Contains(JsonInterFace.APATTributesLists[i]))
                                    fullName.Add(JsonInterFace.APATTributesLists[i]);
                            }
                        }
                    }
                    //设备
                    else
                    {
                        for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                        {
                            if (JsonInterFace.APATTributesLists[i].FullName == JsonInterFace.BlackList.ParameterList[j].DeviceFullPathName)
                            {
                                if (!fullName.Contains(JsonInterFace.APATTributesLists[i]))
                                    fullName.Add(JsonInterFace.APATTributesLists[i]);
                            }
                        }
                    }
                }

                return;
            }
            #endregion
            #region 白名单
            if (new Regex(Type).Match("WhiteList").Success)
            { //当前选定的(站点总数或设备数量)
                for (int j = 0; j < JsonInterFace.WhiteList.ParameterList.Count; j++)
                {
                    //站点
                    if (JsonInterFace.WhiteList.ParameterList[j].NodeType == "domain")
                    {
                        for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                        {
                            string TmepDomainFullPathName = JsonInterFace.WhiteList.ParameterList[j].DomainFullPathName + "." + JsonInterFace.APATTributesLists[i].SelfName;

                            if (JsonInterFace.APATTributesLists[i].FullName == TmepDomainFullPathName)
                            {
                                if (!fullName.Contains(JsonInterFace.APATTributesLists[i]))
                                    fullName.Add(JsonInterFace.APATTributesLists[i]);
                            }
                        }
                    }
                    //设备
                    else
                    {
                        for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                        {
                            if (JsonInterFace.APATTributesLists[i].FullName == JsonInterFace.WhiteList.ParameterList[j].DeviceFullPathName)
                            {
                                if (!fullName.Contains(JsonInterFace.APATTributesLists[i]))
                                    fullName.Add(JsonInterFace.APATTributesLists[i]);
                            }
                        }
                    }
                }
                return;
            }
            #endregion
            #region 普通用户
            if (new Regex(Type).Match("OtherList").Success)
            {
                //当前选定的(站点总数或设备数量)
                for (int j = 0; j < JsonInterFace.CustomList.ParameterList.Count; j++)
                {
                    //站点
                    if (JsonInterFace.CustomList.ParameterList[j].NodeType == "domain")
                    {
                        for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                        {
                            string TmepDomainFullPathName = JsonInterFace.CustomList.ParameterList[j].DomainFullPathName + "." + JsonInterFace.APATTributesLists[i].SelfName;

                            if (JsonInterFace.APATTributesLists[i].FullName == TmepDomainFullPathName)
                            {
                                if (!fullName.Contains(JsonInterFace.APATTributesLists[i]))
                                    fullName.Add(JsonInterFace.APATTributesLists[i]);
                            }
                        }
                    }
                    //设备
                    else
                    {
                        for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                        {
                            if (JsonInterFace.APATTributesLists[i].FullName == JsonInterFace.CustomList.ParameterList[j].DeviceFullPathName)
                            {
                                if (!fullName.Contains(JsonInterFace.APATTributesLists[i]))
                                    fullName.Add(JsonInterFace.APATTributesLists[i]);
                            }
                        }
                    }
                }
                return;
            }
            #endregion
        }
        /// <summary>
        /// 重定向查询
        /// </summary>
        public void RedirectListQuery()
        {
            try
            {
                JsonInterFace.ReDirection.RedirectionTable.Clear();
                SelfRedirectionList.Clear();
                if (JsonInterFace.ReDirection.FullName != null && JsonInterFace.ReDirection.FullName != "")
                {
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        NetWorkClient.ControllerServer.Send(JsonInterFace.Get_redirection_Request(JsonInterFace.ReDirection.FullName, JsonInterFace.ReDirection.Name, JsonInterFace.ReDirection.UserType));
                    }
                    else
                    {
                        Parameters.PrintfLogsExtended("向服务器请求重定向:", "Connected: Failed!");
                    }
                }
                else
                {
                    MessageBox.Show("请先选择左边列表中的站点或设备名称！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("重定向查询失败", ex.Message, ex.StackTrace);
            }
        }

        private void btnRedirectSelect_Click(object sender, RoutedEventArgs e)
        {
            //用户类型
            if ((bool)WhiteName.IsChecked)
            {
                JsonInterFace.ReDirection.UserType = "0";
            }
            else if ((bool)BlackName.IsChecked)
            {
                JsonInterFace.ReDirection.UserType = "1";
            }
            else
            {
                JsonInterFace.ReDirection.UserType = "2";
            }
            Parameters.ConfigType = "Redirect";
            RedirectListQuery();
        }

        private void btnRedirectOK_Click(object sender, RoutedEventArgs e)
        {
            string GeranRedirect = "0";
            string arfcn = String.Empty;
            string UtranRedirect = "0";
            string uarfcn = String.Empty;
            string EutranRedirect = "0";
            string earfcn = String.Empty;
            string category = string.Empty;
            string priority = string.Empty;
            string RejectMethod = string.Empty;
            string additionalFreq = txtAdditionalFreq.Text;
            //用户类型
            if ((bool)WhiteName.IsChecked)
            {
                category = "0";
                Parameters.RedirectionParam.Category = WhiteName.Content.ToString();
            }
            else if ((bool)BlackName.IsChecked)
            {
                category = "1";
                Parameters.RedirectionParam.Category = BlackName.Content.ToString();
            }
            else
            {
                category = "2";
                Parameters.RedirectionParam.Category = OtherName.Content.ToString();
            }
            //优选
            if ((bool)rbGeranRedirect.IsChecked)
            {
                priority = "2";
                GeranRedirect = "1";
                arfcn = txtFrequency.Text.Trim();
                Parameters.RedirectionParam.Optimization = "2G";
            }
            else if ((bool)rbUtranRedirect.IsChecked)
            {
                priority = "3";
                UtranRedirect = "1";
                uarfcn = txtFrequency.Text.Trim();
                Parameters.RedirectionParam.Optimization = "3G";
            }
            else if ((bool)rbEutranRedirect.IsChecked)
            {
                priority = "4";
                EutranRedirect = "1";
                earfcn = txtFrequency.Text.Trim();
                Parameters.RedirectionParam.Optimization = "4G";
            }
            else
            {
                priority = "5";
                Parameters.RedirectionParam.Optimization = "无";
            }
            //制式
            if (cmbRejectMethod.SelectedIndex == 0)
            {
                RejectMethod = "1";
            }
            else if (cmbRejectMethod.SelectedIndex == 1)
            {
                RejectMethod = "2";
            }
            else if (cmbRejectMethod.SelectedIndex == 2)
            {
                RejectMethod = "255";
            }
            else
            {
                RejectMethod = System.Convert.ToInt32(cmbRejectMethod.Text, 10).ToString();
            }

            Parameters.RedirectionParam.Priority = priority;
            Parameters.RedirectionParam.RejectMethod = cmbRejectMethod.Text;
            Parameters.RedirectionParam.Freq = txtFrequency.Text;
            Parameters.RedirectionParam.AdditionalFreq = txtAdditionalFreq.Text;


            if (NetWorkClient.ControllerServer.Connected)
            {
                NetWorkClient.ControllerServer.Send(JsonInterFace.Set_redirection_Request(JsonInterFace.ReDirection.FullName, JsonInterFace.ReDirection.Name, category, priority,
                                                                                          GeranRedirect, arfcn, UtranRedirect, uarfcn, EutranRedirect, earfcn,
                                                                                          RejectMethod, additionalFreq));
            }
            else
            {
                Parameters.PrintfLogsExtended("向服务器请求设置重定向:", "Connected: Failed!");
            }
        }

        private void btnBFirstPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string PageIndexEnd = JsonInterFace.BlackList.CurPageIndex.Split(new char[] { ':' })[1];

                if (NetController.NetWorkClient.ControllerServer.Connected)
                {
                    Parameters.ConfigType = "BlackList";
                    string PageIndex = "1:" + PageIndexEnd;
                    SelfBlackLists.Clear();
                    NetWorkClient.ControllerServer.Send(JsonInterFace.APBWListDataPageRequest(PageIndex));
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
                string PageIndexEnd = JsonInterFace.BlackList.CurPageIndex.Split(new char[] { ':' })[1];

                if (NetController.NetWorkClient.ControllerServer.Connected)
                {
                    Parameters.ConfigType = "BlackList";
                    string PageIndex = PageIndexEnd + ":" + PageIndexEnd;
                    SelfBlackLists.Clear();
                    NetWorkClient.ControllerServer.Send(JsonInterFace.APBWListDataPageRequest(PageIndex));
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

        private void btnWFirstPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string PageIndexEnd = JsonInterFace.WhiteList.CurPageIndex.Split(new char[] { ':' })[1];
                if (NetController.NetWorkClient.ControllerServer.Connected)
                {
                    Parameters.ConfigType = "WhiteList";
                    string PageIndex = "1:" + PageIndexEnd;
                    SelfWhiteLists.Clear();
                    NetWorkClient.ControllerServer.Send(JsonInterFace.APBWListDataPageRequest(PageIndex));
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

        private void btnWLastPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string PageIndexEnd = JsonInterFace.WhiteList.CurPageIndex.Split(new char[] { ':' })[1];
                if (NetController.NetWorkClient.ControllerServer.Connected)
                {
                    Parameters.ConfigType = "WhiteList";
                    string PageIndex = PageIndexEnd + ":" + PageIndexEnd;
                    SelfWhiteLists.Clear();
                    NetWorkClient.ControllerServer.Send(JsonInterFace.APBWListDataPageRequest(PageIndex));
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

        private void btnCustomListFirstPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string PageIndexEnd = JsonInterFace.CustomList.CurPageIndex.Split(new char[] { ':' })[1];
                if (NetController.NetWorkClient.ControllerServer.Connected)
                {
                    Parameters.ConfigType = "OtherList";
                    string PageIndex = "1:" + PageIndexEnd;
                    SelfCustomLists.Clear();
                    NetWorkClient.ControllerServer.Send(JsonInterFace.APBWListDataPageRequest(PageIndex));
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

        private void btnCustomListLastPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string PageIndexEnd = JsonInterFace.CustomList.CurPageIndex.Split(new char[] { ':' })[1];
                if (NetController.NetWorkClient.ControllerServer.Connected)
                {
                    Parameters.ConfigType = "OtherList";
                    string PageIndex = PageIndexEnd + ":" + PageIndexEnd;
                    SelfCustomLists.Clear();
                    NetWorkClient.ControllerServer.Send(JsonInterFace.APBWListDataPageRequest(PageIndex));
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

        private void txtBlackListPageFirstIndex_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    if (!Parameters.ISDigital(txtBlackListPageFirstIndex.Text))
                    {
                        MessageBox.Show("输入非法页！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        txtBlackListPageFirstIndex.Text = JsonInterFace.BlackList.CurPageFirstIndexCaption;
                        return;
                    }

                    string PageIndexEnd = JsonInterFace.BlackList.CurPageIndex.Split(new char[] { ':' })[1];

                    if (Convert.ToInt32(txtBlackListPageFirstIndex.Text) < 1
                        || Convert.ToInt32(txtBlackListPageFirstIndex.Text) > Convert.ToInt32(PageIndexEnd))
                    {
                        MessageBox.Show("输入非法页！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        txtBlackListPageFirstIndex.Text = JsonInterFace.BlackList.CurPageFirstIndexCaption;
                        return;
                    }

                    if (NetController.NetWorkClient.ControllerServer.Connected)
                    {
                        Parameters.ConfigType = "BlackList";
                        string PageIndex = txtBlackListPageFirstIndex.Text + ":" + PageIndexEnd;
                        SelfBlackLists.Clear();
                        NetWorkClient.ControllerServer.Send(JsonInterFace.APBWListDataPageRequest(PageIndex));
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
        }

        private void txtWhiteListPageFirstIndex_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    if (!Parameters.ISDigital(txtWhiteListPageFirstIndex.Text))
                    {
                        MessageBox.Show("输入非法页！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        txtWhiteListPageFirstIndex.Text = JsonInterFace.WhiteList.CurPageFirstIndexCaption;
                        return;
                    }
                    string PageIndexEnd = JsonInterFace.WhiteList.CurPageIndex.Split(new char[] { ':' })[1];
                    if (Convert.ToInt32(txtWhiteListPageFirstIndex.Text) < 1
                        || Convert.ToInt32(txtWhiteListPageFirstIndex.Text) > Convert.ToInt32(PageIndexEnd))
                    {
                        MessageBox.Show("输入非法页！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        txtWhiteListPageFirstIndex.Text = JsonInterFace.WhiteList.CurPageFirstIndexCaption;
                        return;
                    }
                    if (NetController.NetWorkClient.ControllerServer.Connected)
                    {
                        Parameters.ConfigType = "WhiteList";
                        string PageIndex = txtWhiteListPageFirstIndex.Text + ":" + PageIndexEnd;
                        SelfWhiteLists.Clear();
                        NetWorkClient.ControllerServer.Send(JsonInterFace.APBWListDataPageRequest(PageIndex));
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
        }

        private void txtCustomListPageFirstIndex_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    if (!Parameters.ISDigital(txtCustomListPageFirstIndex.Text))
                    {
                        MessageBox.Show("输入非法页！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        txtCustomListPageFirstIndex.Text = JsonInterFace.CustomList.CurPageFirstIndexCaption;
                        return;
                    }
                    string PageIndexEnd = JsonInterFace.CustomList.CurPageIndex.Split(new char[] { ':' })[1];
                    if (Convert.ToInt32(txtCustomListPageFirstIndex.Text) < 1
                        || Convert.ToInt32(txtCustomListPageFirstIndex.Text) > Convert.ToInt32(PageIndexEnd))
                    {
                        MessageBox.Show("输入非法页！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        txtCustomListPageFirstIndex.Text = JsonInterFace.CustomList.CurPageFirstIndexCaption;
                        return;
                    }
                    if (NetController.NetWorkClient.ControllerServer.Connected)
                    {
                        Parameters.ConfigType = "OtherList";
                        string PageIndex = txtCustomListPageFirstIndex.Text + ":" + PageIndexEnd;
                        SelfCustomLists.Clear();
                        NetWorkClient.ControllerServer.Send(JsonInterFace.APBWListDataPageRequest(PageIndex));
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
        }

        private void rbOtherRedirect_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)rbOtherRedirect.IsChecked)
            {
                txtFrequency.Text = "";
                txtFrequency.IsEnabled = false;
            }
            else
            {
                txtFrequency.IsEnabled = true;
            }
        }

        //选择设备
        private static object[] DeviceTreeViewItem = new object[2];
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

                string SelfID = ((CheckBoxTreeModel)(sender as CheckBox).DataContext).Id;
                string Model = ((CheckBoxTreeModel)(sender as CheckBox).DataContext).Mode;
                string FullName = ((CheckBoxTreeModel)(sender as CheckBox).DataContext).FullName;
                string NodeName = ((CheckBoxTreeModel)(sender as CheckBox).DataContext).Name;
                string IsStation = ((CheckBoxTreeModel)(sender as CheckBox).DataContext).IsStation;
                string SelfNodeType = ((CheckBoxTreeModel)(sender as CheckBox).DataContext).SelfNodeType;
                Boolean NodeChecked = ((CheckBoxTreeModel)(sender as CheckBox).DataContext).IsChecked;

                string DeviceName = ((CheckBoxTreeModel)(sender as CheckBox).DataContext).Name;
                string IpAddr = ((CheckBoxTreeModel)(sender as CheckBox).DataContext).IPAddr;
                string Port = ((CheckBoxTreeModel)(sender as CheckBox).DataContext).Port.ToString();
                string InnerType = ((CheckBoxTreeModel)(sender as CheckBox).DataContext).InnerType;
                string SN = ((CheckBoxTreeModel)(sender as CheckBox).DataContext).SN;

                //重置
                JsonInterFace.BlackList.ParameterList.Clear();
                JsonInterFace.WhiteList.ParameterList.Clear();
                JsonInterFace.CustomList.ParameterList.Clear();
                JsonInterFace.ReDirection.FullName = null;
                JsonInterFace.ReDirection.Name = null;
                JsonInterFace.ReDirection.UserType = null;
                SelfBlackLists.Clear();
                SelfWhiteLists.Clear();
                SelfCustomLists.Clear();
                SelfRedirectionList.Clear();

                //已选择
                if (NodeChecked)
                {
                    if ((SelfNodeType == NodeType.LeafNode.ToString() || IsStation == "1"))
                    {
                        for (int i = 0; i < JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows.Count; i++)
                        {
                            if (JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][1].ToString() == (SelfID)
                                && JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][2].ToString() == (NodeName))
                            {
                                BlackListClass.blackListOrderParaClass BlackListPara = new BlackListClass.blackListOrderParaClass();
                                WhiteListClass.blackListOrderParaClass WhiteListPara = new WhiteListClass.blackListOrderParaClass();
                                CustomListClass.blackListOrderParaClass CustomListPara = new CustomListClass.blackListOrderParaClass();

                                if (JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][8].ToString() == (NodeType.StructureNode.ToString()))
                                {
                                    BlackListPara.NodeType = "domain";
                                    WhiteListPara.NodeType = "domain";
                                    CustomListPara.NodeType = "domain";
                                    BlackListPara.DomainFullPathName = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][0].ToString();
                                    WhiteListPara.DomainFullPathName = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][0].ToString();
                                    CustomListPara.DomainFullPathName = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][0].ToString();

                                    BlackListPara.Model = Model;
                                    WhiteListPara.Model = Model;
                                    CustomListPara.Model = Model;

                                    BlackListPara.DeviceFullPathName = null;
                                    WhiteListPara.DeviceFullPathName = null;
                                    CustomListPara.DeviceFullPathName = null;
                                }
                                else if (JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][8].ToString() == (NodeType.LeafNode.ToString()))
                                {
                                    BlackListPara.NodeType = "device";
                                    WhiteListPara.NodeType = "device";
                                    CustomListPara.NodeType = "device";
                                    BlackListPara.DeviceFullPathName = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][0].ToString();
                                    WhiteListPara.DeviceFullPathName = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][0].ToString();
                                    CustomListPara.DeviceFullPathName = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][0].ToString();

                                    BlackListPara.Model = Model;
                                    WhiteListPara.Model = Model;
                                    CustomListPara.Model = Model;
                                    BlackListPara.DeviceName = DeviceName;
                                    WhiteListPara.DeviceName = DeviceName;
                                    CustomListPara.DeviceName = DeviceName;
                                    BlackListPara.IpAddr = IpAddr;
                                    WhiteListPara.IpAddr = IpAddr;
                                    CustomListPara.IpAddr = IpAddr;
                                    BlackListPara.Port = Port;
                                    WhiteListPara.Port = Port;
                                    CustomListPara.Port = Port;
                                    BlackListPara.InnerType = InnerType;
                                    WhiteListPara.InnerType = InnerType;
                                    CustomListPara.InnerType = InnerType;
                                    BlackListPara.SN = SN;
                                    WhiteListPara.SN = SN;
                                    CustomListPara.SN = SN;

                                    BlackListPara.DomainFullPathName = null;
                                    WhiteListPara.DomainFullPathName = null;
                                    CustomListPara.DomainFullPathName = null;
                                }

                                //黑名单参数
                                JsonInterFace.BlackList.ParameterList.Add(BlackListPara);

                                //白名单参数
                                JsonInterFace.WhiteList.ParameterList.Add(WhiteListPara);

                                //普通用户参数
                                JsonInterFace.CustomList.ParameterList.Add(CustomListPara);
                                break;
                            }
                        }

                        //重定向参数
                        string[] _DomainFullNamePath = FullName.Split(new char[] { '.' });
                        string DomainFullNamePath = string.Empty;
                        for (int k = 0; k < _DomainFullNamePath.Length - 1; k++)
                        {
                            if (DomainFullNamePath == null || DomainFullNamePath == "")
                            {
                                DomainFullNamePath = _DomainFullNamePath[k];
                            }
                            else
                            {
                                DomainFullNamePath += "." + _DomainFullNamePath[k];
                            }
                        }
                        JsonInterFace.ReDirection.FullName = DomainFullNamePath;
                        JsonInterFace.ReDirection.Name = NodeName;
                        JsonInterFace.ReDirection.UserType = "3"; //3表示获取所有
                    }
                    else
                    {
                        MessageBox.Show("请先选择左边列表中的站点或设备名称！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        ((CheckBoxTreeModel)(DeviceTreeViewItem[0] as CheckBox).DataContext).IsChecked = false;
                        (DeviceTreeViewItem[0] as CheckBox).IsChecked = false;

                        ((CheckBoxTreeModel)(sender as CheckBox).DataContext).IsChecked = false;
                        (sender as CheckBox).IsChecked = false;
                        return;
                    }
                }

                //自动查询开始
                if (JsonInterFace.WhiteList.ParameterList.Count > 0
                || JsonInterFace.BlackList.ParameterList.Count > 0
                || JsonInterFace.CustomList.ParameterList.Count > 0
                || (JsonInterFace.ReDirection.FullName != "" && JsonInterFace.ReDirection.FullName != null))
                {
                    QueryTask();
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("特殊名单管理", ex.Message, ex.StackTrace);
            }
        }

        //复位重选后重置参数
        private void ResettingValues(string SelfID, string FullName, string NodeName)
        {
            JsonInterFace.BlackList.ParameterList.Clear();
            JsonInterFace.WhiteList.ParameterList.Clear();
            JsonInterFace.CustomList.ParameterList.Clear();
            JsonInterFace.ReDirection.FullName = null;
            JsonInterFace.ReDirection.Name = null;
            JsonInterFace.ReDirection.UserType = null;

            for (int i = 0; i < JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows.Count; i++)
            {
                if (JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][1].ToString() == (SelfID)
                    && JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][2].ToString() == (NodeName))
                {
                    BlackListClass.blackListOrderParaClass BlackListPara = new BlackListClass.blackListOrderParaClass();
                    WhiteListClass.blackListOrderParaClass WhiteListPara = new WhiteListClass.blackListOrderParaClass();
                    CustomListClass.blackListOrderParaClass CustomListPara = new CustomListClass.blackListOrderParaClass();

                    if (JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][8].ToString() == (NodeType.StructureNode.ToString()))
                    {
                        BlackListPara.NodeType = "domain";
                        WhiteListPara.NodeType = "domain";
                        CustomListPara.NodeType = "domain";
                        BlackListPara.DomainFullPathName = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][0].ToString();
                        WhiteListPara.DomainFullPathName = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][0].ToString();
                        CustomListPara.DomainFullPathName = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][0].ToString();

                        BlackListPara.DeviceFullPathName = null;
                        WhiteListPara.DeviceFullPathName = null;
                        CustomListPara.DeviceFullPathName = null;
                    }
                    else if (JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][8].ToString() == (NodeType.LeafNode.ToString()))
                    {
                        BlackListPara.NodeType = "device";
                        WhiteListPara.NodeType = "device";
                        CustomListPara.NodeType = "device";
                        BlackListPara.DeviceFullPathName = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][0].ToString();
                        WhiteListPara.DeviceFullPathName = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][0].ToString();
                        CustomListPara.DeviceFullPathName = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][0].ToString();

                        BlackListPara.DomainFullPathName = null;
                        WhiteListPara.DomainFullPathName = null;
                        CustomListPara.DomainFullPathName = null;
                    }

                    //黑名单参数
                    JsonInterFace.BlackList.ParameterList.Add(BlackListPara);

                    //白名单参数
                    JsonInterFace.WhiteList.ParameterList.Add(WhiteListPara);

                    //普通用户参数
                    JsonInterFace.CustomList.ParameterList.Add(CustomListPara);
                    break;
                }
            }

            //重定向参数
            string[] _DomainFullNamePath = FullName.Split(new char[] { '.' });
            string DomainFullNamePath = string.Empty;
            for (int k = 0; k < _DomainFullNamePath.Length - 1; k++)
            {
                if (DomainFullNamePath == null || DomainFullNamePath == "")
                {
                    DomainFullNamePath = _DomainFullNamePath[k];
                }
                else
                {
                    DomainFullNamePath += "." + _DomainFullNamePath[k];
                }
            }
            JsonInterFace.ReDirection.FullName = DomainFullNamePath;
            JsonInterFace.ReDirection.Name = NodeName;
            JsonInterFace.ReDirection.UserType = "3"; //3表示获取所有
        }

        private void lblBlackListCaption_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            tabControl.SelectedIndex = 0;
            JsonInterFace.BlackList.TabControlItemName = tiBlacklist.Name;
            JsonInterFace.WhiteList.TabControlItemName = null;
            JsonInterFace.CustomList.TabControlItemName = null;
            JsonInterFace.ReDirection.TabControlItemName = null;
            QueryTask();
        }

        private void lblWhiteListCaption_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            tabControl.SelectedIndex = 1;
            JsonInterFace.WhiteList.TabControlItemName = tiWhitelist.Name;
            JsonInterFace.BlackList.TabControlItemName = null;
            JsonInterFace.CustomList.TabControlItemName = null;
            JsonInterFace.ReDirection.TabControlItemName = null;
            QueryTask();
        }

        private void lblCustomListCaption_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            tabControl.SelectedIndex = 2;
            JsonInterFace.CustomList.TabControlItemName = tiCustomList.Name;
            JsonInterFace.BlackList.TabControlItemName = null;
            JsonInterFace.WhiteList.TabControlItemName = null;
            JsonInterFace.ReDirection.TabControlItemName = null;
            QueryTask();
        }

        private void lblSettingCaption_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            tabControl.SelectedIndex = 3;
            JsonInterFace.ReDirection.TabControlItemName = tiSetting.Name;
            JsonInterFace.BlackList.TabControlItemName = null;
            JsonInterFace.WhiteList.TabControlItemName = null;
            JsonInterFace.CustomList.TabControlItemName = null;
            QueryTask();
        }

        //递归操作设备树子项是否选择状态
        private bool ChangeDeviceTreeItemStatus(IList<CheckBoxTreeModel> DeviceTree, string Name, bool Active)
        {
            try
            {
                foreach (CheckBoxTreeModel Item in DeviceTree)
                {
                    if (Item.Children.Count > 0 && Item.IsStation == "0")
                    {
                        ChangeDeviceTreeItemStatus(Item.Children, Name, Active);
                    }
                    else
                    {
                        if (Item.IsStation == "1" && Item.FullName == Name)
                        {
                            Item.IsChecked = Active;
                            return true;
                        }
                        else if (Item.SelfNodeType == NodeType.LeafNode.ToString() && Item.FullName == Name)
                        {
                            Item.IsChecked = Active;
                            return true;
                        }
                        else if (Item.Children.Count > 0)
                        {
                            ChangeDeviceTreeItemStatus(Item.Children, Name, Active);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "设备列表返回上一次正确选择项失败," + ex.Message + Environment.NewLine + ex.StackTrace, "特殊名单管理设备选择", "失败:");
                Parameters.PrintfLogsExtended("设备列表返回上一次正确选择项失败", ex.Message, ex.StackTrace);
            }
            return false;
        }
    }
}
