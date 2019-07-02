namespace Language_EN
{
    #region 公共事件提示标题
    public class WindowEventTitleContent
    {
        private string ttitleWarning = "Warning";
        private string titleError = "Error";
        private string titleInfomation = "Message";

        public string TtitleWarning
        {
            get
            {
                return ttitleWarning;
            }

            set
            {
                ttitleWarning = value;
            }
        }

        public string TitleError
        {
            get
            {
                return titleError;
            }

            set
            {
                titleError = value;
            }
        }

        public string TitleInfomation
        {
            get
            {
                return titleInfomation;
            }

            set
            {
                titleInfomation = value;
            }
        }
    }
    #endregion

    #region 登录界面配置
    public class LoginWindow
    {
        private string title = "Unkown";
        private string userName = "Unkown";
        private string passWord = "Unkown";
        private string server = "Unkown";
        private string login = "Unkown";
        private string reSet = "Unkown";

        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                title = value;
            }
        }

        public string UserName
        {
            get
            {
                return userName;
            }

            set
            {
                userName = value;
            }
        }

        public string PassWord
        {
            get
            {
                return passWord;
            }

            set
            {
                passWord = value;
            }
        }

        public string Login
        {
            get
            {
                return login;
            }

            set
            {
                login = value;
            }
        }

        public string ReSet
        {
            get
            {
                return reSet;
            }

            set
            {
                reSet = value;
            }
        }

        public string Server
        {
            get
            {
                return server;
            }

            set
            {
                server = value;
            }
        }
    }
    #endregion

    #region 登录界面事件提示信息

    public class LoginWindowEventContents
    {
        private string loginFailedTitle = "登录失败";
        private string loginNull = "服务器无法连接!";
        private string loginDeny = "服务器无法连接!(拒绝登录)";
        private string nullUserName = "Please enter the username！";
        private string nullServerHost = "请输入服务器信息！";
        private string errorServerHostFormat = "服务器地址格式不正确！";

        public string LoginFailedTitle
        {
            get
            {
                return loginFailedTitle;
            }

            set
            {
                loginFailedTitle = value;
            }
        }

        public string LoginNull
        {
            get
            {
                return loginNull;
            }

            set
            {
                loginNull = value;
            }
        }

        public string LoginDeny
        {
            get
            {
                return loginDeny;
            }

            set
            {
                loginDeny = value;
            }
        }

        public string NullUserName
        {
            get
            {
                return nullUserName;
            }

            set
            {
                nullUserName = value;
            }
        }

        public string NullServerHost
        {
            get
            {
                return nullServerHost;
            }

            set
            {
                nullServerHost = value;
            }
        }

        public string ErrorServerHostFormat
        {
            get
            {
                return errorServerHostFormat;
            }

            set
            {
                errorServerHostFormat = value;
            }
        }
    }
    #endregion

    #region 主界面控制信息
    public class SystemConrol
    {
        private string title = "智能通讯管控管理系统";
        private string mExit = "退出(X)";

        public string MExit
        {
            get
            {
                return mExit;
            }

            set
            {
                mExit = value;
            }
        }

        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                title = value;
            }
        }
    }
    #endregion

    #region 主界面菜单
    public class MainMenu
    {
        private string mFeature = "功能(P)";
        private string mFeatureDem = "设备管理";
        private string mFeatureNam = "特殊名单管理";
        private string mFeatureDam = "历史记录管理";
        private string mFeatureLgm = "系统日志管理";

        private string mSetting = "设置(S)";
        private string mSettingUnm = "用户管理";
        private string mSettingDmm = "域管理";
        private string mSettingSym = "系统管理";
        private string mSettingAds = "高级管理";
        private string operationsCaption = "选项...";

        private string mView = "显示(V)";
        private string mViewScw = "捕号窗口";
        private string mViewBlw = "黑名单窗口";
        private string mCallRdw = "通话记录窗口";
        private string mSMSRw = "短信记录窗口";
        private string mViewStw = "状态窗口";
        private string mDefaultWindowCaption = "默认窗口";

        private string mHelp = "帮助(H)";
        private string mHelpHlp = "帮助";
        private string mHelpAbt = "关于";

        public string MFeature
        {
            get
            {
                return mFeature;
            }

            set
            {
                mFeature = value;
            }
        }

        public string MFeatureDem
        {
            get
            {
                return mFeatureDem;
            }

            set
            {
                mFeatureDem = value;
            }
        }

        public string MFeatureNam
        {
            get
            {
                return mFeatureNam;
            }

            set
            {
                mFeatureNam = value;
            }
        }

        public string MFeatureDam
        {
            get
            {
                return mFeatureDam;
            }

            set
            {
                mFeatureDam = value;
            }
        }

        public string MSetting
        {
            get
            {
                return mSetting;
            }

            set
            {
                mSetting = value;
            }
        }

        public string MSettingUnm
        {
            get
            {
                return mSettingUnm;
            }

            set
            {
                mSettingUnm = value;
            }
        }

        public string MSettingDmm
        {
            get
            {
                return mSettingDmm;
            }

            set
            {
                mSettingDmm = value;
            }
        }

        public string MSettingSym
        {
            get
            {
                return mSettingSym;
            }

            set
            {
                mSettingSym = value;
            }
        }

        public string MSettingAds
        {
            get
            {
                return mSettingAds;
            }

            set
            {
                mSettingAds = value;
            }
        }

        public string MView
        {
            get
            {
                return mView;
            }

            set
            {
                mView = value;
            }
        }

        public string MViewScw
        {
            get
            {
                return mViewScw;
            }

            set
            {
                mViewScw = value;
            }
        }

        public string MViewBlw
        {
            get
            {
                return mViewBlw;
            }

            set
            {
                mViewBlw = value;
            }
        }

        public string MViewStw
        {
            get
            {
                return mViewStw;
            }

            set
            {
                mViewStw = value;
            }
        }

        public string MHelp
        {
            get
            {
                return mHelp;
            }

            set
            {
                mHelp = value;
            }
        }

        public string MHelpHlp
        {
            get
            {
                return mHelpHlp;
            }

            set
            {
                mHelpHlp = value;
            }
        }

        public string MHelpAbt
        {
            get
            {
                return mHelpAbt;
            }

            set
            {
                mHelpAbt = value;
            }
        }

        public string OperationsCaption
        {
            get
            {
                return operationsCaption;
            }

            set
            {
                operationsCaption = value;
            }
        }

        public string MDefaultWindowCaption
        {
            get
            {
                return mDefaultWindowCaption;
            }

            set
            {
                mDefaultWindowCaption = value;
            }
        }

        public string MFeatureLgm
        {
            get
            {
                return mFeatureLgm;
            }

            set
            {
                mFeatureLgm = value;
            }
        }

        public string MCallRdw
        {
            get
            {
                return mCallRdw;
            }

            set
            {
                mCallRdw = value;
            }
        }

        public string MSMSRw
        {
            get
            {
                return mSMSRw;
            }

            set
            {
                mSMSRw = value;
            }
        }
    }
    #endregion

    #region 设备及域名列表
    public class DeviceTree
    {
        public DeviceTree_ContentMenu contentMenu;
        public TreeViewContent treeContent;
        public DeviceTree()
        {
            contentMenu = new DeviceTree_ContentMenu();
            TreeViewContent treeContent = new TreeViewContent();
        }
        public class DeviceTree_ContentMenu
        {
            private string deviceActive = "激活";
            private string deviceUnActive = "去激活";
            private string deviceReboot = "重启";
            private string treeNodeExpande = "展开/收起";
            private string statisticalInfo = "统计信息";
            private string deviceManage = "设备管理";
            private string deviceInformation = "详细信息";
            private string scannerInfo = "扫频结果";
            private string addNode = "添加";
            private string deleteNode = "删除";
            private string edit = "修改";
            private string whiteListSelfLearningCaption = "白名单自学习";
            private string whiteListSelfLearningStartCaption = "启动";
            private string whiteListSelfLearningStopCaption = "停止";
            private string updateNodes = "刷新";
            private string reloadDeviceMenulCaption = "重载";

            public string DeviceActive
            {
                get
                {
                    return deviceActive;
                }

                set
                {
                    deviceActive = value;
                }
            }

            public string DeviceReboot
            {
                get
                {
                    return deviceReboot;
                }

                set
                {
                    deviceReboot = value;
                }
            }

            public string TreeNodeExpande
            {
                get
                {
                    return treeNodeExpande;
                }

                set
                {
                    treeNodeExpande = value;
                }
            }

            public string StatisticalInfo
            {
                get
                {
                    return statisticalInfo;
                }

                set
                {
                    statisticalInfo = value;
                }
            }

            public string DeviceManage
            {
                get
                {
                    return deviceManage;
                }

                set
                {
                    deviceManage = value;
                }
            }

            public string DeviceInformation
            {
                get
                {
                    return deviceInformation;
                }

                set
                {
                    deviceInformation = value;
                }
            }

            public string ScannerInfo
            {
                get
                {
                    return scannerInfo;
                }

                set
                {
                    scannerInfo = value;
                }
            }

            public string AddNode
            {
                get
                {
                    return addNode;
                }

                set
                {
                    addNode = value;
                }
            }

            public string DeleteNode
            {
                get
                {
                    return deleteNode;
                }

                set
                {
                    deleteNode = value;
                }
            }

            public string Edit
            {
                get
                {
                    return edit;
                }

                set
                {
                    edit = value;
                }
            }

            public string UpdateNodes
            {
                get
                {
                    return updateNodes;
                }

                set
                {
                    updateNodes = value;
                }
            }

            public string DeviceUnActive
            {
                get
                {
                    return deviceUnActive;
                }

                set
                {
                    deviceUnActive = value;
                }
            }

            public string WhiteListSelfLearningCaption
            {
                get
                {
                    return whiteListSelfLearningCaption;
                }

                set
                {
                    whiteListSelfLearningCaption = value;
                }
            }

            public string WhiteListSelfLearningStartCaption
            {
                get
                {
                    return whiteListSelfLearningStartCaption;
                }

                set
                {
                    whiteListSelfLearningStartCaption = value;
                }
            }

            public string WhiteListSelfLearningStopCaption
            {
                get
                {
                    return whiteListSelfLearningStopCaption;
                }

                set
                {
                    whiteListSelfLearningStopCaption = value;
                }
            }

            public string ReloadDeviceMenulCaption
            {
                get
                {
                    return reloadDeviceMenulCaption;
                }

                set
                {
                    reloadDeviceMenulCaption = value;
                }
            }
        }

        public class TreeViewContent
        {
            private string rootNode = "所有设备";

            public string RootNode
            {
                get
                {
                    return rootNode;
                }

                set
                {
                    rootNode = value;
                }
            }
        }
    }
    #endregion

    #region 主界面捕号显示区
    public class UEScannerReports
    {
        public ScannerInfoSearch scannerInfoSearchContent;
        public ScannerInfoDataGrid scannerInfoGridContent;

        public UEScannerReports()
        {
            scannerInfoSearchContent = new ScannerInfoSearch();
            scannerInfoGridContent = new ScannerInfoDataGrid();
        }
        public class ScannerInfoSearch
        {
            private string iMSILable = "IMSI";
            private string timeStartLable = "开始时间";
            private string toLable = "至";
            private string timeEndLable = "结束时间";
            private string deviceNameLable = "设备";
            private string userTypeLable = "用户类型";
            private string searchLable = "过滤";

            public string IMSILable
            {
                get
                {
                    return iMSILable;
                }

                set
                {
                    iMSILable = value;
                }
            }

            public string TimeStartLable
            {
                get
                {
                    return timeStartLable;
                }

                set
                {
                    timeStartLable = value;
                }
            }

            public string ToLable
            {
                get
                {
                    return toLable;
                }

                set
                {
                    toLable = value;
                }
            }

            public string TimeEndLable
            {
                get
                {
                    return timeEndLable;
                }

                set
                {
                    timeEndLable = value;
                }
            }

            public string DeviceNameLable
            {
                get
                {
                    return deviceNameLable;
                }

                set
                {
                    deviceNameLable = value;
                }
            }

            public string UserTypeLable
            {
                get
                {
                    return userTypeLable;
                }

                set
                {
                    userTypeLable = value;
                }
            }

            public string SearchLable
            {
                get
                {
                    return searchLable;
                }

                set
                {
                    searchLable = value;
                }
            }
        }
        public class ScannerInfoDataGrid
        {
            private string iDHead = "编号";
            private string iMSIHead = "IMSI";
            private string dTimeHead = "时间";
            private string userTypeHead = "用户类型";
            private string tMSIHead = "TMSI";
            private string iMEIHead = "IMEI";
            private string intensityHead = "信号";
            private string operatorsHead = "运营商";
            private string districtHead = "号码归属地";
            private string deviceNameHead = "设备";
            private string aliasNameHead = "别名";

            public string IDHead
            {
                get
                {
                    return iDHead;
                }

                set
                {
                    iDHead = value;
                }
            }

            public string IMSIHead
            {
                get
                {
                    return iMSIHead;
                }

                set
                {
                    iMSIHead = value;
                }
            }

            public string DTimeHead
            {
                get
                {
                    return dTimeHead;
                }

                set
                {
                    dTimeHead = value;
                }
            }

            public string UserTypeHead
            {
                get
                {
                    return userTypeHead;
                }

                set
                {
                    userTypeHead = value;
                }
            }

            public string TMSIHead
            {
                get
                {
                    return tMSIHead;
                }

                set
                {
                    tMSIHead = value;
                }
            }

            public string IMEIHead
            {
                get
                {
                    return iMEIHead;
                }

                set
                {
                    iMEIHead = value;
                }
            }

            public string IntensityHead
            {
                get
                {
                    return intensityHead;
                }

                set
                {
                    intensityHead = value;
                }
            }

            public string OperatorsHead
            {
                get
                {
                    return operatorsHead;
                }

                set
                {
                    operatorsHead = value;
                }
            }

            public string DistrictHead
            {
                get
                {
                    return districtHead;
                }

                set
                {
                    districtHead = value;
                }
            }

            public string DeviceNameHead
            {
                get
                {
                    return deviceNameHead;
                }

                set
                {
                    deviceNameHead = value;
                }
            }

            public string AliasNameHead
            {
                get
                {
                    return aliasNameHead;
                }

                set
                {
                    aliasNameHead = value;
                }
            }
        }
    }
    #endregion

    #region 主界面黑白名单显示区
    public class UEBWLists
    {
        public BlcakListInfoSearch BlackListInfoSearchContent;
        public BlcakListInfoDataGrid BlackListInfoGridContent;

        public UEBWLists()
        {
            BlackListInfoSearchContent = new BlcakListInfoSearch();
            BlackListInfoGridContent = new BlcakListInfoDataGrid();
        }
        public class BlcakListInfoSearch
        {
            private string iMSILable = "IMSI";
            private string timeStartLable = "开始时间";
            private string toLable = "至";
            private string timeEndLable = "结束时间";
            private string deviceNameLable = "设备";
            private string userTypeLable = "用户类型";
            private string callerlocLable = "归属地";
            private string searchLable = "过滤";

            public string IMSILable
            {
                get
                {
                    return iMSILable;
                }

                set
                {
                    iMSILable = value;
                }
            }

            public string TimeStartLable
            {
                get
                {
                    return timeStartLable;
                }

                set
                {
                    timeStartLable = value;
                }
            }

            public string ToLable
            {
                get
                {
                    return toLable;
                }

                set
                {
                    toLable = value;
                }
            }

            public string TimeEndLable
            {
                get
                {
                    return timeEndLable;
                }

                set
                {
                    timeEndLable = value;
                }
            }

            public string DeviceNameLable
            {
                get
                {
                    return deviceNameLable;
                }

                set
                {
                    deviceNameLable = value;
                }
            }

            public string UserTypeLable
            {
                get
                {
                    return userTypeLable;
                }

                set
                {
                    userTypeLable = value;
                }
            }

            public string SearchLable
            {
                get
                {
                    return searchLable;
                }

                set
                {
                    searchLable = value;
                }
            }

            public string CallerlocLable
            {
                get
                {
                    return callerlocLable;
                }

                set
                {
                    callerlocLable = value;
                }
            }
        }
        public class BlcakListInfoDataGrid
        {
            private string iMSIHead = "IMSI";
            private string dTimeHead = "最近接入时间";
            private string tMSIHead = "TMSI";
            private string iMEIHead = "IMEI";
            private string intensityHead = "信号";
            private string operatorsHead = "运营商";
            private string districtHead = "号码归属地";
            private string deviceNameHead = "设备";

            public string IMSIHead
            {
                get
                {
                    return iMSIHead;
                }

                set
                {
                    iMSIHead = value;
                }
            }

            public string DTimeHead
            {
                get
                {
                    return dTimeHead;
                }

                set
                {
                    dTimeHead = value;
                }
            }

            public string TMSIHead
            {
                get
                {
                    return tMSIHead;
                }

                set
                {
                    tMSIHead = value;
                }
            }

            public string IMEIHead
            {
                get
                {
                    return iMEIHead;
                }

                set
                {
                    iMEIHead = value;
                }
            }

            public string IntensityHead
            {
                get
                {
                    return intensityHead;
                }

                set
                {
                    intensityHead = value;
                }
            }

            public string OperatorsHead
            {
                get
                {
                    return operatorsHead;
                }

                set
                {
                    operatorsHead = value;
                }
            }

            public string DistrictHead
            {
                get
                {
                    return districtHead;
                }

                set
                {
                    districtHead = value;
                }
            }

            public string DeviceNameHead
            {
                get
                {
                    return deviceNameHead;
                }

                set
                {
                    deviceNameHead = value;
                }
            }
        }
    }

    #endregion

    #region 主界面状态显示区
    public class MainStatusInformation
    {
        private string dTimeHead = "时间";
        private string objectHead = "详细信息";
        private string actionHead = "动作";
        private string otherHead = "其它";

        public string DTimeHead
        {
            get
            {
                return dTimeHead;
            }

            set
            {
                dTimeHead = value;
            }
        }

        public string ObjectHead
        {
            get
            {
                return objectHead;
            }

            set
            {
                objectHead = value;
            }
        }

        public string ActionHead
        {
            get
            {
                return actionHead;
            }

            set
            {
                actionHead = value;
            }
        }

        public string OtherHead
        {
            get
            {
                return otherHead;
            }

            set
            {
                otherHead = value;
            }
        }
    }
    #endregion

    #region 设备管理显示
    public class Device_managerWindow
    {
        private string deviceManagement = "设备管理";
        public string DeviceManagement
        {
            get { return deviceManagement; }
            set { deviceManagement = value; }
        }
        #region 设备信息参数
        private string deviceInfo = "设备信息";
        private string domainNameCaption = "域名";
        private string stationCaption = "站点";
        private string modeCaption = "制式";
        private string ipCaption = "IP设置";
        private string serialNumberCaption = "序列号";
        private string deviceDistinguishCaption = "设备识别";
        private string deviceNameCaption = "设备名";
        private string stticIPCaption = "固定IP";
        private string dynamicIPCaption = "动态IP";
        private string add = "添加";
        private string delete = "删除";
        private string update = "更新";
        private string cancel = "取消";
        private string deviceInfo_DeviceName = "设备名";
        private string deviceInfo_IPAddress = "IP";
        private string deviceInfo_SerialNumber = "序列号";
        #endregion

        #region 小区信息参数
        private string cellNeighInfo = "小区信息";
        private string plmnCaption = "PLMN";
        private string freqPointCaption = "频点";
        private string bandWidthCaption = "带宽";
        private string powerDownCaption = "功率衰减";
        private string freqMethodCaption = "选频方式";
        private string operatorNameCaption = "运营商";
        private string disturbCodeCaption = "扰码";
        private string tACorLACCaption = "TAC/LAC";
        private string cycleCaption = "周期";
        private string lblsecondCaption = "秒";
        private string startupModeCaption = "启动方式";
        private string automaticCaption = "自动";
        private string manualCaption = "手动";
        private string deviceUpdateCaption = "更新";
        private string morePLMNSListCaption = "PLMN";
        private string perierFreqListCaption = "频点";
        private string otherPlmnSettingCaption = "其它PLMN更新";
        private string periodFreqSettingCaption = "周期频点更新";

        public string CellNeighInfo
        {
            get
            {
                return cellNeighInfo;
            }

            set
            {
                cellNeighInfo = value;
            }
        }

        public string PlmnCaption
        {
            get
            {
                return plmnCaption;
            }

            set
            {
                plmnCaption = value;
            }
        }

        public string FreqPointCaption
        {
            get
            {
                return freqPointCaption;
            }

            set
            {
                freqPointCaption = value;
            }
        }

        public string BandWidthCaption
        {
            get
            {
                return bandWidthCaption;
            }

            set
            {
                bandWidthCaption = value;
            }
        }

        public string PowerDownCaption
        {
            get
            {
                return powerDownCaption;
            }

            set
            {
                powerDownCaption = value;
            }
        }

        public string FreqMethodCaption
        {
            get
            {
                return freqMethodCaption;
            }

            set
            {
                freqMethodCaption = value;
            }
        }

        public string OperatorNameCaption
        {
            get
            {
                return operatorNameCaption;
            }

            set
            {
                operatorNameCaption = value;
            }
        }

        public string DisturbCodeCaption
        {
            get
            {
                return disturbCodeCaption;
            }

            set
            {
                disturbCodeCaption = value;
            }
        }

        public string TACorLACCaption
        {
            get
            {
                return tACorLACCaption;
            }

            set
            {
                tACorLACCaption = value;
            }
        }

        public string CycleCaption
        {
            get
            {
                return cycleCaption;
            }

            set
            {
                cycleCaption = value;
            }
        }

        public string LblsecondCaption
        {
            get
            {
                return lblsecondCaption;
            }

            set
            {
                lblsecondCaption = value;
            }
        }

        public string StartupModeCaption
        {
            get
            {
                return startupModeCaption;
            }

            set
            {
                startupModeCaption = value;
            }
        }

        public string AutomaticCaption
        {
            get
            {
                return automaticCaption;
            }

            set
            {
                automaticCaption = value;
            }
        }

        public string ManualCaption
        {
            get
            {
                return manualCaption;
            }

            set
            {
                manualCaption = value;
            }
        }

        public string DeviceUpdateCaption
        {
            get
            {
                return deviceUpdateCaption;
            }

            set
            {
                deviceUpdateCaption = value;
            }
        }
        #endregion

        #region 高级设置参数
        private string advancedSettingCaption = "高级设置";
        private string sweepFreqPointCaption = "扫频频点";
        private string gpsCaption = "GPS";
        private string configureCaption = "配置";
        private string unConfigureCaption = "配置";
        private string freqOffsetSettingCaption = "频偏设置";
        private string nTPCaption = "NTP";
        private string priorityCaption = "优先级";
        private string synchroSourceCaption = "同步源";
        private string rbGPSCaption = "GPS";
        private string emptyMouthCaption = "空口";
        private string appointVillageCaption = "指定小区";
        private string rbYesCaption = "是";
        private string rbNoCaption = "否";
        private string startTimeCaption = "一时间段";
        private string secondPeriodTimeCaption = "二时间段";
        private string threePeriodTimeCaption = "三时间段";
        private string freqPointUpdateCaption = "更新";
        private string gPSFreqUpdateCaption = "更新";
        private string nTPUpdateCaption = "更新";
        private string syncSourceUpdateCaption = "更新";
        private string lteTimeUpdateCaption = "更新";
        private string wCDMAFreqUpdateCaption = "更新";
        private string wCDMANTPUpdateCaption = "更新";
        private string wCDMAUpdateTimeCaption = "更新";

        public string AdvancedSettingCaption
        {
            get
            {
                return advancedSettingCaption;
            }

            set
            {
                advancedSettingCaption = value;
            }
        }

        public string SweepFreqPointCaption
        {
            get
            {
                return sweepFreqPointCaption;
            }

            set
            {
                sweepFreqPointCaption = value;
            }
        }

        public string GpsCaption
        {
            get
            {
                return gpsCaption;
            }

            set
            {
                gpsCaption = value;
            }
        }

        public string ConfigureCaption
        {
            get
            {
                return configureCaption;
            }

            set
            {
                configureCaption = value;
            }
        }

        public string UnConfigureCaption
        {
            get
            {
                return unConfigureCaption;
            }

            set
            {
                unConfigureCaption = value;
            }
        }

        public string FreqOffsetSettingCaption
        {
            get
            {
                return freqOffsetSettingCaption;
            }

            set
            {
                freqOffsetSettingCaption = value;
            }
        }

        public string NTPCaption
        {
            get
            {
                return nTPCaption;
            }

            set
            {
                nTPCaption = value;
            }
        }

        public string PriorityCaption
        {
            get
            {
                return priorityCaption;
            }

            set
            {
                priorityCaption = value;
            }
        }

        public string SynchroSourceCaption
        {
            get
            {
                return synchroSourceCaption;
            }

            set
            {
                synchroSourceCaption = value;
            }
        }

        public string RbGPSCaption
        {
            get
            {
                return rbGPSCaption;
            }

            set
            {
                rbGPSCaption = value;
            }
        }

        public string EmptyMouthCaption
        {
            get
            {
                return emptyMouthCaption;
            }

            set
            {
                emptyMouthCaption = value;
            }
        }

        public string AppointVillageCaption
        {
            get
            {
                return appointVillageCaption;
            }

            set
            {
                appointVillageCaption = value;
            }
        }

        public string RbYesCaption
        {
            get
            {
                return rbYesCaption;
            }

            set
            {
                rbYesCaption = value;
            }
        }

        public string RbNoCaption
        {
            get
            {
                return rbNoCaption;
            }

            set
            {
                rbNoCaption = value;
            }
        }

        public string StartTimeCaption
        {
            get
            {
                return startTimeCaption;
            }

            set
            {
                startTimeCaption = value;
            }
        }

        #endregion

        #region 系统维护参数
        private string sysSetting = "系统维护";
        private string file = "升级文件";
        private string logFile = "日志文件";
        private string updateFile = "升级";
        private string getlog = "获取LOG";
        private string versionCaption = "升级包版本";
        public string SysSetting
        {
            get
            {
                return sysSetting;
            }

            set
            {
                sysSetting = value;
            }
        }
        public string File
        {
            get
            {
                return file;
            }

            set
            {
                file = value;
            }
        }
        public string LogFile
        {
            get
            {
                return logFile;
            }

            set
            {
                logFile = value;
            }
        }
        public string UpdateFile
        {
            get
            {
                return updateFile;
            }

            set
            {
                updateFile = value;
            }
        }
        public string Getlog
        {
            get
            {
                return getlog;
            }

            set
            {
                getlog = value;
            }
        }

        public string VersionCaption
        {
            get
            {
                return versionCaption;
            }

            set
            {
                versionCaption = value;
            }
        }
        #endregion

        #region 工程设置参数
        private string projectSet = "工程设置";
        private string updatePragram = "更新参数";
        private string sendInfo = "直接发送命令";
        private string pragramName = "参数名";
        private string pragramValue = "参数值";
        public string ProjectSet
        {
            get
            {
                return projectSet;
            }

            set
            {
                projectSet = value;
            }
        }
        public string UpdatePragram
        {
            get
            {
                return updatePragram;
            }

            set
            {
                updatePragram = value;
            }
        }
        public string SendInfo
        {
            get
            {
                return sendInfo;
            }

            set
            {
                sendInfo = value;
            }
        }
        public string PragramName
        {
            get
            {
                return pragramName;
            }

            set
            {
                pragramName = value;
            }
        }
        public string PragramValue
        {
            get
            {
                return pragramValue;
            }

            set
            {
                pragramValue = value;
            }
        }

        public string DomainNameCaption
        {
            get
            {
                return domainNameCaption;
            }

            set
            {
                domainNameCaption = value;
            }
        }

        public string StationCaption
        {
            get
            {
                return stationCaption;
            }

            set
            {
                stationCaption = value;
            }
        }

        public string ModeCaption
        {
            get
            {
                return modeCaption;
            }

            set
            {
                modeCaption = value;
            }
        }

        public string IpCaption
        {
            get
            {
                return ipCaption;
            }

            set
            {
                ipCaption = value;
            }
        }

        public string SerialNumberCaption
        {
            get
            {
                return serialNumberCaption;
            }

            set
            {
                serialNumberCaption = value;
            }
        }

        public string DeviceDistinguishCaption
        {
            get
            {
                return deviceDistinguishCaption;
            }

            set
            {
                deviceDistinguishCaption = value;
            }
        }

        public string DeviceNameCaption
        {
            get
            {
                return deviceNameCaption;
            }

            set
            {
                deviceNameCaption = value;
            }
        }

        public string StticIPCaption
        {
            get
            {
                return stticIPCaption;
            }

            set
            {
                stticIPCaption = value;
            }
        }

        public string DynamicIPCaption
        {
            get
            {
                return dynamicIPCaption;
            }

            set
            {
                dynamicIPCaption = value;
            }
        }

        public string DeviceInfo_DeviceName
        {
            get
            {
                return deviceInfo_DeviceName;
            }

            set
            {
                deviceInfo_DeviceName = value;
            }
        }

        public string DeviceInfo_IPAddress
        {
            get
            {
                return deviceInfo_IPAddress;
            }

            set
            {
                deviceInfo_IPAddress = value;
            }
        }

        public string DeviceInfo_SerialNumber
        {
            get
            {
                return deviceInfo_SerialNumber;
            }

            set
            {
                deviceInfo_SerialNumber = value;
            }
        }

        public string Add
        {
            get
            {
                return add;
            }

            set
            {
                add = value;
            }
        }

        public string Delete
        {
            get
            {
                return delete;
            }

            set
            {
                delete = value;
            }
        }

        public string Update
        {
            get
            {
                return update;
            }

            set
            {
                update = value;
            }
        }

        public string Cancel
        {
            get
            {
                return cancel;
            }

            set
            {
                cancel = value;
            }
        }

        public string DeviceInfo
        {
            get
            {
                return deviceInfo;
            }

            set
            {
                deviceInfo = value;
            }
        }

        public string MorePLMNSListCaption
        {
            get
            {
                return morePLMNSListCaption;
            }

            set
            {
                morePLMNSListCaption = value;
            }
        }

        public string PerierFreqListCaption
        {
            get
            {
                return perierFreqListCaption;
            }

            set
            {
                perierFreqListCaption = value;
            }
        }

        public string OtherPlmnSettingCaption
        {
            get
            {
                return otherPlmnSettingCaption;
            }

            set
            {
                otherPlmnSettingCaption = value;
            }
        }

        public string PeriodFreqSettingCaption
        {
            get
            {
                return periodFreqSettingCaption;
            }

            set
            {
                periodFreqSettingCaption = value;
            }
        }

        public string FreqPointUpdateCaption
        {
            get
            {
                return freqPointUpdateCaption;
            }

            set
            {
                freqPointUpdateCaption = value;
            }
        }

        public string GPSFreqUpdateCaption
        {
            get
            {
                return gPSFreqUpdateCaption;
            }

            set
            {
                gPSFreqUpdateCaption = value;
            }
        }

        public string NTPUpdateCaption
        {
            get
            {
                return nTPUpdateCaption;
            }

            set
            {
                nTPUpdateCaption = value;
            }
        }

        public string SyncSourceUpdateCaption
        {
            get
            {
                return syncSourceUpdateCaption;
            }

            set
            {
                syncSourceUpdateCaption = value;
            }
        }

        public string LteTimeUpdateCaption
        {
            get
            {
                return lteTimeUpdateCaption;
            }

            set
            {
                lteTimeUpdateCaption = value;
            }
        }

        public string WCDMAFreqUpdateCaption
        {
            get
            {
                return wCDMAFreqUpdateCaption;
            }

            set
            {
                wCDMAFreqUpdateCaption = value;
            }
        }

        public string WCDMANTPUpdateCaption
        {
            get
            {
                return wCDMANTPUpdateCaption;
            }

            set
            {
                wCDMANTPUpdateCaption = value;
            }
        }

        public string WCDMAUpdateTimeCaption
        {
            get
            {
                return wCDMAUpdateTimeCaption;
            }

            set
            {
                wCDMAUpdateTimeCaption = value;
            }
        }

        public string SecondPeriodTimeCaption
        {
            get
            {
                return secondPeriodTimeCaption;
            }

            set
            {
                secondPeriodTimeCaption = value;
            }
        }

        public string ThreePeriodTimeCaption
        {
            get
            {
                return threePeriodTimeCaption;
            }

            set
            {
                threePeriodTimeCaption = value;
            }
        }

        #endregion
    }
    #endregion

    #region 设备列表(域)操作对话框
    public class DeviceTreeOperationDialog
    {
        private string title = "添加域名或设备名称";
        private string stationSettingCaption = "站点设置";
        private string nodeTypeCaption = "[是/否]站点";
        private string domainNameCaption = "域名";
        private string desCaption = "备注";
        private string enter = "确定";
        private string cancel = "取消";

        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                title = value;
            }
        }

        public string DomainNameCaption
        {
            get
            {
                return domainNameCaption;
            }

            set
            {
                domainNameCaption = value;
            }
        }

        public string DesCaption
        {
            get
            {
                return desCaption;
            }

            set
            {
                desCaption = value;
            }
        }

        public string Enter
        {
            get
            {
                return enter;
            }

            set
            {
                enter = value;
            }
        }

        public string Cancel
        {
            get
            {
                return cancel;
            }

            set
            {
                cancel = value;
            }
        }

        public string NodeTypeCaption
        {
            get
            {
                return nodeTypeCaption;
            }

            set
            {
                nodeTypeCaption = value;
            }
        }

        public string StationSettingCaption
        {
            get
            {
                return stationSettingCaption;
            }

            set
            {
                stationSettingCaption = value;
            }
        }
    }
    #endregion

    #region 设备统计信息
    public class DeviceStatistion
    {
        private string title = "统计信息";
        private string bootSituationTabHead = "开机情况";
        private string aPNodeNameHead = "节点名";
        private string aPNoneConnectHead = "无连接";
        private string aPNoneActiveHead = "未激活";
        private string aPActiveHead = "激活";
        private string buttonCloseTitle = "关闭";

        public string APNodeNameHead
        {
            get
            {
                return aPNodeNameHead;
            }

            set
            {
                aPNodeNameHead = value;
            }
        }

        public string APNoneConnectHead
        {
            get
            {
                return aPNoneConnectHead;
            }

            set
            {
                aPNoneConnectHead = value;
            }
        }

        public string APNoneActiveHead
        {
            get
            {
                return aPNoneActiveHead;
            }

            set
            {
                aPNoneActiveHead = value;
            }
        }

        public string APActiveHead
        {
            get
            {
                return aPActiveHead;
            }

            set
            {
                aPActiveHead = value;
            }
        }

        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                title = value;
            }
        }

        public string ButtonCloseTitle
        {
            get
            {
                return buttonCloseTitle;
            }

            set
            {
                buttonCloseTitle = value;
            }
        }

        public string BootSituationTabHead
        {
            get
            {
                return bootSituationTabHead;
            }

            set
            {
                bootSituationTabHead = value;
            }
        }
    }
    #endregion

    #region 扫频结果
    public class ScannerInfos
    {
        private string title = "扫描结果";
        private string aPCommunityIDHead = "小区ID";
        private string aPPLMNHead = "PLMN";
        private string aPFrePointHead = "频点";
        private string aPScramblerHead = "扰码";
        private string aPTLACHead = "TAC/LAC";
        private string aPSignalStrengthHead = "信号强度";
        private string aPAreaInfoHead = "临区信息";
        private string aPPriorityHead = "优先级";
        private string closeTitle = "退出";
        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                title = value;
            }
        }
        public string APCommunityIDHead
        {
            get
            {
                return aPCommunityIDHead;
            }

            set
            {
                aPCommunityIDHead = value;
            }
        }
        public string APPLMNHead
        {
            get
            {
                return aPPLMNHead;
            }

            set
            {
                aPPLMNHead = value;
            }
        }
        public string APFrePointHead
        {
            get
            {
                return aPFrePointHead;
            }

            set
            {
                aPFrePointHead = value;
            }
        }
        public string APScramblerHead
        {
            get
            {
                return aPScramblerHead;
            }

            set
            {
                aPScramblerHead = value;
            }
        }
        public string APTLACHead
        {
            get
            {
                return aPTLACHead;
            }

            set
            {
                aPTLACHead = value;
            }
        }
        public string APSignalStrengthHead
        {
            get
            {
                return aPSignalStrengthHead;
            }

            set
            {
                aPSignalStrengthHead = value;
            }
        }
        public string APAreaInfoHead
        {
            get
            {
                return aPAreaInfoHead;
            }

            set
            {
                aPAreaInfoHead = value;
            }
        }
        public string APPriorityHead
        {
            get
            {
                return aPPriorityHead;
            }

            set
            {
                aPPriorityHead = value;
            }
        }
        public string CloseTitle
        {
            get
            {
                return closeTitle;
            }

            set
            {
                closeTitle = value;
            }
        }

    }
    #endregion

    #region 特殊名单管理
    public class SpeciallistManageClass
    {
        private string title = "特殊名单管理";

        private string blackListCaption = "黑名单";
        private string whiteListCaption = "白名单";
        private string customListCaption = "普通用户";
        private string settingCaption = "设置";

        private string iMSICaption = "IMSI";
        private string iMEICaption = "IMEI";
        private string aliasNameCaption = "别名";
        private string deviceNameCaption = "设备";
        private string filterCaption = "查询";

        private string resourcesCaption = "资源";
        private string stationCaption = "设备名称";

        private string addCaption = "添加";
        private string editCaption = "修改";
        private string deleteCaption = "删除";
        private string clearCaption = "清空";
        private string exportCaption = "导出";

        private string redirectCaption = "重定向";
        private string batchImportCaption = "批量导入";
        private string fileNameCaption = "文件";
        private string importCaption = "导入";

        private string userTypeCaption = "用户类型";
        private string optimizationCaption = "重定向";
        private string priorityCaption = "优选";
        private string rejectMethodCaption = "模式";
        private string addtionFrequencyCaption = "附加频点";
        private string operationCaption = "操作";

        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                title = value;
            }
        }

        public string BlackListCaption
        {
            get
            {
                return blackListCaption;
            }

            set
            {
                blackListCaption = value;
            }
        }

        public string WhiteListCaption
        {
            get
            {
                return whiteListCaption;
            }

            set
            {
                whiteListCaption = value;
            }
        }

        public string SettingCaption
        {
            get
            {
                return settingCaption;
            }

            set
            {
                settingCaption = value;
            }
        }

        public string IMSICaption
        {
            get
            {
                return iMSICaption;
            }

            set
            {
                iMSICaption = value;
            }
        }

        public string AliasNameCaption
        {
            get
            {
                return aliasNameCaption;
            }

            set
            {
                aliasNameCaption = value;
            }
        }

        public string FilterCaption
        {
            get
            {
                return filterCaption;
            }

            set
            {
                filterCaption = value;
            }
        }

        public string ResourcesCaption
        {
            get
            {
                return resourcesCaption;
            }

            set
            {
                resourcesCaption = value;
            }
        }

        public string StationCaption
        {
            get
            {
                return stationCaption;
            }

            set
            {
                stationCaption = value;
            }
        }

        public string AddCaption
        {
            get
            {
                return addCaption;
            }

            set
            {
                addCaption = value;
            }
        }

        public string EditCaption
        {
            get
            {
                return editCaption;
            }

            set
            {
                editCaption = value;
            }
        }

        public string DeleteCaption
        {
            get
            {
                return deleteCaption;
            }

            set
            {
                deleteCaption = value;
            }
        }

        public string RedirectCaption
        {
            get
            {
                return redirectCaption;
            }

            set
            {
                redirectCaption = value;
            }
        }

        public string BatchImportCaption
        {
            get
            {
                return batchImportCaption;
            }

            set
            {
                batchImportCaption = value;
            }
        }

        public string FileNameCaption
        {
            get
            {
                return fileNameCaption;
            }

            set
            {
                fileNameCaption = value;
            }
        }

        public string ImportCaption
        {
            get
            {
                return importCaption;
            }

            set
            {
                importCaption = value;
            }
        }

        public string UserTypeCaption
        {
            get
            {
                return userTypeCaption;
            }

            set
            {
                userTypeCaption = value;
            }
        }
        public string PriorityCaption
        {
            get
            {
                return priorityCaption;
            }

            set
            {
                priorityCaption = value;
            }
        }

        public string OptimizationCaption
        {
            get
            {
                return optimizationCaption;
            }

            set
            {
                optimizationCaption = value;
            }
        }
        public string RejectMethodCaption
        {
            get
            {
                return rejectMethodCaption;
            }

            set
            {
                rejectMethodCaption = value;
            }
        }
        public string AddtionFrequencyCaption
        {
            get
            {
                return addtionFrequencyCaption;
            }

            set
            {
                addtionFrequencyCaption = value;
            }
        }

        public string OperationCaption
        {
            get
            {
                return operationCaption;
            }

            set
            {
                operationCaption = value;
            }
        }

        public string ExportCaption
        {
            get
            {
                return exportCaption;
            }

            set
            {
                exportCaption = value;
            }
        }

        public string CustomListCaption
        {
            get
            {
                return customListCaption;
            }

            set
            {
                customListCaption = value;
            }
        }

        public string ClearCaption
        {
            get
            {
                return clearCaption;
            }

            set
            {
                clearCaption = value;
            }
        }

        public string IMEICaption
        {
            get
            {
                return iMEICaption;
            }

            set
            {
                iMEICaption = value;
            }
        }

        public string DeviceNameCaption
        {
            get
            {
                return deviceNameCaption;
            }

            set
            {
                deviceNameCaption = value;
            }
        }
    }

    public class RedirectSetClass
    {
        private string parentFullPathNameCaption = "域名";
        private string selfNameCaption = "设备名";
        private string categoryCaption = "用户类型";
        private string optimizationCaption = "重定向";
        private string priorityCaption = "优选";
        private string rejectMethodCaption = "模式";
        private string additionalFreqCaption = "附加频点";
        private string btnOK = "确定";
        private string btnClose = "取消";
        public string ParentFullPathNameCaption
        {
            get
            {
                return parentFullPathNameCaption;
            }

            set
            {
                parentFullPathNameCaption = value;
            }
        }
        public string SelfNameCaption
        {
            get
            {
                return selfNameCaption;
            }

            set
            {
                selfNameCaption = value;
            }
        }
        public string CategoryCaption
        {
            get
            {
                return categoryCaption;
            }

            set
            {
                categoryCaption = value;
            }
        }
        public string OptimizationCaption
        {
            get
            {
                return optimizationCaption;
            }

            set
            {
                optimizationCaption = value;
            }
        }
        public string PriorityCaption
        {
            get
            {
                return priorityCaption;
            }

            set
            {
                priorityCaption = value;
            }
        }
        public string RejectMethodCaption
        {
            get
            {
                return rejectMethodCaption;
            }

            set
            {
                rejectMethodCaption = value;
            }
        }
        public string AdditionalFreqCaption
        {
            get
            {
                return additionalFreqCaption;
            }

            set
            {
                additionalFreqCaption = value;
            }
        }
        public string BtnOK
        {
            get
            {
                return btnOK;
            }

            set
            {
                btnOK = value;
            }
        }
        public string BtnClose
        {
            get
            {
                return btnClose;
            }

            set
            {
                btnClose = value;
            }
        }

    }
    #endregion

    #region 历史记录查询
    public class QueryHistoricalData
    {
        private string QHD_title = "历史记录查询";
        private string QHD_iMSI = "IMSI";
        private string QHD_startTime = "开始时间";
        private string QHD_stopTime = "结束时间";
        private string QHD_deviceManager = "设备";
        private string QHD_all = "全部";
        private string QHD_selectData = "查找";
        private string QHD_exportData = "导出";

        private string QHD_iD = "ID";
        private string QHD_time = "时间";
        private string QHD_customerType = "用户类型";
        private string QHD_tMSI = "TMSI";
        private string QHD_operator = "运营商";
        private string QHD_region = "地区";
        private string QHD_device = "设备";
        public string QHD_Title
        {
            get { return QHD_title; }
            set { QHD_title = value; }
        }
        public string QHD_IMSI
        {
            get { return QHD_iMSI; }
            set { QHD_iMSI = value; }
        }
        public string QHD_StartTime
        {
            get { return QHD_startTime; }
            set { QHD_startTime = value; }
        }
        public string QHD_StopTime
        {
            get { return QHD_stopTime; }
            set { QHD_stopTime = value; }
        }
        public string QHD_DeviceManager
        {
            get { return QHD_deviceManager; }
            set { QHD_deviceManager = value; }
        }
        public string QHD_All
        {
            get { return QHD_all; }
            set { QHD_all = value; }
        }
        public string QHD_SelectData
        {
            get { return QHD_selectData; }
            set { QHD_selectData = value; }
        }
        public string QHD_ExportData
        {
            get { return QHD_exportData; }
            set { QHD_exportData = value; }
        }
        public string QHD_ID
        {
            get { return QHD_iD; }
            set { QHD_iD = value; }
        }
        public string QHD_Time
        {
            get { return QHD_time; }
            set { QHD_time = value; }
        }
        public string QHD_CustomerType
        {
            get { return QHD_customerType; }
            set { QHD_customerType = value; }
        }
        public string QHD_TMSI
        {
            get { return QHD_tMSI; }
            set { QHD_tMSI = value; }
        }
        public string QHD_Operator
        {
            get { return QHD_operator; }
            set { QHD_operator = value; }
        }
        public string QHD_Region
        {
            get { return QHD_region; }
            set { QHD_region = value; }
        }
        public string QHD_Device
        {
            get { return QHD_device; }
            set { QHD_device = value; }
        }
    }
    #endregion

    #region 账号管理
    public class UserManage
    {
        private string accountNumber = "账号";
        private string addUser = "增加";
        private string updateUser = "更新";
        private string deleteUser = "删除";

        public string AccountNumber
        {
            get { return accountNumber; }
            set { accountNumber = value; }
        }
        public string AddUser
        {
            get { return addUser; }
            set { addUser = value; }
        }
        public string UpdateUser
        {
            get { return updateUser; }
            set { updateUser = value; }
        }
        public string DeleteUser
        {
            get { return deleteUser; }
            set { deleteUser = value; }
        }
    }
    #endregion

    #region 域管理
    public class ManageField
    {
        private string manageFieldTitle = "域管理";
        private string MF_delete = "删除域";
        private string MF_addLevel = "增加级别";
        private string MF_reduceLevel = "减少级别";
        private string MF_addField = "添加域";
        public string ManageFieldTitle
        {
            get { return manageFieldTitle; }
            set { manageFieldTitle = value; }
        }
        public string MF_Delete
        {
            get { return MF_delete; }
            set { MF_delete = value; }
        }
        public string MF_AddLevel
        {
            get { return MF_addLevel; }
            set { MF_addLevel = value; }
        }
        public string MF_ReduceLevel
        {
            get { return MF_reduceLevel; }
            set { MF_reduceLevel = value; }
        }
        public string MF_AddField
        {
            get { return MF_addField; }
            set { MF_addField = value; }
        }
    }
    #endregion

    #region 系统设置
    public class SettingSym
    {
        private string settingSymTitle = "系统设置";
        private string passwordModify = "密码修改";
        private string originalPassword = "原密码";
        private string newPassword = "新密码";
        private string reNewPassword = "重新输入新密码";
        private string SS_ok = "确定";

        private string localLocation = "本地位置";
        private string lOGLocation = "LOG位置";
        private string defaultLocation = "默认存储位置";
        private string SS_update = "更新";
        public string SettingSymTitle
        {
            get { return settingSymTitle; }
            set { settingSymTitle = value; }
        }
        public string PasswordModify
        {
            get { return passwordModify; }
            set { passwordModify = value; }
        }
        public string OriginalPassword
        {
            get { return originalPassword; }
            set { originalPassword = value; }
        }
        public string NewPassword
        {
            get { return newPassword; }
            set { newPassword = value; }
        }
        public string ReNewPassword
        {
            get { return reNewPassword; }
            set { reNewPassword = value; }
        }
        public string SS_OK
        {
            get { return SS_ok; }
            set { SS_ok = value; }
        }
        public string LocalLocation
        {
            get { return localLocation; }
            set { localLocation = value; }
        }
        public string LOGLocation
        {
            get { return lOGLocation; }
            set { lOGLocation = value; }
        }
        public string DefaultLocation
        {
            get { return defaultLocation; }
            set { defaultLocation = value; }
        }
        public string SS_Update
        {
            get { return SS_update; }
            set { SS_update = value; }
        }
    }
    #endregion

    #region 高级设置
    public class AdvanceSet
    {
        private string advanceSetTitle = "高级设置";
        private string backstageSetting = "后台设置";
        private string monitorPortConfig = "监听端口配置";
        private string noseMonitorIP = "前端监听IP/端口";
        private string useMonitorIP = "应用监听IP/端口";
        private string bS_Update = "更新";
        private string dBSet = "数据库设置";
        private string dBConnect = "数据库连接";
        private string chkYes = "是";
        private string chkNo = "否";
        private string dBAddress = "数据库地址";
        private string dBUser = "用户名";
        private string dBPassword = "密码";

        private string importExportSetting = "配置导入和导出";
        private string exportConfig = "配置导出";
        private string exportFile = "配置文件";
        private string export = "导出";
        private string exportDevice = "设备";
        private string exportSpecialList = "特殊名单";
        private string exportField = "用户和域";
        private string exportBackstage = "后台设置";
        private string importConfig = "配置导入";
        private string importFile = "配置文件";
        private string import = "导入";
        private string conflictHandle = "冲突处理";
        private string useNewConfig = "采用新配置";
        private string useOldConfig = "保留旧配置";
        private string conflictLOG = "冲突日志";
        public string AdvanceSetTitle
        {
            get { return advanceSetTitle; }
            set { advanceSetTitle = value; }
        }
        public string BackstageSetting
        {
            get { return backstageSetting; }
            set { backstageSetting = value; }
        }
        public string MonitorPortConfig
        {
            get { return monitorPortConfig; }
            set { monitorPortConfig = value; }
        }
        public string NoseMonitorIP
        {
            get { return noseMonitorIP; }
            set { noseMonitorIP = value; }
        }
        public string UseMonitorIP
        {
            get { return useMonitorIP; }
            set { useMonitorIP = value; }
        }
        public string BS_Update
        {
            get { return bS_Update; }
            set { bS_Update = value; }
        }
        public string DBSet
        {
            get { return dBSet; }
            set { dBSet = value; }
        }
        public string DBConnect
        {
            get { return dBConnect; }
            set { dBConnect = value; }
        }
        public string ChkYes
        {
            get { return chkYes; }
            set { chkYes = value; }
        }
        public string ChkNo
        {
            get { return chkNo; }
            set { chkNo = value; }
        }
        public string DBAddress
        {
            get { return dBAddress; }
            set { dBAddress = value; }
        }
        public string DBUser
        {
            get { return dBUser; }
            set { dBUser = value; }
        }
        public string DBPassword
        {
            get { return dBPassword; }
            set { dBPassword = value; }
        }
        public string ImportExportSetting
        {
            get { return importExportSetting; }
            set { importExportSetting = value; }
        }
        public string ExportConfig
        {
            get { return exportConfig; }
            set { exportConfig = value; }
        }
        public string ExportFile
        {
            get { return exportFile; }
            set { exportFile = value; }
        }
        public string Export
        {
            get { return export; }
            set { export = value; }
        }
        public string ExportDevice
        {
            get { return exportDevice; }
            set { exportDevice = value; }
        }
        public string ExportSpecialList
        {
            get { return exportSpecialList; }
            set { exportSpecialList = value; }
        }
        public string ExportField
        {
            get { return exportField; }
            set { exportField = value; }
        }
        public string ExportBackstage
        {
            get { return exportBackstage; }
            set { exportBackstage = value; }
        }
        public string ImportConfig
        {
            get { return importConfig; }
            set { importConfig = value; }
        }
        public string ImportFile
        {
            get { return importFile; }
            set { importFile = value; }
        }
        public string Import
        {
            get { return import; }
            set { import = value; }
        }
        public string ConflictHandle
        {
            get { return conflictHandle; }
            set { conflictHandle = value; }
        }
        public string UseNewConfig
        {
            get { return useNewConfig; }
            set { useNewConfig = value; }
        }
        public string UseOldConfig
        {
            get { return useOldConfig; }
            set { useOldConfig = value; }
        }
        public string ConflictLOG
        {
            get { return conflictLOG; }
            set { conflictLOG = value; }
        }
    }
    #endregion

    #region 增加用户组
    public class AddUserGroup
    {
        private string addUserGroupTitle = "增加用户组";
        private string userGroupName = "用户组名";
        private string inherit = "是否继承";
        private string noInherit = "不继承";
        private string advancedOperator = "高级操作员";
        private string operatorGroup = "操作员";
        public string AddUserGroupTitle
        {
            get { return addUserGroupTitle; }
            set { addUserGroupTitle = value; }
        }
        public string UserGroupName
        {
            get { return userGroupName; }
            set { userGroupName = value; }
        }
        public string Inherit
        {
            get { return inherit; }
            set { inherit = value; }
        }
        public string NoInherit
        {
            get { return noInherit; }
            set { noInherit = value; }
        }
        public string AdvancedOperator
        {
            get { return advancedOperator; }
            set { advancedOperator = value; }
        }
        public string OperatorGroup
        {
            get { return operatorGroup; }
            set { operatorGroup = value; }
        }
    }
    #endregion

    #region 添加账号
    public class AddUserInfor
    {
        private string addUserInforTitle = "添加账号";
        private string addUserName = "用户名";
        private string addUserGroup = "用户组";
        private string addUserPassword = "密码";
        private string addOtherName = "别名";
        private string addBuilder = "生成人";
        private string addOK = "确定";
        private string addCancel = "取消";
        public string AddUserInforTitle { get { return addUserInforTitle; } set { addUserInforTitle = value; } }
        public string AddUserName { get { return addUserName; } set { addUserName = value; } }
        public string AddUserGroup { get { return addUserGroup; } set { addUserGroup = value; } }
        public string AddUserPassword { get { return addUserPassword; } set { addUserPassword = value; } }
        public string AddOtherName { get { return addOtherName; } set { addOtherName = value; } }
        public string AddBuilder { get { return addBuilder; } set { addBuilder = value; } }
        public string AddOK { get { return addOK; } set { addOK = value; } }
        public string AddCancel { get { return addCancel; } set { addCancel = value; } }
    }
    #endregion

    #region 设备属性详细信息
    public class LTEDeviceDetail
    {
        private string deviceNameCaption = "设备名称";
        private string pLMNCaption = "PLMN";
        private string freqPCICaption = "频点/扰码";
        private string cellStatusCaption = "小区状态";
        private string scannerStatusCaption = "射频状态";
        private string gPSStatusCaption = "GPS状态";
        private string gPSDetailCaption = "GPS详细信息";
        private string syncStatusCaption = "同步状态";
        private string syncSourceCaption = "同步源信息";
        private string whiteListSellLearningStatusCaption = "白名单自学习状态";
        private string aPReadyStCaption = "就绪状态";
        private string licenseStatusCaption = "Lience信息";
        private string versionCaption = "版本号";

        public string DeviceNameCaption
        {
            get
            {
                return deviceNameCaption;
            }

            set
            {
                deviceNameCaption = value;
            }
        }

        public string PLMNCaption
        {
            get
            {
                return pLMNCaption;
            }

            set
            {
                pLMNCaption = value;
            }
        }

        public string FreqPCICaption
        {
            get
            {
                return freqPCICaption;
            }

            set
            {
                freqPCICaption = value;
            }
        }

        public string CellStatusCaption
        {
            get
            {
                return cellStatusCaption;
            }

            set
            {
                cellStatusCaption = value;
            }
        }

        public string ScannerStatusCaption
        {
            get
            {
                return scannerStatusCaption;
            }

            set
            {
                scannerStatusCaption = value;
            }
        }

        public string GPSStatusCaption
        {
            get
            {
                return gPSStatusCaption;
            }

            set
            {
                gPSStatusCaption = value;
            }
        }

        public string GPSDetailCaption
        {
            get
            {
                return gPSDetailCaption;
            }

            set
            {
                gPSDetailCaption = value;
            }
        }

        public string SyncStatusCaption
        {
            get
            {
                return syncStatusCaption;
            }

            set
            {
                syncStatusCaption = value;
            }
        }

        public string SyncSourceCaption
        {
            get
            {
                return syncSourceCaption;
            }

            set
            {
                syncSourceCaption = value;
            }
        }

        public string LicenseStatusCaption
        {
            get
            {
                return licenseStatusCaption;
            }

            set
            {
                licenseStatusCaption = value;
            }
        }

        public string VersionCaption
        {
            get
            {
                return versionCaption;
            }

            set
            {
                versionCaption = value;
            }
        }

        public string WhiteListSellLearningStatusCaption
        {
            get
            {
                return whiteListSellLearningStatusCaption;
            }

            set
            {
                whiteListSellLearningStatusCaption = value;
            }
        }

        public string APReadyStCaption
        {
            get
            {
                return aPReadyStCaption;
            }

            set
            {
                aPReadyStCaption = value;
            }
        }
    }

    public class WCDMADeviceDetail : LTEDeviceDetail
    {

    }

    public class CDMADeviceDetail : WCDMADeviceDetail
    {

    }

    public class GSMDeviceDetail : CDMADeviceDetail
    {

    }
    #endregion

    #region 错误弹框提示
    public class MessageTipsInformation
    {
        public class ErrorMessages
        {
            string USERNAME_NULL = "请输入用户名！";
        }

        private string title;
        private string message;
    }
    #endregion
}
