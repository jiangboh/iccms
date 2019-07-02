using ColorPickerWPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace ParameterControl
{
    #region 消息窗口警告类型
    public enum WindowMessageType
    {
        Information = 64,
        Warnning = 48,
        Error = 16
    }
    #endregion

    public enum DeviceTreeOperation
    {
        DomainAdd,
        DomainDelete,
        DomainReName,
        DeviceAdd,
        DeviceDelete,
        DeviceUpdate
    }

    public enum APOnOffLineKey
    {
        OffLine,    //AP下线
        OnLine      //AP上线
    }

    public enum CharactorCodeType
    {
        UTF8 = 1,    //UTF8编码
        Default = 2  //默认
    }

    //文件类型
    public enum FileExtension
    {
        JPG = 255216,
        GIF = 7173,
        BMP = 6677,
        PNG = 13780,
        COM = 7790,
        EXE = 7790,
        DLL = 7790,
        RAR = 8297,
        ZIP = 8075,
        XML = 6063,
        HTML = 6033,
        ASPX = 239187,
        CS = 117115,
        JS = 119105,
        TXT = 210187,
        SQL = 255254,
        BAT = 64101,
        BTSEED = 10056,
        RDP = 255254,
        PSD = 5666,
        PDF = 3780,
        CHM = 7384,
        LOG = 70105,
        REG = 8269,
        HLP = 6395,
        DOC = 208207,
        XLS = 208207,
        DOCX = 208207,
        XLSX = 208207,
    }

    public enum ActionTypeList
    {
        Auto,
        Show,
        Hide
    }

    public class ColumnOderImage
    {
        public readonly string ASC = @"pack://application:,,,/iccms;component/Icon/1downarrow1.png";
        public readonly string DESC = @"pack://application:,,,/iccms;component/Icon/1downarrow.png";
    }

    public class StatusIcon
    {
        public readonly string OK = @"../Icon/Ok.ico";
        public readonly string None = @"../Icon/ErrorStatu.ico";
        public readonly string RenameNone = @"../Icon/Problem.ico";
        public readonly string RenameOk = @"../Icon/Problem_ok.ico";
        public readonly string RenameNoneTips = @"未重命名该未知设备";
        public readonly string RenameOkTips = @"设备已重命名";
    }

    public class UserTypesClass
    {
        string _whiteList = "白名单";
        string _blackList = "黑名单";
        string _otherList = "普通用户";

        public string WhiteList
        {
            get
            {
                return _whiteList;
            }

            set
            {
                _whiteList = value;
            }
        }

        public string BlackList
        {
            get
            {
                return _blackList;
            }

            set
            {
                _blackList = value;
            }
        }

        public string OtherList
        {
            get
            {
                return _otherList;
            }

            set
            {
                _otherList = value;
            }
        }
    }

    //窗口位置坐标
    public class UserMousePosition
    {
        private Double x;
        private Double y;

        public Double X
        {
            get
            {
                return x;
            }

            set
            {
                x = value;
            }
        }

        public Double Y
        {
            get
            {
                return y;
            }

            set
            {
                y = value;
            }
        }
    }

    /// <summary>
    /// 域操作
    /// </summary>
    public class DomainActionInfo : INotifyPropertyChanged
    {
        private string pathName;
        private string isStation;
        private bool isDeleted;
        private string permission;
        private string nodeType;
        private string nodeIcon;
        private int selfID;
        private string selfName;
        private string nodeContent;
        private string aliasName;
        private int parentID;
        private string oldFullDomainName;
        private string newFullDomainName;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string value)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(value));
            }
        }

        public int SelfID
        {
            get
            {
                return selfID;
            }

            set
            {
                selfID = value;
                NotifyPropertyChanged("selfID");
            }
        }

        public string SelfName
        {
            get
            {
                return selfName;
            }

            set
            {
                selfName = value;
                NotifyPropertyChanged("selfName");
            }
        }

        public int ParentID
        {
            get
            {
                return parentID;
            }

            set
            {
                parentID = value;
                NotifyPropertyChanged("parentID");
            }
        }

        public string NodeContent
        {
            get
            {
                return nodeContent;
            }

            set
            {
                nodeContent = value;
                NotifyPropertyChanged("nodeContent");
            }
        }

        public string PathName
        {
            get
            {
                return pathName;
            }

            set
            {
                pathName = value;
                NotifyPropertyChanged("pathName");
            }
        }

        public string IsStation
        {
            get
            {
                return isStation;
            }

            set
            {
                isStation = value;
                NotifyPropertyChanged("isStation");
            }
        }

        public bool IsDeleted
        {
            get
            {
                return isDeleted;
            }

            set
            {
                isDeleted = value;
                NotifyPropertyChanged("isDeleted");
            }
        }

        public string Permission
        {
            get
            {
                return permission;
            }

            set
            {
                permission = value;
                NotifyPropertyChanged("permission");
            }
        }

        public string NodeType
        {
            get
            {
                return nodeType;
            }

            set
            {
                nodeType = value;
                NotifyPropertyChanged("nodeType");
            }
        }

        public string NodeIcon
        {
            get
            {
                return nodeIcon;
            }

            set
            {
                nodeIcon = value;
                NotifyPropertyChanged("nodeIcon");
            }
        }

        public string OldFullDomainName
        {
            get
            {
                return oldFullDomainName;
            }

            set
            {
                oldFullDomainName = value;
                NotifyPropertyChanged("oldFullDomainName");
            }
        }

        public string NewFullDomainName
        {
            get
            {
                return newFullDomainName;
            }

            set
            {
                newFullDomainName = value;
                NotifyPropertyChanged("newFullDomainName");
            }
        }

        public string AliasName
        {
            get
            {
                return aliasName;
            }

            set
            {
                aliasName = value;
                NotifyPropertyChanged("AliasName");
            }
        }
    }

    /// <summary>
    /// 设备操作
    /// </summary>
    public class DeviceActionInfo
    {
        private string domainFullName;
        private string deviceName;
        private string selfID;
        private string parentID;
        private string sN;
        private string carrier;
        private string iPAddr;
        private string port;
        private string netMask;
        private string mode;
        private string online;
        private string lastOnline;
        private string isActive;

        public string DomainFullName
        {
            get
            {
                return domainFullName;
            }

            set
            {
                domainFullName = value;
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

        public string Carrier
        {
            get
            {
                return carrier;
            }

            set
            {
                carrier = value;
            }
        }

        public string IPAddr
        {
            get
            {
                return iPAddr;
            }

            set
            {
                iPAddr = value;
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

        public string Online
        {
            get
            {
                return online;
            }

            set
            {
                online = value;
            }
        }

        public string LastOnline
        {
            get
            {
                return lastOnline;
            }

            set
            {
                lastOnline = value;
            }
        }

        public string IsActive
        {
            get
            {
                return isActive;
            }

            set
            {
                isActive = value;
            }
        }
    }

    #region AP配置响应结果
    public class APResoultMsgTypeClass
    {
        private string resoult_0 = "SUCCESS";
        private string resoult_0_CN = "成功";
        private string resoult_1 = "GENERAL FAILURE";
        private string resoult_1_CN = "常规错误";
        private string resoult_2 = "CONFIGURATION FAIURE OR NOT SUPPORTED";
        private string resoult_2_CN = "配置失败或者提交的参数不受支持";
        private string resoult_3 = "RECEIVER MSG IN WRONG STATE";
        private string resoult_3_CN = "接收消息处于错误状态";
        private string resoult_4 = "FUNCTION NOT SUPPORTED";
        private string resoult_4_CN = "功能不支持";
        private string resoult_5 = "RESOURCE NOT ENOUGH";
        private string resoult_5_CN = "资源不足够";

        public string Resoult_0
        {
            get
            {
                if (Parameters.LanguageType.Equals("EN"))
                {
                    return resoult_0;
                }
                else
                {
                    return resoult_0_CN;
                }
            }

            set
            {
                resoult_0 = value;
            }
        }

        public string Resoult_1
        {
            get
            {
                if (Parameters.LanguageType.Equals("EN"))
                {
                    return resoult_1;
                }
                else
                {
                    return resoult_1_CN;
                }
            }

            set
            {
                resoult_1 = value;
            }
        }

        public string Resoult_2
        {
            get
            {
                if (Parameters.LanguageType.Equals("EN"))
                {
                    return resoult_2;
                }
                else
                {
                    return resoult_2_CN;
                }
            }

            set
            {
                resoult_2 = value;
            }
        }

        public string Resoult_3
        {
            get
            {
                if (Parameters.LanguageType.Equals("EN"))
                {
                    return resoult_3;
                }
                else
                {
                    return resoult_3_CN;
                }
            }

            set
            {
                resoult_3 = value;
            }
        }

        public string Resoult_4
        {
            get
            {
                if (Parameters.LanguageType.Equals("EN"))
                {
                    return resoult_4;
                }
                else
                {
                    return resoult_4_CN;
                }
            }

            set
            {
                resoult_4 = value;
            }
        }

        public string Resoult_5
        {
            get
            {
                if (Parameters.LanguageType.Equals("EN"))
                {
                    return resoult_5;
                }
                else
                {
                    return resoult_5_CN;
                }
            }

            set
            {
                resoult_5 = value;
            }
        }
    }

    public class APResoultRebootMsgTypeClss
    {
        private string rebootNow = "立刻重启";
        private string needReboot = "需要重启";
        private string defaultInfo = "成功";

        public string RebootFlag(string rebootMode)
        {
            if (rebootMode == ("0"))
            {
                return defaultInfo;
            }
            else if (rebootMode == ("1"))
            {
                return rebootNow;
            }
            else if (rebootMode == ("2"))
            {
                return needReboot;
            }
            return string.Empty;
        }
    }
    #endregion

    #region 实时上报捕号参数
    public class ScannerDataControlClass : INotifyPropertyChanged
    {
        private int refreshTime;
        private int tatol = 200;
        private string soundFile;
        private string soundDelay;
        private int playCount;
        private double volume = 1;
        private string _soundEnable = "0";
        private string _speeckContent;
        private bool _playerMode;
        private string _whiteListBackGround;
        private string _blackListBackGround;
        private string _otherListBackGround;
        private bool _whiteListMode;
        private bool _blackListMode;
        private bool _otherListMode;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string value)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(value));
            }
        }

        public int RefreshTime
        {
            get
            {
                return refreshTime;
            }

            set
            {
                refreshTime = value;
            }
        }
        public int Tatol
        {
            get
            {
                return tatol;
            }

            set
            {
                tatol = value;
            }
        }

        public string SoundFile
        {
            get
            {
                return soundFile;
            }

            set
            {
                soundFile = value;
            }
        }

        public string SoundDelay
        {
            get
            {
                return soundDelay;
            }

            set
            {
                soundDelay = value;
            }
        }

        public int PlayCount
        {
            get
            {
                return playCount;
            }

            set
            {
                playCount = value;
            }
        }

        public double Volume
        {
            get
            {
                return volume;
            }

            set
            {
                volume = value;
            }
        }

        public string SoundEnable
        {
            get
            {
                return _soundEnable;
            }

            set
            {
                _soundEnable = value;
            }
        }

        public string SpeeckContent
        {
            get
            {
                return _speeckContent;
            }

            set
            {
                _speeckContent = value;
            }
        }

        public bool PlayerMode
        {
            get
            {
                return _playerMode;
            }

            set
            {
                _playerMode = value;
            }
        }

        public string WhiteListBackGround
        {
            get
            {
                return _whiteListBackGround;
            }

            set
            {
                _whiteListBackGround = value;
                NotifyPropertyChanged("WhiteListBackGround");
            }
        }

        public string BlackListBackGround
        {
            get
            {
                return _blackListBackGround;
            }

            set
            {
                _blackListBackGround = value;
                NotifyPropertyChanged("BlackListBackGround");
            }
        }

        public string OtherListBackGround
        {
            get
            {
                return _otherListBackGround;
            }

            set
            {
                _otherListBackGround = value;
                NotifyPropertyChanged("OtherListBackGround");
            }
        }

        public bool WhiteListMode
        {
            get
            {
                return _whiteListMode;
            }

            set
            {
                _whiteListMode = value;
            }
        }

        public bool BlackListMode
        {
            get
            {
                return _blackListMode;
            }

            set
            {
                _blackListMode = value;
            }
        }

        public bool OtherListMode
        {
            get
            {
                return _otherListMode;
            }

            set
            {
                _otherListMode = value;
            }
        }
    }
    #endregion

    #region 重定向参数
    public class RedirectionClass
    {
        private string category;
        private string optimization;
        private string priority;
        private string rejectMethod;
        private string freq;
        private string additionalFreq;
        public string Category
        {
            get
            {
                return category;
            }

            set
            {
                category = value;
            }
        }
        public string Optimization
        {
            get
            {
                return optimization;
            }

            set
            {
                optimization = value;
            }
        }
        public string Priority
        {
            get
            {
                return priority;
            }

            set
            {
                priority = value;
            }
        }
        public string RejectMethod
        {
            get
            {
                return rejectMethod;
            }

            set
            {
                rejectMethod = value;
            }
        }
        public string Freq
        {
            get
            {
                return freq;
            }

            set
            {
                freq = value;
            }
        }
        public string AdditionalFreq
        {
            get
            {
                return additionalFreq;
            }

            set
            {
                additionalFreq = value;
            }
        }
    }
    #endregion

    #region Windows标题栏系统事件
    public static class WindowsAPIStyleEventsParameterClass
    {
        public const int GWL_STYLE = -16;
        public const int WS_SYSMENU = 0x80000;
        public const int LWA_ALPHA = 0x00000002;
        public const int LWA_COLORKEY = 0x00000001;
        public const int WS_EX_LAYERED = 0x00000016;
    }
    #endregion

    #region 文件类型
    public class FileTypeExClass
    {
        private string _JPG = ".JPG";
        private string _GIF = ".GIF";
        private string _BMP = ".BMP";
        private string _PNG = ".PNG";
        private string _COM = ".COM";

        private string _EXE = ".EXE";
        private string _DLL = ".DLL";
        private string _RAR = ".RAR";
        private string _ZIP = ".ZIP";
        private string _XML = ".XML";

        private string _HTML = ".HTML";
        private string _HTM = ".HTM";
        private string _ASPX = ".ASPX";
        private string _CS = ".CS";
        private string _JS = ".JS";

        private string _TXT = ".TXT";
        private string _SQL = ".SQL";
        private string _BAT = ".BAT";
        private string _BTSEED = ".BTSEED";
        private string _RDP = ".RDP";

        private string _PSD = ".PSD";
        private string _PDF = ".PDF";
        private string _CHM = ".CHM";
        private string _LOG = ".LOG";
        private string _REG = ".REG";

        private string _HLP = ".HLP";
        private string _DOC = ".DOC";
        private string _XLS = ".XLS";
        private string _DOCX = ".DOCX";
        private string _XLSX = ".XLSX";

        private string _TAR = ".TAR";
        private string _GZ = ".GZ";

        public string JPG
        {
            get
            {
                return _JPG;
            }

            set
            {
                _JPG = value;
            }
        }

        public string GIF
        {
            get
            {
                return _GIF;
            }

            set
            {
                _GIF = value;
            }
        }

        public string BMP
        {
            get
            {
                return _BMP;
            }

            set
            {
                _BMP = value;
            }
        }

        public string PNG
        {
            get
            {
                return _PNG;
            }

            set
            {
                _PNG = value;
            }
        }

        public string COM
        {
            get
            {
                return _COM;
            }

            set
            {
                _COM = value;
            }
        }

        public string EXE
        {
            get
            {
                return _EXE;
            }

            set
            {
                _EXE = value;
            }
        }

        public string DLL
        {
            get
            {
                return _DLL;
            }

            set
            {
                _DLL = value;
            }
        }

        public string RAR
        {
            get
            {
                return _RAR;
            }

            set
            {
                _RAR = value;
            }
        }

        public string ZIP
        {
            get
            {
                return _ZIP;
            }

            set
            {
                _ZIP = value;
            }
        }

        public string XML
        {
            get
            {
                return _XML;
            }

            set
            {
                _XML = value;
            }
        }

        public string HTML
        {
            get
            {
                return _HTML;
            }

            set
            {
                _HTML = value;
            }
        }

        public string HTM
        {
            get
            {
                return _HTM;
            }

            set
            {
                _HTM = value;
            }
        }

        public string ASPX
        {
            get
            {
                return _ASPX;
            }

            set
            {
                _ASPX = value;
            }
        }

        public string CS
        {
            get
            {
                return _CS;
            }

            set
            {
                _CS = value;
            }
        }

        public string JS
        {
            get
            {
                return _JS;
            }

            set
            {
                _JS = value;
            }
        }

        public string TXT
        {
            get
            {
                return _TXT;
            }

            set
            {
                _TXT = value;
            }
        }

        public string SQL
        {
            get
            {
                return _SQL;
            }

            set
            {
                _SQL = value;
            }
        }

        public string BAT
        {
            get
            {
                return _BAT;
            }

            set
            {
                _BAT = value;
            }
        }

        public string BTSEED
        {
            get
            {
                return _BTSEED;
            }

            set
            {
                _BTSEED = value;
            }
        }

        public string RDP
        {
            get
            {
                return _RDP;
            }

            set
            {
                _RDP = value;
            }
        }

        public string PSD
        {
            get
            {
                return _PSD;
            }

            set
            {
                _PSD = value;
            }
        }

        public string PDF
        {
            get
            {
                return _PDF;
            }

            set
            {
                _PDF = value;
            }
        }

        public string CHM
        {
            get
            {
                return _CHM;
            }

            set
            {
                _CHM = value;
            }
        }

        public string LOG
        {
            get
            {
                return _LOG;
            }

            set
            {
                _LOG = value;
            }
        }

        public string REG
        {
            get
            {
                return _REG;
            }

            set
            {
                _REG = value;
            }
        }

        public string HLP
        {
            get
            {
                return _HLP;
            }

            set
            {
                _HLP = value;
            }
        }

        public string DOC
        {
            get
            {
                return _DOC;
            }

            set
            {
                _DOC = value;
            }
        }

        public string XLS
        {
            get
            {
                return _XLS;
            }

            set
            {
                _XLS = value;
            }
        }

        public string DOCX
        {
            get
            {
                return _DOCX;
            }

            set
            {
                _DOCX = value;
            }
        }

        public string XLSX
        {
            get
            {
                return _XLSX;
            }

            set
            {
                _XLSX = value;
            }
        }

        public string TAR
        {
            get
            {
                return _TAR;
            }

            set
            {
                _TAR = value;
            }
        }

        public string GZ
        {
            get
            {
                return _GZ;
            }

            set
            {
                _GZ = value;
            }
        }
    }
    #endregion

    #region 文件类型对应图标
    public class FileTypeIcon
    {
        FileTypeExClass FileType = new FileTypeExClass();

        public string GetFileTypeIcon(string FileEx)
        {
            if (FileEx.ToLower() == FileType.ZIP.ToLower())
            {
                return @"../Icon/rar.ico";
            }
            else if (FileEx.ToLower() == FileType.RAR.ToLower())
            {
                return @"../Icon/rar.ico";
            }
            else if (FileEx.ToLower() == FileType.TAR.ToLower())
            {
                return @"../Icon/rar.ico";
            }
            else if (FileEx.ToLower() == FileType.GZ.ToLower())
            {
                return @"../Icon/rar.ico";
            }
            else if (FileEx.ToLower() == FileType.LOG.ToLower() || FileEx.ToLower() == FileType.TXT.ToLower())
            {
                return @"../Icon/textfile.ico";
            }
            else
            {
                return @"../Icon/unkown.ico";
            }
        }
    }
    #endregion

    #region 窗口显示控制
    public class WindowsControlParameterClass : INotifyPropertyChanged
    {
        private Visibility _deviceTreeWindow = Visibility.Visible;
        private Visibility _scannerWindow = Visibility.Visible;
        private Visibility _measurementWindow = Visibility.Visible;
        private Visibility _ueInfoWindow = Visibility.Visible;
        private Visibility _systemLogsWindow = Visibility.Visible;
        private Visibility _deviceTreeMoveBar = Visibility.Visible;
        private Visibility _systemLogsMoveBar = Visibility.Visible;
        private Visibility _scannerListMoveBar = Visibility.Visible;
        private Visibility _measurementListMoveBar = Visibility.Visible;

        public Visibility DeviceTreeWindow
        {
            get
            {
                return _deviceTreeWindow;
            }

            set
            {
                _deviceTreeWindow = value;
                NotifyPropertyChanged("DeviceTreeWindow");
            }
        }

        public Visibility ScannerWindow
        {
            get
            {
                return _scannerWindow;
            }

            set
            {
                _scannerWindow = value;
                NotifyPropertyChanged("ScannerWindow");
            }
        }

        public Visibility MeasurementWindow
        {
            get
            {
                return _measurementWindow;
            }

            set
            {
                _measurementWindow = value;
                NotifyPropertyChanged("MeasurementWindow");
            }
        }

        public Visibility SystemLogsWindow
        {
            get
            {
                return _systemLogsWindow;
            }

            set
            {
                _systemLogsWindow = value;
                NotifyPropertyChanged("SystemLogsWindow");
            }
        }

        public Visibility DeviceTreeMoveBar
        {
            get
            {
                return _deviceTreeMoveBar;
            }

            set
            {
                _deviceTreeMoveBar = value;
                NotifyPropertyChanged("DeviceTreeMoveBar");
            }
        }

        public Visibility SystemLogsMoveBar
        {
            get
            {
                return _systemLogsMoveBar;
            }

            set
            {
                _systemLogsMoveBar = value;
                NotifyPropertyChanged("SystemLogsMoveBar");
            }
        }

        public Visibility ScannerListMoveBar
        {
            get
            {
                return _scannerListMoveBar;
            }

            set
            {
                _scannerListMoveBar = value;
                NotifyPropertyChanged("ScannerListMoveBar");
            }
        }

        public Visibility MeasurementListMoveBar
        {
            get
            {
                return _measurementListMoveBar;
            }

            set
            {
                _measurementListMoveBar = value;
                NotifyPropertyChanged("MeasurementListMoveBar");
            }
        }

        public Visibility UeInfoWindow
        {
            get
            {
                return _ueInfoWindow;
            }

            set
            {
                _ueInfoWindow = value;
                NotifyPropertyChanged("UeInfoWindow");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string value)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(value));
            }
        }
    }
    #endregion

    #region 未知设备窗口显示控制参数
    public class UnknownDeviceWindowControlParametersClass : INotifyPropertyChanged
    {
        private Visibility _unknownDeviceTipsBlockAttribute = Visibility.Collapsed;
        private string _backGroundColor = "Red";
        private string _backGroundTincture = "LightPink";
        private string _toolTipContent = "系统有新的未知设备...";
        private int _devcieTreeRowSpan = 2;
        private string _actionType = ActionTypeList.Auto.ToString();
        private double _ellipseRadiueX = 20;
        private double _ellipseRadiueY = 20;
        private double elementScaleCoefficient = 1;

        private Visibility _signleLine0Show = Visibility.Collapsed;
        private Visibility _signleLine1Show = Visibility.Collapsed;
        private Visibility _signleLine2Show = Visibility.Collapsed;
        private Visibility _signleLine3Show = Visibility.Collapsed;
        private Visibility _signleLine4Show = Visibility.Collapsed;
        private Visibility _signleLine5Show = Visibility.Collapsed;

        private Visibility _signleLine0SHide = Visibility.Collapsed;
        private Visibility _signleLine1SHide = Visibility.Collapsed;
        private Visibility _signleLine2SHide = Visibility.Collapsed;
        private Visibility _signleLine3SHide = Visibility.Collapsed;
        private Visibility _signleLine4SHide = Visibility.Collapsed;
        private Visibility _signleLine5SHide = Visibility.Collapsed;

        //第0个信号
        private double signalZeroStn = 7;
        private Point signalZeroPfSp = new Point(10, 4);
        private Point signalZeroAsp = new Point(10, 46);
        private Size signalZeroAss = new Size(50, 50);

        //第1个信号
        private double signalOneStn = 6;
        private Point signalOnePfSp = new Point(30, 7);
        private Point signalOneAsp = new Point(30, 43);
        private Size signalOneAss = new Size(50, 50);

        //第2个信号
        private double signalTwoStn = 5;
        private Point signalTwoPfSp = new Point(50, 10);
        private Point signalTwoAsp = new Point(50, 40);
        private Size signalTwoAss = new Size(50, 50);

        //第3个信号
        private double signalThreeStn = 4;
        private Point signalThreePfSp = new Point(70, 13);
        private Point signalThreeAsp = new Point(70, 37);
        private Size signalThreeAss = new Size(50, 50);

        //第4个信号
        private double signalFourStn = 3;
        private Point signalFourPfSp = new Point(90, 33);
        private Point signalFourAsp = new Point(90, 17);
        private Size signalFourAss = new Size(50, 50);

        //第5个信号
        private double signalFiveStn = 2;
        private Point signalFivePfSp = new Point(110, 30);
        private Point signalFiveAsp = new Point(110, 20);
        private Size signalFiveAss = new Size(50, 50);

        //中心点
        private Point signalDot = new Point(140, 25);

        //第6个信号
        private double signalSixStn = 2;
        private Point signalSixPfSp = new Point(170, 30);
        private Point signalSixAsp = new Point(170, 20);
        private Size signalSixAss = new Size(50, 50);

        //第7个信号
        private double signalSevenStn = 3;
        private Point signalsSvenPfSp = new Point(190, 33);
        private Point signalSevenAsp = new Point(190, 17);
        private Size signalSevenAss = new Size(50, 50);

        //第8个信号
        private double signalEightStn = 4;
        private Point signalEightPfSp = new Point(210, 13);
        private Point signalEightAsp = new Point(210, 37);
        private Size signalEightAss = new Size(50, 50);

        //第9个信号
        private double signalNineStn = 5;
        private Point signalNinePfSp = new Point(230, 10);
        private Point signalNineAsp = new Point(230, 40);
        private Size signalNineAss = new Size(50, 50);

        //第10个信号
        private double signalTenStn = 6;
        private Point signalTenPfSp = new Point(250, 7);
        private Point signalTenAsp = new Point(250, 43);
        private Size signalTenAss = new Size(50, 50);

        //第11个信号
        private double signalElevenStn = 7;
        private Point signalElevenPfSp = new Point(270, 4);
        private Point signalElevenAsp = new Point(270, 46);
        private Size signalElevenAss = new Size(50, 50);

        public Visibility UnknownDeviceTipsBlockAttribute
        {
            get
            {
                return _unknownDeviceTipsBlockAttribute;
            }

            set
            {
                _unknownDeviceTipsBlockAttribute = value;
                NotifyPropertyChanged("UnknownDeviceTipsBlockAttribute");
            }
        }

        public string BackGroundColor
        {
            get
            {
                return _backGroundColor;
            }

            set
            {
                _backGroundColor = value;
                NotifyPropertyChanged("BackGroundColor");
            }
        }

        public int DevcieTreeRowSpan
        {
            get
            {
                return _devcieTreeRowSpan;
            }

            set
            {
                _devcieTreeRowSpan = value;
                NotifyPropertyChanged("DevcieTreeRowSpan");
            }
        }

        public string ActionType
        {
            get
            {
                return _actionType;
            }

            set
            {
                _actionType = value;
                NotifyPropertyChanged("ActionType");
            }
        }

        public Visibility SignleLine0Show
        {
            get
            {
                return _signleLine0Show;
            }

            set
            {
                _signleLine0Show = value;
                NotifyPropertyChanged("SignleLine0Show");
            }
        }

        public Visibility SignleLine1Show
        {
            get
            {
                return _signleLine1Show;
            }

            set
            {
                _signleLine1Show = value;
                NotifyPropertyChanged("SignleLine1Show");
            }
        }

        public Visibility SignleLine2Show
        {
            get
            {
                return _signleLine2Show;
            }

            set
            {
                _signleLine2Show = value;
                NotifyPropertyChanged("SignleLine2Show");
            }
        }

        public Visibility SignleLine3Show
        {
            get
            {
                return _signleLine3Show;
            }

            set
            {
                _signleLine3Show = value;
                NotifyPropertyChanged("SignleLine3Show");
            }
        }

        public Visibility SignleLine4Show
        {
            get
            {
                return _signleLine4Show;
            }

            set
            {
                _signleLine4Show = value;
                NotifyPropertyChanged("SignleLine4Show");
            }
        }

        public Visibility SignleLine5Show
        {
            get
            {
                return _signleLine5Show;
            }

            set
            {
                _signleLine5Show = value;
                NotifyPropertyChanged("SignleLine5Show");
            }
        }

        public Visibility SignleLine0SHide
        {
            get
            {
                return _signleLine0SHide;
            }

            set
            {
                _signleLine0SHide = value;
                NotifyPropertyChanged("SignleLine0SHide");
            }
        }

        public Visibility SignleLine1SHide
        {
            get
            {
                return _signleLine1SHide;
            }

            set
            {
                _signleLine1SHide = value;
                NotifyPropertyChanged("SignleLine1SHide");
            }
        }

        public Visibility SignleLine2SHide
        {
            get
            {
                return _signleLine2SHide;
            }

            set
            {
                _signleLine2SHide = value;
                NotifyPropertyChanged("SignleLine2SHide");
            }
        }

        public Visibility SignleLine3SHide
        {
            get
            {
                return _signleLine3SHide;
            }

            set
            {
                _signleLine3SHide = value;
                NotifyPropertyChanged("SignleLine3SHide");
            }
        }

        public Visibility SignleLine4SHide
        {
            get
            {
                return _signleLine4SHide;
            }

            set
            {
                _signleLine4SHide = value;
                NotifyPropertyChanged("SignleLine4SHide");
            }
        }

        public Visibility SignleLine5SHide
        {
            get
            {
                return _signleLine5SHide;
            }
            set
            {
                _signleLine5SHide = value;
                NotifyPropertyChanged("SignleLine5SHide");
            }
        }

        public string BackGroundTincture
        {
            get
            {
                return _backGroundTincture;
            }

            set
            {
                _backGroundTincture = value;
                NotifyPropertyChanged("BackGroundTincture");
            }
        }

        public string ToolTipContent
        {
            get
            {
                return _toolTipContent;
            }

            set
            {
                _toolTipContent = value;
                NotifyPropertyChanged("ToolTipContent");
            }
        }

        public double EllipseRadiueX
        {
            get
            {
                return _ellipseRadiueX;
            }

            set
            {
                _ellipseRadiueX = value;
                NotifyPropertyChanged("EllipseRadiueX");
            }
        }

        public double EllipseRadiueY
        {
            get
            {
                return _ellipseRadiueY;
            }

            set
            {
                _ellipseRadiueY = value;
                NotifyPropertyChanged("EllipseRadiueY");
            }
        }

        public double SignalZeroStn
        {
            get
            {
                return signalZeroStn;
            }

            set
            {
                signalZeroStn = value;
                NotifyPropertyChanged("SignalZeroStn");
            }
        }

        public Point SignalZeroPfSp
        {
            get
            {
                return signalZeroPfSp;
            }

            set
            {
                signalZeroPfSp = value;
                NotifyPropertyChanged("SignalZeroPfSp");
            }
        }

        public Point SignalZeroAsp
        {
            get
            {
                return signalZeroAsp;
            }

            set
            {
                signalZeroAsp = value;
                NotifyPropertyChanged("SignalZeroAsp");
            }
        }

        public Size SignalZeroAss
        {
            get
            {
                return signalZeroAss;
            }

            set
            {
                signalZeroAss = value;
                NotifyPropertyChanged("SignalZeroAss");
            }
        }

        public double SignalOneStn
        {
            get
            {
                return signalOneStn;
            }

            set
            {
                signalOneStn = value;
                NotifyPropertyChanged("SignalOneStn");
            }
        }

        public Point SignalOnePfSp
        {
            get
            {
                return signalOnePfSp;
            }

            set
            {
                signalOnePfSp = value;
                NotifyPropertyChanged("SignalOnePfSp");
            }
        }

        public Point SignalOneAsp
        {
            get
            {
                return signalOneAsp;
            }

            set
            {
                signalOneAsp = value;
                NotifyPropertyChanged("SignalOneAsp");
            }
        }

        public Size SignalOneAss
        {
            get
            {
                return signalOneAss;
            }

            set
            {
                signalOneAss = value;
                NotifyPropertyChanged("SignalOneAss");
            }
        }

        public double SignalTwoStn
        {
            get
            {
                return signalTwoStn;
            }

            set
            {
                signalTwoStn = value;
                NotifyPropertyChanged("SignalTwoStn");
            }
        }

        public Point SignalTwoPfSp
        {
            get
            {
                return signalTwoPfSp;
            }

            set
            {
                signalTwoPfSp = value;
                NotifyPropertyChanged("SignalTwoPfSp");
            }
        }

        public Point SignalTwoAsp
        {
            get
            {
                return signalTwoAsp;
            }

            set
            {
                signalTwoAsp = value;
                NotifyPropertyChanged("SignalTwoAsp");
            }
        }

        public Size SignalTwoAss
        {
            get
            {
                return signalTwoAss;
            }

            set
            {
                signalTwoAss = value;
                NotifyPropertyChanged("SignalTwoAss");
            }
        }

        public double SignalThreeStn
        {
            get
            {
                return signalThreeStn;
            }

            set
            {
                signalThreeStn = value;
                NotifyPropertyChanged("SignalThreeStn");
            }
        }

        public Point SignalThreePfSp
        {
            get
            {
                return signalThreePfSp;
            }

            set
            {
                signalThreePfSp = value;
                NotifyPropertyChanged("SignalThreePfSp");
            }
        }

        public Point SignalThreeAsp
        {
            get
            {
                return signalThreeAsp;
            }

            set
            {
                signalThreeAsp = value;
                NotifyPropertyChanged("SignalThreeAsp");
            }
        }

        public Size SignalThreeAss
        {
            get
            {
                return signalThreeAss;
            }

            set
            {
                signalThreeAss = value;
                NotifyPropertyChanged("SignalThreeAss");
            }
        }

        public double SignalFourStn
        {
            get
            {
                return signalFourStn;
            }

            set
            {
                signalFourStn = value;
                NotifyPropertyChanged("SignalFourStn");
            }
        }

        public Point SignalFourPfSp
        {
            get
            {
                return signalFourPfSp;
            }

            set
            {
                signalFourPfSp = value;
                NotifyPropertyChanged("SignalFourPfSp");
            }
        }

        public Point SignalFourAsp
        {
            get
            {
                return signalFourAsp;
            }

            set
            {
                signalFourAsp = value;
                NotifyPropertyChanged("SignalFourAsp");
            }
        }

        public Size SignalFourAss
        {
            get
            {
                return signalFourAss;
            }

            set
            {
                signalFourAss = value;
                NotifyPropertyChanged("SignalFourAss");
            }
        }

        public double SignalFiveStn
        {
            get
            {
                return signalFiveStn;
            }

            set
            {
                signalFiveStn = value;
                NotifyPropertyChanged("SignalFiveStn");
            }
        }

        public Point SignalFivePfSp
        {
            get
            {
                return signalFivePfSp;
            }

            set
            {
                signalFivePfSp = value;
                NotifyPropertyChanged("SignalFivePfSp");
            }
        }

        public Point SignalFiveAsp
        {
            get
            {
                return signalFiveAsp;
            }

            set
            {
                signalFiveAsp = value;
                NotifyPropertyChanged("SignalFiveAsp");
            }
        }

        public Size SignalFiveAss
        {
            get
            {
                return signalFiveAss;
            }

            set
            {
                signalFiveAss = value;
                NotifyPropertyChanged("SignalFiveAss");
            }
        }

        public Point SignalDot
        {
            get
            {
                return signalDot;
            }

            set
            {
                signalDot = value;
                NotifyPropertyChanged("SignalDot");
            }
        }

        public double SignalSixStn
        {
            get
            {
                return signalSixStn;
            }

            set
            {
                signalSixStn = value;
                NotifyPropertyChanged("SignalSixStn");
            }
        }

        public Point SignalSixPfSp
        {
            get
            {
                return signalSixPfSp;
            }

            set
            {
                signalSixPfSp = value;
                NotifyPropertyChanged("SignalSixPfSp");
            }
        }

        public Point SignalSixAsp
        {
            get
            {
                return signalSixAsp;
            }

            set
            {
                signalSixAsp = value;
                NotifyPropertyChanged("SignalSixAsp");
            }
        }

        public Size SignalSixAss
        {
            get
            {
                return signalSixAss;
            }

            set
            {
                signalSixAss = value;
                NotifyPropertyChanged("SignalSixAss");
            }
        }

        public double SignalSevenStn
        {
            get
            {
                return signalSevenStn;
            }

            set
            {
                signalSevenStn = value;
                NotifyPropertyChanged("SignalSevenStn");
            }
        }

        public Point SignalsSvenPfSp
        {
            get
            {
                return signalsSvenPfSp;
            }

            set
            {
                signalsSvenPfSp = value;
                NotifyPropertyChanged("SignalsSvenPfSp");
            }
        }

        public Point SignalSevenAsp
        {
            get
            {
                return signalSevenAsp;
            }

            set
            {
                signalSevenAsp = value;
                NotifyPropertyChanged("SignalSevenAsp");
            }
        }

        public Size SignalSevenAss
        {
            get
            {
                return signalSevenAss;
            }

            set
            {
                signalSevenAss = value;
                NotifyPropertyChanged("SignalSevenAss");
            }
        }

        public double SignalEightStn
        {
            get
            {
                return signalEightStn;
            }

            set
            {
                signalEightStn = value;
                NotifyPropertyChanged("SignalEightStn");
            }
        }

        public Point SignalEightPfSp
        {
            get
            {
                return signalEightPfSp;
            }

            set
            {
                signalEightPfSp = value;
                NotifyPropertyChanged("SignalEightPfSp");
            }
        }

        public Point SignalEightAsp
        {
            get
            {
                return signalEightAsp;
            }

            set
            {
                signalEightAsp = value;
                NotifyPropertyChanged("SignalEightAsp");
            }
        }

        public Size SignalEightAss
        {
            get
            {
                return signalEightAss;
            }

            set
            {
                signalEightAss = value;
                NotifyPropertyChanged("SignalEightAss");
            }
        }

        public double SignalNineStn
        {
            get
            {
                return signalNineStn;
            }

            set
            {
                signalNineStn = value;
                NotifyPropertyChanged("SignalNineStn");
            }
        }

        public Point SignalNinePfSp
        {
            get
            {
                return signalNinePfSp;
            }

            set
            {
                signalNinePfSp = value;
                NotifyPropertyChanged("SignalNinePfSp");
            }
        }

        public Point SignalNineAsp
        {
            get
            {
                return signalNineAsp;
            }

            set
            {
                signalNineAsp = value;
                NotifyPropertyChanged("SignalNineAsp");
            }
        }

        public Size SignalNineAss
        {
            get
            {
                return signalNineAss;
            }

            set
            {
                signalNineAss = value;
                NotifyPropertyChanged("SignalNineAss");
            }
        }

        public double SignalTenStn
        {
            get
            {
                return signalTenStn;
            }

            set
            {
                signalTenStn = value;
                NotifyPropertyChanged("SignalTenStn");
            }
        }

        public Point SignalTenPfSp
        {
            get
            {
                return signalTenPfSp;
            }

            set
            {
                signalTenPfSp = value;
                NotifyPropertyChanged("SignalTenPfSp");
            }
        }

        public Point SignalTenAsp
        {
            get
            {
                return signalTenAsp;
            }

            set
            {
                signalTenAsp = value;
                NotifyPropertyChanged("SignalTenAsp");
            }
        }

        public Size SignalTenAss
        {
            get
            {
                return signalTenAss;
            }

            set
            {
                signalTenAss = value;
                NotifyPropertyChanged("SignalTenAss");
            }
        }

        public double SignalElevenStn
        {
            get
            {
                return signalElevenStn;
            }

            set
            {
                signalElevenStn = value;
                NotifyPropertyChanged("SignalElevenStn");
            }
        }

        public Point SignalElevenPfSp
        {
            get
            {
                return signalElevenPfSp;
            }

            set
            {
                signalElevenPfSp = value;
                NotifyPropertyChanged("SignalElevenPfSp");
            }
        }

        public Point SignalElevenAsp
        {
            get
            {
                return signalElevenAsp;
            }

            set
            {
                signalElevenAsp = value;
                NotifyPropertyChanged("SignalElevenAsp");
            }
        }

        public Size SignalElevenAss
        {
            get
            {
                return signalElevenAss;
            }

            set
            {
                signalElevenAss = value;
                NotifyPropertyChanged("SignalElevenAss");
            }
        }

        public double ElementScaleCoefficient
        {
            get
            {
                return elementScaleCoefficient;
            }

            set
            {
                elementScaleCoefficient = value;
                NotifyPropertyChanged("ElementScaleCoefficient");
            }
        }

        public void ScaleElement(double scaleCoefficient)
        {
            try
            {
                if (scaleCoefficient > 0 && scaleCoefficient <= 1)
                {
                    //第0个信号
                    SignalZeroStn = 7 * scaleCoefficient;
                    SignalZeroPfSp = new Point(10 * scaleCoefficient, 4 * scaleCoefficient);
                    SignalZeroAsp = new Point(10 * scaleCoefficient, 46 * scaleCoefficient);
                    SignalZeroAss = new Size(50 * scaleCoefficient, 50 * scaleCoefficient);

                    //第1个信号
                    SignalOneStn = 6 * scaleCoefficient;
                    SignalOnePfSp = new Point(30 * scaleCoefficient, 7 * scaleCoefficient);
                    SignalOneAsp = new Point(30 * scaleCoefficient, 43 * scaleCoefficient);
                    SignalOneAss = new Size(50 * scaleCoefficient, 50 * scaleCoefficient);

                    //第2个信号
                    SignalTwoStn = 5 * scaleCoefficient;
                    SignalTwoPfSp = new Point(50 * scaleCoefficient, 10 * scaleCoefficient);
                    SignalTwoAsp = new Point(50 * scaleCoefficient, 40 * scaleCoefficient);
                    SignalTwoAss = new Size(50 * scaleCoefficient, 50 * scaleCoefficient);

                    //第3个信号
                    SignalThreeStn = 4 * scaleCoefficient;
                    SignalThreePfSp = new Point(70 * scaleCoefficient, 13 * scaleCoefficient);
                    SignalThreeAsp = new Point(70 * scaleCoefficient, 37 * scaleCoefficient);
                    SignalThreeAss = new Size(50 * scaleCoefficient, 50 * scaleCoefficient);

                    //第4个信号
                    SignalFourStn = 3 * scaleCoefficient;
                    SignalFourPfSp = new Point(90 * scaleCoefficient, 33 * scaleCoefficient);
                    SignalFourAsp = new Point(90 * scaleCoefficient, 17 * scaleCoefficient);
                    SignalFourAss = new Size(50 * scaleCoefficient, 50 * scaleCoefficient);

                    //第5个信号
                    SignalFiveStn = 2 * scaleCoefficient;
                    SignalFivePfSp = new Point(110 * scaleCoefficient, 30 * scaleCoefficient);
                    SignalFiveAsp = new Point(110 * scaleCoefficient, 20 * scaleCoefficient);
                    SignalFiveAss = new Size(50 * scaleCoefficient, 50 * scaleCoefficient);

                    //中心点
                    SignalDot = new Point(140 * scaleCoefficient, 25 * scaleCoefficient);

                    //第6个信号
                    SignalSixStn = 2 * scaleCoefficient;
                    SignalSixPfSp = new Point(170 * scaleCoefficient, 30 * scaleCoefficient);
                    SignalSixAsp = new Point(170 * scaleCoefficient, 20 * scaleCoefficient);
                    SignalSixAss = new Size(50 * scaleCoefficient, 50 * scaleCoefficient);

                    //第7个信号
                    SignalSevenStn = 3 * scaleCoefficient;
                    SignalsSvenPfSp = new Point(190 * scaleCoefficient, 33 * scaleCoefficient);
                    SignalSevenAsp = new Point(190 * scaleCoefficient, 17 * scaleCoefficient);
                    SignalSevenAss = new Size(50 * scaleCoefficient, 50 * scaleCoefficient);

                    //第8个信号
                    SignalEightStn = 4 * scaleCoefficient;
                    SignalEightPfSp = new Point(210 * scaleCoefficient, 13 * scaleCoefficient);
                    SignalEightAsp = new Point(210 * scaleCoefficient, 37 * scaleCoefficient);
                    SignalEightAss = new Size(50 * scaleCoefficient, 50 * scaleCoefficient);

                    //第9个信号
                    SignalNineStn = 5 * scaleCoefficient;
                    SignalNinePfSp = new Point(230 * scaleCoefficient, 10 * scaleCoefficient);
                    SignalNineAsp = new Point(230 * scaleCoefficient, 40 * scaleCoefficient);
                    SignalNineAss = new Size(50 * scaleCoefficient, 50 * scaleCoefficient);

                    //第10个信号
                    SignalTenStn = 6 * scaleCoefficient;
                    SignalTenPfSp = new Point(250 * scaleCoefficient, 7 * scaleCoefficient);
                    SignalTenAsp = new Point(250 * scaleCoefficient, 43 * scaleCoefficient);
                    SignalTenAss = new Size(50 * scaleCoefficient, 50 * scaleCoefficient);

                    //第11个信号
                    SignalElevenStn = 7 * scaleCoefficient;
                    SignalElevenPfSp = new Point(270 * scaleCoefficient, 4 * scaleCoefficient);
                    SignalElevenAsp = new Point(270 * scaleCoefficient, 46 * scaleCoefficient);
                    SignalElevenAss = new Size(50 * scaleCoefficient, 50 * scaleCoefficient);

                    //比列系数
                    ElementScaleCoefficient = scaleCoefficient;
                }
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("缩放未知设备提示器内部异常,当前系数(" + scaleCoefficient.ToString() + ")", Ex.Message, Ex.StackTrace);
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

    #region 后台服务器基本参数
    public class ServerBaseParameterClass : INotifyPropertyChanged
    {
        private string strDbIpAddr;                 //数据库IP地址    
        private string logOutputLevel;              //DEBG = "0", INFO = "1",WARN = "2", EROR = "3"
        private string strFtpIpAddr;                //FTP服务器IP地址
        private string strFtpUserId;                //FTP用户名
        private string strFtpUserPsw;               //FTP用户密码
        private string strFtpPort;                  //FTP端口
        private string strFtpUpdateDir;             //FTP的更新路径
        private string strStartPortCDMA_ZYF;        //CDMA，ZYF的端口
        private string strStartPortGSM_ZYF;         //GSM，ZYF的端口
        private string strStartPortGSM_HJT;         //GSM，HJT的端口
        private string strStartPortLTE;             //LTE的端口
        private string strStartPortTDS;             //TDS的端口
        private string strStartPortWCDMA;           //WCDMA的端口
        private string strStartPortAppWindows;      //Windows APP的端口
        private string strStartPortAppLinux;        //Linux APP的端口
        private string strStartPortAppAndroid;      //Android APP的端口
        private string dataAlignMode;               //数据对齐基准:"0"数据库为基准，"1"以Ap为基准
        private string logMaxSize;                  //每个Log文件的大小，单位为MB

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string value)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(value));
            }
        }

        public string StrDbIpAddr
        {
            get
            {
                return strDbIpAddr;
            }

            set
            {
                strDbIpAddr = value;
                NotifyPropertyChanged("StrDbIpAddr");
            }
        }

        public string LogOutputLevel
        {
            get
            {
                return logOutputLevel;
            }

            set
            {
                logOutputLevel = value;
                NotifyPropertyChanged("LogOutputLevel");
            }
        }

        public string StrFtpIpAddr
        {
            get
            {
                return strFtpIpAddr;
            }

            set
            {
                strFtpIpAddr = value;
                NotifyPropertyChanged("StrFtpIpAddr");
            }
        }

        public string StrFtpUserId
        {
            get
            {
                return strFtpUserId;
            }

            set
            {
                strFtpUserId = value;
                NotifyPropertyChanged("StrFtpUserId");
            }
        }

        public string StrFtpUserPsw
        {
            get
            {
                return strFtpUserPsw;
            }

            set
            {
                strFtpUserPsw = value;
                NotifyPropertyChanged("StrFtpUserPsw");
            }
        }

        public string StrFtpPort
        {
            get
            {
                return strFtpPort;
            }

            set
            {
                strFtpPort = value;
                NotifyPropertyChanged("StrFtpPort");
            }
        }

        public string StrFtpUpdateDir
        {
            get
            {
                return strFtpUpdateDir;
            }

            set
            {
                strFtpUpdateDir = value;
                NotifyPropertyChanged("StrFtpUpdateDir");
            }
        }

        public string StrStartPortCDMA_ZYF
        {
            get
            {
                return strStartPortCDMA_ZYF;
            }

            set
            {
                strStartPortCDMA_ZYF = value;
                NotifyPropertyChanged("StrStartPortCDMA_ZYF");
            }
        }

        public string StrStartPortGSM_ZYF
        {
            get
            {
                return strStartPortGSM_ZYF;
            }

            set
            {
                strStartPortGSM_ZYF = value;
                NotifyPropertyChanged("StrStartPortGSM_ZYF");
            }
        }

        public string StrStartPortGSM_HJT
        {
            get
            {
                return strStartPortGSM_HJT;
            }

            set
            {
                strStartPortGSM_HJT = value;
                NotifyPropertyChanged("StrStartPortGSM_HJT");
            }
        }

        public string StrStartPortLTE
        {
            get
            {
                return strStartPortLTE;
            }

            set
            {
                strStartPortLTE = value;
                NotifyPropertyChanged("StrStartPortLTE");
            }
        }

        public string StrStartPortTDS
        {
            get
            {
                return strStartPortTDS;
            }

            set
            {
                strStartPortTDS = value;
                NotifyPropertyChanged("StrStartPortTDS");
            }
        }

        public string StrStartPortWCDMA
        {
            get
            {
                return strStartPortWCDMA;
            }

            set
            {
                strStartPortWCDMA = value;
                NotifyPropertyChanged("StrStartPortWCDMA");
            }
        }

        public string StrStartPortAppWindows
        {
            get
            {
                return strStartPortAppWindows;
            }

            set
            {
                strStartPortAppWindows = value;
                NotifyPropertyChanged("StrStartPortAppWindows");
            }
        }

        public string StrStartPortAppLinux
        {
            get
            {
                return strStartPortAppLinux;
            }

            set
            {
                strStartPortAppLinux = value;
                NotifyPropertyChanged("StrStartPortAppLinux");
            }
        }

        public string StrStartPortAppAndroid
        {
            get
            {
                return strStartPortAppAndroid;
            }

            set
            {
                strStartPortAppAndroid = value;
                NotifyPropertyChanged("StrStartPortAppAndroid");
            }
        }

        public string DataAlignMode
        {
            get
            {
                return dataAlignMode;
            }

            set
            {
                dataAlignMode = value;
                NotifyPropertyChanged("DataAlignMode");
            }
        }

        public string LogMaxSize
        {
            get
            {
                return logMaxSize;
            }

            set
            {
                logMaxSize = value;
                NotifyPropertyChanged("LogMaxSize");
            }
        }
    }
    #endregion

    #region 主窗口设置参数
    public class MainWinControlParameterClass : INotifyPropertyChanged
    {
        private bool _allWin;
        private bool _deviceListWin;
        private bool _scannerWin;
        private bool _blackListWin;
        private bool _systemLogsWin;

        public bool AllWin
        {
            get
            {
                return _allWin;
            }

            set
            {
                _allWin = value;
                NotifyPropertyChanged("AllWin");
            }
        }

        public bool DeviceListWin
        {
            get
            {
                return _deviceListWin;
            }

            set
            {
                _deviceListWin = value;
                NotifyPropertyChanged("DeviceListWin");
            }
        }

        public bool ScannerWin
        {
            get
            {
                return _scannerWin;
            }

            set
            {
                _scannerWin = value;
                NotifyPropertyChanged("ScannerWin");
            }
        }

        public bool BlackListWin
        {
            get
            {
                return _blackListWin;
            }

            set
            {
                _blackListWin = value;
                NotifyPropertyChanged("BlackListWin");
            }
        }

        public bool SystemLogsWin
        {
            get
            {
                return _systemLogsWin;
            }

            set
            {
                _systemLogsWin = value;
                NotifyPropertyChanged("SystemLogsWin");
            }
        }

        public int GettingStatu()
        {
            int Flag = 0;
            try
            {
                //1 -->
                if (AllWin && !DeviceListWin && !ScannerWin && !BlackListWin && !SystemLogsWin)
                {
                    Flag = 1;
                }
                else if (!AllWin && DeviceListWin && !ScannerWin && !BlackListWin && !SystemLogsWin)
                {
                    Flag = 2;
                }
                else if (!AllWin && !DeviceListWin && ScannerWin && !BlackListWin && !SystemLogsWin)
                {
                    Flag = 3;
                }
                else if (!AllWin && !DeviceListWin && !ScannerWin && BlackListWin && !SystemLogsWin)
                {
                    Flag = 4;
                }
                else if (!AllWin && !DeviceListWin && !ScannerWin && !BlackListWin && SystemLogsWin)
                {
                    Flag = 5;
                }
                //6 -->
                else if (!AllWin && DeviceListWin && ScannerWin && !BlackListWin && !SystemLogsWin)
                {
                    Flag = 6;
                }
                else if (!AllWin && DeviceListWin && !ScannerWin && BlackListWin && !SystemLogsWin)
                {
                    Flag = 7;
                }
                else if (!AllWin && DeviceListWin && !ScannerWin && !BlackListWin && SystemLogsWin)
                {
                    Flag = 8;
                }
                else if (!AllWin && !DeviceListWin && ScannerWin && BlackListWin && !SystemLogsWin)
                {
                    Flag = 9;
                }
                else if (!AllWin && !DeviceListWin && ScannerWin && !BlackListWin && SystemLogsWin)
                {
                    Flag = 10;
                }
                else if (!AllWin && !DeviceListWin && !ScannerWin && BlackListWin && SystemLogsWin)
                {
                    Flag = 11;
                }
                //12 -->
                else if (!AllWin && DeviceListWin && ScannerWin && BlackListWin && !SystemLogsWin)
                {
                    Flag = 12;
                }
                else if (!AllWin && DeviceListWin && ScannerWin && !BlackListWin && SystemLogsWin)
                {
                    Flag = 13;
                }
                else if (!AllWin && DeviceListWin && !ScannerWin && BlackListWin && SystemLogsWin)
                {
                    Flag = 14;
                }
                else if (!AllWin && !DeviceListWin && ScannerWin && BlackListWin && SystemLogsWin)
                {
                    Flag = 15;
                }
                else
                {
                    Flag = 1;
                }
            }
            catch (Exception Ex)
            {
                Flag = 1;
                Parameters.PrintfLogsExtended("窗口显示控制配置获取失败", Ex.Message, Ex.StackTrace);
            }
            return Flag;
        }

        public void SettingStatu()
        {
            try
            {
                Parameters.SendMessage(Parameters.WinHandle, Parameters.WM_SubWindowsConfigurationMessage, 0, 0);
                string value = AllWin.ToString() + ";" +
                                DeviceListWin.ToString() + ";" +
                                ScannerWin.ToString() + ";" +
                                BlackListWin.ToString() + ";" +
                                SystemLogsWin.ToString();


                Parameters.WriteIniFile("WinControl", "Status", new DesEncrypt().Encrypt(value, new DefineCode().Code()), Parameters.INIFile);

                new Thread(() =>
                {
                    System.Windows.MessageBox.Show("设置成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }).Start();
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("窗口显示控制配置失败", Ex.Message, Ex.StackTrace);
                new Thread(() =>
                {
                    System.Windows.MessageBox.Show("设置失败," + Ex.Message + "！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }).Start();
            }
        }

        public void SettingStatu(bool allWin, bool deviceListWin, bool scannerWin, bool blackListWin, bool systemLogsWin)
        {
            try
            {
                AllWin = allWin;
                DeviceListWin = deviceListWin;
                ScannerWin = scannerWin;
                BlackListWin = blackListWin;
                SystemLogsWin = systemLogsWin;

                int value = Convert.ToInt32(AllWin) +
                            Convert.ToInt32(DeviceListWin) +
                            Convert.ToInt32(ScannerWin) +
                            Convert.ToInt32(BlackListWin) +
                            Convert.ToInt32(SystemLogsWin);

                Parameters.WriteIniFile("WinControl", "Status", value.ToString(), Parameters.INIFile);
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("窗口显示控制配置", Ex.Message, Ex.StackTrace);
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

    #region 捕号窗口与黑名单窗口，通话记录窗口，短信记录窗口参数
    public class DataReportWindowsControlParameterClass : WindowsControlParameterClass
    {
        #region 字段
        //公用参数
        private Visibility uEInfoWndVisibility = Visibility.Visible;
        private int uEInfoWndRow = 1;
        private int uEInfoWndColumn = 0;
        private int uEInfoWndRowSpan = 3;
        private int uEInfoWndColumnSpan = 4;
        private Visibility scannerWndVisibility = Visibility.Visible;
        private Visibility scannerWndLineVisibility = Visibility.Visible;
        private Visibility functionWndsAreaVisibility = Visibility.Visible;
        private int pageIndex = 0;

        //显示捕号窗口参数
        private int scannerWndRow = 0;
        private int scannerWndColumn = 0;
        private int scannerWndRowSpan = 1;
        private int scannerWndColumnSpan = 1;

        //显示译码器捕号窗口参数
        //......

        //窗口移动条
        private int scannerWndLineRow = 1;
        private int scannerWndLineColumn = 0;
        private int scannerWndLineRowSpan = 1;
        private int scannerWndLineColumnSpan = 1;

        //显示黑名单窗口参数
        private int functionWndAreaRow = 2;
        private int functionWndAreaColumn = 0;
        private int functionWndAreaRowSpan = 1;
        private int functionWndAreaColumnSpan = 1;
        private Visibility measurementReportItem = Visibility.Visible;

        //显示通话记录窗口参数
        private Visibility callRecordeItem = Visibility.Visible;

        //显示短信记录窗口参数
        private Visibility sMSRecordeItem = Visibility.Visible;

        public Visibility UEInfoWndVisibility
        {
            get
            {
                return uEInfoWndVisibility;
            }

            set
            {
                uEInfoWndVisibility = value;
                NotifyPropertyChanged("UEInfoWndVisibility");
            }
        }

        public int UEInfoWndRow
        {
            get
            {
                return uEInfoWndRow;
            }

            set
            {
                uEInfoWndRow = value;
                NotifyPropertyChanged("UEInfoWndRow");
            }
        }

        public int UEInfoWndColumn
        {
            get
            {
                return uEInfoWndColumn;
            }

            set
            {
                uEInfoWndColumn = value;
                NotifyPropertyChanged("UEInfoWndColumn");
            }
        }

        public int UEInfoWndRowSpan
        {
            get
            {
                return uEInfoWndRowSpan;
            }

            set
            {
                uEInfoWndRowSpan = value;
                NotifyPropertyChanged("UEInfoWndRowSpan");
            }
        }

        public int UEInfoWndColumnSpan
        {
            get
            {
                return uEInfoWndColumnSpan;
            }

            set
            {
                uEInfoWndColumnSpan = value;
                NotifyPropertyChanged("UEInfoWndColumnSpan");
            }
        }

        public Visibility ScannerWndVisibility
        {
            get
            {
                return scannerWndVisibility;
            }

            set
            {
                scannerWndVisibility = value;
                NotifyPropertyChanged("ScannerWndVisibility");
            }
        }

        public Visibility ScannerWndLineVisibility
        {
            get
            {
                return scannerWndLineVisibility;
            }

            set
            {
                scannerWndLineVisibility = value;
                NotifyPropertyChanged("ScannerWndLineVisibility");
            }
        }

        public Visibility FunctionWndsAreaVisibility
        {
            get
            {
                return functionWndsAreaVisibility;
            }

            set
            {
                functionWndsAreaVisibility = value;
                NotifyPropertyChanged("FunctionWndsAreaVisibility");
            }
        }

        public int ScannerWndRow
        {
            get
            {
                return scannerWndRow;
            }

            set
            {
                scannerWndRow = value;
                NotifyPropertyChanged("ScannerWndRow");
            }
        }

        public int ScannerWndColumn
        {
            get
            {
                return scannerWndColumn;
            }

            set
            {
                scannerWndColumn = value;
                NotifyPropertyChanged("ScannerWndColumn");
            }
        }

        public int ScannerWndRowSpan
        {
            get
            {
                return scannerWndRowSpan;
            }

            set
            {
                scannerWndRowSpan = value;
                NotifyPropertyChanged("ScannerWndRowSpan");
            }
        }

        public int ScannerWndColumnSpan
        {
            get
            {
                return scannerWndColumnSpan;
            }

            set
            {
                scannerWndColumnSpan = value;
                NotifyPropertyChanged("ScannerWndColumnSpan");
            }
        }

        public int FunctionWndAreaRow
        {
            get
            {
                return functionWndAreaRow;
            }

            set
            {
                functionWndAreaRow = value;
                NotifyPropertyChanged("FunctionWndAreaRow");
            }
        }

        public int FunctionWndAreaColumn
        {
            get
            {
                return functionWndAreaColumn;
            }

            set
            {
                functionWndAreaColumn = value;
                NotifyPropertyChanged("FunctionWndAreaColumn");
            }
        }

        public int FunctionWndAreaRowSpan
        {
            get
            {
                return functionWndAreaRowSpan;
            }

            set
            {
                functionWndAreaRowSpan = value;
                NotifyPropertyChanged("FunctionWndAreaRowSpan");
            }
        }

        public int FunctionWndAreaColumnSpan
        {
            get
            {
                return functionWndAreaColumnSpan;
            }

            set
            {
                functionWndAreaColumnSpan = value;
                NotifyPropertyChanged("FunctionWndAreaColumnSpan");
            }
        }

        public Visibility MeasurementReportItem
        {
            get
            {
                return measurementReportItem;
            }

            set
            {
                measurementReportItem = value;
                NotifyPropertyChanged("MeasurementReportItem");
            }
        }

        public Visibility CallRecordeItem
        {
            get
            {
                return callRecordeItem;
            }

            set
            {
                callRecordeItem = value;
                NotifyPropertyChanged("CallRecordeItem");
            }
        }

        public Visibility SMSRecordeItem
        {
            get
            {
                return sMSRecordeItem;
            }

            set
            {
                sMSRecordeItem = value;
                NotifyPropertyChanged("SMSRecordeItem");
            }
        }

        public int ScannerWndLineRow
        {
            get
            {
                return scannerWndLineRow;
            }

            set
            {
                scannerWndLineRow = value;
                NotifyPropertyChanged("ScannerWndLineRow");
            }
        }

        public int ScannerWndLineColumn
        {
            get
            {
                return scannerWndLineColumn;
            }

            set
            {
                scannerWndLineColumn = value;
                NotifyPropertyChanged("ScannerWndLineColumn");
            }
        }

        public int ScannerWndLineRowSpan
        {
            get
            {
                return scannerWndLineRowSpan;
            }

            set
            {
                scannerWndLineRowSpan = value;
                NotifyPropertyChanged("ScannerWndLineRowSpan");
            }
        }

        public int ScannerWndLineColumnSpan
        {
            get
            {
                return scannerWndLineColumnSpan;
            }

            set
            {
                scannerWndLineColumnSpan = value;
                NotifyPropertyChanged("ScannerWndLineColumnSpan");
            }
        }

        public int PageIndex
        {
            get
            {
                return pageIndex;
            }

            set
            {
                pageIndex = value;
                NotifyPropertyChanged("PageIndex");
            }
        }
        #endregion

        /// <summary>
        /// 显示捕号窗口
        /// </summary>
        public void ScannerWindowControl()
        {
            ScannerWndVisibility = Visibility.Visible;
            ScannerWndLineVisibility = Visibility.Collapsed;
            FunctionWndsAreaVisibility = Visibility.Collapsed;
            ScannerWndRow = 0;
            ScannerWndColumn = 0;
            ScannerWndRowSpan = 3;
            ScannerWndColumnSpan = 1;
        }

        //显示译码器捕号窗口

        /// <summary>
        /// 显示黑名单窗口,显示通话记录窗口,显示短信记录窗口
        /// </summary>
        /// <param name="tabIndex"></param>
        /// <param name="blackListWnd"></param>
        /// <param name="callRecordeWnd"></param>
        /// <param name="smsRecordeWnd"></param>
        public void FunctionWindowsAreaControl(int tabIndex, Visibility blackListWnd, Visibility callRecordeWnd, Visibility smsRecordeWnd)
        {
            ScannerWndVisibility = Visibility.Collapsed;
            ScannerWndLineVisibility = Visibility.Collapsed;
            FunctionWndsAreaVisibility = Visibility.Visible;
            FunctionWndAreaRow = 0;
            FunctionWndAreaColumn = 0;
            FunctionWndAreaRowSpan = 3;
            FunctionWndAreaColumnSpan = 1;
            PageIndex = tabIndex;
            MeasurementReportItem = blackListWnd;
            CallRecordeItem = callRecordeWnd;
            SMSRecordeItem = smsRecordeWnd;
        }

        /// <summary>
        /// 全部正常显示
        /// </summary>
        public void AllElementNormalShow()
        {
            #region 捕号窗口，黑名单，通话记录，短信记录，译码器捕号窗口
            //捕号窗口
            ScannerWndVisibility = Visibility.Visible;
            ScannerWndRow = 0;
            ScannerWndColumn = 0;
            ScannerWndRowSpan = 1;
            ScannerWndColumnSpan = 1;

            //移动条
            ScannerWndLineVisibility = Visibility.Visible;
            ScannerWndLineRow = 0;
            ScannerWndColumn = 0;
            ScannerWndRowSpan = 1;
            ScannerWndColumnSpan = 1;

            //黑名单窗口
            FunctionWndsAreaVisibility = Visibility.Visible;
            FunctionWndAreaRow = 2;
            FunctionWndAreaColumn = 0;
            FunctionWndAreaRowSpan = 1;
            FunctionWndAreaColumnSpan = 1;
            PageIndex = 0;

            //表项
            MeasurementReportItem = Visibility.Visible;
            CallRecordeItem = Visibility.Visible;
            SMSRecordeItem = Visibility.Visible;
            #endregion
        }
    }
    #endregion

    /// <summary>
    /// 构造
    /// </summary>
    public class Parameters
    {
        //API 接口
        [DllImport("ws2_32.dll")]
        private static extern int inet_addr(string cp);
        [DllImport("IPHLPAPI.dll")]
        private static extern int SendARP(Int32 DestIP, Int32 SrcIP, ref Int64 pMacAddr, ref Int32 PhyAddrLen);

        #region 字段定义
        public static readonly byte ZERO = 0;
        public static object UseObject = new object();
        private static string languageType = "CN";
        private static string logsDir = string.Empty;
        private static string logsFile = string.Empty;
        private static string logsDbFile = null;
        private static string apLogDir = string.Empty;
        private static string iNIFile = null;
        private static readonly LoginUserLevel loginUserLevel = new LoginUserLevel();
        private static int pLMN_Lengh = 5;
        private static byte heartStatu = 1;
        public static int UniversalCounter = 0;
        public static readonly int GSMMaxSMSCount = 665;
        public static readonly int GSMV2MaxSMSCount = 70;
        public static readonly int CDMAMaxSMSCount = 60;
        public static EncodingType CheckFileEncodingType = new EncodingType();
        public static String UDPHost = "127.0.0.1";
        public static int UDPPort = 6601;

        //基本消息内容
        public static string WelcomTips = "欢迎使用智能通讯管控管理系统";

        //支持最大AP数
        public static int APMax = 410;

        //日志文件编号
        public static int LogFileNumber = 0;

        //日志文件大小(MB)
        public static long LogFileSize = 5 * 1024 * 1024;

        //黑名单追踪曲线图显示方式 0/1
        public static int ChartAxialModel = 1;
        public static int MeasureReportTotal = 16;

        //不关注黑名单追踪曲线图时是否停止 0/1
        public static int ChartAxialEnable = 0;

        //检测网络断连延时
        public static int NetCheckDelayFactor = 5;

        //批量设置特殊名单失败最大延时
        public static int SpecialListInputDelay = 60;

        //CDMA,GSMV2 短信IMSI缓存个数
        public static int SMSIMSITotal = 5000;

        //保存事件日志线程
        #region 事件日志信息类
        public class EventLogsInfoClass
        {
            private List<StringBuilder> MsgList = null;
            private Thread PrintfEventLogsThread = null;
            public object SaveLock = null;

            public EventLogsInfoClass()
            {
                if (MsgList == null)
                {
                    MsgList = new List<StringBuilder>();
                }

                if (PrintfEventLogsThread == null)
                {
                    PrintfEventLogsThread = new Thread(new ThreadStart(this.Save));
                    PrintfEventLogsThread.Start();
                }

                if (SaveLock == null)
                {
                    SaveLock = new object();
                }
            }

            public void Input(params string[] Msg)
            {
                lock (SaveLock)
                {
                    if (Msg.Length > 0)
                    {
                        StringBuilder LogsItem = new StringBuilder();
                        for (int i = 0; i < Msg.Length; i++)
                        {
                            LogsItem.AppendLine(Msg[i]);
                        }
                        MsgList.Add(LogsItem);
                    }
                }
            }

            private void Save()
            {
                while (true)
                {
                    try
                    {
                        string DTime = string.Format("{0:D4}", DateTime.Now.Year) + "-" + string.Format("{0:D2}", DateTime.Now.Month) + "-" + string.Format("{0:D2}", DateTime.Now.Day);
                        logsFile = DTime + "_" + LogFileNumber.ToString() + ".txt";
                        if (File.Exists(LogsDir + @"\" + DTime + @"\" + logsFile))
                        {
                            System.IO.FileInfo _logFileSize = new FileInfo(LogsDir + @"\" + DTime + @"\" + logsFile);
                            if (_logFileSize.Length >= LogFileSize)
                            {
                                LogFileNumber++;
                                logsFile = DTime + "_" + LogFileNumber.ToString() + ".txt";
                            }
                        }

                        lock (SaveLock)
                        {
                            if (MsgList.Count > 0)
                            {
                                string STime = string.Format("{0:D2}", DateTime.Now.Hour) + ":" + string.Format("{0:D2}", DateTime.Now.Minute) + ":" + string.Format("{0:D2}", DateTime.Now.Second);
                                if (Directory.Exists(LogsDir + @"\" + DTime))
                                {
                                    File.AppendAllText(LogsDir + @"\" + DTime + @"\" + logsFile, Environment.NewLine + "============ " + DTime + " " + STime + " ============" + Environment.NewLine + MsgList[0].ToString());
                                }
                                else
                                {
                                    Directory.CreateDirectory(LogsDir + @"\" + DTime);
                                    File.AppendAllText(LogsDir + @"\" + DTime + @"\" + logsFile, Environment.NewLine + "============ " + DTime + " " + STime + " ============" + Environment.NewLine + MsgList[0].ToString());
                                }

                                MsgList.RemoveAt(0);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        printfLogs("日志存储失败", ex.Message, ex.StackTrace);
                    }

                    Thread.Sleep(50);
                }
            }
        }
        #endregion

        //LTE黑白名单(IMSI)最大提交数量
        public static UserTypesClass UserTypes = new UserTypesClass();
        public static int BWListIMSIConfigurationTotal = 16;

        //日志输出控制
        public static byte LogStatus = 0; //0 保存到库文件  1 保存到文本文件 2 不保存任何信息
        public static byte DebugLogs = 1;
        public static EventLogsInfoClass EventLogsInfoOutPut = new EventLogsInfoClass();

        //实时日志显示总数
        private static int sysLogsTotal = 1000;

        //是否预览GSM信息
        private static bool sMSBrowse = false;

        //字符集编码
        public static byte CharactorCode = Convert.ToByte(ParameterControl.CharactorCodeType.UTF8);

        //---登录默认信息---
        private static string loginUserName = string.Empty;
        private static bool loginDefault = false;
        private static string loginPassWord = string.Empty;
        private static string loginServer = string.Empty;
        private static string tcpServerHost = "127.0.0.1";
        private static int tcpServerPort = 14789;
        //---------------

        //通用文件类型
        public static FileTypeExClass FileTypeEx = new FileTypeExClass();

        //文件类型对应图标
        public static FileTypeIcon FileIcon = new FileTypeIcon();

        private static int loginTimeOut = 5;
        private static byte delayTime = 5;
        private static string configType = string.Empty;
        private static string userLoginPermit = "success";
        private static string userLoginDeny = "failed";
        private static string actionType = string.Empty;
        private static string getSYNCInfoType = string.Empty;
        private static int completeCount = 0;

        //未知设备屏蔽开/关
        public static bool STRefresh = true;
        //未知设备默认参数
        public static readonly string ToStationDefault = "未指定将未知设备添加到哪个站点";
        public static readonly string UnknownDeviceNameDefault = "device_unknown";

        //心跳延时(默认60秒)
        private static int heartTime = 60;

        //FTP用户名、密码、目录
        private static string downLoadLogUser = "loguser";
        private static string downLoadLogPass = "loguser";
        private static string downLoadLogRoot = "/";
        private static int downLoadLogPort = 21;
        private static bool downLoadLogLastDirWithSN = true;

        //设备管理窗口句柄
        private static IntPtr deviceManageWinHandle = IntPtr.Zero;
        //特殊名单窗口句柄
        private static IntPtr speciallistWinHandle = IntPtr.Zero;
        //主窗口句柄
        private static IntPtr winHandle = IntPtr.Zero;
        //历史查询窗口句柄
        private static IntPtr historyDataWinHandle = IntPtr.Zero;
        //域管理窗体句柄
        private static IntPtr domainManageWinHandle = IntPtr.Zero;
        //添加黑白名单窗体句柄
        private static IntPtr addBWListWinHandle = IntPtr.Zero;
        //未知设备列表窗口句柄
        private static IntPtr unknownDeviceWinHandle = IntPtr.Zero;
        //未知设备提示器窗口句柄
        private static IntPtr unknownDeviceTipsWinHandle = IntPtr.Zero;
        //用户管理窗体句柄
        private static IntPtr userManagerWinHandle = IntPtr.Zero;
        //批量激活与去激活进度窗口句柄
        private static IntPtr volumeActiveWinHandle = IntPtr.Zero;
        //高级设置窗口句柄
        private static IntPtr advanceSettingWinHandle = IntPtr.Zero;
        //欢迎窗口句柄
        private static IntPtr welcomeWindowHandle = IntPtr.Zero;
        //地图显示窗口句柄
        private static IntPtr showDBMapWinHandle = IntPtr.Zero;
        //分时显示
        private static IntPtr statisticalWinHandle = IntPtr.Zero;
        //退出提示窗口句柄
        private static IntPtr tipsWinCloseHandle = IntPtr.Zero;
        //批量重启AP窗口句柄
        private static IntPtr batchRebootApWinHandle = IntPtr.Zero;

        //升级超时默认5分钟
        private static int upDonwLoadTimeOutValue = 5;
        //升级包存放路径
        private static string upDateFileSourceDir = "/Update";
        //登录成功
        public static bool Logined = false;

        private static ScannerDataControlClass scannerDataControlParameter = new ScannerDataControlClass();
        private static DomainActionInfo domainActionInfoClass = new DomainActionInfo();
        private static DeviceActionInfo deviceActionInfoClass = new DeviceActionInfo();
        private static UserMousePosition userMousePosition = new UserMousePosition();
        private static APResoultMsgTypeClass aPResourltMsgType = new APResoultMsgTypeClass();
        private static APResoultRebootMsgTypeClss aPResoultRebootMsgType = new APResoultRebootMsgTypeClss();
        private static RedirectionClass redirectionParam = new RedirectionClass();
        public static WindowsControlParameterClass WindowsControlParameter = new WindowsControlParameterClass();
        public static UnknownDeviceWindowControlParametersClass UnknownDeviceWindowControlParameters = new UnknownDeviceWindowControlParametersClass();
        public static ServerBaseParameterClass ServerBaseParameter = new ServerBaseParameterClass();
        public static MainWinControlParameterClass MainWinControlParameter = new MainWinControlParameterClass();
        public static DataReportWindowsControlParameterClass DataReportWindowsControl = new DataReportWindowsControlParameterClass();

        //系统事件消息
        public readonly static int WM_HIDE = 0;
        public readonly static int WM_NORMAL = 1;
        public readonly static int WM_MAXIMIZE = 3;
        public readonly static int WM_SHOWNOACTIVATE = 4;
        public readonly static int WM_SHOW = 5;
        public readonly static int WM_MINIMIZE = 6;
        public readonly static int WM_RESTORE = 9;
        public readonly static int WM_SHOWDEFAULT = 10;
        public readonly static int WM_CLOSE = 0x10;
        public readonly static int BM_CLICK = 0xF5;

        //用户事件消息
        public readonly static int WM_USER = 0x0400;
        public readonly static int WM_DeviceTreeViewListing = WM_USER + 100;
        public readonly static int WM_DeviceListInfoLoad = WM_USER + 101;
        public readonly static int WM_MainWinActive = WM_USER + 200;
        public readonly static int WM_RequestDeviceLists = WM_USER + 201;
        public readonly static int WM_AddDomainNameResponse = WM_USER + 202;
        public readonly static int WM_AddDeviceNameResponse = WM_USER + 203;
        public readonly static int WM_ReNameDomainNameResponse = WM_USER + 204;
        public readonly static int WM_ReNameDeviceNameResponse = WM_USER + 205;
        public readonly static int WM_DeleteDomainNameResponse = WM_USER + 206;
        public readonly static int WM_DeleteDeviceNameResponse = WM_USER + 207;
        public readonly static int WM_UpdateDeviceInfoResponse = WM_USER + 208;
        public readonly static int WM_UpdateCellNeighConfigrationResponse = WM_USER + 209;
        public readonly static int WM_GPSConfigrationResponse = WM_USER + 210;
        public readonly static int WM_NTPConfigrationResponse = WM_USER + 211;
        public readonly static int WM_SyncSourceConfigrationResponse = WM_USER + 212;
        public readonly static int WM_APPeriodTimeConfigrationResponse = WM_USER + 213;
        public readonly static int WM_ScannerFrequencyConfigrationResponse = WM_USER + 214;
        public readonly static int WM_GetDeviceDetailResponse = WM_USER + 215;
        public readonly static int WM_UpgradeAPSystemResponse = WM_USER + 216;
        public readonly static int WM_GetBlackListResponse = WM_USER + 217;
        public readonly static int WM_GetWhiteListResponse = WM_USER + 218;
        public readonly static int WM_GetHistoryDataResponse = WM_USER + 219;                                     //历史数据查询
        public readonly static int WM_GetHistoryDataToCSVResponse = WM_USER + 220;                         //历史数据导出
        public readonly static int WM_DeviceManageWinTreeViewReLoade = WM_USER + 221;
        public readonly static int WM_DeviceManageGenParaResponse = WM_USER + 222;
        public readonly static int WM_RedirectConfigurationResponse = WM_USER + 223;

        //AP激活操作
        public readonly static int WM_APActiveResponse = WM_USER + 224;
        public readonly static int WM_APNoActiveResponse = WM_USER + 225;
        //CDMA小区参数
        public readonly static int WM_CDMACellParameterResponse = WM_USER + 226;
        //CDMA IMSI 添加响应消息
        public readonly static int WM_CDMAIMSIConfigWithAddResponse = WM_USER + 227;
        //CDMA IMSI 删除全部响应消息
        public readonly static int WM_CDMAIMSIConfigWithClearResponse = WM_USER + 228;
        //CDMA IMSI 删除部分响应消息
        public readonly static int WM_CDMAIMSIConfigWithDeletePartResponse = WM_USER + 229;
        //CDMA IMSI 查询响应消息
        public readonly static int WM_CDMAIMSIConfigWithQueryResponse = WM_USER + 230;
        //黑白名单选中设备查询事件
        public readonly static int WM_BWListQueryRequest = WM_USER + 231;
        //获取所有设备详细信息消息
        public readonly static int WM_GettingAllDeviceDetailMessage = WM_USER + 232;
        //多PLMN，周期频点响应消息
        public readonly static int WM_GettingOtherPLMNListResponse = WM_USER + 233;
        //未知设备显示控制
        public readonly static int WM_UnknownDeviceWinStatusControlToOpen = WM_USER + 234;
        public readonly static int WM_UnknownDeviceWinStatusControlToClose = WM_USER + 235;
        //更改Scanner数据表背景色
        public readonly static int WM_ChangeScannerDataRowsBackGroundColor = WM_USER + 236;
        public readonly static int WM_RestartPlayerThreadRequest = WM_USER + 237;

        //------窗口显示控制事件------
        public readonly static int WM_ShowScannerWindowControl = WM_USER + 238;
        public readonly static int WM_ShowMeasurementReportWindowControl = WM_USER + 239;
        public readonly static int WM_ShowSystemLogsInfoWindowControl = WM_USER + 240;
        public readonly static int WM_ShowDefaultWindowControl = WM_USER + 241;
        public readonly static int WM_ShowCallRecordsWindowControl = WM_USER + 242;
        public readonly static int WM_ShowSMSRecordsWindowControl = WM_USER + 243;

        //删除对应黑名单追踪表IMSI
        public readonly static int WM_DeleteMeasureReportsIMSIRequest = WM_USER + 242;

        //批量配置导出消息
        public readonly static int WM_BatchConfigurationOutputMessage = WM_USER + 243;
        //批量配置导入消息
        public readonly static int WM_BatchConfigurationImportMessage = WM_USER + 244;
        //重载设备列表
        public readonly static int WM_ReLoadDeviceListMessage = WM_USER + 245;
        //UDP服务器消息
        public readonly static int WM_StartUDPServerMessage = WM_USER + 246;
        //欢迎窗口消息
        public readonly static int WM_WelcomeWindowMessage = WM_USER + 247;
        //主窗口显示
        public readonly static int WM_MainWindowShowMessage = WM_USER + 248;
        //启动信息统计消息
        public readonly static int WM_InfoStatisticMessage = WM_USER + 249;
        //退出提示窗口消息
        public readonly static int WM_TipsWinCloseMessage = WM_USER + 250;
        //LTE 下载日志响应消息
        public readonly static int WM_DOWNLOAD_LTE_LOG_RESULT_MESSAGE = WM_USER + 251;
        //WCDMA 下载日志响应消息
        public readonly static int WM_DOWNLOAD_WCDMA_LOG_RESULT_MESSAGE = WM_USER + 252;
        //下载日志过程中错误消息
        public readonly static int WM_DOWNLOAD_AP_LOGS_ERROR_MESSAGE = WM_USER + 253;
        //AP系统升级过程中交互消息
        public readonly static int WM_AP_SytemUpgrade_MESSAGE = WM_USER + 254;
        //未知设备拖动并提交事件
        public readonly static int WM_UnknownDeviceDragToAddMessage = WM_USER + 255;
        //主界面的子窗口按需组合事件
        public readonly static int WM_SubWindowsConfigurationMessage = WM_USER + 256;
        //批量重启设备响应事件
        public readonly static int WM_BatchApRebootCompleteMessage = WM_USER + 257;
        //TDS 下载日志响应消息
        public readonly static int WM_DOWNLOAD_TDS_LOG_RESULT_MESSAGE = WM_USER + 258;
        /*......*/

        public readonly static int WM_DownloadSystemLogsResponse = WM_USER + 300;
        public readonly static int WM_DomainManageDeviceReloadEven = WM_USER + 301;

        //黑白名单,普通用户操作
        public readonly static int WM_BlackListAddResponse = WM_USER + 302;
        public readonly static int WM_BlackListEditResponse = WM_USER + 303;
        public readonly static int WM_BlackListDeleteResponse = WM_USER + 304;
        public readonly static int WM_BlackListQueryResponse = WM_USER + 308;

        public readonly static int WM_WhiteListAddResponse = WM_USER + 305;
        public readonly static int WM_WhiteListEditResponse = WM_USER + 306;
        public readonly static int WM_WhiteListDeleteResponse = WM_USER + 307;
        public readonly static int WM_WhiteListQueryResponse = WM_USER + 309;

        public readonly static int WM_CustomListQueryResponse = WM_USER + 310;
        public readonly static int WM_CustomListAddResponse = WM_USER + 311;
        public readonly static int WM_CustomListEditResponse = WM_USER + 312;
        public readonly static int WM_CustomListDeleteResponse = WM_USER + 313;
        //重定向
        public readonly static int WM_RedirectListQueryResponse = WM_USER + 315;
        //进度条窗口
        public readonly static int WM_ProgressBarWindowClose = WM_USER + 314;
        //地图显示
        public readonly static int WM_StationLocationResponse = WM_USER + 316;

        //分时显示
        public readonly static int WM_StatisticalResponse = WM_USER + 317;
        //常驻人口
        public readonly static int WM_ResidentIMSIResponse = WM_USER + 318;
        //碰撞分析
        public readonly static int WM_ConditionsIMSIResponse = WM_USER + 319;
        //伴随分析
        public readonly static int WM_AccompanyIMSIResponse = WM_USER + 320;
        //IMSI轨迹
        public readonly static int WM_IMSIPathResponse = WM_USER + 321;

        //GSM设备操作事件
        public readonly static int WM_GSMLibraryRegDelAllResponse = WM_USER + 500;
        public readonly static int WM_GSMSMSListResponse = WM_USER + 502;
        public readonly static int WM_GSMIMSILibraryRegQueryResponse = WM_USER + 503;
        public readonly static int WM_GSMIMEILibraryRegQueryResponse = WM_USER + 504;
        public readonly static int WM_GSMCarrierActionResponse = WM_USER + 505;

        //添加用户管理窗口
        public readonly static int WM_UserManagerResponse = WM_USER + 506;
        //删除用户管理窗口
        public readonly static int WM_DelUserManagerResponse = WM_USER + 507;

        //未知设备操作事件
        public readonly static int WM_UnknownDeviceAddResponse = WM_USER + 600;
        public readonly static int WM_UnknownDeviceAutoUpdate = WM_USER + 601;

        //请求权限
        public readonly static int WM_PrivilegeManageResponse = WM_USER + 602;

        public static LoginUserLevel LoginUserLevel
        {
            get
            {
                return loginUserLevel;
            }
        }
        public static string LanguageType
        {
            get
            {
                return languageType;
            }

            set
            {
                languageType = value;
            }
        }

        public static string LogsDir
        {
            get
            {
                return logsDir;
            }

            set
            {
                logsDir = value;
            }
        }

        public static string LoginUserName
        {
            get
            {
                return loginUserName;
            }

            set
            {
                loginUserName = value;
            }
        }

        public static string LoginPassWord
        {
            get
            {
                return loginPassWord;
            }

            set
            {
                loginPassWord = value;
            }
        }

        public static string LoginServer
        {
            get
            {
                return loginServer;
            }

            set
            {
                loginServer = value;
            }
        }

        public static string INIFile
        {
            get
            {
                return iNIFile;
            }

            set
            {
                iNIFile = value;
            }
        }

        public static string TcpServerHost
        {
            get
            {
                return tcpServerHost;
            }

            set
            {
                tcpServerHost = value;
            }
        }

        public static int TcpServerPort
        {
            get
            {
                return tcpServerPort;
            }

            set
            {
                tcpServerPort = value;
            }
        }

        public static int LoginTimeOut
        {
            get
            {
                return loginTimeOut;
            }

            set
            {
                loginTimeOut = value;
            }
        }

        public static string UserLoginPermit
        {
            get
            {
                return userLoginPermit;
            }

            set
            {
                userLoginPermit = value;
            }
        }

        public static string UserLoginDeny
        {
            get
            {
                return userLoginDeny;
            }

            set
            {
                userLoginDeny = value;
            }
        }

        public static IntPtr WinHandle
        {
            get
            {
                return winHandle;
            }

            set
            {
                winHandle = value;
            }
        }

        public static byte DelayTime
        {
            get
            {
                return delayTime;
            }

            set
            {
                delayTime = value;
            }
        }

        public static DomainActionInfo DomainActionInfoClass
        {
            get
            {
                return domainActionInfoClass;
            }

            set
            {
                domainActionInfoClass = value;
            }
        }

        public static DeviceActionInfo DeviceActionInfoClass
        {
            get
            {
                return deviceActionInfoClass;
            }

            set
            {
                deviceActionInfoClass = value;
            }
        }

        public static UserMousePosition UserMousePosition
        {
            get
            {
                return userMousePosition;
            }

            set
            {
                userMousePosition = value;
            }
        }

        public static IntPtr DeviceManageWinHandle
        {
            get
            {
                return deviceManageWinHandle;
            }

            set
            {
                deviceManageWinHandle = value;
            }
        }

        public static APResoultMsgTypeClass APResoultMsgType
        {
            get
            {
                return aPResourltMsgType;
            }

            set
            {
                aPResourltMsgType = value;
            }
        }

        public static string ConfigType
        {
            get
            {
                return configType;
            }

            set
            {
                configType = value;
            }
        }
        public static string ActionType
        {
            get
            {
                return actionType;
            }

            set
            {
                actionType = value;
            }
        }

        public static string GetSYNCInfoType
        {
            get
            {
                return getSYNCInfoType;
            }
            set
            {
                getSYNCInfoType = value;
            }
        }

        public static APResoultRebootMsgTypeClss APResoultRebootMsgType
        {
            get
            {
                return aPResoultRebootMsgType;
            }

            set
            {
                aPResoultRebootMsgType = value;
            }
        }

        public static int UpDonwLoadTimeOutValue
        {
            get
            {
                return upDonwLoadTimeOutValue;
            }

            set
            {
                upDonwLoadTimeOutValue = value;
            }
        }

        public static IntPtr SpeciallistWinHandle
        {
            get
            {
                return speciallistWinHandle;
            }

            set
            {
                speciallistWinHandle = value;
            }
        }
        public static IntPtr HistoryDataWinHandle
        {
            get
            {
                return historyDataWinHandle;
            }

            set
            {
                historyDataWinHandle = value;
            }
        }
        public static IntPtr UserManagerWinHandle
        {
            get
            {
                return userManagerWinHandle;
            }

            set
            {
                userManagerWinHandle = value;
            }
        }

        public static ScannerDataControlClass ScannerDataControlParameter
        {
            get
            {
                return scannerDataControlParameter;
            }

            set
            {
                scannerDataControlParameter = value;
            }
        }

        public static bool LoginDefault
        {
            get
            {
                return loginDefault;
            }

            set
            {
                loginDefault = value;
            }
        }

        public static int PLMN_Lengh
        {
            get
            {
                return pLMN_Lengh;
            }

            set
            {
                pLMN_Lengh = value;
            }
        }

        public static IntPtr DomainManageWinHandle
        {
            get
            {
                return domainManageWinHandle;
            }

            set
            {
                domainManageWinHandle = value;
            }
        }

        public static IntPtr AddBWListWinHandle
        {
            get
            {
                return addBWListWinHandle;
            }

            set
            {
                addBWListWinHandle = value;
            }
        }

        public static int HeartTime
        {
            get
            {
                return heartTime;
            }

            set
            {
                heartTime = value;
            }
        }

        public static string LogsDbFile
        {
            get
            {
                return logsDbFile;
            }

            set
            {
                logsDbFile = value;
            }
        }
        public static RedirectionClass RedirectionParam
        {
            get
            {
                return redirectionParam;
            }

            set
            {
                redirectionParam = value;
            }
        }

        public static IntPtr UnknownDeviceWinHandle
        {
            get
            {
                return unknownDeviceWinHandle;
            }

            set
            {
                unknownDeviceWinHandle = value;
            }
        }

        public static byte HeartStatu
        {
            get
            {
                return heartStatu;
            }

            set
            {
                heartStatu = value;
            }
        }

        public static int SysLogsTotal
        {
            get
            {
                return sysLogsTotal;
            }

            set
            {
                sysLogsTotal = value;
            }
        }

        public static bool SMSBrowse
        {
            get
            {
                return sMSBrowse;
            }

            set
            {
                sMSBrowse = value;
            }
        }

        public static IntPtr UnknownDeviceTipsWinHandle
        {
            get
            {
                return unknownDeviceTipsWinHandle;
            }

            set
            {
                unknownDeviceTipsWinHandle = value;
            }
        }

        public static IntPtr VolumeActiveWinHandle
        {
            get
            {
                return volumeActiveWinHandle;
            }

            set
            {
                volumeActiveWinHandle = value;
            }
        }

        public static IntPtr AdvanceSettingWinHandle
        {
            get
            {
                return advanceSettingWinHandle;
            }

            set
            {
                advanceSettingWinHandle = value;
            }
        }

        public static IntPtr WelcomeWindowHandle
        {
            get
            {
                return welcomeWindowHandle;
            }

            set
            {
                welcomeWindowHandle = value;
            }
        }

        public static IntPtr ShowDBMapWinHandle
        {
            get
            {
                return showDBMapWinHandle;
            }

            set
            {
                showDBMapWinHandle = value;
            }
        }
        public static IntPtr StatisticalWinHandle
        {
            get
            {
                return statisticalWinHandle;
            }

            set
            {
                statisticalWinHandle = value;
            }
        }

        public static IntPtr TipsWinCloseHandle
        {
            get
            {
                return tipsWinCloseHandle;
            }

            set
            {
                tipsWinCloseHandle = value;
            }
        }

        public static string DownLoadLogUser
        {
            get
            {
                return downLoadLogUser;
            }

            set
            {
                downLoadLogUser = value;
            }
        }

        public static string DownLoadLogPass
        {
            get
            {
                return downLoadLogPass;
            }

            set
            {
                downLoadLogPass = value;
            }
        }

        public static string DownLoadLogRoot
        {
            get
            {
                return downLoadLogRoot;
            }

            set
            {
                downLoadLogRoot = value;
            }
        }

        public static int DownLoadLogPort
        {
            get
            {
                return downLoadLogPort;
            }

            set
            {
                downLoadLogPort = value;
            }
        }

        public static string ApLogDir
        {
            get
            {
                return apLogDir;
            }

            set
            {
                apLogDir = value;
            }
        }

        public static bool DownLoadLogLastDirWithSN
        {
            get
            {
                return downLoadLogLastDirWithSN;
            }

            set
            {
                downLoadLogLastDirWithSN = value;
            }
        }

        public static string UpDateFileSourceDir
        {
            get
            {
                return upDateFileSourceDir;
            }

            set
            {
                upDateFileSourceDir = value;
            }
        }

        public static int CompleteCount
        {
            get
            {
                return completeCount;
            }

            set
            {
                completeCount = value;
            }
        }

        public static IntPtr BatchRebootApWinHandle
        {
            get
            {
                return batchRebootApWinHandle;
            }

            set
            {
                batchRebootApWinHandle = value;
            }
        }
        #endregion

        #region 系统调用
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        public extern static int FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Auto)]
        public extern static IntPtr FindWindowEx(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true)]
        public extern static int SendMessage(IntPtr hwnd, int wsMsg, int wParam, int lParam);

        [DllImport("user32.dll", EntryPoint = "PostMessage", SetLastError = true)]
        public extern static int PostMessage(IntPtr hwnd, int wsMsg, int wParam, int lParam);

        [DllImport("user32.dll", EntryPoint = "DestroyWindow", SetLastError = true)]
        public extern static bool DestroyWindow(IntPtr hwnd);  //根据句柄关于 MessageBox
        #endregion

        #region 构造函数
        public Parameters()
        {
            try
            {
                if (LogsDbFile == null)
                {
                    LogsDbFile = Thread.GetDomain().BaseDirectory + @"\Logs\SystemLog\SysLogs.mdb";
                }

                if (INIFile == null)
                {
                    INIFile = Thread.GetDomain().BaseDirectory + @"\Configuration\SysConfig.ini";
                }

                LanguageType = ReadIniFile("Language", "AbbreviationCode", "", INIFile);
                if (ReadIniFile("SystemSetting", "LogDir", "", INIFile) != (""))
                {
                    LogsDir = ReadIniFile("SystemSetting", "LogDir", "", INIFile);
                    if (!Directory.Exists(LogsDir))
                    {
                        LogsDir = Thread.GetDomain().BaseDirectory + @"Logs\SystemLog";
                    }
                }
                else
                {
                    LogsDir = Thread.GetDomain().BaseDirectory + @"Logs\SystemLog";
                }

                //下载AP日志到本地的目录
                ApLogDir = Thread.GetDomain().BaseDirectory + @"Logs\ApLog";

                if (!ReadIniFile("Login", "Default", "", INIFile).Trim().Equals(""))
                {
                    if (ReadIniFile("Login", "Default", "", INIFile).Equals("1"))
                    {
                        LoginDefault = Convert.ToBoolean(Convert.ToInt32(ReadIniFile("Login", "Default", "", INIFile)));
                    }
                    else if (ReadIniFile("Login", "Default", "", INIFile).Equals("0"))
                    {
                        LoginDefault = false;
                    }
                    else
                    {
                        LoginDefault = true;
                    }
                }
                else
                {
                    LoginDefault = false;
                }

                try
                {
                    if (LoginDefault)
                    {
                        LoginUserName = new DesEncrypt().UnEncrypt(ReadIniFile("Login", "UserName", "", INIFile), new DefineCode().Code());
                    }

                    LoginServer = new DesEncrypt().UnEncrypt(ReadIniFile("Login", "Server", "", INIFile), new DefineCode().Code());
                }
                catch
                {

                }

                //显示实时上报时间隔
                if (ReadIniFile("Scanner", "Refresh", "", INIFile) != "")
                {
                    try
                    {
                        ScannerDataControlParameter.RefreshTime = int.Parse(ReadIniFile("Scanner", "Refresh", "", INIFile));
                    }
                    catch
                    {
                        ScannerDataControlParameter.RefreshTime = 10;
                    }
                }
                else
                {
                    ScannerDataControlParameter.RefreshTime = 10;
                }

                //时实上报显示条数
                if (ReadIniFile("Scanner", "Tatol", "", INIFile) != "")
                {
                    if (ISDigital(ReadIniFile("Scanner", "Tatol", "", INIFile)))
                    {
                        ScannerDataControlParameter.Tatol = int.Parse(ReadIniFile("Scanner", "Tatol", "", INIFile));
                    }
                    else
                    {
                        ScannerDataControlParameter.Tatol = 200;
                    }
                }
                else
                {
                    ScannerDataControlParameter.Tatol = 200;
                }

                //黑名单追踪时实上报显示条数
                if (ReadIniFile("Track", "Tatol", "", INIFile) != "")
                {
                    if (ISDigital(ReadIniFile("Track", "Tatol", "", INIFile)))
                    {
                        MeasureReportTotal = int.Parse(ReadIniFile("Track", "Tatol", "", INIFile));
                    }
                    else
                    {
                        MeasureReportTotal = 16;
                    }
                }
                else
                {
                    MeasureReportTotal = 16;
                }

                //警告音文件
                ScannerDataControlParameter.SoundFile = ReadIniFile("Scanner", "SoundFile", "", INIFile);
                //警告音重复时间
                ScannerDataControlParameter.SoundDelay = ReadIniFile("Scanner", "SoundDelay", "", INIFile);
                //警告音播放次数
                if (ReadIniFile("Scanner", "PlayCount", "", INIFile) != "")
                {
                    if (ISDigital(ReadIniFile("Scanner", "PlayCount", "", INIFile)))
                    {
                        ScannerDataControlParameter.PlayCount = Convert.ToInt32(ReadIniFile("Scanner", "PlayCount", "", INIFile));
                    }
                    else
                    {
                        ScannerDataControlParameter.PlayCount = 1;
                    }
                }
                else
                {
                    ScannerDataControlParameter.PlayCount = 1;
                }

                if (ReadIniFile("Scanner", "Operator", "", INIFile).Trim() != "")
                {
                    PLMN_Lengh = int.Parse(ReadIniFile("Device", "Operator", "", INIFile));
                }

                //是否启用语音播报
                if (ReadIniFile("Scanner", "SoundEnable", "", INIFile).Trim() != "")
                {
                    if (ReadIniFile("Scanner", "SoundEnable", "", INIFile).Trim() == "0" || ReadIniFile("Scanner", "SoundEnable", "", INIFile).Trim() == "1")
                    {
                        ScannerDataControlParameter.SoundEnable = ReadIniFile("Scanner", "SoundEnable", "", INIFile);
                    }
                    else
                    {
                        ScannerDataControlParameter.SoundEnable = "0";
                    }
                }
                else
                {
                    ScannerDataControlParameter.SoundEnable = "0";
                }

                //媒体播放模式
                if (ReadIniFile("Scanner", "PlayerMode", "", INIFile).Trim() != "")
                {
                    if (ReadIniFile("Scanner", "PlayerMode", "", INIFile).Trim() == "false"
                        || ReadIniFile("Scanner", "PlayerMode", "", INIFile).Trim() == "False"
                        || ReadIniFile("Scanner", "PlayerMode", "", INIFile).Trim() == "true"
                        || ReadIniFile("Scanner", "PlayerMode", "", INIFile).Trim() == "True")
                    {
                        ScannerDataControlParameter.PlayerMode = Convert.ToBoolean(ReadIniFile("Scanner", "PlayerMode", "", INIFile).Trim());
                    }
                    else
                    {
                        ScannerDataControlParameter.PlayerMode = false;
                    }
                }
                else
                {
                    ScannerDataControlParameter.PlayerMode = false;
                }

                //媒体语音内容
                ScannerDataControlParameter.SpeeckContent = ReadIniFile("Scanner", "SpeeckContent", "", INIFile).Trim();

                if (ReadIniFile("Heart", "Time", "", INIFile).Trim() != "")
                {
                    if (new Regex(@"\d{1,3}").Match(ReadIniFile("Heart", "Time", "", INIFile).Trim()).Success)
                    {
                        if (int.Parse(ReadIniFile("Heart", "Time", "", INIFile)) < 3 || int.Parse(ReadIniFile("Heart", "Time", "", INIFile)) > 60)
                        {
                            HeartTime = 60;
                        }
                        else
                        {
                            HeartTime = int.Parse(ReadIniFile("Heart", "Time", "", INIFile));
                        }
                    }
                    else
                    {
                        HeartTime = 60;
                    }
                }

                //颜色属性--------------------------------------------------------
                Parameters.ScannerDataControlParameter.WhiteListBackGround = ReadIniFile("Scanner", "WBG", "", INIFile).Trim();
                Parameters.ScannerDataControlParameter.BlackListBackGround = ReadIniFile("Scanner", "BBG", "", INIFile).Trim();
                Parameters.ScannerDataControlParameter.OtherListBackGround = ReadIniFile("Scanner", "OBG", "", INIFile).Trim();

                //播报控制
                try
                {
                    Parameters.ScannerDataControlParameter.WhiteListMode = Convert.ToBoolean(ReadIniFile("Scanner", "WPM", "", INIFile));
                }
                catch
                {
                    Parameters.ScannerDataControlParameter.WhiteListMode = false;
                }
                try
                {
                    Parameters.ScannerDataControlParameter.BlackListMode = Convert.ToBoolean(ReadIniFile("Scanner", "BPM", "", INIFile));
                }
                catch
                {
                    Parameters.ScannerDataControlParameter.BlackListMode = false;
                }
                try
                {
                    Parameters.ScannerDataControlParameter.OtherListMode = Convert.ToBoolean(ReadIniFile("Scanner", "OPM", "", INIFile));
                }
                catch
                {
                    Parameters.ScannerDataControlParameter.OtherListMode = false;
                }
                //---------------------------------------------------------------

                //曲线图控制参数 显示模式 停止
                if (ReadIniFile("Chart", "Model", "", INIFile).Trim() != "")
                {
                    if (ReadIniFile("Chart", "Model", "", INIFile).Trim() == "1"
                        || ReadIniFile("Chart", "Model", "", INIFile).Trim() == "0")
                    {
                        ChartAxialModel = Convert.ToInt32(ReadIniFile("Chart", "Model", "", INIFile).Trim());
                    }
                    else
                    {
                        ChartAxialModel = 1;
                    }
                }
                if (ReadIniFile("Chart", "Enable", "", INIFile).Trim() != "")
                {
                    if (ReadIniFile("Chart", "Enable", "", INIFile).Trim() == "1"
                        || ReadIniFile("Chart", "Enable", "", INIFile).Trim() == "0")
                    {
                        ChartAxialEnable = Convert.ToInt32(ReadIniFile("Chart", "Enable", "", INIFile).Trim());
                    }
                    else
                    {
                        ChartAxialEnable = 0;
                    }
                }

                //日志保存方式
                string TmpLogStatu = ReadIniFile("Logs", "LogStatus", "", INIFile);
                if (ReadIniFile("Logs", "LogStatus", "", INIFile).Trim() != "")
                {
                    if (ISDigital(TmpLogStatu))
                    {
                        LogStatus = byte.Parse(TmpLogStatu);
                    }
                    else
                    {
                        LogStatus = 2;
                    }
                }

                //日志显示数量
                if (ReadIniFile("Logs", "Total", "", INIFile).Trim() != "")
                {
                    string TotalStr = ReadIniFile("Logs", "Total", "", INIFile).Trim();
                    if (ISDigital(TotalStr))
                    {
                        if (TotalStr.Length <= 5)
                        {
                            int Total = int.Parse(ReadIniFile("Logs", "Total", "", INIFile));
                            SysLogsTotal = Total > 20000 ? 20000 : Total <= 0 ? 1000 : Total;
                        }
                        else
                        {
                            SysLogsTotal = 20000;
                        }
                    }
                    else
                    {
                        SysLogsTotal = 1000;
                    }
                }

                //日志大小
                if (ReadIniFile("Logs", "Size", "", INIFile).Trim() != "")
                {
                    if (ISDigital(ReadIniFile("Logs", "Size", "", INIFile).Trim()))
                    {
                        LogFileSize = Convert.ToInt32(ReadIniFile("Logs", "Size", "", INIFile).Trim());
                        if (LogFileSize <= 20)
                        {
                            LogFileSize = Convert.ToInt32(ReadIniFile("Logs", "Size", "", INIFile).Trim()) * 1024 * 1024;
                        }
                        else
                        {
                            LogFileSize = 5L;
                        }
                    }
                    else
                    {
                        LogFileSize = 5L;
                    }
                }

                //是否浏览GSM信息
                if (ReadIniFile("Device", "SMSBrowse", "", INIFile).Trim() != "")
                {
                    string smsbrowse = ReadIniFile("Device", "SMSBrowse", "", INIFile).Trim();

                    if (smsbrowse == "false" || smsbrowse == "true")
                    {
                        SMSBrowse = Convert.ToBoolean(smsbrowse);
                    }
                    else
                    {
                        SMSBrowse = false;
                    }
                }

                //窗口样式控制初始化
                if (DesktopWinStyle == null)
                {
                    DesktopWinStyle = new DesktopWinAPI();
                }

                //Log FTP下载参数
                string DLLgPass = ReadIniFile("RemoteLogDownLoad", "Code", "", INIFile);
                if (DLLgPass != "")
                {
                    try
                    {
                        DownLoadLogPass = new DesEncrypt().UnEncrypt(DLLgPass, new DefineCode().Code());
                    }
                    catch (Exception Ex)
                    {
                        PrintfLogsExtended("FTP 下载AP日志密码被撺改,初始化失败！");
                    }
                }

                string DLLgUser = ReadIniFile("RemoteLogDownLoad", "ID", "", INIFile);
                if (DLLgUser != "")
                {
                    try
                    {
                        DownLoadLogUser = new DesEncrypt().UnEncrypt(DLLgUser, new DefineCode().Code());
                    }
                    catch (Exception Ex)
                    {
                        PrintfLogsExtended("FTP 下载AP日志用户被撺改,初始化失败！");
                    }
                }

                string DLLgRoot = ReadIniFile("RemoteLogDownLoad", "Root", "", INIFile);
                if (DLLgRoot != "")
                {
                    try
                    {
                        DownLoadLogRoot = new DesEncrypt().UnEncrypt(DLLgRoot, new DefineCode().Code());
                    }
                    catch (Exception Ex)
                    {
                        PrintfLogsExtended("FTP 下载AP日志保存文件的本地目录被撺改,初始化失败！");
                    }
                }

                string DLLgPort = ReadIniFile("RemoteLogDownLoad", "Port", "", INIFile);
                if (DLLgPort != "")
                {
                    try
                    {
                        string UnEnDLLgPort = new DesEncrypt().UnEncrypt(DLLgPort, new DefineCode().Code());
                        if (ISDigital(UnEnDLLgPort))
                        {
                            if (Convert.ToInt32(UnEnDLLgPort) > 0 && Convert.ToInt32(UnEnDLLgPort) < 65535)
                            {
                                DownLoadLogPort = Convert.ToInt32(UnEnDLLgPort);
                            }
                        }
                    }
                    catch
                    {
                        PrintfLogsExtended("FTP 下载AP日志端口被撺改,初始化失败！");
                    }
                }

                string DLLgLogRootExtend = ReadIniFile("RemoteLogDownLoad", "LogRootExtend", "", INIFile);
                if (DLLgLogRootExtend != "")
                {
                    if (DLLgLogRootExtend.ToLower() == "true" || DLLgLogRootExtend.ToLower() == "false")
                    {
                        DownLoadLogLastDirWithSN = Convert.ToBoolean(DLLgLogRootExtend);
                    }
                }

                //AP系统升级超时控制时间 TimeOut
                string _UpDonwLoadTimeOutValue = ReadIniFile("Update", "TimeOut", "", INIFile);
                if (_UpDonwLoadTimeOutValue != "")
                {
                    if (ISDigital(_UpDonwLoadTimeOutValue))
                    {
                        UpDonwLoadTimeOutValue = Convert.ToInt32(_UpDonwLoadTimeOutValue);
                    }
                }

                //升级包文件存放路径
                string _UpDateFileSourceDir = ReadIniFile("Update", "PackageSource", "", INIFile);
                if (_UpDateFileSourceDir != "" && _UpDateFileSourceDir != null)
                {
                    UpDateFileSourceDir = _UpDateFileSourceDir;
                }

                //窗口控制选项
                string _MainSubWindowControlPara = ReadIniFile("WinControl", "Status", "", INIFile);
                if (_MainSubWindowControlPara != null && _MainSubWindowControlPara != "")
                {
                    string MainSubWindowControlPara = new DesEncrypt().UnEncrypt(_MainSubWindowControlPara, new DefineCode().Code());
                    string[] Values = MainSubWindowControlPara.Split(new char[] { ';' });

                    for (int i = 0; i < Values.Length; i++)
                    {
                        if ((i == 0) && (Values[i].ToLower() != "true" || Values[i].ToLower() != "false"))
                        {
                            MainWinControlParameter.AllWin = Convert.ToBoolean(Values[i]);
                        }
                        else if ((i == 1) && (Values[i].ToLower() != "true" || Values[i].ToLower() != "false"))
                        {
                            MainWinControlParameter.DeviceListWin = Convert.ToBoolean(Values[i]);
                        }
                        else if ((i == 2) && (Values[i].ToLower() != "true" || Values[i].ToLower() != "false"))
                        {
                            MainWinControlParameter.ScannerWin = Convert.ToBoolean(Values[i]);
                        }
                        else if ((i == 3) && (Values[i].ToLower() != "true" || Values[i].ToLower() != "false"))
                        {
                            MainWinControlParameter.BlackListWin = Convert.ToBoolean(Values[i]);
                        }
                        else if ((i == 4) && (Values[i].ToLower() != "true" || Values[i].ToLower() != "false"))
                        {
                            MainWinControlParameter.SystemLogsWin = Convert.ToBoolean(Values[i]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                PrintfLogsExtended("初始化参数失败：" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        #endregion

        /// <summary>
        /// 获取现在年月日及时间
        /// </summary>
        /// <returns></returns>
        public static string GetNowTime()
        {
            return string.Format("{0:D4}", DateTime.Now.Year) + "-" + string.Format("{0:D2}", DateTime.Now.Month) + "-" + string.Format("{0:D2}", DateTime.Now.Day) + " " + string.Format("{0:D2}", DateTime.Now.Hour + ":" + string.Format("{0:D2}", DateTime.Now.Minute) + ":" + string.Format("{0:D2}", DateTime.Now.Second));
        }

        /// <summary>
        /// 获取提示标题
        /// </summary>
        /// <param name="KeyWord"></param>
        /// <returns></returns>
        public static string GetTitleContent(string KeyWord)
        {
            string titleContent = string.Empty;
            object WindowEventTitleClass = null;
            Regex titleRegex = new Regex(KeyWord);

            if (LanguageType.Equals("CN") || LanguageType.Equals(""))
            {
                WindowEventTitleClass = new Language_CN.WindowEventTitleContent();
                if (titleRegex.Match("W").Success)
                {
                    titleContent = ((Language_CN.WindowEventTitleContent)WindowEventTitleClass).TtitleWarning;
                }
                else if (titleRegex.Match("E").Success)
                {
                    titleContent = ((Language_CN.WindowEventTitleContent)WindowEventTitleClass).TitleError;
                }
                else if (titleRegex.Match("I").Success)
                {
                    titleContent = ((Language_CN.WindowEventTitleContent)WindowEventTitleClass).TitleInfomation;
                }
            }
            else
            {
                WindowEventTitleClass = new Language_EN.WindowEventTitleContent();
                if (titleRegex.Match("W").Success)
                {
                    titleContent = ((Language_EN.WindowEventTitleContent)WindowEventTitleClass).TtitleWarning;
                }
                else if (titleRegex.Match("E").Success)
                {
                    titleContent = ((Language_EN.WindowEventTitleContent)WindowEventTitleClass).TitleError;
                }
                else if (titleRegex.Match("I").Success)
                {
                    titleContent = ((Language_EN.WindowEventTitleContent)WindowEventTitleClass).TitleInfomation;
                }
            }

            return titleContent;
        }

        public static DesktopWinAPI DesktopWinStyle = null;

        /// <summary>
        /// Http上传文件 Post方式上传
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static bool UploadFile(string FileName)
        {
            try
            {
                string UploadServerUrl = string.Empty;
                WebClient VersionUpload = new WebClient();
                VersionUpload.Credentials = CredentialCache.DefaultCredentials; //获取或设置发送到主机并用于请求进行身份验证的网络凭据

                try
                {
                    UploadServerUrl = ConfigurationManager.AppSettings["UploadServerUrl"].ToString() + "/uploads/" + new FileInfo(FileName).Name;
                }
                catch
                {
                    return false;
                }
                VersionUpload.UploadFileAsync(new Uri(UploadServerUrl), FileName);
                VersionUpload.UploadFileCompleted += new UploadFileCompletedEventHandler(VersionUpload_UploadFileCompleted);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("文件上传失败！" + ex.Message, "提示", System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return false;
        }

        public static void VersionUpload_UploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                System.Windows.Forms.MessageBox.Show("文件上传失败！" + e.Error.Message, "提示", System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("文件上传成功！" + e.Error.Message, "提示", System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// //Http上传文件 PUT方式上传
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="Method"></param>
        /// <returns></returns>
        public static bool UploadFile(string FileName, string Method)
        {
            string UploadServerUrl = string.Empty;
            WebClient VersionUpload = new WebClient();
            VersionUpload.Credentials = CredentialCache.DefaultCredentials;
            try
            {
                UploadServerUrl = @ConfigurationManager.AppSettings["UploadServerUrl"].ToString() + "/patch/" + new FileInfo(FileName).Name;
            }
            catch
            {
                return false;
            }

            try
            {
                VersionUpload.UploadFile(new Uri(UploadServerUrl), Method.Trim(), FileName);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("文件上传失败！" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;

            }
            return true;
        }

        /// <summary>
        /// Http以PUT方式下载
        /// </summary>
        /// <param name="RemoteSourceFile">远程文件</param>
        /// <param name="DestFileName">保存到本地文件</param>
        /// <returns></returns>
        public static bool DownloadFile(string RemoteSourceFile, string DestFileName)
        {
            WebClient VersionUpload = new WebClient();
            VersionUpload.Credentials = CredentialCache.DefaultCredentials;
            try
            {
                VersionUpload.DownloadFile(RemoteSourceFile, DestFileName);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("文件下载失败！" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 上传数据流文件到http服务器
        /// 附件上传地址 
        /// 附件源地址
        /// </summary>
        /// <param name="sServerPath"></param>
        /// <param name="sEnclosurePath"></param>
        /// <returns></returns>
        public static bool UploadFileToHttpServer(string sServerPath, string sEnclosurePath)
        {
            try
            {
                string sServerEnclousePath = string.Empty;
                string sClientSourcePath = string.Empty;
                sClientSourcePath = sEnclosurePath;
                int i = sEnclosurePath.LastIndexOf("\\");
                string sFileName = sEnclosurePath.Substring(i + 1);
                sServerEnclousePath = sServerPath + "\\uploads\\" + sFileName;
                WebClient webclient = new WebClient();
                webclient.Credentials = CredentialCache.DefaultCredentials;
                FileStream filestream = new FileStream(sClientSourcePath, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(filestream);
                byte[] uploadArray = br.ReadBytes((int)filestream.Length);
                Stream uploadStream = webclient.OpenWrite(sServerEnclousePath, "PUT");
                if (uploadStream.CanWrite)
                {
                    uploadStream.Write(uploadArray, 0, uploadArray.Length);
                    uploadStream.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("上传文件失败！" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return false;
        }

        /// <summary>
        /// INI文件读操作API
        /// </summary>
        /// <param name="section">节点</param>
        /// <param name="key">键</param>
        /// <param name="defvalue">未读取的默认值</param>
        /// <param name="retvalue">读取到的默认值</param>
        /// <param name="size">内容大小</param>
        /// <param name="filepath">INI文件路径</param>
        /// <returns></returns>
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defvalue, StringBuilder retvalue, int size, string filepath);

        /// <summary>
        /// INI文件写操作API
        /// </summary>
        /// <param name="section">节点</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="filepath">文件路径</param>
        /// <returns></returns>
        [DllImport("kernel32")]
        public static extern int WritePrivateProfileString(string section, string key, string value, string filepath);

        public static string RootPath = AppDomain.CurrentDomain.BaseDirectory + "config.dat";

        /// <summary>
        /// 读取INI文件内容函数
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="def"></param>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static string GetIniFileString(string section, string key, string def, string filepath)
        {
            StringBuilder temp = new StringBuilder(1024);
            temp.Clear();
            GetPrivateProfileString(section, key, def, temp, 1024, filepath);
            return temp.ToString();
        }

        /// <summary>
        /// 写入INI文件函数不加密码
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="def"></param>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static bool WriteIniFile(string section, string key, string value, string filepath)
        {
            try
            {
                WritePrivateProfileString(section, key, value, filepath);
                return true;
            }
            catch (Exception e)
            {
                ShowCurrentMessage(e.Message);
            }
            return false;
        }

        /// <summary>
        /// 写入INI文件函数
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="def"></param>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static bool WriteIniFileString(string section, string key, string value, string filepath)
        {
            try
            {
                WritePrivateProfileString(section, key, value, filepath);
                return true;
            }
            catch (Exception e)
            {
                ShowCurrentMessage(e.Message);
            }
            return false;
        }

        public static bool CheckIniFile(string FilePathEx)
        {
            bool value = false;
            if (FilePathEx == "")
            {
                if (File.Exists(RootPath))
                {
                    value = true;
                }
                else
                {
                    value = false;
                }
            }
            else
            {
                if (File.Exists(FilePathEx))
                {
                    value = true;
                }
                else
                {
                    value = false;
                }
            }
            return value;
        }

        public bool WriteIniFile(string section, string key, string value, string FileName, string Code)
        {
            DesEncrypt DoDESCrypt = new DesEncrypt();
            bool Flag = false;
            try
            {
                if (WriteIniFileString(DoDESCrypt.Encrypt(section, Code), DoDESCrypt.Encrypt(key, Code), DoDESCrypt.Encrypt(value, Code), FileName))
                {
                    Flag = true;
                }
                else
                {
                    Flag = false;
                }
            }
            catch (Exception e)
            {
                ShowCurrentMessage(e.Message);
                Flag = false;
            }
            return Flag;
        }

        public static string ReadIniFile(string section, string key, string value, string FileName)
        {
            string DocumentText = "\0";
            try
            {
                DocumentText = GetIniFileString(section, key, value, FileName);
            }
            catch (Exception e)
            {
                ShowCurrentMessage(e.Message);

            }
            return DocumentText;
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="Msg"></param>
        /// <returns></returns>
        public static int printfLogs(params string[] Msg)
        {
            try
            {
                if (Msg.Length > 0)
                {
                    StringBuilder MsgList = new StringBuilder();
                    MsgList.Clear();
                    string DTime = string.Format("{0:D4}", DateTime.Now.Year) + "-" + string.Format("{0:D2}", DateTime.Now.Month) + "-" + string.Format("{0:D2}", DateTime.Now.Day);
                    string STime = string.Format("{0:D2}", DateTime.Now.Hour) + ":" + string.Format("{0:D2}", DateTime.Now.Minute) + ":" + string.Format("{0:D2}", DateTime.Now.Second);
                    logsFile = DTime + "_" + LogFileNumber.ToString() + ".txt";
                    if (File.Exists(LogsDir + @"\" + DTime + @"\" + logsFile))
                    {
                        System.IO.FileInfo _logFileSize = new FileInfo(LogsDir + @"\" + DTime + @"\" + logsFile);
                        if (_logFileSize.Length >= LogFileSize)
                        {
                            LogFileNumber++;
                            logsFile = DTime + "_" + LogFileNumber.ToString() + ".txt";
                        }
                    }

                    lock (EventLogsInfoOutPut.SaveLock)
                    {
                        for (int i = 0; i < Msg.Length; i++)
                        {
                            MsgList.AppendLine(Msg[i]);
                        }

                        if (Directory.Exists(LogsDir + @"\" + DTime))
                        {
                            File.AppendAllText(LogsDir + @"\" + DTime + @"\" + logsFile, Environment.NewLine + "============ " + DTime + " " + STime + " ============" + Environment.NewLine + MsgList.ToString());
                        }
                        else
                        {
                            Directory.CreateDirectory(LogsDir + @"\" + DTime);
                            File.AppendAllText(LogsDir + @"\" + DTime + @"\" + logsFile, Environment.NewLine + "============ " + DTime + " " + STime + " ============" + Environment.NewLine + MsgList.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Thread.GetDomain().BaseDirectory + @"\Logs\SystemLog\Error.log", (ex.Message + Environment.NewLine + ex.StackTrace));
            }
            return 0;
        }

        public static void ShowCurrentMessage(string Info)
        {
            System.Windows.Forms.MessageBox.Show(Info, "错误提示", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
        }

        /// <summary>
        /// 记录日志扩展
        /// </summary>
        /// <param name="Msg"></param>
        /// <returns></returns>
        public static int PrintfLogsExtended(params string[] Msg)
        {
            try
            {
                EventLogsInfoOutPut.Input(Msg);
            }
            catch (Exception ex)
            {
                File.AppendAllText(Thread.GetDomain().BaseDirectory + @"\Logs\SystemLog\Error.log", (ex.Message + Environment.NewLine + ex.StackTrace));
            }
            return 0;
        }

        /// <summary>
        /// 检测IP地址是否合法
        /// </summary>
        /// <param name="IP"></param>
        /// <returns></returns>
        public static bool CheckIPFormat(string IP)
        {
            Regex intRegex = new Regex(@"[\d]");
            Regex ipAddrRegex = new Regex(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");
            if (ipAddrRegex.Match(IP).Success)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 检测端口是否合法
        /// </summary>
        /// <param name="Port"></param>
        /// <returns></returns>
        public static bool CheckPOrtFormat(string Port)
        {
            Regex intRegex = new Regex(@"[\d]");
            Regex ipAddrRegex = new Regex(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");
            if (intRegex.Match(Port).Success)
            {
                if (Convert.ToInt32(Port) > 0 && Convert.ToInt32(Port) < 65535)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 检测参数是否是整数
        /// </summary>
        /// <param name="Param"></param>
        /// <param name="FromMin"></param>
        /// <param name="ToMax"></param>
        /// <returns></returns>
        public static bool CheckParameters(string Param, object FromMin, object ToMax)
        {
            Regex intParam = new Regex(@"\d");
            try
            {
                if (Param != null && FromMin != null && ToMax != null)
                {
                    if (intParam.Match(Param).Success)
                    {
                        if (Convert.ToInt32(Param) < Convert.ToInt32(FromMin) || Convert.ToInt32(Param) > Convert.ToInt32(ToMax))
                        {
                            System.Windows.MessageBox.Show("参数[" + Param + "]超出范围[" + FromMin + "~" + ToMax + "]", "提示", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("参数[" + Param + "]格式错误[" + FromMin + "~" + ToMax + "]", "提示", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("参数[" + Param + "]格式异常，" + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.ServiceNotification);
            }
            return false;
        }

        /// <summary>
        /// 检测是不是数字
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool ISDigital(string Value)
        {
            try
            {
                Int64 v = Convert.ToInt64(Value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 检测是不是延时时间
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool ISSoundTime(string Value)
        {
            try
            {
                if (new Regex(@"\.").Match(Value).Success)
                {
                    string[] Data = Value.Split(new char[] { '.' });
                    if (Data.Length == 2)
                    {
                        if (Data[1].Length > 3)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                double ST = Math.Abs(Convert.ToDouble(Value));
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 检测是不是实数
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool ISDouble(string Value)
        {
            try
            {
                if (!new Regex(@"\.").Match(Value).Success)
                {
                    return false;
                }

                double D = Convert.ToDouble(Value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 是否为日期型字符串
        /// </summary>
        /// <param name="source">日期字符串(2000-01-01 / 2000\01\01)</param>
        /// <returns></returns>
        public static bool IsDate(string StrSource)
        {
            return (Regex.IsMatch(StrSource, @"^\d{4}-\d{1,2}-\d{1,2}") || Regex.IsMatch(StrSource, @"^\d{4}\\\d{1,2}\\\d{1,2}"));
        }

        /// <summary>
        /// 是否为时间型字符串
        /// </summary>
        /// <param name="source">时间字符串(15:00:00)</param>
        /// <returns></returns>
        public static bool IsTime(string StrSource)
        {
            return Regex.IsMatch(StrSource, @"^((20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d)$");
        }

        /// <summary>
        /// 检测地址是否是IP:Port模式，正确True,否则为False
        /// </summary>
        /// <param name="IP"></param>
        /// <returns></returns>
        public static bool IsIP(string IP)
        {
            //[IP:Port]正则表达式
            Regex IPPortRegex = new Regex(@"((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?[0-9])))(\:\d){1,5})");
            return IPPortRegex.Match(IP.Trim()).Success;
        }

        /// <summary>
        /// 检测地址是否是IP地址，正确True,否则为False
        /// </summary>
        /// <param name="IP"></param>
        /// <returns></returns>
        public static bool IsIP(object IPAddr)
        {
            //[IP:Port]正则表达式
            Regex IPRegex = new Regex(@"((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?[1-9]))))");
            return IPRegex.Match(Convert.ToString(IPAddr).Trim()).Success;
        }

        /// <summary>
        /// 根据IP地址获得MAC地址
        /// </summary>
        /// <param name="hostip"></param>
        /// <returns></returns>
        /// 获取远程IP（不能跨网段）的MAC地址
        public static string GetMacAddress(string HostIP)
        {
            string Mac = "";
            try
            {
                Int32 ldest = inet_addr(HostIP); //将IP地址从 点数格式转换成无符号长整型 
                Int64 macinfo = new Int64();
                Int32 len = 6;
                SendARP(ldest, 0, ref macinfo, ref len);
                string TmpMac = Convert.ToString(macinfo, 16).PadLeft(12, '0');//转换成16进制　注意有些没有十二位 
                Mac = TmpMac.Substring(0, 2).ToUpper();// 
                for (int i = 2; i < TmpMac.Length; i = i + 2)
                {
                    Mac = TmpMac.Substring(i, 2).ToUpper() + "-" + Mac;
                }
            }
            catch (Exception ex)
            {
                Mac = "获取MAC地址失败：" + ex.Message;
            }
            return Mac;
        }

        /// <summary>
        /// 检测文件类型
        /// </summary>
        /// <param name="path"></param>
        public static string[] CheckTrueFileName(string path)
        {
            System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.BinaryReader r = new System.IO.BinaryReader(fs);
            string bx = " ";
            byte buffer;
            try
            {
                buffer = r.ReadByte();
                bx = buffer.ToString();
                buffer = r.ReadByte();
                bx += buffer.ToString();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
            r.Close();
            fs.Close();

            string[] FileType = new string[4];
            //真实的文件类型
            FileType[0] = bx;
            //文件名，包括格式
            FileType[1] = System.IO.Path.GetFileName(path);
            //文件名， 不包括格式
            FileType[2] = System.IO.Path.GetFileNameWithoutExtension(path);
            //文件格式
            FileType[3] = System.IO.Path.GetExtension(path);
            return FileType;
        }

        /// <summary>
        /// 获到颜色
        /// </summary>
        /// <returns></returns>
        public static bool GettingColor(ref Color iColor)
        {
            ColorPickerWindow ColorPicker = new ColorPickerWindow();
            ColorPicker.Owner = System.Windows.Application.Current.MainWindow;
            ColorPicker.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("SkyBlue"));
            ColorPicker.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ColorPicker.Topmost = true;
            if ((bool)ColorPicker.ShowDialog())
            {
                iColor = ColorPickerSwatch.ColorPickerControl.Color;
                return true;
            }
            return false;
        }

        //struct转换为byte[]
        public static byte[] StructToBytes(object structObj)
        {
            int size = Marshal.SizeOf(structObj);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(structObj, buffer, false);
                byte[] bytes = new byte[size];
                Marshal.Copy(buffer, bytes, 0, size);
                return bytes;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        //byte[]转换为struct
        public static object BytesToStruct(byte[] bytes, Type type)
        {
            try
            {
                int size = Marshal.SizeOf(type);
                IntPtr buffer = Marshal.AllocHGlobal(size);

                Marshal.Copy(bytes, 0, buffer, size);
                return Marshal.PtrToStructure(buffer, type);
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }

            return null;
        }

        //byte[]转换为Intptr
        public static IntPtr BytesToIntptr(byte[] bytes)
        {
            int size = bytes.Length;
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(bytes, 0, buffer, size);
                return buffer;
            }
            finally

            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        /// <summary>
        /// 查找控件Template中的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static T GetVisualChild<T>(DependencyObject parent, Func<T, bool> predicate) where T : Visual
        {
            try
            {
                int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
                for (int i = 0; i < numVisuals; i++)
                {
                    DependencyObject Item = VisualTreeHelper.GetChild(parent, i);
                    T child = Item as T;

                    if (child == null)
                    {
                        child = GetVisualChild<T>(Item, predicate);
                        if (child != null)
                        {
                            return child;
                        }
                    }
                    else
                    {
                        if (predicate(child))
                        {
                            return child;
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                PrintfLogsExtended(Ex.Message, Ex.StackTrace);
            }

            return null;
        }

        public static void DrawingCornerLine(object sender, double left, double top, double right, double bottom, double lineWidth, double lineLength, Color lineColor)
        {
            try
            {
                //角线一
                Line cornerLine0 = new Line();
                cornerLine0.X1 = left - 1;
                cornerLine0.Y1 = top + 1;
                cornerLine0.X2 = lineLength;
                cornerLine0.Y2 = top + 1;
                cornerLine0.Stroke = new SolidColorBrush(lineColor);
                cornerLine0.StrokeThickness = lineWidth;

                Line cornerLine1 = new Line();
                cornerLine1.X1 = left - 1;
                cornerLine1.Y1 = top - 1;
                cornerLine1.X2 = left - 1;
                cornerLine1.Y2 = lineLength;
                cornerLine1.Stroke = new SolidColorBrush(lineColor);
                cornerLine1.StrokeThickness = lineWidth;

                //角线二
                Line cornerLine2 = new Line();
                cornerLine2.X1 = left - 1;
                cornerLine2.Y1 = bottom + 1;
                cornerLine2.X2 = lineLength;
                cornerLine2.Y2 = bottom + 1;
                cornerLine2.Stroke = new SolidColorBrush(lineColor);
                cornerLine2.StrokeThickness = lineWidth;

                Line cornerLine3 = new Line();
                cornerLine3.X1 = left - 1;
                cornerLine3.Y1 = bottom + 1;
                cornerLine3.X2 = left - 1;
                cornerLine3.Y2 = -lineLength;
                cornerLine3.Stroke = new SolidColorBrush(lineColor);
                cornerLine3.StrokeThickness = lineWidth;

                //角线三
                Line cornerLine4 = new Line();
                cornerLine4.X1 = right + 1;
                cornerLine4.Y1 = -1;
                cornerLine4.X2 = right + 1;
                cornerLine4.Y2 = -lineLength;
                cornerLine4.Stroke = new SolidColorBrush(lineColor);
                cornerLine4.StrokeThickness = lineWidth;

                Line cornerLine5 = new Line();
                cornerLine4.X1 = right + 1;
                cornerLine4.Y1 = -1;
                cornerLine4.X2 = -lineLength;
                cornerLine4.Y2 = -1;
                cornerLine5.Stroke = new SolidColorBrush(lineColor);
                cornerLine5.StrokeThickness = lineWidth;

                //角线四
                Line cornerLine6 = new Line();
                cornerLine6.X1 = right + 1;
                cornerLine6.Y1 = bottom + 1;
                cornerLine6.X2 = -lineLength;
                cornerLine6.Y2 = bottom + 1;
                cornerLine6.Stroke = new SolidColorBrush(lineColor);
                cornerLine6.StrokeThickness = lineWidth;

                Line cornerLine7 = new Line();
                cornerLine7.X1 = right + 1;
                cornerLine7.Y1 = bottom + 1;
                cornerLine7.X2 = right + 1;
                cornerLine7.Y2 = lineLength;
                cornerLine7.Stroke = new SolidColorBrush(lineColor);
                cornerLine7.StrokeThickness = lineWidth;

                if (sender is System.Windows.Controls.Label)
                {
                    Grid.SetRow(cornerLine0, Grid.GetRow(sender as System.Windows.Controls.Label));
                    Grid.SetColumn(cornerLine0, Grid.GetColumn(sender as System.Windows.Controls.Label));

                    Grid.SetRow(cornerLine1, Grid.GetRow(sender as System.Windows.Controls.Label));
                    Grid.SetColumn(cornerLine1, Grid.GetColumn(sender as System.Windows.Controls.Label));

                    Grid.SetRow(cornerLine2, Grid.GetRow(sender as System.Windows.Controls.Label));
                    Grid.SetColumn(cornerLine2, Grid.GetColumn(sender as System.Windows.Controls.Label));

                    Grid.SetRow(cornerLine3, Grid.GetRow(sender as System.Windows.Controls.Label));
                    Grid.SetColumn(cornerLine3, Grid.GetColumn(sender as System.Windows.Controls.Label));

                    Grid.SetRow(cornerLine4, Grid.GetRow(sender as System.Windows.Controls.Label));
                    Grid.SetColumn(cornerLine4, Grid.GetColumn(sender as System.Windows.Controls.Label));

                    Grid.SetRow(cornerLine5, Grid.GetRow(sender as System.Windows.Controls.Label));
                    Grid.SetColumn(cornerLine5, Grid.GetColumn(sender as System.Windows.Controls.Label));

                    Grid.SetRow(cornerLine6, Grid.GetRow(sender as System.Windows.Controls.Label));
                    Grid.SetColumn(cornerLine6, Grid.GetColumn(sender as System.Windows.Controls.Label));

                    Grid.SetRow(cornerLine7, Grid.GetRow(sender as System.Windows.Controls.Label));
                    Grid.SetColumn(cornerLine7, Grid.GetColumn(sender as System.Windows.Controls.Label));

                    ((sender as System.Windows.Controls.Label).Parent as Grid).Children.Add(cornerLine0);
                    ((sender as System.Windows.Controls.Label).Parent as Grid).Children.Add(cornerLine1);
                    ((sender as System.Windows.Controls.Label).Parent as Grid).Children.Add(cornerLine2);
                    ((sender as System.Windows.Controls.Label).Parent as Grid).Children.Add(cornerLine3);
                    ((sender as System.Windows.Controls.Label).Parent as Grid).Children.Add(cornerLine4);
                    ((sender as System.Windows.Controls.Label).Parent as Grid).Children.Add(cornerLine5);
                    ((sender as System.Windows.Controls.Label).Parent as Grid).Children.Add(cornerLine6);
                    ((sender as System.Windows.Controls.Label).Parent as Grid).Children.Add(cornerLine7);
                }
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("创建角线内部故障", Ex.Message, Ex.StackTrace);
            }
        }
    }

    /// <summary>
    /// 密钥
    /// </summary>
    public class DefineCode
    {
        private readonly string CurCode = "0XCA08D9";
        public string Code()
        {
            return this.CurCode;
        }
    }

    /// <summary>
    /// DES 加密/解密 
    /// </summary>
    public class DesEncrypt
    {
        public string Encrypt(string CryptValue, string key)
        {
            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
            byte[] CryptValueDatatype = Encoding.GetEncoding("UTF-8").GetBytes(CryptValue);
            DES.Key = ASCIIEncoding.ASCII.GetBytes(key);
            DES.IV = ASCIIEncoding.ASCII.GetBytes(key);
            string GetDesCryptPass = "";
            if (CryptValue.Length > 0 && key.Length > 0)
            {
                using (MemoryStream mst = new MemoryStream())
                {
                    try
                    {
                        CryptoStream cts = new CryptoStream(mst, DES.CreateEncryptor(), CryptoStreamMode.Write);
                        cts.Write(CryptValueDatatype, 0, CryptValueDatatype.Length);
                        cts.FlushFinalBlock();
                        StringBuilder DecPasswd = new StringBuilder();
                        foreach (byte desbyte in mst.ToArray())
                        {
                            DecPasswd.AppendFormat("{0:X2}", desbyte);
                        }
                        GetDesCryptPass = DecPasswd.ToString();
                    }
                    catch (Exception Ex)
                    {
                        Parameters.PrintfLogsExtended(Ex.Message, Ex.StackTrace);
                    }
                }
            }
            return GetDesCryptPass;
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="UnCryptValue"></param>
        /// <param name="Unkey"></param>
        /// <returns></returns>
        public string UnEncrypt(string UnCryptValue, string Unkey)
        {
            StringBuilder UnDesPassWd = new StringBuilder();
            if (UnCryptValue.Length > 0 && Unkey.Length > 0)
            {
                using (DESCryptoServiceProvider UnDES = new DESCryptoServiceProvider())
                {
                    byte[] UndesCode = new byte[UnCryptValue.Length / 2];
                    for (int x = 0; x < (UnCryptValue.Length / 2); x++)
                    {
                        int y = (Convert.ToInt32(UnCryptValue.Substring(x * 2, 2), 16));
                        UndesCode[x] = Convert.ToByte(y);
                    }
                    UnDES.Key = ASCIIEncoding.ASCII.GetBytes(Unkey);
                    UnDES.IV = ASCIIEncoding.ASCII.GetBytes(Unkey);
                    using (MemoryStream Unmst = new MemoryStream())
                    {
                        CryptoStream Uncst = new CryptoStream(Unmst, UnDES.CreateDecryptor(), CryptoStreamMode.Write);
                        Uncst.Write(UndesCode, 0, UndesCode.Length);
                        Uncst.FlushFinalBlock();
                        UnDesPassWd.Append(System.Text.Encoding.UTF8.GetString(Unmst.ToArray()));
                    }
                }
            }
            return UnDesPassWd.ToString();
        }
    }

    /// <summary>
    /// MD5加密
    /// </summary>
    public class MD5Encrypt
    {
        public string Encrypt(string RetValue)
        {
            string Md5Passworld = "";
            if (RetValue.Length > 0)
            {
                MD5CryptoServiceProvider MD5pro = new MD5CryptoServiceProvider();
                byte[] Result = Encoding.Default.GetBytes(RetValue);
                byte[] Md5Code = MD5pro.ComputeHash(Result);
                Md5Passworld = BitConverter.ToString(Md5Code).Replace("-", "");
            }
            return Md5Passworld;
        }
    }

    /// <summary>
    /// 获取硬件信息
    /// </summary>

    /// <summary>
    /// 用户等级
    /// </summary>
    public class LoginUserLevel
    {
        private readonly string user_operator = "operator";
        private readonly string user_administratorr = "administrator";
        private readonly string user_superadmin = "superadmin";

        public string User_operator
        {
            get
            {
                return user_operator;
            }
        }

        public string User_administratorr
        {
            get
            {
                return user_administratorr;
            }
        }

        public string User_superadmin
        {
            get
            {
                return user_superadmin;
            }
        }
    }

    /// <summary> 
    /// 获取文件的编码格式 
    /// </summary> 
    public class EncodingType
    {
        /// <summary> 
        /// 给定文件的路径，读取文件的二进制数据，判断文件的编码类型 
        /// </summary> 
        /// <param name=“FILE_NAME“>文件路径</param> 
        /// <returns>文件的编码类型</returns> 
        public System.Text.Encoding FileGetTypeByName(string FILE_NAME)
        {
            FileStream fs = new FileStream(FILE_NAME, FileMode.Open, FileAccess.Read);
            Encoding r = GetFileTypeByStream(fs);
            fs.Close();
            return r;
        }

        /// <summary> 
        /// 通过给定的文件流，判断文件的编码类型 
        /// </summary> 
        /// <param name=“fs“>文件流</param> 
        /// <returns>文件的编码类型</returns> 
        public System.Text.Encoding GetFileTypeByStream(FileStream fs)
        {
            byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM 
            Encoding reVal = Encoding.Default;

            BinaryReader r = new BinaryReader(fs, System.Text.Encoding.Default);
            int i;
            int.TryParse(fs.Length.ToString(), out i);
            byte[] ss = r.ReadBytes(i);
            if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
            {
                reVal = Encoding.UTF8;
            }
            else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = Encoding.Unicode;
            }
            r.Close();
            return reVal;

        }

        /// <summary> 
        /// 判断是否是不带 BOM 的 UTF8 格式 
        /// </summary> 
        /// <param name=“data“></param> 
        /// <returns></returns> 
        private bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1; //计算当前正分析的字符应还有的字节数 
            byte curByte; //当前分析的字节. 
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前 
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X 
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1 
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式");
            }
            return true;
        }
    }

    /// <summary>
    /// 窗口样式配置
    /// </summary>
    public class DesktopWinAPI : Form
    {
        [DllImport("user32.dll")]
        public extern static IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        public extern static bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
        public static uint LWA_COLORKEY = 0x00000001;
        public static uint LWA_ALPHA = 0x00000002;

        [DllImport("user32.dll")]
        public extern static uint SetWindowLong(IntPtr hwnd, int nIndex, uint dwNewLong);[DllImport("user32.dll")]
        public extern static uint GetWindowLong(IntPtr hwnd, int nIndex);
        public enum WindowStyle : int { GWL_EXSTYLE = -20 }
        public enum ExWindowStyle : uint { WS_EX_LAYERED = 0x00080000 }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Parent = DesktopWinAPI.GetDesktopWindow(); cp.ExStyle = 0x00000080 | 0x00000008;  //WS_EX_TOOLWINDOW | WS_EX_TOPMOST
                return cp;
            }
        }

        private void SetWindowTransparent(IntPtr hWnd, byte bAlpha)
        {
            try
            {
                DesktopWinAPI.SetWindowLong(hWnd, (int)DesktopWinAPI.WindowStyle.GWL_EXSTYLE, DesktopWinAPI.GetWindowLong(hWnd, (int)DesktopWinAPI.WindowStyle.GWL_EXSTYLE) | (uint)DesktopWinAPI.ExWindowStyle.WS_EX_LAYERED);
                DesktopWinAPI.SetLayeredWindowAttributes(hWnd, 0, bAlpha, DesktopWinAPI.LWA_COLORKEY | DesktopWinAPI.LWA_ALPHA);
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("窗口样式设置失败", ex.Message, ex.StackTrace);
            }
        }

        public void Setting(IntPtr hWnd, byte bAlpha)
        {
            SetWindowTransparent(hWnd, bAlpha);
        }
    }
}
