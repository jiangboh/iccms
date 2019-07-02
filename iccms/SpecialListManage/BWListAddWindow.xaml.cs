using DataInterface;
using IODataControl;
using NetController;
using ParameterControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace iccms.SpecialListManage
{
    /// <summary>
    /// BWListAddWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BWListAddWindow : Window
    {
        SubWindow.ProgressBarWindow ProgressBarWin = null;
        DataTable BWListInfoTable = null;

        public BWListAddWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;

            InitDataTalbe();

            if (ProgressBarWin == null)
            {
                ProgressBarWin = new SubWindow.ProgressBarWindow();
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
                //添加黑名单响应
                if (msg == Parameters.WM_BlackListAddResponse)
                {
                    DataTabClear();
                }
                //删除黑名单响应
                else if (msg == Parameters.WM_BlackListDeleteResponse)
                {

                }
                //编辑黑名单响应
                else if (msg == Parameters.WM_BlackListEditResponse)
                {

                }
                //添加白名单响应
                else if (msg == Parameters.WM_WhiteListAddResponse)
                {
                    DataTabClear();
                }
                //删除白名单响应
                else if (msg == Parameters.WM_BlackListDeleteResponse)
                {

                }
                //编辑白名单响应
                else if (msg == Parameters.WM_WhiteListEditResponse)
                {

                }
                //添加普通用户单响应
                else if (msg == Parameters.WM_CustomListAddResponse)
                {
                    DataTabClear();
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("Windows 消息事件：", ex.Message, ex.StackTrace);
            }
            return hwnd;
        }

        private void InitDataTalbe()
        {
            BWListInfoTable = new DataTable("BWListInfo");
            DataColumn Column0 = new DataColumn();
            Column0.DataType = System.Type.GetType("System.String");
            Column0.ColumnName = "IMSI";

            DataColumn Column1 = new DataColumn();
            Column1.DataType = System.Type.GetType("System.String");
            Column1.ColumnName = "IMEI";

            DataColumn Column2 = new DataColumn();
            Column2.DataType = System.Type.GetType("System.String");
            Column2.ColumnName = "UserType";

            DataColumn Column3 = new DataColumn();
            Column3.DataType = System.Type.GetType("System.String");
            Column3.ColumnName = "RbStart";

            DataColumn Column4 = new DataColumn();
            Column4.DataType = System.Type.GetType("System.String");
            Column4.ColumnName = "RbEnd";

            DataColumn Column5 = new DataColumn();
            Column5.DataType = System.Type.GetType("System.String");
            Column5.ColumnName = "AliasName";

            BWListInfoTable.Columns.Add(Column0);
            BWListInfoTable.Columns.Add(Column1);
            BWListInfoTable.Columns.Add(Column2);
            BWListInfoTable.Columns.Add(Column3);
            BWListInfoTable.Columns.Add(Column4);
            BWListInfoTable.Columns.Add(Column5);
        }

        //清空列表
        private void DataTabClear()
        {
            BWListInfoTable.Rows.Clear();
            dgBWInfoAdd.Items.Clear();
        }

        //导入名单
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (txtIMSI.Text.Trim().Equals("") && txtIMEI.Text.Trim().Equals(""))
            {
                MessageBox.Show("请输入[IMSI]相关信息！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cbbUserType.SelectedIndex < 0)
            {
                MessageBox.Show("请选择用户类型！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataRow rw = BWListInfoTable.NewRow();
            rw[0] = txtIMSI.Text;
            rw[1] = txtIMEI.Text;
            rw[2] = cbbUserType.Text;
            rw[3] = txtRbStart.Text;
            rw[4] = txtRbEnd.Text;
            rw[5] = txtAliasName.Text;
            BWListInfoTable.Rows.Add(rw);
            dgBWInfoAdd.ItemsSource = BWListInfoTable.DefaultView;
        }

        //关闭
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //提交
        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            Regex BlackListFlag = new Regex("BlackList");
            Regex WhiteListFlag = new Regex("WhiteList");
            Regex CustomListFlag = new Regex("OtherList");
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

            List<List<Dictionary<string, string>>> DataList = null;
            List<string> SpecialListID = new List<string>();
            List<APATTributes> APATTributeList = new List<APATTributes>();

            try
            {
                if (BWListInfoTable.Rows.Count <= 0)
                {
                    if (BlackListFlag.Match(Parameters.ConfigType).Success)
                    {
                        MessageBox.Show("未添加任何黑名单！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else if (WhiteListFlag.Match(Parameters.ConfigType).Success)
                    {
                        MessageBox.Show("未添加任何白名单！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else if (CustomListFlag.Match(Parameters.ConfigType).Success)
                    {
                        MessageBox.Show("未添加任何普通用户！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    return;
                }

                if (BlackListFlag.Match(Parameters.ConfigType).Success)
                {
                    TipsContent = "确定添加列表中的黑名单？";
                }
                else if (WhiteListFlag.Match(Parameters.ConfigType).Success)
                {
                    TipsContent = "确定添加列表中的白名单？";
                }
                else if (CustomListFlag.Match(Parameters.ConfigType).Success)
                {
                    TipsContent = "确定添加列表中的普通用户？";
                }

                if (MessageBox.Show(TipsContent, "提示", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        if (BWListInfoTable.Rows.Count > 0)
                        {
                            #region 黑名单
                            if (BlackListFlag.Match(Parameters.ConfigType).Success)
                            {
                                SpecialListID.Clear();
                                GetData(ref DataList, APATTributeList, Parameters.ConfigType);

                                //===========启动进度条窗口===========
                                if (SubWindow.ProgressBarWindow.BWOProgressBarParameter == null)
                                {
                                    ProgressBarWin = new SubWindow.ProgressBarWindow();
                                }

                                SubWindow.ProgressBarWindow.BWOProgressBarParameter.TipsContent = "正在提交黑名单, 请稍后....";
                                SubWindow.ProgressBarWindow.BWOProgressBarParameter.SubmitTotal = JsonInterFace.BlackList.Count * APATTributeList.Count;
                                SubWindow.ProgressBarWindow.BWOProgressBarParameter.CompleteTotal = JsonInterFace.BlackList.Count * APATTributeList.Count;
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
                                    //提交到设备
                                    for (int j = 0; j < APATTributeList.Count; j++)
                                    {
                                        if (APATTributeList[j].OnLine != "1")
                                            continue;
                                        //获取参数
                                        JsonInterFace.ActionResultStatus.ResoultStatus = string.Empty;
                                        JsonInterFace.ActionResultStatus.Finished = true;
                                        //数据组
                                        for (int i = 0; i <= DataList.Count; i++)
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
                                                        string AddStatus = string.Empty;
                                                        switch (Convert.ToInt32(JsonInterFace.ActionResultStatus.ResoultStatus))
                                                        {
                                                            case 0:
                                                                AddStatus = "成功";
                                                                break;
                                                            case 1:
                                                                Flag = true;
                                                                AddStatus = "失败";
                                                                break;
                                                            default:
                                                                Flag = true;
                                                                AddStatus = "未知错误";
                                                                break;
                                                        }
                                                        for (int k = 0; k < DataList[i > DataList.Count - 1 ? DataList.Count - 1 : i - 1].Count; k++)
                                                        {
                                                            string IMSI = string.Empty;
                                                            foreach (KeyValuePair<string, string> Item in DataList[i > DataList.Count - 1 ? DataList.Count - 1 : i - 1][k])
                                                            {
                                                                if (Item.Key == "imsi")
                                                                {
                                                                    IMSI = Item.Value;
                                                                    SpecialListID.Add(IMSI);
                                                                    break;
                                                                }
                                                            }
                                                            JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "设备[" + APATTributeList[j].FullName + "]添加黑名单[" + IMSI + "]", "添加黑名单", AddStatus);
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
                                                        string AddStatus = "超时";
                                                        for (int k = 0; k < DataList[i > DataList.Count - 1 ? DataList.Count - 1 : i - 1].Count; k++)
                                                        {
                                                            string IMSI = string.Empty;
                                                            foreach (KeyValuePair<string, string> Item in DataList[i > DataList.Count - 1 ? DataList.Count - 1 : i - 1][k])
                                                            {
                                                                if (Item.Key == "imsi")
                                                                {
                                                                    IMSI = Item.Value;
                                                                    break;
                                                                }
                                                            }
                                                            JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "设备[" + JsonInterFace.BlackList.ParameterList[j].DeviceFullPathName + "]添加黑名单[" + IMSI + "]", "添加黑名单", AddStatus);
                                                        }
                                                        break;
                                                    }

                                                    break;
                                                }
                                                else if (DelayTime > 5000)
                                                {
                                                    JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "设备[" + JsonInterFace.BlackList.ParameterList[j].DeviceFullPathName + "]添加黑名单", "添加黑名单", "超时");
                                                    break;
                                                }
                                                else
                                                {
                                                    Thread.Sleep(DelayTime += 5);
                                                }
                                            }

                                            if (i == DataList.Count) { break; }

                                            WaitResultTimer.Start();

                                            #region GSMV2 and CDMA
                                            if (APATTributeList[j].Mode == DeviceType.GSMV2 || APATTributeList[j].Mode == DeviceType.CDMA)
                                            {
                                                //数据组
                                                Dictionary<string, string> ParaList = new Dictionary<string, string>();
                                                Parameters.ActionType = "Add";
                                                ParaList.Add("wTotalImsi", BWListInfoTable.Rows.Count.ToString());
                                                ParaList.Add("bActionType", "3");
                                                for (int k = 0; k < BWListInfoTable.Rows.Count; k++)
                                                {
                                                    string IMSI = string.Empty;
                                                    IMSI = BWListInfoTable.Rows[k][0].ToString();
                                                    SpecialListID.Add(IMSI);
                                                    ParaList.Add("bIMSI_#" + k.ToString() + "#", IMSI);
                                                    ParaList.Add("bUeActionFlag_#" + k.ToString() + "#", "5");
                                                }
                                                if (NetWorkClient.ControllerServer.Connected)
                                                {
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
                                                }
                                                else
                                                {
                                                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                                    return;
                                                }
                                                break;
                                            }
                                            #endregion
                                            else if(APATTributeList[j].Mode != DeviceType.GSM)
                                            {
                                                //发送
                                                NetWorkClient.ControllerServer.Send(
                                                    JsonInterFace.APBWListDataAddRequest(
                                                            "device",
                                                            APATTributeList[j].FullName,
                                                            APATTributeList[j].DomainFullPathName,
                                                            DataList[i]
                                                        )
                                                    );
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
                                        for (int j = 0; j < BWListInfoTable.Rows.Count; j++)
                                        {
                                            if (SpecialListID[i] == BWListInfoTable.Rows[j]["imsi"].ToString())
                                            {
                                                BWListInfoTable.Rows.RemoveAt(j);
                                                break;
                                            }
                                        }
                                    }

                                    Dispatcher.Invoke(() =>
                                    {
                                        dgBWInfoAdd.ItemsSource = null;
                                        dgBWInfoAdd.ItemsSource = BWListInfoTable.DefaultView;

                                        if (Flag)
                                        {
                                            MessageBox.Show("添加黑名单失败,详见日志！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                        }
                                        else
                                        {
                                            MessageBox.Show("添加黑名单成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                                        }
                                    });
                                }).Start();
                            }
                            #endregion
                            #region 白名单
                            else if (WhiteListFlag.Match(Parameters.ConfigType).Success)
                            {
                                SpecialListID.Clear();
                                GetData(ref DataList, APATTributeList, Parameters.ConfigType);

                                //===========启动进度条窗口===========
                                SubWindow.ProgressBarWindow.BWOProgressBarParameter.TipsContent = "正在提交白名单, 请稍后....";
                                SubWindow.ProgressBarWindow.BWOProgressBarParameter.SubmitTotal = JsonInterFace.WhiteList.Count * APATTributeList.Count;
                                SubWindow.ProgressBarWindow.BWOProgressBarParameter.CompleteTotal = JsonInterFace.WhiteList.Count * APATTributeList.Count;
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
                                    for (int j = 0; j < APATTributeList.Count; j++)
                                    {
                                        if (APATTributeList[j].OnLine != "1")
                                            break;
                                        //获取参数
                                        JsonInterFace.ActionResultStatus.ResoultStatus = string.Empty;
                                        JsonInterFace.ActionResultStatus.Finished = true;
                                        //数据组
                                        for (int i = 0; i <= DataList.Count; i++)
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
                                                        string AddStatus = string.Empty;
                                                        switch (Convert.ToInt32(JsonInterFace.ActionResultStatus.ResoultStatus))
                                                        {
                                                            case 0:
                                                                AddStatus = "成功";
                                                                break;
                                                            case 1:
                                                                Flag = true;
                                                                AddStatus = "失败";
                                                                break;
                                                            default:
                                                                Flag = true;
                                                                AddStatus = "未知错误";
                                                                break;
                                                        }
                                                        for (int k = 0; k < DataList[i > DataList.Count - 1 ? DataList.Count - 1 : i - 1].Count; k++)
                                                        {
                                                            string IMSI = string.Empty;
                                                            foreach (KeyValuePair<string, string> Item in DataList[i > DataList.Count - 1 ? DataList.Count - 1 : i - 1][k])
                                                            {
                                                                if (Item.Key == "imsi")
                                                                {
                                                                    IMSI = Item.Value;
                                                                    SpecialListID.Add(IMSI);
                                                                    break;
                                                                }
                                                            }
                                                            JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "设备[" + APATTributeList[j].FullName + "]添加白名单[" + IMSI + "]", "添加白名单", AddStatus);
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
                                                        string AddStatus = "超时";
                                                        for (int k = 0; k < DataList[i > DataList.Count - 1 ? DataList.Count - 1 : i - 1].Count; k++)
                                                        {
                                                            string IMSI = string.Empty;
                                                            foreach (KeyValuePair<string, string> Item in DataList[i > DataList.Count - 1 ? DataList.Count - 1 : i - 1][k])
                                                            {
                                                                if (Item.Key == "imsi")
                                                                {
                                                                    IMSI = Item.Value;
                                                                    break;
                                                                }
                                                            }
                                                            JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "设备[" + APATTributeList[j].FullName + "]添加白名单[" + IMSI + "]", "添加白名单", AddStatus);
                                                        }
                                                    }
                                                    break;
                                                }
                                                else if (DelayTime > 5000)
                                                {
                                                    JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "设备[" + JsonInterFace.BlackList.ParameterList[j].DeviceFullPathName + "]添加白名单", "添加白名单", "超时");
                                                    break;
                                                }
                                                else
                                                {
                                                    Thread.Sleep(DelayTime += 5);
                                                }
                                            }

                                            if (i == DataList.Count) { break; }

                                            WaitResultTimer.Start();

                                            #region GSMV2 and CDMA
                                            if (APATTributeList[j].Mode == DeviceType.GSMV2 || APATTributeList[j].Mode == DeviceType.CDMA)
                                            {
                                                //数据组
                                                Dictionary<string, string> ParaList = new Dictionary<string, string>();
                                                Parameters.ActionType = "Add";
                                                ParaList.Add("wTotalImsi", BWListInfoTable.Rows.Count.ToString());
                                                ParaList.Add("bActionType", "3");
                                                for (int k = 0; k < BWListInfoTable.Rows.Count; k++)
                                                {
                                                    string IMSI = string.Empty;
                                                    IMSI = BWListInfoTable.Rows[k][0].ToString();
                                                    SpecialListID.Add(IMSI);
                                                    ParaList.Add("bIMSI_#" + k.ToString() + "#", IMSI);
                                                    ParaList.Add("bUeActionFlag_#" + k.ToString() + "#", "1");
                                                }

                                                if (NetWorkClient.ControllerServer.Connected)
                                                {
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
                                                }
                                                else
                                                {
                                                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                                    return;
                                                }
                                            }
                                            #endregion
                                            else if(APATTributeList[j].Mode != DeviceType.GSM)
                                            {
                                                //发送
                                                NetWorkClient.ControllerServer.Send(
                                                    JsonInterFace.APBWListDataAddRequest(
                                                            "device",
                                                            APATTributeList[j].FullName,
                                                            APATTributeList[j].DomainFullPathName,
                                                            DataList[i]
                                                        )
                                                    );
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
                                        for (int j = 0; j < BWListInfoTable.Rows.Count; j++)
                                        {
                                            if (SpecialListID[i] == BWListInfoTable.Rows[j]["imsi"].ToString())
                                            {
                                                BWListInfoTable.Rows.RemoveAt(j);
                                                break;
                                            }
                                        }
                                    }

                                    Dispatcher.Invoke(() =>
                                    {
                                        dgBWInfoAdd.ItemsSource = null;
                                        dgBWInfoAdd.ItemsSource = BWListInfoTable.DefaultView;
                                        if (Flag)
                                        {
                                            MessageBox.Show("添加白名单失败,详见日志！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                        }
                                        else
                                        {
                                            MessageBox.Show("添加白名单成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                                        }
                                    });
                                }).Start();
                            }
                            #endregion
                            #region 自定义名单
                            if (CustomListFlag.Match(Parameters.ConfigType).Success)
                            {
                                SpecialListID.Clear();
                                GetData(ref DataList, APATTributeList, Parameters.ConfigType);

                                //===========启动进度条窗口===========
                                SubWindow.ProgressBarWindow.BWOProgressBarParameter.TipsContent = "正在提交黑名单, 请稍后....";
                                SubWindow.ProgressBarWindow.BWOProgressBarParameter.SubmitTotal = JsonInterFace.CustomList.Count * APATTributeList.Count;
                                SubWindow.ProgressBarWindow.BWOProgressBarParameter.CompleteTotal = JsonInterFace.CustomList.Count * APATTributeList.Count;
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
                                    for (int j = 0; j < APATTributeList.Count; j++)
                                    {
                                        if (APATTributeList[j].OnLine != "1")
                                            break;
                                        //获取参数
                                        JsonInterFace.ActionResultStatus.ResoultStatus = string.Empty;
                                        JsonInterFace.ActionResultStatus.Finished = true;
                                        //数据组
                                        for (int i = 0; i <= DataList.Count; i++)
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
                                                        string AddStatus = string.Empty;
                                                        switch (Convert.ToInt32(JsonInterFace.ActionResultStatus.ResoultStatus))
                                                        {
                                                            case 0:
                                                                AddStatus = "成功";
                                                                break;
                                                            case 1:
                                                                Flag = true;
                                                                AddStatus = "失败";
                                                                break;
                                                            default:
                                                                Flag = true;
                                                                AddStatus = "未知错误";
                                                                break;
                                                        }
                                                        for (int k = 0; k < DataList[i > DataList.Count - 1 ? DataList.Count - 1 : i - 1].Count; k++)
                                                        {
                                                            string IMSI = string.Empty;
                                                            foreach (KeyValuePair<string, string> Item in DataList[i > DataList.Count - 1 ? DataList.Count - 1 : i - 1][k])
                                                            {
                                                                if (Item.Key == "imsi")
                                                                {
                                                                    IMSI = Item.Value;
                                                                    SpecialListID.Add(IMSI);
                                                                    break;
                                                                }
                                                            }
                                                            JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "设备[" + APATTributeList[j].FullName + "]添加自定义名单[" + IMSI + "]", "添加自定义名单", AddStatus);
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
                                                        string AddStatus = "超时";
                                                        for (int k = 0; k < DataList[i > DataList.Count - 1 ? DataList.Count - 1 : i - 1].Count; k++)
                                                        {
                                                            string IMSI = string.Empty;
                                                            foreach (KeyValuePair<string, string> Item in DataList[i > DataList.Count - 1 ? DataList.Count - 1 : i - 1][k])
                                                            {
                                                                if (Item.Key == "imsi")
                                                                {
                                                                    IMSI = Item.Value;
                                                                    break;
                                                                }
                                                            }
                                                            JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "设备[" + APATTributeList[j].FullName + "]添加自定义名单[" + IMSI + "]", "添加自定义名单", AddStatus);
                                                        }
                                                    }
                                                    break;
                                                }
                                                else if (DelayTime > 5000)
                                                {
                                                    JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "设备[" + JsonInterFace.BlackList.ParameterList[j].DeviceFullPathName + "]添加自定义名单", "添加自定义名单", "超时");
                                                    break;
                                                }
                                                else
                                                {
                                                    Thread.Sleep(DelayTime += 5);
                                                }
                                            }

                                            if (i == DataList.Count) { break; }

                                            WaitResultTimer.Start();
                                            if (APATTributeList[j].Mode != DeviceType.GSMV2 && APATTributeList[j].Mode != DeviceType.CDMA && APATTributeList[j].Mode != DeviceType.GSM)
                                            {
                                                //发送
                                                NetWorkClient.ControllerServer.Send(
                                                JsonInterFace.APBWListDataAddRequest(
                                                            "device",
                                                            APATTributeList[j].FullName,
                                                            APATTributeList[j].DomainFullPathName,
                                                            DataList[i]
                                                    )
                                                );
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
                                        for (int j = 0; j < BWListInfoTable.Rows.Count; j++)
                                        {
                                            if (SpecialListID[i] == BWListInfoTable.Rows[j]["imsi"].ToString())
                                            {
                                                BWListInfoTable.Rows.RemoveAt(j);
                                                break;
                                            }
                                        }
                                    }

                                    Dispatcher.Invoke(() =>
                                    {
                                        dgBWInfoAdd.ItemsSource = null;
                                        dgBWInfoAdd.ItemsSource = BWListInfoTable.DefaultView;
                                        if (Flag)
                                        {
                                            MessageBox.Show("添加自定义名单失败,详见日志！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                        }
                                        else
                                        {
                                            MessageBox.Show("添加自定义名单成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                                        }
                                    });
                                }).Start();
                            }
                            #endregion
                        }
                        else
                        {
                            MessageBox.Show("请添加相应的名单信息到列表中，再提交！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
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

        private void WaitResultTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            JsonInterFace.ActionResultStatus.NoResultErrorDeviceName = true;
        }

        private void GetData(ref List<List<Dictionary<string, string>>> DataList, List<APATTributes> APATTributeList, string Type)
        {
            #region 黑名单
            if (new Regex(Type).Match("BlackList").Success)
            {
                if ((BWListInfoTable.Rows.Count % Parameters.BWListIMSIConfigurationTotal) == 0)
                {
                    JsonInterFace.BlackList.Count = (BWListInfoTable.Rows.Count / Parameters.BWListIMSIConfigurationTotal);
                }
                else
                {
                    JsonInterFace.BlackList.Count = (BWListInfoTable.Rows.Count / Parameters.BWListIMSIConfigurationTotal) + 1;
                }
                DataList = new List<List<Dictionary<string, string>>>();
                DataList.Clear();

                for (int j = 0, i = 0; j < JsonInterFace.BlackList.Count; j++)
                {
                    List<Dictionary<string, string>> WItem = new List<Dictionary<string, string>>();
                    for (int k = 0; i < BWListInfoTable.Rows.Count; i++, k++)
                    {
                        Dictionary<string, string> BWListPara = new Dictionary<string, string>();
                        if (!BWListInfoTable.Rows[i][0].ToString().Equals(""))
                        {
                            BWListPara.Add("imsi", BWListInfoTable.Rows[i][0].ToString());
                            lock (JsonInterFace.BlackList.BLackListAddingLock)
                            {
                                JsonInterFace.BlackList.IMSIPara.Add(BWListInfoTable.Rows[i][0].ToString());
                            }
                        }

                        if (!BWListInfoTable.Rows[i][1].ToString().Equals(""))
                        {
                            BWListPara.Add("imei", BWListInfoTable.Rows[i][1].ToString());
                            lock (JsonInterFace.BlackList.BLackListAddingLock)
                            {
                                JsonInterFace.BlackList.IMEIPara.Add(BWListInfoTable.Rows[i][1].ToString());
                            }
                        }

                        if (!BWListInfoTable.Rows[i][2].ToString().Equals(""))
                        {
                            JsonInterFace.BlackList.Type = BWListInfoTable.Rows[i][2].ToString();

                            if (JsonInterFace.BlackList.Type == ("黑名单"))
                            {
                                BWListPara.Add("bwFlag", "black");
                            }
                            else if (JsonInterFace.BlackList.Type == ("白名单"))
                            {
                                BWListPara.Add("bwFlag", "white");
                            }
                            else
                            {
                                BWListPara.Add("bwFlag", "other");
                            }
                        }
                        if (!BWListInfoTable.Rows[i][3].ToString().Equals(""))
                        {
                            BWListPara.Add("rbStart", BWListInfoTable.Rows[i][3].ToString());
                        }

                        if (!BWListInfoTable.Rows[i][4].ToString().Equals(""))
                        {
                            BWListPara.Add("rbEnd", BWListInfoTable.Rows[i][4].ToString());
                        }

                        BWListPara.Add("time", DateTime.Now.ToString());

                        if (!BWListInfoTable.Rows[i][5].ToString().Equals(""))
                        {
                            BWListPara.Add("des", BWListInfoTable.Rows[i][5].ToString());
                        }

                        if (k == 0)
                        {
                            WItem.Add(BWListPara);
                            continue;
                        }

                        if (k % (Parameters.BWListIMSIConfigurationTotal) != 0)
                        {
                            WItem.Add(BWListPara);
                        }
                        else if (k % (Parameters.BWListIMSIConfigurationTotal) == 0)
                        {
                            break;
                        }
                    }
                    DataList.Add(WItem);
                }

                //当前选定的(站点总数或设备数量)
                for (int j = 0; j < JsonInterFace.BlackList.ParameterList.Count; j++)
                {
                    //站点
                    if (JsonInterFace.BlackList.ParameterList[j].NodeType == "domain")
                    {
                        JsonInterFace.BlackList.StationCount++;
                        for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                        {
                            string TmepDomainFullPathName = string.Empty;
                            string[] TmepDomainFullPathNameList = JsonInterFace.APATTributesLists[i].FullName.Split(new char[] { '.' });
                            for (int list = 0; list < TmepDomainFullPathNameList.Length - 1; list++)
                            {
                                if (list == 0)
                                    TmepDomainFullPathName += TmepDomainFullPathNameList[list];
                                else
                                    TmepDomainFullPathName += "." + TmepDomainFullPathNameList[list];
                            }
                            if (TmepDomainFullPathName == JsonInterFace.BlackList.ParameterList[j].DomainFullPathName)
                            {
                                APATTributeList.Add(JsonInterFace.APATTributesLists[i]);
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
                                APATTributeList.Add(JsonInterFace.APATTributesLists[i]);
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
                if ((BWListInfoTable.Rows.Count % Parameters.BWListIMSIConfigurationTotal) == 0)
                {
                    JsonInterFace.WhiteList.Count = (BWListInfoTable.Rows.Count / Parameters.BWListIMSIConfigurationTotal);
                }
                else
                {
                    JsonInterFace.WhiteList.Count = (BWListInfoTable.Rows.Count / Parameters.BWListIMSIConfigurationTotal) + 1;
                }
                DataList = new List<List<Dictionary<string, string>>>();
                DataList.Clear();

                for (int j = 0, i = 0; j < JsonInterFace.WhiteList.Count; j++)
                {
                    List<Dictionary<string, string>> WItem = new List<Dictionary<string, string>>();
                    for (int k = 0; i < BWListInfoTable.Rows.Count; i++, k++)
                    {
                        Dictionary<string, string> BWListPara = new Dictionary<string, string>();
                        if (!BWListInfoTable.Rows[i][0].ToString().Equals(""))
                        {
                            BWListPara.Add("imsi", BWListInfoTable.Rows[i][0].ToString());
                            lock (JsonInterFace.WhiteList.BLackListAddingLock)
                            {
                                JsonInterFace.WhiteList.IMSIPara.Add(BWListInfoTable.Rows[i][0].ToString());
                            }
                        }

                        if (!BWListInfoTable.Rows[i][1].ToString().Equals(""))
                        {
                            BWListPara.Add("imei", BWListInfoTable.Rows[i][1].ToString());
                            lock (JsonInterFace.WhiteList.BLackListAddingLock)
                            {
                                JsonInterFace.WhiteList.IMEIPara.Add(BWListInfoTable.Rows[i][1].ToString());
                            }
                        }

                        if (!BWListInfoTable.Rows[i][2].ToString().Equals(""))
                        {
                            JsonInterFace.WhiteList.Type = BWListInfoTable.Rows[i][2].ToString();

                            if (JsonInterFace.WhiteList.Type == ("黑名单"))
                            {
                                BWListPara.Add("bwFlag", "black");
                            }
                            else if (JsonInterFace.WhiteList.Type == ("白名单"))
                            {
                                BWListPara.Add("bwFlag", "white");
                            }
                            else
                            {
                                BWListPara.Add("bwFlag", "other");
                            }
                        }
                        if (!BWListInfoTable.Rows[i][3].ToString().Equals(""))
                        {
                            BWListPara.Add("rbStart", BWListInfoTable.Rows[i][3].ToString());
                        }

                        if (!BWListInfoTable.Rows[i][4].ToString().Equals(""))
                        {
                            BWListPara.Add("rbEnd", BWListInfoTable.Rows[i][4].ToString());
                        }

                        BWListPara.Add("time", DateTime.Now.ToString());

                        if (!BWListInfoTable.Rows[i][5].ToString().Equals(""))
                        {
                            BWListPara.Add("des", BWListInfoTable.Rows[i][5].ToString());
                        }

                        if (k == 0)
                        {
                            WItem.Add(BWListPara);
                            continue;
                        }

                        if (k % (Parameters.BWListIMSIConfigurationTotal) != 0)
                        {
                            WItem.Add(BWListPara);
                        }
                        else if (k % (Parameters.BWListIMSIConfigurationTotal) == 0)
                        {
                            break;
                        }
                    }
                    DataList.Add(WItem);
                }

                //当前选定的(站点总数或设备数量)
                for (int j = 0; j < JsonInterFace.WhiteList.ParameterList.Count; j++)
                {
                    //站点
                    if (JsonInterFace.WhiteList.ParameterList[j].NodeType == "domain")
                    {
                        JsonInterFace.WhiteList.StationCount++;
                        for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                        {
                            string TmepDomainFullPathName = string.Empty;
                            string[] TmepDomainFullPathNameList = JsonInterFace.APATTributesLists[i].FullName.Split(new char[] { '.' });
                            for (int list = 0; list < TmepDomainFullPathNameList.Length - 1; list++)
                            {
                                if (list == 0)
                                    TmepDomainFullPathName += TmepDomainFullPathNameList[list];
                                else
                                    TmepDomainFullPathName += "." + TmepDomainFullPathNameList[list];
                            }
                            if (TmepDomainFullPathName == JsonInterFace.BlackList.ParameterList[j].DomainFullPathName)
                            {
                                APATTributeList.Add(JsonInterFace.APATTributesLists[i]);
                            }
                        }
                    }
                    //设备
                    else
                    {
                        JsonInterFace.WhiteList.DeviceCount++;
                        for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                        {
                            if (JsonInterFace.APATTributesLists[i].FullName == JsonInterFace.BlackList.ParameterList[j].DeviceFullPathName)
                            {
                                APATTributeList.Add(JsonInterFace.APATTributesLists[i]);
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
                if ((BWListInfoTable.Rows.Count % Parameters.BWListIMSIConfigurationTotal) == 0)
                {
                    JsonInterFace.CustomList.Count = (BWListInfoTable.Rows.Count / Parameters.BWListIMSIConfigurationTotal);
                }
                else
                {
                    JsonInterFace.CustomList.Count = (BWListInfoTable.Rows.Count / Parameters.BWListIMSIConfigurationTotal) + 1;
                }
                DataList = new List<List<Dictionary<string, string>>>();
                DataList.Clear();

                for (int j = 0, i = 0; j < JsonInterFace.CustomList.Count; j++)
                {
                    List<Dictionary<string, string>> WItem = new List<Dictionary<string, string>>();
                    for (int k = 0; i < BWListInfoTable.Rows.Count; i++, k++)
                    {
                        Dictionary<string, string> BWListPara = new Dictionary<string, string>();
                        if (!BWListInfoTable.Rows[i][0].ToString().Equals(""))
                        {
                            BWListPara.Add("imsi", BWListInfoTable.Rows[i][0].ToString());
                            lock (JsonInterFace.CustomList.BLackListAddingLock)
                            {
                                JsonInterFace.CustomList.IMSIPara.Add(BWListInfoTable.Rows[i][0].ToString());
                            }
                        }

                        if (!BWListInfoTable.Rows[i][1].ToString().Equals(""))
                        {
                            BWListPara.Add("imei", BWListInfoTable.Rows[i][1].ToString());
                            lock (JsonInterFace.CustomList.BLackListAddingLock)
                            {
                                JsonInterFace.CustomList.IMEIPara.Add(BWListInfoTable.Rows[i][1].ToString());
                            }
                        }

                        if (!BWListInfoTable.Rows[i][2].ToString().Equals(""))
                        {
                            JsonInterFace.CustomList.Type = BWListInfoTable.Rows[i][2].ToString();

                            if (JsonInterFace.CustomList.Type == ("黑名单"))
                            {
                                BWListPara.Add("bwFlag", "black");
                            }
                            else if (JsonInterFace.CustomList.Type == ("白名单"))
                            {
                                BWListPara.Add("bwFlag", "white");
                            }
                            else
                            {
                                BWListPara.Add("bwFlag", "other");
                            }
                        }
                        if (!BWListInfoTable.Rows[i][3].ToString().Equals(""))
                        {
                            BWListPara.Add("rbStart", BWListInfoTable.Rows[i][3].ToString());
                        }

                        if (!BWListInfoTable.Rows[i][4].ToString().Equals(""))
                        {
                            BWListPara.Add("rbEnd", BWListInfoTable.Rows[i][4].ToString());
                        }

                        BWListPara.Add("time", DateTime.Now.ToString());

                        if (!BWListInfoTable.Rows[i][5].ToString().Equals(""))
                        {
                            BWListPara.Add("des", BWListInfoTable.Rows[i][5].ToString());
                        }

                        if (k == 0)
                        {
                            WItem.Add(BWListPara);
                            continue;
                        }

                        if (k % (Parameters.BWListIMSIConfigurationTotal) != 0)
                        {
                            WItem.Add(BWListPara);
                        }
                        else if (k % (Parameters.BWListIMSIConfigurationTotal) == 0)
                        {
                            break;
                        }
                    }
                    DataList.Add(WItem);
                }

                //当前选定的(站点总数或设备数量)
                for (int j = 0; j < JsonInterFace.CustomList.ParameterList.Count; j++)
                {
                    //站点
                    if (JsonInterFace.CustomList.ParameterList[j].NodeType == "domain")
                    {
                        JsonInterFace.CustomList.StationCount++;
                        for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                        {
                            string TmepDomainFullPathName = string.Empty;
                            string[] TmepDomainFullPathNameList = JsonInterFace.APATTributesLists[i].FullName.Split(new char[] { '.' });
                            for (int list = 0; list < TmepDomainFullPathNameList.Length - 1; list++)
                            {
                                if (list == 0)
                                    TmepDomainFullPathName += TmepDomainFullPathNameList[list];
                                else
                                    TmepDomainFullPathName += "." + TmepDomainFullPathNameList[list];
                            }
                            if (TmepDomainFullPathName == JsonInterFace.BlackList.ParameterList[j].DomainFullPathName)
                            {
                                APATTributeList.Add(JsonInterFace.APATTributesLists[i]);
                            }
                        }
                    }
                    //设备
                    else
                    {
                        JsonInterFace.CustomList.DeviceCount++;
                        for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                        {
                            if (JsonInterFace.APATTributesLists[i].FullName == JsonInterFace.BlackList.ParameterList[j].DeviceFullPathName)
                            {
                                APATTributeList.Add(JsonInterFace.APATTributesLists[i]);
                            }
                        }
                    }
                }

                return;
            }
            #endregion
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
                System.Environment.Exit(System.Environment.ExitCode);
            }
        }

        /// <summary>
        /// 移动窗口
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Regex BlackListFlag = new Regex("BlackList");
            Regex WhiteListFlag = new Regex("WhiteList");
            Regex CustomListFlag = new Regex("OtherList");

            //应用用户类型
            List<string> UserTypeList = new List<string>();
            JsonInterFace.IODataHelper.CustomUserTypeGetting(ref UserTypeList);
            cbbUserType.Items.Clear();
            for (int i = 0; i < UserTypeList.Count; i++)
            {
                cbbUserType.Items.Add(UserTypeList[i]);
            }

            if (BlackListFlag.Match(Parameters.ConfigType).Success)
            {
                for (int i = 0; i < cbbUserType.Items.Count; i++)
                {
                    if (cbbUserType.Items[i].ToString() == "黑名单")
                    {
                        cbbUserType.SelectedIndex = i;
                    }
                }
                txtRbStart.IsEnabled = true;
                txtRbEnd.IsEnabled = true;
                btnImport.IsEnabled = true;
                cbbUserType.IsEnabled = false;
            }
            else if (WhiteListFlag.Match(Parameters.ConfigType).Success)
            {
                for (int i = 0; i < cbbUserType.Items.Count; i++)
                {
                    if (cbbUserType.Items[i].ToString() == "白名单")
                    {
                        cbbUserType.SelectedIndex = i;
                    }
                }
                txtRbStart.IsEnabled = false;
                txtRbEnd.IsEnabled = false;
                btnImport.IsEnabled = true;
                cbbUserType.IsEnabled = false;
            }
            else if (CustomListFlag.Match(Parameters.ConfigType).Success)
            {
                for (int i = 0; i < cbbUserType.Items.Count; i++)
                {
                    if (cbbUserType.Items[i].ToString() == "普通用户")
                    {
                        cbbUserType.SelectedIndex = i;
                    }
                }
                txtRbStart.IsEnabled = true;
                txtRbEnd.IsEnabled = true;
                btnImport.IsEnabled = true;
                cbbUserType.IsEnabled = false;
            }

            //获取窗口句柄
            Parameters.AddBWListWinHandle = new WindowInteropHelper(this).Handle;
        }
        /// <summary>
        /// 导入黑，白名单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            InputBWList();
        }

        //批量导入黑白名单列表
        private void InputBWList()
        {
            int Total = 0;
            int Normal = 0;
            int Error = 0;
            Regex BlackListFlag = new Regex("BlackList");
            Regex WhiteListFlag = new Regex("WhiteList");
            Regex CustomListFlag = new Regex("OtherList");

            string UserType = string.Empty;

            if (BlackListFlag.Match(Parameters.ConfigType).Success)
            {
                UserType = "黑名单";
            }
            else if (WhiteListFlag.Match(Parameters.ConfigType).Success)
            {
                UserType = "白名单";
            }
            else if (CustomListFlag.Match(Parameters.ConfigType).Success)
            {
                UserType = "普通用户";
            }

            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "(文本文件 *.txt)|*.txt|(Excel文件 *.xls,*.xlsx)|*.xls;*.xlsx";
            ofd.AddExtension = true;
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;

            if (!(bool)ofd.ShowDialog())
            {
                return;
            }

            new Thread(() =>
            {
                string BWListFileName = ofd.FileName.ToString();
                if (new FileInfo(BWListFileName).Extension.ToLower() == Parameters.FileTypeEx.TXT.ToLower())
                {
                    if (Parameters.CheckFileEncodingType.FileGetTypeByName(BWListFileName).BodyName.ToLower() != "utf-8")
                    {
                        MessageBox.Show("请将文本文件转为[UTF-8]格式再执行导入！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    FileStream fs = new FileStream(BWListFileName, FileMode.Open, FileAccess.Read);
                    StreamReader FileStreamReader = new StreamReader(fs);
                    string strLine = FileStreamReader.ReadLine();
                    BWListInfoTable.Rows.Clear();
                    while (strLine != null)
                    {
                        DataRow rw = BWListInfoTable.NewRow();
                        if (BlackListFlag.Match(Parameters.ConfigType).Success)
                        {
                            if (strLine.Split(new char[] { ',' }).Length == 6)
                            {
                                rw[0] = strLine.Split(new char[] { ',' })[0];
                                rw[1] = strLine.Split(new char[] { ',' })[1];
                                rw[2] = strLine.Split(new char[] { ',' })[2];
                                rw[3] = strLine.Split(new char[] { ',' })[3];
                                rw[4] = strLine.Split(new char[] { ',' })[4];
                                rw[5] = strLine.Split(new char[] { ',' })[5];
                                BWListInfoTable.Rows.Add(rw);
                                Normal++;
                            }
                            else
                            {
                                Error++;
                            }
                        }
                        else if (WhiteListFlag.Match(Parameters.ConfigType).Success)
                        {
                            if (strLine.Split(new char[] { ',' }).Length == 4)
                            {
                                rw[0] = strLine.Split(new char[] { ',' })[0];
                                rw[1] = strLine.Split(new char[] { ',' })[1];
                                rw[2] = strLine.Split(new char[] { ',' })[2];
                                rw[5] = strLine.Split(new char[] { ',' })[3];
                                BWListInfoTable.Rows.Add(rw);
                                Normal++;
                            }
                            else if (strLine.Split(new char[] { ',' }).Length == 6)
                            {
                                rw[0] = strLine.Split(new char[] { ',' })[0];
                                rw[1] = strLine.Split(new char[] { ',' })[1];
                                rw[2] = strLine.Split(new char[] { ',' })[2];
                                rw[3] = strLine.Split(new char[] { ',' })[3];
                                rw[4] = strLine.Split(new char[] { ',' })[4];
                                rw[5] = strLine.Split(new char[] { ',' })[5];
                                BWListInfoTable.Rows.Add(rw);
                                Normal++;
                            }
                            else
                            {
                                Error++;
                            }
                        }
                        else if (CustomListFlag.Match(Parameters.ConfigType).Success)
                        {
                            if (strLine.Split(new char[] { ',' }).Length == 6)
                            {
                                rw[0] = strLine.Split(new char[] { ',' })[0];
                                rw[1] = strLine.Split(new char[] { ',' })[1];
                                rw[2] = strLine.Split(new char[] { ',' })[2];
                                rw[3] = strLine.Split(new char[] { ',' })[3];
                                rw[4] = strLine.Split(new char[] { ',' })[4];
                                rw[5] = strLine.Split(new char[] { ',' })[5];
                                BWListInfoTable.Rows.Add(rw);
                                Normal++;
                            }
                            else
                            {
                                Error++;
                            }
                        }
                        strLine = FileStreamReader.ReadLine();
                        Total++;
                    }
                    //关闭此StreamReader对象
                    FileStreamReader.Close();

                    Dispatcher.Invoke(() =>
                    {
                        dgBWInfoAdd.ItemsSource = null;
                        dgBWInfoAdd.ItemsSource = BWListInfoTable.DefaultView;
                    });
                }
                else if (new FileInfo(BWListFileName).Extension.ToLower() == Parameters.FileTypeEx.XLS.ToLower() || new FileInfo(BWListFileName).Extension.ToLower() == Parameters.FileTypeEx.XLSX.ToLower())
                {
                    using (ExcelHelper excelHelper = new ExcelHelper(@BWListFileName))
                    {
                        DataTable dt = excelHelper.ExcelToDataTable("Sheet1", true);//读取数据
                        if (dt != null)
                        {
                            BWListInfoTable.Rows.Clear();
                            if (BlackListFlag.Match(Parameters.ConfigType).Success)
                            {
                                foreach (DataRow dr in dt.Rows)//DataTable转ObservableCollection
                                {
                                    try
                                    {
                                        DataRow rw = BWListInfoTable.NewRow();
                                        rw[0] = dr[0].ToString();
                                        rw[1] = dr[1].ToString();
                                        rw[2] = dr[2].ToString();
                                        rw[3] = dr[3].ToString();
                                        rw[4] = dr[4].ToString();
                                        rw[5] = dr[5].ToString();
                                        BWListInfoTable.Rows.Add(rw);
                                        Normal++;
                                        Total++;
                                    }
                                    catch
                                    {
                                        Error++;
                                    }
                                }
                            }
                            else if (WhiteListFlag.Match(Parameters.ConfigType).Success)
                            {
                                if (dt.Columns.Count == 4)
                                {
                                    foreach (DataRow dr in dt.Rows)//DataTable转ObservableCollection
                                    {
                                        try
                                        {
                                            DataRow rw = BWListInfoTable.NewRow();
                                            rw[0] = dr[0].ToString();
                                            rw[1] = dr[1].ToString();
                                            rw[2] = dr[2].ToString();
                                            rw[3] = "";
                                            rw[4] = "";
                                            rw[5] = dr[3].ToString();
                                            BWListInfoTable.Rows.Add(rw);
                                            Normal++;
                                            Total++;
                                        }
                                        catch
                                        {
                                            Error++;
                                        }
                                    }
                                }
                                else if (dt.Columns.Count == 6)
                                {
                                    foreach (DataRow dr in dt.Rows)//DataTable转ObservableCollection
                                    {
                                        try
                                        {
                                            DataRow rw = BWListInfoTable.NewRow();
                                            rw[0] = dr[0].ToString();
                                            rw[1] = dr[1].ToString();
                                            rw[2] = dr[2].ToString();
                                            rw[3] = dr[3].ToString();
                                            rw[4] = dr[4].ToString();
                                            rw[5] = dr[5].ToString();
                                            BWListInfoTable.Rows.Add(rw);
                                            Normal++;
                                            Total++;
                                        }
                                        catch
                                        {
                                            Error++;
                                        }
                                    }
                                }
                                else
                                {
                                    Error++;
                                }
                            }
                            else if (CustomListFlag.Match(Parameters.ConfigType).Success)
                            {
                                foreach (DataRow dr in dt.Rows)//DataTable转ObservableCollection
                                {
                                    try
                                    {
                                        DataRow rw = BWListInfoTable.NewRow();
                                        rw[0] = dr[0].ToString();
                                        rw[1] = dr[1].ToString();
                                        rw[2] = dr[2].ToString();
                                        rw[3] = dr[3].ToString();
                                        rw[4] = dr[4].ToString();
                                        rw[5] = dr[5].ToString();
                                        BWListInfoTable.Rows.Add(rw);
                                        Normal++;
                                        Total++;
                                    }
                                    catch
                                    {
                                        Error++;
                                    }
                                }
                            }
                        }
                        else
                        {
                            return;
                        }
                    }

                    Dispatcher.Invoke(() =>
                    {
                        dgBWInfoAdd.ItemsSource = null;
                        dgBWInfoAdd.ItemsSource = BWListInfoTable.DefaultView;
                    });
                }
                else
                {
                    MessageBox.Show("只支持Excel或Text文本文件导入！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                //提示信息
                if (Error > 0)
                {
                    MessageBox.Show("总计导入" + UserType + "：\n总数:[" + Total.ToString() + "]\n格式不正确总数：[" + Error.ToString() + "]\n实际导入总数：[" + Normal.ToString() + "]\n" +
                        "[黑白名单格式说明]:\n" +
                        "【文本文件】\n" +
                        "[黑名单格式]:IMSI,IMEI,用户类型,RbStart,RbEnd,别名 每个参数之间用逗号(',')分开" +
                        "[白名单格式]:IMSI,IMEI,用户类型,RbStart,RbEnd,别名 每个参数之间用逗号(',')分开\n" +
                        "[普通用户格式]:IMSI,IMEI,用户类型,RbStart,RbEnd,别名 每个参数之间用逗号(',')分开\n" +
                        "【Excel文件】\n" +
                        "黑白名单格式按照文件模板提供的格式输入正确的参数到相应的栏位即可！\n" +
                        "注：导入白名单时，(RbStart，RbEnd) 参数不需要录入，留空即可！",
                        "提示",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                        );
                }
                else
                {
                    MessageBox.Show("总计导入" + UserType + "：\n总数:[" + Total.ToString() + "]\n格式不正确总数：[" + Error.ToString() + "]\n实际导入总数：[" + Normal.ToString() + "]", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }).Start();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            BWListInfoTable.Rows.Clear();
            dgBWInfoAdd.ItemsSource = null;
            dgBWInfoAdd.ItemsSource = BWListInfoTable.DefaultView;
        }
    }
}
