using DataInterface;
using Microsoft.Win32;
using NetController;
using ParameterControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
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
    /// DeviceManagerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceManagerWindow : Window
    {
        #region 参数
        public class SelfDeviceParameter
        {
            private string selfID;
            private string parentID;
            private string domainFullNamePath;
            private string station;
            private string deviceName;
            private string mode;
            private bool netWorkMode;
            private string iP;
            private string port;
            private string netMask;
            private string sN;
            private string deviceNameFlag;
            private int deviceFlagModeIndex;
            private bool isoneline;
            private string innerType;

            public bool IsOneline
            {
                get
                {
                    return isoneline;
                }

                set
                {
                    isoneline = value;
                }
            }

            public string DomainFullNamePath
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

            public string Station
            {
                get
                {
                    return station;
                }

                set
                {
                    station = value;
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

            public string Mode
            {
                get
                {
                    return mode;
                }

                set
                {
                    mode = value;
                }
            }

            public bool NetWorkMode
            {
                get
                {
                    return netWorkMode;
                }

                set
                {
                    netWorkMode = value;
                }
            }

            public string IP
            {
                get
                {
                    return iP;
                }

                set
                {
                    iP = value;
                }
            }

            public string Port
            {
                get
                {
                    return port;
                }

                set
                {
                    port = value;
                }
            }

            public string NetMask
            {
                get
                {
                    return netMask;
                }

                set
                {
                    netMask = value;
                }
            }

            public string SN
            {
                get
                {
                    return sN;
                }

                set
                {
                    sN = value;
                }
            }

            public string DeviceNameFlag
            {
                get
                {
                    return deviceNameFlag;
                }

                set
                {
                    deviceNameFlag = value;
                }
            }

            public int DeviceFlagModeIndex
            {
                get
                {
                    return deviceFlagModeIndex;
                }

                set
                {
                    deviceFlagModeIndex = value;
                }
            }

            public string SelfID
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

            public string InnerType
            {
                get
                {
                    return innerType;
                }

                set
                {
                    innerType = value;
                }
            }
        }
        private class MorePLMNListClass
        {
            private string _pLMNS;

            public string PLMNS
            {
                get
                {
                    return _pLMNS;
                }

                set
                {
                    _pLMNS = value;
                }
            }
        }
        private class PerierFreqListClass
        {
            private string _perierFreq;

            public string PerierFreq
            {
                get
                {
                    return _perierFreq;
                }

                set
                {
                    _perierFreq = value;
                }
            }
        }
        #endregion

        #region 字段
        private object Device_managerLanguageClass = null;
        private Dictionary<string, Uri> AllWindow = new Dictionary<string, Uri>();
        private SelfDeviceParameter selfParam = new SelfDeviceParameter();
        private string TreeViewSelectDomainFullPathName = string.Empty;
        private string TreeViewSelectItemID = string.Empty;
        private Thread ControlUpDownActionInfoThread = null;
        private Thread GettingSelfHandle = null;
        private System.Timers.Timer WaitUpDownLoadTimeOut = null;
        public SelfDeviceParameter SelfDevicePara = new SelfDeviceParameter();
        public static string SelfIMSIList = null;
        private FtpHelper FTP = null;
        private static string SelfName = string.Empty;
        private static string FullName = string.Empty;
        private static string SelfID = string.Empty;
        private static string ParentID = string.Empty;
        private static string SelfNodeType = string.Empty;
        private static string IsStation = string.Empty;
        private static bool IsOnline = false;
        private static string Model = string.Empty;

        private static string ParameterKeyName = string.Empty;

        //GSM白名单显示表
        private static ObservableCollection<GSMLibyraryRegQueryClass> GSMLibyraryRegIMSILists = new ObservableCollection<GSMLibyraryRegQueryClass>();
        private static ObservableCollection<GSMLibyraryRegQueryClass> GSMLibyraryRegIMEILists = new ObservableCollection<GSMLibyraryRegQueryClass>();

        //多PLMN ，周期频点列表
        private static ObservableCollection<MorePLMNListClass> MorePLMNList = new ObservableCollection<MorePLMNListClass>();
        private static ObservableCollection<PerierFreqListClass> PeriorFreqList = new ObservableCollection<PerierFreqListClass>();

        //WCDMA多PLMN ，周期频点列表
        private static ObservableCollection<MorePLMNListClass> WCDMAMorePLMNList = new ObservableCollection<MorePLMNListClass>();
        private static ObservableCollection<PerierFreqListClass> WCDMAPeriorFreqList = new ObservableCollection<PerierFreqListClass>();

        //TDS多PLMN ，周期频点列表
        private static ObservableCollection<MorePLMNListClass> TDSMorePLMNList = new ObservableCollection<MorePLMNListClass>();
        private static ObservableCollection<PerierFreqListClass> TDSPeriorFreqList = new ObservableCollection<PerierFreqListClass>();

        //GSM 短信列表
        private GSMSMSParameterClass GSMSMSTmp = new GSMSMSParameterClass();
        private static ObservableCollection<GSMSMSParameterClass> GSMSMSList = new ObservableCollection<GSMSMSParameterClass>();

        //GSM 电话记录
        private Thread PhoneRecordListThread = null;
        private static ObservableCollection<GSMDeviceSMSPhoneNumberRecordClass> PhoneRecordList = new ObservableCollection<GSMDeviceSMSPhoneNumberRecordClass>();

        //GSM 短信记录
        private Thread SMSRecordListThread = null;
        private static ObservableCollection<GSMDeviceSMSPhoneNumberRecordClass> SMSRecordList = new ObservableCollection<GSMDeviceSMSPhoneNumberRecordClass>();

        //CDMA邻小区信息
        private Thread NeithCellInfoThread = null;
        private static ObservableCollection<CDMACellNeighParameterClass.NeithCellInfo> CDMANeighCellInfo = new ObservableCollection<CDMACellNeighParameterClass.NeithCellInfo>();
        private static ObservableCollection<CDMACellNeighParameterClass.ItemNeithCellInfo> CDMAItemNeighCellInfo = new ObservableCollection<CDMACellNeighParameterClass.ItemNeithCellInfo>();

        //CDMA 短信列表
        private Thread CDMASMSConfigListThread = null;
        private CDMAConfigSMSMSGClass CDMASMSTmp = new CDMAConfigSMSMSGClass();
        private static ObservableCollection<CDMAConfigSMSMSGClass> CDMASMSList = new ObservableCollection<CDMAConfigSMSMSGClass>();

        //CDMA FAP上报UE主叫信息
        private Thread CDMASMSRecordListThread = null;
        private static ObservableCollection<CDMAUEReportInfoClass> CDMASMSRecordList = new ObservableCollection<CDMAUEReportInfoClass>();

        //GSMV2邻小区信息
        private Thread GSMV2NeithCellInfoThread = null;
        private static ObservableCollection<GSMV2CellNeighParameterClass.NeithCellInfo> GSMV2NeighCellInfo = new ObservableCollection<GSMV2CellNeighParameterClass.NeithCellInfo>();
        private static ObservableCollection<GSMV2CellNeighParameterClass.ItemNeithCellInfo> GSMV2ItemNeighCellInfo = new ObservableCollection<GSMV2CellNeighParameterClass.ItemNeithCellInfo>();

        //GSMV2 短信列表
        private Thread GSMV2SMSConfigListThread = null;
        private GSMV2ConfigSMSMSGClass GSMV2SMSTmp = new GSMV2ConfigSMSMSGClass();
        private static ObservableCollection<GSMV2ConfigSMSMSGClass> GSMV2SMSList = new ObservableCollection<GSMV2ConfigSMSMSGClass>();

        //GSMV2 FAP上报UE主叫信息
        private Thread GSMV2SMSRecordListThread = null;
        private static ObservableCollection<GSMV2UEReportInfoClass> GSMV2SMSRecordList = new ObservableCollection<GSMV2UEReportInfoClass>();

        //IMSI导入列表管理窗口
        private static SubWindow.CDMAIMSIListInputWindow CDMAIMSIListInputWin = null;
        private static ObservableCollection<CDMAIMSIControlInfoClass> IMSILists = new ObservableCollection<CDMAIMSIControlInfoClass>();
        private static ObservableCollection<GSMV2IMSIControlInfoClass> GSMV2IMSILists = new ObservableCollection<GSMV2IMSIControlInfoClass>();
        private List<string> GSMV2SelectIMSIList = new List<string>();

        #endregion

        #region 构造函数
        public DeviceManagerWindow()
        {
            InitializeComponent();

            this.Owner = Application.Current.MainWindow;

            AllWindow.Add("DeviceListWindow", new Uri("NavigatePages/DeviceListWindow.xaml", UriKind.Relative));
            AllWindow.Add("DeviceInfoSettingModel", new Uri("NavigatePages/DeviceInfoSettingModel.xaml", UriKind.Relative));

            if (Parameters.LanguageType.Equals("CN") || Parameters.LanguageType.Equals(""))
            {
                Device_managerLanguageClass = new Language_CN.Device_managerWindow();
            }
            else if (Parameters.LanguageType.Equals("EN"))
            {
                Device_managerLanguageClass = new Language_EN.Device_managerWindow();
            }

            if (CDMAIMSIListInputWin == null)
            {
                CDMAIMSIListInputWin = new SubWindow.CDMAIMSIListInputWindow();
            }

            if (WaitUpDownLoadTimeOut == null)
            {
                WaitUpDownLoadTimeOut = new System.Timers.Timer();
                WaitUpDownLoadTimeOut.Interval = Parameters.UpDonwLoadTimeOutValue * (60 * 1000);  //单位：分钟
                WaitUpDownLoadTimeOut.AutoReset = false;
                WaitUpDownLoadTimeOut.Elapsed += WaitUpDownLoadTimeOut_Elapsed;
            }

            if (NeithCellInfoThread == null)
            {
                NeithCellInfoThread = new Thread(new ThreadStart(ShowNeighCellInfo));
                NeithCellInfoThread.Start();
            }

            if (GSMV2NeithCellInfoThread == null)
            {
                GSMV2NeithCellInfoThread = new Thread(new ThreadStart(ShowGSMV2NeighCellInfo));
                GSMV2NeithCellInfoThread.Start();
            }

            if (GettingSelfHandle == null)
            {
                GettingSelfHandle = new Thread(new ThreadStart(GettingSelfWindowHandle));
                GettingSelfHandle.Start();
            }

            if (PhoneRecordListThread == null)
            {
                PhoneRecordListThread = new Thread(new ThreadStart(PhoneRecordListShow));
                //PhoneRecordListThread.Start();
            }

            if (SMSRecordListThread == null)
            {
                SMSRecordListThread = new Thread(new ThreadStart(SMSRecordListShow));
                //SMSRecordListThread.Start();
            }
            if (GSMV2SMSConfigListThread == null)
            {
                GSMV2SMSConfigListThread = new Thread(new ThreadStart(GSMV2SMSConfigListShow));
                GSMV2SMSConfigListThread.Start();
            }
            if (GSMV2SMSRecordListThread == null)
            {
                GSMV2SMSRecordListThread = new Thread(new ThreadStart(GSMV2SMSRecordListShow));
                //GSMV2SMSRecordListThread.Start();
            }
            if (CDMASMSConfigListThread == null)
            {
                CDMASMSConfigListThread = new Thread(new ThreadStart(CDMASMSConfigListShow));
                CDMASMSConfigListThread.Start();
            }
            if (CDMASMSRecordListThread == null)
            {
                CDMASMSRecordListThread = new Thread(new ThreadStart(CDMASMSRecordListShow));
                CDMASMSRecordListThread.Start();
            }
        }
        #endregion

        /// <summary>
        /// 电话记录显示
        /// </summary>
        private void PhoneRecordListShow()
        {
            while (true)
            {
                Thread.Sleep(3000);
                try
                {
                    for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                    {
                        if (JsonInterFace.APATTributesLists[i].FullName == (JsonInterFace.GSMDeviceParameter.DomainFullPathName + "." + JsonInterFace.GSMDeviceParameter.DeviceName) && JsonInterFace.APATTributesLists[i].Mode == DeviceType.GSM)
                        {
                            for (int j = 0; j < JsonInterFace.APATTributesLists[i].PhoneRecordTab.Rows.Count; j++)
                            {
                                GSMDeviceSMSPhoneNumberRecordClass PhoneReport = new GSMDeviceSMSPhoneNumberRecordClass();
                                PhoneReport.IMSI = JsonInterFace.APATTributesLists[i].PhoneRecordTab.Rows[j]["IMSI"].ToString();
                                PhoneReport.PhoneNumber = JsonInterFace.APATTributesLists[i].PhoneRecordTab.Rows[j]["PhoneNumber"].ToString();
                                PhoneRecordList.Add(PhoneReport);
                            }
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended("通话记录获取异常", ex.Message, ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// 短信记录显示
        /// </summary>
        private void SMSRecordListShow()
        {
            while (true)
            {
                Thread.Sleep(1000);

                try
                {
                    for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                    {
                        if (JsonInterFace.APATTributesLists[i].FullName == (JsonInterFace.GSMDeviceParameter.DomainFullPathName + "." + JsonInterFace.GSMDeviceParameter.DeviceName) && JsonInterFace.APATTributesLists[i].Mode == DeviceType.GSM)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                DataTable SMSRecordTab = JsonInterFace.APATTributesLists[i].SMSRecordTab.Copy();

                                for (int j = 0; j < SMSRecordTab.Rows.Count; j++)
                                {
                                    bool Flag = true;
                                    GSMDeviceSMSPhoneNumberRecordClass SMSReport = new GSMDeviceSMSPhoneNumberRecordClass();
                                    SMSReport.IMSI = SMSRecordTab.Rows[j]["IMSI"].ToString();
                                    SMSReport.PhoneNumber = SMSRecordTab.Rows[j]["PhoneNumber"].ToString();
                                    SMSReport.CodeType = SMSRecordTab.Rows[j]["CodeType"].ToString();
                                    SMSReport.SMSData = SMSRecordTab.Rows[j]["SMSData"].ToString();

                                    //显示
                                    for (int k = 0; k < SMSRecordList.Count; k++)
                                    {
                                        if (SMSRecordList[k].IMSI == SMSReport.IMSI
                                            && SMSRecordList[k].PhoneNumber == SMSReport.PhoneNumber
                                            && SMSRecordList[k].CodeType == SMSReport.CodeType
                                            && SMSRecordList[k].SMSData == SMSReport.SMSData
                                            )
                                        {
                                            Flag = false;
                                            break;
                                        }
                                    }

                                    if (Flag)
                                    {
                                        SMSRecordList.Add(SMSReport);
                                    }

                                    //清缓存
                                    for (int k = 0; k < JsonInterFace.APATTributesLists[i].SMSRecordTab.Rows.Count; k++)
                                    {
                                        if (JsonInterFace.APATTributesLists[i].SMSRecordTab.Rows[k]["IMSI"].ToString() == SMSReport.IMSI
                                            && JsonInterFace.APATTributesLists[i].SMSRecordTab.Rows[k]["PhoneNumber"].ToString() == SMSReport.PhoneNumber
                                            && JsonInterFace.APATTributesLists[i].SMSRecordTab.Rows[k]["CodeType"].ToString() == SMSReport.CodeType
                                            && JsonInterFace.APATTributesLists[i].SMSRecordTab.Rows[k]["SMSData"].ToString() == SMSReport.SMSData
                                            )
                                        {
                                            JsonInterFace.APATTributesLists[i].SMSRecordTab.Rows.RemoveAt(k);
                                            break;
                                        }
                                    }

                                    SMSReport = null;
                                }
                            });

                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended("通话记录获取异常", ex.Message, ex.StackTrace);
                }
            }
        }

        private void GettingSelfWindowHandle()
        {
            //获取窗体句柄
            while (true)
            {
                Dispatcher.Invoke(() =>
                {
                    WindowInteropHelper WinHandelHelper = new WindowInteropHelper(this);
                    Parameters.DeviceManageWinHandle = WinHandelHelper.Handle;
                });

                if (Parameters.DeviceManageWinHandle != IntPtr.Zero)
                {
                    #region ==========================默认配置界面(非属性启动)==========================
                    //初始化非XML信息
                    JsonInterFace.DeviceNoXMLUpload.Enable = "";
                    JsonInterFace.DeviceNoXMLUpload.Type = "";
                    JsonInterFace.DeviceNoXMLUpload.MessageFormat = "";
                    JsonInterFace.DeviceNoXMLUpload.Period = "";
                    JsonInterFace.DeviceNoXMLUpload.NameFormat = "";
                    JsonInterFace.DeviceNoXMLUpload.DataFormat = "";
                    JsonInterFace.DeviceNoXMLUpload.URLorIP = "";
                    JsonInterFace.DeviceNoXMLUpload.AddInfo = "";
                    JsonInterFace.DeviceNoXMLUpload.CommEnable = "";
                    JsonInterFace.DeviceNoXMLUpload.CommIp = "";
                    JsonInterFace.DeviceNoXMLUpload.CommPort = "";
                    JsonInterFace.DeviceNoXMLUpload.EncryptType = "";
                    JsonInterFace.DeviceNoXMLUpload.CacheMax = "";
                    //初始化同步状态信息
                    JsonInterFace.SYNCInfo.Status = "";
                    JsonInterFace.SYNCInfo.Source = "";
                    JsonInterFace.SYNCInfo.Euarfcn = "";
                    JsonInterFace.SYNCInfo.PCI = "";
                    IsOnline = SelfDevicePara.IsOneline;
                    Model = Parameters.ConfigType;
                    if (Parameters.ConfigType == DeviceType.UnknownType)
                    {
                        SettingOnOffLineControl(DeviceType.UnknownType, false, null);
                    }
                    //右键设备管理相关操作
                    else if (new Regex(DeviceType.LTE).Match(Parameters.ConfigType).Success)
                    {
                        //右键设备管理选中
                        if (SelfDevicePara.DomainFullNamePath != null && SelfDevicePara.DeviceName != null)
                        {
                            //选中设备
                            SelfSelected(JsonInterFace.UsrdomainData, SelfDevicePara.DomainFullNamePath, SelfDevicePara.DeviceName);
                        }

                        SettingOnOffLineControl(DeviceType.LTE, true, NodeType.LeafNode.ToString());
                        string[] _DomainFullPathName = SelfDevicePara.DomainFullNamePath.Split(new char[] { '.' });
                        string _DomainName = string.Empty;
                        for (int i = 0; i < _DomainFullPathName.Length - 1; i++)
                        {
                            if (_DomainName == "" || _DomainName == null)
                            {
                                _DomainName = _DomainFullPathName[i];
                            }
                            else
                            {
                                _DomainName += "." + _DomainFullPathName[i];
                            }
                        }
                        JsonInterFace.LteDeviceParameter.DomainFullPathName = _DomainName;
                        JsonInterFace.LteDeviceParameter.Station = _DomainFullPathName[_DomainFullPathName.Length - 2];

                        //获取所选设备通用参数(LTE)
                        if (NetWorkClient.ControllerServer.Connected)
                        {
                            NetWorkClient.ControllerServer.Send(JsonInterFace.GetAPGenParaRequest(JsonInterFace.LteDeviceParameter.DomainFullPathName, JsonInterFace.LteDeviceParameter.DeviceName));
                        }
                    }
                    else if (Parameters.ConfigType == DeviceType.GSM)
                    {
                        //右键设备管理选中
                        if (SelfDevicePara.DomainFullNamePath != null && SelfDevicePara.DeviceName != null)
                        {
                            //选中设备
                            SelfSelected(JsonInterFace.UsrdomainData, SelfDevicePara.DomainFullNamePath, SelfDevicePara.DeviceName);
                        }
                        SettingOnOffLineControl(DeviceType.GSM, true, NodeType.LeafNode.ToString());
                        string[] _DomainFullPathName = SelfDevicePara.DomainFullNamePath.Split(new char[] { '.' });
                        string _DomainName = string.Empty;
                        for (int i = 0; i < _DomainFullPathName.Length - 1; i++)
                        {
                            if (_DomainName == "" || _DomainName == null)
                            {
                                _DomainName = _DomainFullPathName[i];
                            }
                            else
                            {
                                _DomainName += "." + _DomainFullPathName[i];
                            }
                        }
                        JsonInterFace.GSMDeviceParameter.DomainFullPathName = _DomainName;
                    }
                    else if (Parameters.ConfigType == DeviceType.WCDMA)
                    {
                        //右键设备管理选中
                        if (SelfDevicePara.DomainFullNamePath != null && SelfDevicePara.DeviceName != null)
                        {
                            //选中设备
                            SelfSelected(JsonInterFace.UsrdomainData, SelfDevicePara.DomainFullNamePath, SelfDevicePara.DeviceName);
                        }
                        SettingOnOffLineControl(DeviceType.WCDMA, true, NodeType.LeafNode.ToString());
                        string[] _DomainFullPathName = SelfDevicePara.DomainFullNamePath.Split(new char[] { '.' });
                        string _DomainName = string.Empty;
                        for (int i = 0; i < _DomainFullPathName.Length - 1; i++)
                        {
                            if (_DomainName == "" || _DomainName == null)
                            {
                                _DomainName = _DomainFullPathName[i];
                            }
                            else
                            {
                                _DomainName += "." + _DomainFullPathName[i];
                            }
                        }
                        JsonInterFace.WCDMADeviceParameter.DomainFullPathName = _DomainName;

                        if (NetWorkClient.ControllerServer.Connected)
                        {
                            NetWorkClient.ControllerServer.Send(JsonInterFace.GetAPGenParaRequest(JsonInterFace.WCDMADeviceParameter.DomainFullPathName, JsonInterFace.WCDMADeviceParameter.DeviceName));
                        }
                    }
                    else if (Parameters.ConfigType == DeviceType.CDMA)
                    {
                        //右键设备管理选中
                        if (SelfDevicePara.DomainFullNamePath != null && SelfDevicePara.DeviceName != null)
                        {
                            //选中设备
                            SelfSelected(JsonInterFace.UsrdomainData, SelfDevicePara.DomainFullNamePath, SelfDevicePara.DeviceName);
                        }
                        SettingOnOffLineControl(DeviceType.CDMA, true, NodeType.LeafNode.ToString());
                        string[] _DomainFullPathName = SelfDevicePara.DomainFullNamePath.Split(new char[] { '.' });
                        string _DomainName = string.Empty;
                        for (int i = 0; i < _DomainFullPathName.Length - 1; i++)
                        {
                            if (_DomainName == "" || _DomainName == null)
                            {
                                _DomainName = _DomainFullPathName[i];
                            }
                            else
                            {
                                _DomainName += "." + _DomainFullPathName[i];
                            }
                        }
                        JsonInterFace.CDMADeviceParameter.DomainFullPathName = _DomainName;

                        JsonInterFace.ResultMessageList.Clear();
                        JsonInterFace.CDMAConfigSMSMSG.GSMV2ConfigSMSMSGDataTab.Clear();
                        Parameters.ConfigType = "Auto";

                        //小区参数
                        NetWorkClient.ControllerServer.Send(
                                                            JsonInterFace.CDMACellPARAMRequest(
                                                                                                JsonInterFace.CDMADeviceParameter.DomainFullPathName,
                                                                                                JsonInterFace.CDMADeviceParameter.DeviceName,
                                                                                                JsonInterFace.CDMADeviceParameter.IpAddr,
                                                                                                JsonInterFace.CDMADeviceParameter.Port,
                                                                                                JsonInterFace.CDMADeviceParameter.InnerType,
                                                                                                JsonInterFace.CDMADeviceParameter.SN
                                                                                              )
                                                           );

                        //多载波信息
                        NetWorkClient.ControllerServer.Send(
                                                            JsonInterFace.CDMAMultiCarrierQueryRequest(
                                                                                                            JsonInterFace.CDMADeviceParameter.DomainFullPathName,
                                                                                                            JsonInterFace.CDMADeviceParameter.DeviceName,
                                                                                                            JsonInterFace.CDMADeviceParameter.IpAddr,
                                                                                                            JsonInterFace.CDMADeviceParameter.Port,
                                                                                                            JsonInterFace.CDMADeviceParameter.InnerType,
                                                                                                            JsonInterFace.CDMADeviceParameter.SN
                                                                                                        )
                                                           );

                        //IMSI信息
                        NetWorkClient.ControllerServer.Send(
                                                            JsonInterFace.CDMAIMSIQueryRequest(
                                                                                                JsonInterFace.CDMADeviceParameter.DomainFullPathName,
                                                                                                JsonInterFace.CDMADeviceParameter.DeviceName,
                                                                                                JsonInterFace.CDMADeviceParameter.IpAddr,
                                                                                                JsonInterFace.CDMADeviceParameter.Port,
                                                                                                JsonInterFace.CDMADeviceParameter.InnerType,
                                                                                                JsonInterFace.CDMADeviceParameter.SN
                                                                                               )
                                                           );
                    }
                    else if (Parameters.ConfigType == DeviceType.GSMV2)
                    {
                        //右键设备管理选中
                        if (SelfDevicePara.DomainFullNamePath != null && SelfDevicePara.DeviceName != null)
                        {
                            //选中设备
                            SelfSelected(JsonInterFace.UsrdomainData, SelfDevicePara.DomainFullNamePath, SelfDevicePara.DeviceName);
                        }
                        SettingOnOffLineControl(DeviceType.GSMV2, true, NodeType.LeafNode.ToString());
                        string[] _DomainFullPathName = SelfDevicePara.DomainFullNamePath.Split(new char[] { '.' });
                        string _DomainName = string.Empty;
                        for (int i = 0; i < _DomainFullPathName.Length - 1; i++)
                        {
                            if (_DomainName == "" || _DomainName == null)
                            {
                                _DomainName = _DomainFullPathName[i];
                            }
                            else
                            {
                                _DomainName += "." + _DomainFullPathName[i];
                            }
                        }
                        JsonInterFace.GSMV2DeviceParameter.DomainFullPathName = _DomainName;

                        //请求通用参数
                        JsonInterFace.ResultMessageList.Clear();
                        JsonInterFace.GSMV2ConfigSMSMSG.GSMV2ConfigSMSMSGDataTab.Clear();
                        JsonInterFace.GSMV2UEReportInfo.UEReportDataTab.Clear();
                        NetWorkClient.ControllerServer.Send(JsonInterFace.GSMV2CellPARAMRequest(
                                                                                                JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                                JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                                JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                                                                                JsonInterFace.GSMV2DeviceParameter.Port,
                                                                                                JsonInterFace.GSMV2DeviceParameter.InnerType,
                                                                                                JsonInterFace.GSMV2DeviceParameter.SN,
                                                                                                "1"
                                                                                              )
                                                           );
                        NetWorkClient.ControllerServer.Send(JsonInterFace.GSMV2CellPARAMRequest(
                                                                                                JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                                JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                                JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                                                                                JsonInterFace.GSMV2DeviceParameter.Port,
                                                                                                JsonInterFace.GSMV2DeviceParameter.InnerType,
                                                                                                JsonInterFace.GSMV2DeviceParameter.SN,
                                                                                                "0"
                                                                                              )
                                                           );
                    }
                    else if (Parameters.ConfigType == DeviceType.TD_SCDMA)
                    {
                        //右键设备管理选中
                        if (SelfDevicePara.DomainFullNamePath != null && SelfDevicePara.DeviceName != null)
                        {
                            //选中设备
                            SelfSelected(JsonInterFace.UsrdomainData, SelfDevicePara.DomainFullNamePath, SelfDevicePara.DeviceName);
                        }
                        SettingOnOffLineControl(DeviceType.TD_SCDMA, true, NodeType.LeafNode.ToString());
                        string[] _DomainFullPathName = SelfDevicePara.DomainFullNamePath.Split(new char[] { '.' });
                        string _DomainName = string.Empty;
                        for (int i = 0; i < _DomainFullPathName.Length - 1; i++)
                        {
                            if (_DomainName == "" || _DomainName == null)
                            {
                                _DomainName = _DomainFullPathName[i];
                            }
                            else
                            {
                                _DomainName += "." + _DomainFullPathName[i];
                            }
                        }
                        JsonInterFace.TDSDeviceParameter.DomainFullPathName = _DomainName;

                        if (NetWorkClient.ControllerServer.Connected)
                        {
                            NetWorkClient.ControllerServer.Send(JsonInterFace.GetAPGenParaRequest(JsonInterFace.TDSDeviceParameter.DomainFullPathName, JsonInterFace.TDSDeviceParameter.DeviceName));
                        }
                    }
                    #endregion
                    break;
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }

        /// <summary>
        /// 显示GSM白名单库信息 IMSI
        /// </summary>
        private void GSMIMSILibraryRegResponse()
        {
            Dispatcher.Invoke(() =>
            {
                for (int i = 0; i < JsonInterFace.GSMLibyraryRegQuery.LibraryRegIMSITab.Rows.Count; i++)
                {
                    bool Flag = true;
                    for (int j = 0; j < GSMLibyraryRegIMSILists.Count; j++)
                    {
                        if (JsonInterFace.GSMLibyraryRegQuery.LibraryRegIMSITab.Rows[i][0].ToString() == GSMLibyraryRegIMSILists[j].IMSI)
                        {
                            Flag = false;
                            break;
                        }
                    }

                    if (Flag)
                    {
                        GSMLibyraryRegQueryClass IMSILBR = new GSMLibyraryRegQueryClass();

                        IMSILBR.IMSI = JsonInterFace.GSMLibyraryRegQuery.LibraryRegIMSITab.Rows[i][0].ToString();

                        GSMLibyraryRegIMSILists.Add(IMSILBR);
                    }
                }
            });
        }

        /// <summary>
        /// 显示GSM白名单库信息 IMEI
        /// </summary>
        private void GSMIMEILibraryRegResponse()
        {
            Dispatcher.Invoke(() =>
            {
                for (int i = 0; i < JsonInterFace.GSMLibyraryRegQuery.LibraryRegIMEITab.Rows.Count; i++)
                {
                    bool Flag = true;
                    for (int j = 0; j < GSMLibyraryRegIMEILists.Count; j++)
                    {
                        if (JsonInterFace.GSMLibyraryRegQuery.LibraryRegIMEITab.Rows[i][0].ToString() == GSMLibyraryRegIMEILists[j].IMEI)
                        {
                            Flag = false;
                            break;
                        }
                    }

                    if (Flag)
                    {
                        GSMLibyraryRegQueryClass IMEILBR = new GSMLibyraryRegQueryClass();

                        IMEILBR.IMEI = JsonInterFace.GSMLibyraryRegQuery.LibraryRegIMEITab.Rows[i][0].ToString();

                        GSMLibyraryRegIMEILists.Add(IMEILBR);
                    }
                }

            });
        }

        /// <summary>
        /// 显示多PLMN及周期频点列表
        /// </summary>
        private void ShowMorePLMNInfo()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    MorePLMNList.Clear();
                    PeriorFreqList.Clear();
                    WCDMAMorePLMNList.Clear();
                    WCDMAPeriorFreqList.Clear();
                    TDSMorePLMNList.Clear();
                    TDSPeriorFreqList.Clear();

                    lock (JsonInterFace.LteCellNeighParameter.InputLock)
                    {
                        for (int j = 0; j < JsonInterFace.APATTributesLists.Count; j++)
                        {
                            if (JsonInterFace.APATTributesLists[j].FullName == (JsonInterFace.LteDeviceParameter.DomainFullPathName + "." + JsonInterFace.LteDeviceParameter.DeviceName))
                            {
                                for (int i = 0; i < JsonInterFace.APATTributesLists[j].MorePLMNSTab.Rows.Count; i++)
                                {
                                    MorePLMNListClass PLMNS = new MorePLMNListClass();

                                    PLMNS.PLMNS = JsonInterFace.APATTributesLists[j].MorePLMNSTab.Rows[i][0].ToString();

                                    if (PLMNS.PLMNS != "" && PLMNS.PLMNS != null)
                                    {
                                        MorePLMNList.Add(PLMNS);
                                    }
                                }
                                JsonInterFace.APATTributesLists[j].MorePLMNSTab.Rows.Clear();
                            }
                            if (JsonInterFace.APATTributesLists[j].FullName == (JsonInterFace.WCDMADeviceParameter.DomainFullPathName + "." + JsonInterFace.WCDMADeviceParameter.DeviceName))
                            {
                                for (int i = 0; i < JsonInterFace.APATTributesLists[j].MorePLMNSTab.Rows.Count; i++)
                                {
                                    MorePLMNListClass WCDMAPLMNS = new MorePLMNListClass();

                                    WCDMAPLMNS.PLMNS = JsonInterFace.APATTributesLists[j].MorePLMNSTab.Rows[i][0].ToString();

                                    if (WCDMAPLMNS.PLMNS != "" && WCDMAPLMNS.PLMNS != null)
                                    {
                                        WCDMAMorePLMNList.Add(WCDMAPLMNS);
                                    }
                                }
                                JsonInterFace.APATTributesLists[j].MorePLMNSTab.Rows.Clear();
                            }
                            if (JsonInterFace.APATTributesLists[j].FullName == (JsonInterFace.TDSDeviceParameter.DomainFullPathName + "." + JsonInterFace.TDSDeviceParameter.DeviceName))
                            {
                                for (int i = 0; i < JsonInterFace.APATTributesLists[j].MorePLMNSTab.Rows.Count; i++)
                                {
                                    MorePLMNListClass TDSPLMNS = new MorePLMNListClass();

                                    TDSPLMNS.PLMNS = JsonInterFace.APATTributesLists[j].MorePLMNSTab.Rows[i][0].ToString();

                                    if (TDSPLMNS.PLMNS != "" && TDSPLMNS.PLMNS != null)
                                    {
                                        TDSMorePLMNList.Add(TDSPLMNS);
                                    }
                                }
                                JsonInterFace.APATTributesLists[j].MorePLMNSTab.Rows.Clear();
                            }
                        }
                    }


                    lock (JsonInterFace.LteCellNeighParameter.InputLock)
                    {
                        for (int j = 0; j < JsonInterFace.APATTributesLists.Count; j++)
                        {
                            if (JsonInterFace.APATTributesLists[j].FullName == (JsonInterFace.LteDeviceParameter.DomainFullPathName + "." + JsonInterFace.LteDeviceParameter.DeviceName))
                            {
                                JsonInterFace.LteCellNeighParameter.Cycle = JsonInterFace.APATTributesLists[j].Cycle;
                                for (int i = 0; i < JsonInterFace.APATTributesLists[j].PerierFreqTab.Rows.Count; i++)
                                {
                                    PerierFreqListClass PerierFreqList = new PerierFreqListClass();

                                    PerierFreqList.PerierFreq = JsonInterFace.APATTributesLists[j].PerierFreqTab.Rows[i][0].ToString();

                                    if (PerierFreqList.PerierFreq != "" && PerierFreqList.PerierFreq != null)
                                    {
                                        PeriorFreqList.Add(PerierFreqList);
                                    }
                                }
                                JsonInterFace.APATTributesLists[j].PerierFreqTab.Rows.Clear();
                            }
                            if (JsonInterFace.APATTributesLists[j].FullName == (JsonInterFace.WCDMADeviceParameter.DomainFullPathName + "." + JsonInterFace.WCDMADeviceParameter.DeviceName))
                            {
                                JsonInterFace.WCDMACellNeighParameter.Cycle = JsonInterFace.APATTributesLists[j].Cycle;
                                for (int i = 0; i < JsonInterFace.APATTributesLists[j].PerierFreqTab.Rows.Count; i++)
                                {
                                    PerierFreqListClass WCDMAPerierFreqList = new PerierFreqListClass();

                                    WCDMAPerierFreqList.PerierFreq = JsonInterFace.APATTributesLists[j].PerierFreqTab.Rows[i][0].ToString();

                                    if (WCDMAPerierFreqList.PerierFreq != "" && WCDMAPerierFreqList.PerierFreq != null)
                                    {
                                        WCDMAPeriorFreqList.Add(WCDMAPerierFreqList);
                                    }
                                }
                                JsonInterFace.APATTributesLists[j].PerierFreqTab.Rows.Clear();
                            }
                            if (JsonInterFace.APATTributesLists[j].FullName == (JsonInterFace.TDSDeviceParameter.DomainFullPathName + "." + JsonInterFace.TDSDeviceParameter.DeviceName))
                            {
                                JsonInterFace.TDSCellNeighParameter.Cycle = JsonInterFace.APATTributesLists[j].Cycle;
                                for (int i = 0; i < JsonInterFace.APATTributesLists[j].PerierFreqTab.Rows.Count; i++)
                                {
                                    PerierFreqListClass TDSPerierFreqList = new PerierFreqListClass();

                                    TDSPerierFreqList.PerierFreq = JsonInterFace.APATTributesLists[j].PerierFreqTab.Rows[i][0].ToString();

                                    if (TDSPerierFreqList.PerierFreq != "" && TDSPerierFreqList.PerierFreq != null)
                                    {
                                        TDSPeriorFreqList.Add(TDSPerierFreqList);
                                    }
                                }
                                JsonInterFace.APATTributesLists[j].PerierFreqTab.Rows.Clear();
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// CDMA显示邻小区信息
        /// </summary>
        private void ShowNeighCellInfo()
        {
            try
            {
                while (true)
                {
                    if (JsonInterFace.CDMACellNeighParameter.CellInfoTab.Rows.Count > 0)
                    {
                        lock (JsonInterFace.CDMACellNeighParameter.CellInfoInputLock)
                        {
                            for (int i = 0; i < JsonInterFace.CDMACellNeighParameter.CellInfoTab.Rows.Count; i++)
                            {
                                CDMACellNeighParameterClass.NeithCellInfo cellInfo = new CDMACellNeighParameterClass.NeithCellInfo();
                                Dispatcher.Invoke(() =>
                                {
                                    string _ID = string.Empty;
                                    if (CDMANeighCellInfo.Count <= 0)
                                    {
                                        _ID = "1";
                                    }
                                    else
                                    {
                                        _ID = (Convert.ToInt32(CDMANeighCellInfo[CDMANeighCellInfo.Count - 1].ID.ToString()) + 1).ToString();
                                    }
                                    cellInfo.ID = _ID;
                                    cellInfo.BSID = JsonInterFace.CDMACellNeighParameter.CellInfoTab.Rows[i][0].ToString();
                                    cellInfo.BNID = JsonInterFace.CDMACellNeighParameter.CellInfoTab.Rows[i][1].ToString();
                                    cellInfo.BPLMNId = JsonInterFace.CDMACellNeighParameter.CellInfoTab.Rows[i][2].ToString();
                                    cellInfo.CRSRP = JsonInterFace.CDMACellNeighParameter.CellInfoTab.Rows[i][3].ToString();
                                    cellInfo.WTac = JsonInterFace.CDMACellNeighParameter.CellInfoTab.Rows[i][4].ToString();
                                    cellInfo.WPhyCellId = JsonInterFace.CDMACellNeighParameter.CellInfoTab.Rows[i][5].ToString();
                                    cellInfo.WUARFCN = JsonInterFace.CDMACellNeighParameter.CellInfoTab.Rows[i][6].ToString();
                                    cellInfo.CRefTxPower = JsonInterFace.CDMACellNeighParameter.CellInfoTab.Rows[i][7].ToString();
                                    cellInfo.BNbCellNum = JsonInterFace.CDMACellNeighParameter.CellInfoTab.Rows[i][8].ToString();
                                    cellInfo.BC2 = JsonInterFace.CDMACellNeighParameter.CellInfoTab.Rows[i][9].ToString();
                                    cellInfo.Rkey = JsonInterFace.CDMACellNeighParameter.CellInfoTab.Rows[i][10].ToString();
                                    CDMANeighCellInfo.Add(cellInfo);
                                });
                            }
                            JsonInterFace.CDMACellNeighParameter.CellInfoTab.Rows.Clear();
                        }
                    }

                    Thread.Sleep(500);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        #region LTE系统升级进度条控制
        private void WaitUpDownLoadTimeOut_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                JsonInterFace.ProgressBarInfo.RunProgressBar = false;
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    //倒记时停止
                    JsonInterFace.ProgressBarInfo.UpgradeTimer.Stop();

                    //进度线程停止
                    if (ControlUpDownActionInfoThread != null)
                    {
                        if (ControlUpDownActionInfoThread.ThreadState == ThreadState.Running || ControlUpDownActionInfoThread.ThreadState == ThreadState.WaitSleepJoin)
                        {
                            ControlUpDownActionInfoThread.Abort();
                            ControlUpDownActionInfoThread.Join();
                        }
                    }

                    StringBuilder UpdateRequestMsg = new StringBuilder();
                    UpdateRequestMsg.AppendLine("------------ 系统升级结束... [" + DateTime.Now.ToString() + "] -----------");
                    UpdateRequestMsg.AppendLine(">>> 服务器未响应超时[" + JsonInterFace.ProgressBarInfo.UpgradeTimed + "] <<<");
                    UpdateRequestMsg.AppendLine("---------------------------------------------------------------------------------");
                    JsonInterFace.ProgressBarInfo.ResoultMessage += (UpdateRequestMsg.ToString());

                    JsonInterFace.ProgressBarInfo.UpgradeTimed = "00:00:00";
                    JsonInterFace.ProgressBarInfo.StepValue = 0;
                    JsonInterFace.ProgressBarInfo.ProgressBarShow = Visibility.Collapsed;
                    JsonInterFace.ProgressBarInfo.UpdateStart = true;
                }));

                MessageBox.Show("系统升级超时！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("系统升级超时检测", ex.Message, ex.StackTrace);
            }
        }
        #endregion

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
            //请求获取设备列表
            try
            {
                //LTE,WCDMA请求系充升级响应
                if (msg == Parameters.WM_UpgradeAPSystemResponse)
                {
                    UpgradeAPSystemStartting(lParam);
                }
                //LTE扫频配置信息
                else if (msg == Parameters.WM_ScannerFrequencyConfigrationResponse)
                {
                    JsonInterFace.LteDeviceAdvanceSettingParameter.FrequencyList = txtSweepFreqPoint.Text;
                }
                //设备树列表显示
                else if (msg == Parameters.WM_DeviceManageWinTreeViewReLoade)
                {
                    LoadDeviceListTreeView();
                }
                //LTE下载日志
                else if (msg == Parameters.WM_DownloadSystemLogsResponse)
                {

                }
                //LTE获取设备通用参数响应
                else if (msg == Parameters.WM_DeviceManageGenParaResponse)
                {

                }
                //GSM白名单清空
                else if (msg == Parameters.WM_GSMLibraryRegDelAllResponse)
                {
                    GSMLibyraryRegIMSILists.Clear();
                    GSMLibyraryRegIMEILists.Clear();
                }
                //CDMA IMSI Add
                else if (msg == Parameters.WM_CDMAIMSIConfigWithAddResponse)
                {
                    UpdateIMSIList(Parameters.ConfigType);
                }
                //CDMA IMSI Clear
                else if (msg == Parameters.WM_CDMAIMSIConfigWithClearResponse)
                {
                    UpdateIMSIList(Parameters.ConfigType);
                }
                //CDMA IMSI Delete Part
                else if (msg == Parameters.WM_CDMAIMSIConfigWithDeletePartResponse)
                {
                    UpdateIMSIList(Parameters.ConfigType);
                }
                //CDMA IMSI Query
                else if (msg == Parameters.WM_CDMAIMSIConfigWithQueryResponse)
                {
                    UpdateIMSIList(Parameters.ConfigType);
                }
                //多PLMN获取响应、同步参考源查询
                else if (msg == Parameters.WM_GettingOtherPLMNListResponse)
                {
                    ShowMorePLMNInfo();
                    if (new Regex(DeviceType.LTE).Match(Model).Success)
                        ShowSyncinfo();
                    //非XML查询
                    GetUpload();
                    //获取TDS扫频频点
                    if (DeviceType.TD_SCDMA == Model)
                        GetSonEarfcn();
                }
                //GSM短信列表响应
                else if (msg == Parameters.WM_GSMSMSListResponse)
                {
                    GSMSMSListGetting();
                }
                //GSM IMSI白名单查询响应
                else if (msg == Parameters.WM_GSMIMSILibraryRegQueryResponse)
                {
                    GSMIMSILibraryRegResponse();
                }
                //GSM IMEI白名单查询响应
                else if (msg == Parameters.WM_GSMIMEILibraryRegQueryResponse)
                {
                    GSMIMEILibraryRegResponse();
                }
                //下载LTE日志响应消息
                else if (msg == Parameters.WM_DOWNLOAD_LTE_LOG_RESULT_MESSAGE)
                {
                    DownLoadLTELogsActionInfoPrint(lParam);
                }
                //下载WCDMA日志响应消息
                else if (msg == Parameters.WM_DOWNLOAD_WCDMA_LOG_RESULT_MESSAGE)
                {
                    DownLoadWCDMALogsActionInfoPrint(lParam);
                }
                //下载TDS日志响应消息
                else if (msg == Parameters.WM_DOWNLOAD_TDS_LOG_RESULT_MESSAGE)
                {
                    DownLoadTDSLogsActionInfoPrint(lParam);
                }
                //下载AP日志出错消息
                else if (msg == Parameters.WM_DOWNLOAD_AP_LOGS_ERROR_MESSAGE)
                {
                    DownLoadAPLogsErrMessage(lParam);
                }
                //系充升级交互信息
                else if (msg == Parameters.WM_AP_SytemUpgrade_MESSAGE)
                {
                    ApSystemUpgradeMessage(lParam);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("Windows 消息事件：", ex.Message, ex.StackTrace);
            }
            return hwnd;
        }

        /// <summary>
        /// LTE高级设置-->>频点设置
        /// </summary>
        private void FrequencyConfigrationResponse()
        {
            for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
            {
                if (JsonInterFace.APATTributesLists[i].SelfID.Equals(JsonInterFace.LteDeviceAdvanceSettingParameter.SelfID)
                    && JsonInterFace.APATTributesLists[i].ParentID.Equals(JsonInterFace.LteDeviceAdvanceSettingParameter.ParentID))
                {
                    JsonInterFace.APATTributesLists[i].FrequencyList = JsonInterFace.LteDeviceAdvanceSettingParameter.FrequencyList;
                    break;
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //中/英文初始化
                if (Parameters.LanguageType.Equals("CN") || Parameters.LanguageType.Equals(""))
                {
                    this.DataContext = (Language_CN.Device_managerWindow)Device_managerLanguageClass;
                    txtMorePLMNSList.DataContext = (Language_CN.Device_managerWindow)Device_managerLanguageClass;
                    txtPerierFreqList.DataContext = (Language_CN.Device_managerWindow)Device_managerLanguageClass;
                    lblStartTime.DataContext = (Language_CN.Device_managerWindow)Device_managerLanguageClass;
                    lblSecondPeriodTime.DataContext = (Language_CN.Device_managerWindow)Device_managerLanguageClass;
                    lblThreePeriodTime.DataContext = (Language_CN.Device_managerWindow)Device_managerLanguageClass;
                    lblWCDMAStartTime.DataContext = (Language_CN.Device_managerWindow)Device_managerLanguageClass;
                    lblWCDMASecondPeriodTime.DataContext = (Language_CN.Device_managerWindow)Device_managerLanguageClass;
                    lblWCDMAThreePeriodTime.DataContext = (Language_CN.Device_managerWindow)Device_managerLanguageClass;
                    lblCDMAStartTime.DataContext = (Language_CN.Device_managerWindow)Device_managerLanguageClass;
                    lblCDMAThreePeriodTime.DataContext = (Language_CN.Device_managerWindow)Device_managerLanguageClass;
                    lblCDMASecondPeriodTime.DataContext = (Language_CN.Device_managerWindow)Device_managerLanguageClass;
                    lblGSMV2StartTime.DataContext = (Language_CN.Device_managerWindow)Device_managerLanguageClass;
                    lblGSMV2SecondPeriodTime.DataContext = (Language_CN.Device_managerWindow)Device_managerLanguageClass;
                    lblGSMV2ThreePeriodTime.DataContext = (Language_CN.Device_managerWindow)Device_managerLanguageClass;
                }
                else
                {
                    this.DataContext = (Language_EN.Device_managerWindow)Device_managerLanguageClass;
                    txtMorePLMNSList.DataContext = (Language_EN.Device_managerWindow)Device_managerLanguageClass;
                    txtPerierFreqList.DataContext = (Language_EN.Device_managerWindow)Device_managerLanguageClass;
                    lblStartTime.DataContext = (Language_EN.Device_managerWindow)Device_managerLanguageClass;
                    lblSecondPeriodTime.DataContext = (Language_EN.Device_managerWindow)Device_managerLanguageClass;
                    lblThreePeriodTime.DataContext = (Language_EN.Device_managerWindow)Device_managerLanguageClass;
                    lblWCDMAStartTime.DataContext = (Language_EN.Device_managerWindow)Device_managerLanguageClass;
                    lblWCDMASecondPeriodTime.DataContext = (Language_EN.Device_managerWindow)Device_managerLanguageClass;
                    lblWCDMAThreePeriodTime.DataContext = (Language_EN.Device_managerWindow)Device_managerLanguageClass;
                    lblCDMAStartTime.DataContext = (Language_EN.Device_managerWindow)Device_managerLanguageClass;
                    lblCDMAThreePeriodTime.DataContext = (Language_EN.Device_managerWindow)Device_managerLanguageClass;
                    lblCDMASecondPeriodTime.DataContext = (Language_EN.Device_managerWindow)Device_managerLanguageClass;
                    lblGSMV2StartTime.DataContext = (Language_EN.Device_managerWindow)Device_managerLanguageClass;
                    lblGSMV2SecondPeriodTime.DataContext = (Language_EN.Device_managerWindow)Device_managerLanguageClass;
                    lblGSMV2ThreePeriodTime.DataContext = (Language_EN.Device_managerWindow)Device_managerLanguageClass;
                }

                dgGSMV2IMSIList.ItemsSource = GSMV2IMSILists;
                #region LTE 功能模块
                //设备信息
                txtDomainName.DataContext = JsonInterFace.LteDeviceParameter;
                txtStation.DataContext = JsonInterFace.LteDeviceParameter;
                txtDeviceName.DataContext = JsonInterFace.LteDeviceParameter;
                cbxDeviceMode.DataContext = JsonInterFace.LteDeviceParameter;
                rbStaticIPMode.DataContext = JsonInterFace.LteDeviceParameter;
                rbDynamicIPMode.DataContext = JsonInterFace.LteDeviceParameter;
                txtIPAddr.DataContext = JsonInterFace.LteDeviceParameter;
                txtPort.DataContext = JsonInterFace.LteDeviceParameter;
                txtNetMask.DataContext = JsonInterFace.LteDeviceParameter;
                txtSN.DataContext = JsonInterFace.LteDeviceParameter;
                cbxDeviceIdentificationMode.DataContext = JsonInterFace.LteDeviceParameter;

                //小区信息
                txtPLMN.DataContext = JsonInterFace.LteCellNeighParameter;
                txtFreqPoint.DataContext = JsonInterFace.LteCellNeighParameter;
                txtBandWidth.DataContext = JsonInterFace.LteCellNeighParameter;
                txtPowerDown.DataContext = JsonInterFace.LteCellNeighParameter;
                txtOperatorName.DataContext = JsonInterFace.LteCellNeighParameter;
                txtDisturbCode.DataContext = JsonInterFace.LteCellNeighParameter;
                txtTACorLAC.DataContext = JsonInterFace.LteCellNeighParameter;
                txtCycle.DataContext = JsonInterFace.LteCellNeighParameter;
                MorePLMNSListDataGrid.ItemsSource = MorePLMNList;
                PeriodFreqDataGrid.ItemsSource = PeriorFreqList;
                txtPeriodFreq.DataContext = JsonInterFace.LteCellNeighParameter;
                txtCellID.DataContext = JsonInterFace.LteCellNeighParameter;

                //同步状态信息
                txtLTEStatus.DataContext = JsonInterFace.SYNCInfo;
                txtLTESource.DataContext = JsonInterFace.SYNCInfo;
                txtLTEEuarfcn.DataContext = JsonInterFace.SYNCInfo;
                txtLTEPCI.DataContext = JsonInterFace.SYNCInfo;

                //工作模式
                rdbFreqAuto.DataContext = JsonInterFace.LteSetWorkModeParameter;
                rdbFreqManul.DataContext = JsonInterFace.LteSetWorkModeParameter;
                rdbRebootModeAuto.DataContext = JsonInterFace.LteSetWorkModeParameter;
                rdbRebootModeManul.DataContext = JsonInterFace.LteSetWorkModeParameter;

                //高级设置
                txtSweepFreqPoint.DataContext = JsonInterFace.LteDeviceAdvanceSettingParameter;
                rbConfigure.DataContext = JsonInterFace.LteDeviceAdvanceSettingParameter;
                rbUnConfigure.DataContext = JsonInterFace.LteDeviceAdvanceSettingParameter;
                txtFreqOffsetSetting.DataContext = JsonInterFace.LteDeviceAdvanceSettingParameter;
                txtNTP.DataContext = JsonInterFace.LteDeviceAdvanceSettingParameter;
                txtPriority.DataContext = JsonInterFace.LteDeviceAdvanceSettingParameter;
                rbGPS.DataContext = JsonInterFace.LteDeviceAdvanceSettingParameter;
                rbEmptyMouth.DataContext = JsonInterFace.LteDeviceAdvanceSettingParameter;
                rbYes.DataContext = JsonInterFace.LteDeviceAdvanceSettingParameter;
                rbNo.DataContext = JsonInterFace.LteDeviceAdvanceSettingParameter;
                txtAppointNeighList.DataContext = JsonInterFace.LteDeviceAdvanceSettingParameter;
                txtAppointPci.DataContext = JsonInterFace.LteDeviceAdvanceSettingParameter;
                txtAppointBandWidth.DataContext = JsonInterFace.LteDeviceAdvanceSettingParameter;
                txtFirstPeriodTimeStart.DataContext = JsonInterFace.LteDeviceAdvanceSettingParameter;
                txtFirstPeriodTimeEnd.DataContext = JsonInterFace.LteDeviceAdvanceSettingParameter;
                txtSecondPeriodTimeStart.DataContext = JsonInterFace.LteDeviceAdvanceSettingParameter;
                txtSecoondPeriodTimeEnd.DataContext = JsonInterFace.LteDeviceAdvanceSettingParameter;
                txtThreePeriodTimeStart.DataContext = JsonInterFace.LteDeviceAdvanceSettingParameter;
                txtThreePeriodTimeEnd.DataContext = JsonInterFace.LteDeviceAdvanceSettingParameter;
                cbScanEnable.DataContext = JsonInterFace.LteDeviceAdvanceSettingParameter;

                //系统维护
                txtupgradeFile.DataContext = JsonInterFace.LteDeviceSystemMaintenenceParameter;
                txtLogFilePath.DataContext = JsonInterFace.LteDeviceSystemMaintenenceParameter;
                txtVersion.DataContext = JsonInterFace.LteDeviceSystemMaintenenceParameter;
                txtUpdateLogsShow.DataContext = JsonInterFace.ProgressBarInfo;
                lblSystemUpgrading.DataContext = JsonInterFace.ProgressBarInfo;
                pgbUpdateProgressBar.DataContext = JsonInterFace.ProgressBarInfo;
                lblUpgradeTimed.DataContext = JsonInterFace.ProgressBarInfo;
                lblUpgradeTimedCaption.DataContext = JsonInterFace.ProgressBarInfo;

                //系统维护参数初始化
                JsonInterFace.LteDeviceSystemMaintenenceParameter.UpgradeFile = string.Empty;
                JsonInterFace.LteDeviceSystemMaintenenceParameter.FileVertion = string.Empty;
                JsonInterFace.LteDeviceSystemMaintenenceParameter.LogFiles = string.Empty;
                JsonInterFace.LteDeviceSystemMaintenenceParameter.DeviceLists.Clear();

                //工程设置
                cbParameterKeyName.DataContext = JsonInterFace.LteDeviceObjectSettingParameter;
                cbParameterValue.DataContext = JsonInterFace.LteDeviceObjectSettingParameter;
                txtparameterCommandList.DataContext = JsonInterFace.LteDeviceObjectSettingParameter;
                txtparameterResultValueList.DataContext = JsonInterFace.LteDeviceObjectSettingParameter;

                //非XML设置
                cbLTEEnable.DataContext = JsonInterFace.DeviceNoXMLUpload;
                cbLTECommEnable.DataContext = JsonInterFace.DeviceNoXMLUpload;
                cbLTEMessageFormat.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtLTECommIp.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtLTECommPort.DataContext = JsonInterFace.DeviceNoXMLUpload;
                cbLTEType.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtLTEPeriod.DataContext = JsonInterFace.DeviceNoXMLUpload;
                cbLTEEncryptType.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtLTECacheMax.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtLTEDataFormat.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtLTENameFormat.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtLTEURLorIP.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtLTEAddInfo.DataContext = JsonInterFace.DeviceNoXMLUpload;
                #endregion

                #region GSM功能模块
                txtGSMDomainName.DataContext = JsonInterFace.GSMDeviceParameter;
                txtGSMStation.DataContext = JsonInterFace.GSMDeviceParameter;
                txtGSMDeviceName.DataContext = JsonInterFace.GSMDeviceParameter;
                cbxGSMDeviceMode.DataContext = JsonInterFace.GSMDeviceParameter;
                rbGSMStaticIPMode.DataContext = JsonInterFace.GSMDeviceParameter;
                rbGSMDynamicIPMode.DataContext = JsonInterFace.GSMDeviceParameter;
                txtGSMIPAddr.DataContext = JsonInterFace.GSMDeviceParameter;
                txtGSMPort.DataContext = JsonInterFace.GSMDeviceParameter;
                txtGSMNetMask.DataContext = JsonInterFace.GSMDeviceParameter;
                txtGSMSN.DataContext = JsonInterFace.GSMDeviceParameter;
                cbxGSMDeviceIdentificationMode.DataContext = JsonInterFace.GSMDeviceParameter;

                //高级设置
                //载波
                chkGSMCarrierOne.DataContext = JsonInterFace.GSMCarrierParameter;
                chkGSMCarrierTwo.DataContext = JsonInterFace.GSMCarrierParameter;

                //系统参数
                txtGSMMcc.DataContext = JsonInterFace.GSMSystemParameter;
                txtGSMMnc.DataContext = JsonInterFace.GSMSystemParameter;
                txtGSMBsic.DataContext = JsonInterFace.GSMSystemParameter;
                txtGSMLac.DataContext = JsonInterFace.GSMSystemParameter;
                txtGSMCellID.DataContext = JsonInterFace.GSMSystemParameter;
                txtGSMC2.DataContext = JsonInterFace.GSMSystemParameter;
                txtGSMPeri.DataContext = JsonInterFace.GSMSystemParameter;

                //系统选项
                cbbGSMopLuSms.DataContext = JsonInterFace.GSMSystemOptionParameter;
                cbbGSMopLuImei.DataContext = JsonInterFace.GSMSystemOptionParameter;
                cbbGSMopCallEn.DataContext = JsonInterFace.GSMSystemOptionParameter;
                cbbGSMLuType.DataContext = JsonInterFace.GSMSystemOptionParameter;
                cbbGSMSmsType.DataContext = JsonInterFace.GSMSystemOptionParameter;

                //射频参数
                txtrfEnable.DataContext = JsonInterFace.GSMRadioFrequencyParameter;
                txtrfFreq.DataContext = JsonInterFace.GSMRadioFrequencyParameter;
                txtrfPwr.DataContext = JsonInterFace.GSMRadioFrequencyParameter;

                //注册工作模式
                cbbWorkMode.DataContext = JsonInterFace.GSMRegModeParameter;

                //白名单设置
                GSMIMSIInputBar.DataContext = JsonInterFace.GSMLibyraryRegAdd;
                btnGSMIMSIAdd.DataContext = JsonInterFace.GSMLibyraryRegAdd;
                btnGSMIMSIInput.DataContext = JsonInterFace.GSMLibyraryRegAdd;

                //短信息参数
                GSMSMSMessageDataGrid.ItemsSource = GSMSMSList;
                chkGSMSMSCycleAction.DataContext = JsonInterFace.GSMSMSParameter;
                txtGSMCycleTime.DataContext = JsonInterFace.GSMSMSParameter;
                chkGSMMessageCarrierOne.DataContext = JsonInterFace.GSMSMSParameter;
                chkGSMMessageCarrierTwo.DataContext = JsonInterFace.GSMSMSParameter;
                txtGSMMessageContent.DataContext = JsonInterFace.GSMSMSParameter;
                txtGSMSMSC.DataContext = JsonInterFace.GSMSMSParameter;
                txtGSMSendNumber.DataContext = JsonInterFace.GSMSMSParameter;
                txtGSMSMSCoding.DataContext = JsonInterFace.GSMSMSParameter;
                txtFilterDelayTime.DataContext = JsonInterFace.GSMSMSParameter;
                cbbGSMFilterSMS.DataContext = JsonInterFace.GSMSMSParameter;
                cbbGSMAutoSender.DataContext = JsonInterFace.GSMSMSParameter;

                //通话记录
                CallRecordGrid.ItemsSource = PhoneRecordList;

                //短信息记录
                SMSRecordGrid.ItemsSource = SMSRecordList;

                //系统维护
                txtGSMupgradeFile.DataContext = null;
                txtGSMLogFilePath.DataContext = null;
                txtGSMVersion.DataContext = null;
                txtGSMUpdateLogsShow.DataContext = null;
                pgbGSMUpdateProgressBar.DataContext = null;

                //工程设置
                txtGSMparameterCommandList.DataContext = JsonInterFace.GSMDeviceObjectSettingParameter;
                txtGSMparameterResultValueList.DataContext = JsonInterFace.GSMDeviceObjectSettingParameter;

                //时段时控
                txtGSMFirstPeriodTimeStart.DataContext = JsonInterFace.GSMDeviceAdvanceSettingParameter;
                txtGSMFirstPeriodTimeEnd.DataContext = JsonInterFace.GSMDeviceAdvanceSettingParameter;
                txtGSMSecondPeriodTimeStart.DataContext = JsonInterFace.GSMDeviceAdvanceSettingParameter;
                txtGSMSecondPeriodTimeEnd.DataContext = JsonInterFace.GSMDeviceAdvanceSettingParameter;
                txtGSMThreePeriodTimeStart.DataContext = JsonInterFace.GSMDeviceAdvanceSettingParameter;
                txtGSMThreePeriodTimeEnd.DataContext = JsonInterFace.GSMDeviceAdvanceSettingParameter;

                //非XML设置
                cbGSMEnable.DataContext = JsonInterFace.DeviceNoXMLUpload;
                cbGSMCommEnable.DataContext = JsonInterFace.DeviceNoXMLUpload;
                cbGSMMessageFormat.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtGSMCommIp.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtGSMCommPort.DataContext = JsonInterFace.DeviceNoXMLUpload;
                cbGSMType.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtGSMPeriod.DataContext = JsonInterFace.DeviceNoXMLUpload;
                cbGSMEncryptType.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtGSMCacheMax.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtGSMDataFormat.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtGSMNameFormat.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtGSMURLorIP.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtGSMAddInfo.DataContext = JsonInterFace.DeviceNoXMLUpload;
                #endregion

                #region WCDMA 功能模块
                //设备信息
                txtWCDMADomainName.DataContext = JsonInterFace.WCDMADeviceParameter;
                txtWCDMAStation.DataContext = JsonInterFace.WCDMADeviceParameter;
                txtWCDMADeviceName.DataContext = JsonInterFace.WCDMADeviceParameter;
                cbxWCDMADeviceMode.DataContext = JsonInterFace.WCDMADeviceParameter;
                rbWCDMAStaticIPMode.DataContext = JsonInterFace.WCDMADeviceParameter;
                rbWCDMADynamicIPMode.DataContext = JsonInterFace.WCDMADeviceParameter;
                txtWCDMAIPAddr.DataContext = JsonInterFace.WCDMADeviceParameter;
                txtWCDMAPort.DataContext = JsonInterFace.WCDMADeviceParameter;
                txtWCDMANetMask.DataContext = JsonInterFace.WCDMADeviceParameter;
                txtWCDMASN.DataContext = JsonInterFace.WCDMADeviceParameter;
                cbxWCDMADeviceIdentificationMode.DataContext = JsonInterFace.WCDMADeviceParameter;

                //小区信息
                txtWCDMAPLMN.DataContext = JsonInterFace.WCDMACellNeighParameter;
                txtWCDMAFreqPoint.DataContext = JsonInterFace.WCDMACellNeighParameter;
                txtWCDMAPowerDown.DataContext = JsonInterFace.WCDMACellNeighParameter;
                txtWCDMAOperatorName.DataContext = JsonInterFace.WCDMACellNeighParameter;
                txtWCDMADisturbCode.DataContext = JsonInterFace.WCDMACellNeighParameter;
                txtWCDMATACorLAC.DataContext = JsonInterFace.WCDMACellNeighParameter;
                txtWCDMACycle.DataContext = JsonInterFace.WCDMACellNeighParameter;
                WCDMAMorePLMNSListDataGrid.ItemsSource = WCDMAMorePLMNList;
                WCDMAPeriodFreqDataGrid.ItemsSource = WCDMAPeriorFreqList;
                txtWCDMAPeriodFreq.DataContext = JsonInterFace.WCDMACellNeighParameter;

                //同步状态信息
                txtWCDMAStatus.DataContext = JsonInterFace.SYNCInfo;
                txtWCDMASource.DataContext = JsonInterFace.SYNCInfo;
                txtWCDMAEuarfcn.DataContext = JsonInterFace.SYNCInfo;
                txtWCDMAPCI.DataContext = JsonInterFace.SYNCInfo;

                //工作模式
                rdbWCDMARebootModeAuto.DataContext = JsonInterFace.WCDMASetWorkModeParameter;
                rdbWCDMARebootModeManul.DataContext = JsonInterFace.WCDMASetWorkModeParameter;

                //高级设置
                rbWCDMAConfigure.DataContext = JsonInterFace.WCDMADeviceAdvanceSettingParameter;
                rbWCDMAUnConfigure.DataContext = JsonInterFace.WCDMADeviceAdvanceSettingParameter;
                txtWCDMAFreqOffsetSetting.DataContext = JsonInterFace.WCDMADeviceAdvanceSettingParameter;
                txtWCDMANTP.DataContext = JsonInterFace.WCDMADeviceAdvanceSettingParameter;
                txtWCDMAPriority.DataContext = JsonInterFace.WCDMADeviceAdvanceSettingParameter;
                txtWCDMAFirstPeriodTimeStart.DataContext = JsonInterFace.WCDMADeviceAdvanceSettingParameter;
                txtWCDMAFirstPeriodTimeEnd.DataContext = JsonInterFace.WCDMADeviceAdvanceSettingParameter;
                txtWCDMASecondPeriodTimeStart.DataContext = JsonInterFace.WCDMADeviceAdvanceSettingParameter;
                txtWCDMASecoondPeriodTimeEnd.DataContext = JsonInterFace.WCDMADeviceAdvanceSettingParameter;
                txtWCDMAThreePeriodTimeStart.DataContext = JsonInterFace.WCDMADeviceAdvanceSettingParameter;
                txtWCDMAThreePeriodTimeEnd.DataContext = JsonInterFace.WCDMADeviceAdvanceSettingParameter;

                //系统维护
                txtWCDMAupgradeFile.DataContext = JsonInterFace.WCDMADeviceSystemMaintenenceParameter;
                txtWCDMALogFilePath.DataContext = JsonInterFace.WCDMADeviceSystemMaintenenceParameter;
                txtWCDMAVersion.DataContext = JsonInterFace.WCDMADeviceSystemMaintenenceParameter;
                txtWCDMAUpdateLogsShow.DataContext = JsonInterFace.ProgressBarInfo;
                pgbWCDMAUpdateProgressBar.DataContext = JsonInterFace.ProgressBarInfo;
                lblWCDMAUpgradeTimedCaption.DataContext = JsonInterFace.ProgressBarInfo;
                lblWCDMAUpgradeTimed.DataContext = JsonInterFace.ProgressBarInfo;

                //工程设置
                txtWCDMAparameterCommandList.DataContext = JsonInterFace.WCDMADeviceObjectSettingParameter;
                txtWCDMAparameterResultValueList.DataContext = JsonInterFace.WCDMADeviceObjectSettingParameter;

                //非XML设置
                cbWCDMAEnable.DataContext = JsonInterFace.DeviceNoXMLUpload;
                cbWCDMACommEnable.DataContext = JsonInterFace.DeviceNoXMLUpload;
                cbWCDMAMessageFormat.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtWCDMACommIp.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtWCDMACommPort.DataContext = JsonInterFace.DeviceNoXMLUpload;
                cbWCDMAType.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtWCDMAPeriod.DataContext = JsonInterFace.DeviceNoXMLUpload;
                cbWCDMAEncryptType.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtWCDMACacheMax.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtWCDMADataFormat.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtWCDMANameFormat.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtWCDMAURLorIP.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtWCDMAAddInfo.DataContext = JsonInterFace.DeviceNoXMLUpload;
                #endregion

                #region GSMV2功能模块
                txtGSMV2DomainName.DataContext = JsonInterFace.GSMV2DeviceParameter;
                txtGSMV2Station.DataContext = JsonInterFace.GSMV2DeviceParameter;
                txtGSMV2DeviceName.DataContext = JsonInterFace.GSMV2DeviceParameter;
                cbxGSMV2DeviceMode.DataContext = JsonInterFace.GSMV2DeviceParameter;
                rbGSMV2StaticIPMode.DataContext = JsonInterFace.GSMV2DeviceParameter;
                rbGSMV2DynamicIPMode.DataContext = JsonInterFace.GSMV2DeviceParameter;
                txtGSMV2IPAddr.DataContext = JsonInterFace.GSMV2DeviceParameter;
                txtGSMV2Port.DataContext = JsonInterFace.GSMV2DeviceParameter;
                txtGSMV2NetMask.DataContext = JsonInterFace.GSMV2DeviceParameter;
                txtGSMV2SN.DataContext = JsonInterFace.GSMV2DeviceParameter;
                cbxGSMV2DeviceIdentificationMode.DataContext = JsonInterFace.GSMV2DeviceParameter;

                //小区信息
                //载波
                chkGSMV2CarrierOne.DataContext = JsonInterFace.GSMV2CarrierParameter;
                chkGSMV2CarrierTwo.DataContext = JsonInterFace.GSMV2CarrierParameter;

                txtGSMV2bPLMNId.DataContext = JsonInterFace.GSMV2CellNeighParameter;
                txtGSMV2OperatorName.DataContext = JsonInterFace.GSMV2CellNeighParameter;
                txtGSMV2bTxPower.DataContext = JsonInterFace.GSMV2CellNeighParameter;
                txtGSMV2bRxGain.DataContext = JsonInterFace.GSMV2CellNeighParameter;
                txtGSMV2wPhyCellId.DataContext = JsonInterFace.GSMV2CellNeighParameter;
                txtGSMV2wLAC.DataContext = JsonInterFace.GSMV2CellNeighParameter;
                txtGSMV2dwCellId.DataContext = JsonInterFace.GSMV2CellNeighParameter;
                txtGSMV2bWorkingMode.DataContext = JsonInterFace.GSMV2CellNeighParameter;
                txtGSMV2wUARFCN.DataContext = JsonInterFace.GSMV2CellNeighParameter;
                txtGSMV2dwDateTime.DataContext = JsonInterFace.GSMV2CellNeighParameter;
                rdbGSMV2Yes.DataContext = JsonInterFace.GSMV2CellNeighParameter;
                rdbGSMV2No.DataContext = JsonInterFace.GSMV2CellNeighParameter;

                //同步状态信息
                txtGSMV2Status.DataContext = JsonInterFace.SYNCInfo;
                txtGSMV2Source.DataContext = JsonInterFace.SYNCInfo;
                txtGSMV2Euarfcn.DataContext = JsonInterFace.SYNCInfo;
                txtGSMV2PCI.DataContext = JsonInterFace.SYNCInfo;

                //高级设置
                chkGSMV2ActiveTimeCarrierOne.DataContext = JsonInterFace.GSMV2CarrierParameter;
                chkGSMV2ActiveTimeCarrierTwo.DataContext = JsonInterFace.GSMV2CarrierParameter;

                txtGSMV2FirstPeriodTimeStart.DataContext = JsonInterFace.GSMV2DeviceAdvanceSettingParameter;
                txtGSMV2FirstPeriodTimeEnd.DataContext = JsonInterFace.GSMV2DeviceAdvanceSettingParameter;
                txtGSMV2SecondPeriodTimeStart.DataContext = JsonInterFace.GSMV2DeviceAdvanceSettingParameter;
                txtGSMV2SecoondPeriodTimeEnd.DataContext = JsonInterFace.GSMV2DeviceAdvanceSettingParameter;
                txtGSMV2ThreePeriodTimeStart.DataContext = JsonInterFace.GSMV2DeviceAdvanceSettingParameter;
                txtGSMV2ThreePeriodTimeEnd.DataContext = JsonInterFace.GSMV2DeviceAdvanceSettingParameter;

                //短信息记录
                cbGSMV2SMSCarrierOne.DataContext = JsonInterFace.GSMV2ConfigSMSMSG;
                cbGSMV2SMSCarrierTwo.DataContext = JsonInterFace.GSMV2ConfigSMSMSG;
                txtGSMV2MessageContent.DataContext = JsonInterFace.GSMV2ConfigSMSMSG;
                txtGSMV2SMSC.DataContext = JsonInterFace.GSMV2ConfigSMSMSG;
                txtGSMV2SMSI.DataContext = JsonInterFace.GSMV2ConfigSMSMSG;

                cbGSMV2SMSMSGCarrierOne.DataContext = JsonInterFace.GSMV2CarrierParameter;
                cbGSMV2SMSMSGCarrierTwo.DataContext = JsonInterFace.GSMV2CarrierParameter;
                GSMV2SMSMessageDataGrid.ItemsSource = GSMV2SMSList;
                //GSMV2SMSRecordGrid.ItemsSource = GSMV2SMSRecordList;

                //系统维护
                txtGSMV2upgradeFile.DataContext = null;
                txtGSMV2LogFilePath.DataContext = null;
                txtGSMV2Version.DataContext = null;
                txtGSMV2UpdateLogsShow.DataContext = null;
                pgbGSMV2UpdateProgressBar.DataContext = null;

                //工程设置
                txtGSMV2parameterCommandList.DataContext = JsonInterFace.GSMDeviceObjectSettingParameter;
                txtGSMV2parameterResultValueList.DataContext = JsonInterFace.GSMDeviceObjectSettingParameter;

                //非XML设置
                cbGSMV2Enable.DataContext = JsonInterFace.DeviceNoXMLUpload;
                cbGSMV2CommEnable.DataContext = JsonInterFace.DeviceNoXMLUpload;
                cbGSMV2MessageFormat.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtGSMV2CommIp.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtGSMV2CommPort.DataContext = JsonInterFace.DeviceNoXMLUpload;
                cbGSMV2Type.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtGSMV2Period.DataContext = JsonInterFace.DeviceNoXMLUpload;
                cbGSMV2EncryptType.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtGSMV2CacheMax.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtGSMV2DataFormat.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtGSMV2NameFormat.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtGSMV2URLorIP.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtGSMV2AddInfo.DataContext = JsonInterFace.DeviceNoXMLUpload;
                #endregion

                #region CDMA功能模块
                //设备信息
                txtCDMADomainName.DataContext = JsonInterFace.CDMADeviceParameter;
                txtCDMAStation.DataContext = JsonInterFace.CDMADeviceParameter;
                txtCDMADeviceName.DataContext = JsonInterFace.CDMADeviceParameter;
                cbxCDMADeviceMode.DataContext = JsonInterFace.CDMADeviceParameter;
                rbCDMAStaticIPMode.DataContext = JsonInterFace.LteDeviceParameter;
                rbCDMADynamicIPMode.DataContext = JsonInterFace.CDMADeviceParameter;
                txtCDMAIPAddr.DataContext = JsonInterFace.CDMADeviceParameter;
                txtCDMAPort.DataContext = JsonInterFace.CDMADeviceParameter;
                txtCDMANetMask.DataContext = JsonInterFace.CDMADeviceParameter;
                txtCDMASN.DataContext = JsonInterFace.CDMADeviceParameter;
                cbxCDMADeviceIdentificationMode.DataContext = JsonInterFace.CDMADeviceParameter;

                //小区参数信息
                txtCDMAbPLMNId.DataContext = JsonInterFace.CDMACellNeighParameter;
                txtCDMAOperatorName.DataContext = JsonInterFace.CDMACellNeighParameter;
                txtCDMAbTxPower.DataContext = JsonInterFace.CDMACellNeighParameter;
                txtCDMAbRxGain.DataContext = JsonInterFace.CDMACellNeighParameter;
                txtCDMAwPhyCellId.DataContext = JsonInterFace.CDMACellNeighParameter;
                txtCDMAwLAC.DataContext = JsonInterFace.CDMACellNeighParameter;
                txtCDMAwUARFCN.DataContext = JsonInterFace.CDMACellNeighParameter;
                txtCDMASID.DataContext = JsonInterFace.CDMACellNeighParameter;
                txtCDMANID.DataContext = JsonInterFace.CDMACellNeighParameter;
                txtCDMAbWorkingMode.DataContext = JsonInterFace.CDMACellNeighParameter;
                txtCDMAwRedirectCellUarfcn.DataContext = JsonInterFace.CDMACellNeighParameter;
                txtCDMAdwDateTime.DataContext = JsonInterFace.CDMACellNeighParameter;
                rdbCDMABCYes.DataContext = JsonInterFace.CDMACellNeighParameter;
                rdbCDMABCNo.DataContext = JsonInterFace.CDMACellNeighParameter;

                //同步状态信息
                txtCDMAStatus.DataContext = JsonInterFace.SYNCInfo;
                txtCDMASource.DataContext = JsonInterFace.SYNCInfo;
                txtCDMAEuarfcn.DataContext = JsonInterFace.SYNCInfo;
                txtCDMAPCI.DataContext = JsonInterFace.SYNCInfo;

                //高级设置 --> 多载波参数
                cbbCDMAFreqListOne.DataContext = JsonInterFace.CDMAMultiCarrierParameter;
                rdbCDMAFreqModeScannerOne.DataContext = JsonInterFace.CDMAMultiCarrierParameter.BARFCN1Mode;
                rdbCDMAFreqModeNormallyOpenOne.DataContext = JsonInterFace.CDMAMultiCarrierParameter.BARFCN1Mode;
                rdbCDMAFreqModeClosedOne.DataContext = JsonInterFace.CDMAMultiCarrierParameter.BARFCN1Mode;
                txtCDMAScannerTimeOne.DataContext = JsonInterFace.CDMAMultiCarrierParameter;
                txtCDMAScannerSpaceOne.DataContext = JsonInterFace.CDMAMultiCarrierParameter;

                cbbCDMAFreqListTwo.DataContext = JsonInterFace.CDMAMultiCarrierParameter;
                rdbCDMAFreqModeScannerTwo.DataContext = JsonInterFace.CDMAMultiCarrierParameter.BARFCN2Mode;
                rdbCDMAFreqModeNormallyOpenTwo.DataContext = JsonInterFace.CDMAMultiCarrierParameter.BARFCN2Mode;
                rdbCDMAFreqModeClosedTwo.DataContext = JsonInterFace.CDMAMultiCarrierParameter.BARFCN2Mode;
                txtCDMAScannerTimeTwo.DataContext = JsonInterFace.CDMAMultiCarrierParameter;
                txtCDMAScannerSpaceTwo.DataContext = JsonInterFace.CDMAMultiCarrierParameter;

                cbbCDMAFreqListThree.DataContext = JsonInterFace.CDMAMultiCarrierParameter;
                rdbCDMAFreqModeScannerThree.DataContext = JsonInterFace.CDMAMultiCarrierParameter.BARFCN3Mode;
                rdbCDMAFreqModeNormallyOpenThree.DataContext = JsonInterFace.CDMAMultiCarrierParameter.BARFCN3Mode;
                rdbCDMAFreqModeClosedThree.DataContext = JsonInterFace.CDMAMultiCarrierParameter.BARFCN3Mode;
                txtCDMAScannerTimeThree.DataContext = JsonInterFace.CDMAMultiCarrierParameter;
                txtCDMAScannerSpaceThree.DataContext = JsonInterFace.CDMAMultiCarrierParameter;

                cbbCDMAFreqListFour.DataContext = JsonInterFace.CDMAMultiCarrierParameter;
                rdbCDMAFreqModeScannerFour.DataContext = JsonInterFace.CDMAMultiCarrierParameter.BARFCN4Mode;
                rdbCDMAFreqModeNormallyOpenFour.DataContext = JsonInterFace.CDMAMultiCarrierParameter.BARFCN4Mode;
                rdbCDMAFreqModeClosedFour.DataContext = JsonInterFace.CDMAMultiCarrierParameter.BARFCN4Mode;
                txtCDMAScannerTimeFour.DataContext = JsonInterFace.CDMAMultiCarrierParameter;
                txtCDMAScannerSpaceFour.DataContext = JsonInterFace.CDMAMultiCarrierParameter;

                //高级设置 --> 时间控制
                txtCDMAFirstPeriodTimeStart.DataContext = JsonInterFace.CDMADeviceAdvanceSettingParameter;
                txtCDMAFirstPeriodTimeEnd.DataContext = JsonInterFace.CDMADeviceAdvanceSettingParameter;
                txtCDMASecondPeriodTimeStart.DataContext = JsonInterFace.CDMADeviceAdvanceSettingParameter;
                txtCDMASecoondPeriodTimeEnd.DataContext = JsonInterFace.CDMADeviceAdvanceSettingParameter;
                txtCDMAThreePeriodTimeStart.DataContext = JsonInterFace.CDMADeviceAdvanceSettingParameter;
                txtCDMAThreePeriodTimeEnd.DataContext = JsonInterFace.CDMADeviceAdvanceSettingParameter;

                //IMSI设置
                txtCDMAIMSIInput.DataContext = JsonInterFace.CDMAIMSIControlInfo;

                //短信息记录
                txtCDMAMessageContent.DataContext = JsonInterFace.CDMAConfigSMSMSG;
                txtCDMASMSC.DataContext = JsonInterFace.CDMAConfigSMSMSG;
                txtCDMASMSI.DataContext = JsonInterFace.CDMAConfigSMSMSG;
                CDMASMSMessageDataGrid.ItemsSource = CDMASMSList;
                CDMASMSRecordGrid.ItemsSource = CDMASMSRecordList;

                //系统维护
                txtCDMAupgradeFile.DataContext = JsonInterFace.CDMADeviceSystemMaintenenceParameter;
                txtCDMALogFilePath.DataContext = JsonInterFace.CDMADeviceSystemMaintenenceParameter;
                txtCDMAVersion.DataContext = JsonInterFace.CDMADeviceSystemMaintenenceParameter;
                txtCDMAUpdateLogsShow.DataContext = JsonInterFace.ProgressBarInfo;
                pgbCDMAUpdateProgressBar.DataContext = JsonInterFace.ProgressBarInfo;

                //工程设置
                txtCDMAparameterCommandList.DataContext = JsonInterFace.CDMADeviceObjectSettingParameter;
                txtCDMAparameterResultValueList.DataContext = JsonInterFace.CDMADeviceObjectSettingParameter;

                //非XML设置
                cbCDMAEnable.DataContext = JsonInterFace.DeviceNoXMLUpload;
                cbCDMACommEnable.DataContext = JsonInterFace.DeviceNoXMLUpload;
                cbCDMAMessageFormat.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtCDMACommIp.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtCDMACommPort.DataContext = JsonInterFace.DeviceNoXMLUpload;
                cbCDMAType.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtCDMAPeriod.DataContext = JsonInterFace.DeviceNoXMLUpload;
                cbCDMAEncryptType.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtCDMACacheMax.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtCDMADataFormat.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtCDMANameFormat.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtCDMAURLorIP.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtCDMAAddInfo.DataContext = JsonInterFace.DeviceNoXMLUpload;
                #endregion

                #region TDS 功能模块
                //设备信息
                txtTDSDomainName.DataContext = JsonInterFace.TDSDeviceParameter;
                txtTDSStation.DataContext = JsonInterFace.TDSDeviceParameter;
                txtTDSDeviceName.DataContext = JsonInterFace.TDSDeviceParameter;
                cbxTDSDeviceMode.DataContext = JsonInterFace.TDSDeviceParameter;
                rbTDSStaticIPMode.DataContext = JsonInterFace.TDSDeviceParameter;
                rbTDSDynamicIPMode.DataContext = JsonInterFace.TDSDeviceParameter;
                txtTDSIPAddr.DataContext = JsonInterFace.TDSDeviceParameter;
                txtTDSPort.DataContext = JsonInterFace.TDSDeviceParameter;
                txtTDSNetMask.DataContext = JsonInterFace.TDSDeviceParameter;
                txtTDSSN.DataContext = JsonInterFace.TDSDeviceParameter;
                cbxTDSDeviceIdentificationMode.DataContext = JsonInterFace.TDSDeviceParameter;

                //小区信息
                txtTDSPLMN.DataContext = JsonInterFace.TDSCellNeighParameter;
                txtTDSFreqPoint.DataContext = JsonInterFace.TDSCellNeighParameter;
                txtTDSPowerDown.DataContext = JsonInterFace.TDSCellNeighParameter;
                txtTDSOperatorName.DataContext = JsonInterFace.TDSCellNeighParameter;
                txtTDSDisturbCode.DataContext = JsonInterFace.TDSCellNeighParameter;
                txtTDSTACorLAC.DataContext = JsonInterFace.TDSCellNeighParameter;
                txtTDSCycle.DataContext = JsonInterFace.TDSCellNeighParameter;
                TDSMorePLMNSListDataGrid.ItemsSource = TDSMorePLMNList;
                TDSPeriodFreqDataGrid.ItemsSource = TDSPeriorFreqList;
                txtTDSPeriodFreq.DataContext = JsonInterFace.TDSCellNeighParameter;

                //同步状态信息
                txtTDSStatus.DataContext = JsonInterFace.SYNCInfo;
                txtTDSSource.DataContext = JsonInterFace.SYNCInfo;
                txtTDSEuarfcn.DataContext = JsonInterFace.SYNCInfo;
                txtTDSPCI.DataContext = JsonInterFace.SYNCInfo;

                //工作模式
                rdbTDSFreqAuto.DataContext = JsonInterFace.TDSSetWorkModeParameter;
                rdbTDSFreqManul.DataContext = JsonInterFace.TDSSetWorkModeParameter;
                rdbTDSRebootModeAuto.DataContext = JsonInterFace.TDSSetWorkModeParameter;
                rdbTDSRebootModeManul.DataContext = JsonInterFace.TDSSetWorkModeParameter;

                //高级设置
                txtTDSSweepFreqPoint.DataContext = JsonInterFace.TDSDeviceAdvanceSettingParameter;
                txtTDSNTP.DataContext = JsonInterFace.TDSDeviceAdvanceSettingParameter;
                txtTDSPriority.DataContext = JsonInterFace.TDSDeviceAdvanceSettingParameter;
                txtTDSFirstPeriodTimeStart.DataContext = JsonInterFace.TDSDeviceAdvanceSettingParameter;
                txtTDSFirstPeriodTimeEnd.DataContext = JsonInterFace.TDSDeviceAdvanceSettingParameter;
                txtTDSSecondPeriodTimeStart.DataContext = JsonInterFace.TDSDeviceAdvanceSettingParameter;
                txtTDSSecoondPeriodTimeEnd.DataContext = JsonInterFace.TDSDeviceAdvanceSettingParameter;
                txtTDSThreePeriodTimeStart.DataContext = JsonInterFace.TDSDeviceAdvanceSettingParameter;
                txtTDSThreePeriodTimeEnd.DataContext = JsonInterFace.TDSDeviceAdvanceSettingParameter;

                //系统维护
                txtTDSupgradeFile.DataContext = JsonInterFace.TDSDeviceSystemMaintenenceParameter;
                txtTDSLogFilePath.DataContext = JsonInterFace.TDSDeviceSystemMaintenenceParameter;
                txtTDSVersion.DataContext = JsonInterFace.TDSDeviceSystemMaintenenceParameter;
                txtTDSUpdateLogsShow.DataContext = JsonInterFace.ProgressBarInfo;
                pgbTDSUpdateProgressBar.DataContext = JsonInterFace.ProgressBarInfo;

                //工程设置
                txtTDSparameterCommandList.DataContext = JsonInterFace.TDSDeviceObjectSettingParameter;
                txtTDSparameterResultValueList.DataContext = JsonInterFace.TDSDeviceObjectSettingParameter;

                //非XML设置
                cbTDSEnable.DataContext = JsonInterFace.DeviceNoXMLUpload;
                cbTDSCommEnable.DataContext = JsonInterFace.DeviceNoXMLUpload;
                cbTDSMessageFormat.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtTDSCommIp.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtTDSCommPort.DataContext = JsonInterFace.DeviceNoXMLUpload;
                cbTDSType.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtTDSPeriod.DataContext = JsonInterFace.DeviceNoXMLUpload;
                cbTDSEncryptType.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtTDSCacheMax.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtTDSDataFormat.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtTDSNameFormat.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtTDSURLorIP.DataContext = JsonInterFace.DeviceNoXMLUpload;
                txtTDSAddInfo.DataContext = JsonInterFace.DeviceNoXMLUpload;
                #endregion

                #region 权限 
                //设备信息,小区信息,高级设置,系统维护,工程设置,邻小区信息,IMSI设置,短信息设置
                if (RoleTypeClass.RoleType != null && RoleTypeClass.RoleType != "")
                {
                    if (RoleTypeClass.RoleType.Equals("RoleType"))
                    {
                        if (!RoleTypeClass.RolePrivilege.ContainsKey("设备信息"))
                        {
                            tmDeviceInfo.Visibility = System.Windows.Visibility.Collapsed;
                            tmGSMDeviceInfo.Visibility = System.Windows.Visibility.Collapsed;
                            tmWCDMADeviceInfo.Visibility = System.Windows.Visibility.Collapsed;
                            tmCDMADeviceInfo.Visibility = System.Windows.Visibility.Collapsed;
                            tmGSMV2DeviceInfo.Visibility = System.Windows.Visibility.Collapsed;
                            tmTDSDeviceInfo.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            btnAdd.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["设备信息"]));
                            btnDelete.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["设备信息"]));
                            btnUpdate.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["设备信息"]));

                            btnGSMAdd.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["设备信息"]));
                            btnGSMDelete.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["设备信息"]));
                            btnGSMUpdate.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["设备信息"]));

                            btnWCDMAAdd.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["设备信息"]));
                            btnWCDMADelete.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["设备信息"]));
                            btnWCDMAUpdate.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["设备信息"]));

                            btnCDMAAdd.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["设备信息"]));
                            btnCDMADelete.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["设备信息"]));
                            btnCDMAUpdate.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["设备信息"]));

                            btnGSMV2Add.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["设备信息"]));
                            btnGSMV2Delete.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["设备信息"]));
                            btnGSMV2Update.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["设备信息"]));

                            btnTDSAdd.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["设备信息"]));
                            btnTDSDelete.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["设备信息"]));
                            btnTDSUpdate.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["设备信息"]));
                        }
                        if (!RoleTypeClass.RolePrivilege.ContainsKey("小区信息"))
                        {
                            tmCellNeighInfo.Visibility = System.Windows.Visibility.Collapsed;
                            tmWCDMACellNeighInfo.Visibility = System.Windows.Visibility.Collapsed;
                            tmCDMACellNeighInfo.Visibility = System.Windows.Visibility.Collapsed;
                            tmGSMV2CellNeighInfo.Visibility = System.Windows.Visibility.Collapsed;
                            tmTDSCellNeighInfo.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            btnOtherPlmnSetting.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["小区信息"]));
                            btnPeriodFreqSetting.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["小区信息"]));
                            btnCellNeighUpdate.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["小区信息"]));

                            btnWCDMACellNeighUpdate.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["小区信息"]));

                            btnCDMACellNeighUpdate.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["小区信息"]));

                            btnGSMV2CellNeighUpdate.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["小区信息"]));
                            btnStartRF.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["小区信息"]));
                            btnCloseRF.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["小区信息"]));
                            btnreStartRF.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["小区信息"]));

                            btnTDSCellNeighUpdate.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["小区信息"]));
                        }
                        if (!RoleTypeClass.RolePrivilege.ContainsKey("高级设置"))
                        {
                            tmAdvancedSetting.Visibility = System.Windows.Visibility.Collapsed;
                            tmGSMAdvancedSetting.Visibility = System.Windows.Visibility.Collapsed;
                            tmWCDMAAdvancedSetting.Visibility = System.Windows.Visibility.Collapsed;
                            tmCDMAAdvancedSetting.Visibility = System.Windows.Visibility.Collapsed;
                            tmGSMV2AdvancedSetting.Visibility = System.Windows.Visibility.Collapsed;
                            tmTDSAdvancedSetting.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            if (RoleTypeClass.RolePrivilege.ContainsKey("时间段控制"))
                            {
                                btnUpdateTime.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["时间段控制"]));
                                btnTimesSetting.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["时间段控制"]));
                                btnWCDMAUpdateTime.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["时间段控制"]));
                                btnCDMAUpdateTime.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["时间段控制"]));
                                btnGSMV2UpdateTime.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["时间段控制"]));
                                btnTDSUpdateTime.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["时间段控制"]));
                            }
                            if (RoleTypeClass.RolePrivilege.ContainsKey("其它设置"))
                            {
                                btnSweepFreqPointUpdate.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["其它设置"]));
                                btnFreqUpdate.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["其它设置"]));
                                btnNTPUpdate.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["其它设置"]));
                                btnSyncSourceUpdate.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["其它设置"]));

                                btnReSetting.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["其它设置"]));
                                btnSetting.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["其它设置"]));

                                btnWCDMAFreqUpdate.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["其它设置"]));
                                btnWCDMANTPUpdate.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["其它设置"]));

                                btnCMDAMultiCarrierSetting.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["其它设置"]));
                                btnCMDAMultiCarrierQuery.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["其它设置"]));

                                btnTDSSweepFreqPointUpdate.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["其它设置"]));
                                btnTDSNTPUpdate.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["其它设置"]));
                            }

                        }
                        if (!RoleTypeClass.RolePrivilege.ContainsKey("系统维护"))
                        {
                            SysSetting.Visibility = System.Windows.Visibility.Collapsed;
                            GSMSysSetting.Visibility = System.Windows.Visibility.Collapsed;
                            SysWCDMASetting.Visibility = System.Windows.Visibility.Collapsed;
                            SysCDMASetting.Visibility = System.Windows.Visibility.Collapsed;
                            GSMV2SysSetting.Visibility = System.Windows.Visibility.Collapsed;
                            SysTDSSetting.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            btnFileBrower.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["系统维护"]));
                            btnUpdateFile.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["系统维护"]));
                            btnLogFileBrower.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["系统维护"]));
                            btnDownLoadlogs.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["系统维护"]));

                            btnGSMFileBrower.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["系统维护"]));
                            btnGSMUpdateFile.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["系统维护"]));
                            btnGSMLogFileBrower.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["系统维护"]));
                            btnGSMDownLoadlogs.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["系统维护"]));

                            btnWCDMAFileBrower.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["系统维护"]));
                            btnWCDMAUpdateFile.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["系统维护"]));
                            btnWCDMALogFileBrower.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["系统维护"]));
                            btnWCDMADownLoadlogs.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["系统维护"]));

                            btnCDMAFileBrower.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["系统维护"]));
                            btnCDMAUpdateFile.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["系统维护"]));
                            btnCDMALogFileBrower.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["系统维护"]));
                            btnCDMADownLoadlogs.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["系统维护"]));

                            btnGSMV2FileBrower.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["系统维护"]));
                            btnGSMV2UpdateFile.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["系统维护"]));
                            btnGSMV2LogFileBrower.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["系统维护"]));
                            btnGSMV2DownLoadlogs.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["系统维护"]));

                            btnTDSFileBrower.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["系统维护"]));
                            btnTDSUpdateFile.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["系统维护"]));
                            btnTDSLogFileBrower.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["系统维护"]));
                            btnTDSDownLoadlogs.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["系统维护"]));
                        }
                        if (!RoleTypeClass.RolePrivilege.ContainsKey("工程设置"))
                        {
                            ProjectSetting.Visibility = System.Windows.Visibility.Collapsed;
                            GSMProjectSetting.Visibility = System.Windows.Visibility.Collapsed;
                            WCDMAProjectSetting.Visibility = System.Windows.Visibility.Collapsed;
                            CDMAProjectSetting.Visibility = System.Windows.Visibility.Collapsed;
                            GSMV2ProjectSetting.Visibility = System.Windows.Visibility.Collapsed;
                            TDSProjectSetting.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            btnUpdatePragram.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["工程设置"]));
                            btnSendInfo.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["工程设置"]));

                            btnGSMSendInfo.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["工程设置"]));

                            btnWCDMASendInfo.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["工程设置"]));

                            btnCDMASendInfo.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["工程设置"]));

                            btnGSMV2SendInfo.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["工程设置"]));

                            btnTDSSendInfo.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["工程设置"]));
                        }
                        if (!RoleTypeClass.RolePrivilege.ContainsKey("邻小区信息"))
                        {
                            tmCDMACellNeighInfo.Visibility = System.Windows.Visibility.Collapsed;
                            tmGSMV2CellNeighInfo.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            //
                        }
                        if (!RoleTypeClass.RolePrivilege.ContainsKey("IMSI设置"))
                        {
                            tmGSMLibraryInfo.Visibility = System.Windows.Visibility.Collapsed;
                            tmCDMAIMSISetting.Visibility = System.Windows.Visibility.Collapsed;
                            tmGSMV2IMSISetting.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            btnGSMIMSIAdd.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["IMSI设置"]));
                            btnGSMIMSIInput.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["IMSI设置"]));
                            btnLibraryRegGetting.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["IMSI设置"]));
                            btnLibraryRegClean.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["IMSI设置"]));

                            btnCDMAIMSIInput.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["IMSI设置"]));
                            btnCDMAIMSISubmit.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["IMSI设置"]));

                            btnGSMV2IMSIInput.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["IMSI设置"]));
                            btnGSMV2IMSISubmit.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["IMSI设置"]));
                        }
                        if (!RoleTypeClass.RolePrivilege.ContainsKey("短信息设置"))
                        {
                            tmGSMCellNeighInfo.Visibility = System.Windows.Visibility.Collapsed;
                            tmGSMV2SendMessage.Visibility = System.Windows.Visibility.Collapsed;
                            tmCDMASendMessage.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            btnFilterList.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["短信息设置"]));
                            btnGSMStopSendMessage.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["短信息设置"]));
                            btnGSMSendMessage.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["短信息设置"]));
                            btnGSMSMSEditCancel.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["短信息设置"]));
                            btnGSMSMSSave.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["短信息设置"]));
                            btnGSMSMSSend.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["短信息设置"]));

                            btnSendMessage.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["短信息设置"]));
                            btnCDMASMSSave.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["短信息设置"]));
                            btnGSMV2SMSSave.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["短信息设置"]));
                        }
                        if (!RoleTypeClass.RolePrivilege.ContainsKey("通话记录"))
                        {
                            tmGSMCallRecord.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            //
                        }
                        if (!RoleTypeClass.RolePrivilege.ContainsKey("短信记录"))
                        {
                            tmGSMCellNeighInfo.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            //
                        }
                        if (!RoleTypeClass.RolePrivilege.ContainsKey("非XML设置"))
                        {
                            LTENoXMLSetting.Visibility = System.Windows.Visibility.Collapsed;
                            GSMNoXMLSetting.Visibility = System.Windows.Visibility.Collapsed;
                            WCDMANoXMLSetting.Visibility = System.Windows.Visibility.Collapsed;
                            CDMANoXMLSetting.Visibility = System.Windows.Visibility.Collapsed;
                            GSMV2NoXMLSetting.Visibility = System.Windows.Visibility.Collapsed;
                            TDSNoXMLSetting.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            btnLTESetUpLoad.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["非XML设置"]));
                            btnGSMSetUpLoad.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["非XML设置"]));
                            btnWCDMASetUpLoad.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["非XML设置"]));
                            btnCDMASetUpLoad.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["非XML设置"]));
                            btnGSMV2SetUpLoad.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["非XML设置"]));
                            btnTDSSetUpLoad.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["非XML设置"]));
                        }

                    }
                    else
                    {
                        if (int.Parse(RoleTypeClass.RoleType) > 0)
                        {
                            //工程设置
                            this.ProjectSetting.Visibility = System.Windows.Visibility.Collapsed;
                            this.GSMProjectSetting.Visibility = System.Windows.Visibility.Collapsed;
                            this.WCDMAProjectSetting.Visibility = System.Windows.Visibility.Collapsed;
                            this.CDMAProjectSetting.Visibility = System.Windows.Visibility.Collapsed;
                            this.GSMV2ProjectSetting.Visibility = System.Windows.Visibility.Collapsed;
                            this.TDSProjectSetting.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        if (int.Parse(RoleTypeClass.RoleType) > 3)//操作员权限
                        {
                            //LTE
                            this.btnAdd.IsEnabled = false;
                            this.btnDelete.IsEnabled = false;
                            this.btnUpdate.IsEnabled = false;

                            this.btnOtherPlmnSetting.IsEnabled = false;
                            this.btnPeriodFreqSetting.IsEnabled = false;
                            this.btnCellNeighUpdate.IsEnabled = false;
                            this.btnSweepFreqPointUpdate.IsEnabled = false;
                            this.btnFreqUpdate.IsEnabled = false;
                            this.btnNTPUpdate.IsEnabled = false;
                            this.btnSyncSourceUpdate.IsEnabled = false;
                            this.btnUpdateTime.IsEnabled = false;
                            this.btnFileBrower.IsEnabled = false;
                            this.btnUpdateFile.IsEnabled = false;
                            this.btnLogFileBrower.IsEnabled = false;
                            this.btnDownLoadlogs.IsEnabled = false;
                            //GSM
                            this.btnGSMAdd.IsEnabled = false;
                            this.btnGSMDelete.IsEnabled = false;
                            this.btnGSMUpdate.IsEnabled = false;

                            this.btnTimesSetting.IsEnabled = false;
                            this.btnReSetting.IsEnabled = false;
                            this.btnSetting.IsEnabled = false;
                            this.btnGSMIMSIAdd.IsEnabled = false;
                            this.btnLibraryRegClean.IsEnabled = false;
                            this.btnGSMFileBrower.IsEnabled = false;
                            this.btnGSMUpdateFile.IsEnabled = false;
                            this.btnGSMLogFileBrower.IsEnabled = false;
                            this.btnGSMDownLoadlogs.IsEnabled = false;
                            //WCDMA
                            this.btnWCDMAAdd.IsEnabled = false;
                            this.btnWCDMADelete.IsEnabled = false;
                            this.btnWCDMAUpdate.IsEnabled = false;

                            this.btnWCDMACellNeighUpdate.IsEnabled = false;
                            this.btnWCDMAFreqUpdate.IsEnabled = false;
                            this.btnWCDMANTPUpdate.IsEnabled = false;
                            this.btnWCDMAUpdateTime.IsEnabled = false;
                            this.btnWCDMAFileBrower.IsEnabled = false;
                            this.btnWCDMAUpdateFile.IsEnabled = false;
                            this.btnWCDMALogFileBrower.IsEnabled = false;
                            this.btnWCDMADownLoadlogs.IsEnabled = false;
                            //CDMA
                            this.btnCDMAAdd.IsEnabled = false;
                            this.btnCDMADelete.IsEnabled = false;
                            this.btnCDMAUpdate.IsEnabled = false;

                            this.btnCDMACellNeighUpdate.IsEnabled = false;
                            this.btnOtherPlmnSetting.IsEnabled = false;
                            this.btnPeriodFreqSetting.IsEnabled = false;
                            this.btnCellNeighUpdate.IsEnabled = false;
                            this.btnCDMAIMSIInput.IsEnabled = false;
                            this.btnCDMAIMSISubmit.IsEnabled = false;
                            this.btnCDMAFileBrower.IsEnabled = false;
                            this.btnCDMAUpdateFile.IsEnabled = false;
                            this.btnCDMALogFileBrower.IsEnabled = false;
                            this.btnCDMADownLoadlogs.IsEnabled = false;
                            //GSMV2
                            this.btnGSMV2Add.IsEnabled = false;
                            this.btnGSMV2Delete.IsEnabled = false;
                            this.btnGSMV2Update.IsEnabled = false;

                            this.btnGSMV2CellNeighUpdate.IsEnabled = false;
                            this.btnStartRF.IsEnabled = false;
                            this.btnCloseRF.IsEnabled = false;
                            this.btnreStartRF.IsEnabled = false;

                            this.gbxGSMV2NBCellNeighInfo.IsEnabled = false;

                            this.btnGSMV2IMSIInput.IsEnabled = false;
                            this.btnGSMV2IMSISubmit.IsEnabled = false;

                            this.btnGSMV2FileBrower.IsEnabled = false;
                            this.btnGSMV2UpdateFile.IsEnabled = false;
                            this.btnGSMV2LogFileBrower.IsEnabled = false;
                            this.btnGSMV2DownLoadlogs.IsEnabled = false;
                            //TDS
                            this.btnTDSAdd.IsEnabled = false;
                            this.btnTDSDelete.IsEnabled = false;
                            this.btnTDSUpdate.IsEnabled = false;

                            this.btnTDSCellNeighUpdate.IsEnabled = false;
                            this.btnTDSSweepFreqPointUpdate.IsEnabled = false;
                            this.btnTDSNTPUpdate.IsEnabled = false;
                            this.btnTDSUpdateTime.IsEnabled = false;
                            this.btnTDSFileBrower.IsEnabled = false;
                            this.btnTDSUpdateFile.IsEnabled = false;
                            this.btnTDSLogFileBrower.IsEnabled = false;
                            this.btnTDSDownLoadlogs.IsEnabled = false;
                        }
                    }
                }
                #endregion

                //显示设备列表
                LoadDeviceListTreeView();

                //选项默认第一个选项
                tabControlSeting.SelectedIndex = 0;

                //CDMA邻小区信息
                CellInfoDataGrid.ItemsSource = CDMANeighCellInfo;
                //CDMA邻小区的邻小区信息
                CellInfoItemDataGrid.ItemsSource = CDMAItemNeighCellInfo;

                //GSMV2邻小区信息
                GSMV2NeighCellInfo.Clear();
                GSMV2ItemNeighCellInfo.Clear();
                GSMV2CellInfoDataGrid.ItemsSource = GSMV2NeighCellInfo;
                //GSMV2邻小区的邻小区信息
                GSMV2CellInfoItemDataGrid.ItemsSource = GSMV2ItemNeighCellInfo;

                //-----------------------------------
                //LTE设备参数修改前缓存
                selfParam.DomainFullNamePath = JsonInterFace.LteDeviceParameter.DomainFullPathName;
                selfParam.Station = JsonInterFace.LteDeviceParameter.Station;
                selfParam.DeviceName = JsonInterFace.LteDeviceParameter.DeviceName;
                selfParam.Mode = JsonInterFace.LteDeviceParameter.DeviceMode;
                selfParam.IP = JsonInterFace.LteDeviceParameter.IpAddr;
                selfParam.Port = JsonInterFace.LteDeviceParameter.Port;
                selfParam.NetMask = JsonInterFace.LteDeviceParameter.NetMask;
                selfParam.SN = JsonInterFace.LteDeviceParameter.SN;
                selfParam.DeviceNameFlag = JsonInterFace.LteDeviceParameter.DomainFullPathName;

                Dispatcher.Invoke(() =>
                {
                    btnUpdate.IsEnabled = false;
                    btnDelete.IsEnabled = false;
                    btnAdd.IsEnabled = false;
                });
                //-----------------------------------

                //GSM白名单列表
                chkGSMWhiteListCarrierOne.DataContext = JsonInterFace.GSMLibyraryRegAdd;
                chkGSMWhiteListCarrierTwo.DataContext = JsonInterFace.GSMLibyraryRegAdd;
                txtGSMIMSI.DataContext = JsonInterFace.GSMLibyraryRegAdd;
                txtGSMIMEI.DataContext = JsonInterFace.GSMLibyraryRegAdd;
                dgGSMIMSI.ItemsSource = GSMLibyraryRegIMSILists;
                dgGSMIMEI.ItemsSource = GSMLibyraryRegIMEILists;

                //重发参数标志默认值
                btnReSetting.Tag = "0";

                //下载日志到本地默认文件路径
                JsonInterFace.LteDeviceSystemMaintenenceParameter.LogFiles = Parameters.ApLogDir + @"\" + MainWindow.aDeviceSelected.SN;
                JsonInterFace.WCDMADeviceSystemMaintenenceParameter.LogFiles = Parameters.ApLogDir + @"\" + MainWindow.aDeviceSelected.SN;

            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("设备管理初始化", ex.Message, ex.StackTrace);
            }
        }

        //递归选择项
        private void SelfSelected(IList<CheckBoxTreeModel> Children, string FullName, string SelfName)
        {
            if (FullName != null && FullName != "")
            {
                foreach (CheckBoxTreeModel child in Children)
                {
                    if (child.Children.Count > 0)
                    {
                        SelfSelected(child.Children, FullName, SelfName);
                    }
                    else
                    {
                        if (FullName == child.FullName && SelfName == child.Name)
                        {
                            child.IsChecked = true;
                            break;
                        }
                    }
                }
            }
        }

        //递归取消选择项
        private void SelfUnSelected(IList<CheckBoxTreeModel> Children, string FullName, string SelfName)
        {
            if (FullName != null && FullName != "")
            {
                foreach (CheckBoxTreeModel child in Children)
                {
                    if (child.Children.Count > 0)
                    {
                        SelfUnSelected(child.Children, FullName, SelfName);
                    }
                    else
                    {
                        if (FullName == child.FullName && SelfName == child.Name)
                        {
                            child.IsChecked = false;
                        }
                    }
                }
            }
        }

        //递归取消选择项
        private void SelfUnSelected(IList<CheckBoxTreeModel> Children)
        {
            foreach (CheckBoxTreeModel child in Children)
            {
                if (child.Children.Count > 0)
                {
                    SelfUnSelected(child.Children);
                }
                else
                {
                    if (child.SelfNodeType == NodeType.LeafNode.ToString())
                    {
                        child.IsChecked = false;
                    }
                }
            }
        }

        /// <summary>
        /// 设备管理界面控制
        /// </summary>
        /// <param name="OnLine"></param>
        private void SettingOnOffLineControl(string Model, bool OnLine, string SelfType)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    if (Model == null || new Regex(DeviceType.UnknownType).Match(Model).Success)
                    {
                        tabDeviceModelControl.SelectedIndex = 0;
                        tabControlSeting.SelectedIndex = 0;
                        for (int i = 0; i < tabControlSeting.Items.Count; i++)
                        {
                            (tabControlSeting.Items[i] as TabItem).IsEnabled = OnLine;
                        }
                        return;
                    }

                    if (new Regex(DeviceType.LTE).Match(Model).Success)
                    {
                        tabDeviceModelControl.SelectedIndex = 0;
                        //不在线不能设置
                        if (SelfType != null)
                        {
                            if (NodeType.LeafNode.ToString().Equals(SelfType))
                            {
                                for (int i = 0; i < tabControlSeting.Items.Count; i++)
                                {
                                    (tabControlSeting.Items[i] as TabItem).IsEnabled = OnLine;
                                }
                            }
                            else
                            {
                                tabControlSeting.SelectedIndex = 0;
                                for (int i = 0; i < tabControlSeting.Items.Count; i++)
                                {
                                    if (i <= 0)
                                    {
                                        (tabControlSeting.Items[i] as TabItem).IsEnabled = !OnLine;
                                    }
                                    else
                                    {
                                        (tabControlSeting.Items[i] as TabItem).IsEnabled = OnLine;
                                    }
                                }
                            }
                        }
                        else
                        {
                            tabControlSeting.SelectedIndex = 0;
                            for (int i = 0; i < tabControlSeting.Items.Count; i++)
                            {
                                (tabControlSeting.Items[i] as TabItem).IsEnabled = OnLine;
                            }
                        }
                    }
                    else if (DeviceType.GSM == Model)
                    {
                        tabDeviceModelControl.SelectedIndex = 1;

                        //不在线不能设置
                        if (SelfType != null)
                        {
                            if (NodeType.LeafNode.ToString().Equals(SelfType))
                            {
                                for (int i = 0; i < tabGSMControlSeting.Items.Count; i++)
                                {
                                    (tabGSMControlSeting.Items[i] as TabItem).IsEnabled = OnLine;
                                }
                            }
                            else
                            {
                                tabGSMControlSeting.SelectedIndex = 0;
                                for (int i = 0; i < tabGSMControlSeting.Items.Count; i++)
                                {
                                    if (i <= 0)
                                    {
                                        (tabGSMControlSeting.Items[i] as TabItem).IsEnabled = !OnLine;
                                    }
                                    else
                                    {
                                        (tabGSMControlSeting.Items[i] as TabItem).IsEnabled = OnLine;
                                    }
                                }
                            }
                        }
                        else
                        {
                            tabGSMControlSeting.SelectedIndex = 0;
                            for (int i = 0; i < tabGSMControlSeting.Items.Count; i++)
                            {
                                (tabGSMControlSeting.Items[i] as TabItem).IsEnabled = OnLine;
                            }
                        }
                    }
                    else if (DeviceType.WCDMA == Model)
                    {
                        tabDeviceModelControl.SelectedIndex = 2;
                        //不在线不能设置
                        if (SelfType != null)
                        {
                            if (NodeType.LeafNode.ToString().Equals(SelfType))
                            {
                                for (int i = 0; i < tabWCDMAControlSeting.Items.Count; i++)
                                {
                                    (tabWCDMAControlSeting.Items[i] as TabItem).IsEnabled = OnLine;
                                }
                            }
                            else
                            {
                                tabWCDMAControlSeting.SelectedIndex = 0;
                                for (int i = 0; i < tabWCDMAControlSeting.Items.Count; i++)
                                {
                                    if (i <= 0)
                                    {
                                        (tabWCDMAControlSeting.Items[i] as TabItem).IsEnabled = !OnLine;
                                    }
                                    else
                                    {
                                        (tabWCDMAControlSeting.Items[i] as TabItem).IsEnabled = OnLine;
                                    }
                                }
                            }
                        }
                        else
                        {
                            tabWCDMAControlSeting.SelectedIndex = 0;
                            for (int i = 0; i < tabWCDMAControlSeting.Items.Count; i++)
                            {
                                (tabWCDMAControlSeting.Items[i] as TabItem).IsEnabled = OnLine;
                            }
                        }
                    }
                    else if (DeviceType.CDMA == Model)
                    {
                        tabDeviceModelControl.SelectedIndex = 3;
                        //不在线不能设置
                        if (SelfType != null)
                        {
                            if (NodeType.LeafNode.ToString().Equals(SelfType))
                            {
                                for (int i = 0; i < tabCDMAControlSeting.Items.Count; i++)
                                {
                                    (tabCDMAControlSeting.Items[i] as TabItem).IsEnabled = OnLine;
                                }
                            }
                            else
                            {
                                tabCDMAControlSeting.SelectedIndex = 0;
                                for (int i = 0; i < tabCDMAControlSeting.Items.Count; i++)
                                {
                                    if (i <= 0)
                                    {
                                        (tabCDMAControlSeting.Items[i] as TabItem).IsEnabled = !OnLine;
                                    }
                                    else
                                    {
                                        (tabCDMAControlSeting.Items[i] as TabItem).IsEnabled = OnLine;
                                    }
                                }
                            }
                        }
                        else
                        {
                            tabCDMAControlSeting.SelectedIndex = 0;
                            for (int i = 0; i < tabCDMAControlSeting.Items.Count; i++)
                            {
                                (tabCDMAControlSeting.Items[i] as TabItem).IsEnabled = OnLine;
                            }
                        }
                    }
                    else if (DeviceType.GSMV2 == Model)
                    {
                        tabDeviceModelControl.SelectedIndex = 4;
                        //不在线不能设置
                        if (SelfType != null)
                        {
                            if (NodeType.LeafNode.ToString().Equals(SelfType))
                            {
                                for (int i = 0; i < tabGSMV2ControlSeting.Items.Count; i++)
                                {
                                    (tabGSMV2ControlSeting.Items[i] as TabItem).IsEnabled = OnLine;
                                }
                            }
                            else
                            {
                                tabGSMV2ControlSeting.SelectedIndex = 0;
                                for (int i = 0; i < tabGSMV2ControlSeting.Items.Count; i++)
                                {
                                    if (i <= 0)
                                    {
                                        (tabGSMV2ControlSeting.Items[i] as TabItem).IsEnabled = !OnLine;
                                    }
                                    else
                                    {
                                        (tabGSMV2ControlSeting.Items[i] as TabItem).IsEnabled = OnLine;
                                    }
                                }
                            }
                        }
                        else
                        {
                            tabGSMV2ControlSeting.SelectedIndex = 0;
                            for (int i = 0; i < tabGSMV2ControlSeting.Items.Count; i++)
                            {
                                (tabGSMV2ControlSeting.Items[i] as TabItem).IsEnabled = OnLine;
                            }
                        }
                    }
                    else if (DeviceType.TD_SCDMA == Model)
                    {
                        tabDeviceModelControl.SelectedIndex = 5;
                        //不在线不能设置
                        if (SelfType != null)
                        {
                            if (NodeType.LeafNode.ToString().Equals(SelfType))
                            {
                                for (int i = 0; i < tabTDSControlSeting.Items.Count; i++)
                                {
                                    (tabTDSControlSeting.Items[i] as TabItem).IsEnabled = OnLine;
                                }
                            }
                            else
                            {
                                tabTDSControlSeting.SelectedIndex = 0;
                                for (int i = 0; i < tabTDSControlSeting.Items.Count; i++)
                                {
                                    if (i <= 0)
                                    {
                                        (tabTDSControlSeting.Items[i] as TabItem).IsEnabled = !OnLine;
                                    }
                                    else
                                    {
                                        (tabTDSControlSeting.Items[i] as TabItem).IsEnabled = OnLine;
                                    }
                                }
                            }
                        }
                        else
                        {
                            tabTDSControlSeting.SelectedIndex = 0;
                            for (int i = 0; i < tabTDSControlSeting.Items.Count; i++)
                            {
                                (tabTDSControlSeting.Items[i] as TabItem).IsEnabled = OnLine;
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void tabControlSeting_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (btnClose.IsFocused)
                {
                    this.DragMove();
                }
            }
        }

        private void tabControlSeting_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnClose.Focus();
        }

        private void DeviceListTreeView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (JsonInterFace.LteDeviceParameter.DeviceName == "" || JsonInterFace.LteDeviceParameter.DeviceName == null)
                {
                    txtDeviceName.Focus();
                    MessageBox.Show("请输入设备名称！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (cbxDeviceMode.SelectedIndex < 0)
                {
                    cbxDeviceMode.Focus();
                    cbxDeviceMode.IsDropDownOpen = true;
                    MessageBox.Show("请选择该设备的制式！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (MessageBox.Show("确定添加设备[" + JsonInterFace.LteDeviceParameter.DeviceName + "]?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        //手动
                        Parameters.ConfigType = "Manul";

                        NetWorkClient.ControllerServer.Send(JsonInterFace.AddDeviceNameRequest(JsonInterFace.LteDeviceParameter.DomainFullPathName, JsonInterFace.LteDeviceParameter.DeviceName, JsonInterFace.LteDeviceParameter.DeviceMode));
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void btnGSMV2Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (JsonInterFace.GSMV2DeviceParameter.DeviceName == "" || JsonInterFace.GSMV2DeviceParameter.DeviceName == null)
                {
                    txtGSMV2DeviceName.Focus();
                    MessageBox.Show("请输入设备名称！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (cbxGSMV2DeviceMode.SelectedIndex < 0)
                {
                    cbxGSMV2DeviceMode.Focus();
                    cbxGSMV2DeviceMode.IsDropDownOpen = true;
                    MessageBox.Show("请选择该设备的制式！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (MessageBox.Show("确定添加设备[" + JsonInterFace.GSMV2DeviceParameter.DeviceName + "]?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        NetWorkClient.ControllerServer.Send(JsonInterFace.AddDeviceNameRequest(JsonInterFace.GSMV2DeviceParameter.DomainFullPathName, JsonInterFace.GSMV2DeviceParameter.DeviceName, JsonInterFace.GSMV2DeviceParameter.DeviceMode));
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
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
                Parameters.PrintfLogsExtended("删除设备异常", ex.Message, ex.StackTrace);
            }
        }

        private void btnGSMV2Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("确定删除设备[" + JsonInterFace.GSMV2DeviceParameter.DeviceName + "]?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        NetWorkClient.ControllerServer.Send(JsonInterFace.DeleteDeviceNameRequest(JsonInterFace.GSMV2DeviceParameter.DomainFullPathName, JsonInterFace.GSMV2DeviceParameter.DeviceName));
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("删除设备异常", ex.Message, ex.StackTrace);
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Dictionary<string, string> Params = new Dictionary<string, string>();
                if (selfParam.Mode != (JsonInterFace.LteDeviceParameter.DeviceMode))
                {
                    Params.Add("mode", cbxDeviceMode.Text.Trim());
                }

                if (selfParam.SN != (JsonInterFace.LteDeviceParameter.SN))
                {
                    Params.Add("sn", txtSN.Text.Trim());
                }

                if (selfParam.IP != (JsonInterFace.LteDeviceParameter.IpAddr))
                {
                    Params.Add("ipAddr", txtIPAddr.Text.Trim());
                }

                if (selfParam.Port != (JsonInterFace.LteDeviceParameter.Port))
                {
                    Params.Add("port", txtPort.Text.Trim());
                }

                if (selfParam.NetMask != (JsonInterFace.LteDeviceParameter.NetMask))
                {
                    Params.Add("netmask", txtNetMask.Text.Trim());
                }

                if (selfParam.DeviceName != (JsonInterFace.LteDeviceParameter.DeviceName))
                {
                    Params.Add("name", txtDeviceName.Text.Trim());

                }
                if (Params.Count <= 0)
                {
                    MessageBox.Show("参数内容未改变！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                    return;
                }

                if (MessageBox.Show("确定更新设备信息[" + JsonInterFace.LteDeviceParameter.DeviceName + "]?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        Parameters.ConfigType = "Manul";
                        NetWorkClient.ControllerServer.Send(JsonInterFace.AddDeviceNameUpdateParametersRequest(JsonInterFace.LteDeviceParameter.DomainFullPathName, selfParam.DeviceName, Params));
                    }
                    else
                    {
                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
                MessageBox.Show("参数异常(未初始化),更新失败！");
            }
        }

        private void btnGSMV2Update_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Dictionary<string, string> Params = new Dictionary<string, string>();
                if (selfParam.Mode != (JsonInterFace.GSMV2DeviceParameter.DeviceMode))
                {
                    Params.Add("mode", cbxGSMV2DeviceMode.Text.Trim());
                }

                if (selfParam.SN != (JsonInterFace.GSMV2DeviceParameter.SN))
                {
                    Params.Add("sn", txtGSMV2SN.Text.Trim());
                }

                if (selfParam.IP != (JsonInterFace.GSMV2DeviceParameter.IpAddr))
                {
                    Params.Add("ipAddr", txtGSMV2IPAddr.Text.Trim());
                }

                if (selfParam.Port != (JsonInterFace.GSMV2DeviceParameter.Port))
                {
                    Params.Add("port", txtGSMV2Port.Text.Trim());
                }

                if (selfParam.NetMask != (JsonInterFace.GSMV2DeviceParameter.NetMask))
                {
                    Params.Add("netmask", txtGSMV2NetMask.Text.Trim());
                }

                if (selfParam.DeviceName != (JsonInterFace.GSMV2DeviceParameter.DeviceName))
                {
                    Params.Add("name", txtGSMV2DeviceName.Text.Trim());
                }
                if (Params.Count <= 0)
                {
                    MessageBox.Show("参数内容未改变！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                    return;
                }

                if (MessageBox.Show("确定更新设备信息[" + JsonInterFace.GSMV2DeviceParameter.DeviceName + "]?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        Parameters.ConfigType = "Manul";
                        NetWorkClient.ControllerServer.Send(JsonInterFace.AddDeviceNameUpdateParametersRequest(JsonInterFace.GSMV2DeviceParameter.DomainFullPathName, selfParam.DeviceName, Params));
                    }
                    else
                    {
                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
                MessageBox.Show("参数异常(未初始化),更新失败！");
            }
        }

        /// <summary>
        /// 小区信息提交更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCellNeighUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Dictionary<string, string> CellNeighParams = new Dictionary<string, string>();
                Dictionary<string, string> SetWorkModeParams = new Dictionary<string, string>();
                for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                {
                    if (JsonInterFace.APATTributesLists[i].FullName == FullName)
                    {
                        if (JsonInterFace.LteCellNeighParameter.PLMN != null && JsonInterFace.LteCellNeighParameter.PLMN != JsonInterFace.APATTributesLists[i].PLMN)
                        {
                            string mcc = JsonInterFace.LteCellNeighParameter.PLMN.Substring(0, Parameters.PLMN_Lengh - 2);
                            CellNeighParams.Add("mcc", mcc);
                            string mnc = JsonInterFace.LteCellNeighParameter.PLMN.Substring(Parameters.PLMN_Lengh - 2, JsonInterFace.LteCellNeighParameter.PLMN.Length - (Parameters.PLMN_Lengh - 2));
                            CellNeighParams.Add("mnc", mnc);
                        }

                        if (JsonInterFace.LteCellNeighParameter.FrequencyPoint != null && JsonInterFace.LteCellNeighParameter.FrequencyPoint != JsonInterFace.APATTributesLists[i].FrequencyPoint)
                        {
                            CellNeighParams.Add("euarfcn", JsonInterFace.LteCellNeighParameter.FrequencyPoint);
                        }

                        if (JsonInterFace.LteCellNeighParameter.PowerAttenuation != null && JsonInterFace.LteCellNeighParameter.PowerAttenuation != JsonInterFace.APATTributesLists[i].PowerAttenuation)
                        {
                            CellNeighParams.Add("txpower", JsonInterFace.LteCellNeighParameter.PowerAttenuation);
                        }

                        if (JsonInterFace.LteCellNeighParameter.BandWidth != null && JsonInterFace.LteCellNeighParameter.BandWidth != JsonInterFace.APATTributesLists[i].BandWidth)
                        {
                            CellNeighParams.Add("bandwidth", JsonInterFace.LteCellNeighParameter.BandWidth);
                        }

                        if (JsonInterFace.LteSetWorkModeParameter.FrequencyChioceModeAuto && JsonInterFace.LteSetWorkModeParameter.FrequencyChioceModeAuto != JsonInterFace.APATTributesLists[i].FrequencyChioceModeAuto)
                        {
                            SetWorkModeParams.Add("manualFreq", "0");
                        }

                        else if (JsonInterFace.LteSetWorkModeParameter.FrequencyChioceModeManul && JsonInterFace.LteSetWorkModeParameter.FrequencyChioceModeManul != JsonInterFace.APATTributesLists[i].FrequencyChioceModeManul)
                        {
                            SetWorkModeParams.Add("manualFreq", "1");
                        }

                        if (JsonInterFace.LteCellNeighParameter.Scrambler != null && JsonInterFace.LteCellNeighParameter.Scrambler != JsonInterFace.APATTributesLists[i].Scrambler)
                        {
                            CellNeighParams.Add("pci", JsonInterFace.LteCellNeighParameter.Scrambler);
                        }

                        if (JsonInterFace.LteCellNeighParameter.TacLac != null && JsonInterFace.LteCellNeighParameter.TacLac != JsonInterFace.APATTributesLists[i].TacLac)
                        {
                            CellNeighParams.Add("tac", JsonInterFace.LteCellNeighParameter.TacLac);
                        }

                        if (JsonInterFace.LteCellNeighParameter.Period != null && JsonInterFace.LteCellNeighParameter.Period != JsonInterFace.APATTributesLists[i].Period)
                        {
                            CellNeighParams.Add("periodTac", JsonInterFace.LteCellNeighParameter.Period);
                        }

                        if (JsonInterFace.LteCellNeighParameter.CellID != null && JsonInterFace.LteCellNeighParameter.CellID != JsonInterFace.APATTributesLists[i].CellID)
                        {
                            CellNeighParams.Add("cellid", JsonInterFace.LteCellNeighParameter.CellID);
                        }

                        if (JsonInterFace.LteSetWorkModeParameter.RebootModeAuto && JsonInterFace.LteSetWorkModeParameter.RebootModeAuto != JsonInterFace.APATTributesLists[i].RebootModeAuto)
                        {
                            SetWorkModeParams.Add("boot", "1");
                        }
                        else if (JsonInterFace.LteSetWorkModeParameter.RebootModeManul && JsonInterFace.LteSetWorkModeParameter.RebootModeManul != JsonInterFace.APATTributesLists[i].RebootModeManul)
                        {
                            SetWorkModeParams.Add("boot", "0");
                        }

                        if (CellNeighParams.Count <= 0 && SetWorkModeParams.Count <= 0)
                        {
                            MessageBox.Show("内容未改变！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                            return;
                        }

                        if (MessageBox.Show("确定更新小区信息[" + JsonInterFace.LteCellNeighParameter.DeviceName + "]?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                        {
                            if (NetWorkClient.ControllerServer.Connected)
                            {
                                JsonInterFace.ResultMessageList.Clear();
                                if (SetWorkModeParams.Count > 0)
                                {
                                    //工作模式设置
                                    NetWorkClient.ControllerServer.Send(JsonInterFace.APSetWorkModeRequest(
                                                                                                        JsonInterFace.LteCellNeighParameter.DomainFullPathName,
                                                                                                        JsonInterFace.LteCellNeighParameter.DeviceName,
                                                                                                        JsonInterFace.LteCellNeighParameter.IpAddr,
                                                                                                        JsonInterFace.LteCellNeighParameter.Port,
                                                                                                        JsonInterFace.LteCellNeighParameter.InnerType,
                                                                                                        JsonInterFace.LteCellNeighParameter.SN,
                                                                                                        SetWorkModeParams
                                                                                                      ));
                                }
                                if (CellNeighParams.Count > 0)
                                {
                                    //小区设置
                                    NetWorkClient.ControllerServer.Send(JsonInterFace.APSetConfigurationRequest(
                                                                                                            JsonInterFace.LteCellNeighParameter.DomainFullPathName,
                                                                                                            JsonInterFace.LteCellNeighParameter.DeviceName,
                                                                                                            JsonInterFace.LteCellNeighParameter.IpAddr,
                                                                                                            JsonInterFace.LteCellNeighParameter.Port,
                                                                                                            JsonInterFace.LteCellNeighParameter.InnerType,
                                                                                                            JsonInterFace.LteCellNeighParameter.SN,
                                                                                                            CellNeighParams
                                                                                                            ));
                                }
                            }
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "更新失败！");
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// 频点更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSweepFreqPointUpdate_Click(object sender, RoutedEventArgs e)
        {

            if (JsonInterFace.LteDeviceAdvanceSettingParameter.SN.Equals(""))
            {
                MessageBox.Show("SN信息不存在！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (JsonInterFace.LteDeviceAdvanceSettingParameter.IpAddr.Equals(""))
            {
                MessageBox.Show("IP地址信息不存在！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (JsonInterFace.LteDeviceAdvanceSettingParameter.Port.Equals(""))
            {
                MessageBox.Show("AP端口信息不存在！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (JsonInterFace.LteDeviceAdvanceSettingParameter.InnerType.Equals(""))
            {
                MessageBox.Show("InnerType信息错误！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (JsonInterFace.LteDeviceAdvanceSettingParameter.OnLine == "0")
            {
                MessageBox.Show("设备不在线！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!JsonInterFace.LteDeviceAdvanceSettingParameter.FrequencyList.Trim().Equals(""))
            {
                if (NetWorkClient.ControllerServer.Connected)
                {
                    NetWorkClient.ControllerServer.Send(
                                                        JsonInterFace.APSetSonEarfcnRequest(
                                                                                            JsonInterFace.LteDeviceAdvanceSettingParameter.DomainFullPathName,
                                                                                            JsonInterFace.LteDeviceAdvanceSettingParameter.DeviceName,
                                                                                            JsonInterFace.LteDeviceAdvanceSettingParameter.IpAddr,
                                                                                            int.Parse(JsonInterFace.LteDeviceAdvanceSettingParameter.Port),
                                                                                            JsonInterFace.LteDeviceAdvanceSettingParameter.InnerType,
                                                                                            JsonInterFace.LteDeviceAdvanceSettingParameter.SN,
                                                                                            JsonInterFace.LteDeviceAdvanceSettingParameter.FrequencyList
                                                                                            )
                                                        );
                }
            }
            else
            {
                MessageBox.Show("请输入扫频信息！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// GPS配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFreqUpdate_Click(object sender, RoutedEventArgs e)
        {
            Parameters.ConfigType = "GPS";
            if (!(bool)rbConfigure.IsChecked && !(bool)rbUnConfigure.IsChecked)
            {
                MessageBox.Show("是否配置GPS？", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (txtFreqOffsetSetting.Text.Trim().Equals(""))
            {
                if (MessageBox.Show("频偏是否为空？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            if (!NetWorkClient.ControllerServer.Connected)
            {
                if (MessageBox.Show("网络与服务器已断开！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            if (Parameters.ConfigType.Trim().Equals(""))
            {
                MessageBox.Show("未配置ConfigType类型！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            NetWorkClient.ControllerServer.Send(JsonInterFace.APGPSConfigrationRequest(
                                                                                        JsonInterFace.LteDeviceAdvanceSettingParameter.DomainFullPathName,
                                                                                        JsonInterFace.LteDeviceAdvanceSettingParameter.DeviceName,
                                                                                        JsonInterFace.LteDeviceAdvanceSettingParameter.IpAddr,
                                                                                        int.Parse(JsonInterFace.LteDeviceAdvanceSettingParameter.Port),
                                                                                        JsonInterFace.LteDeviceAdvanceSettingParameter.InnerType,
                                                                                        JsonInterFace.LteDeviceAdvanceSettingParameter.SN,
                                                                                        Convert.ToInt32(JsonInterFace.LteDeviceAdvanceSettingParameter.GPSStatusConfig),
                                                                                        JsonInterFace.LteDeviceAdvanceSettingParameter.FrequencyOffsetList
                                                                                      ));
        }

        /// <summary>
        /// NTP信息配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNTPUpdate_Click(object sender, RoutedEventArgs e)
        {
            Parameters.ConfigType = "NTP";
            if (txtNTP.Text.Equals(""))
            {
                MessageBox.Show("请输入NTP服务器地址！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (txtPriority.Text.Equals(""))
            {
                MessageBox.Show("请输入NTP优先级！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (Parameters.ConfigType.Trim().Equals(""))
            {
                MessageBox.Show("未配置ConfigType类型！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (NetWorkClient.ControllerServer.Connected)
            {
                NetWorkClient.ControllerServer.Send(JsonInterFace.NTPConfigrationRequest(
                                                                                            JsonInterFace.LteDeviceAdvanceSettingParameter.DomainFullPathName,
                                                                                            JsonInterFace.LteDeviceAdvanceSettingParameter.DeviceName,
                                                                                            JsonInterFace.LteDeviceAdvanceSettingParameter.IpAddr,
                                                                                            int.Parse(JsonInterFace.LteDeviceAdvanceSettingParameter.Port),
                                                                                            JsonInterFace.LteDeviceAdvanceSettingParameter.InnerType,
                                                                                            JsonInterFace.LteDeviceAdvanceSettingParameter.SN,
                                                                                            JsonInterFace.LteDeviceAdvanceSettingParameter.NTPServerIP,
                                                                                            JsonInterFace.LteDeviceAdvanceSettingParameter.NTPLevel
                                                                                        ));
            }
            else
            {
                MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        /// <summary>
        /// 同步源更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSyncSourceUpdate_Click(object sender, RoutedEventArgs e)
        {
            string appointNeighBandWidth = string.Empty;
            if (!(bool)rbGPS.IsChecked && !(bool)rbEmptyMouth.IsChecked)
            {
                MessageBox.Show("请配置GPS同步源！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!(bool)rbYes.IsChecked && !(bool)rbNo.IsChecked)
            {
                MessageBox.Show("请选择是否指定小区！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else
            {
                if (JsonInterFace.LteDeviceAdvanceSettingParameter.AppointNeighConfig)
                {
                    if (txtAppointNeighList.Text.Equals("")
                        && txtAppointPci.Text.Equals("")
                        && txtAppointBandWidth.Text.Equals(""))
                    {
                        MessageBox.Show("小区已指定，请输入参数：(频点，PCI，带宽)！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
            }

            if (!NetWorkClient.ControllerServer.Connected)
            {
                MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            int CNMSyncpriority = 0;
            int ManualEnable = 0;

            //GPS同步源
            if (JsonInterFace.LteDeviceAdvanceSettingParameter.SyncSourceWithGPS)
            {
                CNMSyncpriority = 0;
            }
            else if (JsonInterFace.LteDeviceAdvanceSettingParameter.SyncSourceWithKongKou)
            {
                CNMSyncpriority = 1;
            }

            //指定小区
            if (JsonInterFace.LteDeviceAdvanceSettingParameter.AppointNeighConfig)
            {
                ManualEnable = 1;
            }
            else if (JsonInterFace.LteDeviceAdvanceSettingParameter.AppointNeighNoneConfig)
            {
                ManualEnable = 0;
            }
            if (JsonInterFace.LteDeviceAdvanceSettingParameter.AppointNeighBandWidth.Equals("1.4"))
            {
                appointNeighBandWidth = "6";
            }
            else
            {
                appointNeighBandWidth = (System.Convert.ToInt32(JsonInterFace.LteDeviceAdvanceSettingParameter.AppointNeighBandWidth, 10) * 5).ToString();
            }
            NetWorkClient.ControllerServer.Send(JsonInterFace.APSyncinfoSettingRequest(
                                                                                        JsonInterFace.LteDeviceAdvanceSettingParameter.DomainFullPathName,
                                                                                        JsonInterFace.LteDeviceAdvanceSettingParameter.DeviceName,
                                                                                        JsonInterFace.LteDeviceAdvanceSettingParameter.IpAddr,
                                                                                        int.Parse(JsonInterFace.LteDeviceAdvanceSettingParameter.Port),
                                                                                        JsonInterFace.LteDeviceAdvanceSettingParameter.InnerType,
                                                                                        JsonInterFace.LteDeviceAdvanceSettingParameter.SN,
                                                                                        CNMSyncpriority,
                                                                                        ManualEnable,
                                                                                        JsonInterFace.LteDeviceAdvanceSettingParameter.AppointNeighList,
                                                                                        JsonInterFace.LteDeviceAdvanceSettingParameter.AppointNeighPci,
                                                                                        appointNeighBandWidth,
                                                                                        JsonInterFace.LteDeviceAdvanceSettingParameter.ScanEnable.ToString()
                                                                                       ));
        }

        /// <summary>
        /// 获到文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFileBrower_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog logFileName = new Microsoft.Win32.OpenFileDialog();
            if ((bool)logFileName.ShowDialog())
            {
                JsonInterFace.LteDeviceSystemMaintenenceParameter.UpgradeFile = logFileName.FileName;
            }
        }

        /// <summary>
        /// 按ESC键退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmDeviceAdvanceSetting_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void btnUpdateFile_Click(object sender, RoutedEventArgs e)
        {
            ApSystemUpDateTask();
        }

        private void ApSystemUpDateTask()
        {
            string FileName = string.Empty;
            string FileMD5 = string.Empty;
            string SelfID = string.Empty;
            string SelfName = string.Empty;
            string FileVersion = string.Empty;

            try
            {
                if (!MainWindow.aDeviceSelected.IsOnline)
                {
                    MessageBox.Show("设备[" + MainWindow.aDeviceSelected.LongFullNamePath + "]不在线！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                JsonInterFace.ProgressBarInfo.UpdateStart = false;
                //检测线程
                if (ControlUpDownActionInfoThread != null)
                {
                    if (ControlUpDownActionInfoThread.ThreadState == ThreadState.Running || ControlUpDownActionInfoThread.ThreadState == ThreadState.WaitSleepJoin)
                    {
                        ControlUpDownActionInfoThread.Abort();
                        ControlUpDownActionInfoThread.Join();
                        ControlUpDownActionInfoThread = null;
                    }
                }

                if (new Regex(DeviceType.LTE).Match(MainWindow.aDeviceSelected.Model).Success)
                {
                    if (JsonInterFace.LteDeviceSystemMaintenenceParameter.UpgradeFile != null)
                    {
                        if (File.Exists(JsonInterFace.LteDeviceSystemMaintenenceParameter.UpgradeFile))
                        {
                            if (new FileInfo(JsonInterFace.LteDeviceSystemMaintenenceParameter.UpgradeFile).Length <= int.MaxValue)
                            {
                                FileName = new FileInfo(JsonInterFace.LteDeviceSystemMaintenenceParameter.UpgradeFile).Name;
                                FileMD5 = JsonInterFace.GetMD5HashFromFile(JsonInterFace.LteDeviceSystemMaintenenceParameter.UpgradeFile);
                                FileVersion = JsonInterFace.LteDeviceSystemMaintenenceParameter.FileVertion;
                            }
                            else
                            {
                                MessageBox.Show("升级包文件太大[" + new FileInfo(JsonInterFace.LteDeviceSystemMaintenenceParameter.UpgradeFile).Length.ToString() + "]Byte！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show("升级文件[" + JsonInterFace.LteDeviceSystemMaintenenceParameter.UpgradeFile + "]不存在！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("请选择需要升级的文件！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        txtupgradeFile.Focus();
                        return;
                    }

                    //初始化
                    JsonInterFace.LteDeviceSystemMaintenenceParameter.DeviceLists.Clear();

                    //与服务器通讯
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        //发送升级请求
                        NetWorkClient.ControllerServer.Send(JsonInterFace.APUpgradeSystemRequest(
                                                                                                    FileName,
                                                                                                    FileVersion,
                                                                                                    FileMD5,
                                                                                                    MainWindow.aDeviceSelected.SelfIP,
                                                                                                    MainWindow.aDeviceSelected.SN,
                                                                                                    MainWindow.aDeviceSelected.InnerType,
                                                                                                    MainWindow.aDeviceSelected.SelfPort,
                                                                                                    MainWindow.aDeviceSelected.LongFullNamePath
                                                                                                )
                                                           );
                    }
                    else
                    {
                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                else if (MainWindow.aDeviceSelected.Model == DeviceType.WCDMA)
                {
                    if (JsonInterFace.WCDMADeviceSystemMaintenenceParameter.UpgradeFile != null)
                    {
                        FileName = new FileInfo(JsonInterFace.WCDMADeviceSystemMaintenenceParameter.UpgradeFile).Name;
                        FileMD5 = JsonInterFace.GetMD5HashFromFile(JsonInterFace.WCDMADeviceSystemMaintenenceParameter.UpgradeFile);
                        FileVersion = JsonInterFace.WCDMADeviceSystemMaintenenceParameter.FileVertion;
                    }
                    else
                    {
                        MessageBox.Show("请选择需要升级的文件！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        txtupgradeFile.Focus();
                        return;
                    }

                    //初始化
                    JsonInterFace.WCDMADeviceSystemMaintenenceParameter.DeviceLists.Clear();

                    //与服务器通讯
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        //发送升级请求
                        NetWorkClient.ControllerServer.Send(JsonInterFace.APUpgradeSystemRequest(
                                                                                                    FileName,
                                                                                                    FileVersion,
                                                                                                    FileMD5,
                                                                                                    MainWindow.aDeviceSelected.SelfIP,
                                                                                                    MainWindow.aDeviceSelected.SN,
                                                                                                    MainWindow.aDeviceSelected.InnerType,
                                                                                                    MainWindow.aDeviceSelected.SelfPort,
                                                                                                    MainWindow.aDeviceSelected.LongFullNamePath
                                                                                                )
                                                           );
                    }
                    else
                    {
                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                else if (MainWindow.aDeviceSelected.Model == DeviceType.TD_SCDMA)
                {
                    if (JsonInterFace.TDSDeviceSystemMaintenenceParameter.UpgradeFile != null)
                    {
                        FileName = new FileInfo(JsonInterFace.TDSDeviceSystemMaintenenceParameter.UpgradeFile).Name;
                        FileMD5 = JsonInterFace.GetMD5HashFromFile(JsonInterFace.TDSDeviceSystemMaintenenceParameter.UpgradeFile);
                        FileVersion = JsonInterFace.TDSDeviceSystemMaintenenceParameter.FileVertion;
                    }
                    else
                    {
                        MessageBox.Show("请选择需要升级的文件！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        txtupgradeFile.Focus();
                        return;
                    }

                    //初始化
                    JsonInterFace.TDSDeviceSystemMaintenenceParameter.DeviceLists.Clear();

                    //与服务器通讯
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        JsonInterFace.ProgressBarInfo.UpgradeTimer.Start();
                        //发送升级请求
                        NetWorkClient.ControllerServer.Send(JsonInterFace.APUpgradeSystemRequest(
                                                                                                    FileName,
                                                                                                    FileVersion,
                                                                                                    FileMD5,
                                                                                                    MainWindow.aDeviceSelected.SelfIP,
                                                                                                    MainWindow.aDeviceSelected.SN,
                                                                                                    MainWindow.aDeviceSelected.InnerType,
                                                                                                    MainWindow.aDeviceSelected.SelfPort,
                                                                                                    MainWindow.aDeviceSelected.LongFullNamePath
                                                                                                )
                                                           );
                    }
                    else
                    {
                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
            }
            catch (Exception Ex)
            {
                JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "扏行AP系统升级内部故障，" + Ex.Message, "系统升级", "升级异常终止");
                Parameters.PrintfLogsExtended("扏行AP系统升级内部故障", Ex.Message, Ex.StackTrace);
            }
        }

        /// <summary>
        /// 开始执行系统升级
        /// </summary>
        private void UpgradeAPSystemStartting(IntPtr lParam)
        {
            //打印请求升级响应信息
            StringBuilder UpdateRequestMsg = new StringBuilder();
            try
            {
                JsonInterFace.ProgressBarInfo.MaxValue = 100;
                JsonInterFace.ProgressBarInfo.StepValue = 0;
                JsonInterFace.ProgressBarInfo.ProgressBarShow = Visibility.Visible;

                UpdateRequestMsg.AppendLine(Marshal.PtrToStringBSTR(lParam));
                JsonInterFace.ProgressBarInfo.ResoultMessage += UpdateRequestMsg.ToString();
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("AP系统升级响应消息获取失败", Ex.Message, Ex.StackTrace);
            }

            //LTE
            if (new Regex(DeviceType.LTE).Match(Model).Success)
            {
                string FileName = new FileInfo(JsonInterFace.LteDeviceSystemMaintenenceParameter.UpgradeFile).Name;
                JsonInterFace.LteDeviceSystemMaintenenceParameter.DeviceLists.Add(MainWindow.aDeviceSelected.LongFullNamePath);

                new Thread(() =>
                {
                    try
                    {
                        #region LTE系列系统升级
                        //1：上传文件
                        if (JsonInterFace.FTPServerConnection.UploadFileStatus == ("1"))
                        {
                            UpdateRequestMsg.Clear();
                            UpdateRequestMsg.AppendLine("------------ 客户端登录FTP服务器开始... [" + DateTime.Now.ToString() + "] -----------");
                            UpdateRequestMsg.AppendLine("FTP ServerIP：" + JsonInterFace.FTPServerConnection.FTPAddr);
                            UpdateRequestMsg.AppendLine("FTP Root：" + JsonInterFace.FTPServerConnection.FTPRootDir);
                            UpdateRequestMsg.AppendLine("FTP UserName：" + JsonInterFace.FTPServerConnection.FTPUserName);
                            UpdateRequestMsg.AppendLine("FTP PassWord：********");
                            UpdateRequestMsg.AppendLine("FTP Port：" + JsonInterFace.FTPServerConnection.FTPPort);
                            UpdateRequestMsg.AppendLine("------------------------------------------------------------------------------");

                            JsonInterFace.ProgressBarInfo.ResoultMessage += (UpdateRequestMsg.ToString());

                            FtpHelper FTPServerConnetion = new FtpHelper(
                                                                            JsonInterFace.FTPServerConnection.FTPAddr,
                                                                            JsonInterFace.FTPServerConnection.FTPRootDir,
                                                                            JsonInterFace.FTPServerConnection.FTPUserName,
                                                                            JsonInterFace.FTPServerConnection.FTPPassWord,
                                                                            int.Parse(JsonInterFace.FTPServerConnection.FTPPort)
                                                                        );

                            Thread.Sleep(2000);

                            if (FTPServerConnetion.Connected)
                            {
                                UpdateRequestMsg.Clear();
                                UpdateRequestMsg.AppendLine("------------ 客户端登录FTP服务器成功... [" + DateTime.Now.ToString() + "] -----------");
                                UpdateRequestMsg.AppendLine("开始上传升级包[" + JsonInterFace.LteDeviceSystemMaintenenceParameter.UpgradeFile + "],请稍后......");
                                UpdateRequestMsg.AppendLine("------------------------------------------------------------------------------");
                                JsonInterFace.ProgressBarInfo.ResoultMessage += (UpdateRequestMsg.ToString());

                                //启动信息窗口与进度条控制线程
                                object[] FtpInfos = new object[] { FTPServerConnetion, JsonInterFace.LteDeviceSystemMaintenenceParameter.UpgradeFile };
                                ControlUpDownActionInfoThread = new Thread(new ParameterizedThreadStart(ControlUpDownActionInfoWindow));
                                ControlUpDownActionInfoThread.Start(FtpInfos);

                                int res = FTPServerConnetion.Put(JsonInterFace.LteDeviceSystemMaintenenceParameter.UpgradeFile);
                                if (res == 0)
                                {
                                    UpdateRequestMsg.Clear();
                                    UpdateRequestMsg.AppendLine("------------ 上传升级包到服务器结果消息... [" + DateTime.Now.ToString() + "] -----------");
                                    UpdateRequestMsg.AppendLine("升级包上传成功......");
                                    UpdateRequestMsg.AppendLine("上传返回状态：" + res.ToString());
                                    UpdateRequestMsg.AppendLine("执行升级开始......");
                                    UpdateRequestMsg.AppendLine("------------------------------------------------------------------------------");
                                    JsonInterFace.ProgressBarInfo.ResoultMessage += (UpdateRequestMsg.ToString());

                                    //关闭FTP
                                    if (FTPServerConnetion.Connected)
                                    {
                                        FTPServerConnetion.DisConnect();
                                    }

                                    if (NetWorkClient.ControllerServer.Connected)
                                    {
                                        //上传完成，通知服务器执行升级
                                        Dispatcher.BeginInvoke((Action)(() =>
                                        {
                                            NetWorkClient.ControllerServer.Send(JsonInterFace.UploadFinishedAckServerRequest("1", FileName, JsonInterFace.LteDeviceSystemMaintenenceParameter.DeviceLists));
                                        }));

                                        //启动定时器检查升级超时
                                        WaitUpDownLoadTimeOut.Start();

                                        //启动升级耗时时记录
                                        JsonInterFace.ProgressBarInfo.UpgradeTimer.Start();
                                    }
                                    else
                                    {
                                        UpdateRequestMsg.Clear();
                                        UpdateRequestMsg.AppendLine("------------ 网络通迅状态消息... [" + DateTime.Now.ToString() + "] -----------");
                                        UpdateRequestMsg.AppendLine("由于网络与服务器断开，未执行系统升级！");
                                        UpdateRequestMsg.AppendLine("---------------------------------------------------------------------------------");
                                        JsonInterFace.ProgressBarInfo.ResoultMessage += (UpdateRequestMsg.ToString());
                                    }
                                }
                                else
                                {
                                    //上传不成功 通知服务器不执行升级
                                    NetWorkClient.ControllerServer.Send(JsonInterFace.UploadFinishedAckServerRequest("0", FileName, JsonInterFace.LteDeviceSystemMaintenenceParameter.DeviceLists));
                                    if (FTPServerConnetion.Connected)
                                    {
                                        FTPServerConnetion.DisConnect();
                                    }

                                    UpdateRequestMsg.Clear();
                                    UpdateRequestMsg.AppendLine("------------ 上传升级包到服务器结果消息... [" + DateTime.Now.ToString() + "] -----------");
                                    UpdateRequestMsg.AppendLine("升级包上传失败......");
                                    UpdateRequestMsg.AppendLine("上传返回状态：" + res.ToString());
                                    UpdateRequestMsg.AppendLine("----------------------------------------------------------------------------");
                                    JsonInterFace.ProgressBarInfo.ResoultMessage += (UpdateRequestMsg.ToString());
                                }
                            }
                            else
                            {
                                UpdateRequestMsg.Clear();
                                UpdateRequestMsg.AppendLine("------------ 客户端登录FTP服务器失败... [" + DateTime.Now.ToString() + "] -----------");
                                UpdateRequestMsg.AppendLine("FTP服务器登录失败，文件上传失败,升级终止......");
                                UpdateRequestMsg.AppendLine("---------------------------------------------------------------------------------");
                                JsonInterFace.ProgressBarInfo.ResoultMessage += (UpdateRequestMsg.ToString());

                                MessageBox.Show("FTP服务器登录失败！，文件上传失败,升级终止！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                                return;
                            }
                        }
                        //0:文件不需要上传，直接通知服务器执行升级
                        else if (JsonInterFace.FTPServerConnection.UploadFileStatus == ("0"))
                        {
                            UpdateRequestMsg.Clear();
                            UpdateRequestMsg.AppendLine("------------ 服务器执行升级开始... [" + DateTime.Now.ToString() + "] -----------");
                            UpdateRequestMsg.AppendLine("AP升级包无需上传,直接执行升级，正在执行系统升级,请稍后......");
                            UpdateRequestMsg.AppendLine("---------------------------------------------------------------------------------");
                            JsonInterFace.ProgressBarInfo.ResoultMessage += (UpdateRequestMsg.ToString());

                            if (NetWorkClient.ControllerServer.Connected)
                            {
                                Dispatcher.BeginInvoke((Action)(() =>
                                {
                                    NetWorkClient.ControllerServer.Send(JsonInterFace.UploadFinishedAckServerRequest("1", FileName, JsonInterFace.LteDeviceSystemMaintenenceParameter.DeviceLists));
                                }));

                                //启动信息窗口与进度条控制线程
                                ControlUpDownActionInfoThread = new Thread(new ParameterizedThreadStart(ControlUpDownActionInfoWindow));
                                ControlUpDownActionInfoThread.Start();

                                //启动定时器检查升级超时
                                WaitUpDownLoadTimeOut.Start();

                                //启动升级耗时时记录
                                JsonInterFace.ProgressBarInfo.UpgradeTimer.Start();
                            }
                            else
                            {
                                UpdateRequestMsg.Clear();
                                UpdateRequestMsg.AppendLine("------------ 网络通迅状态信息... [" + DateTime.Now.ToString() + "] -----------");
                                UpdateRequestMsg.AppendLine("由于网络与服务器断开，未执行系统升级！");
                                UpdateRequestMsg.AppendLine("---------------------------------------------------------------------------------");
                                JsonInterFace.ProgressBarInfo.ResoultMessage += (UpdateRequestMsg.ToString());
                            }
                        }
                        else
                        {
                            MessageBox.Show("(" + JsonInterFace.FTPServerConnection.UploadFileStatus + ") 未知错误！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        SystemUpgradeExceptionMessage(ex);
                    }

                }).Start();
            }
            //WCDMA
            else if (Model == DeviceType.WCDMA)
            {
                string FileName = new FileInfo(JsonInterFace.WCDMADeviceSystemMaintenenceParameter.UpgradeFile).Name;
                JsonInterFace.WCDMADeviceSystemMaintenenceParameter.DeviceLists.Add(MainWindow.aDeviceSelected.LongFullNamePath);

                new Thread(() =>
                {
                    try
                    {
                        #region WCDMA 系统升级
                        //1：上传文件
                        if (JsonInterFace.FTPServerConnection.UploadFileStatus == ("1"))
                        {
                            UpdateRequestMsg.Clear();
                            UpdateRequestMsg.AppendLine("------------ 客户端登录FTP服务器开始... [" + DateTime.Now.ToString() + "] -----------");
                            UpdateRequestMsg.AppendLine("FTP ServerIP：" + JsonInterFace.FTPServerConnection.FTPAddr);
                            UpdateRequestMsg.AppendLine("FTP Root：" + JsonInterFace.FTPServerConnection.FTPRootDir);
                            UpdateRequestMsg.AppendLine("FTP UserName：" + JsonInterFace.FTPServerConnection.FTPUserName);
                            UpdateRequestMsg.AppendLine("FTP PassWord：********");
                            UpdateRequestMsg.AppendLine("FTP Port：" + JsonInterFace.FTPServerConnection.FTPPort);
                            UpdateRequestMsg.AppendLine("------------------------------------------------------------------------------");

                            JsonInterFace.ProgressBarInfo.ResoultMessage += (UpdateRequestMsg.ToString());

                            FtpHelper FTPServerConnetion = new FtpHelper(
                                                                            JsonInterFace.FTPServerConnection.FTPAddr,
                                                                            JsonInterFace.FTPServerConnection.FTPRootDir,
                                                                            JsonInterFace.FTPServerConnection.FTPUserName,
                                                                            JsonInterFace.FTPServerConnection.FTPPassWord,
                                                                            int.Parse(JsonInterFace.FTPServerConnection.FTPPort)
                                                                        );

                            Thread.Sleep(2000);

                            if (FTPServerConnetion.Connected)
                            {
                                UpdateRequestMsg.Clear();
                                UpdateRequestMsg.AppendLine("------------ 客户端登录FTP服务器成功... [" + DateTime.Now.ToString() + "] -----------");
                                UpdateRequestMsg.AppendLine("开始上传升级包[" + JsonInterFace.WCDMADeviceSystemMaintenenceParameter.UpgradeFile + "],请稍后......");
                                UpdateRequestMsg.AppendLine("------------------------------------------------------------------------------");
                                JsonInterFace.ProgressBarInfo.ResoultMessage += (UpdateRequestMsg.ToString());

                                //启动信息窗口与进度条控制线程
                                object[] FtpInfos = new object[] { FTPServerConnetion, JsonInterFace.WCDMADeviceSystemMaintenenceParameter.UpgradeFile };
                                ControlUpDownActionInfoThread = new Thread(new ParameterizedThreadStart(ControlUpDownActionInfoWindow));
                                ControlUpDownActionInfoThread.Start(FtpInfos);

                                int res = FTPServerConnetion.Put(JsonInterFace.WCDMADeviceSystemMaintenenceParameter.UpgradeFile);
                                if (res == 0)
                                {
                                    UpdateRequestMsg.Clear();
                                    UpdateRequestMsg.AppendLine("------------ 上传升级包到服务器结果消息... [" + DateTime.Now.ToString() + "] -----------");
                                    UpdateRequestMsg.AppendLine("升级包上传成功......");
                                    UpdateRequestMsg.AppendLine("上传返回状态：" + res.ToString());
                                    UpdateRequestMsg.AppendLine("执行升级开始......");
                                    UpdateRequestMsg.AppendLine("------------------------------------------------------------------------------");
                                    JsonInterFace.ProgressBarInfo.ResoultMessage += (UpdateRequestMsg.ToString());

                                    //关闭FTP
                                    if (FTPServerConnetion.Connected)
                                    {
                                        FTPServerConnetion.DisConnect();
                                    }

                                    if (NetWorkClient.ControllerServer.Connected)
                                    {
                                        //上传完成，通知服务器执行升级
                                        Dispatcher.BeginInvoke((Action)(() =>
                                        {
                                            NetWorkClient.ControllerServer.Send(JsonInterFace.UploadFinishedAckServerRequest("1", FileName, JsonInterFace.WCDMADeviceSystemMaintenenceParameter.DeviceLists));
                                        }));

                                        //启动定时器检查升级超时
                                        WaitUpDownLoadTimeOut.Start();

                                        //启动升级耗时时记录
                                        JsonInterFace.ProgressBarInfo.UpgradeTimer.Start();
                                    }
                                    else
                                    {
                                        UpdateRequestMsg.Clear();
                                        UpdateRequestMsg.AppendLine("------------ 网络通迅状态消息... [" + DateTime.Now.ToString() + "] -----------");
                                        UpdateRequestMsg.AppendLine("由于网络与服务器断开，未执行系统升级！");
                                        UpdateRequestMsg.AppendLine("---------------------------------------------------------------------------------");
                                        JsonInterFace.ProgressBarInfo.ResoultMessage += (UpdateRequestMsg.ToString());
                                    }
                                }
                                else
                                {
                                    //上传不成功 通知服务器不执行升级
                                    NetWorkClient.ControllerServer.Send(JsonInterFace.UploadFinishedAckServerRequest("0", FileName, JsonInterFace.WCDMADeviceSystemMaintenenceParameter.DeviceLists));
                                    if (FTPServerConnetion.Connected)
                                    {
                                        FTPServerConnetion.DisConnect();
                                    }

                                    UpdateRequestMsg.Clear();
                                    UpdateRequestMsg.AppendLine("------------ 上传升级包到服务器结果消息... [" + DateTime.Now.ToString() + "] -----------");
                                    UpdateRequestMsg.AppendLine("升级包上传失败......");
                                    UpdateRequestMsg.AppendLine("上传返回状态：" + res.ToString());
                                    UpdateRequestMsg.AppendLine("----------------------------------------------------------------------------");
                                    JsonInterFace.ProgressBarInfo.ResoultMessage += (UpdateRequestMsg.ToString());
                                }
                            }
                            else
                            {
                                UpdateRequestMsg.Clear();
                                UpdateRequestMsg.AppendLine("------------ 客户端登录FTP服务器失败... [" + DateTime.Now.ToString() + "] -----------");
                                UpdateRequestMsg.AppendLine("FTP服务器登录失败，文件上传失败,升级终止......");
                                UpdateRequestMsg.AppendLine("---------------------------------------------------------------------------------");
                                JsonInterFace.ProgressBarInfo.ResoultMessage += (UpdateRequestMsg.ToString());

                                MessageBox.Show("FTP服务器登录失败！，文件上传失败,升级终止！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                            }
                        }
                        //0: 文件不需要上传，直接通知服务器执行升级
                        else if (JsonInterFace.FTPServerConnection.UploadFileStatus == ("0"))
                        {
                            UpdateRequestMsg.Clear();
                            UpdateRequestMsg.AppendLine("------------ 客户端登录FTP服务器成功... [" + DateTime.Now.ToString() + "] -----------");
                            UpdateRequestMsg.AppendLine("AP升级包无需上传,直接执行升级，正在执行系统升级,请稍后......");
                            UpdateRequestMsg.AppendLine("---------------------------------------------------------------------------------");
                            JsonInterFace.ProgressBarInfo.ResoultMessage += (UpdateRequestMsg.ToString());

                            if (NetWorkClient.ControllerServer.Connected)
                            {
                                Dispatcher.BeginInvoke((Action)(() =>
                                {
                                    NetWorkClient.ControllerServer.Send(JsonInterFace.UploadFinishedAckServerRequest("1", FileName, JsonInterFace.WCDMADeviceSystemMaintenenceParameter.DeviceLists));
                                }));

                                //启动信息窗口与进度条控制线程
                                ControlUpDownActionInfoThread = new Thread(new ParameterizedThreadStart(ControlUpDownActionInfoWindow));
                                ControlUpDownActionInfoThread.Start();

                                //启动定时器检查升级超时
                                WaitUpDownLoadTimeOut.Start();

                                //启动升级耗时时记录
                                JsonInterFace.ProgressBarInfo.UpgradeTimer.Start();
                            }
                            else
                            {
                                UpdateRequestMsg.Clear();
                                UpdateRequestMsg.AppendLine("------------ 网络通迅状态信息... [" + DateTime.Now.ToString() + "] -----------");
                                UpdateRequestMsg.AppendLine("由于网络与服务器断开，未执行系统升级！");
                                UpdateRequestMsg.AppendLine("---------------------------------------------------------------------------------");
                                JsonInterFace.ProgressBarInfo.ResoultMessage += (UpdateRequestMsg.ToString());
                            }
                        }
                        else
                        {
                            MessageBox.Show("(" + JsonInterFace.FTPServerConnection.UploadFileStatus + ") 参数错误！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        SystemUpgradeExceptionMessage(ex);
                    }

                }).Start();
            }
            //TDS
            else if (Model == DeviceType.TD_SCDMA)
            {
                string FileName = new FileInfo(JsonInterFace.TDSDeviceSystemMaintenenceParameter.UpgradeFile).Name;
                JsonInterFace.TDSDeviceSystemMaintenenceParameter.DeviceLists.Add(MainWindow.aDeviceSelected.LongFullNamePath);

                new Thread(() =>
                {
                    try
                    {
                        #region TDS 系统升级
                        //1：上传文件
                        if (JsonInterFace.FTPServerConnection.UploadFileStatus == ("1"))
                        {
                            UpdateRequestMsg.Clear();
                            UpdateRequestMsg.AppendLine("------------ 客户端登录服务器开始... [" + DateTime.Now.ToString() + "] -----------");
                            UpdateRequestMsg.AppendLine("FTP ServerIP：" + JsonInterFace.FTPServerConnection.FTPAddr);
                            UpdateRequestMsg.AppendLine("FTP Root：" + JsonInterFace.FTPServerConnection.FTPRootDir);
                            UpdateRequestMsg.AppendLine("FTP UserName：" + JsonInterFace.FTPServerConnection.FTPUserName);
                            UpdateRequestMsg.AppendLine("FTP PassWord：********");
                            UpdateRequestMsg.AppendLine("FTP Port：" + JsonInterFace.FTPServerConnection.FTPPort);
                            UpdateRequestMsg.AppendLine("------------------------------------------------------------------------------");

                            JsonInterFace.ProgressBarInfo.ResoultMessage += (UpdateRequestMsg.ToString());

                            FtpHelper FTPServerConnetion = new FtpHelper(
                                                                            JsonInterFace.FTPServerConnection.FTPAddr,
                                                                            JsonInterFace.FTPServerConnection.FTPRootDir,
                                                                            JsonInterFace.FTPServerConnection.FTPUserName,
                                                                            JsonInterFace.FTPServerConnection.FTPPassWord,
                                                                            int.Parse(JsonInterFace.FTPServerConnection.FTPPort)
                                                                        );

                            Thread.Sleep(2000);

                            if (FTPServerConnetion.Connected)
                            {
                                UpdateRequestMsg.Clear();
                                UpdateRequestMsg.AppendLine("------------ 客户端登录FTP服务器成功... [" + DateTime.Now.ToString() + "] -----------");
                                UpdateRequestMsg.AppendLine("正在执行系统升级,请稍后......");
                                UpdateRequestMsg.AppendLine("------------------------------------------------------------------------------");
                                JsonInterFace.ProgressBarInfo.ResoultMessage += (UpdateRequestMsg.ToString());

                                //启动信息窗口与进度条控制线程
                                object[] FtpInfos = new object[] { FTPServerConnetion, JsonInterFace.WCDMADeviceSystemMaintenenceParameter.UpgradeFile };
                                ControlUpDownActionInfoThread = new Thread(new ParameterizedThreadStart(ControlUpDownActionInfoWindow));
                                ControlUpDownActionInfoThread.Start(FtpInfos);

                                int res = FTPServerConnetion.Put(JsonInterFace.TDSDeviceSystemMaintenenceParameter.UpgradeFile);
                                if (res == 0)
                                {
                                    //启动定时器检查升级超时
                                    WaitUpDownLoadTimeOut.Start();

                                    //倒记时
                                    JsonInterFace.ProgressBarInfo.UpgradeTimer.Start();

                                    if (NetWorkClient.ControllerServer.Connected)
                                    {
                                        //上传完成，通知服务器执行升级
                                        NetWorkClient.ControllerServer.Send(JsonInterFace.UploadFinishedAckServerRequest("1", FileName, JsonInterFace.TDSDeviceSystemMaintenenceParameter.DeviceLists));
                                    }

                                    //关闭FTP
                                    if (FTPServerConnetion.Connected)
                                    {
                                        FTPServerConnetion.DisConnect();
                                    }

                                    UpdateRequestMsg.Clear();
                                    UpdateRequestMsg.AppendLine("------------ 上传升级包到服务器... [" + DateTime.Now.ToString() + "] -----------");
                                    UpdateRequestMsg.AppendLine("升级包上传成功......");
                                    UpdateRequestMsg.AppendLine("------------------------------------------------------------------------------");
                                    JsonInterFace.ProgressBarInfo.ResoultMessage += (UpdateRequestMsg.ToString());
                                }
                                else
                                {
                                    //上传不成功 通知服务器不执行升级
                                    NetWorkClient.ControllerServer.Send(JsonInterFace.UploadFinishedAckServerRequest("0", FileName, JsonInterFace.TDSDeviceSystemMaintenenceParameter.DeviceLists));
                                    if (FTPServerConnetion.Connected)
                                    {
                                        FTPServerConnetion.DisConnect();
                                    }

                                    UpdateRequestMsg.Clear();
                                    UpdateRequestMsg.AppendLine("------------ 上传升级包到服务器... [" + DateTime.Now.ToString() + "] -----------");
                                    UpdateRequestMsg.AppendLine("升级包上传失败......");
                                    UpdateRequestMsg.AppendLine("----------------------------------------------------------------------------");
                                    JsonInterFace.ProgressBarInfo.ResoultMessage += (UpdateRequestMsg.ToString());
                                }
                            }
                            else
                            {
                                UpdateRequestMsg.Clear();
                                UpdateRequestMsg.AppendLine("------------ 客户端登录FTP服务器失败... [" + DateTime.Now.ToString() + "] -----------");
                                UpdateRequestMsg.AppendLine("FTP服务器登录失败，文件上传失败,升级终止......");
                                UpdateRequestMsg.AppendLine("---------------------------------------------------------------------------------");
                                JsonInterFace.ProgressBarInfo.ResoultMessage += (UpdateRequestMsg.ToString());

                                MessageBox.Show("FTP服务器登录失败！，文件上传失败,升级终止！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                            }
                        }
                        //0: 文件不需要上传，直接通知服务器执行升级
                        else if (JsonInterFace.FTPServerConnection.UploadFileStatus == ("0"))
                        {
                            //启动信息窗口与进度条控制线程
                            ControlUpDownActionInfoThread = new Thread(new ParameterizedThreadStart(ControlUpDownActionInfoWindow));
                            ControlUpDownActionInfoThread.Start();

                            //启动定时器
                            WaitUpDownLoadTimeOut.Start();

                            //倒记时
                            JsonInterFace.ProgressBarInfo.UpgradeTimer.Start();

                            UpdateRequestMsg.Clear();
                            UpdateRequestMsg.AppendLine("------------ 客户端登录FTP服务器成功... [" + DateTime.Now.ToString() + "] -----------");
                            UpdateRequestMsg.AppendLine("AP升级包无需上传,直接执行升级，正在执行系统升级,请稍后......");
                            UpdateRequestMsg.AppendLine("---------------------------------------------------------------------------------");
                            JsonInterFace.ProgressBarInfo.ResoultMessage += (UpdateRequestMsg.ToString());

                            NetWorkClient.ControllerServer.Send(JsonInterFace.UploadFinishedAckServerRequest("1", FileName, JsonInterFace.TDSDeviceSystemMaintenenceParameter.DeviceLists));
                        }
                        else
                        {
                            MessageBox.Show("(" + JsonInterFace.FTPServerConnection.UploadFileStatus + ") 参数错误！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        UpdateRequestMsg.Clear();
                        UpdateRequestMsg.AppendLine("------------ AP系统升级异常... [" + DateTime.Now.ToString() + "] -----------");
                        UpdateRequestMsg.AppendLine("异常消息：" + ex.Message);
                        UpdateRequestMsg.AppendLine("--------------------------------------------------------------------------");
                        JsonInterFace.ProgressBarInfo.ResoultMessage += (UpdateRequestMsg.ToString());

                        JsonInterFace.ProgressBarInfo.StepValue = 0;
                        JsonInterFace.ProgressBarInfo.ProgressBarShow = Visibility.Collapsed;
                        JsonInterFace.ProgressBarInfo.UpdateStart = true;
                        //倒记时
                        JsonInterFace.ProgressBarInfo.UpgradeTimer.Stop();
                        JsonInterFace.ProgressBarInfo.UpgradeTimed = "00:00:00";
                        Parameters.PrintfLogsExtended("AP系统升级异常", ex.Message, ex.StackTrace);
                    }

                }).Start();
            }
        }

        private void ControlUpDownActionInfoWindow(object FtpInfos)
        {
            int MaxValue = JsonInterFace.ProgressBarInfo.MaxValue;
            int fileSize = 0;
            //显示信息窗口与进度条
            JsonInterFace.ProgressBarInfo.ProgressBarShow = Visibility.Visible;
            if (FtpInfos != null)
            {
                fileSize = (int)new FileInfo(((string)(FtpInfos as object[])[1])).Length;
            }

            while (true)
            {
                try
                {
                    if (!JsonInterFace.ProgressBarInfo.RunProgressBar)
                    {
                        JsonInterFace.ProgressBarInfo.UpdateStart = true;
                        JsonInterFace.ProgressBarInfo.StepValue = 0;
                        JsonInterFace.ProgressBarInfo.Tips = ConfigurationManager.AppSettings["UpgradeTips"];
                        JsonInterFace.ProgressBarInfo.ProgressBarShow = Visibility.Collapsed;

                        JsonInterFace.WCDMADeviceSystemMaintenenceParameter.DeviceLists.Clear();
                        JsonInterFace.WCDMADeviceSystemMaintenenceParameter.UpgradeFile = string.Empty;
                        JsonInterFace.WCDMADeviceSystemMaintenenceParameter.FileVertion = string.Empty;

                        JsonInterFace.LteDeviceSystemMaintenenceParameter.DeviceLists.Clear();
                        JsonInterFace.LteDeviceSystemMaintenenceParameter.UpgradeFile = string.Empty;
                        JsonInterFace.LteDeviceSystemMaintenenceParameter.FileVertion = string.Empty;

                        JsonInterFace.TDSDeviceSystemMaintenenceParameter.DeviceLists.Clear();
                        JsonInterFace.TDSDeviceSystemMaintenenceParameter.UpgradeFile = string.Empty;
                        JsonInterFace.TDSDeviceSystemMaintenenceParameter.FileVertion = string.Empty;

                        WaitUpDownLoadTimeOut.Stop();
                        break;
                    }
                    else
                    {
                        if (JsonInterFace.FTPServerConnection.UploadFileStatus == ("1"))
                        {
                            //上传文件进度
                            if (((FtpHelper)(FtpInfos as object[])[0]).Complete < fileSize)
                            {
                                JsonInterFace.ProgressBarInfo.Tips = ConfigurationManager.AppSettings["UpLoadeTips"];
                                JsonInterFace.ProgressBarInfo.MaxValue = fileSize;
                                JsonInterFace.ProgressBarInfo.StepValue = ((FtpHelper)(FtpInfos as object[])[0]).Complete;
                            }
                            //升级进度
                            else
                            {
                                JsonInterFace.ProgressBarInfo.Tips = ConfigurationManager.AppSettings["UpgradeTips"];
                                JsonInterFace.ProgressBarInfo.MaxValue = MaxValue;
                                if (JsonInterFace.ProgressBarInfo.StepValue < JsonInterFace.ProgressBarInfo.MaxValue)
                                {
                                    JsonInterFace.ProgressBarInfo.StepValue += 1;
                                }
                                else
                                {
                                    JsonInterFace.ProgressBarInfo.StepValue = 1;
                                }
                            }
                        }
                        //不需要上传文件时，系统升级进度
                        else
                        {
                            JsonInterFace.ProgressBarInfo.Tips = ConfigurationManager.AppSettings["UpgradeTips"];
                            if (JsonInterFace.ProgressBarInfo.StepValue < JsonInterFace.ProgressBarInfo.MaxValue)
                            {
                                JsonInterFace.ProgressBarInfo.StepValue += 1;
                            }
                            else
                            {
                                JsonInterFace.ProgressBarInfo.StepValue = 1;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended("等待系统升级完成内部故障...", ex.Message, ex.StackTrace);
                }

                Thread.Sleep(100);
            }

            GC.Collect();
        }

        private void SystemUpgradeExceptionMessage(Exception ex)
        {
            StringBuilder UpdateRequestMsg = new StringBuilder();
            UpdateRequestMsg.AppendLine("------------ AP系统升级异常... [" + DateTime.Now.ToString() + "] -----------");
            UpdateRequestMsg.AppendLine("异常消息：" + ex.Message);
            UpdateRequestMsg.AppendLine("--------------------------------------------------------------------------");
            JsonInterFace.ProgressBarInfo.ResoultMessage += (UpdateRequestMsg.ToString());

            //进度线程停止
            if (ControlUpDownActionInfoThread != null)
            {
                if (ControlUpDownActionInfoThread.ThreadState == ThreadState.Running || ControlUpDownActionInfoThread.ThreadState == ThreadState.WaitSleepJoin)
                {
                    ControlUpDownActionInfoThread.Abort();
                    ControlUpDownActionInfoThread.Join();
                }
            }

            JsonInterFace.ProgressBarInfo.StepValue = 0;
            JsonInterFace.ProgressBarInfo.ProgressBarShow = Visibility.Collapsed;
            JsonInterFace.ProgressBarInfo.UpdateStart = true;
            //倒记时
            JsonInterFace.ProgressBarInfo.UpgradeTimer.Stop();
            JsonInterFace.ProgressBarInfo.UpgradeTimed = "00:00:00";
            Parameters.PrintfLogsExtended("AP系统升级异常", ex.Message, ex.StackTrace);
        }

        /// <summary>
        /// 进度条循环(用于某种未知等待情况)
        /// </summary>
        private void ProgressBarController()
        {
            try
            {
                while (true)
                {
                    for (int i = 0; i < JsonInterFace.ProgressBarInfo.MaxValue; i++)
                    {
                        if (!JsonInterFace.ProgressBarInfo.RunProgressBar) { break; }
                        JsonInterFace.ProgressBarInfo.StepValue += i + 1;
                        Thread.Sleep(1);
                    }

                    if (!JsonInterFace.ProgressBarInfo.RunProgressBar)
                    {
                        pgbUpdateProgressBar.Visibility = Visibility.Visible;
                        break;
                    }

                    if (JsonInterFace.ProgressBarInfo.MaxValue == 0)
                    {
                        JsonInterFace.ProgressBarInfo.StepValue = JsonInterFace.ProgressBarInfo.MaxValue;
                    }
                    JsonInterFace.ProgressBarInfo.StepValue = 0;
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void FrmDeviceAdvanceSetting_Closed(object sender, EventArgs e)
        {
            try
            {
                JsonInterFace.ProgressBarInfo.RunProgressBar = false;

                if (ControlUpDownActionInfoThread != null)
                {
                    if (ControlUpDownActionInfoThread.ThreadState == ThreadState.Running || ControlUpDownActionInfoThread.ThreadState == ThreadState.WaitSleepJoin)
                    {
                        ControlUpDownActionInfoThread.Abort();
                        ControlUpDownActionInfoThread.Join();
                    }
                }

                JsonInterFace.ProgressBarInfo.ResoultMessage = string.Empty;
                txtUpdateLogsShow.Visibility = Visibility.Hidden;
                pgbUpdateProgressBar.Visibility = Visibility.Hidden;

                SelfUnSelected(JsonInterFace.UsrdomainData);
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        //设备列表邦定
        private void LoadDeviceListTreeView()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                DeviceListTreeView.ItemsSource = null;
                DeviceListTreeView.Items.Clear();
                DeviceListTreeView.ItemsSource = JsonInterFace.UsrdomainData;
            }));
        }

        private void btnLogFileBrower_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog SelectDir = new System.Windows.Forms.FolderBrowserDialog();

            if (txtLogFilePath.Text != "")
            {
                if (Directory.Exists(txtLogFilePath.Text))
                {
                    SelectDir.SelectedPath = txtLogFilePath.Text;
                }
            }

            if (SelectDir.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                JsonInterFace.LteDeviceSystemMaintenenceParameter.LogFiles = SelectDir.SelectedPath;
            }
        }

        private void txtBandWidth_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SubWindow.SelectBandWidthWindow SelectBandWidthWin = new SubWindow.SelectBandWidthWindow();
            SelectBandWidthWin.ShowDialog();
        }

        /// <summary>
        /// 下载系统日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDownLoadlogs_Click(object sender, RoutedEventArgs e)
        {
            if (new Regex(DeviceType.LTE).Match(MainWindow.aDeviceSelected.Model).Success
                || DeviceType.WCDMA == MainWindow.aDeviceSelected.Model
                || DeviceType.TD_SCDMA == MainWindow.aDeviceSelected.Model)
            {
                if (txtLogFilePath.Text.Equals(""))
                {
                    MessageBox.Show("请选择系统日志存放目标文件夹！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    btnLogFileBrower_Click(sender, e);
                    return;
                }
                else
                {
                    if (!Directory.Exists(txtLogFilePath.Text))
                    {
                        try
                        {
                            Directory.CreateDirectory(txtLogFilePath.Text);
                        }
                        catch (Exception Ex)
                        {
                            Parameters.PrintfLogsExtended("创建存放AP日志的目标文件夹失败", Ex.Message, Ex.StackTrace);
                            MessageBox.Show("请选择存放AP日志的目标文件夹，当前指定的文件路径无效不可用！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }
                }

                DownLoadLogsTask(sender, e);
            }
            else
            {
                MessageBox.Show("目前下载AP日志只支持[LTE系列]及[WCDMA]设备！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DownLoadLogsTask(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!MainWindow.aDeviceSelected.IsOnline)
                {
                    MessageBox.Show("设备[" + MainWindow.aDeviceSelected.LongFullNamePath + "]不在线！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!NetWorkClient.ControllerServer.Connected)
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (Parameters.DownLoadLogUser == "" || Parameters.DownLoadLogUser == null)
                {
                    MessageBox.Show("获取ApLog的用户名未设置！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                //登录请求
                string FTPLoginStr = JsonInterFace.APLogFilesDownloadLoginRequest(
                                                                                    Parameters.DownLoadLogUser,
                                                                                    Parameters.DownLoadLogPass,
                                                                                    "1",
                                                                                    MainWindow.aDeviceSelected.SelfIP,
                                                                                    MainWindow.aDeviceSelected.SN,
                                                                                    MainWindow.aDeviceSelected.InnerType,
                                                                                    MainWindow.aDeviceSelected.SelfPort,
                                                                                    MainWindow.aDeviceSelected.ShortFullNamePath
                                                                                 );

                if (NetWorkClient.ControllerServer.Connected)
                {
                    NetWorkClient.ControllerServer.Send(FTPLoginStr);
                }
                else
                {
                    MessageBox.Show("网络连接中断，操作请求无效！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void FrmDeviceAdvanceSetting_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (btnClose.IsFocused)
                {
                    this.DragMove();
                }
            }
        }

        private void FrmDeviceAdvanceSetting_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnClose.Focus();
        }

        private void rdbFreqAuto_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)rdbFreqAuto.IsChecked)
            {
                rdbFreqManul.IsChecked = false;
            }
            else
            {
                if (!(bool)rdbFreqManul.IsChecked)
                {
                    rdbFreqAuto.IsChecked = true;
                }
            }
        }

        private void rdbFreqManul_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)rdbFreqManul.IsChecked)
            {
                rdbFreqAuto.IsChecked = false;
            }
            else
            {
                if (!(bool)rdbFreqAuto.IsChecked)
                {
                    rdbFreqManul.IsChecked = true;
                }
            }
        }

        private void btnDeviceTreeViewFilter_Click(object sender, RoutedEventArgs e)
        {

        }

        private void tabGSMControlSeting_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void tabGSMControlSeting_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void txtGSMBandWidth_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void rdbGSMFreqAuto_Click(object sender, RoutedEventArgs e)
        {

        }

        private void rdbGSMFreqManul_Click(object sender, RoutedEventArgs e)
        {

        }

        private void rdbGSMRebootModeAuto_Click(object sender, RoutedEventArgs e)
        {

        }

        private void rdbGSMRebootModeManul_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnGSMCellNeighUpdate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnGSMSweepFreqPointUpdate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnGSMFreqUpdate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnGSMNTPUpdate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnGSMUpdateTime_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnGSMFileBrower_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnGSMUpdateFile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnGSMLogFileBrower_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnGSMDownLoadlogs_Click(object sender, RoutedEventArgs e)
        {

        }

        private void chkGSMCarrierOne_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((bool)chkGSMCarrierOne.IsChecked)
                {
                    chkGSMCarrierTwo.IsChecked = false;
                }
                else
                {
                    if (!(bool)chkGSMCarrierTwo.IsChecked)
                    {
                        chkGSMCarrierOne.IsChecked = true;
                    }
                }

                //获取所选设备参数(GSM 第一载波)
                if (NetWorkClient.ControllerServer.Connected)
                {
                    Parameters.ConfigType = "Manul";
                    JsonInterFace.ResultMessageList.Clear();
                    NetWorkClient.ControllerServer.Send(JsonInterFace.GetGSMCarrierOneGenParaRequest(JsonInterFace.GSMDeviceParameter.DomainFullPathName, JsonInterFace.GSMDeviceParameter.DeviceName, (Convert.ToInt32(JsonInterFace.GSMCarrierParameter.CarrierOne) - 1).ToString()));
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

        private void chkGSMCarrierTwo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((bool)chkGSMCarrierTwo.IsChecked)
                {
                    chkGSMCarrierOne.IsChecked = false;
                }
                else
                {
                    if (!(bool)chkGSMCarrierOne.IsChecked)
                    {
                        chkGSMCarrierTwo.IsChecked = true;
                    }
                }

                //获取所选设备参数(GSM 第二载波)
                if (NetWorkClient.ControllerServer.Connected)
                {
                    Parameters.ConfigType = "Manul";
                    JsonInterFace.ResultMessageList.Clear();
                    NetWorkClient.ControllerServer.Send(JsonInterFace.GetGSMCarrierOneGenParaRequest(JsonInterFace.GSMDeviceParameter.DomainFullPathName, JsonInterFace.GSMDeviceParameter.DeviceName, (Convert.ToInt32(JsonInterFace.GSMCarrierParameter.CarrierTwo)).ToString()));
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

        private void chkGSMWhiteListCarrierOne_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)chkGSMWhiteListCarrierOne.IsChecked)
            {
                chkGSMWhiteListCarrierTwo.IsChecked = false;
            }
            else
            {
                if (!(bool)chkGSMWhiteListCarrierTwo.IsChecked)
                {
                    chkGSMWhiteListCarrierOne.IsChecked = true;
                }
            }
        }

        private void chkGSMWhiteListCarrierTwo_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)chkGSMWhiteListCarrierTwo.IsChecked)
            {
                chkGSMWhiteListCarrierOne.IsChecked = false;
            }
            else
            {
                if (!(bool)chkGSMWhiteListCarrierOne.IsChecked)
                {
                    chkGSMWhiteListCarrierTwo.IsChecked = true;
                }
            }
        }

        private void btnSetting_Click(object sender, RoutedEventArgs e)
        {
            Regex regexInt = new Regex(@"\d");
            bool ParametersCheck = false;
            try
            {
                //系统参数
                //ParaMcc
                if (regexInt.Match(JsonInterFace.GSMSystemParameter.ParaMcc).Success)
                {
                    if (int.Parse(JsonInterFace.GSMSystemParameter.ParaMcc) < 0 || int.Parse(JsonInterFace.GSMSystemParameter.ParaMcc) > 999)
                    {
                        ParametersCheck = true;
                    }
                }
                else
                {
                    ParametersCheck = true;
                }

                if (ParametersCheck)
                {
                    ParametersCheck = false;
                    MessageBox.Show("[MCC]格式不正确，请输入[000-999]之间！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                //ParaMnc
                if (regexInt.Match(JsonInterFace.GSMSystemParameter.ParaMnc).Success)
                {
                    if (int.Parse(JsonInterFace.GSMSystemParameter.ParaMnc) < 0 || int.Parse(JsonInterFace.GSMSystemParameter.ParaMnc) > 999)
                    {
                        ParametersCheck = true;
                    }
                }
                else
                {
                    ParametersCheck = true;
                }

                if (ParametersCheck)
                {
                    ParametersCheck = false;
                    MessageBox.Show("[MNC]格式不正确，请输入[00-999]之间！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                //ParaBisc
                if (regexInt.Match(JsonInterFace.GSMSystemParameter.ParaBsic).Success)
                {
                    if (JsonInterFace.GSMSystemParameter.ParaBsic.Length == 2)
                    {
                        if (Convert.ToInt32(JsonInterFace.GSMSystemParameter.ParaBsic.Substring(0, 1)) < 0 || Convert.ToInt32(JsonInterFace.GSMSystemParameter.ParaBsic.Substring(0, 1)) > 7)
                        {
                            ParametersCheck = true;
                        }

                        if (Convert.ToInt32(JsonInterFace.GSMSystemParameter.ParaBsic.Substring(1, 1)) < 0 || Convert.ToInt32(JsonInterFace.GSMSystemParameter.ParaBsic.Substring(1, 1)) > 7)
                        {
                            ParametersCheck = true;
                        }
                    }
                    else
                    {
                        ParametersCheck = true;
                    }
                }
                else
                {
                    ParametersCheck = true;
                }

                if (ParametersCheck)
                {
                    ParametersCheck = false;
                    MessageBox.Show("[BSIC]的格式为：[NCC-BCC]；NCC取值范围为：[0～7]，BCC取值范围为：[0～7]！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                //ParaLac
                if (regexInt.Match(JsonInterFace.GSMSystemParameter.ParaLac).Success)
                {
                    if (Convert.ToInt32(JsonInterFace.GSMSystemParameter.ParaLac) < 1 || Convert.ToInt32(JsonInterFace.GSMSystemParameter.ParaLac) > 65535)
                    {
                        ParametersCheck = true;
                    }
                }
                else
                {
                    ParametersCheck = true;
                }

                if (ParametersCheck)
                {
                    ParametersCheck = false;
                    MessageBox.Show("[Lac]的格式为：[1～65535]！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                //paraCellID
                if (regexInt.Match(JsonInterFace.GSMSystemParameter.CellID).Success)
                {
                    if (Convert.ToInt32(JsonInterFace.GSMSystemParameter.CellID) < 0 || Convert.ToInt32(JsonInterFace.GSMSystemParameter.CellID) > 65535)
                    {
                        ParametersCheck = true;
                    }
                }
                else
                {
                    ParametersCheck = true;
                }

                if (ParametersCheck)
                {
                    ParametersCheck = false;
                    MessageBox.Show("[Cell ID]的格式为：[0～65535]！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                //ParaC2
                if (regexInt.Match(JsonInterFace.GSMSystemParameter.ParaC2).Success)
                {
                    if (Convert.ToInt32(JsonInterFace.GSMSystemParameter.ParaC2) < -63 || Convert.ToInt32(JsonInterFace.GSMSystemParameter.ParaC2) > 63)
                    {
                        ParametersCheck = true;
                    }
                }
                else
                {
                    ParametersCheck = true;
                }

                if (ParametersCheck)
                {
                    ParametersCheck = false;
                    MessageBox.Show("[ParaC2]偏移量的格式为：[-63～+63]！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                //paraPeri
                if (regexInt.Match(JsonInterFace.GSMSystemParameter.ParaPeri).Success)
                {
                    if (Convert.ToInt32(JsonInterFace.GSMSystemParameter.ParaPeri) < 0 || Convert.ToInt32(JsonInterFace.GSMSystemParameter.ParaPeri) > 255)
                    {
                        ParametersCheck = true;
                    }
                }
                else
                {
                    ParametersCheck = true;
                }

                if (ParametersCheck)
                {
                    ParametersCheck = false;
                    MessageBox.Show("周期性位置更新[paraPeri]的格式为：[0～255]！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                //paraAccPwr
                if (regexInt.Match(JsonInterFace.GSMSystemParameter.ParaAccPwr).Success)
                {
                    if (Convert.ToInt32(JsonInterFace.GSMSystemParameter.ParaAccPwr) < 0 || Convert.ToInt32(JsonInterFace.GSMSystemParameter.ParaAccPwr) > 31)
                    {
                        ParametersCheck = true;
                    }
                }
                else
                {
                    ParametersCheck = true;
                }

                if (ParametersCheck)
                {
                    ParametersCheck = false;
                    MessageBox.Show("接入功率[paraAccPwr]的格式为：[0～31]！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                //ParaMsPwr
                if (regexInt.Match(JsonInterFace.GSMSystemParameter.ParaMsPwr).Success)
                {
                    if (Convert.ToInt32(JsonInterFace.GSMSystemParameter.ParaMsPwr) < 0 || Convert.ToInt32(JsonInterFace.GSMSystemParameter.ParaMsPwr) > 31)
                    {
                        ParametersCheck = true;
                    }
                }
                else
                {
                    ParametersCheck = true;
                }

                if (ParametersCheck)
                {
                    ParametersCheck = false;
                    MessageBox.Show("发射功率[ParaMsPwr]的格式为：[0～31]！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                //ParaRejCau
                if (regexInt.Match(JsonInterFace.GSMSystemParameter.ParaRejCau).Success)
                {
                    if (!new Regex(@"[2、3、5、6、11、12、13、15、17、22]").Match(JsonInterFace.GSMSystemParameter.ParaRejCau).Success)
                    {
                        ParametersCheck = true;
                    }
                }
                else
                {
                    ParametersCheck = true;
                }

                if (ParametersCheck)
                {
                    ParametersCheck = false;
                    MessageBox.Show("位置更新拒绝原因[ParaRejCau]的格式为：[2、3、5、6、11、12、13、15、17、22]中的其中一项！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                if (NetWorkClient.ControllerServer.Connected)
                {
                    //参数类型列表
                    string Carrier = null;
                    List<string> GSMMsgType = new List<string>();
                    GSMMsgType.Add("RECV_SYS_PARA");
                    GSMMsgType.Add("RECV_SYS_OPTION");
                    GSMMsgType.Add("RECV_RF_PARA");
                    GSMMsgType.Add("RECV_REG_MODE");

                    if (JsonInterFace.GSMCarrierParameter.CarrierOne)
                    {
                        Carrier = "0";
                    }
                    else if (JsonInterFace.GSMCarrierParameter.CarrierTwo)
                    {
                        Carrier = "1";
                    }

                    if (Carrier == null)
                    {
                        MessageBox.Show("请选择载波！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    //参数列表
                    List<Dictionary<string, string>> GSMParametersLists = new List<Dictionary<string, string>>();

                    //系统参数
                    Dictionary<string, string> SysPara_n_dic = new Dictionary<string, string>();

                    if (JsonInterFace.GSMSystemParameter.ParaMcc.Length <= 2)
                    {
                        SysPara_n_dic.Add("paraMcc", string.Format("{0:D3}", JsonInterFace.GSMSystemParameter.ParaMcc));
                    }
                    else
                    {
                        SysPara_n_dic.Add("paraMcc", JsonInterFace.GSMSystemParameter.ParaMcc);
                    }

                    if (JsonInterFace.GSMSystemParameter.ParaMnc.Length <= 1)
                    {
                        SysPara_n_dic.Add("paraMnc", string.Format("{0:D2}", JsonInterFace.GSMSystemParameter.ParaMnc));
                    }
                    else
                    {
                        SysPara_n_dic.Add("paraMnc", JsonInterFace.GSMSystemParameter.ParaMnc);
                    }

                    SysPara_n_dic.Add("paraBsic", JsonInterFace.GSMSystemParameter.ParaBsic);
                    SysPara_n_dic.Add("paraLac", JsonInterFace.GSMSystemParameter.ParaLac);
                    SysPara_n_dic.Add("paraCellId", JsonInterFace.GSMSystemParameter.CellID);
                    SysPara_n_dic.Add("paraC2", JsonInterFace.GSMSystemParameter.ParaC2);
                    SysPara_n_dic.Add("paraPeri", JsonInterFace.GSMSystemParameter.ParaPeri);
                    SysPara_n_dic.Add("paraAccPwr", JsonInterFace.GSMSystemParameter.ParaAccPwr);
                    SysPara_n_dic.Add("paraMsPwr", JsonInterFace.GSMSystemParameter.ParaMsPwr);
                    SysPara_n_dic.Add("paraRejCau", JsonInterFace.GSMSystemParameter.ParaRejCau);
                    GSMParametersLists.Add(SysPara_n_dic);

                    //系统选项
                    Dictionary<string, string> SysOption_n_dic = new Dictionary<string, string>();
                    SysOption_n_dic.Add("opLuSms", JsonInterFace.GSMSystemOptionParameter.OpLuSms);
                    SysOption_n_dic.Add("opLuImei", JsonInterFace.GSMSystemOptionParameter.OpLuImei);
                    SysOption_n_dic.Add("opCallEn", JsonInterFace.GSMSystemOptionParameter.OpCallEn);
                    SysOption_n_dic.Add("opDebug", JsonInterFace.GSMSystemOptionParameter.OpDebug);
                    SysOption_n_dic.Add("opLuType", (int.Parse(JsonInterFace.GSMSystemOptionParameter.OpLuType) + 1).ToString());
                    SysOption_n_dic.Add("opSmsType", (int.Parse(JsonInterFace.GSMSystemOptionParameter.OpSmsType) + 1).ToString());
                    GSMParametersLists.Add(SysOption_n_dic);

                    //射频参数
                    Dictionary<string, string> RF_n_dic = new Dictionary<string, string>();
                    RF_n_dic.Add("rfEnable", JsonInterFace.GSMRadioFrequencyParameter.RfEnable);
                    RF_n_dic.Add("rfFreq", JsonInterFace.GSMRadioFrequencyParameter.RfFreq);
                    RF_n_dic.Add("rfPwr", JsonInterFace.GSMRadioFrequencyParameter.RfPwr);
                    GSMParametersLists.Add(RF_n_dic);

                    //注册工作模式
                    Dictionary<string, string> WorkMode_n_dic = new Dictionary<string, string>();
                    WorkMode_n_dic.Add("regMode", JsonInterFace.GSMRegModeParameter.RegMode);
                    GSMParametersLists.Add(WorkMode_n_dic);

                    Parameters.ConfigType = "ParameterSetting";

                    JsonInterFace.ResultMessageList.Clear();
                    for (int i = 0; i < GSMParametersLists.Count; i++)
                    {
                        NetWorkClient.ControllerServer.Send(JsonInterFace.GSMAPSettingRequest(
                                                                                                JsonInterFace.GSMDeviceParameter.DomainFullPathName,
                                                                                                JsonInterFace.GSMDeviceParameter.DeviceName,
                                                                                                JsonInterFace.GSMDeviceParameter.IpAddr,
                                                                                                JsonInterFace.GSMDeviceParameter.Port,
                                                                                                JsonInterFace.GSMDeviceParameter.InnerType,
                                                                                                JsonInterFace.GSMDeviceParameter.SN,
                                                                                                GSMParametersLists[i],
                                                                                                GSMMsgType[i],
                                                                                                Carrier
                                                                                             ));
                        Thread.Sleep(1000);
                    }
                }
                else
                {
                    MessageBox.Show("网张与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void btnReSetting_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((JsonInterFace.GSMSystemParameter.ParaBsic != "" && JsonInterFace.GSMSystemParameter.ParaBsic != null)
                    && (JsonInterFace.GSMSystemParameter.ParaLac != "" && JsonInterFace.GSMSystemParameter.ParaLac != null)
                    && (JsonInterFace.GSMSystemParameter.CellID != "" && JsonInterFace.GSMSystemParameter.CellID != null))
                {
                    if ((sender as Button).Tag.ToString() == "0")
                    {
                        JsonInterFace.GSMSystemParameter.ParaBsic = (int.Parse(JsonInterFace.GSMSystemParameter.ParaBsic) + 1).ToString();
                        JsonInterFace.GSMSystemParameter.ParaLac = (int.Parse(JsonInterFace.GSMSystemParameter.ParaLac) + 1).ToString();
                        JsonInterFace.GSMSystemParameter.CellID = (int.Parse(JsonInterFace.GSMSystemParameter.CellID) + 1).ToString();
                    }
                    else
                    {
                        JsonInterFace.GSMSystemParameter.ParaBsic = (int.Parse(JsonInterFace.GSMSystemParameter.ParaBsic) - 1).ToString();
                        JsonInterFace.GSMSystemParameter.ParaLac = (int.Parse(JsonInterFace.GSMSystemParameter.ParaLac) - 1).ToString();
                        JsonInterFace.GSMSystemParameter.CellID = (int.Parse(JsonInterFace.GSMSystemParameter.CellID) - 1).ToString();
                    }

                    if ((sender as Button).Tag.ToString() == "0")
                    {
                        (sender as Button).Tag = "1";
                    }
                    else
                    {
                        (sender as Button).Tag = "0";
                    }

                    btnSetting_Click(sender, new RoutedEventArgs());
                }
                else
                {
                    MessageBox.Show("参数式有错误！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// 时间段设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTimesSetting_Click(object sender, RoutedEventArgs e)
        {
            string Carrier = string.Empty;
            try
            {
                if ((txtGSMFirstPeriodTimeStart.Text != ""
                && txtGSMFirstPeriodTimeEnd.Text == "")
                || (txtGSMFirstPeriodTimeStart.Text == ""
                && txtGSMFirstPeriodTimeEnd.Text != ""))
                {
                    MessageBox.Show("第一时间段的时间不完整！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else if (txtGSMFirstPeriodTimeStart.Text != "" && txtGSMFirstPeriodTimeEnd.Text != "")
                {
                    //检测时间
                    if (!Parameters.IsTime(JsonInterFace.GSMDeviceAdvanceSettingParameter.FirstPeriodTimeStart))
                    {
                        MessageBox.Show("第一时间段的起始时间格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (!Parameters.IsTime(JsonInterFace.GSMDeviceAdvanceSettingParameter.FirstPeriodTimeEnd))
                    {
                        MessageBox.Show("第一时间段的结束时间格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                if ((txtGSMSecondPeriodTimeStart.Text != ""
                    && txtGSMSecondPeriodTimeEnd.Text == "")
                    || (txtGSMSecondPeriodTimeStart.Text == ""
                    && txtGSMSecondPeriodTimeEnd.Text != ""))
                {
                    MessageBox.Show("第二时间段的时间不完整！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else if (txtGSMSecondPeriodTimeStart.Text == "" && txtGSMSecondPeriodTimeEnd.Text != "")
                {
                    //检测时间
                    if (!Parameters.IsTime(JsonInterFace.GSMDeviceAdvanceSettingParameter.SecondPeriodTimeStart))
                    {
                        MessageBox.Show("第二时间段的起始时间格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (!Parameters.IsTime(JsonInterFace.GSMDeviceAdvanceSettingParameter.SecoondPeriodTimeEnd))
                    {
                        MessageBox.Show("第二时间段的结束时间格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                if ((txtGSMThreePeriodTimeStart.Text != ""
                    && txtGSMThreePeriodTimeEnd.Text == "")
                    || (txtGSMThreePeriodTimeStart.Text == ""
                    && txtGSMThreePeriodTimeEnd.Text != ""))
                {
                    MessageBox.Show("第三时间段的时间不完整！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else if (txtGSMThreePeriodTimeStart.Text == "" && txtGSMThreePeriodTimeEnd.Text != "")
                {
                    //检测时间
                    if (!Parameters.IsTime(JsonInterFace.GSMDeviceAdvanceSettingParameter.ThreePeriodTimeStart))
                    {
                        MessageBox.Show("第三时间段的起始时间格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (!Parameters.IsTime(JsonInterFace.GSMDeviceAdvanceSettingParameter.ThreePeriodTimeEnd))
                    {
                        MessageBox.Show("第二时间段的结束时间格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                if (JsonInterFace.GSMCarrierParameter.CarrierOne)
                {
                    Carrier = "0";
                }
                else if (JsonInterFace.GSMCarrierParameter.CarrierTwo)
                {
                    Carrier = "1";
                }
                else
                {
                    MessageBox.Show("请选择[GSM]设备载波！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (NetWorkClient.ControllerServer.Connected)
                {
                    Dictionary<string, string> ApperiodTimeList = new Dictionary<string, string>();
                    ApperiodTimeList.Add("activeTime1Start", JsonInterFace.GSMDeviceAdvanceSettingParameter.FirstPeriodTimeStart);
                    ApperiodTimeList.Add("activeTime1Ended", JsonInterFace.GSMDeviceAdvanceSettingParameter.FirstPeriodTimeEnd);
                    ApperiodTimeList.Add("activeTime2Start", JsonInterFace.GSMDeviceAdvanceSettingParameter.SecondPeriodTimeStart);
                    ApperiodTimeList.Add("activeTime2Ended", JsonInterFace.GSMDeviceAdvanceSettingParameter.SecoondPeriodTimeEnd);
                    ApperiodTimeList.Add("activeTime3Start", JsonInterFace.GSMDeviceAdvanceSettingParameter.ThreePeriodTimeStart);
                    ApperiodTimeList.Add("activeTime3Ended", JsonInterFace.GSMDeviceAdvanceSettingParameter.ThreePeriodTimeEnd);

                    Parameters.ConfigType = DeviceType.GSM;
                    JsonInterFace.ResultMessageList.Clear();
                    NetWorkClient.ControllerServer.Send(JsonInterFace.GSMPeriodTimeConrolRequest(
                                                                                                JsonInterFace.GSMDeviceParameter.DomainFullPathName,
                                                                                                JsonInterFace.GSMDeviceParameter.DeviceName,
                                                                                                Carrier,
                                                                                                ApperiodTimeList
                                                                                               ));
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// 添加白名单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGSMIMSIAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((bool)chkGSMWhiteListCarrierOne.IsChecked && (bool)chkGSMWhiteListCarrierTwo.IsChecked)
                {
                    MessageBox.Show("请选择载波！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if ((txtGSMIMSI.Text == "" || txtGSMIMSI.Text == null) && (txtGSMIMEI.Text == "" || txtGSMIMEI.Text == null))
                {
                    MessageBox.Show("请输入[IMSI]号或者[IMEI]号！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (JsonInterFace.GSMDeviceParameter.OnLine == "0")
                {
                    MessageBox.Show("该设备不在线操作无效！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (NetWorkClient.ControllerServer.Connected)
                {
                    if (txtGSMIMSI.Text != "" && txtGSMIMSI.Text != null)
                    {
                        NetWorkClient.ControllerServer.Send(
                                                            JsonInterFace.GSMLibraryRegAddIMSIRequest(JsonInterFace.GSMDeviceParameter.DomainFullPathName,
                                                                                                      JsonInterFace.GSMDeviceParameter.DeviceName,
                                                                                                      JsonInterFace.GSMDeviceParameter.IpAddr,
                                                                                                      JsonInterFace.GSMDeviceParameter.Port,
                                                                                                      JsonInterFace.GSMDeviceParameter.InnerType,
                                                                                                      JsonInterFace.GSMDeviceParameter.SN
                                                                                                     )
                                                           );
                    }

                    if (txtGSMIMEI.Text != "" && txtGSMIMEI.Text != null)
                    {
                        NetWorkClient.ControllerServer.Send(
                                                            JsonInterFace.GSMLibraryRegAddIMEIRequest(
                                                                                                      JsonInterFace.GSMDeviceParameter.DomainFullPathName,
                                                                                                      JsonInterFace.GSMDeviceParameter.DeviceName,
                                                                                                      JsonInterFace.GSMDeviceParameter.IpAddr,
                                                                                                      JsonInterFace.GSMDeviceParameter.Port,
                                                                                                      JsonInterFace.GSMDeviceParameter.InnerType,
                                                                                                      JsonInterFace.GSMDeviceParameter.SN
                                                                                                     )
                                                           );

                    }
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// 查询白名单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLibraryRegGetting_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!(bool)chkGSMWhiteListCarrierOne.IsChecked && !(bool)chkGSMWhiteListCarrierTwo.IsChecked)
                {
                    MessageBox.Show("请选择载波！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (JsonInterFace.GSMDeviceParameter.OnLine == "0")
                {
                    MessageBox.Show("该设备不在线操作无效！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (NetWorkClient.ControllerServer.Connected)
                {
                    //IMSI显示表
                    GSMLibyraryRegIMSILists.Clear();
                    //IMEI显示表
                    GSMLibyraryRegIMEILists.Clear();
                    //IMEI缓存表
                    JsonInterFace.GSMLibyraryRegQuery.LibraryRegIMEITab.Rows.Clear();
                    //IMSI缓存表
                    JsonInterFace.GSMLibyraryRegQuery.LibraryRegIMSITab.Rows.Clear();

                    NetWorkClient.ControllerServer.Send(
                                                        JsonInterFace.GSMLibraryRegQueryRequest(
                                                                                                JsonInterFace.GSMDeviceParameter.DomainFullPathName,
                                                                                                JsonInterFace.GSMDeviceParameter.DeviceName,
                                                                                                JsonInterFace.GSMDeviceParameter.IpAddr,
                                                                                                JsonInterFace.GSMDeviceParameter.Port,
                                                                                                JsonInterFace.GSMDeviceParameter.InnerType,
                                                                                                JsonInterFace.GSMDeviceParameter.SN
                                                                                                )
                                                       );
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

        /// <summary>
        /// 白名单删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLibraryRegClean_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!(bool)chkGSMWhiteListCarrierOne.IsChecked && !(bool)chkGSMWhiteListCarrierTwo.IsChecked)
                {
                    MessageBox.Show("请选择载波！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (JsonInterFace.GSMDeviceParameter.OnLine == "0")
                {
                    MessageBox.Show("该设备不在线操作无效！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (NetWorkClient.ControllerServer.Connected)
                {
                    NetWorkClient.ControllerServer.Send(
                                                        JsonInterFace.GSMLibraryRegDelAllRequest(
                                                                                                    JsonInterFace.GSMDeviceParameter.DomainFullPathName,
                                                                                                    JsonInterFace.GSMDeviceParameter.DeviceName,
                                                                                                    JsonInterFace.GSMDeviceParameter.IpAddr,
                                                                                                    JsonInterFace.GSMDeviceParameter.Port,
                                                                                                    JsonInterFace.GSMDeviceParameter.InnerType,
                                                                                                    JsonInterFace.GSMDeviceParameter.SN
                                                                                                )
                                                       );
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

        /// <summary>
        /// LTE工程序设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSendInfo_Click(object sender, RoutedEventArgs e)
        {
            if (txtparameterCommandList.Text.Trim() != "")
            {
                if (NetWorkClient.ControllerServer.Connected)
                {
                    Parameters.ConfigType = "ProjectSetting";
                    NetWorkClient.ControllerServer.Send(JsonInterFace.APProjectSettingRequest(
                                                                                                JsonInterFace.LteDeviceParameter.DomainFullPathName,
                                                                                                JsonInterFace.LteDeviceParameter.DeviceName,
                                                                                                JsonInterFace.LteDeviceParameter.IpAddr,
                                                                                                JsonInterFace.LteDeviceParameter.Port,
                                                                                                JsonInterFace.LteDeviceParameter.InnerType,
                                                                                                JsonInterFace.LteDeviceParameter.SN
                                                                                            ));
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("参数内容为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnGSMSendInfo_Click(object sender, RoutedEventArgs e)
        {
            if (txtGSMparameterCommandList.Text.Trim() != "")
            {
                if (NetWorkClient.ControllerServer.Connected)
                {
                    NetWorkClient.ControllerServer.Send(JsonInterFace.APProjectSettingRequest(
                                                                                                JsonInterFace.GSMDeviceParameter.DomainFullPathName,
                                                                                                JsonInterFace.GSMDeviceParameter.DeviceName,
                                                                                                JsonInterFace.GSMDeviceParameter.IpAddr,
                                                                                                JsonInterFace.GSMDeviceParameter.Port,
                                                                                                JsonInterFace.GSMDeviceParameter.InnerType,
                                                                                                JsonInterFace.GSMDeviceParameter.SN
                                                                                            ));
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("参数内容为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void txtAppointBandWidth_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SubWindow.SelectBandWidthWin selectBandWidthFrm = new SubWindow.SelectBandWidthWin();
            selectBandWidthFrm.ShowDialog();
        }

        private void btnWCDMASendInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtWCDMAparameterCommandList.Text.Trim() != "")
                {
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        NetWorkClient.ControllerServer.Send(JsonInterFace.APProjectSettingRequest(
                                                                                                    JsonInterFace.WCDMADeviceParameter.DomainFullPathName,
                                                                                                    JsonInterFace.WCDMADeviceParameter.DeviceName,
                                                                                                    JsonInterFace.WCDMADeviceParameter.IpAddr,
                                                                                                    JsonInterFace.WCDMADeviceParameter.Port,
                                                                                                    JsonInterFace.WCDMADeviceParameter.InnerType,
                                                                                                    JsonInterFace.WCDMADeviceParameter.SN
                                                                                                ));
                    }
                    else
                    {
                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("参数内容为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// CDMA工程设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCDMASendInfo_Click(object sender, RoutedEventArgs e)
        {
            if (txtCDMAparameterCommandList.Text.Trim() != "")
            {
                if (NetWorkClient.ControllerServer.Connected)
                {
                    NetWorkClient.ControllerServer.Send(
                                                        JsonInterFace.APProjectSettingRequest(
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
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("参数内容为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void miCDMAObjectResultClear_Click(object sender, RoutedEventArgs e)
        {
            JsonInterFace.CDMADeviceObjectSettingParameter.ParameterResultValue = string.Empty;
        }

        private void miGSMObjectResultClear_Click(object sender, RoutedEventArgs e)
        {
            JsonInterFace.GSMDeviceObjectSettingParameter.ParameterResultValue = string.Empty;
        }

        private void miLTEObjectResultClear_Click(object sender, RoutedEventArgs e)
        {
            JsonInterFace.LteDeviceObjectSettingParameter.ParameterResultValue = string.Empty;
        }

        private void mWCDMAObjectResultClear_Click(object sender, RoutedEventArgs e)
        {
            JsonInterFace.WCDMADeviceObjectSettingParameter.ParameterResultValue = string.Empty;
        }

        private void btnOtherPlmnSetting_Click(object sender, RoutedEventArgs e)
        {
            //多PLMN设置
            if (MorePLMNList.Count > 0)
            {
                string PLMNList = string.Empty;
                for (int i = 0; i < MorePLMNList.Count; i++)
                {
                    if (MorePLMNList[i].PLMNS != "" || MorePLMNList[i].PLMNS != null)
                    {
                        if (PLMNList == "")
                        {
                            PLMNList = MorePLMNList[i].PLMNS;
                        }
                        else
                        {
                            PLMNList += "," + MorePLMNList[i].PLMNS;
                        }
                    }
                }

                if (PLMNList == "" || PLMNList == null)
                {
                    MessageBox.Show("多PLMN参数为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                JsonInterFace.ResultMessageList.Clear();
                if (NetWorkClient.ControllerServer.Connected)
                {
                    Parameters.ConfigType = "OtherPLMN";
                    NetWorkClient.ControllerServer.Send(JsonInterFace.APSettingOhterPLMNRequest(
                                                                                                JsonInterFace.LteCellNeighParameter.DomainFullPathName,
                                                                                                JsonInterFace.LteCellNeighParameter.DeviceName,
                                                                                                JsonInterFace.LteCellNeighParameter.IpAddr,
                                                                                                JsonInterFace.LteCellNeighParameter.Port,
                                                                                                JsonInterFace.LteCellNeighParameter.InnerType,
                                                                                                JsonInterFace.LteCellNeighParameter.SN,
                                                                                                PLMNList
                                                                                                ));
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("多PLMN参数为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnPeriodFreqSetting_Click(object sender, RoutedEventArgs e)
        {
            //周期频点设置
            if (PeriorFreqList.Count > 0)
            {
                string PeriorFreq = string.Empty;

                if (JsonInterFace.LteCellNeighParameter.Cycle == "" || JsonInterFace.LteCellNeighParameter.Cycle == null)
                {
                    MessageBox.Show("请输入周期时间(单位：秒)！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else if (!new Regex(@"\d").Match(JsonInterFace.LteCellNeighParameter.Cycle).Success)
                {
                    MessageBox.Show("周期时间[" + JsonInterFace.LteCellNeighParameter.Cycle + "]格式不正确,范围[0~65535] (单位：秒)！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else if (Convert.ToInt32(JsonInterFace.LteCellNeighParameter.Cycle) < 0 || Convert.ToInt32(JsonInterFace.LteCellNeighParameter.Cycle) > 65535)
                {
                    MessageBox.Show("周期时间[" + JsonInterFace.LteCellNeighParameter.Cycle + "]已超出范围[0~65535] (单位：秒)！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                for (int i = 0; i < PeriorFreqList.Count; i++)
                {
                    if (PeriorFreqList[i].PerierFreq != "" && PeriorFreqList[i].PerierFreq != null)
                    {
                        if (PeriorFreq == "")
                        {
                            PeriorFreq = PeriorFreqList[i].PerierFreq;
                        }
                        else
                        {
                            PeriorFreq += "," + PeriorFreqList[i].PerierFreq;
                        }
                    }
                }

                if (PeriorFreq == null || PeriorFreq == "")
                {
                    MessageBox.Show("频点参数为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                PeriorFreq = JsonInterFace.LteCellNeighParameter.Cycle + ":" + PeriorFreq;
                JsonInterFace.ResultMessageList.Clear();
                if (NetWorkClient.ControllerServer.Connected)
                {
                    Parameters.ConfigType = "PeriodFreq";
                    NetWorkClient.ControllerServer.Send(JsonInterFace.APSettingPeriodFreqRequest(
                                                                                                JsonInterFace.LteCellNeighParameter.DomainFullPathName,
                                                                                                JsonInterFace.LteCellNeighParameter.DeviceName,
                                                                                                JsonInterFace.LteCellNeighParameter.IpAddr,
                                                                                                JsonInterFace.LteCellNeighParameter.Port,
                                                                                                JsonInterFace.LteCellNeighParameter.InnerType,
                                                                                                JsonInterFace.LteCellNeighParameter.SN,
                                                                                                PeriorFreq
                                                                                                ));
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("频点参数为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void rdbCDMABCYes_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as CheckBox).IsChecked)
            {
                rdbCDMABCNo.IsChecked = false;
            }
            else
            {
                if (!(bool)(sender as CheckBox).IsChecked)
                {
                    rdbCDMABCYes.IsChecked = true;
                }
            }
        }

        private void rdbCDMABCNo_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as CheckBox).IsChecked)
            {
                rdbCDMABCYes.IsChecked = false;
            }
            else
            {
                if (!(bool)(sender as CheckBox).IsChecked)
                {
                    rdbCDMABCNo.IsChecked = true;
                }
            }
        }

        private void rdbRebootModeAuto_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as CheckBox).IsChecked)
            {
                rdbRebootModeManul.IsChecked = false;
            }
            else
            {
                if (!(bool)(sender as CheckBox).IsChecked)
                {
                    rdbRebootModeAuto.IsChecked = true;
                }
            }
        }

        private void rdbRebootModeManul_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as CheckBox).IsChecked)
            {
                rdbRebootModeAuto.IsChecked = false;
            }
            else
            {
                if (!(bool)(sender as CheckBox).IsChecked)
                {
                    rdbRebootModeManul.IsChecked = true;
                }
            }
        }

        private void btnCDMACellNeighUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Dictionary<string, string> CDMACellNeighParams = new Dictionary<string, string>();

                if (JsonInterFace.CDMACellNeighParameter.BPLMNId != null && JsonInterFace.CDMACellNeighParameter.BPLMNId != "")
                {
                    string mcc = JsonInterFace.CDMACellNeighParameter.BPLMNId.Substring(0, Parameters.PLMN_Lengh - 2);
                    if (!Parameters.CheckParameters(mcc, 0, 999)) { return; }
                    string mnc = JsonInterFace.CDMACellNeighParameter.BPLMNId.Substring(Parameters.PLMN_Lengh - 2, JsonInterFace.CDMACellNeighParameter.BPLMNId.Length - (Parameters.PLMN_Lengh - 2));
                    if (!Parameters.CheckParameters(mnc, 0, 999)) { return; }
                    CDMACellNeighParams.Add("bPLMNId", mcc + mnc);
                }

                if (JsonInterFace.CDMACellNeighParameter.BTxPower != null && JsonInterFace.CDMACellNeighParameter.BTxPower != "")
                {
                    if (!Parameters.CheckParameters(JsonInterFace.CDMACellNeighParameter.BTxPower, 0, 255)) { return; }
                    CDMACellNeighParams.Add("bTxPower", JsonInterFace.CDMACellNeighParameter.BTxPower);
                }

                if (JsonInterFace.CDMACellNeighParameter.BRxGain != null || JsonInterFace.CDMACellNeighParameter.BRxGain != "")
                {
                    if (!Parameters.CheckParameters(JsonInterFace.CDMACellNeighParameter.BRxGain, 0, 255)) { return; }
                    CDMACellNeighParams.Add("bRxGain", JsonInterFace.CDMACellNeighParameter.BRxGain);
                }

                if (JsonInterFace.CDMACellNeighParameter.WPhyCellId != null || JsonInterFace.CDMACellNeighParameter.WPhyCellId != "")
                {
                    if (!Parameters.CheckParameters(JsonInterFace.CDMACellNeighParameter.WPhyCellId, 0, 511)) { return; }
                    CDMACellNeighParams.Add("wPhyCellId", JsonInterFace.CDMACellNeighParameter.WPhyCellId);
                }

                if (JsonInterFace.CDMACellNeighParameter.WLAC != null)
                {
                    if (!Parameters.CheckParameters(JsonInterFace.CDMACellNeighParameter.WLAC, 0, 65535)) { return; }
                    CDMACellNeighParams.Add("wLAC", JsonInterFace.CDMACellNeighParameter.WLAC);
                }

                if (JsonInterFace.CDMACellNeighParameter.WUARFCN != null)
                {
                    if (!Parameters.CheckParameters(JsonInterFace.CDMACellNeighParameter.WUARFCN, 0, 65535)) { return; }
                    CDMACellNeighParams.Add("wUARFCN", JsonInterFace.CDMACellNeighParameter.WUARFCN);
                }

                if (JsonInterFace.CDMACellNeighParameter.SID != null && JsonInterFace.CDMACellNeighParameter.NID != null)
                {
                    if (Parameters.ISDigital(JsonInterFace.CDMACellNeighParameter.SID) && Parameters.ISDigital(JsonInterFace.CDMACellNeighParameter.NID))
                    {
                        JsonInterFace.CDMACellNeighParameter.DwCellId = (Convert.ToUInt32(JsonInterFace.CDMACellNeighParameter.SID) * 65536 + Convert.ToUInt32(JsonInterFace.CDMACellNeighParameter.NID)).ToString();
                        if (!Parameters.CheckParameters(JsonInterFace.CDMACellNeighParameter.DwCellId, 0, 429497294)) { return; }
                        CDMACellNeighParams.Add("dwCellId", JsonInterFace.CDMACellNeighParameter.DwCellId);
                    }
                    else
                    {
                        if (!Parameters.ISDigital(JsonInterFace.CDMACellNeighParameter.SID))
                        {
                            MessageBox.Show("SID格式错误！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        else
                        {
                            MessageBox.Show("NID格式错误！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }
                }

                if (JsonInterFace.CDMACellNeighParameter.BWorkingMode != null)
                {
                    string _BWorkingMode = string.Empty;
                    if (JsonInterFace.CDMACellNeighParameter.BWorkingMode == "侦码模式")
                    {
                        _BWorkingMode = "1";
                    }
                    else if (JsonInterFace.CDMACellNeighParameter.BWorkingMode == "驻留模式")
                    {
                        _BWorkingMode = "3";
                    }
                    else
                    {
                        MessageBox.Show("工作模式参数[" + JsonInterFace.CDMACellNeighParameter.BWorkingMode + "]错误！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                        return;
                    }
                    if (!Parameters.CheckParameters(_BWorkingMode, 0, 3)) { return; }
                    CDMACellNeighParams.Add("bWorkingMode", _BWorkingMode);
                }

                if (JsonInterFace.CDMACellNeighParameter.WRedirectCellUarfcn != null)
                {
                    if (!Parameters.CheckParameters(JsonInterFace.CDMACellNeighParameter.WRedirectCellUarfcn, 0, 65535)) { return; }
                    CDMACellNeighParams.Add("wRedirectCellUarfcn", JsonInterFace.CDMACellNeighParameter.WRedirectCellUarfcn);
                }

                if (JsonInterFace.CDMACellNeighParameter.BCYes != null || JsonInterFace.CDMACellNeighParameter.BCNo != null)
                {
                    string BC = string.Empty;

                    if (JsonInterFace.CDMACellNeighParameter.BCYes == "True")
                    {
                        CDMACellNeighParams.Add("bC", "1");
                    }
                    else
                    {
                        CDMACellNeighParams.Add("bC", "0");
                    }
                }

                if (JsonInterFace.CDMACellNeighParameter.DwDateTime != null)
                {
                    CDMACellNeighParams.Add("dwDateTime", JsonInterFace.CDMACellNeighParameter.DwDateTime);
                }

                if (CDMACellNeighParams.Count <= 0)
                {
                    MessageBox.Show("内容未改变！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                    return;
                }


                if (MessageBox.Show("确定更新小区信息[" + JsonInterFace.CDMADeviceParameter.DeviceName + "]?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        JsonInterFace.ResultMessageList.Clear();

                        //小区设置
                        NetWorkClient.ControllerServer.Send(JsonInterFace.CDMAConfigurationFAPRequest(
                                                                                                        JsonInterFace.CDMADeviceParameter.DomainFullPathName,
                                                                                                        JsonInterFace.CDMADeviceParameter.DeviceName,
                                                                                                        JsonInterFace.CDMADeviceParameter.IpAddr,
                                                                                                        JsonInterFace.CDMADeviceParameter.Port,
                                                                                                        JsonInterFace.CDMADeviceParameter.InnerType,
                                                                                                        JsonInterFace.CDMADeviceParameter.SN,
                                                                                                        CDMACellNeighParams
                                                                                                     ));
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
                MessageBox.Show(ex.Message + "！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        //CDMA邻小区信息查询
        private void btnCellInfoQuery_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (NetWorkClient.ControllerServer.Connected)
                {
                    JsonInterFace.CDMACellNeighParameter.CellInfoTab.Clear();
                    JsonInterFace.CDMACellNeighParameter.ItemCellInfoTab.Clear();
                    CDMAItemNeighCellInfo.Clear();
                    CDMANeighCellInfo.Clear();
                    JsonInterFace.ResultMessageList.Clear();
                    Parameters.ConfigType = "Manul";
                    NetWorkClient.ControllerServer.Send(
                                                         JsonInterFace.CDMANeighCellInfoQueryRequest(
                                                                                                        JsonInterFace.CDMADeviceParameter.DomainFullPathName,
                                                                                                        JsonInterFace.CDMADeviceParameter.DeviceName,
                                                                                                        JsonInterFace.CDMADeviceParameter.IpAddr,
                                                                                                        JsonInterFace.CDMADeviceParameter.Port,
                                                                                                        JsonInterFace.CDMADeviceParameter.InnerType,
                                                                                                        JsonInterFace.CDMADeviceParameter.SN
                                                                                                    )
                                                       );
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

        //选取某一行时
        private void CellInfoDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CellInfoDataGrid.SelectedItem != null)
            {
                string Rkey = (CellInfoDataGrid.SelectedItem as CDMACellNeighParameterClass.NeithCellInfo).Rkey;
                CDMAItemNeighCellInfo.Clear();
                for (int i = 0; i < JsonInterFace.CDMACellNeighParameter.ItemCellInfoTab.Rows.Count; i++)
                {
                    if (Rkey == JsonInterFace.CDMACellNeighParameter.ItemCellInfoTab.Rows[i][5].ToString())
                    {
                        CDMACellNeighParameterClass.ItemNeithCellInfo ItemcellInfo = new CDMACellNeighParameterClass.ItemNeithCellInfo();

                        string _ID = string.Empty;
                        if (CDMAItemNeighCellInfo.Count <= 0)
                        {
                            _ID = "1";
                        }
                        else
                        {
                            _ID = (Convert.ToInt32(CDMAItemNeighCellInfo[CDMAItemNeighCellInfo.Count - 1].ID.ToString()) + 1).ToString();
                        }

                        ItemcellInfo.ID = _ID;
                        ItemcellInfo.WUarfcn = JsonInterFace.CDMACellNeighParameter.ItemCellInfoTab.Rows[i][0].ToString();
                        ItemcellInfo.WPhyCellId = JsonInterFace.CDMACellNeighParameter.ItemCellInfoTab.Rows[i][1].ToString();
                        ItemcellInfo.CRSRP = JsonInterFace.CDMACellNeighParameter.ItemCellInfoTab.Rows[i][2].ToString();
                        ItemcellInfo.CC1 = JsonInterFace.CDMACellNeighParameter.ItemCellInfoTab.Rows[i][3].ToString();
                        ItemcellInfo.BC2 = JsonInterFace.CDMACellNeighParameter.ItemCellInfoTab.Rows[i][4].ToString();

                        CDMAItemNeighCellInfo.Add(ItemcellInfo);
                    }
                }
            }
        }

        //CDMA小区参数信息查询
        private void btnCDMACellNeighQuery_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (NetWorkClient.ControllerServer.Connected)
                {
                    JsonInterFace.ResultMessageList.Clear();
                    Parameters.ConfigType = "Manul";
                    NetWorkClient.ControllerServer.Send(
                                                        JsonInterFace.CDMACellPARAMRequest(
                                                                                            JsonInterFace.CDMADeviceParameter.DomainFullPathName,
                                                                                            JsonInterFace.CDMADeviceParameter.DeviceName,
                                                                                            JsonInterFace.CDMADeviceParameter.IpAddr,
                                                                                            JsonInterFace.CDMADeviceParameter.Port,
                                                                                            JsonInterFace.CDMADeviceParameter.InnerType,
                                                                                            JsonInterFace.CDMADeviceParameter.SN
                                                                                          )
                                                       );
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
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
        private void CheckAll_Click(object sender, RoutedEventArgs e)
        {
            string IMSI = string.Empty;
            if ((bool)(((CheckBox)sender).IsChecked))
            {
                for (int i = 0; i < dgGSMV2IMSIList.Items.Count; i++)
                {
                    DataGridRow DGR = (DataGridRow)dgGSMV2IMSIList.ItemContainerGenerator.ContainerFromIndex(i);
                    if (DGR != null)
                    {
                        var itemChkBox = dgGSMV2IMSIList.Columns[0].GetCellContent(DGR);

                        GetVisualChild(itemChkBox, true);

                        IMSI = (dgGSMV2IMSIList.Items.GetItemAt(i) as GSMV2IMSIControlInfoClass).IMSI;
                        GSMV2SelectIMSIList.Add(IMSI);
                    }
                }
            }
            //返选
            else
            {
                for (int i = 0; i < dgGSMV2IMSIList.Items.Count; i++)
                {
                    DataGridRow DGR = (DataGridRow)dgGSMV2IMSIList.ItemContainerGenerator.ContainerFromIndex(i);
                    if (DGR != null)
                    {
                        var itemChkBox = dgGSMV2IMSIList.Columns[0].GetCellContent(DGR);
                        GetVisualChild(itemChkBox, false);
                    }
                    for (int j = 0; j < GSMV2SelectIMSIList.Count; j++)
                    {
                        if ((dgGSMV2IMSIList.Items.GetItemAt(i) as GSMV2IMSIControlInfoClass).IMSI == GSMV2SelectIMSIList[j])
                        {
                            GSMV2SelectIMSIList.RemoveAt(j);
                        }
                    }
                }
            }
        }

        private void IsCheckAll_Click(object sender, RoutedEventArgs e)
        {
            string IMSI = string.Empty;
            try
            {
                if ((bool)((CheckBox)sender).IsChecked)
                {
                    IMSI = (dgGSMV2IMSIList.SelectedItem as GSMV2IMSIControlInfoClass).IMSI;
                    GSMV2SelectIMSIList.Add(IMSI);
                }
                else
                {
                    IMSI = (dgGSMV2IMSIList.SelectedItem as GSMV2IMSIControlInfoClass).IMSI;

                    for (int i = 0; i < GSMV2SelectIMSIList.Count; i++)
                    {
                        if (GSMV2SelectIMSIList[i].Equals(IMSI))
                        {
                            GSMV2SelectIMSIList.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void btnUpdateTime_Click(object sender, RoutedEventArgs e)
        {

            if ((txtFirstPeriodTimeStart.Text != ""
                && txtFirstPeriodTimeEnd.Text == "")
                || (txtFirstPeriodTimeStart.Text == ""
                && txtFirstPeriodTimeEnd.Text != ""))
            {
                MessageBox.Show("第一时间段的时间不完整！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else if (txtFirstPeriodTimeStart.Text != "" && txtFirstPeriodTimeEnd.Text != "")
            {
                //检测时间
                if (!Parameters.IsTime(JsonInterFace.LteDeviceAdvanceSettingParameter.FirstPeriodTimeStart))
                {
                    MessageBox.Show("第一时间段的起始时间格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (!Parameters.IsTime(JsonInterFace.LteDeviceAdvanceSettingParameter.FirstPeriodTimeEnd))
                {
                    MessageBox.Show("第一时间段的结束时间格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            if ((txtSecondPeriodTimeStart.Text != ""
                && txtSecoondPeriodTimeEnd.Text == "")
                || (txtSecondPeriodTimeStart.Text == ""
                && txtSecoondPeriodTimeEnd.Text != ""))
            {
                MessageBox.Show("第二时间段的时间不完整！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else if (txtSecondPeriodTimeStart.Text == "" && txtSecoondPeriodTimeEnd.Text != "")
            {
                //检测时间
                if (!Parameters.IsTime(JsonInterFace.LteDeviceAdvanceSettingParameter.SecondPeriodTimeStart))
                {
                    MessageBox.Show("第二时间段的起始时间格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (!Parameters.IsTime(JsonInterFace.LteDeviceAdvanceSettingParameter.SecoondPeriodTimeEnd))
                {
                    MessageBox.Show("第二时间段的结束时间格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            if ((txtThreePeriodTimeStart.Text != ""
                && txtThreePeriodTimeEnd.Text == "")
                || (txtThreePeriodTimeStart.Text == ""
                && txtThreePeriodTimeEnd.Text != ""))
            {
                MessageBox.Show("第三时间段的时间不完整！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else if (txtThreePeriodTimeStart.Text == "" && txtThreePeriodTimeEnd.Text != "")
            {
                //检测时间
                if (!Parameters.IsTime(JsonInterFace.LteDeviceAdvanceSettingParameter.ThreePeriodTimeStart))
                {
                    MessageBox.Show("第三时间段的起始时间格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (!Parameters.IsTime(JsonInterFace.LteDeviceAdvanceSettingParameter.ThreePeriodTimeEnd))
                {
                    MessageBox.Show("第三时间段的结束时间格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            if (NetWorkClient.ControllerServer.Connected)
            {
                Dictionary<string, string> ApperiodTimeList = new Dictionary<string, string>();
                ApperiodTimeList.Add("activeTime1Start", JsonInterFace.LteDeviceAdvanceSettingParameter.FirstPeriodTimeStart);
                ApperiodTimeList.Add("activeTime1Ended", JsonInterFace.LteDeviceAdvanceSettingParameter.FirstPeriodTimeEnd);
                ApperiodTimeList.Add("activeTime2Start", JsonInterFace.LteDeviceAdvanceSettingParameter.SecondPeriodTimeStart);
                ApperiodTimeList.Add("activeTime2Ended", JsonInterFace.LteDeviceAdvanceSettingParameter.SecoondPeriodTimeEnd);
                ApperiodTimeList.Add("activeTime3Start", JsonInterFace.LteDeviceAdvanceSettingParameter.ThreePeriodTimeStart);
                ApperiodTimeList.Add("activeTime3Ended", JsonInterFace.LteDeviceAdvanceSettingParameter.ThreePeriodTimeEnd);

                string DomainFullPathName = string.Empty;
                string[] DomainFullPathNameTmp = JsonInterFace.LteDeviceAdvanceSettingParameter.DomainFullPathName.Split(new char[] { '.' });
                for (int i = 0; i < DomainFullPathNameTmp.Length - 1; i++)
                {
                    if (DomainFullPathName.Equals(""))
                    {
                        DomainFullPathName = DomainFullPathNameTmp[i];
                    }
                    else
                    {
                        DomainFullPathName += "." + DomainFullPathNameTmp[i];
                    }
                }
                Parameters.ConfigType = DeviceType.LTE;
                NetWorkClient.ControllerServer.Send(JsonInterFace.APPeriodTimeConrolRequest(
                                                                                            DomainFullPathName,
                                                                                            JsonInterFace.LteDeviceAdvanceSettingParameter.DeviceName,
                                                                                            ApperiodTimeList
                                                                                           ));
            }
            else
            {
                MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        private void rdbGSMV2Yes_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as CheckBox).IsChecked)
            {
                rdbGSMV2No.IsChecked = false;
            }
            else
            {
                if (!(bool)(sender as CheckBox).IsChecked)
                {
                    rdbGSMV2Yes.IsChecked = true;
                }
            }
        }

        private void rdbGSMV2No_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as CheckBox).IsChecked)
            {
                rdbGSMV2Yes.IsChecked = false;
            }
            else
            {
                if (!(bool)(sender as CheckBox).IsChecked)
                {
                    rdbGSMV2No.IsChecked = true;
                }
            }
        }

        /// <summary>
        /// 多载波查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCMDAMultiCarrierQuery_Click(object sender, RoutedEventArgs e)
        {
            if (NetWorkClient.ControllerServer.Connected)
            {
                JsonInterFace.ResultMessageList.Clear();
                Parameters.ConfigType = "Manul";
                NetWorkClient.ControllerServer.Send(
                                                    JsonInterFace.CDMAMultiCarrierQueryRequest(
                                                                                                    JsonInterFace.CDMADeviceParameter.DomainFullPathName,
                                                                                                    JsonInterFace.CDMADeviceParameter.DeviceName,
                                                                                                    JsonInterFace.CDMADeviceParameter.IpAddr,
                                                                                                    JsonInterFace.CDMADeviceParameter.Port,
                                                                                                    JsonInterFace.CDMADeviceParameter.InnerType,
                                                                                                    JsonInterFace.CDMADeviceParameter.SN
                                                                                                )
                                                   );
            }
            else
            {
                MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// 多载波设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCMDAMultiCarrierSetting_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (((int.Parse(JsonInterFace.CDMAMultiCarrierParameter.WARFCN4) > int.Parse(JsonInterFace.CDMAMultiCarrierParameter.WARFCN3)) &&
                     (int.Parse(JsonInterFace.CDMAMultiCarrierParameter.WARFCN3) > int.Parse(JsonInterFace.CDMAMultiCarrierParameter.WARFCN2)) &&
                     (int.Parse(JsonInterFace.CDMAMultiCarrierParameter.WARFCN2) > int.Parse(JsonInterFace.CDMAMultiCarrierParameter.WARFCN1))) ||
                    (int.Parse(JsonInterFace.CDMAMultiCarrierParameter.WARFCN4) == 119 &&
                    int.Parse(JsonInterFace.CDMAMultiCarrierParameter.WARFCN3) == 78 &&
                    int.Parse(JsonInterFace.CDMAMultiCarrierParameter.WARFCN2) == 37 &&
                    int.Parse(JsonInterFace.CDMAMultiCarrierParameter.WARFCN1) == 1019))
                {
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        JsonInterFace.ResultMessageList.Clear();
                        NetWorkClient.ControllerServer.Send(
                                                            JsonInterFace.CDMAMultiCarrierSettingRequest(
                                                                                                            JsonInterFace.CDMADeviceParameter.DomainFullPathName,
                                                                                                            JsonInterFace.CDMADeviceParameter.DeviceName,
                                                                                                            JsonInterFace.CDMADeviceParameter.IpAddr,
                                                                                                            JsonInterFace.CDMADeviceParameter.Port,
                                                                                                            JsonInterFace.CDMADeviceParameter.InnerType,
                                                                                                            JsonInterFace.CDMADeviceParameter.SN
                                                                                                        )
                                                           );
                    }
                    else
                    {
                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("要求配置频点1019,37,78,119,160,201,242,283中连续着的4个！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "设置多载波参数失败！");
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// 小区信息提交更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGSMV2CellNeighUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Dictionary<string, string> GSMV2CellNeighParams = new Dictionary<string, string>();
                Dictionary<string, string> SetWorkModeParams = new Dictionary<string, string>();
                string Carrier = string.Empty;
                if ((bool)chkGSMV2CarrierOne.IsChecked)
                {
                    Carrier = (Convert.ToInt32(JsonInterFace.GSMV2CarrierParameter.CarrierOne) - 1).ToString();
                }
                else
                {
                    Carrier = (Convert.ToInt32(JsonInterFace.GSMV2CarrierParameter.CarrierTwo)).ToString();
                }
                if (JsonInterFace.GSMV2CellNeighParameter.BPLMNId != null)
                {
                    string mcc = JsonInterFace.GSMV2CellNeighParameter.BPLMNId.Substring(0, Parameters.PLMN_Lengh - 2);
                    if (!Parameters.CheckParameters(mcc, 0, 999)) { return; }
                    string mnc = JsonInterFace.GSMV2CellNeighParameter.BPLMNId.Substring(Parameters.PLMN_Lengh - 2, JsonInterFace.GSMV2CellNeighParameter.BPLMNId.Length - (Parameters.PLMN_Lengh - 2));
                    if (!Parameters.CheckParameters(mnc, 0, 999)) { return; }
                    GSMV2CellNeighParams.Add("bPLMNId", mcc + mnc);
                }

                if (JsonInterFace.GSMV2CellNeighParameter.BTxPower != null && JsonInterFace.GSMV2CellNeighParameter.BTxPower != "")
                {
                    if (!Parameters.CheckParameters(JsonInterFace.GSMV2CellNeighParameter.BTxPower, 0, 255)) { return; }
                    GSMV2CellNeighParams.Add("bTxPower", JsonInterFace.GSMV2CellNeighParameter.BTxPower);
                }

                if (JsonInterFace.GSMV2CellNeighParameter.BRxGain != null || JsonInterFace.GSMV2CellNeighParameter.BRxGain != "")
                {
                    if (!Parameters.CheckParameters(JsonInterFace.GSMV2CellNeighParameter.BRxGain, 0, 255)) { return; }
                    GSMV2CellNeighParams.Add("bRxGain", JsonInterFace.GSMV2CellNeighParameter.BRxGain);
                }

                if (JsonInterFace.GSMV2CellNeighParameter.WPhyCellId != null || JsonInterFace.GSMV2CellNeighParameter.WPhyCellId != "")
                {
                    if (!Parameters.CheckParameters(JsonInterFace.GSMV2CellNeighParameter.WPhyCellId, 0, 511)) { return; }
                    GSMV2CellNeighParams.Add("wPhyCellId", JsonInterFace.GSMV2CellNeighParameter.WPhyCellId);
                }

                if (JsonInterFace.GSMV2CellNeighParameter.WLAC != null)
                {
                    if (!Parameters.CheckParameters(JsonInterFace.GSMV2CellNeighParameter.WLAC, 0, 65535)) { return; }
                    GSMV2CellNeighParams.Add("wLAC", JsonInterFace.GSMV2CellNeighParameter.WLAC);
                }

                if (JsonInterFace.GSMV2CellNeighParameter.WUARFCN != null)
                {
                    if (!Parameters.CheckParameters(JsonInterFace.GSMV2CellNeighParameter.WUARFCN, 0, 65535)) { return; }
                    GSMV2CellNeighParams.Add("wUARFCN", JsonInterFace.GSMV2CellNeighParameter.WUARFCN);
                }

                if (JsonInterFace.GSMV2CellNeighParameter.DwCellId != null)
                {
                    if (!Parameters.CheckParameters(JsonInterFace.GSMV2CellNeighParameter.DwCellId, 0, 429497294)) { return; }
                    GSMV2CellNeighParams.Add("dwCellId", JsonInterFace.GSMV2CellNeighParameter.DwCellId);
                }

                if (JsonInterFace.GSMV2CellNeighParameter.BWorkingMode != null)
                {
                    string _BWorkingMode = string.Empty;
                    if (JsonInterFace.GSMV2CellNeighParameter.BWorkingMode == "侦码模式")
                    {
                        _BWorkingMode = "1";
                    }
                    else if (JsonInterFace.GSMV2CellNeighParameter.BWorkingMode == "驻留模式")
                    {
                        _BWorkingMode = "3";
                    }
                    else
                    {
                        MessageBox.Show("工作模式参数[" + JsonInterFace.GSMV2CellNeighParameter.BWorkingMode + "]错误！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                        return;
                    }
                    if (!Parameters.CheckParameters(_BWorkingMode, 0, 3)) { return; }
                    GSMV2CellNeighParams.Add("bWorkingMode", _BWorkingMode);
                }

                if (JsonInterFace.GSMV2CellNeighParameter.BCYes != null || JsonInterFace.GSMV2CellNeighParameter.BCNo != null)
                {
                    if (JsonInterFace.GSMV2CellNeighParameter.BCYes == "True")
                    {
                        GSMV2CellNeighParams.Add("bC", "1");
                    }
                    else
                    {
                        GSMV2CellNeighParams.Add("bC", "0");
                    }
                }
                //GSMV2没有黑名单频点
                GSMV2CellNeighParams.Add("wRedirectCellUarfcn", "0");
                if (JsonInterFace.GSMV2CellNeighParameter.DwDateTime != null)
                {
                    GSMV2CellNeighParams.Add("dwDateTime", JsonInterFace.GSMV2CellNeighParameter.DwDateTime);
                }

                if (GSMV2CellNeighParams.Count <= 0)
                {
                    MessageBox.Show("内容未改变！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                    return;
                }
                if (MessageBox.Show("确定更新小区信息[" + JsonInterFace.GSMV2DeviceParameter.DeviceName + "]?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        JsonInterFace.ResultMessageList.Clear();
                        //小区设置
                        NetWorkClient.ControllerServer.Send(JsonInterFace.GSMV2ConfigurationFAPRequest(
                                                                                                    JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.Port,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.InnerType,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.SN,
                                                                                                    Carrier,
                                                                                                    GSMV2CellNeighParams
                                                                                                    ));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "更新失败！");
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        //(1)
        private void rdbCDMAFreqModeScannerOne_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as CheckBox).IsChecked)
            {
                rdbCDMAFreqModeNormallyOpenOne.IsChecked = false;
                rdbCDMAFreqModeClosedOne.IsChecked = false;
            }
            else
            {
                if (!(bool)(sender as CheckBox).IsChecked)
                {
                    rdbCDMAFreqModeScannerOne.IsChecked = true;
                }
            }
        }

        private void rdbCDMAFreqModeNormallyOpenOne_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as CheckBox).IsChecked)
            {
                rdbCDMAFreqModeScannerOne.IsChecked = false;
                rdbCDMAFreqModeClosedOne.IsChecked = false;
            }
            else
            {
                if (!(bool)(sender as CheckBox).IsChecked)
                {
                    rdbCDMAFreqModeNormallyOpenOne.IsChecked = true;
                }
            }
        }

        private void rdbCDMAFreqModeClosedOne_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as CheckBox).IsChecked)
            {
                rdbCDMAFreqModeScannerOne.IsChecked = false;
                rdbCDMAFreqModeNormallyOpenOne.IsChecked = false;
            }
            else
            {
                if (!(bool)(sender as CheckBox).IsChecked)
                {
                    rdbCDMAFreqModeClosedOne.IsChecked = true;
                }
            }
        }
        //(2)
        private void rdbCDMAFreqModeScannerTwo_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as CheckBox).IsChecked)
            {
                rdbCDMAFreqModeNormallyOpenTwo.IsChecked = false;
                rdbCDMAFreqModeClosedTwo.IsChecked = false;
            }
            else
            {
                if (!(bool)(sender as CheckBox).IsChecked)
                {
                    rdbCDMAFreqModeScannerTwo.IsChecked = true;
                }
            }
        }

        private void rdbCDMAFreqModeNormallyOpenTwo_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as CheckBox).IsChecked)
            {
                rdbCDMAFreqModeScannerTwo.IsChecked = false;
                rdbCDMAFreqModeClosedTwo.IsChecked = false;
            }
            else
            {
                if (!(bool)(sender as CheckBox).IsChecked)
                {
                    rdbCDMAFreqModeNormallyOpenTwo.IsChecked = true;
                }
            }
        }

        private void rdbCDMAFreqModeClosedTwo_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as CheckBox).IsChecked)
            {
                rdbCDMAFreqModeScannerTwo.IsChecked = false;
                rdbCDMAFreqModeNormallyOpenTwo.IsChecked = false;
            }
            else
            {
                if (!(bool)(sender as CheckBox).IsChecked)
                {

                    rdbCDMAFreqModeClosedTwo.IsChecked = true;
                }
            }
        }

        //(3)
        private void rdbCDMAFreqModeScannerThree_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as CheckBox).IsChecked)
            {
                rdbCDMAFreqModeNormallyOpenThree.IsChecked = false;
                rdbCDMAFreqModeClosedThree.IsChecked = false;
            }
            else
            {
                if (!(bool)(sender as CheckBox).IsChecked)
                {
                    rdbCDMAFreqModeScannerThree.IsChecked = true;
                }
            }
        }

        private void rdbCDMAFreqModeNormallyOpenThree_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as CheckBox).IsChecked)
            {
                rdbCDMAFreqModeScannerThree.IsChecked = false;
                rdbCDMAFreqModeClosedThree.IsChecked = false;
            }
            else
            {
                if (!(bool)(sender as CheckBox).IsChecked)
                {
                    rdbCDMAFreqModeNormallyOpenThree.IsChecked = true;
                }
            }
        }

        private void rdbCDMAFreqModeClosedThree_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as CheckBox).IsChecked)
            {
                rdbCDMAFreqModeScannerThree.IsChecked = false;
                rdbCDMAFreqModeNormallyOpenThree.IsChecked = false;
            }
            else
            {
                if (!(bool)(sender as CheckBox).IsChecked)
                {

                    rdbCDMAFreqModeClosedThree.IsChecked = true;
                }
            }
        }
        //(4)
        private void rdbCDMAFreqModeScannerFour_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as CheckBox).IsChecked)
            {
                rdbCDMAFreqModeNormallyOpenFour.IsChecked = false;
                rdbCDMAFreqModeClosedFour.IsChecked = false;
            }
            else
            {
                if (!(bool)(sender as CheckBox).IsChecked)
                {
                    rdbCDMAFreqModeScannerFour.IsChecked = true;
                }
            }
        }

        private void rdbCDMAFreqModeNormallyOpenFour_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as CheckBox).IsChecked)
            {
                rdbCDMAFreqModeScannerFour.IsChecked = false;
                rdbCDMAFreqModeClosedFour.IsChecked = false;
            }
            else
            {
                if (!(bool)(sender as CheckBox).IsChecked)
                {
                    rdbCDMAFreqModeNormallyOpenFour.IsChecked = true;
                }
            }
        }

        private void rdbCDMAFreqModeClosedFour_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as CheckBox).IsChecked)
            {
                rdbCDMAFreqModeScannerFour.IsChecked = false;
                rdbCDMAFreqModeNormallyOpenFour.IsChecked = false;
            }
            else
            {
                if (!(bool)(sender as CheckBox).IsChecked)
                {

                    rdbCDMAFreqModeClosedFour.IsChecked = true;
                }
            }
        }

        private void txtCDMAIMSIInput_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            CDMAIMSIListInputWin = new SubWindow.CDMAIMSIListInputWindow();
            CDMAIMSIListInputWin.ShowDialog();

            if (SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Count <= 0)
            {
                txtCDMAIMSIInput.Text = "";
                txtCDMAIMSIInput.IsReadOnly = false;
            }
        }

        /// <summary>
        /// 导入IMSI文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCDMAIMSIInput_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "(文本文件 *.txt)|*.txt";
            int Num = 0;
            try
            {
                if ((bool)openFileDialog.ShowDialog())
                {
                    if ((new FileInfo(openFileDialog.FileName).Extension).ToLower() == ".txt")
                    {
                        string IMSIFileName = openFileDialog.FileName;
                        string[] IMSISoureList = File.ReadAllLines(openFileDialog.FileName);
                        if (SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Count > 0) { SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Clear(); }
                        for (int i = 0; i < IMSISoureList.Length; i++)
                        {
                            SubWindow.CDMAIMSIListInputWindow.IMSIControlInfoClass CDMAIMSI = new SubWindow.CDMAIMSIListInputWindow.IMSIControlInfoClass();
                            if (new Regex(@"\d{15}\,[1-5]").Match(IMSISoureList[i]).Success)
                            {
                                if (IMSISoureList[i].Trim().Length == 17)
                                {
                                    CDMAIMSI.IMSI = IMSISoureList[i].Split(new char[] { ',' })[0];
                                    if (IMSISoureList[i].Split(new char[] { ',' })[1] == "1")
                                    {
                                        CDMAIMSI.ActionFlag = "Reject";
                                    }
                                    else if (IMSISoureList[i].Split(new char[] { ',' })[1] == "5")
                                    {
                                        CDMAIMSI.ActionFlag = "Hold ON";
                                    }
                                    SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Add(CDMAIMSI);
                                }
                                else
                                {
                                    Num++;
                                }
                            }
                            else
                            {
                                Num++;
                            }
                        }

                        if (SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Count > 0)
                        {
                            txtCDMAIMSIInput.Text = "IMSI号已导入...";
                            txtCDMAIMSIInput.IsReadOnly = true;

                            if (Num > 0)
                            {
                                MessageBox.Show("文件中共有[" + IMSISoureList.Length.ToString() + "]条数据，成功导入[" + SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Count.ToString() + "]条IMSI数据导入,其中[" + Num.ToString() + "]条非IMSI格式，未被导入！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            else
                            {
                                MessageBox.Show("文件中共有[" + IMSISoureList.Length.ToString() + "]条数据，[" + SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Count.ToString() + "]条IMSI数据导入成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                        else
                        {
                            txtCDMAIMSIInput.Text = "";
                            txtCDMAIMSIInput.IsReadOnly = false;

                            if (Num > 0)
                            {
                                MessageBox.Show("文件中共有[" + IMSISoureList.Length.ToString() + "]条数据，成功导入[" + SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Count.ToString() + "]条IMSI数据导入,其中[" + Num.ToString() + "]条非IMSI格式，未被导入！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            else
                            {
                                MessageBox.Show("文件中共有[" + IMSISoureList.Length.ToString() + "]条数据！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("导入文件只支持文本文件！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("IMSI导入失败！" + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnCDMAIMSISubmit_Click(object sender, RoutedEventArgs e)
        {
            string segmentType = string.Empty;
            string ActionType = string.Empty;
            string ActionFlag = string.Empty;
            Dictionary<string, string> ParaList = new Dictionary<string, string>();

            try
            {
                if (!IsOnline)
                {
                    MessageBox.Show("设备[" + JsonInterFace.CDMADeviceParameter.DomainFullPathName + "." + JsonInterFace.CDMADeviceParameter.DeviceName + "]不在线！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                //清空
                if (cbbCDMAIMSIActionType.SelectedIndex == 0)
                {
                    Parameters.ConfigType = cbbCDMAIMSIActionType.Text;
                    ParaList.Clear();
                    ParaList.Add("wTotalImsi", "0");
                    ParaList.Add("bSegmentType", "0");
                    ParaList.Add("bActionType", (cbbCDMAIMSIActionType.SelectedIndex + 1).ToString());

                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        JsonInterFace.ResultMessageList.Clear();
                        NetWorkClient.ControllerServer.Send(
                                                            JsonInterFace.CDMAIMSIActionRequest(
                                                                                                JsonInterFace.CDMADeviceParameter.DomainFullPathName,
                                                                                                JsonInterFace.CDMADeviceParameter.DeviceName,
                                                                                                JsonInterFace.CDMADeviceParameter.IpAddr,
                                                                                                JsonInterFace.CDMADeviceParameter.Port,
                                                                                                JsonInterFace.CDMADeviceParameter.InnerType,
                                                                                                JsonInterFace.CDMADeviceParameter.SN,
                                                                                                ParaList
                                                                                               )
                                                           );
                    }
                    else
                    {
                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                //删除(部分删除)
                else if (cbbCDMAIMSIActionType.SelectedIndex == 1)
                {
                    //单个IMSI
                    if (SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Count <= 0)
                    {
                        //IMSI
                        if (JsonInterFace.CDMAIMSIControlInfo.IMSI == "" || JsonInterFace.CDMAIMSIControlInfo.IMSI == null)
                        {
                            txtCDMAIMSIInput.Focus();
                            MessageBox.Show("请输入IMSI号！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        else if (!new Regex(@"^\d{15}$").Match(JsonInterFace.CDMAIMSIControlInfo.IMSI).Success)
                        {
                            txtCDMAIMSIInput.Focus();
                            txtCDMAIMSIInput.SelectAll();
                            MessageBox.Show("IMSI号[" + JsonInterFace.CDMAIMSIControlInfo.IMSI + "]格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        else if (JsonInterFace.CDMAIMSIControlInfo.IMSI.Length < 15)
                        {
                            txtCDMAIMSIInput.Focus();
                            txtCDMAIMSIInput.SelectAll();
                            MessageBox.Show("IMSI号[" + JsonInterFace.CDMAIMSIControlInfo.IMSI + "]格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        //分段类型
                        segmentType = "0";

                        //操作类型
                        if (cbbCDMAIMSIActionType.SelectedIndex <= 0)
                        {
                            MessageBox.Show("请选择操作类型！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        else
                        {
                            ActionType = (cbbCDMAIMSIActionType.SelectedIndex + 1).ToString();
                            Parameters.ConfigType = cbbCDMAIMSIActionType.Text;
                        }

                        //IMSI动作标示
                        if (cbbCDMAIMSIActionFlag.SelectedIndex < 0)
                        {
                            MessageBox.Show("请选择IMSI动作类型！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        else
                        {
                            if (cbbCDMAIMSIActionFlag.SelectedIndex == 0)
                            {
                                ActionFlag = (cbbCDMAIMSIActionFlag.SelectedIndex + 1).ToString();
                            }
                            else if (cbbCDMAIMSIActionFlag.SelectedIndex == 5)
                            {
                                ActionFlag = (cbbCDMAIMSIActionFlag.SelectedIndex + 5).ToString();
                            }
                        }

                        ParaList.Clear();
                        ParaList.Add("wTotalImsi", "1");
                        ParaList.Add("bSegmentType", segmentType);
                        ParaList.Add("bActionType", ActionType);
                        ParaList.Add("bIMSI_#0#", JsonInterFace.CDMAIMSIControlInfo.IMSI);
                        ParaList.Add("bUeActionFlag_#0#", ActionFlag);

                        if (NetWorkClient.ControllerServer.Connected)
                        {
                            JsonInterFace.ResultMessageList.Clear();
                            NetWorkClient.ControllerServer.Send(
                                                                JsonInterFace.CDMAIMSIActionRequest(
                                                                                                    JsonInterFace.CDMADeviceParameter.DomainFullPathName,
                                                                                                    JsonInterFace.CDMADeviceParameter.DeviceName,
                                                                                                    JsonInterFace.CDMADeviceParameter.IpAddr,
                                                                                                    JsonInterFace.CDMADeviceParameter.Port,
                                                                                                    JsonInterFace.CDMADeviceParameter.InnerType,
                                                                                                    JsonInterFace.CDMADeviceParameter.SN,
                                                                                                    ParaList
                                                                                                   )
                                                               );
                        }
                        else
                        {
                            MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }
                    //多个IMSI
                    else if (SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Count > 0)
                    {
                        //分段类型
                        if (cbbCDMAIMSISegmentType.SelectedIndex < 0 && SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Count > 50)
                        {
                            MessageBox.Show("请选择分段类型！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        else if (SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Count <= 50)
                        {
                            segmentType = "0";
                        }
                        else
                        {
                            segmentType = (cbbCDMAIMSISegmentType.SelectedIndex + 1).ToString();
                        }

                        //操作类型
                        if (cbbCDMAIMSIActionType.SelectedIndex <= 0)
                        {
                            MessageBox.Show("请选择操作类型！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        else
                        {
                            ActionType = (cbbCDMAIMSIActionType.SelectedIndex + 1).ToString();
                            Parameters.ConfigType = cbbCDMAIMSIActionType.Text;
                        }

                        ParaList.Clear();
                        ParaList.Add("wTotalImsi", SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Count.ToString());
                        ParaList.Add("bSegmentType", segmentType);
                        ParaList.Add("bActionType", ActionType);
                        for (int i = 0; i < SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Count; i++)
                        {
                            string Flag = string.Empty;
                            if (SubWindow.CDMAIMSIListInputWindow.IMSIInfoList[i].ActionFlag == "Reject")
                            {
                                Flag = "1";
                            }
                            else if (SubWindow.CDMAIMSIListInputWindow.IMSIInfoList[i].ActionFlag == "Hold ON")
                            {
                                Flag = "5";
                            }
                            else
                            {
                                Flag = "1";
                            }
                            ParaList.Add("bIMSI_#" + i.ToString() + "#", SubWindow.CDMAIMSIListInputWindow.IMSIInfoList[i].IMSI);
                            ParaList.Add("bUeActionFlag_#" + i.ToString() + "#", Flag);
                        }

                        if (NetWorkClient.ControllerServer.Connected)
                        {
                            JsonInterFace.ResultMessageList.Clear();
                            NetWorkClient.ControllerServer.Send(
                                                                JsonInterFace.CDMAIMSIActionRequest(
                                                                                                    JsonInterFace.CDMADeviceParameter.DomainFullPathName,
                                                                                                    JsonInterFace.CDMADeviceParameter.DeviceName,
                                                                                                    JsonInterFace.CDMADeviceParameter.IpAddr,
                                                                                                    JsonInterFace.CDMADeviceParameter.Port,
                                                                                                    JsonInterFace.CDMADeviceParameter.InnerType,
                                                                                                    JsonInterFace.CDMADeviceParameter.SN,
                                                                                                    ParaList
                                                                                                   )
                                                               );
                        }
                        else
                        {
                            MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                    }
                }
                //添加
                else if (cbbCDMAIMSIActionType.SelectedIndex == 2)
                {
                    //单个IMSI
                    if (SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Count <= 0)
                    {
                        //IMSI
                        if (JsonInterFace.CDMAIMSIControlInfo.IMSI == "" || JsonInterFace.CDMAIMSIControlInfo.IMSI == null)
                        {
                            txtCDMAIMSIInput.Focus();
                            MessageBox.Show("请输入IMSI号！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        else if (!new Regex(@"^\d{15}$").Match(JsonInterFace.CDMAIMSIControlInfo.IMSI).Success)
                        {
                            txtCDMAIMSIInput.Focus();
                            txtCDMAIMSIInput.SelectAll();
                            MessageBox.Show("IMSI号[" + JsonInterFace.CDMAIMSIControlInfo.IMSI + "]格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        else if (JsonInterFace.CDMAIMSIControlInfo.IMSI.Length < 15)
                        {
                            txtCDMAIMSIInput.Focus();
                            txtCDMAIMSIInput.SelectAll();
                            MessageBox.Show("IMSI号[" + JsonInterFace.CDMAIMSIControlInfo.IMSI + "]格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        //分段类型
                        segmentType = "0";

                        //操作类型
                        if (cbbCDMAIMSIActionType.SelectedIndex <= 0)
                        {
                            MessageBox.Show("请选择操作类型！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        else
                        {
                            ActionType = (cbbCDMAIMSIActionType.SelectedIndex + 1).ToString();
                            Parameters.ConfigType = cbbCDMAIMSIActionType.Text;
                        }

                        //IMSI动作标示
                        if (cbbCDMAIMSIActionFlag.SelectedIndex < 0)
                        {
                            MessageBox.Show("请选择IMSI动作类型！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        else
                        {
                            if (cbbCDMAIMSIActionFlag.SelectedIndex == 0)
                            {
                                ActionFlag = (cbbCDMAIMSIActionFlag.SelectedIndex + 1).ToString();
                            }
                            else if (cbbCDMAIMSIActionFlag.SelectedIndex == 5)
                            {
                                ActionFlag = (cbbCDMAIMSIActionFlag.SelectedIndex + 5).ToString();
                            }
                        }

                        ParaList.Clear();
                        ParaList.Add("wTotalImsi", "1");
                        ParaList.Add("bSegmentType", segmentType);
                        ParaList.Add("bActionType", ActionType);
                        ParaList.Add("bIMSI_#0#", JsonInterFace.CDMAIMSIControlInfo.IMSI);
                        ParaList.Add("bUeActionFlag_#0#", ActionFlag);

                        if (NetWorkClient.ControllerServer.Connected)
                        {
                            JsonInterFace.ResultMessageList.Clear();
                            NetWorkClient.ControllerServer.Send(
                                                                JsonInterFace.CDMAIMSIActionRequest(
                                                                                                    JsonInterFace.CDMADeviceParameter.DomainFullPathName,
                                                                                                    JsonInterFace.CDMADeviceParameter.DeviceName,
                                                                                                    JsonInterFace.CDMADeviceParameter.IpAddr,
                                                                                                    JsonInterFace.CDMADeviceParameter.Port,
                                                                                                    JsonInterFace.CDMADeviceParameter.InnerType,
                                                                                                    JsonInterFace.CDMADeviceParameter.SN,
                                                                                                    ParaList
                                                                                                   )
                                                               );
                        }
                        else
                        {
                            MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }
                    //多个IMSI
                    else if (SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Count > 0)
                    {
                        //分段类型
                        if (cbbCDMAIMSISegmentType.SelectedIndex < 0 && SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Count > 50)
                        {
                            MessageBox.Show("请选择分段类型！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        else if (SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Count <= 50)
                        {
                            segmentType = "0";
                        }
                        else
                        {
                            segmentType = (cbbCDMAIMSISegmentType.SelectedIndex + 1).ToString();
                        }

                        //操作类型
                        if (cbbCDMAIMSIActionType.SelectedIndex <= 0)
                        {
                            MessageBox.Show("请选择操作类型！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        else
                        {
                            ActionType = (cbbCDMAIMSIActionType.SelectedIndex + 1).ToString();
                            Parameters.ConfigType = cbbCDMAIMSIActionType.Text;
                        }

                        ParaList.Clear();
                        ParaList.Add("wTotalImsi", SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Count.ToString());
                        ParaList.Add("bSegmentType", segmentType);
                        ParaList.Add("bActionType", ActionType);
                        for (int i = 0; i < SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Count; i++)
                        {
                            string Flag = string.Empty;
                            if (SubWindow.CDMAIMSIListInputWindow.IMSIInfoList[i].ActionFlag == "Reject")
                            {
                                Flag = "1";
                            }
                            else if (SubWindow.CDMAIMSIListInputWindow.IMSIInfoList[i].ActionFlag == "Hold ON")
                            {
                                Flag = "5";
                            }
                            else
                            {
                                Flag = "1";
                            }
                            ParaList.Add("bIMSI_#" + i.ToString() + "#", SubWindow.CDMAIMSIListInputWindow.IMSIInfoList[i].IMSI);
                            ParaList.Add("bUeActionFlag_#" + i.ToString() + "#", Flag);
                        }

                        if (NetWorkClient.ControllerServer.Connected)
                        {
                            JsonInterFace.ResultMessageList.Clear();
                            NetWorkClient.ControllerServer.Send(
                                                                JsonInterFace.CDMAIMSIActionRequest(
                                                                                                    JsonInterFace.CDMADeviceParameter.DomainFullPathName,
                                                                                                    JsonInterFace.CDMADeviceParameter.DeviceName,
                                                                                                    JsonInterFace.CDMADeviceParameter.IpAddr,
                                                                                                    JsonInterFace.CDMADeviceParameter.Port,
                                                                                                    JsonInterFace.CDMADeviceParameter.InnerType,
                                                                                                    JsonInterFace.CDMADeviceParameter.SN,
                                                                                                    ParaList
                                                                                                   )
                                                               );
                        }
                        else
                        {
                            MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                    }
                }
                //查询
                else if (cbbCDMAIMSIActionType.SelectedIndex == 3)
                {
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        JsonInterFace.ResultMessageList.Clear();
                        NetWorkClient.ControllerServer.Send(
                                                            JsonInterFace.CDMAIMSIQueryRequest(
                                                                                                JsonInterFace.CDMADeviceParameter.DomainFullPathName,
                                                                                                JsonInterFace.CDMADeviceParameter.DeviceName,
                                                                                                JsonInterFace.CDMADeviceParameter.IpAddr,
                                                                                                JsonInterFace.CDMADeviceParameter.Port,
                                                                                                JsonInterFace.CDMADeviceParameter.InnerType,
                                                                                                JsonInterFace.CDMADeviceParameter.SN
                                                                                               )
                                                           );
                    }
                    else
                    {
                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void txtCDMAbPLMNId_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                JsonInterFace.CDMACellNeighParameter.Operators = JsonInterFace.OperatorsList.GetOperators(txtCDMAbPLMNId.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void txtCDMAdwDateTime_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("只用于查询时显示时间", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void chkGSMV2CarrierOne_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((bool)chkGSMV2CarrierOne.IsChecked)
                {
                    chkGSMV2CarrierTwo.IsChecked = false;
                }
                else
                {
                    if (!(bool)chkGSMV2CarrierTwo.IsChecked)
                    {
                        chkGSMV2CarrierOne.IsChecked = true;
                    }
                }

                //获取所选设备参数(GSMV2)
                if (NetWorkClient.ControllerServer.Connected)
                {
                    NetWorkClient.ControllerServer.Send(JsonInterFace.GSMV2CellPARAMRequest(JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                            JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                            JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                                                                            JsonInterFace.GSMV2DeviceParameter.Port,
                                                                                            JsonInterFace.GSMV2DeviceParameter.InnerType,
                                                                                            JsonInterFace.GSMV2DeviceParameter.SN,
                                                                                            (Convert.ToInt32(JsonInterFace.GSMV2CarrierParameter.CarrierOne) - 1).ToString()));
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void chkGSMV2CarrierTwo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((bool)chkGSMV2CarrierTwo.IsChecked)
                {
                    chkGSMV2CarrierOne.IsChecked = false;
                }
                else
                {
                    if (!(bool)chkGSMV2CarrierOne.IsChecked)
                    {
                        chkGSMV2CarrierTwo.IsChecked = true;
                    }
                }

                //获取所选设备参数(GSMV2)
                if (NetWorkClient.ControllerServer.Connected)
                {
                    NetWorkClient.ControllerServer.Send(JsonInterFace.GSMV2CellPARAMRequest(JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                            JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                            JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                                                                            JsonInterFace.GSMV2DeviceParameter.Port,
                                                                                            JsonInterFace.GSMV2DeviceParameter.InnerType,
                                                                                            JsonInterFace.GSMV2DeviceParameter.SN,
                                                                                            (Convert.ToInt32(JsonInterFace.GSMV2CarrierParameter.CarrierTwo)).ToString()));
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void UpdateIMSIList(string Action)
        {
            if (DeviceType.CDMA == Model)
            {
                if (Action == "Delete All")
                {
                    IMSILists.Clear();
                }
                else if (Action == "Delete Special")
                {
                    if (SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Count > 0)
                    {
                        for (int i = 0; i < SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Count; i++)
                        {
                            for (int j = 0; j < IMSILists.Count; j++)
                            {
                                if (SubWindow.CDMAIMSIListInputWindow.IMSIInfoList[i].IMSI == IMSILists[j].IMSI)
                                {
                                    IMSILists.RemoveAt(i);
                                    break;
                                }
                            }
                        }
                        SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < IMSILists.Count; j++)
                        {
                            if (JsonInterFace.CDMAIMSIControlInfo.IMSI == IMSILists[j].IMSI)
                            {
                                IMSILists.RemoveAt(j);
                                break;
                            }
                        }
                    }
                    JsonInterFace.CDMAIMSIControlInfo.IMSI = null;
                }
                else if (Action == "Add")
                {
                    if (SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Count > 0)
                    {
                        for (int i = 0; i < SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Count; i++)
                        {
                            bool IMSIDump = false;
                            for (int j = 0; j < IMSILists.Count; j++)
                            {
                                if (SubWindow.CDMAIMSIListInputWindow.IMSIInfoList[i].IMSI == IMSILists[j].IMSI)
                                {
                                    IMSIDump = true;
                                    break;
                                }
                            }

                            if (!IMSIDump)
                            {
                                CDMAIMSIControlInfoClass IMSIInfo = new CDMAIMSIControlInfoClass();
                                IMSIInfo.IMSI = SubWindow.CDMAIMSIListInputWindow.IMSIInfoList[i].IMSI;
                                IMSIInfo.ActionFlag = SubWindow.CDMAIMSIListInputWindow.IMSIInfoList[i].ActionFlag;
                                IMSILists.Add(IMSIInfo);
                            }
                        }
                        SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Clear();
                    }
                    else
                    {
                        bool IMSIDump = false;
                        for (int j = 0; j < IMSILists.Count; j++)
                        {
                            if (JsonInterFace.CDMAIMSIControlInfo.IMSI == IMSILists[j].IMSI)
                            {
                                IMSIDump = true;
                                break;
                            }
                        }

                        if (!IMSIDump)
                        {
                            CDMAIMSIControlInfoClass IMSIInfo = new CDMAIMSIControlInfoClass();
                            IMSIInfo.IMSI = JsonInterFace.CDMAIMSIControlInfo.IMSI;
                            IMSIInfo.ActionFlag = cbbCDMAIMSIActionFlag.Text;
                            IMSILists.Add(IMSIInfo);
                        }
                    }
                    JsonInterFace.CDMAIMSIControlInfo.IMSI = null;
                }
                else if (Action == "Query")
                {
                    if (JsonInterFace.CDMAIMSIControlInfo.IMSIListTab.Rows.Count > 0)
                    {
                        IMSILists.Clear();
                        for (int i = 0; i < JsonInterFace.CDMAIMSIControlInfo.IMSIListTab.Rows.Count; i++)
                        {
                            CDMAIMSIControlInfoClass IMSIInfo = new CDMAIMSIControlInfoClass();
                            IMSIInfo.IMSI = JsonInterFace.CDMAIMSIControlInfo.IMSIListTab.Rows[i][0].ToString();
                            if (JsonInterFace.CDMAIMSIControlInfo.IMSIListTab.Rows[i][1].ToString() == "1")
                            {
                                IMSIInfo.ActionFlag = "Reject";
                            }
                            else if (JsonInterFace.CDMAIMSIControlInfo.IMSIListTab.Rows[i][1].ToString() == "5")
                            {
                                IMSIInfo.ActionFlag = "Hold ON";
                            }
                            else
                            {
                                IMSIInfo.ActionFlag = JsonInterFace.CDMAIMSIControlInfo.IMSIListTab.Rows[i][1].ToString();
                            }
                            IMSILists.Add(IMSIInfo);
                        }
                    }
                }
            }
            else if (DeviceType.GSMV2 == Model)
            {
                if (Action == "Delete All")
                {
                    GSMV2IMSILists.Clear();
                    GSMV2SelectIMSIList.Clear();
                    txtGSMV2IMSIInput.Text = "";
                    txtGSMV2IMSIInput.IsReadOnly = false;
                    txtGSMV2IMSIInput.Focus();
                }
                else if (Action == "Delete Special")
                {
                    for (int i = 0; i < GSMV2SelectIMSIList.Count; i++)
                    {
                        for (int j = 0; j < GSMV2IMSILists.Count; j++)
                        {
                            if (GSMV2SelectIMSIList[i] == GSMV2IMSILists[j].IMSI)
                            {
                                GSMV2IMSILists.RemoveAt(j);
                                break;
                            }
                        }
                    }
                    GSMV2SelectIMSIList.Clear();
                    txtGSMV2IMSIInput.Text = "";
                    txtGSMV2IMSIInput.IsReadOnly = false;
                    txtGSMV2IMSIInput.Focus();
                }
                else if (Action == "Add")
                {
                    SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Clear();
                    txtGSMV2IMSIInput.Text = "";
                    txtGSMV2IMSIInput.IsReadOnly = false;
                    txtGSMV2IMSIInput.Focus();
                }
                else if (Action == "Query")
                {
                    if (JsonInterFace.GSMV2IMSIControlInfo.GSMV2IMSIListTab.Rows.Count > 0)
                    {
                        GSMV2IMSILists.Clear();
                        for (int i = 0; i < JsonInterFace.GSMV2IMSIControlInfo.GSMV2IMSIListTab.Rows.Count; i++)
                        {
                            GSMV2IMSIControlInfoClass IMSIInfo = new GSMV2IMSIControlInfoClass();
                            IMSIInfo.ID = JsonInterFace.GSMV2IMSIControlInfo.GSMV2IMSIListTab.Rows[i][0].ToString();
                            IMSIInfo.IMSI = JsonInterFace.GSMV2IMSIControlInfo.GSMV2IMSIListTab.Rows[i][1].ToString();
                            if (JsonInterFace.GSMV2IMSIControlInfo.GSMV2IMSIListTab.Rows[i][2].ToString() == "1")
                            {
                                IMSIInfo.ActionFlag = "白名单";
                            }
                            else if (JsonInterFace.GSMV2IMSIControlInfo.GSMV2IMSIListTab.Rows[i][2].ToString() == "5")
                            {
                                IMSIInfo.ActionFlag = "黑名单";
                            }
                            else
                            {
                                IMSIInfo.ActionFlag = JsonInterFace.GSMV2IMSIControlInfo.GSMV2IMSIListTab.Rows[i][2].ToString();
                            }
                            GSMV2IMSILists.Add(IMSIInfo);
                        }
                        MessageBox.Show("共查询IMSI[" + GSMV2IMSILists.Count.ToString() + "]条！", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    }
                    else
                    {
                        MessageBox.Show("共查询IMSI[0]条！", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    }
                    JsonInterFace.GSMV2IMSIControlInfo.IMSIListTab.Clear();
                    txtGSMV2IMSIInput.Text = "";
                    txtGSMV2IMSIInput.IsReadOnly = false;
                    txtGSMV2IMSIInput.Focus();
                }
            }
        }

        private void txtGSMV2dwDateTime_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("只用于查询时显示时间", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void txtGSMV2bPLMNId_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                JsonInterFace.GSMV2CellNeighParameter.Operators = JsonInterFace.OperatorsList.GetOperators(txtGSMV2bPLMNId.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnGSMV2IMSISubmit_Click(object sender, RoutedEventArgs e)
        {
            string ActionType = string.Empty;
            string ActionFlag = string.Empty;
            Dictionary<string, string> ParaList = new Dictionary<string, string>();
            string Carrier = string.Empty;
            try
            {
                JsonInterFace.GSMV2IMSIControlInfo.IsSucess = true;
                if ((bool)cbGSMV2IMSICarrierTwo.IsChecked)
                {
                    Carrier = "1";
                }
                else if ((bool)cbGSMV2IMSICarrierOne.IsChecked)
                {
                    Carrier = "0";
                }
                else
                {
                    MessageBox.Show("载波选择出错", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (!IsOnline)
                {
                    MessageBox.Show("设备[" + JsonInterFace.GSMV2DeviceParameter.DomainFullPathName + "." + JsonInterFace.GSMV2DeviceParameter.DeviceName + "]不在线！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                //清空
                if (cbbGSMV2IMSIActionType.SelectedIndex == 0)
                {
                    Parameters.ConfigType = "Delete All";
                    ParaList.Clear();
                    ParaList.Add("wTotalImsi", "0");
                    ParaList.Add("bSegmentType", "0");
                    ParaList.Add("bActionType", (cbbGSMV2IMSIActionType.SelectedIndex + 1).ToString());

                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        JsonInterFace.ResultMessageList.Clear();
                        NetWorkClient.ControllerServer.Send(
                                                            JsonInterFace.GSMV2IMSIActionRequest(
                                                                                                JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                                JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                                JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                                                                                JsonInterFace.GSMV2DeviceParameter.Port,
                                                                                                JsonInterFace.GSMV2DeviceParameter.InnerType,
                                                                                                JsonInterFace.GSMV2DeviceParameter.SN,
                                                                                                ParaList,
                                                                                                Carrier
                                                                                               )
                                                           );
                    }
                    else
                    {
                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                //删除(部分删除)
                else if (cbbGSMV2IMSIActionType.SelectedIndex == 1)
                {
                    Parameters.ConfigType = "Delete Special";
                    //手动输入IMSI
                    if (SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Count <= 0)
                    {
                        if (txtGSMV2IMSIInput.Text.Trim() != null)
                        {
                            if (!new Regex(@"^\d{15}$").Match(txtGSMV2IMSIInput.Text.Trim()).Success)
                            {
                                txtGSMV2IMSIInput.Focus();
                                txtGSMV2IMSIInput.SelectAll();
                                MessageBox.Show("IMSI号[" + txtGSMV2IMSIInput.Text.Trim() + "]格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }
                            else if (txtGSMV2IMSIInput.Text.Trim().Length < 15)
                            {
                                txtGSMV2IMSIInput.Focus();
                                txtGSMV2IMSIInput.SelectAll();
                                MessageBox.Show("IMSI号[" + txtGSMV2IMSIInput.Text.Trim() + "]格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }
                            GSMV2SelectIMSIList.Add(txtGSMV2IMSIInput.Text.Trim());
                        }
                    }
                    //导入IMSI
                    else if (SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Count > 0)
                    {
                        for (int i = 0; i < SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Count; i++)
                        {
                            GSMV2SelectIMSIList.Add(SubWindow.CDMAIMSIListInputWindow.IMSIInfoList[i].IMSI);
                        }
                    }
                    //去除重复的IMSI
                    DeleteSameIMSI(GSMV2SelectIMSIList);
                    //循环添加
                    if (GSMV2SelectIMSIList.Count > 0)
                    {
                        string Flag = string.Empty;
                        if (cbbGSMV2IMSIActionFlag.SelectedIndex == 0)
                        {
                            Flag = "1";
                        }
                        else if (cbbGSMV2IMSIActionFlag.SelectedIndex == 1)
                        {
                            Flag = "5";
                        }
                        else
                        {
                            Flag = "1";
                        }
                        //操作类型
                        ActionType = (cbbGSMV2IMSIActionType.SelectedIndex + 1).ToString();


                        ParaList.Clear();
                        ParaList.Add("wTotalImsi", GSMV2SelectIMSIList.Count.ToString());
                        ParaList.Add("bActionType", ActionType);

                        //IMSI动作标识及IMSI号
                        if (cbbGSMV2IMSIActionFlag.SelectedIndex < 0)
                        {
                            MessageBox.Show("请选择IMSI动作类型！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        else
                        {
                            for (int i = 0; i < GSMV2SelectIMSIList.Count; i++)
                            {
                                ParaList.Add("bIMSI_#" + i.ToString() + "#", GSMV2SelectIMSIList[i]);
                                ParaList.Add("bUeActionFlag_#" + i.ToString() + "#", Flag);
                            }
                        }

                        if (NetWorkClient.ControllerServer.Connected)
                        {
                            JsonInterFace.ResultMessageList.Clear();
                            NetWorkClient.ControllerServer.Send(
                                                                JsonInterFace.GSMV2IMSIActionRequest(
                                                                                                    JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.Port,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.InnerType,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.SN,
                                                                                                    ParaList,
                                                                                                    Carrier
                                                                                                   )
                                                               );
                        }
                        else
                        {
                            MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                    }
                    else
                    {
                        MessageBox.Show("没有写入删除的IMSI", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                //添加
                else if (cbbGSMV2IMSIActionType.SelectedIndex == 2)
                {
                    Parameters.ConfigType = "Add";
                    //单个IMSI
                    if (SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Count <= 0)
                    {
                        //IMSI
                        if (txtGSMV2IMSIInput.Text.Trim() == "" || txtGSMV2IMSIInput.Text.Trim() == null)
                        {
                            txtGSMV2IMSIInput.Focus();
                            MessageBox.Show("请输入IMSI号！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        else if (!new Regex(@"^\d{15}$").Match(txtGSMV2IMSIInput.Text.Trim()).Success)
                        {
                            txtGSMV2IMSIInput.Focus();
                            txtGSMV2IMSIInput.SelectAll();
                            MessageBox.Show("IMSI号[" + txtGSMV2IMSIInput.Text.Trim() + "]格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        else if (txtGSMV2IMSIInput.Text.Trim().Length < 15)
                        {
                            txtGSMV2IMSIInput.Focus();
                            txtGSMV2IMSIInput.SelectAll();
                            MessageBox.Show("IMSI号[" + txtGSMV2IMSIInput.Text.Trim() + "]格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        //操作类型
                        ActionType = (cbbGSMV2IMSIActionType.SelectedIndex + 1).ToString();

                        //IMSI动作标示
                        if (cbbGSMV2IMSIActionFlag.SelectedIndex < 0)
                        {
                            MessageBox.Show("请选择IMSI动作类型！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        else
                        {
                            if (cbbGSMV2IMSIActionFlag.SelectedIndex == 0)
                            {
                                ActionFlag = (cbbGSMV2IMSIActionFlag.SelectedIndex + 1).ToString();
                            }
                            else if (cbbGSMV2IMSIActionFlag.SelectedIndex == 1)
                            {
                                ActionFlag = (cbbGSMV2IMSIActionFlag.SelectedIndex + 4).ToString();
                            }
                        }

                        ParaList.Clear();
                        ParaList.Add("wTotalImsi", "1");
                        ParaList.Add("bActionType", ActionType);
                        ParaList.Add("bIMSI_#0#", txtGSMV2IMSIInput.Text.Trim());
                        ParaList.Add("bUeActionFlag_#0#", ActionFlag);

                        if (NetWorkClient.ControllerServer.Connected)
                        {
                            JsonInterFace.ResultMessageList.Clear();
                            NetWorkClient.ControllerServer.Send(
                                                                JsonInterFace.GSMV2IMSIActionRequest(
                                                                                                    JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.Port,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.InnerType,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.SN,
                                                                                                    ParaList,
                                                                                                    Carrier
                                                                                                   )
                                                               );
                        }
                        else
                        {
                            MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }
                    //多个IMSI
                    else if (SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Count > 0)
                    {
                        //操作类型
                        ActionType = (cbbGSMV2IMSIActionType.SelectedIndex + 1).ToString();

                        ParaList.Clear();
                        ParaList.Add("wTotalImsi", SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Count.ToString());
                        ParaList.Add("bActionType", ActionType);

                        //IMSI动作标识及IMSI号
                        if (cbbGSMV2IMSIActionFlag.SelectedIndex < 0)
                        {
                            MessageBox.Show("请选择IMSI动作类型！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        else
                        {
                            for (int i = 0; i < SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Count; i++)
                            {
                                string Flag = string.Empty;
                                if (cbbGSMV2IMSIActionFlag.SelectedIndex == 0)
                                {
                                    Flag = "1";
                                }
                                else if (cbbGSMV2IMSIActionFlag.SelectedIndex == 1)
                                {
                                    Flag = "5";
                                }
                                else
                                {
                                    Flag = "1";
                                }
                                ParaList.Add("bIMSI_#" + i.ToString() + "#", SubWindow.CDMAIMSIListInputWindow.IMSIInfoList[i].IMSI);
                                ParaList.Add("bUeActionFlag_#" + i.ToString() + "#", Flag);
                            }
                        }
                        if (NetWorkClient.ControllerServer.Connected)
                        {
                            JsonInterFace.ResultMessageList.Clear();
                            NetWorkClient.ControllerServer.Send(
                                                                JsonInterFace.GSMV2IMSIActionRequest(
                                                                                                    JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.Port,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.InnerType,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.SN,
                                                                                                    ParaList,
                                                                                                    Carrier
                                                                                                   )
                                                               );
                        }
                        else
                        {
                            MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                    }
                }
                //查询
                else if (cbbGSMV2IMSIActionType.SelectedIndex == 3)
                {
                    Parameters.ConfigType = "Query";
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        JsonInterFace.ResultMessageList.Clear();
                        NetWorkClient.ControllerServer.Send(
                                                            JsonInterFace.GSMV2IMSIQueryRequest(
                                                                                                JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                                JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                                JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                                                                                JsonInterFace.GSMV2DeviceParameter.Port,
                                                                                                JsonInterFace.GSMV2DeviceParameter.InnerType,
                                                                                                JsonInterFace.GSMV2DeviceParameter.SN,
                                                                                                Carrier
                                                                                               )
                                                           );
                    }
                    else
                    {
                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// 去除删除的重复IMSI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteSameIMSI(List<string> IMSIList)
        {

            HashSet<string> tempIMSI = new HashSet<string>(IMSIList);
            GSMV2SelectIMSIList.Clear();
            foreach (string imsi in tempIMSI)
            {
                GSMV2SelectIMSIList.Add(imsi);
            }
        }
        private void btnGSMV2IMSIInput_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "(文本文件 *.txt)|*.txt";
            int Num = 0;
            try
            {
                if ((bool)openFileDialog.ShowDialog())
                {
                    if ((new FileInfo(openFileDialog.FileName).Extension).ToLower() == ".txt")
                    {
                        string IMSIFileName = openFileDialog.FileName;
                        string[] IMSISoureList = File.ReadAllLines(openFileDialog.FileName);
                        if (SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Count > 0) { SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Clear(); }
                        for (int i = 0; i < IMSISoureList.Length; i++)
                        {
                            SubWindow.CDMAIMSIListInputWindow.IMSIControlInfoClass GSMV2IMSI = new SubWindow.CDMAIMSIListInputWindow.IMSIControlInfoClass();
                            if (new Regex(@"\d{15}").Match(IMSISoureList[i]).Success)
                            {
                                if (IMSISoureList[i].Trim().Length == 15)
                                {
                                    GSMV2IMSI.IMSI = IMSISoureList[i];
                                    GSMV2IMSI.ActionFlag = (i + 1).ToString();
                                    SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Add(GSMV2IMSI);
                                }
                                else
                                {
                                    Num++;
                                }
                            }
                            else
                            {
                                Num++;
                            }
                        }

                        if (SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Count > 0)
                        {
                            txtGSMV2IMSIInput.Text = "IMSI号已导入...";
                            txtGSMV2IMSIInput.IsReadOnly = true;

                            if (Num > 0)
                            {
                                MessageBox.Show("文件中共有[" + IMSISoureList.Length.ToString() + "]条数据，成功导入[" + SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Count.ToString() + "]条IMSI数据导入,其中[" + Num.ToString() + "]条非IMSI格式，未被导入！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            else
                            {
                                MessageBox.Show("文件中共有[" + IMSISoureList.Length.ToString() + "]条数据，[" + SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Count.ToString() + "]条IMSI数据导入成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                        else
                        {
                            txtGSMV2IMSIInput.Text = "";
                            txtGSMV2IMSIInput.IsReadOnly = false;

                            if (Num > 0)
                            {
                                MessageBox.Show("文件中共有[" + IMSISoureList.Length.ToString() + "]条数据，成功导入[" + SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Count.ToString() + "]条IMSI数据导入,其中[" + Num.ToString() + "]条非IMSI格式，未被导入！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            else
                            {
                                MessageBox.Show("文件中共有[" + IMSISoureList.Length.ToString() + "]条数据！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("导入文件只支持文本文件！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("IMSI导入失败！" + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void chkGSMV2NBCellCarrierOne_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                JsonInterFace.GSMV2CellNeighParameter.GSMV2CellInfoTab.Clear();
                JsonInterFace.GSMV2CellNeighParameter.ItemCellInfoTab.Clear();
                GSMV2NeighCellInfo.Clear();
                GSMV2ItemNeighCellInfo.Clear();
                if ((bool)chkGSMV2NBCellCarrierOne.IsChecked)
                {
                    chkGSMV2NBCellCarrierTwo.IsChecked = false;
                }
                else
                {
                    if (!(bool)chkGSMV2NBCellCarrierTwo.IsChecked)
                    {
                        chkGSMV2NBCellCarrierOne.IsChecked = true;
                    }
                }
                string Carrier = (Convert.ToInt32(chkGSMV2NBCellCarrierOne.IsChecked) - 1).ToString();
                if (NetWorkClient.ControllerServer.Connected)
                {
                    JsonInterFace.ResultMessageList.Clear();
                    Parameters.ConfigType = "Manul";
                    NetWorkClient.ControllerServer.Send(
                                                         JsonInterFace.GSMV2NeighCellInfoQueryRequest(
                                                                                                        JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                                        JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                                        JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                                                                                        JsonInterFace.GSMV2DeviceParameter.Port,
                                                                                                        JsonInterFace.GSMV2DeviceParameter.InnerType,
                                                                                                        JsonInterFace.GSMV2DeviceParameter.SN,
                                                                                                        Carrier
                                                                                                    )
                                                       );
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

        private void chkGSMV2NBCellCarrierTwo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                JsonInterFace.GSMV2CellNeighParameter.GSMV2CellInfoTab.Clear();
                JsonInterFace.GSMV2CellNeighParameter.ItemCellInfoTab.Clear();
                GSMV2NeighCellInfo.Clear();
                GSMV2ItemNeighCellInfo.Clear();
                if ((bool)chkGSMV2NBCellCarrierTwo.IsChecked)
                {
                    chkGSMV2NBCellCarrierOne.IsChecked = false;
                }
                else
                {
                    if (!(bool)chkGSMV2NBCellCarrierOne.IsChecked)
                    {
                        chkGSMV2NBCellCarrierTwo.IsChecked = true;
                    }
                }
                string Carrier = Convert.ToInt32(chkGSMV2NBCellCarrierTwo.IsChecked).ToString();
                if (NetWorkClient.ControllerServer.Connected)
                {
                    JsonInterFace.ResultMessageList.Clear();
                    Parameters.ConfigType = "Manul";
                    NetWorkClient.ControllerServer.Send(
                                                         JsonInterFace.GSMV2NeighCellInfoQueryRequest(
                                                                                                        JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                                        JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                                        JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                                                                                        JsonInterFace.GSMV2DeviceParameter.Port,
                                                                                                        JsonInterFace.GSMV2DeviceParameter.InnerType,
                                                                                                        JsonInterFace.GSMV2DeviceParameter.SN,
                                                                                                        Carrier
                                                                                                    )
                                                       );
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

        private void txtPLMN_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                JsonInterFace.LteCellNeighParameter.Operators = JsonInterFace.OperatorsList.GetOperators(txtPLMN.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void miMultiPLMNListClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MorePLMNList.Clear();
                MessageBox.Show("多PLMN清空成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("多PLMN清空失败！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void miFreqListClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PeriorFreqList.Clear();
                MessageBox.Show("频点清空成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("频点清空失败！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnStartRF_Click(object sender, RoutedEventArgs e)
        {
            if (NetWorkClient.ControllerServer.Connected)
            {
                //请求的载波
                string Carrier = string.Empty;
                if ((bool)chkGSMV2CarrierOne.IsChecked)
                {
                    Carrier = "0";
                }
                else if ((bool)chkGSMV2CarrierTwo.IsChecked)
                {
                    Carrier = "1";
                }
                else
                {
                    MessageBox.Show("GSMV2载波选择有误！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
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
            else
            {
                MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            }
        }

        private void btnCloseRF_Click(object sender, RoutedEventArgs e)
        {
            if (NetWorkClient.ControllerServer.Connected)
            {
                //请求的载波
                string Carrier = string.Empty;
                if ((bool)chkGSMV2CarrierOne.IsChecked)
                {
                    Carrier = "0";
                }
                else if ((bool)chkGSMV2CarrierTwo.IsChecked)
                {
                    Carrier = "1";
                }
                else
                {
                    MessageBox.Show("GSMV2载波选择有误！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
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
            else
            {
                MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            }
        }

        private void btnreStartRF_Click(object sender, RoutedEventArgs e)
        {
            if (NetWorkClient.ControllerServer.Connected)
            {
                //请求的载波
                string Carrier = string.Empty;
                if ((bool)chkGSMV2CarrierOne.IsChecked)
                {
                    Carrier = "0";
                }
                else if ((bool)chkGSMV2CarrierTwo.IsChecked)
                {
                    Carrier = "1";
                }
                else
                {
                    MessageBox.Show("GSMV2载波选择有误！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                NetWorkClient.ControllerServer.Send(JsonInterFace.GSMV2ReStartRequest(
                                                                                JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                                                                JsonInterFace.GSMV2DeviceParameter.Port,
                                                                                JsonInterFace.GSMV2DeviceParameter.InnerType,
                                                                                JsonInterFace.GSMV2DeviceParameter.SN,
                                                                                Carrier
                                                                              ));
            }
            else
            {
                MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            }
        }

        private void GSMV2CellInfoDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GSMV2CellInfoDataGrid.SelectedItem != null)
            {
                string Rkey = (GSMV2CellInfoDataGrid.SelectedItem as GSMV2CellNeighParameterClass.NeithCellInfo).Rkey;
                GSMV2ItemNeighCellInfo.Clear();
                for (int i = 0; i < JsonInterFace.GSMV2CellNeighParameter.ItemCellInfoTab.Rows.Count; i++)
                {
                    if (Rkey == JsonInterFace.GSMV2CellNeighParameter.ItemCellInfoTab.Rows[i][5].ToString())
                    {
                        GSMV2CellNeighParameterClass.ItemNeithCellInfo ItemcellInfo = new GSMV2CellNeighParameterClass.ItemNeithCellInfo();
                        Dispatcher.Invoke(() =>
                        {
                            string _ID = string.Empty;
                            if (GSMV2ItemNeighCellInfo.Count <= 0)
                            {
                                _ID = "1";
                            }
                            else
                            {
                                _ID = (Convert.ToInt32(GSMV2ItemNeighCellInfo[GSMV2ItemNeighCellInfo.Count - 1].ID.ToString()) + 1).ToString();
                            }

                            ItemcellInfo.ID = _ID;
                            ItemcellInfo.WUarfcn = JsonInterFace.GSMV2CellNeighParameter.ItemCellInfoTab.Rows[i][0].ToString();
                            ItemcellInfo.WPhyCellId = JsonInterFace.GSMV2CellNeighParameter.ItemCellInfoTab.Rows[i][1].ToString();
                            ItemcellInfo.CRSRP = JsonInterFace.GSMV2CellNeighParameter.ItemCellInfoTab.Rows[i][2].ToString();
                            ItemcellInfo.CC1 = JsonInterFace.GSMV2CellNeighParameter.ItemCellInfoTab.Rows[i][3].ToString();
                            ItemcellInfo.BC2 = JsonInterFace.GSMV2CellNeighParameter.ItemCellInfoTab.Rows[i][4].ToString();

                            GSMV2ItemNeighCellInfo.Add(ItemcellInfo);
                        });
                    }
                }
            }
        }

        /// <summary>
        /// GSMV2显示邻小区信息
        /// </summary>
        private void ShowGSMV2NeighCellInfo()
        {
            try
            {
                while (true)
                {
                    if (JsonInterFace.GSMV2CellNeighParameter.GSMV2CellInfoTab.Rows.Count > 0)
                    {
                        lock (JsonInterFace.GSMV2CellNeighParameter.CellInfoInputLock)
                        {
                            for (int i = 0; i < JsonInterFace.GSMV2CellNeighParameter.GSMV2CellInfoTab.Rows.Count; i++)
                            {
                                GSMV2CellNeighParameterClass.NeithCellInfo cellInfo = new GSMV2CellNeighParameterClass.NeithCellInfo();
                                Dispatcher.Invoke(() =>
                                {
                                    string _ID = string.Empty;
                                    if (GSMV2NeighCellInfo.Count <= 0)
                                    {
                                        _ID = "1";
                                    }
                                    else
                                    {
                                        _ID = (Convert.ToInt32(GSMV2NeighCellInfo[GSMV2NeighCellInfo.Count - 1].ID.ToString()) + 1).ToString();
                                    }
                                    cellInfo.ID = _ID;
                                    cellInfo.BSID = JsonInterFace.GSMV2CellNeighParameter.GSMV2CellInfoTab.Rows[i][0].ToString();
                                    cellInfo.BPLMNId = JsonInterFace.GSMV2CellNeighParameter.GSMV2CellInfoTab.Rows[i][1].ToString();
                                    cellInfo.CRSRP = JsonInterFace.GSMV2CellNeighParameter.GSMV2CellInfoTab.Rows[i][2].ToString();
                                    cellInfo.WTac = JsonInterFace.GSMV2CellNeighParameter.GSMV2CellInfoTab.Rows[i][3].ToString();
                                    cellInfo.WPhyCellId = JsonInterFace.GSMV2CellNeighParameter.GSMV2CellInfoTab.Rows[i][4].ToString();
                                    cellInfo.WUARFCN = JsonInterFace.GSMV2CellNeighParameter.GSMV2CellInfoTab.Rows[i][5].ToString();
                                    cellInfo.CRefTxPower = JsonInterFace.GSMV2CellNeighParameter.GSMV2CellInfoTab.Rows[i][6].ToString();
                                    cellInfo.BNbCellNum = JsonInterFace.GSMV2CellNeighParameter.GSMV2CellInfoTab.Rows[i][7].ToString();
                                    cellInfo.BC2 = JsonInterFace.GSMV2CellNeighParameter.GSMV2CellInfoTab.Rows[i][8].ToString();
                                    cellInfo.Rkey = JsonInterFace.GSMV2CellNeighParameter.GSMV2CellInfoTab.Rows[i][9].ToString();
                                    GSMV2NeighCellInfo.Add(cellInfo);
                                });
                            }
                            JsonInterFace.GSMV2CellNeighParameter.GSMV2CellInfoTab.Rows.Clear();
                        }
                    }

                    Thread.Sleep(500);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void btnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            string SMSOriginalNum = string.Empty;
            string SMSContent = string.Empty;
            string Carrier = string.Empty;
            if ((bool)cbGSMV2SMSCarrierOne.IsChecked)
            {
                Carrier = "0";
            }
            else if ((bool)cbGSMV2SMSCarrierTwo.IsChecked)
            {
                Carrier = "1";
            }
            else
            {
                MessageBox.Show("载波选择有误！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                return;
            }
            if (!txtSMSOriginalNum.Text.Equals(""))
            {
                if (!new Regex(@"\d").Match(txtSMSOriginalNum.Text.Trim()).Success || txtSMSOriginalNum.Text.Length > 11)
                {
                    MessageBox.Show("号码输入有误！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    return;
                }
                SMSOriginalNum = txtSMSOriginalNum.Text.Trim();
                SMSContent = txtSMSContent.Text.Trim();
                if (NetWorkClient.ControllerServer.Connected)
                {
                    NetWorkClient.ControllerServer.Send(JsonInterFace.GSMV2SMSRequest(
                                                                                    JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                    JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                    JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                                                                    JsonInterFace.GSMV2DeviceParameter.Port,
                                                                                    JsonInterFace.GSMV2DeviceParameter.InnerType,
                                                                                    JsonInterFace.GSMV2DeviceParameter.SN,
                                                                                    JsonInterFace.GSMV2ConfigSMSMSG.SMSctrl.ToString(),
                                                                                    SMSOriginalNum,
                                                                                    SMSContent,
                                                                                    Carrier
                                                                                  ));
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("主叫号码为空！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            }
        }

        private void txtGSMV2IMSIInput_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            CDMAIMSIListInputWin = new SubWindow.CDMAIMSIListInputWindow();
            CDMAIMSIListInputWin.ShowDialog();

            if (SubWindow.CDMAIMSIListInputWindow.IMSIInfoList.Count <= 0)
            {
                txtGSMV2IMSIInput.Text = "";
                txtGSMV2IMSIInput.IsReadOnly = false;
            }
        }

        private void cbGSMV2IMSICarrierOne_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)cbGSMV2IMSICarrierOne.IsChecked)
            {
                cbGSMV2IMSICarrierTwo.IsChecked = false;
            }
            else
            {
                if (!(bool)cbGSMV2IMSICarrierTwo.IsChecked)
                {
                    cbGSMV2IMSICarrierOne.IsChecked = true;
                }
            }
        }

        private void cbGSMV2IMSICarrierTwo_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)cbGSMV2IMSICarrierTwo.IsChecked)
            {
                cbGSMV2IMSICarrierOne.IsChecked = false;
            }
            else
            {
                if (!(bool)cbGSMV2IMSICarrierOne.IsChecked)
                {
                    cbGSMV2IMSICarrierTwo.IsChecked = true;
                }
            }
        }

        private void btnCDMAUpdateTime_Click(object sender, RoutedEventArgs e)
        {
            if ((txtCDMAFirstPeriodTimeStart.Text != ""
            && txtCDMAFirstPeriodTimeEnd.Text == "")
            || (txtCDMAFirstPeriodTimeStart.Text == ""
            && txtCDMAFirstPeriodTimeEnd.Text != ""))
            {
                MessageBox.Show("第一时间段的时间不完整！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (txtCDMAFirstPeriodTimeStart.Text != "" && txtCDMAFirstPeriodTimeEnd.Text != "")
            {
                //检测时间
                if (!Parameters.IsTime(JsonInterFace.CDMADeviceAdvanceSettingParameter.FirstPeriodTimeStart))
                {
                    MessageBox.Show("第一时间段的起始时间格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (!Parameters.IsTime(JsonInterFace.CDMADeviceAdvanceSettingParameter.FirstPeriodTimeEnd))
                {
                    MessageBox.Show("第一时间段的结束时间格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            if ((txtCDMASecondPeriodTimeStart.Text != ""
                && txtCDMASecoondPeriodTimeEnd.Text == "")
                || (txtCDMASecondPeriodTimeStart.Text == ""
                && txtCDMASecoondPeriodTimeEnd.Text != ""))
            {
                MessageBox.Show("第二时间段的时间不完整！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (txtCDMASecondPeriodTimeStart.Text != "" && txtCDMASecoondPeriodTimeEnd.Text != "")
            {
                //检测时间
                if (!Parameters.IsTime(JsonInterFace.CDMADeviceAdvanceSettingParameter.SecondPeriodTimeStart))
                {
                    MessageBox.Show("第二时间段的起始时间格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (!Parameters.IsTime(JsonInterFace.CDMADeviceAdvanceSettingParameter.SecoondPeriodTimeEnd))
                {
                    MessageBox.Show("第二时间段的结束时间格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            if ((txtCDMAThreePeriodTimeStart.Text != ""
                && txtCDMAThreePeriodTimeEnd.Text == "")
                || (txtCDMAThreePeriodTimeStart.Text == ""
                && txtCDMAThreePeriodTimeEnd.Text != ""))
            {
                MessageBox.Show("第三时间段的时间不完整！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (txtCDMAThreePeriodTimeStart.Text != "" && txtCDMAThreePeriodTimeEnd.Text != "")
            {
                //检测时间
                if (!Parameters.IsTime(JsonInterFace.CDMADeviceAdvanceSettingParameter.ThreePeriodTimeStart))
                {
                    MessageBox.Show("第三时间段的起始时间格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (!Parameters.IsTime(JsonInterFace.CDMADeviceAdvanceSettingParameter.ThreePeriodTimeEnd))
                {
                    MessageBox.Show("第三时间段的结束时间格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            if (NetWorkClient.ControllerServer.Connected)
            {
                Dictionary<string, string> ApperiodTimeList = new Dictionary<string, string>();
                ApperiodTimeList.Add("activeTime1Start", JsonInterFace.CDMADeviceAdvanceSettingParameter.FirstPeriodTimeStart);
                ApperiodTimeList.Add("activeTime1Ended", JsonInterFace.CDMADeviceAdvanceSettingParameter.FirstPeriodTimeEnd);
                ApperiodTimeList.Add("activeTime2Start", JsonInterFace.CDMADeviceAdvanceSettingParameter.SecondPeriodTimeStart);
                ApperiodTimeList.Add("activeTime2Ended", JsonInterFace.CDMADeviceAdvanceSettingParameter.SecoondPeriodTimeEnd);
                ApperiodTimeList.Add("activeTime3Start", JsonInterFace.CDMADeviceAdvanceSettingParameter.ThreePeriodTimeStart);
                ApperiodTimeList.Add("activeTime3Ended", JsonInterFace.CDMADeviceAdvanceSettingParameter.ThreePeriodTimeEnd);

                Parameters.ConfigType = DeviceType.CDMA;
                NetWorkClient.ControllerServer.Send(JsonInterFace.APPeriodTimeConrolRequest(
                                                                                            JsonInterFace.CDMADeviceParameter.DomainFullPathName,
                                                                                            JsonInterFace.CDMADeviceParameter.DeviceName,
                                                                                            ApperiodTimeList
                                                                                           ));
            }
            else
            {
                MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        private void btnWCDMACellNeighUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Dictionary<string, string> CellNeighParams = new Dictionary<string, string>();
                Dictionary<string, string> SetWorkModeParams = new Dictionary<string, string>();
                for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                {
                    if (JsonInterFace.APATTributesLists[i].FullName == FullName)
                    {
                        if (JsonInterFace.WCDMACellNeighParameter.PLMN != null && JsonInterFace.WCDMACellNeighParameter.PLMN != JsonInterFace.APATTributesLists[i].PLMN)
                        {
                            string mcc = JsonInterFace.WCDMACellNeighParameter.PLMN.Substring(0, Parameters.PLMN_Lengh - 2);
                            CellNeighParams.Add("mcc", mcc);
                            string mnc = JsonInterFace.WCDMACellNeighParameter.PLMN.Substring(Parameters.PLMN_Lengh - 2, JsonInterFace.WCDMACellNeighParameter.PLMN.Length - (Parameters.PLMN_Lengh - 2));
                            CellNeighParams.Add("mnc", mnc);
                        }

                        if (JsonInterFace.WCDMACellNeighParameter.FrequencyPoint != null && JsonInterFace.WCDMACellNeighParameter.FrequencyPoint != JsonInterFace.APATTributesLists[i].FrequencyPoint)
                        {
                            CellNeighParams.Add("euarfcn", JsonInterFace.WCDMACellNeighParameter.FrequencyPoint);
                        }

                        if (JsonInterFace.WCDMACellNeighParameter.PowerAttenuation != null && JsonInterFace.WCDMACellNeighParameter.PowerAttenuation != JsonInterFace.APATTributesLists[i].PowerAttenuation)
                        {
                            CellNeighParams.Add("txpower", JsonInterFace.WCDMACellNeighParameter.PowerAttenuation);
                        }

                        if (JsonInterFace.WCDMACellNeighParameter.Scrambler != null && JsonInterFace.WCDMACellNeighParameter.Scrambler != JsonInterFace.APATTributesLists[i].Scrambler)
                        {
                            CellNeighParams.Add("pci", JsonInterFace.WCDMACellNeighParameter.Scrambler);
                        }

                        if (JsonInterFace.WCDMACellNeighParameter.TacLac != null && JsonInterFace.WCDMACellNeighParameter.TacLac != JsonInterFace.APATTributesLists[i].TacLac)
                        {
                            CellNeighParams.Add("tac", JsonInterFace.WCDMACellNeighParameter.TacLac);
                        }

                        if (JsonInterFace.WCDMACellNeighParameter.Period != null && JsonInterFace.WCDMACellNeighParameter.Period != JsonInterFace.APATTributesLists[i].Period)
                        {
                            CellNeighParams.Add("periodTac", JsonInterFace.WCDMACellNeighParameter.Period);
                        }

                        if (JsonInterFace.WCDMACellNeighParameter.CellID != null && JsonInterFace.WCDMACellNeighParameter.CellID != JsonInterFace.APATTributesLists[i].CellID)
                        {
                            CellNeighParams.Add("cellid", JsonInterFace.WCDMACellNeighParameter.CellID);
                        }

                        if (JsonInterFace.WCDMASetWorkModeParameter.RebootModeAuto && JsonInterFace.WCDMASetWorkModeParameter.RebootModeAuto != JsonInterFace.APATTributesLists[i].RebootModeAuto)
                        {
                            SetWorkModeParams.Add("boot", "1");
                        }
                        else if (JsonInterFace.WCDMASetWorkModeParameter.RebootModeManul && JsonInterFace.WCDMASetWorkModeParameter.RebootModeManul != JsonInterFace.APATTributesLists[i].RebootModeManul)
                        {
                            SetWorkModeParams.Add("boot", "0");
                        }

                        if (CellNeighParams.Count <= 0 && SetWorkModeParams.Count <= 0)
                        {
                            MessageBox.Show("内容未改变！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                            return;
                        }

                        if (MessageBox.Show("确定更新小区信息[" + JsonInterFace.WCDMACellNeighParameter.DeviceName + "]?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                        {
                            if (NetWorkClient.ControllerServer.Connected)
                            {
                                JsonInterFace.ResultMessageList.Clear();
                                if (SetWorkModeParams.Count > 0)
                                {
                                    //工作模式设置
                                    NetWorkClient.ControllerServer.Send(JsonInterFace.APSetWorkModeRequest(
                                                                                                        JsonInterFace.WCDMACellNeighParameter.DomainFullPathName,
                                                                                                        JsonInterFace.WCDMACellNeighParameter.DeviceName,
                                                                                                        JsonInterFace.WCDMACellNeighParameter.IpAddr,
                                                                                                        JsonInterFace.WCDMACellNeighParameter.Port,
                                                                                                        JsonInterFace.WCDMACellNeighParameter.InnerType,
                                                                                                        JsonInterFace.WCDMACellNeighParameter.SN,
                                                                                                        SetWorkModeParams
                                                                                                      ));
                                }
                                if (CellNeighParams.Count > 0)
                                {
                                    //小区设置
                                    NetWorkClient.ControllerServer.Send(JsonInterFace.APSetConfigurationRequest(
                                                                                                            JsonInterFace.WCDMACellNeighParameter.DomainFullPathName,
                                                                                                            JsonInterFace.WCDMACellNeighParameter.DeviceName,
                                                                                                            JsonInterFace.WCDMACellNeighParameter.IpAddr,
                                                                                                            JsonInterFace.WCDMACellNeighParameter.Port,
                                                                                                            JsonInterFace.WCDMACellNeighParameter.InnerType,
                                                                                                            JsonInterFace.WCDMACellNeighParameter.SN,
                                                                                                            CellNeighParams
                                                                                                            ));
                                }
                            }
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "更新失败！");
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void btnDevieName_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (DeviceListTreeView.SelectedItem != null)
                {
                    SelfUnSelected(JsonInterFace.UsrdomainData);

                    SelfName = ((CheckBoxTreeModel)DeviceListTreeView.SelectedItem).Name;
                    FullName = ((CheckBoxTreeModel)DeviceListTreeView.SelectedItem).FullName;
                    SelfID = ((CheckBoxTreeModel)DeviceListTreeView.SelectedItem).Id;
                    ParentID = ((CheckBoxTreeModel)DeviceListTreeView.SelectedItem).ParentID;
                    SelfNodeType = ((CheckBoxTreeModel)DeviceListTreeView.SelectedItem).SelfNodeType;
                    IsStation = ((CheckBoxTreeModel)DeviceListTreeView.SelectedItem).IsStation;
                    IsOnline = ((CheckBoxTreeModel)DeviceListTreeView.SelectedItem).IsOnLine;
                    Model = ((CheckBoxTreeModel)DeviceListTreeView.SelectedItem).Mode;

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

                    TreeViewSelectDomainFullPathName = SelfName;
                    TreeViewSelectItemID = SelfID;

                    //选定域 或 根域
                    if (SelfNodeType.Equals(NodeType.StructureNode.ToString())
                        && IsStation.Equals("0") || SelfNodeType.Equals(NodeType.RootNode.ToString()))
                    {
                        SettingOnOffLineControl(null, false, SelfNodeType);
                        btnAdd.IsEnabled = false;
                        btnDelete.IsEnabled = false;
                        btnUpdate.IsEnabled = false;

                        //设备信息
                        JsonInterFace.LteDeviceParameter.SelfID = string.Empty;
                        JsonInterFace.LteDeviceParameter.ParentID = string.Empty;
                        JsonInterFace.LteDeviceParameter.DeviceName = string.Empty;
                        JsonInterFace.LteDeviceParameter.DomainFullPathName = string.Empty;
                        JsonInterFace.LteDeviceParameter.Station = string.Empty;

                        JsonInterFace.LteDeviceParameter.DeviceMode = string.Empty;
                        JsonInterFace.LteDeviceParameter.StaticIPMode = false;
                        JsonInterFace.LteDeviceParameter.DynamicIPMode = false;
                        JsonInterFace.LteDeviceParameter.IpAddr = string.Empty;
                        JsonInterFace.LteDeviceParameter.Port = string.Empty;
                        JsonInterFace.LteDeviceParameter.NetMask = string.Empty;
                        JsonInterFace.LteDeviceParameter.SN = string.Empty;
                        JsonInterFace.LteDeviceParameter.DeviceIdentificationMode = string.Empty;
                        JsonInterFace.LteDeviceParameter.OnLine = string.Empty;

                        //GSM设备信息
                        JsonInterFace.GSMDeviceParameter.SelfID = string.Empty;
                        JsonInterFace.GSMDeviceParameter.ParentID = string.Empty;
                        JsonInterFace.GSMDeviceParameter.DeviceName = string.Empty;
                        JsonInterFace.GSMDeviceParameter.DomainFullPathName = string.Empty;
                        JsonInterFace.GSMDeviceParameter.Station = string.Empty;

                        JsonInterFace.GSMDeviceParameter.DeviceMode = string.Empty;
                        JsonInterFace.GSMDeviceParameter.StaticIPMode = false;
                        JsonInterFace.GSMDeviceParameter.DynamicIPMode = false;
                        JsonInterFace.GSMDeviceParameter.IpAddr = string.Empty;
                        JsonInterFace.GSMDeviceParameter.Port = string.Empty;
                        JsonInterFace.GSMDeviceParameter.NetMask = string.Empty;
                        JsonInterFace.GSMDeviceParameter.SN = string.Empty;
                        JsonInterFace.GSMDeviceParameter.DeviceIdentificationMode = string.Empty;
                        JsonInterFace.GSMDeviceParameter.InnerType = string.Empty;
                        JsonInterFace.GSMDeviceParameter.OnLine = string.Empty;

                        //GSMV2
                        JsonInterFace.GSMV2DeviceParameter.SelfID = SelfID;
                        JsonInterFace.GSMV2DeviceParameter.ParentID = ParentID;
                        JsonInterFace.GSMV2DeviceParameter.DeviceName = string.Empty;
                        JsonInterFace.GSMV2DeviceParameter.DomainFullPathName = FullName;
                        JsonInterFace.GSMV2DeviceParameter.Station = string.Empty;

                        JsonInterFace.GSMV2DeviceParameter.DeviceMode = string.Empty;
                        JsonInterFace.GSMV2DeviceParameter.StaticIPMode = false;
                        JsonInterFace.GSMV2DeviceParameter.DynamicIPMode = false;
                        JsonInterFace.GSMV2DeviceParameter.IpAddr = string.Empty;
                        JsonInterFace.GSMV2DeviceParameter.Port = string.Empty;
                        JsonInterFace.GSMV2DeviceParameter.NetMask = string.Empty;
                        JsonInterFace.GSMV2DeviceParameter.SN = string.Empty;
                        JsonInterFace.GSMV2DeviceParameter.DeviceIdentificationMode = string.Empty;
                        JsonInterFace.GSMV2DeviceParameter.InnerType = string.Empty;
                        JsonInterFace.GSMV2DeviceParameter.OnLine = string.Empty;
                        btnGSMV2Add.IsEnabled = false;
                        btnGSMV2Delete.IsEnabled = false;
                        btnGSMV2Update.IsEnabled = false;

                        //WCDMA
                        JsonInterFace.WCDMADeviceParameter.SelfID = SelfID;
                        JsonInterFace.WCDMADeviceParameter.ParentID = ParentID;
                        JsonInterFace.WCDMADeviceParameter.DeviceName = string.Empty;
                        JsonInterFace.WCDMADeviceParameter.DomainFullPathName = FullName;
                        JsonInterFace.WCDMADeviceParameter.Station = string.Empty;

                        JsonInterFace.WCDMADeviceParameter.DeviceMode = string.Empty;
                        JsonInterFace.WCDMADeviceParameter.StaticIPMode = false;
                        JsonInterFace.WCDMADeviceParameter.DynamicIPMode = false;
                        JsonInterFace.WCDMADeviceParameter.IpAddr = string.Empty;
                        JsonInterFace.WCDMADeviceParameter.Port = string.Empty;
                        JsonInterFace.WCDMADeviceParameter.NetMask = string.Empty;
                        JsonInterFace.WCDMADeviceParameter.SN = string.Empty;
                        JsonInterFace.WCDMADeviceParameter.DeviceIdentificationMode = string.Empty;
                        JsonInterFace.WCDMADeviceParameter.OnLine = string.Empty;
                        btnWCDMAAdd.IsEnabled = false;
                        btnWCDMADelete.IsEnabled = false;
                        btnWCDMAUpdate.IsEnabled = false;

                        //CDMA
                        JsonInterFace.CDMADeviceParameter.SelfID = string.Empty;
                        JsonInterFace.CDMADeviceParameter.ParentID = string.Empty;
                        JsonInterFace.CDMADeviceParameter.DeviceName = string.Empty;
                        JsonInterFace.CDMADeviceParameter.DomainFullPathName = string.Empty;
                        JsonInterFace.CDMADeviceParameter.Station = string.Empty;

                        JsonInterFace.CDMADeviceParameter.DeviceMode = string.Empty;
                        JsonInterFace.CDMADeviceParameter.StaticIPMode = false;
                        JsonInterFace.CDMADeviceParameter.DynamicIPMode = false;
                        JsonInterFace.CDMADeviceParameter.IpAddr = string.Empty;
                        JsonInterFace.CDMADeviceParameter.Port = string.Empty;
                        JsonInterFace.CDMADeviceParameter.NetMask = string.Empty;
                        JsonInterFace.CDMADeviceParameter.SN = string.Empty;
                        JsonInterFace.CDMADeviceParameter.DeviceIdentificationMode = string.Empty;
                        JsonInterFace.CDMADeviceParameter.InnerType = string.Empty;
                        JsonInterFace.CDMADeviceParameter.OnLine = string.Empty;

                        //TDS
                        JsonInterFace.TDSDeviceParameter.SelfID = SelfID;
                        JsonInterFace.TDSDeviceParameter.ParentID = ParentID;
                        JsonInterFace.TDSDeviceParameter.DeviceName = string.Empty;
                        JsonInterFace.TDSDeviceParameter.DomainFullPathName = FullName;
                        JsonInterFace.TDSDeviceParameter.Station = string.Empty;

                        JsonInterFace.TDSDeviceParameter.DeviceMode = string.Empty;
                        JsonInterFace.TDSDeviceParameter.StaticIPMode = false;
                        JsonInterFace.TDSDeviceParameter.DynamicIPMode = false;
                        JsonInterFace.TDSDeviceParameter.IpAddr = string.Empty;
                        JsonInterFace.TDSDeviceParameter.Port = string.Empty;
                        JsonInterFace.TDSDeviceParameter.NetMask = string.Empty;
                        JsonInterFace.TDSDeviceParameter.SN = string.Empty;
                        JsonInterFace.TDSDeviceParameter.DeviceIdentificationMode = string.Empty;
                        JsonInterFace.TDSDeviceParameter.OnLine = string.Empty;
                        btnTDSAdd.IsEnabled = false;
                        btnTDSDelete.IsEnabled = false;
                        btnTDSUpdate.IsEnabled = false;
                    }
                    //选定站点
                    else if (SelfNodeType.Equals(NodeType.StructureNode.ToString())
                        && IsStation.Equals("1"))
                    {
                        SettingOnOffLineControl(DeviceType.LTE, IsOnline, SelfNodeType);
                        #region  权限
                        if (RoleTypeClass.RoleType.Equals("RoleType"))
                        {

                        }
                        else
                        {
                            if (int.Parse(RoleTypeClass.RoleType) > 3)
                            {
                                btnAdd.IsEnabled = false;
                                btnGSMAdd.IsEnabled = false;
                                btnWCDMAAdd.IsEnabled = false;
                                btnCDMAAdd.IsEnabled = false;
                                btnGSMV2Add.IsEnabled = false;
                                btnTDSAdd.IsEnabled = false;
                            }
                            else
                            {
                                btnAdd.IsEnabled = true;
                                btnGSMAdd.IsEnabled = true;
                                btnWCDMAAdd.IsEnabled = true;
                                btnCDMAAdd.IsEnabled = true;
                                btnGSMV2Add.IsEnabled = true;
                                btnTDSAdd.IsEnabled = true;
                            }
                        }
                        #endregion
                        btnDelete.IsEnabled = false;
                        btnUpdate.IsEnabled = false;

                        //设备信息(添加，删除，修改)
                        JsonInterFace.LteDeviceParameter.SelfID = (JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows.Count - 1][1]).ToString();
                        JsonInterFace.LteDeviceParameter.ParentID = SelfID;
                        JsonInterFace.LteDeviceParameter.DeviceName = string.Empty;
                        JsonInterFace.LteDeviceParameter.DomainFullPathName = FullName;
                        string[] DomainFullNameTmp = FullName.ToString().Split(new char[] { '.' });
                        JsonInterFace.LteDeviceParameter.Station = DomainFullNameTmp[DomainFullNameTmp.Length - 1];
                        JsonInterFace.LteDeviceParameter.DeviceMode = string.Empty;
                        JsonInterFace.LteDeviceParameter.StaticIPMode = false;
                        JsonInterFace.LteDeviceParameter.DynamicIPMode = false;
                        JsonInterFace.LteDeviceParameter.IpAddr = string.Empty;
                        JsonInterFace.LteDeviceParameter.Port = string.Empty;
                        JsonInterFace.LteDeviceParameter.NetMask = string.Empty;
                        JsonInterFace.LteDeviceParameter.SN = string.Empty;
                        JsonInterFace.LteDeviceParameter.DeviceIdentificationMode = string.Empty;
                        JsonInterFace.LteDeviceParameter.OnLine = string.Empty;

                        //小区信息(配置小区)
                        JsonInterFace.LteCellNeighParameter.SelfID = SelfID;
                        JsonInterFace.LteCellNeighParameter.ParentID = ParentID;
                        JsonInterFace.LteCellNeighParameter.DeviceName = SelfName;
                        JsonInterFace.LteCellNeighParameter.DomainFullPathName = FullName;

                        //高级设置
                        JsonInterFace.LteDeviceAdvanceSettingParameter.SelfID = SelfID;
                        JsonInterFace.LteDeviceAdvanceSettingParameter.ParentID = ParentID;
                        JsonInterFace.LteDeviceAdvanceSettingParameter.DeviceName = SelfName;
                        JsonInterFace.LteDeviceAdvanceSettingParameter.DomainFullPathName = FullName;

                        //系统设置
                        JsonInterFace.LteDeviceSystemMaintenenceParameter.SelfID = SelfID;
                        JsonInterFace.LteDeviceSystemMaintenenceParameter.ParentID = ParentID;
                        JsonInterFace.LteDeviceSystemMaintenenceParameter.DeviceName = SelfName;
                        JsonInterFace.LteDeviceSystemMaintenenceParameter.DomainFullPathName = FullName;

                        //工程设置
                        JsonInterFace.LteDeviceObjectSettingParameter.SelfID = SelfID;
                        JsonInterFace.LteDeviceObjectSettingParameter.ParentID = ParentID;
                        JsonInterFace.LteDeviceObjectSettingParameter.DeviceName = SelfName;
                        JsonInterFace.LteDeviceObjectSettingParameter.DomainFullPathName = FullName;


                        //GSM设备信息(添加，删除，修改)
                        JsonInterFace.GSMDeviceParameter.SelfID = (JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows.Count - 1][1]).ToString();
                        JsonInterFace.GSMDeviceParameter.ParentID = SelfID;
                        JsonInterFace.GSMDeviceParameter.DeviceName = string.Empty;
                        JsonInterFace.GSMDeviceParameter.DomainFullPathName = FullName;
                        DomainFullNameTmp = new string[] { };
                        DomainFullNameTmp = FullName.ToString().Split(new char[] { '.' });
                        JsonInterFace.GSMDeviceParameter.Station = DomainFullNameTmp[DomainFullNameTmp.Length - 1];
                        JsonInterFace.GSMDeviceParameter.DeviceMode = string.Empty;
                        JsonInterFace.GSMDeviceParameter.StaticIPMode = false;
                        JsonInterFace.GSMDeviceParameter.DynamicIPMode = false;
                        JsonInterFace.GSMDeviceParameter.IpAddr = string.Empty;
                        JsonInterFace.GSMDeviceParameter.Port = string.Empty;
                        JsonInterFace.GSMDeviceParameter.NetMask = string.Empty;
                        JsonInterFace.GSMDeviceParameter.SN = string.Empty;
                        JsonInterFace.GSMDeviceParameter.DeviceIdentificationMode = string.Empty;
                        JsonInterFace.GSMDeviceParameter.OnLine = string.Empty;

                        //CDMA设备信息(添加，删除，修改)
                        JsonInterFace.CDMADeviceParameter.SelfID = (JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows.Count - 1][1]).ToString();
                        JsonInterFace.CDMADeviceParameter.ParentID = SelfID;
                        JsonInterFace.CDMADeviceParameter.DeviceName = string.Empty;
                        JsonInterFace.CDMADeviceParameter.DomainFullPathName = FullName;
                        DomainFullNameTmp = new string[] { };
                        DomainFullNameTmp = FullName.ToString().Split(new char[] { '.' });
                        JsonInterFace.CDMADeviceParameter.Station = DomainFullNameTmp[DomainFullNameTmp.Length - 1];
                        JsonInterFace.CDMADeviceParameter.DeviceMode = string.Empty;
                        JsonInterFace.CDMADeviceParameter.StaticIPMode = false;
                        JsonInterFace.CDMADeviceParameter.DynamicIPMode = false;
                        JsonInterFace.CDMADeviceParameter.IpAddr = string.Empty;
                        JsonInterFace.CDMADeviceParameter.Port = string.Empty;
                        JsonInterFace.CDMADeviceParameter.NetMask = string.Empty;
                        JsonInterFace.CDMADeviceParameter.SN = string.Empty;
                        JsonInterFace.CDMADeviceParameter.DeviceIdentificationMode = string.Empty;
                        JsonInterFace.CDMACellNeighParameter.HardWareID = string.Empty;
                        JsonInterFace.CDMACellNeighParameter.OnLine = string.Empty;

                        //GSMV2设备信息(添加，删除，修改)
                        JsonInterFace.GSMV2DeviceParameter.SelfID = (JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows.Count - 1][1]).ToString();
                        JsonInterFace.GSMV2DeviceParameter.ParentID = SelfID;
                        JsonInterFace.GSMV2DeviceParameter.DeviceName = string.Empty;
                        JsonInterFace.GSMV2DeviceParameter.DomainFullPathName = FullName;
                        DomainFullNameTmp = new string[] { };
                        DomainFullNameTmp = FullName.ToString().Split(new char[] { '.' });
                        JsonInterFace.GSMV2DeviceParameter.Station = DomainFullNameTmp[DomainFullNameTmp.Length - 1];
                        JsonInterFace.GSMV2DeviceParameter.DeviceMode = string.Empty;
                        JsonInterFace.GSMV2DeviceParameter.StaticIPMode = false;
                        JsonInterFace.GSMV2DeviceParameter.DynamicIPMode = false;
                        JsonInterFace.GSMV2DeviceParameter.IpAddr = string.Empty;
                        JsonInterFace.GSMV2DeviceParameter.Port = string.Empty;
                        JsonInterFace.GSMV2DeviceParameter.NetMask = string.Empty;
                        JsonInterFace.GSMV2DeviceParameter.SN = string.Empty;
                        JsonInterFace.GSMV2DeviceParameter.DeviceIdentificationMode = string.Empty;
                        JsonInterFace.GSMV2CellNeighParameter.HardWareID = string.Empty;
                        JsonInterFace.GSMV2CellNeighParameter.OnLine = string.Empty;

                        //WCDMA
                        JsonInterFace.WCDMADeviceParameter.SelfID = (JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows.Count - 1][1]).ToString();
                        JsonInterFace.WCDMADeviceParameter.ParentID = SelfID;
                        JsonInterFace.WCDMADeviceParameter.DeviceName = string.Empty;
                        JsonInterFace.WCDMADeviceParameter.DomainFullPathName = FullName;
                        DomainFullNameTmp = FullName.ToString().Split(new char[] { '.' });
                        JsonInterFace.WCDMADeviceParameter.Station = DomainFullNameTmp[DomainFullNameTmp.Length - 1];
                        JsonInterFace.WCDMADeviceParameter.DeviceMode = string.Empty;
                        JsonInterFace.WCDMADeviceParameter.StaticIPMode = false;
                        JsonInterFace.WCDMADeviceParameter.DynamicIPMode = false;
                        JsonInterFace.WCDMADeviceParameter.IpAddr = string.Empty;
                        JsonInterFace.WCDMADeviceParameter.Port = string.Empty;
                        JsonInterFace.WCDMADeviceParameter.NetMask = string.Empty;
                        JsonInterFace.WCDMADeviceParameter.SN = string.Empty;
                        JsonInterFace.WCDMADeviceParameter.DeviceIdentificationMode = string.Empty;
                        JsonInterFace.WCDMADeviceParameter.OnLine = string.Empty;

                        //小区信息(配置小区)
                        JsonInterFace.WCDMACellNeighParameter.SelfID = SelfID;
                        JsonInterFace.WCDMACellNeighParameter.ParentID = ParentID;
                        JsonInterFace.WCDMACellNeighParameter.DeviceName = SelfName;
                        JsonInterFace.WCDMACellNeighParameter.DomainFullPathName = FullName;

                        //高级设置
                        JsonInterFace.WCDMADeviceAdvanceSettingParameter.SelfID = SelfID;
                        JsonInterFace.WCDMADeviceAdvanceSettingParameter.ParentID = ParentID;
                        JsonInterFace.WCDMADeviceAdvanceSettingParameter.DeviceName = SelfName;
                        JsonInterFace.WCDMADeviceAdvanceSettingParameter.DomainFullPathName = FullName;

                        //系统设置
                        JsonInterFace.WCDMADeviceSystemMaintenenceParameter.SelfID = SelfID;
                        JsonInterFace.WCDMADeviceSystemMaintenenceParameter.ParentID = ParentID;
                        JsonInterFace.WCDMADeviceSystemMaintenenceParameter.DeviceName = SelfName;
                        JsonInterFace.WCDMADeviceSystemMaintenenceParameter.DomainFullPathName = FullName;

                        //工程设置
                        JsonInterFace.WCDMADeviceObjectSettingParameter.SelfID = SelfID;
                        JsonInterFace.WCDMADeviceObjectSettingParameter.ParentID = ParentID;
                        JsonInterFace.WCDMADeviceObjectSettingParameter.DeviceName = SelfName;
                        JsonInterFace.WCDMADeviceObjectSettingParameter.DomainFullPathName = FullName;

                        //===========================TDS==============================
                        JsonInterFace.TDSDeviceParameter.SelfID = (JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows.Count - 1][1]).ToString();
                        JsonInterFace.TDSDeviceParameter.ParentID = SelfID;
                        JsonInterFace.TDSDeviceParameter.DeviceName = string.Empty;
                        JsonInterFace.TDSDeviceParameter.DomainFullPathName = FullName;
                        DomainFullNameTmp = FullName.ToString().Split(new char[] { '.' });
                        JsonInterFace.TDSDeviceParameter.Station = DomainFullNameTmp[DomainFullNameTmp.Length - 1];
                        JsonInterFace.TDSDeviceParameter.DeviceMode = string.Empty;
                        JsonInterFace.TDSDeviceParameter.StaticIPMode = false;
                        JsonInterFace.TDSDeviceParameter.DynamicIPMode = false;
                        JsonInterFace.TDSDeviceParameter.IpAddr = string.Empty;
                        JsonInterFace.TDSDeviceParameter.Port = string.Empty;
                        JsonInterFace.TDSDeviceParameter.NetMask = string.Empty;
                        JsonInterFace.TDSDeviceParameter.SN = string.Empty;
                        JsonInterFace.TDSDeviceParameter.DeviceIdentificationMode = string.Empty;
                        JsonInterFace.TDSDeviceParameter.OnLine = string.Empty;

                        //小区信息(配置小区)
                        JsonInterFace.TDSCellNeighParameter.SelfID = SelfID;
                        JsonInterFace.TDSCellNeighParameter.ParentID = ParentID;
                        JsonInterFace.TDSCellNeighParameter.DeviceName = SelfName;
                        JsonInterFace.TDSCellNeighParameter.DomainFullPathName = FullName;

                        //高级设置
                        JsonInterFace.TDSDeviceAdvanceSettingParameter.SelfID = SelfID;
                        JsonInterFace.TDSDeviceAdvanceSettingParameter.ParentID = ParentID;
                        JsonInterFace.TDSDeviceAdvanceSettingParameter.DeviceName = SelfName;
                        JsonInterFace.TDSDeviceAdvanceSettingParameter.DomainFullPathName = FullName;

                        //系统设置
                        JsonInterFace.TDSDeviceSystemMaintenenceParameter.SelfID = SelfID;
                        JsonInterFace.TDSDeviceSystemMaintenenceParameter.ParentID = ParentID;
                        JsonInterFace.TDSDeviceSystemMaintenenceParameter.DeviceName = SelfName;
                        JsonInterFace.TDSDeviceSystemMaintenenceParameter.DomainFullPathName = FullName;

                        //工程设置
                        JsonInterFace.TDSDeviceObjectSettingParameter.SelfID = SelfID;
                        JsonInterFace.TDSDeviceObjectSettingParameter.ParentID = ParentID;
                        JsonInterFace.TDSDeviceObjectSettingParameter.DeviceName = SelfName;
                        JsonInterFace.TDSDeviceObjectSettingParameter.DomainFullPathName = FullName;
                    }
                    //选定设备
                    else if (SelfNodeType.Equals(NodeType.LeafNode.ToString()))
                    {
                        //界面显示控制
                        SettingOnOffLineControl(Model, true, SelfNodeType);
                        #region  权限
                        if (RoleTypeClass.RoleType.Equals("RoleType"))
                        {

                        }
                        else
                        {
                            if (int.Parse(RoleTypeClass.RoleType) > 3)
                            {
                                btnDelete.IsEnabled = false;
                                btnGSMDelete.IsEnabled = false;
                                btnWCDMADelete.IsEnabled = false;
                                btnCDMADelete.IsEnabled = false;
                                btnGSMV2Delete.IsEnabled = false;

                                btnUpdate.IsEnabled = false;
                                btnGSMUpdate.IsEnabled = false;
                                btnWCDMAUpdate.IsEnabled = false;
                                btnCDMAUpdate.IsEnabled = false;
                                btnGSMV2Update.IsEnabled = false;
                            }
                            else
                            {
                                btnDelete.IsEnabled = true;
                                btnGSMDelete.IsEnabled = true;
                                btnWCDMADelete.IsEnabled = true;
                                btnCDMADelete.IsEnabled = true;
                                btnGSMV2Delete.IsEnabled = true;

                                btnUpdate.IsEnabled = true;
                                btnGSMUpdate.IsEnabled = true;
                                btnWCDMAUpdate.IsEnabled = true;
                                btnCDMAUpdate.IsEnabled = true;
                                btnGSMV2Update.IsEnabled = true;
                            }
                        }
                        #endregion
                        btnAdd.IsEnabled = false;
                        btnGSMAdd.IsEnabled = false;
                        btnWCDMAAdd.IsEnabled = false;
                        btnCDMAAdd.IsEnabled = false;
                        btnGSMV2Add.IsEnabled = false;
                        //初始化非XML信息
                        Parameters.ConfigType = "";
                        JsonInterFace.DeviceNoXMLUpload.Enable = "";
                        JsonInterFace.DeviceNoXMLUpload.Type = "";
                        JsonInterFace.DeviceNoXMLUpload.MessageFormat = "";
                        JsonInterFace.DeviceNoXMLUpload.Period = "";
                        JsonInterFace.DeviceNoXMLUpload.NameFormat = "";
                        JsonInterFace.DeviceNoXMLUpload.DataFormat = "";
                        JsonInterFace.DeviceNoXMLUpload.URLorIP = "";
                        JsonInterFace.DeviceNoXMLUpload.AddInfo = "";
                        JsonInterFace.DeviceNoXMLUpload.CommEnable = "";
                        JsonInterFace.DeviceNoXMLUpload.CommIp = "";
                        JsonInterFace.DeviceNoXMLUpload.CommPort = "";
                        JsonInterFace.DeviceNoXMLUpload.EncryptType = "";
                        JsonInterFace.DeviceNoXMLUpload.CacheMax = "";
                        //初始化同步状态信息
                        JsonInterFace.SYNCInfo.Status = "";
                        JsonInterFace.SYNCInfo.Source = "";
                        JsonInterFace.SYNCInfo.Euarfcn = "";
                        JsonInterFace.SYNCInfo.PCI = "";

                        if (new Regex(DeviceType.LTE).Match(Model).Success)
                        {
                            JsonInterFace.LteDeviceParameter.SelfID = SelfID;
                            JsonInterFace.LteDeviceParameter.ParentID = ParentID;
                            JsonInterFace.LteDeviceParameter.DeviceName = SelfName;
                            string DomainFullPathName = string.Empty;
                            string[] DomainFullNameTmp = FullName.Split(new char[] { '.' });

                            for (int j = 0; j < DomainFullNameTmp.Length - 1; j++)
                            {
                                if (DomainFullPathName.Trim().Equals(""))
                                {
                                    DomainFullPathName = DomainFullNameTmp[j];
                                }
                                else
                                {
                                    DomainFullPathName += "." + DomainFullNameTmp[j];
                                }
                            }
                            JsonInterFace.LteDeviceParameter.DomainFullPathName = DomainFullPathName;
                            JsonInterFace.LteDeviceParameter.Station = DomainFullNameTmp[DomainFullNameTmp.Length - 1];
                            JsonInterFace.LteDeviceParameter.OnLine = Convert.ToInt32(IsOnline).ToString();

                            //存放日志目示路径更新(LTE)
                            string[] _TmpLogDir = JsonInterFace.LteDeviceSystemMaintenenceParameter.LogFiles.Split(new char[] { '\\' });
                            for (int i = 0; i < _TmpLogDir.Length - 1; i++)
                            {
                                if (i == 0)
                                {
                                    JsonInterFace.LteDeviceSystemMaintenenceParameter.LogFiles = _TmpLogDir[i];
                                }
                                else
                                {
                                    JsonInterFace.LteDeviceSystemMaintenenceParameter.LogFiles += @"\" + _TmpLogDir[i];
                                }
                            }
                            JsonInterFace.LteDeviceSystemMaintenenceParameter.LogFiles += @"\" + MainWindow.aDeviceSelected.SN;

                            //获取所选设备通用参数(LTE)
                            if (NetWorkClient.ControllerServer.Connected)
                            {
                                NetWorkClient.ControllerServer.Send(JsonInterFace.GetAPGenParaRequest(DomainFullPathName, SelfName));
                            }
                        }
                        else if (DeviceType.GSM == Model)
                        {
                            JsonInterFace.GSMDeviceParameter.SelfID = SelfID;
                            JsonInterFace.GSMDeviceParameter.ParentID = ParentID;
                            JsonInterFace.GSMDeviceParameter.DeviceName = SelfName;
                            string DomainFullPathName = string.Empty;
                            string[] DomainFullNameTmp = FullName.ToString().Split(new char[] { '.' });
                            for (int j = 0; j < DomainFullNameTmp.Length - 1; j++)
                            {
                                if (DomainFullPathName == "")
                                {
                                    DomainFullPathName = DomainFullNameTmp[j];
                                }
                                else
                                {
                                    DomainFullPathName += "." + DomainFullNameTmp[j];
                                }
                            }

                            JsonInterFace.GSMDeviceParameter.DomainFullPathName = DomainFullPathName;
                            string[] SelfStation = JsonInterFace.GSMDeviceParameter.DomainFullPathName.Split(new char[] { '.' });
                            JsonInterFace.GSMDeviceParameter.Station = SelfStation[SelfStation.Length - 1];
                            JsonInterFace.GSMDeviceParameter.OnLine = Convert.ToInt32(IsOnline).ToString();

                            for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                            {
                                if (JsonInterFace.APATTributesLists[i].FullName.Equals(FullName))
                                {
                                    JsonInterFace.GSMDeviceParameter.DeviceMode = JsonInterFace.APATTributesLists[i].Mode;

                                    //待返回值(接口须升级)
                                    //JsonInterFace.GSMDeviceParameter.StaticIPMode = JsonInterFace.APATTributesLists[i].StaticIPMode;
                                    //JsonInterFace.GSMDeviceParameter.DynamicIPMode = JsonInterFace.APATTributesLists[i].DynamicIPMode;

                                    JsonInterFace.GSMDeviceParameter.StaticIPMode = true;
                                    JsonInterFace.GSMDeviceParameter.DynamicIPMode = false;
                                    JsonInterFace.GSMDeviceParameter.IpAddr = JsonInterFace.APATTributesLists[i].IpAddr;
                                    JsonInterFace.GSMDeviceParameter.Port = JsonInterFace.APATTributesLists[i].Port;
                                    JsonInterFace.GSMDeviceParameter.NetMask = JsonInterFace.APATTributesLists[i].NetMask;
                                    JsonInterFace.GSMDeviceParameter.SN = JsonInterFace.APATTributesLists[i].SN;
                                    JsonInterFace.GSMDeviceParameter.InnerType = JsonInterFace.APATTributesLists[i].InnerType;

                                    //待返回值(接口须升级)
                                    //JsonInterFace.GSMDeviceParameter.DeviceIdentificationMode = JsonInterFace.APATTributesLists[i].DeviceIdentificationMode;

                                    JsonInterFace.GSMDeviceParameter.DeviceIdentificationMode = "设备";
                                    break;
                                }
                            }
                        }
                        else if (DeviceType.WCDMA == Model)
                        {
                            //获取所选设备通用参数(WCDMA)
                            string DomainFullPathName = string.Empty;
                            string deviceName = SelfName;
                            string[] DomainFullNameTmp = FullName.Split(new char[] { '.' });
                            for (int j = 0; j < DomainFullNameTmp.Length - 1; j++)
                            {
                                if (DomainFullPathName.Trim().Equals(""))
                                {
                                    DomainFullPathName = DomainFullNameTmp[j];
                                }
                                else
                                {
                                    DomainFullPathName += "." + DomainFullNameTmp[j];
                                }
                            }
                            JsonInterFace.WCDMADeviceParameter.DeviceName = SelfName;
                            JsonInterFace.WCDMADeviceParameter.DomainFullPathName = DomainFullPathName;
                            JsonInterFace.WCDMADeviceParameter.OnLine = Convert.ToInt32(IsOnline).ToString();

                            //存放日志目示路径更新(LTE)
                            string[] _TmpLogDir = JsonInterFace.WCDMADeviceSystemMaintenenceParameter.LogFiles.Split(new char[] { '\\' });
                            for (int i = 0; i < _TmpLogDir.Length - 1; i++)
                            {
                                if (i == 0)
                                {
                                    JsonInterFace.WCDMADeviceSystemMaintenenceParameter.LogFiles = _TmpLogDir[i];
                                }
                                else
                                {
                                    JsonInterFace.WCDMADeviceSystemMaintenenceParameter.LogFiles += @"\" + _TmpLogDir[i];
                                }
                            }
                            JsonInterFace.WCDMADeviceSystemMaintenenceParameter.LogFiles += @"\" + MainWindow.aDeviceSelected.SN;

                            if (NetWorkClient.ControllerServer.Connected)
                            {
                                NetWorkClient.ControllerServer.Send(JsonInterFace.GetAPGenParaRequest(DomainFullPathName, deviceName));
                            }
                        }
                        else if (DeviceType.GSMV2 == Model)
                        {
                            JsonInterFace.GSMV2DeviceParameter.SelfID = SelfID;
                            JsonInterFace.GSMV2DeviceParameter.ParentID = ParentID;
                            JsonInterFace.GSMV2DeviceParameter.DeviceName = SelfName;

                            string DomainFullPathName = string.Empty;
                            string[] DomainFullNameTmp = FullName.ToString().Split(new char[] { '.' });
                            for (int j = 0; j < DomainFullNameTmp.Length - 1; j++)
                            {
                                if (DomainFullPathName == "")
                                {
                                    DomainFullPathName = DomainFullNameTmp[j];
                                }
                                else
                                {
                                    DomainFullPathName += "." + DomainFullNameTmp[j];
                                }
                            }
                            JsonInterFace.GSMV2DeviceParameter.DomainFullPathName = DomainFullPathName;
                            string[] SelfStation = JsonInterFace.GSMV2DeviceParameter.DomainFullPathName.Split(new char[] { '.' });
                            JsonInterFace.GSMV2DeviceParameter.Station = SelfStation[SelfStation.Length - 1];
                            JsonInterFace.GSMV2DeviceParameter.OnLine = Convert.ToInt32(IsOnline).ToString();
                            JsonInterFace.GSMV2CarrierParameter.CarrierOne = true;
                            string Carrier = (Convert.ToInt32(JsonInterFace.GSMV2CarrierParameter.CarrierOne) - 1).ToString();
                            for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                            {
                                if (JsonInterFace.APATTributesLists[i].FullName.Equals(FullName))
                                {
                                    JsonInterFace.GSMV2DeviceParameter.DeviceMode = JsonInterFace.APATTributesLists[i].Mode;

                                    //待返回值(接口须升级)
                                    //JsonInterFace.CDMADeviceParameter.StaticIPMode = JsonInterFace.APATTributesLists[i].StaticIPMode;
                                    //JsonInterFace.CDMADeviceParameter.DynamicIPMode = JsonInterFace.APATTributesLists[i].DynamicIPMode;

                                    JsonInterFace.GSMV2DeviceParameter.StaticIPMode = true;
                                    JsonInterFace.GSMV2DeviceParameter.DynamicIPMode = false;
                                    JsonInterFace.GSMV2DeviceParameter.IpAddr = JsonInterFace.APATTributesLists[i].IpAddr;
                                    JsonInterFace.GSMV2DeviceParameter.Port = JsonInterFace.APATTributesLists[i].Port;
                                    JsonInterFace.GSMV2DeviceParameter.NetMask = JsonInterFace.APATTributesLists[i].NetMask;
                                    JsonInterFace.GSMV2DeviceParameter.SN = JsonInterFace.APATTributesLists[i].SN;
                                    JsonInterFace.GSMV2DeviceParameter.InnerType = JsonInterFace.APATTributesLists[i].InnerType;
                                    JsonInterFace.GSMV2CellNeighParameter.HardWareID = JsonInterFace.APATTributesLists[i].HardWareID;

                                    //待返回值(接口须升级)
                                    //JsonInterFace.CDMADeviceParameter.DeviceIdentificationMode = JsonInterFace.APATTributesLists[i].DeviceIdentificationMode;

                                    JsonInterFace.GSMV2DeviceParameter.DeviceIdentificationMode = "设备";
                                    break;
                                }
                            }
                            //小区参数
                            JsonInterFace.ResultMessageList.Clear();
                            JsonInterFace.GSMV2ConfigSMSMSG.GSMV2ConfigSMSMSGDataTab.Clear();
                            JsonInterFace.GSMV2UEReportInfo.UEReportDataTab.Clear();
                            //Parameters.ConfigType = "Query";
                            NetWorkClient.ControllerServer.Send(JsonInterFace.GSMV2CellPARAMRequest(
                                                                                                    JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.Port,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.InnerType,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.SN,
                                                                                                    "1"
                                                                                                  )
                                                               );
                            NetWorkClient.ControllerServer.Send(JsonInterFace.GSMV2CellPARAMRequest(
                                                                                                    JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.Port,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.InnerType,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.SN,
                                                                                                    "0"
                                                                                                  )
                                                               );
                            //获取非XML消息
                            NetWorkClient.ControllerServer.Send(JsonInterFace.Get_Upload_Request(
                                                                                        JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                        JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                        JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                                                                        JsonInterFace.GSMV2DeviceParameter.Port,
                                                                                        JsonInterFace.GSMV2DeviceParameter.InnerType,
                                                                                        JsonInterFace.GSMV2DeviceParameter.SN
                                                                                    ));
                        }
                        else if (DeviceType.CDMA == Model)
                        {
                            JsonInterFace.CDMADeviceParameter.SelfID = SelfID;
                            JsonInterFace.CDMADeviceParameter.ParentID = ParentID;
                            JsonInterFace.CDMADeviceParameter.DeviceName = SelfName;

                            string DomainFullPathName = string.Empty;
                            string[] DomainFullNameTmp = FullName.ToString().Split(new char[] { '.' });
                            for (int j = 0; j < DomainFullNameTmp.Length - 1; j++)
                            {
                                if (DomainFullPathName == "")
                                {
                                    DomainFullPathName = DomainFullNameTmp[j];
                                }
                                else
                                {
                                    DomainFullPathName += "." + DomainFullNameTmp[j];
                                }
                            }

                            JsonInterFace.CDMADeviceParameter.DomainFullPathName = DomainFullPathName;
                            string[] SelfStation = JsonInterFace.CDMADeviceParameter.DomainFullPathName.Split(new char[] { '.' });
                            JsonInterFace.CDMADeviceParameter.Station = SelfStation[SelfStation.Length - 1];
                            JsonInterFace.CDMADeviceParameter.OnLine = Convert.ToInt32(IsOnline).ToString();

                            for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                            {
                                if (JsonInterFace.APATTributesLists[i].FullName.Equals(FullName))
                                {
                                    JsonInterFace.CDMADeviceParameter.DeviceMode = JsonInterFace.APATTributesLists[i].Mode;

                                    //待返回值(接口须升级)
                                    //JsonInterFace.CDMADeviceParameter.StaticIPMode = JsonInterFace.APATTributesLists[i].StaticIPMode;
                                    //JsonInterFace.CDMADeviceParameter.DynamicIPMode = JsonInterFace.APATTributesLists[i].DynamicIPMode;

                                    JsonInterFace.CDMADeviceParameter.StaticIPMode = true;
                                    JsonInterFace.CDMADeviceParameter.DynamicIPMode = false;
                                    JsonInterFace.CDMADeviceParameter.IpAddr = JsonInterFace.APATTributesLists[i].IpAddr;
                                    JsonInterFace.CDMADeviceParameter.Port = JsonInterFace.APATTributesLists[i].Port;
                                    JsonInterFace.CDMADeviceParameter.NetMask = JsonInterFace.APATTributesLists[i].NetMask;
                                    JsonInterFace.CDMADeviceParameter.SN = JsonInterFace.APATTributesLists[i].SN;
                                    JsonInterFace.CDMADeviceParameter.InnerType = JsonInterFace.APATTributesLists[i].InnerType;
                                    JsonInterFace.CDMACellNeighParameter.HardWareID = JsonInterFace.APATTributesLists[i].HardWareID;

                                    //待返回值(接口须升级)
                                    //JsonInterFace.CDMADeviceParameter.DeviceIdentificationMode = JsonInterFace.APATTributesLists[i].DeviceIdentificationMode;

                                    JsonInterFace.CDMADeviceParameter.DeviceIdentificationMode = "设备";
                                    break;
                                }
                            }
                            //获取所选设备相关参数(CDMA)
                            if (NetWorkClient.ControllerServer.Connected)
                            {
                                JsonInterFace.ResultMessageList.Clear();
                                JsonInterFace.CDMAConfigSMSMSG.GSMV2ConfigSMSMSGDataTab.Clear();
                                Parameters.ConfigType = "Auto";

                                //小区参数
                                NetWorkClient.ControllerServer.Send(
                                                                    JsonInterFace.CDMACellPARAMRequest(
                                                                                                        JsonInterFace.CDMADeviceParameter.DomainFullPathName,
                                                                                                        JsonInterFace.CDMADeviceParameter.DeviceName,
                                                                                                        JsonInterFace.CDMADeviceParameter.IpAddr,
                                                                                                        JsonInterFace.CDMADeviceParameter.Port,
                                                                                                        JsonInterFace.CDMADeviceParameter.InnerType,
                                                                                                        JsonInterFace.CDMADeviceParameter.SN
                                                                                                      )
                                                                   );

                                //多载波信息
                                NetWorkClient.ControllerServer.Send(
                                                                    JsonInterFace.CDMAMultiCarrierQueryRequest(
                                                                                                                    JsonInterFace.CDMADeviceParameter.DomainFullPathName,
                                                                                                                    JsonInterFace.CDMADeviceParameter.DeviceName,
                                                                                                                    JsonInterFace.CDMADeviceParameter.IpAddr,
                                                                                                                    JsonInterFace.CDMADeviceParameter.Port,
                                                                                                                    JsonInterFace.CDMADeviceParameter.InnerType,
                                                                                                                    JsonInterFace.CDMADeviceParameter.SN
                                                                                                                )
                                                                   );

                                //IMSI信息
                                NetWorkClient.ControllerServer.Send(
                                                                    JsonInterFace.CDMAIMSIQueryRequest(
                                                                                                        JsonInterFace.CDMADeviceParameter.DomainFullPathName,
                                                                                                        JsonInterFace.CDMADeviceParameter.DeviceName,
                                                                                                        JsonInterFace.CDMADeviceParameter.IpAddr,
                                                                                                        JsonInterFace.CDMADeviceParameter.Port,
                                                                                                        JsonInterFace.CDMADeviceParameter.InnerType,
                                                                                                        JsonInterFace.CDMADeviceParameter.SN
                                                                                                       )
                                                                   );
                                //获取非XML消息
                                NetWorkClient.ControllerServer.Send(JsonInterFace.Get_Upload_Request(
                                                                                            JsonInterFace.CDMADeviceParameter.DomainFullPathName,
                                                                                            JsonInterFace.CDMADeviceParameter.DeviceName,
                                                                                            JsonInterFace.CDMADeviceParameter.IpAddr,
                                                                                            JsonInterFace.CDMADeviceParameter.Port,
                                                                                            JsonInterFace.CDMADeviceParameter.InnerType,
                                                                                            JsonInterFace.CDMADeviceParameter.SN
                                                                                        ));
                            }
                        }
                        else if (DeviceType.TD_SCDMA == Model)
                        {
                            //获取所选设备通用参数(TDS)
                            string DomainFullPathName = string.Empty;
                            string deviceName = SelfName;
                            string[] DomainFullNameTmp = FullName.Split(new char[] { '.' });
                            for (int j = 0; j < DomainFullNameTmp.Length - 1; j++)
                            {
                                if (DomainFullPathName.Trim().Equals(""))
                                {
                                    DomainFullPathName = DomainFullNameTmp[j];
                                }
                                else
                                {
                                    DomainFullPathName += "." + DomainFullNameTmp[j];
                                }
                            }
                            JsonInterFace.TDSDeviceParameter.DeviceName = SelfName;
                            JsonInterFace.TDSDeviceParameter.DomainFullPathName = DomainFullPathName;
                            JsonInterFace.TDSDeviceParameter.OnLine = Convert.ToInt32(IsOnline).ToString();

                            if (NetWorkClient.ControllerServer.Connected)
                            {
                                NetWorkClient.ControllerServer.Send(JsonInterFace.GetAPGenParaRequest(DomainFullPathName, deviceName));
                            }
                        }
                    }
                    //selfParam赋值
                    for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                    {
                        if (JsonInterFace.APATTributesLists[i].FullName.Equals(FullName))
                        {
                            selfParam.DomainFullNamePath = JsonInterFace.APATTributesLists[i].FullName;
                            selfParam.Station = JsonInterFace.APATTributesLists[i].Station;
                            selfParam.DeviceName = JsonInterFace.APATTributesLists[i].SelfName;
                            selfParam.Mode = JsonInterFace.APATTributesLists[i].Mode;
                            selfParam.IP = JsonInterFace.APATTributesLists[i].IpAddr;
                            selfParam.Port = JsonInterFace.APATTributesLists[i].Port;
                            selfParam.NetMask = JsonInterFace.APATTributesLists[i].NetMask;
                            selfParam.SN = JsonInterFace.APATTributesLists[i].SN;
                            selfParam.DeviceNameFlag = JsonInterFace.APATTributesLists[i].FullName;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void btnGSMV2UpdateTime_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string Carrier = string.Empty;
                if ((bool)chkGSMV2ActiveTimeCarrierTwo.IsChecked)
                {
                    Carrier = "1";
                }
                else if ((bool)chkGSMV2ActiveTimeCarrierOne.IsChecked)
                {
                    Carrier = "0";
                }
                else
                {
                    MessageBox.Show("请选择载波！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if ((txtGSMV2FirstPeriodTimeStart.Text != ""
                && txtGSMV2FirstPeriodTimeEnd.Text == "")
                || (txtGSMV2FirstPeriodTimeStart.Text == ""
                && txtGSMV2FirstPeriodTimeEnd.Text != ""))
                {
                    MessageBox.Show("第一时间段的时间不完整！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else if (txtGSMV2FirstPeriodTimeStart.Text != "" && txtGSMV2FirstPeriodTimeEnd.Text == "")
                {
                    //检测时间
                    if (!Parameters.IsTime(JsonInterFace.GSMV2DeviceAdvanceSettingParameter.FirstPeriodTimeStart))
                    {
                        MessageBox.Show("第一时间段的起始时间格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (!Parameters.IsTime(JsonInterFace.GSMV2DeviceAdvanceSettingParameter.FirstPeriodTimeEnd))
                    {
                        MessageBox.Show("第一时间段的结束时间格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                if ((txtGSMV2SecondPeriodTimeStart.Text != ""
                    && txtGSMV2SecoondPeriodTimeEnd.Text == "")
                    || (txtGSMV2SecondPeriodTimeStart.Text == ""
                    && txtGSMV2SecoondPeriodTimeEnd.Text != ""))
                {
                    MessageBox.Show("第二时间段的时间不完整！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else if (txtGSMV2SecondPeriodTimeStart.Text == "" && txtGSMV2SecoondPeriodTimeEnd.Text != "")
                {
                    //检测时间
                    if (!Parameters.IsTime(JsonInterFace.GSMV2DeviceAdvanceSettingParameter.SecondPeriodTimeStart))
                    {
                        MessageBox.Show("第二时间段的起始时间格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (!Parameters.IsTime(JsonInterFace.GSMV2DeviceAdvanceSettingParameter.SecoondPeriodTimeEnd))
                    {
                        MessageBox.Show("第二时间段的结束时间格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                if ((txtGSMV2ThreePeriodTimeStart.Text != ""
                    && txtGSMV2ThreePeriodTimeEnd.Text == "")
                    || (txtGSMV2ThreePeriodTimeStart.Text == ""
                    && txtGSMV2ThreePeriodTimeEnd.Text != ""))
                {
                    MessageBox.Show("第三时间段的时间不完整！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else if (txtGSMV2ThreePeriodTimeStart.Text == "" && txtGSMV2ThreePeriodTimeEnd.Text != "")
                {
                    //检测时间
                    if (!Parameters.IsTime(JsonInterFace.GSMV2DeviceAdvanceSettingParameter.ThreePeriodTimeStart))
                    {
                        MessageBox.Show("第三时间段的起始时间格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (!Parameters.IsTime(JsonInterFace.GSMV2DeviceAdvanceSettingParameter.ThreePeriodTimeEnd))
                    {
                        MessageBox.Show("第三时间段的结束时间格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                if (NetWorkClient.ControllerServer.Connected)
                {
                    Dictionary<string, string> ApperiodTimeList = new Dictionary<string, string>();
                    ApperiodTimeList.Add("activeTime1Start", JsonInterFace.GSMV2DeviceAdvanceSettingParameter.FirstPeriodTimeStart);
                    ApperiodTimeList.Add("activeTime1Ended", JsonInterFace.GSMV2DeviceAdvanceSettingParameter.FirstPeriodTimeEnd);
                    ApperiodTimeList.Add("activeTime2Start", JsonInterFace.GSMV2DeviceAdvanceSettingParameter.SecondPeriodTimeStart);
                    ApperiodTimeList.Add("activeTime2Ended", JsonInterFace.GSMV2DeviceAdvanceSettingParameter.SecoondPeriodTimeEnd);
                    ApperiodTimeList.Add("activeTime3Start", JsonInterFace.GSMV2DeviceAdvanceSettingParameter.ThreePeriodTimeStart);
                    ApperiodTimeList.Add("activeTime3Ended", JsonInterFace.GSMV2DeviceAdvanceSettingParameter.ThreePeriodTimeEnd);

                    Parameters.ConfigType = DeviceType.GSMV2;
                    NetWorkClient.ControllerServer.Send(JsonInterFace.GSMPeriodTimeConrolRequest(
                                                                                                JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                                JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                                Carrier,
                                                                                                ApperiodTimeList
                                                                                               ));
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("时段控制", ex.Message, ex.StackTrace);
            }
        }

        private void btnWCDMAUpdateTime_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if ((txtWCDMAFirstPeriodTimeStart.Text != ""
                && txtWCDMAFirstPeriodTimeEnd.Text == "")
                || (txtWCDMAFirstPeriodTimeStart.Text == ""
                && txtWCDMAFirstPeriodTimeEnd.Text != ""))
                {
                    MessageBox.Show("第一时间段的时间不完整！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else if (txtWCDMAFirstPeriodTimeStart.Text != "" && txtWCDMAFirstPeriodTimeEnd.Text != "")
                {
                    //检测时间
                    if (!Parameters.IsTime(JsonInterFace.WCDMADeviceAdvanceSettingParameter.FirstPeriodTimeStart))
                    {
                        MessageBox.Show("第一时间段的起始时间格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (!Parameters.IsTime(JsonInterFace.WCDMADeviceAdvanceSettingParameter.FirstPeriodTimeEnd))
                    {
                        MessageBox.Show("第一时间段的结束时间格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                if ((txtWCDMASecondPeriodTimeStart.Text != ""
                    && txtWCDMASecoondPeriodTimeEnd.Text == "")
                    || (txtWCDMASecondPeriodTimeStart.Text == ""
                    && txtWCDMASecoondPeriodTimeEnd.Text != ""))
                {
                    MessageBox.Show("第二时间段的时间不完整！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else if (txtWCDMASecondPeriodTimeStart.Text != "" && txtWCDMASecoondPeriodTimeEnd.Text != "")
                {
                    //检测时间
                    if (!Parameters.IsTime(JsonInterFace.WCDMADeviceAdvanceSettingParameter.SecondPeriodTimeStart))
                    {
                        MessageBox.Show("第二时间段的起始时间格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (!Parameters.IsTime(JsonInterFace.WCDMADeviceAdvanceSettingParameter.SecoondPeriodTimeEnd))
                    {
                        MessageBox.Show("第二时间段的结束时间格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                if ((txtWCDMAThreePeriodTimeStart.Text != ""
                    && txtWCDMAThreePeriodTimeEnd.Text == "")
                    || (txtWCDMAThreePeriodTimeStart.Text == ""
                    && txtWCDMAThreePeriodTimeEnd.Text != ""))
                {
                    MessageBox.Show("第三时间段的时间不完整！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else if (txtWCDMAThreePeriodTimeStart.Text != "" && txtWCDMAThreePeriodTimeEnd.Text != "")
                {
                    //检测时间
                    if (!Parameters.IsTime(JsonInterFace.WCDMADeviceAdvanceSettingParameter.ThreePeriodTimeStart))
                    {
                        MessageBox.Show("第三时间段的起始时间格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (!Parameters.IsTime(JsonInterFace.WCDMADeviceAdvanceSettingParameter.ThreePeriodTimeEnd))
                    {
                        MessageBox.Show("第三时间段的结束时间格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                if (NetWorkClient.ControllerServer.Connected)
                {
                    Dictionary<string, string> ApperiodTimeList = new Dictionary<string, string>();
                    ApperiodTimeList.Add("activeTime1Start", JsonInterFace.WCDMADeviceAdvanceSettingParameter.FirstPeriodTimeStart);
                    ApperiodTimeList.Add("activeTime1Ended", JsonInterFace.WCDMADeviceAdvanceSettingParameter.FirstPeriodTimeEnd);
                    ApperiodTimeList.Add("activeTime2Start", JsonInterFace.WCDMADeviceAdvanceSettingParameter.SecondPeriodTimeStart);
                    ApperiodTimeList.Add("activeTime2Ended", JsonInterFace.WCDMADeviceAdvanceSettingParameter.SecoondPeriodTimeEnd);
                    ApperiodTimeList.Add("activeTime3Start", JsonInterFace.WCDMADeviceAdvanceSettingParameter.ThreePeriodTimeStart);
                    ApperiodTimeList.Add("activeTime3Ended", JsonInterFace.WCDMADeviceAdvanceSettingParameter.ThreePeriodTimeEnd);

                    string DomainFullPathName = string.Empty;
                    string[] DomainFullPathNameTmp = JsonInterFace.WCDMADeviceAdvanceSettingParameter.DomainFullPathName.Split(new char[] { '.' });
                    for (int i = 0; i < DomainFullPathNameTmp.Length - 1; i++)
                    {
                        if (DomainFullPathName.Equals(""))
                        {
                            DomainFullPathName = DomainFullPathNameTmp[i];
                        }
                        else
                        {
                            DomainFullPathName += "." + DomainFullPathNameTmp[i];
                        }
                    }
                    Parameters.ConfigType = DeviceType.WCDMA;
                    NetWorkClient.ControllerServer.Send(JsonInterFace.APPeriodTimeConrolRequest(
                                                                                                DomainFullPathName,
                                                                                                JsonInterFace.WCDMADeviceAdvanceSettingParameter.DeviceName,
                                                                                                ApperiodTimeList
                                                                                                ));
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("时段控制", ex.Message, ex.StackTrace);
            }
        }

        private void cbGSMV2SMSCarrierOne_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)cbGSMV2SMSCarrierOne.IsChecked)
            {
                cbGSMV2SMSCarrierTwo.IsChecked = false;
            }
            else
            {
                if (!(bool)cbGSMV2SMSCarrierTwo.IsChecked)
                {
                    cbGSMV2SMSCarrierOne.IsChecked = true;
                }
            }
            if (NetWorkClient.ControllerServer.Connected)
            {
                NetWorkClient.ControllerServer.Send(JsonInterFace.GSMV2UEReportRequest(
                                                                                                    JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.Port,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.InnerType,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.SN,
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

        private void cbGSMV2SMSCarrierTwo_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)cbGSMV2SMSCarrierTwo.IsChecked)
            {
                cbGSMV2SMSCarrierOne.IsChecked = false;
            }
            else
            {
                if (!(bool)cbGSMV2SMSCarrierOne.IsChecked)
                {
                    cbGSMV2SMSCarrierTwo.IsChecked = true;
                }
            }
            if (NetWorkClient.ControllerServer.Connected)
            {
                NetWorkClient.ControllerServer.Send(JsonInterFace.GSMV2UEReportRequest(
                                                                                                    JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.Port,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.InnerType,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.SN,
                                                                                                    "1"
                                                                                                  )
                                                               );
            }
            else
            {
                MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        private void chkGSMV2ActiveTimeCarrierOne_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((bool)chkGSMV2ActiveTimeCarrierOne.IsChecked)
                {
                    chkGSMV2ActiveTimeCarrierTwo.IsChecked = false;
                }
                else
                {
                    if (!(bool)chkGSMV2ActiveTimeCarrierTwo.IsChecked)
                    {
                        chkGSMV2ActiveTimeCarrierOne.IsChecked = true;
                    }
                }
                string Carrier = (Convert.ToInt32(chkGSMV2ActiveTimeCarrierOne.IsChecked) - 1).ToString();
                if (NetWorkClient.ControllerServer.Connected)
                {
                    NetWorkClient.ControllerServer.Send(JsonInterFace.GSMV2CellPARAMRequest(
                                                                                            JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                            JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                            JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                                                                            JsonInterFace.GSMV2DeviceParameter.Port,
                                                                                            JsonInterFace.GSMV2DeviceParameter.InnerType,
                                                                                            JsonInterFace.GSMV2DeviceParameter.SN,
                                                                                            Carrier
                                                                                            )
                                                               );
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

        private void chkGSMV2ActiveTimeCarrierTwo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((bool)chkGSMV2ActiveTimeCarrierTwo.IsChecked)
                {
                    chkGSMV2ActiveTimeCarrierOne.IsChecked = false;
                }
                else
                {
                    if (!(bool)chkGSMV2ActiveTimeCarrierOne.IsChecked)
                    {
                        chkGSMV2ActiveTimeCarrierTwo.IsChecked = true;
                    }
                }
                string Carrier = (Convert.ToInt32(chkGSMV2ActiveTimeCarrierTwo.IsChecked)).ToString();
                if (NetWorkClient.ControllerServer.Connected)
                {
                    NetWorkClient.ControllerServer.Send(JsonInterFace.GSMV2CellPARAMRequest(
                                                                                            JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                            JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                            JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                                                                            JsonInterFace.GSMV2DeviceParameter.Port,
                                                                                            JsonInterFace.GSMV2DeviceParameter.InnerType,
                                                                                            JsonInterFace.GSMV2DeviceParameter.SN,
                                                                                            Carrier
                                                                                            )
                                                               );
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

        private void btnWCDMAFreqUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Parameters.ConfigType = "GPS";
                if (!(bool)rbWCDMAConfigure.IsChecked && !(bool)rbWCDMAUnConfigure.IsChecked)
                {
                    MessageBox.Show("是否配置GPS？", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (txtWCDMAFreqOffsetSetting.Text.Trim().Equals(""))
                {
                    if (MessageBox.Show("频偏是否为空？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                }
                if (!NetWorkClient.ControllerServer.Connected)
                {
                    if (MessageBox.Show("网络与服务器已断开！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                }

                if (Parameters.ConfigType.Trim().Equals(""))
                {
                    MessageBox.Show("未配置ConfigType类型！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                NetWorkClient.ControllerServer.Send(JsonInterFace.APGPSConfigrationRequest(
                                                                                            JsonInterFace.WCDMADeviceAdvanceSettingParameter.DomainFullPathName,
                                                                                            JsonInterFace.WCDMADeviceAdvanceSettingParameter.DeviceName,
                                                                                            JsonInterFace.WCDMADeviceAdvanceSettingParameter.IpAddr,
                                                                                            int.Parse(JsonInterFace.WCDMADeviceAdvanceSettingParameter.Port),
                                                                                            JsonInterFace.WCDMADeviceAdvanceSettingParameter.InnerType,
                                                                                            JsonInterFace.WCDMADeviceAdvanceSettingParameter.SN,
                                                                                            Convert.ToInt32(JsonInterFace.WCDMADeviceAdvanceSettingParameter.GPSStatusConfig),
                                                                                            JsonInterFace.WCDMADeviceAdvanceSettingParameter.FrequencyOffsetList
                                                                                          ));
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("频偏设置", ex.Message, ex.StackTrace);
            }
        }

        private void btnWCDMANTPUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Parameters.ConfigType = "NTP";
                if (txtWCDMANTP.Text.Equals(""))
                {
                    MessageBox.Show("请输入NTP服务器地址！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (txtWCDMAPriority.Text.Equals(""))
                {
                    MessageBox.Show("请输入NTP优先级！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (Parameters.ConfigType.Trim().Equals(""))
                {
                    MessageBox.Show("未配置ConfigType类型！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (NetWorkClient.ControllerServer.Connected)
                {
                    NetWorkClient.ControllerServer.Send(JsonInterFace.NTPConfigrationRequest(
                                                                                                JsonInterFace.WCDMADeviceAdvanceSettingParameter.DomainFullPathName,
                                                                                                JsonInterFace.WCDMADeviceAdvanceSettingParameter.DeviceName,
                                                                                                JsonInterFace.WCDMADeviceAdvanceSettingParameter.IpAddr,
                                                                                                int.Parse(JsonInterFace.WCDMADeviceAdvanceSettingParameter.Port),
                                                                                                JsonInterFace.WCDMADeviceAdvanceSettingParameter.InnerType,
                                                                                                JsonInterFace.WCDMADeviceAdvanceSettingParameter.SN,
                                                                                                JsonInterFace.WCDMADeviceAdvanceSettingParameter.NTPServerIP,
                                                                                                JsonInterFace.WCDMADeviceAdvanceSettingParameter.NTPLevel
                                                                                            ));
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("NTP优先级设置", ex.Message, ex.StackTrace);
            }
        }

        private void btnWCDMAFileBrower_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog logFileName = new Microsoft.Win32.OpenFileDialog();
            if ((bool)logFileName.ShowDialog())
            {
                JsonInterFace.WCDMADeviceSystemMaintenenceParameter.UpgradeFile = logFileName.FileName;
            }
        }

        private void btnWCDMALogFileBrower_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog SelectDir = new System.Windows.Forms.FolderBrowserDialog();

            if (txtWCDMALogFilePath.Text != "")
            {
                if (Directory.Exists(txtWCDMALogFilePath.Text))
                {
                    SelectDir.SelectedPath = txtWCDMALogFilePath.Text;
                }
            }

            if (SelectDir.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                JsonInterFace.WCDMADeviceSystemMaintenenceParameter.LogFiles = SelectDir.SelectedPath;
            }
        }

        private void btnWCDMADownLoadlogs_Click(object sender, RoutedEventArgs e)
        {
            string SelfID = string.Empty;
            string SelfName = string.Empty;
            try
            {
                if (txtWCDMALogFilePath.Text == (""))
                {
                    MessageBox.Show("请选择存放AP日志的目标文件夹！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    btnWCDMALogFileBrower_Click(sender, e);
                    return;
                }
                else
                {
                    if (!Directory.Exists(txtWCDMALogFilePath.Text))
                    {
                        try
                        {
                            Directory.CreateDirectory(txtWCDMALogFilePath.Text);
                        }
                        catch (Exception Ex)
                        {
                            Parameters.PrintfLogsExtended("创建存放AP日志的目标文件夹失败", Ex.Message, Ex.StackTrace);
                            MessageBox.Show("请选择存放AP日志的目标文件夹，当前指定的文件路径无效不可用！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }
                }

                if (new Regex(DeviceType.LTE).Match(MainWindow.aDeviceSelected.Model).Success
                    || DeviceType.WCDMA == MainWindow.aDeviceSelected.Model)
                {
                    DownLoadLogsTask(sender, e);
                }
                else
                {
                    MessageBox.Show("目前下载AP日志只支持[LTE系列]及[WCDMA]设备！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void btnGSMV2FileBrower_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnGSMV2UpdateFile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnGSMV2LogFileBrower_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnGSMV2DownLoadlogs_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnGSMV2SendInfo_Click(object sender, RoutedEventArgs e)
        {
            if (txtGSMV2parameterCommandList.Text.Trim() != "")
            {
                if (NetWorkClient.ControllerServer.Connected)
                {
                    NetWorkClient.ControllerServer.Send(JsonInterFace.APProjectSettingRequest(
                                                                                                JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                                JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                                JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                                                                                JsonInterFace.GSMV2DeviceParameter.Port,
                                                                                                JsonInterFace.GSMV2DeviceParameter.InnerType,
                                                                                                JsonInterFace.GSMV2DeviceParameter.SN
                                                                                            ));
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("参数内容为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void miGSMV2ObjectResultClear_Click(object sender, RoutedEventArgs e)
        {
            JsonInterFace.GSMV2DeviceObjectSettingParameter.ParameterResultValue = string.Empty;
        }

        /// <summary>
        /// 单项参数设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdatePragram_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbParameterKeyName.Text != "" && cbParameterKeyName.Text != null
                    && txtParameterValue.Text != "" && txtParameterValue.Text != null)
                {
                    string ParameterValue;
                    if (cbParameterKeyName.SelectedIndex == 1 && cbParameterValue.Items.Count > 0)
                    {
                        if (cbParameterValue.Items.IndexOf(txtParameterValue.Text) > -1)
                        {
                            ParameterValue = (cbParameterValue.Items.IndexOf(txtParameterValue.Text) + 1).ToString();
                        }
                        else
                        {
                            ParameterValue = txtParameterValue.Text;
                        }
                    }
                    else if (cbParameterValue.Items.Count > 0)
                    {
                        if (cbParameterValue.Items.IndexOf(txtParameterValue.Text) > -1)
                        {
                            ParameterValue = cbParameterValue.Items.IndexOf(txtParameterValue.Text).ToString();
                        }
                        else
                        {
                            ParameterValue = txtParameterValue.Text;
                        }
                    }
                    else
                    {
                        ParameterValue = txtParameterValue.Text;
                    }
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        Parameters.ConfigType = "ProjectSetting";
                        NetWorkClient.ControllerServer.Send(JsonInterFace.APProjectSingleParameterSettingRequest(
                                                                                                    JsonInterFace.LteDeviceParameter.DomainFullPathName,
                                                                                                    JsonInterFace.LteDeviceParameter.DeviceName,
                                                                                                    JsonInterFace.LteDeviceParameter.IpAddr,
                                                                                                    JsonInterFace.LteDeviceParameter.Port,
                                                                                                    JsonInterFace.LteDeviceParameter.InnerType,
                                                                                                    JsonInterFace.LteDeviceParameter.SN,
                                                                                                    ParameterKeyName,
                                                                                                    ParameterValue
                                                                                                ));
                    }
                    else
                    {
                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("参数内容为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("单项参数设置，" + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnGSMSMSEdit_Click(object sender, RoutedEventArgs e)
        {
            int SelectIndex = GSMSMSMessageDataGrid.SelectedIndex;
            if (MessageBox.Show("确定编辑该信息吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                try
                {
                    //编缉前缓存 GSMSMSTmp
                    GSMSMSTmp.CarrierOne = ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).CarrierOne;
                    GSMSMSTmp.CarrierTwo = ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).CarrierTwo;
                    GSMSMSTmp.GSmsData = ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).GSmsData;
                    GSMSMSTmp.GSmsRpoa = ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).GSmsRpoa;
                    GSMSMSTmp.GSmsTpoa = ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).GSmsTpoa;
                    GSMSMSTmp.GSmsScts = ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).GSmsScts;
                    GSMSMSTmp.AutoSendtiny = ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).AutoSendtiny;
                    GSMSMSTmp.AutoFilterSMStiny = ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).AutoFilterSMStiny;
                    GSMSMSTmp.DelayTime = ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).DelayTime;
                    GSMSMSTmp.SmsCodingtiny = ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).SmsCodingtiny;

                    //编缉
                    JsonInterFace.GSMSMSParameter.CarrierOne = ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).CarrierOne;
                    JsonInterFace.GSMSMSParameter.CarrierTwo = ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).CarrierTwo;
                    JsonInterFace.GSMSMSParameter.GSmsData = ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).GSmsData;
                    JsonInterFace.GSMSMSParameter.GSmsRpoa = ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).GSmsRpoa;
                    JsonInterFace.GSMSMSParameter.GSmsTpoa = ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).GSmsTpoa;
                    JsonInterFace.GSMSMSParameter.GSmsScts = ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).GSmsScts;
                    JsonInterFace.GSMSMSParameter.AutoSendtiny = ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).AutoSendtiny;
                    JsonInterFace.GSMSMSParameter.AutoFilterSMStiny = ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).AutoFilterSMStiny;
                    JsonInterFace.GSMSMSParameter.DelayTime = ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).DelayTime;
                    JsonInterFace.GSMSMSParameter.SmsCodingtiny = ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).SmsCodingtiny;

                    GSMSMSMessageDataGrid.IsEnabled = false;
                    btnGSMSMSEditCancel.Visibility = Visibility.Visible;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("编辑信息失败，" + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// 发送(新增，编辑模式)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGSMSMSSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (JsonInterFace.GSMDeviceParameter.OnLine == "0")
                {
                    MessageBox.Show("设备不在线！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!LongSMSError(txtGSMMessageContent.Text.Trim()))
                {
                    return;
                }

                if (JsonInterFace.GSMSMSParameter.CarrierOne.Equals(false) && JsonInterFace.GSMSMSParameter.CarrierTwo.Equals(false))
                {
                    MessageBox.Show("请选择载波！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (JsonInterFace.GSMSMSParameter.GSmsData == "" || JsonInterFace.GSMSMSParameter.GSmsData == null)
                {
                    MessageBox.Show("短信息内容为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (JsonInterFace.GSMSMSParameter.GSmsRpoa == "" || JsonInterFace.GSMSMSParameter.GSmsRpoa == null)
                {
                    MessageBox.Show("中心号码为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (JsonInterFace.GSMSMSParameter.GSmsTpoa == "" || JsonInterFace.GSMSMSParameter.GSmsTpoa == null)
                {
                    MessageBox.Show("原叫号码为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (JsonInterFace.GSMSMSParameter.SmsCodingtiny == "" || JsonInterFace.GSMSMSParameter.SmsCodingtiny == null)
                {
                    MessageBox.Show("请选择短信编码方式！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (NetWorkClient.ControllerServer.Connected)
                {
                    //发送时间
                    JsonInterFace.GSMSMSParameter.GSmsScts = string.Format("{0:D4}", DateTime.Now.Year).Substring(2, 2) + string.Format("{0:D2}", DateTime.Now.Month) + string.Format("{0:D2}", DateTime.Now.Day) + string.Format("{0:D2}", DateTime.Now.Hour) + string.Format("{0:D2}", DateTime.Now.Minute) + string.Format("{0:D2}", DateTime.Now.Second);
                    JsonInterFace.ResultMessageList.Clear();
                    NetWorkClient.ControllerServer.Send(
                                                        JsonInterFace.GSMSMSSenderRequest
                                                                                        (
                                                                                            JsonInterFace.GSMDeviceParameter.DomainFullPathName,
                                                                                            JsonInterFace.GSMDeviceParameter.DeviceName,
                                                                                            JsonInterFace.GSMDeviceParameter.IpAddr,
                                                                                            JsonInterFace.GSMDeviceParameter.Port,
                                                                                            JsonInterFace.GSMDeviceParameter.InnerType,
                                                                                            JsonInterFace.GSMDeviceParameter.SN
                                                                                        )
                                                       );
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("短信息发送失败！" + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void cbbGSMSMSEncodingSet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void txtGSMMessageContent_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtGSMMessageContent.Text.Trim().Length <= 0)
            {
                lblContentSize.Content = txtGSMMessageContent.Text.Trim().Length.ToString() + "/" + Parameters.GSMMaxSMSCount.ToString();
                btnGSMSMSSave.IsEnabled = false;
                btnGSMSMSSend.IsEnabled = false;
            }
            else
            {
                if (txtGSMMessageContent.Text.Trim().Length >= Parameters.GSMMaxSMSCount)
                {
                    lblContentSize.Content = txtGSMMessageContent.Text.Trim().Length.ToString() + "/" + Parameters.GSMMaxSMSCount.ToString();
                    lblContentSize.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Yellow"));
                    lblContentSize.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Red"));
                }
                else
                {
                    lblContentSize.Content = txtGSMMessageContent.Text.Trim().Length.ToString() + "/" + Parameters.GSMMaxSMSCount.ToString();
                    lblContentSize.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
                    lblContentSize.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("AliceBlue"));
                }
                btnGSMSMSSave.IsEnabled = true;
                btnGSMSMSSend.IsEnabled = true;
            }
        }

        private void GSMSMSMessageDataGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void tbGSMSMSMessage_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Parameters.SMSBrowse)
            {
                JsonInterFace.GSMSMSParameter.CarrierOne = false;
                JsonInterFace.GSMSMSParameter.CarrierTwo = false;
                JsonInterFace.GSMSMSParameter.GSmsData = null;
                JsonInterFace.GSMSMSParameter.GSmsRpoa = null;
                JsonInterFace.GSMSMSParameter.GSmsTpoa = null;
                JsonInterFace.GSMSMSParameter.GSmsScts = null;
                JsonInterFace.GSMSMSParameter.AutoSendtiny = null;
                JsonInterFace.GSMSMSParameter.AutoFilterSMStiny = null;
                JsonInterFace.GSMSMSParameter.DelayTime = null;
                JsonInterFace.GSMSMSParameter.SmsCodingtiny = null;
            }
        }

        private void tbGSMSMSMessage_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Parameters.SMSBrowse)
            {
                string SMSData = ((TextBlock)sender).Text;
                for (int i = 0; i < GSMSMSList.Count; i++)
                {
                    if (SMSData.Equals(GSMSMSList[i].GSmsData))
                    {
                        JsonInterFace.GSMSMSParameter.CarrierOne = GSMSMSList[i].CarrierOne;
                        JsonInterFace.GSMSMSParameter.CarrierTwo = GSMSMSList[i].CarrierTwo;
                        JsonInterFace.GSMSMSParameter.GSmsData = GSMSMSList[i].GSmsData;
                        JsonInterFace.GSMSMSParameter.GSmsRpoa = GSMSMSList[i].GSmsRpoa;
                        JsonInterFace.GSMSMSParameter.GSmsTpoa = GSMSMSList[i].GSmsTpoa;
                        JsonInterFace.GSMSMSParameter.GSmsScts = GSMSMSList[i].GSmsScts;
                        JsonInterFace.GSMSMSParameter.AutoSendtiny = GSMSMSList[i].AutoSendtiny;
                        JsonInterFace.GSMSMSParameter.AutoFilterSMStiny = GSMSMSList[i].AutoFilterSMStiny;
                        JsonInterFace.GSMSMSParameter.DelayTime = GSMSMSList[i].DelayTime;
                        JsonInterFace.GSMSMSParameter.SmsCodingtiny = GSMSMSList[i].SmsCodingtiny;
                        break;
                    }
                }
            }
        }

        private void btnGSMSMSDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确定删除该信息吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                try
                {
                    for (int i = 0; i < GSMSMSList.Count; i++)
                    {
                        if (
                        GSMSMSList[i].CarrierOne == ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).CarrierOne &&
                        GSMSMSList[i].CarrierTwo == ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).CarrierTwo &&
                        GSMSMSList[i].GSmsData == ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).GSmsData &&
                        GSMSMSList[i].GSmsRpoa == ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).GSmsRpoa &&
                        GSMSMSList[i].GSmsTpoa == ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).GSmsTpoa &&
                        GSMSMSList[i].GSmsScts == ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).GSmsScts &&
                        GSMSMSList[i].AutoSendtiny == ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).AutoSendtiny &&
                        GSMSMSList[i].AutoFilterSMStiny == ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).AutoFilterSMStiny &&
                        GSMSMSList[i].DelayTime == ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).DelayTime &&
                        GSMSMSList[i].SmsCodingtiny == ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).SmsCodingtiny
                        )
                        {
                            GSMSMSList.RemoveAt(i);
                            GSMSMSMessageDataGrid.ItemsSource = null;
                            GSMSMSMessageDataGrid.ItemsSource = GSMSMSList;
                            MessageBox.Show("删除信息成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("删除信息失败，" + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnGSMSMSSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!LongSMSError(txtGSMMessageContent.Text.Trim()))
                {
                    return;
                }

                for (int i = 0; i < GSMSMSList.Count; i++)
                {
                    if (
                        GSMSMSTmp.CarrierOne == GSMSMSList[i].CarrierOne &&
                        GSMSMSTmp.CarrierTwo == GSMSMSList[i].CarrierTwo &&
                        GSMSMSTmp.GSmsData == GSMSMSList[i].GSmsData &&
                        GSMSMSTmp.GSmsRpoa == GSMSMSList[i].GSmsRpoa &&
                        GSMSMSTmp.GSmsTpoa == GSMSMSList[i].GSmsTpoa &&
                        GSMSMSTmp.GSmsScts == GSMSMSList[i].GSmsScts &&
                        GSMSMSTmp.AutoSendtiny == GSMSMSList[i].AutoSendtiny &&
                        GSMSMSTmp.AutoFilterSMStiny == GSMSMSList[i].AutoFilterSMStiny &&
                        GSMSMSTmp.DelayTime == GSMSMSList[i].DelayTime &&
                        GSMSMSTmp.SmsCodingtiny == GSMSMSList[i].SmsCodingtiny
                    )
                    {
                        GSMSMSList[i].CarrierOne = JsonInterFace.GSMSMSParameter.CarrierOne;
                        GSMSMSList[i].CarrierTwo = JsonInterFace.GSMSMSParameter.CarrierTwo;
                        GSMSMSList[i].GSmsData = JsonInterFace.GSMSMSParameter.GSmsData;
                        GSMSMSList[i].GSmsRpoa = JsonInterFace.GSMSMSParameter.GSmsRpoa;
                        GSMSMSList[i].GSmsTpoa = JsonInterFace.GSMSMSParameter.GSmsTpoa;
                        GSMSMSList[i].GSmsScts = JsonInterFace.GSMSMSParameter.GSmsScts;
                        GSMSMSList[i].AutoSendtiny = JsonInterFace.GSMSMSParameter.AutoSendtiny;
                        GSMSMSList[i].AutoFilterSMStiny = JsonInterFace.GSMSMSParameter.AutoFilterSMStiny;
                        GSMSMSList[i].DelayTime = JsonInterFace.GSMSMSParameter.DelayTime;
                        GSMSMSList[i].SmsCodingtiny = JsonInterFace.GSMSMSParameter.SmsCodingtiny;

                        GSMSMSMessageDataGrid.ItemsSource = null;
                        GSMSMSMessageDataGrid.ItemsSource = GSMSMSList;
                        MessageBox.Show("短信息编辑成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);

                        GSMSMSTmp.CarrierOne = false;
                        GSMSMSTmp.CarrierTwo = false;
                        GSMSMSTmp.GSmsData = null;
                        GSMSMSTmp.GSmsRpoa = null;
                        GSMSMSTmp.GSmsTpoa = null;
                        GSMSMSTmp.GSmsScts = null;
                        GSMSMSTmp.AutoSendtiny = null;
                        GSMSMSTmp.AutoFilterSMStiny = null;
                        GSMSMSTmp.DelayTime = null;
                        GSMSMSTmp.SmsCodingtiny = null;

                        JsonInterFace.GSMSMSParameter.CarrierOne = false;
                        JsonInterFace.GSMSMSParameter.CarrierTwo = false;
                        JsonInterFace.GSMSMSParameter.GSmsData = null;
                        JsonInterFace.GSMSMSParameter.GSmsRpoa = null;
                        JsonInterFace.GSMSMSParameter.GSmsTpoa = null;
                        JsonInterFace.GSMSMSParameter.GSmsScts = null;
                        JsonInterFace.GSMSMSParameter.AutoSendtiny = null;
                        JsonInterFace.GSMSMSParameter.AutoFilterSMStiny = null;
                        JsonInterFace.GSMSMSParameter.DelayTime = null;
                        JsonInterFace.GSMSMSParameter.SmsCodingtiny = null;
                        break;
                    }
                }

                GSMSMSMessageDataGrid.IsEnabled = true;
                btnGSMSMSEditCancel.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show("短信息编辑失败，" + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// 短信息太长提示错误
        /// </summary>
        private bool LongSMSError(string messagecontent)
        {
            bool result = false;
            if (messagecontent.Length >= Parameters.GSMMaxSMSCount)
            {
                MessageBox.Show("短信息内容过长！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                result = false;
            }
            else
            {
                result = true;
            }
            return result;
        }

        private void chkGSMMessageCarrierOne_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)chkGSMMessageCarrierOne.IsChecked)
            {
                chkGSMMessageCarrierTwo.IsChecked = false;
            }
            else
            {
                if (!(bool)chkGSMMessageCarrierTwo.IsChecked)
                {
                    chkGSMMessageCarrierOne.IsChecked = true;
                }
            }
        }

        private void chkGSMMessageCarrierTwo_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)chkGSMMessageCarrierTwo.IsChecked)
            {
                chkGSMMessageCarrierOne.IsChecked = false;
            }
            else
            {
                if (!(bool)chkGSMMessageCarrierOne.IsChecked)
                {
                    chkGSMMessageCarrierTwo.IsChecked = true;
                }
            }
        }

        /// <summary>
        /// 显示GSM短信息列表
        /// </summary>
        private void GSMSMSListGetting()
        {
            new Thread(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    GSMSMSList.Clear();
                    for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                    {
                        if (JsonInterFace.APATTributesLists[i].FullName == (JsonInterFace.GSMDeviceParameter.DomainFullPathName + "." + JsonInterFace.GSMDeviceParameter.DeviceName))
                        {
                            lock (JsonInterFace.APATTributesLists[i].SMSRWLock)
                            {
                                if (JsonInterFace.APATTributesLists[i].GSMSMSParameterList.Count > 0)
                                {
                                    for (int j = 0; j < JsonInterFace.APATTributesLists[i].GSMSMSParameterList.Count; j++)
                                    {
                                        GSMSMSParameterClass SMS = new GSMSMSParameterClass();
                                        SMS.CarrierOne = JsonInterFace.APATTributesLists[i].GSMSMSParameterList[j].CarrierOne;
                                        SMS.CarrierTwo = JsonInterFace.APATTributesLists[i].GSMSMSParameterList[j].CarrierTwo;
                                        SMS.GSmsRpoa = JsonInterFace.APATTributesLists[i].GSMSMSParameterList[j].GSmsRpoa;
                                        SMS.GSmsTpoa = JsonInterFace.APATTributesLists[i].GSMSMSParameterList[j].GSmsTpoa;
                                        SMS.GSmsScts = JsonInterFace.APATTributesLists[i].GSMSMSParameterList[j].GSmsScts;
                                        SMS.GSmsData = JsonInterFace.APATTributesLists[i].GSMSMSParameterList[j].GSmsData;
                                        SMS.AutoSendtiny = JsonInterFace.APATTributesLists[i].GSMSMSParameterList[j].AutoSendtiny;
                                        SMS.AutoFilterSMStiny = JsonInterFace.APATTributesLists[i].GSMSMSParameterList[j].AutoFilterSMStiny;
                                        SMS.DelayTime = JsonInterFace.APATTributesLists[i].GSMSMSParameterList[j].DelayTime;
                                        SMS.SmsCodingtiny = JsonInterFace.APATTributesLists[i].GSMSMSParameterList[j].SmsCodingtiny;
                                        GSMSMSList.Add(SMS);
                                    }
                                    JsonInterFace.APATTributesLists[i].GSMSMSParameterList.Clear();
                                }
                            }
                            break;
                        }
                    }
                });
            }).Start();
        }

        /// <summary>
        /// 发送(历史短信)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGSMSMSSendWithLast_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (JsonInterFace.GSMDeviceParameter.OnLine == "0")
                {
                    MessageBox.Show("设备不在线！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (GSMSMSMessageDataGrid.SelectedItem == null)
                {
                    MessageBox.Show("请选择短信息！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                //获取数据
                JsonInterFace.GSMSMSParameter.CarrierOne = ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).CarrierOne;
                JsonInterFace.GSMSMSParameter.CarrierTwo = ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).CarrierTwo;
                JsonInterFace.GSMSMSParameter.GSmsData = ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).GSmsData;
                JsonInterFace.GSMSMSParameter.GSmsRpoa = ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).GSmsRpoa;
                JsonInterFace.GSMSMSParameter.GSmsTpoa = ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).GSmsTpoa;
                JsonInterFace.GSMSMSParameter.GSmsScts = ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).GSmsScts;
                JsonInterFace.GSMSMSParameter.AutoSendtiny = ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).AutoSendtiny;
                JsonInterFace.GSMSMSParameter.AutoFilterSMStiny = ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).AutoFilterSMStiny;
                JsonInterFace.GSMSMSParameter.DelayTime = ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).DelayTime;
                JsonInterFace.GSMSMSParameter.SmsCodingtiny = ((GSMSMSParameterClass)GSMSMSMessageDataGrid.SelectedItem).SmsCodingtiny;

                if (NetWorkClient.ControllerServer.Connected)
                {
                    //发送时间
                    JsonInterFace.GSMSMSParameter.GSmsScts = string.Format("{0:D4}", DateTime.Now.Year).Substring(2, 2) + string.Format("{0:D2}", DateTime.Now.Month) + string.Format("{0:D2}", DateTime.Now.Day) + string.Format("{0:D2}", DateTime.Now.Hour) + string.Format("{0:D2}", DateTime.Now.Minute) + string.Format("{0:D2}", DateTime.Now.Second);
                    JsonInterFace.ResultMessageList.Clear();
                    NetWorkClient.ControllerServer.Send(
                                                        JsonInterFace.GSMSMSSenderRequest
                                                                                        (
                                                                                            JsonInterFace.GSMDeviceParameter.DomainFullPathName,
                                                                                            JsonInterFace.GSMDeviceParameter.DeviceName,
                                                                                            JsonInterFace.GSMDeviceParameter.IpAddr,
                                                                                            JsonInterFace.GSMDeviceParameter.Port,
                                                                                            JsonInterFace.GSMDeviceParameter.InnerType,
                                                                                            JsonInterFace.GSMDeviceParameter.SN
                                                                                        )
                                                       );
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("信息发送失败！" + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// IMSI,IMEI批量导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGSMIMSIInput_Click(object sender, RoutedEventArgs e)
        {
            string IMSIFileName = string.Empty;
            string IMSI = string.Empty;
            string IMEI = string.Empty;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "(文本文件 *.txt)|*.txt";

            if (JsonInterFace.GSMDeviceParameter.OnLine == "0")
            {
                MessageBox.Show("该设备不在线操作无效！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!JsonInterFace.GSMLibyraryRegAdd.CarrierOne && !JsonInterFace.GSMLibyraryRegAdd.CarrierTwo)
            {
                MessageBox.Show("请选择该设备的载波！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if ((bool)openFileDialog.ShowDialog())
            {
                if ((new FileInfo(openFileDialog.FileName).Extension).ToLower() == ".txt")
                {
                    //读入数据
                    JsonInterFace.GSMLibyraryRegAdd.IMSIList.AddRange(File.ReadAllLines(openFileDialog.FileName));

                    if (JsonInterFace.GSMLibyraryRegAdd.IMSIList.Count <= 0)
                    {
                        MessageBox.Show("导入数据为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("导入文件只支持文本文件！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            new Thread(() =>
            {
                try
                {
                    JsonInterFace.GSMLibyraryRegAdd.IMSITotal = JsonInterFace.GSMLibyraryRegAdd.IMSIList.Count - 1;
                    JsonInterFace.GSMLibyraryRegAdd.BarStatus = Visibility.Visible;
                    JsonInterFace.GSMLibyraryRegAdd.Add = false;
                    JsonInterFace.GSMLibyraryRegAdd.Input = false;
                    JsonInterFace.GSMLibyraryRegAdd.ConfigType = "GSMIMSIMultiInput";
                    for (int i = 0; i < JsonInterFace.GSMLibyraryRegAdd.IMSIList.Count; i++)
                    {
                        if (new Regex(",").Match(JsonInterFace.GSMLibyraryRegAdd.IMSIList[i]).Success)
                        {
                            IMSI = JsonInterFace.GSMLibyraryRegAdd.IMSIList[i].Split(new char[] { ',' })[0].Trim();
                            IMEI = JsonInterFace.GSMLibyraryRegAdd.IMSIList[i].Split(new char[] { ',' })[1].Trim();
                            if (IMSI != "")
                            {
                                if (new Regex(@"\d{15}").Match(IMSI).Success)
                                {
                                    JsonInterFace.GSMLibyraryRegAdd.IMSI = IMSI;
                                    if (NetWorkClient.ControllerServer.Connected)
                                    {
                                        NetWorkClient.ControllerServer.Send(
                                                                            JsonInterFace.GSMLibraryRegAddIMSIRequest(
                                                                                                                        JsonInterFace.GSMDeviceParameter.DomainFullPathName,
                                                                                                                        JsonInterFace.GSMDeviceParameter.DeviceName,
                                                                                                                        JsonInterFace.GSMDeviceParameter.IpAddr,
                                                                                                                        JsonInterFace.GSMDeviceParameter.Port,
                                                                                                                        JsonInterFace.GSMDeviceParameter.InnerType,
                                                                                                                        JsonInterFace.GSMDeviceParameter.SN
                                                                                                                     )
                                                                            );
                                        JsonInterFace.GSMLibyraryRegAdd.Flag = 1;
                                        Thread.Sleep(500);
                                    }
                                }
                                else
                                {
                                    JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "IMSI:'" + JsonInterFace.GSMLibyraryRegAdd.IMSIList[i] + "' 长度不正确，IMSI应为15位数字组成的字符！", "GSM白名单导入", "格式不正确");
                                }
                            }
                            else
                            {
                                JsonInterFace.GSMLibyraryRegAdd.Flag = 1;
                            }

                            if (IMEI != "")
                            {
                                if (new Regex(@"\d{15}").Match(IMEI).Success)
                                {
                                    JsonInterFace.GSMLibyraryRegAdd.IMEI = IMEI;
                                    if (NetWorkClient.ControllerServer.Connected)
                                    {
                                        NetWorkClient.ControllerServer.Send(
                                                                            JsonInterFace.GSMLibraryRegAddIMEIRequest(
                                                                                                                      JsonInterFace.GSMDeviceParameter.DomainFullPathName,
                                                                                                                      JsonInterFace.GSMDeviceParameter.DeviceName,
                                                                                                                      JsonInterFace.GSMDeviceParameter.IpAddr,
                                                                                                                      JsonInterFace.GSMDeviceParameter.Port,
                                                                                                                      JsonInterFace.GSMDeviceParameter.InnerType,
                                                                                                                      JsonInterFace.GSMDeviceParameter.SN
                                                                                                                     )
                                                                           );
                                        JsonInterFace.GSMLibyraryRegAdd.Flag = 2;
                                        Thread.Sleep(500);
                                    }
                                }
                                else
                                {
                                    JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "IMEI:'" + JsonInterFace.GSMLibyraryRegAdd.IMSIList[i] + "' 长度不正确，IMEI应为15位数字组成的字符！", "GSM白名单导入", "格式不正确");
                                }
                            }
                            else
                            {
                                JsonInterFace.GSMLibyraryRegAdd.Flag = 2;
                            }
                            JsonInterFace.GSMLibyraryRegAdd.StepValue = i;
                        }
                        else
                        {
                            JsonInterFace.GSMLibyraryRegAdd.Flag = 0;
                            JsonInterFace.GSMLibyraryRegAdd.StepValue = i;
                            JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "'" + JsonInterFace.GSMLibyraryRegAdd.IMSIList[i] + "' 格式不正确，IMSI与IMEI之间以[',']逗号隔开！", "GSM白名单导入", "格式不正确");
                        }
                    }
                    JsonInterFace.GSMLibyraryRegAdd.IMSI = null;
                    JsonInterFace.GSMLibyraryRegAdd.IMEI = null;
                    JsonInterFace.GSMLibyraryRegAdd.Add = true;
                    JsonInterFace.GSMLibyraryRegAdd.Input = true;
                    JsonInterFace.GSMLibyraryRegAdd.StepValue = 0;
                    JsonInterFace.GSMLibyraryRegAdd.BarStatus = Visibility.Collapsed;
                    JsonInterFace.GSMLibyraryRegAdd.ConfigType = string.Empty;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("白名数据导入失败！" + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }).Start();
        }

        private void btnGSMSMSEditCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                JsonInterFace.GSMSMSParameter.CarrierOne = false;
                JsonInterFace.GSMSMSParameter.CarrierTwo = false;
                JsonInterFace.GSMSMSParameter.GSmsData = null;
                JsonInterFace.GSMSMSParameter.GSmsRpoa = null;
                JsonInterFace.GSMSMSParameter.GSmsTpoa = null;
                JsonInterFace.GSMSMSParameter.GSmsScts = null;
                JsonInterFace.GSMSMSParameter.AutoSendtiny = null;
                JsonInterFace.GSMSMSParameter.AutoFilterSMStiny = null;
                JsonInterFace.GSMSMSParameter.DelayTime = null;
                JsonInterFace.GSMSMSParameter.SmsCodingtiny = null;
                GSMSMSMessageDataGrid.IsEnabled = true;
                btnGSMSMSEditCancel.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show("取消失败," + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void rdbWCDMARebootModeAuto_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as CheckBox).IsChecked)
            {
                rdbWCDMARebootModeManul.IsChecked = false;
            }
            else
            {
                if (!(bool)(sender as CheckBox).IsChecked)
                {
                    rdbWCDMARebootModeAuto.IsChecked = true;
                }
            }
        }

        private void rdbWCDMARebootModeManul_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as CheckBox).IsChecked)
            {
                rdbWCDMARebootModeAuto.IsChecked = false;
            }
            else
            {
                if (!(bool)(sender as CheckBox).IsChecked)
                {
                    rdbWCDMARebootModeManul.IsChecked = true;
                }
            }
        }

        private void rbGPS_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)rbGPS.IsChecked)
            {
                rbYes.IsChecked = true;
                rbNo.IsEnabled = false;
                rbYes.IsEnabled = false;
            }
        }

        private void rbEmptyMouth_Click(object sender, RoutedEventArgs e)
        {
            rbNo.IsEnabled = true;
            rbYes.IsEnabled = true;
        }
        /// <summary>
        /// GSMV2短信记录显示
        /// </summary>
        private void GSMV2SMSConfigListShow()
        {
            while (true)
            {
                Thread.Sleep(2000);
                try
                {
                    Dispatcher.Invoke(() =>
                    {
                        for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                        {
                            if (JsonInterFace.APATTributesLists[i].FullName == (JsonInterFace.GSMV2DeviceParameter.DomainFullPathName + "." + JsonInterFace.GSMV2DeviceParameter.DeviceName) && JsonInterFace.APATTributesLists[i].Mode == DeviceType.GSMV2)
                            {
                                for (int j = 0; j < JsonInterFace.APATTributesLists[i].GSMV2ConfigSMSMSGDataTab.Rows.Count; j++)
                                {

                                    if (j == 0)
                                    {
                                        GSMV2SMSList.Clear();
                                    }
                                    GSMV2ConfigSMSMSGClass GSMV2SMSReport = new GSMV2ConfigSMSMSGClass();
                                    GSMV2SMSReport.MSGID = (j + 1).ToString();
                                    GSMV2SMSReport.BSMSOriginalNum = JsonInterFace.APATTributesLists[i].GSMV2ConfigSMSMSGDataTab.Rows[j]["BSMSOriginalNum"].ToString();
                                    GSMV2SMSReport.BSMSContent = JsonInterFace.APATTributesLists[i].GSMV2ConfigSMSMSGDataTab.Rows[j]["BSMSContent"].ToString();
                                    GSMV2SMSReport.CarrierOne = Convert.ToBoolean(JsonInterFace.APATTributesLists[i].GSMV2ConfigSMSMSGDataTab.Rows[j]["CarrierOne"].ToString());
                                    GSMV2SMSReport.CarrierTwo = Convert.ToBoolean(JsonInterFace.APATTributesLists[i].GSMV2ConfigSMSMSGDataTab.Rows[j]["CarrierTwo"].ToString());
                                    GSMV2SMSReport.SMSI = "";
                                    GSMV2SMSReport.SMSctrl = JsonInterFace.APATTributesLists[i].GSMV2ConfigSMSMSGDataTab.Rows[j]["SMSctrl"].ToString();
                                    GSMV2SMSList.Add(GSMV2SMSReport);

                                }
                                JsonInterFace.APATTributesLists[i].GSMV2ConfigSMSMSGDataTab.Clear();
                                break;
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended("短信格式获取异常", ex.Message, ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// GSMV2短信截获记录
        /// </summary>
        private void GSMV2SMSRecordListShow()
        {
            while (true)
            {
                bool bol = false;
                Thread.Sleep(1000);
                try
                {
                    for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                    {
                        if (JsonInterFace.APATTributesLists[i].FullName == (JsonInterFace.GSMV2DeviceParameter.DomainFullPathName + "." + JsonInterFace.GSMV2DeviceParameter.DeviceName) && JsonInterFace.APATTributesLists[i].Mode == DeviceType.GSMV2)
                        {
                            for (int j = 0; j < JsonInterFace.APATTributesLists[i].UEReportDataTab.Rows.Count; j++)
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    if (j == 0)
                                    {
                                        GSMV2SMSRecordList.Clear();
                                    }
                                    GSMV2ConfigSMSMSGClass GSMV2SMSRecord = new GSMV2ConfigSMSMSGClass();
                                    GSMV2SMSRecord.ReportID = (GSMV2SMSRecordList.Count + 1).ToString();
                                    GSMV2SMSRecord.BOrmType = JsonInterFace.APATTributesLists[i].UEReportDataTab.Rows[j]["BOrmType"].ToString();
                                    GSMV2SMSRecord.BUeId = JsonInterFace.APATTributesLists[i].UEReportDataTab.Rows[j]["BUeId"].ToString();
                                    GSMV2SMSRecord.CRSRP = JsonInterFace.APATTributesLists[i].UEReportDataTab.Rows[j]["CRSRP"].ToString();
                                    GSMV2SMSRecord.Carrier = JsonInterFace.APATTributesLists[i].UEReportDataTab.Rows[j]["Carrier"].ToString();
                                    GSMV2SMSRecord.BUeContent = JsonInterFace.APATTributesLists[i].UEReportDataTab.Rows[j]["BUeContent"].ToString();
                                    GSMV2SMSRecordList.Add(GSMV2SMSRecord);
                                });
                                bol = true;
                            }
                            break;
                        }
                    }
                    if (bol)
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended("短信记录获取异常", ex.Message, ex.StackTrace);
                }
            }
        }

        private void cbGSMV2SMSMSGCarrierOne_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)cbGSMV2SMSMSGCarrierOne.IsChecked)
            {
                cbGSMV2SMSMSGCarrierTwo.IsChecked = false;
            }
            else
            {
                if (!(bool)cbGSMV2SMSMSGCarrierTwo.IsChecked)
                {
                    cbGSMV2SMSMSGCarrierOne.IsChecked = true;
                }
            }
        }

        private void cbGSMV2SMSMSGCarrierTwo_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)cbGSMV2SMSMSGCarrierTwo.IsChecked)
            {
                cbGSMV2SMSMSGCarrierOne.IsChecked = false;
            }
            else
            {
                if (!(bool)cbGSMV2SMSMSGCarrierOne.IsChecked)
                {
                    cbGSMV2SMSMSGCarrierTwo.IsChecked = true;
                }
            }
        }

        private void tbGSMV2SMSMessage_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Parameters.SMSBrowse)
            {
                string GSMV2SMSData = ((TextBlock)sender).Text;
                for (int i = 0; i < GSMV2SMSList.Count; i++)
                {
                    if (GSMV2SMSData.Equals(GSMV2SMSList[i].GSmsData))
                    {
                        JsonInterFace.GSMV2ConfigSMSMSG.BSMSOriginalNum = GSMV2SMSList[i].BSMSOriginalNum;
                        JsonInterFace.GSMV2ConfigSMSMSG.BSMSContent = GSMV2SMSList[i].BSMSContent;
                        JsonInterFace.GSMV2ConfigSMSMSG.SMSI = GSMV2SMSList[i].SMSI;
                        JsonInterFace.GSMV2ConfigSMSMSG.CarrierOne = GSMV2SMSList[i].CarrierOne;
                        JsonInterFace.GSMV2ConfigSMSMSG.CarrierTwo = GSMV2SMSList[i].CarrierTwo;
                        break;
                    }
                }
            }
        }

        private void tbGSMV2SMSMessage_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Parameters.SMSBrowse)
            {
                JsonInterFace.GSMV2ConfigSMSMSG.BSMSOriginalNum = null;
                JsonInterFace.GSMV2ConfigSMSMSG.BSMSContent = null;
                JsonInterFace.GSMV2ConfigSMSMSG.SMSI = null;
                JsonInterFace.GSMV2ConfigSMSMSG.CarrierOne = false;
                JsonInterFace.GSMV2ConfigSMSMSG.CarrierTwo = false;
            }
        }

        private void btnGSMV2SMSEdit_Click(object sender, RoutedEventArgs e)
        {
            int SelectIndex = GSMV2SMSMessageDataGrid.SelectedIndex;
            if (MessageBox.Show("确定编辑该信息吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                try
                {
                    //编缉前缓存 GSMSMSTmp
                    GSMV2SMSTmp.BSMSOriginalNum = ((GSMV2ConfigSMSMSGClass)GSMV2SMSMessageDataGrid.SelectedItem).BSMSOriginalNum;
                    GSMV2SMSTmp.BSMSContent = ((GSMV2ConfigSMSMSGClass)GSMV2SMSMessageDataGrid.SelectedItem).BSMSContent;
                    GSMV2SMSTmp.CarrierOne = ((GSMV2ConfigSMSMSGClass)GSMV2SMSMessageDataGrid.SelectedItem).CarrierOne;
                    GSMV2SMSTmp.CarrierTwo = ((GSMV2ConfigSMSMSGClass)GSMV2SMSMessageDataGrid.SelectedItem).CarrierTwo;
                    GSMV2SMSTmp.SMSI = ((GSMV2ConfigSMSMSGClass)GSMV2SMSMessageDataGrid.SelectedItem).SMSI;

                    //编缉
                    JsonInterFace.GSMV2ConfigSMSMSG.BSMSOriginalNum = ((GSMV2ConfigSMSMSGClass)GSMV2SMSMessageDataGrid.SelectedItem).BSMSOriginalNum;
                    JsonInterFace.GSMV2ConfigSMSMSG.BSMSContent = ((GSMV2ConfigSMSMSGClass)GSMV2SMSMessageDataGrid.SelectedItem).BSMSContent;
                    JsonInterFace.GSMV2ConfigSMSMSG.CarrierOne = ((GSMV2ConfigSMSMSGClass)GSMV2SMSMessageDataGrid.SelectedItem).CarrierOne;
                    JsonInterFace.GSMV2ConfigSMSMSG.CarrierTwo = ((GSMV2ConfigSMSMSGClass)GSMV2SMSMessageDataGrid.SelectedItem).CarrierTwo;
                    JsonInterFace.GSMV2ConfigSMSMSG.SMSI = ((GSMV2ConfigSMSMSGClass)GSMV2SMSMessageDataGrid.SelectedItem).SMSI;

                    GSMV2SMSMessageDataGrid.IsEnabled = false;
                    btnGSMV2SMSEditCancel.Visibility = Visibility.Visible;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("编辑信息失败，" + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnGSMV2SMSDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确定删除该信息吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                try
                {
                    for (int i = 0; i < GSMV2SMSList.Count; i++)
                    {
                        if (
                            GSMV2SMSList[i].BSMSOriginalNum == ((GSMV2ConfigSMSMSGClass)GSMV2SMSMessageDataGrid.SelectedItem).BSMSOriginalNum &&
                            GSMV2SMSList[i].BSMSContent == ((GSMV2ConfigSMSMSGClass)GSMV2SMSMessageDataGrid.SelectedItem).BSMSContent &&
                            GSMV2SMSList[i].CarrierOne == ((GSMV2ConfigSMSMSGClass)GSMV2SMSMessageDataGrid.SelectedItem).CarrierOne &&
                            GSMV2SMSList[i].CarrierTwo == ((GSMV2ConfigSMSMSGClass)GSMV2SMSMessageDataGrid.SelectedItem).CarrierTwo
                        )
                        {
                            GSMV2SMSList.RemoveAt(i);
                            GSMV2SMSMessageDataGrid.ItemsSource = null;
                            GSMV2SMSMessageDataGrid.ItemsSource = GSMV2SMSList;
                            MessageBox.Show("删除信息成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("删除信息失败，" + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnGSMV2SMSSendWithLast_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String Carrier = string.Empty;
                if ((bool)cbGSMV2SMSCarrierOne.IsChecked)
                {
                    Carrier = "0";
                }
                else if ((bool)cbGSMV2SMSCarrierTwo.IsChecked)
                {
                    Carrier = "1";
                }
                else
                {
                    MessageBox.Show("没有选择载波！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (JsonInterFace.GSMV2DeviceParameter.OnLine == "0")
                {
                    MessageBox.Show("设备不在线！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (GSMV2SMSMessageDataGrid.SelectedItem == null)
                {
                    MessageBox.Show("请选择短信息！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                //获取数据
                JsonInterFace.GSMV2ConfigSMSMSG.BSMSOriginalNum = ((GSMV2ConfigSMSMSGClass)GSMV2SMSMessageDataGrid.SelectedItem).BSMSOriginalNum;
                JsonInterFace.GSMV2ConfigSMSMSG.BSMSContent = ((GSMV2ConfigSMSMSGClass)GSMV2SMSMessageDataGrid.SelectedItem).BSMSContent;
                JsonInterFace.GSMV2ConfigSMSMSG.SMSI = ((GSMV2ConfigSMSMSGClass)GSMV2SMSMessageDataGrid.SelectedItem).SMSI;
                JsonInterFace.GSMV2ConfigSMSMSG.SMSctrl = ((GSMV2ConfigSMSMSGClass)GSMV2SMSMessageDataGrid.SelectedItem).SMSctrl;

                if (JsonInterFace.GSMV2ConfigSMSMSG.BSMSOriginalNum != null && JsonInterFace.GSMV2ConfigSMSMSG.BSMSOriginalNum != "")
                {
                    if (new Regex(@"\+").Match(JsonInterFace.GSMV2ConfigSMSMSG.BSMSOriginalNum).Success)
                    {
                        string MNumber = JsonInterFace.GSMV2ConfigSMSMSG.BSMSOriginalNum.Substring(1, JsonInterFace.GSMV2ConfigSMSMSG.BSMSOriginalNum.Length - 1);
                        if (!Parameters.ISDigital(MNumber))
                        {
                            MessageBox.Show("主叫号码格式错误！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }
                    else if (!Parameters.ISDigital(JsonInterFace.GSMV2ConfigSMSMSG.BSMSOriginalNum))
                    {
                        MessageBox.Show("主叫号码格式错误！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("请输入主叫号码！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (JsonInterFace.GSMV2ConfigSMSMSG.SMSI != null && JsonInterFace.GSMV2ConfigSMSMSG.SMSI != "")
                {
                    if (!Parameters.ISDigital(JsonInterFace.GSMV2ConfigSMSMSG.SMSI))
                    {
                        MessageBox.Show("IMSI号格式错误！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("请输入IMSI号！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (NetWorkClient.ControllerServer.Connected)
                {
                    //发送IMSI
                    JsonInterFace.ResultMessageList.Clear();
                    List<string> SMSIList = new List<string>();
                    if (new Regex(@"\,").Match(JsonInterFace.GSMV2ConfigSMSMSG.SMSI).Success)
                    {
                        SMSIList.AddRange(JsonInterFace.GSMV2ConfigSMSMSG.SMSI.Split(new char[] { ',' }));
                    }
                    else
                    {
                        SMSIList.Add(JsonInterFace.GSMV2ConfigSMSMSG.SMSI);
                    }
                    string ActionType = "3";

                    new Thread(() =>
                    {
                        //发送IMSI
                        for (int i = 0; i < SMSIList.Count; i++)
                        {
                            NetWorkClient.ControllerServer.Send(
                                                                JsonInterFace.GSMV2SMSIMSISettingRequest
                                                                                                        (
                                                                                                            JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                                            JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                                            JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                                                                                            JsonInterFace.GSMV2DeviceParameter.Port,
                                                                                                            JsonInterFace.GSMV2DeviceParameter.InnerType,
                                                                                                            JsonInterFace.GSMV2DeviceParameter.SN,
                                                                                                            Carrier,
                                                                                                            ActionType,
                                                                                                            SMSIList[i]
                                                                                                        )
                                                               );
                        }

                        //发送短信
                        JsonInterFace.ResultMessageList.Clear();
                        NetWorkClient.ControllerServer.Send(
                                                        JsonInterFace.GSMV2SMSRequest
                                                                                        (
                                                                                            JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                            JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                            JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                                                                            JsonInterFace.GSMV2DeviceParameter.Port,
                                                                                            JsonInterFace.GSMV2DeviceParameter.InnerType,
                                                                                            JsonInterFace.GSMV2DeviceParameter.SN,
                                                                                            JsonInterFace.GSMV2ConfigSMSMSG.SMSctrl.ToString(),
                                                                                            JsonInterFace.GSMV2ConfigSMSMSG.BSMSOriginalNum,
                                                                                            JsonInterFace.GSMV2ConfigSMSMSG.BSMSContent,
                                                                                            Carrier
                                                                                        )
                                                       );
                    }).Start();
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("信息发送失败！" + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void txtGSMV2MessageContent_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtGSMV2MessageContent.Text.Trim().Length <= 0)
            {
                lblGSMV2ContentSize.Content = txtGSMV2MessageContent.Text.Trim().Length.ToString() + "/" + Parameters.GSMV2MaxSMSCount.ToString();
                btnGSMV2SMSSave.IsEnabled = false;
                btnGSMV2SMSSend.IsEnabled = false;
            }
            else
            {
                if (txtGSMV2MessageContent.Text.Trim().Length >= Parameters.GSMV2MaxSMSCount)
                {
                    lblGSMV2ContentSize.Content = txtGSMV2MessageContent.Text.Trim().Length.ToString() + "/" + Parameters.GSMV2MaxSMSCount.ToString();
                    lblGSMV2ContentSize.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Yellow"));
                    lblGSMV2ContentSize.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Red"));
                }
                else
                {
                    lblGSMV2ContentSize.Content = txtGSMV2MessageContent.Text.Trim().Length.ToString() + "/" + Parameters.GSMV2MaxSMSCount.ToString();
                    lblGSMV2ContentSize.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
                    lblGSMV2ContentSize.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("AliceBlue"));
                }
                btnGSMV2SMSSave.IsEnabled = true;
                btnGSMV2SMSSend.IsEnabled = true;
            }
        }

        private void btnGSMV2SMSEditCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                JsonInterFace.GSMV2ConfigSMSMSG.CarrierOne = false;
                JsonInterFace.GSMV2ConfigSMSMSG.CarrierTwo = false;
                JsonInterFace.GSMV2ConfigSMSMSG.BSMSOriginalNum = null;
                JsonInterFace.GSMV2ConfigSMSMSG.BSMSContent = null;
                JsonInterFace.GSMV2ConfigSMSMSG.SMSI = null;
                GSMV2SMSMessageDataGrid.IsEnabled = true;
                btnGSMV2SMSEditCancel.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show("取消失败," + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnGSMV2SMSSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!LongSMSError(txtGSMV2MessageContent.Text.Trim()))
                {
                    return;
                }

                if (JsonInterFace.GSMV2ConfigSMSMSG.BSMSOriginalNum != null && JsonInterFace.GSMV2ConfigSMSMSG.BSMSOriginalNum != "")
                {
                    if (new Regex(@"\+").Match(JsonInterFace.GSMV2ConfigSMSMSG.BSMSOriginalNum).Success)
                    {
                        string MNumber = JsonInterFace.GSMV2ConfigSMSMSG.BSMSOriginalNum.Substring(1, JsonInterFace.GSMV2ConfigSMSMSG.BSMSOriginalNum.Length - 1);
                        if (!Parameters.ISDigital(MNumber))
                        {
                            MessageBox.Show("主叫号码格式错误！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }
                    else if (!Parameters.ISDigital(JsonInterFace.GSMV2ConfigSMSMSG.BSMSOriginalNum))
                    {
                        MessageBox.Show("主叫号码格式错误！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("请输入主叫号码！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (JsonInterFace.GSMV2ConfigSMSMSG.SMSI != null && JsonInterFace.GSMV2ConfigSMSMSG.SMSI != "")
                {
                    if (!Parameters.ISDigital(JsonInterFace.GSMV2ConfigSMSMSG.SMSI))
                    {
                        MessageBox.Show("IMSI号格式错误！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("请输入IMSI号！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                for (int i = 0; i < GSMV2SMSList.Count; i++)
                {
                    if (
                        GSMV2SMSTmp.CarrierOne == GSMV2SMSList[i].CarrierOne &&
                        GSMV2SMSTmp.CarrierTwo == GSMV2SMSList[i].CarrierTwo &&
                        GSMV2SMSTmp.BSMSOriginalNum == GSMV2SMSList[i].BSMSOriginalNum &&
                        GSMV2SMSTmp.BSMSContent == GSMV2SMSList[i].BSMSContent
                    )
                    {
                        GSMV2SMSList[i].CarrierOne = JsonInterFace.GSMV2ConfigSMSMSG.CarrierOne;
                        GSMV2SMSList[i].CarrierTwo = JsonInterFace.GSMV2ConfigSMSMSG.CarrierTwo;
                        GSMV2SMSList[i].BSMSOriginalNum = JsonInterFace.GSMV2ConfigSMSMSG.BSMSOriginalNum;
                        GSMV2SMSList[i].BSMSContent = JsonInterFace.GSMV2ConfigSMSMSG.BSMSContent;
                        GSMV2SMSList[i].SMSI = JsonInterFace.GSMV2ConfigSMSMSG.SMSI;

                        GSMV2SMSMessageDataGrid.ItemsSource = null;
                        GSMV2SMSMessageDataGrid.ItemsSource = GSMV2SMSList;
                        MessageBox.Show("短信息编辑成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);

                        GSMV2SMSTmp.CarrierOne = false;
                        GSMV2SMSTmp.CarrierTwo = false;
                        GSMV2SMSTmp.BSMSOriginalNum = null;
                        GSMV2SMSTmp.BSMSContent = null;
                        GSMV2SMSTmp.SMSI = null;

                        JsonInterFace.GSMV2ConfigSMSMSG.CarrierOne = false;
                        JsonInterFace.GSMV2ConfigSMSMSG.CarrierTwo = false;
                        JsonInterFace.GSMV2ConfigSMSMSG.BSMSOriginalNum = null;
                        JsonInterFace.GSMV2ConfigSMSMSG.BSMSContent = null;
                        JsonInterFace.GSMV2ConfigSMSMSG.SMSI = null;
                        break;
                    }
                }

                GSMV2SMSMessageDataGrid.IsEnabled = true;
                btnGSMV2SMSEditCancel.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show("短信息编辑失败，" + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnGSMV2SMSSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string Carrier = string.Empty;
                if (JsonInterFace.GSMV2DeviceParameter.OnLine == "0")
                {
                    MessageBox.Show("设备不在线！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!LongSMSError(txtGSMV2MessageContent.Text.Trim()))
                {
                    return;
                }

                if (JsonInterFace.GSMV2ConfigSMSMSG.BSMSContent == "" || JsonInterFace.GSMV2ConfigSMSMSG.BSMSContent == null)
                {
                    MessageBox.Show("短信息内容为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (JsonInterFace.GSMV2ConfigSMSMSG.BSMSOriginalNum != null && JsonInterFace.GSMV2ConfigSMSMSG.BSMSOriginalNum != "")
                {
                    if (new Regex(@"\+").Match(JsonInterFace.GSMV2ConfigSMSMSG.BSMSOriginalNum).Success)
                    {
                        string MNumber = JsonInterFace.GSMV2ConfigSMSMSG.BSMSOriginalNum.Substring(1, JsonInterFace.GSMV2ConfigSMSMSG.BSMSOriginalNum.Length - 1);
                        if (!Parameters.ISDigital(MNumber))
                        {
                            MessageBox.Show("主叫号码格式错误！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }
                    else if (!Parameters.ISDigital(JsonInterFace.GSMV2ConfigSMSMSG.BSMSOriginalNum))
                    {
                        MessageBox.Show("主叫号码格式错误！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("请输入主叫号码！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                //发送imis不可以为空
                if (JsonInterFace.GSMV2ConfigSMSMSG.SMSI != null && JsonInterFace.GSMV2ConfigSMSMSG.SMSI != "")
                {
                    if (JsonInterFace.GSMV2ConfigSMSMSG.SMSI.Length < 15)
                    {
                        MessageBox.Show("IMSI号格式错误！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("请输入IMSI号！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                //发送短信不区分载波
                Carrier = "0";
                if (NetWorkClient.ControllerServer.Connected)
                {
                    //发送IMSI
                    JsonInterFace.ResultMessageList.Clear();

                    new Thread(() =>
                    {
                        //发送信息
                        JsonInterFace.ResultMessageList.Clear();
                        NetWorkClient.ControllerServer.Send(
                                                        JsonInterFace.GSMV2SMSRequest
                                                                                        (
                                                                                            JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                            JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                            JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                                                                            JsonInterFace.GSMV2DeviceParameter.Port,
                                                                                            JsonInterFace.GSMV2DeviceParameter.InnerType,
                                                                                            JsonInterFace.GSMV2DeviceParameter.SN,
                                                                                            "0",
                                                                                            JsonInterFace.GSMV2ConfigSMSMSG.BSMSOriginalNum,
                                                                                            JsonInterFace.GSMV2ConfigSMSMSG.BSMSContent,
                                                                                            Carrier
                                                                                        )
                                                       );

                        //发送IMSI  不自动发送短信时需要指定IMSI号
                        List<string> SMSIList = new List<string>();

                        string ActionType = "3";
                        if (new Regex(@"\,").Match(JsonInterFace.GSMV2ConfigSMSMSG.SMSI).Success)
                        {
                            SMSIList.AddRange(JsonInterFace.GSMV2ConfigSMSMSG.SMSI.Split(new char[] { ',' }));
                        }
                        else
                        {
                            SMSIList.Add(JsonInterFace.GSMV2ConfigSMSMSG.SMSI);
                        }
                        for (int i = 0; i < SMSIList.Count; i++)
                        {
                            NetWorkClient.ControllerServer.Send(
                                                                JsonInterFace.GSMV2SMSIMSISettingRequest
                                                                                                        (
                                                                                                            JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                                            JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                                            JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                                                                                            JsonInterFace.GSMV2DeviceParameter.Port,
                                                                                                            JsonInterFace.GSMV2DeviceParameter.InnerType,
                                                                                                            JsonInterFace.GSMV2DeviceParameter.SN,
                                                                                                            Carrier,
                                                                                                            ActionType,
                                                                                                            SMSIList[i]
                                                                                                        )
                                                               );
                        }
                    }).Start();
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("短信息发送失败！" + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        /// <summary>
        /// CDMA短信记录显示
        /// </summary>
        private void CDMASMSConfigListShow()
        {
            while (true)
            {
                Thread.Sleep(2000);
                try
                {
                    for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                    {
                        if (JsonInterFace.APATTributesLists[i].FullName == (JsonInterFace.CDMADeviceParameter.DomainFullPathName + "." + JsonInterFace.CDMADeviceParameter.DeviceName) && JsonInterFace.APATTributesLists[i].Mode == DeviceType.CDMA)
                        {
                            lock (JsonInterFace.GSMV2ConfigSMSMSG.SMSLock)
                            {
                                for (int j = 0; j < JsonInterFace.APATTributesLists[i].GSMV2ConfigSMSMSGDataTab.Rows.Count; j++)
                                {
                                    Dispatcher.Invoke(() =>
                                    {
                                        if (j == 0)
                                        {
                                            CDMASMSList.Clear();
                                        }
                                        CDMAConfigSMSMSGClass CDMASMSReport = new CDMAConfigSMSMSGClass();
                                        CDMASMSReport.MSGID = (j + 1).ToString();
                                        CDMASMSReport.BSMSOriginalNum = JsonInterFace.APATTributesLists[i].GSMV2ConfigSMSMSGDataTab.Rows[j]["BSMSOriginalNum"].ToString();
                                        CDMASMSReport.BSMSContent = JsonInterFace.APATTributesLists[i].GSMV2ConfigSMSMSGDataTab.Rows[j]["BSMSContent"].ToString();
                                        CDMASMSReport.SMSI = "";
                                        CDMASMSReport.SMSctrl = JsonInterFace.APATTributesLists[i].GSMV2ConfigSMSMSGDataTab.Rows[j]["SMSctrl"].ToString();
                                        CDMASMSList.Add(CDMASMSReport);
                                    });
                                }
                                JsonInterFace.APATTributesLists[i].GSMV2ConfigSMSMSGDataTab.Clear();
                            }
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended("短信格式获取异常", ex.Message, ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// CDMA短信截获
        /// </summary>
        private void CDMASMSRecordListShow()
        {
            while (true)
            {
                bool bol = false;
                Thread.Sleep(1000);
                try
                {
                    for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                    {
                        if (JsonInterFace.APATTributesLists[i].FullName == (JsonInterFace.CDMADeviceParameter.DomainFullPathName + "." + JsonInterFace.CDMADeviceParameter.DeviceName) && JsonInterFace.APATTributesLists[i].Mode == DeviceType.CDMA)
                        {

                            for (int j = 0; j < JsonInterFace.APATTributesLists[i].UEReportDataTab.Rows.Count; j++)
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    if (j == 0)
                                    {
                                        CDMASMSRecordList.Clear();
                                    }
                                    CDMAConfigSMSMSGClass CDMASMSRecord = new CDMAConfigSMSMSGClass();
                                    CDMASMSRecord.ReportID = (CDMASMSRecordList.Count + 1).ToString();
                                    CDMASMSRecord.BOrmType = JsonInterFace.APATTributesLists[i].UEReportDataTab.Rows[j]["BOrmType"].ToString();
                                    CDMASMSRecord.BUeId = JsonInterFace.APATTributesLists[i].UEReportDataTab.Rows[j]["BUeId"].ToString();
                                    CDMASMSRecord.CRSRP = JsonInterFace.APATTributesLists[i].UEReportDataTab.Rows[j]["CRSRP"].ToString();
                                    CDMASMSRecord.Carrier = JsonInterFace.APATTributesLists[i].UEReportDataTab.Rows[j]["Carrier"].ToString();
                                    CDMASMSRecord.BUeContent = JsonInterFace.APATTributesLists[i].UEReportDataTab.Rows[j]["BUeContent"].ToString();
                                    CDMASMSRecordList.Add(CDMASMSRecord);
                                });
                                bol = true;
                            }
                            break;
                        }
                    }
                    if (bol)
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended("短信记录获取异常", ex.Message, ex.StackTrace);
                }
            }
        }

        private void btnCDMASMSEdit_Click(object sender, RoutedEventArgs e)
        {
            int SelectIndex = CDMASMSMessageDataGrid.SelectedIndex;
            if (MessageBox.Show("确定编辑该信息吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                try
                {
                    //编缉前缓存 GSMSMSTmp
                    CDMASMSTmp.BSMSOriginalNum = ((CDMAConfigSMSMSGClass)CDMASMSMessageDataGrid.SelectedItem).BSMSOriginalNum;
                    CDMASMSTmp.BSMSContent = ((CDMAConfigSMSMSGClass)CDMASMSMessageDataGrid.SelectedItem).BSMSContent;
                    CDMASMSTmp.SMSI = ((CDMAConfigSMSMSGClass)CDMASMSMessageDataGrid.SelectedItem).SMSI;

                    //编缉
                    JsonInterFace.CDMAConfigSMSMSG.BSMSOriginalNum = ((CDMAConfigSMSMSGClass)CDMASMSMessageDataGrid.SelectedItem).BSMSOriginalNum;
                    JsonInterFace.CDMAConfigSMSMSG.BSMSContent = ((CDMAConfigSMSMSGClass)CDMASMSMessageDataGrid.SelectedItem).BSMSContent;
                    JsonInterFace.CDMAConfigSMSMSG.SMSI = ((CDMAConfigSMSMSGClass)CDMASMSMessageDataGrid.SelectedItem).SMSI;

                    CDMASMSMessageDataGrid.IsEnabled = false;
                    btnCDMASMSEditCancel.Visibility = Visibility.Visible;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("编辑信息失败，" + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnCDMASMSDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确定删除该信息吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                try
                {
                    for (int i = 0; i < CDMASMSList.Count; i++)
                    {
                        if (
                            CDMASMSList[i].BSMSOriginalNum == ((CDMAConfigSMSMSGClass)CDMASMSMessageDataGrid.SelectedItem).BSMSOriginalNum &&
                            CDMASMSList[i].BSMSContent == ((CDMAConfigSMSMSGClass)CDMASMSMessageDataGrid.SelectedItem).BSMSContent
                        )
                        {
                            CDMASMSList.RemoveAt(i);
                            CDMASMSMessageDataGrid.ItemsSource = null;
                            CDMASMSMessageDataGrid.ItemsSource = CDMASMSList;
                            MessageBox.Show("删除信息成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("删除信息失败，" + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnCDMASMSSendWithLast_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String Carrier = "0";
                if (JsonInterFace.CDMADeviceParameter.OnLine == "0")
                {
                    MessageBox.Show("设备不在线！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (CDMASMSMessageDataGrid.SelectedItem == null)
                {
                    MessageBox.Show("请选择短信息！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                //获取数据
                JsonInterFace.CDMAConfigSMSMSG.BSMSOriginalNum = ((CDMAConfigSMSMSGClass)CDMASMSMessageDataGrid.SelectedItem).BSMSOriginalNum;
                JsonInterFace.CDMAConfigSMSMSG.BSMSContent = ((CDMAConfigSMSMSGClass)CDMASMSMessageDataGrid.SelectedItem).BSMSContent;
                JsonInterFace.CDMAConfigSMSMSG.SMSI = ((CDMAConfigSMSMSGClass)CDMASMSMessageDataGrid.SelectedItem).SMSI;
                JsonInterFace.CDMAConfigSMSMSG.SMSctrl = ((CDMAConfigSMSMSGClass)CDMASMSMessageDataGrid.SelectedItem).SMSctrl;

                if (JsonInterFace.CDMAConfigSMSMSG.BSMSContent == null || JsonInterFace.CDMAConfigSMSMSG.BSMSContent == "")
                {
                    MessageBox.Show("短信内容不存在！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (JsonInterFace.CDMAConfigSMSMSG.BSMSOriginalNum != null && JsonInterFace.CDMAConfigSMSMSG.BSMSOriginalNum != "")
                {
                    if (new Regex(@"\+").Match(JsonInterFace.CDMAConfigSMSMSG.BSMSOriginalNum).Success)
                    {
                        string MNumber = JsonInterFace.CDMAConfigSMSMSG.BSMSOriginalNum.Substring(1, JsonInterFace.CDMAConfigSMSMSG.BSMSOriginalNum.Length - 1);
                        if (!Parameters.ISDigital(MNumber))
                        {
                            MessageBox.Show("主叫号码格式错误！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }
                    else if (!Parameters.ISDigital(JsonInterFace.CDMAConfigSMSMSG.BSMSOriginalNum))
                    {
                        MessageBox.Show("主叫号码格式错误！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("主叫号码不存在！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (JsonInterFace.CDMAConfigSMSMSG.SMSI != null && JsonInterFace.CDMAConfigSMSMSG.SMSI != "")
                {
                    if (!Parameters.ISDigital(JsonInterFace.CDMAConfigSMSMSG.SMSI))
                    {
                        MessageBox.Show("IMSI号格式错误！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("IMSI号不存在！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (NetWorkClient.ControllerServer.Connected)
                {
                    new Thread(() =>
                    {
                        JsonInterFace.ResultMessageList.Clear();
                        List<string> SMSIList = new List<string>();
                        if (new Regex(@"\,").Match(JsonInterFace.CDMAConfigSMSMSG.SMSI).Success)
                        {
                            SMSIList.AddRange(JsonInterFace.CDMAConfigSMSMSG.SMSI.Split(new char[] { ',' }));
                        }
                        else
                        {
                            SMSIList.Add(JsonInterFace.CDMAConfigSMSMSG.SMSI);
                        }
                        string ActionType = "3";

                        //发送IMSI
                        for (int i = 0; i < SMSIList.Count; i++)
                        {
                            NetWorkClient.ControllerServer.Send(
                                                                JsonInterFace.GSMV2SMSIMSISettingRequest
                                                                                                        (
                                                                                                            JsonInterFace.CDMADeviceParameter.DomainFullPathName,
                                                                                                            JsonInterFace.CDMADeviceParameter.DeviceName,
                                                                                                            JsonInterFace.CDMADeviceParameter.IpAddr,
                                                                                                            JsonInterFace.CDMADeviceParameter.Port,
                                                                                                            JsonInterFace.CDMADeviceParameter.InnerType,
                                                                                                            JsonInterFace.CDMADeviceParameter.SN,
                                                                                                            Carrier,
                                                                                                            ActionType,
                                                                                                            SMSIList[i]
                                                                                                        )
                                                               );
                        }

                        //发送短信
                        JsonInterFace.ResultMessageList.Clear();
                        NetWorkClient.ControllerServer.Send(
                                                            JsonInterFace.GSMV2SMSRequest
                                                                                        (
                                                                                            JsonInterFace.CDMADeviceParameter.DomainFullPathName,
                                                                                            JsonInterFace.CDMADeviceParameter.DeviceName,
                                                                                            JsonInterFace.CDMADeviceParameter.IpAddr,
                                                                                            JsonInterFace.CDMADeviceParameter.Port,
                                                                                            JsonInterFace.CDMADeviceParameter.InnerType,
                                                                                            JsonInterFace.CDMADeviceParameter.SN,
                                                                                            JsonInterFace.CDMAConfigSMSMSG.SMSctrl.ToString(),
                                                                                            JsonInterFace.CDMAConfigSMSMSG.BSMSOriginalNum,
                                                                                            JsonInterFace.CDMAConfigSMSMSG.BSMSContent,
                                                                                            Carrier
                                                                                        )
                                                           );
                    }).Start();
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("信息发送失败！" + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnCDMASMSEditCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                JsonInterFace.CDMAConfigSMSMSG.BSMSOriginalNum = null;
                JsonInterFace.CDMAConfigSMSMSG.BSMSContent = null;
                JsonInterFace.CDMAConfigSMSMSG.SMSI = null;
                CDMASMSMessageDataGrid.IsEnabled = true;
                btnCDMASMSEditCancel.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show("取消失败," + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void txtCDMAMessageContent_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtCDMAMessageContent.Text.Trim().Length <= 0)
            {
                lblCDMAContentSize.Content = txtCDMAMessageContent.Text.Trim().Length.ToString() + "/" + Parameters.CDMAMaxSMSCount.ToString();
                btnCDMASMSSave.IsEnabled = false;
                btnCDMASMSSend.IsEnabled = false;
            }
            else
            {
                if (txtCDMAMessageContent.Text.Trim().Length >= Parameters.CDMAMaxSMSCount)
                {
                    lblCDMAContentSize.Content = txtCDMAMessageContent.Text.Trim().Length.ToString() + "/" + Parameters.CDMAMaxSMSCount.ToString();
                    lblCDMAContentSize.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Yellow"));
                    lblCDMAContentSize.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Red"));
                }
                else
                {
                    lblCDMAContentSize.Content = txtCDMAMessageContent.Text.Trim().Length.ToString() + "/" + Parameters.CDMAMaxSMSCount.ToString();
                    lblCDMAContentSize.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
                    lblCDMAContentSize.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("AliceBlue"));
                }
                btnCDMASMSSave.IsEnabled = true;
                btnCDMASMSSend.IsEnabled = true;
            }
        }

        private void btnCDMASMSSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!LongSMSError(txtCDMAMessageContent.Text.Trim()))
                {
                    return;
                }

                if (JsonInterFace.CDMAConfigSMSMSG.BSMSOriginalNum != null && JsonInterFace.CDMAConfigSMSMSG.BSMSOriginalNum != "")
                {
                    if (new Regex(@"\+").Match(JsonInterFace.CDMAConfigSMSMSG.BSMSOriginalNum).Success)
                    {
                        string MNumber = JsonInterFace.CDMAConfigSMSMSG.BSMSOriginalNum.Substring(1, JsonInterFace.CDMAConfigSMSMSG.BSMSOriginalNum.Length - 1);
                        if (!Parameters.ISDigital(MNumber))
                        {
                            MessageBox.Show("主叫号码格式错误！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }
                    else if (!Parameters.ISDigital(JsonInterFace.CDMAConfigSMSMSG.BSMSOriginalNum))
                    {
                        MessageBox.Show("主叫号码格式错误！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("请输入主叫号码！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (JsonInterFace.CDMAConfigSMSMSG.SMSI != null && JsonInterFace.CDMAConfigSMSMSG.SMSI != "")
                {
                    if (!Parameters.ISDigital(JsonInterFace.CDMAConfigSMSMSG.SMSI))
                    {
                        MessageBox.Show("IMSI号格式错误！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("请输入IMSI号！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                for (int i = 0; i < CDMASMSList.Count; i++)
                {
                    if (
                        CDMASMSTmp.BSMSOriginalNum == CDMASMSList[i].BSMSOriginalNum &&
                        CDMASMSTmp.BSMSContent == CDMASMSList[i].BSMSContent
                    )
                    {
                        CDMASMSList[i].BSMSOriginalNum = JsonInterFace.CDMAConfigSMSMSG.BSMSOriginalNum;
                        CDMASMSList[i].BSMSContent = JsonInterFace.CDMAConfigSMSMSG.BSMSContent;
                        CDMASMSList[i].SMSI = JsonInterFace.CDMAConfigSMSMSG.SMSI;

                        CDMASMSMessageDataGrid.ItemsSource = null;
                        CDMASMSMessageDataGrid.ItemsSource = CDMASMSList;
                        MessageBox.Show("短信息编辑成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);

                        CDMASMSTmp.BSMSOriginalNum = null;
                        CDMASMSTmp.BSMSContent = null;
                        CDMASMSTmp.SMSI = null;

                        JsonInterFace.CDMAConfigSMSMSG.BSMSOriginalNum = null;
                        JsonInterFace.CDMAConfigSMSMSG.BSMSContent = null;
                        JsonInterFace.CDMAConfigSMSMSG.SMSI = null;
                        break;
                    }
                }

                CDMASMSMessageDataGrid.IsEnabled = true;
                btnCDMASMSEditCancel.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show("短信息编辑失败，" + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnCDMASMSSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string Carrier = "0";
                if (JsonInterFace.CDMADeviceParameter.OnLine == "0")
                {
                    MessageBox.Show("设备不在线！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!LongSMSError(txtCDMAMessageContent.Text.Trim()))
                {
                    return;
                }

                if (JsonInterFace.CDMAConfigSMSMSG.BSMSContent == "" || JsonInterFace.CDMAConfigSMSMSG.BSMSContent == null)
                {
                    MessageBox.Show("短信息内容为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (JsonInterFace.CDMAConfigSMSMSG.BSMSOriginalNum != null && JsonInterFace.CDMAConfigSMSMSG.BSMSOriginalNum != "")
                {
                    if (new Regex(@"\+").Match(JsonInterFace.CDMAConfigSMSMSG.BSMSOriginalNum).Success)
                    {
                        string MNumber = JsonInterFace.CDMAConfigSMSMSG.BSMSOriginalNum.Substring(1, JsonInterFace.CDMAConfigSMSMSG.BSMSOriginalNum.Length - 1);
                        if (!Parameters.ISDigital(MNumber))
                        {
                            MessageBox.Show("主叫号码格式错误！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                    else if (!Parameters.ISDigital(JsonInterFace.CDMAConfigSMSMSG.BSMSOriginalNum))
                    {
                        MessageBox.Show("主叫号码格式错误！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("请输入主叫号码！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                //是否自动发送
                if (JsonInterFace.CDMAConfigSMSMSG.SMSI != null && JsonInterFace.CDMAConfigSMSMSG.SMSI != "")
                {
                    if (!Parameters.ISDigital(JsonInterFace.CDMAConfigSMSMSG.SMSI))
                    {
                        MessageBox.Show("IMSI号格式错误！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("请输入IMSI号！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (NetWorkClient.ControllerServer.Connected)
                {
                    JsonInterFace.ResultMessageList.Clear();

                    new Thread(() =>
                    {
                        //发送短信
                        JsonInterFace.ResultMessageList.Clear();
                        NetWorkClient.ControllerServer.Send(
                                                            JsonInterFace.GSMV2SMSRequest
                                                                                        (
                                                                                            JsonInterFace.CDMADeviceParameter.DomainFullPathName,
                                                                                            JsonInterFace.CDMADeviceParameter.DeviceName,
                                                                                            JsonInterFace.CDMADeviceParameter.IpAddr,
                                                                                            JsonInterFace.CDMADeviceParameter.Port,
                                                                                            JsonInterFace.CDMADeviceParameter.InnerType,
                                                                                            JsonInterFace.CDMADeviceParameter.SN,
                                                                                            "0",
                                                                                            JsonInterFace.CDMAConfigSMSMSG.BSMSOriginalNum,
                                                                                            JsonInterFace.CDMAConfigSMSMSG.BSMSContent,
                                                                                            Carrier
                                                                                        )
                                                           );

                        //发送IMSI 不自动发送短信需要指定imsi
                        List<string> SMSIList = new List<string>();
                        if (new Regex(@"\,").Match(JsonInterFace.CDMAConfigSMSMSG.SMSI).Success)
                        {
                            SMSIList.AddRange(JsonInterFace.CDMAConfigSMSMSG.SMSI.Split(new char[] { ',' }));
                        }
                        else
                        {
                            SMSIList.Add(JsonInterFace.CDMAConfigSMSMSG.SMSI);
                        }
                        string ActionType = "3";

                        for (int i = 0; i < SMSIList.Count; i++)
                        {
                            NetWorkClient.ControllerServer.Send(
                                                                JsonInterFace.GSMV2SMSIMSISettingRequest
                                                                                                        (
                                                                                                            JsonInterFace.CDMADeviceParameter.DomainFullPathName,
                                                                                                            JsonInterFace.CDMADeviceParameter.DeviceName,
                                                                                                            JsonInterFace.CDMADeviceParameter.IpAddr,
                                                                                                            JsonInterFace.CDMADeviceParameter.Port,
                                                                                                            JsonInterFace.CDMADeviceParameter.InnerType,
                                                                                                            JsonInterFace.CDMADeviceParameter.SN,
                                                                                                            Carrier,
                                                                                                            ActionType,
                                                                                                            SMSIList[i]
                                                                                                        )
                                                               );
                        }
                    }).Start();
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("短信息发送失败！" + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnGSMV2SelectCallSMS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!((bool)cbGSMV2SMSMSGCarrierOne.IsChecked || (bool)cbGSMV2SMSMSGCarrierTwo.IsChecked))
                {
                    MessageBox.Show("请选择需要查询的载波！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (cbbGSMV2CallorSMS.SelectedIndex < 0)
                {
                    MessageBox.Show("请选择需要查询的类型！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                string Carrier = (bool)cbGSMV2SMSMSGCarrierOne.IsChecked ? "0" : "1";
                string imsi = txtGSMV2CallSMSIMSI.Text.Trim();
                string phoneNumber = txtGSMV2CallSMSPhone.Text.Trim();
                string starTime = Convert.ToDateTime(dpkcmdStartTime.Text).ToString("yyyy-MM-dd 00:00:00");
                string endTime = Convert.ToDateTime(dpkcmdEndTime.Text).ToString("yyyy-MM-dd 00:00:00");
                if (NetWorkClient.ControllerServer.Connected)
                {
                    if (cbbGSMV2CallorSMS.SelectedIndex == 0)
                    {
                        NetWorkClient.ControllerServer.Send(JsonInterFace.GSMV2CellPARAMRequest(
                                                                                                JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                                JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                                JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                                                                                JsonInterFace.GSMV2DeviceParameter.Port,
                                                                                                JsonInterFace.GSMV2DeviceParameter.InnerType,
                                                                                                JsonInterFace.GSMV2DeviceParameter.SN,
                                                                                                ""
                                                                                                )
                                                                                              );
                    }
                    else
                    {

                    }
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("通话、短信获取记录异常" + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnCDMAStartRF_Click(object sender, RoutedEventArgs e)
        {
            if (NetWorkClient.ControllerServer.Connected)
            {
                //请求的载波
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

        private void btnCDMACloseRF_Click(object sender, RoutedEventArgs e)
        {
            if (NetWorkClient.ControllerServer.Connected)
            {
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

        private void btnCDMAreStartRF_Click(object sender, RoutedEventArgs e)
        {
            if (NetWorkClient.ControllerServer.Connected)
            {
                NetWorkClient.ControllerServer.Send(JsonInterFace.GSMV2ReStartRequest(
                                                                                JsonInterFace.CDMADeviceParameter.DomainFullPathName,
                                                                                JsonInterFace.CDMADeviceParameter.DeviceName,
                                                                                JsonInterFace.CDMADeviceParameter.IpAddr,
                                                                                JsonInterFace.CDMADeviceParameter.Port,
                                                                                JsonInterFace.CDMADeviceParameter.InnerType,
                                                                                JsonInterFace.CDMADeviceParameter.SN,
                                                                                "0"
                                                                              ));
            }
            else
            {
                MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            }
        }

        private void btnWCDMAOtherPlmnSetting_Click(object sender, RoutedEventArgs e)
        {
            //多PLMN设置
            if (WCDMAMorePLMNList.Count > 0)
            {
                string PLMNList = string.Empty;
                for (int i = 0; i < WCDMAMorePLMNList.Count; i++)
                {
                    if (WCDMAMorePLMNList[i].PLMNS != "" || WCDMAMorePLMNList[i].PLMNS != null)
                    {
                        if (PLMNList == "")
                        {
                            PLMNList = WCDMAMorePLMNList[i].PLMNS;
                        }
                        else
                        {
                            PLMNList += "," + WCDMAMorePLMNList[i].PLMNS;
                        }
                    }
                }

                if (PLMNList == "" || PLMNList == null)
                {
                    MessageBox.Show("多PLMN参数为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                JsonInterFace.ResultMessageList.Clear();
                if (NetWorkClient.ControllerServer.Connected)
                {
                    Parameters.ConfigType = "OtherPLMN";
                    NetWorkClient.ControllerServer.Send(JsonInterFace.APSettingOhterPLMNRequest(
                                                                                                JsonInterFace.WCDMACellNeighParameter.DomainFullPathName,
                                                                                                JsonInterFace.WCDMACellNeighParameter.DeviceName,
                                                                                                JsonInterFace.WCDMACellNeighParameter.IpAddr,
                                                                                                JsonInterFace.WCDMACellNeighParameter.Port,
                                                                                                JsonInterFace.WCDMACellNeighParameter.InnerType,
                                                                                                JsonInterFace.WCDMACellNeighParameter.SN,
                                                                                                PLMNList
                                                                                                ));
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("多PLMN参数为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnWCDMAPeriodFreqSetting_Click(object sender, RoutedEventArgs e)
        {
            //周期频点设置
            if (WCDMAPeriorFreqList.Count > 0)
            {
                string PeriorFreq = string.Empty;

                if (JsonInterFace.WCDMACellNeighParameter.Cycle == "" || JsonInterFace.WCDMACellNeighParameter.Cycle == null)
                {
                    MessageBox.Show("请输入周期时间(单位：秒)！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else if (!new Regex(@"\d").Match(JsonInterFace.WCDMACellNeighParameter.Cycle).Success)
                {
                    MessageBox.Show("周期时间[" + JsonInterFace.WCDMACellNeighParameter.Cycle + "]格式不正确,范围[0~65535] (单位：秒)！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else if (Convert.ToInt32(JsonInterFace.WCDMACellNeighParameter.Cycle) < 0 || Convert.ToInt32(JsonInterFace.WCDMACellNeighParameter.Cycle) > 65535)
                {
                    MessageBox.Show("周期时间[" + JsonInterFace.WCDMACellNeighParameter.Cycle + "]已超出范围[0~65535] (单位：秒)！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                for (int i = 0; i < WCDMAPeriorFreqList.Count; i++)
                {
                    if (WCDMAPeriorFreqList[i].PerierFreq != "" && WCDMAPeriorFreqList[i].PerierFreq != null)
                    {
                        if (PeriorFreq == "")
                        {
                            PeriorFreq = WCDMAPeriorFreqList[i].PerierFreq;
                        }
                        else
                        {
                            PeriorFreq += "," + WCDMAPeriorFreqList[i].PerierFreq;
                        }
                    }
                }

                if (PeriorFreq == null || PeriorFreq == "")
                {
                    MessageBox.Show("频点参数为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                PeriorFreq = JsonInterFace.WCDMACellNeighParameter.Cycle + ":" + PeriorFreq;
                JsonInterFace.ResultMessageList.Clear();
                if (NetWorkClient.ControllerServer.Connected)
                {
                    Parameters.ConfigType = "PeriodFreq";
                    NetWorkClient.ControllerServer.Send(JsonInterFace.APSettingPeriodFreqRequest(
                                                                                                JsonInterFace.WCDMACellNeighParameter.DomainFullPathName,
                                                                                                JsonInterFace.WCDMACellNeighParameter.DeviceName,
                                                                                                JsonInterFace.WCDMACellNeighParameter.IpAddr,
                                                                                                JsonInterFace.WCDMACellNeighParameter.Port,
                                                                                                JsonInterFace.WCDMACellNeighParameter.InnerType,
                                                                                                JsonInterFace.WCDMACellNeighParameter.SN,
                                                                                                PeriorFreq
                                                                                                ));
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("频点参数为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void miWCDMAFreqListClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WCDMAPeriorFreqList.Clear();
                MessageBox.Show("频点清空成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("频点清空失败！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void miWCDMAMultiPLMNListClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WCDMAMorePLMNList.Clear();
                MessageBox.Show("多PLMN清空成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("多PLMN清空失败！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void getParameterKeyValue(TextBox txtValue, ComboBox cbValue, int index)
        {
            switch (index)
            {
                case 0:
                    txtValue.ToolTip = "格式[IP:PORT]";
                    txtValue.IsReadOnly = false;
                    ParameterKeyName = "CFG_DEFAULT_CONTROLLER";
                    break;
                case 1:
                    txtValue.ToolTip = "GPS工作模式";
                    txtValue.IsReadOnly = false;
                    ParameterKeyName = "CFG_GPS_MODE";
                    cbValue.Items.Add("GPS server");
                    cbValue.Items.Add("GPS client");
                    break;
                case 2:
                    txtValue.ToolTip = "IMSI等消息上报控制端";
                    txtValue.IsReadOnly = false;
                    ParameterKeyName = "CFG_CONTROLLER_REPORT_DISABLE";
                    cbValue.Items.Add("开启UDP和TCP控制端");
                    cbValue.Items.Add("仅开启TCP控制端");
                    cbValue.Items.Add("仅开启UDP控制端");
                    cbValue.Items.Add("关闭UDP和TCP控制端");
                    break;
                case 3:
                    txtValue.ToolTip = "等效PLMN配置";
                    txtValue.IsReadOnly = false;
                    ParameterKeyName = "CFG_OTHER_PLMN";
                    break;
                case 4:
                    txtValue.ToolTip = "周期变频格式[time：freq1,freq2,freq3]";
                    txtValue.IsReadOnly = false;
                    ParameterKeyName = "CFG_PERIOD_FREQ";
                    break;
                //case 5:
                //    txtValue.ToolTip = "格式[IP:PORT:MAXMSG]";
                //    txtValue.IsReadOnly = false;
                //    ParameterKeyName = "CFG_TCP_CONTROLLER";
                //    break;
                //case 6:
                //    txtValue.ToolTip = "设置全名";
                //    txtValue.IsReadOnly = false;
                //    ParameterKeyName = "CFG_FULL_NAME";
                //    break;
                //case 7:
                //    txtValue.ToolTip = "选择名单过滤模式";
                //    txtValue.IsReadOnly = true;
                //    ParameterKeyName = "CFG_REPORT_FILTER";
                //    cbValue.Items.Add("都过滤");
                //    cbValue.Items.Add("过滤其他名单");
                //    cbValue.Items.Add("过滤黑名单");
                //    cbValue.Items.Add("过滤黑名单和其他名单");
                //    cbValue.Items.Add("过滤白名单");
                //    cbValue.Items.Add("过滤白名单和其他名单");
                //    cbValue.Items.Add("过滤白名单和黑名单");
                //    cbValue.Items.Add("都不过滤");
                //    break;
                //case 8:
                //    txtValue.ToolTip = "文件格式";
                //    txtValue.IsReadOnly = true;
                //    ParameterKeyName = "CFG_INTERFACE_MODE";
                //    cbValue.Items.Add("xml mode");
                //    cbValue.Items.Add("json mode");
                //    cbValue.Items.Add("Hybrid mode");
                //    break;
                //case 9:
                //    txtValue.ToolTip = "0表示use scanner msg;1表示use scanner-zip msg";
                //    txtValue.IsReadOnly = true;
                //    ParameterKeyName = "CFG_SCANNER_ZIP";
                //    cbValue.Items.Add("use scanner msg");
                //    cbValue.Items.Add("use scanner-zip msg");
                //    break;
                case -1:
                    txtValue.ToolTip = "自定义";
                    txtValue.IsReadOnly = true;
                    ParameterKeyName = "";
                    break;
                default:
                    break;
            }
        }
        private void cbParameterKeyName_DropDownClosed(object sender, EventArgs e)
        {
            if (cbParameterKeyName.SelectedIndex > -1)
            {
                cbParameterValue.Items.Clear();
                cbParameterValue.Text = "";
                txtParameterValue.Text = "";
                getParameterKeyValue(txtParameterValue, cbParameterValue, cbParameterKeyName.SelectedIndex);
            }
        }

        private void cbGSMParameterKeyName_DropDownClosed(object sender, EventArgs e)
        {
            if (cbGSMParameterKeyName.SelectedIndex > -1)
            {
                cbGSMParameterValue.Items.Clear();
                cbGSMParameterValue.Text = "";
                txtGSMParameterValue.Text = "";
                getParameterKeyValue(txtGSMParameterValue, cbGSMParameterValue, cbGSMParameterKeyName.SelectedIndex);
            }
        }

        private void cbWCDMAParameterKeyName_DropDownClosed(object sender, EventArgs e)
        {
            if (cbWCDMAParameterKeyName.SelectedIndex > -1)
            {
                cbWCDMAParameterValue.Items.Clear();
                cbWCDMAParameterValue.Text = "";
                txtWCDMAParameterValue.Text = "";
                getParameterKeyValue(txtWCDMAParameterValue, cbWCDMAParameterValue, cbWCDMAParameterKeyName.SelectedIndex);
            }
        }

        private void cbCDMAParameterKeyName_DropDownClosed(object sender, EventArgs e)
        {
            if (cbCDMAParameterKeyName.SelectedIndex > -1)
            {
                cbCDMAParameterValue.Items.Clear();
                cbCDMAParameterValue.Text = "";
                txtCDMAParameterValue.Text = "";
                getParameterKeyValue(txtCDMAParameterValue, cbCDMAParameterValue, cbCDMAParameterKeyName.SelectedIndex);
            }
        }

        private void cbGSMV2ParameterKeyName_DropDownClosed(object sender, EventArgs e)
        {
            if (cbGSMV2ParameterKeyName.SelectedIndex > -1)
            {
                cbGSMV2ParameterValue.Items.Clear();
                cbGSMV2ParameterValue.Text = "";
                txtGSMV2ParameterValue.Text = "";
                getParameterKeyValue(txtGSMV2ParameterValue, cbGSMV2ParameterValue, cbGSMV2ParameterKeyName.SelectedIndex);
            }
        }

        private void btnGSMUpdatePragram_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbGSMParameterKeyName.Text != "" && cbGSMParameterKeyName.Text != null
                    && txtGSMParameterValue.Text != "" && txtGSMParameterValue.Text != null)
                {
                    string ParameterValue;
                    if (cbGSMParameterKeyName.SelectedIndex == 1 && cbGSMParameterValue.Items.Count > 0)
                    {
                        ParameterValue = (cbGSMParameterValue.Items.IndexOf(txtGSMParameterValue.Text) + 1).ToString();
                    }
                    else if (cbGSMParameterValue.Items.Count > 0)
                    {
                        ParameterValue = cbGSMParameterValue.Items.IndexOf(txtGSMParameterValue.Text).ToString();
                    }
                    else
                    {
                        ParameterValue = txtGSMParameterValue.Text;
                    }
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        Parameters.ConfigType = "ProjectSetting";
                        NetWorkClient.ControllerServer.Send(JsonInterFace.APProjectSingleParameterSettingRequest(
                                                                                                    JsonInterFace.GSMDeviceParameter.DomainFullPathName,
                                                                                                    JsonInterFace.GSMDeviceParameter.DeviceName,
                                                                                                    JsonInterFace.GSMDeviceParameter.IpAddr,
                                                                                                    JsonInterFace.GSMDeviceParameter.Port,
                                                                                                    JsonInterFace.GSMDeviceParameter.InnerType,
                                                                                                    JsonInterFace.GSMDeviceParameter.SN,
                                                                                                    ParameterKeyName,
                                                                                                    ParameterValue
                                                                                                ));
                    }
                    else
                    {
                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("参数内容为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("单项参数设置，" + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnWCDMAUpdatePragram_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbWCDMAParameterKeyName.Text != "" && cbWCDMAParameterKeyName.Text != null
                    && txtWCDMAParameterValue.Text != "" && txtWCDMAParameterValue.Text != null)
                {
                    string ParameterValue;
                    if (cbWCDMAParameterKeyName.SelectedIndex == 1 && cbWCDMAParameterValue.Items.Count > 0)
                    {
                        ParameterValue = (cbWCDMAParameterValue.Items.IndexOf(txtWCDMAParameterValue.Text) + 1).ToString();
                    }
                    else if (cbWCDMAParameterValue.Items.Count > 0)
                    {
                        ParameterValue = cbWCDMAParameterValue.Items.IndexOf(txtWCDMAParameterValue.Text).ToString();
                    }
                    else
                    {
                        ParameterValue = txtWCDMAParameterValue.Text;
                    }
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        Parameters.ConfigType = "ProjectSetting";
                        NetWorkClient.ControllerServer.Send(JsonInterFace.APProjectSingleParameterSettingRequest(
                                                                                                    JsonInterFace.WCDMADeviceParameter.DomainFullPathName,
                                                                                                    JsonInterFace.WCDMADeviceParameter.DeviceName,
                                                                                                    JsonInterFace.WCDMADeviceParameter.IpAddr,
                                                                                                    JsonInterFace.WCDMADeviceParameter.Port,
                                                                                                    JsonInterFace.WCDMADeviceParameter.InnerType,
                                                                                                    JsonInterFace.WCDMADeviceParameter.SN,
                                                                                                    ParameterKeyName,
                                                                                                    ParameterValue
                                                                                                ));
                    }
                    else
                    {
                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("参数内容为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("单项参数设置，" + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCDMAUpdatePragram_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbCDMAParameterKeyName.Text != "" && cbCDMAParameterKeyName.Text != null
                    && txtCDMAParameterValue.Text != "" && txtCDMAParameterValue.Text != null)
                {
                    string ParameterValue;
                    if (cbCDMAParameterKeyName.SelectedIndex == 1 && cbCDMAParameterValue.Items.Count > 0)
                    {
                        ParameterValue = (cbCDMAParameterValue.Items.IndexOf(txtCDMAParameterValue.Text) + 1).ToString();
                    }
                    else if (cbCDMAParameterValue.Items.Count > 0)
                    {
                        ParameterValue = cbCDMAParameterValue.Items.IndexOf(txtCDMAParameterValue.Text).ToString();
                    }
                    else
                    {
                        ParameterValue = txtCDMAParameterValue.Text;
                    }
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        Parameters.ConfigType = "ProjectSetting";
                        NetWorkClient.ControllerServer.Send(JsonInterFace.APProjectSingleParameterSettingRequest(
                                                                                                    JsonInterFace.CDMADeviceParameter.DomainFullPathName,
                                                                                                    JsonInterFace.CDMADeviceParameter.DeviceName,
                                                                                                    JsonInterFace.CDMADeviceParameter.IpAddr,
                                                                                                    JsonInterFace.CDMADeviceParameter.Port,
                                                                                                    JsonInterFace.CDMADeviceParameter.InnerType,
                                                                                                    JsonInterFace.CDMADeviceParameter.SN,
                                                                                                    ParameterKeyName,
                                                                                                    ParameterValue
                                                                                                ));
                    }
                    else
                    {
                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("参数内容为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("单项参数设置，" + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnGSMV2UpdatePragram_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbGSMV2ParameterKeyName.Text != "" && cbGSMV2ParameterKeyName.Text != null
                    && txtGSMV2ParameterValue.Text != "" && txtGSMV2ParameterValue.Text != null)
                {
                    string ParameterValue;
                    if (cbGSMV2ParameterKeyName.SelectedIndex == 1 && cbGSMV2ParameterValue.Items.Count > 0)
                    {
                        ParameterValue = (cbGSMV2ParameterValue.Items.IndexOf(txtGSMV2ParameterValue.Text) + 1).ToString();
                    }
                    else if (cbGSMV2ParameterValue.Items.Count > 0)
                    {
                        ParameterValue = cbGSMV2ParameterValue.Items.IndexOf(txtGSMV2ParameterValue.Text).ToString();
                    }
                    else
                    {
                        ParameterValue = txtGSMV2ParameterValue.Text;
                    }
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        Parameters.ConfigType = "ProjectSetting";
                        NetWorkClient.ControllerServer.Send(JsonInterFace.APProjectSingleParameterSettingRequest(
                                                                                                    JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.Port,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.InnerType,
                                                                                                    JsonInterFace.GSMV2DeviceParameter.SN,
                                                                                                    ParameterKeyName,
                                                                                                    ParameterValue
                                                                                                ));
                    }
                    else
                    {
                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("参数内容为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("单项参数设置，" + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnLTESelectUpLoad_Click(object sender, RoutedEventArgs e)
        {
            if (!JsonInterFace.LteDeviceParameter.OnLine.Equals("1"))
            {
                MessageBox.Show("设备不在线！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (NetWorkClient.ControllerServer.Connected)
            {
                Parameters.ConfigType = "SelectUpload";
                NetWorkClient.ControllerServer.Send(JsonInterFace.Get_Upload_Request(
                                                                                            JsonInterFace.LteDeviceParameter.DomainFullPathName,
                                                                                            JsonInterFace.LteDeviceParameter.DeviceName,
                                                                                            JsonInterFace.LteDeviceParameter.IpAddr,
                                                                                            JsonInterFace.LteDeviceParameter.Port,
                                                                                            JsonInterFace.LteDeviceParameter.InnerType,
                                                                                            JsonInterFace.LteDeviceParameter.SN
                                                                                        ));
            }
            else
            {
                MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void btnGSMSelectUpLoad_Click(object sender, RoutedEventArgs e)
        {
            if (!JsonInterFace.GSMDeviceParameter.OnLine.Equals("1"))
            {
                MessageBox.Show("设备不在线！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (NetWorkClient.ControllerServer.Connected)
            {
                Parameters.ConfigType = "SelectUpload";
                NetWorkClient.ControllerServer.Send(JsonInterFace.Get_Upload_Request(
                                                                                            JsonInterFace.GSMDeviceParameter.DomainFullPathName,
                                                                                            JsonInterFace.GSMDeviceParameter.DeviceName,
                                                                                            JsonInterFace.GSMDeviceParameter.IpAddr,
                                                                                            JsonInterFace.GSMDeviceParameter.Port,
                                                                                            JsonInterFace.GSMDeviceParameter.InnerType,
                                                                                            JsonInterFace.GSMDeviceParameter.SN
                                                                                        ));
            }
            else
            {
                MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void btnWCDMASelectUpLoad_Click(object sender, RoutedEventArgs e)
        {
            if (!JsonInterFace.WCDMADeviceParameter.OnLine.Equals("1"))
            {
                MessageBox.Show("设备不在线！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (NetWorkClient.ControllerServer.Connected)
            {
                Parameters.ConfigType = "SelectUpload";
                NetWorkClient.ControllerServer.Send(JsonInterFace.Get_Upload_Request(
                                                                                            JsonInterFace.WCDMADeviceParameter.DomainFullPathName,
                                                                                            JsonInterFace.WCDMADeviceParameter.DeviceName,
                                                                                            JsonInterFace.WCDMADeviceParameter.IpAddr,
                                                                                            JsonInterFace.WCDMADeviceParameter.Port,
                                                                                            JsonInterFace.WCDMADeviceParameter.InnerType,
                                                                                            JsonInterFace.WCDMADeviceParameter.SN
                                                                                        ));
            }
            else
            {
                MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void btnCDMASelectUpLoad_Click(object sender, RoutedEventArgs e)
        {
            if (!JsonInterFace.CDMADeviceParameter.OnLine.Equals("1"))
            {
                MessageBox.Show("设备不在线！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (NetWorkClient.ControllerServer.Connected)
            {
                Parameters.ConfigType = "SelectUpload";
                NetWorkClient.ControllerServer.Send(JsonInterFace.Get_Upload_Request(
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
                MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void btnGSMV2SelectUpLoad_Click(object sender, RoutedEventArgs e)
        {
            if (!JsonInterFace.GSMV2DeviceParameter.OnLine.Equals("1"))
            {
                MessageBox.Show("设备不在线！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (NetWorkClient.ControllerServer.Connected)
            {
                Parameters.ConfigType = "SelectUpload";
                NetWorkClient.ControllerServer.Send(JsonInterFace.Get_Upload_Request(
                                                                                            JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                            JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                            JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                                                                            JsonInterFace.GSMV2DeviceParameter.Port,
                                                                                            JsonInterFace.GSMV2DeviceParameter.InnerType,
                                                                                            JsonInterFace.GSMV2DeviceParameter.SN
                                                                                        ));
            }
            else
            {
                MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void btnLTESetUpLoad_Click(object sender, RoutedEventArgs e)
        {
            if (!JsonInterFace.LteDeviceParameter.OnLine.Equals("1"))
            {
                MessageBox.Show("设备不在线！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (NetWorkClient.ControllerServer.Connected)
            {
                NetWorkClient.ControllerServer.Send(JsonInterFace.Set_Upload_Request(
                                                                                            JsonInterFace.LteDeviceParameter.DomainFullPathName,
                                                                                            JsonInterFace.LteDeviceParameter.DeviceName,
                                                                                            JsonInterFace.LteDeviceParameter.IpAddr,
                                                                                            JsonInterFace.LteDeviceParameter.Port,
                                                                                            JsonInterFace.LteDeviceParameter.InnerType,
                                                                                            JsonInterFace.LteDeviceParameter.SN
                                                                                        ));
            }
            else
            {
                MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void btnGSMSetUpLoad_Click(object sender, RoutedEventArgs e)
        {
            if (!JsonInterFace.GSMDeviceParameter.OnLine.Equals("1"))
            {
                MessageBox.Show("设备不在线！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (NetWorkClient.ControllerServer.Connected)
            {
                NetWorkClient.ControllerServer.Send(JsonInterFace.Set_Upload_Request(
                                                                                            JsonInterFace.GSMDeviceParameter.DomainFullPathName,
                                                                                            JsonInterFace.GSMDeviceParameter.DeviceName,
                                                                                            JsonInterFace.GSMDeviceParameter.IpAddr,
                                                                                            JsonInterFace.GSMDeviceParameter.Port,
                                                                                            JsonInterFace.GSMDeviceParameter.InnerType,
                                                                                            JsonInterFace.GSMDeviceParameter.SN
                                                                                        ));
            }
            else
            {
                MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void btnWCDMASetUpLoad_Click(object sender, RoutedEventArgs e)
        {
            if (!JsonInterFace.WCDMADeviceParameter.OnLine.Equals("1"))
            {
                MessageBox.Show("设备不在线！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (NetWorkClient.ControllerServer.Connected)
            {
                NetWorkClient.ControllerServer.Send(JsonInterFace.Set_Upload_Request(
                                                                                            JsonInterFace.WCDMADeviceParameter.DomainFullPathName,
                                                                                            JsonInterFace.WCDMADeviceParameter.DeviceName,
                                                                                            JsonInterFace.WCDMADeviceParameter.IpAddr,
                                                                                            JsonInterFace.WCDMADeviceParameter.Port,
                                                                                            JsonInterFace.WCDMADeviceParameter.InnerType,
                                                                                            JsonInterFace.WCDMADeviceParameter.SN
                                                                                        ));
            }
            else
            {
                MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void btnCDMASetUpLoad_Click(object sender, RoutedEventArgs e)
        {
            if (!JsonInterFace.CDMADeviceParameter.OnLine.Equals("1"))
            {
                MessageBox.Show("设备不在线！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (NetWorkClient.ControllerServer.Connected)
            {
                NetWorkClient.ControllerServer.Send(JsonInterFace.Set_Upload_Request(
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
                MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void btnGSMV2SetUpLoad_Click(object sender, RoutedEventArgs e)
        {
            if (!JsonInterFace.GSMV2DeviceParameter.OnLine.Equals("1"))
            {
                MessageBox.Show("设备不在线！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (NetWorkClient.ControllerServer.Connected)
            {
                NetWorkClient.ControllerServer.Send(JsonInterFace.Set_Upload_Request(
                                                                                        JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                        JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                        JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                                                                        JsonInterFace.GSMV2DeviceParameter.Port,
                                                                                        JsonInterFace.GSMV2DeviceParameter.InnerType,
                                                                                        JsonInterFace.GSMV2DeviceParameter.SN
                                                                                    ));
            }
            else
            {
                MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnCDMASMSIBrowser_Click(object sender, RoutedEventArgs e)
        {
            SubWindow.GSMV2SMSIMSIListWindow CDMASMSIMSIListWin = new SubWindow.GSMV2SMSIMSIListWindow();
            CDMASMSIMSIListWin.SelfDeviceName = JsonInterFace.CDMADeviceParameter.DomainFullPathName + "." + JsonInterFace.CDMADeviceParameter.DeviceName;
            CDMASMSIMSIListWin.ShowDialog();
            txtCDMASMSI.Text = SelfIMSIList;
        }

        private void btnGSMV2SMSIBrowser_Click(object sender, RoutedEventArgs e)
        {
            SubWindow.GSMV2SMSIMSIListWindow GSMV2SMSIMSIListWin = new SubWindow.GSMV2SMSIMSIListWindow();
            GSMV2SMSIMSIListWin.SelfDeviceName = JsonInterFace.GSMV2DeviceParameter.DomainFullPathName + "." + JsonInterFace.GSMV2DeviceParameter.DeviceName;
            GSMV2SMSIMSIListWin.ShowDialog();
            txtGSMV2SMSI.Text = SelfIMSIList;
        }

        private void cbParameterValue_DropDownClosed(object sender, EventArgs e)
        {
            if (cbParameterValue.SelectedIndex > -1)
            {
                txtParameterValue.Text = cbParameterValue.SelectedItem.ToString();
                cbParameterValue.Text = "";
            }
        }

        private void cbGSMParameterValue_DropDownClosed(object sender, EventArgs e)
        {
            if (cbGSMParameterValue.SelectedIndex > -1)
            {
                txtGSMParameterValue.Text = cbGSMParameterValue.SelectedItem.ToString();
                cbGSMParameterValue.Text = "";
            }

        }

        private void cbWCDMAParameterValue_DropDownClosed(object sender, EventArgs e)
        {
            if (cbWCDMAParameterValue.SelectedIndex > -1)
            {
                txtWCDMAParameterValue.Text = cbWCDMAParameterValue.SelectedItem.ToString();
                cbWCDMAParameterValue.Text = "";
            }
        }

        private void cbCDMAParameterValue_DropDownClosed(object sender, EventArgs e)
        {
            if (cbCDMAParameterValue.SelectedIndex > -1)
            {
                txtCDMAParameterValue.Text = cbCDMAParameterValue.SelectedItem.ToString();
                cbCDMAParameterValue.Text = "";
            }
        }

        private void cbGSMV2ParameterValue_DropDownClosed(object sender, EventArgs e)
        {
            if (cbGSMV2ParameterValue.SelectedIndex > -1)
            {
                txtGSMV2ParameterValue.Text = cbGSMV2ParameterValue.SelectedItem.ToString();
                cbGSMV2ParameterValue.Text = "";
            }
        }

        private void btnLTESYNCInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!JsonInterFace.LteDeviceParameter.OnLine.Equals("1"))
                {
                    MessageBox.Show("设备不在线！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (NetWorkClient.ControllerServer.Connected)
                {
                    NetWorkClient.ControllerServer.Send(JsonInterFace.Get_SYNC_Info_Request(
                                                                                                JsonInterFace.LteDeviceParameter.DomainFullPathName,
                                                                                                JsonInterFace.LteDeviceParameter.DeviceName,
                                                                                                JsonInterFace.LteDeviceParameter.IpAddr,
                                                                                                JsonInterFace.LteDeviceParameter.Port,
                                                                                                JsonInterFace.LteDeviceParameter.InnerType,
                                                                                                JsonInterFace.LteDeviceParameter.SN
                                                                                            ));
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("同步状态信息查询", ex.Message, ex.StackTrace);
            }
        }

        private void btnWCDMASYNCInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!JsonInterFace.WCDMADeviceParameter.OnLine.Equals("1"))
                {
                    MessageBox.Show("设备不在线！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (NetWorkClient.ControllerServer.Connected)
                {
                    NetWorkClient.ControllerServer.Send(JsonInterFace.Get_SYNC_Info_Request(
                                                                                                JsonInterFace.WCDMADeviceParameter.DomainFullPathName,
                                                                                                JsonInterFace.WCDMADeviceParameter.DeviceName,
                                                                                                JsonInterFace.WCDMADeviceParameter.IpAddr,
                                                                                                JsonInterFace.WCDMADeviceParameter.Port,
                                                                                                JsonInterFace.WCDMADeviceParameter.InnerType,
                                                                                                JsonInterFace.WCDMADeviceParameter.SN
                                                                                            ));
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("同步状态信息查询", ex.Message, ex.StackTrace);
            }
        }

        private void btnCDMASYNCInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!JsonInterFace.CDMADeviceParameter.OnLine.Equals("1"))
                {
                    MessageBox.Show("设备不在线！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (NetWorkClient.ControllerServer.Connected)
                {
                    NetWorkClient.ControllerServer.Send(JsonInterFace.Get_SYNC_Info_Request(
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
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("同步状态信息查询", ex.Message, ex.StackTrace);
            }
        }

        private void btnGSMV2SYNCInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!JsonInterFace.GSMV2DeviceParameter.OnLine.Equals("1"))
                {
                    MessageBox.Show("设备不在线！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (NetWorkClient.ControllerServer.Connected)
                {
                    NetWorkClient.ControllerServer.Send(JsonInterFace.Get_SYNC_Info_Request(
                                                                                                JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                                JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                                JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                                                                                JsonInterFace.GSMV2DeviceParameter.Port,
                                                                                                JsonInterFace.GSMV2DeviceParameter.InnerType,
                                                                                                JsonInterFace.GSMV2DeviceParameter.SN
                                                                                            ));
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("同步状态信息查询", ex.Message, ex.StackTrace);
            }
        }
        /// <summary>
        /// 同步参考源查询
        /// </summary>
        private void ShowSyncinfo()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        NetWorkClient.ControllerServer.Send(JsonInterFace.APSyncinfoGetRequest(
                                                                                        JsonInterFace.LteDeviceAdvanceSettingParameter.DomainFullPathName,
                                                                                        JsonInterFace.LteDeviceAdvanceSettingParameter.DeviceName,
                                                                                        JsonInterFace.LteDeviceAdvanceSettingParameter.IpAddr,
                                                                                        int.Parse(JsonInterFace.LteDeviceAdvanceSettingParameter.Port),
                                                                                        JsonInterFace.LteDeviceAdvanceSettingParameter.InnerType,
                                                                                        JsonInterFace.LteDeviceAdvanceSettingParameter.SN
                                                                                       ));
                    }
                    else
                    {
                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                });
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("同步参考源查询", ex.Message, ex.StackTrace);
            }
        }

        private void DownLoadLTELogsActionInfoPrint(IntPtr lParam)
        {
            StringBuilder DownLoadAPLogActionInfoList = new StringBuilder();
            DownLoadAPLogActionInfoList.Append(Marshal.PtrToStringBSTR(lParam));

            Dispatcher.Invoke(() =>
            {
                txtDownLoadLogsShow.AppendText(DownLoadAPLogActionInfoList.ToString());
            });

            try
            {
                if (new Regex("成功").Match(DownLoadAPLogActionInfoList.ToString()).Success)
                {
                    //与服务端建立FTP连接
                    if (FTP == null)
                    {
                        FTP = new FtpHelper(
                                                JsonInterFace.RemoteHost,
                                                Parameters.DownLoadLogLastDirWithSN ? Parameters.DownLoadLogRoot + MainWindow.aDeviceSelected.SN : Parameters.DownLoadLogRoot,
                                                Parameters.DownLoadLogUser,
                                                Parameters.DownLoadLogPass,
                                                Parameters.DownLoadLogPort
                                            );

                    }
                    else if (!FTP.Connected)
                    {
                        FTP = new FtpHelper(
                                                JsonInterFace.RemoteHost,
                                                Parameters.DownLoadLogLastDirWithSN ? Parameters.DownLoadLogRoot + MainWindow.aDeviceSelected.SN : Parameters.DownLoadLogRoot,
                                                Parameters.DownLoadLogUser,
                                                Parameters.DownLoadLogPass,
                                                Parameters.DownLoadLogPort
                                           );
                    }

                    if (FTP.Connected)
                    {
                        DownLoadAPLogActionInfoList.Clear();
                        DownLoadAPLogActionInfoList.AppendLine("---------- 下载AP日志文件, 建立【Client-- > Server】连接成功 ----------");
                        DownLoadAPLogActionInfoList.AppendLine("FTP Server[" + JsonInterFace.RemoteHost + "]");
                        DownLoadAPLogActionInfoList.AppendLine("FTP Port[" + Parameters.DownLoadLogPort.ToString() + "]");
                        DownLoadAPLogActionInfoList.AppendLine("-----------------------------------------------------------------");

                        Dispatcher.Invoke(() =>
                        {
                            txtDownLoadLogsShow.AppendText(DownLoadAPLogActionInfoList.ToString());
                        });

                        //列出文件
                        SubWindow.LogsFileListWindow LogsFileDownLoad = new SubWindow.LogsFileListWindow();
                        LogsFileDownLoad.FTP = FTP;
                        LogsFileDownLoad.FTPRemoteDir = "/" + JsonInterFace.RemoteHost + (Parameters.DownLoadLogLastDirWithSN ? (Parameters.DownLoadLogRoot + MainWindow.aDeviceSelected.SN) : Parameters.DownLoadLogRoot) + "/";
                        LogsFileDownLoad.ShowDialog();
                    }
                    else
                    {
                        DownLoadAPLogActionInfoList.Clear();
                        DownLoadAPLogActionInfoList.AppendLine("---------- 下载AP日志文件, 建立【Client-- > Server】连接失败 ----------");
                        DownLoadAPLogActionInfoList.AppendLine("FTP Server[" + JsonInterFace.RemoteHost + "]");
                        DownLoadAPLogActionInfoList.AppendLine("FTP Port[" + Parameters.DownLoadLogPort.ToString() + "]");
                        DownLoadAPLogActionInfoList.AppendLine("-----------------------------------------------------------------");

                        Dispatcher.Invoke(() =>
                        {
                            txtDownLoadLogsShow.AppendText(DownLoadAPLogActionInfoList.ToString());
                        });
                    }
                }
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("下载AP日志消息------[失败]", Ex.Message, Ex.StackTrace);
                Dispatcher.Invoke(() =>
                {
                    txtDownLoadLogsShow.AppendText("下载AP日志消息------[失败]" + Environment.NewLine + Ex.Message);
                });
            }
        }

        private void DownLoadWCDMALogsActionInfoPrint(IntPtr lParam)
        {
            StringBuilder DownLoadAPLogActionInfoList = new StringBuilder();
            DownLoadAPLogActionInfoList.Append(Marshal.PtrToStringBSTR(lParam));

            Dispatcher.Invoke(() =>
            {
                txtWCDMADownLoadLogsShow.AppendText(DownLoadAPLogActionInfoList.ToString());
            });

            try
            {
                if (new Regex("成功").Match(DownLoadAPLogActionInfoList.ToString()).Success)
                {
                    //与服务端建立FTP连接
                    if (FTP == null)
                    {
                        FTP = new FtpHelper(
                                                JsonInterFace.RemoteHost,
                                                Parameters.DownLoadLogLastDirWithSN ? Parameters.DownLoadLogRoot + MainWindow.aDeviceSelected.SN : Parameters.DownLoadLogRoot,
                                                Parameters.DownLoadLogUser,
                                                Parameters.DownLoadLogPass,
                                                Parameters.DownLoadLogPort
                                            );

                    }
                    else if (!FTP.Connected)
                    {
                        FTP = new FtpHelper(
                                                JsonInterFace.RemoteHost,
                                                Parameters.DownLoadLogLastDirWithSN ? Parameters.DownLoadLogRoot + MainWindow.aDeviceSelected.SN : Parameters.DownLoadLogRoot,
                                                Parameters.DownLoadLogUser,
                                                Parameters.DownLoadLogPass,
                                                Parameters.DownLoadLogPort
                                           );
                    }

                    if (FTP.Connected)
                    {
                        DownLoadAPLogActionInfoList.Clear();
                        DownLoadAPLogActionInfoList.AppendLine("---------- 下载AP日志文件, 建立【Client-- > Server】连接成功 ----------");
                        DownLoadAPLogActionInfoList.AppendLine("FTP Server[" + JsonInterFace.RemoteHost + "]");
                        DownLoadAPLogActionInfoList.AppendLine("FTP Port[" + Parameters.DownLoadLogPort.ToString() + "]");
                        DownLoadAPLogActionInfoList.AppendLine("-----------------------------------------------------------------");

                        Dispatcher.Invoke(() =>
                        {
                            txtWCDMADownLoadLogsShow.AppendText(DownLoadAPLogActionInfoList.ToString());
                        });

                        //列出文件
                        SubWindow.LogsFileListWindow LogsFileDownLoad = new SubWindow.LogsFileListWindow();
                        LogsFileDownLoad.FTP = FTP;
                        LogsFileDownLoad.FTPRemoteDir = "/" + JsonInterFace.RemoteHost + (Parameters.DownLoadLogLastDirWithSN ? (Parameters.DownLoadLogRoot + MainWindow.aDeviceSelected.SN) : Parameters.DownLoadLogRoot) + "/";
                        LogsFileDownLoad.ShowDialog();
                    }
                    else
                    {
                        DownLoadAPLogActionInfoList.Clear();
                        DownLoadAPLogActionInfoList.AppendLine("---------- 下载AP日志文件, 建立【Client-- > Server】连接失败 ----------");
                        DownLoadAPLogActionInfoList.AppendLine("FTP Server[" + JsonInterFace.RemoteHost + "]");
                        DownLoadAPLogActionInfoList.AppendLine("FTP Port[" + Parameters.DownLoadLogPort.ToString() + "]");
                        DownLoadAPLogActionInfoList.AppendLine("-----------------------------------------------------------------");

                        Dispatcher.Invoke(() =>
                        {
                            txtWCDMADownLoadLogsShow.AppendText(DownLoadAPLogActionInfoList.ToString());
                        });
                    }
                }
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("下载AP日志消息------[失败]", Ex.Message, Ex.StackTrace);
                Dispatcher.Invoke(() =>
                {
                    txtWCDMADownLoadLogsShow.AppendText("下载AP日志消息------[失败]" + Environment.NewLine + Ex.Message);
                });
            }
        }

        private void DownLoadTDSLogsActionInfoPrint(IntPtr lParam)
        {
            StringBuilder DownLoadAPLogActionInfoList = new StringBuilder();
            DownLoadAPLogActionInfoList.Append(Marshal.PtrToStringBSTR(lParam));

            Dispatcher.Invoke(() =>
            {
                txtTDSDownLoadLogsShow.AppendText(DownLoadAPLogActionInfoList.ToString());
            });

            try
            {
                if (new Regex("成功").Match(DownLoadAPLogActionInfoList.ToString()).Success)
                {
                    //与服务端建立FTP连接
                    if (FTP == null)
                    {
                        FTP = new FtpHelper(
                                                JsonInterFace.RemoteHost,
                                                Parameters.DownLoadLogLastDirWithSN ? Parameters.DownLoadLogRoot + MainWindow.aDeviceSelected.SN : Parameters.DownLoadLogRoot,
                                                Parameters.DownLoadLogUser,
                                                Parameters.DownLoadLogPass,
                                                Parameters.DownLoadLogPort
                                            );

                    }
                    else if (!FTP.Connected)
                    {
                        FTP = new FtpHelper(
                                                JsonInterFace.RemoteHost,
                                                Parameters.DownLoadLogLastDirWithSN ? Parameters.DownLoadLogRoot + MainWindow.aDeviceSelected.SN : Parameters.DownLoadLogRoot,
                                                Parameters.DownLoadLogUser,
                                                Parameters.DownLoadLogPass,
                                                Parameters.DownLoadLogPort
                                           );
                    }

                    if (FTP.Connected)
                    {
                        DownLoadAPLogActionInfoList.Clear();
                        DownLoadAPLogActionInfoList.AppendLine("---------- 下载AP日志文件, 建立【Client-- > Server】连接成功 ----------");
                        DownLoadAPLogActionInfoList.AppendLine("FTP Server[" + JsonInterFace.RemoteHost + "]");
                        DownLoadAPLogActionInfoList.AppendLine("FTP Port[" + Parameters.DownLoadLogPort.ToString() + "]");
                        DownLoadAPLogActionInfoList.AppendLine("-----------------------------------------------------------------");

                        Dispatcher.Invoke(() =>
                        {
                            txtTDSDownLoadLogsShow.AppendText(DownLoadAPLogActionInfoList.ToString());
                        });

                        //列出文件
                        SubWindow.LogsFileListWindow LogsFileDownLoad = new SubWindow.LogsFileListWindow();
                        LogsFileDownLoad.FTP = FTP;
                        LogsFileDownLoad.FTPRemoteDir = "/" + JsonInterFace.RemoteHost + (Parameters.DownLoadLogLastDirWithSN ? (Parameters.DownLoadLogRoot + MainWindow.aDeviceSelected.SN) : Parameters.DownLoadLogRoot) + "/";
                        LogsFileDownLoad.ShowDialog();
                    }
                    else
                    {
                        DownLoadAPLogActionInfoList.Clear();
                        DownLoadAPLogActionInfoList.AppendLine("---------- 下载AP日志文件, 建立【Client-- > Server】连接失败 ----------");
                        DownLoadAPLogActionInfoList.AppendLine("FTP Server[" + JsonInterFace.RemoteHost + "]");
                        DownLoadAPLogActionInfoList.AppendLine("FTP Port[" + Parameters.DownLoadLogPort.ToString() + "]");
                        DownLoadAPLogActionInfoList.AppendLine("-----------------------------------------------------------------");

                        Dispatcher.Invoke(() =>
                        {
                            txtTDSDownLoadLogsShow.AppendText(DownLoadAPLogActionInfoList.ToString());
                        });
                    }
                }
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("下载AP日志消息------[失败]", Ex.Message, Ex.StackTrace);
                Dispatcher.Invoke(() =>
                {
                    txtTDSDownLoadLogsShow.AppendText("下载AP日志消息------[失败]" + Environment.NewLine + Ex.Message);
                });
            }
        }

        private void DownLoadAPLogsErrMessage(IntPtr lParam)
        {
            StringBuilder DownLoadAPLogActionInfoList = new StringBuilder();
            DownLoadAPLogActionInfoList.Append(Marshal.PtrToStringBSTR(lParam));

            Dispatcher.Invoke(() =>
            {
                if (MainWindow.aDeviceSelected.Model == DeviceType.WCDMA)
                {
                    txtWCDMADownLoadLogsShow.AppendText(DownLoadAPLogActionInfoList.ToString());
                }
                else if (new Regex(DeviceType.LTE).Match(MainWindow.aDeviceSelected.Model).Success)
                {
                    txtDownLoadLogsShow.AppendText(DownLoadAPLogActionInfoList.ToString());
                }
            });
        }

        private void ApSystemUpgradeMessage(IntPtr lParam)
        {
            StringBuilder UpdateRequestMsg = new StringBuilder();
            UpdateRequestMsg.Append(Marshal.PtrToStringBSTR(lParam));
            JsonInterFace.ProgressBarInfo.ResoultMessage += UpdateRequestMsg.ToString();
        }

        /// <summary>
        /// LTE下载Log默认路径被改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtLogFilePath_TextChanged(object sender, TextChangedEventArgs e)
        {
            JsonInterFace.LteDeviceSystemMaintenenceParameter.LogFiles = txtLogFilePath.Text;
        }

        /// <summary>
        /// WCDMA下载Log默认路径被改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtWCDMALogFilePath_TextChanged(object sender, TextChangedEventArgs e)
        {
            JsonInterFace.WCDMADeviceSystemMaintenenceParameter.LogFiles = txtWCDMALogFilePath.Text;
        }

        private void btnWCDMAUpdateFile_Click(object sender, RoutedEventArgs e)
        {
            ApSystemUpDateTask();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //停止超时检测
            WaitUpDownLoadTimeOut.Stop();

            //停止记时
            JsonInterFace.ProgressBarInfo.UpgradeTimer.Stop();

            //进度线程停止
            if (ControlUpDownActionInfoThread != null)
            {
                if (ControlUpDownActionInfoThread.ThreadState == ThreadState.Running || ControlUpDownActionInfoThread.ThreadState == ThreadState.WaitSleepJoin)
                {
                    ControlUpDownActionInfoThread.Abort();
                    ControlUpDownActionInfoThread.Join();
                }
            }

            JsonInterFace.ProgressBarInfo.ResoultMessage = string.Empty;
            Parameters.DeviceManageWinHandle = IntPtr.Zero;
        }
        /// <summary>
        /// GSMV2是否自动发短信
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkGSMV2AutoSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string SMSctrl = string.Empty;

                string BSMSOriginalNum = ((GSMV2ConfigSMSMSGClass)GSMV2SMSMessageDataGrid.SelectedItem).BSMSOriginalNum;
                string BSMSContent = ((GSMV2ConfigSMSMSGClass)GSMV2SMSMessageDataGrid.SelectedItem).BSMSContent;
                if ((bool)(sender as CheckBox).IsChecked)
                {
                    SMSctrl = "1";
                }
                else
                {
                    SMSctrl = "0";
                }
                if (NetWorkClient.ControllerServer.Connected)
                {
                    //发送IMSI
                    JsonInterFace.ResultMessageList.Clear();

                    new Thread(() =>
                    {
                        NetWorkClient.ControllerServer.Send(
                                                        JsonInterFace.GSMV2SMSRequest
                                                                                        (
                                                                                            JsonInterFace.GSMV2DeviceParameter.DomainFullPathName,
                                                                                            JsonInterFace.GSMV2DeviceParameter.DeviceName,
                                                                                            JsonInterFace.GSMV2DeviceParameter.IpAddr,
                                                                                            JsonInterFace.GSMV2DeviceParameter.Port,
                                                                                            JsonInterFace.GSMV2DeviceParameter.InnerType,
                                                                                            JsonInterFace.GSMV2DeviceParameter.SN,
                                                                                            SMSctrl,
                                                                                            BSMSOriginalNum,
                                                                                            BSMSContent,
                                                                                            "0"
                                                                                        )
                                                       );
                    }).Start();
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("是否自动发送短信", ex.Message, ex.StackTrace);
            }
        }

        private void chkCDMAAutoSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string SMSctrl = string.Empty;
                string BSMSOriginalNum = ((CDMAConfigSMSMSGClass)CDMASMSMessageDataGrid.SelectedItem).BSMSOriginalNum;
                string BSMSContent = ((CDMAConfigSMSMSGClass)CDMASMSMessageDataGrid.SelectedItem).BSMSContent;

                if ((bool)(sender as CheckBox).IsChecked)
                {
                    SMSctrl = "1";
                }
                else
                {
                    SMSctrl = "0";
                }
                if (NetWorkClient.ControllerServer.Connected)
                {
                    //发送IMSI
                    JsonInterFace.ResultMessageList.Clear();

                    new Thread(() =>
                    {
                        NetWorkClient.ControllerServer.Send(
                                                        JsonInterFace.GSMV2SMSRequest
                                                                                        (
                                                                                            JsonInterFace.CDMADeviceParameter.DomainFullPathName,
                                                                                            JsonInterFace.CDMADeviceParameter.DeviceName,
                                                                                            JsonInterFace.CDMADeviceParameter.IpAddr,
                                                                                            JsonInterFace.CDMADeviceParameter.Port,
                                                                                            JsonInterFace.CDMADeviceParameter.InnerType,
                                                                                            JsonInterFace.CDMADeviceParameter.SN,
                                                                                            SMSctrl,
                                                                                            BSMSOriginalNum,
                                                                                            BSMSContent,
                                                                                            "0"
                                                                                        )
                                                       );
                    }).Start();
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("是否自动发送短信", ex.Message, ex.StackTrace);
            }
        }

        private void btnGSMUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Dictionary<string, string> Params = new Dictionary<string, string>();
                if (selfParam.Mode != (JsonInterFace.GSMDeviceParameter.DeviceMode))
                {
                    Params.Add("mode", cbxGSMDeviceMode.Text.Trim());
                }

                if (selfParam.SN != (JsonInterFace.GSMDeviceParameter.SN))
                {
                    Params.Add("sn", txtGSMSN.Text.Trim());
                }

                if (selfParam.IP != (JsonInterFace.GSMDeviceParameter.IpAddr))
                {
                    Params.Add("ipAddr", txtGSMIPAddr.Text.Trim());
                }

                if (selfParam.Port != (JsonInterFace.GSMDeviceParameter.Port))
                {
                    Params.Add("port", txtGSMPort.Text.Trim());
                }

                if (selfParam.NetMask != (JsonInterFace.GSMDeviceParameter.NetMask))
                {
                    Params.Add("netmask", txtGSMNetMask.Text.Trim());
                }

                if (selfParam.DeviceName != (JsonInterFace.GSMDeviceParameter.DeviceName))
                {
                    Params.Add("name", txtGSMDeviceName.Text.Trim());

                }
                if (Params.Count <= 0)
                {
                    MessageBox.Show("参数内容未改变！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                    return;
                }

                if (MessageBox.Show("确定更新设备信息[" + JsonInterFace.GSMDeviceParameter.DeviceName + "]?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        Parameters.ConfigType = "Manul";
                        NetWorkClient.ControllerServer.Send(JsonInterFace.AddDeviceNameUpdateParametersRequest(JsonInterFace.GSMDeviceParameter.DomainFullPathName, selfParam.DeviceName, Params));
                    }
                    else
                    {
                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
                MessageBox.Show("参数异常(未初始化),更新失败！");
            }
        }

        private void btnWCDMAUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Dictionary<string, string> Params = new Dictionary<string, string>();
                if (selfParam.Mode != (JsonInterFace.WCDMADeviceParameter.DeviceMode))
                {
                    Params.Add("mode", cbxWCDMADeviceMode.Text.Trim());
                }

                if (selfParam.SN != (JsonInterFace.WCDMADeviceParameter.SN))
                {
                    Params.Add("sn", txtWCDMASN.Text.Trim());
                }

                if (selfParam.IP != (JsonInterFace.WCDMADeviceParameter.IpAddr))
                {
                    Params.Add("ipAddr", txtWCDMAIPAddr.Text.Trim());
                }

                if (selfParam.Port != (JsonInterFace.WCDMADeviceParameter.Port))
                {
                    Params.Add("port", txtWCDMAPort.Text.Trim());
                }

                if (selfParam.NetMask != (JsonInterFace.WCDMADeviceParameter.NetMask))
                {
                    Params.Add("netmask", txtWCDMANetMask.Text.Trim());
                }

                if (selfParam.DeviceName != (JsonInterFace.WCDMADeviceParameter.DeviceName))
                {
                    Params.Add("name", txtWCDMADeviceName.Text.Trim());
                }
                if (Params.Count <= 0)
                {
                    MessageBox.Show("参数内容未改变！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                    return;
                }

                if (MessageBox.Show("确定更新设备信息[" + JsonInterFace.WCDMADeviceParameter.DeviceName + "]?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        Parameters.ConfigType = "Manul";
                        NetWorkClient.ControllerServer.Send(JsonInterFace.AddDeviceNameUpdateParametersRequest(JsonInterFace.WCDMADeviceParameter.DomainFullPathName, selfParam.DeviceName, Params));
                    }
                    else
                    {
                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
                MessageBox.Show("参数异常(未初始化),更新失败！");
            }
        }

        private void btnCDMAUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Dictionary<string, string> Params = new Dictionary<string, string>();
                if (selfParam.Mode != (JsonInterFace.CDMADeviceParameter.DeviceMode))
                {
                    Params.Add("mode", cbxCDMADeviceMode.Text.Trim());
                }

                if (selfParam.SN != (JsonInterFace.CDMADeviceParameter.SN))
                {
                    Params.Add("sn", txtCDMASN.Text.Trim());
                }

                if (selfParam.IP != (JsonInterFace.CDMADeviceParameter.IpAddr))
                {
                    Params.Add("ipAddr", txtCDMAIPAddr.Text.Trim());
                }

                if (selfParam.Port != (JsonInterFace.CDMADeviceParameter.Port))
                {
                    Params.Add("port", txtCDMAPort.Text.Trim());
                }

                if (selfParam.NetMask != (JsonInterFace.CDMADeviceParameter.NetMask))
                {
                    Params.Add("netmask", txtCDMANetMask.Text.Trim());
                }

                if (selfParam.DeviceName != (JsonInterFace.CDMADeviceParameter.DeviceName))
                {
                    Params.Add("name", txtCDMADeviceName.Text.Trim());

                }
                if (Params.Count <= 0)
                {
                    MessageBox.Show("参数内容未改变！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                    return;
                }

                if (MessageBox.Show("确定更新设备信息[" + JsonInterFace.CDMADeviceParameter.DeviceName + "]?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        Parameters.ConfigType = "Manul";
                        NetWorkClient.ControllerServer.Send(JsonInterFace.AddDeviceNameUpdateParametersRequest(JsonInterFace.CDMADeviceParameter.DomainFullPathName, selfParam.DeviceName, Params));
                    }
                    else
                    {
                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
                MessageBox.Show("参数异常(未初始化),更新失败！");
            }
        }

        private void btnTDSUpdate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnGSMDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("确定删除设备[" + JsonInterFace.GSMDeviceParameter.DeviceName + "]?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        NetWorkClient.ControllerServer.Send(JsonInterFace.DeleteDeviceNameRequest(JsonInterFace.GSMDeviceParameter.DomainFullPathName, JsonInterFace.GSMDeviceParameter.DeviceName));

                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("删除设备异常", ex.Message, ex.StackTrace);
            }
        }

        private void btnWCDMADelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("确定删除设备[" + JsonInterFace.WCDMADeviceParameter.DeviceName + "]?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        NetWorkClient.ControllerServer.Send(JsonInterFace.DeleteDeviceNameRequest(JsonInterFace.WCDMADeviceParameter.DomainFullPathName, JsonInterFace.WCDMADeviceParameter.DeviceName));

                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("删除设备异常", ex.Message, ex.StackTrace);
            }
        }

        private void btnCDMADelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("确定删除设备[" + JsonInterFace.CDMADeviceParameter.DeviceName + "]?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        NetWorkClient.ControllerServer.Send(JsonInterFace.DeleteDeviceNameRequest(JsonInterFace.CDMADeviceParameter.DomainFullPathName, JsonInterFace.CDMADeviceParameter.DeviceName));

                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("删除设备异常", ex.Message, ex.StackTrace);
            }
        }

        private void btnTDSDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnTDSUpdatePragram_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbTDSParameterKeyName.Text != "" && cbTDSParameterKeyName.Text != null
                    && txtTDSParameterValue.Text != "" && txtTDSParameterValue.Text != null)
                {
                    string ParameterValue;
                    if (cbTDSParameterValue.Items.Count > 0)
                    {
                        if (cbTDSParameterValue.Items.IndexOf(txtTDSParameterValue.Text) > -1)
                        {
                            if (cbTDSParameterKeyName.SelectedIndex == 1)
                            {
                                ParameterValue = (cbTDSParameterValue.Items.IndexOf(txtTDSParameterValue.Text) + 1).ToString();
                            }
                            else
                            {
                                ParameterValue = cbTDSParameterValue.Items.IndexOf(txtTDSParameterValue.Text).ToString();
                            }
                        }
                        else
                        {
                            ParameterValue = txtTDSParameterValue.Text;
                        }

                    }
                    else
                    {
                        ParameterValue = txtTDSParameterValue.Text;
                    }
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        Parameters.ConfigType = "ProjectSetting";
                        NetWorkClient.ControllerServer.Send(JsonInterFace.APProjectSingleParameterSettingRequest(
                                                                                                    JsonInterFace.TDSDeviceParameter.DomainFullPathName,
                                                                                                    JsonInterFace.TDSDeviceParameter.DeviceName,
                                                                                                    JsonInterFace.TDSDeviceParameter.IpAddr,
                                                                                                    JsonInterFace.TDSDeviceParameter.Port,
                                                                                                    JsonInterFace.TDSDeviceParameter.InnerType,
                                                                                                    JsonInterFace.TDSDeviceParameter.SN,
                                                                                                    ParameterKeyName,
                                                                                                    ParameterValue
                                                                                                ));
                    }
                    else
                    {
                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("参数内容为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("单项参数设置，" + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnTDSSelectUpLoad_Click(object sender, RoutedEventArgs e)
        {
            if (!JsonInterFace.TDSDeviceParameter.OnLine.Equals("1"))
            {
                MessageBox.Show("设备不在线！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (NetWorkClient.ControllerServer.Connected)
            {
                Parameters.ConfigType = "SelectUpload";
                NetWorkClient.ControllerServer.Send(JsonInterFace.Get_Upload_Request(
                                                                                            JsonInterFace.TDSDeviceParameter.DomainFullPathName,
                                                                                            JsonInterFace.TDSDeviceParameter.DeviceName,
                                                                                            JsonInterFace.TDSDeviceParameter.IpAddr,
                                                                                            JsonInterFace.TDSDeviceParameter.Port,
                                                                                            JsonInterFace.TDSDeviceParameter.InnerType,
                                                                                            JsonInterFace.TDSDeviceParameter.SN
                                                                                        ));
            }
            else
            {
                MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnTDSSetUpLoad_Click(object sender, RoutedEventArgs e)
        {
            if (!JsonInterFace.TDSDeviceParameter.OnLine.Equals("1"))
            {
                MessageBox.Show("设备不在线！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (NetWorkClient.ControllerServer.Connected)
            {
                NetWorkClient.ControllerServer.Send(JsonInterFace.Set_Upload_Request(
                                                                                            JsonInterFace.TDSDeviceParameter.DomainFullPathName,
                                                                                            JsonInterFace.TDSDeviceParameter.DeviceName,
                                                                                            JsonInterFace.TDSDeviceParameter.IpAddr,
                                                                                            JsonInterFace.TDSDeviceParameter.Port,
                                                                                            JsonInterFace.TDSDeviceParameter.InnerType,
                                                                                            JsonInterFace.TDSDeviceParameter.SN
                                                                                        ));
            }
            else
            {
                MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnTDSSYNCInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!JsonInterFace.TDSDeviceParameter.OnLine.Equals("1"))
                {
                    MessageBox.Show("设备不在线！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (NetWorkClient.ControllerServer.Connected)
                {
                    NetWorkClient.ControllerServer.Send(JsonInterFace.Get_SYNC_Info_Request(
                                                                                                JsonInterFace.TDSDeviceParameter.DomainFullPathName,
                                                                                                JsonInterFace.TDSDeviceParameter.DeviceName,
                                                                                                JsonInterFace.TDSDeviceParameter.IpAddr,
                                                                                                JsonInterFace.TDSDeviceParameter.Port,
                                                                                                JsonInterFace.TDSDeviceParameter.InnerType,
                                                                                                JsonInterFace.TDSDeviceParameter.SN
                                                                                            ));
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("同步状态信息查询", ex.Message, ex.StackTrace);
            }
        }

        private void btnTDSCellNeighUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Dictionary<string, string> CellNeighParams = new Dictionary<string, string>();
                Dictionary<string, string> SetWorkModeParams = new Dictionary<string, string>();
                for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                {
                    if (JsonInterFace.APATTributesLists[i].FullName == FullName)
                    {
                        if (MessageBox.Show("确定更新小区信息[" + JsonInterFace.TDSCellNeighParameter.DeviceName + "]?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                        {
                            if (JsonInterFace.TDSCellNeighParameter.PLMN != null && JsonInterFace.TDSCellNeighParameter.PLMN != JsonInterFace.APATTributesLists[i].PLMN)
                            {
                                string mcc = JsonInterFace.TDSCellNeighParameter.PLMN.Substring(0, Parameters.PLMN_Lengh - 2);
                                CellNeighParams.Add("mcc", mcc);
                                string mnc = JsonInterFace.TDSCellNeighParameter.PLMN.Substring(Parameters.PLMN_Lengh - 2, JsonInterFace.TDSCellNeighParameter.PLMN.Length - (Parameters.PLMN_Lengh - 2));
                                CellNeighParams.Add("mnc", mnc);
                                JsonInterFace.APATTributesLists[i].PLMN = JsonInterFace.TDSCellNeighParameter.PLMN;
                            }

                            if (JsonInterFace.TDSCellNeighParameter.FrequencyPoint != null && JsonInterFace.TDSCellNeighParameter.FrequencyPoint != JsonInterFace.APATTributesLists[i].FrequencyPoint)
                            {
                                CellNeighParams.Add("euarfcn", JsonInterFace.TDSCellNeighParameter.FrequencyPoint);
                                JsonInterFace.APATTributesLists[i].FrequencyPoint = JsonInterFace.TDSCellNeighParameter.FrequencyPoint;
                            }

                            if (JsonInterFace.TDSCellNeighParameter.PowerAttenuation != null && JsonInterFace.TDSCellNeighParameter.PowerAttenuation != JsonInterFace.APATTributesLists[i].PowerAttenuation)
                            {
                                CellNeighParams.Add("txpower", JsonInterFace.TDSCellNeighParameter.PowerAttenuation);
                                JsonInterFace.APATTributesLists[i].PowerAttenuation = JsonInterFace.TDSCellNeighParameter.PowerAttenuation;
                            }

                            if (JsonInterFace.TDSCellNeighParameter.Scrambler != null && JsonInterFace.TDSCellNeighParameter.Scrambler != JsonInterFace.APATTributesLists[i].Scrambler)
                            {
                                CellNeighParams.Add("pci", JsonInterFace.TDSCellNeighParameter.Scrambler);
                                JsonInterFace.APATTributesLists[i].Scrambler = JsonInterFace.TDSCellNeighParameter.Scrambler;
                            }

                            if (JsonInterFace.TDSCellNeighParameter.TacLac != null && JsonInterFace.TDSCellNeighParameter.TacLac != JsonInterFace.APATTributesLists[i].TacLac)
                            {
                                CellNeighParams.Add("tac", JsonInterFace.TDSCellNeighParameter.TacLac);
                                JsonInterFace.APATTributesLists[i].TacLac = JsonInterFace.TDSCellNeighParameter.TacLac;
                            }

                            if (JsonInterFace.TDSCellNeighParameter.Period != null && JsonInterFace.TDSCellNeighParameter.Period != JsonInterFace.APATTributesLists[i].Period)
                            {
                                CellNeighParams.Add("periodTac", JsonInterFace.TDSCellNeighParameter.Period);
                                JsonInterFace.APATTributesLists[i].Period = JsonInterFace.TDSCellNeighParameter.Period;
                            }
                            if (JsonInterFace.TDSSetWorkModeParameter.RebootModeAuto && JsonInterFace.TDSSetWorkModeParameter.RebootModeAuto != JsonInterFace.APATTributesLists[i].RebootModeAuto)
                            {
                                SetWorkModeParams.Add("boot", "1");
                                JsonInterFace.APATTributesLists[i].RebootModeAuto = JsonInterFace.TDSSetWorkModeParameter.RebootModeAuto;
                                JsonInterFace.APATTributesLists[i].RebootModeManul = false;
                            }
                            else if (JsonInterFace.TDSSetWorkModeParameter.RebootModeManul && JsonInterFace.TDSSetWorkModeParameter.RebootModeManul != JsonInterFace.APATTributesLists[i].RebootModeManul)
                            {
                                SetWorkModeParams.Add("boot", "0");
                                JsonInterFace.APATTributesLists[i].RebootModeManul = JsonInterFace.TDSSetWorkModeParameter.RebootModeManul;
                                JsonInterFace.APATTributesLists[i].RebootModeAuto = false;
                            }
                            if (JsonInterFace.TDSSetWorkModeParameter.FrequencyChioceModeAuto && JsonInterFace.TDSSetWorkModeParameter.FrequencyChioceModeAuto != JsonInterFace.APATTributesLists[i].FrequencyChioceModeAuto)
                            {
                                SetWorkModeParams.Add("manualFreq", "0");
                                JsonInterFace.APATTributesLists[i].FrequencyChioceModeAuto = JsonInterFace.TDSSetWorkModeParameter.FrequencyChioceModeAuto;
                            }

                            else if (JsonInterFace.TDSSetWorkModeParameter.FrequencyChioceModeManul && JsonInterFace.TDSSetWorkModeParameter.FrequencyChioceModeManul != JsonInterFace.APATTributesLists[i].FrequencyChioceModeManul)
                            {
                                SetWorkModeParams.Add("manualFreq", "1");
                                JsonInterFace.APATTributesLists[i].FrequencyChioceModeManul = JsonInterFace.TDSSetWorkModeParameter.FrequencyChioceModeManul;
                            }
                            if (CellNeighParams.Count <= 0 && SetWorkModeParams.Count <= 0)
                            {
                                MessageBox.Show("内容未修改！可以尝试重新获取信息", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                                return;
                            }
                            if (NetWorkClient.ControllerServer.Connected)
                            {
                                JsonInterFace.ResultMessageList.Clear();

                                //工作模式设置
                                if (SetWorkModeParams.Count > 0)
                                {
                                    NetWorkClient.ControllerServer.Send(JsonInterFace.APSetWorkModeRequest(
                                                                                                        JsonInterFace.TDSCellNeighParameter.DomainFullPathName,
                                                                                                        JsonInterFace.TDSCellNeighParameter.DeviceName,
                                                                                                        JsonInterFace.TDSCellNeighParameter.IpAddr,
                                                                                                        JsonInterFace.TDSCellNeighParameter.Port,
                                                                                                        JsonInterFace.TDSCellNeighParameter.InnerType,
                                                                                                        JsonInterFace.TDSCellNeighParameter.SN,
                                                                                                        SetWorkModeParams
                                                                                                      ));
                                }
                                if (CellNeighParams.Count > 0)
                                {
                                    //小区设置
                                    NetWorkClient.ControllerServer.Send(JsonInterFace.APSetConfigurationRequest(
                                                                                                            JsonInterFace.TDSCellNeighParameter.DomainFullPathName,
                                                                                                            JsonInterFace.TDSCellNeighParameter.DeviceName,
                                                                                                            JsonInterFace.TDSCellNeighParameter.IpAddr,
                                                                                                            JsonInterFace.TDSCellNeighParameter.Port,
                                                                                                            JsonInterFace.TDSCellNeighParameter.InnerType,
                                                                                                            JsonInterFace.TDSCellNeighParameter.SN,
                                                                                                            CellNeighParams
                                                                                                            ));
                                }
                            }
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "更新失败！");
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void rdbTDSRebootModeAuto_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as CheckBox).IsChecked)
            {
                rdbTDSRebootModeManul.IsChecked = false;
            }
            else
            {
                if (!(bool)(sender as CheckBox).IsChecked)
                {
                    rdbTDSRebootModeAuto.IsChecked = true;
                }
            }
        }

        private void rdbTDSRebootModeManul_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as CheckBox).IsChecked)
            {
                rdbTDSRebootModeAuto.IsChecked = false;
            }
            else
            {
                if (!(bool)(sender as CheckBox).IsChecked)
                {
                    rdbTDSRebootModeManul.IsChecked = true;
                }
            }
        }

        private void miTDSMultiPLMNListClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TDSMorePLMNList.Clear();
                MessageBox.Show("多PLMN清空成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("多PLMN清空失败！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnTDSOtherPlmnSetting_Click(object sender, RoutedEventArgs e)
        {
            //多PLMN设置
            if (TDSMorePLMNList.Count > 0)
            {
                string PLMNList = string.Empty;
                for (int i = 0; i < TDSMorePLMNList.Count; i++)
                {
                    if (TDSMorePLMNList[i].PLMNS != "" || TDSMorePLMNList[i].PLMNS != null)
                    {
                        if (PLMNList == "")
                        {
                            PLMNList = TDSMorePLMNList[i].PLMNS;
                        }
                        else
                        {
                            PLMNList += "," + TDSMorePLMNList[i].PLMNS;
                        }
                    }
                }

                if (PLMNList == "" || PLMNList == null)
                {
                    MessageBox.Show("多PLMN参数为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                JsonInterFace.ResultMessageList.Clear();
                if (NetWorkClient.ControllerServer.Connected)
                {
                    Parameters.ConfigType = "OtherPLMN";
                    NetWorkClient.ControllerServer.Send(JsonInterFace.APSettingOhterPLMNRequest(
                                                                                                JsonInterFace.TDSCellNeighParameter.DomainFullPathName,
                                                                                                JsonInterFace.TDSCellNeighParameter.DeviceName,
                                                                                                JsonInterFace.TDSCellNeighParameter.IpAddr,
                                                                                                JsonInterFace.TDSCellNeighParameter.Port,
                                                                                                JsonInterFace.TDSCellNeighParameter.InnerType,
                                                                                                JsonInterFace.TDSCellNeighParameter.SN,
                                                                                                PLMNList
                                                                                                ));
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("多PLMN参数为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void miTDSFreqListClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TDSPeriorFreqList.Clear();
                MessageBox.Show("频点清空成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("频点清空失败！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnTDSPeriodFreqSetting_Click(object sender, RoutedEventArgs e)
        {
            //周期频点设置
            if (TDSPeriorFreqList.Count > 0)
            {
                string PeriorFreq = string.Empty;

                if (JsonInterFace.TDSCellNeighParameter.Cycle == "" || JsonInterFace.TDSCellNeighParameter.Cycle == null)
                {
                    MessageBox.Show("请输入周期时间(单位：秒)！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else if (!new Regex(@"\d").Match(JsonInterFace.TDSCellNeighParameter.Cycle).Success)
                {
                    MessageBox.Show("周期时间[" + JsonInterFace.TDSCellNeighParameter.Cycle + "]格式不正确,范围[0~65535] (单位：秒)！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else if (Convert.ToInt32(JsonInterFace.TDSCellNeighParameter.Cycle) < 0 || Convert.ToInt32(JsonInterFace.TDSCellNeighParameter.Cycle) > 65535)
                {
                    MessageBox.Show("周期时间[" + JsonInterFace.TDSCellNeighParameter.Cycle + "]已超出范围[0~65535] (单位：秒)！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                for (int i = 0; i < TDSPeriorFreqList.Count; i++)
                {
                    if (TDSPeriorFreqList[i].PerierFreq != "" && TDSPeriorFreqList[i].PerierFreq != null)
                    {
                        if (PeriorFreq == "")
                        {
                            PeriorFreq = TDSPeriorFreqList[i].PerierFreq;
                        }
                        else
                        {
                            PeriorFreq += "," + TDSPeriorFreqList[i].PerierFreq;
                        }
                    }
                }

                if (PeriorFreq == null || PeriorFreq == "")
                {
                    MessageBox.Show("频点参数为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                PeriorFreq = JsonInterFace.TDSCellNeighParameter.Cycle + ":" + PeriorFreq;
                JsonInterFace.ResultMessageList.Clear();
                if (NetWorkClient.ControllerServer.Connected)
                {
                    Parameters.ConfigType = "PeriodFreq";
                    NetWorkClient.ControllerServer.Send(JsonInterFace.APSettingPeriodFreqRequest(
                                                                                                JsonInterFace.TDSCellNeighParameter.DomainFullPathName,
                                                                                                JsonInterFace.TDSCellNeighParameter.DeviceName,
                                                                                                JsonInterFace.TDSCellNeighParameter.IpAddr,
                                                                                                JsonInterFace.TDSCellNeighParameter.Port,
                                                                                                JsonInterFace.TDSCellNeighParameter.InnerType,
                                                                                                JsonInterFace.TDSCellNeighParameter.SN,
                                                                                                PeriorFreq
                                                                                                ));
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("频点参数为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnTDSNTPUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Parameters.ConfigType = "NTP";
                if (txtTDSNTP.Text.Equals(""))
                {
                    MessageBox.Show("请输入NTP服务器地址！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (txtTDSPriority.Text.Equals(""))
                {
                    MessageBox.Show("请输入NTP优先级！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (Parameters.ConfigType.Trim().Equals(""))
                {
                    MessageBox.Show("未配置ConfigType类型！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (NetWorkClient.ControllerServer.Connected)
                {
                    NetWorkClient.ControllerServer.Send(JsonInterFace.NTPConfigrationRequest(
                                                                                                JsonInterFace.TDSDeviceAdvanceSettingParameter.DomainFullPathName,
                                                                                                JsonInterFace.TDSDeviceAdvanceSettingParameter.DeviceName,
                                                                                                JsonInterFace.TDSDeviceAdvanceSettingParameter.IpAddr,
                                                                                                int.Parse(JsonInterFace.TDSDeviceAdvanceSettingParameter.Port),
                                                                                                JsonInterFace.TDSDeviceAdvanceSettingParameter.InnerType,
                                                                                                JsonInterFace.TDSDeviceAdvanceSettingParameter.SN,
                                                                                                JsonInterFace.TDSDeviceAdvanceSettingParameter.NTPServerIP,
                                                                                                JsonInterFace.TDSDeviceAdvanceSettingParameter.NTPLevel
                                                                                            ));
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("NTP优先级设置", ex.Message, ex.StackTrace);
            }
        }

        private void btnTDSUpdateTime_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if ((txtTDSFirstPeriodTimeStart.Text != ""
                && txtTDSFirstPeriodTimeEnd.Text == "")
                || (txtTDSFirstPeriodTimeStart.Text == ""
                && txtTDSFirstPeriodTimeEnd.Text != ""))
                {
                    MessageBox.Show("第一时间段的时间不完整！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else if (txtTDSFirstPeriodTimeStart.Text != "" && txtTDSFirstPeriodTimeEnd.Text != "")
                {
                    //检测时间
                    if (!Parameters.IsTime(JsonInterFace.TDSDeviceAdvanceSettingParameter.FirstPeriodTimeStart))
                    {
                        MessageBox.Show("第一时间段的起始时间格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (!Parameters.IsTime(JsonInterFace.TDSDeviceAdvanceSettingParameter.FirstPeriodTimeEnd))
                    {
                        MessageBox.Show("第一时间段的结束时间格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                if ((txtTDSSecondPeriodTimeStart.Text != ""
                    && txtTDSSecoondPeriodTimeEnd.Text == "")
                    || (txtTDSSecondPeriodTimeStart.Text == ""
                    && txtTDSSecoondPeriodTimeEnd.Text != ""))
                {
                    MessageBox.Show("第二时间段的时间不完整！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else if (txtTDSSecondPeriodTimeStart.Text != "" && txtTDSSecoondPeriodTimeEnd.Text != "")
                {
                    //检测时间
                    if (!Parameters.IsTime(JsonInterFace.TDSDeviceAdvanceSettingParameter.SecondPeriodTimeStart))
                    {
                        MessageBox.Show("第二时间段的起始时间格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (!Parameters.IsTime(JsonInterFace.TDSDeviceAdvanceSettingParameter.SecoondPeriodTimeEnd))
                    {
                        MessageBox.Show("第二时间段的结束时间格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                if ((txtTDSThreePeriodTimeStart.Text != ""
                    && txtTDSThreePeriodTimeEnd.Text == "")
                    || (txtTDSThreePeriodTimeStart.Text == ""
                    && txtTDSThreePeriodTimeEnd.Text != ""))
                {
                    MessageBox.Show("第三时间段的时间不完整！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else if (txtTDSThreePeriodTimeStart.Text != "" && txtTDSThreePeriodTimeEnd.Text != "")
                {
                    //检测时间
                    if (!Parameters.IsTime(JsonInterFace.TDSDeviceAdvanceSettingParameter.ThreePeriodTimeStart))
                    {
                        MessageBox.Show("第三时间段的起始时间格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (!Parameters.IsTime(JsonInterFace.TDSDeviceAdvanceSettingParameter.ThreePeriodTimeEnd))
                    {
                        MessageBox.Show("第三时间段的结束时间格式不正确！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                if (NetWorkClient.ControllerServer.Connected)
                {
                    Dictionary<string, string> ApperiodTimeList = new Dictionary<string, string>();
                    ApperiodTimeList.Add("activeTime1Start", JsonInterFace.TDSDeviceAdvanceSettingParameter.FirstPeriodTimeStart);
                    ApperiodTimeList.Add("activeTime1Ended", JsonInterFace.TDSDeviceAdvanceSettingParameter.FirstPeriodTimeEnd);
                    ApperiodTimeList.Add("activeTime2Start", JsonInterFace.TDSDeviceAdvanceSettingParameter.SecondPeriodTimeStart);
                    ApperiodTimeList.Add("activeTime2Ended", JsonInterFace.TDSDeviceAdvanceSettingParameter.SecoondPeriodTimeEnd);
                    ApperiodTimeList.Add("activeTime3Start", JsonInterFace.TDSDeviceAdvanceSettingParameter.ThreePeriodTimeStart);
                    ApperiodTimeList.Add("activeTime3Ended", JsonInterFace.TDSDeviceAdvanceSettingParameter.ThreePeriodTimeEnd);

                    string DomainFullPathName = string.Empty;
                    string[] DomainFullPathNameTmp = JsonInterFace.TDSDeviceAdvanceSettingParameter.DomainFullPathName.Split(new char[] { '.' });
                    for (int i = 0; i < DomainFullPathNameTmp.Length - 1; i++)
                    {
                        if (DomainFullPathName.Equals(""))
                        {
                            DomainFullPathName = DomainFullPathNameTmp[i];
                        }
                        else
                        {
                            DomainFullPathName += "." + DomainFullPathNameTmp[i];
                        }
                    }
                    Parameters.ConfigType = DeviceType.TD_SCDMA;
                    NetWorkClient.ControllerServer.Send(JsonInterFace.APPeriodTimeConrolRequest(
                                                                                                DomainFullPathName,
                                                                                                JsonInterFace.TDSDeviceAdvanceSettingParameter.DeviceName,
                                                                                                ApperiodTimeList
                                                                                                ));
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("时段控制", ex.Message, ex.StackTrace);
            }
        }

        private void btnTDSFileBrower_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog logFileName = new Microsoft.Win32.OpenFileDialog();
            if ((bool)logFileName.ShowDialog())
            {
                JsonInterFace.TDSDeviceSystemMaintenenceParameter.UpgradeFile = logFileName.FileName;
            }
        }

        private void btnTDSLogFileBrower_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog SelectDir = new System.Windows.Forms.FolderBrowserDialog();

            if (SelectDir.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                JsonInterFace.TDSDeviceSystemMaintenenceParameter.LogFiles = SelectDir.SelectedPath;
            }
        }

        private void btnTDSDownLoadlogs_Click(object sender, RoutedEventArgs e)
        {
            string SelfID = string.Empty;
            string SelfName = string.Empty;
            try
            {
                if (txtTDSLogFilePath.Text == (""))
                {
                    MessageBox.Show("请选择存放AP日志的目标文件夹！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    btnTDSLogFileBrower_Click(sender, e);
                    return;
                }
                else
                {
                    if (!Directory.Exists(txtTDSLogFilePath.Text))
                    {
                        try
                        {
                            Directory.CreateDirectory(txtTDSLogFilePath.Text);
                        }
                        catch (Exception Ex)
                        {
                            Parameters.PrintfLogsExtended("创建存放AP日志的目标文件夹失败", Ex.Message, Ex.StackTrace);
                            MessageBox.Show("请选择存放AP日志的目标文件夹，当前指定的文件路径无效不可用！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }
                }

                if (DeviceType.TD_SCDMA == MainWindow.aDeviceSelected.Model)
                {
                    DownLoadLogsTask(sender, e);
                }
                else
                {
                    MessageBox.Show("目前下载AP日志只支持[LTE系列]及[WCDMA]设备！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void cbTDSParameterKeyName_DropDownClosed(object sender, EventArgs e)
        {
            if (cbTDSParameterKeyName.SelectedIndex > -1)
            {
                cbTDSParameterValue.Items.Clear();
                cbTDSParameterValue.Text = "";
                txtTDSParameterValue.Text = "";
                getParameterKeyValue(txtTDSParameterValue, cbTDSParameterValue, cbTDSParameterKeyName.SelectedIndex);
            }
        }

        private void cbTDSParameterValue_DropDownClosed(object sender, EventArgs e)
        {
            if (cbTDSParameterValue.SelectedIndex > -1)
            {
                txtTDSParameterValue.Text = cbTDSParameterValue.SelectedItem.ToString();
                cbTDSParameterValue.Text = "";
            }
        }

        private void btnTDSSendInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtTDSparameterCommandList.Text.Trim() != "")
                {
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        NetWorkClient.ControllerServer.Send(JsonInterFace.APProjectSettingRequest(
                                                                                                    JsonInterFace.TDSDeviceParameter.DomainFullPathName,
                                                                                                    JsonInterFace.TDSDeviceParameter.DeviceName,
                                                                                                    JsonInterFace.TDSDeviceParameter.IpAddr,
                                                                                                    JsonInterFace.TDSDeviceParameter.Port,
                                                                                                    JsonInterFace.TDSDeviceParameter.InnerType,
                                                                                                    JsonInterFace.TDSDeviceParameter.SN
                                                                                                ));
                    }
                    else
                    {
                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("参数内容为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void miTDSObjectResultClear_Click(object sender, RoutedEventArgs e)
        {
            JsonInterFace.TDSDeviceObjectSettingParameter.ParameterResultValue = string.Empty;
        }

        private void btnTDSSweepFreqPointUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (JsonInterFace.TDSDeviceAdvanceSettingParameter.SN.Equals(""))
                {
                    MessageBox.Show("SN信息不存在！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (JsonInterFace.TDSDeviceAdvanceSettingParameter.IpAddr.Equals(""))
                {
                    MessageBox.Show("IP地址信息不存在！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (JsonInterFace.TDSDeviceAdvanceSettingParameter.Port.Equals(""))
                {
                    MessageBox.Show("AP端口信息不存在！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (JsonInterFace.TDSDeviceAdvanceSettingParameter.InnerType.Equals(""))
                {
                    MessageBox.Show("InnerType信息错误！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (JsonInterFace.TDSDeviceAdvanceSettingParameter.OnLine == "0")
                {
                    MessageBox.Show("设备不在线！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!JsonInterFace.TDSDeviceAdvanceSettingParameter.FrequencyList.Trim().Equals(""))
                {
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        NetWorkClient.ControllerServer.Send(
                                                            JsonInterFace.APSetSonEarfcnRequest(
                                                                                                JsonInterFace.TDSDeviceAdvanceSettingParameter.DomainFullPathName,
                                                                                                JsonInterFace.TDSDeviceAdvanceSettingParameter.DeviceName,
                                                                                                JsonInterFace.TDSDeviceAdvanceSettingParameter.IpAddr,
                                                                                                int.Parse(JsonInterFace.TDSDeviceAdvanceSettingParameter.Port),
                                                                                                JsonInterFace.TDSDeviceAdvanceSettingParameter.InnerType,
                                                                                                JsonInterFace.TDSDeviceAdvanceSettingParameter.SN,
                                                                                                JsonInterFace.TDSDeviceAdvanceSettingParameter.FrequencyList
                                                                                                )
                                                            );
                    }
                }
                else
                {
                    MessageBox.Show("请输入扫频信息！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("扫频信息设置：", ex.Message);
            }
        }
        private void GetUpload()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                        {
                            if (JsonInterFace.APATTributesLists[i].FullName == FullName)
                            {
                                string DomainFullPathName = string.Empty;
                                string[] DomainFullNameTmp = JsonInterFace.APATTributesLists[i].FullName.Split(new char[] { '.' });

                                for (int j = 0; j < DomainFullNameTmp.Length - 1; j++)
                                {
                                    if (DomainFullPathName.Trim().Equals(""))
                                    {
                                        DomainFullPathName = DomainFullNameTmp[j];
                                    }
                                    else
                                    {
                                        DomainFullPathName += "." + DomainFullNameTmp[j];
                                    }
                                }
                                NetWorkClient.ControllerServer.Send(JsonInterFace.Get_Upload_Request(
                                                                                                    DomainFullPathName,
                                                                                                    JsonInterFace.APATTributesLists[i].SelfName,
                                                                                                    JsonInterFace.APATTributesLists[i].IpAddr,
                                                                                                    JsonInterFace.APATTributesLists[i].Port,
                                                                                                    JsonInterFace.APATTributesLists[i].InnerType,
                                                                                                    JsonInterFace.APATTributesLists[i].SN
                                                                                                ));
                                break;
                            }

                        }
                    }
                    else
                    {
                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                });
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("同步参考源查询", ex.Message, ex.StackTrace);
            }
        }
        private void GetSonEarfcn()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                        {
                            if (JsonInterFace.APATTributesLists[i].FullName == FullName)
                            {
                                string DomainFullPathName = string.Empty;
                                string[] DomainFullNameTmp = JsonInterFace.APATTributesLists[i].FullName.Split(new char[] { '.' });

                                for (int j = 0; j < DomainFullNameTmp.Length - 1; j++)
                                {
                                    if (DomainFullPathName.Trim().Equals(""))
                                    {
                                        DomainFullPathName = DomainFullNameTmp[j];
                                    }
                                    else
                                    {
                                        DomainFullPathName += "." + DomainFullNameTmp[j];
                                    }
                                }
                                NetWorkClient.ControllerServer.Send(JsonInterFace.Get_Son_Earfcn_Request(
                                                                                                    DomainFullPathName,
                                                                                                    JsonInterFace.APATTributesLists[i].SelfName,
                                                                                                    JsonInterFace.APATTributesLists[i].IpAddr,
                                                                                                    JsonInterFace.APATTributesLists[i].Port,
                                                                                                    JsonInterFace.APATTributesLists[i].InnerType,
                                                                                                    JsonInterFace.APATTributesLists[i].SN
                                                                                                ));
                                break;
                            }

                        }
                    }
                    else
                    {
                        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                });
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("扫频频点查询", ex.Message, ex.StackTrace);
            }
        }

        private void rdbTDSFreqAuto_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)rdbTDSFreqAuto.IsChecked)
            {
                rdbTDSFreqManul.IsChecked = false;
            }
            else
            {
                if (!(bool)rdbTDSFreqManul.IsChecked)
                {
                    rdbTDSFreqAuto.IsChecked = true;
                }
            }
        }

        private void rdbTDSFreqManul_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)rdbTDSFreqManul.IsChecked)
            {
                rdbTDSFreqAuto.IsChecked = false;
            }
            else
            {
                if (!(bool)rdbTDSFreqAuto.IsChecked)
                {
                    rdbTDSFreqManul.IsChecked = true;
                }
            }
        }

        private void txtUpdateLogsShow_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextDownToLastLine(sender);
        }

        /// <summary>
        /// 内容向上滚动
        /// </summary>
        /// <param name="sender"></param>
        private void TextDownToLastLine(object sender)
        {
            try
            {
                if (sender != null)
                {
                    if (sender is TextBox)
                    {
                        (sender as TextBox).ScrollToLine((sender as TextBox).LineCount - 1);
                    }
                }
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended(Ex.Message, Ex.StackTrace);
            }
        }

        private void txtDownLoadLogsShow_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextDownToLastLine(sender);
        }
    }
}
